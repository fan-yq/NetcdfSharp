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
	/// Provides support for prefix names and numeric values.
	/// 
	/// Instances of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class PrefixName extends Prefix
	public sealed class PrefixName : Prefix
	{
	  /// <summary>
	  /// Constructs from a name and a numeric value.
	  /// </summary>
	  /// <param name="name"> The name for the prefix. </param>
	  /// <param name="value"> The numeric value for the prefix. </param>
	  public PrefixName(string name, double value) : base(name, value)
	  {
	  }

	  /// <summary>
	  /// Compares this PrefixName with another PrefixName.
	  /// </summary>
	  /// <param name="obj"> The other PrefixName. </param>
	  /// <returns> A negative value, zero, or a positive value
	  ///         depending on whether this PrefixName is less
	  ///         than, equal to, or greater than <code>
	  ///        obj</code>, respectively. </returns>
	  public override int CompareTo(object obj)
	  {
		return string.Compare(ID, ((PrefixName) obj).ID, StringComparison.OrdinalIgnoreCase);
	  }

	  /// <summary>
	  /// Compares this PrefixName with a string.
	  /// </summary>
	  /// <param name="string"> The string to compare this PrefixName against. </param>
	  /// <returns> A negative value, zero, or a positive value
	  ///         depending on whether this PrefixName is less
	  ///         than, equal to, or greater than <code>
	  ///        string</code>, respectively. </returns>
	  public override int CompareTo(string @string)
	  {
		return ID.Length >= @string.Length ? string.Compare(ID, @string, StringComparison.OrdinalIgnoreCase) : string.Compare(ID, @string.Substring(0, ID.Length), StringComparison.OrdinalIgnoreCase);
	  }
	}

}
