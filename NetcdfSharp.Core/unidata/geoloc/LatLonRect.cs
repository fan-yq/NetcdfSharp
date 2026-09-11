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
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;
	using Format = NetcdfSharp.ucar.unidata.util.Format;

	/// <summary>
	/// Bounding box for latitude/longitude points.
	/// This is a rectangle in lat/lon coordinates.
	/// This class handles the longitude wrapping problem.
	/// The Rectangle always starts from lowerLeft, goes east width degrees until upperRight
	/// For latitude, lower < upper.
	/// Since the longitude must be in the range +/-180., right may be less or greater than left.
	/// 
	/// @author Russ Rew
	/// @author John Caron
	///         TODO will be @Immutable in ver6, will use Builders.
	/// </summary>
	public class LatLonRect
	{

	  public static LatLonRect INVALID = new LatLonRect(LatLonPointImmutable.INVALID, LatLonPointImmutable.INVALID);

	  /// <summary>
	  /// upper right corner
	  /// </summary>
	  private LatLonPointImpl upperRight;

	  /// <summary>
	  /// lower left corner
	  /// </summary>
	  private LatLonPointImpl lowerLeft;

	  /// <summary>
	  /// flag for dateline cross
	  /// </summary>
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private bool crossDateline_Conflict;

	  /// <summary>
	  /// All longitudes are included
	  /// </summary>
	  private bool allLongitude;

	  /// <summary>
	  /// width and initial longitude
	  /// </summary>
	  private double width, lon0;

	  /// <summary>
	  /// Construct a lat/lon bounding box from a point, and a delta lat, lon.
	  /// This disambiguates which way the box wraps around the globe.
	  /// </summary>
	  /// <param name="p1"> one corner of the box </param>
	  /// <param name="deltaLat"> delta lat from p1. (may be positive or negetive) </param>
	  /// <param name="deltaLon"> delta lon from p1. (may be positive or negetive) </param>
	  public LatLonRect(LatLonPoint p1, double deltaLat, double deltaLon)
	  {
		init(p1, deltaLat, deltaLon);
	  }

	  private void init(LatLonPoint p1, double deltaLat, double deltaLon)
	  {
		double lonmin, lonmax;
		double latmin = Math.Min(p1.Latitude, p1.Latitude + deltaLat);
		double latmax = Math.Max(p1.Latitude, p1.Latitude + deltaLat);

		double lonpt = p1.Longitude;
		if (deltaLon > 0)
		{
		  lonmin = lonpt;
		  lonmax = lonpt + deltaLon;
		  crossDateline_Conflict = (lonmax > 180.0);
		}
		else
		{
		  lonmax = lonpt;
		  lonmin = lonpt + deltaLon;
		  crossDateline_Conflict = (lonmin < -180.0);
		}

		this.lowerLeft = new LatLonPointImpl(latmin, lonmin);
		this.upperRight = new LatLonPointImpl(latmax, lonmax);

		// these are an alternative way to view the longitude range
		this.width = Math.Abs(deltaLon);
		this.lon0 = LatLonPoints.lonNormal(p1.Longitude + deltaLon / 2);
		this.allLongitude = (this.width >= 360.0);
	  }

	  /// <summary>
	  /// Construct a lat/lon bounding box from two points.
	  /// The order of longitude coord of the two points matters:
	  /// pt1.lon is always the "left" point, then points contained within the box
	  /// increase (unless crossing the Dateline, in which case they jump to -180, but
	  /// then start increasing again) until pt2.lon
	  /// The order of lat doesnt matter: smaller will go to "lower" point (further south)
	  /// </summary>
	  /// <param name="left"> left corner </param>
	  /// <param name="right"> right corner </param>
	  public LatLonRect(LatLonPoint left, LatLonPoint right) : this(left, right.Latitude - left.Latitude, LatLonPoints.lonNormal360(right.Longitude - left.Longitude))
	  {
	  }

	  /// <summary>
	  /// Construct a lat/lon bounding box from a string.
	  /// </summary>
	  /// <param name="spec"> "lat, lon, deltaLat, deltaLon" </param>
	  /// <seealso cref=".LatLonRect(LatLonPoint p1, double deltaLat, double deltaLon)"/>
	  public LatLonRect(string spec)
	  {
		StringTokenizer stoker = new StringTokenizer(spec, " ,");
		int n = stoker.countTokens();
		if (n != 4)
		{
		  throw new ArgumentException("Must be 4 numbers = lat, lon, latWidth, lonWidth");
		}
		double lat = double.Parse(stoker.nextToken());
		double lon = double.Parse(stoker.nextToken());
		double deltaLat = double.Parse(stoker.nextToken());
		double deltaLon = double.Parse(stoker.nextToken());

		init(LatLonPoint.create(lat, lon), deltaLat, deltaLon);
	  }


	  /// <summary>
	  /// Copy Constructor
	  /// </summary>
	  /// <param name="r"> rectangle to copy </param>
	  public LatLonRect(LatLonRect r) : this(r.LowerLeftPoint, r.UpperRightPoint.Latitude - r.LowerLeftPoint.Latitude, r.Width)
	  {
	  }

	  /// <summary>
	  /// Create a LatLonRect that covers the whole world.
	  /// </summary>
	  public LatLonRect() : this(LatLonPoint.create(-90, -180), 180, 360)
	  {
	  }

	  /// <summary>
	  /// Get the upper right corner of the bounding box.
	  /// </summary>
	  /// <returns> upper right corner of the bounding box
	  ///         LOOK will return LatLonPoint in ver6 </returns>
	  public virtual LatLonPointImpl UpperRightPoint
	  {
		  get
		  {
			return upperRight;
		  }
	  }

	  /// <summary>
	  /// Get the lower left corner of the bounding box.
	  /// </summary>
	  /// <returns> lower left corner of the bounding box
	  ///         LOOK will return LatLonPoint in ver6 </returns>
	  public virtual LatLonPointImpl LowerLeftPoint
	  {
		  get
		  {
			return lowerLeft;
		  }
	  }

	  /// <summary>
	  /// Get the upper left corner of the bounding box.
	  /// </summary>
	  /// <returns> upper left corner of the bounding box
	  ///         LOOK will return LatLonPoint in ver6 </returns>
	  public virtual LatLonPointImpl UpperLeftPoint
	  {
		  get
		  {
			return new LatLonPointImpl(upperRight.Latitude, lowerLeft.Longitude);
		  }
	  }

	  /// <summary>
	  /// Get the lower left corner of the bounding box.
	  /// </summary>
	  /// <returns> lower left corner of the bounding box
	  ///         LOOK will return LatLonPoint in ver6 </returns>
	  public virtual LatLonPointImpl LowerRightPoint
	  {
		  get
		  {
			return new LatLonPointImpl(lowerLeft.Latitude, upperRight.Longitude);
		  }
	  }

	  /// <summary>
	  /// Get whether the bounding box crosses the +/- 180 seam
	  /// </summary>
	  /// <returns> true if the bounding box crosses the +/- 180 seam </returns>
	  public virtual bool crossDateline()
	  {
		return crossDateline_Conflict;
	  }

	  // Exact comparison is needed in order to be consistent with hashCode().
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
		LatLonRect that = (LatLonRect) o;
		return Objects.equals(upperRight, that.upperRight) && Objects.equals(lowerLeft, that.lowerLeft);
	  }

	  public override int GetHashCode()
	  {
		return Objects.hash(upperRight, lowerLeft);
	  }

	  /// <summary>
	  /// Returns the result of <seealso cref="nearlyEquals(LatLonRect, double)"/>, with <seealso cref="Misc.defaultMaxRelativeDiffFloat"/>.
	  /// </summary>
	  public virtual bool nearlyEquals(LatLonRect other)
	  {
		return nearlyEquals(other, Misc.defaultMaxRelativeDiffFloat);
	  }

	  /// <summary>
	  /// Returns {@code true} if this rectangle is nearly equal to {@code other}. The "near equality" of corners is
	  /// determined using <seealso cref="LatLonPoint.nearlyEquals(LatLonPoint, double)"/>, with the specified maxRelDiff.
	  /// </summary>
	  /// <param name="other"> the other rectangle to check. </param>
	  /// <param name="maxRelDiff"> the maximum <seealso cref="Misc.relativeDifference relative difference"/> that two corners may have. </param>
	  /// <returns> {@code true} if this rectangle is nearly equal to {@code other}. </returns>
	  public virtual bool nearlyEquals(LatLonRect other, double maxRelDiff)
	  {
		return this.LowerLeftPoint.nearlyEquals(other.LowerLeftPoint, maxRelDiff) && this.UpperRightPoint.nearlyEquals(other.UpperRightPoint, maxRelDiff);
	  }

	  /// <summary>
	  /// return does this rectangle contain all longitudes
	  /// </summary>
	  /// <returns> true if all longitudes are contained (width is >= 360) else false </returns>
	  public virtual bool containsAllLongitude()
	  {
		return allLongitude;
	  }

	  /// <summary>
	  /// return width of bounding box, always between 0 and 360 degrees.
	  /// </summary>
	  /// <returns> width of bounding box in degrees longitude </returns>
	  public virtual double Width
	  {
		  get
		  {
			return width;
		  }
	  }

	  /// <summary>
	  /// return height of bounding box, always between 0 and 180 degrees.
	  /// </summary>
	  /// <returns> height of bounding box in degrees latitude </returns>
	  public virtual double Height
	  {
		  get
		  {
			return LatMax - LatMin;
		  }
	  }

	  /// <summary>
	  /// return center Longitude, always in the range +/-180
	  /// </summary>
	  /// <returns> center Longitude </returns>
	  public virtual double CenterLon
	  {
		  get
		  {
			return lon0;
		  }
	  }

	  /// <summary>
	  /// Get minimum longitude, aka "west" edge
	  /// </summary>
	  /// <returns> minimum longitude </returns>
	  public virtual double LonMin
	  {
		  get
		  {
			if (allLongitude)
			{
			  return -180.0;
			}
			return lowerLeft.Longitude;
		  }
	  }

	  /// <summary>
	  /// Get maximum longitude, aka "east" edge
	  /// </summary>
	  /// <returns> maximum longitude </returns>
	  public virtual double LonMax
	  {
		  get
		  {
			if (allLongitude)
			{
			  return 180.0;
			}
			return lowerLeft.Longitude + width;
		  }
	  }

	  /// <summary>
	  /// Get minimum latitude, aka "south" edge
	  /// </summary>
	  /// <returns> minimum latitude </returns>
	  public virtual double LatMin
	  {
		  get
		  {
			return lowerLeft.Latitude;
		  }
	  }

	  /// <summary>
	  /// Get maximum latitude, aka "north" edge
	  /// </summary>
	  /// <returns> maximum latitude </returns>
	  public virtual double LatMax
	  {
		  get
		  {
			return upperRight.Latitude;
		  }
	  }


	  /// <summary>
	  /// Determine if a specified LatLonPoint is contained in this bounding box.
	  /// </summary>
	  /// <param name="p"> the specified point to be tested </param>
	  /// <returns> true if point is contained in this bounding box </returns>
	  public virtual bool contains(LatLonPoint p)
	  {
		return contains(p.Latitude, p.Longitude);
	  }

	  /// <summary>
	  /// Determine if the given lat/lon point is contined inside this rectangle.
	  /// </summary>
	  /// <param name="lat"> lat of point </param>
	  /// <param name="lon"> lon of point </param>
	  /// <returns> true if the given lat/lon point is contined inside this rectangle </returns>
	  public virtual bool contains(double lat, double lon)
	  {
		// check lat first
		double eps = 1.0e-9;
		if ((lat + eps < lowerLeft.Latitude) || (lat - eps > upperRight.Latitude))
		{
		  return false;
		}

		if (allLongitude)
		{
		  return true;
		}

		if (crossDateline_Conflict)
		{
		  // bounding box crosses the +/- 180 seam
		  return ((lon >= lowerLeft.Longitude) || (lon <= upperRight.Longitude));
		}
		else
		{
		  // check "normal" lon case
		  return ((lon >= lowerLeft.Longitude) && (lon <= upperRight.Longitude));
		}
	  }


	  /// <summary>
	  /// Determine if this bounding box is contained in another LatLonRect.
	  /// </summary>
	  /// <param name="b"> the other box to see if it contains this one </param>
	  /// <returns> true if b contained in this bounding box </returns>
	  public virtual bool containedIn(LatLonRect b)
	  {
		return (b.Width >= width) && b.contains(upperRight) && b.contains(lowerLeft);
	  }

	  /*
	   * Determine if a specified LatLonRect intersects this
	   * 
	   * @param b the specified box to be tested
	   *
	   * @param p
	   * 
	   * @return true if b intersects this bounding box
	   *
	   * public boolean intersects(LatLonRect b) {
	   * if (b.getUpperRightPoint().getLatitude() < lowerLeft.getLatitude())
	   * return false;
	   * if (b.getLowerLeftPoint().getLatitude() > upperRight.getLatitude())
	   * return false;
	   *
	   * double blon0 = b.getCenterLon();
	   * double normal = (blon0 + lon0) / 2;
	   * if (Math.abs(blon0-lon0) > 180.0)
	   * normal += 180;
	   * blon0 = LatLonPoint.lonNormal(blon0, normal);
	   * double mylon0 = LatLonPoint.lonNormal(lon0, normal);
	   *
	   * if (blon0 + b.getWidth() < mylon0 - width)
	   * return false;
	   * if (blon0 - b.getWidth() > mylon0 + width)
	   * return false;
	   *
	   * return true;
	   * }
	   */

	  // LOOK maybe want LatLonRectMutable for efficiency ??

	  /// <summary>
	  /// Extend the bounding box to contain this point. Return new LatLonRect. </summary>
	  public static LatLonRect extend(LatLonRect rect, LatLonPoint p)
	  {
		LatLonRect copy = new LatLonRect(rect);
		copy.extend(p);
		return copy;
	  }

	  /// <summary>
	  /// Extend the bounding box to contain another bounding box. Return new LatLonRect. </summary>
	  public static LatLonRect extend(LatLonRect rect, LatLonRect other)
	  {
		LatLonRect copy = new LatLonRect(rect);
		copy.extend(other);
		return copy;
	  }

	  /// <summary>
	  /// Extend the bounding box to contain this point
	  /// </summary>
	  /// <param name="p"> point to include </param>
	  /// @deprecated use LatLonRect.extend() 
	  [Obsolete("use LatLonRect.extend()")]
	  public virtual void extend(LatLonPoint p)
	  {
		if (contains(p))
		{
		  return;
		}

		double lat = p.Latitude;
		double lon = p.Longitude;

		// lat is easy to deal with
		if (lat > upperRight.Latitude)
		{
		  upperRight.Latitude = lat;
		}
		if (lat < lowerLeft.Latitude)
		{
		  lowerLeft.Latitude = lat;
		}

		// lon is uglier
		if (allLongitude)
		{
		  // do nothing
		}
		else if (crossDateline_Conflict)
		{

		  // bounding box crosses the +/- 180 seam
		  double d1 = lon - upperRight.Longitude;
		  double d2 = lowerLeft.Longitude - lon;
		  if ((d1 > 0.0) && (d2 > 0.0))
		  { // needed ?
			if (d1 > d2)
			{
			  lowerLeft.Longitude = lon;
			}
			else
			{
			  upperRight.Longitude = lon;
			}
		  }

		}
		else
		{

		  // normal case
		  if (lon > upperRight.Longitude)
		  {
			if (lon - upperRight.Longitude > lowerLeft.Longitude - lon + 360)
			{
			  crossDateline_Conflict = true;
			  lowerLeft.Longitude = lon;
			}
			else
			{
			  upperRight.Longitude = lon;
			}
		  }
		  else if (lon < lowerLeft.Longitude)
		  {
			if (lowerLeft.Longitude - lon > lon + 360.0 - upperRight.Longitude)
			{
			  crossDateline_Conflict = true;
			  upperRight.Longitude = lon;
			}
			else
			{
			  lowerLeft.Longitude = lon;
			}
		  }
		}

		// recalc delta, center
		width = upperRight.Longitude - lowerLeft.Longitude;
		lon0 = (upperRight.Longitude + lowerLeft.Longitude) / 2;
		if (crossDateline_Conflict)
		{
		  width += 360;
		  lon0 -= 180;
		}

		this.allLongitude = this.allLongitude || (this.width >= 360.0);
	  }

	  /// <summary>
	  /// Extend the bounding box to contain the given rectangle
	  /// </summary>
	  /// <param name="r"> rectangle to include </param>
	  /// @deprecated use LatLonRect.extend() 
	  [Obsolete("use LatLonRect.extend()")]
	  public virtual void extend(LatLonRect r)
	  {
		Preconditions.checkNotNull(r);

		// lat is easy
		double latMin = r.LatMin;
		double latMax = r.LatMax;

		if (latMax > upperRight.Latitude)
		{
		  upperRight.Latitude = latMax;
		}
		if (latMin < lowerLeft.Latitude)
		{
		  lowerLeft.Latitude = latMin;
		}

		// lon is uglier
		if (allLongitude)
		{
		  return;
		}

		// everything is relative to current LonMin
		double lonMin = LonMin;
		double lonMax = LonMax;

		double nlonMin = LatLonPoints.lonNormal(r.LonMin, lonMin);
		double nlonMax = nlonMin + r.Width;
		lonMin = Math.Min(lonMin, nlonMin);
		lonMax = Math.Max(lonMax, nlonMax);

		width = lonMax - lonMin;
		allLongitude = width >= 360.0;
		if (allLongitude)
		{
		  width = 360.0;
		  lonMin = -180.0;
		}
		else
		{
		  lonMin = LatLonPoints.lonNormal(lonMin);
		}

		lowerLeft.Longitude = lonMin;
		upperRight.Longitude = lonMin + width;
		lon0 = lonMin + width / 2;
		crossDateline_Conflict = lowerLeft.Longitude > upperRight.Longitude;
	  }

	  /// <summary>
	  /// Create the instersection of this LatLon with the given one
	  /// </summary>
	  /// <param name="clip"> intersect with this </param>
	  /// <returns> intersection, or null if there is no intersection </returns>
	  public virtual LatLonRect intersect(LatLonRect clip)
	  {
		double latMin = Math.Max(LatMin, clip.LatMin);
		double latMax = Math.Min(LatMax, clip.LatMax);
		double deltaLat = latMax - latMin;
		if (deltaLat < 0)
		{
		  return null;
		}

		// lon as always is a pain : if not intersection, try +/- 360
		double lon1min = LonMin;
		double lon1max = LonMax;
		double lon2min = clip.LonMin;
		double lon2max = clip.LonMax;
		if (!intersect(lon1min, lon1max, lon2min, lon2max))
		{
		  lon2min = clip.LonMin + 360;
		  lon2max = clip.LonMax + 360;
		  if (!intersect(lon1min, lon1max, lon2min, lon2max))
		  {
			lon2min = clip.LonMin - 360;
			lon2max = clip.LonMax - 360;
		  }
		}

		// we did our best to find an intersection
		double lonMin = Math.Max(lon1min, lon2min);
		double lonMax = Math.Min(lon1max, lon2max);
		double deltaLon = lonMax - lonMin;
		if (deltaLon < 0)
		{
		  return null;
		}

		return new LatLonRect(LatLonPoint.create(latMin, lonMin), deltaLat, deltaLon);
	  }

	  private bool intersect(double min1, double max1, double min2, double max2)
	  {
		double min = Math.Max(min1, min2);
		double max = Math.Min(max1, max2);
		return min < max;
	  }

	  /// <summary>
	  /// Return a String representation of this object.
	  /// 
	  /// <pre>
	  /// eg: ll: 90.0S .0E+ ur: 90.0N .0E
	  /// </pre>
	  /// </summary>
	  /// <returns> a String representation of this object. </returns>
	  public override string ToString()
	  {
		return " ll: " + lowerLeft + "+ ur: " + upperRight;
	  }


	  /// <summary>
	  /// Return a String representation of this object.
	  /// 
	  /// <pre>
	  /// lat= [-90.00,90.00] lon= [0.00,360.00
	  /// </pre>
	  /// </summary>
	  /// <returns> a String representation of this object. </returns>
	  public virtual string toString2()
	  {
		return " lat= [" + Format.dfrac(LatMin, 2) + "," + Format.dfrac(LatMax, 2) + "] lon= [" + Format.dfrac(LonMin, 2) + "," + Format.dfrac(LonMax, 2) + "]";
	  }
	}

}
