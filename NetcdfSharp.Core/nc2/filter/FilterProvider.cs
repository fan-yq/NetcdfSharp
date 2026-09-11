// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 2021 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.filter
{

	/// <summary>
	/// A Service Provider of <seealso cref="NetcdfSharp.ucar.nc2.filter.Filter"/>.
	/// </summary>
	public interface FilterProvider
	{

	  string Name {get;}

	  int Id {get;}

	  /// <summary>
	  /// Determine if this Provider can provide an Filter with the given name </summary>
	  bool canProvide(string name)
	  {
		return name.Equals(Name);
	  }

	  /// <summary>
	  /// Determine if this Provider can provide an Filter with the given numeric ID </summary>
	  bool canProvide(int id)
	  {
		return id == Id;
	  }

	  /// <summary>
	  /// Create a <seealso cref="NetcdfSharp.ucar.nc2.filter.Filter"/> of the correct type
	  /// </summary>
	  /// <returns> <seealso cref="NetcdfSharp.ucar.nc2.filter.Filter"/> </returns>
	  Filter create(IDictionary<string, object> properties);
	}

}
