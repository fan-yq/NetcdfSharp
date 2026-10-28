// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr.tables
{
	using Matcher = NetcdfSharp.com.google.re2j.Matcher;
	using Pattern = NetcdfSharp.com.google.re2j.Pattern;
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using Descriptor = NetcdfSharp.ucar.nc2.iosp.bufr.Descriptor;
	using MessageScanner = NetcdfSharp.ucar.nc2.iosp.bufr.MessageScanner;
	using TableLookup = NetcdfSharp.ucar.nc2.iosp.bufr.TableLookup;
	using TableParser = NetcdfSharp.ucar.nc2.util.TableParser;
	using Util = NetcdfSharp.ucar.nc2.wmo.Util;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Reads BUFR tables of various forms. Interacts with TableLookup.
	/// 
	/// <pre>
	/// Table B:
	/// 
	/// csv----------
	/// Class,FXY,enElementName,BUFR_Unit,BUFR_Scale,BUFR_ReferenceValue,BUFR_DataWidth_Bits,CREX_Unit,CREX_Scale,CREX_DataWidth,Status
	/// 00,000001,Table A: entry,CCITT IA5,0,0,24,Character,0,3,Operational
	/// 
	/// 
	/// ecmwf---------
	/// 000001 TABLE A:  ENTRY                                                  CCITTIA5                   0            0  24 CHARACTER                 0          3
	/// 000001 TABLE A:  ENTRY                                                  CCITTIA5                   0            0  24 CHARACTER                 0         3
	/// 
	/// 
	/// mel-bufr-----------
	/// 0; 7; 190; 1; -1024; 12; M; HEIGHT INCREMENT
	/// 
	/// 
	/// mel-tabs (tab delimited) ---------------
	/// F  X  Y  Scale  RefVal  Width  Units  Element Name
	/// 0  0  1  0  0  24  CCITT_IA5  Table A: entry
	/// 0  0  2  0  0  256  CCITT_IA5  Table A: data category description, line 1
	/// 
	/// 
	/// ncep-----------
	/// #====================================================================================================
	/// # F-XX-YYY |SCALE| REFERENCE   | BIT |      UNIT      | MNEMONIC ;DESC ;  ELEMENT NAME
	/// #          |     |   VALUE     |WIDTH|                |          ;CODE ;
	/// #====================================================================================================
	/// 0-00-001 |   0 |           0 |  24 | CCITT IA5      | TABLAE   ;     ; Table A: entry
	/// 
	/// 
	/// opera ----------------------------
	/// 0;02;181;Supplementary present weather sensor;Flag-Table;0;0;21
	/// 0;07;192;Pixel size in Z-direction;Meters;-1;0;16
	/// 0;21;036;Radar rainfall intensity;mm*h-1;2;0;16
	/// 
	/// 
	/// ===============================================================
	/// Table D:
	/// csv----------
	/// SNo,Category,FXY1,enElementName1,FXY2,enElementName2,Status
	/// 1,00,300002,,000002,"Table A category, line 1",Operational
	/// 
	/// ecmwf-------------
	/// 300002  2 000002
	/// 000003
	/// 300003  3 000010
	/// 000011
	/// 000012
	/// 
	/// mel-bufr------------
	/// 3   1 192  optional_name
	/// 0   1   7
	/// 0  25  60
	/// 0   1  33
	/// 1   1   2
	/// 3  61 169
	/// 0   5  40
	/// -1
	/// 
	/// ncep ------------------
	/// #====================================================================================================
	/// # F-XX-YYY | MNEMONIC   ;DCOD ; NAME           <-- sequence definition
	/// #          | F-XX-YYY > | NAME                 <-- element definition (first thru next-to-last)
	/// #          | F-XX-YYY   | NAME                 <-- element definition (last)
	/// #====================================================================================================
	/// 
	/// 3-00-002 | TABLACAT   ;     ; Table A category definition
	/// | 0-00-002 > | Table A category, line 1
	/// | 0-00-003   | Table A category, line 2
	/// 
	/// opera ---------------
	/// # Heights of side view
	/// 3;13;192;  1;01;000
	/// ;  ;   ;  0;31;001
	/// ;  ;   ;  0;10;007
	/// # 4 bit per pixel radar images (top view)
	/// 3;21;192;  1;10;000
	/// ...
	/// 
	/// </pre>
	/// </summary>

	public class BufrTables
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(BufrTables));

	  public enum Mode
	  {
		wmoOnly, // wmo entries only found from wmo table
		wmoLocal, // if wmo entries not found in wmo table, look in local table
		localOverride // look in local first, then wmo
	  }

	  public enum Format
	  {
		ecmwf,
		mel_bufr,
		mel_tabs,
		ncep,
		ncep_nm,
		opera,
		ukmet,
		wmo_csv,
		wmo_xml,
		cypher,
		embed
	  }

	  internal const string RESOURCE_PATH = "/resources/bufrTables/";
	  private static readonly string canonicalLookup = "resource:" + RESOURCE_PATH + "local/tablelookup.csv";
	  private const int latestVersion = 37;

	  private const bool showTables = false;
	  private const bool showReadErrs = true;

	  private static IList<TableConfig> tables;
	  private static readonly IDictionary<string, TableB> tablesB = new ConcurrentDictionary<string, TableB>();
	  private static readonly IDictionary<string, TableD> tablesD = new ConcurrentDictionary<string, TableD>();

	  private static IList<string> lookups;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized void addLookupFile(String filename) throws FileNotFoundException
	  public static void addLookupFile(string filename)
	  {
		  lock (typeof(BufrTables))
		  {
			if (lookups == null)
			{
			  lookups = new List<string>();
			}
			File f = new File(filename);
			if (f.isDirectory())
			{
			  throw new FileNotFoundException(filename + " is a directory");
			}
			if (!f.exists())
			{
			  throw new FileNotFoundException(filename + " not found");
			}
			lookups.Add(filename);
		  }
	  }

	  private static void readLookupTable()
	  {
		  lock (typeof(BufrTables))
		  {
			tables = new List<TableConfig>();
			if (lookups != null)
			{
			  lookups.Add(canonicalLookup);
			  foreach (string fname in lookups)
			  {
				readLookupTable(fname);
			  }
			}
			else
			{
			  readLookupTable(canonicalLookup);
			}
		  }
	  }

	  // center,subcenter,master,local,cat,tableB,tableBformat,tableD,tableDformat, mode
	  private static void readLookupTable(string filename)
	  {

		try
		{
				using (Stream ios = openStream(filename))
				{
			  StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
			  int count = 0;
			  while (true)
			  {
				string line = dataIS.ReadLine();
				if (string.ReferenceEquals(line, null))
				{
				  break;
				}
				if (line.StartsWith("#", StringComparison.Ordinal))
				{
				  continue;
				}
				count++;

				string[] flds = line.Split(',');
				if (flds.Length < 8)
				{
				  if (showReadErrs)
				  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
					Console.Write("{0:D} BAD line == {1}%n", count, line);
				  }
				  continue;
				}

				int fldidx = 0;
				try
				{
				  TableConfig table = new TableConfig();
				  table.name = flds[fldidx++].Trim();
				  table.center = int.Parse(flds[fldidx++].Trim());
				  table.subcenter = int.Parse(flds[fldidx++].Trim());
				  table.master = int.Parse(flds[fldidx++].Trim());
				  table.local = int.Parse(flds[fldidx++].Trim());
				  table.cat = int.Parse(flds[fldidx++].Trim());
				  table.tableBname = flds[fldidx++].Trim();
				  table.tableBformat = getFormat(flds[fldidx++].Trim(), line);
				  if (table.tableBformat == null)
				  {
					continue;
				  }

				  table.tableDname = flds[fldidx++].Trim();
				  table.tableDformat = getFormat(flds[fldidx++].Trim(), line);

				  if (fldidx < flds.Length)
				  {
					string modes = flds[fldidx++].Trim();
					if (modes.Equals("wmoLocal", StringComparison.OrdinalIgnoreCase))
					{
					  table.mode = Mode.wmoLocal;
					}
					else if (modes.Equals("localWmo", StringComparison.OrdinalIgnoreCase))
					{
					  table.mode = Mode.localOverride;
					}
				  }

				  tables.Add(table);
				  if (showTables)
				  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
					Console.Write("Added table == {0}%n", table);
				  }

				}
				catch (Exception e)
				{
				  if (showReadErrs)
				  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
					Console.Write("{0:D} {1:D} BAD line == {2} ({3})%n", count, fldidx, line, e.Message);
				  }
				}
			  }

				}
		}
		catch (IOException ioe)
		{
		  string mess = "Need BUFR tables in path; looking for " + filename;
		  throw new Exception(mess, ioe);
		}
	  }

	  private static Format? getFormat(string formatS, string line)
	  {
		if (formatS.Length == 0)
		{
		  return null;
		}
		try
		{
		  return (Format)Enum.Parse(typeof(Format), formatS);
		}
		catch (Exception)
		{
		  if (showReadErrs)
		  {
			log.warn("BAD format = {} line == {}%n", formatS, line);
		  }
		  return null;
		}
	  }

	  ////////////////////////////////////////////////////////////////////////////////////

	  public class TableConfig
	  {
		internal string name;
		internal int center, subcenter, master, local, cat;
		internal string tableBname, tableDname;
		internal Format? tableBformat, tableDformat;
		internal Mode? mode = Mode.localOverride;

		internal virtual bool matches(int center, int subcenter, int master, int local, int cat)
		{
		  if ((this.center >= 0) && (center >= 0) && (center != this.center))
		  {
			return false;
		  }
		  if ((this.subcenter >= 0) && (subcenter >= 0) && (subcenter != this.subcenter))
		  {
			return false;
		  }
		  if ((this.master >= 0) && (master >= 0) && (master != this.master))
		  {
			return false;
		  }
		  if ((this.local >= 0) && (local >= 0) && (local != this.local))
		  {
			return false;
		  }
		  return (this.cat < 0) || (cat < 0) || (cat == this.cat);
		}

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual Format TableBformat
		{
			get
			{
			  return tableBformat.Value;
			}
		}

		public virtual Format TableDformat
		{
			get
			{
			  return tableDformat.Value;
			}
		}

		public virtual Mode Mode
		{
			get
			{
			  return mode.HasValue ? mode.Value : Mode.localOverride;
			}
		}

		public virtual string TableDname
		{
			get
			{
			  return tableDname;
			}
		}

		public virtual string TableBname
		{
			get
			{
			  return tableBname;
			}
		}

		public override string ToString()
		{
		  return name;
		}

	  }

	  public static IList<TableConfig> getTables()
	  {
		  if (tables == null)
		  {
			  readLookupTable();
		  }
		  return tables;
	  }

	  public static TableConfig[] TableConfigsAsArray
	  {
		  get
		  {
			if (tables == null)
			{
			  readLookupTable();
			}
			TableConfig[] result = new TableConfig[tables.Count];
			tables.CopyTo(result, 0);
			return result;
		  }
	  }

	  private static TableConfig matchTableConfig(int center, int subcenter, int master, int local, int cat)
	  {
		if (tables == null)
		{
		  readLookupTable();
		}

		foreach (TableConfig tc in tables)
		{
		  if (tc.matches(center, subcenter, master, local, cat))
		  {
			return tc;
		  }
		}
		return null;
	  }

	  /*
	   * private static TableConfig matchTableConfig(BufrIdentificationSection ids) {
	   * if (tables == null) readLookupTable();
	   * 
	   * int center = ids.getCenterId();
	   * int subcenter = ids.getSubCenterId();
	   * int master = ids.getMasterTableVersion();
	   * int local = ids.getLocalTableVersion();
	   * int cat = ids.getCategory();
	   * 
	   * return matchTableConfig(center, subcenter, master, local, cat);
	   * }
	   */

	  ///////////////////////

	  /*
	   * Note Robb has this cleanup in DescriptorTableB
	   * desc = desc.replaceFirst( "\\w+\\s+TABLE B ENTRY( - )?", "" );
	   * desc = desc.trim();
	   * this.description = desc;
	   * desc = desc.replaceAll( "\\s*\\(.*\\)", "" );
	   * desc = desc.replaceAll( "\\*", "" );
	   * desc = desc.replaceAll( "\\s+", "_" );
	   * desc = desc.replaceAll( "\\/", "_or_" );
	   * desc = desc.replaceFirst( "1-", "One-" );
	   * desc = desc.replaceFirst( "2-", "Two-" );
	   */

	  public class Tables
	  {
		public TableB b;
		public TableD d;
		public Mode mode;

		internal Tables()
		{
		}

		internal Tables(TableB b, TableD d, Mode? mode)
		{
		  this.b = b;
		  this.d = d;
		  this.mode = (mode == null) ? Mode.wmoOnly : mode.Value;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static Tables getLocalTables(int center, int subcenter, int master, int local, int cat) throws IOException
	  public static Tables getLocalTables(int center, int subcenter, int master, int local, int cat)
	  {
		TableConfig tc = matchTableConfig(center, subcenter, master, local, cat);
		if (tc == null)
		{
		  return null;
		}

		if (tc.tableBformat == Format.ncep_nm)
		{ // LOOK ??
		  // see if we already have it
		  tablesB.TryGetValue(tc.tableBname, out TableB b);
		  tablesD.TryGetValue(tc.tableBname, out TableD d);
		  if ((b != null) && (d != null))
		  {
			return new Tables(b, d, tc.mode);
		  }

		  // read it
		  b = new TableB(tc.tableBname, tc.tableBname);
		  d = new TableD(tc.tableBname, tc.tableBname);
		  Tables t = new Tables(b, d, tc.mode);
		  using (Stream ios = openStream(tc.tableBname))
		  {
			NcepMnemonic.read(ios, t);
		  }

		  // cache
		  tablesB[tc.tableBname] = t.b; // assume we would get the same table in any thread, so race condition is ok
		  tablesD[tc.tableBname] = t.d; // assume we would get the same table in any thread, so race condition is ok
		  return t;
		}

		Tables tables = new Tables();
		tables.b = readTableB(tc.tableBname, tc.tableBformat.Value, false);
		tables.d = readTableD(tc.tableDname, tc.tableDformat.Value, false);
		tables.mode = tc.mode.Value;

		return tables;
	  }

	  ////////////////////////////////////////////////////

	  private static TableB latestWmoB;

	  public static TableB WmoTableBlatest
	  {
		  get
		  {
			  lock (typeof(BufrTables))
			  {
				if (latestWmoB == null)
				{
				  try
				  {
					latestWmoB = getWmoTableB(latestVersion);
				  }
				  catch (IOException ioe)
				  {
					log.error("Cant open latest WMO ", ioe);
					throw new Exception(ioe.Message, ioe);
				  }
				}
				return latestWmoB;
			  }
		  }
	  }

	  /*
	   * // private static final String version14 = "wmo.v14";
	   * static public TableB getWmoTableBold(int version) throws IOException {
	   * String version13 = "wmo.v13.composite";
	   * String tableName = (version == 14) ? version14 : version13;
	   * TableB tb = tablesB.get(tableName);
	   * if (tb != null) return tb;
	   * 
	   * // always read 14 in
	   * TableConfig tc14 = matchTableConfig(0, 0, 14, 0, -1);
	   * TableB result = readTableB(tc14.tableBname, tc14.tableBformat, false);
	   * tablesB.put(version14, result); // hash by standard name
	   * 
	   * // everyone else uses 13 : cant override - do it in local if needed
	   * if (version < 14) {
	   * TableConfig tc = matchTableConfig(0, 0, 13, 0, -1);
	   * TableB b13 = readTableB(tc.tableBname, tc.tableBformat.Value, false);
	   * TableB.Composite bb = new TableB.Composite(version13, version13);
	   * bb.addTable(b13); // check in 13 first, so it overrides
	   * bb.addTable(result); // then in 14
	   * result = bb;
	   * tablesB.put(version13, result); // hash by standard name
	   * }
	   * 
	   * return result;
	   * }
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static TableB getWmoTableB(int masterTableVersion) throws IOException
	  public static TableB getWmoTableB(int masterTableVersion)
	  {
		TableConfig tc = matchTableConfig(0, 0, masterTableVersion, 0, -1);
		if (tc != null)
		{
		  return readTableB(tc.tableBname, tc.tableBformat.Value, false);
		}
		return null;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static TableB readTableB(String location, Format format, boolean force) throws IOException
	  public static TableB readTableB(string location, Format format, bool force)
	  {
		if (!force)
		{
		  tablesB.TryGetValue(location, out TableB tb);
		  if (tb != null)
		  {
			return tb;
		  }
		}
		if (showTables)
		{
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
		  Console.Write("Read BufrTable B {0} format={1}%n", location, format);
		}

		TableB b = new TableB(location, location);
		using (Stream ios = openStream(location))
		{
		  switch (format)
		  {
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.cypher:
			  readCypherTableB(ios, b);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.ecmwf:
			  readEcmwfTableB(ios, b);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.embed:
			  b = readEmbeddedTableB(location);
			  goto case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.ncep;

			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.ncep:
			  readNcepTableB(ios, b);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.ncep_nm:
			  Tables t = new Tables(b, null, null);
			  NcepMnemonic.read(ios, t);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.mel_bufr:
			  readMelbufrTableB(ios, b);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.mel_tabs:
			  readMeltabTableB(ios, b);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.opera:
			  readOperaTableB(ios, b);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.ukmet:
			  readBmetTableB(ios, b);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.wmo_csv:
			  readWmoCsvTableB(ios, b);
			  break;
			case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.wmo_xml:
			  WmoXmlReader.readWmoXmlTableB(ios, b);
			  break;
		  }
		}

	  if (b != null)
	  {
		tablesB[location] = b; // assume we would get the same table in any thread, so race condition is ok
	  }
	  return b;
	}

  private static void readWmoCsvTableB(Stream ios, TableB b)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
	int count = 0;
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.StartsWith("#", StringComparison.Ordinal))
	  {
		continue;
	  }
	  count++;

	  if (count == 1)
	  { // skip first line - its the header
		continue;
	  }

	  // any commas that are embedded in quotes - replace with blanks for now so split works
	  int pos1 = line.IndexOf('"');
	  if (pos1 >= 0)
	  {
		int pos2 = line.IndexOf('"', pos1 + 1);
		StringBuilder sb = new StringBuilder(line);
		for (int i = pos1; i < pos2; i++)
		{
		  if (sb[i] == ',')
		  {
			sb[i] = ' ';
		  }
		}
		line = sb.ToString();
	  }

	  string[] flds = line.Split(",");
	  if (flds.Length < 7)
	  {
		if (showReadErrs)
		{
		  Console.Write("{0:D} BAD split == {1}" + Environment.NewLine, count, line);
		}
		continue;
	  }

	  int fldidx = 1; // Start at 1 to skip classId
	  try
	  {
		int xy = int.Parse(flds[fldidx++].Trim());
		string name = StringUtil2.remove(flds[fldidx++], '"');
		string units = StringUtil2.filter(flds[fldidx++], " %+-_/()*"); // alphanumeric plus these chars
		int scale = int.Parse(clean(flds[fldidx++].Trim()));
		int refVal = int.Parse(clean(flds[fldidx++].Trim()));
		int width = int.Parse(clean(flds[fldidx++].Trim()));

		int x = xy / 1000;
		int y = xy % 1000;

		b.addDescriptor((short) x, (short) y, scale, refVal, width, name, units, null);

	  }
	  catch (Exception e)
	  {
		if (showReadErrs)
		{
		  Console.Write("{0:D} {1:D} BAD line == {2}" + Environment.NewLine, count, fldidx, line);
		}
	  }
	}
  }

  private static string clean(string s)
  {
	return StringUtil2.remove(s, ' ');
  }

  private static TableB readEmbeddedTableB(string location)
  {
	using (RandomAccessFile raf = new RandomAccessFile(location, "r"))
	{
	  MessageScanner scan = new MessageScanner(raf);
	  TableLookup lookup = scan.TableLookup;
	  if (lookup != null)
	  {
		return lookup.LocalTableB;
	  }
	  return null;
	}
  }

  private static TableD readEmbeddedTableD(string location)
  {
	using (RandomAccessFile raf = new RandomAccessFile(location, "r"))
	{
	  MessageScanner scan = new MessageScanner(raf);
	  TableLookup lookup = scan.TableLookup;
	  if (lookup != null)
	  {
		return lookup.LocalTableD;
	  }
	  return null;
	}
  }

  // tables are in mel-bufr format
  private static TableB readMelbufrTableB(Stream ios, TableB b)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);

	// read table B looking for descriptors
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.StartsWith("#", StringComparison.Ordinal) || line.Length == 0)
	  {
		continue;
	  }

	  try
	  {
		string[] split = line.Split(";");
		short x = short.Parse(split[1].Trim());
		short y = short.Parse(split[2].Trim());
		int scale = int.Parse(split[3].Trim());
		int refVal = int.Parse(split[4].Trim());
		int width = int.Parse(split[5].Trim());

		b.addDescriptor(x, y, scale, refVal, width, split[7], split[6], null);
	  }
	  catch (Exception e)
	  {
		log.error("Bad table B entry: table=" + b.Name + " entry=<" + line + ">", e.Message);
	  }
	}

	return b;
  }

  /*
   * tables are in cypher format : http://www.northern-lighthouse.com/tables/B_d00v13.htm
   * #
   * 002063 Aircraft roll angle
   * Degree
   * 2 -18000 16
   */
  private static TableB readCypherTableB(Stream ios, TableB b)
  {
	bool startMode = false;
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.Length == 0)
	  {
		continue;
	  }
	  if (line.StartsWith("<", StringComparison.Ordinal))
	  {
		continue;
	  }

	  if (line.StartsWith("#", StringComparison.Ordinal))
	  {
		startMode = true;
		continue;
	  }

	  if (startMode)
	  {
		try
		{
		  string xys = line.Substring(0, 8).Trim();
		  int xy = int.Parse(xys);
		  short x = (short) (xy / 1000);
		  short y = (short) (xy % 1000);

		  string name = Util.cleanName(line.Substring(8));
		  string units = "";
		  line = dataIS.ReadLine();
		  if (line != null)
		  {
			units = WmoXmlReader.cleanUnit(line);
		  }

		  int scale = 0, refVal = 0, width = 0;
		  line = dataIS.ReadLine();
		  if (line != null)
		  {
			line = StringUtil2.remove(line, '*');
			string[] split = StringUtil2.splitString(line);
			scale = int.Parse(split[0].Trim());
			refVal = int.Parse(split[1].Trim());
			width = int.Parse(split[2].Trim());
		  }

		  b.addDescriptor(x, y, scale, refVal, width, name, units, null);
		  startMode = false;
		}
		catch (Exception e)
		{
		  log.error("Bad table B entry: table=" + b.Name + " entry=<" + line + ">", e.Message);
		}
	  }
	}

	return b;
  }

  // tables are in mel-bufr format
  // #F X Y Scale RefVal Width Units Element Name
  // 0 0 1 0 0 24 CCITT_IA5 Table A: entry
  private static TableB readMeltabTableB(Stream ios, TableB b)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);

	// read table B looking for descriptors
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.StartsWith("#", StringComparison.Ordinal) || line.Length == 0)
	  {
		continue;
	  }

	  try
	  {
		string[] split = line.Split("\t");
		short x = short.Parse(split[1].Trim());
		short y = short.Parse(split[2].Trim());
		int scale = int.Parse(split[3].Trim());
		int refVal = int.Parse(split[4].Trim());
		int width = int.Parse(split[5].Trim());

		b.addDescriptor(x, y, scale, refVal, width, split[7], split[6], null);
	  }
	  catch (Exception e)
	  {
		log.error("Bad table " + b.Name + " entry=<" + line + ">", e);
	  }
	}

	return b;
  }

  // F-XX-YYY |SCALE| REFERENCE | BIT | UNIT | MNEMONIC ;DESC ; ELEMENT NAME
  private static TableB readNcepTableB(Stream ios, TableB b)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);

	dataIS.ReadLine(); // throw first line away

	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.StartsWith("#", StringComparison.Ordinal) || line.Length == 0)
	  {
		continue;
	  }

	  try
	  {
		string[] flds = Pattern.compile("[\\|;]").split(line);
		if (flds[0].Equals("END"))
		{
		  break;
		}

		if (flds.Length < 8)
		{
		  log.error("Bad line in table " + b.Name + " entry=<" + line + ">");
		  continue;
		}

		string fxys = flds[0];
		int scale = int.Parse(clean(flds[1]));
		int refVal = int.Parse(clean(flds[2]));
		int width = int.Parse(clean(flds[3]));
		string units = StringUtil2.remove(flds[4], '"');
		string name = StringUtil2.remove(flds[5], '"');
		string desc = StringUtil2.remove(flds[7], '"');

		string[] xyflds = fxys.Split("-");
		short x = short.Parse(clean(xyflds[1]));
		short y = short.Parse(clean(xyflds[2]));

		b.addDescriptor(x, y, scale, refVal, width, name, units, desc);

	  }
	  catch (Exception e)
	  {
		log.error("Bad table " + b.Name + " entry=<" + line + ">", e);
	  }
	}

	return b;
  }

  /*
   * opera ----------------------------
   * 0;02;181;Supplementary present weather sensor;Flag-Table;0;0;21
   * 0;07;192;Pixel size in Z-direction;Meters;-1;0;16
   * 0;21;036;Radar rainfall intensity;mm*h-1;2;0;16
   */
  private static void readOperaTableB(Stream ios, TableB b)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
	int count = 0;
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.StartsWith("#", StringComparison.Ordinal))
	  {
		continue;
	  }
	  count++;

	  string[] flds = line.Split(";");
	  if (flds.Length < 8)
	  {
		if (showReadErrs)
		{
		  Console.Write("{0:D} BAD split == {1}" + Environment.NewLine, count, line);
		}
		continue;
	  }

	  int fldidx = 1; // skip classId
	  try
	  {
		int x = int.Parse(flds[fldidx++].Trim());
		int y = int.Parse(flds[fldidx++].Trim());
		string name = StringUtil2.remove(flds[fldidx++], '"');
		string units = StringUtil2.filter(flds[fldidx++], " %+-_/()*"); // alphanumeric plus these chars
		int scale = int.Parse(clean(flds[fldidx++].Trim()));
		int refVal = int.Parse(clean(flds[fldidx++].Trim()));
		int width = int.Parse(clean(flds[fldidx++].Trim()));

		b.addDescriptor((short) x, (short) y, scale, refVal, width, name, units, null);

	  }
	  catch (Exception e)
	  {
		if (showReadErrs)
		{
		  Console.Write("{0:D} {1:D} BAD line == {2}" + Environment.NewLine, count, fldidx, line);
		}
	  }
	}
  }

  /*
   * fxy name units scale ref w units
   * 01234567 72 97 102 119
   * 001015 STATION OR SITE NAME CCITTIA5 0 0 160 CHARACTER 0 20
   * 001041 ABSOLUTE PLATFORM VELOCITY - FIRST COMPONENT (SEE NOTE 6) M/S 5 -1073741824 31 M/S 5 10
   */
  private static TableB readEcmwfTableB(Stream ios, TableB b)
  {
	IList<TableParser.Record> recs = TableParser.readTable(ios, "4i,7i,72,97,102i,114i,119i", 50000);
	foreach (TableParser.Record record in recs)
	{
	  if (record.nfields() < 7)
	  {
		continue;
	  }
	  int x = Convert.ToInt32(record.get(0));
	  int y = Convert.ToInt32(record.get(1));
	  string name = (string) record.get(2);
	  string units = (string) record.get(3);
	  int scale = Convert.ToInt32(record.get(4));
	  int @ref = Convert.ToInt32(record.get(5));
	  int width = Convert.ToInt32(record.get(6));

	  b.addDescriptor((short) x, (short) y, scale, @ref, width, name, units, null);
	}

	return b;
  }

  private static void readBmetTableB(Stream ios, TableB b)
  {
	NetcdfSharp.org.jdom2.Document doc;
	try
	{
	  SAXBuilder builder = new SAXBuilder();
	  builder.setExpandEntities(false);
	  doc = builder.build(ios);
	}
	catch (JDOMException e)
	{
	  throw new IOException(e.Message);
	}

	Element root = doc.getRootElement();
	IList<Element> featList = root.getChildren("featureCatalogue");
	foreach (Element featureCat in featList)
	{
	  IList<Element> features = featureCat.getChildren("feature");

	  foreach (Element feature in features)
	  {
		string name = feature.getChild("annotation").getChildTextNormalize("documentation");
		int f = int.Parse(feature.getChildText("F"));
		int x = int.Parse(feature.getChildText("X"));
		int y = int.Parse(feature.getChildText("Y"));
		int fxy = (f << 16) + (x << 8) + y;

		Element bufrElem = feature.getChild("BUFR");
		string units = bufrElem.getChildTextNormalize("BUFR_units");
		int scale = 0, reference = 0, width = 0;

		string s = null;
		try
		{
		  s = bufrElem.getChildTextNormalize("BUFR_scale");
		  scale = int.Parse(clean(s));
		}
		catch (FormatException e)
		{
		  log.warn(" key {} name '{}' has bad scale='{}'", fxy, name, s);
		}

		try
		{
		  s = bufrElem.getChildTextNormalize("BUFR_reference");
		  reference = int.Parse(clean(s));
		}
		catch (FormatException e)
		{
		  log.warn(" key {} name '{}' has bad reference='{}'", fxy, name, s);
		}

		try
		{
		  s = bufrElem.getChildTextNormalize("BUFR_width");
		  width = int.Parse(clean(s));
		}
		catch (FormatException e)
		{
		  log.warn(" key {} name '{}' has bad width='{}'", fxy, name, s);
		}

		b.addDescriptor((short) x, (short) y, scale, reference, width, name, units, null);
	  }
	}

  }

  ///////////////////////////////////////////////////////

  private static TableD latestWmoD;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized TableD getWmoTableDlatest()
  public static TableD WmoTableDlatest
  {
	  get
	  {
		  lock (typeof(BufrTables))
		  {
			if (latestWmoD == null)
			{
			  try
			  {
				latestWmoD = getWmoTableD(latestVersion);
			  }
			  catch (IOException ioe)
			  {
				log.error("Cant open latest WMO ", ioe);
				throw new Exception(ioe.Message, ioe);
			  }
			}
			return latestWmoD;
		  }
	  }
  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static TableD getWmoTableD(int masterTableVersion) throws IOException
  public static TableD getWmoTableD(int masterTableVersion)
  {
	TableConfig tc = matchTableConfig(0, 0, masterTableVersion, 0, -1);
	if (tc != null)
	{
	  return readTableD(tc.tableDname, tc.tableDformat.Value, false);
	}
	return null;
  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static TableD readTableD(String location, Format format, boolean force) throws IOException
  public static TableD readTableD(string location, Format format, bool force)
  {
	if (location == null)
	{
	  return null;
	}
	if (location.Trim().Length == 0)
	{
	  return null;
	}

	if (!force)
	{
	  tablesD.TryGetValue(location, out TableD tb);
	  if (tb != null)
	  {
		return tb;
	  }
	}

	if (showTables)
	{
	  Console.Write("Read BufrTable D {0} format={1}" + Environment.NewLine, location, format);
	}
	TableD d = new TableD(location, location);

	using (Stream ios = openStream(location))
	{
	  switch (format)
	  {
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.cypher:
		  readCypherTableD(ios, d);
		  break;
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.embed:
		  d = readEmbeddedTableD(location);
		  break;
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.ncep:
		  readNcepTableD(ios, d);
		  break;
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.ncep_nm:
		  Tables t = new Tables(null, d, null);
		  NcepMnemonic.read(ios, t);
		  break;
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.ecmwf:
		  readEcmwfTableD(ios, d);
		  break;
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.mel_bufr:
		  readMelbufrTableD(ios, d);
		  break;
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.opera:
		  readOperaTableD(ios, d);
		  break;
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.wmo_csv:
		  readWmoCsvTableD(ios, d);
		  break;
		case NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.Format.wmo_xml:
		  WmoXmlReader.readWmoXmlTableD(ios, d);
		  break;
		default:
		  log.warn("Unknown format= {}", format);
		  return null;
	  }
	}

	if (d != null)
	{
	  tablesD[location] = d; // assume we would get the same table in any thread, so race condition is ok
	}
	return d;
  }

  /*
   * #
   * <A HREF="#C40"> 40</A>
   * </TT></P>
   * <PRE></PRE>
   * <HR>
   * <A NAME="C00"></A>
   * <H4 ALIGN="CENTER">Class 00</H4>
   * <PRE>
   * #
   * 300002
   * 000002 Table A category, line 1
   * 000003 Table A category, line 2
   * #
   * 300003
   * 000010 F, part descriptor
   * 000011 X, part descriptor
   * 000012 Y, part descriptor
   * #
   * 300004
   * 300003
   * 000013 Element name, line 1
   * 000014 Element name, line 2
   * 000015 Units name
   * 000016 Units scale sign
   * 000017 Units scale
   * 000018 Units reference sign
   * 000019 Units reference value
   * 000020 Element data width
   */
  private static void readCypherTableD(Stream ios, TableD t)
  {
	TableD.Descriptor currDesc = null;
	bool startMode = false;

	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.Length == 0)
	  {
		continue;
	  }
	  if (line.StartsWith("<", StringComparison.Ordinal))
	  {
		continue;
	  }

	  if (line.StartsWith("#", StringComparison.Ordinal))
	  {
		startMode = true;
		continue;
	  }

	  if (startMode)
	  {
		try
		{
		  string[] flds = StringUtil2.splitString(line);
		  int fxy = int.Parse(flds[0]);
		  int y = fxy % 1000;
		  fxy /= 1000;
		  int x = fxy % 100;
		  int f1 = fxy / 100;
		  if (f1 != 3)
		  {
			log.error("Bad table " + t.Name + " entry=<" + line + ">");
		  }
		  else
		  {
			currDesc = t.addDescriptor((short) x, (short) y, "", new List<short>());
		  }
		  startMode = false;
		  continue;

		}
		catch (Exception e)
		{
		  log.warn("Bad table " + t.Name + " line=<" + line + ">", e.Message);
		}
	  }
	  if (currDesc != null)
	  {
		try
		{
		  string[] flds = StringUtil2.splitString(line);
		  string fxys = cleanNumber(flds[0]);
		  int fxy = int.Parse(fxys);
		  int y1 = fxy % 1000;
		  fxy /= 1000;
		  int x1 = fxy % 100;
		  int f1 = fxy / 100;
		  int fxy1 = (f1 << 14) + (x1 << 8) + y1;
		  currDesc.addFeature((short) fxy1);
		}
		catch (Exception e)
		{
		  log.warn("Bad table " + t.Name + " line=<" + line + ">", e.Message);
		}
	  }
	  else
	  {
		log.warn("Bad table " + t.Name + " line=<" + line + ">" + " trying to add feature without descriptor.");
	  }
	}
  }

  static string cleanNumber(string s)
  {
	int pos = s.IndexOf("(");
	if (pos > 0)
	{
	  return s.Substring(0, pos);
	}
	return s;
  }

  /*
   * opera:
   * # Heights of side view
   * 3;13;192; 1;01;000
   * ; ; ; 0;31;001
   * ; ; ; 0;10;007
   * # 4 bit per pixel radar images (top view)
   * 3;21;192; 1;10;000
   * ...
   */
  private static void readOperaTableD(Stream ios, TableD t)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);

	TableD.Descriptor currDesc = null;

	string name = null;
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  line = line.Trim();
	  if (line.Length == 0)
	  {
		continue;
	  }
	  if (line.StartsWith("#", StringComparison.Ordinal))
	  {
		name = line.Substring(2).Trim();
		continue;
	  }

	  try
	  {
		string[] flds = line.Split(";");
		if (flds[0].Trim().Length != 0)
		{
		  int x = int.Parse(flds[1].Trim());
		  int y = int.Parse(flds[2].Trim());
		  currDesc = t.addDescriptor((short) x, (short) y, name, new List<short>());
		}

		if (currDesc != null)
		{
		  int f1 = int.Parse(flds[3].Trim());
		  int x1 = int.Parse(flds[4].Trim());
		  int y1 = int.Parse(flds[5].Trim());
		  int fxy = (f1 << 14) + (x1 << 8) + y1;
		  currDesc.addFeature((short) fxy);
		}
		else
		{
		  throw new Exception("Trying to add feature to null descriptor");
		}

	  }
	  catch (Exception e)
	  {
		log.error("Bad table " + t.Name + " entry=<" + line + ">", e);
	  }
	}
  }


  private static void readWmoCsvTableD(Stream ios, TableD tableD)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
	int count = 0;
	int currSeqno = -1;
	TableD.Descriptor currDesc = null;

	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.StartsWith("#", StringComparison.Ordinal))
	  {
		continue;
	  }
	  count++;

	  if (count == 1)
	  {
		continue;
	  }

	  // commas embedded in quotes - replace with blanks for now
	  int pos1 = line.IndexOf('"');
	  if (pos1 >= 0)
	  {
		int pos2 = line.IndexOf('"', pos1 + 1);
		StringBuilder sb = new StringBuilder(line);
		for (int i = pos1; i < pos2; i++)
		{
		  if (sb[i] == ',')
		  {
			sb[i] = ' ';
		  }
		}
		line = sb.ToString();
	  }

	  string[] flds = line.Split(",");
	  if (flds.Length < 5)
	  {
		if (showReadErrs)
		{
		  Console.Write("{0:D} INCOMPLETE line == {1}" + Environment.NewLine, count, line);
		}
		continue;
	  }

	  int fldidx = 2; // skip sno and cat
	  try
	  {
		int seq = int.Parse(flds[fldidx++]);
		string seqName = flds[fldidx++];
		string featno = flds[fldidx++].Trim();
		if (featno.Length == 0)
		{
		  if (showReadErrs)
		  {
			Console.Write("{0:D} no FXY2 specified; line == {1}" + Environment.NewLine, count, line);
		  }
		  continue;
		}

		if (currSeqno != seq)
		{
		  int yseq = seq % 1000;
		  int wseq = seq / 1000;
		  int xseq = wseq % 100;
		  seqName = StringUtil2.remove(seqName, '"');
		  currDesc = tableD.addDescriptor((short) xseq, (short) yseq, seqName, new List<short>());
		  currSeqno = seq;
		}

		int fno = int.Parse(featno);
		int y = fno % 1000;
		int w = fno / 1000;
		int x = w % 100;
		int f = w / 100;

		int fxy = (f << 14) + (x << 8) + y;
		if (currDesc != null)
		{
		  currDesc.addFeature((short) fxy);
		}
		else
		{
		  log.error("Trying to add feature to null desc!");
		}

	  }
	  catch (Exception e)
	  {
		if (showReadErrs)
		{
		  Console.Write("{0:D} {1:D} BAD line == {2} : {3}" + Environment.NewLine, count, fldidx, line, e.Message);
		}
	  }
	}
  }

  private static readonly Pattern threeInts = Pattern.compile("^\\s*(\\d+)\\s+(\\d+)\\s+(\\d+)"); // get 3 integers from beginning of line
  private static readonly Pattern negOne = Pattern.compile("^\\s*-1"); // check for -1 sequence terminator

  private static void readMelbufrTableD(Stream ios, TableD t)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
	int count = 0;

	// read table D to store sequences and their descriptors
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  count++;
	  // check for comment lines
	  if (line.StartsWith("#", StringComparison.Ordinal) || line.Length == 0)
	  {
		continue;
	  }

	  line = line.Trim();
	  string[] split = Pattern.compile("[ \\t]+").split(line); // 1 or more whitespace
	  if (split.Length < 3)
	  {
		continue;
	  }
	  if (split[0].Equals("END"))
	  {
		break;
	  }

	  try
	  {
		short seqF = short.Parse(split[0]);
		short seqX = short.Parse(split[1]);
		short seqY = short.Parse(split[2]);
		Debug.Assert(seqF == 3);

		string seqName = "";
		if (split.Length > 3)
		{
		  StringBuilder sb = new StringBuilder(40);
		  for (int i = 3; i < split.Length; i++)
		  {
			sb.Append(split[i]).Append(" ");
		  }
		  seqName = sb.ToString();
		  seqName = StringUtil2.remove(seqName, "()");
		}

		List<short> seq = new List<short>();
		// look for descriptors within sequence terminated by -1
		while (true)
		{
		  line = dataIS.ReadLine();
		  if (string.ReferenceEquals(line, null))
		  {
			break;
		  }
		  count++;
		  // check for comment lines
		  if (line.StartsWith("#", StringComparison.Ordinal) || line.Length == 0)
		  {
			continue;
		  }

		  Matcher m = threeInts.matcher(line);
		  // descriptor found
		  if (m.find())
		  {
			short f = short.Parse(m.group(1));
			short x = short.Parse(m.group(2));
			short y = short.Parse(m.group(3));
			seq.Add(Descriptor.getFxy(f, x, y));

		  }
		  else
		  {
			m = negOne.matcher(line);
			if (m.find())
			{
			  // store this sequence
			  t.addDescriptor(seqX, seqY, seqName, seq);
			  break;
			}
		  }
		}
	  }
	  catch (Exception e)
	  {
		log.warn("TableD " + t.Name + " Failed on line " + count + " = " + line + "\n " + e);
		// e.printStackTrace();
	  }
	}
  }

  /*
   * 3-00-010 | DELAYREP ; ; Table D sequence definition
   * | 3-00-003 > | Table D descriptor to be defined
   * | 1-01-000 > | Delayed replication of 1 descriptor
   * | 0-31-001 > | Delayed descriptor replication factor
   * | 0-00-030 | Descriptor defining sequence
   *
   * 3-01-001 | WMOBLKST ; ;
   * | 0-01-001 > | WMO block number
   * | 0-01-002 | WMO station number
   */
  private static void readNcepTableD(Stream ios, TableD t)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);

	dataIS.ReadLine(); // throw first line away

	TableD.Descriptor currDesc = null;

	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  if (line.StartsWith("#", StringComparison.Ordinal) || line.Trim().Length == 0)
	  {
		continue;
	  }

	  try
	  {
		string[] flds = Pattern.compile("[\\|;]").split(line);
		if (flds[0].Equals("END"))
		{
		  break;
		}

		string fxys = flds[0].Trim();

		if (fxys.Length != 0)
		{
		  string[] xyflds = fxys.Split("-");
		  short x = short.Parse(clean(xyflds[1]));
		  short y = short.Parse(clean(xyflds[2]));
		  string seqName = (flds.Length > 3) ? flds[3].Trim() : "";
		  currDesc = t.addDescriptor(x, y, seqName, new List<short>());
		}
		else if (currDesc != null)
		{
		  fxys = StringUtil2.remove(flds[1], '>');
		  string[] xyflds = fxys.Split("-");
		  short f = short.Parse(clean(xyflds[0]));
		  short x = short.Parse(clean(xyflds[1]));
		  short y = short.Parse(clean(xyflds[2]));
		  int fxy = (f << 14) + (x << 8) + y;
		  currDesc.addFeature((short) fxy);
		}

	  }
	  catch (Exception e)
	  {
		log.error("Bad table " + t.Name + " entry=<" + line + ">", e);
	  }

	}
  }

  /*
   * 300002 2 000002
   * 000003
   * 300003 3 000010
   * 000011
   * 000012
   */
  private static void readEcmwfTableD(Stream ios, TableD t)
  {
	StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);

	TableD.Descriptor currDesc = null;

	int n = 0;
	while (true)
	{
	  string line = dataIS.ReadLine();
	  if (string.ReferenceEquals(line, null))
	  {
		break;
	  }
	  line = line.Trim();
	  if (line.StartsWith("#", StringComparison.Ordinal) || line.Length == 0)
	  {
		continue;
	  }

	  try
	  {
		string fxys;
		// Need to do fixed-width parsing since, spaces can disappear (if
		// middle number is triple digits, it can end up right against the
		// first number
		if (line.Length > 6)
		{
		  string[] flds = {line.Substring(0, 6), line.Substring(6, 3), line.Substring(9)};
		  fxys = flds[0].Trim();
		  int fxy0 = int.Parse(fxys);
		  int y0 = fxy0 % 1000;
		  fxy0 /= 1000;
		  int x0 = fxy0 % 100;
		  currDesc = t.addDescriptor((short) x0, (short) y0, "", new List<short>());
		  n = int.Parse(flds[1].Trim());
		  fxys = flds[2].Trim();
		}
		else
		{
		  fxys = line;
		}

		int fxy = int.Parse(fxys);
		int y = fxy % 1000;
		fxy /= 1000;
		int x = fxy % 100;
		fxy /= 100;
		fxy = (fxy << 14) + (x << 8) + y;
		if (currDesc != null)
		{
		  currDesc.addFeature((short) fxy);
		}
		n--;

	  }
	  catch (Exception e)
	  {
		log.error("Bad table " + t.Name + " entry=<" + line + ">", e);
	  }
	}
  }

  /////////////////////////////////////////////////////////////////////////////////////////

  internal static Stream openStream(string location)
  {
	Stream ios;

	if (location.StartsWith("resource:", StringComparison.Ordinal))
	{
	  location = location.Substring(9);
	  if (location.StartsWith(RESOURCE_PATH, StringComparison.Ordinal))
	  {
		// the BUFR tables are embedded as one compressed zip (see NetcdfSharp.Bufr.csproj
		// PackBufrTables + tools/bufr-pack-resources.py); decompress the requested entry
		return openTableFromArchive(location.Substring(RESOURCE_PATH.Length));
	  }
	  ios = typeof(BufrTables).Assembly.GetManifestResourceStream(location);
	  if (ios == null)
	  {
		throw new Exception("resource not found=<" + location + ">");
	  }
	  return ios;
	}

	ios = new FileStream(location, FileMode.Open, FileAccess.Read);
	return ios;
  }

  private const string archiveResource = "/resources/bufrTables.zip";
  private static byte[] archiveBytes; // full zip, loaded lazily and kept in memory
  private static readonly object archiveLock = new object();

  // decompress a single entry of the embedded zip; the caller consumes and disposes the stream
  private static Stream openTableFromArchive(string entryName)
  {
	byte[] zip = archiveBytes;
	if (zip == null)
	{
	  lock (archiveLock)
	  {
		if (archiveBytes == null)
		{
		  using (Stream zs = typeof(BufrTables).Assembly.GetManifestResourceStream(archiveResource))
		  {
			if (zs == null)
			{
			  throw new Exception("resource not found=<" + archiveResource + ">");
			}
			using (MemoryStream ms = new MemoryStream())
			{
			  zs.CopyTo(ms);
			  archiveBytes = ms.ToArray();
			}
		  }
		}
		zip = archiveBytes;
	  }
	}

	using (MemoryStream ms = new MemoryStream(zip, false))
	using (ZipArchive za = new ZipArchive(ms, ZipArchiveMode.Read))
	{
	  ZipArchiveEntry entry = za.GetEntry(entryName);
	  if (entry == null)
	  {
		throw new Exception("resource not found in archive=<" + entryName + ">");
	  }
	  using (Stream es = entry.Open())
	  {
		MemoryStream result = new MemoryStream();
		es.CopyTo(result);
		result.Position = 0;
		return result;
	  }
	}
  }

  }

}
