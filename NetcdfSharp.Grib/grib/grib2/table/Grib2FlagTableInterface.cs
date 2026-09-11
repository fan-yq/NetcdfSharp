// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{

	public interface Grib2FlagTableInterface
	{
	  string Name {get;}

	  string ShortName {get;}

	  ImmutableList<Entry> Entries {get;}

	  /// <summary>
	  /// Find the Entry in this table with the given code.
	  /// </summary>
	  /// <param name="code"> unsigned short. </param>
	  Grib2FlagTableInterface.Entry getEntry(int code);

	  public interface Entry
	  {
		/// <summary>
		/// Unsigned short </summary>
		int Code {get;}

		ImmutableList<int> Values {get;}

		string getName(int value);
	  }
	}

}
