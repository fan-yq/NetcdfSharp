// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/filter/TestFilters.java
// Only the self-contained cases are ported (the rest read src/test/data/filter files that are
// not part of the mock dataset).


using System;
using System.Collections.Generic;
using NetcdfSharp.ucar.nc2.filter;
using Xunit;

namespace NetcdfSharp.ucar.nc2.filter
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.filter.TestFilters - shuffle and fletcher checksum round-trips.
	/// </summary>
	public class FilterTests
	{
		[Fact]
		public void ShouldShuffleDoublesAndLeaveIntAtEnd()
		{
			// two doubles + an int checksum at the end of a chunk.
			// Java's ByteBuffer.putDouble writes big-endian; construct the bytes explicitly.
			byte[] bytes = new byte[2 * 8 + 4];
			Array.Copy(BigEndianDouble(-1.0), 0, bytes, 0, 8);
			Array.Copy(BigEndianDouble(1.0), 0, bytes, 8, 8);
			Array.Copy(BigEndianInt(12345), 0, bytes, 16, 4);

			Dictionary<string, object> props = new Dictionary<string, object>();
			props["id"] = "shuffle";
			props["elementsize"] = 8;
			Shuffle filter = new Shuffle(props);

			// Java expected = {-65, 63, -16, -16, 0, ..., 48, 57}; as unsigned bytes that is
			// {0xBF, 0x3F, 0xF0, 0xF0, ..., 0x30, 0x39}.
			byte[] expected = new byte[] { 0xBF, 0x3F, 0xF0, 0xF0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x30, 0x39 };
			byte[] encoded = filter.encode(bytes);
			Assert.Equal(expected, encoded);
			byte[] decoded = filter.decode(encoded);
			Assert.Equal(bytes, decoded);
		}

		private static byte[] BigEndianDouble(double value)
		{
			byte[] le = BitConverter.GetBytes(value);
			Array.Reverse(le);
			return le;
		}

		private static byte[] BigEndianInt(int value)
		{
			byte[] le = BitConverter.GetBytes(value);
			Array.Reverse(le);
			return le;
		}

		[Fact]
		public void TestFletcher()
		{
			// test case from Wikipedia Fletcher test vectors
			string testString = "abcdefgh";
			int knownChecksum = -1785599007;
			byte[] checksumBytes = BigEndianInt(knownChecksum);
			Checksum32 filter = new Checksum32(Checksum32.CType.FLETCHER);

			byte[] expected = System.Text.Encoding.UTF8.GetBytes(testString);
			byte[] inData = new byte[expected.Length + 4];
			Array.Copy(expected, 0, inData, 0, expected.Length);
			Array.Copy(checksumBytes, 0, inData, expected.Length, 4);

			byte[] outData = filter.decode(inData);
			Assert.Equal(expected, outData);
		}
	}
}
