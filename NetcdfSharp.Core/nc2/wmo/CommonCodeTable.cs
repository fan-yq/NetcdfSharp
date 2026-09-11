// ====================================================================================================
// NetcdfSharp.Core - NetcdfSharp.ucar.nc2.wmo.CommonCodeTable
// Rewritten from Java (netcdf-java). Table rich-enum -> class-based enum; jdom2 XML -> System.Xml.Linq.
// XML resource files are loaded from netcdf-java-maint-5.x source tree (TODO: move to EmbeddedResource).
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NetcdfSharp.ucar.nc2.wmo
{
	/// <summary>
	/// Read and process WMO common code and flag tables.
	/// </summary>
	public class CommonCodeTable : IComparable<CommonCodeTable>
	{
		private static readonly IDictionary<int, CommonCodeTable> tableMap = new Dictionary<int, CommonCodeTable>();
		private const string version = "_20181107_en";
		private static readonly string resourceRoot =
			Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\..\netcdf-java-maint-5.x\cdm\core\src\main\resources\resources\wmo");

		/// <summary>
		/// The WMO common code table definitions. Class-based enum (Java rich-enum).
		/// </summary>
		public sealed class Table
		{
			public static readonly Table C1 = new Table("C1", "Centers-GRIB1,BUFR3", 1, 1,
				new string[] { "CodeFigureForF1F2", "CodeFigureForF3F3F3", "OriginatingGeneratingCentres_en" });
			public static readonly Table C3 = new Table("C3", "Water temperature profile instrument", 3, 1,
				new string[] { "CodeFigureForBUFR", null, "InstrumentMakeAndType_en" });
			public static readonly Table C4 = new Table("C4", "Water temperature profile recorder", 4, 1,
				new string[] { "CodeFigureForBUFR", null, "Meaning_en" });
			public static readonly Table C5 = new Table("C5", "Satellite identifier", 5, 1,
				new string[] { "CodeFigureForBUFR", null, "SatelliteName_en" });
			public static readonly Table C7 = new Table("C7", "Satellite tracking", 7, 1,
				new string[] { "CodeFigureForBUFR", null, "TrackingTechniquesStatusOfSystemUsed_en" });
			public static readonly Table C8 = new Table("C8", "Satellite instruments", 8, 1,
				new string[] { "Code", null, "InstrumentLongName_en", "InstrumentShortName_en" });
			public static readonly Table C11 = new Table("C11", "Centers-GRIB2,BUFR4", 11, 1,
				new string[] { "GRIB2_BUFR4", null, "OriginatingGeneratingCentre_en" });
			public static readonly Table C12 = new Table("C12", "Subcenters", 12, 2,
				new string[] { "CodeFigure_OriginatingCentres", "Name_OriginatingCentres_en", "CodeFigure_SubCentres", "Name_SubCentres_en" });
			public static readonly Table C13 = new Table("C13", "Data sub-categories", 13, 2,
				new string[] { "CodeFigure_DataCategories", "Name_DataCategories_en", "CodeFigure_InternationalDataSubcategories", "Name_InternationalDataSubcategories_en" });
			public static readonly Table C14 = new Table("C14", "Atmospheric chemical or physical constituent type", 14, 2,
				new string[] { "CodeFigure", "ChemicalFormula", null, "Meaning_en" });

			private static readonly IList<Table> valueList = new List<Table>
			{
				C1, C3, C4, C5, C7, C8, C11, C12, C13, C14
			};

			public string name;
			public string[] elems; // type 1: 0 = code, 1 = code2 (may be null), 2 = value
									// type 2: 0 = code1, 1 = comment, 2 = code3 (may be null), 3 = value
			public int num;
			public int type; // type 1 = 1 code, type 2 = 2 codes
			public string num0;

			private Table(string name, string displayName, int num, int type, string[] elems)
			{
				this.name = displayName;
				this.num = num;
				this.type = type;
				this.elems = elems;
				this.num0 = num.ToString("D2");
			}

			public string getResourceName() => "/resources/wmo/Common_C" + this.num0 + version + ".xml";

			public string getRootElemName() => "Common_C" + this.num0 + version;

			public int getTableNo() => num;

			public int getTableType() => type;

			public string[] getElemNames() => elems;

			public string getName() => name;

			public static Table[] values() => valueList.ToArray();

			public static Table valueOf(string name)
			{
				foreach (Table t in valueList)
				{
					if (t.name.Equals(name)) return t;
				}
				throw new ArgumentException(name);
			}

			public static Table find(int tableNo)
			{
				foreach (Table t in valueList)
				{
					if (t.num == tableNo) return t;
				}
				return null;
			}

			public override string ToString() => name;
		}

		/// <summary>
		/// Center name, from table C-1 or C-11.
		/// </summary>
		public static string getCenterName(int center_id, int edition)
		{
			string result = (edition == 1) ? getTableValue(1, center_id) : getTableValue(11, center_id);
			if (!string.ReferenceEquals(result, null))
			{
				return result;
			}
			if (center_id == 0)
			{
				return "WMO standard table";
			}
			return "Unknown center=" + center_id;
		}

		/// <summary>
		/// Center name, from table C-1 or C-11 (BUFR edition).
		/// </summary>
		public static string getCenterNameBufr(int center_id, int edition)
		{
			string result = (edition < 4) ? getTableValue(1, center_id) : getTableValue(11, center_id);
			if (!string.ReferenceEquals(result, null))
			{
				return result;
			}
			if (center_id == 0)
			{
				return "WMO standard table";
			}
			return "Unknown center=" + center_id;
		}

		/// <summary>
		/// Subcenter name, from table C-12.
		/// </summary>
		public static string getSubCenterName(int center_id, int subcenter_id)
		{
			return getTableValue(12, center_id, subcenter_id);
		}

		/// <summary>
		/// Data subcategory name, from table C-13.
		/// </summary>
		public static string getDataSubcategoy(int cat, int subcat)
		{
			return getTableValue(13, cat, subcat);
		}

		public static CommonCodeTable getTable(int tableNo)
		{
			if (tableMap.TryGetValue(tableNo, out CommonCodeTable cct))
			{
				return cct;
			}

			Table want = Table.find(tableNo);
			if (want == null)
			{
				throw new InvalidOperationException("Unknown wmo common code table number= " + tableNo);
			}

			cct = readCommonCodes(want);
			tableMap[tableNo] = cct;
			return cct;
		}

		public static string getTableValue(int tableNo, int code)
		{
			CommonCodeTable cct = getTable(tableNo);
			TableEntry te = cct.get(code);
			if (te == null)
			{
				return null;
			}
			return te.value;
		}

		public static string getTableValue(int tableNo, int code, int code2)
		{
			CommonCodeTable cct = getTable(tableNo);
			TableEntry te = cct.get(code, code2);
			if (te == null)
			{
				return null;
			}
			return te.value;
		}

		private static CommonCodeTable readCommonCodes(Table table)
		{
			string fileName = "Common_C" + table.num0 + version + ".xml";
			string path = Path.Combine(resourceRoot, fileName);
			if (!System.IO.File.Exists(path))
			{
				return new CommonCodeTable(table.name, table.getTableType());
			}

			XDocument doc;
			try
			{
				doc = XDocument.Load(path);
			}
			catch (Exception)
			{
				return new CommonCodeTable(table.name, table.getTableType());
			}

			XElement root = doc.Root;
			string previousValue = null;
			CommonCodeTable ct = new CommonCodeTable(table.name, table.getTableType());
			string[] elems = table.getElemNames();

			foreach (XElement elem in root.Elements(table.getRootElemName()))
			{
				if (table.type == 1)
				{
					string line = ChildTextNormalize(elem, "No");
					string code = ChildTextNormalize(elem, elems[0]);
					string code2 = (elems.Length > 1 && elems[1] != null) ? ChildTextNormalize(elem, elems[1]) : null;
					string value = ChildTextNormalize(elem, elems[2]);
					XElement statusElem = elem.Element("Status");
					string status = (statusElem == null) ? null : statusElem.Value.Trim();
					if (")".Equals(value))
					{
						value = previousValue;
					}
					ct.add(line, code, code2, value, status);
					previousValue = value;
				}
				else
				{
					string line = ChildTextNormalize(elem, "No");
					string code = ChildTextNormalize(elem, elems[0]);
					string value = ChildTextNormalize(elem, elems[1]);
					string code2 = (elems.Length > 2 && elems[2] == null) ? null : ChildTextNormalize(elem, elems[2]);
					string value2 = ChildTextNormalize(elem, elems[3]);
					XElement statusElem = elem.Element("Status");
					string status = (statusElem == null) ? null : statusElem.Value.Trim();
					ct.add(line, code, value, code2, value2, status);
				}
			}
			return ct;
		}

		private static string ChildTextNormalize(XElement parent, string childName)
		{
			if (string.IsNullOrEmpty(childName)) return null;
			XElement child = parent.Element(childName);
			if (child == null) return null;
			return string.Join(" ", child.Value.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
		}

		// //////////////////////////////////////////////////////////////////////////

		public readonly string tableName;
		public readonly int type;
		public readonly IList<TableEntry> entries = new List<TableEntry>();
		private IDictionary<int, string> map;

		internal CommonCodeTable(string name, int type)
		{
			this.tableName = name;
			this.type = type;
		}

		internal void add(string line, string code, string code2, string value, string status)
		{
			entries.Add(new TableEntry(this, line, code, code2, value, status));
		}

		internal void add(string line, string code, string value, string code2, string value2, string status)
		{
			entries.Add(new TableEntry(this, line, code, value, code2, value2, status));
		}

		internal TableEntry get(int code)
		{
			foreach (TableEntry p in entries)
			{
				if (p.code == code)
				{
					return p;
				}
			}
			return null;
		}

		internal TableEntry get(int code, int code2)
		{
			foreach (TableEntry p in entries)
			{
				if ((p.code == code) && (p.code2 == code2))
				{
					return p;
				}
			}
			return null;
		}

		public string getTableName()
		{
			return tableName;
		}

		public IDictionary<int, string> getMap()
		{
			if (map == null)
			{
				map = new Dictionary<int, string>(entries.Count * 2);
				foreach (TableEntry p in entries)
				{
					map[p.code] = p.value;
				}
			}
			return map;
		}

		public int CompareTo(CommonCodeTable o)
		{
			return tableName.CompareTo(o.tableName);
		}

		public class TableEntry : IComparable<TableEntry>
		{
			private readonly CommonCodeTable _outer;

			public int line, code, code2;
			public string value, status, comment;

			internal TableEntry(CommonCodeTable outer, string line, string code1, string code2, string value, string status)
			{
				this._outer = outer;
				this.line = int.Parse(line);
				this.code = parse(code1);
				if (this.code < 0)
				{
					this.code = parse(code2);
				}
				if (this.code < 1)
				{
					this.comment = value;
				}
				else
				{
					this.value = _outer.filter(value);
					this.status = status;
				}
			}

			internal TableEntry(CommonCodeTable outer, string line, string code1, string value1, string code2, string value2, string status)
			{
				this._outer = outer;
				this.line = int.Parse(line);
				this.code = parse(code1);
				this.code2 = parse(code2);
				this.value = _outer.filter(value2);
				this.comment = value1;
				this.status = status;
			}

			internal static int parse(string s)
			{
				if (s == null)
				{
					return -1;
				}
				if (int.TryParse(s, out int result))
				{
					return result;
				}
				return -2;
			}

			public int CompareTo(TableEntry o)
			{
				if (_outer.type == 1)
				{
					return code.CompareTo(o.code);
				}
				else
				{
					int diff = code.CompareTo(o.code);
					return (diff == 0) ? code2.CompareTo(o.code2) : diff;
				}
			}

			public override string ToString()
			{
				if (comment != null)
				{
					return "TableEntry{, line=" + line + ", comment=" + comment + "}";
				}
				else
				{
					return "TableEntry{, line=" + line + ", code=" + code + ", value='" + value + "', status='" + status + "'}";
				}
			}
		}

		private const char badDash = (char)173;

		private string filter(string s)
		{
			if (s == null)
			{
				return "";
			}
			return NetcdfSharp.ucar.unidata.util.StringUtil2.replace(s, badDash, "-");
		}
	}
}
