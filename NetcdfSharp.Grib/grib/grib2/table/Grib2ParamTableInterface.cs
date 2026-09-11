// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;

	public interface Grib2ParamTableInterface
	{
	  string Name {get;}

	  string ShortName {get;}

	  ImmutableList<GribTables.Parameter> Parameters {get;}

	  /// <summary>
	  /// Find the Parameter in this table with the given number.
	  /// </summary>
	  /// <param name="number"> unsigned byte. </param>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable GribTables.Parameter getParameter(int number);
	  GribTables.Parameter getParameter(int number);
	}

}
