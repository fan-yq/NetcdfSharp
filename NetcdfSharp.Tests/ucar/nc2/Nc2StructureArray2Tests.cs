// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestStructureArray2.java
// The testMA case needs NetcdfSharp.ucar.nc2.dataset.StructurePseudoDS (dataset layer, not ported) and is omitted.


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using NetcdfSharp.ucar.tests;
using NetcdfSharp.ucar.unidata.util.test;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestStructureArray2 - run UtilsTestStructureArray over a record structure read from file.
	/// </summary>
	public class Nc2StructureArray2Tests
	{
		private readonly UtilsTestStructureArray test = new UtilsTestStructureArray();

		[Fact]
		public void TestBB()
		{
			using (NetcdfFile ncfile = MockDataSupport.TryOpen("testWriteRecord.nc", NetcdfFile.IOSP_MESSAGE_ADD_RECORD_STRUCTURE))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py / copy testdata to create mock data
				}
				Structure v = (Structure)ncfile.findVariable("record");
				Assert.NotNull(v);
				Assert.Equal(DataType.STRUCTURE, v.DataType);

				NcArray data = v.read();
				Assert.IsAssignableFrom<ArrayStructure>(data);
				Assert.IsAssignableFrom<ArrayStructureBB>(data);
				Assert.Equal(typeof(StructureData), data.ElementType);

				test.testArrayStructure((ArrayStructure)data);
			}
		}
	}
}

