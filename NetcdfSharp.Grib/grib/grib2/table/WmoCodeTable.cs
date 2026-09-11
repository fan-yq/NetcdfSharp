// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using TableType = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.TableType;
	using WmoTable = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.WmoTable;
	using WmoEntry = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.WmoTable.WmoEntry;
	using Entry = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2CodeTableInterface.Entry;

	public class WmoCodeTable : Grib2CodeTableInterface
	{
	  private readonly WmoTable wmoTable;
	  private readonly ImmutableMap<int, WmoCodeEntry> entryMap;

	  internal WmoCodeTable(WmoTable wmoTable)
	  {
		Preconditions.checkNotNull(wmoTable);
		Preconditions.checkArgument(wmoTable.Type == TableType.code || wmoTable.Type == TableType.cat);
		this.wmoTable = wmoTable;
		ImmutableMap<int, WmoCodeEntry>.Builder builder = ImmutableMap<int, WmoCodeEntry>.builder();
		foreach (WmoEntry e in wmoTable.Entries)
		{
		  builder.put(e.Number, new WmoCodeEntry(e));
		}
		this.entryMap = builder.build();
	  }

	  public virtual string Name
	  {
		  get
		  {
			return wmoTable.Name;
		  }
	  }

	  public virtual string ShortName
	  {
		  get
		  {
			return "WMO " + wmoTable.Id;
		  }
	  }

	  public virtual ImmutableList<Grib2CodeTableInterface.Entry> Entries
	  {
		  get
		  {
			return ImmutableList<Grib2CodeTableInterface.Entry>.copyOf(entryMap.Values);
		  }
	  }

	  public virtual Grib2CodeTableInterface.Entry getEntry(int codeValue)
	  {
		return entryMap.get(codeValue);
	  }

	  private class WmoCodeEntry : Grib2CodeTableInterface.Entry
	  {
		internal readonly WmoTable.WmoEntry entry;

		public WmoCodeEntry(WmoTable.WmoEntry entry)
		{
		  this.entry = entry;
		}

		public virtual int Code
		{
			get
			{
			  return entry.Number;
			}
		}

		public virtual string Name
		{
			get
			{
			  return entry.Name;
			}
		}
	  }
	}

}
