// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestStructureArrayW.java


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.unidata.util.test;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestStructureArrayW - ArrayStructureW backed by StructureDataW objects.
	/// </summary>
	public class Ma2StructureArrayWTests
	{
		/*
		 * <pre>
		 * Structure {
		 * float f1;
		 * short f2(3);
		 * 
		 * Structure {
		 * int g1;
		 * double(2) g2;
		 * double(3,4) g3;
		 * 
		 * Structure {
		 * int h1;
		 * double(2) h2;
		 * } nested2(7);
		 * 
		 * } nested1(9);
		 * } s(4);
		 * </pre>
		 */
		[Fact]
		public void TestW()
		{
			StructureMembers members = new StructureMembers("s");

			StructureMembers.Member f1 = members.addMember("f1", "desc", "units", DataType.FLOAT, new int[] { 1 });

			StructureMembers.Member f2 = members.addMember("f2", "desc", "units", DataType.SHORT, new int[] { 3 });

			StructureMembers.Member nested1 = members.addMember("nested1", "desc", "units", DataType.STRUCTURE, new int[] { 9 });

			int size = 4;
			StructureData[] sdata = new StructureData[size];
			for (int i = 0; i < size; i++)
			{
				StructureDataW sdw = new StructureDataW(members);
				sdata[i] = sdw;

				NcArray data = NcArray.factory(DataType.FLOAT, new int[] { 1 });
				sdw.setMemberData(f1, data);
				fill(data, i);

				data = NcArray.factory(DataType.SHORT, new int[] { 3 });
				sdw.setMemberData(f2, data);
				fill(data, i * 2);

				data = makeNested1(nested1, 9, 7);
				sdw.setMemberData(nested1, data);
			}

			ArrayStructureW as_ = new ArrayStructureW(members, new int[] { 4 }, sdata);
			new UtilsTestStructureArray().testArrayStructure(as_);

			// get f2 out of the 2nd "s"
			short[] f2data = as_.getJavaArrayShort(1, f2);
			Assert.Equal(2, f2data[0]);
			Assert.Equal(3, f2data[1]);
			Assert.Equal(4, f2data[2]);

			// get nested1 out of the 3rd "s"
			ArrayStructure nested1Data = as_.getArrayStructure(2, nested1);

			// get g1 out of the 4th "nested1"
			StructureMembers.Member g1 = nested1Data.StructureMembers.findMember("g1");
			int g1data = nested1Data.getScalarInt(3, g1);
			Assert.Equal(66, g1data);

			// get g3 out of the 4th "nested1"
			StructureMembers.Member g3 = nested1Data.StructureMembers.findMember("g3");
			double[] g3data = nested1Data.getJavaArrayDouble(3, g3);
			Assert.Equal(73326.0, g3data[0]);

			// get nested2 out of the 7th "nested1"
			StructureMembers.Member nested2 = nested1Data.StructureMembers.findMember("nested2");
			ArrayStructure nested2Data = nested1Data.getArrayStructure(6, nested2);

			// get h1 out of the 5th "nested2"
			StructureMembers.Member h1 = nested2Data.StructureMembers.findMember("h1");
			int val = nested2Data.getScalarInt(4, h1);
			Assert.Equal(1218, val);

			// get h2 out of the 5th "nested2"
			StructureMembers.Member h2 = nested2Data.StructureMembers.findMember("h2");
			double[] h2data = nested2Data.getJavaArrayDouble(4, h2);
			Assert.Equal(12018, h2data[0]);
			Assert.Equal(12019, h2data[1]);
		}

		public NcArray makeNested1(StructureMembers.Member nested1, int size1, int size2)
		{
			StructureMembers members = new StructureMembers(nested1.Name);
			nested1.StructureMembers = members;

			StructureMembers.Member g1 = members.addMember("g1", "desc", "units", DataType.INT, new int[] { 1 });
			StructureMembers.Member g2 = members.addMember("g2", "desc", "units", DataType.DOUBLE, new int[] { 2 });
			StructureMembers.Member g3 = members.addMember("g3", "desc", "units", DataType.DOUBLE, new int[] { 3, 4 });
			StructureMembers.Member nested2 = members.addMember("nested2", "desc", "units", DataType.STRUCTURE, new int[] { 7 });

			StructureData[] sdata = new StructureData[size1];
			for (int i = 0; i < size1; i++)
			{
				StructureDataW sdw = new StructureDataW(members);
				sdata[i] = sdw;

				NcArray data = NcArray.factory(DataType.INT, new int[] { 1 });
				sdw.setMemberData(g1, data);
				fill(data, i * 22);

				data = NcArray.factory(DataType.DOUBLE, new int[] { 2 });
				sdw.setMemberData(g2, data);
				fill(data, i * 222);

				data = NcArray.factory(DataType.DOUBLE, new int[] { 3, 4 });
				sdw.setMemberData(g3, data);
				fill(data, i * 2222);

				data = makeNested2(nested2, i, size2);
				sdw.setMemberData(nested2, data);
			}

			return new ArrayStructureW(members, new int[] { size1 }, sdata);
		}

		public NcArray makeNested2(StructureMembers.Member nested, int who, int size)
		{
			StructureMembers members = new StructureMembers(nested.Name);
			nested.StructureMembers = members;

			StructureMembers.Member h1 = members.addMember("h1", "desc", "units", DataType.INT, new int[] { 1 });
			StructureMembers.Member h2 = members.addMember("h2", "desc", "units", DataType.DOUBLE, new int[] { 2 });

			StructureData[] sdata = new StructureData[size];
			for (int i = 0; i < size; i++)
			{
				StructureDataW sdw = new StructureDataW(members);
				sdata[i] = sdw;

				NcArray data = NcArray.factory(DataType.INT, new int[] { 1 });
				sdw.setMemberData(h1, data);
				fill(data, i * 303 + who);

				data = NcArray.factory(DataType.DOUBLE, new int[] { 2 });
				sdw.setMemberData(h2, data);
				fill(data, i * 3003 + who);
			}

			return new ArrayStructureW(members, new int[] { size }, sdata);
		}

		private void fill(NcArray a, int start)
		{
			IndexIterator ii = a.getIndexIterator();
			while (ii.hasNext())
			{
				_ = ii.IntNext;
				int[] counter = ii.CurrentCounter;
				int value = 0;
				for (int i = 0; i < counter.Length; i++)
				{
					value = start + value * 10 + counter[i];
				}
				ii.IntCurrent = value;
			}
		}
	}
}
