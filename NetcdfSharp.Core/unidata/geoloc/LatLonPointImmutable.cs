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
	/// An immutable <seealso cref="LatLonPoint"/>.
	/// </summary>
	/// @deprecated move to implementation classes 
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Deprecated("move to implementation classes") @Immutable public class LatLonPointImmutable extends LatLonPointImpl
	[Obsolete("move to implementation classes"), Serializable]
	public class LatLonPointImmutable : LatLonPointImpl
	{
	  public static readonly LatLonPointImmutable INVALID = new LatLonPointImmutable(double.PositiveInfinity, double.PositiveInfinity);

	  public LatLonPointImmutable(double lat, double lon)
	  {
		this.lat = LatLonPoints.latNormal(lat);
		this.lon = LatLonPoints.lonNormal(lon);
	  }

	  public LatLonPointImmutable(LatLonPoint pt) : this(pt.Latitude, pt.Longitude)
	  {
	  }

	  /// <exception cref="UnsupportedOperationException"> because instances of this class are meant to be immutable. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void setLongitude(double lon) throws UnsupportedOperationException
	  public override double Longitude
	  {
		  set
		  {
			throw new System.NotSupportedException("Instances of this class are meant to be immutable.");
		  }
	  }

	  /// <exception cref="UnsupportedOperationException"> because instances of this class are meant to be immutable. </exception>
	  public override double Latitude
	  {
		  set
		  {
			throw new System.NotSupportedException("Instances of this class are meant to be immutable.");
		  }
	  }
	}

}
