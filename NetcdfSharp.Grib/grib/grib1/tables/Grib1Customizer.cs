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
	using TimeUnitConverter = NetcdfSharp.thredds.featurecollection.TimeUnitConverter;
	using NetcdfSharp.ucar.nc2.grib;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using NetcdfSharp.ucar.nc2.grib.grib1;
	using CommonCodeTable = NetcdfSharp.ucar.nc2.wmo.CommonCodeTable;

	/// <summary>
	/// Interprets grib1 info in a way that may be customized.
	/// This class handles the default case, using only standard WMO tables.
	/// Subclasses override as needed.
	/// 
	/// Bit of a contradiction, since getParamter() allows different center, subcenter, version (the version is for sure
	/// needed)
	/// But other tables are fixed by center.
	/// 
	/// @author caron
	/// @since 1/13/12
	/// </summary>
	public class Grib1Customizer : GribTables
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib1Customizer));

	  public static Grib1Customizer factory(Grib1Record proto, Grib1ParamTables tables)
	  {
		int center = proto.PDSsection.Center;
		int subcenter = proto.PDSsection.SubCenter;
		int version = proto.PDSsection.TableVersion;
		return factory(center, subcenter, version, tables);
	  }

	  public static Grib1Customizer factory(int center, int subcenter, int version, Grib1ParamTables tables)
	  {
		switch (center)
		{
		  case 7:
			return new NcepTables(tables);
		  case 9:
			return new NcepRfcTables(tables);
		  case 34:
			return new JmaTables(tables);
		  case 57:
			return new AfwaTables(tables);
		  case 58:
			return new FnmocTables(tables);
		  case 60:
			return new NcarTables(tables);
		  default:
			return new Grib1Customizer(center, tables);
		}
	  }

	  public static string getSubCenterNameStatic(int center, int subcenter)
	  {
		Grib1Customizer cust = Grib1Customizer.factory(center, subcenter, 0, null);
		return cust.getSubCenterName(subcenter);
	  }

	  ///////////////////////////////////////
	  private readonly int center;
	  private readonly Grib1ParamTables tables;

	  protected internal Grib1Customizer(int center, Grib1ParamTables tables)
	  {
		this.center = center;
		this.tables = (tables == null) ? new Grib1ParamTables() : tables;

		lock (typeof(Grib1Customizer))
		{
		  if (wmoTable3 == null)
		  {
			wmoTable3 = readTable3("resources/grib1/wmoTable3.xml");
		  }
		}
	  }

	  public virtual int Center
	  {
		  get
		  {
			return center;
		  }
	  }

	  public virtual Grib1Parameter getParameter(int center, int subcenter, int tableVersion, int param_number)
	  {
		return tables.getParameter(center, subcenter, tableVersion, param_number);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getGeneratingProcessName(int genProcess)
	  public virtual string getGeneratingProcessName(int genProcess)
	  {
		return null;
	  }

	  public virtual string getGeneratingProcessTypeName(int genProcess)
	  {
		return null;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getSubCenterName(int subcenter)
	  public virtual string getSubCenterName(int subcenter)
	  {
		return CommonCodeTable.getSubCenterName(center, subcenter);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getSubCenterName(int center, int subcenter)
	  public virtual string getSubCenterName(int center, int subcenter)
	  {
		return CommonCodeTable.getSubCenterName(center, subcenter);
	  }

	  ///////////////////////////////////////////////////
	  // time

	  public virtual Grib1ParamTime getParamTime(Grib1SectionProductDefinition pds)
	  {
		return new Grib1ParamTime(this, pds);
	  }

	  // code table 5
	  public virtual string getTimeTypeName(int timeRangeIndicator)
	  {
		return Grib1ParamTime.getTimeTypeName(timeRangeIndicator);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public GribStatType getStatType(int timeRangeIndicator)
	  public virtual GribStatType getStatType(int timeRangeIndicator)
	  {
		return Grib1WmoTimeType.getStatType(timeRangeIndicator);
	  }

	  /////////////////////////////////////////
	  // level

	  public virtual Grib1ParamLevel getParamLevel(Grib1SectionProductDefinition pds)
	  {
		return new Grib1ParamLevel(this, pds);
	  }

	  public virtual VertCoordType getVertUnit(int code)
	  {
		return makeVertUnit(code);
	  }

	  public virtual bool isVerticalCoordinate(int levelType)
	  {
		return !string.ReferenceEquals(getLevelUnits(levelType), null);
	  }

	  // below are the methods a subclass may need to override for levels

	  protected internal virtual VertCoordType makeVertUnit(int code)
	  {
		return getLevelType(code);
	  }

	  public virtual string getLevelNameShort(int levelType)
	  {
		VertCoordType lt = getLevelType(levelType);
		string result = lt.Abbrev;
		if (string.ReferenceEquals(result, null))
		{
		  result = "unknownLevel" + levelType;
		}
		return result;
	  }

	  public virtual string getLevelDescription(int levelType)
	  {
		VertCoordType lt = getLevelType(levelType);
		return lt.Desc;
	  }

	  public virtual bool isLayer(int levelType)
	  {
		VertCoordType lt = getLevelType(levelType);
		return lt.Layer;
	  }

	  // only for 3D
	  public virtual bool isPositiveUp(int levelType)
	  {
		VertCoordType lt = getLevelType(levelType);
		return lt.PositiveUp;
	  }

	  // only for 3D
	  public virtual string getLevelUnits(int levelType)
	  {
		VertCoordType lt = getLevelType(levelType);
		return lt.Units;
	  }

	  // only for 3D
	  public virtual string getLevelDatum(int levelType)
	  {
		VertCoordType lt = getLevelType(levelType);
		return lt.Datum;
	  }

	  /////////////////////////////////////////////
	  private TimeUnitConverter timeUnitConverter;

	  public virtual TimeUnitConverter TimeUnitConverter
	  {
		  set
		  {
			this.timeUnitConverter = value;
		  }
	  }

	  public virtual int convertTimeUnit(int timeUnit)
	  {
		if (timeUnitConverter == null)
		{
		  return timeUnit;
		}
		return timeUnitConverter.convertTimeUnit(timeUnit);
	  }

	  ////////////////////////////////////////////////////////////////////////

	  private static IDictionary<int, VertCoordType> wmoTable3; // shared by all instances

	  protected internal virtual VertCoordType getLevelType(int code)
	  {
		VertCoordType result;
		wmoTable3.TryGetValue(code, out result);
		if (result == null)
		{
		  result = new VertCoordType(code, "unknownLayer" + code, null, "unknownLayer" + code, null, false, false);
		}
		return result;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable protected synchronized NetcdfSharp.java.util.Map<int, NetcdfSharp.ucar.nc2.grib.coord.VertCoordType> readTable3(String path)
	  protected internal virtual IDictionary<int, VertCoordType> readTable3(string path)
	  {
		  lock (this)
		  {
			try
			{
					using (Stream @is = GribResourceReader.getInputStream(path))
					{
				  SAXBuilder builder = new SAXBuilder();
				  builder.setExpandEntities(false);
				  NetcdfSharp.org.jdom2.Document doc = builder.build(@is);
				  Element root = doc.getRootElement();

				  IDictionary<int, VertCoordType> result = new Dictionary<int, VertCoordType>(200);
				  IList<Element> @params = root.getChildren("parameter");
				  foreach (Element elem1 in @params)
				  {
					int code = int.Parse(elem1.getAttributeValue("code"));
					string desc = elem1.getChildText("description");
					string abbrev = elem1.getChildText("abbrev");
					string units = elem1.getChildText("units");
					string datum = elem1.getChildText("datum");
					bool isLayer = elem1.getChild("isLayer") != null;
					bool isPositiveUp = elem1.getChild("isPositiveUp") != null;
					VertCoordType lt = new VertCoordType(code, desc, abbrev, units, datum, isPositiveUp, isLayer);
					result[code] = lt;
				  }

				  return Collections.unmodifiableMap(result); // all at once - thread safe
					}
			}
			catch (Exception e) when (e is IOException || e is JDOMException)
			{
			  logger.error("Cant parse NcepLevelTypes = " + path, e);
			  return null;
			}
		  }
	  }

	}

}
