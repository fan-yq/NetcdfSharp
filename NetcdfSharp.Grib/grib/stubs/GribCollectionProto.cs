// Minimal stub for the protobuf index messages used by GribCollectionImmutable when reading
// SparseArray/Record from an ncx index file. Only the members actually called are provided.

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	public class GribCollectionProto
	{
		public class SparseArray
		{
			internal SparseArray(byte[] data)
			{
			}

			public static SparseArray parseFrom(byte[] b)
			{
				return new SparseArray(b);
			}

			public virtual int getSizeCount()
			{
				return 0;
			}

			public virtual int getSize(int i)
			{
				return 0;
			}

			public virtual int getTrackCount()
			{
				return 0;
			}

			public virtual int getTrack(int i)
			{
				return 0;
			}

			public virtual int getRecordsCount()
			{
				return 0;
			}

			public virtual Record getRecords(int i)
			{
				return null;
			}

			public virtual int getNdups()
			{
				return 0;
			}
		}

		public class Record
		{
			public virtual int getFileno()
			{
				return 0;
			}

			public virtual long getStartPos()
			{
				return 0;
			}

			public virtual int getBmsOffset()
			{
				return 0;
			}

			public virtual int getDrsOffset()
			{
				return 0;
			}
		}
	}
}

namespace NetcdfSharp.com.google.protobuf
{
	public class InvalidProtocolBufferException : System.IO.IOException
	{
		public InvalidProtocolBufferException()
		{
		}

		public InvalidProtocolBufferException(string message) : base(message)
		{
		}
	}
}
