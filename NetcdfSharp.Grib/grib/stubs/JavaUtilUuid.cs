// Minimal stub for NetcdfSharp.java.util.UUID (used by Grib2Gds template 102 grid identifiers).
using System;

namespace NetcdfSharp.java.util
{
	public class UUID
	{
		private readonly long msb;
		private readonly long lsb;

		public UUID(long msb, long lsb)
		{
			this.msb = msb;
			this.lsb = lsb;
		}

		public override bool Equals(object obj)
		{
			return obj is UUID other && other.msb == msb && other.lsb == lsb;
		}

		public override int GetHashCode()
		{
			return (int) (msb ^ (msb >> 32)) * 31 + (int) (lsb ^ (lsb >> 32));
		}

		public override string ToString()
		{
			return string.Format("{0:x8}-{1:x4}-{2:x4}-{3:x4}-{4:x12}",
				(long) ((msb >> 32) & 0xFFFFFFFFL),
				(msb >> 16) & 0xFFFF,
				msb & 0xFFFF,
				(lsb >> 48) & 0xFFFF,
				lsb & 0xFFFFFFFFFFFFL);
		}
	}
}
