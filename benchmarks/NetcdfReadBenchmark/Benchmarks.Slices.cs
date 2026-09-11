// Slice-read benchmarks: 2-D and 3-D fixed slices of variable "t".


using BenchmarkDotNet.Attributes;
using Microsoft.Research.Science.Data;
using SwatRecords.NetCDF;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;

namespace bench
{
	public class Netcdf3SliceBenchmarks : NetcdfReadBenchmarksBase
	{
		// t[0,0,0,:,:] -> [225,309] int16 (~139 KB)


		[Benchmark]
		public void NetcdfSharp_Read2D()
		{
			using var nc = NetcdfFiles.open(Fp);
			var s = new Section("0,0,0,:,:").Ranges;
			var a = nc.findVariable("t").read(s);
			sink = a.getShort(0);
		}

		[Benchmark]
		public void SdsLite_Read2D()
		{
			using var ds = DataSet.Open("msds:nc?file=" + Fp);
			var v = ds.Variables["t"];
			var arr = (short[,,,,])v.GetData(
				new int[] { 0, 0, 0, 0, 0 },
				new int[] { 1, 1, 1, 225, 309 });
			sink = arr[0, 0, 0, 0, 0];
		}

		[Benchmark]
		public void Swat_Read2D()
		{
			using var file = NetCdfFile.Open(Fp);
			var data = file.Variables["t"].Read<short>(
				new long[] { 0, 0, 0, 0, 0 },
				new long[] { 1, 1, 1, 225, 309 });
			sink = data[0];
		}

		// t[0,0,0:2,:,:] -> [2,225,309] int16 (~278 KB)


		[Benchmark]
		public void NetcdfSharp_Read3D()
		{
			using var nc = NetcdfFiles.open(Fp);
			var s = new Section("0,0,0:2,:,:").Ranges;
			var a = nc.findVariable("t").read(s);
			sink = a.getShort(0);
		}

		[Benchmark]
		public void SdsLite_Read3D()
		{
			using var ds = DataSet.Open("msds:nc?file=" + Fp);
			var v = ds.Variables["t"];
			var arr = (short[,,,,])v.GetData(
				new int[] { 0, 0, 0, 0, 0 },
				new int[] { 1, 1, 3, 225, 309 });
			sink = arr[0, 0, 0, 0, 0];
		}

		[Benchmark]
		public void Swat_Read3D()
		{
			using var file = NetCdfFile.Open(Fp);
			var data = file.Variables["t"].Read<short>(
				new long[] { 0, 0, 0, 0, 0 },
				new long[] { 1, 1, 3, 225, 309 });
			sink = data[0];
		}
	}
}
