// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

namespace NetcdfSharp.ucar.unidata.geoloc.projection
{
	using NetcdfSharp.ucar.unidata.geoloc;

	/// <summary>
	/// A dummy ProjectionImpl subclass for unstructured grids.
	/// Since unstructured grids do not follow a single mathematical projection,
	/// this class does not implement real forward/inverse transformations.
	/// Instead, it serves as a placeholder so netCDF-Java can represent the
	/// grid in a coordinate system, while actual lat/lon positions are handled
	/// separately (e.g., by per-cell coordinate arrays).
	/// </summary>
	[Serializable]
	public class UnstructuredProjection : ProjectionImpl
	{

	  public const string EARTH_SHAPE = "earth_shape";
	  public const string NUMBER_OF_GRID_USED = "number_of_grid_used";
	  public const string NUMBER_OF_GRID_IN_REFERENCE = "number_of_grid_in_reference";
	  public const string UUID = "uuid";

	  internal int earthShape, numberOfGridUsed, numberOfGridInReference;
	  internal string uuid;

	  /// <summary>
	  /// Create a new UnstructuredProjection with a given name.
	  /// </summary>
	  public UnstructuredProjection(int earthShape, int numberOfGridUsed, int numberOfGridInReference, string uuid) : base("UnstructuredProjection", false) // false => not lat/lon
	  {

		this.earthShape = earthShape;
		this.numberOfGridUsed = numberOfGridUsed;
		this.numberOfGridInReference = numberOfGridInReference;
		this.uuid = uuid;

		addParameter(EARTH_SHAPE, earthShape);
		addParameter(NUMBER_OF_GRID_USED, numberOfGridUsed);
		addParameter(NUMBER_OF_GRID_IN_REFERENCE, numberOfGridInReference);
		addParameter(UUID, uuid);
	  }

	  /// <summary>
	  /// Copy constructor for UnstructuredProjection.
	  /// </summary>
	  private UnstructuredProjection(UnstructuredProjection that) : this(that.earthShape, that.numberOfGridUsed, that.numberOfGridInReference, that.uuid)
	  {
	  }

	  public override ProjectionImpl constructCopy()
	  {
		// Return a new instance with the same name, etc.
		return new UnstructuredProjection(this);
	  }

	  /// <summary>
	  /// Since we do not have a formula for an unstructured grid,
	  /// throw an exception if someone tries to do forward transform.
	  /// </summary>
	  public override ProjectionPoint latLonToProj(LatLonPoint latlon, ProjectionPointImpl dest)
	  {
		throw new System.NotSupportedException("UnstructuredProjection: no formula-based transform available");
	  }

	  /// <summary>
	  /// Similarly, inverse transform is not defined for unstructured grids.
	  /// </summary>
	  public override LatLonPoint projToLatLon(ProjectionPoint world, LatLonPointImpl dest)
	  {
		throw new System.NotSupportedException("UnstructuredProjection: no formula-based transform available");
	  }

	  /// <summary>
	  /// For unstructured grids, the notion of crossing a seam is irrelevant,
	  /// so we return false (or you could throw an exception).
	  /// </summary>
	  public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
	  {
		return false;
	  }

	  public override string paramsToString()
	  {
		return "UnstructuredProjection: no transform parameters";
	  }

	  public override bool Equals(object o)
	  {
		if (!(o is UnstructuredProjection))
		{
		  return false;
		}
		UnstructuredProjection that = (UnstructuredProjection) o;
		return earthShape == that.earthShape && numberOfGridUsed == that.numberOfGridUsed && numberOfGridInReference == that.numberOfGridInReference && Objects.equals(uuid, that.uuid);
	  }

	  public override int GetHashCode()
	  {
		return Objects.hash(earthShape, numberOfGridUsed, numberOfGridInReference, uuid);
	  }
	}

}
