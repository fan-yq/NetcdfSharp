// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestRank.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestRank - iteration over arrays of ranks 0..9.
	/// </summary>
	public class Ma2RankTests
	{
		private void DoIt(int rank)
		{
			int[] shape = new int[rank];
			for (int i = 0; i < rank; i++)
			{
				shape[i] = i + 2;
			}

			NcArray A = NcArray.factory(DataType.DOUBLE, shape);

			IndexIterator iter = A.getIndexIterator();
			int count = 0;
			while (iter.hasNext())
			{
				iter.DoubleNext = count++;
			}

			iter = A.getIndexIterator();
			count = 0;
			while (iter.hasNext())
			{
				double val = iter.DoubleNext;
				Assert.Equal((double)count++, val);
			}

			NcIndex ima = A.Index;
			for (int i = 0; i < rank; i++)
			{
				ima.setDim(i, i + 1);
			}
			double val2 = A.getDouble(ima);
			double myVal = ima.currentElement();
			Assert.Equal(myVal, val2);
		}

		[Fact]
		public void TestRank()
		{
			for (int rank = 0; rank < 10; rank++)
			{
				DoIt(rank);
			}
		}
	}
}
