// Minimal stubs for the partition collection types (NetcdfSharp.ucar.nc2.grib.collection.PartitionCollectionMutable /
// PartitionCollectionImmutable). Partition handling is out of scope ("Direction A"): only the
// members referenced by CoordinatePartitionUnionizer and GribDataReader are provided.
// PartitionCollectionImmutable intentionally does not subclass GribCollectionImmutable (the abstract
// members depend on the unported dataset enhancement layer).


namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using Coordinate = NetcdfSharp.ucar.nc2.grib.coord.Coordinate;
	using GdsHorizCoordSys = NetcdfSharp.ucar.nc2.grib.GdsHorizCoordSys;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using SubsetParams = NetcdfSharp.ucar.nc2.ft2.coverage.SubsetParams;

	public class PartitionCollectionMutable
	{
		public class Partition
		{
			public virtual string Name
			{
				get
				{
					return "";
				}
			}
		}
	}

	public class PartitionCollectionImmutable
	{
		public class Partition
		{
			public virtual RandomAccessFile getRaf(int partno, int fileno)
			{
				return null;
			}
		}

		public class VariableIndexPartitioned : GribCollectionImmutable.VariableIndex
		{
			internal VariableIndexPartitioned() : base(null, null, null)
			{
			}

			public virtual GribCollectionImmutable.GribType Type
			{
				get
				{
					return GribCollectionImmutable.GribType.SRC;
				}
			}

			public virtual Coordinate CoordinateTime
			{
				get
				{
					return null;
				}
			}

			public virtual DataRecord getDataRecord(int[] index)
			{
				return null;
			}

			public virtual DataRecord getDataRecord(SubsetParams coords)
			{
				return null;
			}
		}

		public class DataRecord : GribDataReader.DataRecord
		{
			public readonly int partno;
			public readonly Partition usePartition;

			internal DataRecord(int resultIndex, GribCollectionImmutable.Record record, GdsHorizCoordSys hcs, int partno, Partition usePartition)
				: base(resultIndex, record, hcs)
			{
				this.partno = partno;
				this.usePartition = usePartition;
			}

			public virtual bool usesSameFile(DataRecord other)
			{
				return false;
			}

			public virtual void show()
			{
			}
		}
	}
}
