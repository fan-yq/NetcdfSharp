// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1.tables
{
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using GribResourceReader = NetcdfSharp.ucar.nc2.grib.GribResourceReader;
	using GribStatType = NetcdfSharp.ucar.nc2.grib.GribStatType;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using Grib1ParamTime = NetcdfSharp.ucar.nc2.grib.grib1.Grib1ParamTime;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;

	/// <summary>
	/// NCEP overrides of GRIB tables
	/// LOOK: Why not a singleton?
	/// 
	/// @author caron
	/// @since 1/13/12
	/// </summary>
	public class NcepTables : Grib1Customizer
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(NcepTables));

	  private static IDictionary<int, string> genProcessMap; // shared by all instances
	  private static IDictionary<int, VertCoordType> levelTypesMap; // shared by all instances

	  internal NcepTables(Grib1ParamTables tables) : base(7, tables)
	  {
	  }

	  protected internal NcepTables(int center, Grib1ParamTables tables) : base(center, tables)
	  {
	  }

	  ////////////// time types ////////////////////////////////////
	  // http://www.nco.ncep.noaa.gov/pmb/docs/on388/table5.html
	  public override GribStatType getStatType(int timeRangeIndicator)
	  {

		switch (timeRangeIndicator)
		{
		  case 128:
		  case 129:
		  case 130:
		  case 131:
		  case 132:
		  case 133:
		  case 137:
		  case 138:
		  case 139:
		  case 140:
			return GribStatType.Average;
		  case 134:
			return GribStatType.RootMeanSquare;
		  case 135:
		  case 136:
			return GribStatType.StandardDeviation;
		  default:
			return base.getStatType(timeRangeIndicator);
		}
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

		  case 128:
		  case 129:
		  case 130:
		  case 131:
		  case 137:
		  case 138:
		  case 139:
		  case 140:
			isInterval = true;
			start = p1;
			end = p2;
			break;

		  case 132:
		  case 133:
		  case 134:
		  case 135:
		  case 136:
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
			/*
			 * Average of forecast accumulations. P1 = start of accumulation period. P2 = end of accumulation period.
			 * Reference time is the start time of the first forecast, other forecasts at 24-hour intervals.
			 * Number in Ave = number of forecasts used.
			 */
			return "Average of forecast accumulations at 24 hour intervals, period = (RT + P1) to (RT + P2)";

		  case 129:
			/*
			 * Average of successive forecast accumulations. P1 = start of accumulation period.
			 * P2 = end of accumulation period. Reference time is the start time of the first forecast,
			 * other forecasts at (P2 - P1) intervals. Number in Ave = number of forecasts used
			 */
			return "Average of successive forecast accumulations, period = (RT + P1) to (RT + P2)";

		  case 130:
			/*
			 * Average of forecast averages. P1 = start of averaging period. P2 = end of averaging period.
			 * Reference time is the start time of the first forecast, other forecasts at 24-hour intervals.
			 * Number in Ave = number of forecast used
			 */
			return "Average of forecast averages at 24 hour intervals, period = (RT + P1) to (RT + P2)";

		  case 131:
			/*
			 * Average of successive forecast averages. P1 = start of averaging period. P2 = end of averaging period.
			 * Reference time is the start time of the first forecast, other forecasts at (P2 - P1) intervals.
			 * Number in Ave = number of forecasts used
			 */
			return "Average of successive forecast averages, period = (RT + P1) to (RT + P2)";

		  case 132:
			return "Climatological Average of N analyses, each a year apart, starting from initial time R and for the period from R+P1 to R+P2.";

		  case 133:
			return "Climatological Average of N forecasts, each a year apart, starting from initial time R and for the period from R+P1 to R+P2.";

		  case 134:
			return "Climatological Root Mean Square difference between N forecasts and their verifying analyses, each a year apart, starting with initial time R and for the period from R+P1 to R+P2.";

		  case 135:
			return "Climatological Standard Deviation of N forecasts from the mean of the same N forecasts, for forecasts one year apart. ";

		  case 136:
			return "Climatological Standard Deviation of N analyses from the mean of the same N analyses, for analyses one year apart.";

		  case 137:
			/*
			 * Average of forecast accumulations. P1 = start of accumulation period. P2 = end of accumulation period.
			 * Reference time is the start time of the first forecast, other forecasts at 6-hour intervals.
			 * Number in Ave = number of forecast used
			 */
			return "Average of forecast accumulations at 6 hour intervals, period = (RT + P1) to (RT + P2)";

		  case 138:
			/*
			 * Average of forecast averages. P1 = start of averaging period. P2 = end of averaging period.
			 * Reference time is the start time of the first forecast, other forecasts at 6-hour intervals.
			 * Number in Ave = number of forecast used
			 */
			return "Average of forecast averages at 6 hour intervals, period = (RT + P1) to (RT + P2)";

		  case 139:
			/*
			 * Average of forecast accumulations. P1 = start of accumulation period. P2 = end of accumulation period.
			 * Reference time is the start time of the first forecast, other forecasts at 12-hour intervals.
			 * Number in Ave = number of forecast used
			 */
			return "Average of forecast accumulations at 12 hour intervals, period = (RT + P1) to (RT + P2)";

		  case 140:
			/*
			 * Average of forecast averages. P1 = start of averaging period. P2 = end of averaging period.
			 * Reference time is the start time of the first forecast, other forecasts at 12-hour intervals.
			 * Number in Ave = number of forecast used
			 */
			return "Average of forecast averages at 12 hour intervals, period = (RT + P1) to (RT + P2)";

		  default:
			return base.getTimeTypeName(timeRangeIndicator);
		}
	  }

	  //////////////////////////////////////////// genProcess

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getGeneratingProcessName(int genProcess)
	  public override string getGeneratingProcessName(int genProcess)
	  {
		if (genProcessMap == null)
		{
		  genProcessMap = NcepGenProcess;
		}
		if (genProcessMap == null)
		{
		  return null;
		}

		return genProcessMap[genProcess];
	  }

	  // public so can be called from Grib2
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static NetcdfSharp.java.util.Map<int, String> getNcepGenProcess()
	  public static IDictionary<int, string> NcepGenProcess
	  {
		  get
		  {
			if (genProcessMap != null)
			{
			  return genProcessMap;
			}
			string path = "resources/grib1/ncep/ncepTableA.xml";
			try
			{
					using (Stream @is = GribResourceReader.getInputStream(path))
					{
				  SAXBuilder builder = new SAXBuilder();
				  builder.setExpandEntities(false);
				  NetcdfSharp.org.jdom2.Document doc = builder.build(@is);
				  Element root = doc.getRootElement();

				  Dictionary<int, string> result = new Dictionary<int, string>(200);
				  IList<Element> @params = root.getChildren("parameter");
				  foreach (Element elem1 in @params)
				  {
					int code = int.Parse(elem1.getAttributeValue("code"));
					string desc = elem1.getChildText("description");
					result[code] = desc;
				  }
				  return Collections.unmodifiableMap(result); // all at once - thread safe

					}
			}
			catch (Exception ioe) when (ioe is IOException || ioe is JDOMException)
			{
			  logger.error("Cant read NCEP Table 1 = " + path, ioe);
			  return null;
			}
		  }
	  }

	  ///////////////////////////////////////// levels
	  protected internal override VertCoordType getLevelType(int code)
	  {
		if (code < 129)
		{
		  return base.getLevelType(code); // LOOK dont let NCEP override standard tables (??) looks like a conflict with
		}
										   // level code 210 (!)

		if (levelTypesMap == null)
		{
		  levelTypesMap = readTable3("resources/grib1/ncep/ncepTable3.xml");
		}
		if (levelTypesMap == null)
		{
		  return base.getLevelType(code);
		}

		VertCoordType levelType;
		levelTypesMap.TryGetValue(code, out levelType);
		if (levelType != null)
		{
		  return levelType;
		}

		return base.getLevelType(code);
	  }


	}

}
