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
	/// Provides support for unit prefixes (e.g. "centi", "c").
	/// 
	/// Instances of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public abstract class Prefix implements Comparable<Object>
	public abstract class Prefix : IComparable<object>
	{
	  /// <summary>
	  /// The value of this prefix.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly double value;

	  /// <summary>
	  /// The identifier of this prefix.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly string id;

	  /// <summary>
	  /// Constructs from an identifier and a value.
	  /// </summary>
	  /// <param name="id">
	  ///        The prefix identifier (e.g. "milli" or "m"). </param>
	  /// <param name="value">
	  ///        The prefix value (e.g. 1e-3). </param>
	  protected internal Prefix(in string id, in double value)
	  {
		this.id = id;
		this.value = value;
	  }

	  /// <summary>
	  /// Gets the prefix identifier.
	  /// </summary>
	  /// <returns> The prefix identifier. </returns>
	  public string ID
	  {
		  get
		  {
			return id;
		  }
	  }

	  /// <summary>
	  /// Returns the string representation of this prefix.
	  /// </summary>
	  /// <returns> The string representation of this prefix. </returns>
	  public override sealed string ToString()
	  {
		return ID;
	  }

	  /// <summary>
	  /// Gets the prefix value.
	  /// </summary>
	  /// <returns> The prefix value. </returns>
	  public double Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  /// <summary>
	  /// Compares this prefix to another.
	  /// </summary>
	  /// <param name="obj">
	  ///        The other prefix. </param>
	  /// <returns> A negative value, zero, or a positive value depending on whether
	  ///         this prefix is less than equal to, or greater than
	  ///         <code>obj</code>. </returns>
	  public abstract int CompareTo(object obj);

	  /// <summary>
	  /// Compares this prefix to a string.
	  /// </summary>
	  /// <param name="string">
	  ///        The string. </param>
	  /// <returns> A negative value, zero, or a positive value depending on whether
	  ///         this prefix is less than equal to, or greater than the string. </returns>
	  public abstract int CompareTo(string @string);

	  /// <summary>
	  /// Return the length of the prefix identifier.
	  /// </summary>
	  /// <returns> The length of the prefix identifier. </returns>
	  public int length()
	  {
		return id.Length;
	  }
	}

}
