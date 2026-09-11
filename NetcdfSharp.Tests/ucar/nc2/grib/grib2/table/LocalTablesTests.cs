// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/grib2/table/TestLocalTables.java


using System;
using NetcdfSharp.com.google.common.collect;
using NetcdfSharp.ucar.nc2.grib;
using NetcdfSharp.ucar.nc2.grib.grib2.table;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.grib2.table.TestLocalTables.
	/// </summary>
	public class LocalTablesTests
	{
		[Fact]
		public void TestLocalTables()
		{
			ImmutableList<Grib2Tables> tables = Grib2Tables.AllRegisteredTables;
			foreach (Grib2Tables t in tables)
			{
				foreach (GribTables.Parameter p in t.Parameters)
				{
					Assert.NotEmpty(p.Name);
					Assert.NotEmpty(p.Id);
				}
			}
		}

		[Fact]
		public void TestKmaTable()
		{
			Grib2Tables kma = Grib2Tables.factory(40, -1, -1, -1, -1);
			Assert.NotNull(kma);
			Assert.Equal(Grib2TablesId.Type.kma, kma.Type);
			Assert.NotEmpty(kma.Parameters);
		}

		[Fact]
		public void TestEcmwfCodeTables()
		{
			string[] tableOverrides = new string[] { "4.230", "4.233", "4.192", "5.40000", "5.50002" };

			Grib2Tables ecmwfTable = Grib2Tables.factory(98, -1, -1, -1, -1);
			Assert.NotNull(ecmwfTable);
			Assert.Equal(Grib2TablesId.Type.eccodes, ecmwfTable.Type);

			foreach (string tableName in tableOverrides)
			{
				string[] tokens = tableName.Split('.');
				int discipline = int.Parse(tokens[0]);
				int category = int.Parse(tokens[1]);

				EccodesCodeTable ecmwfCodeTable = EccodesCodeTable.factory(EccodesCodeTable.LATEST_VERSION, discipline, category);
				Assert.NotNull(ecmwfCodeTable);
				foreach (Grib2CodeTableInterface.Entry entry in ecmwfCodeTable.Entries)
				{
					Assert.Equal(entry.Name, ecmwfTable.getCodeTableValue(tableName, entry.Code));
				}
			}
		}
	}
}
