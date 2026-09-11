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
	/// A location on the Earth.
	/// 
	/// @author caron </summary>
	/// @deprecated only use EarthLocation in 6. This class will be renamed. 
	[Obsolete("only use EarthLocation in 6. This class will be renamed.")]
	public class EarthLocationImpl : EarthLocation
	{
	  protected internal double lat, lon, alt;

	  public virtual double Latitude
	  {
		  get
		  {
			return lat;
		  }
		  set
		  {
			this.lat = value;
		  }
	  }

	  public virtual double Longitude
	  {
		  get
		  {
			return lon;
		  }
		  set
		  {
			this.lon = value;
		  }
	  }

	  public virtual double Altitude
	  {
		  get
		  {
			return alt;
		  }
		  set
		  {
			this.alt = value;
		  }
	  }

	  public virtual LatLonPoint LatLon
	  {
		  get
		  {
			return LatLonPoint.create(lat, lon);
		  }
	  }

	  public virtual bool Missing
	  {
		  get
		  {
			return double.IsNaN(lat) || double.IsNaN(lon);
		  }
	  }

	  protected internal EarthLocationImpl()
	  {
	  }

	  /// @deprecated use EarthLocation.create 
	  [Obsolete("use EarthLocation.create")]
	  public EarthLocationImpl(double lat, double lon, double alt)
	  {
		this.lat = lat;
		this.lon = lon;
		this.alt = alt;
	  }




	  public override string ToString()
	  {
		return "lat=" + lat + " lon=" + lon + " alt=" + alt;
	  }


	}

}
