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
	/// Provides support for supplementary base quantities. A supplementary base
	/// quantity is one that is dimensionless (e.g. solid angle).
	/// 
	/// Instances of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class SupplementaryBaseQuantity : BaseQuantity
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// Constructs from a name and symbol.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the quantity. </param>
	  /// <param name="symbol">
	  ///        The symbol for the quantity. </param>
	  /// <exception cref="NameException">
	  ///         Bad quantity name. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public SupplementaryBaseQuantity(final String name, final String symbol) throws NameException
	  public SupplementaryBaseQuantity(in string name, in string symbol) : base(name, symbol)
	  {
	  }

	  /// <summary>
	  /// Constructs from a name and symbol. This is a trusted constructor for use
	  /// by the superclass only.
	  /// </summary>
	  /// <param name="name">
	  ///        The name of the quantity. </param>
	  /// <param name="symbol">
	  ///        The symbol for the quantity. </param>
	  protected internal SupplementaryBaseQuantity(in string name, in string symbol, in bool trusted) : base(name, symbol, trusted)
	  {
	  }

	  /// <summary>
	  /// Indicates whether or not this quantity is dimensionless. Supplementary
	  /// base quantities are dimensionless by definition.
	  /// </summary>
	  /// <returns> <code>true</code>. </returns>
	  public override bool Dimensionless
	  {
		  get
		  {
			/*
			 * These quantities are dimensionless by definition.
			 */
			return true;
		  }
	  }
	}

}
