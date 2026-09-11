// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/coord/TestCoordinateND.java


using System.Collections.Generic;
using NetcdfSharp.ucar.nc2.grib.coord;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.coord.TestCoordinateND - dense/sparse coordinate reindexing.
	/// </summary>
	public class TestCoordinateND
	{
		[Fact]
		public void TestCoordinateND_SparseReindex()
		{
			CoordinateND<short> prev = makeCoordinateND(2, 10);
			CoordinateND<short> curr = makeCoordinateND(2, 11);

			CoordinateND<short> reindexed = new CoordinateND<short>.Builder<short>().reindex(curr.Coordinates, prev);

			Assert.Equal(1.0, (double)curr.SparseArray.Density, 1e-6);
			Assert.Equal(0.826446, (double)reindexed.SparseArray.Density, 1e-6);
		}

		public static CoordinateND<short> makeCoordinateND(int rank, int size)
		{
			IList<Coordinate> coords = new List<Coordinate>();
			for (int i = 0; i < rank; i++)
			{
				coords.Add(new TestCoordinate(size * (i + 1)));
			}

			int[] sizeArray = new int[coords.Count];
			for (int i = 0; i < coords.Count; i++)
			{
				sizeArray[i] = coords[i].Size;
			}
			SparseArray<short>.Builder<short> builder = new SparseArray<short>.Builder<short>(sizeArray);

			int n = builder.TotalSize;
			int[] track = new int[n];
			for (int i = 0; i < n; i++)
			{
				track[i] = i + 1;
			}
			builder.setTrack(track);

			IList<short> content = new List<short>(n);
			for (int i = 0; i < n; i++)
			{
				content.Add((short)(i * 10));
			}
			builder.setContent(content);

			SparseArray<short> sa = builder.finish();
			return new CoordinateND<short>(coords, sa);
		}

		public static CoordinateND<short> makeCoordinateND(int size)
		{
			IList<Coordinate> coords = new List<Coordinate>();
			coords.Add(TestCoordinate.factory(size, Coordinate.Type.runtime));
			coords.Add(TestCoordinate.factory(size, Coordinate.Type.time));
			coords.Add(TestCoordinate.factory(size, Coordinate.Type.vert));

			int[] sizeArray = new int[coords.Count];
			for (int i = 0; i < coords.Count; i++)
			{
				sizeArray[i] = coords[i].Size;
			}
			SparseArray<short>.Builder<short> builder = new SparseArray<short>.Builder<short>(sizeArray);

			int n = builder.TotalSize;
			int[] track = new int[n];
			for (int i = 0; i < n; i++)
			{
				track[i] = i + 1;
			}
			builder.setTrack(track);

			IList<short> content = new List<short>(n);
			for (int i = 0; i < n; i++)
			{
				content.Add((short)(i * 10));
			}
			builder.setContent(content);

			SparseArray<short> sa = builder.finish();
			return new CoordinateND<short>(coords, sa);
		}
	}
}
