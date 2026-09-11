// Ported from Java: netcdf-java cdm-test-utils/src/main/java/ucar/unidata/util/test/UtilsTestStructureArray.java
// Java `assert` statements are converted to xUnit assertions so they actually run in the C# test suite.


using System;
using System.Collections.Generic;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.unidata.util.test
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.unidata.util.test.UtilsTestStructureArray - exercises an ArrayStructure through its
	/// recno / iterator / member-array access paths.
	/// </summary>
	public class UtilsTestStructureArray
	{
		public void testArrayStructure(ArrayStructure as_)
		{
			testArrayStructureByRecno(as_);
			testArrayStructureByIterator(as_);
			testArrayStructureByMemberArray(as_);
		}

		private double testArrayStructureByMemberArray(ArrayStructure as_)
		{
			IList<StructureMembers.Member> members = as_.Members;

			foreach (StructureMembers.Member m in members)
			{
				NcArray memberArray = as_.extractMemberArray(m);
				Assert.Equal(m.DataType.getPrimitiveClassType(), memberArray.ElementType);

				// random tests
				if (m.DataType.isNumeric())
				{
					double sum = 0.0;
					while (memberArray.hasNext())
					{
						sum += memberArray.nextDouble();
					}
					return sum;
				}
				else if (m.DataType.isString())
				{
					while (memberArray.hasNext())
					{
						Console.WriteLine(" " + memberArray.next());
					}
				}
			}
			return 0.0;
		}

		private void testArrayStructureByIterator(ArrayStructure as_)
		{
			using (StructureDataIterator si = as_.StructureDataIterator)
			{
				while (si.hasNext())
				{
					StructureData sdata = si.next();

					// run through each member on the StructureData
					IList<StructureMembers.Member> members = sdata.Members;
					foreach (StructureMembers.Member m in members)
					{
						NcArray sdataArray = sdata.getArray(m);
						Assert.Equal(m.DataType.getPrimitiveClassType(), sdataArray.ElementType);
					}

					testStructureData(sdata);
				}
			}
		}

		private void testArrayStructureByRecno(ArrayStructure as_)
		{
			// run through each StructureData
			for (int recno = 0; recno < as_.Size; recno++)
			{
				object o = as_.getObject(recno);
				Assert.IsAssignableFrom<StructureData>(o);
				StructureData sdata = as_.getStructureData(recno);
				Assert.Same(o, sdata);

				// run through each member on the StructureData
				IList<StructureMembers.Member> members = sdata.Members;
				foreach (StructureMembers.Member m in members)
				{
					NcArray sdataArray = sdata.getArray(m);
					Assert.True(sdataArray.ElementType == m.DataType.getPrimitiveClassType(), sdataArray.ElementType + " != " + m.DataType.getPrimitiveClassType());

					NcArray sdataArray2 = sdata.getArray(m.Name);
					UtilsMa2Test.testEquals(sdataArray, sdataArray2);

					NcArray a = as_.getArray(recno, m);
					Assert.Equal(m.DataType.getPrimitiveClassType(), a.ElementType);
					UtilsMa2Test.testEquals(sdataArray, a);

					testGetArrayByType(as_, recno, m, a);
				}

				testStructureData(sdata);
			}
		}

		private void testGetArrayByType(ArrayStructure as_, int recno, StructureMembers.Member m, NcArray a)
		{
			DataType dtype = m.DataType;
			object data = null;
			if (dtype == DataType.DOUBLE)
			{
				Assert.Equal(typeof(double), a.ElementType);
				data = as_.getJavaArrayDouble(recno, m);

			}
			else if (dtype == DataType.FLOAT)
			{
				Assert.Equal(typeof(float), a.ElementType);
				data = as_.getJavaArrayFloat(recno, m);

			}
			else if (dtype.getPrimitiveClassType() == typeof(long))
			{
				Assert.Equal(typeof(long), a.ElementType);
				data = as_.getJavaArrayLong(recno, m);

			}
			else if (dtype.getPrimitiveClassType() == typeof(int))
			{
				Assert.Equal(typeof(int), a.ElementType);
				data = as_.getJavaArrayInt(recno, m);

			}
			else if (dtype.getPrimitiveClassType() == typeof(short))
			{
				Assert.Equal(typeof(short), a.ElementType);
				data = as_.getJavaArrayShort(recno, m);

			}
			else if (dtype.getPrimitiveClassType() == typeof(byte))
			{
				Assert.Equal(typeof(byte), a.ElementType);
				data = as_.getJavaArrayByte(recno, m);

			}
			else if (dtype == DataType.CHAR)
			{
				Assert.Equal(typeof(char), a.ElementType);
				data = as_.getJavaArrayChar(recno, m);

			}
			else if (dtype == DataType.STRING)
			{
				Assert.Equal(typeof(string), a.ElementType);
				data = as_.getJavaArrayString(recno, m);

			}
			else if (dtype == DataType.STRUCTURE)
			{
				Assert.Equal(typeof(StructureData), a.ElementType);
				ArrayStructure nested = as_.getArrayStructure(recno, m);
				testArrayStructure(nested);
			}

			if (data != null)
			{
				UtilsMa2Test.testJarrayEquals(data, a.Storage, m.Size);
			}
		}

		private void testStructureData(StructureData sdata)
		{
			IList<StructureMembers.Member> members = sdata.Members;
			foreach (StructureMembers.Member m in members)
			{
				NcArray sdataArray = sdata.getArray(m);
				Assert.Equal(m.DataType.getPrimitiveClassType(), sdataArray.ElementType);

				NcArray sdataArray2 = sdata.getArray(m.Name);
				UtilsMa2Test.testEquals(sdataArray, sdataArray2);

				// NCdump.printArray(sdataArray, m.getName(), System.out, null);

				testGetArrayByType(sdata, m, sdataArray);
			}
		}

		private void testGetArrayByType(StructureData sdata, StructureMembers.Member m, NcArray a)
		{
			DataType dtype = m.DataType;
			object data = null;
			if (dtype == DataType.DOUBLE)
			{
				Assert.Equal(typeof(double), a.ElementType);
				data = sdata.getJavaArrayDouble(m);

			}
			else if (dtype == DataType.FLOAT)
			{
				Assert.Equal(typeof(float), a.ElementType);
				data = sdata.getJavaArrayFloat(m);

			}
			else if (dtype.getPrimitiveClassType() == typeof(long))
			{
				Assert.Equal(typeof(long), a.ElementType);
				data = sdata.getJavaArrayLong(m);

			}
			else if (dtype.getPrimitiveClassType() == typeof(int))
			{
				Assert.Equal(typeof(int), a.ElementType);
				data = sdata.getJavaArrayInt(m);

			}
			else if (dtype.getPrimitiveClassType() == typeof(short))
			{
				Assert.Equal(typeof(short), a.ElementType);
				data = sdata.getJavaArrayShort(m);

			}
			else if (dtype.getPrimitiveClassType() == typeof(byte))
			{
				Assert.Equal(typeof(byte), a.ElementType);
				data = sdata.getJavaArrayByte(m);

			}
			else if (dtype == DataType.CHAR)
			{
				Assert.Equal(typeof(char), a.ElementType);
				data = sdata.getJavaArrayChar(m);

			}
			else if (dtype == DataType.STRING)
			{
				Assert.Equal(typeof(string), a.ElementType);
				data = sdata.getJavaArrayString(m);

			}
			else if (dtype == DataType.STRUCTURE)
			{
				Assert.Equal(typeof(StructureData), a.ElementType);
				ArrayStructure nested = sdata.getArrayStructure(m);
				testArrayStructure(nested);
			}

			if (data != null)
			{
				UtilsMa2Test.testJarrayEquals(data, a.Storage, m.Size);
			}
		}
	}
}
