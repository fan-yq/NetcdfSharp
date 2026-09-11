// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestJavaArray.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestJavaArray - NcArray constructors from .NET jagged arrays.
	/// </summary>
	public class Ma2JavaArrayTests
	{
		[Fact]
		public void Test1Dim()
		{
			NcArray aa = NcArray.makeFromJavaArray(new double[] { 1.0, 2.0, 3.0 }, false);

			Assert.Equal(1, aa.Rank);
			Assert.Equal(3, aa.Shape[0]);

			NcIndex ima = aa.Index;
			Assert.Equal(1.0, aa.getFloat(ima.set(0)));
			Assert.Equal(2.0, aa.getFloat(ima.set(1)));
			Assert.Equal(3.0, aa.getFloat(ima.set(2)));

			IndexIterator ai = aa.getIndexIterator();
			Assert.Equal(1.0, ai.FloatNext);
			Assert.Equal(1.0, ai.FloatCurrent);
			Assert.Equal(2.0, ai.FloatNext);
			Assert.Equal(3.0, ai.FloatNext);

			double[] newArray = (double[])aa.copyTo1DJavaArray();
			Assert.Equal(1.0, newArray[0]);
			Assert.Equal(2.0, newArray[1]);
			Assert.Equal(3.0, newArray[2]);

			double[] newArray2 = (double[])aa.copyToNDJavaArray();
			Assert.Equal(1.0, newArray2[0]);
			Assert.Equal(2.0, newArray2[1]);
			Assert.Equal(3.0, newArray2[2]);
		}

		[Fact]
		public void TestNDim()
		{
			double[][][] tData =
			{
				new double[][]
				{
					new double[] { 1, 2, 3, 4 },
					new double[] { 2, 4, 6, 8 },
					new double[] { 3, 6, 9, 12 }
				},
				new double[][]
				{
					new double[] { 2.5, 5, 7.5, 10 },
					new double[] { 5, 10, 15, 20 },
					new double[] { 7.5, 15, 22.5, 30 }
				}
			};

			NcArray aa = NcArray.makeFromJavaArray(tData, false);

			Assert.Equal(3, aa.Rank);
			Assert.Equal(2, aa.Shape[0]);
			Assert.Equal(3, aa.Shape[1]);
			Assert.Equal(4, aa.Shape[2]);

			NcIndex ima = aa.Index;
			for (int i = 0; i < aa.Shape[0]; i++)
			{
				for (int j = 0; j < aa.Shape[1]; j++)
				{
					for (int k = 0; k < aa.Shape[2]; k++)
					{
						Assert.Equal(tData[i][j][k], aa.getDouble(ima.set(i, j, k)));
					}
				}
			}

			double[][][] newArray = (double[][][])aa.copyToNDJavaArray();
			for (int i = 0; i < aa.Shape[0]; i++)
			{
				for (int j = 0; j < aa.Shape[1]; j++)
				{
					for (int k = 0; k < aa.Shape[2]; k++)
					{
						Assert.Equal(tData[i][j][k], newArray[i][j][k]);
					}
				}
			}
		}

		[Fact]
		public void TestTypes()
		{
			TypeSpecifics(new double[][][] { new double[][] { new double[] { 1, 2, 3, 4 }, new double[] { 2, 4, 6, 8 }, new double[] { 3, 6, 9, 12 } }, new double[][] { new double[] { 2.5, 5, 7.5, 10 }, new double[] { 5, 10, 15, 20 }, new double[] { 7.5, 15, 22.5, 30 } } });
			TypeSpecifics(new float[][] { new float[] { 1, 2, 3, 4 }, new float[] { 2, 4, 6, 8 }, new float[] { 3, 6, 9, 12 } });
			TypeSpecifics(new long[][] { new long[] { 1, 2, 3, 4 }, new long[] { 2, 4, 6, 8 }, new long[] { 3, 6, 9, 12 } });
			TypeSpecifics(new int[][] { new int[] { 1, 2, 3, 4 }, new int[] { 2, 4, 6, 8 }, new int[] { 3, 6, 9, 12 } });
			TypeSpecifics(new short[][] { new short[] { 1, 2, 3, 4 }, new short[] { 2, 4, 6, 8 }, new short[] { 3, 6, 9, 12 } });
			TypeSpecifics(new byte[][] { new byte[] { 1, 2, 3, 4 }, new byte[] { 2, 4, 6, 8 }, new byte[] { 3, 6, 9, 12 } });
			TypeSpecifics(new char[][] { new char[] { '1', '2', '3', '4' }, new char[] { '2', '4', '6', '8' }, new char[] { '3', '6', '9', '2' } });
			TypeSpecifics(new bool[][] { new bool[] { true, false, true }, new bool[] { false, true, false } });
		}

		private void TypeSpecifics(object javaArray)
		{
			NcArray aa = NcArray.makeFromJavaArray(javaArray, false);
			object newArray = aa.copyToNDJavaArray();
			TestEquals(javaArray, newArray);
		}

		private void TestEquals(object jArray, object newArray)
		{
			Type cType = jArray.GetType().GetElementType();
			int n = ((System.Array)jArray).Length;

			if (cType.IsPrimitive)
			{
				for (int i = 0; i < n; i++)
				{
					Assert.Equal(((System.Array)jArray).GetValue(i), ((System.Array)newArray).GetValue(i));
				}
			}
			else
			{
				for (int i = 0; i < n; i++) // recurse
				{
					TestEquals(((System.Array)jArray).GetValue(i), ((System.Array)newArray).GetValue(i));
				}
			}
		}
	}
}
