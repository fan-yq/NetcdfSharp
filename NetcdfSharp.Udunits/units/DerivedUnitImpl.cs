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
	/// Provides support for a concrete implementation of derived units.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	// @Immutable
	[Serializable]
	public class DerivedUnitImpl : UnitImpl, DerivedUnit, DerivableUnit
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The dimensionless derived unit.
	  /// </summary>
	  public static readonly DerivedUnitImpl DIMENSIONLESS = new DerivedUnitImpl();

	  /// <summary>
	  /// The dimension of this derived unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private UnitDimension dimension;

	  /// <summary>
	  /// Constructs a dimensionless derived unit from nothing.
	  /// </summary>
	  protected internal DerivedUnitImpl() : this(new UnitDimension(), dimensionlessID())
	  {
		// dimensionless derived unit
	  }

	  /// <summary>
	  /// Returns the identifiers associated with the dimensionless, derived unit.
	  /// </summary>
	  /// <returns> The identifiers of the dimensionless, derived unit. </returns>
	  private static UnitName dimensionlessID()
	  {
		UnitName id;
		try
		{
		  id = UnitName.newUnitName("1", "1", "1");
		}
// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
// ORIGINAL LINE: catch (final NameException e)
		catch (NameException)
		{
		  id = null;
		}
		return id;
	  }

	  /// <summary>
	  /// Constructs from a unit dimension. This is a trusted constructor for use
	  /// by subclasses only.
	  /// </summary>
	  /// <param name="dimension">
	  ///        The unit dimension. </param>
	  protected internal DerivedUnitImpl(in UnitDimension dimension) : this(dimension, null)
	  {
	  }

	  /// <summary>
	  /// Constructs from identifiers. This is a trusted constructor for use by
	  /// subclasses only.
	  /// </summary>
	  /// <param name="id">
	  ///        The identifiers for the unit. </param>
	  protected internal DerivedUnitImpl(in UnitName id) : this(null, id)
	  {
	  }

	  /// <summary>
	  /// Constructs from a unit dimension and identifiers. This is a trusted
	  /// constructor for use by subclasses only.
	  /// </summary>
	  /// <param name="dimension">
	  ///        The unit dimension. </param>
	  /// <param name="id">
	  ///        The identifiers for the unit. </param>
	  protected internal DerivedUnitImpl(in UnitDimension dimension, in UnitName id) : base(id)
	  {
		this.dimension = dimension;
	  }

	  /// <summary>
	  /// Sets the unit dimension of this derived unit. This is a trusted method
	  /// for use by subclasses only and should be called only once immediately
	  /// after construction of this superinstance.
	  /// </summary>
	  /// <param name="dimension">
	  ///        The unit dimension. </param>
	  public virtual UnitDimension Dimension
	  {
		  set
		  {
			this.dimension = value;
		  }
		  get
		  {
			return dimension;
		  }
	  }


	  /// <summary>
	  /// Returns the quantity dimension of this derived unit.
	  /// </summary>
	  /// <returns> The quantity dimension of this derived unit. </returns>
	  public QuantityDimension QuantityDimension
	  {
		  get
		  {
			return Dimension.QuantityDimension;
		  }
	  }

	  /*
	   * From DerivedUnit:
	   */

	  /// <summary>
	  /// Indicates if this derived unit is the reciprocal of another derived unit
	  /// (e.g. "second" and "hertz").
	  /// </summary>
	  /// <param name="that">
	  ///        The other, derived unit. </param>
	  public bool isReciprocalOf(DerivedUnit that)
	  {
		return dimension.isReciprocalOf(that.Dimension);
	  }

	  /*
	   * From UnitImpl:
	   */

	  /// <summary>
	  /// Returns the derived unit that is convertible with this unit. Obviously,
	  /// the method returns this derived unit.
	  /// </summary>
	  /// <returns> <code>this</code>. </returns>
	  public sealed override DerivedUnit DerivedUnit
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <summary>
	  /// Clones the derived unit changing the identifiers.
	  /// </summary>
	  /// <param name="id">
	  ///        The identifiers for the new unit. </param>
	  /// <returns> The new unit. </returns>
	  public sealed override Unit clone(UnitName id)
	  {
		return new DerivedUnitImpl(dimension, id);
	  }

	  /// <summary>
	  /// Multiplies this derived unit by another.
	  /// </summary>
	  /// <param name="that">
	  ///        The other unit. </param>
	  /// <returns> The product of the two units. </returns>
	  /// <exception cref="MultiplyException">
	  ///         Can't multiply these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myMultiplyBy(final Unit that) throws MultiplyException
	  protected internal override Unit myMultiplyBy(Unit that)
	  {
		Unit result;
		if (dimension.Rank == 0)
		{
		  result = that;
		}
		else
		{
		  if (!(that is DerivedUnit))
		  {
			result = that.multiplyBy(this);
		  }
		  else
		  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final UnitDimension thatDimension = ((DerivedUnit) that).getDimension();
			UnitDimension thatDimension = ((DerivedUnit) that).Dimension;
			result = thatDimension.Rank == 0 ? (NetcdfSharp.ucar.units.UnitImpl)this : new DerivedUnitImpl(dimension.multiplyBy(thatDimension));
		  }
		}
		return result;
	  }

	  /// <summary>
	  /// Divides this derived unit by another.
	  /// </summary>
	  /// <param name="that">
	  ///        The other unit. </param>
	  /// <returns> The quotient of the two units. </returns>
	  /// <exception cref="OperationException">
	  ///         Can't divide these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myDivideBy(final Unit that) throws OperationException
	  protected internal override Unit myDivideBy(Unit that)
	  {
		Unit result;
		if (dimension.Rank == 0)
		{
		  result = that.raiseTo(-1);
		}
		else
		{
		  if (!(that is DerivedUnit))
		  {
			result = that.divideInto(this);
		  }
		  else
		  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final UnitDimension thatDimension = ((DerivedUnit) that).getDimension();
			UnitDimension thatDimension = ((DerivedUnit) that).Dimension;
			result = thatDimension.Rank == 0 ? (NetcdfSharp.ucar.units.UnitImpl)this : new DerivedUnitImpl(dimension.divideBy(thatDimension));
		  }
		}
		return result;
	  }

	  /// <summary>
	  /// Divides this derived unit into another.
	  /// </summary>
	  /// <param name="that">
	  ///        The other unit. </param>
	  /// <returns> The quotient of the two units. </returns>
	  /// <exception cref="OperationException">
	  ///         Can't divide these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myDivideInto(final Unit that) throws OperationException
	  protected internal override Unit myDivideInto(Unit that)
	  {
		return that.divideBy(this);
	  }

	  /// <summary>
	  /// Raises this derived unit to a power.
	  /// </summary>
	  /// <param name="power">
	  ///        The power. </param>
	  /// <returns> This derived unit raised to the given power. </returns>
	  protected internal override Unit myRaiseTo(int power)
	  {
		return power == 1 ? (NetcdfSharp.ucar.units.UnitImpl)this : new DerivedUnitImpl(dimension.raiseTo(power));
	  }

	  /// <summary>
	  /// Converts a numerical value from this unit to the derived unit. Obviously,
	  /// the numerical value is unchanged.
	  /// </summary>
	  /// <param name="amount">
	  ///        The numerical values in this unit. </param>
	  /// <returns> The numerical value in the derived unit. </returns>
	  public float toDerivedUnit(float amount)
	  {
		return amount;
	  }

	  /// <summary>
	  /// Converts a numerical value from this unit to the derived unit. Obviously,
	  /// the numerical value is unchanged.
	  /// </summary>
	  /// <param name="amount">
	  ///        The numerical values in this unit. </param>
	  /// <returns> The numerical value in the derived unit. </returns>
	  public double toDerivedUnit(double amount)
	  {
		return amount;
	  }

	  /// <summary>
	  /// Converts numerical values from this unit to the derived unit. Obviously,
	  /// the numerical values are unchanged.
	  /// </summary>
	  /// <param name="input">
	  ///        The numerical values in this unit. </param>
	  /// <param name="output">
	  ///        The numerical values in the derived unit. May be the same
	  ///        array as <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  public float[] toDerivedUnit(float[] input, float[] output)
	  {
		if (input != output)
		{
		  System.Array.Copy(input, 0, output, 0, input.Length);
		}
		return output;
	  }

	  /// <summary>
	  /// Converts numerical values from this unit to the derived unit. Obviously,
	  /// the numerical values are unchanged.
	  /// </summary>
	  /// <param name="input">
	  ///        The numerical values in this unit. </param>
	  /// <param name="output">
	  ///        The numerical values in the derived unit. May be the same
	  ///        array as <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  public double[] toDerivedUnit(double[] input, double[] output)
	  {
		if (input != output)
		{
		  System.Array.Copy(input, 0, output, 0, input.Length);
		}
		return output;
	  }

	  /// <summary>
	  /// Converts a numerical value to this unit from the derived unit. Obviously,
	  /// the numerical value is unchanged.
	  /// </summary>
	  /// <param name="amount">
	  ///        The numerical values in the derived unit. </param>
	  /// <returns> The numerical value in this unit. </returns>
	  public float fromDerivedUnit(float amount)
	  {
		return amount;
	  }

	  /// <summary>
	  /// Converts a numerical value to this unit from the derived unit. Obviously,
	  /// the numerical value is unchanged.
	  /// </summary>
	  /// <param name="amount">
	  ///        The numerical values in the derived unit. </param>
	  /// <returns> The numerical value in this unit. </returns>
	  public double fromDerivedUnit(double amount)
	  {
		return amount;
	  }

	  /// <summary>
	  /// Converts numerical values to this unit from the derived unit. Obviously,
	  /// the numerical values are unchanged.
	  /// </summary>
	  /// <param name="input">
	  ///        The numerical values in the derived unit. </param>
	  /// <param name="output">
	  ///        The numerical values in this unit. May be the same array as
	  ///        <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  public float[] fromDerivedUnit(float[] input, float[] output)
	  {
		return toDerivedUnit(input, output);
	  }

	  /// <summary>
	  /// Converts numerical values to this unit from the derived unit. Obviously,
	  /// the numerical values are unchanged.
	  /// </summary>
	  /// <param name="input">
	  ///        The numerical values in the derived unit. </param>
	  /// <param name="output">
	  ///        The numerical values in this unit. May be the same array as
	  ///        <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  public double[] fromDerivedUnit(double[] input, double[] output)
	  {
		return toDerivedUnit(input, output);
	  }

	  /// <summary>
	  /// Indicates if values in this unit are convertible with another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The other unit. </param>
	  /// <returns> <code>true</code> if and only if values in this unit are
	  ///         convertible to values in <code>
	  ///        that</code>. </returns>
	  public sealed override bool isCompatible(Unit that)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final DerivedUnit unit = that.getDerivedUnit();
		DerivedUnit unit = that.DerivedUnit;
		return Equals(unit) || isReciprocalOf(unit);
	  }

	  /// <summary>
	  /// Indicates if this derived unit is semantically identical to an object.
	  /// </summary>
	  /// <param name="object">
	  ///        The object </param>
	  /// <returns> <code>true</code> if and only if this derived unit is
	  ///         semantically identical to <code>
	  ///        object</code>. </returns>
	  public override bool Equals(object @object)
	  {
		if (this == @object)
		{
		  return true;
		}
		if (!(@object is DerivedUnit))
		{
		  return false;
		}
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final DerivedUnit that = (DerivedUnit) object;
		DerivedUnit that = (DerivedUnit) @object;
		return (!(this is BaseUnit) || !(that is BaseUnit)) && dimension.Equals(that.Dimension);
	  }

	  /// <summary>
	  /// Returns the hash code of this instance.
	  /// </summary>
	  /// <returns> The hash code of this instance. </returns>
	  public override int GetHashCode()
	  {
		return this is BaseUnit ? System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this) : dimension.GetHashCode();
	  }

	  /// <summary>
	  /// Indicates if this derived unit is dimensionless.
	  /// </summary>
	  /// <returns> <code>true</code> if and only if this derived unit is
	  ///         dimensionless. </returns>
	  public override bool Dimensionless
	  {
		  get
		  {
			return dimension.Dimensionless;
		  }
	  }

	  /// <summary>
	  /// Returns a string representation of this unit. If the symbol or name is
	  /// available, then that is returned; otherwise, the corresponding expression
	  /// in base units is returned.
	  /// </summary>
	  /// <returns> The string expression for this derived unit. </returns>
	  public override string ToString()
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final String string = super.toString();
		string @string = base.ToString(); // get symbol or name
		return !string.ReferenceEquals(@string, null) ? @string : CanonicalString;
	  }

	  /// <summary>
	  /// Returns the canonical string representation of the unit.
	  /// </summary>
	  /// <returns> The canonical string representation. </returns>
	  public override string CanonicalString
	  {
		  get
		  {
			return dimension.ToString();
		  }
	  }
	}

}
