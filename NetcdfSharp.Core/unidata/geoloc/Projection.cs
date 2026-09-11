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
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;

	/// <summary>
	/// Projective geometry transformations from (lat,lon) to (x,y) on
	/// a projective cartesian surface.
	/// Unless it is a rotation from (lat,lon) to (lat,lon).
	/// 
	/// @author John Caron
	///         LOOK will be immutable AutoValue in ver6
	/// </summary>

	public interface Projection
	{

	  /// <summary>
	  /// The name of this class of projections, eg "Transverse Mercator". </summary>
	  string ClassName {get;}

	  /// <summary>
	  /// The name of this projection. </summary>
	  string Name {get;}

	  /// <summary>
	  /// Is this the lat/lon Projection ? </summary>
	  bool LatLon {get;}

	  /// <summary>
	  /// String representation of the projection parameters. </summary>
	  string paramsToString();

	  /// @deprecated use latLonToProj(LatLonPoint latlon) 
	  [Obsolete("use latLonToProj(LatLonPoint latlon)")]
	  ProjectionPoint latLonToProj(LatLonPoint latlon, ProjectionPointImpl result);

	  /// <summary>
	  /// Convert lat, lon to Projection point. </summary>
	  ProjectionPoint latLonToProj(double lat, double lon)
	  {
		return latLonToProj(LatLonPoint.create(lat, lon));
	  }

	  /// <summary>
	  /// Convert a LatLonPoint to projection coordinates.
	  /// </summary>
	  /// <param name="latlon"> convert from these lat, lon coordinates </param>
	  /// <returns> ProjectionPoint convert to these projection coordinates </returns>
	  ProjectionPoint latLonToProj(LatLonPoint latlon);

	  /// @deprecated use projToLatLon(ProjectionPoint ppt) 
	  [Obsolete("use projToLatLon(ProjectionPoint ppt)")]
	  LatLonPoint projToLatLon(ProjectionPoint ppt, LatLonPointImpl result);

	  /// <summary>
	  /// Convert projection x, y to LatLonPoint point. </summary>
	  LatLonPoint projToLatLon(double x, double y)
	  {
		return projToLatLon(ProjectionPoint.create(x, y));
	  }

	  /// <summary>
	  /// Convert projection coordinates to a LatLonPoint.
	  /// </summary>
	  /// <param name="ppt"> convert from these projection coordinates </param>
	  /// <returns> lat/lon coordinates </returns>
	  LatLonPoint projToLatLon(ProjectionPoint ppt);

	  /// <summary>
	  /// Does the line between these two points cross the projection "seam", which
	  /// is a discontinuity in the function latlon <-> projection plane
	  /// </summary>
	  /// <param name="pt1"> the line goes between these two points </param>
	  /// <param name="pt2"> the line goes between these two points </param>
	  /// <returns> false if there is no seam, or the line does not cross it. </returns>
	  bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2);

	  /// <summary>
	  /// Get a reasonable bounding box in this projection.
	  /// Projections are typically specific to an area of the world;
	  /// theres no bounding box that works for all projections.
	  /// </summary>
	  /// <returns> a reasonable bounding box in this projection. </returns>
	  /// @deprecated Do not use. 
	  [Obsolete("Do not use.")]
	  ProjectionRect DefaultMapArea {get;}

	  /// <summary>
	  /// Check for equality with the object in question
	  /// </summary>
	  /// <param name="proj"> projection to check </param>
	  /// <returns> true if this represents the same Projection as proj. </returns>
	  bool Equals(object proj);

	  /// <summary>
	  /// Get parameters as list of NetcdfSharp.ucar.unidata.util.Parameter
	  /// </summary>
	  /// <returns> List of parameters
	  ///         TODO will return ImmutableList in ver6 </returns>
	  IList<Parameter> ProjectionParameters {get;}

	  /// <summary>
	  /// Convert a lat/lon bounding box to a world coordinate bounding box,
	  /// by finding the minimum enclosing box.
	  /// Handles lat/lon points that do not intersect the projection panel.
	  /// </summary>
	  /// <param name="latlonRect"> input lat,lon bounding box </param>
	  /// <returns> minimum enclosing box in world coordinates, or null if no part of the LatLonRect intersects the projection
	  ///         plane </returns>
	  ProjectionRect latLonToProjBB(LatLonRect latlonRect);

	  /// <summary>
	  /// Compute lat/lon bounding box from projection bounding box.
	  /// </summary>
	  /// <param name="bb"> projection bounding box </param>
	  /// <returns> lat, lon bounding box. </returns>
	  /// @deprecated use Projections.projToLatLonBB(Projection proj, ...) 
	  LatLonRect projToLatLonBB(ProjectionRect bb);

	}


}
