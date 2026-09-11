// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.grib1.tables
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using GribStatType = NetcdfSharp.ucar.nc2.grib.GribStatType;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using Grib1ParamLevel = NetcdfSharp.ucar.nc2.grib.grib1.Grib1ParamLevel;
	using Grib1ParamTime = NetcdfSharp.ucar.nc2.grib.grib1.Grib1ParamTime;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;

	/// <summary>
	/// Local tables for JMA (center 34)
	/// 
	/// @author caron </summary>
	/// <seealso cref=""http://rda.ucar.edu/datasets/ds628.0/docs/JRA-55.grib_20130319.xlsx""
	/// @since 1/27/2015/>
	public class JmaTables : Grib1Customizer
	{
	  private static IDictionary<int, VertCoordType> levelTypesMap; // shared by all instances

	  internal JmaTables(Grib1ParamTables tables) : base(34, tables)
	  {
	  }

	  /////////////////// local time types
	  public override Grib1ParamTime getParamTime(Grib1SectionProductDefinition pds)
	  {
		int p1 = pds.TimeValue1; // octet 19
		int p2 = pds.TimeValue2; // octet 20
		int timeRangeIndicator = pds.TimeRangeIndicator; // octet 21
		int n = pds.Nincluded;

		int start;
		int end;
		int forecastTime = 0;
		bool isInterval;

		switch (timeRangeIndicator)
		{
		  /*
		   * Monthly-diurnal (ds628.5/fcst_phy2m125_diurnal)
		   * Average over the days of the month.
		   * 128: Average of N forecast products with a valid time ranging between reference time + P1 and reference time +
		   * P2;
		   * products have reference times at Intervals of 24 hours, beginning at the given reference time.
		   */
		  case 128:
			isInterval = true;
			start = p1;
			end = p2;
			break;

		  /*
		   * 129: Temporal variance of N forecasts; each product has valid time ranging between reference time + P1 and
		   * reference time + P2;
		   * products have reference times at intervals of 24 hours, beginning at the given reference time;
		   * unit of measurement is square of that in Code Table 2
		   */
		   // LOOK

		  /*
		   * 131: Temporal variance of N forecasts; valid time of the first product ranges between R + P1 and R + P2,
		   * where R is reference time given in octets 13 to 17, then subsequent products have valid time range at interval
		   * of P2 - P1;
		   * thus all N products cover continuous time span; products have reference times at intervals of P2 - P1,
		   * beginning at the given reference time;
		   * unit of measurement is square of that in Code Table 2
		   */
		  // LOOK


		  case 129:
		  case 131:
			isInterval = true;
			start = p1;
			end = p2;
			break;

		  /*
		   * 130: Average of N forecast products; valid time of the first product ranges between R + P1 and R + P2, where R
		   * is reference time given in octets 13 to 17,
		   * then subsequent products have valid time range at interval of P2 - P1; thus all N products cover continuous
		   * time span; products have reference times at
		   * intervals of P2 - P1, beginning at the given reference time
		   */
		  case 130:
			isInterval = true;
			start = p1;
			end = (n > 0) ? p1 + n * (p2 - p1) : p2; // prob n >= 1
			break;

		  /*
		   * Temporal variance of N uninitialized analyses (P1 = 0) or instantaneous forecasts (P1 > 0);
		   * each product has valid time at the reference time + P1;
		   * products have reference times at intervals of P2, beginning at the given reference time;
		   * unit of measurement is square of that in Code Table 2
		   */
		   // LOOK
		  case 132:
			forecastTime = p1;
			start = p1;
			end = (n > 0) ? p1 + (n - 1) * p2 : p1; // LOOK ??
			isInterval = (n > 0);
			break;

		  default:
			return base.getParamTime(pds);
		}

		return new Grib1ParamTime(this, timeRangeIndicator, isInterval, start, end, forecastTime);
	  }

	  public override string getTimeTypeName(int timeRangeIndicator)
	  {
		switch (timeRangeIndicator)
		{
		  case 128:
			return "Average over the days in the month";
		  case 129:
			return "Temporal variance of N forecasts at 24 hour intervals";
		  case 130:
			return "Forecast, 6-hour averaged, then one-month averaged";
		  case 131:
			return "Temporal variance of N forecasts at intervals of P1 - P2";
		  case 132:
			return "Temporal variance of N uninitialized analyses (P1 = 0) or instantaneous forecasts (P1 > 0)";

		  default:
			return base.getTimeTypeName(timeRangeIndicator);
		}
	  }

	  public override GribStatType getStatType(int timeRangeIndicator)
	  {
		switch (timeRangeIndicator)
		{
		  case 128:
		  case 130:
			return GribStatType.Average;

		  case 129:
		  case 131:
		  case 132:
			return GribStatType.Variance;
		  default:
			return base.getStatType(timeRangeIndicator);
		}
	  }


	  ///////////////////// levels
	  public override Grib1ParamLevel getParamLevel(Grib1SectionProductDefinition pds)
	  {
		int levelType = pds.LevelType;
		int pds11 = pds.LevelValue1;
		int pds12 = pds.LevelValue2;
		int pds1112 = pds11 << 8 | pds12;

		switch (levelType)
		{
		  case 211:
		  case 212:
			return new Grib1ParamLevel(this, levelType, GribNumbers.MISSING, GribNumbers.MISSING);

		  case 100:
			return new Grib1ParamLevel(this, levelType, pds1112, GribNumbers.MISSING);

		  case 213:
			return new Grib1ParamLevel(this, levelType, pds1112, GribNumbers.MISSING);

		  default:
			return new Grib1ParamLevel(this, pds);
		}
	  }

	  protected internal override VertCoordType getLevelType(int code)
	  {
		if (levelTypesMap == null)
		{
		  makeLevelTypesMap();
		}
		VertCoordType levelType;
		levelTypesMap.TryGetValue(code, out levelType);
		if (levelType != null)
		{
		  return levelType;
		}
		return base.getLevelType(code);
	  }

	  private static void makeLevelTypesMap()
	  {
		levelTypesMap = new Dictionary<int, VertCoordType>(10);
		// (int code, String desc, String abbrev, String units, String datum, boolean isPositiveUp, boolean isLayer)
		levelTypesMap[100] = new VertCoordType(100, "Isobaric Surface", "isobaric_surface_low", "hPa", null, false, false); // 3D
		levelTypesMap[211] = new VertCoordType(211, "Entire soil", "entire_soil", "", null, false, false);
		levelTypesMap[212] = new VertCoordType(212, "The bottom of land surface model", "bottom_of_model", "", null, false, false);
		levelTypesMap[213] = new VertCoordType(213, "Underground layer number of land surface model", "underground_layer", "layer", null, false, false); // 3D
	  }

	  //////////////////// gen process
	  private static IDictionary<int, string> genProcessMap;

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getGeneratingProcessName(int genProcess)
	  public override string getGeneratingProcessName(int genProcess)
	  {
		if (genProcessMap == null)
		{
		  makeGenProcessMap();
		}
		string name;
		return genProcessMap.TryGetValue(genProcess, out name) ? name : null;
	  }

	  private static void makeGenProcessMap()
	  {
		genProcessMap = new Dictionary<int, string>(100);
		genProcessMap[0] = "Undefined (not to specify generating process)";
		genProcessMap[1] = "Global Spectral Model (GSM8803_T63L16)";
		genProcessMap[2] = "Global Spectral Model (GSM8903_T106L21)";
		genProcessMap[3] = "Global Spectral Model (GSM9603_T213L30)";
		genProcessMap[4] = "Global Spectral Model (GSM0103_T213L40) ";
		genProcessMap[21] = "One-week EPS (GSM0103_T106L40)";
		genProcessMap[31] = "Regional Spectral Model (RSM0103)";
		genProcessMap[32] = "Mesoscale Model (MSM0103)";
		genProcessMap[51] = "One-month EPS (GSM9603_T63L30)";
		genProcessMap[52] = "One-month EPS (GSM9603_T106L40)";
		genProcessMap[53] = "One-month EPS (GSM0603C_TL159L40)";
		genProcessMap[70] = "Seasonal EPS (GSM0103_T63L40)";
		genProcessMap[71] = "Seasonal EPS (GSM0502C_TL95L40)";
		genProcessMap[90] = "Sea surface wind correction";
		genProcessMap[101] = "NOAA-AVHRR analysis data";
		genProcessMap[102] = "VISSR grid point data (for cloudiness, TBB etc.)";
		genProcessMap[103] = "Long-wave radiation data";
		genProcessMap[104] = "GMS data (sea surface temperature)";
		genProcessMap[105] = "Snow/ice area data";
		genProcessMap[106] = "Global solar radiation";
		genProcessMap[141] = "Sea surface temperature analysis (average for dekad or ten days)";
		genProcessMap[142] = "Sea surface temperature analysis";
		genProcessMap[143] = "Ocean current analysis";
		genProcessMap[144] = "Global ocean wave model";
		genProcessMap[150] = "Regional ocean wave model";
		genProcessMap[200] = "Volcaninc ash prediction";
		genProcessMap[201] = "Japanese 55-year Reanalysis (JRA-55)";
	  }

	}

}
