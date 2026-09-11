// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using Format = NetcdfSharp.ucar.unidata.util.Format;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;



	/// <summary>
	/// Abstract superclass for GRIB2 PDS handling.
	/// Inner classes are specific to each template.
	/// 
	/// @author caron
	/// @since 3/28/11
	/// </summary>

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public abstract class Grib2Pds
	public abstract class Grib2Pds
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2Pds));

	  /// <summary>
	  /// Factory for Grib2Pds
	  /// </summary>
	  /// <param name="template"> pds template number </param>
	  /// <param name="input"> raw bytes </param>
	  /// <returns> Grib2Pds or null on error </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public static Grib2Pds factory(int template, byte[] input)
	  public static Grib2Pds factory(int template, byte[] input)
	  {
		switch (template)
		{
		  case 0:
			return new Grib2Pds0(input);
		  case 1:
			return new Grib2Pds1(input);
		  case 2:
			return new Grib2Pds2(input);
		  case 5:
			return new Grib2Pds5(input);
		  case 6:
			return new Grib2Pds6(input);
		  case 8:
			return new Grib2Pds8(input);
		  case 9:
			return new Grib2Pds9(input);
		  case 10:
			return new Grib2Pds10(input);
		  case 11:
			return new Grib2Pds11(input);
		  case 12:
			return new Grib2Pds12(input);
		  case 15:
			return new Grib2Pds15(input);
		  case 30:
			return new Grib2Pds30(input);
		  case 31:
			return new Grib2Pds31(input);
		  case 32:
			return new Grib2Pds32(input);
		  case 40:
			return new Grib2Pds40(input);
		  case 41:
			return new Grib2Pds41(input);
		  case 48:
			return new Grib2Pds48(input);
		  case 60:
			return new Grib2Pds60(input);
		  case 61:
			return new Grib2Pds61(input);
		  default:
			log.warn("Missing template " + template);
			throw new System.NotSupportedException("Product Definition " + template + " not yet implemented.");
		}
	  }

	  /////////////////////////////////////////////////
	  protected internal readonly byte[] input;
	  protected internal readonly int template; // product definition template

	  /// <summary>
	  /// Constructs a Grib2PDSVariables object from a byte[].
	  /// </summary>
	  /// <param name="input"> raw bytes </param>
	  protected internal Grib2Pds(byte[] input)
	  {
		this.input = input;
		template = GribNumbers.int2(getOctet(8), getOctet(9));
	  }

	  // optional coordinates start after this
	  public abstract int templateLength();

	  /// <summary>
	  /// Number of coordinate values at end of template.
	  /// </summary>
	  /// <returns> Coordinates number </returns>
	  public virtual int ExtraCoordinatesCount
	  {
		  get
		  {
			return GribNumbers.int2(getOctet(6), getOctet(7));
		  }
	  }

	  internal float[] ExtraCoordinates
	  {
		  get
		  {
			int n = ExtraCoordinatesCount;
			if (n == 0)
			{
			  return new float[0];
			}
			float[] result = new float[n];
			int count = templateLength() + 1;
			for (int i = 0; i < n; i++)
			{
			  result[i] = GribNumbers.float4(getOctet(count++), getOctet(count++), getOctet(count++), getOctet(count++));
			}
			return result;
		  }
	  }

	  /// <summary>
	  /// product Definition template, Table 4.0
	  /// </summary>
	  /// <returns> ProductDefinition </returns>
	  public int TemplateNumber
	  {
		  get
		  {
			return template;
		  }
	  }

	  /// <summary>
	  /// Parameter Category
	  /// </summary>
	  /// <returns> parameterCategory as int </returns>
	  public int ParameterCategory
	  {
		  get
		  {
			return getOctet(10);
		  }
	  }

	  /// <summary>
	  /// Parameter Number
	  /// </summary>
	  /// <returns> ParameterNumber </returns>
	  public int ParameterNumber
	  {
		  get
		  {
			return getOctet(11);
		  }
	  }

	  /// <summary>
	  /// Type of Generating Process (Code Table 4.3)
	  /// </summary>
	  /// <returns> Type of Generating Process </returns>
	  public virtual int GenProcessType
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  /// <summary>
	  /// Forecast/Analysis generating process identifier (defined by originating centre).
	  /// <p/>
	  /// For NCEP, apparently
	  /// http://www.nco.ncep.noaa.gov/pmb/docs/on388/tablea.html
	  /// as linked from here:
	  /// http://www.nco.ncep.noaa.gov/pmb/docs/grib2/grib2_temp4-0.shtml
	  /// </summary>
	  /// <returns> generating process id </returns>
	  public virtual int GenProcessId
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  /// <summary>
	  /// Get Background generating process identifier (defined by originating centre)
	  /// </summary>
	  /// <returns> Background generating process identifier </returns>
	  public virtual int BackProcessId
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  /// <summary>
	  /// Indicator of unit of time range (see Code table 4.4)
	  /// </summary>
	  /// <returns> unit of time range </returns>
	  public abstract int TimeUnit {get;}

	  /// <summary>
	  /// Forecast time in units of getTimeUnit()
	  /// forecast time for points, should not be used for intervals
	  /// </summary>
	  /// <returns> Forecast time </returns>
	  public virtual int ForecastTime
	  {
		  get
		  {
			return GribNumbers.int4(getOctet(19), getOctet(20), getOctet(21), getOctet(22));
		  }
	  }

	  public virtual double LevelValue1
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  public virtual int LevelScale1
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  public virtual double LevelValue2
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  public virtual int LevelType1
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  public virtual int LevelType2
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  public virtual int LevelScale2
	  {
		  get
		  {
			return GribNumbers.UNDEFINED;
		  }
	  }

	  public virtual bool Aerosol
	  {
		  get
		  {
			return (template == 48);
		  }
	  }

	  public virtual bool Ensemble
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual bool EnsembleDerived
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual bool Probability
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual bool isTimeInterval()
	  {
		return (template >= 8) && (template <= 14);
	  }

	  public virtual bool SpatialInterval
	  {
		  get
		  {
			return (template == 15);
		  }
	  }

	  public virtual bool Satellite
	  {
		  get
		  {
			return (template >= 30 && template <= 32);
		  }
	  }

	  public virtual bool Percentile
	  {
		  get
		  {
			return false;
		  }
	  }

	  /*
	   * public int getPerturbationNumber() {
	   * return GribNumbers.UNDEFINED;
	   * }
	   *
	   * public int getPerturbationType() {
	   * return GribNumbers.UNDEFINED;
	   * }
	   *
	   * public boolean isEnsembleDerived() {
	   * return false;
	   * }
	   *
	   * public int getNumberEnsembleForecasts() {
	   * return GribNumbers.UNDEFINED;
	   * }
	   */

	  /*
	   * public double ProbabilityLowerLimit {
	   * return GribNumbers.UNDEFINED;
	   * }
	   *
	   * public double ProbabilityUpperLimit {
	   * return GribNumbers.UNDEFINED;
	   * }
	   *
	   * public int ProbabilityType {
	   * return GribNumbers.UNDEFINED;
	   * }
	   */


	  public virtual void show(Formatter f)
	  {
		f.format("Grib2Pds{ id=%d-%d template=%d, forecastTime= %d timeUnit=%s vertLevel=%f}", ParameterCategory, ParameterNumber, template, ForecastTime, TimeUnit, LevelValue1);
	  }

	  /// <summary>
	  /// Get the index'th byte in the PDS as an unsigned integer.
	  /// THIS IS ONE BASED (not zero) to correspond with the manual.
	  /// Will throw Exception if index is out of bounds.
	  /// </summary>
	  /// <param name="index"> 1 based index </param>
	  /// <returns> input[index-1] & 0xff </returns>
	  public int getOctet(int index)
	  {
		return JavaLangCompat.ToUnsignedInt(input[index - 1]);
	  }

	  public int getInt4StartingAtOctet(int index)
	  {
		return GribNumbers.int4(getOctet(index), getOctet(index + 1), getOctet(index + 2), getOctet(index + 3));
	  }

	  public int getOctetSigned(int index)
	  {
		return GribNumbers.convertSignedByte(input[index - 1]);
	  }

	  public int RawLength
	  {
		  get
		  {
			return input.Length;
		  }
	  }


	  protected internal virtual double getScaledValue(int start)
	  {
		int scale = getOctetSigned(start++);
		int value = GribNumbers.int4(getOctet(start++), getOctet(start++), getOctet(start++), getOctet(start++));
		return applyScaleFactor(scale, value);
	  }

	  public virtual int StatisticalProcessType
	  {
		  get
		  {
			if (!(this is PdsInterval))
			{
			  return -1;
			}
			PdsInterval pint = (PdsInterval) this;
			TimeInterval[] ti = pint.TimeIntervals;
			if (ti.Length > 0)
			{
			  TimeInterval ti0 = ti[0];
			  return ti0.statProcessType;
			}
			return -1;
		  }
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  public interface PdsAerosol
	  {
		int AerosolType {get;}

		double AerosolIntervalSizeType {get;}

		double AerosolSize1 {get;}

		double AerosolSize2 {get;}

		double AerosolIntervalWavelengthType {get;}

		double AerosolWavelength1 {get;}

		double AerosolWavelength2 {get;}
	  }

	  public interface PdsInterval
	  {
		int StatisticalProcessType {get;}

		CalendarDate IntervalTimeEnd {get;}

		int ForecastTime {get;}

		int NumberTimeRanges {get;}

		int NumberMissing {get;}

		TimeInterval[] TimeIntervals {get;}

		long IntervalHash {get;}
	  }

	  public interface PdsEnsemble
	  {
		int PerturbationType {get;}

		int PerturbationNumber {get;}

		int NumberEnsembleForecasts {get;}
	  }

	  public interface PdsEnsembleDerived
	  {
		int DerivedForecastType {get;}

		int NumberEnsembleForecasts {get;}
	  }

	  public interface PdsPercentile
	  {
		int PercentileValue {get;}
	  }

	  public interface PdsSpatialInterval
	  {
		int SpatialStatisticalProcessType {get;}

		int SpatialProcessType {get;}

		int NSpatialDataPoints {get;}
	  }

	  public interface PdsProbability
	  {
		int ForecastProbabilityNumber {get;}

		int NumberForecastProbabilities {get;}

		int ProbabilityType {get;}

		double ProbabilityLowerLimit {get;}

		double ProbabilityUpperLimit {get;}

		int ProbabilityHashcode {get;}

		string ProbabilityName {get;}
	  }

	  public interface PdsSatellite
	  {
		int NumSatelliteBands {get;}

		SatelliteBand[] SatelliteBands {get;}
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.0 - analysis or forecast at a horizontal level or in a horizontal layer at a point in
	  /// time
	  /// Many other templates (1-14) have same fields in sameplaces, so can use this as the superclass.
	  /// </summary>
	  private class Grib2Pds0 : Grib2Pds
	  {

		internal Grib2Pds0(byte[] input) : base(input)
		{
		}

		public override int GenProcessType
		{
			get
			{
			  return getOctet(12);
			}
		}

		/// <summary>
		/// Background generating process identifier (defined by originating centre)
		/// </summary>
		/// <returns> Background generating process id </returns>
		public override int BackProcessId
		{
			get
			{
			  return getOctet(13);
			}
		}

		public override int GenProcessId
		{
			get
			{
			  return getOctet(14);
			}
		}

		/// <summary>
		/// Hours after reference time of data cutoff
		/// </summary>
		/// <returns> HoursAfter </returns>
		public virtual int HoursAfterCutoff
		{
			get
			{
			  return GribNumbers.int2(getOctet(15), getOctet(16));
			}
		}

		/// <summary>
		/// Minutes after reference time of data cutoff
		/// </summary>
		/// <returns> MinutesAfter </returns>
		public virtual int MinutesAfterCutoff
		{
			get
			{
			  return getOctet(17);
			}
		}

		/// <summary>
		/// Indicator of unit of time range (see Code table 4.4)
		/// </summary>
		/// <returns> TimeRangeUnit </returns>
		public override int TimeUnit
		{
			get
			{
			  return getOctet(18);
			}
		}

		/// <summary>
		/// Type of first fixed surface (see Code table 4.5)
		/// </summary>
		/// <returns> Type of first fixed surface </returns>
		public override int LevelType1
		{
			get
			{
			  return getOctet(23);
			}
		}

		/// <summary>
		/// Value of first fixed surface, with scale factor already applied
		/// </summary>
		/// <returns> float FirstFixedSurfaceValue </returns>
		public override double LevelValue1
		{
			get
			{
			  return getScaledValue(24);
			}
		}

		// debug
		public virtual int LevelScale
		{
			get
			{
			  return getOctet(24);
			}
		}

		/// <summary>
		/// Type of second fixed surface (see Code table 4.5)
		/// </summary>
		/// <returns> Type of second fixed surface </returns>
		public override int LevelType2
		{
			get
			{
			  return getOctet(29);
			}
		}

		/// <summary>
		/// Value of second fixed surface, with scale factor already applied
		/// </summary>
		/// <returns> float FirstFixedSurfaceValue </returns>
		public override double LevelValue2
		{
			get
			{
			  return getScaledValue(30);
			}
		}

		/// <summary>
		/// length of template (next byte starts info after template, if any
		/// </summary>
		/// <returns> length of template </returns>
		public override int templateLength()
		{
		  return 34;
		}

	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.15 - average, accumulation, extreme values, or other statistically-processed
	  /// values over a spatial area at a horizontal level or in a horizontal layer at a point in time
	  /// </summary>
	  private class Grib2Pds15 : Grib2Pds0, PdsSpatialInterval
	  {
		internal Grib2Pds15(byte[] input) : base(input)
		{
		}

		// table 4.10
		public virtual int SpatialStatisticalProcessType
		{
			get
			{
			  return getOctet(35);
			}
		}

		// code 4.15
		public virtual int SpatialProcessType
		{
			get
			{
			  return getOctet(36);
			}
		}

		public virtual int NSpatialDataPoints
		{
			get
			{
			  return getOctet(37);
			}
		}

	  }


	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.1 -
	  /// individual ensemble forecast, control and perturbed, at a horizontal level or in a horizontal layer at a point in
	  /// time
	  /// </summary>
	  private class Grib2Pds1 : Grib2Pds0, PdsEnsemble
	  {
		internal Grib2Pds1(byte[] input) : base(input)
		{
		}

		public override bool Ensemble
		{
			get
			{
			  return true;
			}
		}

		/// <summary>
		/// Type of ensemble forecast (see Code table 4.6)
		/// </summary>
		/// <returns> Type of ensemble forecast </returns>
		public virtual int PerturbationType
		{
			get
			{
			  return getOctet(35);
			}
		}

		/// <summary>
		/// Perturbation Number - which member of the ensemble is this ?
		/// </summary>
		/// <returns> Perturbation Number </returns>
		public virtual int PerturbationNumber
		{
			get
			{
			  return getOctet(36);
			}
		}

		/// <summary>
		/// Number of forecasts in ensemble
		/// </summary>
		/// <returns> Number of forecasts in ensemble </returns>
		public virtual int NumberEnsembleForecasts
		{
			get
			{
			  return getOctet(37);
			}
		}

		public override int templateLength()
		{
		  return 37;
		}

	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.11 -
	  /// individual ensemble forecast, control and perturbed, at a horizontal level or in a horizontal layer in a continuous
	  /// or non-continuous time interval
	  /// </summary>
	  private class Grib2Pds11 : Grib2Pds1, PdsInterval
	  {

		internal Grib2Pds11(byte[] input) : base(input)
		{
		}

		/// <summary>
		/// End of overall time interval
		/// </summary>
		/// <returns> End of overall time interval </returns>
		public virtual CalendarDate IntervalTimeEnd
		{
			get
			{
			  return calcTime(38);
			}
		}

		/// <summary>
		/// number of time range specifications describing the time intervals used to calculate the statistically-processed
		/// field
		/// </summary>
		/// <returns> number of time range </returns>
		public virtual int NumberTimeRanges
		{
			get
			{
			  return getOctet(45);
			}
		}

		/// <summary>
		/// Total number of data values missing in statistical process
		/// </summary>
		/// <returns> Total number of data values missing in statistical process </returns>
		public virtual int NumberMissing
		{
			get
			{
			  return GribNumbers.int4(getOctet(46), getOctet(47), getOctet(48), getOctet(49));
			}
		}

		public virtual TimeInterval[] TimeIntervals
		{
			get
			{
			  return readTimeIntervals(NumberTimeRanges, 50);
			}
		}

		public override int templateLength()
		{
		  return 49 + NumberTimeRanges * 12;
		}

		public virtual long IntervalHash
		{
			get
			{
			  CRC32 crc32 = new CRC32();
			  crc32.update(input, 49, NumberTimeRanges * 12);
			  return crc32.getValue();
			}
		}

		public override void show(Formatter f)
		{
		  base.show(f);
		  f.format("%n   Grib2Pds8: endInterval=%s%n", IntervalTimeEnd);
		  foreach (TimeInterval ti in TimeIntervals)
		  {
			ti.show(f);
		  }
		}
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.60 -
	  /// individual ensemble reforecast, control and perturbed, at a horizontal level or in a horizontal layer at a point
	  /// in time
	  /// </summary>
	  private class Grib2Pds60 : Grib2Pds1, PdsEnsemble
	  {

		internal Grib2Pds60(byte[] input) : base(input)
		{
		}

		/// <summary>
		/// Model version date
		/// </summary>
		/// <returns> Model version date </returns>
		public virtual CalendarDate ModelVersionDate
		{
			get
			{
			  return calcTime(38);
			}
		}

		public override int templateLength()
		{
		  return 44;
		}

	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.61 -
	  /// individual ensemble forecast, control and perturbed, at a horizontal level or in a horizontal layer in a continuous
	  /// or non-continuous time interval
	  /// </summary>
	  private class Grib2Pds61 : Grib2Pds1, PdsInterval
	  {

		internal Grib2Pds61(byte[] input) : base(input)
		{
		}

		/// <summary>
		/// Model version date
		/// </summary>
		/// <returns> Model version date </returns>
		public virtual CalendarDate ModelVersionDate
		{
			get
			{
			  return calcTime(38);
			}
		}

		/// <summary>
		/// End of overall time interval
		/// </summary>
		/// <returns> End of overall time interval </returns>
		public virtual CalendarDate IntervalTimeEnd
		{
			get
			{
			  return calcTime(45);
			}
		}

		/// <summary>
		/// number of time range specifications describing the time intervals used to calculate the statistically-processed
		/// field
		/// </summary>
		/// <returns> number of time range </returns>
		public virtual int NumberTimeRanges
		{
			get
			{
			  return getOctet(52);
			}
		}

		/// <summary>
		/// Total number of data values missing in statistical process
		/// </summary>
		/// <returns> Total number of data values missing in statistical process </returns>
		public virtual int NumberMissing
		{
			get
			{
			  return GribNumbers.int4(getOctet(53), getOctet(54), getOctet(55), getOctet(56));
			}
		}

		public virtual TimeInterval[] TimeIntervals
		{
			get
			{
			  return readTimeIntervals(NumberTimeRanges, 57);
			}
		}

		public override int templateLength()
		{
		  return 56 + NumberTimeRanges * 12;
		}

		public virtual long IntervalHash
		{
			get
			{
			  CRC32 crc32 = new CRC32();
			  crc32.update(input, 56, NumberTimeRanges * 12);
			  return crc32.getValue();
			}
		}

		public override void show(Formatter f)
		{
		  base.show(f);
		  f.format("%n   Grib2Pds8: endInterval=%s%n", IntervalTimeEnd);
		  foreach (TimeInterval ti in TimeIntervals)
		  {
			ti.show(f);
		  }
		}
	  }


	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.2 -
	  /// derived forecasts based on all ensemble members at a horizontal level or in a horizontal layer at a point in time
	  /// </summary>
	  private class Grib2Pds2 : Grib2Pds0, PdsEnsembleDerived
	  {

		internal Grib2Pds2(byte[] input) : base(input)
		{
		}

		public override bool EnsembleDerived
		{
			get
			{
			  return true;
			}
		}

		/// <summary>
		/// Derived forecast Type (see Code table 4.7)
		/// </summary>
		/// <returns> Derived forecast Type </returns>
		public virtual int DerivedForecastType
		{
			get
			{
			  return getOctet(35);
			}
		}

		/// <summary>
		/// Number of forecasts in ensemble
		/// </summary>
		/// <returns> Number of forecasts in ensemble </returns>
		public virtual int NumberEnsembleForecasts
		{
			get
			{
			  return getOctet(36);
			}
		}

		public override int templateLength()
		{
		  return 36;
		}

	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.12 -
	  /// derived forecasts based on all ensemble members at a horizontal level or in a horizontal layer in a
	  /// continuous or non-continuous time interval
	  /// </summary>
	  private class Grib2Pds12 : Grib2Pds2, PdsInterval
	  {
		// CalendarDate endInterval; // Date msecs
		// int ft;

		internal Grib2Pds12(byte[] input) : base(input)
		{
		  // endInterval = calcTime(37);
		  // ft = makeForecastTime(endInterval, getTimeUnit());
		}

		/// <summary>
		/// End of overall time interval
		/// </summary>
		/// <returns> End of overall time interval </returns>
		public virtual CalendarDate IntervalTimeEnd
		{
			get
			{
			  return calcTime(37);
			}
		}

		/// <summary>
		/// number of time range specifications describing the time intervals used to calculate the statistically-processed
		/// field
		/// </summary>
		/// <returns> number of time range </returns>
		public virtual int NumberTimeRanges
		{
			get
			{
			  return getOctet(44);
			}
		}

		/// <summary>
		/// Total number of data values missing in statistical process
		/// </summary>
		/// <returns> Total number of data values missing in statistical process </returns>
		public virtual int NumberMissing
		{
			get
			{
			  return GribNumbers.int4(getOctet(45), getOctet(46), getOctet(47), getOctet(48));
			}
		}

		public virtual TimeInterval[] TimeIntervals
		{
			get
			{
			  return readTimeIntervals(NumberTimeRanges, 49);
			}
		}

		public override int templateLength()
		{
		  return 48 + NumberTimeRanges * 12;
		}

		public virtual long IntervalHash
		{
			get
			{
			  CRC32 crc32 = new CRC32();
			  crc32.update(input, 48, NumberTimeRanges * 12);
			  return crc32.getValue();
			}
		}

		public override void show(Formatter f)
		{
		  base.show(f);
		  f.format("%n   Grib2Pds8: endInterval=%s%n", IntervalTimeEnd);
		  foreach (TimeInterval ti in TimeIntervals)
		  {
			ti.show(f);
		  }
		}
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.5 – probability forecasts at a horizontal level or in a horizontal layer at a point
	  /// in time
	  /// Octet No. Contents
	  /// 10 Parameter category (see Code table 4.1)
	  /// 11 Parameter number (see Code table 4.2)
	  /// 12 Type of generating process (see Code table 4.3)
	  /// 13 Background generating process identifier (defined by originating centre)
	  /// 14 Forecast generating process identifier (defined by originating centre)
	  /// 15–16 Hours after reference time of data cut-off (see Note)
	  /// 17 Minutes after reference time of data cut-off
	  /// 18 Indicator of unit of time range (see Code table 4.4)
	  /// 19–22 Forecast time in units defined by octet 18
	  /// 23 Type of first fixed surface (see Code table 4.5)
	  /// 24 Scale factor of first fixed surface
	  /// 25–28 Scaled value of first fixed surface
	  /// 29 Type of second fixed surface (see Code table 4.5)
	  /// 30 Scale factor of second fixed surface
	  /// 31–34 Scaled value of second fixed surface
	  /// 35 Forecast probability number
	  /// 36 Total number of forecast probabilities
	  /// 37 Probability type (see Code table 4.9)
	  /// 38 Scale factor of lower limit
	  /// 39–42 Scaled value of lower limit
	  /// 43 Scale factor of upper limit
	  /// 44–47 Scaled value of upper limit
	  /// Note: Hours greater than 65534 will be coded as 65534
	  /// </summary>
	  private class Grib2Pds5 : Grib2Pds0, PdsProbability
	  {

		internal Grib2Pds5(byte[] input) : base(input)
		{
		}

		public override bool Probability
		{
			get
			{
			  return true;
			}
		}

		/// <summary>
		/// Forecast probability number
		/// </summary>
		/// <returns> Forecast probability number </returns>
		public virtual int ForecastProbabilityNumber
		{
			get
			{
			  return getOctet(35);
			}
		}

		/// <summary>
		/// Number of forecasts probabilities
		/// </summary>
		/// <returns> Number of forecasts probabilities </returns>
		public virtual int NumberForecastProbabilities
		{
			get
			{
			  return getOctet(36);
			}
		}

		/// <summary>
		/// Probability type (see Code table 4.9)
		/// </summary>
		/// <returns> Probability type </returns>
		public virtual int ProbabilityType
		{
			get
			{
			  return getOctet(37);
			}
		}

		public virtual double ProbabilityLowerLimit
		{
			get
			{
			  int scale = getOctetSigned(38);
			  int value = GribNumbers.int4(getOctet(39), getOctet(40), getOctet(41), getOctet(42));
			  return applyScaleFactor(scale, value);
			}
		}

		public virtual double ProbabilityUpperLimit
		{
			get
			{
			  int scale = getOctetSigned(43);
			  int value = GribNumbers.int4(getOctet(44), getOctet(45), getOctet(46), getOctet(47));
			  return applyScaleFactor(scale, value);
			}
		}


		public virtual int ProbabilityHashcode
		{
			get
			{
			  if (probHash == 0)
			  {
				int result = 0;
				long temp;
				double prob1, prob2;

				switch (ProbabilityType)
				{
				  case 0:
				  case 3:
					prob1 = ProbabilityLowerLimit;
					temp = prob1 != 0.0d ? BitConverter.DoubleToInt64Bits(prob1) : 0L;
					result = (int)(temp ^ (temp >>> 32));
					break;

				  case 1:
				  case 4:
					prob2 = ProbabilityUpperLimit;
					temp = prob2 != 0.0d ? BitConverter.DoubleToInt64Bits(prob2) : 0L;
					result = (int)(temp ^ (temp >>> 32));
					break;

				  case 2:
					prob1 = ProbabilityLowerLimit;
					prob2 = ProbabilityUpperLimit;
					temp = prob1 != 0.0d ? BitConverter.DoubleToInt64Bits(prob1) : 0L;
					result = (int)(temp ^ (temp >>> 32));
					temp = prob2 != 0.0d ? BitConverter.DoubleToInt64Bits(prob2) : 0L;
					result = 31 * result + (int)(temp ^ (temp >>> 32));
					break;

				  default:
					break;
				}
				result = 31 * result + ProbabilityType;
				probHash = result;
			  }
			  return probHash;
			}
		}

		internal int probHash;

		/*
		 * Code Table Code table 4.9 - Probability type (4.9)
		 * 0: Probability of event below lower limit
		 * 1: Probability of event above upper limit
		 * 2: Probability of event between lower and upper limits (the range includes the lower limit but not the upper
		 * limit)
		 * 3: Probability of event above lower limit
		 * 4: Probability of event below upper limit
		 * -1: Reserved
		 * -1: Reserved for local use
		 * 255: Missing
		 */
		public virtual string ProbabilityName
		{
			get
			{
			  string result;
			  using (Formatter f = new Formatter())
			  {
				int scale1 = Math.Max(1, getOctet(38));
				int scale2 = Math.Max(1, getOctet(43));

				switch (ProbabilityType)
				{
				  case 0:
					f.format("below_%s", Format.dfrac(ProbabilityLowerLimit, scale1));
					break;
				  case 1:
					f.format("above_%s", Format.dfrac(ProbabilityUpperLimit, scale2));
					break;
				  case 2:
					if (ProbabilityLowerLimit == ProbabilityUpperLimit)
					{
					  f.format("equals_%s", Format.dfrac(ProbabilityLowerLimit, scale1));
					}
					else
					{
					  f.format("between_%s_and_%s", Format.dfrac(ProbabilityLowerLimit, scale1), Format.dfrac(ProbabilityUpperLimit, scale2));
					}
					break;
				  case 3:
					f.format("above_%s", Format.dfrac(ProbabilityLowerLimit, scale1));
					break;
				  case 4:
					f.format("below_%s", Format.dfrac(ProbabilityUpperLimit, scale2));
					break;
				  default:
					f.format("UknownProbType=%d", ProbabilityType);
					break;
				}
				result = StringUtil2.removeFromEnd(f.ToString(), '0');
			  }
			  return StringUtil2.removeFromEnd(result, '.');
			}
		}

		public override int templateLength()
		{
		  return 47;
		}
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.9 – probability forecasts at a horizontal level or in a horizontal layer in a
	  /// continuous or non-continuous time interval
	  /// Octet No. Contents
	  /// 10 Parameter category (see Code table 4.1)
	  /// 11 Parameter number (see Code table 4.2)
	  /// 12 Type of generating process (see Code table 4.3)
	  /// 13 Background generating process identifier (defined by originating centre)
	  /// 14 Forecast generating process identifier (defined by originating centre)
	  /// 15–16 Hours after reference time of data cut-off (see Note 1)
	  /// 17 Minutes after reference time of data cut-off
	  /// 18 Indicator of unit of time range (see Code table 4.4)
	  /// 19–22 Forecast time in units defined by octet 18 (see Note 2)
	  /// 23 Type of first fixed surface (see Code table 4.5)
	  /// 24 Scale factor of first fixed surface
	  /// 25–28 Scaled value of first fixed surface
	  /// 29 Type of second fixed surface (see Code table 4.5)
	  /// 30 Scale factor of second fixed surface
	  /// 31–34 Scaled value of second fixed surface
	  /// 35 Forecast probability number
	  /// 36 Total number of forecast probabilities
	  /// 37 Probability type (see Code table 4.9)
	  /// 38 Scale factor of lower limit
	  /// 39–42 Scaled value of lower limit
	  /// 43 Scale factor of upper limit
	  /// 44–47 Scaled value of upper limit
	  /// 48–49 Year of end of overall time interval
	  /// 50 Month of end of overall time interval
	  /// 51 Day of end of overall time interval
	  /// 52 Hour of end of overall time interval
	  /// 53 Minute of end of overall time interval
	  /// 54 Second of end of overall time interval
	  /// 55 n – number of time range specifications describing the time intervals used to calculate the statistically
	  /// processed field
	  /// 56–59 Total number of data values missing in the statistical process
	  /// 60–71 Specification of the outermost (or only) time range over which statistical processing is done
	  /// 60 Statistical process used to calculate the processed field from the field at each time increment during the time
	  /// range (see Code table 4.10)
	  /// 61 Type of time increment between successive fields used in the statistical processing (see Code table 4.11)
	  /// 62 Indicator of unit of time for time range over which statistical processing is done (see Code table 4.4)
	  /// 63–66 Length of the time range over which statistical processing is done, in units defined by the previous octet
	  /// 67 Indicator of unit of time for the increment between the successive fields used (see Code table 4.4)
	  /// 68–71 Time increment between successive fields, in units defined by the previous octet (see Note 3)
	  /// 72–nn These octets are included only if n > 1, where nn = 59 + 12 x n
	  /// 72–83 As octets 60 to 71, next innermost step of processing
	  /// 84–nn Additional time range specifications, included in accordance with the value of n. Contents
	  /// as octets 60 to 71, repeated as necessary.
	  /// Notes:
	  /// (1) Hours greater than 65534 will be coded as 65534.
	  /// (2) The reference time in section 1 and the forecast time together define the beginning of the overall time
	  /// interval.
	  /// (3) An increment of zero means that the statistical processing is the result of a continuous (or near continuous)
	  /// process, not
	  /// the processing of a number of discrete samples. Examples of such continuous processes are the temperatures measured
	  /// by analogue maximum and minimum thermometers or thermographs, and the rainfall measured by a raingauge.
	  /// The reference and forecast times are successively set to their initial values plus or minus the increment, as
	  /// defined by
	  /// the type of time increment (one of octets 46, 58, 70, ...). For all but the innermost (last) time range, the next
	  /// inner range is
	  /// then processed using these reference and forecast times as the initial reference and forecast times.
	  /// </summary>
	  private class Grib2Pds9 : Grib2Pds5, PdsInterval
	  {
		// CalendarDate endInterval; // Date msecs
		// int ft;

		internal Grib2Pds9(byte[] input) : base(input)
		{
		  // endInterval = calcTime(48);
		  // ft = makeForecastTime(endInterval, getTimeUnit());
		}

		/// <summary>
		/// End of overall time interval
		/// </summary>
		/// <returns> End of overall time interval </returns>
		public virtual CalendarDate IntervalTimeEnd
		{
			get
			{
			  return calcTime(48);
			}
		}

		/*
		 * @Override
		 * public int getForecastTime() {
		 * return ft;
		 * }
		 */

		/// <summary>
		/// number of time range specifications describing the time intervals used to calculate the statistically-processed
		/// field
		/// </summary>
		/// <returns> number of time range </returns>
		public virtual int NumberTimeRanges
		{
			get
			{
			  return getOctet(55);
			}
		}

		/// <summary>
		/// Total number of data values missing in statistical process
		/// </summary>
		/// <returns> Total number of data values missing in statistical process </returns>
		public virtual int NumberMissing
		{
			get
			{
			  return GribNumbers.int4(getOctet(56), getOctet(57), getOctet(58), getOctet(59));
			}
		}

		public virtual TimeInterval[] TimeIntervals
		{
			get
			{
			  return readTimeIntervals(NumberTimeRanges, 60);
			}
		}

		public override int templateLength()
		{
		  return 59 + NumberTimeRanges * 12;
		}

		public virtual long IntervalHash
		{
			get
			{
			  CRC32 crc32 = new CRC32();
			  crc32.update(input, 59, NumberTimeRanges * 12);
			  return crc32.getValue();
			}
		}

		public override void show(Formatter f)
		{
		  base.show(f);
		  f.format("%n   Grib2Pds9: endInterval=%s%n", IntervalTimeEnd);
		  foreach (TimeInterval ti in TimeIntervals)
		  {
			ti.show(f);
		  }
		}

	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.8 -
	  /// Average, accumulation, extreme values or other statistically processed values at a horizontal level or in a
	  /// horizontal layer in a continuous or non-continuous time interval
	  /// </summary>
	  private class Grib2Pds8 : Grib2Pds0, PdsInterval
	  {
		// CalendarDate endInterval; // Date msecs

		internal Grib2Pds8(byte[] input) : base(input)
		{
		  // endInterval = calcTime(35);
		}

		/// <summary>
		/// End of overall time interval
		/// </summary>
		/// <returns> End of overall time interval </returns>
		public virtual CalendarDate IntervalTimeEnd
		{
			get
			{
			  return calcTime(35);
			}
		}

		/// <summary>
		/// number of time range specifications describing the time intervals used to calculate the statistically-processed
		/// field
		/// </summary>
		/// <returns> number of time range </returns>
		public virtual int NumberTimeRanges
		{
			get
			{
			  return getOctet(42);
			}
		}

		/// <summary>
		/// Total number of data values missing in statistical process
		/// </summary>
		/// <returns> Total number of data values missing in statistical process </returns>
		public virtual int NumberMissing
		{
			get
			{
			  return GribNumbers.int4(getOctet(43), getOctet(44), getOctet(45), getOctet(46));
			}
		}

		public virtual TimeInterval[] TimeIntervals
		{
			get
			{
			  return readTimeIntervals(NumberTimeRanges, 47);
			}
		}

		public override int templateLength()
		{
		  return 46 + NumberTimeRanges * 12;
		}

		public override void show(Formatter f)
		{
		  base.show(f);
		  try
		  {
			f.format("%n   Grib2Pds8: endInterval=%s%n", IntervalTimeEnd);
			foreach (TimeInterval ti in TimeIntervals)
			{
			  ti.show(f);
			}
		  }
		  catch (Exception t)
		  {
			f.format("%n   Grib2Pds8: endInterval error=%s%n", t.Message);
		  }
		}

		public virtual long IntervalHash
		{
			get
			{
			  CRC32 crc32 = new CRC32();
			  crc32.update(input, 46, 12);
			  if (NumberTimeRanges > 1)
			  {
				crc32.update(input, 58, 1);
			  }
			  return crc32.getValue();
			}
		}

	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.6 -
	  /// percentile forecasts at a horizontal level or in a horizontal layer at a point in time
	  /// </summary>
	  private class Grib2Pds6 : Grib2Pds0, PdsPercentile
	  {

		internal Grib2Pds6(byte[] input) : base(input)
		{
		}

		public override bool Percentile
		{
			get
			{
			  return true;
			}
		}

		/// <summary> Percentile - from 0 to 100 </summary>
		public virtual int PercentileValue
		{
			get
			{
			  return getOctet(35);
			}
		}

		public override int templateLength()
		{
		  return 36;
		}
	  }

	  /// <summary>
	  /// Product definition template 4.10 -
	  /// percentile forecasts at a horizontal level or in a horizontal layer in a continuous or non-continuous time interval
	  /// </summary>
	  private class Grib2Pds10 : Grib2Pds6, PdsInterval
	  {
		// CalendarDate endInterval;

		internal Grib2Pds10(byte[] input) : base(input)
		{
		  // endInterval = calcTime(36);
		}

		/// <summary>
		/// End of overall time interval
		/// </summary>
		/// <returns> End of overall time interval </returns>
		public virtual CalendarDate IntervalTimeEnd
		{
			get
			{
			  return calcTime(36);
			}
		}

		/// <summary>
		/// number of time range specifications describing the time intervals used to calculate the statistically-processed
		/// field
		/// </summary>
		/// <returns> number of time range </returns>
		public virtual int NumberTimeRanges
		{
			get
			{
			  return getOctet(43);
			}
		}

		/// <summary>
		/// Total number of data values missing in statistical process
		/// </summary>
		/// <returns> Total number of data values missing in statistical process </returns>
		public virtual int NumberMissing
		{
			get
			{
			  return GribNumbers.int4(getOctet(44), getOctet(45), getOctet(46), getOctet(47));
			}
		}

		public virtual TimeInterval[] TimeIntervals
		{
			get
			{
			  return readTimeIntervals(NumberTimeRanges, 48);
			}
		}

		public override int templateLength()
		{
		  return 48 + NumberTimeRanges * 12;
		}

		public virtual long IntervalHash
		{
			get
			{
			  CRC32 crc32 = new CRC32();
			  crc32.update(input, 48, NumberTimeRanges * 12);
			  return crc32.getValue();
			}
		}

	  }


	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.30 - satellite product
	  /// </summary>
	  /// <deprecated> 4.31 should be used </deprecated>
	  private class Grib2Pds30 : Grib2Pds, PdsSatellite
	  {

		internal Grib2Pds30(byte[] input) : base(input)
		{
		  log.debug("Product Definition Template 4.30 is deprecated in favor of 4.31 (WMO Manual on Codes");
		}

		// LOOK - could put this into a dummy superclass in case others need

		public override int TimeUnit
		{
			get
			{
			  return 0;
			}
		}

		public override int ForecastTime
		{
			get
			{
			  return 0;
			}
		}

		/// <summary>
		/// Observation generating process identifier (defined by originating centre)
		/// </summary>
		/// <returns> GenProcess </returns>
		public override int GenProcessId
		{
			get
			{
			  return getOctet(13);
			}
		}

		/// <summary>
		/// Number of contributing spectral bands (NB)
		/// </summary>
		/// <returns> Number of contributing spectral </returns>
		public virtual int NumSatelliteBands
		{
			get
			{
			  return getOctet(14);
			}
		}

		/// <summary>
		/// SatelliteBand
		/// </summary>
		/// <returns> SatelliteBands </returns>
		public virtual SatelliteBand[] SatelliteBands
		{
			get
			{
			  int nb = NumSatelliteBands;
			  SatelliteBand[] result = new SatelliteBand[nb];
			  int pos = 15;
			  for (int i = 0; i < nb; i++)
			  {
				SatelliteBand sb = new SatelliteBand();
				sb.series = GribNumbers.int2(getOctet(pos), getOctet(pos + 1));
				sb.number = GribNumbers.int2(getOctet(pos + 2), getOctet(pos + 3));
				sb.instrumentType = getOctet(pos + 4);
				int scaleFactor = getOctetSigned(pos + 5);
				int svalue = GribNumbers.int4(getOctet(pos + 6), getOctet(pos + 7), getOctet(pos + 8), getOctet(pos + 9));
				sb.value = applyScaleFactor(scaleFactor, svalue);
				pos += 10;
				result[i] = sb;
			  }
			  return result;
			}
		}

		public override int templateLength()
		{
		  return 14 + NumSatelliteBands * 10;
		}
	  }

	  public class SatelliteBand
	  {
		public int series; // Satellite series of band nb (code table defined by originating/generating centre)
		public int number; // Satellite numbers of band nb (code table defined by originating/generating centre)
		public int instrumentType; // Instrument types of band nb (code table defined by originating/generating centre)
		public double value; // value of central wave number of band nb (units: m**-1)
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.31 - satellite product
	  /// </summary>
	  private class Grib2Pds31 : Grib2Pds, PdsSatellite
	  {

		internal const int octetsPerBand = 11;

		internal Grib2Pds31(byte[] input) : base(input)
		{
		}

		// LOOK - could put this into a dummy superclass in case others need

		public override int TimeUnit
		{
			get
			{
			  return 0;
			}
		}

		public override int ForecastTime
		{
			get
			{
			  return 0;
			}
		}

		/// <summary>
		/// Observation generating process identifier (defined by originating centre)
		/// </summary>
		/// <returns> GenProcess </returns>
		public override int GenProcessId
		{
			get
			{
			  return getOctet(13);
			}
		}

		/// <summary>
		/// Number of contributing spectral bands (NB)
		/// </summary>
		/// <returns> Number of contributing spectral </returns>
		public virtual int NumSatelliteBands
		{
			get
			{
			  return getOctet(14);
			}
		}

		/// <summary>
		/// SatelliteBand
		/// </summary>
		/// <returns> SatelliteBands </returns>
		public virtual SatelliteBand[] SatelliteBands
		{
			get
			{
			  int nb = NumSatelliteBands;
			  SatelliteBand[] result = new SatelliteBand[nb];
			  int pos = 15;
			  for (int i = 0; i < nb; i++)
			  {
				SatelliteBand sb = new SatelliteBand();
				sb.series = GribNumbers.int2(getOctet(pos), getOctet(pos + 1));
				sb.number = GribNumbers.int2(getOctet(pos + 2), getOctet(pos + 3));
				sb.instrumentType = GribNumbers.int2(getOctet(pos + 4), getOctet(pos + 5));
				int scaleFactor = getOctetSigned(pos + 6);
				int svalue = GribNumbers.int4(getOctet(pos + 7), getOctet(pos + 8), getOctet(pos + 9), getOctet(pos + 10));
				sb.value = applyScaleFactor(scaleFactor, svalue);
				pos += octetsPerBand;
				result[i] = sb;
			  }
			  return result;
			}
		}

		public override int templateLength()
		{
		  return 14 + NumSatelliteBands * octetsPerBand;
		}
	  }

	  ///////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Product definition template 4.32 - satellite product
	  /// </summary>
	  private class Grib2Pds32 : Grib2Pds, PdsSatellite
	  {

		internal const int octetsPerBand = 11;

		internal Grib2Pds32(byte[] input) : base(input)
		{
		}

		public override int GenProcessType
		{
			get
			{
			  return getOctet(12);
			}
		}

		public override int BackProcessId
		{
			get
			{
			  return getOctet(13);
			}
		}

		public override int GenProcessId
		{
			get
			{
			  return getOctet(14);
			}
		}

		public virtual int HoursAfterCutoff
		{
			get
			{
			  return GribNumbers.int2(getOctet(15), getOctet(16));
			}
		}

		public virtual int MinutesAfterCutoff
		{
			get
			{
			  return getOctet(17);
			}
		}

		public override int TimeUnit
		{
			get
			{
			  return getOctet(18);
			}
		}

		/// <summary>
		/// Number of contributing spectral bands (NB)
		/// </summary>
		/// <returns> Number of contributing spectral </returns>
		public virtual int NumSatelliteBands
		{
			get
			{
			  return getOctet(23);
			}
		}

		/// <summary>
		/// SatelliteBand
		/// </summary>
		/// <returns> SatelliteBands </returns>
		public virtual SatelliteBand[] SatelliteBands
		{
			get
			{
			  int nb = NumSatelliteBands;
			  SatelliteBand[] result = new SatelliteBand[nb];
			  int pos = 24;
			  for (int i = 0; i < nb; i++)
			  {
				SatelliteBand sb = new SatelliteBand();
				sb.series = GribNumbers.int2(getOctet(pos), getOctet(pos + 1));
				sb.number = GribNumbers.int2(getOctet(pos + 2), getOctet(pos + 3));
				sb.instrumentType = GribNumbers.int2(getOctet(pos + 4), getOctet(pos + 5));
				int scaleFactor = getOctetSigned(pos + 6);
				int svalue = GribNumbers.int4(getOctet(pos + 7), getOctet(pos + 8), getOctet(pos + 9), getOctet(pos + 10));
				sb.value = applyScaleFactor(scaleFactor, svalue);
				pos += octetsPerBand;
				result[i] = sb;
			  }
			  return result;
			}
		}

		public override int templateLength()
		{
		  return 24 + NumSatelliteBands * octetsPerBand;
		}
	  }

	  ///////////////////////////////////////////////////////////////////////////////

	  /*
	   * Product definition template 4.40 – atmospheric constituents
	   * Octet No. Contents
	   * 10 Parameter category (see Code table 4.1)
	   * 11 Parameter number (see Code table 4.2)
	   * 12–13 Constituent Type (see Code Table 4.230)
	   * 14 Type of Generating Process (see Code table 4.3)
	   * 15 Background Process
	   * 16 Generating Process Identifier
	   * 17–18 Hours of observational data cut-off after reference time (see Note)
	   * 19 Minutes of observational data cut-off after reference time
	   * 20 Indicator of unit of time range (see Code table 4.4)
	   * 21-24 Forecast time in units defined by octet 18
	   * 25 Type of first fixed surface (see Code table 4.5)
	   * 26 Scale factor of first fixed surface
	   * 27–30 Scaled value of first fixed surface
	   * 31 Type of second fixed surface (see Code table 4.5)
	   * 32 Scale factor of second fixed surface
	   * 33-36 Scaled value of second fixed surface
	   * Note: Hours greater than 65534 will be coded as 65534.
	   */

	  private class Grib2Pds40 : Grib2Pds
	  {

		internal Grib2Pds40(byte[] input) : base(input)
		{
		}

		// 12–13 Constituent type (see Code Table 4.230)
		public virtual int ConstituentType
		{
			get
			{
			  return GribNumbers.uint2(getOctet(12), getOctet(13));
			}
		}

		// 14 Type of generating process (see Code table 4.3)
		public override int GenProcessType
		{
			get
			{
			  return getOctet(14);
			}
		}

		// 15 Background generating process identifier (defined by originating centre)
		public override int BackProcessId
		{
			get
			{
			  return getOctet(15);
			}
		}

		// 16 Analysis or forecast generating process identifier (defined by originating centre)
		public override int GenProcessId
		{
			get
			{
			  return getOctet(16);
			}
		}

		// 17–18 Hours of observational data cut-off after reference time (see Note)
		// Note: Hours greater than 65534 will be coded as 65534.
		public virtual int HoursAfterCutoff
		{
			get
			{
			  return GribNumbers.int2(getOctet(17), getOctet(18));
			}
		}

		// 19 Minutes of observational data cut-off after reference time
		public virtual int MinutesAfterCutoff
		{
			get
			{
			  return getOctet(19);
			}
		}

		// 20 Indicator of unit of time range (see Code table 4.4)
		public override int TimeUnit
		{
			get
			{
			  return getOctet(20);
			}
		}

		// 21–24 Forecast time in units defined by octet 20
		public override int ForecastTime
		{
			get
			{
			  return GribNumbers.int4(getOctet(21), getOctet(22), getOctet(23), getOctet(24));
			}
		}

		// 25 Type of first fixed surface (see Code table 4.5)
		public override int LevelType1
		{
			get
			{
			  return getOctet(25);
			}
		}

		// 26 Scale factor of first fixed surface
		public override int LevelScale1
		{
			get
			{
			  return getOctet(26);
			}
		}

		// 27–30 Scaled value of first fixed surface
		public override double LevelValue1
		{
			get
			{
			  return getScaledValue(26);
			}
		}

		// 31 Type of second fixed surface (see Code table 4.5)
		public override int LevelType2
		{
			get
			{
			  return getOctet(31);
			}
		}

		// 32 Scale factor of second fixed surface
		public override int LevelScale2
		{
			get
			{
			  return getOctet(32);
			}
		}

		// 33–36 Scaled value of second fixed surface
		public override double LevelValue2
		{
			get
			{
			  return getScaledValue(32);
			}
		}

		public override int templateLength()
		{
		  return 36;
		}
	  }

	  ///////////////////////////////////////////////////////////////////////////////

	  /*
	   * Product definition template 4.41 – individual ensemble forecast, control and perturbed, for atmospheric
	   * constituents
	   * Octet No. Contents
	   * 10 Parameter category (see Code table 4.1)
	   * 11 Parameter number (see Code table 4.2)
	   * 12–13 Constituent Type (see Code Table 4.230)
	   * 14 Type of Generating Process (see Code table 4.3)
	   * 15 Background Process
	   * 16 Generating Process Identifier
	   * 17–18 Hours of observational data cut-off after reference time (see Note)
	   * 19 Minutes of observational data cut-off after reference time
	   * 20 Indicator of unit of time range (see Code table 4.4)
	   * 21-24 Forecast time in units defined by octet 18
	   * 25 Type of first fixed surface (see Code table 4.5)
	   * 26 Scale factor of first fixed surface
	   * 27–30 Scaled value of first fixed surface
	   * 31 Type of second fixed surface (see Code table 4.5)
	   * 32 Scale factor of second fixed surface
	   * 33-36 Scaled value of second fixed surface
	   * 37 Type of ensemble forecast (see Code table 4.6)
	   * 38 Perturbation number
	   * 39 Number of forecasts in ensemble
	   * Note: Hours greater than 65534 will be coded as 65534.
	   */

	  private class Grib2Pds41 : Grib2Pds40, PdsEnsemble
	  {

		internal Grib2Pds41(byte[] input) : base(input)
		{
		}

		public override bool Ensemble
		{
			get
			{
			  return true;
			}
		}

		/* Type of ensemble forecast (see Code table 4.6) */
		public virtual int PerturbationType
		{
			get
			{
			  return getOctet(37);
			}
		}

		/* Perturbation Ensemble Member Number */
		public virtual int PerturbationNumber
		{
			get
			{
			  return getOctet(38);
			}
		}

		/* Number of forecasts in ensemble */
		public virtual int NumberEnsembleForecasts
		{
			get
			{
			  return getOctet(39);
			}
		}

		public override int templateLength()
		{
		  return 39;
		}
	  }

	  ///////////////////////////////////////////////////////////////////////////////

	  /*
	   * Product definition template 4.48 – analysis or forecast at a horizontal level or in a horizontal layer at a point
	   * in time for optical properties of aerosol
	   * Octet No. Contents
	   * 10 Parameter category (see Code table 4.1)
	   * 11 Parameter number (see Code table 4.2)
	   * 12–13 Aerosol type (see Common Code table C–14)
	   * 14 Type of interval for first and second size (see Code table 4.91)
	   * 15 Scale factor of first size
	   * 16–19 Scaled value of first size in metres
	   * 20 Scale factor of second size
	   * 21–24 Scaled value of second size in metres
	   * 25 Type of interval for first and second wavelength (see Code table 4.91)
	   * 26 Scale factor of first wavelength
	   * 27–30 Scaled value of first wavelength in metres
	   * 31 Scale factor of second wavelength
	   * 32–35 Scaled value of second wavelength in metres
	   * 36 Type of generating process (see Code table 4.3)
	   * 37 Background generating process identifier (defined by originating centre)
	   * 38 Analysis or forecast generating process identifier (defined by originating centre)
	   * 39–40 Hours of observational data cut-off after reference time (see Note)
	   * 41 Minutes of observational data cut-off after reference time
	   * 42 Indicator of unit of time range (see Code table 4.4)
	   * 43–46 Forecast time in units defined by octet 42
	   * 47 Type of first fixed surface (see Code table 4.5)
	   * 48 Scale factor of first fixed surface
	   * 49–52 Scaled value of first fixed surface
	   * 53 Type of second fixed surface (see Code table 4.5)
	   * 54 Scale factor of second fixed surface
	   * 55–58 Scaled value of second fixed surface
	   * Note: Hours greater than 65534 will be coded as 65534.
	   */

	  private class Grib2Pds48 : Grib2Pds, PdsAerosol
	  {

		internal Grib2Pds48(byte[] input) : base(input)
		{
		}

		// 12–13 Aerosol type (see Common Code table C–14)
		public virtual int AerosolType
		{
			get
			{
			  return GribNumbers.uint2(getOctet(12), getOctet(13));
			}
		}

		// 14 Type of interval for first and second size (see Code table 4.91)
		public virtual double AerosolIntervalSizeType
		{
			get
			{
			  return getOctet(14);
			}
		}

		// 15 Scale factor of first size
		// 16–19 Scaled value of first size in metres
		public virtual double AerosolSize1
		{
			get
			{
			  return getScaledValue(15);
			}
		}

		// 20 Scale factor of second size
		// 21–24 Scaled value of second size in metres
		public virtual double AerosolSize2
		{
			get
			{
			  return getScaledValue(20);
			}
		}

		// 25 Type of interval for first and second wavelength (see Code table 4.91)
		public virtual double AerosolIntervalWavelengthType
		{
			get
			{
			  return getOctet(25);
			}
		}

		// 26 Scale factor of first wavelength
		// 27–30 Scaled value of first wavelength in metres
		public virtual double AerosolWavelength1
		{
			get
			{
			  return getScaledValue(26);
			}
		}

		// 31 Scale factor of second wavelength
		// 32–35 Scaled value of second wavelength in metres
		public virtual double AerosolWavelength2
		{
			get
			{
			  return getScaledValue(31);
			}
		}

		// 36 Type of generating process (see Code table 4.3)
		public override int GenProcessType
		{
			get
			{
			  return getOctet(36);
			}
		}

		// 37 Background generating process identifier (defined by originating centre)
		public override int BackProcessId
		{
			get
			{
			  return getOctet(37);
			}
		}

		// 38 Analysis or forecast generating process identifier (defined by originating centre)
		public override int GenProcessId
		{
			get
			{
			  return getOctet(38);
			}
		}

		// 39–40 Hours of observational data cut-off after reference time (see Note)
		// Note: Hours greater than 65534 will be coded as 65534.
		public virtual int HoursAfterCutoff
		{
			get
			{
			  return GribNumbers.int2(getOctet(39), getOctet(40));
			}
		}

		// 41 Minutes of observational data cut-off after reference time
		public virtual int MinutesAfterCutoff
		{
			get
			{
			  return getOctet(41);
			}
		}

		// 42 Indicator of unit of time range (see Code table 4.4)
		public override int TimeUnit
		{
			get
			{
			  return getOctet(42);
			}
		}

		// 43–46 Forecast time in units defined by octet 42
		public override int ForecastTime
		{
			get
			{
			  return GribNumbers.int4(getOctet(43), getOctet(44), getOctet(45), getOctet(46));
			}
		}

		// 47 Type of first fixed surface (see Code table 4.5)
		public override int LevelType1
		{
			get
			{
			  return getOctet(47);
			}
		}

		// 48 Scale factor of first fixed surface
		public override int LevelScale1
		{
			get
			{
			  return getOctet(48);
			}
		}

		// 49–52 Scaled value of first fixed surface
		public override double LevelValue1
		{
			get
			{
			  return getScaledValue(48);
			}
		}

		// 53 Type of second fixed surface (see Code table 4.5)
		public override int LevelType2
		{
			get
			{
			  return getOctet(53);
			}
		}

		// 54 Scale factor of second fixed surface
		public override int LevelScale2
		{
			get
			{
			  return getOctet(54);
			}
		}

		// 55–58 Scaled value of second fixed surface
		public override double LevelValue2
		{
			get
			{
			  return getScaledValue(54);
			}
		}

		public override int templateLength()
		{
		  return 58;
		}
	  }

	  //////////////////////////////////////////////////////////////////////////////////

	  // translate 7 byte time into CalendarDate
	  // null means use refTime
	  protected internal virtual CalendarDate calcTime(int startIndex)
	  {

		int year = GribNumbers.int2(getOctet(startIndex++), getOctet(startIndex++));
		int month = getOctet(startIndex++);
		int day = getOctet(startIndex++);
		int hour = getOctet(startIndex++);
		int minute = getOctet(startIndex++);
		int second = getOctet(startIndex++);

		if ((year == 0) && (month == 0) && (day == 0) && (hour == 0) && (minute == 0) && (second == 0))
		{
		  return CalendarDate.UNKNOWN;
		}

		// href.t00z.prob.f36.grib2
		if (hour > 23)
		{
		  day += (hour / 24);
		  hour = hour % 24;
		}

		return CalendarDate.of(null, year, month, day, hour, minute, second);
	  }

	  /// <summary>
	  /// Apply scale factor to value, return a double result.
	  /// </summary>
	  /// <param name="scale"> signed scale factor </param>
	  /// <param name="value"> apply to this value </param>
	  /// <returns> value ^ -scale </returns>
	  internal virtual double applyScaleFactor(int scale, int value)
	  {
		return ((scale == 0) || (scale == 255) || (value == 0)) ? value : value * Math.Pow(10, -scale);
	  }

	  internal virtual TimeInterval[] readTimeIntervals(int n, int startIndex)
	  {
		TimeInterval[] result = new TimeInterval[n];
		for (int i = 0; i < n; i++)
		{
		  TimeInterval ti = new TimeInterval();
		  ti.statProcessType = getOctet(startIndex++);
		  ti.timeIncrementType = getOctet(startIndex++);
		  ti.timeRangeUnit = getOctet(startIndex++);
		  ti.timeRangeLength = GribNumbers.int4(getOctet(startIndex++), getOctet(startIndex++), getOctet(startIndex++), getOctet(startIndex++));
		  ti.timeIncrementUnit = getOctet(startIndex++);
		  ti.timeIncrement = GribNumbers.int4(getOctet(startIndex++), getOctet(startIndex++), getOctet(startIndex++), getOctet(startIndex++));
		  result[i] = ti;
		}
		return result;
	  }

	  public class TimeInterval
	  {
		public int statProcessType; // (code table 4.10) Statistical process used to calculate the processed field from the
		// field at each time increment during the time range
		public int timeIncrementType; // (code table 4.11) Type of time increment between successive fields used in the
		// statistical processing<
		public int timeRangeUnit; // (code table 4.4) Indicator of unit of time for time range over which statistical
		// processing is done
		public int timeRangeLength; // Length of the time range over which statistical processing is done, in units defined
		// by the previous octet
		public int timeIncrementUnit; // (code table 4.4) Indicator of unit of time for the increment between the successive
		// fields used
		public int timeIncrement; // Time increment between successive fields, in units defined by the previous octet

		public virtual void show(Formatter f)
		{
		  f.format("  TimeInterval: statProcessType= %d, timeIncrementType= %d, timeRangeUnit= %d, timeRangeLength= %d, timeIncrementUnit= %d, timeIncrement=%d%n", statProcessType, timeIncrementType, timeRangeUnit, timeRangeLength, timeIncrementUnit, timeIncrement);
		}
	  }
	}

}
