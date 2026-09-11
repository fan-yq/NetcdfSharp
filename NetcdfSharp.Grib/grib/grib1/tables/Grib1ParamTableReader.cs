// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1.tables
{
	using Matcher = NetcdfSharp.com.google.re2j.Matcher;
	using Pattern = NetcdfSharp.com.google.re2j.Pattern;
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using Namespace = NetcdfSharp.org.jdom2.Namespace;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using Catalog = NetcdfSharp.thredds.client.catalog.Catalog;
	using GribResourceReader = NetcdfSharp.ucar.nc2.grib.GribResourceReader;
	using NetcdfSharp.ucar.nc2.grib.grib1;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// A Grib1 Parameter Table (table 2). This is a map: code -> Grib1Parameter. Handles reading the
	/// table in from various formats
	/// 
	/// @author caron
	/// @since 11/16/11
	/// </summary>
	public class Grib1ParamTableReader
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib1ParamTableReader));
	  private const bool debug = false;

	  private int center_id;
	  private int subcenter_id;
	  private int version;
	  private readonly string name; // name of the table
	  private string path; // path of filename containing this table
	  private string desc; // optional desc from within the file
	  private IDictionary<int, Grib1Parameter> parameters; // param number -> param
	  private bool useName;

	  /// <summary>
	  /// Read a dataset-specific table from a file
	  /// </summary>
	  /// <param name="path"> read from this path, may be relative </param>
	  public Grib1ParamTableReader(string path)
	  {
		this.path = StringUtil2.replace(path, "\\", "/");
		File f = new File(path);
		this.name = f.getName();
		this.parameters = readParameterTable();
	  }

	  /// <summary>
	  /// Create a standard table from a file. Defer reading until getParameters is called.
	  /// </summary>
	  /// <param name="center_id"> associate with this center </param>
	  /// <param name="subcenter_id"> associate with this subcenter </param>
	  /// <param name="version"> associate with this version </param>
	  /// <param name="path"> read from this path, may be relative </param>
	  internal Grib1ParamTableReader(int center_id, int subcenter_id, int version, string path)
	  {
		this.center_id = center_id;
		this.subcenter_id = subcenter_id;
		this.version = version;
		this.path = StringUtil2.replace(path, "\\", "/");
		File f = new File(path);
		this.name = f.getName();
	  }

	  /// <summary>
	  /// Create a dataset-specific table from a jdom tree using the DSS parser
	  /// </summary>
	  /// <param name="paramTableElem"> the jdom tree </param>
	  internal Grib1ParamTableReader(Element paramTableElem)
	  {
		this.name = paramTableElem.getChildText("title");
		DssParser p = new DssParser(this, Catalog.ncmlNS);
		this.parameters = p.parseXml(paramTableElem);
	  }

	  public virtual int Center_id
	  {
		  get
		  {
			return center_id;
		  }
	  }

	  public virtual int Subcenter_id
	  {
		  get
		  {
			return subcenter_id;
		  }
	  }

	  public virtual int Version
	  {
		  get
		  {
			return version;
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

	  // whether to use the param name or description in the variable name
	  public virtual bool useParamName()
	  {
		return useName;
	  }

	  public virtual int Key
	  {
		  get
		  {
			return Grib1ParamTables.makeKey(center_id, subcenter_id, version);
		  }
	  }

	  public virtual string Path
	  {
		  get
		  {
			return path;
		  }
	  }

	  public virtual IDictionary<int, Grib1Parameter> Parameters
	  {
		  get
		  {
			if (parameters == null)
			{
			  parameters = readParameterTable();
			}
			return parameters;
		  }
		  set
		  {
			parameters = value;
		  }
	  }


	  /// <summary>
	  /// Get the parameter with id. If not found, look in default table.
	  /// </summary>
	  /// <param name="id"> the parameter number </param>
	  /// <returns> the Grib1Parameter, or null if not found </returns>
	  public virtual Grib1Parameter getParameter(int id)
	  {
		if (parameters == null)
		{
		  parameters = readParameterTable();
		}

		Grib1Parameter p;
		return parameters.TryGetValue(id, out p) ? p : null;
	  }

	  /// <summary>
	  /// Get the parameter with id, but dont look in default table.
	  /// </summary>
	  /// <param name="id"> the parameter number </param>
	  /// <returns> the Grib1Parameter, or null if not found </returns>
	  public virtual Grib1Parameter getLocalParameter(int id)
	  {
		if (parameters == null)
		{
		  parameters = readParameterTable();
		}
		Grib1Parameter p;
		return parameters.TryGetValue(id, out p) ? p : null;
	  }

	  public override string ToString()
	  {
		return "Grib1ParamTable{" + "center_id=" + center_id + ", subcenter_id=" + subcenter_id + ", version=" + version + ", name='" + name + '\'' + ", path='" + path + '\'' + '}';
	  }

	  //////////////////////////////////////////////////////////////////////////////////////
	  // reading

	  private IDictionary<int, Grib1Parameter> readParameterTable()
	  {
		  lock (this)
		  {
			if (string.ReferenceEquals(path, null))
			{
			  throw new System.InvalidOperationException(name);
			}

			try
			{
			  if (name.StartsWith("table_2_", StringComparison.Ordinal) || name.StartsWith("local_table_2_", StringComparison.Ordinal))
			  {
				return readParameterTableEcmwf(); // ecmwf
			  }
			  else if (name.StartsWith("US058", StringComparison.Ordinal))
			  {
				return readParameterTableXml(new FnmocParser(this)); // FNMOC
			  }
			  else if (name.EndsWith(".tab", StringComparison.Ordinal))
			  {
				return readParameterTableTab(); // wgrib format
			  }
			  else if (name.EndsWith(".wrf", StringComparison.Ordinal))
			  {
				return readParameterTableSplit("\\|", new int[] {0, 3, 1, 2}); // WRF AMPS
			  }
			  else if (name.EndsWith(".h", StringComparison.Ordinal))
			  {
				return readParameterTableNcl(); // NCL
			  }
			  else if (name.EndsWith(".dss", StringComparison.Ordinal))
			  {
				return readParameterTableSplit("\t", new int[] {0, -1, 1, 2}); // NCAR DSS
			  }
			  else if (name.EndsWith(".xml", StringComparison.Ordinal))
			  {
				return readParameterTableXml(new DssParser(this, Namespace.NO_NAMESPACE)); // NCAR DSS XML format
			  }
			  else if (name.StartsWith("2.98", StringComparison.Ordinal))
			  {
				return readParameterTableEcmwfEcCodes(); // ecmwf from ecCodes package
			  }
			  else
			  {
				throw new Exception("Grib1ParamTableReader: Dont know how to read " + name + " file=" + path);
			  }

			}
			catch (IOException ioError)
			{
			  logger.warn("An error occurred in Grib1ParamTable while trying to open the parameter table {}:{}", path, ioError.Message);
			  throw new Exception(ioError.Message, ioError);
			}
		  }
	  }

	  /*
	   * Brazilian Space Agency - INPE/CPTEC
	   * Center: 46
	   * Subcenter: 0
	   * Parameter table version: 254
	   *
	   * 
	   * TBLE2 cptec_254_params[] = {
	   * {1, "Pressure", "hPa", "PRES"},
	   * {2, "Pressure reduced to MSL", "hPa", "PSNM"},
	   * {3, "Pressure tendency", "Pa/s", "TSPS"},
	   * {6, "Geopotential", "dam", "GEOP"},
	   * {7, "Geopotential height", "gpm", "ZGEO"},
	   * {8, "Geometric height", "m", "GZGE"},
	   * {11, "ABSOLUTE TEMPERATURE", "K", "TEMP"},
	   * 
	   */

	  private static readonly Pattern nclPattern = Pattern.compile("\\{(\\d*),\\s*\"([^\"]*)\",\\s*\"([^\"]*)\",\\s*\"([^\"]*)\".*");

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.java.util.Map<int, Grib1Parameter> readParameterTableNcl() throws NetcdfSharp.java.io.IOException
	  private IDictionary<int, Grib1Parameter> readParameterTableNcl()
	  {
		Dictionary<int, Grib1Parameter> result = new Dictionary<int, Grib1Parameter>();

		using (Stream @is = GribResourceReader.getInputStream(path))
		using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
		{

		  // Ignore header
		  while (true)
		  {
			string line = br.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  break; // done with the file
			}
			if (line.StartsWith("TBLE2", StringComparison.Ordinal))
			{
			  break;
			}

			if (line.Contains("Center:"))
			{
			  center_id = extract(line, "Center:");
			}
			else if (line.Contains("Subcenter:"))
			{
			  subcenter_id = extract(line, "Subcenter:");
			}
			else if (line.Contains("version:"))
			{
			  version = extract(line, "version:");
			}
		  }

		  while (true)
		  {
			string line = br.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  break; // done with the file
			}
			if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
			{
			  continue;
			}

			Matcher m = nclPattern.matcher(line);
			if (!m.matches())
			{
			  continue;
			}

			int p1;
			try
			{
			  p1 = int.Parse(m.group(1));
			}
			catch (Exception)
			{
			  logger.warn("Cant parse " + m.group(1) + " in file " + path);
			  continue;
			}
			Grib1Parameter parameter = new Grib1Parameter(this, p1, m.group(4), m.group(2), m.group(3));
			result[parameter.Number] = parameter;
			logger.debug(" {}", parameter);
		  }

		  return Collections.unmodifiableMap(result); // all at once - thread safe
		}

	  }

	  private int extract(string line, string key)
	  {
		int pos = line.IndexOf(key, StringComparison.Ordinal);
		if (pos < 0)
		{
		  return -1;
		}
		string want = line.Substring(pos + key.Length);
		try
		{
		  return int.Parse(want.Trim());
		}
		catch (System.FormatException)
		{
		  logger.warn("BAD {} ({})", line, path);
		  return -1;
		}
	  }

	  /*
	   * WMO standard table 2: Version Number 3.
	   * Codes and data units for FM 92-X Ext.GRIB.
	   * ......................
	   * 001
	   * P
	   * Pressure
	   * Pa
	   * Pa
	   * ......................
	   * 002
	   * MSL
	   * Mean sea level pressure
	   * Pa
	   * Pa
	   * ......................
	   * 003
	   * None
	   * Pressure tendency
	   * Pa s**-1
	   * Pa s**-1
	   * ......................
	   * 004
	   * PV
	   * Potential vorticity
	   * K m**2 kg**-1 s**-1
	   * K m**2 kg**-1 s**-1
	   * ......................
	   * 005
	   * None
	   * ICAO Standard Atmosphere reference height
	   * m
	   * m
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.java.util.Map<int, Grib1Parameter> readParameterTableEcmwf() throws NetcdfSharp.java.io.IOException
	  private IDictionary<int, Grib1Parameter> readParameterTableEcmwf()
	  {
		Dictionary<int, Grib1Parameter> result = new Dictionary<int, Grib1Parameter>();

		using (Stream @is = GribResourceReader.getInputStream(path))
		using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
		{
		  string line = br.ReadLine();
		  if (string.ReferenceEquals(line, null))
		  {
			throw new FileNotFoundException(path + " is empty");
		  }
		  if (!line.StartsWith("...", StringComparison.Ordinal))
		  {
			this.desc = line; // maybe ??
		  }

		  while (!string.ReferenceEquals(line, null) && !line.StartsWith("...", StringComparison.Ordinal))
		  {
			line = br.ReadLine(); // skip
		  }

		  while (true)
		  {
			line = br.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  break; // done with the file
			}
			if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
			{
			  continue;
			}
			if (line.StartsWith("...", StringComparison.Ordinal))
			{ // ... may have already been read
			  line = br.ReadLine();
			  if (string.ReferenceEquals(line, null))
			  {
				break;
			  }
			}
			string num = line.Trim();
			line = br.ReadLine();
			string name = (!string.ReferenceEquals(line, null)) ? line.Trim() : null;
			line = br.ReadLine();
			string desc = (!string.ReferenceEquals(line, null)) ? line.Trim() : null;
			line = br.ReadLine();
			string units1 = (!string.ReferenceEquals(line, null)) ? line.Trim() : null;

			// optional notes
			line = br.ReadLine();
			string notes = (string.ReferenceEquals(line, null) || line.StartsWith("...", StringComparison.Ordinal)) ? null : line.Trim();
			if ("undefined".Equals(desc, StringComparison.OrdinalIgnoreCase))
			{
			  continue; // skip
			}

			int p1;
			try
			{
			  p1 = int.Parse(num);
			}
			catch (Exception)
			{
			  logger.warn("Cant parse " + num + " in file " + path);
			  continue;
			}
			Grib1Parameter parameter = new Grib1Parameter(this, p1, name, desc, units1);
			result[parameter.Number] = parameter;
			logger.debug("readParameterTableEcmwf {} ({})", parameter, notes);
		  }
		  return Collections.unmodifiableMap(result); // all at once - thread safe
		}

	  }

	  /// <summary>
	  /// This method will read in ECMWF grib1 tables. Note that these tables are generated locally by
	  /// Unidata, and come directly from the ECMWF GRIB-API package localConcepts files. They are
	  /// generated by:
	  /// <para>
	  /// NetcdfSharp.ucar.nc2.grib.grib1.tables.EcmwfLocalConcepts
	  /// </para>
	  /// <para>
	  /// The original localConcepts files are located in:
	  /// <p/>
	  /// grib/src/main/sources/ecmwfEcCodes/
	  /// <p/>
	  /// Since we write the table file that are ultimately read by CDM, the format is controled and is
	  /// the following:
	  /// </para>
	  /// <para>
	  /// paramNum shortName [description] (units)
	  /// </para>
	  /// <para>
	  /// for example,
	  /// </para>
	  /// <para>
	  /// 251 atte [Adiabatic tendency of temperature] (K)
	  /// </para>
	  /// </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.java.util.Map<int, Grib1Parameter> readParameterTableEcmwfEcCodes() throws NetcdfSharp.java.io.IOException
	  private IDictionary<int, Grib1Parameter> readParameterTableEcmwfEcCodes()
	  {
		Dictionary<int, Grib1Parameter> result = new Dictionary<int, Grib1Parameter>();

		using (Stream @is = GribResourceReader.getInputStream(path))
		using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
		{
		  string line = br.ReadLine();
		  if (string.ReferenceEquals(line, null))
		  {
			throw new FileNotFoundException(path + " is empty");
		  }

		  // create table name from file name
		  string[] splitPath = path.Split('/');
		  string tableNum = splitPath[splitPath.Length - 1].Replace(".table", "");
		  this.desc = "ECMWF GRIB API TABLE " + tableNum;

		  // skip header
		  while (!string.ReferenceEquals(line, null) && !line.StartsWith("#", StringComparison.Ordinal))
		  {
			line = br.ReadLine(); // skip
		  }

		  // keep going until the end of the file is reached (line == null)
		  while (true)
		  {
			// exmaple: 251 atte [Adiabatic tendency of temperature] (K)
			line = br.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  break; // done with the file
			}
			if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
			{
			  continue;
			}

			// get unit - (K)
			string[] tmpUnitArray = line.Split('(');
			string tmpUnit = tmpUnitArray[tmpUnitArray.Length - 1];
			int lastUnitIndex;
			while ((lastUnitIndex = tmpUnit.LastIndexOf(")", StringComparison.Ordinal)) > 0)
			{
			  tmpUnit = tmpUnit.Substring(0, lastUnitIndex).Trim();
			}
			string unit = tmpUnit.Trim();
			// unit = Util.cleanUnit(unit); // fixes some common unit mistakes // jcaron - just use unit as it is

			// get parameter number - 251
			string[] lineArray = line.Trim().Split((char[]) null, StringSplitOptions.RemoveEmptyEntries); // all and any white space
			string num = lineArray[0];

			// get shortName - atte
			string name = lineArray[1].Trim();
			// if (name.equals("~")) {}; - todo create name from long name(?)

			// get description. bracketed by [] - [Adiabatic Tendency of temperature]
			int startDesc = line.IndexOf("[", StringComparison.Ordinal);
			int endDesc = line.IndexOf("]", StringComparison.Ordinal);
			string desc = line.Substring(startDesc, endDesc - startDesc).Trim();

			// stuff information into a Grib1Parameter object
			int p1;
			try
			{
			  p1 = int.Parse(num);
			}
			catch (Exception)
			{
			  logger.warn("Cant parse " + num + " in file " + path);
			  continue;
			}
			Grib1Parameter parameter = new Grib1Parameter(this, p1, name, desc, unit);
			result[parameter.Number] = parameter;
			logger.debug(" {}", parameter);
		  }

		  return Collections.unmodifiableMap(result); // all at once - thread safe
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.java.util.Map<int, Grib1Parameter> readParameterTableXml(XmlTableParser parser) throws NetcdfSharp.java.io.IOException
	  private IDictionary<int, Grib1Parameter> readParameterTableXml(XmlTableParser parser)
	  {
		try
		{
				using (Stream @is = GribResourceReader.getInputStream(path))
				{
			  SAXBuilder builder = new SAXBuilder();
			  builder.setExpandEntities(false);
			  NetcdfSharp.org.jdom2.Document doc = builder.build(@is);
			  Element root = doc.getRootElement();
			  return parser.parseXml(root); // all at once - thread safe

				}
		}
		catch (JDOMException e)
		{
		  throw new IOException(e.Message, e);
		}
		catch (IOException e)
		{
		  throw new IOException(e.Message, e);
		}
	  }

	  private interface XmlTableParser
	  {
		IDictionary<int, Grib1Parameter> parseXml(Element root);
	  }

	  private class DssParser : XmlTableParser
	  {

		private readonly Grib1ParamTableReader outerInstance;
		private readonly Namespace ns;

		internal DssParser(Grib1ParamTableReader outerInstance, Namespace ns)
		{
		  this.outerInstance = outerInstance;
		  this.ns = ns;
		}

		public IDictionary<int, Grib1Parameter> parseXml(Element root)
		{
		  Dictionary<int, Grib1Parameter> result = new Dictionary<int, Grib1Parameter>();
		  IList<Element> @params = root.getChildren("parameter", ns);
		  foreach (Element elem1 in @params)
		  {
			int code = int.Parse(elem1.getAttributeValue("code"));
			string desc = elem1.getChildText("description", ns);
			if (desc == null)
			{
			  continue;
			}
			string units = elem1.getChildText("units", ns);
			if (units == null)
			{
			  units = "";
			}
			string name = elem1.getChildText("name", ns);
			if (name == null)
			{
			  name = elem1.getChildText("shortName", ns);
			}
			string cf = elem1.getChildText("CF", ns);
			Grib1Parameter parameter = new Grib1Parameter(outerInstance, code, name, desc, units, cf);
			result[parameter.Number] = parameter;
			logger.debug(" {}", parameter);
		  }
		  return Collections.unmodifiableMap(result); // all at once - thread safe
		}
	  }

	  private class FnmocParser : XmlTableParser
	  {
		private readonly Grib1ParamTableReader outerInstance;

		internal FnmocParser(Grib1ParamTableReader outerInstance)
		{
		  this.outerInstance = outerInstance;
		}

		public IDictionary<int, Grib1Parameter> parseXml(Element root)
		{
		  Dictionary<int, Grib1Parameter> result = new Dictionary<int, Grib1Parameter>();
		  Element fnmocTable = root.getChild("fnmocTable");
		  IList<Element> @params = fnmocTable.getChildren("entry");
		  foreach (Element elem1 in @params)
		  {
			int code;
			try
			{
			  code = int.Parse(elem1.getChildText("grib1Id"));
			}
			catch (FormatException)
			{
			  logger.warn("BAD number= {}", elem1.getChildText("grib1Id"));
			  continue;
			}
			string desc = elem1.getChildText("description");
			if (desc == null)
			{
			  continue;
			}
			// if (desc.startsWith("no definition")) continue; // skip; use standard def
			desc = StringUtil2.collapseWhitespace(desc);
			string units = elem1.getChildText("unitsFNMOC");
			if (units == null)
			{
			  units = "";
			}
			string name = elem1.getChildText("name");
			Grib1Parameter parameter = new Grib1Parameter(outerInstance, code, name, desc, units, null);
			result[parameter.Number] = parameter;
			logger.debug(" {}", parameter);
		  }
		  outerInstance.useName = true;
		  return result;
		}
	  }

	  // order: num, name, desc, unit
	  private IDictionary<int, Grib1Parameter> readParameterTableSplit(string regexp, int[] order)
	  {
		Dictionary<int, Grib1Parameter> result = new Dictionary<int, Grib1Parameter>();

		using (Stream @is = GribResourceReader.getInputStream(path))
		using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
		{
		  // rdg - added the 0 line length check to cover the case of blank lines at the end of the parameter table file.
		  while (true)
		  {
			string line = br.ReadLine();
			if (line == null)
			{
			  break;
			}
			if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
			{
			  continue;
			}
			string[] flds = Pattern.compile(regexp).split(line);

			int p1 = int.Parse(flds[order[0]].Trim()); // must have a number
			string name = (order[1] >= 0) ? flds[order[1]].Trim() : null;
			string desc = flds[order[2]].Trim();
			string units = (flds.Length > order[3]) ? flds[order[3]].Trim() : "";

			Grib1Parameter parameter = new Grib1Parameter(this, p1, name, desc, units);
			result[parameter.Number] = parameter;
			logger.debug(" {}", parameter);
		  }
		  return Collections.unmodifiableMap(result); // all at once - thread safe
		}

	  }

	  private IDictionary<int, Grib1Parameter> readParameterTableTab()
	  {

		using (Stream @is = GribResourceReader.getInputStream(path))
		{
		  Dictionary<int, Grib1Parameter> @params;
		  using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
		  {
			br.ReadLine(); // skip a line

			// thread safe - local var
			@params = new Dictionary<int, Grib1Parameter>();
			while (true)
			{
			  string line = br.ReadLine();
			  if (line == null)
			  {
				break;
			  }
			  if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
			  {
				continue;
			  }
			  string[] tableDefArr = line.Split(':');

			  int p1 = int.Parse(tableDefArr[0].Trim());
			  string name = tableDefArr[1].Trim();
			  string desc, units;
			  // check to see if unit defined, if not, parameter is undefined
			  if (tableDefArr[2].IndexOf('[') == -1)
			  {
				// Undefined unit
				desc = tableDefArr[2].Trim();
				units = "";
			  }
			  else
			  {
				string[] arr2 = tableDefArr[2].Split('[');
				desc = arr2[0].Trim();
				units = arr2[1].Substring(0, arr2[1].LastIndexOf(']')).Trim();
			  }

			  Grib1Parameter parameter = new Grib1Parameter(this, p1, name, desc, units);
			  if (!parameter.Description.Equals("undefined", StringComparison.OrdinalIgnoreCase))
			  {
				@params[parameter.Number] = parameter;
			  }
			  logger.debug(parameter.Number + " " + parameter.Description + " " + parameter.Unit);
			}
		  }
		  return Collections.unmodifiableMap(@params); // all at once - thread safe
		}

	  }
	}

}
