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



namespace NetcdfSharp.ucar.unidata.io.bzip2
{

	/// <summary>
	/// Exception for BZip problems
	/// 
	/// @author IDV Development Team
	/// @version $Revision: 1.1 $
	/// </summary>
	public class BZip2ReadException : Exception
	{

	  /// <summary>
	  /// Create a new BZip2ReadException with no message
	  /// </summary>
	  public BZip2ReadException()
	  {
	  }

	  /// <summary>
	  /// Create a new BZip2ReadException with the message
	  /// </summary>
	  /// <param name="message"> detailed message associated with this exception </param>
	  public BZip2ReadException(string message) : base(message)
	  {
	  }

	}


}
