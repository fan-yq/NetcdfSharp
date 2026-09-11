// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestStructureArrayBB.java


using System;
using NetcdfSharp.java.nio;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.unidata.util.test;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestStructureArrayBB - ArrayStructureBB with nested structures and offset-based layout.
	/// </summary>
	public class Ma2StructureArrayBBTests
	{
		/*
		 * <pre>
		 * Structure {
		 * int f1;
		 * int f2(9);
		 * 
		 * Structure {
		 * int g1;
		 * int(2) g2;
		 * int(3,4) g3;
		 * 
		 * Structure {
		 * int(3) h1;
		 * int(2) h2;
		 * } nested2(17);
		 * 
		 * } nested1(10);
		 * } s(4);
		 * </pre>
		 * so
		 * <pre>
		 * each s record 1010 * 4 = 4040
		 * 10 ints
		 * 10 nested1 10 * 100 = 1000
		 * 15 ints
		 * 17 nested2 17 * 5 = 85
		 * 5 ints
		 */
		[Fact]
		public void TestBB()
		{
			StructureMembers members = new StructureMembers("s");
			members.addMember("f1", "desc", "units", DataType.INT, new int[] { 1 });
			members.addMember("f2", "desc", "units", DataType.INT, new int[] { 9 }); // 10

			StructureMembers.Member nested1 = members.addMember("nested1", "desc", "units", DataType.STRUCTURE, new int[] { 10 });
			StructureMembers nested1_members = new StructureMembers("nested1");
			nested1_members.addMember("g1", "desc", "units", DataType.INT, new int[] { 1 });
			nested1_members.addMember("g2", "desc", "units", DataType.INT, new int[] { 2 });
			nested1_members.addMember("g3", "desc", "units", DataType.INT, new int[] { 3, 4 }); // (15 + 85) * 10 = 1000

			StructureMembers.Member nested2 = nested1_members.addMember("nested2", "desc", "units", DataType.STRUCTURE, new int[] { 17 }); // 5 * 17 = 85
			nested1.StructureMembers = nested1_members;

			StructureMembers nested2_members = new StructureMembers("nested2");
			nested2_members.addMember("h1", "desc", "units", DataType.INT, new int[] { 3 });
			nested2_members.addMember("h2", "desc", "units", DataType.INT, new int[] { 2 }); // 5
			nested2.StructureMembers = nested2_members;

			ArrayStructureBB.setOffsets(members);
			int[] offs = { 0, 4, 40 };
			for (int i = 0; i < offs.Length; ++i)
			{
				StructureMembers.Member m = members.getMember(i);
				Assert.Equal(offs[i], m.DataParam);
			}

			int[] offs2 = { 0, 4, 12, 60 };
			for (int i = 0; i < offs2.Length; ++i)
			{
				StructureMembers.Member m = nested1_members.getMember(i);
				Assert.Equal(offs2[i], m.DataParam);
			}

			ArrayStructureBB bb = new ArrayStructureBB(members, new int[] { 4 });
			fillStructureArray(bb);

			new UtilsTestStructureArray().testArrayStructure(bb);

			int sreclen = 1010;
			int n1reclen = 100;
			int n2reclen = 5;

			// get f2 out of the 3rd "s"
			int srecno = 2;
			StructureMembers.Member f2 = bb.StructureMembers.findMember("f2");
			int[] f2data = bb.getJavaArrayInt(srecno, f2);
			Assert.Equal(srecno * sreclen + 1, f2data[0]);
			Assert.Equal(srecno * sreclen + 2, f2data[1]);
			Assert.Equal(srecno * sreclen + 3, f2data[2]);

			// get nested1 out of the 3rd "s"
			ArrayStructure nested1Data = bb.getArrayStructure(srecno, nested1);
			// get g1 out of the 7th "nested1"
			int n1recno = 6;
			StructureMembers.Member g1 = nested1Data.StructureMembers.findMember("g1");
			int g1data = nested1Data.getScalarInt(n1recno, g1);
			Assert.Equal(srecno * sreclen + n1recno * n1reclen + 10, g1data);

			// get nested2 out of the 7th "nested1"
			ArrayStructure nested2Data = nested1Data.getArrayStructure(n1recno, nested2);
			// get h1 out of the 4th "nested2"
			int n2recno = 3;
			StructureMembers.Member h1 = nested2Data.StructureMembers.findMember("h1");
			int val = nested2Data.getScalarInt(n2recno, h1);
			Assert.Equal(srecno * sreclen + n1recno * n1reclen + n2recno * n2reclen + 15 + 10, val);
		}

		private void fillStructureArray(ArrayStructureBB sa)
		{
			ByteBuffer bb = sa.ByteBuffer;
			IntBuffer ibb = bb.asIntBuffer();
			int count = 0;
			for (int i = 0; i < ibb.capacity(); i++)
			{
				ibb.put(i, count++);
			}
		}
	}
}
