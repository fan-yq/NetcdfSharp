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
	/// Enumeration of Coordinate Axis types. These are used for tagging georeferencing axes.
	/// Do not change the ordering of these enums, as they are used in protobuf messages, only add new ones onto the end.
	/// TODO: remove dependency on ordering.
	/// </summary>
	public sealed class AxisType
	{
	  public static readonly AxisType RunTime = new AxisType("RunTime", InnerEnum.RunTime, 0, "R"); // runtime / reference time
	  public static readonly AxisType Ensemble = new AxisType("Ensemble", InnerEnum.Ensemble, 1, "E");
	  public static readonly AxisType Time = new AxisType("Time", InnerEnum.Time, 2, "T");
	  public static readonly AxisType GeoX = new AxisType("GeoX", InnerEnum.GeoX, 5, "X");
	  public static readonly AxisType GeoY = new AxisType("GeoY", InnerEnum.GeoY, 4, "Y");
	  public static readonly AxisType GeoZ = new AxisType("GeoZ", InnerEnum.GeoZ, 3, "Z"); // typically "dimensionless" vertical
																				// coordinate
	  public static readonly AxisType Lat = new AxisType("Lat", InnerEnum.Lat, 4, "Y");
	  public static readonly AxisType Lon = new AxisType("Lon", InnerEnum.Lon, 5, "X");
	  public static readonly AxisType Height = new AxisType("Height", InnerEnum.Height, 3, "Z"); // vertical height coordinate
	  public static readonly AxisType Pressure = new AxisType("Pressure", InnerEnum.Pressure, 3, "Z"); // vertical pressure coordinate
	  public static readonly AxisType RadialAzimuth = new AxisType("RadialAzimuth", InnerEnum.RadialAzimuth, 7);
	  public static readonly AxisType RadialDistance = new AxisType("RadialDistance", InnerEnum.RadialDistance, 8);
	  public static readonly AxisType RadialElevation = new AxisType("RadialElevation", InnerEnum.RadialElevation, 6);
	  public static readonly AxisType Spectral = new AxisType("Spectral", InnerEnum.Spectral, 1);
	  public static readonly AxisType TimeOffset = new AxisType("TimeOffset", InnerEnum.TimeOffset, 1, "TO"); // time offset from runtime
																								 // reference time
	  public static readonly AxisType Dimension = new AxisType("Dimension", InnerEnum.Dimension, 99, "Dim"); // used for dimension axis (experimental);
	  public static readonly AxisType SimpleGeometryX = new AxisType("SimpleGeometryX", InnerEnum.SimpleGeometryX, 100, "SgX"); // Simple Geometry X
	  public static readonly AxisType SimpleGeometryY = new AxisType("SimpleGeometryY", InnerEnum.SimpleGeometryY, 101, "SgY"); // Simple Geometry Y
	  public static readonly AxisType SimpleGeometryZ = new AxisType("SimpleGeometryZ", InnerEnum.SimpleGeometryZ, 102, "SgZ"); // Simple Geometry Z
	  public static readonly AxisType SimpleGeometryID = new AxisType("SimpleGeometryID", InnerEnum.SimpleGeometryID, 103, "SgID"); // Simple Geometry ID Axis, used for indexing simple geometry variables

	  private static readonly List<AxisType> valueList = new List<AxisType>();

	  static AxisType()
	  {
		  valueList.Add(RunTime);
		  valueList.Add(Ensemble);
		  valueList.Add(Time);
		  valueList.Add(GeoX);
		  valueList.Add(GeoY);
		  valueList.Add(GeoZ);
		  valueList.Add(Lat);
		  valueList.Add(Lon);
		  valueList.Add(Height);
		  valueList.Add(Pressure);
		  valueList.Add(RadialAzimuth);
		  valueList.Add(RadialDistance);
		  valueList.Add(RadialElevation);
		  valueList.Add(Spectral);
		  valueList.Add(TimeOffset);
		  valueList.Add(Dimension);
		  valueList.Add(SimpleGeometryX);
		  valueList.Add(SimpleGeometryY);
		  valueList.Add(SimpleGeometryZ);
		  valueList.Add(SimpleGeometryID);
	  }

	  public enum InnerEnum
	  {
		  RunTime,
		  Ensemble,
		  Time,
		  GeoX,
		  GeoY,
		  GeoZ,
		  Lat,
		  Lon,
		  Height,
		  Pressure,
		  RadialAzimuth,
		  RadialDistance,
		  RadialElevation,
		  Spectral,
		  TimeOffset,
		  Dimension,
		  SimpleGeometryX,
		  SimpleGeometryY,
		  SimpleGeometryZ,
		  SimpleGeometryID
	  }

	  public readonly InnerEnum innerEnumValue;
	  private readonly string nameValue;
	  private readonly int ordinalValue;
	  private static int nextOrdinal = 0;

	  // canonical ordering runTime - ensemble - time - z - y - x or elev - azimuth - distance
	  private readonly int order;
	  // X, Y, Z, T from http://cf-pcmdi.llnl.gov/documents/cf-conventions/1.6/cf-conventions.html#coordinate-types
	  private readonly string cfAxisName;

	  internal AxisType(string name, InnerEnum innerEnum, int order)
	  {
		this.order = order;
		this.cfAxisName = null;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  internal AxisType(string name, InnerEnum innerEnum, int order, string cfAxisName)
	  {
		this.order = order;
		this.cfAxisName = cfAxisName;

		  nameValue = name;
		  ordinalValue = nextOrdinal++;
		  innerEnumValue = innerEnum;
	  }

	  /// <summary>
	  /// Find the AxisType that matches this name.
	  /// </summary>
	  /// <param name="name"> match this name </param>
	  /// <returns> AxisType or null if no match. </returns>
	  public static AxisType getType(string name)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  return null;
		}
		try
		{
		  return valueOf(name);
		}
		catch (ArgumentException)
		{ // lame!
		  return null;
		}
	  }

	  /// <summary>
	  /// axis ordering: runTime - ensemble - time - z - y - x or elev - azimuth - distance
	  /// </summary>
	  /// <returns> order </returns>
	  public int axisOrder()
	  {
		return order;
	  }

	  public string CFAxisName
	  {
		  get
		  {
			return cfAxisName;
		  }
	  }

	  public bool isHoriz()
	  {
		return this == GeoX || this == GeoY || this == Lat || this == Lon;
	  }

	  public bool isTime()
	  {
		return this == Time || this == RunTime || this == TimeOffset;
	  }

	  public bool isVert()
	  {
		return this == Height || this == Pressure || this == GeoZ;
	  }


		public static AxisType[] values()
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

		public static AxisType valueOf(string name)
		{
			foreach (AxisType enumInstance in AxisType.valueList)
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
