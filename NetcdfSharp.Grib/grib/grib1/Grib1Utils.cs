// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	/// <summary>
	/// static utilities for Grib-1
	/// 
	/// @author caron
	/// @since 8/30/11
	/// </summary>
	public class Grib1Utils
	{

	  public static string extractParameterCode(Grib1Record record)
	  {
		Grib1SectionProductDefinition pds = record.PDSsection;
		return pds.Center + "-" + pds.SubCenter + "-" + pds.TableVersion + "-" + pds.ParameterNumber;
	  }

	}

}
