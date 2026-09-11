// Minimal stubs for out-of-scope thredds.inventory types referenced by the grib module.
// "Direction A": neutralize references to the inventory layer instead of porting it.
using System.Collections.Generic;

namespace NetcdfSharp.thredds.inventory
{
	using NetcdfSharp.ucar.nc2.util;

	/// <summary>Enum-like stub for NetcdfSharp.thredds.inventory.CollectionUpdateType (matches the Java constants).</summary>
	public sealed class CollectionUpdateType
	{
		public static readonly CollectionUpdateType always = new CollectionUpdateType("always");
		public static readonly CollectionUpdateType test = new CollectionUpdateType("test");
		public static readonly CollectionUpdateType testIndexOnly = new CollectionUpdateType("testIndexOnly");
		public static readonly CollectionUpdateType nocheck = new CollectionUpdateType("nocheck");
		public static readonly CollectionUpdateType never = new CollectionUpdateType("never");
		public static readonly CollectionUpdateType last = new CollectionUpdateType("last");

		private readonly string name;

		private CollectionUpdateType(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}
	}

	/// <summary>Minimal interface for NetcdfSharp.thredds.inventory.MCollection.</summary>
	public interface MCollection
	{
		string Root {get;}

		CloseableIterator<MFile> FileIterator {get;}

		object getAuxInfo(string key);

		string getIndexFilename(string suffix);
	}

	/// <summary>Minimal stub for NetcdfSharp.thredds.inventory.CollectionManager.</summary>
	public class CollectionManager
	{
		/// <summary>Checks whether a file (or its index) has changed since a given time.</summary>
		public class ChangeChecker
		{
			public virtual bool hasChangedSince(MFile file, long when)
			{
				return true;
			}

			public virtual bool hasntChangedSince(MFile file, long when)
			{
				return false;
			}
		}
	}
}
