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
	/// Provides support for implementations of unit converters. A unit converter
	/// class may be created by subclassing this class and implementing the methods
	/// <code>convert(double)</code>, <code>convert(float[] input, float[] output)
	/// </code>, and <code>convert(double[] input, double[] output)</code> of
	/// interface <code>Converter</code>.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public abstract class ConverterImpl : IShimConverter
	{
		public abstract double[] convert(double[] input, double[] output);
		public abstract float[] convert(float[] input, float[] output);
		public abstract double convert(double amount);
		/// <summary>
		/// Constructs from a "from" unit and a "to" unit.
		/// </summary>
		/// <param name="fromUnit"> The unit from which to convert. </param>
		/// <param name="toUnit"> The unit to which to convert. </param>
		/// <exception cref="ConversionException"> The units are not convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected ConverterImpl(Unit fromUnit, Unit toUnit) throws ConversionException
		protected internal ConverterImpl(Unit fromUnit, Unit toUnit)
		{
			if (!fromUnit.isCompatible(toUnit))
			{
				throw new ConversionException(fromUnit, toUnit);
			}
		}

		/// <summary>
		/// Factory method for creating a unit converter.
		/// </summary>
		/// <param name="fromUnit"> The unit from which to convert. </param>
		/// <param name="toUnit"> The unit to which to convert. </param>
		/// <exception cref="ConversionException"> The units are not convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Converter create(Unit fromUnit, Unit toUnit) throws ConversionException
		public static IShimConverter create(Unit fromUnit, Unit toUnit)
		{
			return fromUnit.getConverterTo(toUnit);
		}

		/// <summary>
		/// Converts a numeric value.
		/// </summary>
		/// <param name="amount"> The numeric value to convert. </param>
		/// <returns> The converted numeric value. </returns>
		public float convert(float amount)
		{
			return (float)convert((double)amount);
		}

		/// <summary>
		/// Converts an array of numeric values.
		/// </summary>
		/// <param name="amounts"> The numeric values to convert. </param>
		/// <returns> The converted numeric values in allocated
		///         space. </returns>
		public float[] convert(float[] amounts)
		{
			return convert(amounts, new float[amounts.Length]);
		}

		/// <summary>
		/// Converts an array of numeric values.
		/// </summary>
		/// <param name="amounts"> The numeric values to convert. </param>
		/// <returns> The converted numeric values in allocated
		///         space. </returns>
		public double[] convert(double[] amounts)
		{
			return convert(amounts, new double[amounts.Length]);
		}
	}

}
