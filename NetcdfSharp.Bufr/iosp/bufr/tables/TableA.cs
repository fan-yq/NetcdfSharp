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
	using Element = NetcdfSharp.org.jdom2.Element;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;

	/// <summary>
	/// Read standard WMO Table A (data categories). </summary>
	public class TableA
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(TableA));
	  private const string TABLEA_FILENAME = "wmo/BUFR_41_0_0_TableA_en.xml";
	  private static IDictionary<int, Descriptor> tableA;
	  private readonly string name;
	  private readonly string location;

	  /*
	   * <BUFR_19_1_1_TableA_en>
	   * <No>27</No>
	   * <CodeFigure>28</CodeFigure>
	   * <Meaning_en>Precision orbit (satellite)</Meaning_en>
	   * <Status>Operational</Status>
	   * </BUFR_19_1_1_TableA_en>
	   * 
	   * <Exp_BUFRTableA_E>
	   * <No>4</No>
	   * <CodeFigure>3</CodeFigure>
	   * <Meaning_E>Vertical soundings (satellite)</Meaning_E>
	   * <Status>Operational</Status>
	   * </Exp_BUFRTableA_E>
	   */
	  private static void init()
	  {
		string filename = BufrTables.RESOURCE_PATH + TABLEA_FILENAME;
		try
		{
				using (Stream @is = NetcdfSharp.ucar.nc2.util.IO.getFileResource(filename))
				{

			  Dictionary<int, Descriptor> map = new Dictionary<int, Descriptor>(100);
			  SAXBuilder builder = new SAXBuilder();
			  builder.setExpandEntities(false);
			  NetcdfSharp.org.jdom2.Document tdoc = builder.build(@is);
			  Element root = tdoc.getRootElement();

			  IList<Element> elems = root.getChildren();
			  foreach (Element elem in elems)
			  {
				string line = elem.getChildText("No");
				string codeS = elem.getChildText("CodeFigure");
				string desc = elem.getChildText("Meaning_en");

				try
				{
				  int code = int.Parse(codeS);
				  Descriptor descriptor = new Descriptor(code, desc);
				  map[code] = descriptor;
				}
				catch (System.FormatException)
				{
				  log.debug("NumberFormatException on line " + line + " in " + codeS);
				}

			  }
			  tableA = map;

				}
		}
		catch (Exception e)
		{
		  log.error("Can't read BUFR code table " + filename, e);
		}
	  }

	  public TableA(string name, string location)
	  {
		this.name = name;
		this.location = location;
		tableA = new Dictionary<int, Descriptor>();
	  }

	  public virtual Descriptor getDescriptor(int code)
	  {
		if (tableA == null)
		{
		  init();
		}
		return tableA[code];
	  }

	  /// <summary>
	  /// data category name, from table A
	  /// </summary>
	  /// <param name="cat"> data category </param>
	  /// <returns> category name, or null if not found </returns>
	  public static string getDataCategory(int cat)
	  {
		if (tableA == null)
		{
		  init();
		}
		Descriptor descriptor = tableA[cat];
		return descriptor != null ? descriptor.Description : "Unknown category=" + cat;
	  }

	  /// <summary>
	  /// data category name, from table A
	  /// </summary>
	  /// <param name="cat"> data category </param>
	  /// <returns> category name, or null if not found </returns>
	  public static string getDataCategoryName(int cat)
	  {
		if (tableA == null)
		{
		  init();
		}
		Descriptor descriptor = tableA[cat];
		return descriptor != null ? descriptor.Name : "obs_" + cat;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Location
	  {
		  get
		  {
			return location;
		  }
	  }

	  public virtual TableA.Descriptor addDescriptor(int code, string description)
	  {
		TableA.Descriptor d = new TableA.Descriptor(code, description);
		tableA[code] = d;
		return d;
	  }

	  public class Descriptor : IComparable<Descriptor>
	  {
		internal int code;
		internal string name;
		internal string description;
		internal bool localOverride;

		internal Descriptor(int code, string description)
		{
		  this.code = code;
		  this.description = description;
		  this.name = "obs_" + code.ToString();
		}

		public virtual string Name
		{
			get
			{
			  return name;
			}
			set
			{
			  this.name = value;
			}
		}


		public virtual string Description
		{
			get
			{
			  return this.description;
			}
		}

		/// <summary>
		/// Get code
		/// </summary>
		/// <returns> Code </returns>
		public virtual int Code
		{
			get
			{
			  return this.code;
			}
		}

		public override string ToString()
		{
		  return code.ToString() + " " + Name + " " + this.description;
		}

		public virtual int CompareTo(Descriptor o)
		{
		  return code - o.Code;
		}

		public virtual bool Local
		{
			get
			{
			  return ((code >= 102) && (code <= 239));
			}
		}

		public virtual bool LocalOverride
		{
			set
			{
			  this.localOverride = value;
			}
			get
			{
			  return localOverride;
			}
		}


	  }

	}

}
