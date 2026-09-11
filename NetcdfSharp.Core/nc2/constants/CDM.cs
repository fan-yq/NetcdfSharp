// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2025 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.constants
{
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;

	/// <summary>
	/// CDM constants.
	/// 
	/// @author caron
	/// @since 12/20/11
	/// </summary>
	public class CDM
	{
		public const string UTF8 = "UTF-8";

		// structural
		public const string CHUNK_SIZES = "_ChunkSizes";
		public const string COMPRESS = "_Compress";
		public const string COMPRESS_DEFLATE = "deflate";
		public const string FIELD_ATTS = "_field_atts"; // netcdf4 compound atts

		// from the Netcdf Users Guide
		// https://www.unidata.ucar.edu/software/netcdf/docs/netcdf.html#Attribute-Conventions
		public const string ABBREV = "abbreviation";
		public const string ADD_OFFSET = "add_offset";
		public const string CONVENTIONS = "Conventions";
		public const string CF_VERSION = "CF-1.9";
		public const string DESCRIPTION = "description";
		public const string FILL_VALUE = "_FillValue";
		public const string HISTORY = "history";
		public const string LONG_NAME = "long_name";
		public const string MISSING_VALUE = "missing_value";
		public const string SCALE_FACTOR = "scale_factor";
		public const string TITLE = "title";
		public const string UNITS = "units";
		public const string UDUNITS = "udunits";
		public const string UNSIGNED = "_Unsigned";
		public const string VALID_RANGE = "valid_range";
		public const string VALID_MIN = "valid_min";
		public const string VALID_MAX = "valid_max";

		// staggering for _Coordinate.Stagger
		public const string ARAKAWA_E = "Arakawa-E";
		public const string CurvilinearOrthogonal = "Curvilinear_Orthogonal";
		public const string StaggerType = "stagger_type";

		// misc
		public const string CF_EXTENDED = "CDM-Extended-CF";
		public const string FILE_FORMAT = "file_format";
		public const string GAUSSIAN = "gaussian_lats";
		public const string LAT_UNITS = "degrees_north";
		public const string LON_UNITS = "degrees_east";
		public const string RLATLON_UNITS = "degrees";
		public const string TIME_OFFSET = "time offset from runtime";
		public const string TIME_OFFSET_HOUR = "hoursFrom0z";
		public const string RUNTIME_COORDINATE = "runtimeCoordinate";

		// Special attributes

		private const string SPECIALPREFIX = "_";

		// Define Special attributes that should be suppressed
		public const string NCPROPERTIES = "_NCProperties";
		public const string ISNETCDF4 = "_IsNetcdf4";
		public const string SUPERBLOCKVERSION = "_SuperblockVersion";
		public const string ARRAYDIMENSIONS = "_ARRAY_DIMENSIONS";

		public static readonly string[] SPECIALS = new string[] { NCPROPERTIES, ISNETCDF4, SUPERBLOCKVERSION };

		public const string DAP4_LITTLE_ENDIAN = "_DAP4_Little_Endian";
		public const string EDU_UCAR_PREFIX = "_edu.ucar";

		public static bool isspecial(NcAttribute a)
		{
			string nm = a.ShortName;
			if (nm.StartsWith(SPECIALPREFIX, StringComparison.Ordinal))
			{
				/* Check for selected special attributes */
				foreach (string s in SPECIALS)
				{
					if (nm.StartsWith(s, StringComparison.Ordinal))
					{
						return true; // is special
					}
				}
			}
			return false; // is not special
		}

		// class not interface, per Bloch edition 2 item 19
		private CDM()
		{
		} // disable instantiation
	}

}
