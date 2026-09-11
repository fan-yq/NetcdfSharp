// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.util
{
	using NetcdfSharp.java.util;

	/// <summary>
	/// A HashMap that removes the oldest member when it exceeds the maximum number of entries.
	/// LOOK replace with something in guava?
	/// 
	/// @author caron </summary>
	/// <seealso cref="NetcdfSharp.java.util.LinkedHashMap"/>
	/// @deprecated do not use 
	[Obsolete("do not use")]
	public class HashMapLRU<K, V> : LinkedHashMap<K, V>
	{
	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(HashMapLRU<,>));

	  private int max_entries;

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="initialCapacity"> start with this size </param>
	  /// <param name="max_entries"> dont exceed this number of entries. </param>
	  public HashMapLRU(int initialCapacity, int max_entries) : base(initialCapacity, (float).50, true)
	  {
		this.max_entries = max_entries;
	  }

	  protected internal override bool removeEldestEntry(DictionaryEntry eldest)
	  {
		if (logger.isDebugEnabled() && size() > max_entries)
		{
		  logger.debug("HashMapLRU ejected entry, max_entries = {}", max_entries);
		}
		return size() > max_entries;
	  }

	}

}
