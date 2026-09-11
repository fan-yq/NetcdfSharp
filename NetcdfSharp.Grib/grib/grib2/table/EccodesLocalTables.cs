// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using static NetcdfSharp.ucar.nc2.grib.grib2.table.EccodesCodeTable;
	using Parameter = NetcdfSharp.ucar.nc2.grib.GribTables.Parameter;

	/// <summary>
	/// The results of EcmwfParamTableCompare indicate there are no significant differences of the parameter tables with WMO.
	/// The results of EcmwfCodeTableCompare indicate there are some differences of the code tables with WMO.
	/// See the comments at in EcmwfCodeTable.java.
	/// 
	/// 1) This change in ECMWF table 1.0 implies that if master table = 255, then all WMO entries may be overridden.
	/// Havent seen this in practice; in particular I wonder if any ECMWF GRIB2 files do this.
	/// LOOK: This is not implemented.
	/// 
	/// Code table 1.0 - GRIB master tables version number
	/// Ecmwf version 21 discipline 1 category 0 (resources/grib2/ecmwf/tables/21/1.0.table)
	/// WMO= 255 Missing
	/// ECMWF= 255 Master tables not used. Local table entries and local templates may use the entire range of the table, not
	/// just
	/// those sections marked Reserved for local used.
	/// 
	/// 2) Despite missing code entry 3.1.12, there is a WMO 3.12 template. However none for 3.130.
	/// I dont yet see templates defined by ECMWF. So, not implemented.
	/// 
	/// Code table 3.1 - Grid definition template number
	/// Ecmwf version 21 discipline 3 category 1 (resources/grib2/ecmwf/tables/21/3.1.table)
	/// WMO 3.1 missing EcmwfEntry{codeValue=12, name=Transverse Mercator}
	/// WMO 3.1 missing EcmwfEntry{codeValue=130, name=Irregular latitude/longitude grid}
	/// 
	/// 3) LOOK: none of these are implemented.
	/// Code table 4.0 - Product definition template number
	/// Ecmwf version 21 discipline 4 category 0 (resources/grib2/ecmwf/tables/21/4.0.table)
	/// WMO 4.0 missing EcmwfEntry{codeValue=311, name=Satellite product auxiliary information}
	/// WMO 4.0 missing EcmwfEntry{codeValue=40033, name=Individual ensemble forecast, control and perturbed, at a horizontal
	/// level or in a horizontal layer at a point in time for simulated}
	/// WMO 4.0 missing EcmwfEntry{codeValue=40034, name=Individual ensemble forecast, control and perturbed, at a horizontal
	/// level or in a horizontal layer, in a continuous or non-continuous interval for simulated}
	/// WMO 4.0 missing EcmwfEntry{codeValue=50001, name=Forecasting Systems with Variable Resolution in a point in time}
	/// WMO 4.0 missing EcmwfEntry{codeValue=50011, name=Forecasting Systems with Variable Resolution in a continous or non
	/// countinous time interval}
	/// 
	/// 4) Implemented below.
	/// Code table 4.230 - Atmospheric chemical constituent type
	/// Ecmwf version 21 discipline 4 category 230 (resources/grib2/ecmwf/tables/21/4.230.table)
	/// WMO 4.230 missing EcmwfEntry{codeValue=0, name=Ozone O3}
	/// ... (all of them?)
	/// Same with
	/// Code table 4.233 - Aerosol type
	/// 
	/// 5) LOOK: not implemented.
	/// Code table 5.0 - Data representation template number
	/// Ecmwf version 21 discipline 5 category 0 (resources/grib2/ecmwf/tables/21/5.0.table)
	/// WMO 5.0 missing EcmwfEntry{codeValue=6, name=Grid point data - simple packing with pre-processing}
	/// WMO 5.0 missing EcmwfEntry{codeValue=40000, name=JPEG2000 Packing}
	/// WMO 5.0 missing EcmwfEntry{codeValue=40010, name=PNG pacling}
	/// WMO 5.0 missing EcmwfEntry{codeValue=50000, name=Sperical harmonics ieee packing}
	/// WMO 5.0 missing EcmwfEntry{codeValue=50001, name=Second order packing}
	/// WMO 5.0 missing EcmwfEntry{codeValue=50002, name=Second order packing}
	/// 
	/// 6) LOOK not implemented: So where is it specified?
	/// Code table 6.0 - Bit map indicator
	/// Ecmwf version 21 discipline 6 category 0 (resources/grib2/ecmwf/tables/21/6.0.table)
	/// WMO 6.0 missing EcmwfEntry{codeValue=1, name=A bit map pre-determined by the originating/generating centre applies to
	/// this product and is not specified in this Section}
	/// 
	/// 7) Implemented below.
	/// No WMO table that matches ECMWF table 4.192
	/// No WMO table that matches ECMWF table 5.40000
	/// No WMO table that matches ECMWF table 5.50002
	/// </summary>
	public class EccodesLocalTables : LocalTables
	{
	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(EccodesLocalTables));
	  private const string RESOURCE_DIRECTORY = "resources/grib2/ecmwf/tables/21";
	  private const string MATCH = "4.2.";

	  private const bool debugOpen = false;
	  private const bool debug = true;

	  private readonly IDictionary<int, EccodesParamTable> tableMap = new Dictionary<int, EccodesParamTable>(30);

	  // Each of the centers using eccodes has a separate EccodesLocalTables with its own Grib2TableConfig, and center
	  // value.
	  internal EccodesLocalTables(Grib2TableConfig config) : base(config)
	  {
		// LOOK: change to lazy init
		initParams(config.Path);
	  }

	  // These are the tables that have been augmented
	  private ImmutableSet<string> whitelist = ImmutableSet<string>.of("4.230", "4.233", "4.192", "5.40000", "5.50002");
	  private IDictionary<string, EccodesCodeTable> localTables = new Dictionary<string, EccodesCodeTable>();

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getCodeTableValue(String tableName, int code)
	  public override string getCodeTableValue(string tableName, int code)
	  {
		if (!whitelist.contains(tableName))
		{
		  return base.getCodeTableValue(tableName, code);
		}
		if (localTables.ContainsKey(tableName))
		{
		  Grib2CodeTableInterface.Entry entry = localTables[tableName].getEntry(code);
		  return (entry == null) ? null : entry.Name;
		}

		IEnumerator<string> tokens = Splitter.on('.').trimResults().omitEmptyStrings().split(tableName).GetEnumerator();

// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		tokens.MoveNext();
		int discipline = int.Parse(tokens.Current);
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		tokens.MoveNext();
		int category = int.Parse(tokens.Current);
		try
		{
		  EccodesCodeTable ecmwfCodeTable = EccodesCodeTable.factory(LATEST_VERSION, discipline, category);
		  localTables[tableName] = ecmwfCodeTable;

		  Grib2CodeTableInterface.Entry entry = ecmwfCodeTable.getEntry(code);
		  return (entry == null) ? null : entry.Name;
		}
		catch (IOException)
		{
		  logger.warn("No such EccodesCodeTable {} {} {}", LATEST_VERSION, discipline, category);
		}
		return null;
	  }

	  internal EccodesLocalConcepts localConcepts;
	  internal ImmutableListMultimap<int, Grib2Parameter> localConceptMultimap;

	  private void initParams(string path)
	  {
		try
		{
		  localConcepts = new EccodesLocalConcepts(path);
		  localConceptMultimap = localConcepts.LocalConceptMultimap;
		  foreach (KeyValuePair<int, ICollection<Grib2Parameter>> entry in localConceptMultimap.asMap().entrySet())
		  {
			if (isLocal(entry.Key))
			{
			  localParams[entry.Key] = entry.Value.First();
			}
		  }
		}
		catch (IOException e)
		{
		  logger.warn("EccodesLocalTables failed on {}", path, e);
		}
	  }

	  public override ImmutableList<Parameter> Parameters
	  {
		  get
		  {
	// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
			return ImmutableList<GribTables.Parameter>.copyOf(localConceptMultimap.values().Select(p => (GribTables.Parameter) p).OrderBy(p => p, new ParameterSort()));
		  }
	  }

	  public override void showDetails(Formatter f)
	  {
		localConcepts.showDetails(f);
	  }

	  public override void showEntryDetails(Formatter f, IList<GribTables.Parameter> @params)
	  {
		localConcepts.showEntryDetails(f, @params);
	  }

	}

}
