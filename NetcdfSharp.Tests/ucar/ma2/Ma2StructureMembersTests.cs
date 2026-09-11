// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestStructureMembers.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestStructureMembers - StructureMembers / Member equality.
	/// </summary>
	public class Ma2StructureMembersTests
	{
		// test cases: (name, desc, units, dtype, shape, isEqual)
		public static object[][] GetTestParameters()
		{
			return new object[][]
			{
				new object[] { "name", "description", "units", DataType.BOOLEAN, new int[] { 1 }, true },
				new object[] { "otherName", "description", "units", DataType.BOOLEAN, new int[] { 1 }, false },
				new object[] { "name", "otherDescription", "units", DataType.BOOLEAN, new int[] { 1 }, false },
				new object[] { "name", null, "units", DataType.BOOLEAN, new int[] { 1 }, false },
				new object[] { "name", "description", "OtherUnits", DataType.BOOLEAN, new int[] { 1 }, false },
				new object[] { "name", "description", null, DataType.BOOLEAN, new int[] { 1 }, false },
				new object[] { "name", "description", "units", DataType.INT, new int[] { 1 }, false },
				new object[] { "name", "description", "units", DataType.BOOLEAN, new int[] { 2 }, false },
				new object[] { "name", "description", "units", DataType.BOOLEAN, new int[] { -1 }, false },
				new object[] { "name", "description", "units", DataType.BOOLEAN, new int[] { 1, 1 }, false },
			};
		}

		[Theory]
		[MemberData(nameof(GetTestParameters))]
		public void ShouldCompareMembers(string name, string description, string units, DataType dataType, int[] shape, bool isEqual)
		{
			StructureMembers.Member member = StructureMembers.builder().addMember("name", "description", "units", DataType.BOOLEAN, new int[] { 1 }).build();
			StructureMembers.Member otherMember = StructureMembers.builder().addMember(name, description, units, dataType, shape).build();

			Assert.Equal(isEqual, member.Equals(otherMember));
			Assert.Equal(isEqual, otherMember.Equals(member));
		}

		[Theory]
		[MemberData(nameof(GetTestParameters))]
		public void ShouldCompareStructureMembers(string name, string description, string units, DataType dataType, int[] shape, bool isEqual)
		{
			StructureMembers.MemberBuilder memberBuilder = StructureMembers.builder().addMember("name", "description", "units", DataType.BOOLEAN, new int[] { 1 });
			StructureMembers structureMembers = StructureMembers.builder().addMember(memberBuilder).build();

			StructureMembers.MemberBuilder otherMember = StructureMembers.builder().addMember(name, description, units, dataType, shape);
			StructureMembers otherStructureMembers = StructureMembers.builder().addMember(otherMember).build();

			Assert.Equal(isEqual, structureMembers.Equals(otherStructureMembers));
			Assert.Equal(isEqual, otherStructureMembers.Equals(structureMembers));
		}

		[Theory]
		[MemberData(nameof(GetTestParameters))]
		public void ShouldCompareStructureMembersWithMultipleMembers(string name, string description, string units, DataType dataType, int[] shape, bool isEqual)
		{
			StructureMembers.MemberBuilder memberBuilder = StructureMembers.builder().addMember("name1", "description1", "units1", DataType.INT, new int[] { 2 });
			StructureMembers.MemberBuilder memberBuilder2 = StructureMembers.builder().addMember("name", "description", "units", DataType.BOOLEAN, new int[] { 1 });
			StructureMembers structureMembers = StructureMembers.builder().addMember(memberBuilder).addMember(memberBuilder2).build();

			StructureMembers.MemberBuilder otherMemberBuilder = StructureMembers.builder().addMember("name1", "description1", "units1", DataType.INT, new int[] { 2 });
			StructureMembers.MemberBuilder otherMemberBuilder2 = StructureMembers.builder().addMember(name, description, units, dataType, shape);
			StructureMembers otherStructureMembers = StructureMembers.builder().addMember(otherMemberBuilder).addMember(otherMemberBuilder2).build();

			Assert.Equal(isEqual, structureMembers.Equals(otherStructureMembers));
			Assert.Equal(isEqual, otherStructureMembers.Equals(structureMembers));
		}

		[Theory]
		[MemberData(nameof(GetTestParameters))]
		public void ShouldCompareMembersThatAreStructures(string name, string description, string units, DataType dataType, int[] shape, bool isEqual)
		{
			StructureMembers.MemberBuilder innerMemberBuilder = StructureMembers.builder().addMember("name", "description", "units", DataType.BOOLEAN, new int[] { 1 });
			StructureMembers innerStructureMembers = StructureMembers.builder().addMember(innerMemberBuilder).build();
			StructureMembers.MemberBuilder mb = StructureMembers.builder().addMember("name2", "description2", "units2", DataType.STRUCTURE, new int[] { 1 });
			mb.StructureMembers = innerStructureMembers;
			StructureMembers.Member member = mb.build();

			StructureMembers.MemberBuilder otherInnerMember = StructureMembers.builder().addMember(name, description, units, dataType, shape);
			StructureMembers otherInnerStructureMembers = StructureMembers.builder().addMember(otherInnerMember).build();
			StructureMembers.MemberBuilder omb = StructureMembers.builder().addMember("name2", "description2", "units2", DataType.STRUCTURE, new int[] { 1 });
			omb.StructureMembers = otherInnerStructureMembers;
			StructureMembers.Member otherMember = omb.build();

			Assert.Equal(isEqual, member.Equals(otherMember));
			Assert.Equal(isEqual, otherMember.Equals(member));
		}
	}
}
