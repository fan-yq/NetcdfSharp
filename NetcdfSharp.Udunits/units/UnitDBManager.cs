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
namespace NetcdfSharp.ucar.units
{

	/// <summary>
	/// Provides support for managing a default unit database.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class UnitDBManager
	{
	  private const long serialVersionUID = 1L;
	  /// <summary>
	  /// The singleton instance of the default unit database.
	  /// 
	  /// @serial
	  /// </summary>
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private static UnitDB instance_Conflict;

	  /// <summary>
	  /// Gets the default unit database.
	  /// </summary>
	  /// <returns> The default unit database. </returns>
	  /// <exception cref="UnitDBException">
	  ///         The default unit database couldn't be created. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized UnitDB instance() throws UnitDBException
	  public static UnitDB instance()
	  {
		  lock (typeof(UnitDBManager))
		  {
			if (instance_Conflict == null)
			{
			  instance_Conflict = StandardUnitDB.instance();
			}
			return instance_Conflict;
		  }
	  }

	  /// <summary>
	  /// Sets the default unit database. You'd better know what you're doing if
	  /// you call this method!
	  /// </summary>
	  /// <param name="instance">
	  ///        The unit database to be made the default one. </param>
	  public static UnitDB Instance
	  {
		  set
		  {
			  lock (typeof(UnitDBManager))
			  {
				UnitDBManager.instance_Conflict = value;
			  }
		  }
	  }
	}

}
