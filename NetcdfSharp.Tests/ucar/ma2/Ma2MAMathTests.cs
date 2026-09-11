// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestMAMath.java
// Guava's Lists used in the Java original maps to a plain System.Collections.Generic.List here.


using System;
using System.Collections.Generic;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestMAMath - MAMath helpers: conformable, set/sum, add, section,
	/// equality and hashCode.
	/// </summary>
	public class Ma2MAMathTests
	{
		private const int m = 4, n = 3, p = 2;
		private const int m1 = 2;
		private const int m2 = 3;

		private NcArray MakeArray()
		{
			NcArray A = NcArray.factory(DataType.DOUBLE, new int[] { m, n, p });
			NcIndex ima = A.Index;

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						A.setDouble(ima.set(i, j, k), i * 100 + j * 10 + k);
					}
				}
			}
			return A;
		}

		[Fact]
		public void TestConformable()
		{
			int[] a1 = { 1, 3, 1, 5, 1, 1 };
			int[] b1 = { 3, 5 };
			Assert.True(MAMath.conformable(b1, a1));
			Assert.True(MAMath.conformable(a1, b1));

			int[] a2 = { 1, 3, 1, 5, 1, 1 };
			int[] b2 = { 1, 5, 3 };
			Assert.False(MAMath.conformable(b2, a2));
			Assert.False(MAMath.conformable(a2, b2));

			int[] a3 = { 1, 3, 1, 5, 1, 1 };
			int[] b3 = { 3, 5, 1, 1, 1, 2 };
			Assert.False(MAMath.conformable(b3, a3));
			Assert.False(MAMath.conformable(a3, b3));

			int[] a4 = { 1, 3, 1, 5, 1, 1, 7 };
			int[] b4 = { 1, 1, 3, 5, 1, 7, 1, 1 };
			Assert.True(MAMath.conformable(b4, a4));
			Assert.True(MAMath.conformable(a4, b4));
		}

		[Fact]
		public void TestSet()
		{
			NcArray A = MakeArray();
			MAMath.setDouble(A, 1.0);
			Assert.Equal((double)m * n * p, MAMath.sumDouble(A));

			NcArray secA = A.section(new Section(m1 + ":" + m2 + ",:,:").Ranges);
			int mlen = (m2 - m1 + 1);
			Assert.Equal((double)mlen * n * p, MAMath.sumDouble(secA));
		}

		[Fact]
		public void TestAdd()
		{
			int dim0 = 2;
			int dim1 = 3;
			int[] shape = { dim0, dim1 };
			NcArray A = NcArray.factory(DataType.DOUBLE, shape);
			NcArray B = NcArray.factory(DataType.DOUBLE, shape);

			MAMath.setDouble(A, 1.0);
			MAMath.setDouble(B, 22.0f);
			MAMath.addDouble(A, A, A);
			NcArray result = MAMath.add(A, B);

			IndexIterator iterR = result.getIndexIterator();
			while (iterR.hasNext())
			{
				double vala = iterR.DoubleNext;
				Assert.Equal(24.0, vala);
			}
		}

		[Fact]
		public void TestSectionCopy()
		{
			int dim0 = 2;
			int dim1 = 3;
			int dim2 = 4;
			int[] shape = { dim0, dim1, dim2 };
			NcArray A = NcArray.factory(DataType.DOUBLE, shape);

			// test section
			MAMath.setDouble(A, 1.0);
			Assert.Equal((double)dim0 * dim1 * dim2, MAMath.sumDouble(A));

			IList<NcRange> ranges = new Section(":,:,1").Ranges;
			NcArray secA = A.section(ranges);
			MAMath.setDouble(secA, 0.0);

			Assert.Equal(0.0, MAMath.sumDouble(secA));
			Assert.Equal((double)(dim0 * dim1 * dim2 - dim0 * dim1), MAMath.sumDouble(A));

			// test copy
			MAMath.setDouble(A, 1.0);
			Assert.Equal((double)dim0 * dim1 * dim2, MAMath.sumDouble(A));

			NcArray copyA = A.section(ranges).copy();
			MAMath.setDouble(copyA, 0.0);

			Assert.Equal((double)dim0 * dim1 * dim2, MAMath.sumDouble(A));
		}

		[Fact]
		public void TestSectionIterator()
		{
			NcArray data = MakeArray();

			Section section = new Section("1,:,:");
			NcArray datasection = data.sectionNoReduce(section.Ranges);

			double sum = MAMath.sumDouble(datasection);
			Assert.Equal(663.0, sum);
		}

		[Fact]
		public void EqualsAndHashCode()
		{
			// Create 3 different arrays that are deemed equal, even though they have different backing arrays and Indexes.

			// Index2D[shape={2,10},stride={10,1},current={0,0},offset=0]
			NcIndex indexX = new Index2D(new int[] { 2, 10 });
			int[] storageX = new int[] { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };
			NcArray arrayX = NcArray.factory(DataType.INT, indexX, storageX);

			// Index[shape={2,10},stride={20,2},current={0,0},offset=0]
			NcIndex indexY = new Index2D(new int[] { 2, 20 });
			indexY = indexY.section(new List<NcRange> { null, new NcRange(0, 19, 2) });
			int[] storageY = new int[] { 20, 0, 21, 0, 22, 0, 23, 0, 24, 0, 25, 0, 26, 0, 27, 0, 28, 0, 29, 0, 30, 0, 31, 0, 32, 0, 33, 0, 34, 0, 35, 0, 36, 0, 37, 0, 38, 0, 39, 0 };
			NcArray arrayY = NcArray.factory(DataType.INT, indexY, storageY);

			// Index[shape={3,2,10},stride={20,10,1},current={0,0},offset=20]
			NcIndex indexZ = new Index3D(new int[] { 3, 2, 10 });
			indexZ = indexZ.section(new List<NcRange> { new NcRange(1, 1), null, null });
			int[] storageZ = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 };
			NcArray arrayZ = NcArray.factory(DataType.INT, indexZ, storageZ);

			// Reflexive
			Assert.True(MAMath.equals(arrayX, arrayX));
			Assert.True(MAMath.equals(arrayY, arrayY));
			Assert.True(MAMath.equals(arrayZ, arrayZ));

			// Symmetric
			Assert.True(MAMath.equals(arrayX, arrayY));
			Assert.True(MAMath.equals(arrayY, arrayX));
			Assert.True(MAMath.equals(arrayY, arrayZ));
			Assert.True(MAMath.equals(arrayZ, arrayY));

			// Transitive
			Assert.True(MAMath.equals(arrayX, arrayZ));
			Assert.True(MAMath.equals(arrayZ, arrayX));

			// Consistent
			Assert.True(MAMath.equals(arrayX, arrayY));

			// Null
			Assert.False(MAMath.equals(arrayX, null));

			// hashCode
			Assert.Equal(MAMath.hashCode(arrayX), MAMath.hashCode(arrayY));
			Assert.Equal(MAMath.hashCode(arrayY), MAMath.hashCode(arrayZ));
			Assert.Equal(MAMath.hashCode(arrayX), MAMath.hashCode(arrayZ));

			// Null
			Assert.Equal(0, MAMath.hashCode(null));
		}
	}
}
