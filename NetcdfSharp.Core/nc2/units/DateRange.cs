// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.units
{
	using CalendarDateRange = NetcdfSharp.ucar.nc2.time.CalendarDateRange;

	/// <summary>
	/// Implements a range of dates, using DateType and/or TimeDuration.
	/// You can use a DateType = "present" and a time duration to specify "real time" intervals, eg
	/// "last 3 days" uses endDate = "present" and duration = "3 days".
	/// 
	/// @author john caron
	/// </summary>

	public class DateRange
	{
	  private DateType start, end;
	  private TimeDuration duration, resolution;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private bool isEmpty, isMoving, useStart_Conflict, useEnd_Conflict, useDuration_Conflict, useResolution_Conflict;

	  /// <summary>
	  /// default Constructor
	  /// </summary>
	  /// <exception cref="NetcdfSharp.java.text.ParseException"> artifact, cant happen </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public DateRange() throws NetcdfSharp.java.text.ParseException
	  public DateRange() : this(null, new DateType(false, DateTime.Now), new TimeDuration("1 day"), new TimeDuration("15 min"))
	  {
	  }

	  public DateRange(CalendarDateRange cdr) : this(new DateType(cdr.Start), new DateType(cdr.End), null, null)
	  {
	  }

	  /// <summary>
	  /// Create Date Range from a start and end date
	  /// </summary>
	  /// <param name="start"> start of range </param>
	  /// <param name="end"> end of range </param>
	  public DateRange(DateTime start, DateTime end) : this(new DateType(false, start), new DateType(false, end), null, null)
	  {
	  }

	  /// <summary>
	  /// Create Date Range from a start date and duration
	  /// </summary>
	  /// <param name="start"> start of range </param>
	  /// <param name="duration"> duration of range </param>
	  public DateRange(DateTime start, TimeDuration duration) : this(new DateType(false, start), null, duration, null)
	  {
	  }

	  /// <summary>
	  /// Create DateRange from another DateRange, with a different units of resolution.
	  /// </summary>
	  /// <param name="range"> copy start and end from here </param>
	  /// <param name="timeUnits"> make resolution using new TimeDuration( timeUnits) </param>
	  /// <exception cref="ParseException"> is units are not valid time units </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public DateRange(DateRange range, String timeUnits) throws NetcdfSharp.java.text.ParseException
	  public DateRange(DateRange range, string timeUnits) : this(new DateType(false, range.Start.Date), new DateType(false, range.End.Date), null, new TimeDuration(timeUnits))
	  {
	  }

	  /// <summary>
	  /// Encapsulates a range of dates, using DateType start/end, and/or a TimeDuration.
	  /// A DateRange can be specified in any of the following ways:
	  /// <ol>
	  /// <li>a start date and end date
	  /// <li>a start date and duration
	  /// <li>an end date and duration
	  /// </ol>
	  /// </summary>
	  /// <param name="start"> starting date </param>
	  /// <param name="end"> ending date </param>
	  /// <param name="duration"> time duration; may be null </param>
	  /// <param name="resolution"> time resolution; may be null </param>
	  public DateRange(DateType start, DateType end, TimeDuration duration, TimeDuration resolution)
	  {
		this.start = start;
		this.end = end;
		this.duration = duration;
		this.resolution = resolution;

		useStart_Conflict = (start != null) && !start.Blank;
		useEnd_Conflict = (end != null) && !end.Blank;
		useDuration_Conflict = (duration != null) && !duration.Blank;
		useResolution_Conflict = (resolution != null);

		bool invalid = true;
		if (useStart_Conflict && useEnd_Conflict)
		{
		  invalid = false;
		  this.isMoving = this.start.Present || this.end.Present;
		  useDuration_Conflict = false;
		  recalcDuration();

		}
		else if (useStart_Conflict && useDuration_Conflict)
		{
		  invalid = false;
		  this.isMoving = this.start.Present;
		  this.end = this.start.add(duration);

		}
		else if (useEnd_Conflict && useDuration_Conflict)
		{
		  invalid = false;
		  this.isMoving = this.end.Present;
		  this.start = this.end.subtract(duration);
		}

		if (invalid)
		{
		  throw new ArgumentException("DateRange must have 2 of start, end, duration");
		}

		checkIfEmpty();
		hashCode_Conflict = 0;
	  }

	  private void checkIfEmpty()
	  {
		if (this.start.Present && this.end.Present)
		{
		  isEmpty = true;
		}
		else if (this.start.Present || this.end.Present)
		{
		  isEmpty = duration.Value <= 0;
		}
		else
		{
		  isEmpty = this.end.before(this.start);
		}

		if (isEmpty)
		{
		  duration.ValueInSeconds = 0;
		}
	  }

	  // choose a resolution based on # seconds
	  private string chooseResolution(double time)
	  {
		if (time < 180) // 3 minutes
		{
		  return "secs";
		}
		time /= 60; // minutes
		if (time < 180) // 3 hours
		{
		  return "minutes";
		}
		time /= 60; // hours
		if (time < 72) // 3 days
		{
		  return "hours";
		}
		time /= 24; // days
		if (time < 90) // 3 months
		{
		  return "days";
		}
		time /= 30; // months
		if (time < 36) // 3 years
		{
		  return "months";
		}
		return "years";
	  }

	  /// <summary>
	  /// Determine if the given date is included in this date range.
	  /// The date range includes the start and end dates.
	  /// </summary>
	  /// <param name="d"> date to check </param>
	  /// <returns> true if date in inside this range </returns>
	  public virtual bool included(DateTime d)
	  {
		if (isEmpty)
		{
		  return false;
		}

		if (Start.after(d))
		{
		  return false;
		}
		return !End.before(d);

	  }

	  /// <summary>
	  /// Same as included()
	  /// </summary>
	  /// <param name="d"> date to check </param>
	  /// <returns> true if date in inside this range </returns>
	  public virtual bool contains(DateTime d)
	  {
		return included(d);
	  }


	  /// <summary>
	  /// Determine if the given range intersects this date range.
	  /// </summary>
	  /// <param name="start_want"> range starts here </param>
	  /// <param name="end_want"> range ends here </param>
	  /// <returns> true if ranges intersect </returns>
	  public virtual bool intersects(DateTime start_want, DateTime end_want)
	  {
		if (isEmpty)
		{
		  return false;
		}

		if (Start.after(end_want))
		{
		  return false;
		}
		return !End.before(start_want);

	  }

	  /// <summary>
	  /// Determine if the given range intersects this date range.
	  /// </summary>
	  /// <param name="other"> date range </param>
	  /// <returns> true if ranges intersect </returns>
	  public virtual bool intersects(DateRange other)
	  {
		return intersects(other.Start.Date, other.End.Date);
	  }

	  /// <summary>
	  /// Intersect with another date range
	  /// </summary>
	  /// <param name="clip"> intersect with this date range </param>
	  /// <returns> new date range that is the intersection </returns>
	  public virtual DateRange intersect(DateRange clip)
	  {
		if (isEmpty)
		{
		  return this;
		}
		if (clip.isEmpty)
		{
		  return clip;
		}

		DateType ss = Start;
		DateType s = ss.before(clip.Start) ? clip.Start : ss;

		DateType ee = End;
		DateType e = ee.before(clip.End) ? ee : clip.End;

		return new DateRange(s, e, null, resolution);
	  }

	  /// <summary>
	  /// Extend this date range by the given one.
	  /// </summary>
	  /// <param name="dr"> given DateRange </param>
	  public virtual void extend(DateRange dr)
	  {
		bool localEmpty = isEmpty;
		if (localEmpty || dr.Start.before(Start))
		{
		  Start = dr.Start;
		}
		if (localEmpty || End.before(dr.End))
		{
		  End = dr.End;
		}
	  }

	  /// <summary>
	  /// Extend this date range by the given Date.
	  /// </summary>
	  /// <param name="d"> given Date </param>
	  public virtual void extend(DateTime d)
	  {
		if (d < Start.Date)
		{
		  Start = new DateType(false, d);
		}
		if (End.before(d))
		{
		  End = new DateType(false, d);
		}
	  }

	  /// <summary>
	  /// Get the starting Date.
	  /// </summary>
	  /// <returns> starting Date </returns>
	  public virtual DateType Start
	  {
		  get
		  {
			return (isMoving && !useStart_Conflict) ? this.end.subtract(duration) : start;
		  }
		  set
		  {
			this.start = value;
			useStart_Conflict = true;
    
			if (useEnd_Conflict)
			{
			  this.isMoving = this.start.Present || this.end.Present;
			  useDuration_Conflict = false;
			  recalcDuration();
    
			}
			else
			{
			  this.isMoving = this.start.Present;
			  this.end = this.start.add(duration);
			}
			checkIfEmpty();
		  }
	  }


	  /// <summary>
	  /// Get the ending Date.
	  /// </summary>
	  /// <returns> ending Date </returns>
	  public virtual DateType End
	  {
		  get
		  {
			return (isMoving && !useEnd_Conflict) ? this.start.add(duration) : end;
		  }
		  set
		  {
			this.end = value;
			useEnd_Conflict = true;
    
			if (useStart_Conflict)
			{
			  this.isMoving = this.start.Present || this.end.Present;
			  useDuration_Conflict = false;
			  recalcDuration();
    
			}
			else
			{
			  this.isMoving = this.end.Present;
			  this.start = this.end.subtract(duration);
			}
			checkIfEmpty();
		  }
	  }


	  /// <summary>
	  /// Get the duration of the interval
	  /// </summary>
	  /// <returns> duration of the interval </returns>
	  public virtual TimeDuration Duration
	  {
		  get
		  {
			if (isMoving && !useDuration_Conflict)
			{
			  recalcDuration();
			}
			return duration;
		  }
		  set
		  {
			this.duration = value;
			useDuration_Conflict = true;
    
			if (useStart_Conflict)
			{
			  this.isMoving = this.start.Present;
			  this.end = this.start.add(value);
			  useEnd_Conflict = false;
    
			}
			else
			{
			  this.isMoving = this.end.Present;
			  this.start = this.end.subtract(value);
			}
			checkIfEmpty();
		  }
	  }


	  // assumes not moving
	  private void recalcDuration()
	  {
		long min = Start.Date.Ticks;
		long max = End.Date.Ticks;
		double secs = .001 * (max - min);
		if (secs < 0)
		{
		  secs = 0;
		}

		if (duration == null)
		{
		  try
		  {
			duration = new TimeDuration(chooseResolution(secs));
		  }
		  catch (ParseException e)
		  {
			// cant happen
			throw new Exception(e.Message);
		  }
		}

		if (resolution == null)
		{
		  duration.ValueInSeconds = secs;

		}
		else
		{
		  // make it a multiple of resolution
		  double resSecs = resolution.ValueInSeconds;
		  double closest = (long)Math.Floor(secs / resSecs + 0.5);
		  secs = closest * resSecs;
		  duration.ValueInSeconds = secs;
		}

		hashCode_Conflict = 0;
	  }

	  /// <summary>
	  /// Get the time resolution.
	  /// </summary>
	  /// <returns> time resolution as a duration </returns>
	  public virtual TimeDuration Resolution
	  {
		  get
		  {
			return resolution;
		  }
		  set
		  {
			this.resolution = value;
			useResolution_Conflict = true;
		  }
	  }


	  /// <summary>
	  /// Get if the start is fixed.
	  /// </summary>
	  /// <returns> if start is fixed </returns>
	  public virtual bool useStart()
	  {
		return useStart_Conflict;
	  }

	  /// <summary>
	  /// Get if the end is fixed.
	  /// </summary>
	  /// <returns> if end is fixed </returns>
	  public virtual bool useEnd()
	  {
		return useEnd_Conflict;
	  }

	  /// <summary>
	  /// Get if the duration is fixed.
	  /// </summary>
	  /// <returns> if duration is fixed </returns>
	  public virtual bool useDuration()
	  {
		return useDuration_Conflict;
	  }

	  /// <summary>
	  /// Get if the resolution is set.
	  /// </summary>
	  /// <returns> if resolution is fixed </returns>
	  public virtual bool useResolution()
	  {
		return useResolution_Conflict;
	  }

	  /// <summary>
	  /// Return true if start date equals end date, so date range is a point.
	  /// </summary>
	  /// <returns> true if start = end </returns>
	  public virtual bool Point
	  {
		  get
		  {
			return !isEmpty && start.Equals(end);
		  }
	  }

	  /// <summary>
	  /// If the range is empty
	  /// </summary>
	  /// <returns> if the range is empty </returns>
	  public virtual bool Empty
	  {
		  get
		  {
			return isEmpty;
		  }
	  }

	  public override string ToString()
	  {
		return "start= " + start + " end= " + end + " duration= " + duration + " resolution= " + resolution;
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (!(o is DateRange))
		{
		  return false;
		}
		DateRange oo = (DateRange) o;
		if (useStart_Conflict && !start.Equals(oo.start))
		{
		  return false;
		}
		if (useEnd_Conflict && !end.Equals(oo.end))
		{
		  return false;
		}
		if (useDuration_Conflict && !duration.Equals(oo.duration))
		{
		  return false;
		}
		return !useResolution_Conflict || resolution.Equals(oo.resolution);
	  }

	  /// <summary>
	  /// Override Object.hashCode() to implement equals.
	  /// </summary>
	  public override int GetHashCode()
	  {
		if (hashCode_Conflict == 0)
		{
		  int result = 17;
		  if (useStart_Conflict)
		  {
			result = 37 * result + start.GetHashCode();
		  }
		  if (useEnd_Conflict)
		  {
			result = 37 * result + end.GetHashCode();
		  }
		  if (useDuration_Conflict)
		  {
			result = 37 * result + duration.GetHashCode();
		  }
		  if (useResolution_Conflict)
		  {
			result = 37 * result + resolution.GetHashCode();
		  }
		  hashCode_Conflict = result;
		}
		return hashCode_Conflict;
	  }

// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private int hashCode_Conflict; // Bloch, item 8
	}

}
