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
	/// Provides support for managing a default UnitFormat.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class UnitFormatManager
	{
	  private const long serialVersionUID = 1L;
	  /// <summary>
	  /// The singleton instance of the default unit format.
	  /// 
	  /// @serial
	  /// </summary>
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private static UnitFormat instance_Conflict;

	  /// <summary>
	  /// Returns an instance of the default unit format.
	  /// </summary>
	  /// <returns> An instance of the default unit format. </returns>
	  public static UnitFormat instance()
	  {
		  lock (typeof(UnitFormatManager))
		  {
			if (instance_Conflict == null)
			{
			  instance_Conflict = StandardUnitFormat.instance();
			}
			return instance_Conflict;
		  }
	  }

	  /// <summary>
	  /// Sets the instance of the default unit format. You'd better know what
	  /// you're doing if you call this method.
	  /// </summary>
	  /// <param name="instance"> An instance of the new, default unit format. </param>
	  public static UnitFormat Instance
	  {
		  set
		  {
			  lock (typeof(UnitFormatManager))
			  {
				UnitFormatManager.instance_Conflict = value;
			  }
		  }
	  }
	}

}
