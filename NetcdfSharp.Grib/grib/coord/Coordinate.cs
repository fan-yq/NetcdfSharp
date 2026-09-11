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

namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using AxisType = NetcdfSharp.ucar.nc2.constants.AxisType;
	using Counters = NetcdfSharp.ucar.nc2.util.Counters;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// Abstract coordinate
	/// 
	/// @author caron
	/// @since 11/24/13
	/// </summary>
	public interface Coordinate
	{
	  /// <summary>
	  /// Coordinate types </summary>
	  public sealed class Type
	  {
		public static readonly Type runtime = new Type("runtime", InnerEnum.runtime, 0, NetcdfSharp.ucar.nc2.constants.AxisType.RunTime);
		public static readonly Type time = new Type("time", InnerEnum.time, 1, NetcdfSharp.ucar.nc2.constants.AxisType.Time);
		public static readonly Type timeIntv = new Type("timeIntv", InnerEnum.timeIntv, 1, NetcdfSharp.ucar.nc2.constants.AxisType.Time);
		public static readonly Type vert = new Type("vert", InnerEnum.vert, 3, NetcdfSharp.ucar.nc2.constants.AxisType.Height);
		public static readonly Type time2D = new Type("time2D", InnerEnum.time2D, 1, NetcdfSharp.ucar.nc2.constants.AxisType.TimeOffset);
		public static readonly Type ens = new Type("ens", InnerEnum.ens, 2, NetcdfSharp.ucar.nc2.constants.AxisType.Ensemble); // cant change order, protobuf uses

		private static readonly List<Type> valueList = new List<Type>();

		static Type()
		{
			valueList.Add(runtime);
			valueList.Add(time);
			valueList.Add(timeIntv);
			valueList.Add(vert);
			valueList.Add(time2D);
			valueList.Add(ens);
		}

		public enum InnerEnum
		{
			runtime,
			time,
			timeIntv,
			vert,
			time2D,
			ens
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;
																						 // the ordinal

		public readonly int order;
		public readonly NetcdfSharp.ucar.nc2.constants.AxisType axisType;

		internal Type(string name, InnerEnum innerEnum, int order, NetcdfSharp.ucar.nc2.constants.AxisType axisType)
		{
		  this.order = order;
		  this.axisType = axisType;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		  public static Type[] values()
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

		  public static Type valueOf(string name)
		  {
			  foreach (Type enumInstance in Type.valueList)
			  {
				  if (enumInstance.nameValue == name)
				  {
					  return enumInstance;
				  }
			  }
			  throw new ArgumentException(name);
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: NetcdfSharp.java.util.List<?> getValues();
	  IList<object> Values {get;}

	  object getValue(int idx); // get the ith value

	  int getIndex(object val); // LOOK assumes the values are unique;

	  int Size {get;}

	  int Code {get;}

	  Type getType();

	  string Name {get;}

	  string Unit {get;}

	  int NCoords {get;}

	  void showInfo(Formatter info, Indent indent);

	  void showCoords(Formatter info);

	  Counters calcDistributions();

	  int estMemorySize(); // estimated memory size in bytes (debugging)
	}

}
