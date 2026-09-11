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
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using Grib1ParamLevel = NetcdfSharp.ucar.nc2.grib.grib1.Grib1ParamLevel;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;

	/// <summary>
	/// Air Force Weather Center (center 57)
	/// 
	/// @author caron
	/// @since 7/31/13
	/// </summary>
	public class AfwaTables : Grib1Customizer
	{
	  private static IDictionary<int, VertCoordType> levelTypesMap; // shared by all instances
	  private static IDictionary<int, string> genProcessMap; // shared by all instances
	  private static IDictionary<int, string> subcenterMap; // shared by all instances

	  internal AfwaTables(Grib1ParamTables tables) : base(57, tables)
	  {
	  }

	  // subcenters

	  public override string getSubCenterName(int subcenter)
	  {
		if (subcenterMap == null)
		{
		  subcenterMap = makeSubcenterMap();
		}
		return subcenterMap[subcenter];
	  }

	  private static IDictionary<int, string> makeSubcenterMap()
	  {
		IDictionary<int, string> result = new Dictionary<int, string>(100);

		result[0] = "AFWA Primary Table";
		result[1] = "AFWA Numerical Weather Prediction Models Products";
		result[2] = "AFWA Special Environmental Models Products";
		result[3] = "AFWA Space Products";
		result[4] = "AFWA Ensemble Products";
		result[6] = "AFWA Aerosol Products";
		result[7] = "CDFS-II Aerosol Model";
		result[8] = "AFWA Volcano Products";
		result[10] = "14 WS (Formerly Air Force Combat Climatology Center (AFCCC))";
		result[11] = "14 WS Numerical Weather Prediction Models Products";
		result[15] = "15 OWS (Scott)";
		result[17] = "17 OWS (Hickam)";
		result[20] = "20 OWS (Yokota)";
		result[21] = "21 OWS (Sembach)";
		result[25] = "25 OWS (Davis-Monthan)";
		result[26] = "26 OWS (Barksdale)";
		result[27] = "27 OWS (Offutt)";
		result[28] = "28 OWS (Shaw)";
		result[29] = "Air Force Tactical Application Center (AFTAC) ";
		result[90] = "AFWA 4DF 1";
		result[91] = "AFWA 4DF 2";
		result[92] = "AFWA 4DF 3";
		result[93] = "AFWA 4DF 4";
		result[94] = "AFWA 4DF 5";
		result[110] = "Air Force Research Lab (AFRL) Office of Research and Applications";
		result[130] = "Patrick AFB OWS Eastern Test Range";
		result[150] = "IPDS Satellite";

		return Collections.unmodifiableMap(result); // all at once - thread safe
	  }

	  // gen process
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getGeneratingProcessName(int genProcess)
	  public override string getGeneratingProcessName(int genProcess)
	  {
		if (genProcessMap == null)
		{
		  makeGenProcessMap();
		}
		return genProcessMap[genProcess];
	  }

	  private static void makeGenProcessMap()
	  {
		genProcessMap = new Dictionary<int, string>(100);
		genProcessMap[10] = "Mesoscale Model 5 (MM5)";
		genProcessMap[11] = "Weather Research and Forecasting Model (WRF)";
		genProcessMap[12] = "Multivariate Optimum Interpolation Model (MVOI)";
		genProcessMap[13] = "Three Dimensional Variate Model (3DVAR)";
		genProcessMap[14] = "Weather Research and Forecasting Model Chemical & Aerosol (WRF-CHEM)";
		genProcessMap[25] = "Snow Depth Model (SNODEP) (Manually modified)";
		genProcessMap[26] = "Real Time Nephanalysis Model (RTNEPH)";
		genProcessMap[27] = "Surface Temperature Model (SFCTMP)";
		genProcessMap[28] = "Advect Cloud Model (ADVCLD)";
		genProcessMap[29] = "Worldwide Merged Cloud Analysis (WWMCA) (Manually modified)";
		genProcessMap[30] = "Short Range Cloud Forecast (SRCF)";
		genProcessMap[31] = "Long Range Cloud Forecast (LRCF)";
		genProcessMap[32] = "Worldwide Merged Cloud Analysis (WWMCA) (Not modified)";
		genProcessMap[33] = "Diagnostic Cloud Forecast v. 3 (DCF3) 3-layer";
		genProcessMap[34] = "Stochastic Cloud Forecast Model (SCFM)";
		genProcessMap[35] = "Snow Depth Model (SNODEP) (Not modified)";
		genProcessMap[36] = "Diagnostic Cloud Forecast v. 3 (DCF3) 5-layer";
		genProcessMap[39] = "Diagnostic Cloud Forecast v. 3 (DCF3) P-layer";
		genProcessMap[40] = "Diagnostic Cloud Forecast v. 4 (DCF4)";
		genProcessMap[50] = "Dust Transport Application (DTA)";
		genProcessMap[55] = "Aerosol Model (CDFS-II)";
		genProcessMap[75] = "Advanced Climate Modeling and Environmental Simulations (ACMES)";
		genProcessMap[86] = "Geostationary satellite-based precipitation model (GEO_PRECIP)";
		genProcessMap[87] = "Agricultural Meteorology (AGRMET)";
		genProcessMap[88] = "Land Information System (LIS)";
		genProcessMap[96] = "Array-flipped (to NOGAPS grid structure) Global Forecast System (GFS)";
		genProcessMap[99] = "Ensemble Post-Processor";
		genProcessMap[100] = "Ionospheric Forecast Model (IFM)";
		genProcessMap[101] = "Parameterized Real-Time Specification Model (PRISM)";
		genProcessMap[125] = "Snow Depth Climatology";
		genProcessMap[127] = "Unified Model (UM)";
		genProcessMap[200] = "NOGAPS post-processed by AFWA";
		genProcessMap[201] = "GFS post processed by AFWA";
		genProcessMap[204] = "FNMOC NOGAPS geographically subsected by AFWA";
		genProcessMap[250] = "Quality Control Display Process (SCIF only)";
	  }

	  /// levels
	  public override Grib1ParamLevel getParamLevel(Grib1SectionProductDefinition pds)
	  {
		int levelType = pds.LevelType;
		int pds11 = pds.LevelValue1;
		int pds12 = pds.LevelValue2;
		int pds1112 = pds11 << 8 | pds12;

		switch (levelType)
		{
		  case 210:
		  case 246:
			return new Grib1ParamLevel(this, levelType, (float) pds1112, GribNumbers.MISSING);

		  case 218:
			return new Grib1ParamLevel(this, levelType, (float) pds11 + 200, (float) pds12 + 200);

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
		levelTypesMap = new Dictionary<int, VertCoordType>(100);
		// (int code, String desc, String abbrev, String units, String datum, boolean isPositiveUp, boolean isLayer)
		levelTypesMap[21] = new VertCoordType(21, "RTNEPH cloud layer", "RTNEPH", "", null, true, true);
		levelTypesMap[210] = new VertCoordType(210, "Isobaric Surface", "ISBP", "", null, false, false);
		levelTypesMap[211] = new VertCoordType(211, "Boundary layer cloud bottom level", "BCBL", "", null, false, false);
		levelTypesMap[212] = new VertCoordType(212, "Boundary layer cloud top level", "BCTL", "", null, false, false);
		levelTypesMap[213] = new VertCoordType(213, "Boundary layer cloud layer", "BCY", "", null, false, true);
		levelTypesMap[214] = new VertCoordType(214, "Low cloud bottom level", "LCBL", "", null, false, false);
		levelTypesMap[215] = new VertCoordType(215, "Low cloud top level", "LCTL", "", null, false, false);
		levelTypesMap[216] = new VertCoordType(216, "Low cloud layer", "LCY", "", null, false, true);
		levelTypesMap[217] = new VertCoordType(217, "Highest tropospheric freezing level", "HTFL", "K", null, false, false);
		levelTypesMap[218] = new VertCoordType(218, "Layer between two temperature levels", "DEGY", "K", null, false, true);
		levelTypesMap[222] = new VertCoordType(222, "Middle cloud bottom level", "MCBL", "", null, false, false);
		levelTypesMap[223] = new VertCoordType(223, "Middle cloud top level", "MCTL", "", null, false, false);
		levelTypesMap[224] = new VertCoordType(224, "Middle cloud layer", "MCY", "", null, false, true);
		levelTypesMap[232] = new VertCoordType(232, "High cloud bottom level", "HCBL", "", null, false, false);
		levelTypesMap[233] = new VertCoordType(233, "High cloud top level", "HCTL", "", null, false, false);
		levelTypesMap[234] = new VertCoordType(234, "High cloud layer", "HCY", "", null, false, true);
		levelTypesMap[242] = new VertCoordType(242, "Convective cloud bottom level", "CCBL", "", null, false, false);
		levelTypesMap[243] = new VertCoordType(243, "Convective cloud top level", "CCTL", "", null, false, false);
		levelTypesMap[244] = new VertCoordType(244, "Convective cloud layer", "CCY", "", null, false, true);
		levelTypesMap[246] = new VertCoordType(246, "Specified height level above MSL", "HTIO", "km", "msl", false, false);
		levelTypesMap[251] = new VertCoordType(251, "Layer between ground and 850 hPa level", "PTLR", "", null, false, true);
		levelTypesMap[252] = new VertCoordType(252, "Layer between lowest soil layer (layer 112) and 800cm", "SBLR", "", null, false, true);
	  }

	}

}
