// Open and metadata benchmarks for the three libraries.


using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.Research.Science.Data;
using NetcdfSharp.ucar.nc2;
using SwatRecords.NetCDF;

namespace bench
{
	public class Netcdf3OpenBenchmarks : NetcdfReadBenchmarksBase
	{
		// ------------------------------------------------------------------
		// Open
		// ------------------------------------------------------------------


		[Benchmark]
		public void NetcdfSharp_Open()
		{
			using var nc = NetcdfFiles.open(Fp);
		}

		[Benchmark]
		public void SdsLite_Open()
		{
			using var ds = DataSet.Open("msds:nc?file=" + Fp);
		}

		[Benchmark]
		public void Swat_Open()
		{
			using var file = NetCdfFile.Open(Fp);
		}

		// ------------------------------------------------------------------
		// Metadata: locate variable + read shape
		// ------------------------------------------------------------------


		[Benchmark]
		public void NetcdfSharp_Metadata()
		{
			using var nc = NetcdfFiles.open(Fp);
			NetcdfSharp.ucar.nc2.Variable v = nc.findVariable("t");
			int[] shape = v.Shape;
			sink = shape[0];
		}

		[Benchmark]
		public void SdsLite_Metadata()
		{
			using var ds = DataSet.Open("msds:nc?file=" + Fp);
			var v = ds.Variables["t"];
			int rank = v.Rank;
			long len = v.Dimensions[0].Length;
			sink = rank + len;
		}

		[Benchmark]
		public void Swat_Metadata()
		{
			using var file = NetCdfFile.Open(Fp);
			var v = file.Variables["t"];
			long[] shape = v.Shape.ToArray();
			sink = shape[0];
		}
	}
}
