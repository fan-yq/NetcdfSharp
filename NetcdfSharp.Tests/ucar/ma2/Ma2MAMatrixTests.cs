// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestMAMatrix.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestMAMatrix - MAMatrix dot/multiply/transpose/diagonal/row/column.
	/// </summary>
	public class Ma2MAMatrixTests
	{
		private const int n = 5, m = 10;

		private static bool tolerance(double val)
		{
			return Math.Abs(val) < 1.0e-10;
		}

		[Fact]
		public void TestDot()
		{
			MAVector v1 = new MAVector(n);
			for (int i = 0; i < n; i++)
			{
				v1.setDouble(i, 1.0);
			}

			MAMatrix M = new MAMatrix(m, n);
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					M.setDouble(i, j, 1.0);
				}
			}

			MAVector v2 = M.dot(v1);

			Assert.True(tolerance(v2.norm() - Math.Sqrt(m * n * n)));
		}

		[Fact]
		public void TestMultiply()
		{
			MAMatrix M1 = new MAMatrix(2, 3);
			MAMatrix M2 = new MAMatrix(3, 2);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					M1.setDouble(i, j, i + 1);
					M2.setDouble(j, i, i + 1);
				}
			}

			MAMatrix r = MAMatrix.multiply(M1, M2);

			Assert.True(tolerance(r.getDouble(0, 0) - 3.0));
			Assert.True(tolerance(r.getDouble(0, 1) - 6.0));
			Assert.True(tolerance(r.getDouble(1, 0) - 6.0));
			Assert.True(tolerance(r.getDouble(1, 1) - 12.0));
		}

		[Fact]
		public void TestTranspose()
		{
			MAMatrix M1 = new MAMatrix(2, 3);
			MAMatrix M2 = new MAMatrix(3, 2);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					M1.setDouble(i, j, i + j);
					M2.setDouble(j, i, i + j);
				}
			}

			MAMatrix t = M2.transpose();
			Assert.True(tolerance(t.getDouble(0, 0) - M1.getDouble(0, 0)));
			Assert.True(tolerance(t.getDouble(0, 1) - M1.getDouble(0, 1)));
			Assert.True(tolerance(t.getDouble(0, 2) - M1.getDouble(0, 2)));
			Assert.True(tolerance(t.getDouble(1, 0) - M1.getDouble(1, 0)));
			Assert.True(tolerance(t.getDouble(1, 1) - M1.getDouble(1, 1)));
			Assert.True(tolerance(t.getDouble(1, 2) - M1.getDouble(1, 2)));
		}

		[Fact]
		public void TestDiag()
		{
			MAMatrix M1 = new MAMatrix(2, 3);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					M1.setDouble(i, j, i + j + 1.0);
				}
			}

			MAVector v1 = new MAVector(3);
			for (int i = 0; i < 3; i++)
			{
				v1.setDouble(i, i + 1.0);
			}

			M1.postMultiplyDiagonal(v1);
			Assert.True(tolerance(1.0 - M1.getDouble(0, 0)));
			Assert.True(tolerance(4.0 - M1.getDouble(0, 1)));
			Assert.True(tolerance(9.0 - M1.getDouble(0, 2)));
			Assert.True(tolerance(2.0 - M1.getDouble(1, 0)));
			Assert.True(tolerance(6.0 - M1.getDouble(1, 1)));
			Assert.True(tolerance(12.0 - M1.getDouble(1, 2)));

			MAVector v2 = new MAVector(2);
			for (int i = 0; i < 2; i++)
			{
				v2.setDouble(i, i + 2.0);
			}

			M1.preMultiplyDiagonal(v2);
			Assert.True(tolerance(2.0 - M1.getDouble(0, 0)));
			Assert.True(tolerance(8.0 - M1.getDouble(0, 1)));
			Assert.True(tolerance(18.0 - M1.getDouble(0, 2)));
			Assert.True(tolerance(6.0 - M1.getDouble(1, 0)));
			Assert.True(tolerance(18.0 - M1.getDouble(1, 1)));
			Assert.True(tolerance(36.0 - M1.getDouble(1, 2)));

			Assert.Throws<ArgumentException>(() => M1.preMultiplyDiagonal(v1));
			Assert.Throws<ArgumentException>(() => M1.postMultiplyDiagonal(v2));
		}

		[Fact]
		public void TestProjection()
		{
			MAMatrix M1 = new MAMatrix(2, 3);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					M1.setDouble(i, j, i + j + 1.0);
				}
			}

			MAVector v1 = M1.row(1);
			Assert.True(tolerance(2.0 - v1.getDouble(0)));
			Assert.True(tolerance(3.0 - v1.getDouble(1)));
			Assert.True(tolerance(4.0 - v1.getDouble(2)));

			MAVector v2 = M1.column(2);
			Assert.True(tolerance(3.0 - v2.getDouble(0)));
			Assert.True(tolerance(4.0 - v2.getDouble(1)));
		}
	}
}
