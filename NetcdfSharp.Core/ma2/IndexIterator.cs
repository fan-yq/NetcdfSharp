// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// Iteration through each element of an Array in "canonical order".
	/// The user obtains this by calling getIndexIterator() on an Array.
	/// <p/>
	/// Canonical order for A[i][j][k] has k varying fastest, then j, then i.<br>
	/// <para>
	/// Example: Replace array with its square:
	/// <br>
	/// 
	/// <pre>
	/// IndexIterator iter = A.getIndexIterator();
	/// while (iter.hasNext()) {
	///   double val = iter.getDoubleNext();
	///   iter.setDoubleCurrent(val * val);
	/// }
	/// </pre>
	/// <p/>
	/// Note that canonical order may not be physical order.
	/// 
	/// @author caron
	/// </para>
	/// </summary>

	public interface IndexIterator
	{

	  /// <summary>
	  /// Return true if there are more elements in the iteration.
	  /// </summary>
	  /// <returns> true if there are more elements in the iteration. </returns>
	  bool hasNext();

	  /// <summary>
	  /// Get next value as a double
	  /// </summary>
	  /// <returns> next value as a double </returns>
	  double DoubleNext {get;set;}


	  /// <summary>
	  /// Get current value as a double
	  /// </summary>
	  /// <returns> current value as a double </returns>
	  double DoubleCurrent {get;set;}


	  /// <summary>
	  /// Get next value as a float
	  /// </summary>
	  /// <returns> next value as a float </returns>
	  float FloatNext {get;set;}


	  /// <summary>
	  /// Get current value as a float
	  /// </summary>
	  /// <returns> current value as a float </returns>
	  float FloatCurrent {get;set;}


	  /// <summary>
	  /// Get next value as a long
	  /// </summary>
	  /// <returns> next value as a long </returns>
	  long LongNext {get;set;}


	  /// <summary>
	  /// Get current value as a long
	  /// </summary>
	  /// <returns> current value as a long </returns>
	  long LongCurrent {get;set;}


	  /// <summary>
	  /// Get next value as a int
	  /// </summary>
	  /// <returns> next value as a int </returns>
	  int IntNext {get;set;}


	  /// <summary>
	  /// Get current value as a int
	  /// </summary>
	  /// <returns> current value as a int </returns>
	  int IntCurrent {get;set;}


	  /// <summary>
	  /// Get next value as a short
	  /// </summary>
	  /// <returns> next value as a short </returns>
	  short ShortNext {get;set;}


	  /// <summary>
	  /// Get current value as a short
	  /// </summary>
	  /// <returns> current value as a short </returns>
	  short ShortCurrent {get;set;}


	  /// <summary>
	  /// Get next value as a byte
	  /// </summary>
	  /// <returns> next value as a byte </returns>
	  byte ByteNext {get;set;}


	  /// <summary>
	  /// Get current value as a byte
	  /// </summary>
	  /// <returns> current value as a byte </returns>
	  byte ByteCurrent {get;set;}


	  /// <summary>
	  /// Get next value as a char
	  /// </summary>
	  /// <returns> next value as a char </returns>
	  char CharNext {get;set;}


	  /// <summary>
	  /// Get current value as a char
	  /// </summary>
	  /// <returns> current value as a char </returns>
	  char CharCurrent {get;set;}


	  /// <summary>
	  /// Get next value as a boolean
	  /// </summary>
	  /// <returns> next value as a boolean </returns>
	  bool BooleanNext {get;set;}


	  /// <summary>
	  /// Get current value as a boolean
	  /// </summary>
	  /// <returns> current value as a boolean </returns>
	  bool BooleanCurrent {get;set;}


	  /// <summary>
	  /// Get next value as an Object
	  /// </summary>
	  /// <returns> next value as an Object </returns>
	  object ObjectNext {get;set;}


	  /// <summary>
	  /// Get current value as a Object
	  /// </summary>
	  /// <returns> current value as a Object </returns>
	  object ObjectCurrent {get;set;}


	  /// <summary>
	  /// Get next value as an Object
	  /// </summary>
	  /// <returns> next value as an Object </returns>
	  object next();

	  /// <summary>
	  /// Get the current counter, use for debugging
	  /// </summary>
	  /// <returns> the current counter, use for debugging </returns>
	  int[] CurrentCounter {get;}
	}

}
