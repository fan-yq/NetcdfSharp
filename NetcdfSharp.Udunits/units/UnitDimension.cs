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

	/*
	 * Provides support for the concept of the dimension of a unit. The
	 * dimension of a unit is like the dimension of a quantity except that
	 * the individual quantity dimensions have been replaced with the
	 * corresponding base unit. For example, the dimension of the quantity
	 * "force" is "M.L.t-2" and the dimension of the corresponding SI unit
	 * (the "newton") is "kg.m.s-2".
	 *
	 * Instances of this class are immutable.
	 *
	 * @author Steven R. Emmerson
	 */
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class UnitDimension extends Dimension
	public sealed class UnitDimension : Dimension
	{
	  /// <summary>
	  /// Constructs a dimensionless unit dimension.
	  /// </summary>
	  public UnitDimension() : base()
	  {
	  }

	  /// <summary>
	  /// Constructs the unit dimension corresponding to a base unit.
	  /// </summary>
	  /// <param name="baseUnit"> A base unit. </param>
	  public UnitDimension(BaseUnit baseUnit) : base(new Factor(baseUnit))
	  {
	  }

	  /// <summary>
	  /// Constructs a unit dimension comprised of the given factors.
	  /// </summary>
	  /// <param name="factors"> The factors that constitute the unit dimension. </param>
	  private UnitDimension(Factor[] factors) : base(factors)
	  {
	  }

	  /// <summary>
	  /// Multiplies this unit dimension by another.
	  /// </summary>
	  /// <param name="that"> The other unit dimension. </param>
	  /// <returns> The product of this unit dimension multiplied
	  ///         by the other. </returns>
	  public UnitDimension multiplyBy(UnitDimension that)
	  {
		return new UnitDimension(mult(that));
	  }

	  /// <summary>
	  /// Divides this unit dimension by another.
	  /// </summary>
	  /// <param name="that"> The other unit dimension. </param>
	  /// <returns> The quotient of this unit dimension divided
	  ///         by the other. </returns>
	  public UnitDimension divideBy(UnitDimension that)
	  {
		return multiplyBy(that.raiseTo(-1));
	  }

	  /// <summary>
	  /// Raises this unit dimension to a power.
	  /// </summary>
	  /// <param name="power"> The power. </param>
	  /// <returns> The result of raising this unit dimension
	  ///         to the power. </returns>
	  public UnitDimension raiseTo(int power)
	  {
		return new UnitDimension(pow(power));
	  }

	  /// <summary>
	  /// Returns the corresponding quantity dimension.
	  /// </summary>
	  /// <returns> The quantity dimension corresponding to this
	  ///         unit dimension. </returns>
	  public QuantityDimension QuantityDimension
	  {
		  get
		  {
			Factor[] factors = Factors;
			for (int i = factors.Length; --i >= 0;)
			{
			  Factor factor = factors[i];
			  factors[i] = new Factor(((BaseUnit) factor.Base).BaseQuantity, factor.Exponent);
			}
			return new QuantityDimension(factors);
		  }
	  }

	}

}
