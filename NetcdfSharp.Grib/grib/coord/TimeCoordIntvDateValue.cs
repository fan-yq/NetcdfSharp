// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;

	/// <summary>
	/// Time intervals represented by start and end CalendarDate.
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class TimeCoordIntvDateValue implements Comparable<TimeCoordIntvDateValue>
	public class TimeCoordIntvDateValue : IComparable<TimeCoordIntvDateValue>
	{

	  private readonly CalendarDate start, end;

	  public TimeCoordIntvDateValue(CalendarPeriod period, CalendarDate end)
	  {
		this.end = end;
		this.start = end.subtract(period);
	  }

	  public TimeCoordIntvDateValue(CalendarDate start, CalendarPeriod period)
	  {
		this.start = start;
		this.end = start.add(period);
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
		TimeCoordIntvDateValue that = (TimeCoordIntvDateValue) o;
		return start.Equals(that.start) && end.Equals(that.end);
	  }

	  public override int GetHashCode()
	  {
		return Objects.hash(start, end);
	  }

	  public TimeCoordIntvDateValue(CalendarDate start, CalendarDate end)
	  {
		this.start = start;
		this.end = end;
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

	  // Calculate the offset in units of timeUnit from the given reference date?
	  public virtual TimeCoordIntvValue convertReferenceDate(CalendarDate refDate, CalendarPeriod timeUnit)
	  {
		if (timeUnit == null)
		{
		  throw new ArgumentException("null time unit");
		}
		int startOffset = timeUnit.getOffset(refDate, start); // LOOK wrong - not dealing with value ??
		int endOffset = timeUnit.getOffset(start, end) + startOffset;
		return new TimeCoordIntvValue(startOffset, endOffset);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: public int compareTo(@Nonnull TimeCoordIntvDateValue that)
	  public virtual int CompareTo(TimeCoordIntvDateValue that)
	  { // first compare start, then end
		int c1 = start.CompareTo(that.start);
		return (c1 == 0) ? end.CompareTo(that.end) : c1;
	  }

	  public override string ToString()
	  {
		return string.Format("({0},{1})", start, end);
	  }

	}


}
