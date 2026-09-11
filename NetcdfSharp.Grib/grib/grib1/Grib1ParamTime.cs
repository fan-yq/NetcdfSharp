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

namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using GribStatType = NetcdfSharp.ucar.nc2.grib.GribStatType;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;

	/// <summary>
	/// Time coordinate from the PDS.
	/// Process information from GRIB-1 Table 4: "Forecast time unit"
	/// Process information from GRIB-1 Table 5: "Time range indicator"
	/// 
	/// Handles standard (WMO), Grib1Customizer must override / augment
	/// 
	/// @author John
	/// @since 9/4/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib1ParamTime
	public class Grib1ParamTime
	{

	  // code table 5 - 2010 edition of WMO manual on codes
	  public static string getTimeTypeName(int timeRangeIndicator)
	  {
		string timeRange;

		switch (timeRangeIndicator)
		{

		  /*
		   * Forecast product valid for reference time + P1 (P1 > 0), or
		   * Uninitialized analysis product for reference time (P1 = 0), or
		   * Image product for reference time (P1 = 0)
		   */
		  case 0:
			timeRange = "Uninitialized analysis / image product / forecast product valid for RT + P1";
			break;

		  // Initialized analysis product for reference time (P1 = 0)
		  case 1:
			timeRange = "Initialized analysis product for reference time";
			break;

		  // Product with a valid time ranging between reference time + P1 and reference time + P2
		  case 2:
			timeRange = "product valid, interval = (RT + P1) to (RT + P2)";
			break;

		  // Average (reference time + P1 to reference time + P2)
		  case 3:
			timeRange = "Average, interval = (RT + P1) to (RT + P2)";
			break;

		  /*
		   * Accumulation (reference time + P1 to reference time + P2) product considered valid at
		   * reference time + P2
		   */
		  case 4:
			timeRange = "Accumulation, interval = (RT + P1) to (RT + P2)";
			break;

		  /*
		   * Difference (reference time + P2 minus reference time + P1) product considered valid at
		   * reference time + P2
		   */
		  case 5:
			timeRange = "Difference, interval = (RT + P2) - (RT + P1)";
			break;

		  // Average (reference time - P1 to reference time - P2)
		  case 6:
			timeRange = "Average, interval = (RT - P1) to (RT - P2)";
			break;

		  // Average (reference time - P1 to reference time + P2)
		  case 7:
			timeRange = "Average, interval = (RT - P1) to (RT + P2)";
			break;

		  // P1 occupies octets 19 and 20; product valid at reference time + P1
		  case 10:
			timeRange = "product valid at RT + P1";
			break;

		  /*
		   * Climatological mean value: multiple year averages of quantities which are themselves
		   * means over some period of time (P2) less than a year. The reference time (R) indicates the
		   * date and time of the start of a period of time, given by R to R + P2, over which a mean is
		   * formed; N indicates the number of such period-means that are averaged together to form
		   * the climatological value, assuming that the N period-mean fields are separated by one
		   * year. The reference time indicates the start of the N-year climatology.
		   * 
		   * If P1 = 0 then the data averaged in the basic interval P2 are assumed to be continuous, i.e. all available data
		   * are simply averaged together.
		   * 
		   * If P1 = 1 (the unit of time octet 18, Code table 4 is not
		   * relevant here) then the data averaged together in the basic interval P2 are valid only at the
		   * time (hour, minute) given in the reference time, for all the days included in the P2 period.
		   * The units of P2 are given by the contents of octet 18 and Code table 4
		   */
		  case 51:
			timeRange = "Climatological mean values from RT to (RT + P2)";
			// if (p1 == 0) timeRange += " continuous";
			break;

		  /*
		   * Average of N forecasts (or initialized analyses); each product has forecast period of P1
		   * (P1 = 0 for initialized analyses); products have reference times at intervals of P2, beginning
		   * at the given reference time
		   */
		  case 113:
			timeRange = "Average of N forecasts, intervals = (refTime + i * P2, refTime + i * P2 + P1)";
			break;

		  /*
		   * Accumulation of N forecasts (or initialized analyses); each product has forecast period of
		   * P1 (P1 = 0 for initialized analyses); products have reference times at intervals of P2,
		   * beginning at the given reference time
		   */
		  case 114:
			timeRange = "Accumulation of N forecasts, intervals = (refTime + i * P2, refTime + i * P2 + P1)";
			break;

		  /*
		   * Average of N forecasts, all with the same reference time; the first has a forecast period of
		   * P1, the remaining forecasts follow at intervals of P2
		   */
		  case 115:
			timeRange = "Average of N forecasts, intervals = (refTime, refTime + P1 + i * P2)";
			break;

		  /*
		   * Accumulation of N forecasts, all with the same reference time; the first has a forecast
		   * period of P1, the remaining forecasts follow at intervals of P2
		   */
		  case 116:
			timeRange = "Accumulation of N forecasts, intervals = (refTime, refTime + P1 + i * P2)";
			break;

		  /*
		   * Average of N forecasts; the first has a forecast period of P1, the subsequent ones have
		   * forecast periods reduced from the previous one by an interval of P2; the reference time for
		   * the first is given in octets 13 to 17, the subsequent ones have reference times increased
		   * from the previous one by an interval of P2. Thus all the forecasts have the same valid time,
		   * given by the initial reference time + P1
		   */
		  case 117:
			timeRange = "Average of N forecasts, intervals = (refTime + i * P2, refTime + P1)";
			break;

		  /*
		   * Temporal variance, or covariance, of N initialized analyses; each product has forecast
		   * period of P1 = 0; products have reference times at intervals of P2, beginning at the given
		   * reference time
		   */
		  case 118:
			timeRange = "Temporal variance or covariance of N initialized analyses, timeCoord = (refTime + i * P2)";
			break;

		  /*
		   * Standard deviation of N forecasts, all with the same reference time with respect to the time
		   * average of forecasts; the first forecast has a forecast period of P1, the remaining forecasts
		   * follow at intervals of P2
		   */
		  case 119:
			timeRange = "Standard Deviation of N forecasts, timeCoord = (refTime + P1 + i * P2)";
			break;

		  // ECMWF "Average of N Forecast" added 11/21/2014. pretend its WMO standard. maybe should move to ecmwf ??
		  // see "http://emoslib.sourcearchive.com/documentation/000370.dfsg.2/grchk1_8F-source.html"
		  // C Add Time range indicator = 120 Average of N Forecast. Each product
		  // C is an accumulation from forecast lenght P1 to forecast
		  // C lenght P2, with reference times at intervals P2-P1
		  case 120:
			timeRange = "Average of N Forecasts (ECMWF), accumulation from forecast P1 to P2, with reference times at intervals P2-P1";
			break;

		  // Average of N uninitialized analyses, starting at the reference time, at intervals of P2
		  case 123:
			timeRange = "Average of N uninitialized analyses, intervals = (refTime, refTime + i * P2)";
			break;

		  // Accumulation of N uninitialized analyses, starting at the reference time, at intervals of P2
		  case 124:
			timeRange = "Accumulation of N uninitialized analyses, intervals = (refTime, refTime + i * P2)";
			break;

		  /*
		   * Standard deviation of N forecasts, all with the same reference time with respect to time
		   * average of the time tendency of forecasts; the first forecast has a forecast period of P1,
		   * the remaining forecasts follow at intervals of P2
		   */
		  case 125:
			timeRange = "Standard deviation of N forecasts, intervals = (refTime, refTime + P1 + i * P2)";
			break;

		  default:
			timeRange = "Unknown Time Range Indicator " + timeRangeIndicator;
		break;
		}

		return timeRange;
	  }

	  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	  private readonly Grib1Customizer cust; // passed in

	  private readonly int timeRangeIndicator; // code Table 5 (octet 21)
	  private readonly bool isInterval;
	  private readonly int start, end; // for intervals
	  private readonly int forecastTime; // for non-intervals

	  public Grib1ParamTime(Grib1Customizer cust, int timeRangeIndicator, bool isInterval, int start, int end, int forecastTime)
	  {
		this.cust = cust;
		this.timeRangeIndicator = timeRangeIndicator;
		this.isInterval = isInterval;
		this.start = start;
		this.end = end;
		this.forecastTime = forecastTime;
	  }

	  /// <summary>
	  /// Handles GRIB-1 code table 5 : "Time range indicator".
	  /// </summary>
	  /// <param name="cust"> customizer </param>
	  /// <param name="pds"> the Grib1SectionProductDefinition </param>
	  public Grib1ParamTime(Grib1Customizer cust, Grib1SectionProductDefinition pds)
	  {
		this.cust = cust;

		int p1 = pds.TimeValue1; // octet 19
		int p2 = pds.TimeValue2; // octet 20
		int timeRangeIndicatorLocal = pds.TimeRangeIndicator; // octet 21
		int n = pds.Nincluded;

		int startLocal = 0;
		int endLocal = 0;
		int forecastTimeLocal = 0;
		bool isIntervalLocal = false;

		switch (timeRangeIndicatorLocal)
		{

		  /*
		   * Forecast product valid for reference time + P1 (P1 > 0), or
		   * Uninitialized analysis product for reference time (P1 = 0), or
		   * Image product for reference time (P1 = 0)
		   */
		  case 0:
			forecastTimeLocal = p1;
			break;

		  // Initialized analysis product for reference time (P1 = 0)
		  case 1:
			// accept defaults
			break;

		  case 2: // Product with a valid time ranging between reference time + P1 and reference time + P2
		  case 3: // Average (reference time + P1 to reference time + P2)
		  case 4: // Accumulation (reference time + P1 to reference time + P2) product considered valid at reference time +
				  // P2
		  case 5: // Difference (reference time + P2 minus reference time + P1) product considered valid at reference time +
				  // P2
			startLocal = p1;
			endLocal = p2;
			isIntervalLocal = true;
			break;

		  // Average (reference time - P1 to reference time - P2)
		  case 6:
			startLocal = -p1;
			endLocal = -p2;
			isIntervalLocal = true;
			break;

		  // Average (reference time - P1 to reference time + P2)
		  case 7:
			startLocal = -p1;
			endLocal = p2;
			isIntervalLocal = true;
			break;

		  // P1 occupies octets 19 and 20; product valid at reference time + P1
		  case 10:
			forecastTimeLocal = GribNumbers.int2(p1, p2);
			break;


		  /*
		   * Climatological mean value: multiple year averages of quantities which are themselves
		   * means over some period of time (P2) less than a year. The reference time (R) indicates the
		   * date and time of the start of a period of time, given by R to R + P2, over which a mean is
		   * formed; N indicates the number of such period-means that are averaged together to form
		   * the climatological value, assuming that the N period-mean fields are separated by one
		   * year. The reference time indicates the start of the N-year climatology. If P1 = 0 then the
		   * data averaged in the basic interval P2 are assumed to be continuous, i.e. all available data
		   * are simply averaged together. If P1 = 1 (the unit of time – octet 18, Code table 4 – is not
		   * relevant here) then the data averaged together in the basic interval P2 are valid only at the
		   * time (hour, minute) given in the reference time, for all the days included in the P2 period.
		   * The units of P2 are given by the contents of octet 18 and Code table 4
		   */
		  case 51: // LOOK ??
			forecastTimeLocal = p2;
			break;

		  /*
		   * 113: Average of N forecasts (or initialized analyses); each product has forecast period of P1
		   * (P1 = 0 for initialized analyses); products have reference times at intervals of P2, beginning
		   * at the given reference time.
		   * 
		   * 114: Accumulation of N forecasts (or initialized analyses); each product has forecast period of
		   * P1 (P1 = 0 for initialized analyses); products have reference times at intervals of P2,
		   * beginning at the given reference time
		   * 115: Average of N forecasts, all with the same reference time; the first has a forecast period of
		   * P1, the remaining forecasts follow at intervals of P2
		   * 116: Accumulation of N forecasts, all with the same reference time; the first has a forecast
		   * period of P1, the remaining forecasts follow at intervals of P2
		   * 
		   */
		  case 113:
		  case 114:
		  case 115:
		  case 116:
			forecastTimeLocal = p1;
			startLocal = p1;
			endLocal = (n > 0) ? p1 + (n - 1) * p2 : p1; // LOOK switch to n-1 on 3/13/2015
			isIntervalLocal = (n > 0);
			break;

		  /*
		   * Average of N forecasts; the first has a forecast period of P1, the subsequent ones have
		   * forecast periods reduced from the previous one by an interval of P2; the reference time for
		   * the first is given in octets 13 to 17, the subsequent ones have reference times increased
		   * from the previous one by an interval of P2. Thus all the forecasts have the same valid time,
		   * given by the initial reference time + P1
		   */
		  case 117:
			endLocal = p1;
			isIntervalLocal = true;
			break;

		  /*
		   * Temporal variance, or covariance, of N initialized analyses; each product has forecast
		   * period of P1 = 0; products have reference times at intervals of P2, beginning at the given
		   * reference time
		   */
		  case 118:
			endLocal = n * p2;
			isIntervalLocal = true;
			break;

		  /*
		   * Standard deviation of N forecasts, all with the same reference time with respect to the time
		   * average of forecasts; the first forecast has a forecast period of P1, the remaining forecasts
		   * follow at intervals of P2
		   */
		  case 119:
			startLocal = p1;
			endLocal = p1 + n * p2;
			isIntervalLocal = true;
			break;

		  // ECMWF "Average of N Forecast" added 11/21/2014
		  // see "http://emoslib.sourcearchive.com/documentation/000370.dfsg.2/grchk1_8F-source.html"
		  // C Add Time range indicator = 120 Average of N Forecast. Each product
		  // C is an accumulation from forecast lenght P1 to forecast
		  // C lenght P2, with reference times at intervals P2-P1
		  case 120:
			startLocal = p1;
			endLocal = p2;
			isIntervalLocal = true;
			break;

		  // Average of N uninitialized analyses, starting at the reference time, at intervals of P2
		  case 123:
			endLocal = n * p2;
			isIntervalLocal = true;
			break;

		  // Accumulation of N uninitialized analyses, starting at the reference time, at intervals of P2
		  case 124:
			endLocal = n * p2;
			isIntervalLocal = true;
			break;

		  /*
		   * Standard deviation of N forecasts, all with the same reference time with respect to time
		   * average of the time tendency of forecasts; the first forecast has a forecast period of P1,
		   * the remaining forecasts follow at intervals of P2
		   */
		  case 125:
			endLocal = p1 + n * p2;
			isIntervalLocal = true;
			break;

		  default:
			throw new ArgumentException("PDS: Unknown Time Range Indicator " + timeRangeIndicatorLocal);
		}

		// added 11/30/2014. If interval (0,0), change to non interval at 0
		// analysis (0-hour) datasets use these (0,0) intervals, they are initialization values (I think).
		// by eliminating the extra coordinate, things get simpler.
		/*
		 * LOOK default we eliminate (0,0) so maybe not needed? 3/13/2015
		 * if (isIntervalLocal && (p1 == p2) && (p1 == 0)) {
		 * timeRangeIndicatorLocal = 1;
		 * forecastTimeLocal = 0;
		 * startLocal = endLocal = 0;
		 * isIntervalLocal = false;
		 * }
		 */

		// rigamorole to keep things final
		timeRangeIndicator = timeRangeIndicatorLocal;
		isInterval = isIntervalLocal;
		start = startLocal;
		end = endLocal;
		forecastTime = forecastTimeLocal;
	  }

	  /// <summary>
	  /// Get interval [start, end] since reference time in units of timeUnit, must be an interval.
	  /// </summary>
	  /// <returns> interval [start, end] </returns>
	  public virtual int[] Interval
	  {
		  get
		  {
			return new int[] {start, end};
		  }
	  }

	  /// <summary>
	  /// Get interval size (end - start) in units of timeUnit, only if an interval.
	  /// </summary>
	  /// <returns> interval size </returns>
	  public virtual int IntervalSize
	  {
		  get
		  {
			return isInterval ? end - start : 0;
		  }
	  }

	  /// <summary>
	  /// Is this an interval time coordinate
	  /// </summary>
	  /// <returns> If an interval time coordinate </returns>
	  public virtual bool IsInterval()
	  {
		return isInterval;
	  }

	  /// <summary>
	  /// Forecast time since reference time in units of timeUnit, only if not an interval.
	  /// </summary>
	  /// <returns> Forecast time </returns>
	  public virtual int ForecastTime
	  {
		  get
		  {
			return forecastTime;
		  }
	  }

	  /// <summary>
	  /// The time unit name (code table 5)
	  /// </summary>
	  /// <returns> time unit name </returns>
	  public virtual string TimeTypeName
	  {
		  get
		  {
			return cust.getTimeTypeName(timeRangeIndicator);
		  }
	  }

	  /// <summary>
	  /// The time unit statistical type, derived from code table 5)
	  /// </summary>
	  /// <returns> time unit statistical type </returns>
	  public virtual GribStatType StatType
	  {
		  get
		  {
			return cust.getStatType(timeRangeIndicator);
		  }
	  }

	  /// <summary>
	  /// A string representation of the time coordinate, whether its an interval or not.
	  /// </summary>
	  /// <returns> string representation of the time coordinate </returns>
	  public virtual string TimeCoord
	  {
		  get
		  {
			if (IsInterval())
			{
			  int[] intv = Interval;
			  return intv[0] + "-" + intv[1];
			}
			return Convert.ToString(ForecastTime);
		  }
	  }
	}

}
