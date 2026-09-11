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

namespace NetcdfSharp.ucar.nc2.grib.grib1.tables
{
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using GribResourceReader = NetcdfSharp.ucar.nc2.grib.GribResourceReader;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;

	/// <summary>
	/// FNMOC local tables
	/// 
	/// @author caron
	/// @since 1/14/12
	/// </summary>
	public class FnmocTables : Grib1Customizer
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(FnmocTables));
	  private const string fnmocTableA = "resources/grib1/fnmoc/US058MMTA-ALPdoc.pntabs-prodname-masterModelTableOrdered.GRIB1.TblA.xml";
	  private const string fnmocTable2 = "resources/grib1/fnmoc/US058MMTA-ALPdoc.pntabs-prodname-masterParameterTableOrdered.GRIB1.Tbl2.xml";
	  private const string fnmocTable3 = "resources/grib1/fnmoc/US058MMTA-ALPdoc.pntabs-prodname-masterLevelTypeTableOrdered.GRIB1.Tbl3.xml";

	  private static IDictionary<int, VertCoordType> levelTypesMap; // shared by all instances
	  private static IDictionary<int, string> genProcessMap; // shared by all instances

	  internal FnmocTables(Grib1ParamTables tables) : base(58, tables)
	  {
	  }

	  // genProcess

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getGeneratingProcessName(int genProcess)
	  public override string getGeneratingProcessName(int genProcess)
	  {
		if (genProcessMap == null)
		{
		  genProcessMap = readGenProcess(fnmocTableA);
		}
		if (genProcessMap == null)
		{
		  return null;
		}
		return genProcessMap[genProcess];
	  }

	  /*
	   * <fnmocTable>
	   * <entry>
	   * <grib1Id>0004</grib1Id>
	   * <fnmocId>0004</fnmocId>
	   * <name>MISC_GRIDS</name>
	   * <fullName>Miscellaneous Grids</fullName>
	   * <description>atmospheric model</description>
	   * <status>current</status>
	   * </entry>
	   * <entry>
	   * <grib1Id>0008</grib1Id>
	   * <fnmocId>0008</fnmocId>
	   * <name>STRATO</name>
	   * <fullName>NOGAPS Stratosphere Functions</fullName>
	   * <description>atmospheric stratosphere model</description>
	   * <status>current</status>
	   * </entry>
	   */
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable private NetcdfSharp.java.util.Map<int, String> readGenProcess(String path)
	  private IDictionary<int, string> readGenProcess(string path)
	  {
		try
		{
				using (Stream @is = GribResourceReader.getInputStream(path))
				{
			  SAXBuilder builder = new SAXBuilder();
			  builder.setExpandEntities(false);
			  NetcdfSharp.org.jdom2.Document doc = builder.build(@is);
			  Element root = doc.getRootElement();

			  IDictionary<int, string> result = new Dictionary<int, string>(200);
			  Element fnmocTable = root.getChild("fnmocTable");
			  IList<Element> @params = fnmocTable.getChildren("entry");
			  foreach (Element elem1 in @params)
			  {
				int code = int.Parse(elem1.getChildText("grib1Id"));
				string desc = elem1.getChildText("fullName");
				result[code] = desc;
			  }

			  return Collections.unmodifiableMap(result); // all at once - thread safe

				}
		}
		catch (Exception e) when (e is IOException || e is JDOMException)
		{
		  logger.error("Cant read FNMOC Table 1 = " + path, e);
		  return null;
		}
	  }


	  /// levels
	  protected internal override VertCoordType getLevelType(int code)
	  {
		if (levelTypesMap == null)
		{
		  levelTypesMap = readFnmocTable3(fnmocTable3);
		}
		if (levelTypesMap == null)
		{
		  return base.getLevelType(code);
		}

		VertCoordType levelType;
		levelTypesMap.TryGetValue(code, out levelType);
		if (levelType != null)
		{
		  return levelType;
		}
		return base.getLevelType(code);
	  }

	  /*
	   * <entry>
	   * <grib1Id>222</grib1Id>
	   * <fnmocId>mid_cld</fnmocId>
	   * <name>mid_cld</name>
	   * <description>middle cloud layer</description>
	   * <status>deprecated</status>
	   * </entry>
	   * 
	   * <fnmocTable>
	   * <entry>
	   * <grib1Id>001</grib1Id>
	   * <fnmocId>surface</fnmocId>
	   * <name>surface</name>
	   * <description>ground or water surface (the atmosphere's lower boundary, land/sea surface)</description>
	   * <status>current</status>
	   * </entry>
	   */
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable private NetcdfSharp.java.util.HashMap<int, NetcdfSharp.ucar.nc2.grib.coord.VertCoordType> readFnmocTable3(String path)
	  private Dictionary<int, VertCoordType> readFnmocTable3(string path)
	  {
		try
		{
				using (Stream @is = GribResourceReader.getInputStream(path))
				{
			  SAXBuilder builder = new SAXBuilder();
			  builder.setExpandEntities(false);
			  NetcdfSharp.org.jdom2.Document doc = builder.build(@is);
			  Element root = doc.getRootElement();

			  Dictionary<int, VertCoordType> result = new Dictionary<int, VertCoordType>(200);
			  Element fnmocTable = root.getChild("fnmocTable");
			  IList<Element> @params = fnmocTable.getChildren("entry");
			  foreach (Element elem1 in @params)
			  {
				int code = int.Parse(elem1.getChildText("grib1Id"));
				if (code < 129)
				{
				  continue;
				}
				string desc = elem1.getChildText("description");
				string abbrev = elem1.getChildText("name");
				string units = elem1.getChildText("units");
				if (string.ReferenceEquals(units, null))
				{
				  units = (code == 219) ? "Pa" : "";
				}
				string datum = elem1.getChildText("datum");
				bool isLayer = elem1.getChild("isLayer") != null;
				bool isPositiveUp = elem1.getChild("isPositiveUp") != null;
				VertCoordType lt = new VertCoordType(code, desc, abbrev, units, datum, isPositiveUp, isLayer);
				result[code] = lt;
			  }

			  return result; // all at once - thread safe

				}
		}
		catch (Exception e) when (e is IOException || e is JDOMException)
		{
		  logger.error("Cant read FnmocTable3 = " + path, e);
		  return null;
		}
	  }

	}

}
