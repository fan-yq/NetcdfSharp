// Minimal stub for NetcdfSharp.ucar.unidata.geoloc.projection.sat.MSGnavigation.
// The satellite projection math is out of scope ("Direction A"): the stub returns defaults.

namespace NetcdfSharp.ucar.unidata.geoloc.projection.sat
{
	using ProjectionImpl = NetcdfSharp.ucar.unidata.geoloc.ProjectionImpl;
	using ProjectionPoint = NetcdfSharp.ucar.unidata.geoloc.ProjectionPoint;
	using ProjectionPointImpl = NetcdfSharp.ucar.unidata.geoloc.ProjectionPointImpl;
	using LatLonPoint = NetcdfSharp.ucar.unidata.geoloc.LatLonPoint;
	using LatLonPointImpl = NetcdfSharp.ucar.unidata.geoloc.LatLonPointImpl;

	public class MSGnavigation : ProjectionImpl
	{
		internal MSGnavigation(double laP, double loP, double majorAxis, double minorAxis, double height, double scaleX, double scaleY)
			: base("MSGnavigation", false)
		{
		}

		public override ProjectionImpl constructCopy()
		{
			return null;
		}

		public override string paramsToString()
		{
			return "";
		}

#pragma warning disable CS0672 // base class methods are [Obsolete]; stub overrides them faithfully
		public override ProjectionPoint latLonToProj(LatLonPoint latlon, ProjectionPointImpl destPoint)
		{
			return null;
		}

		public override LatLonPoint projToLatLon(ProjectionPoint ppt, LatLonPointImpl destPoint)
		{
			return null;
		}
#pragma warning restore CS0672

		public override bool crossSeam(ProjectionPoint pt1, ProjectionPoint pt2)
		{
			return false;
		}

		public override bool Equals(object proj)
		{
			return ReferenceEquals(this, proj);
		}

		public override int GetHashCode()
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
		}
	}
}
