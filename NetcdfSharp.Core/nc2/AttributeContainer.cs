// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// An immutable Container of Attributes.
	/// Use AttributeContainerMutable if you want a mutable container.
	/// </summary>
	public interface AttributeContainer : IEnumerable<NcAttribute>
	{

		/// <summary>
		/// Create a new AttributeContainer, removing any whose name starts with one in the given list.
		/// </summary>
		/// <param name="atts"> Start with this set of Attributes. </param>
		/// <param name="remove"> Remove any whose name starts with one of these. </param>
		/// <returns> new AttributeContainer with attributes removed. </returns>
		static AttributeContainer filter(AttributeContainer atts, params string[] remove)
		{
			IList<NcAttribute> result = new List<NcAttribute>();
			foreach (NcAttribute att in atts.Attributes)
			{
				bool ok = true;
				foreach (string s in remove)
				{
					if (att.ShortName.StartsWith(s, StringComparison.Ordinal))
					{
						ok = false;
						break;
					}
				}
				if (ok)
				{
					result.Add(att);
				}
			}
			return new AttributeContainerHelper(atts.Name, result);
		}

		/// <summary>
		/// Find an Attribute by exact match on name. </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable Attribute findAttribute(String attName);
		NcAttribute? findAttribute(string attName);

		/// <summary>
		/// Determine if named attribute exists (exact match). </summary>
		bool hasAttribute(string attName)
		{
			return null != findAttribute(attName);
		}

		/// <summary>
		/// Find an Attribute by name, first doing an exact match, then ignoring case. </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable Attribute findAttributeIgnoreCase(String attName);
		NcAttribute? findAttributeIgnoreCase(string attName);

		/// <summary>
		/// Determine if named attribute exists, ignoring case. </summary>
		bool hasAttributeIgnoreCase(string attName)
		{
			return null != findAttributeIgnoreCase(attName);
		}

		/// <summary>
		/// Find a Numeric Attribute by name (ignore case), return the double value of the Attribute.
		/// </summary>
		/// <returns> the attribute value, or defaultValue if not found </returns>
		double findAttributeDouble(string attName, double defaultValue);

		/// <summary>
		/// Find a Numeric Attribute by name (ignore case), return the integer value of the Attribute.
		/// </summary>
		/// <returns> the attribute value, or defaultValue if not found </returns>
		int findAttributeInteger(string attName, int defaultValue);

		/// <summary>
		/// Find a String Attribute by name (ignore case), return the String value of the Attribute.
		/// </summary>
		/// <returns> the attribute value, or defaultValue if not found </returns>
		string findAttributeString(string attName, string defaultValue);

		/// <summary>
		/// True is there are no attributes in the container. </summary>
		bool Empty { get; }

		/// <summary>
		/// Get the (optional) name of the AttributeContainer. </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable String getName();
		string Name { get; }

		/// <summary>
		/// An unordered iterator over the contained attributes. </summary>
		IEnumerator<NcAttribute> iterator()
		{
			return Attributes.GetEnumerator();
		}

		/// @deprecated use findAttributeString(). 
		[Obsolete("use findAttributeString().")]
		string findAttValueIgnoreCase(string attName, string defaultValue)
		{
			return findAttributeString(attName, defaultValue);
		}

		///// will be removed in version 6 to make AttributeContainer immutable
		/// <summary>
		/// Returns immutable list of attributes.
		/// </summary>
		/// @deprecated use Iterable<Attribute> 
		[Obsolete("use Iterable<Attribute>")]
		IList<NcAttribute> Attributes { get; }

		/// <summary>
		/// Add all; replace old if has same name
		/// </summary>
		/// @deprecated will be removed in version 6. 
		[Obsolete("will be removed in version 6.")]
		void addAll(IEnumerable<NcAttribute> atts);

		/// <summary>
		/// Add new or replace old if has same name
		/// </summary>
		/// <param name="att"> add this Attribute </param>
		/// <returns> the added attribute </returns>
		/// @deprecated will be removed in version 6. 
		[Obsolete("will be removed in version 6.")]
		NcAttribute addAttribute(NcAttribute att);

		/// <summary>
		/// Remove an Attribute : uses the attribute hashCode to find it.
		/// </summary>
		/// <param name="a"> remove this attribute </param>
		/// <returns> true if was found and removed </returns>
		/// @deprecated will be removed in version 6. 
		[Obsolete("will be removed in version 6.")]
		bool remove(NcAttribute a);

		/// <summary>
		/// Remove an Attribute by name.
		/// </summary>
		/// <param name="attName"> if exists, remove this attribute </param>
		/// <returns> true if was found and removed </returns>
		/// @deprecated will be removed in version 6. 
		[Obsolete("will be removed in version 6.")]
		bool removeAttribute(string attName);

		/// <summary>
		/// Remove an Attribute by name, ignoring case
		/// </summary>
		/// <param name="attName"> if exists, remove this attribute </param>
		/// <returns> true if was found and removed </returns>
		/// @deprecated will be removed in version 6. 
		[Obsolete("will be removed in version 6.")]
		bool removeAttributeIgnoreCase(string attName);
	}

}
