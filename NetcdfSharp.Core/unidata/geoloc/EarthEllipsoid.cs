// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc
{

	/// <summary>
	/// Type-safe enumeration of Earth Ellipsoids. Follows EPSG.
	/// 
	/// @author john caron </summary>
	/// <seealso cref="<a href="http://www.epsg.org/">http://www.epsg.org/</a>"/>
	/// <seealso cref=""http://www.epsg.org/""/>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class EarthEllipsoid extends Earth
	[Serializable]
	public class EarthEllipsoid : Earth
	{

	  // must be done first, as other static fields depend on it
	  private static readonly IDictionary<string, EarthEllipsoid> hash = new NetcdfSharp.java.util.LinkedHashMap<string, EarthEllipsoid>(10);

	  /// <summary>
	  /// Ellipsoid for WGS84 (edavis - correct 1/f as per EPSG database ver 6.14)
	  /// </summary>
	  public static readonly EarthEllipsoid WGS84 = new EarthEllipsoid("WGS84", 7030, 6378137.0, 298.257223563);

	  /// <summary>
	  /// Airy 1830 ellipsoid from EPSG database version 6.14.
	  /// </summary>
	  public static readonly EarthEllipsoid Airy1830 = new EarthEllipsoid("Airy 1830", 7001, 6377563.396, 299.3249646);

	  // From Grib1: IAU in 1965 (6378.160 km, 6356.775 km, f = 1/297.0)
	  // From Grib2: 2 oblate spheroid with size as determined by IAU in 1965 (major axis = 6 378 160.0 m, minor axis = 6
	  // 356 775.0 m, f = 1/297.0)
	  public static readonly EarthEllipsoid IAU = new EarthEllipsoid("IAU 1965", -1, 6378160.0, 297.0);

	  // From Grib2: 4: oblate spheroid as defined in IAG-GRS80 model (major axis = 6 378 137.0 m, minor axis = 6 356
	  // 752.314 m, f = 1/298.257 222 101)
	  public static readonly EarthEllipsoid IAG_GRS80 = new EarthEllipsoid("IIAG-GRS80", -1, 6378137.0, 298.257222101);


	  /// <summary>
	  /// get a collection of all defined EarthEllipsoid objects
	  /// </summary>
	  /// <returns> all defined EarthEllipsoid objects </returns>
	  public static ICollection<EarthEllipsoid> All
	  {
		  get
		  {
			return hash.Values;
		  }
	  }


	  /// <summary>
	  /// Find the EarthEllipsoid that matches this name.
	  /// </summary>
	  /// <param name="name"> : name to match </param>
	  /// <returns> EarthEllipsoid or null if no match. </returns>
	  public static EarthEllipsoid getType(string name)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  return null;
		}
		return hash[name];
	  }

	  /// <summary>
	  /// Find the EarthEllipsoid that matches this EPSG Id.
	  /// </summary>
	  /// <param name="epsgId"> : epsg Id to match </param>
	  /// <returns> EarthEllipsoid or null if no match. </returns>
	  public static EarthEllipsoid getType(int epsgId)
	  {
		ICollection<EarthEllipsoid> all = All;
		foreach (EarthEllipsoid ellipsoid in all)
		{
		  if (ellipsoid.epsgId == epsgId)
		  {
			return ellipsoid;
		  }
		}
		return null;
	  }

	  ///////////////////////////////////////////////////////////
	  private readonly int epsgId;

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="name"> EPSG name </param>
	  /// <param name="epsgId"> EPSG id </param>
	  /// <param name="a"> semimajor (equatorial) radius, in meters. </param>
	  /// <param name="invF"> inverse flattening. </param>
	  private EarthEllipsoid(string name, int epsgId, double a, double invF) : this(name, epsgId, a, 0, invF)
	  {
	  }

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="name"> EPSG name </param>
	  /// <param name="epsgId"> EPSG id </param>
	  /// <param name="a"> semimajor (equatorial) radius, in meters. </param>
	  /// <param name="b"> semiminor (polar) radius, in meters. </param>
	  /// <param name="invF"> inverse flattening. </param>
	  public EarthEllipsoid(string name, int epsgId, double a, double b, double invF) : base(a, b, invF, name)
	  {
		this.epsgId = epsgId;
		hash[name] = this;
	  }


	  /// <summary>
	  /// EPSG name
	  /// </summary>
	  /// <returns> the EPSG name </returns>
	  public override string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  /// <summary>
	  /// EPSG id
	  /// </summary>
	  /// <returns> the EPSG id </returns>
	  public virtual int EpsgId
	  {
		  get
		  {
			return epsgId;
		  }
	  }

	  /// <summary>
	  /// Same as EPSG name
	  /// </summary>
	  /// <returns> the EPSG name </returns>
	  public override string ToString()
	  {
		return name;
	  }

	  /// <summary>
	  /// Override Object.hashCode() to be consistent with this equals.
	  /// </summary>
	  /// <returns> hashCode </returns>
	  public override int GetHashCode()
	  {
		return name.GetHashCode();
	  }

	  /// <summary>
	  /// Objects with same name are equal.
	  /// </summary>
	  /// <param name="o"> test this for equals </param>
	  /// <returns> +,0,- </returns>
	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (!(o is EarthEllipsoid))
		{
		  return false;
		}
		EarthEllipsoid oe = (EarthEllipsoid) o;
		return oe.name.Equals(name);
	  }
	}


}
