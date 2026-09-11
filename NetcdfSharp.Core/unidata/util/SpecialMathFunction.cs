// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.util
{

	/// <summary>
	/// Special Math functions.
	/// Recoded from online sources for licensing purposes.
	/// caron 6/22/2012
	/// </summary>


	public sealed class SpecialMathFunction
	{
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private static readonly double log2_Conflict = Math.Log(2);

	  /// <summary>
	  /// Get the log base 2 of a number
	  /// </summary>
	  /// <param name="x"> a double value </param>
	  /// <returns> The log<sub>2</sub> of x </returns>
	  /// <exception cref="ArithmeticException"> if (x < 0) </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static double log2(double x) throws ArithmeticException
	  public static double log2(double x)
	  {
		if (x <= 0.0)
		{
		  throw new ArithmeticException("range exception");
		}
		return Math.Log(x) / log2_Conflict;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static double atanh(double x) throws ArithmeticException
	  public static double atanh(double x)
	  {
		if ((x > 1.0) || (x < -1.0))
		{
		  throw new ArithmeticException("range exception");
		}
		return 0.5 * Math.Log((1.0 + x) / (1.0 - x));
	  }
	}



}
