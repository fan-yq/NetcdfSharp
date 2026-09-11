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
	/// Provides support for units that are offset from reference units (ex: as the
	/// unit "degree Celsius" is offset from the reference unit "kelvin"). Instances
	/// of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class OffsetUnit extends UnitImpl implements DerivableUnit
	[Serializable]
	public sealed class OffsetUnit : UnitImpl, DerivableUnit
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The origin of this unit in terms of the reference unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly double _offset;

	  /// <summary>
	  /// The reference unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly Unit _unit;

	  /// <summary>
	  /// The derived unit that is convertible with this unit.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly DerivedUnit _derivedUnit;

	  /// <summary>
	  /// Constructs from a reference unit and an offset.
	  /// </summary>
	  /// <param name="unit">
	  ///        The reference unit. </param>
	  /// <param name="offset">
	  ///        The origin of this unit in terms of the reference unit. For
	  ///        example, a degree Celsius unit would be created as "<code>new
	  ///         OffsetUnit(kelvin, 273.15)</code>. </param>
	  public OffsetUnit(in Unit unit, in double offset) : this(unit, offset, null)
	  {
	  }

	  /// <summary>
	  /// Constructs from a reference unit, and offset, and a unit identifier.
	  /// </summary>
	  /// <param name="unit">
	  ///        The reference unit. </param>
	  /// <param name="offset">
	  ///        The origin of this unit in terms of the reference unit. For
	  ///        example, a degree Celsius unit would be created as "<code>new
	  ///         OffsetUnit(kelvin, 273.15)</code>. </param>
	  /// <param name="id">
	  ///        The identifier for the new unit. </param>
	  public OffsetUnit(in Unit unit, in double offset, in UnitName id) : base(id)
	  {
		if (!(unit is OffsetUnit))
		{
		  _unit = unit;
		  _offset = offset;
		}
		else
		{
		  _unit = ((OffsetUnit) unit)._unit;
		  _offset = ((OffsetUnit) unit)._offset + offset;
		}
		_derivedUnit = _unit.DerivedUnit;
	  }

	  internal static Unit getInstance(in Unit unit, in double origin)
	  {
		return (origin == 0) ? unit : new OffsetUnit(unit, origin);
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

	  /// <summary>
	  /// Returns the offset. The offset is the location of the origin of this unit
	  /// in terms of the reference unit.
	  /// </summary>
	  /// <returns> The origin of this unit in terms of the reference unit. </returns>
	  public double Offset
	  {
		  get
		  {
			return _offset;
		  }
	  }

	  /*
	   * From UnitImpl:
	   */

	  /// <summary>
	  /// Clones this unit, changing the identifier.
	  /// </summary>
	  /// <param name="id">
	  ///        The identifier for the new unit. </param>
	  /// <returns> This unit with its identifier changed. </returns>
	  public override Unit clone(UnitName id)
	  {
		return new OffsetUnit(Unit, Offset, id);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public Unit multiplyBy(final double scale) throws MultiplyException
	  public override Unit multiplyBy(double scale)
	  {
		if (scale == 0)
		{
		  throw new MultiplyException(scale, this);
		}
		return getInstance(_unit.multiplyBy(scale), _offset / scale);
	  }

	  public override Unit shiftTo(double origin)
	  {
		return getInstance(_unit, origin + _offset);
	  }

	  /// <summary>
	  /// Multiply this unit by another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The unit to multiply this unit by. </param>
	  /// <returns> The product of this unit and <code>that</code>. The offset of
	  ///         this unit will be ignored; thus, for example
	  ///         "celsius.myMultiplyBy(day)" is equivalent to
	  ///         "kelvin.myMultiplyBy(day)". </returns>
	  /// <exception cref="MultiplyException">
	  ///         Can't multiply these units together. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myMultiplyBy(final Unit that) throws MultiplyException
	  protected internal override Unit myMultiplyBy(Unit that)
	  {
		return that is OffsetUnit ? Unit.multiplyBy(((OffsetUnit) that).Unit) : Unit.multiplyBy(that);
	  }

	  /// <summary>
	  /// Divide this unit by another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The unit to divide this unit by. </param>
	  /// <returns> The quotient of this unit and <code>that</code>. The offset of
	  ///         this unit will be ignored; thus, for example
	  ///         "celsius.myDivideBy(day)" is equivalent to
	  ///         "kelvin.myDivideBy(day)". </returns>
	  /// <exception cref="OperationException">
	  ///         Can't divide these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myDivideBy(final Unit that) throws OperationException
	  protected internal override Unit myDivideBy(Unit that)
	  {
		return that is OffsetUnit ? Unit.divideBy(((OffsetUnit) that).Unit) : Unit.divideBy(that);
	  }

	  /// <summary>
	  /// Divide this unit into another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The unit to divide this unit into. </param>
	  /// <returns> The quotient of <code>that</code> unit and this unit. The offset
	  ///         of this unit will be ignored; thus, for example
	  ///         "celsius.myDivideInto(day)" is equivalent to
	  ///         "kelvin.myDivideInto(day)". </returns>
	  /// <exception cref="OperationException">
	  ///         Can't divide these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myDivideInto(final Unit that) throws OperationException
	  protected internal override Unit myDivideInto(Unit that)
	  {
		return that is OffsetUnit ? Unit.divideInto(((OffsetUnit) that).Unit) : Unit.divideInto(that);
	  }

	  /// <summary>
	  /// Raise this unit to a power.
	  /// </summary>
	  /// <param name="power">
	  ///        The power to raise this unit by. </param>
	  /// <returns> The result of raising this unit by the power <code>power</code>.
	  ///         The offset of this unit will be ignored; thus, for example
	  ///         "celsius.myRaiseTo(2)" is equivalent to "kelvin.myRaiseTo(2)". </returns>
	  /// <exception cref="RaiseException">
	  ///         Can't raise this unit to a power. </exception>
	  // Ignore offset (e.g. "Cel2" == "K2")
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myRaiseTo(final int power) throws RaiseException
	  protected internal override Unit myRaiseTo(int power)
	  {
		return Unit.raiseTo(power);
	  }

	  /// <summary>
	  /// Returns the derived unit that is convertible with this unit.
	  /// </summary>
	  /// <returns> The derived unit that is convertible with this unit. </returns>
	  public override DerivedUnit DerivedUnit
	  {
		  get
		  {
			return _derivedUnit;
		  }
	  }

	  /// <summary>
	  /// Converts a value in this unit to the equivalent value in the convertible
	  /// derived unit.
	  /// </summary>
	  /// <param name="amount">
	  ///        The value in this unit. </param>
	  /// <returns> The equivalent value in the convertible derived unit. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert between units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float toDerivedUnit(final float amount) throws ConversionException
	  public float toDerivedUnit(float amount)
	  {
		return (float) toDerivedUnit((double) amount);
	  }

	  /// <summary>
	  /// Converts a value in this unit to the equivalent value in the convertible
	  /// derived unit.
	  /// </summary>
	  /// <param name="amount">
	  ///        The value in this unit. </param>
	  /// <returns> The equivalent value in the convertible derived unit. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert between units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double toDerivedUnit(final double amount) throws ConversionException
	  public double toDerivedUnit(double amount)
	  {
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(this, DerivedUnit);
		}
		return ((DerivableUnit) Unit).toDerivedUnit(amount + Offset);
	  }

	  /// <summary>
	  /// Converts values in this unit to the equivalent values in the convertible
	  /// derived unit.
	  /// </summary>
	  /// <param name="input">
	  ///        The values in this unit. </param>
	  /// <param name="output">
	  ///        The equivalent values in the convertible derived unit. May be
	  ///        the same array as <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert between units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] toDerivedUnit(final float[] input, final float[] output) throws ConversionException
	  public float[] toDerivedUnit(float[] input, float[] output)
	  {
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(this, DerivedUnit);
		}
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final float origin = (float) getOffset();
		float origin = (float) Offset;
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] = input[i] + origin;
		}
		return ((DerivableUnit) Unit).toDerivedUnit(output, output);
	  }

	  /// <summary>
	  /// Converts values in this unit to the equivalent values in the convertible
	  /// derived unit.
	  /// </summary>
	  /// <param name="input">
	  ///        The values in this unit. </param>
	  /// <param name="output">
	  ///        The equivalent values in the convertible derived unit. May be
	  ///        the same array as <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert between units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double[] toDerivedUnit(final double[] input, final double[] output) throws ConversionException
	  public double[] toDerivedUnit(double[] input, double[] output)
	  {
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(this, DerivedUnit);
		}
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final double origin = getOffset();
		double origin = Offset;
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] = input[i] + origin;
		}
		return ((DerivableUnit) Unit).toDerivedUnit(output, output);
	  }

	  /// <summary>
	  /// Converts a value in the convertible derived unit to the equivalent value
	  /// in this unit.
	  /// </summary>
	  /// <param name="amount">
	  ///        The value in the convertible derived unit. </param>
	  /// <returns> The equivalent value in this unit. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert between units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float fromDerivedUnit(final float amount) throws ConversionException
	  public float fromDerivedUnit(float amount)
	  {
		return (float) fromDerivedUnit((double) amount);
	  }

	  /// <summary>
	  /// Converts a value in the convertible derived unit to the equivalent value
	  /// in this unit.
	  /// </summary>
	  /// <param name="amount">
	  ///        The value in the convertible derived unit. </param>
	  /// <returns> The equivalent value in this unit. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert between units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double fromDerivedUnit(final double amount) throws ConversionException
	  public double fromDerivedUnit(double amount)
	  {
		if (!(_unit is DerivableUnit))
		{
		  throw new ConversionException(DerivedUnit, this);
		}
		return ((DerivableUnit) Unit).fromDerivedUnit(amount) - Offset;
	  }

	  /// <summary>
	  /// Converts values in the convertible derived unit to the equivalent values
	  /// in this unit.
	  /// </summary>
	  /// <param name="input">
	  ///        The values in the convertible derived unit. </param>
	  /// <param name="output">
	  ///        The equivalent values in this unit. May be the same array as
	  ///        <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert between units. </exception>
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
// ORIGINAL LINE: final float origin = (float) getOffset();
		float origin = (float) Offset;
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] -= origin;
		}
		return output;
	  }

	  /// <summary>
	  /// Converts values in the convertible derived unit to the equivalent values
	  /// in this unit.
	  /// </summary>
	  /// <param name="input">
	  ///        The values in the convertible derived unit. </param>
	  /// <param name="output">
	  ///        The equivalent values in this unit. May be the same array as
	  ///        <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException">
	  ///         Can't convert between units. </exception>
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
// ORIGINAL LINE: final double origin = getOffset();
		double origin = Offset;
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] -= origin;
		}
		return output;
	  }

	  /// <summary>
	  /// Indicates if this unit is semantically identical to an object.
	  /// </summary>
	  /// <param name="object">
	  ///        The object. </param>
	  /// <returns> <code>true</code> if and only if this unit is semantically
	  ///         identical to <code>object
	  ///        </code>. </returns>
	  public override bool Equals(object @object)
	  {
		if (this == @object)
		{
		  return true;
		}
		if (_offset == 0)
		{
		  return @object.Equals(_unit);
		}
		if (!(@object is OffsetUnit))
		{
		  return false;
		}

		OffsetUnit that = (OffsetUnit) @object;
		if (that._offset.CompareTo(_offset) != 0)
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
		return (Offset == 0 ? 0 : Convert.ToDouble(Offset).GetHashCode()) ^ Unit.GetHashCode();
	  }

	  /// <summary>
	  /// Indicates if this unit is dimensionless.
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
			return "(" + Unit.ToString() + ") @ " + Offset;
		  }
	  }
	}

}
