// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestReadSection.java,
// TestReadSlice.java, TestReadStrides.java and TestReadVariableData.java.
// The Java originals read testWrite.nc / time0.nc (testdata files not in this repo); here the
// same structural assertions are run against the generated mock file test_netcdf3.nc
// (variable "temperature" has shape [10,5,8]). Data values are compared in logical order via
// copyTo1DJavaArray(), exactly as the Java originals do with Assert2.assertArrayNearlyEquals.


using System;
using System.IO;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of the nc2 read-path tests: Variable.read()/section/slice/strides against mock data.
	/// </summary>
	public class Nc2ReadVariableDataTests
	{
		private static string MockPath(string name)
		{
			string dir = AppContext.BaseDirectory;
			while (dir != null)
			{
				string candidate = Path.Combine(dir, "mock-datasets");
				if (Directory.Exists(candidate))
				{
					return Path.Combine(candidate, name);
				}
				dir = Path.GetDirectoryName(dir);
			}
			return null;
		}

		private static NetcdfFile TryOpen(string name)
		{
			string p = MockPath(name);
			return (p != null && File.Exists(p)) ? NetcdfFiles.open(p) : null;
		}

		// Element-wise comparison in logical order (matches Java Assert2.assertArrayNearlyEquals on
		// copyTo1DJavaArray results). Works for float and double backing arrays.
		private static void AssertArrayNearlyEquals(NcArray expected, NcArray actual)
		{
			Assert.Equal(expected.Size, actual.Size);
			System.Array e = (System.Array)expected.copyTo1DJavaArray();
			System.Array a = (System.Array)actual.copyTo1DJavaArray();
			for (long i = 0; i < expected.Size; i++)
			{
				double ev = Convert.ToDouble(e.GetValue(i));
				double av = Convert.ToDouble(a.GetValue(i));
				Assert.Equal(ev, av, 1e-6);
			}
		}

		[Fact]
		public void TestReadVariableSection()
		{
			using (NetcdfFile ncfile = TryOpen("test_netcdf3.nc"))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py
				}
				Variable temp = ncfile.findVariable("temperature"); // shape [10,5,8]

				int[] origin = { 3, 1, 2 };
				int[] shape = { 4, 3, 2 };

				Variable tempSection = temp.section(new Section(origin, shape));
				NcArray Asection = tempSection.read();
				Assert.Equal(3, Asection.Rank);
				Assert.Equal(shape[0], Asection.Shape[0]);
				Assert.Equal(shape[1], Asection.Shape[1]);
				Assert.Equal(shape[2], Asection.Shape[2]);

				// read entire array and compare against the section
				NcArray A = temp.read();
				Assert.Equal(3, A.Rank);

				NcArray Asection2 = A.section(origin, shape, null);
				Assert.Equal(3, Asection2.Rank);
				Assert.Equal(shape[0], Asection2.Shape[0]);
				Assert.Equal(shape[1], Asection2.Shape[1]);
				Assert.Equal(shape[2], Asection2.Shape[2]);

				AssertArrayNearlyEquals(Asection2, Asection);
			}
		}

		[Fact]
		public void TestReadVariableSection2()
		{
			using (NetcdfFile ncfile = TryOpen("test_netcdf3.nc"))
			{
				if (ncfile == null)
				{
					return;
				}
				Variable temp = ncfile.findVariable("temperature"); // shape [10,5,8]

				NcRange r0 = new NcRange(2, 5);
				NcRange r1 = new NcRange(1, 3);
				NcRange r2 = new NcRange(0, 3);
				System.Collections.Generic.List<NcRange> ranges = new System.Collections.Generic.List<NcRange> { r0, r1, r2 };

				Variable tempSection = temp.section(ranges);
				Assert.Equal(3, tempSection.Rank);
				Assert.Equal(r0.length(), tempSection.Shape[0]);
				Assert.Equal(r1.length(), tempSection.Shape[1]);
				Assert.Equal(r2.length(), tempSection.Shape[2]);

				NcArray Asection = tempSection.read();
				Assert.Equal(3, Asection.Rank);
				Assert.Equal(r0.length(), Asection.Shape[0]);
				Assert.Equal(r1.length(), Asection.Shape[1]);
				Assert.Equal(r2.length(), Asection.Shape[2]);

				NcArray A = temp.read();
				Assert.Equal(3, A.Rank);

				NcArray Asection2 = A.section(ranges);
				Assert.Equal(3, Asection2.Rank);

				AssertArrayNearlyEquals(Asection2, Asection);
			}
		}

		[Fact]
		public void TestReadSlice()
		{
			using (NetcdfFile ncfile = TryOpen("test_netcdf3.nc"))
			{
				if (ncfile == null)
				{
					return;
				}
				Variable temp = ncfile.findVariable("temperature"); // shape [10,5,8]

				// slice dimension 0 at index 2 -> shape [5,8]
				Variable tempSlice = temp.slice(0, 2);
				NcArray Asection = tempSlice.read();
				Assert.Equal(2, Asection.Rank);
				Assert.Equal(5, Asection.Shape[0]);
				Assert.Equal(8, Asection.Shape[1]);

				// read whole array then slice it the same way
				NcArray A = temp.read();
				Assert.Equal(3, A.Rank);
				NcArray Asection2 = A.slice(0, 2);
				Assert.Equal(2, Asection2.Rank);

				AssertArrayNearlyEquals(Asection2, Asection);
			}
		}

		[Fact]
		public void TestReadSliceCompose()
		{
			using (NetcdfFile ncfile = TryOpen("test_netcdf3.nc"))
			{
				if (ncfile == null)
				{
					return;
				}
				Variable temp = ncfile.findVariable("temperature"); // shape [10,5,8]

				Variable tempSlice = temp.slice(0, 2);   // [5,8]
				Variable slice2 = tempSlice.slice(0, 3);  // [8], rank 1
				Assert.Equal(1, slice2.Rank);

				NcArray Asection = slice2.read();
				Assert.Equal(1, Asection.Rank);
				Assert.Equal(8, Asection.Shape[0]);

				NcArray A = temp.read();
				NcArray data = A.slice(0, 2);
				data = data.slice(0, 3);
				Assert.Equal(1, data.Rank);

				AssertArrayNearlyEquals(data, Asection);
			}
		}

		[Fact]
		public void TestReadStrides()
		{
			using (NetcdfFile ncfile = TryOpen("test_netcdf3.nc"))
			{
				if (ncfile == null)
				{
					return;
				}
				Variable temp = ncfile.findVariable("temperature"); // [10,5,8]

				// strided reads are self-consistent against whole-array sectioning
				NcArray A = temp.read("0:2,0:2,0:2");
				Assert.Equal(3, A.Rank);
				Assert.Equal(3, A.Shape[0]);
				Assert.Equal(3, A.Shape[1]);
				Assert.Equal(3, A.Shape[2]);

				NcArray whole = temp.read();
				NcArray section = whole.section(new Section("0:2,0:2,0:2").Ranges);
				AssertArrayNearlyEquals(section, A);

				// with strides
				NcArray As = temp.read("0:2:2,0:2:2,0:2:2");
				Assert.Equal(3, As.Rank);
				Assert.Equal(2, As.Shape[0]);
				Assert.Equal(2, As.Shape[1]);
				Assert.Equal(2, As.Shape[2]);

				NcArray sectionS = whole.section(new Section("0:2:2,0:2:2,0:2:2").Ranges);
				AssertArrayNearlyEquals(sectionS, As);
			}
		}

		[Fact]
		public void TestReadPartAndReduce()
		{
			using (NetcdfFile ncfile = TryOpen("test_netcdf3.nc"))
			{
				if (ncfile == null)
				{
					return;
				}
				Variable temp = ncfile.findVariable("temperature"); // [10,5,8]

				// read a sub-array of the 3-D array
				int[] origin2 = new int[3];
				int[] shape2 = new int[3];
				origin2[0] = 1;
				origin2[1] = 2;
				origin2[2] = 3;
				shape2[0] = 3;
				shape2[1] = 2;
				shape2[2] = 2;

				NcArray A = temp.read(origin2, shape2);
				Assert.Equal(3, A.Rank);
				Assert.Equal(shape2[0], A.Shape[0]);
				Assert.Equal(shape2[1], A.Shape[1]);
				Assert.Equal(shape2[2], A.Shape[2]);

				// compare against whole-array sectioning
				NcArray whole = temp.read();
				NcArray section2 = whole.section(origin2, shape2, null);
				AssertArrayNearlyEquals(section2, A);
			}
		}
	}
}
