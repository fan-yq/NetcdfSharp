// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestMAVector.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestMAVector - MAVector dot/cos/norm/normalize.
	/// </summary>
	public class Ma2MAVectorTests
	{
		private static bool tolerance(double val)
		{
			return Math.Abs(val) < 1.0e-10;
		}

		[Fact]
		public void TestDot()
		{
			MAVector v1 = new MAVector(5);
			MAVector v2 = new MAVector(5);

			for (int i = 0; i < 5; i++)
			{
				v1.setDouble(i, 1.0);
				v2.setDouble(i, 1.0);
			}

			Assert.True(tolerance(v1.dot(v2) - 5.0));
			Assert.True(tolerance(v1.norm() - Math.Sqrt(5.0)));
			v1.normalize();
			Assert.True(tolerance(v1.norm() - 1.0));
		}

		[Fact]
		public void TestCos()
		{
			MAVector v1 = new MAVector(5);
			MAVector v2 = new MAVector(5);

			for (int i = 0; i < 5; i++)
			{
				v1.setDouble(i, 1.0);
				v2.setDouble(i, 1.0);
			}
			Assert.True(tolerance(v1.cos(v2) - 1.0));

			MAVector v3 = new MAVector(2);
			MAVector v4 = new MAVector(2);

			v3.setDouble(0, 1.0);
			v3.setDouble(1, 1.0);

			v4.setDouble(0, 1.0);
			v4.setDouble(1, -1.0);
			Assert.True(tolerance(v3.cos(v4)));

			v4.setDouble(0, -1.0);
			v4.setDouble(1, -1.0);
			Assert.True(tolerance(v3.cos(v4) + 1.0));

			v4.setDouble(0, -1.0);
			v4.setDouble(1, 1.0);
			Assert.True(tolerance(v3.cos(v4)));
		}
	}
}
