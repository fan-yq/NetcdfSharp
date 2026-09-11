// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestGroupBuilder.java


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestGroupBuilder - Group.Builder dimension/variable/group management.
	/// </summary>
	public class Nc2GroupBuilderTests
	{
		[Fact]
		public void TestBuilder()
		{
			NcAttribute att = new NcAttribute("attName", "value");
			Dimension dim = new Dimension("dimName", 42);
			Group.Builder nested = Group.builder().setName("child");
			Variable.Builder vb = Variable.builder().setName("varName").setDataType(DataType.STRING);
			Group group = Group.builder().setName("name").addAttribute(att).addDimension(dim).addGroup(nested).addVariable(vb).build();

			Assert.Equal("name", group.ShortName);
			Assert.True(group.Root);
			Assert.NotEmpty(group.attributes());
			Assert.Single(group.attributes().Attributes);
			Assert.Equal(att, group.findAttribute("attName"));
			Assert.Equal("value", group.findAttributeString("attName", null));

			Assert.NotEmpty(group.Dimensions);
			Assert.Single(group.Dimensions);
			Assert.Equal(dim, group.findDimension("dimName"));

			Assert.NotEmpty(group.Groups);
			Assert.Single(group.Groups);
			Group child = group.findGroupLocal("child");
			Assert.Equal(group, child.ParentGroup);

			Assert.NotEmpty(group.Variables);
			Assert.Single(group.Variables);
			Variable v = group.findVariableLocal("varName");
			Assert.Equal(group, v.ParentGroup);
		}

		[Fact]
		public void TestReplaceDimension()
		{
			Dimension dim = new Dimension("dimName", 42);
			Group.Builder builder = Group.builder().setName("name");

			Assert.False(builder.replaceDimension(dim));
			Assert.Equal(dim, builder.findDimension("dimName").get());

			Dimension dim2 = new Dimension("dimName", 99);
			Assert.True(builder.replaceDimension(dim2));
			Assert.Equal(dim2, builder.findDimension("dimName").get());
		}

		[Fact]
		public void TestDuplicateDimension()
		{
			Dimension dim = new Dimension("dimName", 42);
			Group.Builder builder = Group.builder().setName("name").addDimension(dim);

			Exception e = Assert.ThrowsAny<Exception>(() => builder.addDimension(dim));
			Assert.Contains("Dimension 'dimName' already exists", e.Message);
		}

		[Fact]
		public void TestRemoveGroup()
		{
			Group.Builder child = Group.builder().setName("child");
			Group.Builder child2 = Group.builder().setName("child2");
			Group.Builder builder = Group.builder().setName("name").addGroup(child).addGroup(child2);

			Assert.Equal(2, builder.gbuilders.Count);
			Assert.True(builder.removeGroup("child"));
			Assert.Single(builder.gbuilders);
			Assert.False(builder.findGroupLocal("child").Present);
			Assert.True(builder.findGroupLocal("child2").Present);
		}

		[Fact]
		public void TestSetDimensionsByNameNeedsParent()
		{
			Assert.ThrowsAny<NullReferenceException>(() => Variable.builder().setName("varName").setDimensionsByName("dim"));
		}

		[Fact]
		public void TestDimNotExist()
		{
			Group.Builder gb = Group.builder().setName("name");
			Exception e = Assert.ThrowsAny<Exception>(() => Variable.builder().setName("varName").setParentGroupBuilder(gb).setDimensionsByName("dim"));
			Assert.Contains("Dimension dim does not exist", e.Message);
		}

		[Fact]
		public void TestReplaceVariable()
		{
			Variable.Builder vb = Variable.builder().setName("varName");
			Group.Builder gb = Group.builder().setName("name").addDimension(new Dimension("dim", 42));

			Assert.False(gb.replaceVariable(vb));
			Assert.Equal(vb, gb.findVariableLocal("varName").get());

			Variable.Builder vb2 = Variable.builder().setName("varName").setParentGroupBuilder(gb).setDimensionsByName("dim");
			Assert.True(gb.replaceVariable(vb2));
			Assert.Equal(vb2, gb.findVariableLocal("varName").get());
		}

		[Fact]
		public void TestDuplicateVariable()
		{
			Variable.Builder vb = Variable.builder().setName("varName");
			Group.Builder builder = Group.builder().setName("name").addVariable(vb);

			Exception e = Assert.ThrowsAny<Exception>(() => builder.addVariable(vb));
			Assert.Contains("Variable 'varName' already exists", e.Message);
		}

		[Fact]
		public void TestAttributes()
		{
			NcAttribute att1 = new NcAttribute("attName", "value");
			NcAttribute att2 = new NcAttribute("attName2", "value2");
			Group.Builder builder = Group.builder().setName("name").addAttribute(att1).addAttribute(att2);

			AttributeContainer atts = builder.AttributeContainer;
			Assert.NotEmpty(atts.Attributes);
			Assert.Equal(2, atts.Attributes.Count);
			Assert.Equal(att1, atts.findAttribute("attName"));
			Assert.Equal("value", atts.findAttributeString("attName", null));
			Assert.Equal("value2", atts.findAttributeString("attName2", null));
		}
	}
}


