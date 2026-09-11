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

namespace NetcdfSharp.ucar.nc2.ft.point.bufr
{
	using Element = NetcdfSharp.org.jdom2.Element;
	using Catalog = NetcdfSharp.thredds.client.catalog.Catalog;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using ArraySequence = NetcdfSharp.ucar.ma2.ArraySequence;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using StructureData = NetcdfSharp.ucar.ma2.StructureData;
	using StructureDataComposite = NetcdfSharp.ucar.ma2.StructureDataComposite;
	using StructureDataIterator = NetcdfSharp.ucar.ma2.StructureDataIterator;
	using StructureDataW = NetcdfSharp.ucar.ma2.StructureDataW;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;
	using Attribute = NetcdfSharp.ucar.nc2.Attribute;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using VariableSimpleIF = NetcdfSharp.ucar.nc2.VariableSimpleIF;
	using FeatureType = NetcdfSharp.ucar.nc2.constants.FeatureType;
	using NetcdfDataset = NetcdfSharp.ucar.nc2.dataset.NetcdfDataset;
	using SequenceDS = NetcdfSharp.ucar.nc2.dataset.SequenceDS;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;
	using FeatureDataset = NetcdfSharp.ucar.nc2.ft.FeatureDataset;
	using FeatureDatasetFactory = NetcdfSharp.ucar.nc2.ft.FeatureDatasetFactory;
	using PointFeature = NetcdfSharp.ucar.nc2.ft.PointFeature;
	using PointFeatureCollection = NetcdfSharp.ucar.nc2.ft.PointFeatureCollection;
	using PointFeatureIterator = NetcdfSharp.ucar.nc2.ft.PointFeatureIterator;
	using NetcdfSharp.ucar.nc2.ft.point;
	using IOServiceProvider = NetcdfSharp.ucar.nc2.iosp.IOServiceProvider;
	using BufrIosp2 = NetcdfSharp.ucar.nc2.iosp.bufr.BufrIosp2;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CalendarDateUnit = NetcdfSharp.ucar.nc2.time.CalendarDateUnit;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using EarthLocation = NetcdfSharp.ucar.unidata.geoloc.EarthLocation;
	using LatLonRect = NetcdfSharp.ucar.unidata.geoloc.LatLonRect;

	/// <summary>
	/// Use BufrConfig to make BUFR files into PointFeatureDataset
	/// 
	/// @author caron
	/// @since 8/14/13
	/// </summary>
	public class BufrFeatureDatasetFactory : FeatureDatasetFactory
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(BufrFeatureDatasetFactory));
	  private static CalendarDateUnit bufrDateUnits = CalendarDateUnit.of(null, "msecs since 1970-01-01T00:00:00");
	  private static string bufrAltUnits = "m"; // LOOK fake

	  public virtual object isMine(FeatureType wantFeatureType, NetcdfDataset ncd, Formatter errlog)
	  {
		IOServiceProvider iosp = ncd.Iosp;
		return (iosp is BufrIosp2) ? true : null;
	  }

	  public virtual FeatureType[] FeatureTypes
	  {
		  get
		  {
			return new FeatureType[] {FeatureType.ANY_POINT};
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.ucar.nc2.ft.FeatureDataset open(NetcdfSharp.ucar.nc2.constants.FeatureType ftype, NetcdfSharp.ucar.nc2.dataset.NetcdfDataset ncd, Object analysis, NetcdfSharp.ucar.nc2.util.CancelTask task, NetcdfSharp.java.util.Formatter errlog) throws NetcdfSharp.java.io.IOException
	  public virtual FeatureDataset open(FeatureType ftype, NetcdfDataset ncd, object analysis, CancelTask task, Formatter errlog)
	  {

		// must have an index file
		File indexFile = BufrCdmIndex.calcIndexFile(ncd.Location);
		if (indexFile == null)
		{
		  return null;
		}

		BufrCdmIndex index = BufrCdmIndex.readIndex(indexFile.getPath());
		return new BufrStationDataset(ncd, index);
	  }

	  private void show(Element parent, Indent indent, Formatter f)
	  {
		if (parent == null)
		{
		  return;
		}
		foreach (Element child in parent.getChildren("fld", Catalog.ncmlNS))
		{
		  string idx = child.getAttributeValue("idx");
		  string fxy = child.getAttributeValue("fxy");
		  string name = child.getAttributeValue("name");
		  string action = child.getAttributeValue("action");
		  f.format("%sidx='%s' fxy='%s' name='%s' action='%s'%n", indent, idx, fxy, name, action);
		  indent.incr();
		  show(child, indent, f);
		  indent.decr();
		}
	  }

	  private void processSeq(Structure @struct, Element parent)
	  {
		if (parent == null || @struct == null)
		{
		  return;
		}
		IList<Variable> vars = @struct.Variables;
		foreach (Element child in parent.getChildren("fld", Catalog.ncmlNS))
		{
		  string idxS = child.getAttributeValue("idx");
		  int idx = int.Parse(idxS);
		  if (idx < 0 || idx >= vars.Count)
		  {
			log.error("Bad index = {}", child);
			continue;
		  }
		  Variable want = vars[idx];
		  @struct.removeMemberVariable(want);
		}
	  }

	  private class BufrStationDataset : PointDatasetImpl
	  {
		internal Munge munger;
		internal BufrCdmIndex index;
		internal SequenceDS obs;

		internal BufrStationDataset(NetcdfDataset ncfile, BufrCdmIndex index) : base(ncfile, FeatureType.STATION)
		{
		  this.index = index;

		  // create the list of data variables
		  munger = new Munge();
		  obs = (SequenceDS) ncfile.findVariable(BufrIosp2.obsRecordName);
		  this.dataVariables = munger.makeDataVariables(index, obs);

		  BufrStationCollection bufrCollection = new BufrStationCollection(this, ncfile.Location);
		  setPointFeatureCollection(bufrCollection);

		  CalendarDateRange dateRange = CalendarDateRange.of(CalendarDate.of(index.start), CalendarDate.of(index.end));
		  DateRange = dateRange;
		}

		public override FeatureType FeatureType
		{
			get
			{
			  return FeatureType.STATION;
			}
		}

		public override void getDetailInfo(Formatter sf)
		{
		  base.getDetailInfo(sf);
		  index.showIndex(sf);
		}

		private class BufrStationCollection : StationTimeSeriesCollectionImpl
		{
			private readonly BufrFeatureDatasetFactory.BufrStationDataset outerInstance;

		  internal StandardFields.StandardFieldsFromStructure extract;

		  internal BufrStationCollection(BufrFeatureDatasetFactory.BufrStationDataset outerInstance, string name) : base(name, null, null)
		  {
			  this.outerInstance = outerInstance;

			// need the center id to match the standard fields
			Attribute centerAtt = outerInstance.netcdfDataset.findGlobalAttribute(BufrIosp2.centerId);
			int center = (centerAtt == null) ? 0 : Convert.ToInt32(centerAtt.NumericValue);
			this.extract = new StandardFields.StandardFieldsFromStructure(center, outerInstance.obs);

			try
			{
			  this.timeUnit = bufrDateUnits;
			}
			catch (Exception e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace); // cant happen
			}

			this.altUnits = "m"; // LOOK fake units
		  }

		  protected internal override StationHelper createStationHelper()
		  {
			StationHelper stationHelper = new StationHelper();
			foreach (BufrCdmIndexProto.Station s in outerInstance.index.stations)
			{
			  stationHelper.addStation(new BufrStation(this, s));
			}

			return stationHelper;
		  }

		  private class BufrStation : StationTimeSeriesFeatureImpl
		  {
			  private readonly BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection outerInstance;

			internal BufrStation(BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection outerInstance, BufrCdmIndexProto.Station proto) : base(proto.getId(), proto.getDesc(), proto.getWmoId(), proto.getLat(), proto.getLon(), proto.getAlt(), bufrDateUnits, bufrAltUnits, proto.getCount(), StructureData.EMPTY)
			{
				this.outerInstance = outerInstance;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.ucar.nc2.ft.PointFeatureIterator getPointFeatureIterator() throws NetcdfSharp.java.io.IOException
			public override PointFeatureIterator PointFeatureIterator
			{
				get
				{
				  return new BufrStationIterator(this, outerInstance.outerInstance.obs.getStructureIterator(), null);
				}
			}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nonnull @Override public NetcdfSharp.ucar.ma2.StructureData getFeatureData()
			public override StructureData FeatureData
			{
				get
				{
				  return StructureData.EMPTY;
				}
			}

			// iterates over the records for this station
			public class BufrStationIterator : PointIteratorFromStructureData
			{
				private readonly BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection.BufrStation outerInstance;

			  public BufrStationIterator(BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection.BufrStation outerInstance, StructureDataIterator structIter, PointFeatureIterator.Filter filter) : base(structIter, filter)
			  {
				  this.outerInstance = outerInstance;
			  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected NetcdfSharp.ucar.nc2.ft.PointFeature makeFeature(int recnum, NetcdfSharp.ucar.ma2.StructureData sdata) throws NetcdfSharp.java.io.IOException
			  protected internal override PointFeature makeFeature(int recnum, StructureData sdata)
			  {
				outerInstance.outerInstance.extract.extract(sdata);
				string stationId = outerInstance.outerInstance.extract.getStationId();
				if (!stationId.Equals(outerInstance.s.Name))
				{
				  return null;
				}
				CalendarDate date = outerInstance.outerInstance.extract.makeCalendarDate();
				return new BufrStationPoint(outerInstance, outerInstance.s, date.Millis, 0, outerInstance.outerInstance.outerInstance.munger.munge(sdata)); // LOOK obsTime, nomTime
			  }
			}

			public class BufrStationPoint : PointFeatureImpl, StationFeatureHas
			{
				private readonly BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection.BufrStation outerInstance;

			  internal StructureData sdata;

			  public BufrStationPoint(BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection.BufrStation outerInstance, EarthLocation location, double obsTime, double nomTime, StructureData sdata) : base(outerInstance, location, obsTime, nomTime, bufrDateUnits)
			  {
				  this.outerInstance = outerInstance;
				this.sdata = sdata;
			  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nonnull @Override public NetcdfSharp.ucar.ma2.StructureData getDataAll()
			  public override StructureData DataAll
			  {
				  get
				  {
					return sdata;
				  }
			  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nonnull @Override public NetcdfSharp.ucar.ma2.StructureData getFeatureData()
			  public override StructureData FeatureData
			  {
				  get
				  {
					return sdata;
				  }
			  }

			  public virtual StationFeature StationFeature
			  {
				  get
				  {
					return outerInstance;
				  }
			  }
			}
		  }

		  // flatten into a PointFeatureCollection
		  // if empty, may return null
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.ucar.nc2.ft.PointFeatureCollection flatten(NetcdfSharp.ucar.unidata.geoloc.LatLonRect boundingBox, NetcdfSharp.ucar.nc2.time.CalendarDateRange dateRange) throws NetcdfSharp.java.io.IOException
		  public override PointFeatureCollection flatten(LatLonRect boundingBox, CalendarDateRange dateRange)
		  {
			return new BufrPointFeatureCollection(this, boundingBox, dateRange);
		  }

		  private class BufrPointFeatureCollection : PointCollectionImpl
		  {
			  private readonly BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection outerInstance;

			internal StationHelper stationsWanted;
			internal PointFeatureIterator.Filter filter;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: BufrPointFeatureCollection(NetcdfSharp.ucar.unidata.geoloc.LatLonRect boundingBox, NetcdfSharp.ucar.nc2.time.CalendarDateRange dateRange) throws NetcdfSharp.java.io.IOException
			internal BufrPointFeatureCollection(BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection outerInstance, LatLonRect boundingBox, CalendarDateRange dateRange) : base("BufrPointFeatureCollection", bufrDateUnits, bufrAltUnits)
			{
				this.outerInstance = outerInstance;
			  outerInstance.outerInstance.BoundingBox = boundingBox;
			  if (dateRange != null)
			  {
				Info;
				info.CalendarDateRange = dateRange;
			  }
			  outerInstance.createStationHelper();
			  stationsWanted = outerInstance.StationHelper.subset(boundingBox);
			  if (dateRange != null)
			  {
				filter = new PointIteratorFiltered.SpaceAndTimeFilter(null, dateRange);
			  }
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.ucar.nc2.ft.PointFeatureIterator getPointFeatureIterator() throws NetcdfSharp.java.io.IOException
			public override PointFeatureIterator PointFeatureIterator
			{
				get
				{
				  return new BufrRecordIterator(this, outerInstance.outerInstance.obs.getStructureIterator(), filter);
				}
			}

			// iterates once over all the records
			public class BufrRecordIterator : PointIteratorFromStructureData
			{
				private readonly BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection.BufrPointFeatureCollection outerInstance;

			  internal int countHere;

			  public BufrRecordIterator(BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection.BufrPointFeatureCollection outerInstance, StructureDataIterator structIter, PointFeatureIterator.Filter filter) : base(structIter, filter)
			  {
				  this.outerInstance = outerInstance;
			  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected NetcdfSharp.ucar.nc2.ft.PointFeature makeFeature(int recnum, NetcdfSharp.ucar.ma2.StructureData sdata) throws NetcdfSharp.java.io.IOException
			  protected internal override PointFeature makeFeature(int recnum, StructureData sdata)
			  {
				outerInstance.outerInstance.extract.extract(sdata);
				string stationId = outerInstance.outerInstance.extract.getStationId();
				StationFeature want = outerInstance.stationsWanted.getStation(stationId);
				if (want == null)
				{
				  return null;
				}
				CalendarDate date = outerInstance.outerInstance.extract.makeCalendarDate();
				countHere++;
				return new BufrPoint(outerInstance, want, date.Millis, 0, outerInstance.outerInstance.outerInstance.munger.munge(sdata));
			  }

			  public override void Dispose()
			  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				log.debug(string.Format("BufrRecordIterator passed {0:D} features super claims {1:D}%n", countHere, outerInstance.Info.nfeatures));
				base.Dispose();
			  }

			}

			public class BufrPoint : PointFeatureImpl, StationPointFeature
			{
				private readonly BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection.BufrPointFeatureCollection outerInstance;

			  internal StructureData sdata;

			  public BufrPoint(BufrFeatureDatasetFactory.BufrStationDataset.BufrStationCollection.BufrPointFeatureCollection outerInstance, StationFeature want, double obsTime, double nomTime, StructureData sdata) : base(outerInstance, want, obsTime, nomTime, bufrDateUnits)
			  {
				  this.outerInstance = outerInstance;
				this.sdata = sdata;
			  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nonnull @Override public NetcdfSharp.ucar.ma2.StructureData getDataAll()
			  public override StructureData DataAll
			  {
				  get
				  {
					return sdata;
				  }
			  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nonnull @Override public NetcdfSharp.ucar.ma2.StructureData getFeatureData()
			  public override StructureData FeatureData
			  {
				  get
				  {
					return sdata;
				  }
			  }

			  public virtual StationFeature Station
			  {
				  get
				  {
					return (StationFeature) location;
				  }
			  }
			}
		  }

		}
	  }

	  private class Action
	  {
		internal BufrCdmIndexProto.FldAction what;

		internal Action(BufrCdmIndexProto.FldAction what)
		{
		  this.what = what;
		}
	  }

	  private class Munge
	  {
		internal string sdataName;
		internal bool needed;
		protected internal IDictionary<string, Action> actions = new Dictionary<string, Action>(32);
		protected internal IDictionary<string, StructureData> missingData = new Dictionary<string, StructureData>(32);
		protected internal IDictionary<string, VariableDS> vars = new Dictionary<string, VariableDS>(32);

		internal virtual IList<VariableSimpleIF> makeDataVariables(BufrCdmIndex index, Structure obs)
		{
		  this.sdataName = obs.ShortName + "Munged";

		  IList<Variable> members = obs.Variables;
		  IList<VariableSimpleIF> result = new List<VariableSimpleIF>(members.Count);

		  IList<BufrCdmIndexProto.Field> flds = index.root.getFldsList();
		  int count = 0;
		  foreach (Variable v in members)
		  {
			BufrCdmIndexProto.Field fld = flds[count++];
			if (fld.getAction() != null && fld.getAction() != BufrCdmIndexProto.FldAction.none)
			{
			  needed = true;
			  Action act = new Action(fld.getAction());
			  actions[v.ShortName] = act;

			  if (fld.getAction() == BufrCdmIndexProto.FldAction.remove)
			  {
				continue; // skip

			  }
			  else if (fld.getAction() == BufrCdmIndexProto.FldAction.asMissing)
			  {
				// promote the children
				Structure s = (Structure) v;
				foreach (Variable child in s.Variables)
				{
				  result.Add(child);
				  vars[child.ShortName] = (VariableDS) child; // track ones we may have to create missing values for
				}
				continue;
			  }
			}

			if (v.DataType == DataType.SEQUENCE)
			{
			  continue;
			}
			result.Add(v);
		  }
		  return result;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: NetcdfSharp.ucar.ma2.StructureData munge(NetcdfSharp.ucar.ma2.StructureData org) throws NetcdfSharp.java.io.IOException
		internal virtual StructureData munge(StructureData org)
		{
		  return needed ? new StructureDataMunged2(this, org) : org;
		}

		// LOOK needs to be ported to immutable StructureDataComposite
		private class StructureDataMunged2 : StructureDataComposite
		{
			private readonly BufrFeatureDatasetFactory.Munge outerInstance;


// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: StructureDataMunged2(NetcdfSharp.ucar.ma2.StructureData sdata) throws NetcdfSharp.java.io.IOException
			internal StructureDataMunged2(BufrFeatureDatasetFactory.Munge outerInstance, StructureData sdata)
			{
			  this.outerInstance = outerInstance;
			  add(sdata);
			  foreach (StructureMembers.Member m in sdata.Members)
			  {
				Action act = outerInstance.actions.TryGetValue(m.Name, out Action actVal) ? actVal : null;
				if (act == null)
				{
				  // do nothing

				}
				else if (act.what == BufrCdmIndexProto.FldAction.remove)
				{
				  this.members.hideMember(m);

				}
				else if (act.what == BufrCdmIndexProto.FldAction.asMissing)
				{ // 0 or 1
				  int pos = this.members.hideMember(m);
				  ArraySequence seq = sdata.getArraySequence(m);
				  StructureDataIterator iter = seq.StructureDataIterator;
				  if (iter.hasNext())
				  {
					add(pos, iter.next());
				  }
				  else
				  {
					// missing data
					add(pos, makeMissing(m, seq));
				  }
				}
			  }
			}

			internal StructureData makeMissing(StructureMembers.Member seqm, ArraySequence seq)
			{
			  StructureData result = outerInstance.missingData.TryGetValue(seqm.Name, out StructureData resultVal) ? resultVal : null;
			  if (result != null)
			  {
				return result;
			  }

			  StructureMembers sm = seq.StructureMembers.toBuilder(false).build();
			  StructureDataW resultW = new StructureDataW(sm);
			  foreach (StructureMembers.Member m in sm.Members)
			  {
				VariableDS var = outerInstance.vars[m.Name];
				NcArray missingData = var.getMissingDataArray(m.Shape);
				resultW.setMemberData(m, missingData);
			  }

			  outerInstance.missingData[seqm.Name] = resultW;
			  return resultW;
			}

		} // Munge.StructureDataMunged2

	  } // Munge

	} // BufrFeatureDatasetFactory

}
