// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc
{
	using LatLonProjection = NetcdfSharp.ucar.unidata.geoloc.projection.LatLonProjection;
	using NetcdfSharp.ucar.unidata.util;

	/// <summary>
	/// Superclass for our implementations of geoloc.Projection.
	/// <p/>
	/// <para>
	/// All subclasses must:
	/// <ul>
	/// <li>override equals() and return true when all parameters are equal
	/// <li>create "atts" list of parameters as string-valued Attribute pairs
	/// <li>implement abstract methods
	/// </ul>
	/// <p/>
	/// If possible, set defaultmapArea to some reasonable world coord bounding box
	/// otherwise, provide a way for the user to specify it when a specific projection
	/// is created.
	/// <p/>
	/// </para>
	/// <para>
	/// Note on "false_easting" and "fale_northing" projection parameters:
	/// <ul>
	/// <li>false_easting(northing) = The value added to all x (y) values in the rectangular coordinates for a map
	/// projection.
	/// This value frequently is assigned to eliminate negative numbers.
	/// Expressed in the unit of measure identified in Planar Coordinate Units.
	/// <li>We dont currently use, assuming that the x and y are just fine as negative numbers.
	/// </ul>
	/// 
	/// @author John Caron
	/// </para>
	/// </summary>
	/// <seealso cref="Projection"/>
	/// @deprecated only use Projection interface in 6; will not implement Serializable in ver6 
	[Obsolete("only use Projection interface in 6; will not implement Serializable in ver6"), Serializable]
	public abstract class ProjectionImpl : Projection
	{
	  /// @deprecated not public. 
	  public const double EARTH_RADIUS = Earth.WGS84_EARTH_RADIUS_KM;

	  /// @deprecated not public. 
	  public const int INDEX_LAT = 0;

	  /// @deprecated not public. 
	  public const int INDEX_LON = 1;

	  /// @deprecated not public. 
	  public const int INDEX_X = 0;

	  /// @deprecated not public. 
	  public const int INDEX_Y = 1;

	  /// @deprecated not public. 
	  protected internal const double TOLERANCE = 1.0e-6;

	  /// @deprecated use Math.PI 
	  public static readonly double PI = Math.PI;

	  /// @deprecated use Math.PI/2 
	  public static readonly double PI_OVER_2 = Math.PI / 2.0;

	  /// @deprecated use Math.PI/4 
	  public static readonly double PI_OVER_4 = Math.PI / 4.0;

	  ///////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// name of this projection.
	  /// </summary>
	  protected internal string name; // LOOK should be final, IDV needs setName()

	  /// <summary>
	  /// name of the default units for this projection
	  /// </summary>
	  protected internal string defaultUnits = "kilometers";

	  /// <summary>
	  /// flag for latlon
	  /// </summary>
	  protected internal readonly bool isLatLon;

	  /// <summary>
	  /// list of attributes
	  /// </summary>
	  protected internal readonly IList<Parameter> atts = new List<Parameter>();

	  /// <summary>
	  /// default map area
	  /// </summary>
	  protected internal ProjectionRect defaultMapArea = new ProjectionRect();

	  /// <summary>
	  /// copy constructor - avoid clone !!
	  /// </summary>
	  /// <returns> a copy of this Projection.
	  ///         TODO return Projection in ver6 </returns>
	  public abstract ProjectionImpl constructCopy();

	  protected internal ProjectionImpl(string name, string defaultUnits, bool isLatLon)
	  {
		this.name = name;
		this.defaultUnits = defaultUnits;
		this.isLatLon = isLatLon;
	  }

	  protected internal ProjectionImpl(string name, bool isLatLon)
	  {
		this.name = name;
		this.isLatLon = isLatLon;
	  }

	  /// <summary>
	  /// Get the name of the type of the projection.
	  /// </summary>
	  /// <returns> the class name </returns>
	  public virtual string ClassName
	  {
		  get
		  {
	// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string className = this.GetType().FullName;
			int index = className.LastIndexOf(".", StringComparison.Ordinal);
			if (index >= 0)
			{
			  className = className.Substring(index + 1);
			}
			return className;
		  }
	  }

	  /// <summary>
	  /// Get a string representation of the projection parameters
	  /// </summary>
	  /// <returns> string representation of the projection parameters </returns>
	  public abstract string paramsToString();

	  /// <summary>
	  /// Get the label to be used in the gui for this type of projection.
	  /// This defaults to call getClassName
	  /// </summary>
	  /// <returns> Type label </returns>
	  public virtual string ProjectionTypeLabel
	  {
		  get
		  {
			return ClassName;
		  }
	  }

	  /// <summary>
	  /// Convert a LatLonPoint to projection coordinates
	  /// </summary>
	  /// <param name="latlon"> convert from these lat, lon coordinates </param>
	  /// <param name="destPoint"> the object to write to </param>
	  /// <returns> the given destPoint </returns>
	  /// @deprecated use latLonToProj(LatLonPoint latLon) 
	  [Obsolete("use latLonToProj(LatLonPoint latLon)")]
	  public abstract ProjectionPoint latLonToProj(LatLonPoint latlon, ProjectionPointImpl destPoint);

	  /// <summary>
	  /// Convert a LatLonPoint to projection coordinates
	  /// Note: a new object is now created on each call for the return value, as of 4.0.46
	  /// </summary>
	  /// <param name="latLon"> convert from these lat, lon coordinates </param>
	  /// <returns> ProjectionPoint convert to these projection coordinates </returns>
	  public virtual ProjectionPoint latLonToProj(LatLonPoint latLon)
	  {
		return latLonToProj(latLon, new ProjectionPointImpl());
	  }

	  /// <summary>
	  /// Convert projection coordinates to a LatLonPoint
	  /// Note: a new object is not created on each call for the return value.
	  /// </summary>
	  /// <param name="ppt"> convert from these projection coordinates </param>
	  /// <param name="destPoint"> the object to write to </param>
	  /// <returns> LatLonPoint convert to these lat/lon coordinates </returns>
	  /// @deprecated use projToLatLon(ProjectionPoint ppt) 
	  [Obsolete("use projToLatLon(ProjectionPoint ppt)")]
	  public abstract LatLonPoint projToLatLon(ProjectionPoint ppt, LatLonPointImpl destPoint);

	  /// <summary>
	  /// Convert projection coordinates to a LatLonPoint
	  /// Note: a new object is now created on each call for the return value, as of 4.0.46
	  /// </summary>
	  /// <param name="ppt"> convert from these projection coordinates </param>
	  /// <returns> LatLonPoint convert to these lat/lon coordinates </returns>
	  public virtual LatLonPoint projToLatLon(ProjectionPoint ppt)
	  {
		return projToLatLon(ppt, new LatLonPointImpl());
	  }

	  public virtual LatLonPoint projToLatLon(double x, double y)
	  {
		return projToLatLon(ProjectionPoint.create(x, y));
	  }

	  /// <summary>
	  /// Does the line between these two points cross the projection "seam".
	  /// </summary>
	  /// <param name="pt1"> the line goes between these two points </param>
	  /// <param name="pt2"> the line goes between these two points </param>
	  /// <returns> false if there is no seam </returns>
	  public abstract bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2);

	  /// <summary>
	  /// Returns true if this represents the same Projection as proj.
	  /// </summary>
	  /// <param name="proj"> projection in question </param>
	  /// <returns> true if this represents the same Projection as proj. </returns>
	  public override abstract bool Equals(object proj);

	  public override abstract int GetHashCode();

	  /// <summary>
	  /// Get the name of this specific projection (also see getClassName)
	  /// </summary>
	  /// <returns> name of the projection </returns>
	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  /// <summary>
	  /// Get the name of the default units for this projection
	  /// </summary>
	  /// <returns> the name of the default units </returns>
	  public virtual string DefaultUnits
	  {
		  get
		  {
			return defaultUnits;
		  }
	  }

	  /// <summary>
	  /// Get parameters as list of NetcdfSharp.ucar.unidata.util.Parameter
	  /// </summary>
	  /// <returns> List of parameters </returns>
	  public virtual IList<Parameter> ProjectionParameters
	  {
		  get
		  {
			return atts;
		  }
	  }

	  /// <summary>
	  /// Add an attribute to this projection
	  /// </summary>
	  /// <param name="name"> name of the attribute </param>
	  /// <param name="value"> attribute value as a string </param>
	  protected internal virtual void addParameter(string name, string value)
	  {
		atts.Add(new Parameter(name, value));
	  }

	  /// <summary>
	  /// Add an attribute to this projection
	  /// </summary>
	  /// <param name="name"> name of the attribute </param>
	  /// <param name="value"> attribute value as a double </param>
	  protected internal virtual void addParameter(string name, double value)
	  {
		atts.Add(new Parameter(name, value));
	  }

	  /// <summary>
	  /// Add an attribute to this projection
	  /// </summary>
	  /// <param name="p"> specify as a Parameter </param>
	  protected internal virtual void addParameter(Parameter p)
	  {
		atts.Add(p);
	  }

	  /// <summary>
	  /// Is this the lat/lon Projection ?
	  /// </summary>
	  /// <returns> true if it is the lat/lon Projection </returns>
	  public virtual bool LatLon
	  {
		  get
		  {
			return isLatLon;
		  }
	  }

	  /// <summary>
	  /// Get a header for display.
	  /// </summary>
	  /// <returns> human readable header for display </returns>
	  public static string Header
	  {
		  get
		  {
			StringBuilder headerB = new StringBuilder(60);
			headerB.Append("Name");
			Format.tab(headerB, 20, true);
			headerB.Append("Class");
			Format.tab(headerB, 40, true);
			headerB.Append("Parameters");
			return headerB.ToString();
		  }
	  }

	  /// <summary>
	  /// Get a String representation of this projection.
	  /// </summary>
	  /// <returns> the name of the projection. This is what gets
	  ///         displayed when you add the projection object to
	  ///         a UI widget (e.g. label, combobox) </returns>
	  public override string ToString()
	  {
		return Name;
	  }

	  /// <summary>
	  /// Get a reasonable bounding box for this projection.
	  /// </summary>
	  /// <returns> reasonable bounding box </returns>
	  public virtual ProjectionRect DefaultMapArea
	  {
		  get
		  {
			return defaultMapArea;
		  }
		  set
		  {
			if (value == null)
			{
			  return;
			}
			defaultMapArea = new ProjectionRect(value);
		  }
	  }

	  /// <summary>
	  /// Get the bounding box in lat/lon.
	  /// </summary>
	  /// <returns> the LatLonRectangle for the bounding box </returns>
	  public virtual LatLonRect DefaultMapAreaLL
	  {
		  get
		  {
			return projToLatLonBB(defaultMapArea);
		  }
	  }


	  //////// convenience routines

	  ///////////////////////////////////////////////////////////////////////////////////
	  // optimizations for doing double and float arrays

	  /// <summary>
	  /// Convert projection coordinates to lat/lon coordinates.
	  /// </summary>
	  /// <param name="from"> array of projection coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the x, y coordinate
	  ///        of the ith point </param>
	  /// <returns> resulting array of lat/lon coordinates, where to[0][i], to[1][i]
	  ///         is the lat,lon coordinate of the ith point </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual double[][] projToLatLon(double[][] from)
	  {
		return projToLatLon(from, new double[][] { new double[from[0].Length], new double[from[0].Length] });
	  }

	  /// <summary>
	  /// Convert projection coordinates to lat/lon coordinate.
	  /// </summary>
	  /// <param name="from"> array of projection coordinates: from[2][n], where
	  ///        (from[0][i], from[1][i]) is the (x, y) coordinate
	  ///        of the ith point </param>
	  /// <param name="to"> resulting array of lat/lon coordinates: to[2][n] where
	  ///        (to[0][i], to[1][i]) is the (lat, lon) coordinate of
	  ///        the ith point </param>
	  /// <returns> the "to" array </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual double[][] projToLatLon(double[][] from, double[][] to)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "from array not same length as to array");
		}

		for (int i = 0; i < from[0].Length; i++)
		{
		  LatLonPoint endL = projToLatLon(from[0][i], from[1][i]);
		  to[0][i] = endL.Latitude;
		  to[1][i] = endL.Longitude;
		}

		return to;
	  }

	  /// <summary>
	  /// Convert projection coordinates to lat/lon coordinates.
	  /// </summary>
	  /// <param name="from"> array of projection coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the x, y coordinate
	  ///        of the ith point </param>
	  /// <returns> resulting array of lat/lon coordinates, where to[0][i], to[1][i]
	  ///         is the lat,lon coordinate of the ith point </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual float[][] projToLatLon(float[][] from)
	  {
		return projToLatLon(from, new float[][] { new float[from[0].Length], new float[from[0].Length] });
	  }

	  /// <summary>
	  /// Convert projection coordinates to lat/lon coordinate.
	  /// </summary>
	  /// <param name="from"> array of projection coordinates: from[2][n], where
	  ///        (from[0][i], from[1][i]) is the (x, y) coordinate
	  ///        of the ith point </param>
	  /// <param name="to"> resulting array of lat/lon coordinates: to[2][n] where
	  ///        (to[0][i], to[1][i]) is the (lat, lon) coordinate of
	  ///        the ith point </param>
	  /// <returns> the "to" array </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual float[][] projToLatLon(float[][] from, float[][] to)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.projToLatLon:" + "from array not same length as to array");
		}

		for (int i = 0; i < from[0].Length; i++)
		{
		  ProjectionPoint ppi = ProjectionPoint.create(from[0][i], from[1][i]);
		  LatLonPoint llpi = projToLatLon(ppi);
		  to[0][i] = (float) llpi.Latitude;
		  to[1][i] = (float) llpi.Longitude;
		}

		return to;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <returns> resulting array of projection coordinates, where to[0][i],
	  ///         to[1][i] is the (x,y) coordinate of the ith point </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual double[][] latLonToProj(double[][] from)
	  {
		return latLonToProj(from, new double[][] { new double[from[0].Length], new double[from[0].Length] });
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[0][i], from[1][i]) is the (lat,lon) coordinate
	  ///        of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates: to[2][n]
	  ///        where (to[0][i], to[1][i]) is the (x,y) coordinate
	  ///        of the ith point </param>
	  /// <returns> the "to" array </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual double[][] latLonToProj(double[][] from, double[][] to)
	  {
		return latLonToProj(from, to, INDEX_LAT, INDEX_LON);
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[latIndex][i], from[lonIndex][i]) is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="latIndex"> index of lat coordinate; must be 0 or 1 </param>
	  /// <param name="lonIndex"> index of lon coordinate; must be 0 or 1 </param>
	  /// <returns> resulting array of projection coordinates: to[2][n] where
	  ///         (to[0][i], to[1][i]) is the (x,y) coordinate of the ith point </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual double[][] latLonToProj(double[][] from, int latIndex, int lonIndex)
	  {
		return latLonToProj(from, new double[][] { new double[from[0].Length], new double[from[0].Length] }, latIndex, lonIndex);
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[latIndex][i], from[lonIndex][i]) is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates: to[2][n]
	  ///        where (to[0][i], to[1][i]) is the (x,y) coordinate of
	  ///        the ith point </param>
	  /// <param name="latIndex"> index of lat coordinate; must be 0 or 1 </param>
	  /// <param name="lonIndex"> index of lon coordinate; must be 0 or 1 </param>
	  /// <returns> the "to" array </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual double[][] latLonToProj(double[][] from, double[][] to, int latIndex, int lonIndex)
	  {
		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "from array not same length as to array");
		}

		for (int i = 0; i < from[0].Length; i++)
		{
		  LatLonPoint llpi = LatLonPoint.create(from[latIndex][i], from[lonIndex][i]);
		  ProjectionPoint ppi = latLonToProj(llpi);
		  to[0][i] = ppi.X;
		  to[1][i] = ppi.Y;
		}
		return to;
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n],
	  ///        where from[0][i], from[1][i] is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <returns> resulting array of projection coordinates, where to[0][i],
	  ///         to[1][i] is the (x,y) coordinate of the ith point </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual float[][] latLonToProj(float[][] from)
	  {
		return latLonToProj(from, new float[][] { new float[from[0].Length], new float[from[0].Length] });
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[0][i], from[1][i]) is the (lat,lon) coordinate
	  ///        of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates: to[2][n]
	  ///        where (to[0][i], to[1][i]) is the (x,y) coordinate
	  ///        of the ith point </param>
	  /// <returns> the "to" array </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual float[][] latLonToProj(float[][] from, float[][] to)
	  {
		return latLonToProj(from, to, INDEX_LAT, INDEX_LON);
	  }

	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[latIndex][i], from[lonIndex][i]) is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="latIndex"> index of lat coordinate; must be 0 or 1 </param>
	  /// <param name="lonIndex"> index of lon coordinate; must be 0 or 1 </param>
	  /// <returns> resulting array of projection coordinates: to[2][n] where
	  ///         (to[0][i], to[1][i]) is the (x,y) coordinate of the ith point </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual float[][] latLonToProj(float[][] from, int latIndex, int lonIndex)
	  {
		return latLonToProj(from, new float[][] { new float[from[0].Length], new float[from[0].Length] }, latIndex, lonIndex);
	  }


	  /// <summary>
	  /// Convert lat/lon coordinates to projection coordinates.
	  /// </summary>
	  /// <param name="from"> array of lat/lon coordinates: from[2][n], where
	  ///        (from[latIndex][i], from[lonIndex][i]) is the (lat,lon)
	  ///        coordinate of the ith point </param>
	  /// <param name="to"> resulting array of projection coordinates: to[2][n]
	  ///        where (to[0][i], to[1][i]) is the (x,y) coordinate of
	  ///        the ith point </param>
	  /// <param name="latIndex"> index of lat coordinate; must be 0 or 1 </param>
	  /// <param name="lonIndex"> index of lon coordinate; must be 0 or 1 </param>
	  /// <returns> the "to" array </returns>
	  /// @deprecated use Projections.latLonToProj(Projection proj, ...) 
	  [Obsolete("use Projections.latLonToProj(Projection proj, ...)")]
	  public virtual float[][] latLonToProj(float[][] from, float[][] to, int latIndex, int lonIndex)
	  {
		// NetcdfSharp.ucar.unidata.util.Misc.printStack ("latLonToProj-" + this + " size=" + from[0].length, 4, null);

		if ((from == null) || (from.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "null array argument or wrong dimension (from)");
		}
		if ((to == null) || (to.Length != 2))
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "null array argument or wrong dimension (to)");
		}

		if (from[0].Length != to[0].Length)
		{
		  throw new ArgumentException("ProjectionImpl.latLonToProj:" + "from array not same length as to array");
		}

		for (int i = 0; i < from[0].Length; i++)
		{
		  LatLonPoint llpi = LatLonPoint.create(from[latIndex][i], from[lonIndex][i]);
		  ProjectionPoint ppi = latLonToProj(llpi);
		  to[0][i] = (float) ppi.X;
		  to[1][i] = (float) ppi.Y;
		}

		return to;
	  }

	  //////////////////////////////////////////////////////////////////////

	  // Allow subclasses to override.
	  public virtual ProjectionRect latLonToProjBB(LatLonRect latlonRect)
	  {
		if (isLatLon)
		{
		  LatLonProjection llp = (LatLonProjection) this;
		  llp.setCenterLon(latlonRect.CenterLon); // LOOK side effect BAD !!
		}

		LatLonPoint ll = latlonRect.LowerLeftPoint;
		LatLonPoint ur = latlonRect.UpperRightPoint;
		ProjectionPoint w1 = latLonToProj(ll);
		ProjectionPoint w2 = latLonToProj(ur);

		// make bounding box out of those two corners
		ProjectionRect world = new ProjectionRect(w1.X, w1.Y, w2.X, w2.Y);

		LatLonPoint la = LatLonPoint.create(ur.Latitude, ll.Longitude);
		LatLonPoint lb = LatLonPoint.create(ll.Latitude, ur.Longitude);

		// now extend if needed to the other two corners
		world.add(latLonToProj(la));
		world.add(latLonToProj(lb));

		return world;
	  }

	  // Allow subclasses to override.
	  public virtual LatLonRect projToLatLonBB(ProjectionRect bb)
	  {
		// look at all 4 corners of the bounding box
		LatLonPoint llpt = projToLatLon(bb.LowerLeftPoint);
		LatLonPoint lrpt = projToLatLon(bb.LowerRightPoint);
		LatLonPoint urpt = projToLatLon(bb.UpperRightPoint);
		LatLonPoint ulpt = projToLatLon(bb.UpperLeftPoint);

		// Check if grid contains poles.
		bool includesNorthPole = false;
		/*
		 * int[] resultNP;
		 * findXYindexFromLatLon(90.0, 0, resultNP);
		 * if (resultNP[0] != -1 && resultNP[1] != -1)
		 * includesNorthPole = true;
		 */
		bool includesSouthPole = false;
		/*
		 * int[] resultSP = new int[2];
		 * findXYindexFromLatLon(-90.0, 0, resultSP);
		 * if (resultSP[0] != -1 && resultSP[1] != -1)
		 * includesSouthPole = true;
		 */

		LatLonRect llbb;

		if (includesNorthPole && !includesSouthPole)
		{
		  llbb = new LatLonRect(llpt, LatLonPoint.create(90.0, 0.0)); // ??? lon=???
		  llbb.extend(lrpt);
		  llbb.extend(urpt);
		  llbb.extend(ulpt);
		  // OR
		  // llbb.extend( new LatLonRect( llpt, lrpt ));
		  // llbb.extend( new LatLonRect( lrpt, urpt ) );
		  // llbb.extend( new LatLonRect( urpt, ulpt ) );
		  // llbb.extend( new LatLonRect( ulpt, llpt ) );
		}
		else if (includesSouthPole && !includesNorthPole)
		{
		  llbb = new LatLonRect(llpt, LatLonPoint.create(-90.0, -180.0)); // ??? lon=???
		  llbb.extend(lrpt);
		  llbb.extend(urpt);
		  llbb.extend(ulpt);

		}
		else
		{
		  double latMin = Math.Min(llpt.Latitude, lrpt.Latitude);
		  double latMax = Math.Max(ulpt.Latitude, urpt.Latitude);

		  // longitude is a bit tricky as usual
		  double lonMin = getMinOrMaxLon(llpt.Longitude, ulpt.Longitude, true);
		  double lonMax = getMinOrMaxLon(lrpt.Longitude, urpt.Longitude, false);

		  LatLonPoint min = LatLonPoint.create(latMin, lonMin);
		  LatLonPoint max = LatLonPoint.create(latMax, lonMax);
		  llbb = new LatLonRect(min, max);
		}

		return llbb;
	  }

	  /// <summary>
	  /// Alternate way to calculate latLonToProjBB, originally in GridCoordSys.
	  /// Difficult to do this in a general way.
	  /// TODO evaluate if this is better than latLonToProjBB
	  /// </summary>
	  /// <param name="latlonRect"> desired lat/lon rectangle </param>
	  /// <returns> a ProjectionRect </returns>
	  internal virtual ProjectionRect latLonToProjBB2(LatLonRect latlonRect)
	  {
		double minx, maxx, miny, maxy;

		LatLonPoint llpt = latlonRect.LowerLeftPoint;
		LatLonPoint urpt = latlonRect.UpperRightPoint;
		LatLonPoint lrpt = latlonRect.LowerRightPoint;
		LatLonPoint ulpt = latlonRect.UpperLeftPoint;

		if (LatLon)
		{
		  minx = getMinOrMaxLon(llpt.Longitude, ulpt.Longitude, true);
		  miny = Math.Min(llpt.Latitude, lrpt.Latitude);
		  maxx = getMinOrMaxLon(urpt.Longitude, lrpt.Longitude, false);
		  maxy = Math.Min(ulpt.Latitude, urpt.Latitude);

		}
		else
		{
		  ProjectionPoint ll = latLonToProj(llpt);
		  ProjectionPoint ur = latLonToProj(urpt);
		  ProjectionPoint lr = latLonToProj(lrpt);
		  ProjectionPoint ul = latLonToProj(ulpt);

		  minx = Math.Min(ll.X, ul.X);
		  miny = Math.Min(ll.Y, lr.Y);
		  maxx = Math.Max(ur.X, lr.X);
		  maxy = Math.Max(ul.Y, ur.Y);
		}

		return new ProjectionRect(minx, miny, maxx, maxy);
	  }

	  private double getMinOrMaxLon(double lon1, double lon2, bool wantMin)
	  {
		double midpoint = (lon1 + lon2) / 2;
		lon1 = LatLonPoints.lonNormal(lon1, midpoint);
		lon2 = LatLonPoints.lonNormal(lon2, midpoint);

		return wantMin ? Math.Min(lon1, lon2) : Math.Max(lon1, lon2);
	  }
	}
}

