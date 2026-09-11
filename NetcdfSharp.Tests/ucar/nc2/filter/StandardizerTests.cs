// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/filter/TestStandardizer.java


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2.filter;
using Xunit;

namespace NetcdfSharp.ucar.nc2.filter
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.filter.TestStandardizer - Standardizer filter.
	/// </summary>
	public class StandardizerTests
	{
		public const double DATA_MEAN = 10.0;
		public const double DATA_STDDEV = 3.3166247903554;
		public const double TOLERANCE = 1.0E-9;

		public static readonly double[] DOUBLES = { double.NaN, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0 };
		public static readonly NcArray DATA_DOUBLES = NcArray.makeFromJavaArray(DOUBLES);
		public static readonly double[] SAMEDOUBLES = { double.NaN, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0 };
		public static readonly NcArray DATA_SAMEDOUBLES = NcArray.makeFromJavaArray(SAMEDOUBLES);

		public static readonly float[] FLOATS = { float.NaN, 5.0F, 6.0F, 7.0F, 8.0F, 9.0F, 10.0F, 11.0F, 12.0F, 13.0F, 14.0F, 15.0F };
		public static readonly NcArray DATA_FLOATS = NcArray.makeFromJavaArray(FLOATS);

		public double[] standardize(NcArray arr)
		{
			double[] standardizedSample = new double[(int)arr.Size];
			IndexIterator iterArr = arr.getIndexIterator();
			for (int i = 0; i < (int)arr.Size; i++)
			{
				object value = iterArr.ObjectNext;
				standardizedSample[i] = (Convert.ToDouble(value) - DATA_MEAN) / DATA_STDDEV;
			}
			return standardizedSample;
		}

		[Fact]
		public void TestConvertDouble()
		{
			Standardizer filter = Standardizer.createFromArray(DATA_DOUBLES, DataType.DOUBLE);
			double[] dataStandardized = standardize(DATA_DOUBLES);
			NcArray dataConverted = filter.convert(DATA_DOUBLES);
			Assert.True(MAMath.nearlyEquals(dataConverted, NcArray.makeFromJavaArray(dataStandardized)));
		}

		[Fact]
		public void TestConvertSameDouble()
		{
			Standardizer filter = Standardizer.createFromArray(DATA_SAMEDOUBLES, DataType.DOUBLE);
			NcArray dataConverted = filter.convert(DATA_SAMEDOUBLES);
			Assert.True(MAMath.nearlyEquals(dataConverted, DATA_SAMEDOUBLES));
		}

		[Fact]
		public void TestConvertFloat()
		{
			Standardizer filter = Standardizer.createFromArray(DATA_FLOATS, DataType.FLOAT);
			double[] dataStandardized = standardize(DATA_FLOATS);
			NcArray dataConverted = filter.convert(DATA_FLOATS);
			Assert.True(MAMath.nearlyEquals(dataConverted, NcArray.makeFromJavaArray(dataStandardized)));
		}

		[Fact]
		public void TestCalculateMean()
		{
			Standardizer filter = Standardizer.createFromArray(DATA_DOUBLES, DataType.DOUBLE);
			double calcMean = filter.Mean;
			Assert.True(Math.Abs(calcMean - DATA_MEAN) < TOLERANCE);
		}

		[Fact]
		public void TestCalculateMeanFloat()
		{
			Standardizer filter = Standardizer.createFromArray(DATA_FLOATS, DataType.FLOAT);
			double calcMean = filter.Mean;
			Assert.True(Math.Abs(calcMean - DATA_MEAN) < TOLERANCE);
		}

		[Fact]
		public void TestCalculateStandardDeviation()
		{
			Standardizer filter = Standardizer.createFromArray(DATA_DOUBLES, DataType.DOUBLE);
			double calcStd = filter.StdDev;
			Assert.True(Math.Abs(calcStd - DATA_STDDEV) < TOLERANCE);
		}

		[Fact]
		public void TestCalculateStandardDeviationFloat()
		{
			Standardizer filter = Standardizer.createFromArray(DATA_FLOATS, DataType.FLOAT);
			double calcStd = filter.StdDev;
			Assert.True(Math.Abs(calcStd - DATA_STDDEV) < TOLERANCE);
		}
	}
}
