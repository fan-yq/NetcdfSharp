// Round-trip write tests ported from Java:
//   - TestWriteFill.java      - writing with _FillValue, unwritten regions read back as fill
//   - TestWriteString.java    - writing CHAR string variables (incl. with an unlimited dimension)
//   - TestWrite.java (parts)  - multi-type variables, attribute arrays, scalar, signed byte


using System;
using System.IO;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using NetcdfSharp.ucar.nc2.iosp.netcdf3;
using NetcdfSharp.ucar.nc2.write;
using Xunit;

namespace NetcdfSharp.ucar.nc2.write
{

	/// <summary>
	/// netCDF-3 write round-trips: fill values, string variables, multi-type attributes and variables.
	/// Ported from the Java NetcdfSharp.ucar.nc2.write tests.
	/// </summary>
	public class Netcdf3WriteFillTests
	{
		private static string TempPath(string name)
		{
			string dir = Path.Combine(Path.GetTempPath(), "NetcdfSharpWriteTests");
			Directory.CreateDirectory(dir);
			return Path.Combine(dir, name + "_" + Guid.NewGuid().ToString("N") + ".nc");
		}

		[Fact]
		public void TestCreateWithFill()
		{
			string filename = TempPath("fill");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				writerb.addDimension(Dimension.builder().setName("time").setIsUnlimited(true).build());
				Dimension latDim = writerb.addDimension("lat", 6);
				Dimension lonDim = writerb.addDimension("lon", 12);

				writerb.addVariable("temperature", DataType.DOUBLE, "lat lon")
					.addAttribute(new NcAttribute("units", "K"))
					.addAttribute(new NcAttribute("_FillValue", -999.9));
				writerb.addVariable("lat", DataType.DOUBLE, "lat");
				writerb.addVariable("lon", DataType.FLOAT, "lon");
				writerb.addVariable("shorty", DataType.SHORT, "lat");
				writerb.addVariable("rtemperature", DataType.INT, "time lat lon")
					.addAttribute(new NcAttribute("units", "K"))
					.addAttribute(new NcAttribute("_FillValue", -9999));
				writerb.addVariable("rdefault", DataType.INT, "time lat lon");

				using (NetcdfFormatWriter writer = writerb.build())
				{
					// write only the first half of lon for rtemperature
					ArrayDouble.D3 A = new ArrayDouble.D3(1, latDim.Length, lonDim.Length / 2);
					for (int i = 0; i < latDim.Length; i++)
					{
						for (int j = 0; j < lonDim.Length / 2; j++)
						{
							A.set(0, i, j, i * 1000000 + j * 1000);
						}
					}
					writer.write("rtemperature", new int[3], A);
				}

				// reading back: temperature fully unwritten -> _FillValue
				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable temp = ncfile.findVariable("temperature");
					Assert.NotNull(temp);
					NcArray tA = temp.read();
					Assert.Equal(2, tA.Rank);
					for (long i = 0; i < tA.Size; i++)
					{
						Assert.Equal(-999.9, tA.getDouble((int)i));
					}

					// rtemperature: written half has data, unwritten half is -9999
					Variable rtemp = ncfile.findVariable("rtemperature");
					Assert.NotNull(rtemp);
					NcArray rA = rtemp.read();
					Assert.Equal(3, rA.Rank);
					int[] rshape = rA.Shape;
					for (int i = 0; i < rshape[1]; i++)
					{
						for (int j = rshape[2] / 2 + 1; j < rshape[2]; j++)
						{
							Assert.Equal(-9999.0, rA.getDouble(i * rshape[2] + j));
						}
					}

					// unwritten coordinate variables get default fill values
					Variable v = ncfile.findVariable("lat");
					Assert.Equal(DataType.DOUBLE, v.DataType);
					NcArray data = v.read();
					for (long i = 0; i < data.Size; i++)
					{
						Assert.Equal(N3iospFillValues.NC_FILL_DOUBLE, data.getDouble((int)i));
					}

					v = ncfile.findVariable("lon");
					data = v.read();
					for (long i = 0; i < data.Size; i++)
					{
						Assert.Equal(N3iospFillValues.NC_FILL_FLOAT, data.getFloat((int)i));
					}

					v = ncfile.findVariable("shorty");
					data = v.read();
					for (long i = 0; i < data.Size; i++)
					{
						Assert.Equal(N3iospFillValues.NC_FILL_SHORT, data.getShort((int)i));
					}

					v = ncfile.findVariable("rdefault");
					data = v.read();
					for (long i = 0; i < data.Size; i++)
					{
						Assert.Equal(N3iospFillValues.NC_FILL_INT, data.getInt((int)i));
					}
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void TestWriteStringWithUnlimited()
		{
			// from TestWriteString: create a time-lat-lon cube with an unlimited time dim, no data written
			string filename = TempPath("strcube");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				writerb.addDimension("lat", 2);
				writerb.addDimension("lon", 4);
				writerb.addUnlimitedDimension("time");

				writerb.addVariable("lat", DataType.FLOAT, "lat").addAttribute(new NcAttribute("units", "degrees_north"));
				writerb.addVariable("lon", DataType.FLOAT, "lon").addAttribute(new NcAttribute("units", "degrees_east"));
				writerb.addVariable("dataVar", DataType.FLOAT, "time lat lon").addAttribute(new NcAttribute("units", "units"));
				writerb.addVariable("cellId", DataType.CHAR, "lat lon").addAttribute(new NcAttribute("long_name", "Cell ID"));
				writerb.addVariable("time", DataType.INT, "time").addAttribute(new NcAttribute("standard_name", "time"));

				using (NetcdfFormatWriter writer = writerb.build())
				{
					// empty
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable dv = ncfile.findVariable("dataVar");
					Assert.NotNull(dv);
					Assert.Equal(new int[] { 0, 2, 4 }, dv.Shape); // unlimited time starts at 0
					Variable cellId = ncfile.findVariable("cellId");
					Assert.NotNull(cellId);
					Assert.Equal(DataType.CHAR, cellId.DataType);
					Variable t = ncfile.findVariable("time");
					Assert.Equal(DataType.INT, t.DataType);
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void TestWriteAttributeArray()
		{
			// from TestWrite: a variable attribute that is an array [1,2,3]
			string filename = TempPath("attarr");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				writerb.addDimension("lat", 3);
				writerb.addDimension("lon", 4);
				NcArray scale = NcArray.factory(DataType.INT, new int[] { 3 }, new int[] { 1, 2, 3 });
				writerb.addVariable("temperature", DataType.DOUBLE, "lat lon")
					.addAttribute(new NcAttribute("units", "K"))
					.addAttribute(new NcAttribute("scale", scale));

				using (NetcdfFormatWriter writer = writerb.build())
				{
					ArrayDouble.D2 A = new ArrayDouble.D2(3, 4);
					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 4; j++)
						{
							A.set(i, j, i * 1000000 + j * 1000);
						}
					}
					writer.write("temperature", new int[2], A);
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable temp = ncfile.findVariable("temperature");
					NcAttribute att = temp.findAttribute("scale");
					Assert.NotNull(att);
					Assert.True(att.Array);
					Assert.Equal(3, att.Length);
					Assert.Equal(1, att.getNumericValue(0));
					Assert.Equal(2, att.getNumericValue(1));
					Assert.Equal(3, att.getNumericValue(2));
				}
			}
			finally
			{
				File.Delete(filename);
			}
		}

		[Fact]
		public void TestWriteMultiTypeVariables()
		{
			// from TestWrite: write a double var, a char var and a signed byte var, then read back
			string filename = TempPath("multitype");
			try
			{
				NetcdfFormatWriter.Builder writerb = NetcdfFormatWriter.createNewNetcdf3(filename);
				Dimension latDim = writerb.addDimension("lat", 8);
				Dimension lonDim = writerb.addDimension("lon", 12);
				Dimension svar_len = writerb.addDimension("svar_len", 80);
				writerb.addVariable("temperature", DataType.DOUBLE, "lat lon").addAttribute(new NcAttribute("units", "K"));
				writerb.addVariable("svar", DataType.CHAR, "svar_len");
				writerb.addVariable("bvar", DataType.BYTE, "lat");

				using (NetcdfFormatWriter writer = writerb.build())
				{
					// double 2D
					ArrayDouble.D2 A = new ArrayDouble.D2(latDim.Length, lonDim.Length);
					for (int i = 0; i < latDim.Length; i++)
					{
						for (int j = 0; j < lonDim.Length; j++)
						{
							A.set(i, j, i * 1000000 + j * 1000);
						}
					}
					writer.write("temperature", new int[2], A);

					// char 1D string
					ArrayChar.D1 ac = new ArrayChar.D1(svar_len.Length);
					string val = "Two pairs of ladies stockings!";
					for (int j = 0; j < val.Length; j++)
					{
						ac.set(j, val[j]);
					}
					writer.write("svar", new int[1], ac);

					// signed byte
					ArrayByte.D1 barray = new ArrayByte.D1(latDim.Length, false);
					int start = -latDim.Length / 2;
					for (int j = 0; j < latDim.Length; j++)
					{
						barray.set(j, (byte)(start + j));
					}
					writer.write("bvar", barray);
				}

				using (NetcdfFile ncfile = NetcdfFiles.open(filename))
				{
					Variable temp = ncfile.findVariable("temperature");
					NcArray tA = temp.read();
					for (int i = 0; i < latDim.Length; i++)
					{
						for (int j = 0; j < lonDim.Length; j++)
						{
							Assert.Equal(i * 1000000 + j * 1000, tA.getDouble(i * lonDim.Length + j));
						}
					}

					Variable c = ncfile.findVariable("svar");
					NcArray cA = c.read();
					Assert.IsAssignableFrom<ArrayChar>(cA);
					Assert.Equal("Two pairs of ladies stockings!", ((ArrayChar)cA).getString(cA.Index));

					Variable bv = ncfile.findVariable("bvar");
					NcArray data = bv.read();
					int s2 = -latDim.Length / 2;
					for (int j = 0; j < latDim.Length; j++)
					{
						Assert.Equal((byte)(s2 + j), data.getValue<byte>(j));
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
