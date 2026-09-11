// Tests for the C# rectangular (multi-dimensional) array support added to NetcdfSharp.ucar.ma2.NcArray:
//   copyToCSharpArray()      - NcArray -> double[,] / double[,,] / ...
//   makeFromCSharpArray()    - double[,] / double[,,] / ... -> NcArray
//   getCSharpArrayRank/Shape - dimension inspection of rectangular and jagged arrays


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Exercises the C# rectangular array interop methods on NcArray.
	/// </summary>
	public class Ma2CSharpArrayTests
	{
		// ------------------------------------------------------------------
		// copyToCSharpArray: NcArray -> rectangular C# array
		// ------------------------------------------------------------------


		[Fact]
		public void Copy1Dim_ToCSharpArray()
		{
			NcArray a = NcArray.makeArray(DataType.DOUBLE, 5, 10.0, 1.0);
			System.Array result = a.copyToCSharpArray();

			Assert.Equal(typeof(double), result.GetType().GetElementType());
			Assert.Equal(1, result.Rank);
			Assert.Equal(5, result.GetLength(0));
			for (int i = 0; i < 5; i++)
			{
				Assert.Equal(10.0 + i, (double)result.GetValue(i));
			}
		}

		[Fact]
		public void Copy2Dim_ToCSharpArray_IsDouble2D()
		{
			int m = 3, n = 4;
			NcArray a = NcArray.factory(DataType.DOUBLE, new int[] { m, n });
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					a.setDouble(i * n + j, i * 100 + j);
				}
			}

			double[,] result = (double[,])a.copyToCSharpArray();
			Assert.Equal(m, result.GetLength(0));
			Assert.Equal(n, result.GetLength(1));
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					Assert.Equal(i * 100 + j, result[i, j]);
				}
			}
		}

		[Fact]
		public void Copy3Dim_ToCSharpArray_IsDouble3D()
		{
			int a0 = 2, a1 = 3, a2 = 4;
			NcArray a = NcArray.factory(DataType.DOUBLE, new int[] { a0, a1, a2 });
			for (int i = 0; i < a0; i++)
			{
				for (int j = 0; j < a1; j++)
				{
					for (int k = 0; k < a2; k++)
					{
						a.setDouble((i * a1 + j) * a2 + k, i * 100 + j * 10 + k);
					}
				}
			}

			double[,,] result = (double[,,])a.copyToCSharpArray();
			Assert.Equal(a0, result.GetLength(0));
			Assert.Equal(a1, result.GetLength(1));
			Assert.Equal(a2, result.GetLength(2));
			for (int i = 0; i < a0; i++)
			{
				for (int j = 0; j < a1; j++)
				{
					for (int k = 0; k < a2; k++)
					{
						Assert.Equal(i * 100 + j * 10 + k, result[i, j, k]);
					}
				}
			}
		}

		[Fact]
		public void CopyShort_ToCSharpArray_IsShort2D()
		{
			short[,] src =
			{
				{ 10, 11, 12 },
				{ 20, 21, 22 }
			};
			NcArray a = NcArray.makeFromCSharpArray(src);

			short[,] result = (short[,])a.copyToCSharpArray();
			Assert.Equal(new int[] { 2, 3 }, a.Shape);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Assert.Equal(src[i, j], result[i, j]);
				}
			}
		}

		[Fact]
		public void CopyStridedView_ToCSharpArray_MatchesLogicalOrder()
		{
			// transpose creates a strided view whose storage is NOT in canonical order,
			// so copyToCSharpArray must copy via get1DJavaArray().
			NcArray a = NcArray.factory(DataType.SHORT, new int[] { 2, 3 });
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					a.setShort(i * 3 + j, (short)(i * 10 + j));
				}
			}
			NcArray v = a.transpose(0, 1); // shape [3,2], v[i,j] == a[j,i]

			short[,] result = (short[,])v.copyToCSharpArray();
			Assert.Equal(3, result.GetLength(0));
			Assert.Equal(2, result.GetLength(1));
			Assert.Equal((short)0, result[0, 0]); // a[0,0]
			Assert.Equal((short)10, result[0, 1]); // a[1,0]
			Assert.Equal((short)1, result[1, 0]); // a[0,1]
			Assert.Equal((short)12, result[2, 1]); // a[1,2]
		}

		[Fact]
		public void CopyConstantArray_ToCSharpArray_ExpandsValue()
		{
			// IndexConstant has FastIterator=false: the fast path must expand the constant value
			// to the full array size, not return the length-1 backing storage.
			NcArray a = NcArray.factoryConstant(DataType.SHORT, new int[] { 2, 3 }, new short[] { 7 });

			short[,] result = (short[,])a.copyToCSharpArray();
			Assert.Equal(new int[] { 2, 3 }, a.Shape);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Assert.Equal((short)7, result[i, j]);
				}
			}
		}

		[Fact]
		public void CopyNonDouble_ToCSharpArray_KeepsElementType()
		{
			NcArray a = NcArray.factory(DataType.INT, new int[] { 2, 3 });
			for (int i = 0; i < a.Size; i++)
			{
				a.setInt(i, i * 7);
			}
			System.Array result = a.copyToCSharpArray();
			Assert.Equal(typeof(int), result.GetType().GetElementType());
			int[,] r2 = (int[,])result;
			Assert.Equal(0, r2[0, 0]);
			Assert.Equal(7, r2[0, 1]);
			Assert.Equal(21, r2[1, 0]);
		}

		// ------------------------------------------------------------------
		// makeFromCSharpArray: rectangular C# array -> NcArray
		// ------------------------------------------------------------------


		[Fact]
		public void MakeFromCSharpArray_1Dim()
		{
			double[] src = { 1.0, 2.5, 3.5, 4.5 };
			NcArray a = NcArray.makeFromCSharpArray(src);

			Assert.Equal(DataType.DOUBLE, a.DataType);
			Assert.Equal(new int[] { 4 }, a.Shape);
			for (int i = 0; i < 4; i++)
			{
				Assert.Equal(src[i], a.getDouble(i));
			}
		}

		[Fact]
		public void MakeFromCSharpArray_2Dim()
		{
			double[,] src =
			{
				{ 1, 2, 3 },
				{ 4, 5, 6 }
			};
			NcArray a = NcArray.makeFromCSharpArray(src);

			Assert.Equal(DataType.DOUBLE, a.DataType);
			Assert.Equal(new int[] { 2, 3 }, a.Shape);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Assert.Equal(src[i, j], a.getDouble(i * 3 + j));
				}
			}
		}

		[Fact]
		public void MakeFromCSharpArray_3Dim()
		{
			double[,,] src = new double[2, 3, 4];
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						src[i, j, k] = i * 100 + j * 10 + k;
					}
				}
			}
			NcArray a = NcArray.makeFromCSharpArray(src);

			Assert.Equal(DataType.DOUBLE, a.DataType);
			Assert.Equal(new int[] { 2, 3, 4 }, a.Shape);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						Assert.Equal(src[i, j, k], a.getDouble((i * 3 + j) * 4 + k));
					}
				}
			}
		}

		[Fact]
		public void MakeFromCSharpArray_NonDouble_InfersDataType()
		{
			int[,] src =
			{
				{ 1, 2 },
				{ 3, 4 },
				{ 5, 6 }
			};
			NcArray a = NcArray.makeFromCSharpArray(src);
			Assert.Equal(DataType.INT, a.DataType);
			Assert.Equal(new int[] { 3, 2 }, a.Shape);
			Assert.Equal(5, a.getInt(4));
		}

		[Fact]
		public void MakeFromCSharpArray_Jagged_Delegates()
		{
			double[][] src =
			{
				new double[] { 1, 2, 3 },
				new double[] { 4, 5, 6 }
			};
			NcArray a = NcArray.makeFromCSharpArray(src);
			Assert.Equal(DataType.DOUBLE, a.DataType);
			Assert.Equal(new int[] { 2, 3 }, a.Shape);
			Assert.Equal(6.0, a.getDouble(5));
		}

		// ------------------------------------------------------------------
		// rank / shape inspection
		// ------------------------------------------------------------------


		[Fact]
		public void GetRank_Rectangular()
		{
			Assert.Equal(1, NcArray.getCSharpArrayRank(new double[4]));
			Assert.Equal(2, NcArray.getCSharpArrayRank(new double[2, 3]));
			Assert.Equal(3, NcArray.getCSharpArrayRank(new double[2, 3, 4]));
		}

		[Fact]
		public void GetRank_Jagged()
		{
			Assert.Equal(2, NcArray.getCSharpArrayRank(new double[2][]));
			Assert.Equal(3, NcArray.getCSharpArrayRank(new double[2][][]));
		}

		[Fact]
		public void GetShape_Rectangular()
		{
			Assert.Equal(new int[] { 2, 3 }, NcArray.getCSharpArrayShape(new double[2, 3]));
			Assert.Equal(new int[] { 2, 3, 4 }, NcArray.getCSharpArrayShape(new double[2, 3, 4]));
		}

		[Fact]
		public void GetShape_Jagged()
		{
			double[][] jagged =
			{
				new double[] { 1, 2, 3 },
				new double[] { 4, 5, 6 }
			};
			Assert.Equal(new int[] { 2, 3 }, NcArray.getCSharpArrayShape(jagged));
		}

		// ------------------------------------------------------------------
		// round-trips
		// ------------------------------------------------------------------


		[Fact]
		public void RoundTrip_2Dim_Double()
		{
			double[,] src =
			{
				{ 1.5, 2.5, 3.5 },
				{ 4.5, 5.5, 6.5 }
			};
			NcArray a = NcArray.makeFromCSharpArray(src);
			double[,] back = (double[,])a.copyToCSharpArray();
			Assert.Equal(src, back);
		}

		[Fact]
		public void RoundTrip_3Dim_Int()
		{
			int[,,] src = new int[2, 3, 2];
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						src[i, j, k] = i * 10 + j * 2 + k;
					}
				}
			}
			NcArray a = NcArray.makeFromCSharpArray(src);
			int[,,] back = (int[,,])a.copyToCSharpArray();
			Assert.Equal(src, back);
		}
	}
}
