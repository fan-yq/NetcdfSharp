// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/ArrayTest.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.ArrayTest - array factory, constant arrays, reduce views.
	/// </summary>
	public class Ma2ArrayTests
	{
		[Fact]
		public void TestFactory()
		{
			NcArray zeroRank = NcArray.factory(DataType.INT, new int[0]);
			IndexIterator iter = zeroRank.getIndexIterator();
			int[] currentCounter = iter.CurrentCounter;
			Assert.Empty(currentCounter);
		}

		[Fact]
		public void TestUnsigned2()
		{
			int nz = 1;
			int ny = 2030;
			int nx = 1354;
			int size = nz * ny * nx;

			short[] vals = new short[size];
			for (int i = 0; i < size; i++)
			{
				vals[i] = (short)i;
			}

			NcArray data = NcArray.factory(DataType.USHORT, new int[] { nz, ny, nx }, vals);
			double sum = MAMath.sumDouble(data);
			double sumReduce = MAMath.sumDouble(data.reduce(0));
			Assert.Equal(sum, sumReduce, 1e-3);
		}

		// Regression test for https://github.com/Unidata/thredds/issues/581.
		[Fact]
		public void TestConstantArray_get1DJavaArray()
		{
			NcArray array = NcArray.factoryConstant(DataType.INT, new int[] { 3 }, new int[] { 47 });

			// Prior to fix, the actual value returned by get1DJavaArray was {47}.
			Assert.Equal(new int[] { 47, 47, 47 }, (int[])array.get1DJavaArray(typeof(int)));
		}

		[Fact]
		public void TestConstantArray_createView()
		{
			// For all Array subtypes, assert that we can create a logical view of an Array that uses an IndexConstant.
			// This is a regression test for the Array*.factory(Index, ...) methods.
			foreach (DataType dataType in Enum.GetValues(typeof(DataType)))
			{
				NcArray array = NcArray.factoryConstant(dataType, new int[] { 1, 3, 1 }, null);
				Assert.Equal(new int[] { 3 }, array.reduce().Shape);
			}
		}
	}
}
