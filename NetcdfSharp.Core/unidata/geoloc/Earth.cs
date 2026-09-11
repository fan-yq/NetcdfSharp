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

namespace NetcdfSharp.ucar.unidata.geoloc
{

	/// <summary>
	/// Defines the shape of the earth ellipsoid.
	/// 
	/// LOOK will not implement Serializable in ver6
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Earth implements NetcdfSharp.java.io.Serializable
	[Serializable]
	public class Earth
	{
	  public static readonly Earth DEFAULT = new Earth();

	  /// <summary>
	  /// Get canonical radius of the spherical earth in meters from "WGS 84" </summary>
	  public const double WGS84_EARTH_RADIUS_METERS = 6371229.0;

	  /// <summary>
	  /// Get canonical radius of the spherical earth in km from "WGS 84" </summary>
	  public const double WGS84_EARTH_RADIUS_KM = 6371.229;

	  ///////////////////////////////////////////////////////////////////////////////////
	  protected internal readonly double eccentricity; // eccentricity
	  protected internal readonly double eccentricitySquared; // eccentricity squared
	  protected internal readonly double equatorRadius; // equatorial radius (semimajor axis)
	  protected internal readonly double poleRadius; // polar radius (semiminor axis)
	  protected internal readonly double flattening; // flattening
	  protected internal readonly string name;

	  /// <summary>
	  /// Spherical earth with canonical radius.
	  /// </summary>
	  public Earth() : this(WGS84_EARTH_RADIUS_METERS)
	  {
	  }

	  /// <summary>
	  /// Create a spherical earth.
	  /// </summary>
	  /// <param name="radius"> radius of spherical earth. </param>
	  public Earth(double radius)
	  {
		this.equatorRadius = radius;
		this.poleRadius = radius;
		this.flattening = 0.0;
		this.eccentricity = 1.0;
		this.eccentricitySquared = 1.0;
		this.name = "spherical_earth";
	  }

	  /// <summary>
	  /// Create an ellipsoidal earth.
	  /// The reciprocalFlattening is used if not zero, else the poleRadius is used.
	  /// </summary>
	  /// <param name="equatorRadius"> equatorial radius (semimajor axis) in meters, must be specified </param>
	  /// <param name="poleRadius"> polar radius (semiminor axis) in meters </param>
	  /// <param name="reciprocalFlattening"> inverse flattening = 1/flattening = a / (a-b) </param>
	  public Earth(double equatorRadius, double poleRadius, double reciprocalFlattening) : this(equatorRadius, poleRadius, reciprocalFlattening, "ellipsoidal_earth")
	  {
	  }


	  /// <summary>
	  /// Create an ellipsoidal earth with a name.
	  /// </summary>
	  /// <param name="equatorRadius"> equatorial radius (semimajor axis) in meters, must be specified </param>
	  /// <param name="poleRadius"> polar radius (semiminor axis) in meters; if reciprocalFlattening != 0 then this is ignored </param>
	  /// <param name="reciprocalFlattening"> inverse flattening = 1/flattening = a / (a-b); if 0 use the poleRadius to calculate </param>
	  /// <param name="name"> name of the Earth </param>
	  public Earth(double equatorRadius, double poleRadius, double reciprocalFlattening, string name)
	  {
		this.equatorRadius = equatorRadius;
		this.name = name;
		if (reciprocalFlattening != 0)
		{
		  flattening = 1.0 / reciprocalFlattening;
		  eccentricitySquared = 2 * flattening - flattening * flattening;
		  this.poleRadius = equatorRadius * Math.Sqrt(1.0 - eccentricitySquared);
		}
		else
		{
		  this.poleRadius = poleRadius;
		  eccentricitySquared = 1.0 - (poleRadius * poleRadius) / (equatorRadius * equatorRadius);
		  flattening = 1.0 - poleRadius / equatorRadius;
		}
		eccentricity = Math.Sqrt(eccentricitySquared);
	  }

	  public virtual bool Spherical
	  {
		  get
		  {
			return flattening == 0.0;
		  }
	  }

	  /// <summary>
	  /// Get the equatorial radius (semimajor axis) of the earth, in meters.
	  /// </summary>
	  /// <returns> equatorial radius (semimajor axis) in meters </returns>
	  public virtual double Major
	  {
		  get
		  {
			return equatorRadius;
		  }
	  }

	  /// <summary>
	  /// Get the polar radius (semiminor axis) of the earth, in meters.
	  /// </summary>
	  /// <returns> polar radius (semiminor axis) in meters </returns>
	  public virtual double Minor
	  {
		  get
		  {
			return poleRadius;
		  }
	  }

	  /// <summary>
	  /// Get the Name property.
	  /// </summary>
	  /// <returns> The Name </returns>
	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  /// <summary>
	  /// Get the Eccentricity property.
	  /// </summary>
	  /// <returns> The Eccentricity </returns>
	  public virtual double Eccentricity
	  {
		  get
		  {
			return eccentricity;
		  }
	  }

	  /// <summary>
	  /// Get the EccentricitySquared property.
	  /// </summary>
	  /// <returns> The EccentricitySquared </returns>
	  public virtual double EccentricitySquared
	  {
		  get
		  {
			return eccentricitySquared;
		  }
	  }

	  /// <summary>
	  /// Get the EquatorRadius property.
	  /// </summary>
	  /// <returns> The EquatorRadius </returns>
	  public virtual double EquatorRadius
	  {
		  get
		  {
			return equatorRadius;
		  }
	  }

	  /// <summary>
	  /// Get the PoleRadius property.
	  /// </summary>
	  /// <returns> The PoleRadius </returns>
	  public virtual double PoleRadius
	  {
		  get
		  {
			return poleRadius;
		  }
	  }

	  /// <summary>
	  /// Get the Flattening property.
	  /// </summary>
	  /// <returns> The Flattening </returns>
	  public virtual double Flattening
	  {
		  get
		  {
			return flattening;
		  }
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		Earth earth = (Earth) o;

		if (earth.equatorRadius.CompareTo(equatorRadius) != 0)
		{
		  return false;
		}
		return earth.poleRadius.CompareTo(poleRadius) == 0;
		// if (name != null ? !name.equals(earth.name) : earth.name != null) return false;

	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		temp = equatorRadius != 0.0d ? System.BitConverter.DoubleToInt64Bits(equatorRadius) : 0L;
		result = (int)(temp ^ (temp >>> 32));
		temp = poleRadius != 0.0d ? System.BitConverter.DoubleToInt64Bits(poleRadius) : 0L;
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		result = 31 * result + (!string.ReferenceEquals(name, null) ? name.GetHashCode() : 0);
		return result;
	  }

	  public override string ToString()
	  {
		Formatter ff = new Formatter();
		ff.format("equatorRadius=%f inverseFlattening=%f", equatorRadius, (1.0 / flattening));
		return ff.ToString();
	  }

	}


}
