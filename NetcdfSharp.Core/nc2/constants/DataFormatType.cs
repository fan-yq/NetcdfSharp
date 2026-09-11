// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.constants
{

	/// <summary>
	/// Redo NetcdfSharp.thredds.catalog.DataFormatType as enum in order to break dependency of ucar.nc2 on server catalog </summary>
	public sealed class DataFormatType
	{
	  public static readonly DataFormatType BUFR = new DataFormatType("BUFR", InnerEnum.BUFR, null);
	  public static readonly DataFormatType ESML = new DataFormatType("ESML", InnerEnum.ESML, null);
	  public static readonly DataFormatType GEMPAK = new DataFormatType("GEMPAK", InnerEnum.GEMPAK, null);
	  public static readonly DataFormatType GINI = new DataFormatType("GINI", InnerEnum.GINI, null);
	  public static readonly DataFormatType GRIB1 = new DataFormatType("GRIB1", InnerEnum.GRIB1, "GRIB-1");
	  public static readonly DataFormatType GRIB2 = new DataFormatType("GRIB2", InnerEnum.GRIB2, "GRIB-2");
	  public static readonly DataFormatType HDF4 = new DataFormatType("HDF4", InnerEnum.HDF4, null);
	  public static readonly DataFormatType HDF5 = new DataFormatType("HDF5", InnerEnum.HDF5, null);
	  public static readonly DataFormatType MCIDAS_AREA = new DataFormatType("MCIDAS_AREA", InnerEnum.MCIDAS_AREA, "McIDAS-AREA");
	  public static readonly DataFormatType NCML = new DataFormatType("NCML", InnerEnum.NCML, "NcML");
	  public static readonly DataFormatType NETCDF = new DataFormatType("NETCDF", InnerEnum.NETCDF, "NetCDF");
	  public static readonly DataFormatType NETCDF4 = new DataFormatType("NETCDF4", InnerEnum.NETCDF4, "NetCDF-4");
	  public static readonly DataFormatType NEXRAD2 = new DataFormatType("NEXRAD2", InnerEnum.NEXRAD2, null);
	  public static readonly DataFormatType NIDS = new DataFormatType("NIDS", InnerEnum.NIDS, null);

	  public static readonly DataFormatType GIF = new DataFormatType("GIF", InnerEnum.GIF, "image/gif");
	  public static readonly DataFormatType JPEG = new DataFormatType("JPEG", InnerEnum.JPEG, "image/jpeg");
	  public static readonly DataFormatType TIFF = new DataFormatType("TIFF", InnerEnum.TIFF, "image/tiff");

	  public static readonly DataFormatType CSV = new DataFormatType("CSV", InnerEnum.CSV, "text/csv");
	  public static readonly DataFormatType HTML = new DataFormatType("HTML", InnerEnum.HTML, "text/html");
	  public static readonly DataFormatType PLAIN = new DataFormatType("PLAIN", InnerEnum.PLAIN, "text/plain");
	  public static readonly DataFormatType TSV = new DataFormatType("TSV", InnerEnum.TSV, "text/tab-separated-values");
	  public static readonly DataFormatType XML = new DataFormatType("XML", InnerEnum.XML, "text/xml");

	  public static readonly DataFormatType MPEG = new DataFormatType("MPEG", InnerEnum.MPEG, "video/mpeg");
	  public static readonly DataFormatType QUICKTIME = new DataFormatType("QUICKTIME", InnerEnum.QUICKTIME, "video/quicktime");
	  public static readonly DataFormatType REALTIME = new DataFormatType("REALTIME", InnerEnum.REALTIME, "video/realtime");

	  private static readonly List<DataFormatType> valueList = new List<DataFormatType>();

	  static DataFormatType()
	  {
		  valueList.Add(BUFR);
		  valueList.Add(ESML);
		  valueList.Add(GEMPAK);
		  valueList.Add(GINI);
		  valueList.Add(GRIB1);
		  valueList.Add(GRIB2);
		  valueList.Add(HDF4);
		  valueList.Add(HDF5);
		  valueList.Add(MCIDAS_AREA);
		  valueList.Add(NCML);
		  valueList.Add(NETCDF);
		  valueList.Add(NETCDF4);
		  valueList.Add(NEXRAD2);
		  valueList.Add(NIDS);
		  valueList.Add(GIF);
		  valueList.Add(JPEG);
		  valueList.Add(TIFF);
		  valueList.Add(CSV);
		  valueList.Add(HTML);
		  valueList.Add(PLAIN);
		  valueList.Add(TSV);
		  valueList.Add(XML);
		  valueList.Add(MPEG);
		  valueList.Add(QUICKTIME);
		  valueList.Add(REALTIME);
	  }

	  public enum InnerEnum
	  {
		  BUFR,
		  ESML,
		  GEMPAK,
		  GINI,
		  GRIB1,
		  GRIB2,
		  HDF4,
		  HDF5,
		  MCIDAS_AREA,
		  NCML,
		  NETCDF,
		  NETCDF4,
		  NEXRAD2,
		  NIDS,
		  GIF,
		  JPEG,
		  TIFF,
		  CSV,
		  HTML,
		  PLAIN,
		  TSV,
		  XML,
		  MPEG,
		  QUICKTIME,
		  REALTIME
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private readonly string desc;

	  internal DataFormatType(string name, InnerEnum innerEnum, string desc)
	  {
		this.desc = (string.ReferenceEquals(desc, null)) ? ToString() : desc;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  /// <summary>
	  /// case insensitive name lookup. </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static DataFormatType getType(String name)
	  public static DataFormatType getType(string name)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  return null;
		}
		foreach (DataFormatType m in values())
		{
		  if (m.desc.Equals(name, StringComparison.OrdinalIgnoreCase))
		  {
			return m;
		  }
		  if (m.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
		  {
			return m;
		  }
		}
		return null;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getDescription()
	  public string Description
	  {
		  get
		  {
			return desc;
		  }
	  }

		public static DataFormatType[] values()
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

		public static DataFormatType valueOf(string name)
		{
			foreach (DataFormatType enumInstance in DataFormatType.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new ArgumentException(name);
		}
	}

}
