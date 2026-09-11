// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestDimensions.java


using System;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestDimensions - anonymous dimensions and builder.
	/// </summary>
	public class Nc2DimensionsTests
	{
		[Fact]
		public void TestAnonDimFullName()
		{
			Dimension dim = new Dimension(null, 5, false, false, false); // Anonymous dimension.
			Assert.Null(dim.getShortName());
			Assert.Null(dim.FullName); // This used to cause a NullPointerException.
		}

		[Fact]
		public void TestBuilder()
		{
			Dimension dim = Dimension.builder().setName("name").setLength(7).setIsUnlimited(true).build();
			Assert.Equal("name", dim.getShortName());
			Assert.True(dim.Unlimited);

			Dimension dim2 = dim.toBuilder().setName("name2").build();
			Assert.Equal(new Dimension("name2", 7, true, true, false), dim2);
		}
	}
}
