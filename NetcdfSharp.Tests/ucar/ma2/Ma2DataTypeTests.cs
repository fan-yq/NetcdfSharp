// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestDataType.java


using System;
using System.Numerics;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestDataType - tests unsigned-number widening helpers on DataTypes.
	/// </summary>
	public class Ma2DataTypeTests
	{
		[Fact]
		public void TestUnsignedLongToBigInt()
		{
			Assert.Equal(new BigInteger(long.MaxValue), DataTypes.unsignedLongToBigInt(long.MaxValue));

			BigInteger overflowAsBigInt = new BigInteger(long.MaxValue) + BigInteger.One;
			Assert.Equal(BigInteger.Parse("9223372036854775808"), overflowAsBigInt);

			// low 64 bits of 2^63 reinterpreted as signed long = long.MinValue
			long overflowAsLong = unchecked((long)(ulong)overflowAsBigInt);
			Assert.Equal(long.MinValue, overflowAsLong);

			// Interpret signed long as unsigned long
			Assert.Equal(overflowAsBigInt, DataTypes.unsignedLongToBigInt(overflowAsLong));
		}

		[Fact]
		public void ConvertUnsignedByte()
		{
			// Widen regardless
			AssertSameNumber((short)12, DataTypes.widenNumber((byte)12));

			// Widen only if negative.
			AssertSameNumber((byte)12, DataTypes.widenNumberIfNegative((byte)12));
			AssertSameNumber((short)155, DataTypes.widenNumberIfNegative((byte)155));
			AssertSameNumber((short)224, DataTypes.widenNumberIfNegative((byte)224));
		}

		[Fact]
		public void ConvertUnsignedShort()
		{
			// Widen regardless
			AssertSameNumber(3251, DataTypes.widenNumber((short)3251));

			// Widen only if negative.
			AssertSameNumber((short)3251, DataTypes.widenNumberIfNegative((short)3251));
			AssertSameNumber(40000, DataTypes.widenNumberIfNegative((short)(unchecked((short)40000))));
			AssertSameNumber(43314, DataTypes.widenNumberIfNegative((short)(unchecked((short)43314))));
		}

		[Fact]
		public void ConvertUnsignedInt()
		{
			// Widen regardless
			AssertSameNumber(123456L, DataTypes.widenNumber(123456));

			// Widen only if negative.
			AssertSameNumber(123456, DataTypes.widenNumberIfNegative(123456));
			AssertSameNumber(3500000000L, DataTypes.widenNumberIfNegative(unchecked((int)3500000000L)));
			AssertSameNumber(4289531025L, DataTypes.widenNumberIfNegative(unchecked((int)4289531025L)));
		}

		[Fact]
		public void ConvertUnsignedLong()
		{
			// The maximum signed long is 9223372036854775807.
			// So, the number below fits in an unsigned long, but not a signed long.
			BigInteger tenQuintillion = BigInteger.Parse("10000000000000000000"); // Nineteen zeros.

			// Widen regardless
			AssertSameNumber(new BigInteger(3372036854775L), DataTypes.widenNumber(3372036854775L));

			// Widen only if negative.
			AssertSameNumber(3372036854775L, DataTypes.widenNumberIfNegative(3372036854775L));
			long overflowed = unchecked((long)(ulong)tenQuintillion); // low 64 bits of 10^19
			AssertSameNumber(tenQuintillion, DataTypes.widenNumberIfNegative(overflowed));
			AssertSameNumber(BigInteger.Parse("18446620616920539271"), DataTypes.widenNumberIfNegative(-123456789012345L));
		}

		// Asserts that numbers have the same type and value.
		private static void AssertSameNumber(object expected, object actual)
		{
			Assert.Equal(expected.GetType(), actual.GetType());
			Assert.Equal(expected, actual);
		}
	}
}
