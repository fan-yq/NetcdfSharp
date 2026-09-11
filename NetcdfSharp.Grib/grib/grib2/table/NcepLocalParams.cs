// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using GribResourceReader = NetcdfSharp.ucar.nc2.grib.GribResourceReader;
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;

	/// <summary>
	/// Read NCEP parameter tables.
	/// There will be a Table for each discipline, category
	/// 
	/// @author caron
	/// @since 1/9/12
	/// </summary>
	public class NcepLocalParams
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(NcepLocalParams));
	  private const string MATCH = "Table4.2.";

	  private const bool debugOpen = false;
	  private const bool debug = false;

#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks

	  private readonly IDictionary<int, Table> tableMap = new Dictionary<int, Table>(30);
	  private readonly string resourcePath;

	  internal NcepLocalParams(string resourcePath)
	  {
		this.resourcePath = resourcePath;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter getParameter(int discipline, int category, int number)
	  public virtual Grib2Parameter getParameter(int discipline, int category, int number)
	  {
		int key = (discipline << 8) + category;
		Table @params = tableMap[key];
		if (@params == null)
		{
		  @params = factory(discipline, category);
		  if (@params == null)
		  {
			return null;
		  }
		  tableMap[key] = @params;
		}
		return @params.getParameter(number);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getCategory(int discipline, int category)
	  public virtual string getCategory(int discipline, int category)
	  {
		int key = (discipline << 8) + category;
		Table @params = tableMap[key];
		return (@params == null) ? null : @params.title;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable ImmutableList<NetcdfSharp.ucar.nc2.grib.GribTables.Parameter> getParameters(String path)
	  internal virtual ImmutableList<GribTables.Parameter> getParameters(string path)
	  {
		Table table = new Table();
		if (!table.readParameterTableXml(path))
		{
		  return null;
		}
		return table.Parameters;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable private Table factory(int discipline, int category)
	  private Table factory(int discipline, int category)
	  {
		Table @params = new Table();
		if (!@params.readParameterTableFromResource(getTablePath(discipline, category)))
		{
		  return null;
		}
		return @params;
	  }

	  internal virtual string getTablePath(int discipline, int category)
	  {
		return resourcePath + "Table4.2." + discipline + "." + category + ".xml";
	  }

	  ////////////////////////////////////////////////////
	  // one table for each discipline, category
	  internal class Table
	  {

		internal string title;
		internal string source;
		internal string tableName;
		internal int discipline, category;
		internal IDictionary<int, Grib2Parameter> paramMap;

		internal virtual ImmutableList<GribTables.Parameter> Parameters
		{
			get
			{
	// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
			  return ImmutableList<GribTables.Parameter>.copyOf(paramMap.Values.OrderBy(c => c));
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter getParameter(int code)
		public virtual Grib2Parameter getParameter(int code)
		{
		  return paramMap[code];
		}

		internal virtual bool readParameterTableXml(string path)
		{
		  if (debugOpen)
		  {
			logger.debug("readParameterTableXml table {}", path);
		  }
		  try
		  {
				  using (Stream @is = GribResourceReader.getInputStream(path))
				  {
				SAXBuilder builder = new SAXBuilder();
				builder.setExpandEntities(false);
				NetcdfSharp.org.jdom2.Document doc = builder.build(@is);
				Element root = doc.getRootElement();
				paramMap = parseXml(root); // all at once - thread safe
				return true;

				  }
		  }
		  catch (Exception ioe) when (ioe is IOException || ioe is JDOMException)
		  {
			Console.WriteLine(ioe);
			return false;
		  }
		}

		internal virtual bool readParameterTableFromResource(string resource)
		{
		  if (debugOpen)
		  {
			logger.debug("readParameterTableFromResource from resource {}", resource);
		  }
		  try
		  {
				  using (Stream @is = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStream(resource))
				  {
				if (@is == null)
				{
				  logger.info("Cant read resource " + resource);
				  return false;
				}
				SAXBuilder builder = new SAXBuilder();
				builder.setExpandEntities(false);
				NetcdfSharp.org.jdom2.Document doc = builder.build(@is);
				Element root = doc.getRootElement();
				paramMap = parseXml(root); // all at once - thread safe
				return true;

				  }
		  }
		  catch (Exception ioe) when (ioe is IOException || ioe is JDOMException)
		  {
			Console.WriteLine(ioe);
			return false;
		  }
		}

		/*
		 * <parameterMap>
		 * <table>Table4.2.0.0</table>
		 * <title>Temperature</title>
		 * <source>http://www.nco.ncep.noaa.gov/pmb/docs/grib2/grib2_table4-2-0-0.shtml</source>
		 * <parameter code="0">
		 * <shortName>TMP</shortName>
		 * <description>Temperature</description>
		 * <units>K</units>
		 * </parameter>
		 */
		internal virtual Dictionary<int, Grib2Parameter> parseXml(Element root)
		{
		  tableName = root.getChildText("table");
		  title = root.getChildText("title");
		  source = root.getChildText("source");

		  // Table4.2.0.0
		  int pos = tableName.IndexOf(MATCH, StringComparison.Ordinal);
		  string dc = tableName.Substring(pos + MATCH.Length);
		  string[] dcs = System.Text.RegularExpressions.Regex.Split(dc, "\\.");
		  discipline = int.Parse(dcs[0]);
		  category = int.Parse(dcs[1]);

		  Dictionary<int, Grib2Parameter> result = new Dictionary<int, Grib2Parameter>();
		  IList<Element> @params = root.getChildren("parameter");
		  foreach (Element elem in @params)
		  {
			int code = int.Parse(elem.getAttributeValue("code"));
			string abbrev = elem.getChildText("shortName");
			string desc = elem.getChildText("description");
			string units = elem.getChildText("units");
			if (string.ReferenceEquals(units, null))
			{
			  units = "";
			}

			string name;
			if (desc.Length > 80 && !string.ReferenceEquals(abbrev, null) && !abbrev.Equals("Validation", StringComparison.OrdinalIgnoreCase))
			{
			  name = abbrev;
			}
			else
			{
			  name = desc;
			  desc = null;
			}

			// public Grib2Parameter(int discipline, int category, int number, String name, String unit, String abbrev) {
			Grib2Parameter parameter = new Grib2Parameter(discipline, category, code, name, units, abbrev, desc);
			result[parameter.Number] = parameter;
			if (debug)
			{
			  logger.debug(" {}", parameter);
			}
		  }
		  return result;
		}

		public override string ToString()
		{
		  return MoreObjects.toStringHelper(this).add("title", title).add("source", source).add("tableName", tableName).add("discipline", discipline).add("category", category).ToString();
		}
	  }

	}

#pragma warning restore CS0162

}
