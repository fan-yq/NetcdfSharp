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
	/// Provides support for a base quantity that is dimensionfull.
	/// 
	/// Instances of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class RegularBaseQuantity : BaseQuantity
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from a name and symbol.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the base unit. </param>
	  /// <param name="symbol">
	  ///        The symbol of the base unit. </param>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public RegularBaseQuantity(final String name, final String symbol) throws NameException
	  public RegularBaseQuantity(in string name, in string symbol) : base(name, symbol)
	  {
	  }

	  /// <summary>
	  /// Constructs from a name and a symbol. This is a trusted constructor for
	  /// use by the parent class only.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the base unit. </param>
	  /// <param name="symbol">
	  ///        The symbol of the base unit. </param>
	  protected internal RegularBaseQuantity(in string name, in string symbol, in bool trusted) : base(name, symbol, trusted)
	  {
	  }

	  /// <summary>
	  /// Indicates if this base quantity is dimensionless. Regular base quantities
	  /// are always dimensionfull.
	  /// </summary>
	  /// <returns> <code>false</code>. </returns>
	  public override bool Dimensionless
	  {
		  get
		  {
			return false;
		  }
	  }
	}

}
