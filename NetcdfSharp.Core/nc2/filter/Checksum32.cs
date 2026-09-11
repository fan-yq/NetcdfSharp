// ====================================================================================================
// NetcdfSharp.Core - NetcdfSharp.ucar.nc2.filter.Checksum32
// Rewritten from Java rich-enum (netcdf-java). CType is a class-based enum; the checksum
// implementations (Fletcher32/Adler32/CRC32) are self-contained .NET ports.
// ====================================================================================================

using System;
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.filter
{
	/// <summary>
	/// Filter implementation for 32-bit checksums: Fletcher, Adler, and CRC
	/// </summary>
	public class Checksum32 : Filter
	{
		/// <summary>
		/// Checksum algorithm type. Class-based enum (Java rich-enum).
		/// </summary>
		public sealed class CType
		{
			public static readonly CType FLETCHER = new CType("fletcher32", 3);
			public static readonly CType ADLER = new CType("adler32", -1);
			public static readonly CType CRC = new CType("crc32", -1);

			public string name { get; }
			public int id { get; }

			private CType(string name, int id)
			{
				this.name = name;
				this.id = id;
			}

			public override string ToString() => name;
		}

		private const int nbytes = 4; // number of bytes in the checksum

		private readonly CType type; // type of checksum

		public Checksum32(CType type)
		{
			this.type = type;
		}

		public override string Name => type.name;

		public override int Id => type.id;

		public override byte[] encode(byte[] dataIn)
		{
			// create a checksum
			int checksum = (int)getChecksum(dataIn);
			// append checksum in front or behind data
			// Adler and CRC are supported by Zarr, which follows the NumCodec spec with a checksum before the data
			// Fletcher is support by hdf5, which has the checksum after the data
			byte[] dataOut = new byte[dataIn.Length + nbytes];
			int dataStart = this.type == CType.FLETCHER ? 0 : nbytes;
			System.Array.Copy(dataIn, 0, dataOut, dataStart, dataIn.Length);
			int checksumStart = this.type == CType.FLETCHER ? dataOut.Length - nbytes : 0;
			// encode as little endian by default
			byte[] csum = Ints.toByteArray(JavaLangCompat.ReverseBytes(checksum));
			System.Array.Copy(csum, 0, dataOut, checksumStart, nbytes);
			return dataOut;
		}

		public override byte[] decode(byte[] dataIn)
		{
			// strip the checksum
			byte[] dataOut = new byte[dataIn.Length - nbytes];
			// Adler and CRC are supported by Zarr, which follows the NumCodec spec with a checksum before the data
			// Fletcher is support by hdf5, which has the checksum after the data
			int dataStart = this.type == CType.FLETCHER ? 0 : nbytes;
			System.Array.Copy(dataIn, dataStart, dataOut, 0, dataOut.Length);
			// verify checksum
			int checksum = (int)getChecksum(dataOut);
			byte[] bytes = new byte[nbytes];
			int checksumStart = this.type == CType.FLETCHER ? dataIn.Length - nbytes : 0;
			System.Array.Copy(dataIn, checksumStart, bytes, 0, nbytes);
			int i = JavaLangCompat.ReverseBytes(Ints.fromByteArray(bytes)); // convert from little endian
			if (i != checksum)
			{
				throw new Exception("Checksum invalid");
			}
			return dataOut;
		}

		private long getChecksum(byte[] data)
		{
			IStreamChecksum checksum;
			switch (type)
			{
				case var t when t == CType.ADLER:
					checksum = new Adler32Impl();
					break;
				case var t when t == CType.CRC:
					checksum = new CRC32Impl();
					break;
				default: // FLETCHER
					checksum = new Fletcher32Impl();
					break;
			}
			checksum.update(data, 0, data.Length);
			return checksum.getValue();
		}

		/// <summary>
		/// Minimal checksum interface (NetcdfSharp.java.util.zip.Checksum equivalent).
		/// </summary>
		private interface IStreamChecksum
		{
			void update(byte[] b, int off, int len);
			long getValue();
		}

		/// <summary>
		/// Fletcher-32 (HDF5 flavor), as in netcdf-java Fletcher32 which extends Adler32.
		/// </summary>
		private sealed class Fletcher32Impl : IStreamChecksum
		{
			private long sum1 = 0;
			private long sum2 = 0;

			public void update(byte[] b, int off, int len)
			{
				if (b == null)
				{
					throw new NullReferenceException();
				}
				if (off < 0 || len < 0 || off > b.Length - len)
				{
					throw new IndexOutOfRangeException();
				}

				int i = off;
				int end = len / 2;
				while (end > 0)
				{
					int blocklen = end > 360 ? 360 : end;
					end -= blocklen;
					do
					{
						sum1 += (b[i] & 0xff) << 8 | b[i + 1] & 0xff;
						sum2 += sum1;
						i += 2;
						blocklen--;
					} while (blocklen > 0);
					sum1 = (sum1 & 0xffff) + (sum1 >> 16);
					sum2 = (sum2 & 0xffff) + (sum2 >> 16);
				}

				// handle odd # of bytes
				if (len % 2 > 0)
				{
					sum1 += (b[i] & 0xff) << 8;
					sum2 += sum1;
					sum1 = (sum1 & 0xffff) + (sum1 >> 16);
					sum2 = (sum2 & 0xffff) + (sum2 >> 16);
				}

				sum1 = (sum1 & 0xffff) + (sum1 >> 16);
				sum2 = (sum2 & 0xffff) + (sum2 >> 16);
			}

			public long getValue()
			{
				return (sum2 << 16) | sum1;
			}
		}

		/// <summary>
		/// Adler-32.
		/// </summary>
		private sealed class Adler32Impl : IStreamChecksum
		{
			private const uint MOD_ADLER = 65521;
			private uint a = 1;
			private uint b = 0;

			public void update(byte[] data, int off, int len)
			{
				for (int i = off; i < off + len; i++)
				{
					a = (a + data[i]) % MOD_ADLER;
					b = (b + a) % MOD_ADLER;
				}
			}

			public long getValue()
			{
				return ((long)b << 16) | a;
			}
		}

		/// <summary>
		/// CRC-32 (IEEE 802.3), same polynomial as NetcdfSharp.java.util.zip.CRC32.
		/// </summary>
		private sealed class CRC32Impl : IStreamChecksum
		{
			private const uint POLY = 0xEDB88320;
			private static readonly uint[] TABLE = BuildTable();
			private uint crc = 0xFFFFFFFF;

			private static uint[] BuildTable()
			{
				uint[] table = new uint[256];
				for (uint n = 0; n < 256; n++)
				{
					uint c = n;
					for (int k = 0; k < 8; k++)
					{
						c = (c & 1) != 0 ? POLY ^ (c >> 1) : c >> 1;
					}
					table[n] = c;
				}
				return table;
			}

			public void update(byte[] data, int off, int len)
			{
				for (int i = off; i < off + len; i++)
				{
					crc = (crc >> 8) ^ TABLE[(crc ^ data[i]) & 0xFF];
				}
			}

			public long getValue()
			{
				return ~crc & 0xFFFFFFFFL;
			}
		}

		public class Fletcher32Provider : FilterProvider
		{
			public virtual string Name => CType.FLETCHER.name;
			public virtual int Id => CType.FLETCHER.id;
			public virtual bool canProvide(string name) => name.Equals(CType.FLETCHER.name, StringComparison.OrdinalIgnoreCase);
			public virtual Filter create(IDictionary<string, object> properties) => new Checksum32(CType.FLETCHER);
		}

		public class Adler32Provider : FilterProvider
		{
			public virtual string Name => CType.ADLER.name;
			public virtual int Id => CType.ADLER.id;
			public virtual bool canProvide(string name) => name.Equals(CType.ADLER.name, StringComparison.OrdinalIgnoreCase);
			public virtual Filter create(IDictionary<string, object> properties) => new Checksum32(CType.ADLER);
		}

		public class CRC32Provider : FilterProvider
		{
			public virtual string Name => CType.CRC.name;
			public virtual int Id => CType.CRC.id;
			public virtual bool canProvide(string name) => name.Equals(CType.CRC.name, StringComparison.OrdinalIgnoreCase);
			public virtual Filter create(IDictionary<string, object> properties) => new Checksum32(CType.CRC);
		}
	}
}
