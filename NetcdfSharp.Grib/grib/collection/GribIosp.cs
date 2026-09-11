// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

/*
 * Copyright (c) 1998-2020 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using NetcdfSharp.ucar.ma2;
	using NetcdfSharp.ucar.nc2;
	using NetcdfSharp.ucar.nc2.grib;
	using _Coordinate = NetcdfSharp.ucar.nc2.constants._Coordinate;
	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using AxisType = NetcdfSharp.ucar.nc2.constants.AxisType;
	using Calendar = NetcdfSharp.ucar.nc2.time.Calendar;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using Catalog = NetcdfSharp.thredds.client.catalog.Catalog;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using CollectionUpdateType = NetcdfSharp.thredds.inventory.CollectionUpdateType;
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using CoordinateEns = NetcdfSharp.ucar.nc2.grib.coord.CoordinateEns;
	using CoordinateRuntime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime;
	using CoordinateTime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime;
	using CoordinateTime2D = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime2D;
	using CoordinateTimeAbstract = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeAbstract;
	using CoordinateTimeIntv = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeIntv;
	using CoordinateVert = NetcdfSharp.ucar.nc2.grib.coord.CoordinateVert;
	using Element = NetcdfSharp.org.jdom2.Element;
	using EnsCoordValue = NetcdfSharp.ucar.nc2.grib.coord.EnsCoordValue;
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using Grib2Utils = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Utils;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using RotatedPole = NetcdfSharp.ucar.unidata.geoloc.projection.RotatedPole;
	using TimeCoordIntvValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvValue;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using VertCoordValue = NetcdfSharp.ucar.nc2.grib.coord.VertCoordValue;

	/// <summary>
	/// Grib Collection IOSP, version 2. Handles both collections and single GRIB files. Immutable after
	/// open() is called.
	/// 
	/// @author caron
	/// @since 4/6/11
	/// </summary>
	public abstract class GribIosp : AbstractIOServiceProvider
	{
		public static int debugIndexOnlyCount; // count number of data accesses

		// store custom tables in here
		protected internal readonly FeatureCollectionConfig config = new FeatureCollectionConfig();

		public virtual Element ParamTable
		{
			set
			{
				config.gribConfig.paramTable = value;
			}
		}

		public virtual string LookupTablePath
		{
			set
			{
				config.gribConfig.lookupTablePath = value;
			}
		}

		public virtual string ParamTablePath
		{
			set
			{
				config.gribConfig.paramTablePath = value;
			}
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Override @Nullable public Object sendIospMessage(Object special)
		public override object sendIospMessage(object special)
		{
			if (special is string)
			{
				string s = (string)special;
				if (s.StartsWith("gribParameterTableLookup", StringComparison.Ordinal))
				{
					int pos = s.IndexOf("=", StringComparison.Ordinal);
					if (pos > 0)
					{
						config.gribConfig.lookupTablePath = s.Substring(pos + 1).Trim();
					}

				}
				else if (s.StartsWith("gribParameterTable", StringComparison.Ordinal))
				{
					int pos = s.IndexOf("=", StringComparison.Ordinal);
					if (pos > 0)
					{
						config.gribConfig.paramTablePath = s.Substring(pos + 1).Trim();
					}
				}
				return null;
			}

			if (special is Element)
			{ // the root element will be <iospParam>
				Element root = (Element)special;
				config.gribConfig.configFromXml(root, Catalog.ncmlNS);
				return null;
			}

			return base.sendIospMessage(special);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		protected internal readonly bool isGrib1;
		protected internal readonly NetcdfSharp.org.slf4j.Logger logger;

		protected internal GribCollectionImmutable gribCollection;
		protected internal GribCollectionImmutable.GroupGC gHcs;
		protected internal GribCollectionImmutable.GribType gtype; // only used if gHcs was set
		protected internal bool owned; // if Iosp is owned by GribCollection; affects close() LOOK get rid of this
		protected internal NetcdfSharp.ucar.nc2.grib.GribTables gribTable;

		[DoesNotReturn]
		private static void throwPartitionNotSupported()
		{
			throw new NotSupportedException("Partitioned GRIB collections are not supported (out of scope)");
		}

		public GribIosp(bool isGrib1, NetcdfSharp.org.slf4j.Logger logger)
		{
			this.isGrib1 = isGrib1;
			this.logger = logger;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected abstract NetcdfSharp.ucar.nc2.grib.GribTables createCustomizer() throws NetcdfSharp.java.io.IOException;
		protected internal abstract NetcdfSharp.ucar.nc2.grib.GribTables createCustomizer();

		public abstract string makeVariableName(GribCollectionImmutable.VariableIndex vindex);

		public abstract string makeVariableLongName(GribCollectionImmutable.VariableIndex vindex);

		protected internal abstract string makeVariableUnits(GribCollectionImmutable.VariableIndex vindex);

		protected internal abstract string getVerticalCoordDesc(int vc_code);

		protected internal abstract GribTables.Parameter getParameter(GribCollectionImmutable.VariableIndex vindex);

		public override bool Builder
		{
			get
			{
				return true;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void build(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Group.Builder rootGroup, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		public override void build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask)
		{
			base.open(raf, rootGroup.getNcfile(), cancelTask);

			if (gHcs != null)
			{ // just use the one group that was set in the constructor
				this.gribCollection = gHcs.GribCollection;
#pragma warning disable CS0184 // PartitionCollectionImmutable is a stub; partitioned collections are out of scope
				if (this.gribCollection is PartitionCollectionImmutable)
				{
					throwPartitionNotSupported();
				}
#pragma warning restore CS0184
				gribTable = createCustomizer();
				GribIospBuilder helper = new GribIospBuilder(this, isGrib1, logger, gribCollection, gribTable);

				helper.addGroup(rootGroup, gHcs, gtype, false);

			}
			else if (gribCollection == null)
			{ // may have been set in the constructor

				this.gribCollection = GribCdmIndex.openGribCollectionFromRaf(raf, config, CollectionUpdateType.testIndexOnly, logger);
				if (gribCollection == null)
				{
					throw new System.InvalidOperationException("Not a GRIB data file or index file " + raf.Location);
				}

#pragma warning disable CS0184 // PartitionCollectionImmutable is a stub; partitioned collections are out of scope
				if (this.gribCollection is PartitionCollectionImmutable)
				{
					throwPartitionNotSupported();
				}
#pragma warning restore CS0184
				gribTable = createCustomizer();
				GribIospBuilder helper = new GribIospBuilder(this, isGrib1, logger, gribCollection, gribTable);

				bool useDatasetGroup = gribCollection.Datasets.Count > 1;
				foreach (GribCollectionImmutable.Dataset ds in gribCollection.Datasets)
				{
					Group.Builder topGroup;
					if (useDatasetGroup)
					{
						topGroup = Group.builder().setName(ds.Type.ToString());
						rootGroup.addGroup(topGroup);
					}
					else
					{
						topGroup = rootGroup;
					}

					IEnumerable<GribCollectionImmutable.GroupGC> groups = ds.Groups;
					bool useGroups = ds.GroupsSize > 1;
					foreach (GribCollectionImmutable.GroupGC g in groups)
					{
						helper.addGroup(topGroup, g, ds.Type, useGroups);
					}
				}
			}

			foreach (NcAttribute att in gribCollection.GlobalAttributes)
			{
				rootGroup.addAttribute(att);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////
		// old way (before version 5.3)

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		public override void open(RandomAccessFile raf, NetcdfFile ncfile, CancelTask cancelTask)
		{
			base.open(raf, ncfile, cancelTask);

			if (gHcs != null)
			{ // just use the one group that was set in the constructor
				this.gribCollection = gHcs.GribCollection;
#pragma warning disable CS0184 // PartitionCollectionImmutable is a stub; partitioned collections are out of scope
				if (this.gribCollection is PartitionCollectionImmutable)
				{
					throwPartitionNotSupported();
				}
#pragma warning restore CS0184
				gribTable = createCustomizer();

				addGroup(ncfile, ncfile.RootGroup, gHcs, gtype, false);

			}
			else if (gribCollection == null)
			{ // may have been set in the constructor

				this.gribCollection = GribCdmIndex.openGribCollectionFromRaf(raf, config, CollectionUpdateType.testIndexOnly, logger);
				if (gribCollection == null)
				{
					throw new System.InvalidOperationException("Not a GRIB data file or index file " + raf.Location);
				}

#pragma warning disable CS0184 // PartitionCollectionImmutable is a stub; partitioned collections are out of scope
				if (this.gribCollection is PartitionCollectionImmutable)
				{
					throwPartitionNotSupported();
				}
#pragma warning restore CS0184
				gribTable = createCustomizer();

				bool useDatasetGroup = gribCollection.Datasets.Count > 1;
				foreach (GribCollectionImmutable.Dataset ds in gribCollection.Datasets)
				{
					Group topGroup;
					if (useDatasetGroup)
					{
						topGroup = new Group(ncfile, null, ds.Type.ToString());
						ncfile.addGroup(topGroup);
					}
					else
					{
						topGroup = ncfile.RootGroup;
					}

					IEnumerable<GribCollectionImmutable.GroupGC> groups = ds.Groups;
					bool useGroups = ds.GroupsSize > 1;
					foreach (GribCollectionImmutable.GroupGC g in groups)
					{
						addGroup(ncfile, topGroup, g, ds.Type, useGroups);
					}
				}
			}

			foreach (NcAttribute att in gribCollection.GlobalAttributes)
			{
				ncfile.addAttribute(att);
			}
		}

		private void addGroup(NetcdfFile ncfile, Group parent, GribCollectionImmutable.GroupGC group, GribCollectionImmutable.GribType gctype, bool useGroups)
		{

			Group g;
			if (useGroups)
			{
				g = new Group(ncfile, parent, group.Id);
				g.addAttribute(new NcAttribute(CDM.LONG_NAME, group.Description));
				try
				{
					parent.addGroup(g);
				}
				catch (Exception)
				{
					logger.warn("Duplicate Group - skipping");
					return;
				}
			}
			else
			{
				g = parent;
			}

			makeGroup(ncfile, g, group, gctype);
		}

		private void makeGroup(NetcdfFile ncfile, Group g, GribCollectionImmutable.GroupGC group, GribCollectionImmutable.GribType gctype)
		{
			GdsHorizCoordSys hcs = group.GdsHorizCoordSys;
			string grid_mapping = hcs.Name + "_Projection";

			string horizDims;

			bool isRotatedLatLon = !isGrib1 && hcs.proj is RotatedPole;
			bool isLatLon2D = !isGrib1 && Grib2Utils.isCurvilinearOrthogonal(hcs.template, gribCollection.Center);
			bool isLatLon = isGrib1 ? hcs.LatLon : Grib2Utils.isLatLon(hcs.template, gribCollection.Center);

			if (isRotatedLatLon)
			{
				Variable hcsV = new Variable(ncfile, g, null, grid_mapping, DataType.INT, "");
				g.addVariable(hcsV);
				hcsV.CachedData = NcArray.factory(DataType.INT, new int[0], new int[] { 0 });
				foreach (Parameter p in hcs.proj.ProjectionParameters)
				{
					hcsV.addAttribute(new NcAttribute(p));
				}
				horizDims = "rlat rlon";
				g.addDimension(new Dimension("rlat", hcs.ny));
				g.addDimension(new Dimension("rlon", hcs.nx));
				Variable rlat = new Variable(ncfile, g, null, "rlat", DataType.FLOAT, "rlat");
				g.addVariable(rlat);
				rlat.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.GRID_LATITUDE));
				rlat.addAttribute(new NcAttribute(CDM.UNITS, CDM.RLATLON_UNITS));
				rlat.CachedData = NcArray.makeArray(DataType.FLOAT, hcs.ny, hcs.starty, hcs.dy);
				Variable rlon = new Variable(ncfile, g, null, "rlon", DataType.FLOAT, "rlon");
				g.addVariable(rlon);
				rlon.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.GRID_LONGITUDE));
				rlon.addAttribute(new NcAttribute(CDM.UNITS, CDM.RLATLON_UNITS));
				rlon.CachedData = NcArray.makeArray(DataType.FLOAT, hcs.nx, hcs.startx, hcs.dx);
			}
			else if (isLatLon2D)
			{ // CurvilinearOrthogonal - lat and lon fields must be present in the file
				horizDims = "lat lon";

				// LOOK - assume same number of points for all grids
				g.addDimension(new Dimension("lon", hcs.nx));
				g.addDimension(new Dimension("lat", hcs.ny));

			}
			else if (isLatLon)
			{
				// make horiz coordsys coordinate variable
				Variable hcsV = new Variable(ncfile, g, null, grid_mapping, DataType.INT, "");
				g.addVariable(hcsV);
				hcsV.CachedData = NcArray.factory(DataType.INT, new int[0], new int[] { 0 });
				foreach (Parameter p in hcs.proj.ProjectionParameters)
				{
					hcsV.addAttribute(new NcAttribute(p));
				}

				horizDims = "lat lon";
				g.addDimension(new Dimension("lon", hcs.nx));
				g.addDimension(new Dimension("lat", hcs.ny));

				Variable cv = new Variable(ncfile, g, null, "lat", DataType.FLOAT, "lat");

				g.addVariable(cv);
				cv.addAttribute(new NcAttribute(CDM.UNITS, CDM.LAT_UNITS));
				if (hcs.GaussianLats != null)
				{
					cv.CachedData = hcs.GaussianLats;
					cv.addAttribute(new NcAttribute(CDM.GAUSSIAN, "true"));
				}
				else
				{
					cv.CachedData = NcArray.makeArray(DataType.FLOAT, hcs.ny, hcs.starty, hcs.dy);
				}

				cv = new Variable(ncfile, g, null, "lon", DataType.FLOAT, "lon");

				g.addVariable(cv);
				cv.addAttribute(new NcAttribute(CDM.UNITS, CDM.LON_UNITS));
				cv.CachedData = NcArray.makeArray(DataType.FLOAT, hcs.nx, hcs.startx, hcs.dx);

			}
			else
			{
				// make horiz coordsys coordinate variable
				Variable hcsV = new Variable(ncfile, g, null, grid_mapping, DataType.INT, "");
				g.addVariable(hcsV);
				hcsV.CachedData = NcArray.factory(DataType.INT, new int[0], new int[] { 0 });
				foreach (Parameter p in hcs.proj.ProjectionParameters)
				{
					hcsV.addAttribute(new NcAttribute(p));
				}

				horizDims = "y x";
				g.addDimension(new Dimension("x", hcs.nx));
				g.addDimension(new Dimension("y", hcs.ny));

				Variable cv = new Variable(ncfile, g, null, "x", DataType.FLOAT, "x");

				g.addVariable(cv);
				cv.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.PROJECTION_X_COORDINATE));
				cv.addAttribute(new NcAttribute(CDM.UNITS, "km"));
				cv.CachedData = NcArray.makeArray(DataType.FLOAT, hcs.nx, hcs.startx, hcs.dx);

				cv = new Variable(ncfile, g, null, "y", DataType.FLOAT, "y");

				g.addVariable(cv);
				cv.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.PROJECTION_Y_COORDINATE));
				cv.addAttribute(new NcAttribute(CDM.UNITS, "km"));
				cv.CachedData = NcArray.makeArray(DataType.FLOAT, hcs.ny, hcs.starty, hcs.dy);
			}

			foreach (Coordinate coord in group.coords)
			{
				Coordinate.Type ctype = coord.getType();
				switch (ctype.innerEnumValue)
				{
					case Coordinate.Type.InnerEnum.runtime:
						if (gctype.isTwoD() || coord.NCoords == 1)
						{
							makeRuntimeCoordinate(ncfile, g, (CoordinateRuntime)coord);
						}
						break;
					case Coordinate.Type.InnerEnum.timeIntv:
						makeTimeCoordinate1D(ncfile, g, (CoordinateTimeIntv)coord);
						break;
					case Coordinate.Type.InnerEnum.time:
						makeTimeCoordinate1D(ncfile, g, (CoordinateTime)coord);
						break;
					case Coordinate.Type.InnerEnum.vert:
						makeVerticalCoordinate(ncfile, g, (CoordinateVert)coord);
						break;
					case Coordinate.Type.InnerEnum.ens:
						makeEnsembleCoordinate(ncfile, g, (CoordinateEns)coord);
						break;
					case Coordinate.Type.InnerEnum.time2D:
						if (gctype.UniqueTime)
						{
							makeUniqueTimeCoordinate2D(ncfile, g, (CoordinateTime2D)coord);
						}
						else
						{
							CoordinateTime2D time2D = (CoordinateTime2D)coord;
							if (time2D.Orthogonal)
							{
								string timeDimName = makeTimeOffsetOrthogonal(g, time2D);
								makeTimeCoordinate2D(ncfile, g, time2D, timeDimName);
							}
							else if (time2D.Regular)
							{
								string timeDimName = makeTimeOffsetRegular(g, time2D);
								makeTimeCoordinate2D(ncfile, g, time2D, timeDimName);
							}
							else
							{
								makeTimeCoordinate2D(ncfile, g, time2D, make2dValidTimeDimensionName(time2D.Name));
							}
						}
						break;
				}
			}

			foreach (GribCollectionImmutable.VariableIndex vindex in group.variList)
			{
				using (Formatter dimNames = new Formatter())
				using (Formatter coordinateAtt = new Formatter())
				{

					// do the times first
					Coordinate run = vindex.getCoordinate(Coordinate.Type.runtime);
					Coordinate time = vindex.getCoordinateTime();
					if (time == null)
					{
						throw new System.InvalidOperationException("No time coordinate = " + vindex);
					}

					string timeDimName = time.Name;
					string timeCoordName = time.Name;

					if (time is CoordinateTime2D)
					{
						CoordinateTime2D time2D = (CoordinateTime2D)time;
						if (!gctype.UniqueTime && (time2D.Orthogonal || time2D.Regular))
						{
							timeDimName = makeTimeOffsetName(time.Name);
						}
						else
						{
							timeDimName = make2dValidTimeDimensionName(time.Name);
							timeCoordName = make2dValidTimeCoordName(timeDimName);
						}
					}

					bool isRunScaler = (run != null) && run.Size == 1;

					switch (gctype.innerEnumValue)
					{
						case NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.GribType.InnerEnum.SRC: // GC: Single Runtime Collection [ntimes] (run, 2D) scalar runtime
							Debug.Assert(isRunScaler);
							dimNames.format("%s ", timeDimName);
							coordinateAtt.format("%s %s ", run.Name, timeDimName);
							break;

						case NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.GribType.InnerEnum.MRUTP: // PC: Multiple Runtime Unique Time Partition [ntimes]
						case NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.GribType.InnerEnum.MRUTC: // GC: Multiple Runtime Unique Time Collection [ntimes]
																										// case MRSTC: // GC: Multiple Runtime Single Time Collection [nruns, 1]
																										// case MRSTP: // PC: Multiple Runtime Single Time Partition [nruns, 1] (run, 2D) ignore the run, its
																										// generated from the 2D in
							dimNames.format("%s ", timeDimName);
							coordinateAtt.format("ref%s %s ", timeDimName, timeDimName);
							break;

						case NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.GribType.InnerEnum.MRC: // GC: Multiple Runtime Collection [nruns, ntimes] (run, 2D) use Both
						case NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.GribType.InnerEnum.TwoD: // PC: TwoD time partition [nruns, ntimes]
							Debug.Assert(run != null, "GRIB MRC or TWOD does not have run coordinate");
							if (isRunScaler)
							{
								dimNames.format("%s ", timeDimName);
							}
							else
							{
								dimNames.format("%s %s ", run.Name, timeDimName);
							}
							coordinateAtt.format("%s %s ", run.Name, timeCoordName);
							if (!timeDimName.Equals(timeCoordName))
							{
								coordinateAtt.format("%s ", timeDimName);
							}
							break;

						case NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.GribType.InnerEnum.Best: // PC: Best time partition [ntimes] (time) reftime is generated in makeTimeAuxReference()
						case NetcdfSharp.ucar.nc2.grib.collection.GribCollectionImmutable.GribType.InnerEnum.BestComplete: // PC: Best complete time partition [ntimes]
							dimNames.format("%s ", timeDimName);
							coordinateAtt.format("ref%s %s ", timeDimName, timeDimName);
							break;

						default:
							throw new System.InvalidOperationException("Unknown GribCollection Type = " + gctype);
					}

					// do other (vert, ens) coordinates
					foreach (Coordinate coord in vindex.Coordinates)
					{
						if (coord is CoordinateTimeAbstract || coord is CoordinateRuntime)
						{
							continue;
						}
						string name = coord.Name.ToLower();
						dimNames.format("%s ", name);
						coordinateAtt.format("%s ", name);
					}
					// do horiz coordinates
					dimNames.format("%s", horizDims);
					coordinateAtt.format("%s ", horizDims);

					string vname = makeVariableName(vindex);
					Variable v = new Variable(ncfile, g, null, vname, DataType.FLOAT, dimNames.ToString());
					g.addVariable(v);

					string desc = makeVariableLongName(vindex);
					v.addAttribute(new NcAttribute(CDM.LONG_NAME, desc));
					v.addAttribute(new NcAttribute(CDM.UNITS, makeVariableUnits(vindex)));

					GribTables.Parameter gp = getParameter(vindex);
					if (gp != null)
					{
						if (!string.ReferenceEquals(gp.Description, null))
						{
							v.addAttribute(new NcAttribute(CDM.DESCRIPTION, gp.Description));
						}
						if (!string.ReferenceEquals(gp.Abbrev, null))
						{
							v.addAttribute(new NcAttribute(CDM.ABBREV, gp.Abbrev));
						}
						v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, gp.Missing));
						if (gp.Fill != null)
						{
							v.addAttribute(new NcAttribute(CDM.FILL_VALUE, gp.Fill));
						}
					}
					else
					{
						v.addAttribute(new NcAttribute(CDM.MISSING_VALUE, float.NaN));
					}

					// horiz coord system
					if (isLatLon2D)
					{ // special case of "LatLon Orthogononal"
						string s = searchCoord(Grib2Utils.getLatLon2DcoordType(desc), group.variList);
						if (string.ReferenceEquals(s, null))
						{ // its a 2D lat/lon coordinate
							v.setDimensions(horizDims); // LOOK make this 2D and munge the units
							string units = desc.Contains("Latitude of") ? CDM.LAT_UNITS : CDM.LON_UNITS;
							v.addAttribute(new NcAttribute(CDM.UNITS, units));

						}
						else
						{ // its a variable using the coordinates described by s
							coordinateAtt.format("%s ", s);
						}
					}
					else
					{
						v.addAttribute(new NcAttribute(CF.GRID_MAPPING, grid_mapping));
					}
					v.addAttribute(new NcAttribute(CF.COORDINATES, coordinateAtt.ToString()));

					// statistical interval type
					if (vindex.IntvType >= 0)
					{
						// LOOK find the time coordinate
						GribStatType statType = gribTable.getStatType(vindex.IntvType);
						if (statType != null)
						{
							v.addAttribute(new NcAttribute(Grib.GRIB_STAT_TYPE, statType.ToString()));
							CF.CellMethods cm = GribStatType.getCFCellMethod(statType);
							Coordinate timeCoord = vindex.getCoordinate(Coordinate.Type.timeIntv);
							if (cm != null && timeCoord != null)
							{
								v.addAttribute(new NcAttribute(CF.CELL_METHODS, timeCoord.Name + ": " + cm));
							}
						}
						else
						{
							v.addAttribute(new NcAttribute(Grib.GRIB_STAT_TYPE, vindex.IntvType));
						}
					}

					gribCollection.addVariableAttributes(v, vindex);
					v.setSPobject(vindex);
				}
			}
		}

		private void makeRuntimeCoordinate(NetcdfFile ncfile, Group g, CoordinateRuntime rtc)
		{
			int n = rtc.Size;
			bool isScalar = (n == 1); // this is the case of runtime[1]
			string tcName = rtc.Name;
			string dims = isScalar ? null : rtc.Name; // null means scalar
			if (!isScalar)
			{
				g.addDimension(new Dimension(tcName, n));
			}

			Variable v = new Variable(ncfile, g, null, tcName, DataType.DOUBLE, dims);

			g.addVariable(v);
			v.addAttribute(new NcAttribute(CDM.UNITS, rtc.Unit));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_REFERENCE));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_RUNTIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));

			/*
			 * String vsName = tcName + "_ISO";
			 * Variable vs = g.addVariable(new Variable(ncfile, g, null, vsName, DataType.STRING, dims));
			 * vs.addAttribute(new Attribute(CDM.UNITS, "ISO8601"));
			 * v.addAttribute(new Attribute(CDM.LONG_NAME, "GRIB reference time"));
			 * v.addAttribute(new Attribute(CF.CALENDAR, Calendar.proleptic_gregorian.toString()));
			 * 
			 * // coordinate values
			 * String[] dataS = new String[n];
			 * int count = 0;
			 * for (CalendarDate val : rtc.getRuntimesSorted()) {
			 * dataS[count++] = val.toString();
			 * }
			 * vs.setCachedData(Array.factory(DataType.STRING, isScalar ? new int[0] : new int[]{n}, dataS));
			 */

			// lazy eval
			v.setSPobject(new Time2Dinfo(Time2DinfoType.reftime, null, rtc));
		}

		internal enum Time2DinfoType
		{
			off, offU, intv, intvU, bounds, boundsU, is1Dtime, isUniqueRuntime, reftime, timeAuxRef
		}

		internal class Time2Dinfo
		{
			internal readonly Time2DinfoType which;
			internal readonly CoordinateTime2D time2D;
			internal readonly Coordinate time1D;

			internal Time2Dinfo(Time2DinfoType which, CoordinateTime2D time2D, Coordinate time1D)
			{
				this.which = which;
				this.time2D = time2D;
				this.time1D = time1D;
			}
		}

		// time coordinates are unique
		// time(nruns, ntimes) -> time(ntimes) with dependent reftime(ntime) coordinate
		private void makeUniqueTimeCoordinate2D(NetcdfFile ncfile, Group g, CoordinateTime2D time2D)
		{
			CoordinateRuntime runtime = time2D.RuntimeCoordinate;

			int countU = 0;
			for (int run = 0; run < time2D.Nruns; run++)
			{
				CoordinateTimeAbstract timeCoord = time2D.getTimeCoordinate(run);
				countU += timeCoord.Size;
			}
			int ntimes = countU;
			string tcName = time2D.Name;
			string timeDimName = make2dValidTimeDimensionName(tcName);

			g.addDimension(new Dimension(timeDimName, ntimes));
			Variable v = new Variable(ncfile, g, null, tcName, DataType.DOUBLE, timeDimName);
			g.addVariable(v);
			string units = runtime.Unit; // + " since " + runtime.getFirstDate();
			v.addAttribute(new NcAttribute(CDM.UNITS, units));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_VALID_TIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));

			// the data is not generated until asked for to save space
			if (!time2D.TimeInterval)
			{
				v.setSPobject(new Time2Dinfo(Time2DinfoType.offU, time2D, null));
			}
			else
			{
				v.setSPobject(new Time2Dinfo(Time2DinfoType.intvU, time2D, null));
				// bounds for intervals
				string bounds_name = timeDimName + "_bounds";
				Variable bounds = new Variable(ncfile, g, null, bounds_name, DataType.DOUBLE, timeDimName + " 2");
				g.addVariable(bounds);
				v.addAttribute(new NcAttribute(CF.BOUNDS, bounds_name));
				bounds.addAttribute(new NcAttribute(CDM.UNITS, units));
				bounds.addAttribute(new NcAttribute(CDM.LONG_NAME, "bounds for " + tcName));
				bounds.setSPobject(new Time2Dinfo(Time2DinfoType.boundsU, time2D, null));
			}

			if (runtime.NCoords != 1)
			{
				// for this case we have to generate a separate reftime, because have to use the same dimension
				string refName = "ref" + tcName;
				if (g.findVariableLocal(refName) == null)
				{
					Variable vref = new Variable(ncfile, g, null, refName, DataType.DOUBLE, timeDimName);
					g.addVariable(vref);
					vref.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_REFERENCE));
					vref.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_RUNTIME));
					vref.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));
					vref.addAttribute(new NcAttribute(CDM.UNITS, units));
					vref.setSPobject(new Time2Dinfo(Time2DinfoType.isUniqueRuntime, time2D, null));
				}
			}
		}

		/*
		 * For better compatibility with CF recommendations, the 2D time coordinates should not have the same name as
		 * a dimension (a recommendation, but not a strict requirement). 2D time coordinates are now called "validtime{n}",
		 * where n is empty or a number (starting with 1). In order to maintain shared dimensions with other time variables,
		 * we need to transform that name into a dimension name. It's all pretty simple, but we do it enough times that it
		 * gets its own method. Basically, the variable "validtime{n}" will use dimension "time{n}".
		 * See https://github.com/Unidata/netcdf-java/issues/152
		 */
		private string make2dValidTimeDimensionName(string variableName)
		{
			return variableName.Replace("valid", "");
		}

		private string make2dValidTimeCoordName(string dimName)
		{
			return "valid" + dimName;
		}

		private string makeTimeOffsetName(string timeName)
		{
			return timeName.ToLower() + "Offset";
		}

		/*
		 * non unique time case
		 * 3) time(nruns, ntimes) with reftime(nruns)
		 */
		private void makeTimeCoordinate2D(NetcdfFile ncfile, Group g, CoordinateTime2D time2D, string timeDimName)
		{
			CoordinateRuntime runtime = time2D.RuntimeCoordinate;

			int ntimes = time2D.Ntimes;
			string tcName = time2D.Name;
			string dims = runtime.Name + " " + timeDimName;
			int dimLength = ntimes;
			if (g.findDimension(timeDimName) == null)
			{
				g.addDimension(new Dimension(timeDimName, dimLength));
			}
			Variable v = new Variable(ncfile, g, null, tcName, DataType.DOUBLE, dims);
			g.addVariable(v);
			string units = runtime.Unit; // + " since " + runtime.getFirstDate();
			v.addAttribute(new NcAttribute(CDM.UNITS, units));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_VALID_TIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));
			if (!tcName.Equals(timeDimName, StringComparison.OrdinalIgnoreCase))
			{
				// explicitly set the axis type as Time
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Time.ToString()));
			}

			// the data is not generated until asked for to save space
			if (!time2D.TimeInterval)
			{
				v.setSPobject(new Time2Dinfo(Time2DinfoType.off, time2D, null));
			}
			else
			{
				v.setSPobject(new Time2Dinfo(Time2DinfoType.intv, time2D, null));
				// bounds for intervals
				string bounds_name = tcName + "_bounds";
				Variable bounds = new Variable(ncfile, g, null, bounds_name, DataType.DOUBLE, dims + " 2");
				g.addVariable(bounds);
				v.addAttribute(new NcAttribute(CF.BOUNDS, bounds_name));
				bounds.addAttribute(new NcAttribute(CDM.UNITS, units));
				bounds.addAttribute(new NcAttribute(CDM.LONG_NAME, "bounds for " + tcName));
				bounds.setSPobject(new Time2Dinfo(Time2DinfoType.bounds, time2D, null));
			}

		}

		private NcArray makeLazyCoordinateData(Variable v2, Time2Dinfo info)
		{
			if (info.time2D != null)
			{
				return makeLazyTime2Darray(v2, info);
			}
			else
			{
				return makeLazyTime1Darray(v2, info);
			}
		}

		// only for the 2d times
		private NcArray makeLazyTime1Darray(Variable v2, Time2Dinfo info)
		{
			int length = info.time1D.Size;
			double[] data = new double[length];
			for (int i = 0; i < length; i++)
			{
				data[i] = double.NaN;
			}

			// coordinate values
			switch (info.which)
			{
				case Time2DinfoType.reftime:
					CoordinateRuntime rtc = (CoordinateRuntime)info.time1D;
					int count = 0;
					foreach (double val in rtc.OffsetsInTimeUnits)
					{
						data[count++] = val;
					}
					return NcArray.factory(DataType.DOUBLE, v2.Shape, data);

				case Time2DinfoType.timeAuxRef:
					CoordinateTimeAbstract time = (CoordinateTimeAbstract)info.time1D;
					count = 0;
					IList<double> masterOffsets = gribCollection.MasterRuntime.OffsetsInTimeUnits;
					foreach (int masterIdx in time.Time2runtime)
					{
						data[count++] = masterOffsets[masterIdx - 1];
					}
					return NcArray.factory(DataType.DOUBLE, v2.Shape, data);

				default:
					throw new System.InvalidOperationException("makeLazyTime1Darray must be reftime or timeAuxRef");
			}
		}

		// only for the 2d times
		private NcArray makeLazyTime2Darray(Variable coord, Time2Dinfo info)
		{
			CoordinateTime2D time2D = info.time2D;
			CalendarPeriod timeUnit = time2D.TimeUnit;

			int nruns = time2D.Nruns;
			int ntimes = time2D.Ntimes;

			int length = (int)coord.Size;
			if (info.which == Time2DinfoType.bounds)
			{
				length *= 2;
			}

			double[] data = new double[length];
			for (int i = 0; i < length; i++)
			{
				data[i] = double.NaN;
			}
			int count;

			// coordinate values
			switch (info.which)
			{
				case Time2DinfoType.off:
					for (int runIdx = 0; runIdx < nruns; runIdx++)
					{
						CoordinateTime coordTime = (CoordinateTime)time2D.getTimeCoordinate(runIdx);
						int timeIdx = 0;
						foreach (int val in coordTime.OffsetSorted)
						{
							data[runIdx * ntimes + timeIdx] = timeUnit.Value * val + time2D.getOffset(runIdx);
							timeIdx++;
						}
					}
					break;

				case Time2DinfoType.offU:
					count = 0;
					for (int runIdx = 0; runIdx < nruns; runIdx++)
					{
						CoordinateTime coordTime = (CoordinateTime)time2D.getTimeCoordinate(runIdx);
						foreach (int val in coordTime.OffsetSorted)
						{
							data[count++] = timeUnit.Value * val + time2D.getOffset(runIdx);
						}
					}
					break;

				case Time2DinfoType.intv:
				case Time2DinfoType.intvU:
					// data[nruns*ntimes]
					count = 0; // dataIndex
					for (int runIdx = 0; runIdx < nruns; runIdx++)
					{
						CoordinateTimeIntv timeIntv = (CoordinateTimeIntv)time2D.getTimeCoordinate(runIdx);
						int runOffsetIndex = runIdx * ntimes;
						int timeUnitValue = timeUnit.Value;
						int time2D_Offset = time2D.getOffset(runIdx);
						Debug.Assert(count == runOffsetIndex);
						count = GribTimeCoordIntervalUtils.generateTimeCoordValuesFromTimeCoordIntervals(timeIntv.TimeIntervals, data, null, runOffsetIndex, timeUnitValue, time2D_Offset);
					}
					break;

				case Time2DinfoType.is1Dtime:
					CoordinateRuntime runtime = time2D.RuntimeCoordinate;
					count = 0;
					foreach (double val in runtime.OffsetsInTimeUnits)
					{ // convert to udunits
						data[count++] = val;
					}
					break;

				case Time2DinfoType.isUniqueRuntime: // the aux runtime coordinate
					CoordinateRuntime runtimeU = time2D.RuntimeCoordinate;
					IList<double> runOffsets = runtimeU.OffsetsInTimeUnits;
					count = 0;
					for (int run = 0; run < time2D.Nruns; run++)
					{
						CoordinateTimeAbstract timeCoord = time2D.getTimeCoordinate(run);
						for (int time = 0; time < timeCoord.NCoords; time++)
						{
							data[count++] = runOffsets[run];
						}
					}
					break;

				case Time2DinfoType.bounds:
					for (int runIdx = 0; runIdx < nruns; runIdx++)
					{
						CoordinateTimeIntv timeIntv = (CoordinateTimeIntv)time2D.getTimeCoordinate(runIdx);
						int timeIdx = 0;
						foreach (TimeCoordIntvValue tinv in timeIntv.TimeIntervals)
						{
							data[runIdx * ntimes * 2 + timeIdx] = timeUnit.Value * tinv.Bounds1 + time2D.getOffset(runIdx);
							data[runIdx * ntimes * 2 + timeIdx + 1] = timeUnit.Value * tinv.Bounds2 + time2D.getOffset(runIdx);
							timeIdx += 2;
						}
					}
					break;

				case Time2DinfoType.boundsU:
					count = 0;
					for (int runIdx = 0; runIdx < nruns; runIdx++)
					{
						CoordinateTimeIntv timeIntv = (CoordinateTimeIntv)time2D.getTimeCoordinate(runIdx);
						foreach (TimeCoordIntvValue tinv in timeIntv.TimeIntervals)
						{
							data[count++] = timeUnit.Value * tinv.Bounds1 + time2D.getOffset(runIdx);
							data[count++] = timeUnit.Value * tinv.Bounds2 + time2D.getOffset(runIdx);
						}
					}
					break;

				default:
					throw new System.InvalidOperationException();
			}

			return NcArray.factory(DataType.DOUBLE, coord.Shape, data);
		}


		private void makeTimeCoordinate1D(NetcdfFile ncfile, Group g, CoordinateTime coordTime)
		{ // }, CoordinateRuntime
		  // runtime) {
			int ntimes = coordTime.Size;
			string tcName = coordTime.Name;
			string dims = coordTime.Name;
			g.addDimension(new Dimension(tcName, ntimes));
			Variable v = new Variable(ncfile, g, null, tcName, DataType.DOUBLE, dims);
			g.addVariable(v);
			string units = coordTime.TimeUdUnit;
			v.addAttribute(new NcAttribute(CDM.UNITS, units));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_VALID_TIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));

			double[] data = new double[ntimes];
			int count = 0;

			// coordinate values
			foreach (int val in coordTime.OffsetSorted)
			{
				data[count++] = val;
			}
			v.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { ntimes }, data));

			makeTimeAuxReference(ncfile, g, tcName, units, coordTime);
		}

		private void makeTimeAuxReference(NetcdfFile ncfile, Group g, string timeName, string units, CoordinateTimeAbstract time)
		{
			if (time.Time2runtime == null)
			{
				return;
			}
			string tcName = "ref" + timeName;
			Variable v = new Variable(ncfile, g, null, tcName, DataType.DOUBLE, timeName);
			g.addVariable(v);
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_REFERENCE));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_RUNTIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));
			v.addAttribute(new NcAttribute(CDM.UNITS, units));

			// lazy evaluation
			v.setSPobject(new Time2Dinfo(Time2DinfoType.timeAuxRef, null, time));
		}

		private void makeTimeCoordinate1D(NetcdfFile ncfile, Group g, CoordinateTimeIntv coordTime)
		{ // }, CoordinateRuntime
		  // runtime) {
			int ntimes = coordTime.Size;
			string tcName = coordTime.Name;
			string dims = coordTime.Name;
			g.addDimension(new Dimension(tcName, ntimes));
			Variable v = new Variable(ncfile, g, null, tcName, DataType.DOUBLE, dims);
			g.addVariable(v);
			string units = coordTime.TimeUdUnit;
			v.addAttribute(new NcAttribute(CDM.UNITS, units));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_VALID_TIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));

			double[] data = new double[ntimes];
			int count = 0;

			foreach (TimeCoordIntvValue tinv in coordTime.TimeIntervals)
			{
				data[count++] = tinv.CoordValue;
			}
			v.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { ntimes }, data));

			// bounds
			string bounds_name = tcName + "_bounds";
			Variable bounds = new Variable(ncfile, g, null, bounds_name, DataType.DOUBLE, dims + " 2");
			g.addVariable(bounds);
			v.addAttribute(new NcAttribute(CF.BOUNDS, bounds_name));
			bounds.addAttribute(new NcAttribute(CDM.UNITS, units));
			bounds.addAttribute(new NcAttribute(CDM.LONG_NAME, "bounds for " + tcName));

			data = new double[ntimes * 2];
			count = 0;
			foreach (TimeCoordIntvValue tinv in coordTime.TimeIntervals)
			{
				data[count++] = tinv.Bounds1;
				data[count++] = tinv.Bounds2;
			}
			bounds.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { ntimes, 2 }, data));

			makeTimeAuxReference(ncfile, g, tcName, units, coordTime);
		}

		private void makeVerticalCoordinate(NetcdfFile ncfile, Group g, CoordinateVert vc)
		{
			int n = vc.Size;
			string vcName = vc.Name.ToLower();

			g.addDimension(new Dimension(vcName, n));
			Variable v = new Variable(ncfile, g, null, vcName, DataType.FLOAT, vcName);
			g.addVariable(v);
			if (vc.Unit != null)
			{
				v.addAttribute(new NcAttribute(CDM.UNITS, vc.Unit));
				string desc = getVerticalCoordDesc(vc.Code);
				if (!string.ReferenceEquals(desc, null))
				{
					v.addAttribute(new NcAttribute(CDM.LONG_NAME, desc));
				}
				v.addAttribute(new NcAttribute(CF.POSITIVE, vc.PositiveUp ? CF.POSITIVE_UP : CF.POSITIVE_DOWN));
			}

			v.addAttribute(new NcAttribute("Grib_level_type", vc.Code));
			VertCoordType vu = vc.VertUnit;
			if (vu != null)
			{
				if (!string.ReferenceEquals(vu.Datum, null))
				{
					v.addAttribute(new NcAttribute("datum", vu.Datum));
				}
			}

			if (vc.Layer)
			{
				float[] data = new float[n];
				int count = 0;
				foreach (VertCoordValue val in vc.LevelSorted)
				{
					data[count++] = (float)(val.Value1 + val.Value2) / 2;
				}
				v.setCachedData(NcArray.factory(DataType.FLOAT, new int[] { n }, data));

				Variable bounds = new Variable(ncfile, g, null, vcName + "_bounds", DataType.FLOAT, vcName + " 2");

				g.addVariable(bounds);
				v.addAttribute(new NcAttribute(CF.BOUNDS, vcName + "_bounds"));
				string vcUnit = vc.Unit;
				if (!string.ReferenceEquals(vcUnit, null))
				{
					bounds.addAttribute(new NcAttribute(CDM.UNITS, vcUnit));
				}
				bounds.addAttribute(new NcAttribute(CDM.LONG_NAME, "bounds for " + vcName));

				data = new float[2 * n];
				count = 0;
				foreach (VertCoordValue level in vc.LevelSorted)
				{
					data[count++] = (float)level.Value1;
					data[count++] = (float)level.Value2;
				}
				bounds.setCachedData(NcArray.factory(DataType.FLOAT, new int[] { n, 2 }, data));

			}
			else
			{
				float[] data = new float[n];
				int count = 0;
				foreach (VertCoordValue val in vc.LevelSorted)
				{
					data[count++] = (float)val.Value1;
				}
				v.setCachedData(NcArray.factory(DataType.FLOAT, new int[] { n }, data));
			}
		}

		private void makeEnsembleCoordinate(NetcdfFile ncfile, Group g, CoordinateEns ec)
		{
			int n = ec.Size;
			string ecName = ec.Name.ToLower();
			g.addDimension(new Dimension(ecName, n));

			Variable v = new Variable(ncfile, g, null, ecName, DataType.INT, ecName);
			g.addVariable(v);
			v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Ensemble.ToString()));

			int[] data = new int[n];
			int count = 0;
			foreach (EnsCoordValue ecc in ec.EnsSorted)
			{
				data[count++] = ecc.EnsMember;
			}
			v.setCachedData(NcArray.factory(DataType.INT, new int[] { n }, data));
		}

		internal virtual string searchCoord(Grib2Utils.LatLonCoordType? type, IList<GribCollectionImmutable.VariableIndex> list)
		{
			if (type == null)
			{
				return null;
			}

			GribCollectionImmutable.VariableIndex lat, lon;
			switch (type)
			{
				case Grib2Utils.LatLonCoordType.U:
					lat = findParameter(list, 198);
					lon = findParameter(list, 199);
					return (lat != null && lon != null) ? makeVariableName(lat) + " " + makeVariableName(lon) : null;
				case Grib2Utils.LatLonCoordType.V:
					lat = findParameter(list, 200);
					lon = findParameter(list, 201);
					return (lat != null && lon != null) ? makeVariableName(lat) + " " + makeVariableName(lon) : null;
				case Grib2Utils.LatLonCoordType.P:
					lat = findParameter(list, 202);
					lon = findParameter(list, 203);
					return (lat != null && lon != null) ? makeVariableName(lat) + "  " + makeVariableName(lon) : null;
			}
			return null;
		}

		private GribCollectionImmutable.VariableIndex findParameter(IList<GribCollectionImmutable.VariableIndex> list, int p)
		{
			foreach (GribCollectionImmutable.VariableIndex vindex in list)
			{
				if ((vindex.Discipline == 0) && (vindex.Category == 2) && (vindex.Parameter == p))
				{
					return vindex;
				}
			}
			return null;
		}

		public override void close()
		{
			if (!owned && gribCollection != null) // LOOK klugerino
			{
				gribCollection.close();
			}
			gribCollection = null;
			base.close();
		}

		public override string DetailInfo
		{
			get
			{
				using (Formatter f = new Formatter())
				{
					f.format("%s", base.DetailInfo);
					if (gribCollection != null)
					{
						gribCollection.showIndex(f);
					}
					return f.ToString();
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////

		public override NcArray readData(Variable v2, Section section)
		{
			long start = JSystem.currentTimeMillis();

			// see if its time2D - then generate data on the fly
			if (v2.SPobject is Time2Dinfo)
			{
				Time2Dinfo info = (Time2Dinfo)v2.SPobject;
				NcArray data = makeLazyCoordinateData(v2, info);
				Section sectionFilled = Section.fill(section, v2.Shape);
				return data.sectionNoReduce(sectionFilled.Ranges);
			}

			try
			{
				NcArray result;
				GribCollectionImmutable.VariableIndex vindex = (GribCollectionImmutable.VariableIndex)v2.SPobject;
				GribDataReader dataReader = GribDataReader.factory(gribCollection, vindex);
				SectionIterable sectionIter = new SectionIterable(section, v2.Shape);
				result = dataReader.readData(sectionIter);

				long took = JSystem.currentTimeMillis() - start;
				return result;

			}
			catch (IOException ioe)
			{
				logger.error("Failed to readData ", ioe);
				throw;
			}
		}

		// orthogonal runtime, offset; both independent
		private string makeTimeOffsetOrthogonal(Group g, CoordinateTime2D time2D)
		{
			IList<object> offsets = time2D.OffsetsSorted;
			int n = offsets.Count;
			string toName = makeTimeOffsetName(time2D.Name);
			g.addDimension(new Dimension(toName, n));
			Variable v = new Variable(ncfile, g, null, toName, DataType.DOUBLE, toName);
			g.addVariable(v);
			v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.TimeOffset.ToString()));
			v.addAttribute(new NcAttribute(CDM.UNITS, time2D.Unit));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_OFFSET));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, CDM.TIME_OFFSET));
			v.addAttribute(new NcAttribute(CDM.UDUNITS, time2D.TimeUdUnit));
			v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.TimeOffset.ToString()));

			double[] midpoints = new double[n];
			double[] bounds = null;
			if (time2D.TimeInterval)
			{
				bounds = new double[2 * n];
				int count = 0;
				int countb = 0;
				foreach (object offset in offsets)
				{
					TimeCoordIntvValue tinv = (TimeCoordIntvValue)offset;
					midpoints[count++] = (tinv.Bounds1 + tinv.Bounds2) / 2.0;
					bounds[countb++] = tinv.Bounds1;
					bounds[countb++] = tinv.Bounds2;
				}
			}
			else
			{
				int count = 0;
				foreach (object val in offsets)
				{
					int off = (int)val;
					midpoints[count++] = off; // int ??
				}
			}
			v.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { n }, midpoints), false);

			if (time2D.TimeInterval)
			{
				string boundsName = toName + "_bounds";
				Variable varBounds = new Variable(ncfile, g, null, boundsName, DataType.DOUBLE, toName + " 2");
				varBounds.addAttribute(new NcAttribute(CDM.LONG_NAME, "TimeOffset coord bounds"));
				VariableDS coordVarBounds = new VariableDS(g, varBounds, true);
				coordVarBounds.setUnitsString(time2D.TimeUdUnit);
				coordVarBounds.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { n, 2 }, bounds), false);

				g.addVariable(coordVarBounds);

				v.addAttribute(new NcAttribute(CF.BOUNDS, boundsName));
			}

			return toName;
		}

		// regular runtime, offset; offset depends on runtime hour from 0z
		private string makeTimeOffsetRegular(Group g, CoordinateTime2D time2D)
		{
			try
			{
				ImmutableList<int> hourFrom0z = ImmutableList<int>.copyOf(time2D.RegularHourOffsets);
				int nhours = hourFrom0z.Count;
				int noffsets = time2D.Ntimes;
				string toName = makeTimeOffsetName(time2D.Name);
				if (g.findDimension(toName) == null)
				{
					g.addDimension(new Dimension(toName, noffsets));
				}
				string dimNames = nhours + " " + toName;

				Variable v = new Variable(ncfile, g, null, toName, DataType.DOUBLE, dimNames);
				g.addVariable(v);
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.TimeOffset.ToString()));
				v.addAttribute(new NcAttribute(CDM.UNITS, time2D.Unit));
				v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_OFFSET));
				v.addAttribute(new NcAttribute(CDM.LONG_NAME, CDM.TIME_OFFSET));
				v.addAttribute(new NcAttribute(CDM.UDUNITS, time2D.TimeUdUnit));
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.TimeOffset.ToString()));

				// Special Coordinates. TODO is there a CF equivalent?
				v.addAttribute(new NcAttribute(CDM.RUNTIME_COORDINATE, g.FullName + time2D.RuntimeCoordinate.Name));
				NcAttribute.Builder attb = NcAttribute.builder(CDM.TIME_OFFSET_HOUR).setDataType(DataType.INT).setValues(hourFrom0z.Select(x => (object)x).ToList(), false);
				v.addAttribute(attb.build());

				// We use a rectangular array; uneven arrays will have NaNs.
				double[] midpoints = new double[nhours * noffsets];
				System.Array.Fill(midpoints, double.NaN);
				double[] bounds = null;
				if (time2D.TimeInterval)
				{
					bounds = new double[2 * nhours * noffsets];
					System.Array.Fill(bounds, double.NaN);
					int houridx = 0;
					foreach (int hour in time2D.RegularHourOffsets)
					{
						CoordinateTimeIntv timeCoord = (CoordinateTimeIntv)time2D.getRegularTimeCoordinate(hour);
						// uneven number of values for each hour
						int count = houridx * noffsets;
						int countb = 2 * houridx * noffsets;
						foreach (TimeCoordIntvValue tinv in timeCoord.TimeIntervals)
						{
							midpoints[count++] = (tinv.Bounds1 + tinv.Bounds2) / 2.0;
							bounds[countb++] = tinv.Bounds1;
							bounds[countb++] = tinv.Bounds2;
						}
						houridx++;
					}
				}
				else
				{
					System.Array.Fill(midpoints, double.NaN);
					int houridx = 0;
					foreach (int hour in time2D.RegularHourOffsets)
					{
						CoordinateTime timeCoord = (CoordinateTime)time2D.getRegularTimeCoordinate(hour);
						// may be uneven number of values for each hour
						int count = houridx * noffsets;
						foreach (int offset in timeCoord.OffsetSorted)
						{
							midpoints[count++] = offset;
						}
						houridx++;
					}
				}
				v.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { nhours, noffsets }, midpoints), false);

				if (time2D.TimeInterval)
				{
					string boundsName = toName + "_bounds";
					Variable varBounds = new Variable(ncfile, g, null, boundsName, DataType.DOUBLE, toName + " 2");
					varBounds.addAttribute(new NcAttribute(CDM.LONG_NAME, "time offset coord bounds"));
					VariableDS coordVarBounds = new VariableDS(g, varBounds, true);
					coordVarBounds.setUnitsString(time2D.TimeUdUnit);
					coordVarBounds.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { nhours, noffsets, 2 }, bounds), false);

					v.addAttribute(new NcAttribute(CF.BOUNDS, boundsName));
				}
				return toName;
			}
			catch (Exception t)
			{
				logger.error("Error in makeTimeOffsetRegular variable {}", makeTimeOffsetName(time2D.Name), t);
				throw new Exception(t.Message, t);
			}
		}

		///////////////////////////////////////
		// debugging back door
		public abstract object LastRecordRead { get; }
		public abstract void clearLastRecordRead();
		public abstract object GribCustomizer { get; }

	}

}
