// Ported from Java: netcdf-java cdm-test-utils/src/main/java/ucar/unidata/util/test/UtilsMa2Test.java
// The Java original used NetcdfSharp.java.lang.reflect.Array.get(element-wise compare); ported via System.Array.GetValue.


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.unidata.util.test
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.unidata.util.test.UtilsMa2Test - ma2 testing utilities.
	/// </summary>
	public class UtilsMa2Test
	{
		public static void testEquals(NcArray array1, NcArray array2)
		{
			Assert.Equal(array1.ElementType, array2.ElementType);
			Assert.Equal(array1.Size, array2.Size);

			testShape(array1.Shape, array2.Shape);

			// see if backing store is identical content
			// NOTE: Java original reads array1.getStorage() for both (self-comparison); keep faithful.
			System.Array jarray1 = (System.Array)array1.Storage;
			System.Array jarray2 = (System.Array)array1.Storage;
			int n = (int)array1.Size;
			for (int i = 0; i < n; i++)
			{
				Assert.Equal(jarray1.GetValue(i), jarray2.GetValue(i));
			}
		}

		private static void testShape(int[] shape1, int[] shape2)
		{
			Assert.Equal(shape1.Length, shape2.Length);
			for (int i = 0; i < shape1.Length; i++)
			{
				Assert.Equal(shape1[i], shape2[i]);
			}
		}

		public static void testJarrayEquals(object jarray1, object jarray2, int n)
		{
			System.Array a1 = (System.Array)jarray1;
			System.Array a2 = (System.Array)jarray2;
			for (int i = 0; i < n; i++)
			{
				Assert.Equal(a1.GetValue(i), a2.GetValue(i));
			}
		}
	}
}
