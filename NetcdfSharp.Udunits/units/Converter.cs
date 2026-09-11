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
	/// Interface for converting numeric values from one unit to another.
	/// 
	/// @author Steven R. Emmerson </summary>
	/// <seealso cref="Unit.getConverterTo(Unit)"/>
	public interface IShimConverter
	{
		/// <summary>
		/// Converts a numeric value.
		/// </summary>
		/// <param name="amount"> The numeric value to convert. </param>
		/// <returns> The converted numeric value. </returns>
		float convert(float amount);

		/// <summary>
		/// Converts a numeric value.
		/// </summary>
		/// <param name="amount"> The numeric value to convert. </param>
		/// <returns> The converted numeric value. </returns>
		double convert(double amount);

		/// <summary>
		/// Converts an array of numeric values.
		/// </summary>
		/// <param name="amounts"> The numeric values to convert. </param>
		/// <returns> The converted numeric values in allocated
		///         space. </returns>
		float[] convert(float[] amounts);

		/// <summary>
		/// Converts an array of numeric values.
		/// </summary>
		/// <param name="amounts"> The numeric values to convert. </param>
		/// <returns> The converted numeric values in allocated
		///         space. </returns>
		double[] convert(double[] amounts);

		/// <summary>
		/// Converts an array of numeric values.
		/// </summary>
		/// <param name="input"> The numeric values to convert. </param>
		/// <param name="output"> The converted numeric values. May be
		///        same array as <code>input</code>. </param>
		/// <returns> <code>output</code>. </returns>
		float[] convert(float[] input, float[] output);

		/// <summary>
		/// Converts an array of numeric values.
		/// </summary>
		/// <param name="input"> The numeric values to convert. </param>
		/// <param name="output"> The converted numeric values. May be
		///        same array as <code>input</code>. </param>
		/// <returns> <code>output</code>. </returns>
		double[] convert(double[] input, double[] output);
	}

}
