// Tests for the C#-convenience typed value accessors:
//   NcArray.getValue<T>() / getValue<T>(int) / getValue<T>(params int[]) / to1D<T>()
//   NcAttribute.getValue<T>() / getValue<T>(int)


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	public class Ma2ValueAccessorTests
	{
		[Fact]
		public void ScalarGetValue()
		{
			NcArray s = NcArray.factory(DataType.DOUBLE, new int[0]);
			s.setDouble(0, 3.5);
			Assert.Equal(3.5, s.getValue<double>());
			Assert.Equal(3.5f, s.getValue<float>());
			// Convert.ToInt32 uses banker's rounding: 3.5 -> 4
			Assert.Equal(4, s.getValue<int>());
		}

		[Fact]
		public void LinearGetValue()
		{
			NcArray a = NcArray.factory(DataType.FLOAT, new int[] { 3 });
			a.setFloat(0, 1.5f);
			a.setFloat(1, 2.5f);
			a.setFloat(2, 3.5f);
			Assert.Equal(1.5, a.getValue<double>(0));
			Assert.Equal(2.5, a.getValue<double>(1));
			// single-int on a 1-D array is linear access, not a rank check
			Assert.Equal(2, a.getValue<int>(0));
		}

		[Fact]
		public void MultiDimGetValue()
		{
			NcArray a = NcArray.factory(DataType.INT, new int[] { 2, 3 });
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					a.setInt(a.Index.set(i, j), i * 10 + j);
				}
			}
			Assert.Equal(0, a.getValue<int>(0, 0));
			Assert.Equal(11, a.getValue<int>(1, 1));
			Assert.Equal(12, a.getValue<int>(1, 2));
			Assert.Equal(0, a.getValue<int>(new[] { 0, 0 }));
			Assert.Equal(12, a.getValue<int>(new[] { 1, 2 }));
		}

		[Fact]
		public void MultiDimGetValueLinearAccess()
		{
			// a single int argument on a multi-dim array selects the linear-index overload
			NcArray a = NcArray.factory(DataType.INT, new int[] { 2, 3 });
			a.setInt(0, 42);
			a.setInt(1, 43);
			Assert.Equal(42, a.getValue<int>(0));
			Assert.Equal(43, a.getValue<int>(1));
		}

		[Fact]
		public void MultiDimGetValueWrongRank()
		{
			NcArray a = NcArray.factory(DataType.INT, new int[] { 2, 3 });
			Assert.Throws<InvalidRangeException>(() => a.getValue<int>(1, 1, 1));
			Assert.Throws<InvalidRangeException>(() => a.getValue<int>(new[] { 1 }));
		}

		[Fact]
		public void ScalarGetValueThrowsForNonScalar()
		{
			NcArray a = NcArray.factory(DataType.INT, new int[] { 2 });
			Assert.Throws<InvalidOperationException>(() => a.getValue<int>());
		}

		[Fact]
		public void To1DTypedArray()
		{
			NcArray a = NcArray.factory(DataType.DOUBLE, new int[] { 2, 2 });
			a.setDouble(0, 1.0);
			a.setDouble(1, 2.0);
			a.setDouble(2, 3.0);
			a.setDouble(3, 4.0);

			double[] flat = a.to1D<double>();
			Assert.Equal(new double[] { 1.0, 2.0, 3.0, 4.0 }, flat);
		}

		[Fact]
		public void To1DTypedArrayConverts()
		{
			NcArray a = NcArray.factory(DataType.FLOAT, new int[] { 3 });
			a.setFloat(0, 1.0f);
			a.setFloat(1, 2.0f);
			a.setFloat(2, 3.0f);

			int[] asInt = a.to1D<int>();
			Assert.Equal(new int[] { 1, 2, 3 }, asInt);
		}

		[Fact]
		public void To1DTypedArrayEmpty()
		{
			NcArray a = NcArray.factory(DataType.INT, new int[] { 0 });
			Assert.Empty(a.to1D<int>());
		}

		[Fact]
		public void AttributeGetValue()
		{
			NcAttribute att = new NcAttribute("scale", 0.1);
			Assert.Equal(0.1, att.getValue<double>());
			Assert.Equal(0.1f, att.getValue<float>());
			Assert.Equal(0, att.getValue<int>());

			NcAttribute str = new NcAttribute("units", "degC");
			Assert.Equal("degC", str.getValue<string>());
		}

		[Fact]
		public void AttributeGetValueIndex()
		{
			NcArray vals = NcArray.factory(DataType.INT, new int[] { 2 });
			vals.setInt(0, 0);
			vals.setInt(1, 100);
			NcAttribute att = new NcAttribute("valid_range", vals);
			Assert.Equal(0, att.getValue<int>(0));
			Assert.Equal(100, att.getValue<int>(1));
			Assert.Equal(100.0, att.getValue<double>(1));
		}
	}
}
