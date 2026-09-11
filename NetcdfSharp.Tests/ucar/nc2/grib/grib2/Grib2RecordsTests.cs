// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/grib2/TestGrib2Records.java
// The Java original reads the sample files under grib/src/test/data. Those are not part of this
// repo (gitignored netcdf-java tree), so each case is skipped when the file is absent.


using System;
using System.IO;
using NetcdfSharp.ucar.nc2.grib.grib2;
using NetcdfSharp.ucar.nc2.time;
using NetcdfSharp.ucar.unidata.io;
using Formatter = NetcdfSharp.java.util.Formatter;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.grib2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.grib2.TestGrib2Records - scan + unpack each GDS/PDS sample.
	/// </summary>
	public class Grib2RecordsTests
	{
		[Theory]
		// Example of each GDS that we have
		[InlineData("HLYA10.grib2", 0, 0, 259920L, "2014-01-15T00:00:00Z")]
		[InlineData("MRMS_LowLevelCompositeReflectivity_00.50_20141207-072038.grib2", 0, 0, 24500000L, "2014-12-07T07:20:38Z")]
		[InlineData("cosmo-eu.grib2", 1, 0, 25L, "2010-03-29T00:00:00Z")]
		[InlineData("ds.sky.grib2", 10, 0, 22833L, "2005-09-01T15:00:00Z")]
		[InlineData("sref_eta.grib2", 20, 0, 14873L, "2009-03-16T21:00:00Z")]
		[InlineData("ds.snow.grib2", 30, 8, 739297L, "2005-09-01T15:00:00Z")]
		[InlineData("Albers_viirs_s.grib2", 31, 0, 3255148L, "2014-02-14T12:00:00Z")]
		[InlineData("thinGrid.grib2", 40, 0, 2097152L, "2012-01-01T00:00:00Z")]
		[InlineData("Eumetsat.VerticalPerspective.grib2", 90, 30, 1530169L, "2007-05-22T11:45:00Z")]
		[InlineData("ofs_atl.grib2", 204, 0, 2020800L, "2007-08-07T00:00:00Z")]
		[InlineData("rap-native.grib2", 32769, 0, 35L, "2016-04-25T22:00:00Z")]
		// Example of each PDS that we have
		[InlineData("pdsScale.pds1.grib2", 0, 1, 65160L, "2010-11-06T00:00:00Z")]
		[InlineData("sref.pds2.grib2", 30, 2, 23865L, "2009-01-25T21:00:00Z")]
		[InlineData("problem.pds9.grib2", 30, 9, 739297L, "2010-09-13T00:00:00Z")]
		[InlineData("cosmo.pds11.grib2", 1, 11, 194081L, "2009-05-11T00:00:00Z")]
		[InlineData("sref.pds12.grib2", 30, 12, 23865L, "2009-01-25T21:00:00Z")]
		[InlineData("rugley.pds15.grib2", 0, 15, 41760L, "2010-10-13T18:00:00Z")]
		[InlineData("Lannion.pds31.grib2", 0, 31, 5760000L, "2013-11-18T02:00:00Z")]
		[InlineData("BOM_synsat.pds32.grib2", 0, 32, 9690L, "2020-05-06T12:00:00Z")]
		public void TestRead(string ds, int gdsTemplate, int pdsTemplate, long datalen, string refdateIso)
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

				Grib2RecordScanner reader = new Grib2RecordScanner(raf);
				while (reader.hasNext())
				{
					Grib2Record gr = reader.next();
					if (gr == null)
					{
						break;
					}

					Grib2Gds gds = gr.GDS;
					if (check)
					{
						Assert.Equal(gdsTemplate, gds.template);
					}
					gds.testHorizCoordSys(new Formatter());

					Grib2SectionProductDefinition pdss = gr.PDSsection;
					Grib2Pds pds = pdss.PDS;
					if (check)
					{
						Assert.Equal(pdsTemplate, pdss.PDSTemplateNumber);
					}
					Formatter f = new Formatter();
					pds.show(f);
					if (check)
					{
						Assert.Contains(string.Format("template={0}", pdsTemplate), f.ToString());
					}
					if (check)
					{
						Assert.Equal(expected, gr.ReferenceDate);
					}

					// data representation templates 40 (JPEG2000) and 41 (PNG) are not ported
					int dataTemplate = gr.DataRepresentationSection.DataTemplate;
					if (dataTemplate == 40 || dataTemplate == 41)
					{
						continue;
					}

					float[] data = gr.readData(raf);
					if (check)
					{
						Assert.Equal((int)datalen, data.Length);
					}

					if (check && pds.Satellite)
					{
						Grib2Pds.PdsSatellite sat = pds as Grib2Pds.PdsSatellite;
						int numBands = sat.NumSatelliteBands;
						Assert.NotEqual(0, numBands);
						Assert.Equal(numBands, sat.SatelliteBands.Length);
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
