// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestSection.java
// The Java original used NetcdfSharp.ucar.nc2.write.Ncdump for debug printing only; that is out of
// scope here, so the Ncdump calls are omitted and the assertions are kept verbatim.


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestSection - section/slice/reduce/intersect on arrays.
	/// </summary>
	public class Ma2SectionTests
	{
		private const int m = 4;
		private const int n = 5;
		private const int p = 6;
		private static readonly int[] sA = { m, n, p };
		private const int m1 = 2;
		private const int m2 = 3;
		private const int mlen = (m2 - m1 + 1);

		private NcArray MakeArray()
		{
			NcArray A = NcArray.factory(DataType.DOUBLE, sA);
			NcIndex ima = A.Index;

			// write
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
		public void TestSection()
		{
			NcArray A = MakeArray();
			NcArray secA = A.section(new Section(m1 + ":" + m2 + ",:,:").Ranges);

			NcIndex ima = secA.Index;
			Assert.Equal(3, ima.Rank);
			Assert.Equal(mlen * n * p, ima.Size);
			Assert.Equal(mlen, ima.Shape[0]);
			Assert.Equal(n, ima.Shape[1]);
			Assert.Equal(p, ima.Shape[2]);
		}

		[Fact]
		public void TestSectionRead()
		{
			NcArray A = MakeArray();
			NcArray secA = A.section(new Section(m1 + ":" + m2 + ",:,:").Ranges);
			NcIndex ima = secA.Index;

			for (int i = 0; i < mlen; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = secA.getDouble(ima.set(i, j, k));
						Assert.Equal((i + m1) * 100 + j * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestIter()
		{
			NcArray A = MakeArray();
			NcArray secA = A.section(new Section(m1 + ":" + m2 + ",:,:").Ranges);
			IndexIterator iter = secA.getIndexIterator();

			for (int i = 0; i < mlen; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = iter.DoubleNext;
						Assert.Equal((i + m1) * 100 + j * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestRangeIterator()
		{
			NcArray A = MakeArray();
			IndexIterator iter = A.getRangeIterator(new Section(m1 + ":" + m2 + ",:,:").Ranges);

			for (int i = 0; i < mlen; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = iter.DoubleNext;
						Assert.Equal((i + m1) * 100 + j * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestSectionReduce()
		{
			NcArray A = MakeArray();
			NcArray secA2 = A.section(new Section("2,:,:").Ranges);

			NcIndex ima = secA2.Index;
			Assert.Equal(2, ima.Rank);
			Assert.Equal(n * p, ima.Size);
			Assert.Equal(n, ima.Shape[0]);
			Assert.Equal(p, ima.Shape[1]);

			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < p; j++)
				{
					ima.set(i, j);
					Assert.Equal(200 + i * 10 + j, secA2.getDouble(ima));
				}
			}

			IndexIterator iter = secA2.getIndexIterator();
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < p; j++)
				{
					Assert.Equal(200 + i * 10 + j, iter.DoubleNext);
				}
			}
		}

		[Fact]
		public void TestSectionNoReduce()
		{
			NcArray A = MakeArray();
			NcArray secA2 = A.sectionNoReduce(new Section("2,:,:").Ranges);

			NcIndex ima = secA2.Index;
			Assert.Equal(3, ima.Rank);
			Assert.Equal(n * p, ima.Size);
			Assert.Equal(1, ima.Shape[0]);
			Assert.Equal(n, ima.Shape[1]);
			Assert.Equal(p, ima.Shape[2]);

			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < p; j++)
				{
					ima.set(0, i, j);
					Assert.Equal(200 + i * 10 + j, secA2.getDouble(ima));
				}
			}

			IndexIterator iter = secA2.getIndexIterator();
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < p; j++)
				{
					Assert.Equal(200 + i * 10 + j, iter.DoubleNext);
				}
			}
		}

		[Fact]
		public void TestDoubleRange()
		{
			NcArray A = MakeArray();
			NcArray secA2 = A.section(new Section("2,:,:").Ranges);
			NcArray secA1 = secA2.section(new Section(":,1").Ranges);

			NcIndex ima = secA1.Index;
			Assert.Equal(1, ima.Rank);
			Assert.Equal(n, ima.Size);
			Assert.Equal(n, ima.Shape[0]);

			for (int j = 0; j < n; j++)
			{
				ima.set(j);
				Assert.Equal(200 + j * 10 + 1, secA1.getDouble(ima));
			}

			IndexIterator iter = secA1.getIndexIterator();
			for (int i = 0; i < n; i++)
			{
				Assert.Equal(200 + i * 10 + 1, iter.DoubleNext);
			}
		}

		[Fact]
		public void TestDoubleRange2()
		{
			NcArray A = MakeArray();
			NcArray secA2 = A.section(new Section("2,:,:").Ranges);
			secA2 = secA2.reduce();
			NcArray secA1 = secA2.section(new Section(":,1").Ranges);

			NcIndex ima = secA1.Index;
			Assert.Equal(1, ima.Rank);
			Assert.Equal(n, ima.Size);
			Assert.Equal(n, ima.Shape[0]);

			for (int j = 0; j < n; j++)
			{
				ima.set(j);
				Assert.Equal(200 + j * 10 + 1, secA1.getDouble(ima));
			}

			IndexIterator iter = secA1.getIndexIterator();
			for (int i = 0; i < n; i++)
			{
				Assert.Equal(200 + i * 10 + 1, iter.DoubleNext);
			}
		}

		[Fact]
		public void TestRange3()
		{
			NcArray A = MakeArray();
			NcArray secA2 = A.section(new Section(1 + ":" + (m - 2) + ",0:" + (n - 2) + ",1:" + (p - 1)).Ranges);

			NcIndex imaOrg = A.Index;
			NcIndex ima = secA2.Index;

			Assert.Equal(3, ima.Rank);
			Assert.Equal((m - 2) * (n - 1) * (p - 1), ima.Size);
			Assert.Equal(m - 2, ima.Shape[0]);
			Assert.Equal(n - 1, ima.Shape[1]);
			Assert.Equal(p - 1, ima.Shape[2]);

			for (int i = 0; i < m - 2; i++)
			{
				for (int j = 0; j < n - 1; j++)
				{
					for (int k = 0; k < p - 1; k++)
					{
						ima.set(i, j);
						double val = secA2.getDouble(ima.set(i, j, k));
						double valOrg = A.getDouble(imaOrg.set(i + 1, j, k + 1));
						Assert.Equal(valOrg, val);
					}
				}
			}
		}

		[Fact]
		public void TestSlice()
		{
			NcArray A = MakeArray();
			NcArray secA2 = A.slice(0, 2);
			NcIndex ima = secA2.Index;

			Assert.Equal(2, ima.Rank);
			Assert.Equal(n * p, ima.Size);
			Assert.Equal(n, ima.Shape[0]);
			Assert.Equal(p, ima.Shape[1]);

			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < p; j++)
				{
					ima.set(i, j);
					Assert.Equal(200 + i * 10 + j, secA2.getDouble(ima));
				}
			}

			IndexIterator iter = secA2.getIndexIterator();
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < p; j++)
				{
					Assert.Equal(200 + i * 10 + j, iter.DoubleNext);
				}
			}
		}

		[Fact]
		public void TestSliceOne()
		{
			NcArray A = MakeArray();
			NcArray secA1 = A.sectionNoReduce(new Section("1,0:" + (n - 2) + ",1:" + (p - 1)).Ranges);

			Assert.Equal(3, secA1.Rank);
			Assert.Equal((n - 1) * (p - 1), secA1.Size);
			Assert.Equal(1, secA1.Shape[0]);
			Assert.Equal(n - 1, secA1.Shape[1]);
			Assert.Equal(p - 1, secA1.Shape[2]);

			NcArray secA2 = secA1.slice(1, 2);
			Assert.Equal(2, secA2.Rank);
			Assert.Equal(p - 1, secA2.Size);
			Assert.Equal(1, secA2.Shape[0]);
			Assert.Equal(p - 1, secA2.Shape[1]);

			NcIndex imaOrg = A.Index;
			NcIndex ima = secA2.Index;
			for (int k = 0; k < p - 1; k++)
			{
				double val = secA2.getDouble(ima.set(0, k));
				double valOrg = A.getDouble(imaOrg.set(1, 2, k + 1));
				Assert.Equal(valOrg, val);
			}
		}

		[Fact]
		public void TestSectionCopy()
		{
			NcArray A = MakeArray();
			NcArray secA2 = A.section(new Section(1 + ":" + (m - 2) + ",0:" + (n - 2) + ",1:" + (p - 1)).Ranges).copy();

			NcIndex imaOrg = A.Index;
			NcIndex ima = secA2.Index;

			Assert.Equal(3, ima.Rank);
			Assert.Equal((m - 2) * (n - 1) * (p - 1), ima.Size);
			Assert.Equal(m - 2, ima.Shape[0]);
			Assert.Equal(n - 1, ima.Shape[1]);
			Assert.Equal(p - 1, ima.Shape[2]);

			for (int i = 0; i < m - 2; i++)
			{
				for (int j = 0; j < n - 1; j++)
				{
					for (int k = 0; k < p - 1; k++)
					{
						ima.set(i, j);
						double val = secA2.getDouble(ima.set(i, j, k));
						secA2.setDouble(ima, 0.0);
						double valOrg = A.getDouble(imaOrg.set(i + 1, j, k + 1));
						Assert.Equal(valOrg, val);
					}
				}
			}
		}

		[Fact]
		public void TestIntersect()
		{
			Section have = new Section("101:101,0:1919");
			Section want = new Section("100:229:3,100:191:7");

			bool b = have.intersects(want);
			Assert.False(b);
		}

		[Fact]
		public void TestIntersectWithStrides()
		{
			Section have = new Section("121:121:1,0:1919:1");
			Section want = new Section("100:229:3,100:191:7");
			Section intersectCorrect = new Section("121:121:3,100:191:7");
			Section intersect = have.intersect(want);
			Assert.Equal(intersectCorrect, intersect);

			int offset = want.offset(intersect);
			Assert.Equal(98, offset);
		}

		[Fact]
		public void TestSlice3D()
		{
			// Regression: slices must return proper concrete Array subtypes (Java threw
			// ClassCastException: IndexConstant cannot be cast to Index1D before the fix).
			NcArray a = NcArray.makeArray(DataType.DOUBLE, 1000, 0.0, 1.0);
			NcArray a3 = a.reshape(new int[] { 10, 10, 10 });
			NcArray a2 = a3.slice(0, 1);
			NcArray a1 = a2.slice(0, 1);

			Assert.Equal(2, a2.Rank);
			Assert.Equal(10 * 10, a2.Size);
			Assert.Equal(1, a1.Rank);
			Assert.Equal(10, a1.Size);

			// data access through an Index reads the correct slice values
			// (the flat-index getDouble(int) on a sliced view is a documented Java quirk)
			NcIndex idx = a1.Index;
			for (int k = 0; k < 10; k++)
			{
				Assert.Equal(a.getDouble(1 * 100 + 1 * 10 + k), a1.getDouble(idx.set(k)));
			}
		}
	}
}
