// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1.tables
{
	/// <summary>
	/// NCAR (center 60) overrides
	/// 
	/// @author caron
	/// @since 8/29/13
	/// </summary>
	public class NcarTables : Grib1Customizer
	{

	  internal NcarTables(Grib1ParamTables tables) : base(60, tables)
	  {
	  }

	  // from http://rda.ucar.edu/docs/formats/grib/gribdoc/
	  public override string getSubCenterName(int subcenter)
	  {
		switch (subcenter)
		{
		  case 1:
			return "CISL/SCD/Data Support Section";
		  case 2:
			return "NCAR Command Language";
		  case 3:
			return "ESSL/MMM/WRF Model";
		  default:
			return "unknown";
		}
	  }
	}

}
