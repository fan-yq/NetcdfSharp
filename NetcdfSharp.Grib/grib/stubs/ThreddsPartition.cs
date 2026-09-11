// Minimal stubs for out-of-scope NetcdfSharp.thredds.inventory.partition types referenced by the grib module.
using System.Collections.Generic;

namespace NetcdfSharp.thredds.inventory.partition
{
	using MCollection = NetcdfSharp.thredds.inventory.MCollection;
	using MFile = NetcdfSharp.thredds.inventory.MFile;

	/// <summary>Minimal stub for NetcdfSharp.thredds.inventory.partition.PartitionManager.</summary>
	public class PartitionManager
	{
		public virtual void putAuxInfo(object key, object value)
		{
		}
	}

	/// <summary>Minimal stub for NetcdfSharp.thredds.inventory.partition.PartitionManagerFromIndexList.</summary>
	public class PartitionManagerFromIndexList : PartitionManager
	{
		internal PartitionManagerFromIndexList(MCollection dcm, IList<MFile> partitions, NetcdfSharp.org.slf4j.Logger logger)
		{
		}
	}
}
