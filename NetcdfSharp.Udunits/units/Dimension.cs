// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{
	/// <summary>
	/// Provides support for the concept of "dimension": pairs of base entities and
	/// exponents.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	public abstract class Dimension
	{
	  /// <summary>
	  /// The individual elements of this dimension.
	  /// 
	  /// @serial
	  /// </summary>
	  protected internal readonly Factor[] _factors;

	  [NonSerialized]
	  private volatile int hashCode;

	  /// <summary>
	  /// Constructs a dimensionless dimension from nothing.
	  /// </summary>
	  public Dimension() : this(new Factor[0])
	  {
	  }

	  /// <summary>
	  /// Constructs from a single Factor.
	  /// </summary>
	  /// <param name="factor">
	  ///        The single Factor that defines the dimension. </param>
	  protected internal Dimension(in Factor factor) : this(new Factor[] {factor})
	  {
	  }

	  /// <summary>
	  /// Constructs from an array of Factor-s. This is a trusted constructor for
	  /// use by subclasses only.
	  /// </summary>
	  /// <param name="factors">
	  ///        The factors that define the dimension. </param>
	  protected internal Dimension(in Factor[] factors)
	  {
		_factors = factors;
	  }

	  /// <summary>
	  /// Returns the rank of this dimension. The rank is the number of base entity
	  /// and exponent pairs (i.e. the number of Factor-s constituting this
	  /// dimension).
	  /// </summary>
	  /// <returns> The rank of this dimension. </returns>
	  public int Rank
	  {
		  get
		  {
			return _factors.Length;
		  }
	  }

	  /// <summary>
	  /// Returns the array of Factor-s constituting this dimension.
	  /// </summary>
	  /// <returns> The array of Factor-s constituting this dimension. </returns>
	  public Factor[] Factors
	  {
		  get
		  {
	// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	// ORIGINAL LINE: final Factor[] factors = new Factor[_factors.length];
			Factor[] factors = new Factor[_factors.Length];
			System.Array.Copy(_factors, 0, factors, 0, factors.Length);
			return factors;
		  }
	  }

	  /// <summary>
	  /// Multiplies this dimension by another dimension.
	  /// </summary>
	  /// <param name="that">
	  ///        The other dimension. </param>
	  /// <returns> The product of the Factor-s of this dimension and the Factor-s of
	  ///         the other dimension. </returns>
	  protected internal virtual Factor[] mult(in Dimension that)
	  {
		// relys on _factors always sorted
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor[] factors1 = _factors;
		Factor[] factors1 = _factors;
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor[] factors2 = that._factors;
		Factor[] factors2 = that._factors;
		int i1 = 0;
		int i2 = 0;
		int k = 0;
		Factor[] newFactors = new Factor[factors1.Length + factors2.Length];
		for (;;)
		{
		  if (i1 == factors1.Length)
		  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final int n = factors2.length - i2;
			int n = factors2.Length - i2;
			System.Array.Copy(factors2, i2, newFactors, k, n);
			k += n;
			break;
		  }
		  if (i2 == factors2.Length)
		  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final int n = factors1.length - i1;
			int n = factors1.Length - i1;
			System.Array.Copy(factors1, i1, newFactors, k, n);
			k += n;
			break;
		  }
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor f1 = factors1[i1];
		  Factor f1 = factors1[i1];
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor f2 = factors2[i2];
		  Factor f2 = factors2[i2];
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final int comp = f1.getID().compareTo(f2.getID());
		  int comp = string.CompareOrdinal(f1.ID, f2.ID);
		  if (comp < 0)
		  {
			newFactors[k++] = f1;
			i1++;
		  }
		  else if (comp == 0)
		  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final int exponent = f1.getExponent() + f2.getExponent();
			int exponent = f1.Exponent + f2.Exponent;
			if (exponent != 0)
			{
			  newFactors[k++] = new Factor(f1, exponent);
			}
			i1++;
			i2++;
		  }
		  else
		  {
			newFactors[k++] = f2;
			i2++;
		  }
		}
		if (k < newFactors.Length)
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor[] tmp = new Factor[k];
		  Factor[] tmp = new Factor[k];
		  System.Array.Copy(newFactors, 0, tmp, 0, k);
		  newFactors = tmp;
		}
		return newFactors;
	  }

	  /// <summary>
	  /// Raises this dimension to a power.
	  /// </summary>
	  /// <param name="power">
	  ///        The power to raise this dimension by. </param>
	  /// <returns> The Factor-s of this dimension raised to the power
	  ///         <code>power</code>. </returns>
	  protected internal virtual Factor[] pow(in int power)
	  {
		Factor[] factors;
		if (power == 0)
		{
		  factors = new Factor[0];
		}
		else
		{
		  factors = Factors;
		  if (power != 1)
		  {
			for (int i = factors.Length; --i >= 0;)
			{
			  factors[i] = factors[i].pow(power);
			}
		  }
		}
		return factors;
	  }

	  /// <summary>
	  /// Indicates if this Dimension is the reciprocal of another dimension.
	  /// </summary>
	  /// <param name="that">
	  ///        The other dimension. </param>
	  /// <returns> <code>true</code> if and only if this dimension is the reciprocal
	  ///         of the other dimension. </returns>
	  public bool isReciprocalOf(in Dimension that)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor[] theseFactors = _factors;
		Factor[] theseFactors = _factors;
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor[] thoseFactors = that._factors;
		Factor[] thoseFactors = that._factors;
		bool isReciprocalOf;
		if (theseFactors.Length != thoseFactors.Length)
		{
		  isReciprocalOf = false;
		}
		else
		{
		  int i;
		  for (i = theseFactors.Length; --i >= 0;)
		  {
			if (!theseFactors[i].isReciprocalOf(thoseFactors[i]))
			{
			  break;
			}
		  }
		  isReciprocalOf = i < 0;
		}
		return isReciprocalOf;
	  }

	  /// <summary>
	  /// Indicates if this dimension is semantically identical to an object.
	  /// </summary>
	  /// <param name="object">
	  ///        The object. </param>
	  /// <returns> <code>true</code> if and only if this dimension is semantically
	  ///         identical to <code>object</code>. </returns>
	  public override sealed bool Equals(object @object)
	  {
		bool equals;
		if (this == @object)
		{
		  equals = true;
		}
		else if (!(@object is Dimension))
		{
		  equals = false;
		}
		else
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor[] thatFactors = ((Dimension) object)._factors;
		  Factor[] thatFactors = ((Dimension) @object)._factors;
		  if (_factors.Length != thatFactors.Length)
		  {
			equals = false;
		  }
		  else
		  {
			int i = _factors.Length;
			while (--i >= 0)
			{
			  if (!_factors[i].Equals(thatFactors[i]))
			  {
				break;
			  }
			}
			equals = i < 0;
		  }
		}
		return equals;
	  }

	  /// <summary>
	  /// Returns the hash code of this instance.
	  /// </summary>
	  /// <returns> The hash code of this instance. </returns>
	  public override int GetHashCode()
	  {
		if (hashCode == 0)
		{
		  int hash = 0;
		  foreach (Factor factor in _factors)
		  {
			hash ^= factor.GetHashCode();
		  }
		  hashCode = hash;
		}
		return hashCode;
	  }

	  /// <summary>
	  /// Indicates if this dimension is dimensionless. A dimension is
	  /// dimensionless if it has no Factor-s or if all Factor-s are, themselves,
	  /// dimensionless.
	  /// </summary>
	  /// <returns> <code>true</code> if and only if this dimension is dimensionless. </returns>
	  public bool Dimensionless
	  {
		  get
		  {
			for (int i = _factors.Length; --i >= 0;)
			{
			  if (!_factors[i].Dimensionless)
			  {
				return false;
			  }
			}
			return true;
		  }
	  }

	  /// <summary>
	  /// Returns the string representation of this dimension.
	  /// </summary>
	  /// <returns> The string representation of this dimension. </returns>
	  public override string ToString()
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final StringBuilder buf = new StringBuilder(40);
		StringBuilder buf = new StringBuilder(40);
		foreach (Factor factor in _factors)
		{
		  buf.Append(factor).Append('.');
		}
		if (buf.Length != 0)
		{
		  buf.Length = buf.Length - 1;
		}
		return buf.ToString();
	  }
	}

}
