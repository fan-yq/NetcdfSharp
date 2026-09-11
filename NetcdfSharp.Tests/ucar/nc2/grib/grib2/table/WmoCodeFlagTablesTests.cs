// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/grib2/table/TestWmoCodeFlagTables.java


using NetcdfSharp.ucar.nc2.grib.grib2.table;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.grib2.table.TestWmoCodeFlagTables.
	/// </summary>
	public class WmoCodeFlagTablesTests
	{
		[Fact]
		public void TestWmoTableConsistency()
		{
			WmoCodeFlagTables tables = WmoCodeFlagTables.Instance;
			foreach (WmoCodeFlagTables.WmoTable table in tables.WmoTables)
			{
				if (table.Type == WmoCodeFlagTables.TableType.param)
				{
					WmoParamTable paramTable = new WmoParamTable(table);
					foreach (GribTables.Parameter entry in paramTable.Parameters)
					{
						Assert.True(paramTable.getParameter(entry.Number) != null);
					}
				}
				else if (table.Type == WmoCodeFlagTables.TableType.code)
				{
					WmoCodeTable codeTable = new WmoCodeTable(table);
					foreach (Grib2CodeTableInterface.Entry entry in codeTable.Entries)
					{
						Assert.True(codeTable.getEntry(entry.Code) != null);
					}
				}
				else if (table.Type == WmoCodeFlagTables.TableType.flag)
				{
					WmoFlagTable flagTable = new WmoFlagTable(table);
					foreach (Grib2FlagTableInterface.Entry entry in flagTable.Entries)
					{
						Assert.True(flagTable.getEntry(entry.Code) != null);
					}
				}
				else
				{
					Assert.Equal(WmoCodeFlagTables.TableType.cat, table.Type);
					Assert.StartsWith("4.1.", table.Id);
					WmoCodeTable codeTable = new WmoCodeTable(table);
					foreach (Grib2CodeTableInterface.Entry entry in codeTable.Entries)
					{
						Assert.True(codeTable.getEntry(entry.Code) != null);
					}
				}
			}
		}
	}
}
