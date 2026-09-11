// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using _Coordinate = NetcdfSharp.ucar.nc2.constants._Coordinate;
	using AttributeContainerMutable = NetcdfSharp.ucar.nc2.AttributeContainerMutable;
	using AxisType = NetcdfSharp.ucar.nc2.constants.AxisType;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using CodeFlagTables = NetcdfSharp.ucar.nc2.iosp.bufr.tables.CodeFlagTables;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using EnumTypedef = NetcdfSharp.ucar.nc2.EnumTypedef;
	using FieldConverter = NetcdfSharp.ucar.nc2.iosp.bufr.BufrConfig.FieldConverter;
	using Group = NetcdfSharp.ucar.nc2.Group;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using NetcdfFiles = NetcdfSharp.ucar.nc2.NetcdfFiles;
	using Sequence = NetcdfSharp.ucar.nc2.Sequence;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Construction of the Netcdf objects using builders.
	/// </summary>
	internal class BufrIospBuilder
	{
		private static Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(BufrIospBuilder));
		private const bool warnUnits = false;

		private readonly Group.Builder rootGroup;
		private Sequence.BuilderSeq recordStructure;
		private readonly Formatter coordinates = new Formatter();

		private int tempNo = 1; // fishy

		internal BufrIospBuilder(Message proto, BufrConfig bufrConfig, Group.Builder root, string location)
		{
			this.rootGroup = root;
			Sequence.BuilderSeq bs = Sequence.builderSeq();
			bs.setName1(BufrIosp2.obsRecordName);
			this.recordStructure = bs;
			this.rootGroup.addVariable(recordStructure);

			// global Attributes
			AttributeContainerMutable atts = root.AttributeContainer;
			addCommonGlobalAttributes(atts, proto, location);

			if (bufrConfig.FeatureType != null)
			{
				atts.addAttribute(CF.FEATURE_TYPE, bufrConfig.FeatureType.ToString());
			}

			string header = proto.Header;
			if (!string.ReferenceEquals(header, null) && header.Length > 0)
			{
				atts.addAttribute("WMO Header", header);
			}

			makeObsRecord(bufrConfig);
			string coordS = coordinates.ToString();
			if (coordS.Length > 0)
			{
				recordStructure.addAttribute(new NcAttribute("coordinates", coordS));
			}
		}

		internal BufrIospBuilder(Message proto, IList<BufrConfig> bufrConfigs, Group.Builder root, string location)
		{
			this.rootGroup = root;

			// global Attributes
			AttributeContainerMutable atts = root.AttributeContainer;
			addCommonGlobalAttributes(atts, proto, location);

			foreach (BufrConfig bufrConfig in bufrConfigs)
			{
				string varName = proto.Lookup.getCategoryName(bufrConfig.Message.ids.Category);
				Sequence.BuilderSeq rs = Sequence.builderSeq();
				rs.setName1(varName);
				this.rootGroup.addVariable(rs);
				makeObsRecord(bufrConfig, rs);
				string coordS = coordinates.ToString();
				if (coordS.Length > 0)
				{
					rs.addAttribute(new NcAttribute("coordinates", coordS));
				}
			}
		}

		private void addCommonGlobalAttributes(AttributeContainerMutable atts, Message proto, string location)
		{
			atts.addAttribute(CDM.HISTORY, "Read using CDM BufrIosp2");
			atts.addAttribute("location", location);

			atts.addAttribute("BUFR:categoryName", proto.Lookup.CategoryName);
			atts.addAttribute("BUFR:subCategoryName", proto.Lookup.SubCategoryName);
			atts.addAttribute("BUFR:centerName", proto.Lookup.CenterName);
			atts.addAttribute("BUFR:category", proto.ids.Category);
			atts.addAttribute("BUFR:subCategory", proto.ids.SubCategory);
			atts.addAttribute("BUFR:localSubCategory", proto.ids.LocalSubCategory);
			atts.addAttribute(BufrIosp2.centerId, proto.ids.CenterId);
			atts.addAttribute("BUFR:subCenter", proto.ids.SubCenterId);
			atts.addAttribute("BUFR:table", proto.ids.MasterTableId);
			atts.addAttribute("BUFR:tableVersion", proto.ids.MasterTableVersion);
			atts.addAttribute("BUFR:localTableVersion", proto.ids.LocalTableVersion);
			atts.addAttribute("Conventions", "BUFR/CDM");
			atts.addAttribute("BUFR:edition", proto.@is.BufrEdition);

			string header = proto.Header;
			if (!string.ReferenceEquals(header, null) && header.Length > 0)
			{
				atts.addAttribute("WMO Header", header);
			}
		}

		internal virtual Sequence.BuilderSeq ObsStructure
		{
			get
			{
				return recordStructure;
			}
		}

		private void makeObsRecord(BufrConfig bufrConfig)
		{
			BufrConfig.FieldConverter root = bufrConfig.RootConverter;
			foreach (BufrConfig.FieldConverter fld in root.flds)
			{
				DataDescriptor dkey = fld.dds;
				if (!dkey.OkForVariable)
				{
					continue;
				}

				if (dkey.replication == 0)
				{
					addSequence(rootGroup, recordStructure, fld);

				}
				else if (dkey.replication > 1)
				{

					IList<FieldConverter> subFlds = fld.flds;
					IList<DataDescriptor> subKeys = dkey.subKeys;
					if (subKeys.Count == 1)
					{ // only one member
						DataDescriptor subDds = dkey.subKeys[0];
						BufrConfig.FieldConverter subFld = subFlds[0];
						if (subDds.dpi != null)
						{
							addDpiStructure(recordStructure, fld, subFld);

						}
						else if (subDds.replication == 1)
						{ // one member not a replication
							Variable.Builder v = addVariable(rootGroup, recordStructure, subFld, dkey.replication);
							v.SPobject = fld; // set the replicating field as SPI object

						}
						else
						{ // one member is a replication (two replications in a row)
							addStructure(rootGroup, recordStructure, fld, dkey.replication);
						}
					}
					else if (subKeys.Count > 1)
					{
						addStructure(rootGroup, recordStructure, fld, dkey.replication);
					}

				}
				else
				{ // replication == 1
					addVariable(rootGroup, recordStructure, fld, dkey.replication);
				}
			}
		}

		private void makeObsRecord(BufrConfig bufrConfig, Sequence.BuilderSeq rs)
		{
			BufrConfig.FieldConverter root = bufrConfig.RootConverter;
			foreach (BufrConfig.FieldConverter fld in root.flds)
			{
				DataDescriptor dkey = fld.dds;
				if (!dkey.OkForVariable)
				{
					continue;
				}

				if (dkey.replication == 0)
				{
					addSequence(rootGroup, rs, fld);

				}
				else if (dkey.replication > 1)
				{

					IList<BufrConfig.FieldConverter> subFlds = fld.flds;
					IList<DataDescriptor> subKeys = dkey.subKeys;
					if (subKeys.Count == 1)
					{ // only one member
						DataDescriptor subDds = dkey.subKeys[0];
						BufrConfig.FieldConverter subFld = subFlds[0];
						if (subDds.dpi != null)
						{
							addDpiStructure(rs, fld, subFld);

						}
						else if (subDds.replication == 1)
						{ // one member not a replication
							Variable.Builder v = addVariable(rootGroup, rs, subFld, dkey.replication);
							v.SPobject = fld; // set the replicating field as SPI object

						}
						else
						{ // one member is a replication (two replications in a row)
							addStructure(rootGroup, rs, fld, dkey.replication);
						}
					}
					else if (subKeys.Count > 1)
					{
						addStructure(rootGroup, rs, fld, dkey.replication);
					}

				}
				else
				{ // replication == 1
					addVariable(rootGroup, rs, fld, dkey.replication);
				}
			}
		}

		private void addStructure(Group.Builder group, Structure.BuilderStruct parent, BufrConfig.FieldConverter fld, int count)
		{
			DataDescriptor dkey = fld.dds;
			string uname = findUniqueName(parent, fld.Name, "struct");
			dkey.name = uname; // name may need to be changed for uniqueness

			Structure.BuilderStruct @struct = Structure.builder1().setName1(uname);
			@struct.DimensionsAnonymous = new int[] { count }; // anon vector
			foreach (BufrConfig.FieldConverter subKey in fld.flds)
			{
				addMember(group, @struct, subKey);
			}

			parent.addMemberVariable(@struct);
			@struct.SPobject = fld;
		}

		private void addSequence(Group.Builder group, Structure.BuilderStruct parent, BufrConfig.FieldConverter fld)
		{
			DataDescriptor dkey = fld.dds;
			string uname = findUniqueName(parent, fld.Name, "seq");
			dkey.name = uname; // name may need to be changed for uniqueness

			Sequence.BuilderSeq seq = Sequence.builderSeq();
			seq.setName1(uname);
			foreach (BufrConfig.FieldConverter subKey in fld.flds)
			{
				addMember(group, seq, subKey);
			}

			parent.addMemberVariable(seq);
			seq.setSPobject(fld);
		}

		private void addMember(Group.Builder group, Structure.BuilderStruct parent, BufrConfig.FieldConverter fld)
		{
			DataDescriptor dkey = fld.dds;

			if (dkey.replication == 0)
			{
				addSequence(group, parent, fld);
			}
			else if (dkey.replication > 1)
			{
				IList<DataDescriptor> subKeys = dkey.subKeys;
				if (subKeys.Count == 1)
				{
					BufrConfig.FieldConverter subFld = fld.flds[0];
					Variable.Builder v = addVariable(group, parent, subFld, dkey.replication);
					v.SPobject = fld; // set the replicating field as SPI object

				}
				else
				{
					addStructure(group, parent, fld, dkey.replication);
				}

			}
			else
			{
				addVariable(group, parent, fld, dkey.replication);
			}
		}

		private void addDpiStructure(Structure.BuilderStruct parent, BufrConfig.FieldConverter parentFld, BufrConfig.FieldConverter dpiField)
		{
			DataDescriptor dpiKey = dpiField.dds;
			string uname = findUniqueName(parent, dpiField.Name, "struct");
			dpiKey.name = uname; // name may need to be changed for uniqueness

			Structure.BuilderStruct @struct = Structure.builder1().setName1(uname);
			parent.addMemberVariable(@struct);
			int n = parentFld.dds.replication;
			@struct.DimensionsAnonymous = new int[] { n }; // anon vector

			Variable.Builder v = Variable.builder().setName("name");
			v.DataType = DataType.STRING; // scalar
			@struct.addMemberVariable(v);

			v = Variable.builder().setName("data");
			v.DataType = DataType.FLOAT; // scalar
			@struct.addMemberVariable(v);

			@struct.SPobject = dpiField; // ??
		}

		private void addDpiSequence(Structure.BuilderStruct parent, BufrConfig.FieldConverter fld)
		{
			Structure.BuilderStruct @struct = Structure.builder1().setName1("statistics");
			@struct.DimensionsAnonymous = new int[] { fld.dds.replication }; // scalar

			Variable.Builder v = Variable.builder().setName("name");
			v.DataType = DataType.STRING; // scalar
			@struct.addMemberVariable(v);

			v = Variable.builder().setName("data");
			v.DataType = DataType.FLOAT; // scalar
			@struct.addMemberVariable(v);

			parent.addMemberVariable(@struct);
		}

		private Variable.Builder addVariable(Group.Builder group, Structure.BuilderStruct @struct, BufrConfig.FieldConverter fld, int count)
		{
			DataDescriptor dkey = fld.dds;
			string uname = findGloballyUniqueName(fld.Name, "unknown");
			dkey.name = uname; // name may need to be changed for uniqueness

			Variable.Builder v = Variable.builder().setName(uname);
			if (count > 1)
			{
				v.DimensionsAnonymous = new int[] { count }; // anon vector
			}

			if (!string.ReferenceEquals(fld.Desc, null))
			{
				v.addAttribute(new NcAttribute(CDM.LONG_NAME, fld.Desc));
			}

			if (string.ReferenceEquals(fld.Units, null))
			{
				if (warnUnits)
				{
					log.warn("dataDesc.units == null for " + uname);
				}
			}
			else
			{
				string units = fld.Units;
				if (DataDescriptor.isCodeTableUnit(units))
				{
					v.addAttribute(new NcAttribute(CDM.UNITS, "CodeTable " + fld.dds.FxyName));
				}
				else if (DataDescriptor.isFlagTableUnit(units))
				{
					v.addAttribute(new NcAttribute(CDM.UNITS, "FlagTable " + fld.dds.FxyName));
				}
				else if (!DataDescriptor.isInternationalAlphabetUnit(units) && !units.StartsWith("Numeric", StringComparison.Ordinal))
				{
					v.addAttribute(new NcAttribute(CDM.UNITS, units));
				}
			}

			DataDescriptor dataDesc = fld.dds;
			if (dataDesc.type == 1)
			{
				v.DataType = DataType.CHAR;
				int size = dataDesc.bitWidth / 8;
				v.DimensionsAnonymous = new int[] { size };

			}
			else if ((dataDesc.type == 2) && CodeFlagTables.hasTable(dataDesc.fxy))
			{ // enum
				int nbits = dataDesc.bitWidth;
				int nbytes = (nbits % 8 == 0) ? nbits / 8 : nbits / 8 + 1;

				CodeFlagTables ct = CodeFlagTables.getTable(dataDesc.fxy);
				if (nbytes == 1)
				{
					v.DataType = DataType.ENUM1;
				}
				else if (nbytes == 2)
				{
					v.DataType = DataType.ENUM2;
				}
				else if (nbytes == 4)
				{
					v.DataType = DataType.ENUM4;
				}

				// v.removeAttribute(CDM.UNITS);
				v.addAttribute(new NcAttribute("BUFR:CodeTable", ct.Name + " (" + dataDesc.FxyName + ")"));

				EnumTypedef type = group.findOrAddEnumTypedef(ct.Name, ct.Map);
				v.EnumTypeName = type.ShortName;

			}
			else
			{
				int nbits = dataDesc.bitWidth;
				// use of unsigned seems fishy, since only time it uses high bit is for missing
				// not necessarily true, just when they "add one bit" to deal with missing case
				if (nbits < 9)
				{
					v.DataType = DataType.BYTE;
					if (nbits == 8)
					{
						v.addAttribute(new NcAttribute(CDM.UNSIGNED, "true"));
						v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, (short)BufrNumbers.missingValue(nbits)));
					}
					else
					{
						v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, (byte)BufrNumbers.missingValue(nbits)));
					}

				}
				else if (nbits < 17)
				{
					v.DataType = DataType.SHORT;
					if (nbits == 16)
					{
						v.addAttribute(new NcAttribute(CDM.UNSIGNED, "true"));
						v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, (int)BufrNumbers.missingValue(nbits)));
					}
					else
					{
						v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, (short)BufrNumbers.missingValue(nbits)));
					}

				}
				else if (nbits < 33)
				{
					v.DataType = DataType.INT;
					if (nbits == 32)
					{
						v.addAttribute(new NcAttribute(CDM.UNSIGNED, "true"));
						v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, (int)BufrNumbers.missingValue(nbits)));
					}
					else
					{
						v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, (int)BufrNumbers.missingValue(nbits)));
					}

				}
				else
				{
					v.DataType = DataType.LONG;
					v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, BufrNumbers.missingValue(nbits)));
				}

				// value = scale_factor * packed + add_offset
				// bpacked = (value * 10^scale - refVal)
				// (bpacked + refVal) / 10^scale = value
				// value = bpacked * 10^-scale + refVal * 10^-scale
				// scale_factor = 10^-scale
				// add_ofset = refVal * 10^-scale
				int scale10 = dataDesc.scale;
				double scale = (scale10 == 0) ? 1.0 : Math.Pow(10.0, -scale10);
				if (scale10 != 0)
				{
					v.addAttribute(new NcAttribute(CDM.SCALE_FACTOR, (float)scale));
				}
				if (dataDesc.refVal != 0)
				{
					v.addAttribute(new NcAttribute(CDM.ADD_OFFSET, (float)scale * dataDesc.refVal));
				}

			}

			annotate(v, fld);
			v.addAttribute(new NcAttribute(BufrIosp2.fxyAttName, dataDesc.FxyName));
			v.addAttribute(new NcAttribute("BUFR:bitWidth", dataDesc.bitWidth));
			@struct.addMemberVariable(v);

			v.setSPobject(fld);
			return v;
		}

		private string findUniqueName(Structure.BuilderStruct @struct, string want, string def)
		{
			if (string.ReferenceEquals(want, null))
			{
				return def + tempNo++;
			}

			string vwant = NetcdfFiles.makeValidCdmObjectName(want);
			Variable.Builder oldV = @struct.findMemberVariable(vwant);
			if (oldV == null)
			{
				return vwant;
			}

			int seq = 2;
			while (true)
			{
				string wantSeq = vwant + "-" + seq;
				oldV = @struct.findMemberVariable(wantSeq);
				if (oldV == null)
				{
					return wantSeq;
				}
				seq++;
			}
		}

		// force globally unique variable names, even when they are in different Structures.
		// this allows us to promote structure members without worrying about name collisions
		private IDictionary<string, int> names = new Dictionary<string, int>(100);

		private string findGloballyUniqueName(string want, string def)
		{
			if (string.ReferenceEquals(want, null))
			{
				return def + tempNo++;
			}

			string vwant = NetcdfFiles.makeValidCdmObjectName(want);
			int have;
			if (!names.TryGetValue(vwant, out have))
			{
				names[vwant] = 1;
				return vwant;
			}
			else
			{
				have = have + 1;
				string wantSeq = vwant + "-" + have;
				names[vwant] = have;
				return wantSeq;
			}
		}

		private void annotate(Variable.Builder v, BufrConfig.FieldConverter fld)
		{
			if (fld.type == null)
			{
				return;
			}

			switch (fld.type.Value)
			{
				case NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto.FldType.lat:
					v.addAttribute(new NcAttribute(CDM.UNITS, CDM.LAT_UNITS));
					v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Lat.ToString()));
					coordinates.format("{0} ", v.shortName);
					break;

				case NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto.FldType.lon:
					v.addAttribute(new NcAttribute(CDM.UNITS, CDM.LON_UNITS));
					v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Lon.ToString()));
					coordinates.format("{0} ", v.shortName);
					break;

				case NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto.FldType.height:
				case NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto.FldType.heightOfStation:
				case NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto.FldType.heightAboveStation:
					v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Height.ToString()));
					coordinates.format("{0} ", v.shortName);
					break;

				case NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto.FldType.stationId:
					v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.STATION_ID));
					break;

				case NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto.FldType.wmoId:
					v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.STATION_WMOID));
					break;
			}
		}
	}
}
