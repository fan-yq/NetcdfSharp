// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/TestQuasiRegular.java


using NetcdfSharp.ucar.nc2.grib;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.TestQuasiRegular - cubic spline interpolation on a quasi-regular grid.
	/// </summary>
	public class GribQuasiRegularTests
	{
		private const double x1d = 1.0e30; // derivative of the first end point
		private const double xnd = 1.0e30; // derivative of the nth end point

		[Fact]
		public void TestCubicSpline()
		{
			float[] @in = new float[] { 0.0f, 9.0f, 18.0f, 27.0f };

			double[] d2 = new double[4];
			QuasiRegular.secondDerivative(@in, 0, 4, x1d, xnd, d2);

			float[] @out = new float[8];
			QuasiRegular.cubicSpline(@in, 0, d2, 0.0, @out, 0);
			Assert.Equal(0.0, @out[0], 0.0);
			QuasiRegular.cubicSpline(@in, 0, d2, 0.5, @out, 1);
			Assert.Equal(4.5, @out[1], 0.0);
			QuasiRegular.cubicSpline(@in, 0, d2, 1.0, @out, 2);
			Assert.Equal(9.0, @out[2], 0.0);
			QuasiRegular.cubicSpline(@in, 0, d2, 1.5, @out, 3);
			Assert.Equal(13.5, @out[3], 0.0);
			QuasiRegular.cubicSpline(@in, 0, d2, 2.0, @out, 4);
			Assert.Equal(18.0, @out[4], 0.0);
			QuasiRegular.cubicSpline(@in, 0, d2, 2.5, @out, 5);
			Assert.Equal(22.5, @out[5], 0.0);
			QuasiRegular.cubicSpline(@in, 0, d2, 3.0, @out, 6);
			Assert.Equal(27.0, @out[6], 0.0);
			QuasiRegular.cubicSpline(@in, 0, d2, 3.5, @out, 7);
			Assert.Equal(13.5, @out[7], 0.0);
		}
	}
}
