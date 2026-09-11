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

namespace NetcdfSharp.ucar.nc2.grib
{
	using CF = NetcdfSharp.ucar.nc2.constants.CF;

	/// <summary>
	/// Grib1 derived from Code table 5 : Time range indicator
	/// Grib2 code table 4.10: Statistical process used to calculate the processed field from the field at each time
	/// increment during the time range
	/// These are the standard WMO tables only.
	/// 
	/// @author caron
	/// @since 1/17/12
	/// </summary>
	public sealed class GribStatType
	{
	  public static readonly GribStatType Average = new GribStatType("Average", InnerEnum.Average);
	  public static readonly GribStatType Accumulation = new GribStatType("Accumulation", InnerEnum.Accumulation);
	  public static readonly GribStatType Maximum = new GribStatType("Maximum", InnerEnum.Maximum);
	  public static readonly GribStatType Minimum = new GribStatType("Minimum", InnerEnum.Minimum);
	  public static readonly GribStatType DifferenceFromEnd = new GribStatType("DifferenceFromEnd", InnerEnum.DifferenceFromEnd);
	  public static readonly GribStatType RootMeanSquare = new GribStatType("RootMeanSquare", InnerEnum.RootMeanSquare);
	  public static readonly GribStatType StandardDeviation = new GribStatType("StandardDeviation", InnerEnum.StandardDeviation);
	  public static readonly GribStatType Covariance = new GribStatType("Covariance", InnerEnum.Covariance);
	  public static readonly GribStatType DifferenceFromStart = new GribStatType("DifferenceFromStart", InnerEnum.DifferenceFromStart);
	  public static readonly GribStatType Ratio = new GribStatType("Ratio", InnerEnum.Ratio);
	  public static readonly GribStatType Variance = new GribStatType("Variance", InnerEnum.Variance);

	  private static readonly List<GribStatType> valueList = new List<GribStatType>();

	  static GribStatType()
	  {
		  valueList.Add(Average);
		  valueList.Add(Accumulation);
		  valueList.Add(Maximum);
		  valueList.Add(Minimum);
		  valueList.Add(DifferenceFromEnd);
		  valueList.Add(RootMeanSquare);
		  valueList.Add(StandardDeviation);
		  valueList.Add(Covariance);
		  valueList.Add(DifferenceFromStart);
		  valueList.Add(Ratio);
		  valueList.Add(Variance);
	  }

	  public enum InnerEnum
	  {
		  Average,
		  Accumulation,
		  Maximum,
		  Minimum,
		  DifferenceFromEnd,
		  RootMeanSquare,
		  StandardDeviation,
		  Covariance,
		  DifferenceFromStart,
		  Ratio,
		  Variance
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  private GribStatType(string name, InnerEnum innerEnum)
	  {
		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  // (code table 4.10) Statistical process used to calculate the processed field from the field at each time increment
	  // during the time range
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static GribStatType getStatTypeFromGrib2(int grib2StatCode)
	  public static GribStatType getStatTypeFromGrib2(int grib2StatCode)
	  {
		switch (grib2StatCode)
		{
		  case 0:
			return GribStatType.Average;
		  case 1:
			return GribStatType.Accumulation;
		  case 2:
			return GribStatType.Maximum;
		  case 3:
			return GribStatType.Minimum;
		  case 4:
			return GribStatType.DifferenceFromEnd;
		  case 5:
			return GribStatType.RootMeanSquare;
		  case 6:
			return GribStatType.StandardDeviation;
		  case 7:
			return GribStatType.Covariance;
		  case 8:
			return GribStatType.DifferenceFromStart;
		  case 9:
			return GribStatType.Ratio;
		  default:
			return null;
		}
	  }

	  /*
	   * public static String getStatTypeDescription(GribStatType statType) {
	   * switch (statType) {
	   * case Average:
	   * return "Average";
	   * case Accumulation:
	   * return "Accumulation";
	   * case Maximum:
	   * return "Maximum";
	   * case Minimum:
	   * return "Minimum";
	   * case DifferenceFromEnd:
	   * return "Difference (Value at the end of time range minus value at the beginning)";
	   * case RootMeanSquare:
	   * return "RootMeanSquare";
	   * case StandardDeviation:
	   * return "StandardDeviation";
	   * case Covariance:
	   * return "Covariance (Temporal variance)";
	   * case DifferenceFromStart:
	   * return "Difference (Value at the start of time range minus value at the end)";
	   * case Ratio:
	   * return "Ratio";
	   * default:
	   * return "UnknownIntervalType-" + statType;
	   * }
	   * }
	   */

	  public static int getStatTypeNumber(string name)
	  {
		if (name.StartsWith("Average", StringComparison.Ordinal))
		{
		  return 0;
		}
		if (name.StartsWith("Accumulation", StringComparison.Ordinal))
		{
		  return 1;
		}
		if (name.StartsWith("Maximum", StringComparison.Ordinal))
		{
		  return 2;
		}
		if (name.StartsWith("Minimum", StringComparison.Ordinal))
		{
		  return 3;
		}
		if (name.StartsWith("Difference (Value at the end", StringComparison.Ordinal))
		{
		  return 4;
		}
		if (name.StartsWith("Root", StringComparison.Ordinal))
		{
		  return 5;
		}
		if (name.StartsWith("Standard", StringComparison.Ordinal))
		{
		  return 6;
		}
		if (name.StartsWith("Covariance", StringComparison.Ordinal))
		{
		  return 7;
		}
		if (name.StartsWith("Difference (Value at the start", StringComparison.Ordinal))
		{
		  return 8;
		}
		if (name.StartsWith("Ratio", StringComparison.Ordinal))
		{
		  return 9;
		}
		if (name.StartsWith("Variance", StringComparison.Ordinal))
		{
		  return 10;
		}
		return -1;
	  }

	  /// <summary>
	  /// Convert StatType to CF.CellMethods
	  /// </summary>
	  /// <param name="stat"> the GRIB1 statistical type </param>
	  /// <returns> equivalent CF, or null </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static NetcdfSharp.ucar.nc2.constants.CF.CellMethods getCFCellMethod(GribStatType stat)
	  public static NetcdfSharp.ucar.nc2.constants.CF.CellMethods getCFCellMethod(GribStatType stat)
	  {
		switch (stat.innerEnumValue)
		{
		  case NetcdfSharp.ucar.nc2.grib.GribStatType.InnerEnum.Average:
			return CF.CellMethods.mean;
		  case NetcdfSharp.ucar.nc2.grib.GribStatType.InnerEnum.Accumulation:
			return CF.CellMethods.sum;
		  case NetcdfSharp.ucar.nc2.grib.GribStatType.InnerEnum.Covariance:
			return CF.CellMethods.variance;
		  case NetcdfSharp.ucar.nc2.grib.GribStatType.InnerEnum.Minimum:
			return CF.CellMethods.minimum;
		  case NetcdfSharp.ucar.nc2.grib.GribStatType.InnerEnum.Maximum:
			return CF.CellMethods.maximum;
		  case NetcdfSharp.ucar.nc2.grib.GribStatType.InnerEnum.StandardDeviation:
			return CF.CellMethods.standard_deviation;
		  case NetcdfSharp.ucar.nc2.grib.GribStatType.InnerEnum.Variance:
			return CF.CellMethods.variance;
		  default:
			return null;
		}
	  }


		public static GribStatType[] values()
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

		public static GribStatType valueOf(string name)
		{
			foreach (GribStatType enumInstance in GribStatType.valueList)
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
