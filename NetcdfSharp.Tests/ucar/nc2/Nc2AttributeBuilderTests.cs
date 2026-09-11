// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestAttributeBuilder.java
// and the pure-model parts of TestAttributes.java (the file-reading part of TestAttributes
// depends on a testdata file that is not part of the mock dataset and is not ported).


using System;
using System.Collections.Generic;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using NetcdfSharp.ucar.nc2.iosp.netcdf3;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestAttributeBuilder - NcAttribute builder and accessors.
	/// </summary>
	public class Nc2AttributeBuilderTests
	{
		[Fact]
		public void TestBuilder()
		{
			NcAttribute att = NcAttribute.builder().setName("name").setDataType(DataType.FLOAT)
				.setValues(NcArray.makeArray(DataType.FLOAT, new List<string> { "3.14", ".0015" })).build();
			Assert.Equal("name", att.ShortName);
			Assert.Equal(DataType.FLOAT, att.DataType);
			Assert.Equal(2, att.Length);
			Assert.Equal(float.Parse("3.14"), (float)att.getObject(0));
			Assert.Equal(float.Parse(".0015"), (float)att.getObject(1));

			Assert.Equal(float.Parse("3.14"), (float)att.NumericValue);
			Assert.Equal(float.Parse("3.14"), (float)att.getNumericValue(0));
			Assert.Equal(float.Parse(".0015"), (float)att.getNumericValue(1));

			Assert.True(att.Array);
			Assert.False(att.String);
			Assert.Null(att.StringValue);
			Assert.Null(att.EnumType);
		}

		[Fact]
		public void TestStringBuilder()
		{
			NcAttribute att = NcAttribute.builder().setName("name").setStringValue("666").build();
			Assert.Equal("name", att.ShortName);
			Assert.Equal(DataType.STRING, att.DataType);
			Assert.Equal(1, att.Length);

			Assert.Equal("666", att.getObject(0));
			Assert.Equal("666", att.StringValue);
			Assert.Equal(double.Parse("666"), Convert.ToDouble(att.NumericValue));

			Assert.False(att.Array);
			Assert.True(att.String);
			Assert.Null(att.EnumType);
		}

		[Fact]
		public void TestNumericBuilder()
		{
			NcAttribute att = NcAttribute.builder().setName("name").setNumericValue(123, false).build();
			Assert.Equal("name", att.ShortName);
			Assert.Equal(DataType.INT, att.DataType);
			Assert.Equal(1, att.Length);

			Assert.Equal(123, att.getObject(0));
			Assert.Null(att.StringValue);
			Assert.Equal(123, att.NumericValue);

			Assert.False(att.Array);
			Assert.False(att.String);
			Assert.Null(att.EnumType);
		}

		[Fact]
		public void TestNumericUnsignedBuilder()
		{
			NcAttribute att = NcAttribute.builder().setName("name").setNumericValue(123, true).build();
			Assert.Equal("name", att.ShortName);
			Assert.Equal(DataType.UINT, att.DataType);
			Assert.Equal(1, att.Length);

			Assert.Equal(123, att.getObject(0));
			Assert.Null(att.StringValue);
			Assert.Equal(123, att.NumericValue);

			Assert.False(att.Array);
			Assert.False(att.String);
			Assert.Null(att.EnumType);
		}

		[Fact]
		public void TestStringConstructor()
		{
			NcAttribute att = NcAttribute.builder().setName("name").setStringValue("666").build();
			Assert.Equal(new NcAttribute("name", "666"), att);
		}

		[Fact]
		public void TestArrayConstructor()
		{
			NcArray data = NcArray.makeArray(DataType.FLOAT, new List<string> { "3.14", ".0015" });
			NcAttribute att = NcAttribute.builder().setName("name").setValues(data).build();
			Assert.Equal(new NcAttribute("name", data), att);
		}

		[Fact]
		public void TestNumericConstructor()
		{
			NcAttribute att = NcAttribute.builder().setName("name").setNumericValue(123, true).build();
			Assert.Equal(new NcAttribute("name", 123, true), att);
		}

		[Fact]
		public void TestBuilderWithEnum()
		{
			Dictionary<int, string> map = new Dictionary<int, string> { { 123, "name" } };
			EnumTypedef typedef = new EnumTypedef("enum", map);
			NcAttribute att = NcAttribute.builder().setName("name").setNumericValue(123, true).setEnumType(typedef).build();
			Assert.Equal("name", att.ShortName);
			Assert.Equal(DataType.UINT, att.DataType);
			Assert.Equal(1, att.Length);

			Assert.Equal(123, att.getObject(0));
			Assert.Null(att.StringValue);
			Assert.Equal(123, att.NumericValue);

			Assert.False(att.Array);
			Assert.False(att.String);
			Assert.Equal(typedef, att.EnumType);
		}

		[Fact]
		public void TestLargeLongValue()
		{
			// Demonstrates https://github.com/Unidata/thredds/issues/715
			long ncFillInt64 = -9223372036854775806L;
			NcAttribute att = new NcAttribute("name", ncFillInt64);
			long result = Convert.ToInt64(att.NumericValue);
			Assert.Equal(ncFillInt64, result);
		}

		[Fact]
		public void TestStringBuilder2()
		{
			NcAttribute att = NcAttribute.builder().setName("name").setStringValue("svalue").build();
			Assert.Equal(new NcAttribute("name", "svalue"), att);
			NcAttribute att2 = att.toBuilder().setName("name2").build();
			Assert.Equal(new NcAttribute("name2", "svalue"), att2);
		}

		[Fact]
		public void TestBuilder2()
		{
			NcAttribute att = NcAttribute.builder().setName("name").setValues(new List<object> { 1, 2, 3 }, true).build();
			Assert.Equal(DataType.UINT, att.DataType);

			NcAttribute atts = NcAttribute.builder().setName("name").setValues(new List<object> { "1", "2", "3" }, false).build();
			Assert.Equal(DataType.STRING, atts.DataType);

			NcArray array = NcArray.factory(DataType.SHORT, new int[] { 4 }, new short[] { 1, 2, 3, 4 });
			NcAttribute att2 = NcAttribute.builder().setName("name").setValues(array).build();
			Assert.Equal(DataType.SHORT, att2.DataType);
			Assert.True(MAMath.equals(att2.Values, array));
		}
	}
}
