// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf4
{
	using Group = NetcdfSharp.ucar.nc2.Group;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Interface for HDF5 and HDF4 headers, needed by HdfEos. </summary>
	public interface HdfHeaderIF
	{

	  Group.Builder RootGroup {get;}

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: void makeVinfoForDimensionMapVariable(NetcdfSharp.ucar.nc2.Group.Builder parent, NetcdfSharp.ucar.nc2.Variable.Builder<?> v);
	  void makeVinfoForDimensionMapVariable(Group.Builder parent, Variable.Builder v);

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: String readStructMetadata(NetcdfSharp.ucar.nc2.Variable.Builder<?> structMetadataVar) throws NetcdfSharp.java.io.IOException;
	  string readStructMetadata(Variable.Builder structMetadataVar);

	}

}
