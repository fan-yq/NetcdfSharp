// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/grib1/TestGrib1Records.java
// The Java original reads the sample files under grib/src/test/data. Those are not part of this
// repo (gitignored netcdf-java tree), so each case is skipped when the file is absent.


using System;
using System.IO;
using NetcdfSharp.ucar.nc2.grib.grib1;
using NetcdfSharp.ucar.nc2.grib.grib1.tables;
using NetcdfSharp.ucar.nc2.time;
using NetcdfSharp.ucar.unidata.io;
using Formatter = NetcdfSharp.java.util.Formatter;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.grib1
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.grib1.TestGrib1Records - scan + unpack each GDS/PDS sample.
	/// </summary>
	public class Grib1RecordsTests
	{
		[Theory]
		[InlineData("afwa.grib1", 0, 61, 16899L, "2005-05-04T00:00:00Z")]
		[InlineData("ECMWF.grib1", 0, 129, 1681L, "2006-12-25T12:00:00Z")]
		[InlineData("HPPI89_KWBC.grib1", 0, 2, 5329L, "2010-03-31T18:00:00Z")]
		[InlineData("thinGrid.grib1", 0, 11, 5329L, "2005-07-27T12:00:00Z")]
		[InlineData("airtmp_zht_000002_000000_1a0061x0061_2010011200_00240000_fcstfld.grib1", 1, 11, 3721L, "2010-01-12T00:00:00Z")]
		[InlineData("radar_national.grib1", 3, 201, 400000L, "2005-01-20T02:15:00Z")]
		[InlineData("jma.grib1", 3, 2, 83525L, "2006-08-14T00:00:00Z")]
		[InlineData("complex_packing.grib1", 4, 122, 131072L, "2015-11-09T00:00:00Z")]
		[InlineData("complex_packing2.grib1", 4, 144, 131072L, "2015-11-09T00:00:00Z")]
		[InlineData("D2.2006091400.F012.002M.CLWMR.grib1", 5, 153, 102960L, "2006-09-14T00:00:00Z")]
		[InlineData("noaaRFC-QPE.grib1", 5, 237, 157500L, "2010-10-05T00:00:00Z")]
		[InlineData("rotatedlatlon.grib1", 10, 7, 176904L, "2003-12-15T18:00:00Z")]
		[InlineData("ncepPredefinedGds.grib1", 21000, 100, 1332L, "2004-11-19T00:00:00Z")]
		public void TestRead(string ds, int gdsTemplate, int param, long datalen, string refdateIso)
		{
			string dir = DataDir();
			if (dir == null)
			{
				return; // netcdf-java sample data not present
			}
			string path = Path.Combine(dir, ds);
			if (!File.Exists(path))
			{
				return;
			}

			bool check = gdsTemplate >= 0;
			CalendarDate expected = CalendarDate.parseISOformat("ISO8601", refdateIso);

			RandomAccessFile raf = new RandomAccessFile(path, "r");
			try
			{
				raf.order(RandomAccessFile.BIG_ENDIAN);
				raf.seek(0);

				Grib1RecordScanner reader = new Grib1RecordScanner(raf);
				while (reader.hasNext())
				{
					Grib1Record gr = reader.next();
					if (gr == null)
					{
						break;
					}

					Grib1Gds gds = gr.GDS;
					if (check)
					{
						Assert.Equal(gdsTemplate, gds.template);
					}
					if (check)
					{
						Assert.Contains("template=" + gdsTemplate, gds.ToString());
					}
					gds.testHorizCoordSys(new Formatter());

					Grib1SectionProductDefinition pds = gr.PDSsection;
					if (check)
					{
						Assert.Equal(param, pds.ParameterNumber);
					}
					Formatter f = new Formatter();
					pds.showPds(Grib1Customizer.factory(gr, null), f);
					if (check)
					{
						Assert.True(f.ToString().Contains(string.Format("Parameter Name : ({0})", param)) || f.ToString().Contains(string.Format("Parameter {0} not found", param)));
					}
					if (check)
					{
						Assert.Equal(expected, pds.ReferenceDate);
					}

					float[] data = gr.readData(raf);
					if (check)
					{
						Assert.Equal((int)datalen, data.Length);
					}
				}
			}
			finally
			{
				raf.close();
			}
		}

		private static string DataDir()
		{
			string dir = AppContext.BaseDirectory;
			while (dir != null)
			{
				string candidate = Path.Combine(dir, "netcdf-java-maint-5.x", "grib", "src", "test", "data");
				if (Directory.Exists(candidate))
				{
					return candidate;
				}
				dir = Path.GetDirectoryName(dir);
			}
			return null;
		}
	}
}
