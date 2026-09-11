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
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;

	/// <summary>
	/// Constants used in CF Conventions.
	/// 
	/// Updated for CF 1.8
	/// 
	/// @author caron
	/// @author wchen@usgs.gov
	/// </summary>
	public class CF
	{

	  // general attribute names
	  public const string ACTUAL_RANGE = "actual_range";
	  public const string ANCILLARY_VARIABLES = "ancillary_variables";
	  public const string ADD_OFFSET = "add_offset";
	  public const string AXIS = "axis";
	  public const string BOUNDS = "bounds";
	  public const string CALENDAR = "calendar";
	  public const string CELL_METHODS = "cell_methods";
	  public const string CELL_MEASURES = "cell_measures";
	  public const string CF_ROLE = "cf_role";
	  public const string CLIMATOLOGY = "climatology";
	  public const string CONVENTIONS = "Conventions";
	  public const string COMPRESS = "compress";
	  public const string COORDINATES = "coordinates";
	  public const string EXTERNAL_VARIABLES = "external_variables";
	  public const string FEATURE_TYPE = "featureType";
	  public const string FORMULA_TERMS = "formula_terms";
	  public const string LONG_NAME = "long_name";
	  public const string POSITIVE = "positive";
	  public const string SCALE_FACTOR = "scale_factor";
	  public const string STANDARD_NAME = "standard_name";
	  public const string UNITS = "units";

	  // data types
	  public const string CHAR = "char";
	  public const string BYTE = "byte";
	  public const string SHORT = "short";
	  public const string INT = "int";
	  public const string FLOAT = "float";
	  public const string REAL = "real";
	  public const string DOUBLE = "double";

	  // data type range attributes
	  public const string VALID_MIN = "valid_min";
	  public const string VALID_MAX = "valid_max";
	  public const string VALID_RANGE = "valid_range";
	  public const string _FILLVALUE = "_FillValue";
	  public const string MISSING_VALUE = "missing_value";

	  // flag related attributes
	  public const string FLAG_MASKS = "flag_masks";
	  public const string FLAG_VALUES = "flag_values";
	  public const string FLAG_MEANINGS = "flag_meanings";

	  // general file attributes, though not required for compatibility with COOARDS
	  public const string TITLE = "title";
	  public const string HISTORY = "history";
	  public const string INSTITUTION = "institution";
	  public const string SOURCE = "source";
	  public const string COMMENT = "comment";
	  public const string REFERENCES = "references";

	  // positive values
	  public const string POSITIVE_UP = "up";
	  public const string POSITIVE_DOWN = "down";

	  // geometry attributes, introduced in CF 1.8
	  public const string GEOMETRY = "geometry";
	  public const string GEOMETRY_TYPE = "geometry_type";
	  public const string PART_NODE_COUNT = "part_node_count";
	  public const string NODES = "nodes";
	  public const string NODE_COUNT = "node_count";
	  public const string NODE_COORDINATES = "node_coordinates";
	  public const string INTERIOR_RING = "interior_ring";
	  public const string POINT = "point";
	  public const string LINE = "line";
	  public const string POLYGON = "polygon";

	  // calendar leap specification
	  public const string LEAP_MONTH = "leap_month";
	  public const string LEAP_YEAR = "leap_year";

	  // grid mapping names
	  public const string ALBERS_CONICAL_EQUAL_AREA = "albers_conical_equal_area";
	  public const string AZIMUTHAL_EQUIDISTANT = "azimuthal_equidistant";
	  public const string GEOSTATIONARY = "geostationary";
	  public const string LAMBERT_AZIMUTHAL_EQUAL_AREA = "lambert_azimuthal_equal_area";
	  public const string LAMBERT_CONFORMAL_CONIC = "lambert_conformal_conic";
	  public const string LAMBERT_CYLINDRICAL_EQUAL_AREA = "lambert_cylindrical_equal_area";
	  public const string LATITUDE_LONGITUDE = "latitude_longitude";
	  public const string MERCATOR = "mercator";
	  public const string ORTHOGRAPHIC = "orthographic";
	  public const string POLAR_STEREOGRAPHIC = "polar_stereographic";
	  public const string ROTATED_LATITUDE_LONGITUDE = "rotated_latitude_longitude";
	  public const string STEREOGRAPHIC = "stereographic";
	  public const string SINUSOIDAL = "sinusoidal"; // NOY
	  public const string TRANSVERSE_MERCATOR = "transverse_mercator";
	  public const string VERTICAL_PERSPECTIVE = "vertical_perspective";

	  // for grid_mappings
	  public const string EARTH_RADIUS = "earth_radius";
	  public const string FALSE_EASTING = "false_easting";
	  public const string FALSE_NORTHING = "false_northing";
	  public const string GRID_LATITUDE = "grid_latitude";
	  public const string GRID_LONGITUDE = "grid_longitude";
	  public const string GRID_MAPPING = "grid_mapping";
	  public const string FIXED_ANGLE_AXIS = "fixed_angle_axis"; // geostationary
	  public const string GRID_MAPPING_NAME = "grid_mapping_name";
	  public const string GRID_NORTH_POLE_LATITUDE = "grid_north_pole_latitude"; // rotated grid
	  public const string GRID_NORTH_POLE_LONGITUDE = "grid_north_pole_longitude"; // rotated grid
	  public const string ROTATED_LATITUDE = "rotated_latitude";
	  public const string ROTATED_LONGITUDE = "rotated_longitude";
	  public const string INVERSE_FLATTENING = "inverse_flattening";
	  public const string LATITUDE_OF_PROJECTION_ORIGIN = "latitude_of_projection_origin";
	  public const string LONGITUDE_OF_PROJECTION_ORIGIN = "longitude_of_projection_origin";
	  public const string LATITUDE_OF_PRIME_MERIDIAN = "latitude_of_prime_meridian";
	  public const string LONGITUDE_OF_PRIME_MERIDIAN = "longitude_of_prime_meridian";
	  public const string LONGITUDE_OF_CENTRAL_MERIDIAN = "longitude_of_central_meridian";
	  public const string NORTH_POLE_GRID_LONGITUDE = "north_pole_grid_longitude"; // rotated grid synonym for
																						  // GRID_NORTH_POLE_LONGITUDE
	  public const string PERSPECTIVE_POINT_HEIGHT = "perspective_point_height"; // geostationary
	  public const string SCALE_FACTOR_AT_CENTRAL_MERIDIAN = "scale_factor_at_central_meridian";
	  public const string SCALE_FACTOR_AT_PROJECTION_ORIGIN = "scale_factor_at_projection_origin";
	  public const string SEMI_MAJOR_AXIS = "semi_major_axis";
	  public const string SEMI_MINOR_AXIS = "semi_minor_axis";
	  public const string SWEEP_ANGLE_AXIS = "sweep_angle_axis"; // geostationary
	  public const string STANDARD_PARALLEL = "standard_parallel";
	  public const string STRAIGHT_VERTICAL_LONGITUDE_FROM_POLE = "straight_vertical_longitude_from_pole";

	  // vertical coordinate
	  public const string atmosphere_ln_pressure_coordinate = "atmosphere_ln_pressure_coordinate";
	  public const string atmosphere_sigma_coordinate = "atmosphere_sigma_coordinate";
	  public const string atmosphere_hybrid_sigma_pressure_coordinate = "atmosphere_hybrid_sigma_pressure_coordinate";
	  public const string atmosphere_hybrid_height_coordinate = "atmosphere_hybrid_height_coordinate";
	  public const string atmosphere_sleve_coordinate = "atmosphere_sleve_coordinate";
	  public const string ocean_sigma_coordinate = "ocean_sigma_coordinate";
	  public const string ocean_s_coordinate = "ocean_s_coordinate";
	  public const string ocean_sigma_z_coordinate = "ocean_sigma_z_coordinate";
	  public const string ocean_double_sigma_coordinate = "ocean_double_sigma_coordinate";

	  public const string formula_terms = "formula_terms";

	  // standard_names
	  public const string ENSEMBLE = "realization";
	  public const string LATITUDE = "latitude";
	  public const string LONGITUDE = "longitude";
	  public const string TIME = "time"; // valid; time, obs time
	  public const string TIME_REFERENCE = "forecast_reference_time"; // the "data time", the time of the analysis
																			 // from which the forecast was made.
	  public const string TIME_OFFSET = "forecast_period"; // Forecast period is the time interval between the
																  // forecast reference time and the validity time. A period
																  // is an interval of time,

	  public const string PROJECTION_X_COORDINATE = "projection_x_coordinate";
	  public const string PROJECTION_Y_COORDINATE = "projection_y_coordinate";

	  public const string PROJECTION_X_ANG_COORDINATE = "projection_x_angular_coordinate";
	  public const string PROJECTION_Y_ANG_COORDINATE = "projection_y_angular_coordinate";

	  // cf_role
	  public const string PROFILE_ID = "profile_id";
	  public const string TIMESERIES_ID = "timeseries_id"; // alias STATION_ID
	  public const string TRAJECTORY_ID = "trajectory_id";

	  // DSG
	  public const string SAMPLE_DIMENSION = "sample_dimension";
	  public const string INSTANCE_DIMENSION = "instance_dimension";

	  public const string PLATFORM_NAME = "platform_name"; // instead of STATION_DESC
	  public const string SURFACE_ALTITUDE = "surface_altitude"; // alias STATION_ALTITUDE
	  public const string PLATFORM_ID = "platform_id"; // alias STATION_WMOID

	  // not official - used in CFwriter for attribute name
	  public const string DSG_REPRESENTATION = "DSG_representation";

	  ///////////////////////////////////////////////////////////////////
	  // DSG proposed - not adopted; here for backwards compatibility
	  public const string RAGGED_ROWSIZE = "CF:ragged_row_count";
	  public const string RAGGED_PARENTINDEX = "CF:ragged_parent_index";

	  // proposed standard_names
	  public const string STATION_ID = "station_id";
	  public const string STATION_DESC = "station_description";
	  public const string STATION_ALTITUDE = "station_altitude";
	  public const string STATION_WMOID = "station_WMO_id";

	  public const string featureTypeAtt2 = "CF:featureType";
	  public const string featureTypeAtt3 = "CF:feature_type"; // GRIB was using this form (!)
	  ///////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Map from CF feature type names to our FeatureType enums. </summary>
	  public sealed class FeatureType
	  {
		public static readonly FeatureType point = new FeatureType("point", InnerEnum.point);
		public static readonly FeatureType timeSeries = new FeatureType("timeSeries", InnerEnum.timeSeries);
		public static readonly FeatureType profile = new FeatureType("profile", InnerEnum.profile);
		public static readonly FeatureType trajectory = new FeatureType("trajectory", InnerEnum.trajectory);
		public static readonly FeatureType timeSeriesProfile = new FeatureType("timeSeriesProfile", InnerEnum.timeSeriesProfile);
		public static readonly FeatureType trajectoryProfile = new FeatureType("trajectoryProfile", InnerEnum.trajectoryProfile);
		public static readonly FeatureType line = new FeatureType("line", InnerEnum.line);
		public static readonly FeatureType polygon = new FeatureType("polygon", InnerEnum.polygon);

		private static readonly List<FeatureType> valueList = new List<FeatureType>();

		static FeatureType()
		{
			valueList.Add(point);
			valueList.Add(timeSeries);
			valueList.Add(profile);
			valueList.Add(trajectory);
			valueList.Add(timeSeriesProfile);
			valueList.Add(trajectoryProfile);
			valueList.Add(line);
			valueList.Add(polygon);
		}

		public enum InnerEnum
		{
			point,
			timeSeries,
			profile,
			trajectory,
			timeSeriesProfile,
			trajectoryProfile,
			line,
			polygon
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


// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static FeatureType convert(NetcdfSharp.ucar.nc2.constants.FeatureType ft)
		public static FeatureType convert(NetcdfSharp.ucar.nc2.constants.FeatureType ft)
		{
		  switch (ft.innerEnumValue)
		  {
			case NetcdfSharp.ucar.nc2.constants.FeatureType.InnerEnum.POINT:
			  return CF.FeatureType.point;
			case NetcdfSharp.ucar.nc2.constants.FeatureType.InnerEnum.STATION:
			  return CF.FeatureType.timeSeries;
			case NetcdfSharp.ucar.nc2.constants.FeatureType.InnerEnum.PROFILE:
			  return CF.FeatureType.profile;
			case NetcdfSharp.ucar.nc2.constants.FeatureType.InnerEnum.TRAJECTORY:
			  return CF.FeatureType.trajectory;
			case NetcdfSharp.ucar.nc2.constants.FeatureType.InnerEnum.STATION_PROFILE:
			  return CF.FeatureType.timeSeriesProfile;
			case NetcdfSharp.ucar.nc2.constants.FeatureType.InnerEnum.TRAJECTORY_PROFILE:
			  return CF.FeatureType.trajectoryProfile;
		  }
		  return null;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static NetcdfSharp.ucar.nc2.constants.FeatureType convert(FeatureType cff)
		public static NetcdfSharp.ucar.nc2.constants.FeatureType convert(FeatureType cff)
		{
		  switch (cff.innerEnumValue)
		  {
			case NetcdfSharp.ucar.nc2.constants.CF.FeatureType.InnerEnum.point:
			  return NetcdfSharp.ucar.nc2.constants.FeatureType.POINT;
			case NetcdfSharp.ucar.nc2.constants.CF.FeatureType.InnerEnum.timeSeries:
			  return NetcdfSharp.ucar.nc2.constants.FeatureType.STATION;
			case NetcdfSharp.ucar.nc2.constants.CF.FeatureType.InnerEnum.profile:
			  return NetcdfSharp.ucar.nc2.constants.FeatureType.PROFILE;
			case NetcdfSharp.ucar.nc2.constants.CF.FeatureType.InnerEnum.trajectory:
			  return NetcdfSharp.ucar.nc2.constants.FeatureType.TRAJECTORY;
			case NetcdfSharp.ucar.nc2.constants.CF.FeatureType.InnerEnum.timeSeriesProfile:
			  return NetcdfSharp.ucar.nc2.constants.FeatureType.STATION_PROFILE;
			case NetcdfSharp.ucar.nc2.constants.CF.FeatureType.InnerEnum.trajectoryProfile:
			  return NetcdfSharp.ucar.nc2.constants.FeatureType.TRAJECTORY_PROFILE;
		  }
		  return null;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static FeatureType getFeatureType(String s)
		public static FeatureType getFeatureType(string s)
		{
		  if (s.Equals("point", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.point;
		  }
		  if (s.Equals("timeSeries", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.timeSeries;
		  }
		  if (s.Equals("stationTimeSeries", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.timeSeries;
		  }
		  if (s.Equals("station", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.timeSeries;
		  }
		  if (s.Equals("profile", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.profile;
		  }
		  if (s.Equals("trajectory", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.trajectory;
		  }
		  if (s.Equals("timeSeriesProfile", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.timeSeriesProfile;
		  }
		  if (s.Equals("stationProfile", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.timeSeriesProfile;
		  }
		  if (s.Equals("stationProfileTimeSeries", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.timeSeriesProfile;
		  }
		  if (s.Equals("trajectoryProfile", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.trajectoryProfile;
		  }
		  if (s.Equals("section", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.trajectoryProfile;
		  }
		  if (s.Equals("line", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.line;
		  }
		  if (s.Equals("polygon", StringComparison.OrdinalIgnoreCase))
		  {
			return FeatureType.polygon;
		  }
		  return null;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static FeatureType getFeatureTypeFromGlobalAttribute(NetcdfSharp.ucar.nc2.NetcdfFile ds)
		public static FeatureType getFeatureTypeFromGlobalAttribute(NetcdfSharp.ucar.nc2.NetcdfFile ds)
		{
		  string ftypeS = ds.RootGroup.findAttributeString(CF.FEATURE_TYPE, null);
		  if (string.ReferenceEquals(ftypeS, null))
		  {
			ftypeS = ds.RootGroup.findAttributeString(CF.featureTypeAtt2, null);
		  }
		  if (string.ReferenceEquals(ftypeS, null))
		  {
			ftypeS = ds.RootGroup.findAttributeString(CF.featureTypeAtt3, null);
		  }

		  if (string.ReferenceEquals(ftypeS, null))
		  {
			return null;
		  }

		  return CF.FeatureType.getFeatureType(ftypeS);
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

	  /// <summary>
	  /// Enumeration of CF cell methods.
	  /// see "http://cf-pcmdi.llnl.gov/documents/cf-conventions/1.6/cf-conventions.html#appendix-cell-methods"
	  /// </summary>
	  public sealed class CellMethods
	  {
		public static readonly CellMethods point = new CellMethods("point", InnerEnum.point);
		public static readonly CellMethods sum = new CellMethods("sum", InnerEnum.sum);
		public static readonly CellMethods maximum = new CellMethods("maximum", InnerEnum.maximum);
		public static readonly CellMethods median = new CellMethods("median", InnerEnum.median);
		public static readonly CellMethods mid_range = new CellMethods("mid_range", InnerEnum.mid_range);
		public static readonly CellMethods minimum = new CellMethods("minimum", InnerEnum.minimum);
		public static readonly CellMethods mean = new CellMethods("mean", InnerEnum.mean);
		public static readonly CellMethods mode = new CellMethods("mode", InnerEnum.mode);
		public static readonly CellMethods standard_deviation = new CellMethods("standard_deviation", InnerEnum.standard_deviation);
		public static readonly CellMethods variance = new CellMethods("variance", InnerEnum.variance);

		private static readonly List<CellMethods> valueList = new List<CellMethods>();

		static CellMethods()
		{
			valueList.Add(point);
			valueList.Add(sum);
			valueList.Add(maximum);
			valueList.Add(median);
			valueList.Add(mid_range);
			valueList.Add(minimum);
			valueList.Add(mean);
			valueList.Add(mode);
			valueList.Add(standard_deviation);
			valueList.Add(variance);
		}


		public enum InnerEnum
		{
			point,
			sum,
			maximum,
			median,
			mid_range,
			minimum,
			mean,
			mode,
			standard_deviation,
			variance
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private CellMethods(string name, InnerEnum innerEnum)
		{
			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public static CellMethods[] values()
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

		public static CellMethods valueOf(string name)
		{
			foreach (CellMethods enumInstance in CellMethods.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new ArgumentException(name);
		}

		// deprecated
		public static CellMethods convertGribCodeTable4_10(int code)
		{
			switch (code)
			{
				case 0:
					return CellMethods.mean; // "Average";
				case 1:
					return CellMethods.sum; // "Accumulation";
				case 2:
					return CellMethods.maximum; // "Maximum";
				case 3:
					return CellMethods.minimum; // "Minimum";
				// case 4: return "Difference"; // (Value at the end of time range minus value at the beginning)
				// case 5: return "RootMeanSquare";
				case 6:
					return CellMethods.standard_deviation; // "StandardDeviation";
				case 7:
					return CellMethods.variance; // "Covariance"; // (Temporal variance)
				// case 8: return "Difference"; // (Value at the start of time range minus value at the end)
				// case 9: return "Ratio";
				default:
					return null;
			}
		}
	  }

	  // class not interface, per Bloch edition 2 item 19
	  private CF() { } // disable instantiation

	}

}
