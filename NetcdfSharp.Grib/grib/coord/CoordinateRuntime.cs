// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using Counters = NetcdfSharp.ucar.nc2.util.Counters;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// Grib runtime coordinate
	/// Effectively Immutable
	/// 
	/// @author caron
	/// @since 11/24/13
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CoordinateRuntime implements Coordinate
	public class CoordinateRuntime : Coordinate
	{
	  private readonly long[] runtimes;
	  internal readonly CalendarDate firstDate;
	  internal readonly CalendarPeriod timeUnit;
	  internal readonly string periodName;
	  private string name = "reftime"; // yeah yeah, not final, bugger off

	  public CoordinateRuntime(IList<long> runtimeSorted, CalendarPeriod timeUnit)
	  {
		this.runtimes = new long[runtimeSorted.Count];
		int idx = 0;
		foreach (long val in runtimeSorted)
		{
		  this.runtimes[idx++] = val;
		}

		this.firstDate = CalendarDate.of(runtimeSorted[0]);
		this.timeUnit = timeUnit == null ? CalendarPeriod.Hour : timeUnit;

		CalendarPeriod.Field cf = this.timeUnit.FieldProp;
		if (cf == CalendarPeriod.Field.Month || cf == CalendarPeriod.Field.Year)
		{
		  this.periodName = "calendar " + cf;
		}
		else
		{
		  this.periodName = cf.ToString();
		}
	  }

	  public virtual CalendarPeriod TimeUnits
	  {
		  get
		  {
			return timeUnit;
		  }
	  }

	  public virtual CalendarDate getRuntimeDate(int idx)
	  {
		return CalendarDate.of(runtimes[idx]);
	  }

	  public virtual long getRuntime(int idx)
	  {
		return runtimes[idx];
	  }

	  public virtual string PeriodName
	  {
		  get
		  {
			return periodName;
		  }
	  }

	  /// <summary>
	  /// Get offsets from firstDate, in units of timeUnit
	  /// </summary>
	  /// <returns> for each runtime, a list of values from firstdate </returns>
	  public virtual IList<double> OffsetsInTimeUnits
	  {
		  get
		  {
			double start = firstDate.Millis;

			IList<double> result = new List<double>(runtimes.Length);
			for (int idx = 0; idx < runtimes.Length; idx++)
			{
			  double runtime = (double) getRuntime(idx);
			  double msecs = (runtime - start);
			  result.Add(msecs / timeUnit.ValueInMillisecs);
			}
			return result;
		  }
	  }

	  public virtual double getOffsetInTimeUnits(CalendarDate start)
	  {
		return timeUnit.getOffset(start, FirstDate);
	  }

	  public virtual int Size
	  {
		  get
		  {
			return runtimes.Length;
		  }
	  }

	  public virtual int NCoords
	  {
		  get
		  {
			return Size;
		  }
	  }

	  public virtual Coordinate.Type getType()
	  {
		return NetcdfSharp.ucar.nc2.grib.coord.Coordinate.Type.runtime;
	  }

	  public virtual int estMemorySize()
	  {
		return 616 + Size * (48);
	  }

	  public virtual string Unit
	  {
		  get
		  {
			return periodName + " since " + firstDate;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  { // LOOK need to get this in the constructor to make the value final
			if (!this.name.Equals("reftime"))
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			this.name = value;
		  }
	  }


	  public virtual int Code
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual CalendarDate FirstDate
	  {
		  get
		  {
			return firstDate;
		  }
	  }

	  public virtual CalendarDate LastDate
	  {
		  get
		  {
			return getRuntimeDate(Size - 1);
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public List<?> getValues()
	  public virtual IList<object> Values
	  {
		  get
		  {
			IList<object> result = new List<object>(runtimes.Length);
			foreach (long val in runtimes)
			{
			  result.Add(val);
			}
			return result;
		  }
	  }

	  public virtual int getIndex(object val)
	  {
		long want;

		if (val is CalendarDate)
		{
		  want = ((CalendarDate) val).Millis;
		}
		else if (val is int or long or short or byte or float or double or uint or ulong or ushort)
		{
		  want = Convert.ToInt64(val);
		}
		else
		{
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ArgumentException(val.GetType().FullName);
		}

		return System.Array.BinarySearch(runtimes, want);
	  }

	  public virtual object getValue(int idx)
	  {
		return runtimes[idx];
	  }

	  public virtual void showInfo(Formatter info, Indent indent)
	  {
		info.format("%s%s:", indent, getType());
		for (int idx = 0; idx < Size; idx++)
		{
		  info.format(" %s,", getRuntimeDate(idx));
		}
		info.format(" (%d) %n", runtimes.Length);
	  }

	  public virtual void showCoords(Formatter info)
	  {
		info.format("Run Times: %s (%s)%n", Name, Unit);
		IList<double> udunits = OffsetsInTimeUnits;
		int count = 0;
		for (int idx = 0; idx < Size; idx++)
		{
		  info.format("   %s (%f)%n", getRuntimeDate(idx), udunits[count++]);
		}
	  }

	  public virtual Counters calcDistributions()
	  {
		Counters counters = new Counters();
		counters.add("resol");

		IList<double> offsets = OffsetsInTimeUnits;
		for (int i = 0; i < offsets.Count - 1; i++)
		{
		  double? diff = offsets[i + 1] - offsets[i];
		  counters.count("resol", diff);
		}

		return counters;
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

		CoordinateRuntime that = (CoordinateRuntime) o;

		if (!periodName.Equals(that.periodName))
		{
		  return false;
		}
		return runtimes.SequenceEqual(that.runtimes);

	  }

	  public override int GetHashCode()
	  {
		int result = 1;
		foreach (long v in runtimes)
		{
		  result = 31 * result + (int)(v ^ (v >> 32));
		}
		result = 31 * result + periodName.GetHashCode();
		return result;
	  }

	  public override string ToString()
	  {
		return name;
	  }

	  ///////////////////////////////////////////////////////

	  public class Builder2 : CoordinateBuilderImpl<Grib2Record>
	  {

		internal CalendarPeriod timeUnit;

		public Builder2(CalendarPeriod timeUnit)
		{
		  this.timeUnit = timeUnit;
		}

		public override object extract(Grib2Record gr)
		{
		  return gr.ReferenceDate.Millis;
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<long> runtimeSorted = new List<long>(values.Count);
		  foreach (object val in values)
		  {
			runtimeSorted.Add(((long?) val).Value);
		  }
		  runtimeSorted.Sort();
		  return new CoordinateRuntime(runtimeSorted, timeUnit);
		}
	  }

	  public class Builder1 : CoordinateBuilderImpl<Grib1Record>
	  {
		internal CalendarPeriod timeUnit;

		public Builder1(CalendarPeriod timeUnit)
		{
		  this.timeUnit = timeUnit;
		}

		public override object extract(Grib1Record gr)
		{
		  return gr.ReferenceDate.Millis;
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<long> runtimeSorted = new List<long>(values.Count);
		  foreach (object val in values)
		  {
			runtimeSorted.Add(((long?) val).Value);
		  }
		  runtimeSorted.Sort();
		  return new CoordinateRuntime(runtimeSorted, timeUnit);
		}
	  }


	}

}
