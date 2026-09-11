// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestReadArrayStructure.java
// The Java test opened files from the dap4 test-data resources; here they are served from the
// mock dataset. The @Ignore'd vlen-extraction cases are not ported.


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using NetcdfSharp.ucar.nc2.dataset;
using NetcdfSharp.ucar.tests;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestReadArrayStructure - reading scalar structures and vlen members.
	/// </summary>
	public class Ma2ReadArrayStructureTests
	{
		[Fact]
		public void ShouldReadScalarStructure()
		{
			using (NetcdfFile ncfile = MockDataSupport.TryOpen("test_struct1.nc"))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py / copy testdata to create mock data
				}
				Structure structure = (Structure)ncfile.findVariable("s");
				Assert.NotNull(structure);

				Assert.Equal(2, structure.Variables.Count);
				Variable x = structure.Variables[0];
				Assert.Equal("x", x.getShortName());
				Variable y = structure.Variables[1];
				Assert.Equal("y", y.getShortName());

				NcArray xValues = x.read();
				Assert.Equal(1, xValues.Size);
				Assert.Equal(1, xValues.getInt(0));

				NcArray yValues = y.read();
				Assert.Equal(1, yValues.Size);
				Assert.Equal(-2, yValues.getInt(0));
			}
		}

		[Fact]
		public void ShouldGetArrayForVlenMember()
		{
			int[] expectedValues = new int[] { 1, 3, 5, 7 };
			using (NetcdfFile ncfile = MockDataSupport.TryOpen("test_vlen3.nc"))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py / copy testdata to create mock data
				}
				Structure structure = (Structure)ncfile.findVariable("v1");
				Assert.NotNull(structure);

				NcArray data = structure.read();
				Assert.IsAssignableFrom<ArrayStructure>(data);
				ArrayStructure arrayStructure = (ArrayStructure)data;
				NcArray values = arrayStructure.getStructureData(0).getArray("f1");
				checkValues(values, expectedValues);
			}
		}

		[Fact]
		public void ShouldGetArrayForVlenMember2()
		{
			using (NetcdfFile ncfile = MockDataSupport.TryOpen("test_vlen4.nc"))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py / copy testdata to create mock data
				}
				Structure structure = (Structure)ncfile.findVariable("v1");
				Assert.NotNull(structure);

				NcArray data = structure.read();
				Assert.IsAssignableFrom<ArrayStructure>(data);
				ArrayStructure arrayStructure = (ArrayStructure)data;
				System.Console.WriteLine("sd members=" + string.Join(",", arrayStructure.StructureMembers.MemberNames));
				StructureData sd0 = arrayStructure.getStructureData(0);
				System.Console.WriteLine("sd0 members=" + string.Join(",", sd0.Members));
				foreach (NetcdfSharp.ucar.ma2.StructureMembers.Member m in sd0.Members)
				{
					System.Console.WriteLine("  member " + m.Name + " dtype=" + m.DataType + " shape=[" + string.Join(",", m.Shape) + "]");
				}
				NcArray values = sd0.getArray("f1");
				Assert.Equal(2, values.Size);
				Assert.IsAssignableFrom<NcArray>(values.getObject(0));
				Assert.IsAssignableFrom<NcArray>(values.getObject(1));
			}
		}

		private static void checkValues(NcArray values, int[] expectedValues)
		{
			Assert.Equal(expectedValues.Length, values.Size);
			IndexIterator iter = values.getIndexIterator();
			int i = 0;
			while (iter.hasNext())
			{
				Assert.Equal(expectedValues[i++], iter.IntNext);
			}
		}
	}
}





