// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.util
{
	/// <summary>
	/// Interface for global debug flags.
	/// Allows decoupling of packages.
	/// 
	/// TODO will move in ver 6
	/// </summary>
	public interface DebugFlags
	{

	  /// <summary>
	  /// Test if named debug flag is set.
	  /// </summary>
	  /// <param name="flagName"> name of flag </param>
	  /// <returns> true if named flag is set true </returns>
	  bool isSet(string flagName);

	  /// <summary>
	  /// Set named debug flag.
	  /// </summary>
	  /// <param name="flagName"> set this flag </param>
	  /// <param name="value"> set to this value </param>
	  void set(string flagName, bool value);

	  /// <summary>
	  /// Return the string representing the current debug flag(s) set. Flags can be either
	  /// or false - they just need to be set.
	  /// </summary>
	  string SetFlags {get;}

	}

}
