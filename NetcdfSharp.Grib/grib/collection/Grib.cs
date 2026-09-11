// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.collection
{
	/// <summary>
	/// GRIB constants.
	/// 
	/// @author caron
	/// @since 2/23/2016.
	/// </summary>
	public class Grib
	{

	  public const string VARIABLE_ID_ATTNAME = "Grib_Variable_Id";
	  public const string GRIB_VALID_TIME = "GRIB forecast or observation time";
	  public const string GRIB_RUNTIME = "GRIB reference time";
	  public const string GRIB_STAT_TYPE = "Grib_Statistical_Interval_Type";

	  // do not use
	  public static bool debugRead;
	  public static bool debugGbxIndexOnly; // we are running with only ncx and gbx index files, no data
	  internal static bool debugIndexOnlyShow; // debugIndexOnly must be true; show record fetch
	  internal static bool debugIndexOnly; // we are running with only ncx index files, no data


	  public static NetcdfSharp.ucar.nc2.util.DebugFlags DebugFlags
	  {
		  set
		  {
			debugRead = value.isSet("Grib/showRead");
			debugIndexOnly = value.isSet("Grib/indexOnly");
			debugIndexOnlyShow = value.isSet("Grib/indexOnlyShow");
			debugGbxIndexOnly = value.isSet("Grib/debugGbxIndexOnly");
		  }
	  }

	  // Class, not interface, per Bloch edition 2 item 19
	  private Grib()
	  {
	  } // disable instantiation
	}

}
