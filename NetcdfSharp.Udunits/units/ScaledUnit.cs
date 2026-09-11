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
	/// Provides support for a unit that is a mutiplicative factor of a reference
	/// unit.
	/// 
	/// Instances of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class ScaledUnit extends UnitImpl implements DerivableUnit
	[Serializable]
	public sealed class ScaledUnit : UnitImpl, DerivableUnit
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The multiplicative factor.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly double _scale;

	  /// <summary>
	  /// The reference unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly Unit _unit;

	  /// <summary>
	  /// Constructs from a multiplicative factor. Returns a dimensionless unit
	  /// whose value is the multiplicative factor rather than unity.
	  /// </summary>
	  /// <param name="scale">
	  ///        The multiplicative factor. </param>
	  public ScaledUnit(in double scale) : this(scale, DerivedUnitImpl.DIMENSIONLESS)
	  {
	  }

	  /// <summary>
	  /// Constructs from a multiplicative factor and a reference unit.
	  /// </summary>
	  /// <param name="scale">
	  ///        The multiplicative factor. </param>
	  /// <param name="unit">
	  ///        The reference unit. </param>
	  public ScaledUnit(in double scale, in Unit unit) : this(scale, unit, null)
	  {
	  }

	  /// <summary>
	  /// Constructs from a multiplicative factor, a reference unit, and an
	  /// identifier.
	  /// </summary>
	  /// <param name="scale">
	  ///        The multiplicative factor. </param>
	  /// <param name="unit">
	  ///        The reference unit. </param>
	  /// <param name="id">
	  ///        The identifier for the unit. </param>
	  public ScaledUnit(in double scale, in Unit unit, in UnitName id) : base(id)
	  {
		if (!(unit is ScaledUnit))
		{
		  _unit = unit;
		  _scale = scale;
		}
		else
		{
		  _unit = ((ScaledUnit) unit)._unit;
		  _scale = ((ScaledUnit) unit)._scale * scale;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static Unit getInstance(final double scale, final Unit unit) throws MultiplyException
	  internal static Unit getInstance(in double scale, in Unit unit)
	  {
		if (scale == 0)
		{
		  throw new MultiplyException(scale, unit);
		}
		return scale == 1 ? unit : new ScaledUnit(scale, unit);
	  }

	  /// <summary>
	  /// Returns the multiplicative factor.
	  /// </summary>
	  /// <returns> The multiplicative factor. </returns>
	  public double Scale
	  {
		  get
		  {
			return _scale;
		  }
	  }

	  /// <summary>
	  /// Returns the reference unit.
	  /// </summary>
	  /// <returns> The reference unit. </returns>
	  public Unit Unit
	  {
		  get
		  {
			return _unit;
		  }
	  }

	  /*
	   * From UnitImpl:
	   */

	  /// <summary>
	  /// Clones this unit, changing the identifier.
	  /// </summary>
	  /// <param name="id">
	  ///        The new identifier. </param>
	  /// <returns> A ScaledUnit with the new identifier. </returns>
	  public override Unit clone(UnitName id)
	  {
		return new ScaledUnit(_scale, Unit, id);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public Unit multiplyBy(final double scale) throws MultiplyException
	  public override Unit multiplyBy(double scale)
	  {
		return getInstance(scale * _scale, _unit);
	  }

	  /// <summary>
	  /// Multiplies this unit by another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The other unit. </param>
	  /// <returns> The product of this unit and the other unit. </returns>
	  /// <exception cref="MultiplyException">
	  ///         Can't multiply these units together. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myMultiplyBy(final Unit that) throws MultiplyException
	  protected internal override Unit myMultiplyBy(Unit that)
	  {
		return that is ScaledUnit ? new ScaledUnit(Scale * ((ScaledUnit) that).Scale, Unit.multiplyBy(((ScaledUnit) that).Unit)) : new ScaledUnit(Scale, Unit.multiplyBy(that));
	  }

	  /// <summary>
	  /// Divides this unit by another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The other unit. </param>
	  /// <returns> The quotient of this unit divided by the other unit. </returns>
	  /// <exception cref="OperationException">
	  ///         Can't divide these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myDivideBy(final Unit that) throws OperationException
	  protected internal override Unit myDivideBy(Unit that)
	  {
		return that is ScaledUnit ? new ScaledUnit(Scale / ((ScaledUnit) that).Scale, Unit.divideBy(((ScaledUnit) that).Unit)) : new ScaledUnit(Scale, Unit.divideBy(that));
	  }

	  /// <summary>
	  /// Divides this unit into another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The other unit. </param>
	  /// <returns> The quotient of this unit divided into the other unit. </returns>
	  /// <exception cref="OperationException">
	  ///         Can't divide these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myDivideInto(final Unit that) throws OperationException
	  protected internal override Unit myDivideInto(Unit that)
	  {
		return that is ScaledUnit ? new ScaledUnit(((ScaledUnit) that).Scale / Scale, Unit.divideInto(((ScaledUnit) that).Unit)) : new ScaledUnit(1 / Scale, Unit.divideInto(that));
	  }

	  /// <summary>
	  /// Raises this unit to a power.
	  /// </summary>
	  /// <param name="power">
	  ///        The power. </param>
	  /// <returns> The result of raising this unit to the power. </returns>
	  /// <exception cref="RaiseException">
	  ///         Can't raise this unit to a power. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myRaiseTo(final int power) throws RaiseException
	  protected internal override Unit myRaiseTo(int power)
	  {
		return new ScaledUnit(Math.Pow(Scale, power), Unit.raiseTo(power));
	  }

	  /// <summary>
	  /// Gets the derived unit underlying this unit.
	  /// </summary>
	  /// <returns> The derived unit which underlies this unit. </returns>
	  public override DerivedUnit DerivedUnit
	  {
		  get
		  {
			return Unit.DerivedUnit;
		  }
	  }

	  /// <summary>
	  /// Converts a numeric value from this unit to the underlying derived unit.
	  /// </summary>
	  /// <param name="amount">
	  ///        The numeric value in this unit. </param>
	  /// <returns> The equivalent value in the underlying derived unit. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert value to the underlying derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float toDerivedUnit(final float amount) throws ConversionException
	  public float toDerivedUnit(float amount)
	  {
		return (float) toDerivedUnit((double) amount);
	  }

	  /// <summary>
	  /// Converts a numeric value from this unit to the underlying derived unit.
	  /// </summary>
	  /// <param name="amount">
	  ///        The numeric value in this unit. </param>
	  /// <returns> The equivalent value in the underlying derived unit. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert value to the underlying derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double toDerivedUnit(final double amount) throws ConversionException
	  public double toDerivedUnit(double amount)
	  {
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(this, DerivedUnit);
		}
		return ((DerivableUnit) _unit).toDerivedUnit(amount * Scale);
	  }

	  /// <summary>
	  /// Converts numeric values from this unit to the underlying derived unit.
	  /// </summary>
	  /// <param name="input">
	  ///        The numeric values in this unit. </param>
	  /// <param name="output">
	  ///        The equivalent values in the underlying derived unit. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert values to the underlying derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] toDerivedUnit(final float[] input, final float[] output) throws ConversionException
	  public float[] toDerivedUnit(float[] input, float[] output)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final float scale = (float) getScale();
		float scale = (float) Scale;
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] = input[i] * scale;
		}
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(this, DerivedUnit);
		}
		return ((DerivableUnit) Unit).toDerivedUnit(output, output);
	  }

	  /// <summary>
	  /// Converts numeric values from this unit to the underlying derived unit.
	  /// </summary>
	  /// <param name="input">
	  ///        The numeric values in this unit. </param>
	  /// <param name="output">
	  ///        The equivalent values in the underlying derived unit. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert values to the underlying derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double[] toDerivedUnit(final double[] input, final double[] output) throws ConversionException
	  public double[] toDerivedUnit(double[] input, double[] output)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final double scale = getScale();
		double scale = Scale;
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] = input[i] * scale;
		}
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(this, DerivedUnit);
		}
		return ((DerivableUnit) Unit).toDerivedUnit(output, output);
	  }

	  /// <summary>
	  /// Converts a numeric value from the underlying derived unit to this unit.
	  /// </summary>
	  /// <param name="amount">
	  ///        The numeric value in the underlying derived unit. </param>
	  /// <returns> The equivalent value in this unit. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert value. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float fromDerivedUnit(final float amount) throws ConversionException
	  public float fromDerivedUnit(float amount)
	  {
		return (float) fromDerivedUnit((double) amount);
	  }

	  /// <summary>
	  /// Converts a numeric value from the underlying derived unit to this unit.
	  /// </summary>
	  /// <param name="amount">
	  ///        The numeric value in the underlying derived unit. </param>
	  /// <returns> The equivalent value in this unit. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert value. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double fromDerivedUnit(final double amount) throws ConversionException
	  public double fromDerivedUnit(double amount)
	  {
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(DerivedUnit, this);
		}
		return ((DerivableUnit) Unit).fromDerivedUnit(amount) / Scale;
	  }

	  /// <summary>
	  /// Converts numeric values from the underlying derived unit to this unit.
	  /// </summary>
	  /// <param name="input">
	  ///        The numeric values in the underlying derived unit. </param>
	  /// <param name="output">
	  ///        The equivalent values in this unit. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert values. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] fromDerivedUnit(final float[] input, final float[] output) throws ConversionException
	  public float[] fromDerivedUnit(float[] input, float[] output)
	  {
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(DerivedUnit, this);
		}
		((DerivableUnit) Unit).fromDerivedUnit(input, output);
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final float scale = (float) getScale();
		float scale = (float) Scale;
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] /= scale;
		}
		return output;
	  }

	  /// <summary>
	  /// Converts numeric values from the underlying derived unit to this unit.
	  /// </summary>
	  /// <param name="input">
	  ///        The numeric values in the underlying derived unit. </param>
	  /// <param name="output">
	  ///        The equivalent values in this unit. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert values. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double[] fromDerivedUnit(final double[] input, final double[] output) throws ConversionException
	  public double[] fromDerivedUnit(double[] input, double[] output)
	  {
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(DerivedUnit, this);
		}
		((DerivableUnit) Unit).fromDerivedUnit(input, output);
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final double scale = getScale();
		double scale = Scale;
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] /= scale;
		}
		return output;
	  }

	  /// <summary>
	  /// Indicates if this unit is semantically identical to an object.
	  /// </summary>
	  /// <param name="object">
	  ///        The object. </param>
	  /// <returns> <code>true</code> if an only if this unit is semantically
	  ///         identical to <code>object
	  ///        </code>. </returns>
	  public override bool Equals(object @object)
	  {
		if (this == @object)
		{
		  return true;
		}
		if (_scale == 1)
		{
		  return @object.Equals(_unit);
		}
		if (!(@object is ScaledUnit))
		{
		  return false;
		}
		ScaledUnit that = (ScaledUnit) @object;
		if (that._scale.CompareTo(_scale) != 0)
		{
		  return false;
		}
		return _unit.Equals(that._unit);
	  }

	  /// <summary>
	  /// Returns the hash code of this instance.
	  /// </summary>
	  /// <returns> The hash code of this instance. </returns>
	  public override int GetHashCode()
	  {
		return (Scale == 1 ? 0 : Convert.ToDouble(Scale).GetHashCode()) ^ Unit.GetHashCode();
	  }

	  /// <summary>
	  /// Indicates if this unit is dimensionless. A ScaledUnit is dimensionless if
	  /// and only if the reference unit is dimensionless.
	  /// </summary>
	  /// <returns> <code>true</code> if and only if this unit is dimensionless. </returns>
	  public override bool Dimensionless
	  {
		  get
		  {
			return Unit.Dimensionless;
		  }
	  }

	  /// <summary>
	  /// Returns the string representation of this unit.
	  /// </summary>
	  /// <returns> The string representation of this unit. </returns>
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
			return DerivedUnitImpl.DIMENSIONLESS.Equals(_unit) ? Convert.ToString(Scale) : Scale + " " + _unit.ToString();
		  }
	  }
	}

}
