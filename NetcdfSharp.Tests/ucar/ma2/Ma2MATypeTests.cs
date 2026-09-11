// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestMAType.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestMAType - per-type element conversion rules.
	/// Numeric types must return numbers from the numeric getters; boolean arrays must throw.
	/// </summary>
	public class Ma2MATypeTests
	{
		private void Convert(NcArray A, bool isBoolean)
		{
			NcIndex ima = A.Index;

			// numeric getters are legal for non-boolean types
			Assert.Equal(!isBoolean, IsLegal(() => A.getDouble(ima)));
			Assert.Equal(!isBoolean, IsLegal(() => A.getFloat(ima)));
			Assert.Equal(!isBoolean, IsLegal(() => A.getLong(ima)));
			Assert.Equal(!isBoolean, IsLegal(() => A.getInt(ima)));
			Assert.Equal(!isBoolean, IsLegal(() => A.getShort(ima)));
			Assert.Equal(!isBoolean, IsLegal(() => A.getByte(ima)));
			Assert.Equal(!isBoolean, IsLegal(() => A.getChar(ima)));

			// the boolean getter is legal only for boolean arrays
			Assert.Equal(isBoolean, IsLegal(() => A.getBoolean(ima)));
		}

		private static bool IsLegal(Func<object> getter)
		{
			try
			{
				getter();
				return true;
			}
			catch (ForbiddenConversionException)
			{
				return false;
			}
		}

		[Fact]
		public void TestConvert()
		{
			int[] shape = new int[1];
			shape[0] = 10;

			Convert(new ArrayDouble(shape), false);
			Convert(new ArrayFloat(shape), false);
			Convert(new ArrayLong(shape, false), false);
			Convert(new ArrayInt(shape, false), false);
			Convert(new ArrayShort(shape, false), false);
			Convert(new ArrayByte(shape, false), false);
			Convert(new ArrayChar(shape), false);
			Convert(new ArrayBoolean(shape), true);
		}
	}
}
