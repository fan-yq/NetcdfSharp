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
	/// Provides support for units that are based on a logarithm of the ratio of a
	/// physical quantity to an underlying reference level.
	/// 
	/// Instances of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class LogarithmicUnit extends UnitImpl implements DerivableUnit
	[Serializable]
	public sealed class LogarithmicUnit : UnitImpl, DerivableUnit
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The logarithmic base.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly double @base;

	  [NonSerialized]
	  private readonly double lnBase;

	  /// <summary>
	  /// The reference level.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly DerivableUnit reference;
	  [NonSerialized]
	  private readonly DerivedUnit derivedUnit;

	  /// <summary>
	  /// Constructs from a reference level and a logarithmic base.
	  /// </summary>
	  /// <param name="reference">
	  ///        The reference level. Must be a <seealso cref="DerivableUnit"/>. </param>
	  /// <param name="base">
	  ///        The logarithmic base. Must be 2, <seealso cref="Math.E"/>, or 10. </param>
	  /// <exception cref="IllegalArgumentException">
	  ///         if {@code reference} isn't a <seealso cref="DerivableUnit"/>. </exception>
	  /// <exception cref="IllegalArgumentException">
	  ///         if {@code base} isn't one of the allowed values. </exception>
	  /// <exception cref="NullPointerException">
	  ///         if {@code reference} is {@code null}. </exception>
	  public LogarithmicUnit(in Unit reference, in double @base) : this(reference, @base, null)
	  {
	  }

	  /// <summary>
	  /// Constructs from a reference level, a logarithmic base, and a unit
	  /// identifier.
	  /// </summary>
	  /// <param name="reference">
	  ///        The reference level. Must be a <seealso cref="DerivableUnit"/>. </param>
	  /// <param name="base">
	  ///        The logarithmic base. Must be 2, <seealso cref="Math.E"/>, or 10. </param>
	  /// <param name="id">
	  ///        The identifier for the new unit. </param>
	  /// <exception cref="IllegalArgumentException">
	  ///         if {@code reference} isn't a <seealso cref="DerivableUnit"/>. </exception>
	  /// <exception cref="IllegalArgumentException">
	  ///         if {@code base} isn't one of the allowed values. </exception>
	  /// <exception cref="NullPointerException">
	  ///         if {@code reference} is {@code null}. </exception>
	  public LogarithmicUnit(in Unit reference, in double @base, in UnitName id) : base(id)
	  {
		if (reference == null)
		{
		  throw new System.NullReferenceException("Null reference argument");
		}
		if (!(reference is DerivableUnit))
		{
		  throw new ArgumentException("Not a DerivableUnit: " + reference);
		}
		this.reference = (DerivableUnit) reference;
		if (@base != 2 && @base != 10 && @base != Math.E)
		{
		  throw new ArgumentException("Invalid base: " + @base);
		}
		this.@base = @base;
		lnBase = @base == Math.E ? 1 : Math.Log(@base);
		derivedUnit = reference.DerivedUnit;
	  }

	  internal static Unit getInstance(in Unit unit, in double @base)
	  {
		return new LogarithmicUnit(unit, @base);
	  }

	  /// <summary>
	  /// Returns the reference level.
	  /// </summary>
	  /// <returns> The reference level. </returns>
	  public DerivableUnit Reference
	  {
		  get
		  {
			return reference;
		  }
	  }

	  /// <summary>
	  /// Returns the logarithmic base.
	  /// </summary>
	  /// <returns> The logarithmic base of this unit. </returns>
	  public double Base
	  {
		  get
		  {
			return @base;
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
		return new LogarithmicUnit((Unit) reference, Base, id);
	  }

	  /// <summary>
	  /// Multiply this unit by another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The unit to multiply this unit by. Must be dimensionless. </param>
	  /// <returns> The product of this unit and <code>that</code>. </returns>
	  /// <exception cref="MultiplyException">
	  ///         Can't multiply these units together. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myMultiplyBy(final Unit that) throws MultiplyException
	  protected internal override Unit myMultiplyBy(Unit that)
	  {
		if (!that.Dimensionless)
		{
		  throw new MultiplyException(that);
		}
		return that is ScaledUnit ? (NetcdfSharp.ucar.units.UnitImpl)new ScaledUnit(((ScaledUnit) that).Scale, this) : this;
	  }

	  /// <summary>
	  /// Divide this unit by another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The unit to divide this unit by. </param>
	  /// <returns> The quotient of this unit and <code>that</code>. </returns>
	  /// <exception cref="DivideException">
	  ///         Can't divide these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myDivideBy(final Unit that) throws DivideException
	  protected internal override Unit myDivideBy(Unit that)
	  {
		if (!that.Dimensionless)
		{
		  throw new DivideException(that);
		}
		return that is ScaledUnit ? (NetcdfSharp.ucar.units.UnitImpl)new ScaledUnit(1.0 / ((ScaledUnit) that).Scale, this) : this;
	  }

	  /// <summary>
	  /// Divide this unit into another unit.
	  /// </summary>
	  /// <param name="that">
	  ///        The unit to divide this unit into. </param>
	  /// <returns> The quotient of <code>that</code> unit and this unit. </returns>
	  /// <exception cref="DivideException">
	  ///         Can't divide these units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myDivideInto(final Unit that) throws OperationException
	  protected internal override Unit myDivideInto(Unit that)
	  {
		throw new DivideException(that);
	  }

	  /// <summary>
	  /// Raise this unit to a power.
	  /// </summary>
	  /// <param name="power">
	  ///        The power to raise this unit by. The only meaningful values
	  ///        are 0 and 1. </param>
	  /// <returns> The result of raising this unit by the power <code>power</code>. </returns>
	  /// <exception cref="RaiseException">
	  ///         Can't raise this unit to {@code power}, which is neither 0
	  ///         nor 1. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override protected Unit myRaiseTo(final int power) throws RaiseException
	  protected internal override Unit myRaiseTo(int power)
	  {
		if (power == 0)
		{
		  return DerivedUnitImpl.DIMENSIONLESS;
		}
		if (power == 1)
		{
		  return this;
		}
		throw new RaiseException(this);
	  }

	  /// <summary>
	  /// Returns the derived unit that is convertible with this unit.
	  /// </summary>
	  /// <returns> The derived unit that is convertible with this unit. </returns>
	  public override DerivedUnit DerivedUnit
	  {
		  get
		  {
			return derivedUnit;
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
		return reference.toDerivedUnit(Math.Exp(amount * lnBase));
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
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] = (float)(Math.Exp(input[i] * lnBase));
		}
		return reference.toDerivedUnit(output, output);
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
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] = Math.Exp(input[i] * lnBase);
		}
		return reference.toDerivedUnit(output, output);
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
		return Math.Log(reference.fromDerivedUnit(amount)) / lnBase;
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
		reference.fromDerivedUnit(input, output);
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] = (float)(Math.Log(output[i]) / lnBase);
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
		reference.fromDerivedUnit(input, output);
		for (int i = input.Length; --i >= 0;)
		{
		  output[i] = (float)(Math.Log(output[i]) / lnBase);
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
		if (!(@object is LogarithmicUnit))
		{
		  return false;
		}
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final LogarithmicUnit that = (LogarithmicUnit) object;
		LogarithmicUnit that = (LogarithmicUnit) @object;
		return @base == that.@base && reference.Equals(that.reference);
	  }

	  /// <summary>
	  /// Returns the hash code of this instance.
	  /// </summary>
	  /// <returns> The hash code of this instance. </returns>
	  public override int GetHashCode()
	  {
		return Convert.ToDouble(@base).GetHashCode() ^ Reference.GetHashCode();
	  }

	  /// <summary>
	  /// Indicates if this unit is dimensionless.
	  /// </summary>
	  /// <returns> <code>true</code>, always. </returns>
	  public override bool Dimensionless
	  {
		  get
		  {
			return true;
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
			return (@base == 2 ? "lb" : @base == Math.E ? "ln" : "lg") + "(re " + Reference.ToString() + ")";
		  }
	  }

	}

}
