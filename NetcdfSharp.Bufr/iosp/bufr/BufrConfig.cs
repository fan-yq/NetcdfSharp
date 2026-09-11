// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using NetcdfSharp.ucar.ma2;
	using BufrCdmIndexProto = NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto;
	using BufrField = NetcdfSharp.ucar.nc2.ft.point.bufr.BufrField;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using Catalog = NetcdfSharp.thredds.client.catalog.Catalog;
	using Element = NetcdfSharp.org.jdom2.Element;
	using FeatureType = NetcdfSharp.ucar.nc2.constants.FeatureType;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using NetcdfFiles = NetcdfSharp.ucar.nc2.NetcdfFiles;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Sequence = NetcdfSharp.ucar.nc2.Sequence;
	using StandardFields = NetcdfSharp.ucar.nc2.ft.point.bufr.StandardFields;
	using StationImpl = NetcdfSharp.ucar.unidata.geoloc.StationImpl;

	/// <summary>
	/// Configuration for converting BUFR files to CDM
	/// DataDescriptor tree becomes FieldConverter tree with annotations.
	/// 
	/// @author caron
	/// @since 8/8/13
	/// </summary>
	public class BufrConfig
	{
		private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(BufrConfig));

		public static BufrConfig scanEntireFile(RandomAccessFile raf)
		{
			return new BufrConfig(raf);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: static BufrConfig openFromMessage(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Message m, NetcdfSharp.org.jdom2.Element iospParam) throws NetcdfSharp.java.io.IOException
		internal static BufrConfig openFromMessage(RandomAccessFile raf, Message m, Element iospParam)
		{
			BufrConfig config = new BufrConfig(raf, m);
			if (iospParam != null)
			{
				config.merge(iospParam);
			}
			return config;
		}

		private string filename;
		private Message message;
		private StandardFields.StandardFieldsFromMessage standardFields;
		private FieldConverter rootConverter;
		private int messHash;
		private FeatureType featureType;
		private IDictionary<string, BufrStation> map;
		private long start = long.MaxValue;
		private long end = long.MinValue;
		private bool debug;

		/*
		 * Open file as a stream of BUFR messages, create config file.
		 *
		 * Examine length of sequences, annotate
		 *
		 * @param bufrFilename open this file
		 * 
		 * @throws NetcdfSharp.java.io.IOException on IO error
		 *
		 * private BufrConfig(String bufrFilename, boolean read) throws IOException {
		 * this.filename = bufrFilename;
		 * try {
		 * scanBufrFile(new RandomAccessFile(bufrFilename, "r"), read);
		 * } catch (Exception e) {
		 * e.printStackTrace();
		 * throw new RuntimeException(e.getMessage());
		 * }
		 * }
		 */

		private BufrConfig(RandomAccessFile raf)
		{
			this.filename = raf.Location;
			try
			{
				scanBufrFile(raf);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new Exception(e.Message);
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private BufrConfig(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Message m) throws NetcdfSharp.java.io.IOException
		private BufrConfig(RandomAccessFile raf, Message m)
		{
			this.filename = raf.Location;
			this.message = m;
			this.messHash = m.GetHashCode();
			this.rootConverter = new FieldConverter(m.ids.CenterId, m.RootDataDescriptor);
			standardFields = StandardFields.extract(m);
		}

		public virtual string Filename
		{
			get
			{
				return filename;
			}
		}

		public virtual Message Message
		{
			get
			{
				return this.message;
			}
		}

		public virtual FieldConverter RootConverter
		{
			get
			{
				return rootConverter;
			}
		}

		public virtual IDictionary<string, BufrStation> StationMap
		{
			get
			{
				return map;
			}
		}

		public virtual int MessHash
		{
			get
			{
				return messHash;
			}
		}

		public virtual FeatureType FeatureType
		{
			get
			{
				return featureType;
			}
		}

		public virtual FieldConverter getStandardField(BufrCdmIndexProto.FldType want)
		{
			foreach (FieldConverter fld in rootConverter.flds)
			{
				if (fld.type == want)
				{
					return fld;
				}
			}
			return null;
		}

		public virtual long Start
		{
			get
			{
				return start;
			}
		}

		public virtual long End
		{
			get
			{
				return end;
			}
		}

		public virtual long Nobs
		{
			get
			{
				return countObs;
			}
		}

		////////////////////////////////////////////////////////////////////////////

		private void merge(Element iospParam)
		{
			Debug.Assert(iospParam.getName().Equals("iospParam"));
			Element bufr2nc = iospParam.getChild("bufr2nc", Catalog.ncmlNS);
			if (bufr2nc == null)
			{
				return;
			}
			foreach (Element child in bufr2nc.getChildren("fld", Catalog.ncmlNS))
			{
				merge(child, rootConverter);
			}
		}

		private void merge(Element jdom, FieldConverter parent)
		{
			if (jdom == null || parent == null)
			{
				return;
			}

			FieldConverter fld = null;

			// find the corresponding field
			string idxName = jdom.getAttributeValue("idx");
			if (!string.ReferenceEquals(idxName, null))
			{
				try
				{
					int idx = int.Parse(idxName);
					fld = parent.getChild(idx);
				}
				catch (System.FormatException)
				{
					log.info("BufrConfig cant find Child member index={} for file = {}", idxName, filename);
				}
			}

			if (fld == null)
			{
				string fxyName = jdom.getAttributeValue("fxy");
				if (!string.ReferenceEquals(fxyName, null))
				{
					fld = parent.findChildByFxyName(fxyName);
					if (fld == null)
					{
						log.info("BufrConfig cant find Child member fxy={} for file = {}", fxyName, filename);
					}
				}
			}

			if (fld == null)
			{
				string name = jdom.getAttributeValue("name");
				if (!string.ReferenceEquals(name, null))
				{
					fld = parent.findChild(name);
					if (fld == null)
					{
						log.info("BufrConfig cant find Child member name={} for file = {}", name, filename);
					}
				}
			}

			if (fld == null)
			{
				log.info("BufrConfig must have idx, name or fxy attribute = {} for file = {}", jdom, filename);
				return;
			}

			string action = jdom.getAttributeValue("action");
			if (!string.ReferenceEquals(action, null) && action.Length > 0)
			{
				fld.setAction(action);
			}

			if (jdom.getChildren("fld") != null)
			{
				foreach (Element child in jdom.getChildren("fld", Catalog.ncmlNS))
				{
					merge(child, fld);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////
		private StandardFields.StandardFieldsFromStructure extract;
		private bool hasStations;
		private bool hasDate;
		private int countObs;

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void scanBufrFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws Exception
		private void scanBufrFile(RandomAccessFile raf)
		{
			NetcdfFile ncd = null;
			countObs = 0;

			try
			{
				MessageScanner scanner = new MessageScanner(raf);
				Message protoMessage = scanner.FirstDataMessage;
				if (protoMessage == null)
				{
					throw new IOException("No message found!");
				}

				messHash = protoMessage.GetHashCode();
				standardFields = StandardFields.extract(protoMessage);
				rootConverter = new FieldConverter(protoMessage.ids.CenterId, protoMessage.RootDataDescriptor);

				if (standardFields.hasStation())
				{
					hasStations = true;
					map = new Dictionary<string, BufrStation>(1000);
				}
				featureType = guessFeatureType(standardFields);
				hasDate = standardFields.hasTime();

				ncd = NetcdfFiles.open(raf.Location); // LOOK opening another raf
				NcAttribute centerAtt = ncd.findGlobalAttribute(BufrIosp2.centerId);
				int center = (centerAtt == null) ? 0 : Convert.ToInt32(centerAtt.NumericValue);

				Sequence seq = (Sequence)ncd.RootGroup.findVariableLocal(BufrIosp2.obsRecordName);
				extract = new StandardFields.StandardFieldsFromStructure(center, seq);

				StructureDataIterator iter = seq.StructureIterator;
				processSeq(iter, rootConverter, true);

				StandardActions = rootConverter;

			}
			finally
			{
				if (ncd != null)
				{
					ncd.Dispose();
				}
			}
		}

		private FeatureType guessFeatureType(StandardFields.StandardFieldsFromMessage standardFields)
		{
			if (standardFields.hasStation())
			{
				return FeatureType.STATION;
			}
			if (standardFields.hasTime())
			{
				return FeatureType.POINT;
			}
			return FeatureType.ANY;
		}

		private FieldConverter StandardActions
		{
			set
			{
				value.Action = value.makeAction() ?? BufrCdmIndexProto.FldAction.defa;
				if (value.flds == null)
				{
					return;
				}
				foreach (FieldConverter child in value.flds)
				{
					StandardActions = child;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////

		private CalendarDate today = CalendarDate.present();

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void processSeq(StructureDataIterator sdataIter, FieldConverter parent, boolean isTop) throws NetcdfSharp.java.io.IOException
		private void processSeq(StructureDataIterator sdataIter, FieldConverter parent, bool isTop)
		{
			try
			{
				while (sdataIter.hasNext())
				{
					StructureData sdata = sdataIter.next();

					if (isTop)
					{
						countObs++;

						if (hasStations)
						{
							processStations(parent, sdata);
						}
						if (hasDate)
						{
							extract.extract(sdata);
							CalendarDate date = extract.makeCalendarDate();
							if (Math.Abs(date.getDifferenceInMsecs(today)) > 1000L * 3600 * 24 * 100)
							{
								extract.makeCalendarDate();
							}
							long msecs = date.Millis;
							if (this.start > msecs)
							{
								this.start = msecs;
							}
							if (this.end < msecs)
							{
								this.end = msecs;
							}
						}
					}

					int count = 0;
					foreach (StructureMembers.Member m in sdata.Members)
					{
						if (m.DataType == DataType.SEQUENCE)
						{
							FieldConverter fld = parent.getChild(count);
							ArraySequence data = (ArraySequence)sdata.getArray(m);
							int n = data.StructureDataCount;
							fld.trackSeqCounts(n);
							processSeq(data.StructureDataIterator, fld, false);
						}
						count++;
					}
				}
			}
			finally
			{
				sdataIter.Dispose();
			}
		}

		private void processStations(FieldConverter parent, StructureData sdata)
		{
			BufrStation station = new BufrStation(this);
			station.read(parent, sdata);

			BufrStation check = map[station.Name];
			if (check == null)
			{
				map[station.Name] = station;
			}
			else
			{
				check.count++;
				if (!station.Equals(check))
				{
					log.warn("bad station doesnt equal " + station + " != " + check);
				}
			}
		}

		public class BufrStation : StationImpl
		{
			private readonly BufrConfig outerInstance;

			public BufrStation(BufrConfig outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public int count = 1;

			internal virtual void read(FieldConverter parent, StructureData sdata)
			{
				outerInstance.extract.extract(sdata);

				Name = outerInstance.extract.StationId;
				Latitude = outerInstance.extract.getFieldValueD(BufrCdmIndexProto.FldType.lat);
				Longitude = outerInstance.extract.getFieldValueD(BufrCdmIndexProto.FldType.lon);
				if (outerInstance.extract.hasField(BufrCdmIndexProto.FldType.stationDesc))
				{
					Description = outerInstance.extract.getFieldValueS(BufrCdmIndexProto.FldType.stationDesc);
				}
				if (outerInstance.extract.hasField(BufrCdmIndexProto.FldType.wmoId))
				{
					WmoId = outerInstance.extract.getFieldValueS(BufrCdmIndexProto.FldType.wmoId);
				}
				if (outerInstance.extract.hasField(BufrCdmIndexProto.FldType.heightOfStation))
				{
					Altitude = outerInstance.extract.getFieldValueD(BufrCdmIndexProto.FldType.heightOfStation);
				}
			}

			/*
			 * void read(FieldConverter parent, StructureData sdata) {
			 * int count = 0;
			 * List<FieldConverter> flds = parent.getChildren(); // asssume these track exactly the members
			 * for (StructureMembers.Member m : sdata.getMembers()) {
			 * FieldConverter fld = flds.get(count++);
			 * if (fld.getType() == null) continue;
			 * 
			 * switch (fld.getType()) {
			 * case stationId:
			 * setName( readString(sdata, m));
			 * break;
			 * case stationDesc:
			 * setDescription(sdata.getScalarString(m));
			 * break;
			 * case wmoId:
			 * setWmoId(readString(sdata, m));
			 * break;
			 * case lat:
			 * setLatitude(sdata.convertScalarDouble(m));
			 * break;
			 * case lon:
			 * setLongitude(sdata.convertScalarDouble(m));
			 * break;
			 * case height:
			 * setAltitude(sdata.convertScalarDouble(m));
			 * break;
			 * case heightOfStation:
			 * setAltitude(sdata.convertScalarDouble(m));
			 * break;
			 * }
			 * }
			 * }
			 * 
			 * String readString(StructureData sdata, StructureMembers.Member m) {
			 * if (m.getDataType().isString())
			 * return sdata.getScalarString(m);
			 * else if (m.getDataType().isIntegral())
			 * return JavaLangCompat.IntToString(sdata.convertScalarInt(m));
			 * else if (m.getDataType().isNumeric())
			 * return JavaLangCompat.DoubleToString(sdata.convertScalarDouble(m));
			 * else return "type "+ m.getDataType();
			 * }
			 */

			public override bool Equals(object o)
			{
				if (this == o)
				{
					return true;
				}
				if (o == null || this.GetType() != o.GetType())
				{
					return false;
				}

				BufrStation that = (BufrStation)o;

				if (that.alt.CompareTo(alt) != 0)
				{
					return false;
				}
				if (that.lat.CompareTo(lat) != 0)
				{
					return false;
				}
				if (that.lon.CompareTo(lon) != 0)
				{
					return false;
				}
				if (desc != that.desc)
				{
					return false;
				}
				if (!name.Equals(that.name))
				{
					return false;
				}
				return wmoId == that.wmoId;

			}


			public override int GetHashCode()
			{
				int result;
				long temp;
				temp = BitConverter.DoubleToInt64Bits(lat);
				result = (int)(temp ^ ((long)((ulong)temp >> 32)));
				temp = BitConverter.DoubleToInt64Bits(lon);
				result = 31 * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
				temp = BitConverter.DoubleToInt64Bits(alt);
				result = 31 * result + (int)(temp ^ ((long)((ulong)temp >> 32)));
				result = 31 * result + name.GetHashCode();
				result = 31 * result + (desc != null ? desc.GetHashCode() : 0);
				result = 31 * result + (wmoId != null ? wmoId.GetHashCode() : 0);
				return result;
			}

		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////

		public class FieldConverter : BufrField
		{
			internal DataDescriptor dds;
			internal IList<FieldConverter> flds;
			internal BufrCdmIndexProto.FldType? type;
			internal BufrCdmIndexProto.FldAction? action;
			internal int min = int.MaxValue;
			internal int max;
			internal bool isSeq;

			internal FieldConverter(int center, DataDescriptor dds)
			{
				this.dds = dds;
				this.type = StandardFields.findField(center, dds.FxyName) ?? BufrCdmIndexProto.FldType.def;

				if (dds.SubKeys != null)
				{
					this.flds = new List<FieldConverter>(dds.SubKeys.Count);
					foreach (DataDescriptor subdds in dds.SubKeys)
					{
						FieldConverter subfld = new FieldConverter(center, subdds);
						flds.Add(subfld);
					}
				}
			}

			public virtual string Name { get { return dds.Name; } }

			public virtual string Desc { get { return dds.Desc; } }

			public virtual string Units { get { return dds.Units; } }

			public virtual short Fxy { get { return dds.Fxy; } }

			public virtual string FxyName { get { return dds.FxyName; } }

			public virtual BufrCdmIndexProto.FldAction Action
			{
				get
				{
					return action.HasValue ? action.Value : BufrCdmIndexProto.FldAction.defa;
				}
				set
				{
					action = value;
				}
			}

			public virtual BufrCdmIndexProto.FldType Type { get { return type.HasValue ? type.Value : BufrCdmIndexProto.FldType.def; } }

			public virtual bool Seq { get { return isSeq; } }

			public virtual int Min { get { return min; } }

			public virtual int Max { get { return max; } }

			public virtual int Scale { get { return dds.Scale; } }

			public virtual int Reference { get { return dds.RefVal; } }

			public virtual int BitWidth { get { return dds.BitWidth; } }

			public virtual IList<BufrField> Children
			{
				get
				{
					if (flds == null)
					{
						return null;
					}
					IList<BufrField> result = new List<BufrField>(flds.Count);
					foreach (FieldConverter cc in flds)
					{
						result.Add(cc);
					}
					return result;
				}
			}

			public virtual void setAction(string action)
			{
				try
				{
					this.action = BufrCdmIndexProto.valueOfFldAction(action);
				}
				catch (Exception e)
				{
					log.warn("Unknown action {}", action);
				}
			}

			public virtual void setAction(BufrCdmIndexProto.FldAction action)
			{
				this.action = action;
			}

			internal FieldConverter findChild(string want)
			{
				foreach (FieldConverter child in flds)
				{
					string name = child.dds.Name;
					if (name != null && name.Equals(want))
					{
						return child;
					}
				}
				return null;
			}

			internal FieldConverter findChildByFxyName(string fxyName)
			{
				foreach (FieldConverter child in flds)
				{
					string name = child.dds.FxyName;
					if (name != null && name.Equals(fxyName))
					{
						return child;
					}
				}
				return null;
			}

			internal FieldConverter getChild(int i)
			{
				return flds[i];
			}

			internal void trackSeqCounts(int n)
			{
				isSeq = true;
				if (n > max)
				{
					max = n;
				}
				if (n < min)
				{
					min = n;
				}
			}

			internal void showRange(NetcdfSharp.java.util.Formatter f)
			{
				if (!isSeq)
				{
					return;
				}
				if (max == min)
				{
					f.format(" isConstant='{0}'", max);
				}
				else if (max < 2)
				{
					f.format(" isBinary='true'");
				}
				else
				{
					f.format(" range='[{0},{1}]'", min, max);
				}
			}

			internal BufrCdmIndexProto.FldAction? makeAction()
			{
				if (!isSeq)
				{
					return null;
				}
				if (max == 0)
				{
					return BufrCdmIndexProto.FldAction.remove;
				}
				if (max < 2)
				{
					return BufrCdmIndexProto.FldAction.asMissing;
				}
				else
				{
					return BufrCdmIndexProto.FldAction.asArray;
				}
			}

			internal void show(NetcdfSharp.java.util.Formatter f, Indent indent, int index)
			{
				bool hasContent = false;
				if (isSeq)
				{
					f.format("{0}<fld idx='{1}' name='{2}'", indent, index, dds.Name);
				}
				else
				{
					f.format("{0}<fld idx='{1}' fxy='{2}' name='{3}' desc='{4}' units='{5}' bits='{6}'", indent, index, dds.FxyName, dds.Name, dds.Desc, dds.Units, dds.BitWidth);
				}

				if (type != null)
				{
					f.format(" type='{0}'", type);
				}
				showRange(f);
				f.format(" action='{0}'", makeAction());

				/*
				 * if (type != null) {
				 * f.format(">%n");
				 * indent.incr();
				 * f.format("%s<type>%s</type>%n", indent, type);
				 * indent.decr();
				 * hasContent = true;
				 * }
				 */

				if (flds != null)
				{
					f.format(">" + Environment.NewLine);
					indent.incr();
					int subidx = 0;
					foreach (FieldConverter cc in flds)
					{
						cc.show(f, indent, subidx++);
					}
					indent.decr();
					hasContent = true;
				}

				if (hasContent)
				{
					f.format("{0}</fld>" + Environment.NewLine, indent);
				}
				else
				{
					f.format(" />" + Environment.NewLine);
				}
			}
		}


		//////////////////////////////////////////////////////////////////////////////////////////////////////


		public virtual void show(NetcdfSharp.java.util.Formatter @out)
		{
			if (standardFields != null)
			{
				@out.format("Standard Fields" + Environment.NewLine + "{0}" + Environment.NewLine + Environment.NewLine, standardFields);
			}

			Indent indent = new Indent(2);
			@out.format("<bufr2nc location='{0}' hash='{1}' featureType='{2}'>" + Environment.NewLine, filename, messHash.ToString("x"), featureType);
			indent.incr();
			int index = 0;
			foreach (FieldConverter fld in rootConverter.flds)
			{
				fld.show(@out, indent, index++);
			}
			indent.decr();
			@out.format("</bufr2nc>" + Environment.NewLine);
		}
	}

}
