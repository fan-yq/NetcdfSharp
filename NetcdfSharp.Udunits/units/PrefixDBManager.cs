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
	/// Provides support for managing a database of unit prefixes.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class PrefixDBManager
	{
	  private const long serialVersionUID = 1L;
	  /// <summary>
	  /// @serial
	  /// </summary>
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private static PrefixDB instance_Conflict;

	  /// <summary>
	  /// Gets the current prefix database.
	  /// </summary>
	  /// <returns> The current prefix database. </returns>
	  /// <exception cref="PrefixDBException">
	  ///         The current prefix database couldn't be created. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized PrefixDB instance() throws PrefixDBException
	  public static PrefixDB instance()
	  {
		  lock (typeof(PrefixDBManager))
		  {
			if (instance_Conflict == null)
			{
			  instance_Conflict = StandardPrefixDB.instance();
			}
			return instance_Conflict;
		  }
	  }

	  /// <summary>
	  /// Sets the current prefix database.
	  /// </summary>
	  /// <param name="instance">
	  ///        The prefix database to be made the current one. </param>
	  public static PrefixDB Instance
	  {
		  set
		  {
			  lock (typeof(PrefixDBManager))
			  {
				PrefixDBManager.instance_Conflict = value;
			  }
		  }
	  }
	}

}
