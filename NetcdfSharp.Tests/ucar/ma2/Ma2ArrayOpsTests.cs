// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestArrayOps.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestArrayOps - reshape and its size-validation.
	/// </summary>
	public class Ma2ArrayOpsTests
	{
		private const int m = 4, n = 3, p = 2;
		private static readonly int[] shapeArray = { m, n, p };

		private NcArray MakeArray()
		{
			NcArray array = NcArray.factory(DataType.DOUBLE, shapeArray);
			NcIndex index = array.Index;

			int count = 0;
			for (int i = 0; i < m; i++)
			{
				index.set0(i);
				for (int j = 0; j < n; j++)
				{
					index.set1(j);
					for (int k = 0; k < p; k++)
					{
						index.set2(k);
						array.setDouble(index, count++);
					}
				}
			}
			return array;
		}

		[Fact]
		public void TestReshape()
		{
			NcArray array = MakeArray();

			CheckArrayValues(array.reshape(new int[] { 4, 6 }));
			CheckArrayValues(array.reshape(new int[] { 24 }));
			CheckArrayValues(array.reshape(new int[] { 2, 2, 3, 2 }));
		}

		[Fact]
		public void ShouldThrowWhenSizeIsIncorrect()
		{
			NcArray array = MakeArray();
			Assert.Throws<ArgumentException>(() => array.reshape(new int[] { 12 }));
		}

		private static void CheckArrayValues(NcArray array)
		{
			IndexIterator indexIterator = array.getIndexIterator();
			int count = 0;
			while (indexIterator.hasNext())
			{
				Assert.Equal(count++, indexIterator.DoubleNext, 1.0E-10);
			}
		}
	}
}
