// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.time
{
	using JDateTime = NetcdfSharp.org.joda.time.JDateTime;
	using DateRange = NetcdfSharp.ucar.nc2.units.DateRange;

	/// <summary>
	/// A range of CalendarDates.
	/// 
	/// @author caron
	/// @since 3/21/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CalendarDateRange
	public class CalendarDateRange
	{
	  private readonly CalendarDate start, end;
	  private readonly JDateTime startDt, endDt;
	  private readonly Calendar cal;

	  public static CalendarDateRange of(CalendarDate start, CalendarDate end)
	  {
		return new CalendarDateRange(start, end);
	  }

	  public static CalendarDateRange of(JDateTime start, JDateTime end)
	  {
		return new CalendarDateRange(CalendarDate.of(start), CalendarDate.of(end));
	  }

	  private CalendarDateRange(CalendarDate start, CalendarDate end)
	  {
		this.start = start;
		this.end = end;
		this.startDt = start.JDateTime;
		this.endDt = end.JDateTime;
		this.cal = start.Calendar;
		Debug.Assert(this.cal == end.Calendar);
	  }

	  public CalendarDateRange(CalendarDate start, long durationInSecs)
	  {
		this.start = start;
		this.end = start.add((int) durationInSecs, CalendarPeriod.Field.Second);
		this.startDt = start.JDateTime;
		this.endDt = end.JDateTime;
		this.cal = start.Calendar;
	  }

	  public virtual CalendarDate Start
	  {
		  get
		  {
			return start;
		  }
	  }

	  public virtual CalendarDate End
	  {
		  get
		  {
			return end;
		  }
	  }

	  public virtual long DurationInSecs
	  {
		  get
		  {
			return (endDt.getMillis() - startDt.getMillis()) / 1000;
		  }
	  }

	  // LOOK
	  public virtual CalendarDuration Duration
	  {
		  get
		  {
			return null;
		  }
	  }

	  // LOOK
	  public virtual CalendarDuration Resolution
	  {
		  get
		  {
			return null;
		  }
	  }

	  // LOOK
	  public virtual void setResolution()
	  {
	  }

	  public virtual bool intersects(CalendarDateRange o)
	  {
		return intersects(o.Start, o.End);
	  }

	  public virtual bool intersects(CalendarDate start, CalendarDate end)
	  {
		if (startDt.isAfter(end.JDateTime))
		{
		  return false;
		}
		return !endDt.isBefore(start.JDateTime);
	  }

	  public virtual bool includes(CalendarDate cd)
	  {
		JDateTime dt = cd.JDateTime;
		if (startDt.isAfter(dt))
		{
		  return false;
		}
		return !endDt.isBefore(dt);
	  }

	  public virtual CalendarDateRange intersect(CalendarDateRange clip)
	  {
		JDateTime cs = clip.Start.JDateTime;
		JDateTime s = startDt.isBefore(cs) ? cs : startDt; // later one

		JDateTime ce = clip.End.JDateTime;
		JDateTime e = endDt.isBefore(ce) ? endDt : ce; // earlier one

		return CalendarDateRange.of(CalendarDate.of(cal, s), CalendarDate.of(cal, e));
	  }

	  public virtual CalendarDateRange extend(CalendarDateRange other)
	  {
		JDateTime cs = other.Start.JDateTime;
		JDateTime s = startDt.isBefore(cs) ? startDt : cs; // earlier one

		JDateTime ce = other.End.JDateTime;
		JDateTime e = endDt.isBefore(ce) ? ce : endDt; // later one

		return CalendarDateRange.of(CalendarDate.of(cal, s), CalendarDate.of(cal, e));
	  }

	  public virtual bool Point
	  {
		  get
		  {
			return start.Equals(end);
		  }
	  }

	  public override string ToString()
	  {
		return start + " - " + end;
	  }



	  ///////////////////////////////////////////////
	  /// <summary>
	  /// Does not handle non-standard calendars
	  /// 
	  /// @deprecated
	  /// </summary>
	  public static CalendarDateRange of(DateRange dr)
	  {
		if (dr == null)
		{
		  return null;
		}
		return CalendarDateRange.of(CalendarDate.of(dr.Start.getDate()), CalendarDate.of(dr.End.getDate()));
	  }

	  /// <summary>
	  /// Does not handle non-standard calendars
	  /// 
	  /// @deprecated
	  /// </summary>
	  public virtual DateRange toDateRange()
	  {
		return new DateRange(start.toDate(), end.toDate());
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

		CalendarDateRange that = (CalendarDateRange) o;

		// All other fields in this class are derived from start or end.
		return start.Equals(that.start) && end.Equals(that.end);
	  }

	  public override int GetHashCode()
	  {
		int result = start.GetHashCode();
		result = 31 * result + end.GetHashCode();
		return result;
	  }
	}

}
