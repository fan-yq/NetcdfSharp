// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

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
	using AxisType = NetcdfSharp.ucar.nc2.constants.AxisType;
	using Calendar = NetcdfSharp.ucar.nc2.time.Calendar;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using CoordinateEns = NetcdfSharp.ucar.nc2.grib.coord.CoordinateEns;
	using CoordinateRuntime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateRuntime;
	using CoordinateTime = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime;
	using CoordinateTime2D = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime2D;
	using CoordinateTimeAbstract = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeAbstract;
	using CoordinateTimeIntv = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeIntv;
	using CoordinateVert = NetcdfSharp.ucar.nc2.grib.coord.CoordinateVert;
	using EnsCoordValue = NetcdfSharp.ucar.nc2.grib.coord.EnsCoordValue;
	using Grib2Utils = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Utils;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;
	using RotatedPole = NetcdfSharp.ucar.unidata.geoloc.projection.RotatedPole;
	using Time2Dinfo = NetcdfSharp.ucar.nc2.grib.collection.GribIosp.Time2Dinfo;
	using Time2DinfoType = NetcdfSharp.ucar.nc2.grib.collection.GribIosp.Time2DinfoType;
	using TimeCoordIntvValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvValue;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using VertCoordValue = NetcdfSharp.ucar.nc2.grib.coord.VertCoordValue;

	/// <summary>
	/// Grib Collection IOSP helper, for builders.
	/// </summary>
	internal class GribIospBuilder
	{
		private readonly GribIosp iosp;
		private readonly bool isGrib1;
		private readonly Logger logger;
		private readonly GribCollectionImmutable gribCollection;
		private readonly NetcdfSharp.ucar.nc2.grib.GribTables gribTable;

		internal GribIospBuilder(GribIosp iosp, bool isGrib1, Logger logger, GribCollectionImmutable gribCollection, GribTables gribTable)
		{
			this.iosp = iosp;
			this.isGrib1 = isGrib1;
			this.logger = logger;
			this.gribCollection = gribCollection;
			this.gribTable = gribTable;
		}

		internal virtual void addGroup(Group.Builder parent, GribCollectionImmutable.GroupGC group, GribCollectionImmutable.GribType gctype, bool useGroups)
		{
			Group.Builder g;
			if (useGroups)
			{
				if (parent.findGroupLocal(group.Id).isPresent())
				{
					logger.warn("Duplicate Group - skipping");
					return;
				}
				g = Group.builder().setName(group.Id);
				g.addAttribute(new NcAttribute(CDM.LONG_NAME, group.Description));
				parent.addGroup(g);
			}
			else
			{
				g = parent;
			}

			makeGroup(g, group, gctype);
		}

		private void makeGroup(Group.Builder g, GribCollectionImmutable.GroupGC group, GribCollectionImmutable.GribType gctype)
		{
			GdsHorizCoordSys hcs = group.GdsHorizCoordSys;
			string grid_mapping = hcs.Name + "_Projection";

			string horizDims;

			bool isRotatedLatLon = !isGrib1 && hcs.proj is RotatedPole;
			bool isLatLon2D = !isGrib1 && Grib2Utils.isCurvilinearOrthogonal(hcs.template, gribCollection.Center);
			bool isLatLon = isGrib1 ? hcs.LatLon : Grib2Utils.isLatLon(hcs.template, gribCollection.Center);

			if (isRotatedLatLon)
			{
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> hcsV = Variable.builder().setName(grid_mapping).setDataType(DataType.INT);
				Variable.Builder hcsV = Variable.builder().setName(grid_mapping).setDataType(DataType.INT);
				g.addVariable(hcsV);
				hcsV.setCachedData(NcArray.factory(DataType.INT, new int[0], new int[] { 0 }), false);
				foreach (Parameter p in hcs.proj.ProjectionParameters)
				{
					hcsV.addAttribute(new NcAttribute(p));
				}

				horizDims = "rlat rlon";
				g.addDimension(new Dimension("rlat", hcs.ny));
				g.addDimension(new Dimension("rlon", hcs.nx));
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> rlat = Variable.builder().setName("rlat").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("rlat");
				Variable.Builder rlat = Variable.builder().setName("rlat").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("rlat");
				g.addVariable(rlat);
				rlat.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.GRID_LATITUDE));
				rlat.addAttribute(new NcAttribute(CDM.UNITS, CDM.RLATLON_UNITS));
				rlat.setCachedData(NcArray.makeArray(DataType.FLOAT, hcs.ny, hcs.starty, hcs.dy), false);
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> rlon = Variable.builder().setName("rlon").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("rlon");
				Variable.Builder rlon = Variable.builder().setName("rlon").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("rlon");
				g.addVariable(rlon);
				rlon.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.GRID_LONGITUDE));
				rlon.addAttribute(new NcAttribute(CDM.UNITS, CDM.RLATLON_UNITS));
				rlon.setCachedData(NcArray.makeArray(DataType.FLOAT, hcs.nx, hcs.startx, hcs.dx), false);
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
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> hcsV = Variable.builder().setName(grid_mapping).setDataType(DataType.INT);
				Variable.Builder hcsV = Variable.builder().setName(grid_mapping).setDataType(DataType.INT);
				g.addVariable(hcsV);
				hcsV.setCachedData(NcArray.factory(DataType.INT, new int[0], new int[] { 0 }), false);
				foreach (Parameter p in hcs.proj.ProjectionParameters)
				{
					hcsV.addAttribute(new NcAttribute(p));
				}

				horizDims = "lat lon";
				g.addDimension(new Dimension("lon", hcs.nx));
				g.addDimension(new Dimension("lat", hcs.ny));

				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> lat = Variable.builder().setName("lat").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("lat");
				Variable.Builder lat = Variable.builder().setName("lat").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("lat");
				g.addVariable(lat);
				lat.addAttribute(new NcAttribute(CDM.UNITS, CDM.LAT_UNITS));
				if (hcs.GaussianLats != null)
				{
					lat.setCachedData(hcs.GaussianLats, false);
					lat.addAttribute(new NcAttribute(CDM.GAUSSIAN, "true"));
				}
				else
				{
					lat.setCachedData(NcArray.makeArray(DataType.FLOAT, hcs.ny, hcs.starty, hcs.dy), false);
				}

				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> lon = Variable.builder().setName("lon").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("lon");
				Variable.Builder lon = Variable.builder().setName("lon").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("lon");
				g.addVariable(lon);
				lon.addAttribute(new NcAttribute(CDM.UNITS, CDM.LON_UNITS));
				lon.setCachedData(NcArray.makeArray(DataType.FLOAT, hcs.nx, hcs.startx, hcs.dx), false);

			}
			else
			{
				// make horiz coordsys coordinate variable
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> hcsV = Variable.builder().setName(grid_mapping).setDataType(DataType.INT);
				Variable.Builder hcsV = Variable.builder().setName(grid_mapping).setDataType(DataType.INT);
				g.addVariable(hcsV);
				hcsV.setCachedData(NcArray.factory(DataType.INT, new int[0], new int[] { 0 }), false);
				foreach (Parameter p in hcs.proj.ProjectionParameters)
				{
					hcsV.addAttribute(new NcAttribute(p));
				}

				horizDims = "y x";
				g.addDimension(new Dimension("x", hcs.nx));
				g.addDimension(new Dimension("y", hcs.ny));

				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> xcv = Variable.builder().setName("x").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("x");
				Variable.Builder xcv = Variable.builder().setName("x").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("x");
				g.addVariable(xcv);
				xcv.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.PROJECTION_X_COORDINATE));
				xcv.addAttribute(new NcAttribute(CDM.UNITS, "km"));
				xcv.setCachedData(NcArray.makeArray(DataType.FLOAT, hcs.nx, hcs.startx, hcs.dx), false);

				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: Variable.Builder<?> ycv = Variable.builder().setName("y").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("y");
				Variable.Builder ycv = Variable.builder().setName("y").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName("y");
				g.addVariable(ycv);
				ycv.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.PROJECTION_Y_COORDINATE));
				ycv.addAttribute(new NcAttribute(CDM.UNITS, "km"));
				ycv.setCachedData(NcArray.makeArray(DataType.FLOAT, hcs.ny, hcs.starty, hcs.dy), false);
			}

			foreach (Coordinate coord in group.coords)
			{
				Coordinate.Type ctype = coord.getType();
				switch (ctype.innerEnumValue)
				{
					case Coordinate.Type.InnerEnum.runtime:
						if (gctype.isTwoD() || coord.NCoords == 1)
						{
							makeRuntimeCoordinate(g, (CoordinateRuntime)coord);
						}
						break;
					case Coordinate.Type.InnerEnum.timeIntv:
						makeTimeCoordinate1D(g, (CoordinateTimeIntv)coord);
						break;
					case Coordinate.Type.InnerEnum.time:
						makeTimeCoordinate1D(g, (CoordinateTime)coord);
						break;
					case Coordinate.Type.InnerEnum.vert:
						makeVerticalCoordinate(g, (CoordinateVert)coord);
						break;
					case Coordinate.Type.InnerEnum.ens:
						makeEnsembleCoordinate(g, (CoordinateEns)coord);
						break;
					case Coordinate.Type.InnerEnum.time2D:
						if (gctype.UniqueTime)
						{
							makeUniqueTimeCoordinate2D(g, (CoordinateTime2D)coord);
						}
						else
						{
							CoordinateTime2D time2D = (CoordinateTime2D)coord;
							if (time2D.Orthogonal)
							{
								string timeDimName = makeTimeOffsetOrthogonal(g, time2D);
								makeTimeCoordinate2D(g, time2D, timeDimName);
							}
							else if (time2D.Regular)
							{
								string timeDimName = makeTimeOffsetRegular(g, time2D);
								makeTimeCoordinate2D(g, time2D, timeDimName);
							}
							else
							{
								makeTimeCoordinate2D(g, time2D, make2dValidTimeDimensionName(time2D.Name));
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
							timeCoordName = make2dValidTimeCoordName(timeDimName); // TODO backport this to version 5
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
							throw new System.InvalidOperationException("Uknown GribCollection TYpe = " + gctype);
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

					string vname = iosp.makeVariableName(vindex);
					// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
					// ORIGINAL LINE: Variable.Builder<?> v = Variable.builder().setName(vname).setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName(dimNames.toString());
					Variable.Builder v = Variable.builder().setName(vname).setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName(dimNames.ToString());
					g.addVariable(v);

					string desc = iosp.makeVariableLongName(vindex);
					v.addAttribute(new NcAttribute(CDM.LONG_NAME, desc));
					v.addAttribute(new NcAttribute(CDM.UNITS, iosp.makeVariableUnits(vindex)));

					GribTables.Parameter gp = iosp.getParameter(vindex);
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
						string s = iosp.searchCoord(Grib2Utils.getLatLon2DcoordType(desc), group.variList);
						if (string.ReferenceEquals(s, null))
						{ // its a 2D lat/lon coordinate
							v.setDimensionsByName(horizDims); // LOOK make this 2D and munge the units
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

					gribCollection.addVariableAttributes(v.getAttributeContainer(), vindex);
					v.setSPobject(vindex);
				}
			}
		}

		private void makeRuntimeCoordinate(Group.Builder g, CoordinateRuntime rtc)
		{
			int n = rtc.Size;
			bool isScalar = (n == 1); // this is the case of runtime[1]
			string tcName = rtc.Name;
			string dims = isScalar ? null : rtc.Name; // null means scalar
			if (!isScalar)
			{
				g.addDimension(new Dimension(tcName, n));
			}

			Variable.Builder v = Variable.builder().setName(tcName).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(dims);
			g.addVariable(v);
			v.addAttribute(new NcAttribute(CDM.UNITS, rtc.Unit));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_REFERENCE));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_RUNTIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));

			// lazy eval
			v.setSPobject(new Time2Dinfo(Time2DinfoType.reftime, null, rtc));
		}

		// time coordinates are unique
		// time(nruns, ntimes) -> time(ntimes) with dependent reftime(ntime) coordinate
		private void makeUniqueTimeCoordinate2D(Group.Builder g, CoordinateTime2D time2D)
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
			Variable.Builder v = Variable.builder().setName(tcName).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(timeDimName);
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
				Variable.Builder bounds = Variable.builder().setName(bounds_name).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(timeDimName + " 2");
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
				if (!g.findVariableLocal(refName).isPresent())
				{
					Variable.Builder vref = Variable.builder().setName(refName).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(timeDimName);
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
		private void makeTimeCoordinate2D(Group.Builder g, CoordinateTime2D time2D, string timeDimName)
		{
			CoordinateRuntime runtime = time2D.RuntimeCoordinate;

			int ntimes = time2D.Ntimes;
			string tcName = time2D.Name;
			string dims = runtime.Name + " " + timeDimName;
			int dimLength = ntimes;

			g.addDimensionIfNotExists(new Dimension(timeDimName, dimLength));
			Variable.Builder v = Variable.builder().setName(tcName).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(dims);
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
				Variable.Builder bounds = Variable.builder().setName(bounds_name).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(dims + " 2");
				g.addVariable(bounds);
				v.addAttribute(new NcAttribute(CF.BOUNDS, bounds_name));
				bounds.addAttribute(new NcAttribute(CDM.UNITS, units));
				bounds.addAttribute(new NcAttribute(CDM.LONG_NAME, "bounds for " + tcName));
				bounds.setSPobject(new Time2Dinfo(Time2DinfoType.bounds, time2D, null));
			}

		}

		private void makeTimeCoordinate1D(Group.Builder g, CoordinateTime coordTime)
		{ // }, CoordinateRuntime
		  // runtime) {
			int ntimes = coordTime.Size;
			string tcName = coordTime.Name;
			string dims = coordTime.Name;
			g.addDimension(new Dimension(tcName, ntimes));
			Variable.Builder v = Variable.builder().setName(tcName).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(dims);
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
			v.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { ntimes }, data), false);

			makeTimeAuxReference(g, tcName, units, coordTime);
		}

		private void makeTimeAuxReference(Group.Builder g, string timeName, string units, CoordinateTimeAbstract time)
		{
			if (time.Time2runtime == null)
			{
				return;
			}
			string tcName = "ref" + timeName;
			Variable.Builder v = Variable.builder().setName(tcName).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(timeName);
			g.addVariable(v);
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_REFERENCE));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_RUNTIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));
			v.addAttribute(new NcAttribute(CDM.UNITS, units));

			// lazy evaluation
			v.setSPobject(new Time2Dinfo(Time2DinfoType.timeAuxRef, null, time));
		}

		private void makeTimeCoordinate1D(Group.Builder g, CoordinateTimeIntv coordTime)
		{ // }, CoordinateRuntime
		  // runtime) {
			int ntimes = coordTime.Size;
			string tcName = coordTime.Name;
			string dims = coordTime.Name;
			g.addDimension(new Dimension(tcName, ntimes));
			Variable.Builder v = Variable.builder().setName(tcName).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(dims);
			g.addVariable(v);
			string units = coordTime.TimeUdUnit;
			v.addAttribute(new NcAttribute(CDM.UNITS, units));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, Grib.GRIB_VALID_TIME));
			v.addAttribute(new NcAttribute(CF.CALENDAR, Calendar.proleptic_gregorian.ToString()));

			double[] timeCoordValues = new double[ntimes];
			double[] timeCoordBoundsValues = new double[ntimes * 2];
			int count = GribTimeCoordIntervalUtils.generateTimeCoordValuesFromTimeCoordIntervals(coordTime.TimeIntervals, timeCoordValues, timeCoordBoundsValues, 0, coordTime.TimeUnit.Value, 0);
			Debug.Assert(count == ntimes);
			v.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { ntimes }, timeCoordValues), false);

			// bounds
			string bounds_name = tcName + "_bounds";
			Variable.Builder bounds = Variable.builder().setName(bounds_name).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(dims + " 2");
			g.addVariable(bounds);
			v.addAttribute(new NcAttribute(CF.BOUNDS, bounds_name));
			bounds.addAttribute(new NcAttribute(CDM.UNITS, units));
			bounds.addAttribute(new NcAttribute(CDM.LONG_NAME, "bounds for " + tcName));

			bounds.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { ntimes, 2 }, timeCoordBoundsValues), false);

			makeTimeAuxReference(g, tcName, units, coordTime);
		}

		private void makeVerticalCoordinate(Group.Builder g, CoordinateVert vc)
		{
			int n = vc.Size;
			string vcName = vc.Name.ToLower();

			g.addDimension(new Dimension(vcName, n));
			Variable.Builder v = Variable.builder().setName(vcName).setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName(vcName);
			g.addVariable(v);
			if (vc.Unit != null)
			{
				v.addAttribute(new NcAttribute(CDM.UNITS, vc.Unit));
				string desc = iosp.getVerticalCoordDesc(vc.Code);
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
				v.setCachedData(NcArray.factory(DataType.FLOAT, new int[] { n }, data), false);

				Variable.Builder bounds = Variable.builder().setName(vcName + "_bounds").setDataType(DataType.FLOAT).setParentGroupBuilder(g).setDimensionsByName(vcName + " 2");
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
				bounds.setCachedData(NcArray.factory(DataType.FLOAT, new int[] { n, 2 }, data), false);

			}
			else
			{
				float[] data = new float[n];
				int count = 0;
				foreach (VertCoordValue val in vc.LevelSorted)
				{
					data[count++] = (float)val.Value1;
				}
				v.setCachedData(NcArray.factory(DataType.FLOAT, new int[] { n }, data), false);
			}
		}

		private void makeEnsembleCoordinate(Group.Builder g, CoordinateEns ec)
		{
			int n = ec.Size;
			string ecName = ec.Name.ToLower();
			g.addDimension(new Dimension(ecName, n));

			Variable.Builder v = Variable.builder().setName(ecName).setDataType(DataType.INT).setParentGroupBuilder(g).setDimensionsByName(ecName);
			g.addVariable(v);
			v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Ensemble.ToString()));

			int[] data = new int[n];
			int count = 0;
			foreach (EnsCoordValue ecc in ec.EnsSorted)
			{
				data[count++] = ecc.EnsMember;
			}
			v.setCachedData(NcArray.factory(DataType.INT, new int[] { n }, data), false);
		}

		// orthogonal runtime, offset; both independent
		private string makeTimeOffsetOrthogonal(Group.Builder g, CoordinateTime2D time2D)
		{
			IList<object> offsets = time2D.OffsetsSorted;
			int n = offsets.Count;
			string toName = makeTimeOffsetName(time2D.Name);
			g.addDimension(new Dimension(toName, n));

			Variable.Builder v = Variable.builder().setName(toName).setDataType(DataType.DOUBLE).setParentGroupBuilder(g).setDimensionsByName(toName);
			g.addVariable(v);
			v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.TimeOffset.ToString()));
			v.addAttribute(new NcAttribute(CDM.UNITS, time2D.Unit));
			v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_OFFSET));
			v.addAttribute(new NcAttribute(CDM.LONG_NAME, CDM.TIME_OFFSET));
			v.addAttribute(new NcAttribute(CDM.UDUNITS, time2D.TimeUdUnit));
			v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.TimeOffset.ToString()));

			double[] data = new double[n];
			double[] bounds = null;
			int timeUnitValue = time2D.TimeUnit.Value;

			if (time2D.TimeInterval)
			{
				bounds = new double[2 * n];
				IList<TimeCoordIntvValue> intervals = (IList<TimeCoordIntvValue>)offsets;
				// TODO Should this be using 'timeUnitValue'? Wasn't in previous incarnation.
				int count = GribTimeCoordIntervalUtils.generateTimeCoordValuesFromTimeCoordIntervals(intervals, data, bounds, 0, timeUnitValue, 0);
				Debug.Assert(count == n);
			}
			else
			{
				int count = 0;
				foreach (object val in offsets)
				{
					int off = (int)val;
					data[count++] = timeUnitValue * off; // int ??
				}
			}
			v.setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { n }, data), false);

			if (time2D.TimeInterval)
			{
				string boundsName = toName + "_bounds";
				Variable.Builder coordVarBounds = VariableDS.builder().setName(boundsName).setDataType(DataType.DOUBLE).setDesc("TimeOffset coord bounds").setParentGroupBuilder(g).setDimensionsByName(toName + " 2").setCachedData(NcArray.factory(DataType.DOUBLE, new int[] { n, 2 }, bounds), false);

				g.addVariable(coordVarBounds);

				v.addAttribute(new NcAttribute(CF.BOUNDS, boundsName));
			}

			return toName;
		}

		// regular runtime, offset; offset depends on runtime hour from 0z
		private string makeTimeOffsetRegular(Group.Builder gb, CoordinateTime2D time2D)
		{
			try
			{
				ImmutableList<int> hourFrom0z = ImmutableList<int>.copyOf(time2D.RegularHourOffsets);
				int nhours = hourFrom0z.Count;
				int noffsets = time2D.Ntimes;
				string toName = makeTimeOffsetName(time2D.Name);
				gb.addDimension(new Dimension(toName, noffsets));
				string dimNames = nhours + " " + toName;

				Variable.Builder v = Variable.builder().setName(toName).setDataType(DataType.DOUBLE).setParentGroupBuilder(gb).setDimensionsByName(dimNames);
				gb.addVariable(v);
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.TimeOffset.ToString()));
				v.addAttribute(new NcAttribute(CDM.UNITS, time2D.Unit));
				v.addAttribute(new NcAttribute(CF.STANDARD_NAME, CF.TIME_OFFSET));
				v.addAttribute(new NcAttribute(CDM.LONG_NAME, CDM.TIME_OFFSET));
				v.addAttribute(new NcAttribute(CDM.UDUNITS, time2D.TimeUdUnit));
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.TimeOffset.ToString()));

				// Special Coordinates. TODO is there a CF equivalent?
				v.addAttribute(new NcAttribute(CDM.RUNTIME_COORDINATE, gb.makeFullName() + time2D.RuntimeCoordinate.Name));
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
						// may be uneven number of values for each hour
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
					Variable.Builder coordVarBounds = Variable.builder().setName(boundsName).setDataType(DataType.DOUBLE).setDesc("time offset coord bounds").setParentGroupBuilder(gb).setDimensionsByName(dimNames + " 2");
					gb.addVariable(coordVarBounds);
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
	}

}
