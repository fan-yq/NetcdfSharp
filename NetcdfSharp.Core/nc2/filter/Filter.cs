// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 2021 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.filter
{

	/// <summary>
	/// A class implementing a conversion to be applied on large chunk of data
	/// </summary>
	public abstract class Filter
	{

	  public abstract string Name {get;}

	  public abstract int Id {get;}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public abstract byte[] encode(byte[] dataIn) throws NetcdfSharp.java.io.IOException;
	  public abstract byte[] encode(byte[] dataIn);

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public abstract byte[] decode(byte[] dataIn) throws NetcdfSharp.java.io.IOException;
	  public abstract byte[] decode(byte[] dataIn);

	  public override string ToString()
	  {
		Formatter f = new Formatter();
		return f.format("Name: %s, ID: %d", Name, Id).ToString();
	  }
	}

}
