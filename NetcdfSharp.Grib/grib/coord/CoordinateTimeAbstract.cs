// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;

	/// <summary>
	/// Abstract superclass for time coordinates ( time, timeIntv, time2D)
	/// Effectively Immutable
	/// 
	/// @author caron
	/// @since 1/23/14
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public abstract class CoordinateTimeAbstract implements Coordinate
	public abstract class CoordinateTimeAbstract : Coordinate
	{
		public abstract int estMemorySize();
		public abstract NetcdfSharp.ucar.nc2.util.Counters calcDistributions();
		public abstract void showCoords(NetcdfSharp.java.util.Formatter info);
		public abstract void showInfo(NetcdfSharp.java.util.Formatter info, NetcdfSharp.ucar.nc2.util.Indent indent);
		public abstract Coordinate.Type getType();
		public abstract int Size {get;}
		public abstract int getIndex(object val);
		public abstract object getValue(int idx);
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: public abstract NetcdfSharp.java.util.List<JavaToCSharpGenericWildcard> getValues();
		public abstract IList<object> Values {get;}
	  public const string MIXED_INTERVALS = "Mixed_intervals";
	  public static CalendarDateFactory cdf;

	  internal readonly string periodName; // used to create the udunit
	  protected internal readonly int code; // unit of time (Grib1 table 4, Grib2 table 4.4), eg hour, day, month
	  protected internal readonly CalendarPeriod timeUnit; // time duration, based on code
	  protected internal readonly CalendarDate refDate; // used to create the udunit
	  protected internal readonly int[] time2runtime; // for each time, which runtime is used; index into master runtime

	  protected internal string name = "time";

	  internal CoordinateTimeAbstract(int code, CalendarPeriod timeUnit, CalendarDate refDate, int[] time2runtime)
	  {
		this.code = code;
		this.timeUnit = timeUnit;
		this.refDate = (cdf == null) ? refDate : cdf.get(refDate);
		this.time2runtime = time2runtime;

		CalendarPeriod.Field cf = timeUnit.FieldProp;
		if (cf == CalendarPeriod.Field.Month || cf == CalendarPeriod.Field.Year)
		{
		  this.periodName = "calendar " + cf;
		}
		else
		{
		  this.periodName = cf.ToString();
		}
	  }

	  public virtual int Code
	  {
		  get
		  {
			return code;
		  }
	  }

	  public virtual string Unit
	  {
		  get
		  {
			return periodName;
		  }
	  }

	  public virtual string TimeUdUnit
	  {
		  get
		  {
			return periodName + " since " + refDate;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			if (!this.name.Equals("time"))
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			this.name = value;
		  }
	  }


	  public virtual CalendarDate RefDate
	  {
		  get
		  {
			return refDate;
		  }
	  }

	  public virtual CalendarPeriod TimeUnit
	  {
		  get
		  {
			return timeUnit;
		  }
	  }

	  public virtual int[] Time2runtime
	  {
		  get
		  {
			return time2runtime;
		  }
	  }

	  public virtual int getMasterRuntimeIndex(int timeIdx)
	  {
		if (time2runtime == null)
		{
		  return -1;
		}
		if (timeIdx < 0 || timeIdx >= time2runtime.Length)
		{
		  return -1;
		}
		return time2runtime[timeIdx];
	  }

	  public virtual int NCoords
	  {
		  get
		  {
			return Size;
		  }
	  }

	  public virtual double getOffsetInTimeUnits(CalendarDate start)
	  {
		return timeUnit.getOffset(start, RefDate);
	  }

	  public override string ToString()
	  {
		return name;
	  }

	  ////////////////////////////////////////

	  /// <summary>
	  /// Implements coverting a "complete best" to a "monotonic best".
	  /// The reftime is not allowed to decrease
	  /// </summary>
	  /// <returns> "monotonic best" CoordinateTimeAbstract, based on this one, which is a "complete best" </returns>
	  public virtual CoordinateTimeAbstract makeBestFromComplete()
	  {
		int[] best = new int[time2runtime.Length];
		int last = -1;
		int count = 0;
		for (int i = 0; i < time2runtime.Length; i++)
		{
		  int time = time2runtime[i];
		  if (time >= last)
		  {
			last = time;
			best[i] = time;
			count++;
		  }
		  else
		  {
			best[i] = -1;
		  }
		}
		return makeBestFromComplete(best, count);
	  }

	  protected internal abstract CoordinateTimeAbstract makeBestFromComplete(int[] best, int n);

	  public abstract CalendarDateRange makeCalendarDateRange(NetcdfSharp.ucar.nc2.time.Calendar cal);

	}

}
