// Minimal stubs for out-of-scope thredds.featurecollection types referenced by the grib module.

namespace NetcdfSharp.thredds.featurecollection
{
	using CalendarPeriod = NetcdfSharp.ucar.nc2.time.CalendarPeriod;
	using Element = NetcdfSharp.org.jdom2.Element;
	using Namespace = NetcdfSharp.org.jdom2.Namespace;

	/// <summary>
	/// Stub for NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig.
	/// Only the members actually referenced by the grib module are provided.
	/// </summary>
	public class FeatureCollectionConfig
	{
		// keys for storing AuxInfo objects
		public const string AUX_CONFIG = "fcConfig";

		public static bool useGenTypeDef, useTableVersionDef, intvMergeDef = true, useCenterDef;

		public GribConfig gribConfig = new GribConfig();

		public string type; // FeatureCollectionConfig.FeatureCollectionType; formatting only

		public enum PartitionType
		{
			none, directory, file, timePeriod, all
		}

		/// <summary>Stub for FeatureCollectionConfig.GribConfig.</summary>
		public class GribConfig
		{
			public bool useGenType = useGenTypeDef;
			public bool useTableVersion = useTableVersionDef;
			public bool intvMerge = intvMergeDef;
			public bool useCenter = useCenterDef;
			public bool unionRuntimeCoord;
			public GribIntvFilter intvFilter;
			public CalendarPeriod userTimeUnit;
			public string lookupTablePath, paramTablePath;
			public Element paramTable;
			public TimeUnitConverterHash tuc;

			public virtual TimeUnitConverter TimeUnitConverter
			{
				get
				{
					return tuc;
				}
			}

			public virtual int convertGdsHash(int hashcode)
			{
				return hashcode;
			}

			public virtual void configFromXml(Element configElem, Namespace ns)
			{
			}
		}

		/// <summary>Stub for FeatureCollectionConfig.GribIntvFilter.</summary>
		public class GribIntvFilter
		{
			public bool isZeroExcluded;

			public virtual bool ZeroExcluded
			{
				get
				{
					return isZeroExcluded;
				}
			}

			public virtual bool hasFilter()
			{
				return false;
			}

			// true means discard
			public virtual bool filter(int id, int intvStart, int intvEnd, int prob)
			{
				return false;
			}
		}

		public class TimeUnitConverterHash : TimeUnitConverter
		{
			public virtual int convertTimeUnit(int timeUnit)
			{
				return timeUnit;
			}
		}
	}

	/// <summary>Interface stub for NetcdfSharp.thredds.featurecollection.TimeUnitConverter.</summary>
	public interface TimeUnitConverter
	{
		int convertTimeUnit(int timeUnit);
	}
}
