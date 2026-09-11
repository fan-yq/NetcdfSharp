// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.constants
{
	/// <summary>
	/// Enumeration of CDM Feature types, aka "Scientific Data Types".
	/// 
	/// @author john caron
	/// @author wchen@usgs.gov </summary>
	/// <seealso cref="<a href="
	///      "https://www.unidata.ucar.edu/software/thredds/current/netcdf-java/reference/FeatureDatasets/Overview.html">CDM
	///      Feature Types</a>/>
	/*
	 * 1) The CF discrete sampling proposal will be the recommended one for point data when thats finalized. Unfortunately,
	 * it will be somewhat different from whats gone before. The CF: prefix is dropped until the namespace proposal can be
	 * completed. So those feature types are now proposed to be:
	 * 
	 * point: one or more parameters measured at a set of points in time and space
	 * timeSeries: a time-series of data points at the same location, with varying time
	 * trajectory: a connected set of data points along a 1D curve in time and space
	 * profile: a set of data points along a vertical line
	 * timeSeriesProfile: a time-series of profiles at a named location
	 * trajectoryProfile: a collection of profiles which originate along a trajectory
	 * 
	 * The CDM will be backwards compatible, including:
	 * 
	 * accepting the CF: prefix
	 * being case insensitive
	 * "station" and "stationTimeSeries"as aliases for "timeSeries"
	 * "stationProfile" as alias for "timeSeriesProfile"
	 * "section" as alias for "trajectoryProfile"
	 * 
	 * I know that CF wants to standardize on other feature types also. Its hard to anticipate what they will come with, but
	 * its likely:
	 * 
	 * grid
	 * swath
	 * 
	 * maybe:
	 * 
	 * image
	 * radial
	 * unstructuredGrid
	 */

	public sealed class FeatureType
	{
	  public static readonly FeatureType ANY = new FeatureType("ANY", InnerEnum.ANY); // No specific type

	  public static readonly FeatureType COVERAGE = new FeatureType("COVERAGE", InnerEnum.COVERAGE); // any of the coverage types: GRID, FMRC, SWATH, CURVILINEAR
	  public static readonly FeatureType GRID = new FeatureType("GRID", InnerEnum.GRID); // seperable coordinates
	  public static readonly FeatureType FMRC = new FeatureType("FMRC", InnerEnum.FMRC); // two time dimensions, runtime and forecast time
	  public static readonly FeatureType SWATH = new FeatureType("SWATH", InnerEnum.SWATH); // 2D latlon, dependent time, polar orbiting satellites
	  public static readonly FeatureType CURVILINEAR = new FeatureType("CURVILINEAR", InnerEnum.CURVILINEAR); // 2D latlon, independent time

	  public static readonly FeatureType ANY_POINT = new FeatureType("ANY_POINT", InnerEnum.ANY_POINT); // Any of the point types
	  public static readonly FeatureType POINT = new FeatureType("POINT", InnerEnum.POINT); // unconnected points
	  public static readonly FeatureType PROFILE = new FeatureType("PROFILE", InnerEnum.PROFILE); // fixed x,y with data along z
	  public static readonly FeatureType STATION = new FeatureType("STATION", InnerEnum.STATION); // timeseries at named location
	  public static readonly FeatureType STATION_PROFILE = new FeatureType("STATION_PROFILE", InnerEnum.STATION_PROFILE); // timeseries of profiles
	  public static readonly FeatureType TRAJECTORY = new FeatureType("TRAJECTORY", InnerEnum.TRAJECTORY); // connected points in space and time
	  public static readonly FeatureType TRAJECTORY_PROFILE = new FeatureType("TRAJECTORY_PROFILE", InnerEnum.TRAJECTORY_PROFILE); // trajectory of profiles

	  public static readonly FeatureType RADIAL = new FeatureType("RADIAL", InnerEnum.RADIAL); // polar coordinates
	  public static readonly FeatureType STATION_RADIAL = new FeatureType("STATION_RADIAL", InnerEnum.STATION_RADIAL); // time series of radial data

	  public static readonly FeatureType SIMPLE_GEOMETRY = new FeatureType("SIMPLE_GEOMETRY", InnerEnum.SIMPLE_GEOMETRY); // geospatial associations with data

	  // experimental
	  public static readonly FeatureType IMAGE = new FeatureType("IMAGE", InnerEnum.IMAGE); // pixels, may not be geolocatable
	  public static readonly FeatureType UGRID = new FeatureType("UGRID", InnerEnum.UGRID); // unstructured grids

	  private static readonly List<FeatureType> valueList = new List<FeatureType>();

	  static FeatureType()
	  {
		  valueList.Add(ANY);
		  valueList.Add(COVERAGE);
		  valueList.Add(GRID);
		  valueList.Add(FMRC);
		  valueList.Add(SWATH);
		  valueList.Add(CURVILINEAR);
		  valueList.Add(ANY_POINT);
		  valueList.Add(POINT);
		  valueList.Add(PROFILE);
		  valueList.Add(STATION);
		  valueList.Add(STATION_PROFILE);
		  valueList.Add(TRAJECTORY);
		  valueList.Add(TRAJECTORY_PROFILE);
		  valueList.Add(RADIAL);
		  valueList.Add(STATION_RADIAL);
		  valueList.Add(SIMPLE_GEOMETRY);
		  valueList.Add(IMAGE);
		  valueList.Add(UGRID);
	  }

	  public enum InnerEnum
	  {
		  ANY,
		  COVERAGE,
		  GRID,
		  FMRC,
		  SWATH,
		  CURVILINEAR,
		  ANY_POINT,
		  POINT,
		  PROFILE,
		  STATION,
		  STATION_PROFILE,
		  TRAJECTORY,
		  TRAJECTORY_PROFILE,
		  RADIAL,
		  STATION_RADIAL,
		  SIMPLE_GEOMETRY,
		  IMAGE,
		  UGRID
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private FeatureType(string name, InnerEnum innerEnum)
	  {
		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  /// <summary>
	  /// Find the FeatureType that matches this name.
	  /// </summary>
	  /// <param name="name"> find FeatureType with this name, case insensitive. </param>
	  /// <returns> FeatureType or null if no match. </returns>
	  public static FeatureType getType(string name)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  return null;
		}
		try
		{
		  return valueOf(name.ToUpper());
		}
		catch (ArgumentException)
		{ // lame!
		  return null;
		}
	  }

	  public bool PointFeatureType
	  {
		  get
		  {
			return (this == FeatureType.POINT) || (this == FeatureType.STATION) || (this == FeatureType.TRAJECTORY) || (this == FeatureType.PROFILE) || (this == FeatureType.STATION_PROFILE) || (this == FeatureType.TRAJECTORY_PROFILE) || (this == FeatureType.ANY_POINT);
		  }
	  }

	  public bool CoverageFeatureType
	  {
		  get
		  {
			return (this == FeatureType.COVERAGE) || (this == FeatureType.GRID) || (this == FeatureType.FMRC) || (this == FeatureType.SWATH) || (this == FeatureType.CURVILINEAR);
		  }
	  }

	  public bool UnstructuredGridFeatureType
	  {
		  get
		  {
			return this == FeatureType.UGRID;
		  }
	  }

	  public bool SimpleGeometry
	  {
		  get
		  {
			return this == FeatureType.SIMPLE_GEOMETRY;
		  }
	  }

		public static FeatureType[] values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static FeatureType valueOf(string name)
		{
			foreach (FeatureType enumInstance in FeatureType.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new ArgumentException(name);
		}
	}

}
