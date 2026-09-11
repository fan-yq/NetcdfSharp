// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 2021-2025 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.filter
{

	public class Filters
	{

	  private static readonly ImmutableList<FilterProvider> spFilters;

	  // load filter service providers
	  static Filters()
	  {
		ImmutableList<FilterProvider>.Builder spFiltersBuilder = ImmutableList<FilterProvider>.builder();
		foreach (FilterProvider fp in ServiceLoader.load(typeof(FilterProvider)))
		{
		  spFiltersBuilder.add(fp);
		}
		spFilters = spFiltersBuilder.build();
	  }

	  /// <summary>
	  /// Set of common properties used by Filters
	  /// </summary>
	  public class Keys
	  {
		  private readonly Filters outerInstance;

		  public Keys(Filters outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <summary>
		/// key for a String id/name (e.g. Zarr filters)
		/// </summary>
		public const string NAME = "id";
		/// <summary>
		/// key for a numeric id (e.g. HDF5 filters)
		/// </summary>
		public const string ID = "nid";
		/// <summary>
		/// key for property indicating whether filter is optional,
		/// i.e. whether it should continue or throw if it fails
		/// </summary>
		public const string OPTIONAL = "optional";
		/// <summary>
		/// key mapping to client data used by filter
		/// </summary>
		public const string DATA = "data";
		/// <summary>
		/// key mapping to element size
		/// </summary>
		public const string ELEM_SIZE = "elementsize";
	  }

	  private static NullFilter nullFilter = new NullFilter();

	  /// <summary>
	  /// Create a filter object matching the provided id
	  /// </summary>
	  /// <param name="properties"> </param>
	  /// <returns> A filter object with the given properties </returns>
	  /// <exception cref="UnknownFilterException"> </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static Filter getFilter(NetcdfSharp.java.util.Map<String, Object> properties) throws UnknownFilterException
	  public static Filter getFilter(IDictionary<string, object> properties)
	  {
		if (properties == null)
		{
		  return nullFilter;
		}
		// read id properties
		string name = properties.TryGetValue(Keys.NAME, out object nameObj) ? (string) nameObj : null;
		object oid = properties.TryGetValue(Keys.ID, out object oidVal) ? oidVal : null;
		// if no id or name, return null filter
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final boolean missingName = name == null || name.isEmpty();
		bool missingName = string.ReferenceEquals(name, null) || name.Length == 0;
		if (missingName && !(oid is int or long or short or byte or float or double or uint or ulong or ushort))
		{
		  return nullFilter;
		}

		// try by name first
		if (!missingName)
		{
		  foreach (FilterProvider fp in spFilters)
		  {
			if (fp.canProvide(name))
			{
			  return fp.create(properties);
			}
		  }
		}

		// try by id next
		int id = ((short?) oid).Value;
		foreach (FilterProvider fp in spFilters)
		{
		  if (fp.canProvide(id))
		  {
			return fp.create(properties);
		  }
		}
		// final fallback
		throw new UnknownFilterException(id);
	  }

	  /// <summary>
	  /// A filter which passes data through unchanged
	  /// </summary>
	  private class NullFilter : Filter
	  {

		internal const string name = "null";

		internal const int id = 0;

		public override string Name
		{
			get
			{
			  return name;
			}
		}

		public override int Id
		{
			get
			{
			  return id;
			}
		}

		public override byte[] encode(byte[] dataIn)
		{
		  return dataIn;
		}

		public override byte[] decode(byte[] dataIn)
		{
		  return dataIn;
		}
	  }
	}

}
