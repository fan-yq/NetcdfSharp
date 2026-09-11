// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/*
 * Copyright (c) 2019-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using Util = NetcdfSharp.ucar.nc2.wmo.Util;

	/// <summary>
	/// Read and manage the WMO GRIB2 Code, Flag, and Parameter tables, in their standard XML format
	/// </summary>
	public class WmoCodeFlagTables
	{

	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(WmoCodeFlagTables));
	  public static readonly GribVersion standard = GribVersion.GRIB2_37;

	  public sealed class GribVersion
	  {
		public static readonly GribVersion GRIB2_37 = new GribVersion("GRIB2_37", InnerEnum.GRIB2_37);

		private static readonly List<GribVersion> valueList = new List<GribVersion>();

		static GribVersion()
		{
			valueList.Add(GRIB2_37);
		}

		public enum InnerEnum
		{
			GRIB2_37
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private GribVersion(string name, InnerEnum innerEnum)
		{
			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public string ResourceName
		{
			get
			{
			  return "/resources/grib2/wmo/" + this.nameValue + "_CodeFlag.xml";
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable String[] getElemNames()
		internal string[] ElemNames
		{
			get
			{
			  if (this == GRIB2_37)
			  {
				return new string[] {"GRIB2_CodeFlag_en", "Title_en", "SubTitle_en", "MeaningParameterDescription_en", "UnitComments_en"};
			  }

			  return null;
			}
		}

		  public static GribVersion[] values()
		  {
			  return valueList.ToArray();
		  }

		  public int ordinal()
		  {
			  return ordinalValue;
		  }

		  public override string ToString()
		  {
			  return nameValue;
		  }

		  public static GribVersion valueOf(string name)
		  {
			  foreach (GribVersion enumInstance in GribVersion.valueList)
			  {
				  if (enumInstance.nameValue == name)
				  {
					  return enumInstance;
				  }
			  }
			  throw new ArgumentException(name);
		  }
	  }

	  public enum TableType
	  {
		param,
		code,
		flag,
		cat
	  }

	  private static WmoCodeFlagTables instance;

	  public static WmoCodeFlagTables Instance
	  {
		  get
		  {
			  lock (typeof(WmoCodeFlagTables))
			  {
				if (instance == null)
				{
				  instance = new WmoCodeFlagTables();
				  try
				  {
					instance.readGribCodes(standard);
				  }
				  catch (IOException e)
				  {
					logger.error("Cant read WMO Grib2 tables");
					throw new Exception(e.Message, e);
				  }
				}
				return instance;
			  }
		  }
	  }

	  /////////////////////////////////////////

	  private ImmutableList<WmoTable> wmoTables;
	  private ImmutableMap<string, WmoTable> wmoTableMap;

	  public virtual ImmutableList<WmoTable> WmoTables
	  {
		  get
		  {
			return wmoTables;
		  }
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public TableType getTableType(String tableName)
	  public virtual TableType? getTableType(string tableName)
	  {
		WmoTable wmoTable = wmoTableMap.get(tableName);
		if (wmoTable == null)
		{
		  return null;
		}
		return wmoTable.Type;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public WmoCodeTable getCodeTable(String tableName)
	  public virtual WmoCodeTable getCodeTable(string tableName)
	  {
		WmoTable wmoTable = wmoTableMap.get(tableName);
		if (wmoTable == null)
		{
		  return null;
		}
		return new WmoCodeTable(wmoTable);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public WmoCodeTable getCodeTable(int m1, int m2)
	  public virtual WmoCodeTable getCodeTable(int m1, int m2)
	  {
		string name = string.Format("{0:D}.{1:D}", m1, m2);
		WmoTable wmoTable = wmoTableMap.get(name);
		if (wmoTable == null)
		{
		  return null;
		}
		return new WmoCodeTable(wmoTable);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public WmoParamTable getParamTable(int discipline, int category)
	  public virtual WmoParamTable getParamTable(int discipline, int category)
	  {
		string name = string.Format("4.2.{0:D}.{1:D}", discipline, category);
		WmoTable wmoTable = wmoTableMap.get(name);
		if (wmoTable == null)
		{
		  return null;
		}
		return new WmoParamTable(wmoTable);
	  }

	  /*
	   * Param Table:
	   * <GRIB2_CodeFlag_exp_en>
	   * <No>524</No>
	   * <Title_en>Code table 4.2 - Parameter number by product discipline and parameter category</Title_en>
	   * <SubTitle_en>Product discipline 0 - Meteorological products, parameter category 1: moisture</SubTitle_en>
	   * <CodeFlag>101</CodeFlag>
	   * <MeaningParameterDescription_en>Specific number concentration of snow</MeaningParameterDescription_en>
	   * <UnitComments_en>kg-1</UnitComments_en>
	   * <ElementDescription_en>Number of particles per unit mass of air</ElementDescription_en>
	   * <Status>Operational</Status>
	   * </GRIB2_CodeFlag_exp_en>
	   * 
	   * Code Table:
	   * <GRIB2_CodeFlag_exp_en>
	   * <No>2</No>
	   * <Title_en>Code table 0.0 - Discipline of processed data in the GRIB message, number of GRIB Master table</Title_en>
	   * <CodeFlag>1</CodeFlag>
	   * <MeaningParameterDescription_en>Hydrological products</MeaningParameterDescription_en>
	   * <Status>Operational</Status>
	   * </GRIB2_CodeFlag_exp_en>
	   * 
	   * FlagTable:
	   * <GRIB2_CodeFlag_exp_en>
	   * <No>168</No>
	   * <Title_en>Flag table 3.4 - Scanning mode</Title_en>
	   * <CodeFlag>1</CodeFlag>
	   * <Value>0</Value>
	   * <MeaningParameterDescription_en>Points of first row or column scan in the +i (+x)
	   * direction</MeaningParameterDescription_en>
	   * <Status>Operational</Status>
	   * </GRIB2_CodeFlag_exp_en>
	   * <GRIB2_CodeFlag_exp_en>
	   * <No>169</No>
	   * <Title_en>Flag table 3.4 - Scanning mode</Title_en>
	   * <CodeFlag>1</CodeFlag>
	   * <Value>1</Value>
	   * <MeaningParameterDescription_en>Points of first row or column scan in the -i (-x)
	   * direction</MeaningParameterDescription_en>
	   * <Status>Operational</Status>
	   * </GRIB2_CodeFlag_exp_en>
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void readGribCodes(Version version) throws NetcdfSharp.java.io.IOException
	  private void readGribCodes(GribVersion version)
	  {
		string[] elems = version.ElemNames;
		if (elems == null)
		{
		  throw new System.InvalidOperationException("unknown version = " + version);
		}

		using (Stream ios = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStream(version.ResourceName))
		{
		  if (ios == null)
		  {
			logger.error("cant open WmoCodeTable=" + version.ResourceName);
			throw new IOException("cant open WmoCodeTable=" + version.ResourceName);
		  }

		  NetcdfSharp.org.jdom2.Document doc;
		  try
		  {
			SAXBuilder saxBuilder = new SAXBuilder();
			saxBuilder.setExpandEntities(false);
			doc = saxBuilder.build(ios);
		  }
		  catch (JDOMException e)
		  {
			throw new IOException(e.Message);
		  }
		  Element root = doc.getRootElement();

		  IDictionary<string, WmoTable> map = new Dictionary<string, WmoTable>();

		  IList<Element> featList = root.getChildren(elems[0]); // main element
		  int line = 0;
		  foreach (Element elem in featList)
		  {
			string tableName = elem.getChildTextNormalize(elems[1]); // Title_en
			Element subtableElem = elem.getChild(elems[2]); // "SubTitle_en"

			TableType type;
			if (tableName.StartsWith("Code table 4.1 ", StringComparison.Ordinal))
			{
			  type = TableType.cat;
			}
			else if (tableName.StartsWith("Code table 4.2 ", StringComparison.Ordinal))
			{
			  type = TableType.param;
			}
			else if (tableName.StartsWith("Flag", StringComparison.Ordinal))
			{
			  type = TableType.flag;
			}
			else if (tableName.StartsWith("Code", StringComparison.Ordinal))
			{
			  type = TableType.code;
			}
			else
			{
			  logger.warn("Unknown wmo table entry = '{}'", tableName);
			  continue;
			}

			if (subtableElem != null && !string.IsNullOrEmpty(subtableElem.getTextNormalize()))
			{
			  tableName = subtableElem.getTextNormalize();
			}

			TableType finalType = type;
			if (!map.TryGetValue(tableName, out WmoTable wmoTable))
			{
			  wmoTable = new WmoTable(tableName, finalType);
			  map[tableName] = wmoTable;
			}

			string code = elem.getChildTextNormalize("CodeFlag");
			string value = elem.getChildTextNormalize("Value"); // Flag table only
			string meaning = elem.getChildTextNormalize(elems[3]); // MeaningParameterDescription_en

			Element unitElem = elem.getChild(elems[4]); // "UnitComments_en"
			string unit = (unitElem == null) ? null : unitElem.getTextNormalize();

			Element statusElem = elem.getChild("Status");
			string status = (statusElem == null) ? null : statusElem.getTextNormalize();

			wmoTable.addEntry(line, code, value, meaning, unit, status);
			line++;
		  }
		  ios.Close();

// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
		  this.wmoTables = ImmutableList<WmoTable>.copyOf(map.Values.OrderBy(c => c));
		  ImmutableMap<string, WmoTable>.Builder builder = ImmutableMap<string, WmoTable>.builder();
		  foreach (WmoTable t in map.Values)
		  {
			builder.put(t.Id, t);
		  }
		  this.wmoTableMap = builder.build();
		}
	  }

	  public class WmoTable : IComparable<WmoTable>
	  {

		internal readonly string name;
		internal readonly TableType type;
		internal readonly IList<WmoEntry> entries = new List<WmoEntry>();
		internal readonly string id;
		internal int m1 = -1, m2 = -1, discipline = -1, category = -1;

		internal WmoTable(string name, TableType type)
		{
		  this.name = name;
		  this.type = type;

		  // extract the table id from the name.
		  if (type == TableType.cat)
		  {
			m1 = 4;
			m2 = 1;
			string[] slist = System.Text.RegularExpressions.Regex.Split(name, "[ :]+");
			for (int i = 0; i < slist.Length; i++)
			{
			  if (slist[i].Equals("discipline", StringComparison.OrdinalIgnoreCase))
			  {
				discipline = int.Parse(slist[i + 1]);
			  }
			}
			if (discipline < 0)
			{
			  throw new ArgumentException("Cant extract param id from table " + name);
			}
			this.id = string.Format("{0:D}.{1:D}.{2:D}", m1, m2, discipline);

		  }
		  else if (type == TableType.param)
		  {
			m1 = 4;
			m2 = 2;
			string[] slist = System.Text.RegularExpressions.Regex.Split(name, "[ :]+");
			for (int i = 0; i < slist.Length; i++)
			{
			  if (slist[i].Equals("discipline", StringComparison.OrdinalIgnoreCase))
			  {
				discipline = int.Parse(slist[i + 1]);
			  }
			  if (slist[i].Equals("category", StringComparison.OrdinalIgnoreCase))
			  {
				category = int.Parse(slist[i + 1]);
			  }
			}
			if (discipline < 0 || category < 0)
			{
			  throw new ArgumentException("Cant extract param id from table " + name);
			}
			this.id = string.Format("{0:D}.{1:D}.{2:D}.{3:D}", m1, m2, discipline, category);

		  }
		  else
		  {
			string[] s = System.Text.RegularExpressions.Regex.Split(name, " ");
			string id = s[2];
			string[] slist2 = System.Text.RegularExpressions.Regex.Split(id, "\\.");
			if (slist2.Length == 2)
			{
			  m1 = int.Parse(slist2[0]);
			  m2 = int.Parse(slist2[1]);
			}
			else
			{
			  logger.warn("WmoCodeTable bad= %s%n" + name);
			}
			if (m1 < 0 || m2 < 0)
			{
			  throw new ArgumentException("Cant extract id from table " + name);
			}
			this.id = string.Format("{0:D}.{1:D}", m1, m2);
		  }
		}

		internal virtual WmoEntry addEntry(int line, string code, string value, string meaning, string unit, string status)
		{
		  WmoEntry entry = new WmoEntry(this, line, code, value, meaning, unit, status);
		  bool isRange = (entry.start != entry.stop);
		  if (!isRange)
		  {
			entries.Add(entry);
		  }
		  return entry;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override public int compareTo(@Nonnull WmoTable o)
		public virtual int CompareTo(WmoTable o)
		{
		  if (m1 != o.m1)
		  {
			return m1 - o.m1;
		  }
		  if (m2 != o.m2)
		  {
			return m2 - o.m2;
		  }
		  if (discipline != o.discipline)
		  {
			return discipline - o.discipline;
		  }
		  return category - o.category;
		}

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual TableType Type
		{
			get
			{
			  return type;
			}
		}

		public virtual string Id
		{
			get
			{
			  return id;
			}
		}

		public virtual ImmutableList<WmoEntry> Entries
		{
			get
			{
			  return ImmutableList<WmoEntry>.copyOf(entries);
			}
		}

		public class WmoEntry
		{
			private readonly WmoCodeFlagTables.WmoTable outerInstance;

		  internal readonly int start, stop, line;
		  internal readonly int number, value;
		  internal readonly string code, meaning, name, unit, status;

		  internal WmoEntry(WmoCodeFlagTables.WmoTable outerInstance, int line, string code, string valueS, string meaning, string unit, string status)
		  {
			  this.outerInstance = outerInstance;
			this.line = line;
			this.code = code;
			this.meaning = meaning;
			this.status = status;
			this.name = meaning;

			string unitW = unit;
			int startW, stopW, numberW = 0;

			try
			{
			  int pos = code.IndexOf('-');
			  if (pos > 0)
			  {
				startW = int.Parse(code.Substring(0, pos));
				string stops = code.Substring(pos + 1);
				stopW = int.Parse(stops);
			  }
			  else
			  {
				startW = int.Parse(code);
				stopW = startW;
				numberW = startW;
			  }
			}
			catch (Exception)
			{
			  startW = -1;
			  stopW = 0;
			}
			this.start = startW;
			this.stop = stopW;
			this.number = numberW;

			int valueW = -1;
			if (!string.ReferenceEquals(valueS, null))
			{
			  try
			  {
				valueW = int.Parse(valueS);
			  }
			  catch (Exception)
			  {
				valueW = -2;
			  }
			}
			this.value = valueW;

			if (outerInstance.type == TableType.param)
			{
			  // massage units
			  if (!string.ReferenceEquals(unit, null))
			  {
				unitW = Util.cleanUnit(unit);
			  }
			  else
			  {
				unitW = ""; // no null unit allowed
			  }
			}
			this.unit = unitW;
		  }

		public virtual int Line
		{
			get
			{
			  return line;
			}
		}

		public virtual int Start
		{
			get
			{
			  return start;
			}
		}

		public virtual int Stop
		{
			get
			{
			  return stop;
			}
		}

		public virtual int Number
		{
			get
			{
			  return number;
			}
		}

		public virtual int Value
		{
			get
			{
			  return value;
			}
		}

		public virtual string Code
		{
			get
			{
			  return code;
			}
		}

		public virtual string Meaning
		{
			get
			{
			  return meaning;
			}
		}

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual string Unit
		{
			get
			{
			  return unit;
			}
		}

		public virtual string Status
		{
			get
			{
			  return status;
			}
		}

		public virtual string Id
		{
			get
			{
			  return outerInstance.Id + "." + Number;
			}
		}

		public virtual int Category
		{
			get
			{
			  return outerInstance.category;
			}
		}

		public virtual int Discipline
		{
			get
			{
			  return outerInstance.discipline;
			}
		}
	  }
	}
	}
}
