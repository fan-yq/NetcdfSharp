// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using TableType = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.TableType;
	using WmoTable = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.WmoTable;
	using WmoEntry = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.WmoTable.WmoEntry;
	using Entry = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2FlagTableInterface.Entry;

	public class WmoFlagTable : Grib2FlagTableInterface
	{
	  private readonly WmoTable wmoTable;
	  private readonly ImmutableListMultimap<int, WmoTable.WmoEntry> multimap;

	  internal WmoFlagTable(WmoTable wmoTable)
	  {
		Preconditions.checkNotNull(wmoTable);
		Preconditions.checkArgument(wmoTable.Type == TableType.flag);
		this.wmoTable = wmoTable;
		ImmutableListMultimap<int, WmoTable.WmoEntry>.Builder builder = ImmutableListMultimap<int, WmoTable.WmoEntry>.builder();
		foreach (WmoTable.WmoEntry wmoEntry in wmoTable.Entries)
		{
		  builder.put(wmoEntry.Number, wmoEntry);
		}
		this.multimap = builder.build();
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
			return wmoTable.Name;
		  }
	  }

	  public virtual ImmutableList<Grib2FlagTableInterface.Entry> Entries
	  {
		  get
		  {
			ImmutableList<Grib2FlagTableInterface.Entry>.Builder builder = ImmutableList<Grib2FlagTableInterface.Entry>.builder();
			foreach (KeyValuePair<int, ICollection<WmoTable.WmoEntry>> entry in multimap.asMap().entrySet())
			{
			  builder.add(new WmoFlagEntry(entry.Key, entry.Value));
			}
			return builder.build();
		  }
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable @Override public Entry getEntry(int code)
	  public virtual Grib2FlagTableInterface.Entry getEntry(int code)
	  {
		ImmutableList<WmoTable.WmoEntry> entries = multimap.get(code);
		if (entries == null)
		{
		  return null;
		}
		return new WmoFlagEntry(code, entries);
	  }

	  private class WmoFlagEntry : Grib2FlagTableInterface.Entry
	  {
		internal readonly int code;
		internal readonly ICollection<WmoTable.WmoEntry> entries;

		public WmoFlagEntry(int code, ICollection<WmoTable.WmoEntry> entries)
		{
		  this.code = code;
		  this.entries = entries;
		}

		public virtual int Code
		{
			get
			{
			  return code;
			}
		}

		public virtual ImmutableList<int> Values
		{
			get
			{
	// JAVA TO C# CONVERTER TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
			  return ImmutableList<int>.copyOf(entries.Select(e => e.value));
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable @Override public String getName(int value)
		public virtual string getName(int value)
		{
// JAVA TO C# CONVERTER TASK: Method reference arbitrary object instance method syntax is not converted by Java to C# Converter:
		  return entries.Where(e => e.value == value).Select(e => e.name).FirstOrDefault();
		}
	  }

	}

}
