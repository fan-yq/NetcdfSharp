// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using TableType = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.TableType;
	using WmoTable = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.WmoTable;
	using WmoEntry = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoCodeFlagTables.WmoTable.WmoEntry;

	public class WmoParamTable : Grib2ParamTableInterface
	{

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static NetcdfSharp.ucar.nc2.grib.GribTables.Parameter getParameter(int discipline, int category, int number)
	  public static GribTables.Parameter getParameter(int discipline, int category, int number)
	  {
		// Look rather inefficient.
		WmoParamTable paramTable = WmoCodeFlagTables.Instance.getParamTable(discipline, category);
		return paramTable == null ? null : paramTable.getParameter(number);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static String getParameterName(int discipline, int category, int number)
	  public static string getParameterName(int discipline, int category, int number)
	  {
		// Look rather inefficient.
		WmoParamTable paramTable = WmoCodeFlagTables.Instance.getParamTable(discipline, category);
		GribTables.Parameter param = paramTable == null ? null : paramTable.getParameter(number);
		return (param == null) ? null : param.Name;
	  }

	  private readonly WmoTable wmoTable;
	  private readonly ImmutableMap<int, GribTables.Parameter> entryMap;

	  internal WmoParamTable(WmoTable wmoTable)
	  {
		Preconditions.checkNotNull(wmoTable);
		Preconditions.checkArgument(wmoTable.Type == TableType.param);
		this.wmoTable = wmoTable;
		ImmutableMap<int, GribTables.Parameter>.Builder builder = ImmutableMap<int, GribTables.Parameter>.builder();
		foreach (WmoEntry entry in wmoTable.Entries)
		{
		  builder.put(entry.Number, new WmoParamEntry(this, entry));
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
			return wmoTable.Name;
		  }
	  }

	  public virtual ImmutableList<GribTables.Parameter> Parameters
	  {
		  get
		  {
			return ImmutableList<GribTables.Parameter>.copyOf(entryMap.Values);
		  }
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable @Override public NetcdfSharp.ucar.nc2.grib.GribTables.Parameter getParameter(int number)
	  public virtual GribTables.Parameter getParameter(int number)
	  {
		return entryMap.get(number);
	  }

	  private class WmoParamEntry : GribTables.Parameter
	  {
		  private readonly WmoParamTable outerInstance;

		internal WmoTable.WmoEntry entry;

		public WmoParamEntry(WmoParamTable outerInstance, WmoTable.WmoEntry entry)
		{
			this.outerInstance = outerInstance;
		  this.entry = entry;
		}

		public virtual int Discipline
		{
			get
			{
			  return entry.Discipline;
			}
		}

		public virtual int Category
		{
			get
			{
			  return entry.Category;
			}
		}

		public virtual int Number
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

		public virtual string Unit
		{
			get
			{
			  return entry.Unit;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable @Override public String getAbbrev()
		public virtual string Abbrev
		{
			get
			{
			  return null;
			}
		}

		public virtual string Description
		{
			get
			{
			  return entry.Name;
			}
		}

		public virtual string Id
		{
			get
			{
			  return entry.Id;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable @Override public System.Nullable<float> getFill()
		public virtual float? Fill
		{
			get
			{
			  return null;
			}
		}

		public virtual float? Missing
		{
			get
			{
			  return float.NaN;
			}
		}

		public virtual string OperationalStatus
		{
			get
			{
			  return entry.Status;
			}
		}
	  }

	}

}
