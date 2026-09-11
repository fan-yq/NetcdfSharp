// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestTxCompose.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestTxCompose - flip/transpose/permute and composed views.
	/// </summary>
	public class Ma2TxComposeTests
	{
		private const int m = 4, n = 3, p = 2;
		private static readonly int[] sA = { m, n, p };

		private NcArray MakeArray()
		{
			NcArray A = NcArray.factory(DataType.DOUBLE, sA);
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
		public void TestFlip()
		{
			NcArray A = MakeArray();
			NcArray flipA = A.flip(1);
			NcIndex ima = flipA.Index;

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = flipA.getDouble(ima.set(i, j, k));
						Assert.Equal(i * 100 + (n - j - 1) * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestTranspose()
		{
			NcArray A = MakeArray();
			NcArray trA = A.transpose(0, 1);
			NcIndex ima = trA.Index;

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = trA.getDouble(ima.set(j, i, k));
						Assert.Equal(i * 100 + j * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestPermute()
		{
			NcArray A = MakeArray();
			NcArray pA = A.permute(new int[] { 2, 0, 1 });
			NcIndex ima = pA.Index;

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = pA.getDouble(ima.set(k, i, j));
						Assert.Equal(i * 100 + j * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestCompose()
		{
			NcArray A = MakeArray();
			NcArray trA = A.transpose(0, 1);
			NcArray composeA = trA.flip(1);
			NcIndex ima = composeA.Index;

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = composeA.getDouble(ima.set(j, i, k));
						Assert.Equal((m - i - 1) * 100 + j * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestCompose2()
		{
			NcArray A = MakeArray();
			NcArray flipA = A.flip(1);
			NcArray composeA = flipA.transpose(0, 1);
			NcIndex ima = composeA.Index;

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					for (int k = 0; k < p; k++)
					{
						double val = composeA.getDouble(ima.set(j, i, k));
						Assert.Equal(i * 100 + (n - j - 1) * 10 + k, val);
					}
				}
			}
		}

		[Fact]
		public void TestCompose3()
		{
			NcArray A = MakeArray();
			int ival = 2;

			NcArray secA2 = A.section(new Section(ival + ",:,:").Ranges);
			NcArray composeA = secA2.permute(new int[] { 1, 0 });
			NcIndex ima = composeA.Index;

			for (int j = 0; j < n; j++)
			{
				for (int k = 0; k < p; k++)
				{
					double val = composeA.getDouble(ima.set(k, j));
					Assert.Equal(ival * 100 + j * 10 + k, val);
				}
			}
		}
	}
}
