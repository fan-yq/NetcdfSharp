// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestVariableBuilder.java


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestVariableBuilder - Variable.Builder dims/shape/copy.
	/// </summary>
	public class Nc2VariableBuilderTests
	{
		[Fact]
		public void TestBuilder()
		{
			Variable var = Variable.builder().setName("name").setDataType(DataType.FLOAT).build(TestUtils.makeDummyGroup());
			Assert.Equal(DataType.FLOAT, var.DataType);
			Assert.Equal("name", var.ShortName);
			Assert.True(var.Scalar);
		}

		[Fact]
		public void TestWithDims()
		{
			try
			{
				// Must set dimension first
				Variable.builder().setName("name").setDataType(DataType.FLOAT).setDimensionsByName("dim1 dim2").build(TestUtils.makeDummyGroup());
				throw new Exception("Should have thrown");
			}
			catch (Exception)
			{
				// ok
			}

			Group.Builder gb = Group.builder().addDimension(Dimension.builder("dim1", 7).setIsUnlimited(true).build()).addDimension(new Dimension("dim2", 27));

			Variable var = Variable.builder().setName("name").setDataType(DataType.FLOAT).setParentGroupBuilder(gb).setDimensionsByName("dim1 dim2").build(gb.build());
			Assert.Equal(DataType.FLOAT, var.DataType);
			Assert.Equal("name", var.ShortName);
			Assert.False(var.Scalar);
			Assert.True(var.Unlimited);
			Assert.Equal(new int[] { 7, 27 }, var.Shape);
			Assert.Equal(new Section(new int[] { 7, 27 }), var.ShapeAsSection);
		}

		[Fact]
		public void TestWithAnonymousDims()
		{
			int[] shape = new int[] { 3, 6, -1 };
			Variable var = Variable.builder().setName("name").setDataType(DataType.FLOAT).setDimensionsAnonymous(shape).build(TestUtils.makeDummyGroup());
			Assert.Equal(DataType.FLOAT, var.DataType);
			Assert.Equal("name", var.ShortName);
			Assert.False(var.Scalar);
			Assert.False(var.Unlimited);
			Assert.Equal(new int[] { 3, 6, -1 }, var.Shape);
			Assert.Equal(new Section(new int[] { 3, 6, -1 }), var.ShapeAsSection);
		}

		[Fact]
		public void TestCopy()
		{
			Group.Builder gb = Group.builder().addDimension(Dimension.builder("dim1", 7).setIsUnlimited(true).build()).addDimension(new Dimension("dim2", 27));
			Group group = gb.build();

			Variable var = Variable.builder().setName("name").setDataType(DataType.FLOAT).setParentGroupBuilder(gb).setDimensionsByName("dim1 dim2").build(group);

			Variable copy = var.toBuilder().build(group);

			Assert.Equal(group, copy.ParentGroup);
			Assert.Equal(DataType.FLOAT, copy.DataType);
			Assert.Equal("name", copy.ShortName);
			Assert.False(copy.Scalar);
			Assert.True(copy.Unlimited);
			Assert.Equal(new int[] { 7, 27 }, copy.Shape);
			Assert.Equal(new Section(new int[] { 7, 27 }), copy.ShapeAsSection);

			Assert.Equal((object)var, copy);
		}
	}
}
