// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/TestStructure.java
// The Java test opened testWriteRecord.nc directly; here it is served from the mock dataset.
// The readBothWays case just opens/closes the file and is omitted.


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;
using NetcdfSharp.ucar.nc2.iosp.netcdf3;
using NetcdfSharp.ucar.tests;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.TestStructure - reading record-structure data.
	/// </summary>
	public class Nc2StructureTests
	{
		[Fact]
		public void TestNames()
		{
			using (NetcdfFile ncfile = MockDataSupport.TryOpen("testWriteRecord.nc", NetcdfFile.IOSP_MESSAGE_ADD_RECORD_STRUCTURE))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py / copy testdata to create mock data
				}
				System.Collections.Generic.IList<Variable> vars = ncfile.Variables;
				string[] trueNames = new string[] { "rh", "T", "lat", "lon", "time", "recordvarTest", "record" };
				Assert.Equal(trueNames.Length, vars.Count);
				for (int i = 0; i < vars.Count; i++)
				{
					Assert.Equal(trueNames[i], vars[i].FullName);
				}

				Structure record = (Structure)ncfile.findVariable("record");
				Assert.NotNull(record);

				NetcdfSharp.com.google.common.collect.ImmutableList<Variable> recVars = record.Variables;
				string[] trueRecordNames = new string[] { "record.rh", "record.T", "record.time", "record.recordvarTest" };
				Assert.Equal(trueRecordNames.Length, recVars.Count);
				for (int i = 0; i < recVars.Count; i++)
				{
					Assert.Equal(trueRecordNames[i], recVars[i].FullName);
				}

				Variable time = ncfile.findVariable("record.time");
				Assert.NotNull(time);

				Variable time2 = record.findVariable("time");
				Assert.NotNull(time2);

				Assert.Equal(time, time2);
			}
		}

		[Fact]
		public void TestReadStructureCountBytesRead()
		{
			using (NetcdfFile ncfile = MockDataSupport.TryOpen("testWriteRecord.nc", NetcdfFile.IOSP_MESSAGE_ADD_RECORD_STRUCTURE))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py / copy testdata to create mock data
				}
				Structure record = (Structure)ncfile.findVariable("record");
				Assert.NotNull(record);

				// read all at once
				long totalAll = 0;
				NcArray dataAll = record.read();
				IndexIterator iter = dataAll.getIndexIterator();
				while (iter.hasNext())
				{
					StructureData sd = (StructureData)iter.next();

					System.Collections.IEnumerator viter = sd.Members.GetEnumerator();
					while (viter.MoveNext())
					{
						StructureMembers.Member m = (StructureMembers.Member)viter.Current;
						NcArray data = sd.getArray(m);
						totalAll += data.Size * m.DataType.getSize();
					}
				}
				Assert.Equal(304, totalAll);

				// read one at a time
				int numrecs = record.Shape[0];
				long totalOne = 0;
				for (int i = 0; i < numrecs; i++)
				{
					StructureData sd = record.readStructure(i);

					System.Collections.IEnumerator viter = sd.Members.GetEnumerator();
					while (viter.MoveNext())
					{
						StructureMembers.Member m = (StructureMembers.Member)viter.Current;
						NcArray data = sd.getArray(m);
						totalOne += data.Size * m.DataType.getSize();
					}
				}
				Assert.Equal(totalAll, totalOne);

				// read with the iterator
				long totalIter = 0;
				using (StructureDataIterator iter2 = record.StructureIterator)
				{
					while (iter2.hasNext())
					{
						StructureData sd = iter2.next();
						foreach (StructureMembers.Member m in sd.Members)
						{
							NcArray data = sd.getArray(m);
							totalIter += data.Size * m.DataType.getSize();
						}
					}
				}
				Assert.Equal(totalOne, totalIter);
			}
		}

		[Fact]
		public void TestN3ReadStructureCheckValues()
		{
			using (NetcdfFile ncfile = MockDataSupport.TryOpen("testWriteRecord.nc", NetcdfFile.IOSP_MESSAGE_ADD_RECORD_STRUCTURE))
			{
				if (ncfile == null)
				{
					return; // run generate_mock_data.py / copy testdata to create mock data
				}
				Structure record = (Structure)ncfile.findVariable("record");
				Assert.NotNull(record);

				// read all at once
				int recnum = 0;
				NcArray dataAll = record.read();
				IndexIterator iter = dataAll.getIndexIterator();
				while (iter.hasNext())
				{
					StructureData s = (StructureData)iter.next();
					NcArray rh = s.getArray("rh");
					Assert.IsAssignableFrom<ArrayInt.D2>(rh);
					checkValues(rh, recnum); // check the values are right
					recnum++;
				}

				// read one at a time
				int numrecs = record.Shape[0];
				for (int i = 0; i < numrecs; i++)
				{
					StructureData s = record.readStructure(i);
					NcArray rh = s.getArray("rh");
					Assert.IsAssignableFrom<ArrayInt.D2>(rh);
					checkValues(rh, i); // check the values are right
				}

				// read using iterator
				recnum = 0;
				using (StructureDataIterator iter2 = record.StructureIterator)
				{
					while (iter2.hasNext())
					{
						StructureData s = iter2.next();
						NcArray rh = s.getArray("rh");
						Assert.IsAssignableFrom<ArrayInt.D2>(rh);
						checkValues(rh, recnum); // check the values are right
						recnum++;
					}
				}
			}
		}

		private void checkValues(NcArray rh, int recnum)
		{
			// check the values are right
			ArrayInt.D2 rha = (ArrayInt.D2)rh;
			int[] shape = rha.Shape;
			for (int j = 0; j < shape[0]; j++)
			{
				for (int k = 0; k < shape[1]; k++)
				{
					int want = 20 * recnum + 4 * j + k + 1;
					int val = rha.get(j, k);
					Assert.Equal(want, val);
				}
			}
		}
	}
}


