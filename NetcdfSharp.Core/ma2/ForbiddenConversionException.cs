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
	/// Thrown for an attempt to make a forbidden conversion on primitive data values,
	/// eg boolean to double.
	/// 
	/// @author caron
	/// </summary>

	public sealed class ForbiddenConversionException : Exception
	{
	  public ForbiddenConversionException()
	  {
	  }

	  public ForbiddenConversionException(string s) : base(s)
	  {
	  }
	}

}
