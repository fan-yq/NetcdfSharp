// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestMisc.java
// Only the self-contained parts are ported: the backslash-tokenization and array-concat helpers.
// The date-comparator and grid-related cases depend on out-of-scope modules and are omitted.


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2.util;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestMisc - misc testing (EscapeStrings + array concat).
	/// </summary>
	public class Nc2MiscTests
	{
		[Fact]
		public void TestBackslashTokens()
		{
			testBackslashTokens("group/name.member.mom");
			testBackslashTokens("var\\.name.member.mom");
			testBackslashTokens("var\\.name.member.mom\\");
			testBackslashTokens("var\\.name.member.mom.");
			testBackslashTokens(".var\\.name.member.mom.");
			testBackslashTokens("...mom.");
		}

		private void testBackslashTokens(string escapedName)
		{
			System.Collections.Generic.IList<string> result = EscapeStrings.tokenizeEscapedName(escapedName);
			Assert.NotEmpty(result);
		}

		[Fact]
		public void TestConcatArray()
		{
			NcArray array1 = NcArray.makeFromJavaArray(new float[] { 1, 2, 3 });
			NcArray array2 = NcArray.makeFromJavaArray(new float[] { 4, 5 });
			NcArray result = concatArray(array1, array2);
			Assert.Equal(5, result.Size);
			Assert.Equal(1.0f, result.getFloat(0));
			Assert.Equal(2.0f, result.getFloat(1));
			Assert.Equal(3.0f, result.getFloat(2));
			Assert.Equal(4.0f, result.getFloat(3));
			Assert.Equal(5.0f, result.getFloat(4));
		}

		private NcArray concatArray(NcArray array1, NcArray array2)
		{
			float[] data1 = (float[])array1.copyTo1DJavaArray();
			float[] data2 = (float[])array2.copyTo1DJavaArray();
			float[] result = new float[data1.Length + data2.Length];

			System.Array.Copy(data1, 0, result, 0, data1.Length);
			System.Array.Copy(data2, 0, result, data1.Length, data2.Length);

			// now put it back into an NetcdfSharp.ucar.ma2.Array
			int[] resultShape = new int[] { result.Length };
			return NcArray.factory(DataType.FLOAT, resultShape, result);
		}
	}
}
