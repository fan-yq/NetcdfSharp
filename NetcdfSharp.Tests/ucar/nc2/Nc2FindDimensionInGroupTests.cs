// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestFindDimensionInGroup.java
// The C# port has Group.findDimension (short-name search, walking parents) but no
// NetcdfFile.findDimension path resolver; the assertions here use the Group API directly.


using System;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestFindDimensionInGroup - finding dimensions across nested groups.
	/// </summary>
	public class Nc2FindDimensionInGroupTests
	{
		[Fact]
		public void FindDim()
		{
			NetcdfFile ncFile = new NetcdfFileSubclass();

			Group subGroup = new Group(ncFile, ncFile.RootGroup, "subGroup");
			ncFile.RootGroup.addGroup(subGroup);

			Group subSubGroup = new Group(ncFile, subGroup, "subSubGroup");
			subGroup.addGroup(subSubGroup);

			Dimension dim = new Dimension("dim", 12);
			ncFile.RootGroup.addDimension(dim);

			Dimension subDim = new Dimension("subDim", 7);
			subGroup.addDimension(subDim);

			Dimension subSubDim = new Dimension("subSubDim", 3);
			subSubGroup.addDimension(subSubDim);

			ncFile.finish();

			// root group: find local, or walk up parents from a nested group
			Assert.Same(dim, ncFile.RootGroup.findDimension("dim"));
			Assert.Same(subDim, ncFile.RootGroup.findGroupLocal("subGroup").findDimension("subDim"));
			Assert.Same(subSubDim, ncFile.RootGroup.findGroupLocal("subGroup").findGroupLocal("subSubGroup").findDimension("subSubDim"));

			// findDimension walks up to parents: a nested group can find a dimension defined above it
			Assert.Same(dim, subGroup.findDimension("dim"));
			Assert.Same(dim, subSubGroup.findDimension("dim"));
			Assert.Same(subDim, subSubGroup.findDimension("subDim"));

			// non-existent
			Assert.Null(ncFile.RootGroup.findDimension("subGroup/nonExistentDim"));
			Assert.Null(ncFile.RootGroup.findGroupLocal("subGroup").findDimension("nonExistentDim"));
		}
	}
}
