// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestGetPut.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestGetPut - index-based get/set on a 3-D double array.
	/// </summary>
	public class Ma2GetPutTests
	{
		private const int m = 4, n = 3, p = 2;
		private static readonly int[] sA = { m, n, p };

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
						A.setDouble(ima, i * 1000000 + j * 1000 + k);
					}
				}
			}
			return A;
		}

		[Fact]
		public void TestGetPut()
		{
			NcArray A = MakeArray();
			NcIndex ima = A.Index;

			for (int i = 0; i < m; i++)
			{
				ima.set0(i);
				for (int j = 0; j < n; j++)
				{
					ima.set1(j);
					for (int k = 0; k < p; k++)
					{
						double val = A.getDouble(ima.set2(k));
						Assert.Equal(i * 1000000 + j * 1000 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestDim()
		{
			NcArray A = MakeArray();
			NcIndex index = A.Index;

			for (int i = 0; i < m; i++)
			{
				index.setDim(0, i);
				for (int j = 0; j < n; j++)
				{
					index.setDim(1, j);
					for (int k = 0; k < p; k++)
					{
						index.setDim(2, k);
						double val = A.getDouble(index);
						Assert.Equal(i * 1000000 + j * 1000 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestIndexSet3()
		{
			NcArray A = MakeArray();
			NcIndex index = A.Index;

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = A.getDouble(index.set(i, j, k));
						Assert.Equal(i * 1000000 + j * 1000 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestIter()
		{
			NcArray A = MakeArray();
			IndexIterator iter = A.getIndexIterator();

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = iter.DoubleNext;
						Assert.Equal(i * 1000000 + j * 1000 + k, val);
					}
				}
			}
		}
	}
}
