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
	/// Interface for units that can convert numeric values to and from an
	/// underlying derived unit.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public interface DerivableUnit
	{
	  /// <summary>
	  /// Returns the derived unit that underlies this unit.
	  /// </summary>
	  /// <returns> The derived unit that underlies this unit. </returns>
	  DerivedUnit DerivedUnit {get;}

	  /// <summary>
	  /// Converts a numeric value in this unit to the underlying derived unit.
	  /// This method might fail even though <code>convertTo()</code> succeeds.
	  /// </summary>
	  /// <param name="amount"> The numeric values in this unit. </param>
	  /// <returns> The numeric value in the underlying derived
	  ///         unit. </returns>
	  /// <exception cref="ConversionException"> Can't convert values to underlying
	  ///         derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: float toDerivedUnit(float amount) throws ConversionException;
	  float toDerivedUnit(float amount);

	  /// <summary>
	  /// Converts a numeric value in this unit to the underlying derived unit.
	  /// This method might fail even though <code>convertTo()</code> succeeds.
	  /// </summary>
	  /// <param name="amount"> The numeric value in this unit. </param>
	  /// <returns> The equivalent numeric value in the
	  ///         underlying derived unit. </returns>
	  /// <exception cref="ConversionException"> Can't convert to derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: double toDerivedUnit(double amount) throws ConversionException;
	  double toDerivedUnit(double amount);

	  /// <summary>
	  /// Converts numeric values in this unit to the underlying derived unit.
	  /// This method might fail even though <code>convertTo()</code> succeeds.
	  /// </summary>
	  /// <param name="input"> The numeric values in this unit. </param>
	  /// <param name="output"> The equivalent numeric values in the
	  ///        underlying derived unit. May be the same
	  ///        array as <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException"> Can't convert to derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: float[] toDerivedUnit(float[] input, float[] output) throws ConversionException;
	  float[] toDerivedUnit(float[] input, float[] output);

	  /// <summary>
	  /// Converts numeric values in this unit to the underlying derived unit.
	  /// This method might fail even though <code>convertTo()</code> succeeds.
	  /// </summary>
	  /// <param name="input"> The numeric values in this unit. </param>
	  /// <param name="output"> The equivalent numeric values in the
	  ///        underlying derived unit. May be the same
	  ///        array as <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException"> Can't convert to derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: double[] toDerivedUnit(double[] input, double[] output) throws ConversionException;
	  double[] toDerivedUnit(double[] input, double[] output);

	  /// <summary>
	  /// Converts numeric values from the underlying derived unit to this unit.
	  /// </summary>
	  /// <param name="amount"> The numeric values in the underlying derived
	  ///        unit. </param>
	  /// <returns> The numeric values in this unit. </returns>
	  /// <exception cref="ConversionException"> Can't convert values from underlying
	  ///         derived unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: float fromDerivedUnit(float amount) throws ConversionException;
	  float fromDerivedUnit(float amount);

	  /// <summary>
	  /// Converts a numeric value from the underlying derived unit to this unit.
	  /// This method might fail even though <code>convertTo()</code> succeeds.
	  /// </summary>
	  /// <param name="amount"> The numeric value in the underlying derived
	  ///        unit. </param>
	  /// <returns> The equivalent numeric value in this unit. </returns>
	  /// <exception cref="ConversionException"> Can't convert from underlying derived
	  ///         unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: double fromDerivedUnit(double amount) throws ConversionException;
	  double fromDerivedUnit(double amount);

	  /// <summary>
	  /// Converts numeric values from the underlying derived unit to this unit.
	  /// This method might fail even though <code>convertTo()</code> succeeds.
	  /// </summary>
	  /// <param name="input"> The numeric values in the underlying derived
	  ///        unit. </param>
	  /// <param name="output"> The equivalent numeric values in this unit.
	  ///        May be same array as <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException"> Can't convert from underlying derived
	  ///         unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: float[] fromDerivedUnit(float[] input, float[] output) throws ConversionException;
	  float[] fromDerivedUnit(float[] input, float[] output);

	  /// <summary>
	  /// Converts numeric values from the underlying derived unit to this unit.
	  /// This method might fail even though <code>convertTo()</code> succeeds.
	  /// </summary>
	  /// <param name="input"> The numeric values in the underlying derived
	  ///        unit. </param>
	  /// <param name="output"> The equivalent numeric values in this unit.
	  ///        May be same array as <code>input</code>. </param>
	  /// <returns> <code>output</code>. </returns>
	  /// <exception cref="ConversionException"> Can't convert from underlying derived
	  ///         unit. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: double[] fromDerivedUnit(double[] input, double[] output) throws ConversionException;
	  double[] fromDerivedUnit(double[] input, double[] output);
	}

}
