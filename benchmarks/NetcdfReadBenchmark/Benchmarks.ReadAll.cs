// Full-read benchmark: entire variable "t" [107,2,6,225,309] int16 (~178 MB).


using BenchmarkDotNet.Attributes;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;
using SwatRecords.NetCDF;
using NetcdfSharp.ucar.nc2;

namespace bench
{
	public class Netcdf3ReadAllBenchmarks : NetcdfReadBenchmarksBase
	{
		[Benchmark]
		public void NetcdfSharp_ReadAll()
		{
			using var nc = NetcdfFiles.open(Fp);
			var a = nc.findVariable("t").read();
			var data = (short[,,,,])a.copyToCSharpArray();
			sink = data[0, 0, 0, 0, 0];
		}

		[Benchmark]
		public void SdsLite_ReadAll()
		{
			using var ds = DataSet.Open("msds:nc?file=" + Fp);
			var data = ds.GetData<short[,,,,]>("t");
			sink = data[0, 0, 0, 0, 0];
		}

		[Benchmark]
		public void Swat_ReadAll()
		{
			using var file = NetCdfFile.Open(Fp);
			var data = file.Variables["t"].Read<short>();
			sink = data[0];
		}
	}
}
