// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc
{

	/// <summary>
	/// Implementation of Station
	/// 
	/// @author caron </summary>
	/// @deprecated only use Station in 6. 
	[Obsolete("only use Station in 6.")]
	public class StationImpl : EarthLocationImpl, Station
	{
	  protected internal string name, desc, wmoId;
	  protected internal int nobs = -1;

	  protected internal StationImpl()
	  {
	  }

	  public StationImpl(string name, string desc, string wmoId, double lat, double lon, double alt) : base(lat, lon, alt)
	  {
		Name = name;
		Description = desc;
		WmoId = wmoId;
	  }

	  public StationImpl(string name, string desc, string wmoId, double lat, double lon, double alt, int nobs) : base(lat, lon, alt)
	  {
		Name = name;
		Description = desc;
		WmoId = wmoId;
		Nobs = nobs;
	  }

	  public StationImpl(Station s, int nobs) : base(s.Latitude, s.Longitude, s.Altitude)
	  {
		Name = s.Name;
		Description = s.Description;
		WmoId = s.WmoId;
		Nobs = nobs;
	  }

	  /// <summary>
	  /// Station name or id. Must be unique within the collection
	  /// </summary>
	  /// <returns> station name or id. May not be null. </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nonnull public String getName()
	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value.Trim();
		  }
	  }

	  /// <summary>
	  /// Station description
	  /// </summary>
	  /// <returns> station description </returns>
	  public virtual string Description
	  {
		  get
		  {
			return desc;
		  }
		  set
		  {
			this.desc = !string.ReferenceEquals(value, null) ? value.Trim() : null;
		  }
	  }

	  /// <summary>
	  /// WMO station id
	  /// </summary>
	  /// <returns> WMO station id, or null </returns>
	  public virtual string WmoId
	  {
		  get
		  {
			return wmoId;
		  }
		  set
		  {
			this.wmoId = !string.ReferenceEquals(value, null) ? value.Trim() : null;
		  }
	  }

	  public virtual int Nobs
	  {
		  get
		  {
			return nobs;
		  }
		  set
		  {
			this.nobs = value;
		  }
	  }

	  /////





	  public virtual void incrNobs()
	  {
		this.nobs++;
	  }

	  public virtual int CompareTo(Station so)
	  {
		return string.CompareOrdinal(name, so.Name);
	  }

	  public override string ToString()
	  {
		return "name=" + name + " desc=" + desc + " " + base.ToString();
	  }

	}

}
