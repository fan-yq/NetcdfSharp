// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1.tables
{
	using GribResourceReader = NetcdfSharp.ucar.nc2.grib.GribResourceReader;
	using Grib1Parameter = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Parameter;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;

	/// <summary>
	/// This is the interface to manage GRIB-1 Parameter Table lookups.
	/// A lookup is a (center, subcenter, version) --> Parameter Table path.
	/// The lookups are loaded at startup, but the Parameter Tables arent read until requested,
	/// via getParameter(int center, int subcenter, int tableVersion, int param_number).
	/// 
	/// These are the tables that are loaded at runtime, matching center and versions.
	/// <p/>
	/// Allow different table versions in the same file.
	/// Allow overriding standard grib1 tables on the dataset level.
	/// 
	/// @author caron
	/// @since 9/13/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib1ParamTables
	public class Grib1ParamTables
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib1ParamTables));

	  private static readonly object @lock = new object();
	  private static int standardTablesStart; // heres where the standard tables start - keep track so user additions
											  // can go first

	  private static Lookup standardLookup;
	  private static Grib1ParamTableReader defaultWmoTable;

	  static Grib1ParamTables()
	  {
		try
		{
		  standardLookup = new Lookup();
		  standardLookup.readLookupTable("resources/grib1/lookupTables.txt");
		  standardLookup.readLookupTable("resources/grib1/ecmwfEcCodes/lookupTables.txt");
		  standardLookup.readLookupTable("resources/grib1/ecmwf/lookupTables.txt");
		  standardLookup.readLookupTable("resources/grib1/ncl/lookupTables.txt");
		  standardLookup.readLookupTable("resources/grib1/dss/lookupTables.txt");
		  // standardLookup.readLookupTable("resources/grib1/ncep/lookupTables.txt");
		  standardLookup.readLookupTable("resources/grib1/wrf/lookupTables.txt"); // */
		  // lookup.readLookupTable("resources/grib1/tablesOld/lookupTables.txt"); // too many problems - must check every
		  // one !
		  standardLookup.tables = new List<Grib1ParamTableReader>(standardLookup.tables); // in case user adds tables
		  defaultWmoTable = standardLookup.getParameterTable(0, -1, -1); // user cannot override default

		}
		catch (Exception t)
		{
		  logger.warn("Grib1ParamTables init failed: ", t);
		}
	  }

	  private static bool strict;

	  public static bool Strict
	  {
		  get
		  {
			return strict;
		  }
		  set
		  {
			Grib1ParamTables.strict = value;
		  }
	  }


	  public static Grib1ParamTableReader DefaultWmoTable
	  {
		  get
		  {
			return defaultWmoTable;
		  }
	  }

	  // Make a key from (center, subcenter, version) that provides correct sort order.
	  internal static int makeKey(int center, int subcenter, int version)
	  {
		if (center < 0)
		{
		  center = 255;
		}
		if (subcenter < 0)
		{
		  subcenter = 255;
		}
		if (version < 0)
		{
		  version = 255;
		}
		return center * 1000 * 1000 + subcenter * 1000 + version;
	  }

	  internal static string showKey(int key)
	  {
		int center = key / (1000 * 1000);
		key = key - center * (1000 * 1000);
		int subcenter = key / (1000);
		key = key - subcenter * 1000;
		int version = key;
		return string.Format("{0,2:D}-{1,2:D}-{2,2:D}", center, subcenter, version);
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	  // Map of paramTablePath -> Grib1ParamTableReader
	  private static readonly IDictionary<string, Grib1ParamTableReader> localTableHash = new ConcurrentDictionary<string, Grib1ParamTableReader>();

	  /// <summary>
	  /// Get a Grib1ParamTables object, optionally specifying a parameter table or lookup table specific to this dataset.
	  /// </summary>
	  /// <param name="paramTablePath"> path to a parameter table, in format Grib1ParamTable can read. </param>
	  /// <param name="lookupTablePath"> path to a lookup table, in format Lookup.readLookupTable() can read. </param>
	  /// <returns> Grib1Tables </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static Grib1ParamTables factory(String paramTablePath, String lookupTablePath) throws IOException
	  public static Grib1ParamTables factory(string paramTablePath, string lookupTablePath)
	  {
		if (string.ReferenceEquals(paramTablePath, null) && string.ReferenceEquals(lookupTablePath, null))
		{
		  return new Grib1ParamTables();
		}
		Lookup lookup = null;
		Grib1ParamTableReader @override = null;

		Grib1ParamTableReader table;
		if (!string.ReferenceEquals(paramTablePath, null))
		{
		  table = localTableHash[paramTablePath];
		  if (table == null)
		  {
			table = new Grib1ParamTableReader(paramTablePath);
			localTableHash[paramTablePath] = table;
			@override = table;
		  }
		}

		if (!string.ReferenceEquals(lookupTablePath, null))
		{
		  lookup = new Lookup();
		  if (!lookup.readLookupTable(lookupTablePath))
		  {
			throw new FileNotFoundException("cant read lookup table=" + lookupTablePath);
		  }
		}

		return new Grib1ParamTables(lookup, @override);
	  }

	  /// <summary>
	  /// Get a Grib1Tables object, optionally specifiying a parameter table in XML specific to this dataset.
	  /// </summary>
	  /// <param name="paramTableElem"> parameter table in XML </param>
	  /// <returns> Grib1Tables </returns>
	  public static Grib1ParamTables factory(NetcdfSharp.org.jdom2.Element paramTableElem)
	  {
		if (paramTableElem == null)
		{
		  return new Grib1ParamTables();
		}
		return new Grib1ParamTables(null, new Grib1ParamTableReader(paramTableElem));
	  }

	  ///////////////////////////////////////////////////////////////////////////

	  private readonly Lookup lookup; // if lookup table was set
	  private readonly Grib1ParamTableReader @override; // Dataset specific override.

	  // This is the "StandardLookup". LOOK rename Grib1ParamTables -> Grib1ParamLookup.
	  public Grib1ParamTables()
	  {
		this.lookup = null;
		this.@override = null;
	  }

	  // Possible overrides of the StandardLookup.
	  private Grib1ParamTables(Lookup lookup, Grib1ParamTableReader @override)
	  {
		this.lookup = lookup;
		this.@override = @override;
	  }

	  public virtual Grib1Parameter getParameter(Grib1Record record)
	  {
		Grib1SectionProductDefinition pds = record.PDSsection;
		return getParameter(pds.Center, pds.SubCenter, pds.TableVersion, pds.ParameterNumber);
	  }

	  public virtual Grib1Parameter getParameter(int center, int subcenter, int tableVersion, int param_number)
	  {
		Grib1Parameter param = null;
		if (@override != null)
		{
		  param = @override.getParameter(param_number);
		}
		if (param == null && lookup != null)
		{
		  param = lookup.getParameter(center, subcenter, tableVersion, param_number);
		}
		if (param == null)
		{
		  param = standardLookup.getParameter(center, subcenter, tableVersion, param_number);
		}
		return param;
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Debugging only
	  /// </summary>
	  public virtual Grib1ParamTableReader getParameterTable(int center, int subcenter, int tableVersion)
	  {
		Grib1ParamTableReader result = null;
		if (lookup != null)
		{
		  result = lookup.getParameterTable(center, subcenter, tableVersion);
		}
		if (result == null)
		{
		  result = standardLookup.getParameterTable(center, subcenter, tableVersion); // standard tables
		}

		return result;
	  }

	  // debugging
	  public static IList<Grib1ParamTableReader> StandardParameterTables
	  {
		  get
		  {
			return standardLookup.tables;
		  }
	  }

	  /// <summary>
	  /// Add all tables in list to standard tables
	  /// </summary>
	  /// <param name="lookupFilename"> filename containing list of tables </param>
	  /// <returns> true if read ok, false if file not found </returns>
	  /// <exception cref="IOException"> if file found but read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static boolean addParameterTableLookup(String lookupFilename) throws IOException
	  public static bool addParameterTableLookup(string lookupFilename)
	  {
		Lookup lookup = new Lookup();
		if (!lookup.readLookupTable(lookupFilename))
		{
		  return false;
		}

		lock (@lock)
		{
		  ((List<Grib1ParamTableReader>)standardLookup.tables).InsertRange(standardTablesStart, lookup.tables);
		  standardTablesStart += lookup.tables.Count;
		}
		return true;
	  }

	  /// <summary>
	  /// Add table to standard tables for a specific center, subcenter and version.
	  /// </summary>
	  /// <param name="center"> center id </param>
	  /// <param name="subcenter"> subcenter id, or -1 for all </param>
	  /// <param name="tableVersion"> table verssion, or -1 for all </param>
	  /// <param name="tableFilename"> file to read parameter table from </param>
	  public static void addParameterTable(int center, int subcenter, int tableVersion, string tableFilename)
	  {
		Grib1ParamTableReader table = new Grib1ParamTableReader(center, subcenter, tableVersion, tableFilename);
		lock (@lock)
		{
		  standardLookup.tables.Insert(standardTablesStart, table);
		  standardTablesStart++;
		}
	  }

	  //////////////////////////////////////////////////////////////////////////

	  private class Lookup
	  {
		internal IList<Grib1ParamTableReader> tables = new List<Grib1ParamTableReader>();
		// Map (center, subcenter, version) -> Grib1ParamTable
		internal IDictionary<int, Grib1ParamTableReader> tableMap = new ConcurrentDictionary<int, Grib1ParamTableReader>();

		/// <summary>
		/// read the lookup table from file
		/// </summary>
		/// <param name="resourceName"> read from file </param>
		/// <returns> true if successful </returns>
		/// <exception cref="IOException"> On badness </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: boolean readLookupTable(String resourceName) throws IOException
		internal virtual bool readLookupTable(string resourceName)
		{
		  using (Stream inputStream = GribResourceReader.getInputStream(resourceName))
		  {
			return readLookupTable(inputStream, resourceName);
		  }
		}

		/// <summary>
		/// read the lookup table from an input stream
		/// </summary>
		/// <param name="is"> The input stream </param>
		/// <param name="lookupFile"> full pathname of lookup file </param>
		/// <returns> true if successful </returns>
		/// <exception cref="IOException"> On badness </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private boolean readLookupTable(InputStream is, String lookupFile) throws IOException
		internal virtual bool readLookupTable(Stream @is, string lookupFile)
		{
		  if (@is == null)
		  {
			return false;
		  }

		  string parentPath = (new File(lookupFile)).getParent();
		  File parent = parentPath == null ? null : new File(parentPath);
		  using (StreamReader isr = new StreamReader(@is, Encoding.UTF8))
		  {

			string line;
			while (!string.ReferenceEquals((line = isr.ReadLine()), null))
			{
			  line = line.Trim();
			  if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
			  {
				continue;
			  }
			  string[] tableDefArr = line.Split(':', StringSplitOptions.None);

			  int center = int.Parse(tableDefArr[0].Trim());
			  int subcenter = int.Parse(tableDefArr[1].Trim());
			  int version = int.Parse(tableDefArr[2].Trim());
			  string filename = tableDefArr[3].Trim();
			  string path;
			  if (filename.StartsWith("/", StringComparison.Ordinal) || filename.StartsWith("\\", StringComparison.Ordinal) || filename.StartsWith("file:", StringComparison.Ordinal) || filename.StartsWith("http://", StringComparison.Ordinal))
			  {
				path = filename;
			  }
			  else
			  {
				File tableFile = new File(parent, filename); // relative file
				path = tableFile.getPath();
			  }

			  Grib1ParamTableReader table = new Grib1ParamTableReader(center, subcenter, version, path);
			  tables.Add(table);
			}
		  }
		  return true;
		}

		internal virtual Grib1Parameter getParameter(int center, int subcenter, int tableVersion, int param_number)
		{
		  if (strict && param_number < 128 && tableVersion < 128)
		  {
			return defaultWmoTable.getParameter(param_number);
		  }

		  Grib1ParamTableReader pt = getParameterTable(center, subcenter, tableVersion);
		  Grib1Parameter param = null;
		  if (pt != null)
		  {
			param = pt.getParameter(param_number);
		  }
		  if (!strict && param == null)
		  {
			param = defaultWmoTable.getParameter(param_number);
		  }
		  return param;
		}

		internal virtual Grib1ParamTableReader getParameterTable(int center, int subcenter, int tableVersion)
		{
		  // look in hash table
		  int key = makeKey(center, subcenter, tableVersion);
		  Grib1ParamTableReader table;
		  if (tableMap.TryGetValue(key, out table))
		  {
			return table;
		  }

		  // match from lookup tables(s)
		  table = findParameterTableExact(center, subcenter, tableVersion);
		  if (table == null)
		  {
			table = findParameterTable(center, subcenter, tableVersion);
		  }
		  if (table == null)
		  {
			if (strict || defaultWmoTable == null)
			{
			  // table = findParameterTable(center, subcenter, tableVersion); // debug
			  logger.warn("Could not find a table for GRIB file with center: " + center + " subCenter: " + subcenter + " version: " + tableVersion);
			  throw new System.NotSupportedException("Could not find a table for GRIB file with center: " + center + " subCenter: " + subcenter + " version: " + tableVersion);
			}
			return defaultWmoTable;
		  }

		  Grib1ParamTableReader prevTable;
		  if (tableMap.TryGetValue(key, out prevTable))
		  {
			logger.warn("***Duplicate Table for {} == {}", prevTable.Path, table.Path);
		  }

		  tableMap[key] = table; // assume we would get the same table in any thread, so race condition is ok
		  return table;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable private Grib1ParamTableReader findParameterTableExact(int center, int subcenter, int version)
		internal virtual Grib1ParamTableReader findParameterTableExact(int center, int subcenter, int version)
		{
		  IList<Grib1ParamTableReader> localCopy = tables; // thread safe
		  foreach (Grib1ParamTableReader table in localCopy)
		  {
			// look for a match
			if (center == table.Center_id && subcenter == table.Subcenter_id && version == table.Version)
			{ // match
			  if (table.Parameters == null) // see if the parameters for this table have been read in yet.
			  {
				continue; // failed - maybe theres another entry table that matches
			  }

			  // success - initialize other tables with the same path
			  foreach (Grib1ParamTableReader table2 in localCopy)
			  {
				if (table2.Path.Equals(table.Path))
				{
				  table2.Parameters = table.Parameters;
				}
			  }
			  return table;
			}
		  }
		  return null;
		}

		// wildcard match
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable private Grib1ParamTableReader findParameterTable(int center, int subcenter, int version)
		internal virtual Grib1ParamTableReader findParameterTable(int center, int subcenter, int version)
		{
		  IList<Grib1ParamTableReader> localCopy = tables; // thread safe
		  foreach (Grib1ParamTableReader table in localCopy)
		  {
			// look for a match
			if (center == table.Center_id)
			{
			  if ((table.Subcenter_id == -1) || (subcenter == table.Subcenter_id))
			  {
				// if ((table.subcenter_id == -1) || (table.subcenter_id == 0) || (subcenter == table.subcenter_id)) {
				if ((table.Version == -1) || version == table.Version)
				{ // match
				  // see if the parameters for this table have been read in yet.
				  if (table.Parameters == null)
				  {
					continue; // failed - maybe theres another entry table that matches
				  }

				  // success - initialize other tables with the same path
				  foreach (Grib1ParamTableReader table2 in localCopy)
				  {
					if (table2.Path.Equals(table.Path))
					{
					  table2.Parameters = table.Parameters;
					}
				  }
				  return table;
				}
			  }
			}
		  }
		  return null;
		}

	  } // Lookup

	}

}
