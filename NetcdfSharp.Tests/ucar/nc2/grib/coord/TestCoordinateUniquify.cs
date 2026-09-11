// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/coord/TestCoordinateUniquify.java


using System.Collections.Generic;
using NetcdfSharp.ucar.nc2.grib.coord;
using Formatter = NetcdfSharp.java.util.Formatter;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.coord.TestCoordinateUniquify - unionizer smoke test.
	/// </summary>
	public class TestCoordinateUniquify
	{
		[Fact]
		public void testCoordinateUnionizer()
		{
			IList<CoordinateND<short>> coordNDs = new List<CoordinateND<short>>();
			for (int i = 5; i < 15; i += 2)
			{
				coordNDs.Add(TestCoordinateND.makeCoordinateND(i));
			}

			CoordinateSharerBest unionizer = new CoordinateSharerBest();
			foreach (CoordinateND<short> coordND in coordNDs)
			{
				unionizer.addCoordinates(coordND.Coordinates);
			}
			IList<Coordinate> shared = unionizer.finish();

			Formatter f = new Formatter();
			f.format("Original%n");
			foreach (CoordinateND<short> coordND in coordNDs)
			{
				coordND.showInfo(f, null);
			}

			f.format("Uniqueified%n");
			foreach (Coordinate coord in shared)
			{
				coord.showInfo(f, null);
			}
		}
	}
}
