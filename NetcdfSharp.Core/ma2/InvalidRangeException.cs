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
namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// Thrown if an attempt is made to use an invalid
	/// Range to index an array.
	/// 
	/// @author caron
	/// </summary>

	public class InvalidRangeException : Exception
	{
	  public InvalidRangeException()
	  {
	  }

	  public InvalidRangeException(string s) : base(s)
	  {
	  }
	}

}
