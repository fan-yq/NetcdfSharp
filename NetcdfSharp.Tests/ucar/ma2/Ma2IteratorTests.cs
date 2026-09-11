// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestIterator.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestIterator - fast/slow iterators, scalar and section iteration, object arrays.
	/// </summary>
	public class Ma2IteratorTests
	{
		private const int m = 4, n = 3, p = 2;
		private static readonly int[] sA = { m, n, p };
		private const int m1 = 2;
		private const int m2 = 3;
		private const int mlen = (m2 - m1 + 1);

		private NcArray MakeArray()
		{
			NcArray A = NcArray.factory(DataType.DOUBLE, sA);
			NcIndex ima = A.Index;

			for (int i = 0; i < m; i++)
			{
				ima.set0(i);
				for (int j = 0; j < n; j++)
				{
					ima.set1(j);
					for (int k = 0; k < p; k++)
					{
						ima.set2(k);
						A.setDouble(ima, i * 100 + j * 10 + k);
					}
				}
			}
			return A;
		}

		[Fact]
		public void TestFastIter()
		{
			NcArray A = MakeArray();

			int count = 0;
			IndexIterator iter = A.getIndexIterator();
			while (iter.hasNext())
			{
				_ = iter.DoubleNext;
				count++;
			}
			Assert.Equal(n * m * p, count);

			iter = A.getIndexIterator();
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = _ = iter.DoubleNext;
						Assert.Equal(i * 100 + j * 10 + k, val, 1e-10);
					}
				}
			}
		}

		[Fact]
		public void TestSlowIter()
		{
			NcArray A = MakeArray();

			int count = 0;
			NcIndex index = A.Index;
			IndexIterator iter = index.getSlowIndexIterator(A);

			while (iter.hasNext())
			{
				_ = iter.DoubleNext;
				count++;
			}
			Assert.Equal(n * m * p, count);

			iter = A.getIndexIterator();
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = _ = iter.DoubleNext;
						Assert.Equal(i * 100 + j * 10 + k, val, 1e-10);
					}
				}
			}
		}

		[Fact]
		public void TestScalerIter()
		{
			NcArray scalar = NcArray.factory(DataType.DOUBLE, new int[0]);
			scalar.setDouble(0, 10.0);

			int count = 0;
			double sum = 0;
			IndexIterator iter = scalar.getIndexIterator();
			while (iter.hasNext())
			{
				sum += _ = iter.DoubleNext;
				count++;
			}
			Assert.Equal(1, count);
			Assert.Equal(10.0, sum);
		}

		[Fact]
		public void TestSectionIter()
		{
			NcArray A = MakeArray();
			NcArray secA = A.section(new Section(m1 + ":" + m2 + ",:,:").Ranges);

			int count = 0;
			IndexIterator iter = secA.getIndexIterator();

			while (iter.hasNext())
			{
				_ = iter.DoubleNext;
				count++;
			}
			Assert.Equal(n * mlen * p, count);

			iter = secA.getIndexIterator();
			for (int i = 0; i < mlen; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = _ = iter.DoubleNext;
						Assert.Equal((i + m1) * 100 + j * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestSectionRank2()
		{
			NcArray A = MakeArray();
			NcArray secA2 = A.section(new Section("2,:,:").Ranges);

			IndexIterator iter = secA2.getIndexIterator();
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < p; j++)
				{
					double val = _ = iter.DoubleNext;
					Assert.Equal(200 + i * 10 + j, val);
				}
			}
		}

		[Fact]
		public void TestArrayObjectIter()
		{
			NcArray arrayObject = NcArray.factory(DataType.OPAQUE, new int[] { 30 });
			CheckObjectIter(arrayObject, 30);

			NcArray arrayObjectSubset = arrayObject.sectionNoReduce(new Section("1:20").Ranges);
			CheckObjectIter(arrayObjectSubset, 20);

			NcArray arrayObjectSubsetCopy = arrayObjectSubset.copy();
			CheckObjectIter(arrayObjectSubsetCopy, 20);
		}

		private void CheckObjectIter(NcArray a, int expected)
		{
			int count = 0;
			IndexIterator iter = a.getIndexIterator();
			while (iter.hasNext())
			{
				iter.next();
				count++;
			}
			Assert.Equal(expected, count);

			NcIndex index = a.Index;
			IndexIterator iter2 = index.getSlowIndexIterator(a);
			count = 0;
			while (iter2.hasNext())
			{
				_ = iter2.next();
				count++;
			}
			Assert.Equal(expected, count);
		}
	}
}
