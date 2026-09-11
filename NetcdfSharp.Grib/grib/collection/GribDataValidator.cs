// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using SubsetParams = NetcdfSharp.ucar.nc2.ft2.coverage.SubsetParams;
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// For debugging
	/// </summary>
	public interface GribDataValidator
	{
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void validate(NetcdfSharp.ucar.nc2.grib.GribTables cust, NetcdfSharp.ucar.unidata.io.RandomAccessFile rafData, long pos, NetcdfSharp.ucar.nc2.ft2.coverage.SubsetParams coords) throws NetcdfSharp.java.io.IOException;
	  void validate(GribTables cust, RandomAccessFile rafData, long pos, SubsetParams coords);
	}

}
