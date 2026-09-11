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
	/// Provides support for prefix symbols.
	/// 
	/// Instances of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class PrefixSymbol extends Prefix
	public sealed class PrefixSymbol : Prefix
	{
	  /// <summary>
	  /// Constructs from a name and a numeric value.
	  /// </summary>
	  /// <param name="name"> The name for the prefix. </param>
	  /// <param name="value"> The numeric value for the prefix. </param>
	  public PrefixSymbol(string name, double value) : base(name, value)
	  {
	  }

	  /// <summary>
	  /// Compares this prefix against another PrefixSymbol. The sort keys are
	  /// decreasing length (major) and increasing lexicality (minor).
	  /// </summary>
	  /// <param name="obj"> The other PrefixSymbol. </param>
	  /// <returns> A negative value, zero, or a positive value
	  ///         depending on whether this PrefixSymbol is less
	  ///         than, equal to, or greater than <code>
	  ///        obj</code>, respectively. </returns>
	  public override int CompareTo(object obj)
	  {
		string thatID = ((PrefixSymbol) obj).ID;
		int comp = thatID.Length - ID.Length;
		if (comp == 0)
		{
		  comp = string.CompareOrdinal(ID, thatID);
		}
		return comp;
	  }

	  /// <summary>
	  /// Compares this prefix against a String. The sort keys are
	  /// decreasing length (major) and increasing lexicality (minor).
	  /// </summary>
	  /// <param name="string"> The string. </param>
	  /// <returns> A negative value, zero, or a positive value
	  ///         depending on whether this PrefixSymbol is less
	  ///         than, equal to, or greater than <code>
	  ///        string</code>, respectively. </returns>
	  public override int CompareTo(string @string)
	  {
		int comp = @string.Length - ID.Length;
		return comp < 0 ? comp : comp == 0 ? (string.CompareOrdinal(ID, @string) == 0 ? 0 : -1) : (string.CompareOrdinal(ID, @string.Substring(0, ID.Length)) == 0 ? 0 : -1);
	  }
	}

}
