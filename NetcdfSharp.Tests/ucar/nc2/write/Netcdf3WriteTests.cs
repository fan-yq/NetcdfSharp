// Round-trip write tests for netCDF-3 via NetcdfFormatWriter + N3iospWriter/N3headerWriter.
// Ported scenarios from Java: cdm-test/.../ucar/nc2/write/TestNetcdfFormatWriter.java and TestWrite.java


using System;
using System.IO;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using NetcdfSharp.ucar.nc2.write;
using Xunit;

namespace NetcdfSharp.ucar.nc2.write
{

	/// <summary>
	/// Write a netCDF-3 file with NetcdfFormatWriter and read it back with NetcdfFiles.open.
	/// </summary>
	public class Netcdf3WriteTests
	{
		private static string TempPath(string name)
		{
			string dir = Path.Combine(Path.GetTempPath(), "NetcdfSharpWriteTests");
			Directory.CreateDirectory(dir);
			return Path.Combine(dir, name + "_" + Guid.NewGuid().ToString("N") + ".nc");
		}

		[Fact]
		public void WriteFixedDouble2D_RoundTrip()
		{
			string filename = TempPath("fixed2d");
			try
			{
				int nlat = 64, nlon = 128;
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				Dimension latDim = writerb.addDimension("lat", nlat);
				Dimension lonDim = writerb.addDimension("lon", nlon);
				writerb.addVariable("temperature", DataType.DOUBLE, "lat lon").addAttribute(new NcAttribute("units", "K"));

				using (NetcdfFormatWriter writer = writerb.build())
				{
					ArrayDouble.D2 A = new ArrayDouble.D2(nlat, nlon);
					for (int i = 0; i < nlat; i++)
					{
						for (int j = 0; j < nlon; j++)
						{
							A.set(i, j, i * 1000000 + j * 1000);
						}
					}
					writer.write("temperature", new int[2], A);
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable temp = ncfile.findVariable("temperature");
					Assert.NotNull(temp);
					Assert.Equal("K", temp.findAttributeString("units", null));
					Assert.Equal(new int[] { nlat, nlon }, temp.Shape);

					NcArray tA = temp.read();
					Assert.Equal(2, tA.Rank);
					for (int i = 0; i < nlat; i++)
					{
						for (int j = 0; j < nlon; j++)
						{
							Assert.Equal(i * 1000000 + j * 1000, tA.getValue<double>(i, j));
						}
					}
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void WriteUnlimited_RoundTrip()
		{
			string filename = TempPath("unlimited");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				writerb.addUnlimitedDimension("time");
				writerb.addAttribute(new NcAttribute("name", "value"));
				writerb.addVariable("time", DataType.DOUBLE, "time");

				using (NetcdfFormatWriter writer = writerb.build())
				{
					NcArray data = NcArray.makeFromJavaArray(new double[] { 0, 1, 2, 3 });
					writer.write("time", data);

					Variable time = writer.findVariable("time");
					Assert.Equal(4, time.Size);
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable vv = ncfile.findVariable("time");
					Assert.Equal(4, vv.Size);
					Assert.Equal("value", ncfile.RootGroup.findAttributeString("name", null));

					NcArray data = vv.read();
					Assert.Equal(4, data.Size);
					Assert.Equal(0.0, data.getValue<double>(0));
					Assert.Equal(1.0, data.getValue<double>(1));
					Assert.Equal(2.0, data.getValue<double>(2));
					Assert.Equal(3.0, data.getValue<double>(3));
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void OpenExisting_AppendUnlimited()
		{
			string filename = TempPath("append");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename).setFill(false);
				writerb.addUnlimitedDimension("time");
				writerb.addVariable("time", DataType.INT, "time");

				using (NetcdfFormatWriter writer = writerb.build())
				{
					NcArray data = NcArray.makeFromJavaArray(new int[] { 0, 1, 2, 3 });
					writer.write("time", data);
				}

				using (NetcdfFormatWriter writer = NetcdfFormatWriter.openExisting(filename).setFill(false).build())
				{
					Variable time = writer.findVariable("time");
					Assert.Equal(4, time.Size);

					NcArray data = NcArray.makeFromJavaArray(new int[] { 4, 5, 6 });
					int[] origin = new int[] { (int)time.Size };
					writer.write("time", origin, data);
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable vv = ncfile.findVariable("time");
					Assert.Equal(7, vv.Size);
					NcArray data = vv.read();
					Assert.Equal(7, data.Size);
					Assert.Equal(6, data.getValue<int>(6));
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void WriteCharVariable_RoundTrip()
		{
			string filename = TempPath("char");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				Dimension svar_len = writerb.addDimension("svar_len", 80);
				writerb.addVariable("svar", DataType.CHAR, "svar_len");

				using (NetcdfFormatWriter writer = writerb.build())
				{
					ArrayChar.D1 ac = new ArrayChar.D1(svar_len.Length);
					string val = "Testing 1-2-3";
					for (int j = 0; j < val.Length; j++)
					{
						ac.set(j, val[j]);
					}
					writer.write("svar", new int[1], ac);
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable c = ncfile.findVariable("svar");
					Assert.NotNull(c);
					NcArray tA = c.read();
					Assert.IsAssignableFrom<ArrayChar>(tA);
					string sval = ((ArrayChar)tA).getString(tA.Index);
					Assert.Equal("Testing 1-2-3", sval);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void WriteScalar_RoundTrip()
		{
			string filename = TempPath("scalar");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				writerb.addVariable("scalar", DataType.DOUBLE, new System.Collections.Generic.List<Dimension>());

				using (NetcdfFormatWriter writer = writerb.build())
				{
					ArrayDouble.D0 datas = new ArrayDouble.D0();
					datas.set(222.333);
					writer.write("scalar", datas);
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable vv = ncfile.findVariable("scalar");
					Assert.NotNull(vv);
					Assert.Equal(0, vv.Rank);
					NcArray data = vv.read();
					Assert.Equal(222.333, data.getValue<double>());
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void WriteGlobalAttributes_RoundTrip()
		{
			string filename = TempPath("atts");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				writerb.addAttribute(new NcAttribute("yo", "face"));
				writerb.addAttribute(new NcAttribute("versionD", 1.2));
				writerb.addAttribute(new NcAttribute("versionI", 1));
				writerb.addAttribute(new NcAttribute("versionS", (short)2));

				using (NetcdfFormatWriter writer = writerb.build())
				{
					// nothing to write
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Assert.Equal("face", ncfile.RootGroup.findAttributeString("yo", null));
					Assert.Equal(1.2, ncfile.RootGroup.findAttribute("versionD").getValue<double>());
					Assert.Equal(1, ncfile.RootGroup.findAttribute("versionI").getValue<int>());
					Assert.Equal(2, ncfile.RootGroup.findAttribute("versionS").getValue<int>());
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void WriteByte_RoundTrip()
		{
			string filename = TempPath("byte");
			try
			{
				int nlat = 64;
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				Dimension latDim = writerb.addDimension("lat", nlat);
				writerb.addVariable("bvar", DataType.BYTE, "lat");

				using (NetcdfFormatWriter writer = writerb.build())
				{
					ArrayByte.D1 barray = new ArrayByte.D1(nlat, false);
					int start = -nlat / 2;
					for (int j = 0; j < nlat; j++)
					{
						barray.set(j, (byte)(start + j));
					}
					writer.write("bvar", barray);
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable bv = ncfile.findVariable("bvar");
					Assert.NotNull(bv);
					NcArray data = bv.read();
					int start = -nlat / 2;
					for (int j = 0; j < nlat; j++)
					{
						// byte is unsigned in this port (Plan A): value read back is (start+j) & 0xFF
						Assert.Equal((byte)(start + j), data.getValue<byte>(j));
					}
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}
	}
}
