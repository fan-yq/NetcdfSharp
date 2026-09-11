// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using AxisType = NetcdfSharp.ucar.nc2.constants.AxisType;
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using WmoParamTable = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoParamTable;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Static utilities for Grib-2
	/// 
	/// @author caron
	/// @since 3/29/11
	/// </summary>
	public class Grib2Utils
	{

	  public static string clean(string s)
	  {
		StringBuilder sb = new StringBuilder(s);
		StringUtil2.replace(sb, "/. ", "-p_");
		StringUtil2.removeAll(sb, "(),;");
		char c = sb[0];
		if (char.IsLetter(c))
		{
		  if (char.IsLower(c))
		  {
			sb[0] = char.ToUpper(c);
		  }
		}
		else
		{
		  sb.Insert(0, 'N');
		}

		return sb.ToString().Trim();
	  }

	  public static string cleanupHeader(byte[] raw)
	  {
		string result = StringUtil2.cleanup(raw);
		int pos = result.IndexOf("data", StringComparison.Ordinal);
		if (pos > 0)
		{
		  result = result.Substring(pos);
		}
		return result;
	  }

	  public static string getVariableName(Grib2Record gr)
	  {
		GribTables.Parameter p = WmoParamTable.getParameter(gr.Discipline, gr.PDS.ParameterCategory, gr.PDS.ParameterNumber);
		string s = (p == null) ? null : p.Name;
		if (string.ReferenceEquals(s, null))
		{
		  s = "U" + gr.Discipline + "-" + gr.PDS.ParameterCategory + "-" + gr.PDS.ParameterNumber;
		}
		return s;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static NetcdfSharp.ucar.nc2.time.CalendarPeriod getCalendarPeriod(int timeUnit)
	  public static CalendarPeriod getCalendarPeriod(int timeUnit)
	  {

		switch (timeUnit)
		{ // code table 4.4
		  case 0:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Minute);
		  case 1:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Hour);
		  case 2:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Day);
		  case 3:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Month);
		  case 4:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Year);
		  case 5:
			return CalendarPeriod.of(10, CalendarPeriod.Field.Year);
		  case 6:
			return CalendarPeriod.of(30, CalendarPeriod.Field.Year);
		  case 7:
			return CalendarPeriod.of(100, CalendarPeriod.Field.Year);
		  case 10:
			return CalendarPeriod.of(3, CalendarPeriod.Field.Hour);
		  case 11:
			return CalendarPeriod.of(6, CalendarPeriod.Field.Hour);
		  case 12:
			return CalendarPeriod.of(12, CalendarPeriod.Field.Hour);
		  case 13:
			return CalendarPeriod.of(1, CalendarPeriod.Field.Second);
		  default:
			return null;
		}
	  }

	  /// <summary>
	  /// Check to see if this pds is a layer variable
	  /// </summary>
	  /// <param name="pds"> record to check </param>
	  /// <returns> true if a layer </returns>
	  public static bool isLayer(Grib2Pds pds)
	  {
		return pds.LevelType2 != 255 && pds.LevelType2 != 0;
	  }

	  public static bool isLatLon(int gridTemplate, int center)
	  {
		return ((gridTemplate < 4) || ((gridTemplate >= 40) && (gridTemplate < 44)));
	  }

	  //////////////////////////////////////////////////////////////////////////////////
	  // pretty much lame stuff
	  // possibly move to Customizer

	  // check if grid template is "Curvilinear Orthogonal", (NCEP 204) methods below only used when thats true
	  public static bool isCurvilinearOrthogonal(int gridTemplate, int center)
	  {
		return ((center == 7) && (gridTemplate == 204));
	  }

	  // isLatLon2D is true, check parameter to see if its a 2D lat/lon coordinate
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static LatLon2DCoord getLatLon2DcoordType(int discipline, int category, int parameter)
	  public static LatLon2DCoord getLatLon2DcoordType(int discipline, int category, int parameter)
	  {
		if ((discipline != 0) || (category != 2) || (parameter < 198 || parameter > 203))
		{
		  return null;
		}
		switch (parameter)
		{
		  case 198:
			return LatLon2DCoord.U_Latitude;
		  case 199:
			return LatLon2DCoord.U_Longitude;
		  case 200:
			return LatLon2DCoord.V_Latitude;
		  case 201:
			return LatLon2DCoord.V_Longitude;
		  case 202:
			return LatLon2DCoord.P_Latitude;
		  case 203:
			return LatLon2DCoord.P_Longitude;
		}
		return null;
	  }

	  public enum LatLonCoordType
	  {
		U,
		V,
		P
	  }
	  public sealed class LatLon2DCoord
	  {
		public static readonly LatLon2DCoord U_Latitude = new LatLon2DCoord("U_Latitude", InnerEnum.U_Latitude);
		public static readonly LatLon2DCoord U_Longitude = new LatLon2DCoord("U_Longitude", InnerEnum.U_Longitude);
		public static readonly LatLon2DCoord V_Latitude = new LatLon2DCoord("V_Latitude", InnerEnum.V_Latitude);
		public static readonly LatLon2DCoord V_Longitude = new LatLon2DCoord("V_Longitude", InnerEnum.V_Longitude);
		public static readonly LatLon2DCoord P_Latitude = new LatLon2DCoord("P_Latitude", InnerEnum.P_Latitude);
		public static readonly LatLon2DCoord P_Longitude = new LatLon2DCoord("P_Longitude", InnerEnum.P_Longitude);

		private static readonly List<LatLon2DCoord> valueList = new List<LatLon2DCoord>();

		static LatLon2DCoord()
		{
			valueList.Add(U_Latitude);
			valueList.Add(U_Longitude);
			valueList.Add(V_Latitude);
			valueList.Add(V_Longitude);
			valueList.Add(P_Latitude);
			valueList.Add(P_Longitude);
		}

		public enum InnerEnum
		{
			U_Latitude,
			U_Longitude,
			V_Latitude,
			V_Longitude,
			P_Latitude,
			P_Longitude
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private LatLon2DCoord(string name, InnerEnum innerEnum)
		{
			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public NetcdfSharp.ucar.nc2.constants.AxisType AxisType
		{
			get
			{
			  return this.nameValue.Contains("Latitude") ? AxisType.Lat : AxisType.Lon;
			}
		}

		  public static LatLon2DCoord[] values()
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

		  public static LatLon2DCoord valueOf(string name)
		  {
			  foreach (LatLon2DCoord enumInstance in LatLon2DCoord.valueList)
			  {
				  if (enumInstance.nameValue == name)
				  {
					  return enumInstance;
				  }
			  }
			  throw new ArgumentException(name);
		  }
	  }

	  /// <summary>
	  /// This looks for snippets in the variable name/desc as to whether it wants U, V, or P 2D coordinates
	  /// </summary>
	  /// <param name="desc"> variable name/desc </param>
	  /// <returns> U, V, or P for normal variables, null for the coordinates themselves </returns>
	  public static LatLonCoordType? getLatLon2DcoordType(string desc)
	  {
		LatLonCoordType? type;
		if (desc.Contains("u-component"))
		{
		  type = LatLonCoordType.U;
		}
		else if (desc.Contains("v-component"))
		{
		  type = LatLonCoordType.V;
		}
		else if (desc.Contains("Latitude of") || desc.Contains("Longitude of"))
		{
		  type = null;
		}
		else
		{
		  type = LatLonCoordType.P;
		}
		return type;
	  }

	}

}
