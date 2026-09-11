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
	/// Provides support for managing a UnitSystem.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class UnitSystemManager
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The singleton instance of the system of units.
	  /// 
	  /// @serial
	  /// </summary>
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private static UnitSystem instance_Conflict;

	  /// <summary>
	  /// Returns an instance of the system of units.
	  /// </summary>
	  /// <returns> An instance of the system of units. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized UnitSystem instance() throws UnitSystemException
	  public static UnitSystem instance()
	  {
		  lock (typeof(UnitSystemManager))
		  {
			if (instance_Conflict == null)
			{
			  instance_Conflict = SI.instance();
			}
			return instance_Conflict;
		  }
	  }

	  /// <summary>
	  /// Sets the system of units. This must be called before any call to
	  /// <code>instance()</code>.
	  /// </summary>
	  /// <param name="instance">
	  ///        The system of units. </param>
	  /// <exception cref="UnitSystemException">
	  ///         <code>instance()</code> was called earlier. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized void setInstance(final UnitSystem instance) throws UnitSystemException
	  public static UnitSystem Instance
	  {
		  set
		  {
			  lock (typeof(UnitSystemManager))
			  {
				if (UnitSystemManager.instance_Conflict != null)
				{
				  throw new UnitSystemException("Unit system already used");
				}
				UnitSystemManager.instance_Conflict = value;
			  }
		  }
	  }
	}

}
