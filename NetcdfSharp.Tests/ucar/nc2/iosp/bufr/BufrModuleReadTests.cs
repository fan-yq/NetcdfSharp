// Ported from Java: netcdf-java bufr/src/test/java/ucar/nc2/iosp/bufr/TestBufrModuleRead.java
// The Java original scans the real bufr test-data directory; here the same sanity checks run
// against the generated mock file test_bufr.bufr (an eccodes BUFR4 template message).
// Referencing the Bufr types also forces the NetcdfSharp.Bufr assembly to load, so that
// NetcdfFiles.open can discover BufrIosp2 via the ServiceLoader.


using NetcdfSharp.ucar.nc2;
using NetcdfSharp.ucar.nc2.iosp.bufr;
using NetcdfSharp.ucar.tests;
using NetcdfSharp.ucar.unidata.io;
using Xunit;

namespace NetcdfSharp.ucar.nc2.iosp.bufr
{

	/// <summary>
	/// Sanity check on reading BUFR messages: MessageScanner + Message properties,
	/// and opening a BUFR file through NetcdfFiles.open.
	/// </summary>
	public class BufrModuleReadTests
	{
		private const string BufrFile = "test_bufr.bufr";

		private static string BufrPath()
		{
			Assert.True(MockDataSupport.HasData(BufrFile),
				"missing mock-datasets/" + BufrFile + "; run python generate_mock_data.py (requires eccodes)");
			return MockDataSupport.DataPath(BufrFile);
		}

		[Fact]
		public void ScanMockBufrMessage()
		{
			string fp = BufrPath();

			RandomAccessFile raf = new RandomAccessFile(fp, "r");
			try
			{
				MessageScanner scan = new MessageScanner(raf, 0, true);
				int count = 0;
				while (scan.hasNext())
				{
					Message m = scan.next();
					if (m == null)
					{
						continue;
					}
					count++;

					Assert.True(m.TablesComplete, "incomplete tables");
					Assert.True(m.MessageSize > 0);
					Assert.NotNull(m.Header);
					Assert.NotNull(m.Lookup);
					Assert.True(m.NumberDatasets >= 0);
					Assert.True(m.TotalBits >= 0);
				}
				Assert.True(count >= 1, "expected at least one BUFR message");
			}
			finally
			{
				raf.close();
			}
		}

		[Fact]
		public void OpenBufrFile()
		{
			string fp = BufrPath();

			// referencing the Bufr types forces the assembly load so the ServiceLoader
			// (and therefore NetcdfFiles.open) can discover BufrIosp2
			RandomAccessFile raf = new RandomAccessFile(fp, "r");
			try
			{
				BufrIosp2 iosp = new BufrIosp2();
				Assert.True(iosp.isValidFile(raf), "BufrIosp2 should recognize the mock BUFR file");
			}
			finally
			{
				raf.close();
			}

			using NetcdfFile ncf = NetcdfFiles.open(fp);
			Structure obs = Assert.IsAssignableFrom<Structure>(ncf.findVariable(BufrIosp2.obsRecordName));
			Assert.NotEmpty(ncf.Variables);
			Assert.True(obs.NumberOfMemberVariables > 0, "BUFR table descriptors were not expanded");
			Assert.NotNull(obs.findVariable("PRESSURE"));
			Assert.NotNull(obs.findVariable("TEMPERATURE_DRY-BULB_TEMPERATURE"));
		}
	}
}
