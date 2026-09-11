// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;

	/// <summary>
	/// Read and process WMO GRIB templates from standard WMO source file.
	/// 
	/// @author caron
	/// @since Jul 31, 2010
	/// </summary>

	public class WmoTemplateTables
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(WmoTemplateTables));
	  private static readonly IDictionary<string, string> convertMap = new Dictionary<string, string>();
	  static WmoTemplateTables()
	  {
		// gds
		convertMap["Source of Grid Definition (see code table 3.0)"] = "3.0";
		convertMap["Shape of the Earth"] = "3.2";
		convertMap["Interpretation of list of numbers"] = "3.11";

		// pds
		convertMap["Type of generating process"] = "4.3";
		convertMap["Indicator of unit of time range"] = "4.4";
		convertMap["Indicator of unit of time for time range over which statistical processing is done"] = "4.4";
		convertMap["Indicator of unit of time for the increment between the successive fields used"] = "4.4";
		convertMap["Type of first fixed surface"] = "4.5";
		convertMap["Type of second fixed surface"] = "4.5";
		convertMap["Type of ensemble forecast"] = "4.6";
		convertMap["Derived forecast"] = "4.7";
		convertMap["Probability type"] = "4.9";
		convertMap["Statistical process used to calculate the processed field from the field at each time increment during the time range"] = "4.10";
		convertMap["Type of time increment between successive fields used in the statistical processing"] = "4.11";

		convertMap["Analysis or forecast generating process identifier (defined by originating centre)"] = "ProcessId"; // ??
		convertMap["Background generating process identifier (defined by originating centre)"] = "ProcessId";
		convertMap["Forecast generating process identifier (defined by originating centre)"] = "ProcessId";
	  }

	  public static readonly Version standard = Version.GRIB2_37;

	  public sealed class Version
	  {
		public static readonly Version GRIB2_37 = new Version("GRIB2_37", InnerEnum.GRIB2_37);

		private static readonly List<Version> valueList = new List<Version>();

		static Version()
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

		private Version(string name, InnerEnum innerEnum)
		{
			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		internal string ResourceName
		{
			get
			{
			  return "/resources/grib2/wmo/" + this.nameValue + "_Template.xml";
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
				return new string[] {"GRIB2_Template_en", "Title_en", "Note_en", "Contents_en"};
			  }
			  return null;
			}
		}

		  public static Version[] values()
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

		  public static Version valueOf(string name)
		  {
			  foreach (Version enumInstance in Version.valueList)
			  {
				  if (enumInstance.nameValue == name)
				  {
					  return enumInstance;
				  }
			  }
			  throw new ArgumentException(name);
		  }
	  }

	  private static WmoTemplateTables instance;

	  public static WmoTemplateTables Instance
	  {
		  get
		  {
			if (instance == null)
			{
			  instance = new WmoTemplateTables();
			  try
			  {
				instance.readXml(standard);
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

	  /*
	   * <GRIB2_Template_en>
	   * <No>1451</No>
	   * <Title_en>Product definition template 4.55 - spatio-temporal changing tiles at a horizontal level or horizontal
	   * layer at a point in time</Title_en>
	   * <OctetNo>35</OctetNo>
	   * <Contents_en>Type of second fixed surface</Contents_en>
	   * <Note_en>(see Code table 4.5)</Note_en>
	   * <Status>Operational</Status>
	   * </GRIB2_Template_en>
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void readXml(Version version) throws NetcdfSharp.java.io.IOException
	  private void readXml(Version version)
	  {
		using (Stream ios = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStream(version.ResourceName))
		{
		  if (ios == null)
		  {
			throw new IOException("cant open TemplateTable %s " + version.ResourceName);
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

		  IDictionary<string, TemplateTable> map = new Dictionary<string, TemplateTable>();
		  string[] elems = version.ElemNames;
		  Debug.Assert(elems != null);
		  Debug.Assert(elems.Length > 3);

		  Element root = doc.getRootElement();
		  IList<Element> featList = root.getChildren(elems[0]); // 0 = main element
		  foreach (Element elem in featList)
		  {
			string desc = elem.getChildTextNormalize(elems[1]); // 1 = title
			string octet = elem.getChildTextNormalize("OctetNo");
			string content = elem.getChildTextNormalize(elems[3]); // 3 = content
			string status = elem.getChildTextNormalize("Status");
			string note = elem.getChildTextNormalize(elems[2]); // 2 == note

			if (!map.TryGetValue(desc, out TemplateTable template))
			{
			  template = new TemplateTable(this, desc);
			  map[desc] = template;
			}
			template.add(octet, content, status, note);
		  }
		  ios.Close();

		  IList<TemplateTable> tlist = new List<TemplateTable>(map.Values);
		  foreach (TemplateTable t in tlist)
		  {
			if (t.m1 == 3)
			{
			  t.add(1, 4, "GDS length");
			  t.add(5, 1, "Section");
			  t.add(6, 1, "Source of Grid Definition (see code table 3.0)");
			  t.add(7, 4, "object of data points");
			  t.add(11, 1, "object of octects for optional list of numbers");
			  t.add(12, 1, "Interpretation of list of numbers");
			  t.add(13, 2, "Grid Definition Template Number");

			}
			else if (t.m1 == 4)
			{
			  t.add(1, 4, "PDS length");
			  t.add(5, 1, "Section");
			  t.add(6, 2, "object of coordinates values after Template");
			  t.add(8, 2, "Product Definition Template Number");
			}
			t.flds.Sort(Comparer<Field>.Default);
		  }

// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
		  this.templateTables = ImmutableList<TemplateTable>.copyOf(map.Values.OrderBy(c => c));
		  ImmutableMap<string, TemplateTable>.Builder builder = ImmutableMap<string, TemplateTable>.builder();
		  foreach (TemplateTable t in map.Values)
		  {
			builder.put(t.Id, t);
		  }
		  this.templateMap = builder.build();
		}
	  }

	  private ImmutableList<TemplateTable> templateTables;
	  private ImmutableMap<string, TemplateTable> templateMap;

	  public virtual ImmutableList<TemplateTable> TemplateTables
	  {
		  get
		  {
			return templateTables;
		  }
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public TemplateTable getTemplateTable(String key)
	  public virtual TemplateTable getTemplateTable(string key)
	  {
		return templateMap.get(key);
	  }

	  public class TemplateTable : IComparable<TemplateTable>
	  {
		  private readonly WmoTemplateTables outerInstance;

		internal string name, desc;
		internal int m1, m2;
		internal readonly IList<Field> flds = new List<Field>();

		internal TemplateTable(WmoTemplateTables outerInstance, string desc)
		{
			this.outerInstance = outerInstance;
		  this.desc = desc;

		  string[] slist = System.Text.RegularExpressions.Regex.Split(desc, " ");
		  for (int i = 0; i < slist.Length; i++)
		  {
			if (slist[i].Equals("template", StringComparison.OrdinalIgnoreCase))
			{
			  name = slist[i + 1];
			  string[] slist2 = System.Text.RegularExpressions.Regex.Split(name, "\\.");
			  if (slist2.Length == 2)
			  {
				m1 = int.Parse(slist2[0]);
				m2 = int.Parse(slist2[1]);
			  }
			  else
			  {
				logger.warn("TemplateTable bad= {}", name);
			  }
			  break;
			}
		  }
		}

		internal virtual void add(string octet, string content, string status, string note)
		{
		  flds.Add(new Field(octet, content, status, note));
		}

		internal virtual void add(int start, int nbytes, string content)
		{
		  flds.Add(new Field(start, nbytes, content));
		}

		public virtual string Id
		{
			get
			{
			  return m1 + "." + m2;
			}
		}

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual string Desc
		{
			get
			{
			  return desc;
			}
		}

		public virtual int M1
		{
			get
			{
			  return m1;
			}
		}

		public virtual int M2
		{
			get
			{
			  return m2;
			}
		}

		public virtual ImmutableList<Field> Flds
		{
			get
			{
			  return ImmutableList<Field>.copyOf(flds);
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override public int compareTo(@Nonnull TemplateTable o)
		public virtual int CompareTo(TemplateTable o)
		{
		  if (m1 == o.m1)
		  {
			return m2 - o.m2;
		  }
		  else
		  {
			return m1 - o.m1;
		  }
		}

		public virtual void showInfo(Grib2Tables tables, byte[] raw, Formatter f)
		{
		  f.format("%n(%s) %s %n", name, desc);
		  foreach (Field fld in flds)
		  {
			if (fld.start < 0)
			{
			  continue;
			}

			string info = convertMap[fld.content];
			if (string.ReferenceEquals(info, null))
			{
			  f.format("%3d: %90s == %d %n", fld.start, fld.content, fld.value(raw));
			}
			else
			{
			  string desc = convert(tables, info, fld.value(raw));
			  if (string.ReferenceEquals(desc, null))
			  {
				f.format("%3d: %90s == %d (%s) %n", fld.start, fld.content, fld.value(raw), convert(tables, info, fld.value(raw)));
			  }
			  else
			  {
				f.format("%3d: %90s == %d (table %s: %s) %n", fld.start, fld.content, fld.value(raw), info, desc);
			  }
			}
		  }
		}

		internal virtual string convert(Grib2Tables tables, string table, int value)
		{
		  string result = tables.getCodeTableValue(table, value);
		  return (!string.ReferenceEquals(result, null)) ? result : "Table " + table + " code " + value + " not found";
		}
	  }

	  public class Field : IComparable<Field>
	  {
		internal readonly string octet;
		internal readonly string content;
		internal string status;
		internal string note;
		internal int start, nbytes;

		internal Field(string octet, string content, string status, string note)
		{
		  this.octet = octet;
		  this.content = content;
		  this.status = status;
		  this.note = note;

		  try
		  {
			int pos = octet.IndexOf('-');
			if (pos > 0)
			{
			  start = int.Parse(octet.Substring(0, pos));
			  string stops = octet.Substring(pos + 1);
			  try
			  {
				int stop = int.Parse(stops);
				nbytes = stop - start + 1;
			  }
			  catch (Exception e)
			  {
				logger.debug("Error parsing wmo template line=" + content, e);
			  }
			}
			else
			{
			  start = int.Parse(octet);
			  nbytes = 1;
			}
		  }
		  catch (Exception)
		  {
			start = -1;
			nbytes = 0;
		  }
		}

		internal Field(int start, int nbytes, string content)
		{
		  this.start = start;
		  this.nbytes = nbytes;
		  this.content = content;
		  this.octet = start + "-" + (start + nbytes - 1);
		}

		public virtual string Octet
		{
			get
			{
			  return octet;
			}
		}

		public virtual string Content
		{
			get
			{
			  return content;
			}
		}

		public virtual string Status
		{
			get
			{
			  return status;
			}
		}

		public virtual string Note
		{
			get
			{
			  return note;
			}
		}

		public virtual int Start
		{
			get
			{
			  return start;
			}
		}

		public virtual int Nbytes
		{
			get
			{
			  return nbytes;
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override public int compareTo(@Nonnull Field o)
		public virtual int CompareTo(Field o)
		{
		  return start - o.start;
		}

		internal virtual int value(byte[] pds)
		{
		  switch (nbytes)
		  {
			case 1:
			  return get(pds, start);
			case 2:
			  return GribNumbers.int2(get(pds, start), get(pds, start + 1));
			case 4:
			  return GribNumbers.int4(get(pds, start), get(pds, start + 1), get(pds, start + 2), get(pds, start + 3));
			default:
			  return -9999;
		  }
		}

		internal virtual int get(byte[] pds, int offset)
		{
		  return pds[offset - 1] & 0xff;
		}
	  }
	}
}
