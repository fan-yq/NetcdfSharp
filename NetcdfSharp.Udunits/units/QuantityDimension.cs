// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{

	/// <summary>
	/// Provides support for the dimension of a quantity. For example, the
	/// dimension of the quantity "force" is "M.L.t-2".
	/// 
	/// @author Steven R. Emmerson
	///         Immutable.
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class QuantityDimension extends Dimension
	public sealed class QuantityDimension : Dimension
	{
	  /// <summary>
	  /// Constructs from nothing. Constructs a dimensionless dimension.
	  /// </summary>
	  public QuantityDimension() : base()
	  {
	  }

	  /// <summary>
	  /// Constructs from a base quantity.
	  /// </summary>
	  /// <param name="baseQuantity"> The base quantity constituting the dimension. </param>
	  public QuantityDimension(BaseQuantity baseQuantity) : base(new Factor(baseQuantity))
	  {
	  }

	  /// <summary>
	  /// Constructs from an array of Factor-s. This is a trusted constructor
	  /// for use by subclasses only.
	  /// </summary>
	  /// <param name="factors"> The Factor-s constituting the dimension. </param>
	  protected internal QuantityDimension(Factor[] factors) : base(factors)
	  {
	  }

	  /// <summary>
	  /// Multiplies this quantity dimension by another quantity dimension.
	  /// </summary>
	  /// <param name="that"> The other quantity dimension. </param>
	  /// <returns> The product of this quantity dimension with
	  ///         the other quantity dimension. </returns>
	  public QuantityDimension multiplyBy(QuantityDimension that)
	  {
		return new QuantityDimension(mult(that));
	  }

	  /// <summary>
	  /// Divides this quantity dimension by another quantity dimension.
	  /// </summary>
	  /// <param name="that"> The quantity dimension to divide this
	  ///        quantity dimension by. </param>
	  /// <returns> The quotient of this quantity dimension and
	  ///         the other quantity dimension. </returns>
	  public QuantityDimension divideBy(QuantityDimension that)
	  {
		return multiplyBy(that.raiseTo(-1));
	  }

	  /// <summary>
	  /// Raises this quantity dimension to a power.
	  /// </summary>
	  /// <param name="power"> The power to raise this quantity dimension by. </param>
	  /// <returns> The result of raising this quantity dimension
	  ///         to the power <code>power</code>. </returns>
	  public QuantityDimension raiseTo(int power)
	  {
		return new QuantityDimension(pow(power));
	  }
	}

}
