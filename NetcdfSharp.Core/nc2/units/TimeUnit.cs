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
	using Format = NetcdfSharp.ucar.unidata.util.Format;
	using ConversionException = NetcdfSharp.ucar.units.ShimConversionException;

	/// <summary>
	/// Handles Units that are time durations, eg in seconds, hours, days, years.
	/// It keeps track of the original unit name, rather than converting to canonical "seconds".
	/// The unit name never changes, but the value may.
	/// <para>
	/// This is a wrapper around ucar.units.
	/// The underlying NetcdfSharp.ucar.units.Unit always has a value of "1.0", ie is a base unit.
	/// 
	/// @author John Caron
	/// </para>
	/// </summary>
	public class TimeUnit : SimpleUnit
	{
	  private double value;
	  private double factor = 1.0;
	  private string unitString;

	  /// <summary>
	  /// Constructor from a String.
	  /// </summary>
	  /// <param name="text"> [value] <time unit> eg "hours" or "13 hours". Time unit is from udunits. </param>
	  /// <exception cref="UnitException"> is bad format </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public TimeUnit(String text) throws NetcdfSharp.ucar.units.UnitException
	  public TimeUnit(string text)
	  {
		if (string.ReferenceEquals(text, null))
		{
		  this.value = 1.0;
		  this.unitString = "secs";
		  this.uu = SimpleUnit.makeUnit(unitString); // always a base unit
		  return;
		}

		StringTokenizer stoker = new StringTokenizer(text);
		int ntoke = stoker.countTokens();
		if (ntoke == 1)
		{
		  this.value = 1.0;
		  this.unitString = stoker.nextToken();

		}
		else if (ntoke == 2)
		{
		  this.value = double.Parse(stoker.nextToken());
		  this.unitString = stoker.nextToken();
		}
		else
		{
		  throw new ArgumentException("Not TimeUnit = " + text);
		}

		uu = SimpleUnit.makeUnit(unitString); // always a base unit
		factor = uu.convertTo(1.0, SimpleUnit.secsUnit);
	  }

	  /// <summary>
	  /// Constructor from a value and a unit name.
	  /// </summary>
	  /// <param name="value"> amount of the unit. </param>
	  /// <param name="unitString"> Time unit string from udunits. </param>
	  /// <exception cref="UnitException"> if parse fails </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public TimeUnit(double value, String unitString) throws NetcdfSharp.ucar.units.UnitException
	  public TimeUnit(double value, string unitString)
	  {
		this.value = value;
		this.unitString = unitString;
		uu = SimpleUnit.makeUnit(unitString);
		factor = uu.convertTo(1.0, SimpleUnit.secsUnit);
	  }

	  /// <summary>
	  /// Copy Constructor.
	  /// </summary>
	  /// <param name="src"> copy from here </param>
	  public TimeUnit(TimeUnit src)
	  {
		this.value = src.Value;
		this.unitString = src.UnitString;
		uu = src.uu;
		factor = src.Factor;
	  }

	  /// <summary>
	  /// Get the value. </summary>
	  public override double Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual void setValue(double value)
	  {
		this.value = value;
	  }

	  /// <summary>
	  /// Get the factor that converts this unit to seconds.
	  /// getValueInSeconds = factor * value
	  /// </summary>
	  /// <returns> factor that converts this unit to seconds. </returns>
	  public virtual double Factor
	  {
		  get
		  {
			return factor;
		  }
	  }


	  /// <summary>
	  /// Get the "base" unit String, eg "secs" or "days" </summary>
	  public override string UnitString
	  {
		  get
		  {
			return unitString;
		  }
	  }

	  public virtual string getUnitString() => UnitString;

	  /// <summary>
	  /// String representation. </summary>
	  public override string ToString()
	  {
		return Format.d(value, 5) + " " + unitString;
	  }

	  /// <summary>
	  /// Get the time duration in seconds.
	  /// </summary>
	  /// <returns> get current value in units of seconds </returns>
	  public virtual double ValueInSeconds
	  {
		  get
		  {
			return factor * value;
		  }
		  set
		  {
			// setter parameter 'value' shadows the field; write the field explicitly
			this.value = value / factor;
		  }
	  }

	  /// <summary>
	  /// Get the time duration in seconds of the specified value.
	  /// </summary>
	  /// <param name="value"> convert this value, must be in units of this </param>
	  /// <returns> get the value in units of seconds </returns>
	  public virtual double getValueInSeconds(double value)
	  {
		return factor * value;
	  }


	  // override

	  /// <summary>
	  /// Convert given value of this unit to the new unit.
	  /// <em>NOTE: the current value of this unit ignored, the given value is used instead.
	  /// This is different than ucar.units or SimpleUnit.</em>
	  /// </summary>
	  /// <param name="value"> in the units of this "base unit" </param>
	  /// <param name="outputUnit"> convert to this base type, must be convertible to units of "seconds" </param>
	  /// <returns> new value in the units of the "outputUnit </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public double convertTo(double value, TimeUnit outputUnit) throws NetcdfSharp.ucar.units.ConversionException
	  public virtual double convertTo(double value, TimeUnit outputUnit)
	  {
		return uu.convertTo(value, outputUnit.uu);
	  }

	  /// <summary>
	  /// Add the time amount to the given Date, return a new Date.
	  /// </summary>
	  /// <param name="d"> add to this Date </param>
	  /// <returns> Date with getValueInSeconds() added to it. </returns>
	  public virtual DateTime add(DateTime d)
	  {
		DateTime cal = d;
		cal = cal.AddSeconds((int) ValueInSeconds);
		return cal;
	  }

	  /// <summary>
	  /// TimeUnits with same value and unitString are equal </summary>
	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (!(o is TimeUnit))
		{
		  return false;
		}
		return o.GetHashCode() == this.GetHashCode();
	  }

	  /// <summary>
	  /// Override hashcode to be consistent with equals. </summary>
	  public override int GetHashCode()
	  {
		if (hashCode_Conflict == 0)
		{
		  int result = 17;
		  result = 37 * result + unitString.GetHashCode();
		  result = 37 * result + (int)(1000.0 * value);
		  hashCode_Conflict = result;
		}
		return hashCode_Conflict;
	  }

// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private int hashCode_Conflict;

	}

}
