// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using NetcdfSharp.ucar.nc2.grib;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using NcepTables = NetcdfSharp.ucar.nc2.grib.grib1.tables.NcepTables;
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;
	using Parameter = NetcdfSharp.ucar.nc2.grib.GribTables.Parameter;

	/// <summary>
	/// NCEP local tables.
	/// 
	/// @author caron
	/// @since 4/3/11
	/// </summary>
	public class NcepLocalTables : LocalTables      
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(NcepLocalTables));

	  protected internal readonly NcepLocalParams ncepLocalParams;
	  private readonly IDictionary<string, string> codeMap = new Dictionary<string, string>(400);

	  internal NcepLocalTables(Grib2TableConfig config) : base(config)
	  {
		this.ncepLocalParams = new NcepLocalParams(config.Path);
		initCodes();
	  }

	  public override string getParamTablePathUsedFor(int discipline, int category, int number)
	  {
		if ((category <= 191) && (number <= 191))
		{
		  return base.getParamTablePathUsedFor(discipline, category, number);
		}
		return ncepLocalParams.getTablePath(discipline, category);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private String[] getResourceListing(String path) throws URISyntaxException, NetcdfSharp.java.io.IOException
	  private string[] getResourceListing(string path)
	  {
		string dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Replace('/', System.IO.Path.DirectorySeparatorChar));
		if (Directory.Exists(dirPath))
		{
		  return Directory.GetFiles(dirPath).Select(p => System.IO.Path.GetFileName(p)).ToArray();
		}
		return null;
	  }

	  public override ImmutableList<Parameter> Parameters
	  {
		  get
		  {
			ImmutableList<GribTables.Parameter>.Builder allParams = ImmutableList<GribTables.Parameter>.builder();
			try
			{
			  string[] fileNames = getResourceListing(config.Path);
			  if (fileNames == null)
			  {
				return ImmutableList<GribTables.Parameter>.of();
			  }
			  foreach (string fileName in fileNames)
			  {
				File f = new File(fileName);
				if (f.isDirectory())
				{
				  continue;
				}
				if (!f.getName().Contains("Table4.2."))
				{
				  continue;
				}
				if (!f.getName().EndsWith(".xml"))
				{
				  continue;
				}
				try
				{
				  IList<GribTables.Parameter> @params = ncepLocalParams.getParameters(config.Path + f.getPath());
				  if (@params != null)
				  {
					allParams.addAll(@params);
				  }
				}
				catch (Exception e)
				{
				  logger.error("Error reading wmo tables", e);
				}
			  }
			}
			catch (IOException e)
			{
			  logger.error("NcepLocalTables failed", e);
			}
			return allParams.build();
		  }
	  }

	  public override string getVariableName(int discipline, int category, int parameter)
	  {
		if ((category <= 191) && (parameter <= 191))
		{
		  return base.getVariableName(discipline, category, parameter);
		}

		GribTables.Parameter te = getParameter(discipline, category, parameter);
		if (te == null)
		{
		  return base.getVariableName(discipline, category, parameter);
		}
		else
		{
		  return te.Name;
		}
	  }

	  public override GribTables.Parameter getParameter(int discipline, int category, int number)
	  {
		/*
		 * email from boi.vuong@noaa.gov 1/19/2012
		 * "I find that the parameter 2-4-3 (Haines Index) now is parameter 2 in WMO version 8.
		 * The NAM fire weather nested will take change in next implementation of cnvgrib (NCEP conversion program)."
		 */
		// if (makeHash(discipline, category, number) == makeHash(2,4,3))
		// return getParameter(2,4,2);

		/*
		 * email from boi.vuong@noaa.gov 1/26/2012
		 * The parameter 0-19-242 (Relative Humidity with Respect to Precipitable Water) was in
		 * http://www.nco.ncep.noaa.gov/pmb/docs/grib2/grib2_table4-2-0-1.shtml
		 * It was a mistake in table conversion (from grib1 to grib2) in cnvgrib. It will be fixed in next implementation of
		 * cnvgrib in June or July, 2012.
		 * RHPW in grib1 in table 129 parameter 230 and in grib2 in 0-1-242
		 */
		// if (makeHash(discipline, category, number) == makeHash(0, 19, 242))
		// return getParameter(0, 1, 242);

		Grib2Parameter plocal = ncepLocalParams.getParameter(discipline, category, number);

		if ((category <= 191) && (number <= 191))
		{
		  GribTables.Parameter pwmo = WmoParamTable.getParameter(discipline, category, number);
		  if (plocal == null)
		  {
			return pwmo;
		  }

		  // allow local table to override all but name, units
		  if (pwmo != null)
		  {
			return new Grib2Parameter(plocal, pwmo.Name, pwmo.Unit);
		  }
		}

		return plocal;
	  }

	  public override GribTables.Parameter getParameterRaw(int discipline, int category, int number)
	  {
		return ncepLocalParams.getParameter(discipline, category, number);
	  }

	  public override string getCodeTableValue(string tableName, int code)
	  {
		if (tableName.Equals("ProcessId"))
		{
		  return getGeneratingProcessName(code);
		}

		if ((code < 192) || (code > 254) || tableName.Equals("4.0"))
		{
		  return base.getCodeTableValue(tableName, code);
		}

		return codeMap[tableName + "." + code];
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Vert

	  public override VertCoordType getVertUnit(int code)
	  {

		switch (code)
		{
		  case 235:
			return new VertCoordType(code, "0.1 C", null, true);

		  case 237:
		  case 238:
			return new VertCoordType(code, "m", null, true);

		  case 241:
			return new VertCoordType(code, "count", null, true); // eg see NCEP World Watch datasets

		  default:
			return base.getVertUnit(code);
		}
	  }

	  public override string getLevelNameShort(int id)
	  {
		switch (id)
		{
		  case 200:
			return "entire_atmosphere_single_layer";
		  case 201:
			return "entire_ocean_single_layer";
		  case 204:
			return "highest_tropospheric_freezing";
		  case 206:
			return "grid_scale_cloud_bottom";
		  case 207:
			return "grid_scale_cloud_top";
		  case 209:
			return "boundary_layer_cloud_bottom";
		  case 210:
			return "boundary_layer_cloud_top";
		  case 211:
			return "boundary_layer_cloud";
		  case 212:
			return "low_cloud_bottom";
		  case 213:
			return "low_cloud_top";
		  case 214:
			return "low_cloud";
		  case 215:
			return "cloud_ceiling";
		  case 220:
			return "planetary_boundary";
		  case 221:
			return "between_two_hybrids";
		  case 222:
			return "middle_cloud_bottom";
		  case 223:
			return "middle_cloud_top";
		  case 224:
			return "middle_cloud";
		  case 232:
			return "high_cloud_bottom";
		  case 233:
			return "high_cloud_top";
		  case 234:
			return "high_cloud";
		  case 235:
			return "ocean_isotherm";
		  case 236:
			return "layer_between_two_depths_below_ocean";
		  case 237:
			return "bottom_of_ocean_mixed";
		  case 238:
			return "bottom_of_ocean_isothermal";
		  case 239:
			return "ocean_surface_and_26C_isothermal";
		  case 240:
			return "ocean_mixed";
		  case 241:
			return "ordered_sequence_of_data";
		  case 242:
			return "convective_cloud_bottom";
		  case 243:
			return "convective_cloud_top";
		  case 244:
			return "convective_cloud";
		  case 245:
			return "lowest_level_of_the_wet_bulb_zero";
		  case 246:
			return "maximum_equivalent_potential_temperature";
		  case 247:
			return "equilibrium";
		  case 248:
			return "shallow_convective_cloud_bottom";
		  case 249:
			return "shallow_convective_cloud_top";
		  case 251:
			return "deep_convective_cloud_bottom";
		  case 252:
			return "deep_convective_cloud_top";
		  case 253:
			return "lowest_level_water_layer";
		  case 254:
			return "highest_level_water_layer";
		  default:
			return base.getLevelNameShort(id);
		}
	  }

	  //////////////////////////////////////////////////////////////

	  public override string getStatisticNameShort(int id)
	  {
		switch (id)
		{
		  case 192:
			return "ClimatologicalMeanValue";
		  case 193:
			return "AverageNforecasts";
		  case 194:
			return "AverageNanalysis";
		  case 195:
			// Average of forecast accumulations. P1 = start of accumulation period. P2 = end of accumulation period.
			// Reference time is the start time of the
			// first forecast, other forecasts at 24-hour intervals. Number in Ave = number of forecasts used.
			return "AverageAccum-24hourIntv";
		  case 196:
			return "AverageForecastSuccessiveAccumulations";
		  case 197:
			// Average of forecast averages. P1 = start of averaging period. P2 = end of averaging period. Reference time is
			// the start time of the first forecast,
			// other forecasts at 24-hour intervals. Number in Ave = number of forecast used
			return "AverageAvg-24hourIntv";
		  case 198:
			return "AverageForecastSuccessiveAverages";
		  case 199:
			return "ClimatologicalAverageNanalysis";
		  case 200:
			return "ClimatologicalAverageNforecasts";
		  case 201:
			return "ClimatologicalRMSdiffNforecasts";
		  case 202:
			return "ClimatologicalStandardDeviationNforecasts";
		  case 203:
			return "ClimatologicalStandardDeviationNanalyses";
		  case 204:
			return "AverageForecastAccumulations-204";
		  case 205:
			// Average of forecast averages. P1 = start of averaging period. P2 = end of averaging period. Reference time is
			// the start time of the
			// first forecast, other forecasts at 6-hour intervals. Number in Ave = number of forecast used
			return "AverageAvg-6hourIntv";
		  case 206:
			return "AverageForecastAccumulations-206";
		  case 207:
			return "AverageForecastAverages-207";
		  case 255:
			return "Interval";
		  default:
			return base.getStatisticNameShort(id);
		}
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public GribStatType getStatType(int id)
	  public override GribStatType getStatType(int id)
	  {
		if (id < 192)
		{
		  return base.getStatType(id);
		}
		switch (id)
		{ // LOOK not correct
		  case 192:
		  case 193:
		  case 194:
		  case 195:
		  case 196:
		  case 197:
		  case 198:
		  case 199:
		  case 200:
		  case 204:
		  case 205:
		  case 206:
		  case 207:
			return GribStatType.Average;
		  case 201:
			return GribStatType.RootMeanSquare;
		  case 202:
		  case 203:
			return GribStatType.StandardDeviation;
		}

		return null;
	  }

	  private static IDictionary<int, string> statName; // shared by all instances

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getStatisticName(int id)
	  public override string getStatisticName(int id)
	  {
		if (id < 192)
		{
		  return base.getStatisticName(id);
		}
		if (statName == null)
		{
		  statName = initTable410();
		}
		if (statName == null)
		{
		  return null;
		}
		return statName[id];
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable private Map<Integer, String> initTable410()
	  private IDictionary<int, string> initTable410()
	  {
		string path = config.Path + "Table4.10.xml";
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
			  return result; // all at once - thread safe
			}
		}
		catch (Exception ioe) when (ioe is IOException || ioe is JDOMException)
		{
		  logger.error("Cant read  " + path, ioe);
		  return null;
		}
	  }

	  /////////////////////////////////////////////////////////////////
	  // generating process ids for NCEP
	  // GRIB1 TableA - can share (?)
	  private static IDictionary<int, string> genProcessMap; // shared by all instances

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getGeneratingProcessName(int genProcess)
	  public override string getGeneratingProcessName(int genProcess)
	  {
		if (genProcessMap == null)
		{
		  genProcessMap = NcepTables.NcepGenProcess;
		}
		if (genProcessMap == null)
		{
		  return null;
		}
		return genProcessMap[genProcess];
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getCategory(int discipline, int category)
	  public override string getCategory(int discipline, int category)
	  {
		string catName = ncepLocalParams.getCategory(discipline, category);
		if (catName != null)
		{
		  return catName;
		}
		return base.getCategory(discipline, category);
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////////
	  // NCEP local tables taken from degrib code
	  // validated against /resources/grib2/tablesOld/grib2StdQuantities.xml

	  /*
	   * from degrib:
	   * Updated this table last on 12/29/2005
	   * Based on:
	   * http://www.nco.ncep.noaa.gov/pmb/docs/grib2/GRIB2_parmeter_conversion_table.html
	   * Better source is:
	   * http://www.nco.ncep.noaa.gov/pmb/docs/grib2/grib2_table4-1.shtml
	   * http://www.nco.ncep.noaa.gov/pmb/docs/grib2/grib2_table4-2.shtml
	   * For abreviations see:
	   * http://www.nco.ncep.noaa.gov/pmb/docs/on388/table2.html
	   *
	   * Updated again on 2/14/2006
	   * Updated again on 3/15/2006
	   * Updated again on 3/26/2008
	   */

	  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	  // Code table overrides.

	  // see http://www.nco.ncep.noaa.gov/pmb/docs/grib2/grib2_doc.shtml
	  private IDictionary<string, string> initCodes()
	  {
		codeMap["3.1.204"] = "Curvilinear_Orthogonal";

		codeMap["4.3.192"] = "Forecast Confidence Indicator";
		codeMap["4.3.193"] = "Bias Corrected Ensemble Forecast";

		codeMap["4.5.200"] = "Entire atmosphere layer";
		codeMap["4.5.201"] = "Entire ocean layer";
		codeMap["4.5.204"] = "Highest tropospheric freezing level";
		codeMap["4.5.206"] = "Grid scale cloud bottom level";
		codeMap["4.5.207"] = "Grid scale cloud top level";
		codeMap["4.5.209"] = "Boundary layer cloud bottom level";
		codeMap["4.5.210"] = "Boundary layer cloud top level";
		codeMap["4.5.211"] = "Boundary layer cloud layer";
		codeMap["4.5.212"] = "Low cloud bottom level";
		codeMap["4.5.213"] = "Low cloud top level";
		codeMap["4.5.214"] = "Low cloud layer";
		codeMap["4.5.215"] = "Cloud ceiling";
		codeMap["4.5.220"] = "Planetary Boundary Layer";
		codeMap["4.5.221"] = "Layer Between Two Hybrid Levels";
		codeMap["4.5.222"] = "Middle cloud bottom level";
		codeMap["4.5.223"] = "Middle cloud top level";
		codeMap["4.5.224"] = "Middle cloud layer";
		codeMap["4.5.232"] = "High cloud bottom level";
		codeMap["4.5.233"] = "High cloud top level";
		codeMap["4.5.234"] = "High cloud layer";
		codeMap["4.5.235"] = "Ocean isotherm level";
		codeMap["4.5.236"] = "Layer between two depths below ocean surface";
		codeMap["4.5.237"] = "Bottom of ocean mixed layer";
		codeMap["4.5.238"] = "Bottom of ocean isothermal layer";
		codeMap["4.5.239"] = "Layer Ocean Surface and 26C Ocean Isothermal Level";
		codeMap["4.5.240"] = "Ocean Mixed Layer";
		codeMap["4.5.241"] = "Ordered Sequence of Data";
		codeMap["4.5.242"] = "Convective cloud bottom level";
		codeMap["4.5.243"] = "Convective cloud top level";
		codeMap["4.5.244"] = "Convective cloud layer";
		codeMap["4.5.245"] = "Lowest level of the wet bulb zero";
		codeMap["4.5.246"] = "Maximum equivalent potential temperature level";
		codeMap["4.5.247"] = "Equilibrium level";
		codeMap["4.5.248"] = "Shallow convective cloud bottom level";
		codeMap["4.5.249"] = "Shallow convective cloud top level";
		codeMap["4.5.251"] = "Deep convective cloud bottom level";
		codeMap["4.5.252"] = "Deep convective cloud top level";
		codeMap["4.5.253"] = "Lowest bottom level of supercooled liquid water layer";
		codeMap["4.5.254"] = "Highest top level of supercooled liquid water layer";

		codeMap["4.6.192"] = "Perturbed Ensemble Member";

		codeMap["4.7.192"] = "Unweighted Mode of All Members";
		codeMap["4.7.193"] = "Percentile value (10%) of All Members";
		codeMap["4.7.194"] = "Percentile value (50%) of All Members";
		codeMap["4.7.195"] = "Percentile value (90%) of All Members";

		codeMap["4.10.192"] = "Climatological Mean Value";
		codeMap["4.10.193"] = "Average of N forecasts";
		codeMap["4.10.194"] = "Average of N uninitialized analyses";
		codeMap["4.10.195"] = "Average of forecast accumulations";
		codeMap["4.10.196"] = "Average of successive forecast accumulations";
		codeMap["4.10.197"] = "Average of forecast averages";
		codeMap["4.10.198"] = "Average of successive forecast averages";
		codeMap["4.10.199"] = "Climatological Average of N analyses, each a year apart";
		codeMap["4.10.200"] = "Climatological Average of N forecasts, each a year apart";
		codeMap["4.10.201"] = "Climatological Root Mean Square difference between N forecasts and their verifying analyses, each a year apart";
		codeMap["4.10.202"] = "Climatological Standard Deviation of N forecasts from the mean of the same N forecasts, for forecasts one year apart";
		codeMap["4.10.203"] = "Climatological Standard Deviation of N analyses from the mean of the same N analyses, for analyses one year apart";
		codeMap["4.10.204"] = "Average of forecast accumulations";
		codeMap["4.10.205"] = "Average of forecast averages";
		codeMap["4.10.206"] = "Average of forecast accumulations";
		codeMap["4.10.207"] = "Average of forecast averages";

		return codeMap;
	  }

	}

}
