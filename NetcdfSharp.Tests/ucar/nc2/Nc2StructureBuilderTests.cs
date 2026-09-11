// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestStructureBuilder.java


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestStructureBuilder - Structure.Builder member variables.
	/// </summary>
	public class Nc2StructureBuilderTests
	{
		[Fact]
		public void TestBuilder()
		{
			Variable.Builder var = Variable.builder().setName("member").setDataType(DataType.FLOAT);
			Structure @struct = Structure.builder1().setName1("name").setDataType1(DataType.FLOAT).addMemberVariable(var).build(TestUtils.makeDummyGroup());
			Assert.Equal(DataType.STRUCTURE, @struct.DataType);
			Assert.Equal("name", @struct.ShortName);
			Assert.True(@struct.Scalar);
			Assert.Single(@struct.VariableNames);
			Assert.Equal("member", @struct.VariableNames[0]);
			Assert.Single(@struct.Variables);
			Assert.Equal("member", @struct.Variables[0].getShortName());
			Assert.Equal(DataType.FLOAT, @struct.Variables[0].DataType);
		}

		[Fact]
		public void TestBuilderChain()
		{
			Structure @struct = Structure.builder1().setName1("struct").addMemberVariables(new System.Collections.Generic.List<Variable.Builder>()).build(TestUtils.makeDummyGroup());
			Assert.Equal(DataType.STRUCTURE, @struct.DataType);
			Assert.Equal("struct", @struct.ShortName);
			Assert.Empty(@struct.VariableNames);
			Assert.Empty(@struct.Variables);
		}

		[Fact]
		public void TestToBuilderChain()
		{
			Variable.Builder var = Variable.builder().setName("member").setDataType(DataType.FLOAT);
			Structure @struct = Structure.builder1().setName1("name").setDataType1(DataType.FLOAT).addMemberVariable(var).build(TestUtils.makeDummyGroup());
			Structure struct2 = @struct.toBuilder().setName1("s2").build(TestUtils.makeDummyGroup());
			Assert.Equal(DataType.STRUCTURE, struct2.DataType);
			Assert.Equal("s2", struct2.ShortName);

			Assert.Single(@struct.VariableNames);
			Assert.Equal("member", @struct.VariableNames[0]);
			Assert.Single(@struct.Variables);
			Assert.Equal("member", @struct.Variables[0].getShortName());
			Assert.Equal(DataType.FLOAT, @struct.Variables[0].DataType);
		}
	}
}

