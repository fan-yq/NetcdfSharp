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

namespace NetcdfSharp.ucar.nc2.time
{
	using PeriodType = NetcdfSharp.org.joda.time.PeriodType;
	using TimeDuration = NetcdfSharp.ucar.nc2.units.TimeDuration;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// A CalendarPeriod is a logical duration of time, it requires a Calendar to convert to an actual duration of time.
	/// A CalendarField is expressed as {integer x Field}.
	/// 
	/// Design follows joda Period class.
	/// 
	/// @author caron
	/// @since 3/30/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CalendarPeriod
	public class CalendarPeriod
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(CalendarPeriod));

	  private static readonly Cache<CalendarPeriod, CalendarPeriod> cache = CacheBuilder<object, object>.newBuilder().maximumSize(100).build();

	  public static readonly CalendarPeriod Hour = CalendarPeriod.of(1, Field.Hour);

	  /// <summary>
	  /// Fields that can be set on a CalendarPeriod, used to prevent exposure of underlying implementation. </summary>
	  public sealed class Field
	  {
		public static readonly Field Millisec = new Field("Millisec", InnerEnum.Millisec, NetcdfSharp.org.joda.time.PeriodType.millis());
		public static readonly Field Second = new Field("Second", InnerEnum.Second, NetcdfSharp.org.joda.time.PeriodType.seconds());
		public static readonly Field Minute = new Field("Minute", InnerEnum.Minute, NetcdfSharp.org.joda.time.PeriodType.minutes());
		public static readonly Field Hour = new Field("Hour", InnerEnum.Hour, NetcdfSharp.org.joda.time.PeriodType.hours());
		public static readonly Field Day = new Field("Day", InnerEnum.Day, NetcdfSharp.org.joda.time.PeriodType.days());
		public static readonly Field Month = new Field("Month", InnerEnum.Month, NetcdfSharp.org.joda.time.PeriodType.months());
		public static readonly Field Year = new Field("Year", InnerEnum.Year, NetcdfSharp.org.joda.time.PeriodType.years());

		private static readonly List<Field> valueList = new List<Field>();

		static Field()
		{
			valueList.Add(Millisec);
			valueList.Add(Second);
			valueList.Add(Minute);
			valueList.Add(Hour);
			valueList.Add(Day);
			valueList.Add(Month);
			valueList.Add(Year);
		}

		public enum InnerEnum
		{
			Millisec,
			Second,
			Minute,
			Hour,
			Day,
			Month,
			Year
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		internal NetcdfSharp.org.joda.time.PeriodType p;

		internal Field(string name, InnerEnum innerEnum, NetcdfSharp.org.joda.time.PeriodType p)
		{
		  this.p = p;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		  public static Field[] values()
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

		  public static Field valueOf(string name)
		  {
			  foreach (Field enumInstance in Field.valueList)
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
	  /// Convert a period string into a CalendarPeriod.Field.
	  /// </summary>
	  /// <param name="udunit"> period string </param>
	  /// <returns> CalendarPeriod.Field enum </returns>
	  /// <exception cref="IllegalArgumentException"> if not valid format </exception>
	  public static CalendarPeriod.Field fromUnitString(string udunit)
	  {
		udunit = udunit.Trim();
		udunit = udunit.ToLower();

		if (udunit.Equals("s"))
		{
		  return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Second;
		}
		if (udunit.Equals("ms"))
		{
		  return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Millisec;
		}

		// eliminate plurals
		if (udunit.EndsWith("s", StringComparison.Ordinal))
		{
		  udunit = udunit.Substring(0, udunit.Length - 1);
		}

		switch (udunit)
		{
		  case "second":
		  case "sec":
			return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Second;
		  case "millisecond":
		  case "millisec":
		  case "msec":
			return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Millisec;
		  case "minute":
		  case "min":
			return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Minute;
		  case "hour":
		  case "hr":
		  case "h":
			return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Hour;
		  case "day":
		  case "d":
			return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Day;
		  case "month":
		  case "mon":
			return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Month;
		  case "year":
		  case "yr":
			return NetcdfSharp.ucar.nc2.time.CalendarPeriod.Field.Year;
		  default:
			throw new ArgumentException("cant convert " + udunit + " to CalendarPeriod");
		}
	  }

	  // minimize memory use by interning. wacko shit in GribPartitionBuilder TimeCoordinate, whoduhthunk?
	  public static CalendarPeriod of(int value, Field field)
	  {
		CalendarPeriod want = new CalendarPeriod(value, field);
		if (cache == null)
		{
		  return want;
		}
		CalendarPeriod got = cache.getIfPresent(want);
		if (got != null)
		{
		  return got;
		}
		cache.put(want, want);
		return want;
	  }

	  /// <summary>
	  /// Convert a udunit period string into a CalendarPeriod
	  /// </summary>
	  /// <param name="udunit"> period string : "[val] unit" </param>
	  /// <returns> CalendarPeriod or null if illegal </returns>
	  public static CalendarPeriod of(string udunit)
	  {
		int value;
		string units;

		string[] split = StringUtil2.splitString(udunit);
		if (split.Length == 1)
		{
		  value = 1;
		  units = split[0];

		}
		else if (split.Length == 2)
		{
		  try
		  {
			value = int.Parse(split[0]);
		  }
		  catch (Exception)
		  {
			return null;
		  }
		  units = split[1];
		}
		else
		{
		  return null;
		}


		CalendarPeriod.Field unit = CalendarPeriod.fromUnitString(units);
		return CalendarPeriod.of(value, unit);
	  }

	  public static CalendarPeriod of(TimeDuration td)
	  {
		CalendarPeriod.Field unit = CalendarPeriod.fromUnitString(td.TimeUnit.getUnitString());
		return CalendarPeriod.of((int) td.Value, unit);
	  }

	  ////////////////////////
	  // the common case is a single field
	  private readonly int value;
	  private readonly Field jfield;

	  private CalendarPeriod(int value, Field field)
	  {
		this.value = value;
		this.jfield = field;
	  }

	  /// <summary>
	  /// Multiply the period by an integer
	  /// </summary>
	  /// <param name="value"> multiply by this </param>
	  /// <returns> new period </returns>
	  public virtual CalendarPeriod multiply(int value)
	  {
		return CalendarPeriod.of(this.value * value, this.jfield);
	  }

	  public virtual int Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual Field getField()
	  {
		return jfield;
	  }

	  /// <summary>Property form of getField() used by converter-generated call sites.</summary>
	  public virtual Field FieldProp => jfield;

	  /// <summary>
	  /// Subtract two dates, return difference in units of this period.
	  /// If not even, will round to nearest int
	  /// </summary>
	  /// <param name="start"> start date </param>
	  /// <param name="end"> end date </param>
	  /// <returns> difference in units of this period </returns>
	  public virtual int subtract(CalendarDate start, CalendarDate end)
	  {
		long diff = end.getDifferenceInMsecs(start);
		if (diff == 0)
		{
		  return 0;
		}
		int thislen = millisecs();
		int signOfDiff = (int)(diff / Math.Abs(diff));
		return signOfDiff * (int)Math.Floor(Math.Abs(diff) / (float) thislen + 0.5);
	  }

	  /// <summary>
	  /// Get the conversion factor of the other CalendarPeriod to this one
	  /// </summary>
	  /// <param name="from"> convert from this </param>
	  /// <returns> conversion factor, so that getConvertFactor(from) * from = this </returns>
	  public virtual double getConvertFactor(CalendarPeriod from)
	  {
		if (jfield == CalendarPeriod.Field.Month || jfield == CalendarPeriod.Field.Year)
		{
		  log.warn(" CalendarDate.convert on Month or Year");
		}

		return (double) from.millisecs() / millisecs();
	  }

	  /// <summary>
	  /// Get the duration in milliseconds -+
	  /// </summary>
	  /// <returns> the duration in seconds </returns>
	  /// @deprecated dont use because these are fixed length and thus approximate. 
	  public virtual double ValueInMillisecs
	  {
		  get
		  {
			if (this.jfield == CalendarPeriod.Field.Month)
			{
			  return 30.0 * 24.0 * 60.0 * 60.0 * 1000.0 * value;
			}
			else if (this.jfield == CalendarPeriod.Field.Year)
			{
			  return 365.0 * 24.0 * 60.0 * 60.0 * 1000.0 * value;
			}
			else
			{
			  return millisecs();
			}
		  }
	  }

	  private int millisecs()
	  {
		if (jfield == CalendarPeriod.Field.Millisec)
		{
		  return value;
		}
		else if (jfield == CalendarPeriod.Field.Second)
		{
		  return 1000 * value;
		}
		else if (jfield == CalendarPeriod.Field.Minute)
		{
		  return 60 * 1000 * value;
		}
		else if (jfield == CalendarPeriod.Field.Hour)
		{
		  return 60 * 60 * 1000 * value;
		}
		else if (jfield == CalendarPeriod.Field.Day)
		{
		  return 24 * 60 * 60 * 1000 * value;
		}

		else
		{
		  throw new System.InvalidOperationException("Illegal Field = " + jfield);
		}
	  }

	  /*
	   * LOOK from old TimeCoord code, which is better ??
	   * public static int getOffset(CalendarDate refDate, CalendarDate cd, CalendarPeriod timeUnit) {
	   * long msecs = cd.getDifferenceInMsecs(refDate);
	   * return (int) Math.round(msecs / timeUnit.getValueInMillisecs());
	   * }
	   */

	  // offset from start to end, in these units
	  // start + offset = end
	  // rounds to nearest int
	  public virtual int getOffset(CalendarDate start, CalendarDate end)
	  {
		if (start.Equals(end))
		{
		  return 0;
		}

		return subtract(start, end);
	  }

	  public override string ToString()
	  {
		return value + " " + jfield;
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		CalendarPeriod that = (CalendarPeriod) o;

		if (value != that.value)
		{
		  return false;
		}
		return jfield == that.jfield;

	  }

	  public override int GetHashCode()
	  {
		int result = value;
		result = 31 * result + (jfield != null ? jfield.GetHashCode() : 0);
		return result;
	  }

	}

}
