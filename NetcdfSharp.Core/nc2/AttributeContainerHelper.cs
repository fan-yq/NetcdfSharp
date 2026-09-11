// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;

/* Copyright */
namespace NetcdfSharp.ucar.nc2
{
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// A mutable collection of Attributes.
	/// 
	/// @author caron
	/// @since 5/5/2015 </summary>
	/// @deprecated Use AttributeContainerMutable. 
	[Obsolete("Use AttributeContainerMutable.")]
	public class AttributeContainerHelper : AttributeContainer
	{
		[Obsolete("use AttributeContainer.filter().")]
		public static AttributeContainer filter(AttributeContainer atts, params string[] remove)
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

		[Obsolete]
		public static void show(AttributeContainer atts, Indent indent, Formatter f)
		{
			foreach (NcAttribute att in atts.Attributes)
			{
				f.format("%s%s%n", indent, att);
			}
		}

		private readonly string name;
		private IList<NcAttribute> atts;

		public AttributeContainerHelper(string name)
		{
			this.name = name;
			this.atts = new List<NcAttribute>();
		}

		public AttributeContainerHelper(string name, IList<NcAttribute> from) : this(name)
		{
			addAll(from);
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		[Obsolete]
		public virtual void setImmutable()
		{
			this.atts = atts.AsReadOnly();
		}

		public virtual bool Empty
		{
			get
			{
				return atts.Count == 0;
			}
		}

		public virtual IList<NcAttribute> Attributes
		{
			get
			{
				return atts;
			}
		}

		public virtual IEnumerator<NcAttribute> GetEnumerator()
		{
			return atts.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return atts.GetEnumerator();
		}

		public virtual NcAttribute addAttribute(NcAttribute att)
		{
			if (att == null)
			{
				return null;
			}
			for (int i = 0; i < atts.Count; i++)
			{
				NcAttribute a = atts[i];
				if (att.ShortName.Equals(a.ShortName))
				{
					atts[i] = att; // replace
					return att;
				}
			}
			atts.Add(att);
			return att;
		}

		/// <summary>
		/// Add Attribute; name and value must not be null. </summary>
		public virtual NcAttribute addAttribute(string name, string value)
		{
			if (string.ReferenceEquals(name, null) || string.ReferenceEquals(value, null))
			{
				return null;
			}
			NcAttribute att = new NcAttribute(name, value);
			return addAttribute(att);
		}

		/// <summary>
		/// Add Attribute; name and value must not be null. </summary>
		public virtual NcAttribute addAttribute(string name, object value)
		{
			if (string.ReferenceEquals(name, null) || value == null)
			{
				return null;
			}
			NcAttribute att = new NcAttribute(name, value);
			return addAttribute(att);
		}

		/// <summary>
		/// Add all; replace old if has same name. </summary>
		public virtual void addAll(IEnumerable<NcAttribute> atts)
		{
			foreach (NcAttribute att in atts)
			{
				addAttribute(att);
			}
		}

		public virtual string findAttributeString(string attName, string defaultValue)
		{
			string attValue = null;
			NcAttribute att = findAttributeIgnoreCase(attName);

			if ((att != null) && att.String)
			{
				attValue = att.StringValue;
			}

			if (null == attValue) // not found, use default
			{
				attValue = defaultValue;
			}

			return attValue;
		}

		public virtual NcAttribute? findAttribute(string name)
		{
			foreach (NcAttribute a in atts)
			{
				if (name.Equals(a.ShortName))
				{
					return a;
				}
			}
			return null;
		}

		public virtual NcAttribute? findAttributeIgnoreCase(string name)
		{
			foreach (NcAttribute a in atts)
			{
				if (name.Equals(a.ShortName, StringComparison.OrdinalIgnoreCase))
				{
					return a;
				}
			}
			return null;
		}


		public virtual double findAttributeDouble(string attName, double defaultValue)
		{
			NcAttribute att = findAttributeIgnoreCase(attName);
			if (att == null)
			{
				return defaultValue;
			}
			if (att.String)
			{
				return double.Parse(att.StringValue);
			}
			else
			{
				return Convert.ToDouble(att.NumericValue);
			}
		}

		public virtual int findAttributeInteger(string attName, int defaultValue)
		{
			NcAttribute att = findAttributeIgnoreCase(attName);
			if (att == null)
			{
				return defaultValue;
			}
			if (att.String)
			{
				return int.Parse(att.StringValue);
			}
			else
			{
				return Convert.ToInt32(att.NumericValue);
			}
		}

		/// <summary>
		/// Remove an Attribute : uses the attribute hashCode to find it.
		/// </summary>
		/// <param name="a"> remove this attribute </param>
		/// <returns> true if was found and removed </returns>
		public virtual bool remove(NcAttribute a)
		{
			return a != null && atts.Remove(a);
		}

		/// <summary>
		/// Replace an Attribute with a different name, same value.
		/// </summary>
		/// <param name="a"> remove this attribute </param>
		public virtual void replace(NcAttribute a, string newName)
		{
			atts.Remove(a);
			NcAttribute newAtt = a.toBuilder().setName(newName).build();
			addAttribute(newAtt);
		}

		/// <summary>
		/// Remove an Attribute by name.
		/// </summary>
		/// <param name="attName"> if exists, remove this attribute </param>
		/// <returns> true if was found and removed </returns>
		public virtual bool removeAttribute(string attName)
		{
			NcAttribute att = findAttribute(attName);
			return att != null && atts.Remove(att);
		}

		/// <summary>
		/// Remove an Attribute by name, ignoring case
		/// </summary>
		/// <param name="attName"> if exists, remove this attribute </param>
		/// <returns> true if was found and removed </returns>
		public virtual bool removeAttributeIgnoreCase(string attName)
		{
			NcAttribute att = findAttributeIgnoreCase(attName);
			return att != null && atts.Remove(att);
		}

		public virtual AttributeContainer toImmutable()
		{
			return new AttributeContainerImmutable(name, atts);
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Immutable private static class AttributeContainerImmutable implements AttributeContainer
		private class AttributeContainerImmutable : AttributeContainer
		{
			internal readonly string name;
			internal readonly ImmutableList<NcAttribute> atts;

			internal AttributeContainerImmutable(string name, IList<NcAttribute> atts)
			{
				this.name = name;
				this.atts = ImmutableList<NcAttribute>.copyOf(atts);
			}

			public virtual IList<NcAttribute> Attributes
			{
				get
				{
					return atts;
				}
			}

			public virtual IEnumerator<NcAttribute> GetEnumerator()
			{
				return atts.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return atts.GetEnumerator();
			}

			public virtual string findAttributeString(string attName, string defaultValue)
			{
				var match = atts.FirstOrDefault(a => a.getShortName().Equals(attName));
				return match != null && match.getStringValue() != null ? match.getStringValue() : defaultValue;
			}

			public virtual NcAttribute? findAttribute(string attName)
			{
				return atts.FirstOrDefault(a => a.getShortName().Equals(attName));
			}

			public virtual NcAttribute? findAttributeIgnoreCase(string attName)
			{
				return atts.FirstOrDefault(a => a.getShortName().Equals(attName, StringComparison.OrdinalIgnoreCase));
			}

			public virtual double findAttributeDouble(string attName, double defaultValue)
			{
				NcAttribute att = findAttributeIgnoreCase(attName);
				if (att == null)
				{
					return defaultValue;
				}
				if (att.String)
				{
					return double.Parse(att.StringValue);
				}
				else
				{
					return Convert.ToDouble(att.NumericValue);
				}
			}

			public virtual int findAttributeInteger(string attName, int defaultValue)
			{
				NcAttribute att = findAttributeIgnoreCase(attName);
				if (att == null)
				{
					return defaultValue;
				}
				if (att.String)
				{
					return int.Parse(att.StringValue);
				}
				else
				{
					return Convert.ToInt32(att.NumericValue);
				}
			}

			public virtual string Name
			{
				get
				{
					return name;
				}
			}

			public virtual bool Empty
			{
				get
				{
					return atts.isEmpty();
				}
			}

			[Obsolete]
			public virtual void addAll(IEnumerable<NcAttribute> atts)
			{
				throw new System.NotSupportedException();
			}

			[Obsolete]
			public virtual NcAttribute addAttribute(NcAttribute att)
			{
				throw new System.NotSupportedException();
			}

			[Obsolete]
			public virtual bool remove(NcAttribute a)
			{
				throw new System.NotSupportedException();
			}

			[Obsolete]
			public virtual bool removeAttribute(string attName)
			{
				throw new System.NotSupportedException();
			}

			[Obsolete]
			public virtual bool removeAttributeIgnoreCase(string attName)
			{
				throw new System.NotSupportedException();
			}
		}
	}

}
