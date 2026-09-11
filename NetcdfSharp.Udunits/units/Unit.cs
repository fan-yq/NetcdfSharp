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
	/// Interface for units.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public interface Unit
	{
		/// <summary>
		/// Gets the identifier of this unit.
		/// </summary>
		/// <returns> The identifier of this unit. May be null. </returns>
		UnitName UnitName { get; }

		/// <summary>
		/// Gets the name of this unit.
		/// </summary>
		/// <returns> The name of this unit. May be null. </returns>
		string Name { get; }

		/// <summary>
		/// Gets the plural form of the name of this unit.
		/// </summary>
		/// <returns> The plural of the name of this unit. May be null. </returns>
		string Plural { get; }

		/// <summary>
		/// Gets the symbol of this unit.
		/// </summary>
		/// <returns> The symbol for this unit. May be null. </returns>
		string Symbol { get; }

		/// <summary>
		/// Returns the string representation of the unit.
		/// </summary>
		/// <returns> The string representation of the unit </returns>
		string ToString();

		/// <summary>
		/// Returns the canonical string representation of the unit.
		/// </summary>
		/// <returns> The canonical string representation. </returns>
		string CanonicalString { get; }

		/// <summary>
		/// Returns the derived unit that underlies this unit.
		/// </summary>
		/// <returns> The derived unit that underlies this unit. </returns>
		DerivedUnit DerivedUnit { get; }

		/// <summary>
		/// Clones this unit, changing the identifier.
		/// </summary>
		/// <param name="id">
		///        The identifier for the new unit. </param>
		/// <returns> The new unit. </returns>
		Unit clone(UnitName id);

		/// <summary>
		/// Multiplies this unit by another.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> The product of multiplying this unit by the other unit. </returns>
		/// <exception cref="MultiplyException">
		///         Can't multiply these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Unit multiplyBy(Unit that) throws MultiplyException;
		Unit multiplyBy(Unit that);

		/// <summary>
		/// Multiplies this unit by a scale factor. For example, if {@code m} is a
		/// meter unit, then {@code m.multiplyBy(1e-2)} returns a centimeter unit.
		/// </summary>
		/// <param name="scale">
		///        The scale factor. </param>
		/// <returns> The result of multiplying this unit by the scale factor. </returns>
		/// <exception cref="MultiplyException">
		///         if {@code scale} is zero. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Unit multiplyBy(double scale) throws MultiplyException;
		Unit multiplyBy(double scale);

		/// <summary>
		/// Divides this unit by another.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> The quotient of dividing this unit by the other unit. </returns>
		/// <exception cref="OperationException">
		///         Can't divide these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Unit divideBy(Unit that) throws OperationException;
		Unit divideBy(Unit that);

		/// <summary>
		/// Divides this unit into another.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> The quotient of dividing this unit into the other unit. </returns>
		/// <exception cref="OperationException">
		///         Can't divide these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Unit divideInto(Unit that) throws OperationException;
		Unit divideInto(Unit that);

		/// <summary>
		/// Raises this unit to a power.
		/// </summary>
		/// <param name="power">
		///        The power. </param>
		/// <returns> This result of raising this unit to the power. </returns>
		/// <exception cref="RaiseException">
		///         Can't raise this unit to a power. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Unit raiseTo(int power) throws RaiseException;
		Unit raiseTo(int power);

		/// <summary>
		/// Returns a unit identical to this instance but whose origin (i.e., zero
		/// value) has been shifted to the given value. For example, if {@code degK}
		/// is a Kelvin unit, then {@code degK.shiftTo(273.15)} is a Celsius unit.
		/// </summary>
		/// <param name="origin">
		///        The new origin in units of this instance. </param>
		/// <returns> A unit convertible with this instance but whose zero value is
		///         equal to the value {@code origin} of this instance. </returns>
		/// <exception cref="ShiftException">
		///         if the corresponding new unit can't be created. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Unit shiftTo(double origin) throws ShiftException;
		Unit shiftTo(double origin);

		/// <summary>
		/// Returns a unit identical to this instance but whose origin (i.e., zero
		/// value) has been shifted to the given time. For example, if {@code sec} is
		/// a second unit, then {@code sec.shiftTo(new Date(0L)} is the unit
		/// corresponding to seconds since the epoch (1970-01-01 00:00:00 UTC).
		/// </summary>
		/// <param name="origin">
		///        The new origin. </param>
		/// <returns> A unit whose zero value is the time given by {@code origin}. </returns>
		/// <exception cref="ShiftException">
		///         if the corresponding new unit can't be created. For example,
		///         if this instance isn't a unit of time. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Unit shiftTo(NetcdfSharp.java.util.Date origin) throws ShiftException;
		Unit shiftTo(DateTime origin);

		/// <summary>
		/// Returns a logarithmic unit whose reference level is equal to this unit.
		/// For example, if {@code mW} is a milliwatt unit, then {@code mW.log(10.)}
		/// returns a base-ten logarithmic unit with a milliwatt reference level.
		/// </summary>
		/// <param name="base">
		///        The logarithmic base. Must be one of {@code 2}, <seealso cref="Math.E"/>
		///        , or {@code 10}. </param>
		/// <exception cref="IllegalArgumentException">
		///         if {@code base} isn't one of the allowed values. </exception>
		Unit log(double @base);

		/// <summary>
		/// Gets a Converter that converts numeric values from this unit to another,
		/// compatible unit.
		/// </summary>
		/// <param name="outputUnit">
		///        The unit to which to convert the numerical values. </param>
		/// <returns> A converter of numeric values from this unit to the other unit. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't compatible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Converter getConverterTo(Unit outputUnit) throws ConversionException;
		IShimConverter getConverterTo(Unit outputUnit);

		/// <summary>
		/// Converts a numerical value from this unit to another unit.
		/// </summary>
		/// <param name="amount">
		///        The numerical value in this unit. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numerical value. </param>
		/// <returns> The numerical value in the output unit. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't compatible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: float convertTo(float amount, Unit outputUnit) throws ConversionException;
		float convertTo(float amount, Unit outputUnit);

		/// <summary>
		/// Converts a numerical value from this unit to another unit.
		/// </summary>
		/// <param name="amount">
		///        The numerical value in this unit. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numerical value. </param>
		/// <returns> The numerical value in the output unit. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't compatible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: double convertTo(double amount, Unit outputUnit) throws ConversionException;
		double convertTo(double amount, Unit outputUnit);

		/// <summary>
		/// Converts numerical values from this unit to another unit.
		/// </summary>
		/// <param name="amounts">
		///        The numerical values in this unit. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numerical values. </param>
		/// <returns> The numerical values in the output unit. in allocated space. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't compatible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: float[] convertTo(float[] amounts, Unit outputUnit) throws ConversionException;
		float[] convertTo(float[] amounts, Unit outputUnit);

		/// <summary>
		/// Converts numerical values from this unit to another unit.
		/// </summary>
		/// <param name="amounts">
		///        The numerical values in this unit. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numerical values. </param>
		/// <returns> The numerical values in the output unit. in allocated space. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't compatible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: double[] convertTo(double[] amounts, Unit outputUnit) throws ConversionException;
		double[] convertTo(double[] amounts, Unit outputUnit);

		/// <summary>
		/// Converts numerical values from this unit to another unit.
		/// </summary>
		/// <param name="input">
		///        The numerical values in this unit. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numerical values. </param>
		/// <param name="output">
		///        The output numerical values. May be the same array as the
		///        input values. </param>
		/// <returns> <code>output</code>. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't compatible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: float[] convertTo(float[] input, Unit outputUnit, float[] output) throws ConversionException;
		float[] convertTo(float[] input, Unit outputUnit, float[] output);

		/// <summary>
		/// Converts numerical values from this unit to another unit.
		/// </summary>
		/// <param name="input">
		///        The numerical values in this unit. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numerical values. </param>
		/// <param name="output">
		///        The output numerical values. May be the same array as the
		///        input values. </param>
		/// <returns> <code>output</code>. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't compatible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: double[] convertTo(double[] input, Unit outputUnit, double[] output) throws ConversionException;
		double[] convertTo(double[] input, Unit outputUnit, double[] output);

		/// <summary>
		/// Indicates if this unit is compatible with another unit.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> True iff values in this unit are convertible to values in the
		///         other unit. </returns>
		bool isCompatible(Unit that);

		/// <summary>
		/// Indicates if this unit is semantically identical to an object.
		/// </summary>
		/// <param name="object">
		///        The object. </param>
		/// <returns> <code>true</code> if and only if this unit is semantically
		///         identical to the object. </returns>
		bool Equals(object @object);

		/// <summary>
		/// Makes a label for a named quantity.
		/// </summary>
		/// <param name="quantityID">
		///        An identifier of the quantity for which the label is intended
		///        (e.g. "altitude"). </param>
		/// <returns> A label (e.g. "altitude/km"). </returns>
		string makeLabel(string quantityID);

		/// <summary>
		/// Indicates if values in this unit are dimensionless.
		/// </summary>
		/// <returns> <code>true</code> if and only if this unit is dimensionless. </returns>
		bool Dimensionless { get; }
	}

}
