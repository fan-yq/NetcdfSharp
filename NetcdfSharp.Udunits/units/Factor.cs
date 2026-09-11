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
	/// Provides support for a Base/exponent pair. Instances of this class are
	/// immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public final class Factor implements NetcdfSharp.java.io.Serializable
	[Serializable]
	public sealed class Factor
	{
	  private const long serialVersionUID = 1L;

	  /// <summary>
	  /// The Base entity.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly Base _base;

	  /// <summary>
	  /// The exponent.
	  /// 
	  /// @serial
	  /// </summary>
	  private readonly int _exponent;

	  /// <summary>
	  /// Constructs from a Base. The exponent is set to unity.
	  /// </summary>
	  /// <param name="base">
	  ///        The base entity. </param>
	  public Factor(in Base @base) : this(@base, 1)
	  {
	  }

	  /// <summary>
	  /// Constructs from a Factor and an exponent.
	  /// </summary>
	  /// <param name="factor">
	  ///        The factor. </param>
	  /// <param name="exponent">
	  ///        The exponent. </param>
	  public Factor(in Factor factor, in int exponent) : this(factor.Base, exponent)
	  {
	  }

	  /// <summary>
	  /// Constructs from a Base and an exponent.
	  /// </summary>
	  /// <param name="base">
	  ///        The base entity. </param>
	  /// <param name="exponent">
	  ///        The exponent. </param>
	  public Factor(in Base @base, in int exponent)
	  {
		_base = @base;
		_exponent = exponent;
	  }

	  /// <summary>
	  /// Returns the Base entity.
	  /// </summary>
	  /// <returns> The Base entity. </returns>
	  public Base Base
	  {
		  get
		  {
			return _base;
		  }
	  }

	  /// <summary>
	  /// Returns the identifier of the Base entity.
	  /// </summary>
	  /// <returns> The identifier of the Base entity (symbol or name). </returns>
	  public string ID
	  {
		  get
		  {
			return Base.ID;
		  }
	  }

	  /// <summary>
	  /// Returns the exponent of the Base entity.
	  /// </summary>
	  /// <returns> The exponent of the Base entity. </returns>
	  public int Exponent
	  {
		  get
		  {
			return _exponent;
		  }
	  }

	  /// <summary>
	  /// Raises this Factor to a power.
	  /// </summary>
	  /// <param name="power">
	  ///        The power by which to raise this Factor. </param>
	  /// <returns> The Factor resulting from raising this Factor to the given power. </returns>
	  public Factor pow(in int power)
	  {
		return new Factor(Base, Exponent * power);
	  }

	  /// <summary>
	  /// Returns the string representation of this Factor.
	  /// </summary>
	  /// <returns> The string representation of this Factor. </returns>
	  public override string ToString()
	  {
		return Exponent == 0 ? "" : Exponent == 1 ? Base.ToString() : Base.ToString() + Exponent;
	  }

	  /// <summary>
	  /// Indicates if this Factor is semantically identical to another object.
	  /// </summary>
	  /// <param name="object">
	  ///        The object. </param>
	  /// <returns> <code>true</code> if and only if this Factor is semantically
	  ///         identical to <code>object<
	  ///         /code>. </returns>
	  public override bool Equals(object @object)
	  {
		bool equals;
		if (this == @object)
		{
		  equals = true;
		}
		else if (!(@object is Factor))
		{
		  equals = false;
		}
		else
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Factor that = (Factor) object;
		  Factor that = (Factor) @object;
		  equals = Exponent == that.Exponent && (Exponent == 0 || Base.Equals(that.Base));
		}
		return equals;
	  }

	  /// <summary>
	  /// Returns the hash code of this instance.
	  /// </summary>
	  /// <returns> The hash code of this instance. </returns>
	  public override int GetHashCode()
	  {
		return Exponent == 0 ? this.GetType().GetHashCode() : Exponent ^ Base.GetHashCode();
	  }

	  /// <summary>
	  /// Indicates if this Factor is the reciprocal of another Factor.
	  /// </summary>
	  /// <param name="that">
	  ///        The other factor. </param>
	  /// <returns> <code>true</code> if and only if this Factor is the reciprocal of
	  ///         <code>that</code>. </returns>
	  public bool isReciprocalOf(in Factor that)
	  {
		return Base.Equals(that.Base) && Exponent == -that.Exponent;
	  }

	  /// <summary>
	  /// Indicates if this factor is dimensionless. A Factor is dimensionless if
	  /// and only if the exponent is zero or the Base entity is dimensionless.
	  /// </summary>
	  /// <returns> <code>true</code> if and only if this Factor is dimensionless. </returns>
	  public bool Dimensionless
	  {
		  get
		  {
			return Exponent == 0 || Base.Dimensionless;
		  }
	  }
	}

}
