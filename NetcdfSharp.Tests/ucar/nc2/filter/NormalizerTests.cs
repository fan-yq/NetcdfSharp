// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/filter/TestNormalizer.java


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2.filter;
using Xunit;

namespace NetcdfSharp.ucar.nc2.filter
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.filter.TestNormalizer - Normalizer filter.
	/// </summary>
	public class NormalizerTests
	{
		public const double DATA_MIN = -5.0;
		public const double DATA_RANGE = 10.0;
		public const double TOLERANCE = 1.0E-9;

		public static readonly double[] DOUBLES = { double.NaN, -5.0, -4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0, 5.0 };
		public static readonly NcArray DATA_DOUBLES = NcArray.makeFromJavaArray(DOUBLES);

		public static readonly double[] SAMEDOUBLES = { double.NaN, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0 };
		public static readonly NcArray DATA_SAMEDOUBLES = NcArray.makeFromJavaArray(SAMEDOUBLES);

		public static readonly float[] FLOATS = { float.NaN, -5.0F, -4.0F, -3.0F, -2.0F, -1.0F, 0.0F, 1.0F, 2.0F, 3.0F, 4.0F, 5.0F };
		public static readonly NcArray DATA_FLOATS = NcArray.makeFromJavaArray(FLOATS);

		public double[] normalize(NcArray arr)
		{
			double[] normalizedSample = new double[(int)arr.Size];
			IndexIterator iterArr = arr.getIndexIterator();
			for (int i = 0; i < (int)arr.Size; i++)
			{
				object value = iterArr.ObjectNext;
				normalizedSample[i] = (Convert.ToDouble(value) - DATA_MIN) / DATA_RANGE;
			}
			return normalizedSample;
		}

		[Fact]
		public void TestConvertDouble()
		{
			Normalizer filter = Normalizer.createFromArray(DATA_DOUBLES, DataType.DOUBLE);
			double[] dataNormalized = normalize(DATA_DOUBLES);
			NcArray dataConverted = filter.convert(DATA_DOUBLES);
			Assert.True(MAMath.nearlyEquals(dataConverted, NcArray.makeFromJavaArray(dataNormalized)));
		}

		[Fact]
		public void TestConvertSameDouble()
		{
			Normalizer filter = Normalizer.createFromArray(DATA_SAMEDOUBLES, DataType.DOUBLE);
			NcArray dataConverted = filter.convert(DATA_SAMEDOUBLES);
			Assert.True(MAMath.nearlyEquals(dataConverted, DATA_SAMEDOUBLES));
		}

		[Fact]
		public void TestConvertFloat()
		{
			Normalizer filter = Normalizer.createFromArray(DATA_FLOATS, DataType.FLOAT);
			double[] dataNormalized = normalize(DATA_FLOATS);
			NcArray dataConverted = filter.convert(DATA_FLOATS);
			Assert.True(MAMath.nearlyEquals(dataConverted, NcArray.makeFromJavaArray(dataNormalized)));
		}

		[Fact]
		public void TestCalculateMinimum()
		{
			Normalizer filter = Normalizer.createFromArray(DATA_DOUBLES, DataType.DOUBLE);
			double calcMinimum = filter.Minimum;
			Assert.True(Math.Abs(calcMinimum - DATA_MIN) < TOLERANCE);
		}

		[Fact]
		public void TestCalculateMinimumFloat()
		{
			Normalizer filter = Normalizer.createFromArray(DATA_FLOATS, DataType.FLOAT);
			double calcMinimum = filter.Minimum;
			Assert.True(Math.Abs(calcMinimum - DATA_MIN) < TOLERANCE);
		}

		[Fact]
		public void TestCalculateRange()
		{
			Normalizer filter = Normalizer.createFromArray(DATA_DOUBLES, DataType.DOUBLE);
			double calcRange = filter.Range;
			Assert.True(Math.Abs(calcRange - DATA_RANGE) < TOLERANCE);
		}

		[Fact]
		public void TestCalculateRangeFloat()
		{
			Normalizer filter = Normalizer.createFromArray(DATA_FLOATS, DataType.FLOAT);
			double calcRange = filter.Range;
			Assert.True(Math.Abs(calcRange - DATA_RANGE) < TOLERANCE);
		}
	}
}
