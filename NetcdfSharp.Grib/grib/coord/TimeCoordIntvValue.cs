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

	// use for time intervals
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class TimeCoordIntvValue implements Comparable<TimeCoordIntvValue>
	public class TimeCoordIntvValue : IComparable<TimeCoordIntvValue>, IComparable
	{
	  private readonly int b1, b2; // bounds

	  public TimeCoordIntvValue(int b1, int b2)
	  {
		this.b1 = b1;
		this.b2 = b2;
	  }

	  public virtual int Bounds1
	  {
		  get
		  {
			return b1;
		  }
	  }

	  public virtual int Bounds2
	  {
		  get
		  {
			return b2;
		  }
	  }

	  public virtual int IntervalSize
	  {
		  get
		  {
			return Math.Abs(b2 - b1);
		  }
	  }

	  public virtual double CoordValue
	  {
		  get
		  {
			// Choose interval endpoint as the coordinate value
			return b2;
		  }
	  }

	  public virtual TimeCoordIntvValue convertReferenceDate(CalendarDate fromDate, CalendarPeriod fromUnit, CalendarDate toDate, CalendarPeriod toUnit)
	  {
		CalendarDate start = fromDate.add(fromUnit.multiply(b1));
		CalendarDate end = fromDate.add(fromUnit.multiply(b2));
		int startOffset = toUnit.getOffset(toDate, start);
		int endOffset = toUnit.getOffset(toDate, end);
		return new TimeCoordIntvValue(startOffset, endOffset);
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
		TimeCoordIntvValue that = (TimeCoordIntvValue) o;
		return b1 == that.b1 && b2 == that.b2;
	  }

	  public override int GetHashCode()
	  {
		return Objects.hash(b1, b2);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override public int compareTo(@Nonnull TimeCoordIntvValue o)
	  public virtual int CompareTo(TimeCoordIntvValue o)
	  {
		int c1 = b2 - o.b2;
		return (c1 == 0) ? b1 - o.b1 : c1;
	  }

	  public virtual int CompareTo(object o)
	  {
		return CompareTo((TimeCoordIntvValue) o);
	  }

	  public override string ToString()
	  {
		return string.Format("({0:D},{1:D})", b1, b2);
	  }

	  public virtual TimeCoordIntvValue offset(double offset)
	  {
		return new TimeCoordIntvValue((int)(offset + b1), (int)(offset + b2));
	  }
	}


}
