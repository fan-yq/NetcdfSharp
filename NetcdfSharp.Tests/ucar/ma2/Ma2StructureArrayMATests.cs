// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestStructureArrayMA.java


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.unidata.util.test;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestStructureArrayMA - ArrayStructureMA with member arrays and nested structures.
	/// </summary>
	public class Ma2StructureArrayMATests
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
		public void TestMA()
		{
			StructureMembers members = new StructureMembers("s");

			StructureMembers.Member m = members.addMember("f1", "desc", "units", DataType.FLOAT, new int[] { 1 });
			NcArray data = NcArray.factory(DataType.FLOAT, new int[] { 4 });
			m.DataArray = data;
			fill(data);

			m = members.addMember("f2", "desc", "units", DataType.SHORT, new int[] { 3 });
			data = NcArray.factory(DataType.SHORT, new int[] { 4, 3 });
			m.DataArray = data;
			fill(data);

			m = members.addMember("nested1", "desc", "units", DataType.STRUCTURE, new int[] { 9 });
			data = makeNested1(m);
			m.DataArray = data;

			ArrayStructureMA as_ = new ArrayStructureMA(members, new int[] { 4 });
			new UtilsTestStructureArray().testArrayStructure(as_);

			// get f2 out of the 3rd "s"
			StructureMembers.Member f2 = as_.StructureMembers.findMember("f2");
			short[] f2data = as_.getJavaArrayShort(2, f2);
			Assert.Equal(20, f2data[0]);
			Assert.Equal(21, f2data[1]);
			Assert.Equal(22, f2data[2]);

			// get nested1 out of the 3rd "s"
			StructureMembers.Member nested1 = as_.StructureMembers.findMember("nested1");
			ArrayStructure nested1Data = as_.getArrayStructure(2, nested1);

			// get g1 out of the 7th "nested1"
			StructureMembers.Member g1 = nested1Data.StructureMembers.findMember("g1");
			int g1data = nested1Data.getScalarInt(6, g1);
			Assert.Equal(26, g1data);

			// get nested2 out of the 7th "nested1"
			StructureMembers.Member nested2 = nested1Data.StructureMembers.findMember("nested2");
			ArrayStructure nested2Data = nested1Data.getArrayStructure(6, nested2);

			// get h1 out of the 4th "nested2"
			StructureMembers.Member h1 = nested2Data.StructureMembers.findMember("h1");
			int val = nested2Data.getScalarInt(4, h1);
			Assert.Equal(264, val);
		}

		private ArrayStructure makeNested1(StructureMembers.Member parent)
		{
			StructureMembers members = new StructureMembers(parent.Name);
			parent.StructureMembers = members;

			StructureMembers.Member m = members.addMember("g1", "desc", "units", DataType.INT, new int[] { 1 });
			NcArray data = NcArray.factory(DataType.INT, new int[] { 4, 9 });
			m.DataArray = data;
			fill(data);

			m = members.addMember("g2", "desc", "units", DataType.DOUBLE, new int[] { 2 });
			data = NcArray.factory(DataType.DOUBLE, new int[] { 4, 9, 2 });
			m.DataArray = data;
			fill(data);

			m = members.addMember("g3", "desc", "units", DataType.DOUBLE, new int[] { 3, 4 });
			data = NcArray.factory(DataType.DOUBLE, new int[] { 4, 9, 3, 4 });
			m.DataArray = data;
			fill(data);

			m = members.addMember("nested2", "desc", "units", DataType.STRUCTURE, new int[] { 7 });
			data = makeNested2(m);
			m.DataArray = data;

			return new ArrayStructureMA(members, new int[] { 4, 9 });
		}

		private ArrayStructure makeNested2(StructureMembers.Member parent)
		{
			StructureMembers members = new StructureMembers(parent.Name);
			parent.StructureMembers = members;

			StructureMembers.Member m = members.addMember("h1", "desc", "units", DataType.INT, new int[] { 1 });
			NcArray data = NcArray.factory(DataType.INT, new int[] { 4, 9, 7 });
			m.DataArray = data;
			fill(data);

			m = members.addMember("h2", "desc", "units", DataType.DOUBLE, new int[] { 2 });
			data = NcArray.factory(DataType.DOUBLE, new int[] { 4, 9, 7, 2 });
			m.DataArray = data;
			fill(data);

			return new ArrayStructureMA(members, new int[] { 4, 9, 7 });
		}

		private void fill(NcArray a)
		{
			IndexIterator ii = a.getIndexIterator();
			while (ii.hasNext())
			{
				_ = ii.IntNext;
				int[] counter = ii.CurrentCounter;
				int value = 0;
				for (int i = 0; i < counter.Length; i++)
				{
					value = value * 10 + counter[i];
				}
				ii.IntCurrent = value;
			}
		}
	}
}
