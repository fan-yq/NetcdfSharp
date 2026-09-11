// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr.tables
{
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using Element = NetcdfSharp.org.jdom2.Element;
	using Descriptor = NetcdfSharp.ucar.nc2.iosp.bufr.Descriptor;
	using CommonCodeTable = NetcdfSharp.ucar.nc2.wmo.CommonCodeTable;

	/// <summary>
	/// Read BUFR Code / Flag tables. </summary>
	public class CodeFlagTables
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(CodeFlagTables));
	  private const string CodeFlagFilename = "wmo/BUFRCREX_41_0_0_CodeFlag_en.xml";
	  internal static IDictionary<short, CodeFlagTables> tableMap;

	  public static CodeFlagTables getTable(short id)
	  {
		if (tableMap == null)
		{
		  init();
		}

		if (id == 263)
		{
		  return useCC(id, 5); // 0-1-7
		}
		if (id == 526)
		{
		  return useCC(id, 7); // 0-2-14
		}
		if (id == 531)
		{
		  return useCC(id, 8); // 0-2-19
		}
		if (id == 5699)
		{
		  return useCC(id, 3); // 0-22-67
		}
		if (id == 5700)
		{
		  return useCC(id, 4); // 0-22-68
		}

		return tableMap[id];
	  }

	  private static CodeFlagTables useCC(short fxy, int cc)
	  {
		CodeFlagTables cft = tableMap[fxy];
		if (cft == null)
		{
		  CommonCodeTable cct = CommonCodeTable.getTable(cc);
		  cft = new CodeFlagTables(fxy, cct.getTableName(), cct.getMap());
		  tableMap[fxy] = cft;
		}
		return cft;
	  }

	  public static bool hasTable(short id)
	  {
		if (tableMap == null)
		{
		  init();
		}
		CodeFlagTables result = tableMap[id];
		return result != null;
	  }

	  private static void init()
	  {
		tableMap = new Dictionary<short, CodeFlagTables>(300);
		init(tableMap);
	  }

	  public static IDictionary<short, CodeFlagTables> Tables
	  {
		  get
		  {
			if (tableMap == null)
			{
			  init();
			}
			return tableMap;
		  }
	  }

	  /*
	   * <Exp_CodeFlagTables_E>
	   * <No>837</No>
	   * <FXY>002119</FXY>
	   * <ElementName_E>Instrument operations</ElementName_E>
	   * <CodeFigure>0</CodeFigure>
	   * <EntryName_E>Intermediate frequency calibration mode (IF CAL)</EntryName_E>
	   * <Status>Operational</Status>
	   * </Exp_CodeFlagTables_E>
	   * 
	   * <BUFRCREX_19_1_1_CodeFlag_en>
	   * <No>2905</No>
	   * <FXY>020042</FXY>
	   * <ElementName_en>Airframe icing present</ElementName_en>
	   * <CodeFigure>2</CodeFigure>
	   * <EntryName_en>Reserved</EntryName_en>
	   * <Status>Operational</Status>
	   * </BUFRCREX_19_1_1_CodeFlag_en>
	   * 
	   * <BUFRCREX_22_0_1_CodeFlag_en>
	   * <No>3183</No>
	   * <FXY>020063</FXY>
	   * <ElementName_en>Special phenomena</ElementName_en>
	   * <CodeFigure>31</CodeFigure>
	   * <EntryName_en>Slight coloration of clouds at sunrise associated with a tropical disturbance</EntryName_en>
	   * <Status>Operational</Status>
	   * </BUFRCREX_22_0_1_CodeFlag_en>
	   * 
	   */
	  internal static void init(IDictionary<short, CodeFlagTables> table)
	  {
		string filename = BufrTables.RESOURCE_PATH + CodeFlagFilename;
		try
		{
				using (Stream @is = NetcdfSharp.ucar.nc2.util.IO.getFileResource(filename))
				{
			  SAXBuilder builder = new SAXBuilder();
			  builder.setExpandEntities(false);
			  NetcdfSharp.org.jdom2.Document tdoc = builder.build(@is);
			  Element root = tdoc.getRootElement();

			  IList<Element> elems = root.getChildren();
			  foreach (Element elem in elems)
			  {
				string fxyS = elem.getChildText("FXY");
				string desc = elem.getChildText("ElementName_en");

				short fxy = Descriptor.getFxy2(fxyS);
				CodeFlagTables ct = table[fxy];
				if (ct == null)
				{
				  ct = new CodeFlagTables(fxy, desc);
				  table[fxy] = ct;
				}

				string line = elem.getChildText("No");
				string codeS = elem.getChildText("CodeFigure");
				string value = elem.getChildText("EntryName_en");

				if ((string.ReferenceEquals(codeS, null)) || (string.ReferenceEquals(value, null)))
				{
				  continue;
				}
				if (value.ToLower().StartsWith("reserved", StringComparison.Ordinal))
				{
				  continue;
				}
				if (value.ToLower().StartsWith("not used", StringComparison.Ordinal))
				{
				  continue;
				}

				int code;
				if (codeS.ToLower().Contains("all"))
				{
				  code = -1;
				}
				else
				{
				  try
				  {
					code = int.Parse(codeS);
				  }
				  catch (System.FormatException)
				  {
					log.debug("NumberFormatException on line " + line + " in " + codeS);
					continue;
				  }
				}
				ct.addValue((short) code, value);
			  }

				}
		}
		catch (Exception e) when (e is IOException || e is JDOMException)
		{
		  log.error("Can't read BUFR code table " + filename, e);
		}
	  }

	  ////////////////////////////////////////////////
	  // TODO Make Immutable
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private short fxy_Conflict;
	  private string name;
	  private IDictionary<int, string> map; // needs to be integer for EnumTypedef

	  internal CodeFlagTables(short fxy, string name)
	  {
		this.fxy_Conflict = fxy;
		this.name = (string.ReferenceEquals(name, null)) ? this.fxy() : name; // StringUtil2.replace(name, ' ', "_") + "("+fxy()+")";
		map = new Dictionary<int, string>(20);
	  }

	  private CodeFlagTables(short fxy, string name, IDictionary<int, string> map)
	  {
		this.fxy_Conflict = fxy;
		this.name = (string.ReferenceEquals(name, null)) ? this.fxy() : name;
		this.map = map;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual IDictionary<int, string> Map
	  {
		  get
		  {
			return map;
		  }
	  }

	  internal virtual void addValue(int value, string text)
	  {
		map[value] = text;
	  }

	  public virtual short Id
	  {
		  get
		  {
			return fxy_Conflict;
		  }
	  }

	  public virtual string fxy()
	  {
		int f = fxy_Conflict >> 14;
		int x = (fxy_Conflict & 0xff00) >> 8;
		int y = (fxy_Conflict & 0xff);

		return f + "-" + x + "-" + y;
	  }

	  public override string ToString()
	  {
		return name;
	  }

	}

}
