// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/grib2/table/TestWmoTemplateTables.java


using NetcdfSharp.ucar.nc2.grib.grib2.table;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.grib2.table.TestWmoTemplateTables.
	/// </summary>
	public class WmoTemplateTablesTests
	{
		[Fact]
		public void TestWmoTableConsistency()
		{
			WmoTemplateTables tables = WmoTemplateTables.Instance;
			foreach (WmoTemplateTables.TemplateTable table in tables.TemplateTables)
			{
				foreach (WmoTemplateTables.Field fld in table.Flds)
				{
					Assert.True(fld.Content != null);
				}
			}
		}
	}
}
