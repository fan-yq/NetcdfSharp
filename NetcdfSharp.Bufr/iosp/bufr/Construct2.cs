// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using NetcdfSharp.ucar.ma2;
	using NetcdfSharp.ucar.nc2;
	using _Coordinate = NetcdfSharp.ucar.nc2.constants._Coordinate;
	using AxisType = NetcdfSharp.ucar.nc2.constants.AxisType;
	using BufrCdmIndexProto = NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using CodeFlagTables = NetcdfSharp.ucar.nc2.iosp.bufr.tables.CodeFlagTables;
	using StandardFields = NetcdfSharp.ucar.nc2.ft.point.bufr.StandardFields;

	/// <summary>
	/// BufrIosp2 delegates the construction of the Netcdf objects to Construct2.
	/// 
	/// @author caron
	/// @since 8/8/13
	/// </summary>

	internal class Construct2
	{
		private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Construct2));
		private const bool warnUnits = false;

		private NetcdfSharp.ucar.nc2.NetcdfFile ncfile;

		private Sequence recordStructure;
		private int centerId;
		private Formatter coordinates = new Formatter();

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Construct2(Message proto, BufrConfig bufrConfig, NetcdfSharp.ucar.nc2.NetcdfFile nc) throws NetcdfSharp.java.io.IOException
		internal Construct2(Message proto, BufrConfig bufrConfig, NetcdfSharp.ucar.nc2.NetcdfFile nc)
		{
			this.ncfile = nc;

			// dkeyRoot = dds.getDescriptorRoot();
			// int nbits = dds.getTotalBits();
			// int inputBytes = (nbits % 8 == 0) ? nbits / 8 : nbits / 8 + 1;
			// int outputBytes = dds.getTotalBytes();

			// the category
			// int cat = proto.ids.getCategory();
			// int subcat = proto.ids.getSubCategory();

			// global Attributes
			ncfile.addAttribute(null, new NcAttribute(CDM.HISTORY, "Read using CDM BufrIosp2"));
			if (bufrConfig.FeatureType != null)
			{
				ncfile.addAttribute(null, CF.FEATURE_TYPE, bufrConfig.FeatureType.ToString());
			}
			ncfile.addAttribute(null, "location", nc.Location);

			ncfile.addAttribute(null, "BUFR:categoryName", proto.Lookup.CategoryName);
			ncfile.addAttribute(null, "BUFR:subCategoryName", proto.Lookup.SubCategoryName);
			ncfile.addAttribute(null, "BUFR:centerName", proto.Lookup.CenterName);
			ncfile.addAttribute(null, new NcAttribute("BUFR:category", proto.ids.Category));
			ncfile.addAttribute(null, new NcAttribute("BUFR:subCategory", proto.ids.SubCategory));
			ncfile.addAttribute(null, new NcAttribute("BUFR:localSubCategory", proto.ids.LocalSubCategory));
			ncfile.addAttribute(null, new NcAttribute(BufrIosp2.centerId, proto.ids.CenterId));
			ncfile.addAttribute(null, new NcAttribute("BUFR:subCenter", proto.ids.SubCenterId));
			// ncfile.addAttribute(null, "BUFR:tableName", proto.ids.getMasterTableFilename()));
			ncfile.addAttribute(null, new NcAttribute("BUFR:table", proto.ids.MasterTableId));
			ncfile.addAttribute(null, new NcAttribute("BUFR:tableVersion", proto.ids.MasterTableVersion));
			ncfile.addAttribute(null, new NcAttribute("BUFR:localTableVersion", proto.ids.LocalTableVersion));
			ncfile.addAttribute(null, "Conventions", "BUFR/CDM");
			ncfile.addAttribute(null, new NcAttribute("BUFR:edition", proto.@is.BufrEdition));

			centerId = proto.ids.CenterId;

			string header = proto.Header;
			if (!string.ReferenceEquals(header, null) && header.Length > 0)
			{
				ncfile.addAttribute(null, new NcAttribute("WMO Header", header));
			}

			makeObsRecord(bufrConfig);
			string coordS = coordinates.ToString();
			if (coordS.Length > 0)
			{
				recordStructure.addAttribute(new NcAttribute("coordinates", coordS));
			}

			ncfile.finish();
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Construct2(Message proto, List<BufrConfig> bufrConfigs, NetcdfSharp.ucar.nc2.NetcdfFile nc) throws NetcdfSharp.java.io.IOException
		internal Construct2(Message proto, IList<BufrConfig> bufrConfigs, NetcdfSharp.ucar.nc2.NetcdfFile nc)
		{
			this.ncfile = nc;

			// global Attributes
			ncfile.addAttribute(null, new NcAttribute(CDM.HISTORY, "Read using CDM BufrIosp2"));
			ncfile.addAttribute(null, "location", nc.Location);

			ncfile.addAttribute(null, "BUFR:categoryName", proto.Lookup.CategoryName);
			ncfile.addAttribute(null, "BUFR:subCategoryName", proto.Lookup.SubCategoryName);
			ncfile.addAttribute(null, "BUFR:centerName", proto.Lookup.CenterName);
			ncfile.addAttribute(null, new NcAttribute("BUFR:category", proto.ids.Category));
			ncfile.addAttribute(null, new NcAttribute("BUFR:subCategory", proto.ids.SubCategory));
			ncfile.addAttribute(null, new NcAttribute("BUFR:localSubCategory", proto.ids.LocalSubCategory));
			ncfile.addAttribute(null, new NcAttribute(BufrIosp2.centerId, proto.ids.CenterId));
			ncfile.addAttribute(null, new NcAttribute("BUFR:subCenter", proto.ids.SubCenterId));
			// ncfile.addAttribute(null, "BUFR:tableName", proto.ids.getMasterTableFilename()));
			ncfile.addAttribute(null, new NcAttribute("BUFR:table", proto.ids.MasterTableId));
			ncfile.addAttribute(null, new NcAttribute("BUFR:tableVersion", proto.ids.MasterTableVersion));
			ncfile.addAttribute(null, new NcAttribute("BUFR:localTableVersion", proto.ids.LocalTableVersion));
			ncfile.addAttribute(null, "Conventions", "BUFR/CDM");
			ncfile.addAttribute(null, new NcAttribute("BUFR:edition", proto.@is.BufrEdition));

			centerId = proto.ids.CenterId;

			string header = proto.Header;
			if (!string.ReferenceEquals(header, null) && header.Length > 0)
			{
				ncfile.addAttribute(null, new NcAttribute("WMO Header", header));
			}

			foreach (BufrConfig bufrConfig in bufrConfigs)
			{
				string varName = proto.Lookup.getCategoryName(bufrConfig.Message.ids.Category);
				Sequence rs = new Sequence(ncfile, null, null, varName);
				makeObsRecord(bufrConfig, rs);
				string coordS = coordinates.ToString();
				if (coordS.Length > 0)
				{
					rs.addAttribute(new NcAttribute("coordinates", coordS));
				}
			}

			ncfile.finish();
		}

		internal virtual Sequence ObsStructure
		{
			get
			{
				return recordStructure;
			}
		}

		private void makeObsRecord(BufrConfig bufrConfig)
		{
			recordStructure = new Sequence(ncfile, null, null, BufrIosp2.obsRecordName);
			ncfile.addVariable(null, recordStructure);

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
					addSequence(recordStructure, fld);

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
							addDpiStructure(recordStructure, fld, subFld);

						}
						else if (subDds.replication == 1)
						{ // one member not a replication
							Variable v = addVariable(recordStructure, subFld, dkey.replication);
							v.SPobject = fld; // set the replicating field as SPI object

						}
						else
						{ // one member is a replication (two replications in a row)
							addStructure(recordStructure, fld, dkey.replication);
						}
					}
					else if (subKeys.Count > 1)
					{
						addStructure(recordStructure, fld, dkey.replication);
					}

				}
				else
				{ // replication == 1
					addVariable(recordStructure, fld, dkey.replication);
				}
			}
		}

		private void makeObsRecord(BufrConfig bufrConfig, Sequence rs)
		{
			ncfile.addVariable(null, rs);

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
					addSequence(rs, fld);

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
							Variable v = addVariable(rs, subFld, dkey.replication);
							v.SPobject = fld; // set the replicating field as SPI object

						}
						else
						{ // one member is a replication (two replications in a row)
							addStructure(rs, fld, dkey.replication);
						}
					}
					else if (subKeys.Count > 1)
					{
						addStructure(rs, fld, dkey.replication);
					}

				}
				else
				{ // replication == 1
					addVariable(rs, fld, dkey.replication);
				}
			}
		}

		private void addStructure(Structure parent, BufrConfig.FieldConverter fld, int count)
		{
			DataDescriptor dkey = fld.dds;
			string uname = findUniqueName(parent, fld.Name, "struct");
			dkey.name = uname; // name may need to be changed for uniqueness

			// String structName = dataDesc.name != null ? dataDesc.name : "struct" + structNum++;
			Structure @struct = new Structure(ncfile, null, parent, uname);
			try
			{
				@struct.setDimensionsAnonymous(new int[] { count }); // anon vector
			}
			catch (InvalidRangeException)
			{
				log.error("illegal count= " + count + " for " + fld);
			}

			foreach (BufrConfig.FieldConverter subKey in fld.flds)
			{
				addMember(@struct, subKey);
			}

			parent.addMemberVariable(@struct);
			@struct.SPobject = fld;
		}

		private void addSequence(Structure parent, BufrConfig.FieldConverter fld)
		{

			DataDescriptor dkey = fld.dds;
			string uname = findUniqueName(parent, fld.Name, "seq");
			dkey.name = uname; // name may need to be changed for uniqueness

			// String seqName = ftype == (FeatureType.STATION_PROFILE) ? "profile" : "seq";
			// String seqName = dataDesc.name != null ? dataDesc.name : "seq" + seqNum++;

			Sequence seq = new Sequence(ncfile, null, parent, uname);
			seq.setDimensions(""); // scalar

			foreach (BufrConfig.FieldConverter subKey in fld.flds)
			{
				addMember(seq, subKey);
			}

			parent.addMemberVariable(seq);
			seq.SPobject = fld;

			dkey.refersTo = seq;
		}

		private void addMember(Structure parent, BufrConfig.FieldConverter fld)
		{
			DataDescriptor dkey = fld.dds;

			if (dkey.replication == 0)
			{
				addSequence(parent, fld);
			}

			else if (dkey.replication > 1)
			{
				IList<DataDescriptor> subKeys = dkey.subKeys;
				if (subKeys.Count == 1)
				{
					BufrConfig.FieldConverter subFld = fld.flds[0];
					Variable v = addVariable(parent, subFld, dkey.replication);
					v.SPobject = fld; // set the replicating field as SPI object

				}
				else
				{
					addStructure(parent, fld, dkey.replication);
				}

			}
			else
			{
				addVariable(parent, fld, dkey.replication);
			}
		}

		private void addDpiStructure(Structure parent, BufrConfig.FieldConverter parentFld, BufrConfig.FieldConverter dpiField)
		{
			DataDescriptor dpiKey = dpiField.dds;
			string uname = findUniqueName(parent, dpiField.Name, "struct");
			dpiKey.name = uname; // name may need to be changed for uniqueness

			// String structName = findUnique(parent, dpiField.name);
			Structure @struct = new Structure(ncfile, null, parent, uname);
			int n = parentFld.dds.replication;
			try
			{
				@struct.setDimensionsAnonymous(new int[] { n }); // anon vector
			}
			catch (InvalidRangeException)
			{
				log.error("illegal count= " + 1 + " for " + dpiField);
			}

			Variable v = new Variable(ncfile, null, @struct, "name");
			v.DataType = DataType.STRING; // scalar
			v.setDimensions(""); // scalar
			@struct.addMemberVariable(v);

			v = new Variable(ncfile, null, @struct, "data");
			v.DataType = DataType.FLOAT; // scalar
			v.setDimensions(""); // scalar
			@struct.addMemberVariable(v);

			parent.addMemberVariable(@struct);
			@struct.SPobject = dpiField; // ??

			// add some fake dkeys corresponding to above
			// DataDescriptor nameDD = new DataDescriptor();
		}

		private void addDpiSequence(Structure parent, BufrConfig.FieldConverter fld)
		{
			Structure @struct = new Structure(ncfile, null, parent, "statistics");
			try
			{
				@struct.setDimensionsAnonymous(new int[] { fld.dds.replication }); // scalar
			}
			catch (InvalidRangeException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			Variable v = new Variable(ncfile, null, @struct, "name");
			v.DataType = DataType.STRING; // scalar
			v.setDimensions(""); // scalar
			@struct.addMemberVariable(v);

			v = new Variable(ncfile, null, @struct, "data");
			v.DataType = DataType.FLOAT; // scalar
			v.setDimensions(""); // scalar
			@struct.addMemberVariable(v);

			parent.addMemberVariable(@struct);
		}

		private Variable addVariable(Structure @struct, BufrConfig.FieldConverter fld, int count)
		{
			DataDescriptor dkey = fld.dds;
			string uname = findGloballyUniqueName(fld.Name, "unknown");
			dkey.name = uname; // name may need to be changed for uniqueness

			Variable v = new Variable(ncfile, null, @struct, uname);
			try
			{
				if (count > 1)
				{
					v.setDimensionsAnonymous(new int[] { count }); // anon vector
				}
				else
				{
					v.setDimensions(""); // scalar
				}
			}
			catch (InvalidRangeException)
			{
				log.error("illegal count= " + count + " for " + fld);
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
				try
				{
					v.setDimensionsAnonymous(new int[] { size });
				}
				catch (InvalidRangeException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

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

				Group g = @struct.ParentGroupOrRoot;
				if (g == null)
				{
					log.warn("Struct parent group is null.");
				}
				EnumTypedef enumTypedef = g.findEnumeration(ct.Name);
				if (enumTypedef == null)
				{
					enumTypedef = new EnumTypedef(ct.Name, ct.Map);
					g.addEnumeration(enumTypedef);
				}
				v.EnumTypedef = enumTypedef;

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

			v.SPobject = fld;
			return v;
		}

		private int tempNo = 1;

		private string findUniqueName(Structure @struct, string want, string def)
		{
			if (string.ReferenceEquals(want, null))
			{
				return def + tempNo++;
			}

			string vwant = NetcdfFile.makeValidCdmObjectName(want);
			Variable oldV = @struct.findVariable(vwant);
			if (oldV == null)
			{
				return vwant;
			}

			int seq = 2;
			while (true)
			{
				string wantSeq = vwant + "-" + seq;
				oldV = @struct.findVariable(wantSeq);
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

			string vwant = NetcdfFile.makeValidCdmObjectName(want);
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

		private void annotate(Variable v, BufrConfig.FieldConverter fld)
		{
			if (fld.type == null)
			{
				return;
			}

			switch (fld.type.Value)
			{
				case BufrCdmIndexProto.FldType.lat:
					v.addAttribute(new NcAttribute(CDM.UNITS, CDM.LAT_UNITS));
					v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Lat.ToString()));
					coordinates.format("{0} ", v.ShortName);
					break;

				case BufrCdmIndexProto.FldType.lon:
					v.addAttribute(new NcAttribute(CDM.UNITS, CDM.LON_UNITS));
					v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Lon.ToString()));
					coordinates.format("{0} ", v.ShortName);
					break;

				case BufrCdmIndexProto.FldType.height:
				case BufrCdmIndexProto.FldType.heightOfStation:
				case BufrCdmIndexProto.FldType.heightAboveStation:
					v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Height.ToString()));
					coordinates.format("{0} ", v.ShortName);
					break;

				case BufrCdmIndexProto.FldType.stationId:
					v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.STATION_ID));
					break;

				case BufrCdmIndexProto.FldType.wmoId:
					v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.STATION_WMOID));
					break;
			}

		}

		private void annotateObs(Sequence recordStructure)
		{
			StandardFields.StandardFieldsFromStructure extract =
				new StandardFields.StandardFieldsFromStructure(centerId, recordStructure);

			using (Formatter f = new Formatter())
			{
				string name = extract.getFieldName(BufrCdmIndexProto.FldType.lat);
				if (name != null)
				{
					f.format("{0} ", name);
				}
				name = extract.getFieldName(BufrCdmIndexProto.FldType.lon);
				if (name != null)
				{
					f.format("{0} ", name);
				}
				name = extract.getFieldName(BufrCdmIndexProto.FldType.height);
				if (name != null)
				{
					f.format("{0} ", name);
				}
				name = extract.getFieldName(BufrCdmIndexProto.FldType.heightAboveStation);
				if (name != null)
				{
					f.format("{0} ", name);
				}

				recordStructure.addAttribute(new NcAttribute("coordinates", f.ToString()));
			}
		}
	}

}
