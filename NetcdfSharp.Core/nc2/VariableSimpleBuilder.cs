// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2
{
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;

	/// <summary>
	/// Builder for VariableSimpleIF, makes true immutable objects. </summary>
	public class VariableSimpleBuilder
	{

		public static VariableSimpleBuilder fromMember(StructureMembers.Member m)
		{
			return new VariableSimpleBuilder(m.Name, m.Description, m.UnitsString, m.DataType, Dimensions.makeDimensionsAnon(m.Shape));
		}

		public static VariableSimpleBuilder makeScalar(string name, string desc, string units, DataType dt)
		{
			return new VariableSimpleBuilder(name, desc, units, dt, null);
		}

		public static VariableSimpleBuilder makeString(string name, string desc, string units, int str_len)
		{
			Dimension d = Dimension.builder(name + "_strlen", str_len).setIsShared(false).build();
			return new VariableSimpleBuilder(name, desc, units, DataType.CHAR, Collections.singletonList(d));
		}

		private readonly string name, desc, units;
		private readonly DataType dt;
		private readonly AttributeContainerMutable atts;
		private readonly ImmutableList<Dimension> dims;

		public VariableSimpleBuilder(string name, string desc, string units, DataType dt, IList<Dimension> dims)
		{
			this.name = name;
			this.desc = desc;
			this.units = units;
			this.dt = dt;
			this.dims = (dims == null || dims.Count == 0) ? ImmutableList<Dimension>.of() : ImmutableList<Dimension>.copyOf(dims);
			this.atts = new AttributeContainerMutable(name);

			if (!string.ReferenceEquals(units, null))
			{
				atts.addAttribute(new NcAttribute(CDM.UNITS, units));
			}
			if (!string.ReferenceEquals(desc, null))
			{
				atts.addAttribute(new NcAttribute(CDM.LONG_NAME, desc));
			}
		}

		public virtual VariableSimpleBuilder addAttribute(NcAttribute att)
		{
			atts.addAttribute(att);
			return this;
		}

		public virtual VariableSimpleBuilder addAttribute(string name, string value)
		{
			atts.addAttribute(name, value);
			return this;
		}

		public virtual VariableSimpleIF build()
		{
			return new VariableSimple(this);
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Immutable private static class VariableSimple implements VariableSimpleIF
		private class VariableSimple : VariableSimpleIF
		{
			internal readonly string name, desc, units;
			internal readonly DataType dt;
			internal readonly AttributeContainer atts;
			internal readonly ImmutableList<Dimension> dims;

			internal VariableSimple(VariableSimpleBuilder builder)
			{
				this.name = builder.name;
				this.desc = builder.desc;
				this.units = builder.units;
				this.dt = builder.dt;
				this.atts = builder.atts.toImmutable();
				this.dims = builder.dims;
			}

			public virtual string Name
			{
				get
				{
					return name;
				}
			}

			public virtual string FullName
			{
				get
				{
					return name;
				}
			}

			public virtual string ShortName
			{
				get
				{
					return name;
				}
			}

			public virtual string Description
			{
				get
				{
					return desc;
				}
			}

			public virtual string UnitsString
			{
				get
				{
					return units;
				}
			}

			public virtual int Rank
			{
				get
				{
					return this.dims.size();
				}
			}

			public virtual int[] Shape
			{
				get
				{
					return NetcdfSharp.ucar.nc2.Dimensions.makeShape(this.dims);
				}
			}

			public virtual IList<Dimension> Dimensions
			{
				get
				{
					return dims;
				}
			}

			public virtual DataType DataType
			{
				get
				{
					return dt;
				}
			}

			public virtual AttributeContainer attributes()
			{
				return new AttributeContainerMutable(name, atts);
			}

			public virtual IList<NcAttribute> Attributes
			{
				get
				{
					return atts.Attributes;
				}
			}

			public virtual NcAttribute findAttributeIgnoreCase(string name)
			{
				return atts.findAttributeIgnoreCase(name);
			}

			public virtual int CompareTo(VariableSimpleIF o)
			{
				return string.CompareOrdinal(name, o.ShortName); // ??
			}
		}
	}

}
