using System;
using System.IO;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;
using SwatRecords.NetCDF;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;

public class Correctness
{
	static int mismatches;

	static void Check(string what, short expected, short got)
	{
		if (expected != got)
		{
			mismatches++;
			if (mismatches <= 5) Console.WriteLine($"  MISMATCH {what}: expected={expected} got={got}");
		}
	}

	public static void Run()
	{
		string fp = @"C:\Users\fyq\Documents\data\archive\monthlydata\china-monthly-2007-2023.nc";
		if (!File.Exists(fp)) { Console.WriteLine("file not found"); return; }

		short[] ref2d, ref3d;
		int[] shape2d, shape3d;
		using (var nc = NetcdfFiles.open(fp))
		{
			var v = nc.findVariable("t");
			var a2 = v.read(new Section("0,0,0,:,:").Ranges);
			shape2d = a2.Shape;
			ref2d = new short[a2.Size];
			for (int i = 0; i < ref2d.Length; i++) ref2d[i] = a2.getShort(i);

			var a3 = v.read(new Section("0,0,0:2,:,:").Ranges);
			shape3d = a3.Shape;
			ref3d = new short[a3.Size];
			for (int i = 0; i < ref3d.Length; i++) ref3d[i] = a3.getShort(i);
		}
		Console.WriteLine($"reference shape2d=[{string.Join(",", shape2d)}]  shape3d=[{string.Join(",", shape3d)}]");

		Console.WriteLine("\n=== SDSLite ===");
		using (var ds = DataSet.Open("msds:nc?file=" + fp))
		{
			var v = ds.Variables["t"];
			var a2 = (short[,,,,])v.GetData(new int[] { 0, 0, 0, 0, 0 }, new int[] { 1, 1, 1, 225, 309 });
			int n2 = 0;
			for (int j = 0; j < 225; j++)
				for (int k = 0; k < 309; k++)
					Check($"sds 2d[{j},{k}]", ref2d[n2++], a2[0, 0, 0, j, k]);
			Console.WriteLine($"  2d values checked={n2}, mismatches={mismatches}");

			var a3 = (short[,,,,])v.GetData(new int[] { 0, 0, 0, 0, 0 }, new int[] { 1, 1, 3, 225, 309 });
			int n3 = 0;
			for (int l = 0; l < 3; l++)
				for (int j = 0; j < 225; j++)
					for (int k = 0; k < 309; k++)
						Check($"sds 3d[{l},{j},{k}]", ref3d[n3++], a3[0, 0, l, j, k]);
			Console.WriteLine($"  3d values checked={n3}, mismatches={mismatches}");
		}

		Console.WriteLine("\n=== SwatRecords ===");
		using (var file = NetCdfFile.Open(fp))
		{
			var v = file.Variables["t"];
			var d2 = v.Read<short>(new long[] { 0, 0, 0, 0, 0 }, new long[] { 1, 1, 1, 225, 309 });
			for (int i = 0; i < ref2d.Length; i++) Check($"swat 2d[{i}]", ref2d[i], d2[i]);
			Console.WriteLine($"  2d values checked={ref2d.Length}, mismatches={mismatches}");

			var d3 = v.Read<short>(new long[] { 0, 0, 0, 0, 0 }, new long[] { 1, 1, 3, 225, 309 });
			for (int i = 0; i < ref3d.Length; i++) Check($"swat 3d[{i}]", ref3d[i], d3[i]);
			Console.WriteLine($"  3d values checked={ref3d.Length}, mismatches={mismatches}");
		}

		Console.WriteLine($"\nTOTAL MISMATCHES: {mismatches}");
}
}
