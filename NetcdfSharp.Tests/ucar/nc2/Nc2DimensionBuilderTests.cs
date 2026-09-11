// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestDimensionBuilder.java
// and TestDimensions.java


using System;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestDimensionBuilder + TestDimensions - Dimension builder and accessors.
	/// </summary>
	public class Nc2DimensionBuilderTests
	{
		[Fact]
		public void TestBuilder()
		{
			Dimension dim = Dimension.builder().setName("name").setLength(99).setIsUnlimited(true).setIsShared(false).build();
			Assert.Equal("name", dim.ShortName);
			Assert.Equal(99, dim.Length);
			Assert.True(dim.Unlimited);
			Assert.False(dim.Shared);
		}

		[Fact]
		public void TestVariableLengthBuilder()
		{
			Dimension dim = Dimension.builder().setName("name").setLength(99).setIsUnlimited(true).setIsShared(true)
				.setIsVariableLength(true) // overrides
				.build();
			Assert.Equal("name", dim.ShortName);
			Assert.Equal(-1, dim.Length);
			Assert.False(dim.Unlimited);
			Assert.False(dim.Shared);
			Assert.True(dim.VariableLength);
		}

		[Fact]
		public void TestConstructor()
		{
			Dimension dim = Dimension.builder().setName("name").setLength(99).build();
			Assert.Equal(new Dimension("name", 99), dim);
			Assert.False(dim.Unlimited);
			Assert.True(dim.Shared);
			Assert.False(dim.VariableLength);
		}

		[Fact]
		public void TestAnonDimFullName()
		{
			// From a bug report by Bob Simons.
			Dimension dim = new Dimension(null, 5, false, false, false); // Anonymous dimension.
			Assert.Null(dim.ShortName);
			Assert.Null(dim.makeFullName()); // This used to cause a NullPointerException.
		}

		[Fact]
		public void TestBuilder2()
		{
			Dimension dim = Dimension.builder().setName("name").setLength(7).setIsUnlimited(true).build();
			Assert.Equal("name", dim.ShortName);
			Assert.True(dim.Unlimited);

			Dimension dim2 = dim.toBuilder().setName("name2").build();
			Assert.Equal(new Dimension("name2", 7, true, true, false), dim2);
		}
	}
}
