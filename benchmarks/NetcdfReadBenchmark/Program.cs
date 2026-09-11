// BenchmarkDotNet comparison of NetcdfSharp vs SDSLite vs SwatRecords.NetCDF
// on a local netCDF-3 64-bit-offset dataset.
//
//   Dataset:  C:\Users\fyq\Documents\data\archive\monthlydata\china-monthly-2007-2023.nc
//   Variable: t,u,v,w  int16 [107,2,6,225,309]
//
// Run (from benchmarks/NetcdfReadBenchmark):
//   dotnet run -c Release                          # full
//   dotnet run -c Release -- --job short           # fewer iterations (CI-friendly)
//   dotnet run -c Release -- --filter *ReadAll*    # subset by name
//   dotnet run -c Release -- --inprocess           # avoid antivirus/process-spawn noise
//
// Correctness cross-check: dotnet run -c Release --correctness


using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace bench
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (Array.IndexOf(args, "--correctness") >= 0)
			{
				Correctness.Run();
				return;
			}
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
		}
	}

	/// <summary>
	/// Common setup shared by all benchmark groups: locates the dataset.
	/// The <see cref="sink"/> field keeps reads alive so the JIT cannot elide them.
	/// </summary>
	[MemoryDiagnoser]
	public abstract class NetcdfReadBenchmarksBase
	{
		internal string Fp;
		internal static long sink;

		[GlobalSetup]
		public void Setup()
		{
			Fp = @"C:\Users\fyq\Documents\data\archive\monthlydata\china-monthly-2007-2023.nc";
			if (!File.Exists(Fp))
			{
				throw new InvalidOperationException("dataset not found: " + Fp);
			}
		}
	}
}
