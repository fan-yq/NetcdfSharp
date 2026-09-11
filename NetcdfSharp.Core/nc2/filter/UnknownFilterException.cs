// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 2021 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.filter
{
	public class UnknownFilterException : Exception
	{

	  private const string nameExceptionMessage = "Unknown filter: no filter found with name %s";

	  private const string idExceptionMessage = "Unknown filter: no filter found with id %d";

	  public UnknownFilterException(string name) : base(string.Format(nameExceptionMessage, name))
	  {
	  }

	  public UnknownFilterException(int id) : base(string.Format(idExceptionMessage, id))
	  {
	  }
	}

}
