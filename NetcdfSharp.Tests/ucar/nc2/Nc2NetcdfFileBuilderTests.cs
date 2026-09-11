// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestNetcdfFileBuilder.java


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestNetcdfFileBuilder - NetcdfFile.Builder.
	/// </summary>
	public class Nc2NetcdfFileBuilderTests
	{
		[Fact]
		public void TestBuilder()
		{
			NcAttribute att = new NcAttribute("attName", "value");
			Dimension dim = new Dimension("dimName", 42);
			Group.Builder nested = Group.builder().setName("child");
			Variable.Builder vb = Variable.builder().setName("varName").setDataType(DataType.STRING);
			Group.Builder groupb = Group.builder().setName("name").addAttribute(att).addDimension(dim).addGroup(nested).addVariable(vb);

			NetcdfFile.Builder builder = NetcdfFile.builder().setId("Hid").setLocation("location").setRootGroup(groupb).setTitle("title");

			NetcdfFile ncfile = builder.build();
			Assert.Equal("Hid", ncfile.Id);
			Assert.Equal("location", ncfile.Location);
			Assert.Equal("title", ncfile.Title);

			Group group = ncfile.RootGroup;
			Assert.Equal(ncfile, group.NetcdfFile);
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
			Assert.Equal(ncfile, v.NetcdfFile);
		}
	}
}

