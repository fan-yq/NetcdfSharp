// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/iosp/TestBitReader.java
// Java bytes are signed; in this port byte is unsigned (Plan A), so -1 is written as 255 etc.


using System;
using NetcdfSharp.ucar.nc2.iosp;
using Xunit;

namespace NetcdfSharp.ucar.nc2.iosp
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.iosp.TestBitReader - bit-level unsigned/signed reads.
	/// </summary>
	public class IospBitReaderTests
	{
		[Fact]
		public void TestUcar()
		{
			BitReader bu = new BitReader(new byte[] { 255, 2, 4, 8 }); // Java {-1, 2, 4, 8}
			Assert.Equal(127, bu.bits2UInt(7));
			Assert.Equal(1, bu.bits2UInt(1));
		}

		[Fact]
		public void TestSignedPositive()
		{
			BitReader bu = new BitReader(new byte[] { 32, 0, 0, 0 });
			Assert.Equal(2, (int)bu.bits2SInt(4));
		}

		[Fact]
		public void TestSignedNegative()
		{
			BitReader bu = new BitReader(new byte[] { 160, 0, 0, 0 }); // Java {(byte)160, ...}
			Assert.Equal(-2, (int)bu.bits2SInt(4));
		}

		[Fact]
		public void TestSignedNegative2()
		{
			BitReader bu = new BitReader(new byte[] { 71, 200, 235, 216, 128, 0 });
			Assert.Equal(574, (int)bu.bits2UInt(11));
			Assert.Equal(570, (int)bu.bits2UInt(11));
			Assert.Equal(-945, (int)bu.bits2SInt(11));
		}

		// 1100011111110010 0 0 110 110
		// 012345678901234567890123456789012345678901234567890
		// 1 2 3 4 5
		[Fact]
		public void TestUnsigned()
		{
			byte[] bits = new byte[] { 199, 242, 0, 0, 6, 6 };
			BitReader bu = new BitReader(bits);
			Assert.Equal(799, (int)bu.bits2UInt(10));
			Assert.Equal(800, (int)bu.bits2UInt(10));
			Assert.Equal(0, (int)bu.bits2UInt(10));
			Assert.Equal(6, (int)bu.bits2UInt(10));
			Assert.Equal(6, (int)bu.bits2UInt(8));
		}
	}
}
