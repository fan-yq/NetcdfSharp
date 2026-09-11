// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/TestGribNumbers.java


using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2.grib;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.TestGribNumbers - signed-byte conversion helpers.
	/// </summary>
	public class GribNumbersTests
	{
		[Fact]
		public void TestConvertSignedByte()
		{
			for (int i = 125; i < 256; i++)
			{
				byte b = (byte)i;
				Assert.Equal(GribNumbers.convertSignedByte(b), GribNumbers.convertSignedByte2(b));
			}
		}

		[Fact]
		public void TestConvertUnsigned()
		{
			int val = DataTypes.unsignedByteToShort(unchecked((byte)-200));
			int val2 = DataTypes.unsignedShortToInt((short)-200);
			Assert.NotEqual(val, val2);
		}
	}
}
