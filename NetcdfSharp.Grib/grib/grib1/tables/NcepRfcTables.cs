// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1.tables
{
	using GribResourceReader = NetcdfSharp.ucar.nc2.grib.GribResourceReader;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// NCEP River Forecast Centers
	/// 
	/// @author caron
	/// @since 1/13/12
	/// </summary>
	public class NcepRfcTables : NcepTables
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(NcepRfcTables));
	  private static IDictionary<int, string> nwsoSubCenter;

	  internal NcepRfcTables(Grib1ParamTables tables) : base(8, tables)
	  {
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getGeneratingProcessName(int genProcess)
	  public override string getGeneratingProcessName(int genProcess)
	  {
		switch (genProcess)
		{
		  case 150:
			return "NWS River Forecast System (NWSRFS)";
		  case 151:
			return "NWS Flash Flood Guidance System (NWSFFGS)";
		  case 152:
			return "Quantitative Precipitation Estimation (QPE) - 1 hr dur";
		  case 154:
			return "Quantitative Precipitation Estimation (QPE) - 6 hr dur";
		  case 155:
			return "Quantitative Precipitation Estimation (QPE) - 24hr dur";
		  case 156:
			return "Process 1 (P1) Precipitation Estimation - automatic";
		  case 157:
			return "Process 1 (P1) Precipitation Estimation - manual";
		  case 158:
			return "Process 2 (P2) Precipitation Estimation - automatic";
		  case 159:
			return "Process 2 (P2) Precipitation Estimation - manual";
		  case 160:
			return "Multisensor Precipitation Estimation (MPE) - automatic";
		  case 161:
			return "Multisensor Precipitation Estimation (MPE) - manual";
		  case 165:
			return "Enhanced MPE - automatic";
		  case 166:
			return "Bias Enhanced MPE - automatic";
		  case 170:
			return "Post Analysis of Precipitation Estimation (aggregate)";
		  case 171:
			return "XNAV Aggregate Precipitation Estimation";
		  case 172:
			return "Mountain Mapper Precipitation Estimation";
		  case 180:
			return "Quantitative Precipitation Forecast (QPF)";
		  case 185:
			return "NOHRSC_OPPS";
		  case 190:
			return "Satellite Autoestimator Precipitation";
		  case 191:
			return "Satellite Interactive Flash Flood Analyzer (IFFA)";
		}
		return null;
	  }

	  ///////////////////////////////////////////////////////////////////////
	  /*
	   * TABLE C - SUB-CENTERS FOR CENTER 9 US NWS FIELD STATIONS
	   * from bdgparm.f John Halquist <John.Halquist@noaa.gov> 9/12/2011
	   * These are not in the WMO common tables like NCEP's are
	   */
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getSubCenterName(int subcenter)
	  public override string getSubCenterName(int subcenter)
	  {
		if (nwsoSubCenter == null)
		{
		  nwsoSubCenter = readNwsoSubCenter("resources/grib1/noaa_rfc/tableC.txt");
		}
		if (nwsoSubCenter == null)
		{
		  return null;
		}

		return nwsoSubCenter[subcenter];
	  }

	  // order: num, name, desc, unit
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable private static NetcdfSharp.java.util.Map<int, String> readNwsoSubCenter(String path)
	  private static IDictionary<int, string> readNwsoSubCenter(string path)
	  {
		IDictionary<int, string> result = new Dictionary<int, string>();

		try
		{
				using (Stream @is = GribResourceReader.getInputStream(path))
				using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
				{
			  while (true)
			  {
				string line = br.ReadLine();
				if (string.ReferenceEquals(line, null))
				{
				  break;
				}
				if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
				{
				  continue;
				}

				StringBuilder lineb = new StringBuilder(line);
				StringUtil2.removeAll(lineb, "'+,/");
				string[] flds = lineb.ToString().Split(':', StringSplitOptions.None);

				int val = int.Parse(flds[0].Trim()); // must have a number
				string name = flds[1].Trim() + ": " + flds[2].Trim();

				result[val] = name;
			  }
			  return Collections.unmodifiableMap(result); // all at once - thread safe

				}
		}
		catch (IOException ioError)
		{
		  logger.warn("An error occurred in Grib1Tables while trying to open the table " + path + " : " + ioError);
		  return null;
		}
	  }
	}

}
