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

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using GribResourceReader = NetcdfSharp.ucar.nc2.grib.GribResourceReader;
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;
	using TableParser = NetcdfSharp.ucar.nc2.util.TableParser;

	/// <summary>
	/// Read tables from https://github.com/Unidata/gempak/tree/master/gempak/tables/grid/g2*.tbl
	/// 
	/// eg:
	/// https://raw.githubusercontent.com/Unidata/gempak/master/gempak/tables/grid/g2varsfsl1.tbl
	/// 
	/// LOOK: not used in standardTableMap.txt, so cannot be found in any search in Grib2Tables.
	/// 
	/// @author caron
	/// @since 9/14/2014
	/// </summary>
	public class GempakLocalTables : LocalTables
	{
	  private static bool debug;

	  internal GempakLocalTables(Grib2TableConfig config) : base(config)
	  {
		Formatter f = new Formatter();
		localParams = initLocalTable(config.Path, f);
	  }

	  /*
	   * ! GRIB2 parameter table
	   * !
	   * !D# = Discipline number
	   * !CT# = Category number (Octet 10, Code Table 4.2)
	   * !ID# = Parameter number (Octet 11)
	   * !PD# = Product Definition Template number (Octet 8-9, Code Table 4.0)
	   * !
	   * ! temperature
	   * !D# CT# ID# PD# NAME UNITS GNAM SCALE MISSING HZREMAP DIRECTION
	   * !23|123|123|123|12345678901234567890123456789012|12345678901234567890|123456789012|12345|123456.89|12345678|
	   * 1234567890
	   * 1 2 3 4 5 6 7 8 9 10 11 12
	   * 0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
	   * 000 000 000 000 Temperature K TMPK 0 -9999.00 0 0
	   * 000 000 000 019 Temperature Below Normal % PTBN 0 -9999.00 0 0
	   * 000 000 000 029 Temperature Near Normal % PTNN 0 -9999.00 0 0
	   * 000 000 000 039 Temperature Above Normal % PTAN 0 -9999.00 0 0
	   * 000 000 001 000 Virtual Temperature K TVRK 0 -9999.00 0 0
	   * 000 000 002 000 Potential Temperature K THTA 0 -9999.00 0 0
	   * 000 000 003 000 Equivalent Potential Temp K THTE 0 -9999.00 0 0
	   * 000 000 004 008 Maximum Temperature K TMXK 0 -9999.00 0 0
	   * 000 000 005 008 Minimum Temperature K TMNK 0 -9999.00 0 0
	   * 000 000 006 000 Dew Point Temperature K DWPK 0 -9999.00 0 0
	   * 000 000 007 000 Dew Point Depression K DPDK 0 -9999.00 0 0
	   * 000 000 008 000 Lapse Rate K m**-1 LAPS 0 -9999.00 0 0
	   * 000 000 009 000 Temperature Anomaly K TMPKA 0 -9999.00 0 0
	   * 000 000 010 000 Latent Heat Net Flux W m**-2 FXLH 0 -9999.00 0 0
	   * 000 000 011 000 Sensible Heat Net Flux W m**-2 FXSH 0 -9999.00 0 0
	   * 000 000 012 000 Heat Index K HEAT 0 -9999.00 0 0
	   * 000 000 013 000 Wind Chill Factor K CHILL 0 -9999.00 0 0
	   * !000 000 014 000 Minimum Dew Point Depression K ???? 0 -9999.00 0 0
	   * !000 000 015 000 Virtual Potential Temperature K ???? 0 -9999.00 0 0
	   * !
	   * ! moisture
	   * !D# CT# ID# PD# NAME UNITS GNAM SCALE MISSING HZREMAP DIRECTION
	   * !23|123|123|123|12345678901234567890123456789012|12345678901234567890|123456789012|12345|123456.89|12345678|
	   * 1234567890
	   * 000 001 000 000 Specific Humidity kg kg**-1 SPFH 0 -9999.00 0 0
	   */

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: private NetcdfSharp.java.util.Map<int, NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter> initLocalTable(String resourcePath, @Nullable Formatter f)
	  private IDictionary<int, Grib2Parameter> initLocalTable(string resourcePath, Formatter f)
	  {
		IDictionary<int, Grib2Parameter> result = new Dictionary<int, Grib2Parameter>(100);

		try
		{
				using (Stream @is = GribResourceReader.getInputStream(resourcePath))
				{
			  if (f != null)
			  {
				f.format("%s, %-20s, %-20s, %-20s%n", "id", "name", "units", "gname");
			  }
			  TableParser parser = new TableParser("3i,7i,11i,15i,49,69,74,");
			  parser.Comment = "!";
			  IList<TableParser.Record> recs = parser.readAllRecords(@is, 50000);
			  foreach (TableParser.Record record in recs)
			  {
				int disc = ((int?) record.get(0)).Value;
				int cat = ((int?) record.get(1)).Value;
				int id = ((int?) record.get(2)).Value;
				int template = ((int?) record.get(3)).Value; // LOOK - 19, 29, 39 ???
				string name = ((string) record.get(4)).Trim();
				string units = ((string) record.get(5)).Trim();
				string gname = ((string) record.get(6)).Trim();

				string ids = disc + "-" + cat + "-" + id;
				if (f != null)
				{
				  f.format("%s == %-20s, %-20s, %-20s%n", ids, name, units, gname);
				}

				Grib2Parameter gp = new Grib2Parameter(disc, cat, id, gname, units, null, name);
				result[Grib2Tables.makeParamId(disc, cat, id)] = gp;
			  }

				}
		}
		catch (IOException ioe)
		{
		  throw new Exception(ioe.Message, ioe);
		}

		return result;
	  }

	}

}
