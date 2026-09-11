// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class VertCoordValue implements Comparable<VertCoordValue>
	public class VertCoordValue : IComparable<VertCoordValue>, IComparable
	{
	  private readonly double value1;
	  private readonly double value2;
	  private readonly double mid;
	  private readonly bool isLayer;

	  public VertCoordValue(double value1)
	  {
		this.value1 = value1;
		this.value2 = GribNumbers.UNDEFINEDD;
		this.mid = value1;
		this.isLayer = false;
	  }

	  public VertCoordValue(double value1, double value2)
	  {
		this.value1 = value1;
		this.value2 = value2;
		this.mid = (value2.CompareTo(0.0) == 0 || GribNumbers.isUndefined(value2)) ? value1 : (value1 + value2) / 2;
		this.isLayer = true;
	  }

	  public virtual double Value1
	  {
		  get
		  {
			return value1;
		  }
	  }

	  public virtual double Value2
	  {
		  get
		  {
			return value2;
		  }
	  }

	  public virtual double Mid
	  {
		  get
		  {
			return mid;
		  }
	  }

	  public virtual bool Layer
	  {
		  get
		  {
			return isLayer;
		  }
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override public int compareTo(@Nonnull VertCoordValue o)
	  public virtual int CompareTo(VertCoordValue o)
	  {
		return mid.CompareTo(o.mid);
	  }

	  public virtual int CompareTo(object o)
	  {
		return CompareTo((VertCoordValue) o);
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
		VertCoordValue that = (VertCoordValue) o;
		return that.value1.CompareTo(value1) == 0 && that.value2.CompareTo(value2) == 0 && that.mid.CompareTo(mid) == 0 && isLayer == that.isLayer;
	  }

	  public override int GetHashCode()
	  {
		return Objects.hash(value1, value2, mid, isLayer);
	  }

	  // cannot do approx equals and be consistent with hashCode, so make separate call
	  public virtual bool nearlyEquals(VertCoordValue other)
	  {
		return Misc.nearlyEquals(value1, other.value1) && Misc.nearlyEquals(value2, other.value2);
	  }

	  public override string ToString()
	  {
		using (Formatter @out = new Formatter())
		{
		  if (isLayer)
		  {
			@out.format("(%f,%f)", value1, value2);
		  }
		  else
		  {
			@out.format("%f", value1);
		  }
		  return @out.ToString();
		}
	  }

	  public virtual string toString(bool isLayer)
	  {
		using (Formatter @out = new Formatter())
		{
		  if (isLayer)
		  {
			@out.format("(%f,%f)", value1, value2);
		  }
		  else
		  {
			@out.format("%f", value1);
		  }
		  return @out.ToString();
		}
	  }
	}

}
