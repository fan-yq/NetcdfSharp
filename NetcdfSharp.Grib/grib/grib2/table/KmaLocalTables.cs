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
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;
	using TableParser = NetcdfSharp.ucar.nc2.util.TableParser;

	/// <summary>
	/// LocalTables from KMA (Japan)
	/// 
	/// @author John
	/// @since 6/22/11
	/// </summary>
	public class KmaLocalTables : LocalTables
	{
	  internal KmaLocalTables(Grib2TableConfig config) : base(config)
	  {
		initLocalTable();
	  }

	  // see http://www.nco.ncep.noaa.gov/pmb/docs/grib2/grib2_doc.shtml
	  private void initLocalTable()
	  {
		string tablePath = config.Path;
		try
		{
				using (Stream @is = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStream(tablePath))
				{
			  if (@is == null)
			  {
				throw new System.InvalidOperationException("Cant find " + tablePath);
			  }

			  IList<TableParser.Record> recs = TableParser.readTable(@is, "41,112,124i,136i,148i,160", 1000);
			  foreach (TableParser.Record record in recs)
			  {
				string name = (string) record.get(0);
				int disc = ((int?) record.get(2)).Value;
				int cat = ((int?) record.get(3)).Value;
				int param = ((int?) record.get(4)).Value;
				string unit = (string) record.get(5);

				Grib2Parameter s = new Grib2Parameter(disc, cat, param, name, unit, null, null);
				localParams[makeParamId(disc, cat, param)] = s;
			  }

				}
		}
		catch (IOException ioe)
		{
		  throw new Exception(ioe.Message, ioe);
		}
	  }
	  /*
	   * 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17
	   * 012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
	   * U_COMPNT_OF_WIND_AFTER_TIMESTEP 56 0 65 2 128 33 0 2 2 m/s UU
	   * ###################################################################################################################
	   * #########################################################
	   * # || UM || GRIB1 || GRIB2 || || |
	   * # Parameter Name || Field||Processing|| Level|| Stash|| Table|| Parameter|| Product|| Parameter|| Parameter|| Unit
	   * || Grid |
	   * # || Code|| Code|| Type|| Item|| Version|| Number|| Disciplin|| Category|| Number|| || |
	   * # || 23(LBFC)||25(LBPROC)|| 26(LBVC)||42(LBUSER)|| SEC 1(4)|| SEC 1(9)|| SEC 0(7)|| SEC 4(10)|| SEC 4(11)|| || |
	   * # 40 || 12 || 12 || 12 || 12 || 12 || 12 || 12 || 12 || 12 || 12 || 12 |
	   * ###################################################################################################################
	   * #########################################################
	   * 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17
	   * 012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
	   * 
	   */


	}

}
