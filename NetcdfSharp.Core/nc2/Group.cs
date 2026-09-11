// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2

{
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using EscapeStrings = NetcdfSharp.ucar.nc2.util.EscapeStrings;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// A logical collection of Variables, Attributes, and Dimensions.
	/// The Groups in a Dataset form a hierarchical tree, like directories on a disk.
	/// A Group has a name and optionally a set of Attributes.
	/// There is always at least one Group in a dataset, the root Group, whose name is the empty string.
	/// Immutable if setImmutable() was called.
	/// 
	/// TODO Group will be immutable in 6.
	/// TODO Group will not implement AttributeContainer in 6, use Group.attributes().
	/// TODO Group will not extend CDMNode in 6.
	/// 
	/// @author caron
	/// </summary>
	public class Group : CDMNode, AttributeContainer
	{

		[Obsolete]
		internal static IList<Group> collectPath(Group g)
		{
			IList<Group> list = new List<Group>();
			while (g != null)
			{
				list.Insert(0, g);
				g = g.ParentGroup;
			}
			return list;
		}

		/// <summary>
		/// Is this the root group?
		/// </summary>
		/// <returns> true if root group </returns>
		public virtual bool Root
		{
			get
			{
				return ParentGroup == null;
			}
		}

		/// <summary>
		/// Get the Variables contained directly in this group.
		/// </summary>
		/// <returns> List of type Variable; may be empty, not null.
		///         TODO return ImmutableList </returns>
		public virtual IList<Variable> Variables
		{
			get
			{
				return variables;
			}
		}

		/// @deprecated use findVariableLocal() 
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Deprecated("use findVariableLocal()") @Nullable public Variable findVariable(String varShortName)
		[Obsolete("use findVariableLocal()")]
		public virtual Variable? findVariable(string varShortName)
		{
			return findVariableLocal(varShortName);
		}

		/// <summary>
		/// Find the Variable with the specified (short) name in this group.
		/// </summary>
		/// <param name="varShortName"> short name of Variable within this group. </param>
		/// <returns> the Variable, or null if not found </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Variable findVariableLocal(String varShortName)
		public virtual Variable? findVariableLocal(string varShortName)
		{
			if (string.ReferenceEquals(varShortName, null))
			{
				return null;
			}

			foreach (Variable v in variables)
			{
				if (varShortName.Equals(v.ShortName))
				{
					return v;
				}
			}
			return null;
		}

		/// <summary>
		/// Find the Variable with the specified (short) name in this group or a parent group.
		/// </summary>
		/// <param name="varShortName"> short name of Variable. </param>
		/// <returns> the Variable, or null if not found </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Variable findVariableOrInParent(String varShortName)
		public virtual Variable? findVariableOrInParent(string varShortName)
		{
			if (string.ReferenceEquals(varShortName, null))
			{
				return null;
			}

			Variable v = findVariableLocal(varShortName);
			Group parent = ParentGroup;
			if ((v == null) && (parent != null))
			{
				v = parent.findVariableOrInParent(varShortName);
			}
			return v;
		}

		/// <summary>
		/// Look in this Group and in its nested Groups for a Variable with a String valued Attribute with the given name
		/// and value.
		/// </summary>
		/// <param name="attName"> look for an Attribuite with this name. </param>
		/// <param name="attValue"> look for an Attribuite with this value. </param>
		/// <returns> the first Variable that matches, or null if none match. </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Variable findVariableByAttribute(String attName, String attValue)
		public virtual Variable? findVariableByAttribute(string attName, string attValue)
		{
			foreach (Variable v in Variables)
			{
				foreach (NcAttribute att in v.attributes())
				{
					if (attName.Equals(att.ShortName) && attValue.Equals(att.StringValue))
					{
						return v;
					}
				}
			}
			foreach (Group nested in Groups)
			{
				Variable v = nested.findVariableByAttribute(attName, attValue);
				if (v != null)
				{
					return v;
				}
			}
			return null;
		}

		/// <summary>
		/// Get the parent Group, or null if its the root group.
		/// Not deprecated.
		/// </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @SuppressWarnings("deprecated") @Nullable public Group getParentGroup()
		public override Group ParentGroup
		{
			get
			{
				return this.group;
			}
			set
			{
				if (immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}
				base.ParentGroup = value == null ? ncfile.RootGroup : value;
			}
		}

		/// <summary>
		/// Get the full name of this object.
		/// Certain characters are backslash escaped (see NetcdfFiles.getFullName(Group))
		/// Not deprecated.
		/// </summary>
		/// <returns> full name with backslash escapes </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @SuppressWarnings("deprecated") public String getFullName()
		public override string FullName
		{
			get
			{
				return NetcdfFiles.makeFullName(this);
			}
		}

		/// <summary>
		/// Get the Groups contained directly in this Group.
		/// </summary>
		/// <returns> List of type Group; may be empty, not null. </returns>
		public virtual ImmutableList<Group> Groups
		{
			get
			{
				return ImmutableList<Group>.copyOf(groups);
			}
		}

		/// <summary>
		/// Get the owning NetcdfFile </summary>
		public virtual NetcdfFile NetcdfFile
		{
			get
			{
				return ncfile;
			}
		}

		/// <summary>
		/// Retrieve the Group with the specified (short) name.
		/// </summary>
		/// <param name="groupShortName"> short name of the nested group you are looking for. </param>
		/// <returns> the Group, or null if not found </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Group findGroupLocal(String groupShortName)
		public virtual Group? findGroupLocal(string groupShortName)
		{
			if (string.ReferenceEquals(groupShortName, null))
			{
				return null;
			}
			// groupShortName = NetcdfFile.makeNameUnescaped(groupShortName);

			foreach (Group group in groups)
			{
				if (groupShortName.Equals(group.ShortName))
				{
					return group;
				}
			}

			return null;
		}

		/// <summary>
		/// Get the shared Dimensions contained directly in this group.
		/// </summary>
		/// <returns> List of type Dimension; may be empty, not null.
		///         TODO return ImmutableList </returns>
		public virtual IList<Dimension> Dimensions
		{
			get
			{
				return dimensions;
			}
		}

		/// <summary>
		/// Create a dimension list using dimension names. The dimension is searched for recursively in the parent groups.
		/// </summary>
		/// <param name="dimString"> : whitespace separated list of dimension names, or '*' for Dimension.UNKNOWN, or number for anon
		///        dimension. null or empty String is a scalar. </param>
		/// <returns> list of dimensions, will return ImmutableList<> in version 6 </returns>
		/// <exception cref="IllegalArgumentException"> if cant find dimension or parse error. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public NetcdfSharp.com.google.common.collect.ImmutableList<Dimension> makeDimensionsList(String dimString) throws IllegalArgumentException
		public virtual ImmutableList<Dimension> makeDimensionsList(string dimString)
		{
			return NetcdfSharp.ucar.nc2.Dimensions.makeDimensionsList(name => this.findDimension(name), dimString);
		}

		/// <summary>
		/// Get the enumerations contained directly in this group.
		/// </summary>
		/// <returns> List of type EnumTypedef; may be empty, not null. </returns>
		public virtual ImmutableList<EnumTypedef> EnumTypedefs
		{
			get
			{
				return ImmutableList<EnumTypedef>.copyOf(enumTypedefs);
			}
		}

		/// <summary>
		/// Find a Dimension in this or a parent Group, matching on short name.
		/// </summary>
		/// <param name="name"> Dimension name. </param>
		/// <returns> the Dimension, or null if not found </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Dimension findDimension(String name)
		public virtual Dimension? findDimension(string name)
		{
			if (string.ReferenceEquals(name, null))
			{
				return null;
			}
			// name = NetcdfFile.makeNameUnescaped(name);
			Dimension d = findDimensionLocal(name);
			if (d != null)
			{
				return d;
			}
			Group parent = ParentGroup;
			if (parent != null)
			{
				return parent.findDimension(name);
			}

			return null;
		}

		/// <summary>
		/// Find a Dimension in this or a parent Group, using equals.
		/// </summary>
		/// <param name="dim"> Dimension . </param>
		/// <returns> the Dimension, or null if not found </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Dimension findDimension(Dimension dim)
		public virtual Dimension? findDimension(Dimension dim)
		{
			if (dim == null)
			{
				return null;
			}
			foreach (Dimension d in dimensions)
			{
				if (d.Equals(dim))
				{
					return d;
				}
			}
			Group parent = ParentGroup;
			if (parent != null)
			{
				return parent.findDimension(dim);
			}
			return null;
		}

		/// <summary>
		/// Find a Dimension using its (short) name, in this group only
		/// </summary>
		/// <param name="shortName"> Dimension name. </param>
		/// <returns> the Dimension, or null if not found </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Dimension findDimensionLocal(String shortName)
		public virtual Dimension? findDimensionLocal(string shortName)
		{
			if (string.ReferenceEquals(shortName, null))
			{
				return null;
			}
			foreach (Dimension d in dimensions)
			{
				if (shortName.Equals(d.ShortName))
				{
					return d;
				}
			}

			return null;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////
		// Attributes

		/// <summary>
		/// The attributes contained by this Group. </summary>
		public virtual AttributeContainer attributes()
		{
			return attributes_Conflict;
		}

		public virtual AttributeContainer AttributeContainer => attributes_Conflict;

		public virtual IEnumerator<NcAttribute> GetEnumerator()
		{
			return attributes_Conflict.GetEnumerator();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return attributes_Conflict.GetEnumerator();
		}

		/// <summary>
		/// Find the attribute by name, return null if not exist </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Attribute findAttribute(String name)
		public virtual NcAttribute? findAttribute(string name)
		{
			return attributes_Conflict.findAttribute(name);
		}

		/// <summary>
		/// Find a String-valued Attribute by name (ignore case), return the String value of the Attribute.
		/// </summary>
		/// <returns> the attribute value, or defaultValue if not found </returns>
		public virtual string findAttributeString(string attName, string defaultValue)
		{
			return attributes_Conflict.findAttributeString(attName, defaultValue);
		}

		/// @deprecated Use attributes() 
		[Obsolete("Use attributes()")]
		public virtual IList<NcAttribute> Attributes
		{
			get
			{
				return AttributeContainer.filter(attributes_Conflict, CDM.SPECIALS).Attributes;
			}
		}

		/// @deprecated Use attributes() 
		public virtual bool Empty
		{
			get
			{
				return attributes_Conflict.Empty;
			}
		}

		/// @deprecated Use findAttributeString() 
		[Obsolete("Use findAttributeString()")]
		public virtual NcAttribute? findAttributeIgnoreCase(string name)
		{
			return attributes_Conflict.findAttributeIgnoreCase(name);
		}

		/// @deprecated Use attributes().findAttributeDouble() 
		[Obsolete("Use attributes().findAttributeDouble()")]
		public virtual double findAttributeDouble(string attName, double defaultValue)
		{
			return attributes_Conflict.findAttributeDouble(attName, defaultValue);
		}

		/// @deprecated Use attributes().findAttributeInteger() 
		[Obsolete("Use attributes().findAttributeInteger()")]
		public virtual int findAttributeInteger(string attName, int defaultValue)
		{
			return attributes_Conflict.findAttributeInteger(attName, defaultValue);
		}

		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual NcAttribute addAttribute(NcAttribute att)
		{
			return attributes_Conflict.addAttribute(att);
		}

		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual void addAll(IEnumerable<NcAttribute> atts)
		{
			attributes_Conflict.addAll(atts);
		}

		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual bool remove(NcAttribute a)
		{
			return attributes_Conflict.remove(a);
		}

		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual bool removeAttribute(string attName)
		{
			return attributes_Conflict.removeAttribute(attName);
		}

		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual bool removeAttributeIgnoreCase(string attName)
		{
			return attributes_Conflict.removeAttributeIgnoreCase(attName);
		}

		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Find a Enumeration in this Group, using its short name. </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public EnumTypedef findEnumeration(String name)
		public virtual EnumTypedef? findEnumeration(string name)
		{
			return findEnumeration(name, false); // Keep the old behavior
		}

		/// <summary>
		/// Find a Enumeration in this or optionally the parent Groups, using its short name. </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public EnumTypedef findEnumeration(String name, boolean searchup)
		public virtual EnumTypedef? findEnumeration(string name, bool searchup)
		{
			if (string.ReferenceEquals(name, null))
			{
				return null;
			}
			// name = NetcdfFile.makeNameUnescaped(name);
			// search this group's EnumTypedefs
			foreach (EnumTypedef d in this.EnumTypedefs)
			{
				if (name.Equals(d.ShortName))
				{
					return d;
				}
			}
			if (searchup)
			{
				Group parent = ParentGroup;
				if (parent != null)
				{
					return parent.findEnumeration(name, searchup);
				}
			}
			return null;
		}

		/// <summary>
		/// Locate an enum type definition that is structurally
		/// similar to the template type def. The Enum names are ignored.
		/// </summary>
		/// <param name="template"> match this enum type def </param>
		/// <param name="searchup"> if true, then search this group and then parent groups. </param>
		public virtual EnumTypedef? findSimilarEnumTypedef(EnumTypedef template, bool searchup)
		{
			Debug.Assert((template != null));
			// search this group builders's EnumTypedefs but with constraint on name
			EnumTypedef ed = enumTypedefs.FirstOrDefault(e => e.equalsMapOnly(template));
			if (ed != null)
			{
				return ed;
			}
			// Optionally search parents
			if (searchup)
			{
				Group gb = ParentGroup;
				if (gb != null)
				{
					ed = gb.findSimilarEnumTypedef(template, searchup);
				}
				return ed;
			}
			return null;
		}

		/// <summary>
		/// Get the common parent of this and the other group.
		/// Cant fail, since the root group is always a parent of any 2 groups.
		/// </summary>
		/// <param name="other"> the other group </param>
		/// <returns> common parent of this and the other group </returns>
		public virtual Group commonParent(Group other)
		{
			if (isParent(other))
			{
				return this;
			}
			if (other.isParent(this))
			{
				return other;
			}
			while (!other.isParent(this))
			{
				other = other.ParentGroup;
			}
			return other;
		}

		/// <summary>
		/// Is this a parent of the other Group?
		/// </summary>
		/// <param name="other"> another Group </param>
		/// <returns> true is it is equal or a parent </returns>
		public virtual bool isParent(Group other)
		{
			while ((other != this) && (other.ParentGroup != null))
			{
				other = other.ParentGroup;
			}
			return (other == this);
		}

		//////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Get String with name and attributes. Used in short descriptions like tooltips.
		/// </summary>
		/// <returns> name and attributes String. </returns>
		public virtual string NameAndAttributes
		{
			get
			{
				StringBuilder sbuff = new StringBuilder();
				sbuff.Append("Group ");
				sbuff.Append(ShortName);
				sbuff.Append("\n");
				foreach (NcAttribute att in attributes_Conflict)
				{
					sbuff.Append("  ").Append(ShortName).Append(":");
					sbuff.Append(att);
					sbuff.Append(";");
					sbuff.Append("\n");
				}
				return sbuff.ToString();
			}
		}

		/// <summary>
		/// CDL representation.
		/// </summary>
		/// <param name="strict"> if true, write in strict adherence to CDL definition. </param>
		/// <returns> CDL representation. </returns>
		/// @deprecated use CDLWriter 
		[Obsolete("use CDLWriter")]
		public virtual string writeCDL(bool strict)
		{
			Formatter buf = new Formatter();
			writeCDL(buf, new Indent(2), strict);
			return buf.ToString();
		}

		/// @deprecated use CDLWriter 
		[Obsolete("use CDLWriter")]
		internal virtual void writeCDL(Formatter @out, Indent indent, bool strict)
		{
			bool hasE = (enumTypedefs.Count > 0);
			bool hasD = (dimensions.Count > 0);
			bool hasV = (variables.Count > 0);
			// boolean hasG = (groups.size() > 0);
			bool hasA = (!Iterables.isEmpty(attributes_Conflict));

			if (hasE)
			{
				@out.format("%stypes:%n", indent);
				indent.incr();
				foreach (EnumTypedef e in enumTypedefs)
				{
					e.writeCDL(@out, indent, strict);
					@out.format("%n");
				}
				indent.decr();
				@out.format("%n");
			}

			if (hasD)
			{
				@out.format("%sdimensions:%n", indent);
				indent.incr();
				foreach (Dimension myd in dimensions)
				{
					myd.writeCDL(@out, indent, strict);
					@out.format("%n");
				}
				indent.decr();
			}

			if (hasV)
			{
				@out.format("%svariables:%n", indent);
				indent.incr();
				foreach (Variable v in variables)
				{
					v.writeCDL(@out, indent, false, strict);
					@out.format("%n");
				}
				indent.decr();
			}

			foreach (Group g in groups)
			{
				string gname = strict ? NetcdfFiles.makeValidCDLName(g.ShortName) : g.ShortName;
				@out.format("%sgroup: %s {%n", indent, gname);
				indent.incr();
				g.writeCDL(@out, indent, strict);
				indent.decr();
				@out.format("%s}%n%n", indent);
			}

			// if (hasA && (hasE || hasD || hasV || hasG))
			// out.format("%n");

			if (hasA)
			{
				if (Root)
				{
					@out.format("%s// global attributes:%n", indent);
				}
				else
				{
					@out.format("%s// group attributes:%n", indent);
				}

				foreach (NcAttribute att in attributes_Conflict)
				{
					// String name = strict ? NetcdfFile.escapeNameCDL(getShortName()) : getShortName();
					if (!CDM.isspecial(att))
					{
						@out.format("%s", indent);
						att.writeCDL(@out, strict, null);
						@out.format(";");
						if (!strict && (att.DataType != DataType.STRING))
						{
							@out.format(" // %s", att.DataType);
						}
						@out.format("%n");
					}
				}
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ncfile"> NetcdfFile owns this Group </param>
		/// <param name="parent"> parent of Group. If null, this is the root Group. </param>
		/// <param name="shortName"> short name of Group. </param>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public Group(NetcdfFile ncfile, Group parent, string shortName) : base(shortName)
		{
			this.ncfile = ncfile;
			this.attributes_Conflict = new AttributeContainerMutable(shortName);
			ParentGroup = parent == null ? ncfile.RootGroup : parent;
		}

		/// <summary>
		/// Set the short name, converting to valid CDM object name if needed.
		/// </summary>
		/// <param name="shortName"> set to this value </param>
		/// <returns> valid CDM object name </returns>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual string setName(string shortName)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			ShortName = shortName;
			return ShortName;
		}

		/// <summary>
		/// Adds the specified shared dimension to this group.
		/// </summary>
		/// <param name="dim"> the dimension to add. </param>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual void addDimension(Dimension dim)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}

			if (!dim.Shared)
			{
				throw new ArgumentException("Dimensions added to a group must be shared.");
			}

			if (findDimensionLocal(dim.ShortName) != null)
			{
				throw new ArgumentException("Dimension name (" + dim.ShortName + ") must be unique within Group " + ShortName);
			}

			dimensions.Add(dim);
			dim.setParentGroup(this) ;
		}

		/// <summary>
		/// Adds the specified shared dimension to this group, but only if another dimension with the same name doesn't
		/// already exist.
		/// </summary>
		/// <param name="dim"> the dimension to add. </param>
		/// <returns> {@code true} if {@code dim} was successfully added to the group. Otherwise, {@code false} will be returned,
		///         meaning that a dimension with {@code dim}'s name already exists within the group. </returns>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual bool addDimensionIfNotExists(Dimension dim)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}

			if (!dim.Shared)
			{
				throw new ArgumentException("Dimensions added to a group must be shared.");
			}

			if (findDimensionLocal(dim.ShortName) != null)
			{
				return false;
			}

			dimensions.Add(dim);
			dim.setParentGroup(this) ;
			return true;
		}

		/// <summary>
		/// Add a nested Group
		/// </summary>
		/// <param name="g"> add this Group. </param>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual void addGroup(Group g)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}

			if (findGroupLocal(g.ShortName) != null)
			{
				throw new ArgumentException("Group name (" + g.ShortName + ") must be unique within Group " + ShortName);
			}

			groups.Add(g);
			g.ParentGroup = this; // groups are a tree - only one parent
		}

		/// <summary>
		/// Add an Enumeration
		/// </summary>
		/// <param name="e"> add this Enumeration. </param>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual void addEnumeration(EnumTypedef e)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			if (e == null)
			{
				return;
			}
			e.ParentGroup = this;
			enumTypedefs.Add(e);
		}

		/// <summary>
		/// Add a Variable
		/// </summary>
		/// <param name="v"> add this Variable. </param>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual void addVariable(Variable v)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			if (v == null)
			{
				return;
			}

			if (findVariableLocal(v.ShortName) != null)
			{
				// Variable other = findVariable(v.getShortName()); // debug
				throw new ArgumentException("Variable name (" + v.ShortName + ") must be unique within Group " + ShortName);
			}

			variables.Add(v);
			v.setParentGroup(this); // variable can only be in one group
		}

		/// <summary>
		/// Remove a Dimension : uses the dimension hashCode to find it.
		/// </summary>
		/// <param name="d"> remove this Dimension. </param>
		/// <returns> true if was found and removed </returns>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual bool remove(Dimension d)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			return (d != null) && dimensions.Remove(d);
		}

		/// <summary>
		/// Remove a Group : uses the Group hashCode to find it.
		/// </summary>
		/// <param name="g"> remove this Group. </param>
		/// <returns> true if was found and removed </returns>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual bool remove(Group g)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			return (g != null) && groups.Remove(g);
		}

		/// <summary>
		/// Remove a Variable : uses the variable hashCode to find it.
		/// </summary>
		/// <param name="v"> remove this Variable. </param>
		/// <returns> true if was found and removed </returns>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public virtual bool remove(Variable v)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			return (v != null) && variables.Remove(v);
		}

		/// <summary>
		/// Make this immutable.
		/// </summary>
		/// <returns> this </returns>
		/// @deprecated Use Group.builder() 
		[Obsolete("Use Group.builder()")]
		public override Group setImmutable()
		{
			base.setImmutable();
			variables = variables.AsReadOnly();
			dimensions = dimensions.AsReadOnly();
			groups = groups.AsReadOnly();
			return this;
		}

		public override string ToString()
		{
			return writeCDL(false);
		}

		/// <summary>
		/// Instances which have same name and parent are equal.
		/// </summary>
		public override bool Equals(object oo)
		{
			if (this == oo)
			{
				return true;
			}
			if (!(oo is Group))
			{
				return false;
			}
			Group og = (Group)oo;
			if (!ShortName.Equals(og.ShortName))
			{
				return false;
			}
			return !((ParentGroup != null) && !ParentGroup.Equals(og.ParentGroup));
		}

		/// <summary>
		/// Override Object.hashCode() to implement equals.
		/// </summary>
		public override int GetHashCode()
		{
			if (hashCode == 0)
			{
				int result = 17;
				result = 37 * result + ShortName.GetHashCode();
				if (ParentGroup != null)
				{
					result = 37 * result + ParentGroup.GetHashCode();
				}
				hashCode = result;
			}
			return hashCode;
		}

		////////////////////////////////////////////////////////////////
		// TODO make private final and immutable in 6
		protected internal NetcdfFile ncfile;
		protected internal IList<Variable> variables = new List<Variable>();
		protected internal IList<Dimension> dimensions = new List<Dimension>();
		protected internal IList<Group> groups = new List<Group>();
		protected AttributeContainer attributes_Conflict;
		protected internal IList<EnumTypedef> enumTypedefs = new List<EnumTypedef>();
		private int hashCode;

		private Group(Builder builder, Group parent) : base(builder.shortName)
		{
			this.group = parent;
			this.ncfile = builder.ncfile;

			foreach (Dimension d in builder.dimensions)
			{
				d.setParentGroup(this) ; // LOOK remove in 6
			}
			this.dimensions = new List<Dimension>(builder.dimensions);
			this.enumTypedefs = new List<EnumTypedef>(builder.enumTypedefs);

			// only the root group build() should be called, the rest get called recursively
			this.groups = new List<Group>();
			foreach (Group.Builder g in builder.gbuilders)
			{
				this.groups.Add(g.setNcfile(this.ncfile).build(this));
			}

			foreach (Variable.Builder vb in builder.vbuilders)
			{
				// dont override ncfile if its been set.
				if (vb.ncfile == null)
				{
					vb.setNcfile(this.ncfile);
				}
			}
			foreach (Variable.Builder vb in builder.vbuilders)
			{
				Variable var = vb.build(this);
				this.variables.Add(var);
			}

			this.attributes_Conflict = builder.attributes;

			// This needs to go away in 6.
			foreach (Dimension d in this.dimensions)
			{
				d.ParentGroup = this;
			}
			foreach (EnumTypedef e in this.enumTypedefs)
			{
				e.ParentGroup = this;
			}
		}

		/// <summary>
		/// Turn into a mutable Builder. Can use toBuilder().build() to copy. </summary>
		public virtual Builder toBuilder()
		{
			Builder b = Group.builder().setName(this.shortName).setNcfile(this.ncfile).addAttributes(this.attributes_Conflict).addDimensions(this.dimensions).addEnumTypedefs(this.enumTypedefs);

			foreach (Group g in this.groups)
			{
				b.addGroup(g.toBuilder());
			}
			foreach (Variable v in this.variables)
			{
				b.addVariable(v.toBuilder());
			}

			return b;
		}

		public static Builder builder()
		{
			return new Builder();
		}

		/// <summary>
		/// A builder of Groups. </summary>
		public class Builder
		{
			private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Group.Builder));

			private Group.Builder parentGroup; // null for root group; ignored during build()
			public IList<Group.Builder> gbuilders = new List<Group.Builder>();
			public IList<Variable.Builder> vbuilders = new List<Variable.Builder>();
			public string shortName = "";
			internal NetcdfFile ncfile; // set by NetcdfFile.build()
			internal AttributeContainerMutable attributes = new AttributeContainerMutable("");
			internal IList<Dimension> dimensions = new List<Dimension>();
			public IList<EnumTypedef> enumTypedefs = new List<EnumTypedef>();
			private bool built;

			public virtual Builder setParentGroup(Group.Builder parentGroup)
			{
				this.parentGroup = parentGroup;
				return this;
			}

			public virtual Group.Builder getParentGroup()
			{
				return this.parentGroup;
			}

			public virtual Builder addAttribute(NcAttribute att)
			{
				Preconditions.checkNotNull(att);
				attributes.addAttribute(att);
				return this;
			}

			public virtual Builder addAttributes(IEnumerable<NcAttribute> atts)
			{
				Preconditions.checkNotNull(atts);
				attributes.addAll(atts);
				return this;
			}

			public virtual AttributeContainerMutable getAttributeContainer()
			{
				return attributes;
			}

			public virtual AttributeContainerMutable AttributeContainer => attributes;

			public virtual IList<Dimension> Dimensions => dimensions;

			/// <summary>
			/// Add Dimension with error if it already exists </summary>
			public virtual Builder addDimension(Dimension dim)
			{
				Preconditions.checkNotNull(dim);
				Optional<Dimension> existing = findDimensionLocal(dim.ShortName);
				if (existing.Present)
				{
					throw new ArgumentException("Dimension '" + existing.get().ShortName + "' already exists");
				}
				dimensions.Add(dim);
				return this;
			}

			/// <summary>
			/// Add Dimension if it doesnt already exist </summary>
			public virtual bool addDimensionIfNotExists(Dimension dim)
			{
				Preconditions.checkNotNull(dim);
				if (!findDimensionLocal(dim.ShortName).Present)
				{
					dimensions.Add(dim);
					return true;
				}
				return false;
			}

			/// <summary>
			/// Add Dimensions with error if any already exist </summary>
			public virtual Builder addDimensions(IEnumerable<Dimension> dims)
			{
				Preconditions.checkNotNull(dims);
				foreach (Dimension d in dims)
				{
					addDimension(d);
				}
				return this;
			}

			/// <summary>
			/// Replace dimension if it exists, else just add it.
			/// </summary>
			/// <returns> true if there was an existing dimension of that name </returns>
			public virtual bool replaceDimension(Dimension dim)
			{
				Optional<Dimension> want = findDimensionLocal(dim.ShortName);
				if (want.Present)
				{
					dimensions.Remove(want.get());
				}
				addDimension(dim);
				return want.Present;
			}

			/// <summary>
			/// Remove dimension, if it exists.
			/// </summary>
			/// <returns> true if there was an existing dimension of that name </returns>
			public virtual bool removeDimension(string name)
			{
				Optional<Dimension> want = findDimensionLocal(name);
				if (want.Present)
				{
					dimensions.Remove(want.get());
				}
				return want.Present;
			}

			/// <summary>
			/// Find Dimension local to this Group </summary>
			public virtual Optional<Dimension> findDimensionLocal(string name)
			{
				foreach (Dimension d in dimensions)
				{
					if (d.ShortName.Equals(name))
					{
						return Optional<Dimension>.of(d);
					}
				}
				return Optional<Dimension>.empty("");
			}

			/// <summary>
			/// Is the Dimension contained within this Group or a parent Group </summary>
			public virtual bool contains(Dimension want)
			{
				Dimension have = null;
				foreach (Dimension d in dimensions)
				{
					if (d.Equals(want))
					{
						have = d;
						break;
					}
				}
				if (have != null)
				{
					return true;
				}
				if (this.parentGroup != null)
				{
					return this.parentGroup.contains(want);
				}
				return false;
			}

			/// <summary>
			/// Find Dimension in this Group or a parent Group </summary>
			public virtual Optional<Dimension> findDimension(string name)
			{
				if (string.ReferenceEquals(name, null))
				{
					return Optional<Dimension>.empty("");
				}
				Optional<Dimension> dopt = findDimensionLocal(name);
				if (dopt.Present)
				{
					return dopt;
				}
				if (this.parentGroup != null)
				{
					return this.parentGroup.findDimension(name);
				}

				return Optional<Dimension>.empty("");
			}

			public virtual IEnumerable<Dimension> getDimensions()
			{
				return dimensions;
			}

			/// <summary>
			/// Add a nested Group. </summary>
			public virtual Builder addGroup(Group.Builder nested)
			{
				Preconditions.checkNotNull(nested);
				Optional<Group.Builder> existing = findGroupLocal(nested.shortName);
				if (existing.Present)
				{
					throw new System.InvalidOperationException("Nested group already exists " + nested.shortName);
				}
				gbuilders.Add(nested);
				nested.setParentGroup(this);
				return this;
			}

			public virtual Builder addGroups(IEnumerable<Group.Builder> groups)
			{
				Preconditions.checkNotNull(groups);
				foreach (Group.Builder g in groups)
				{
					gbuilders.Add(g);
				}
				return this;
			}

			/// <summary>
			/// Remove group, if it exists.
			/// </summary>
			/// <returns> true if there was an existing group of that name </returns>
			public virtual bool removeGroup(string name)
			{
				Optional<Group.Builder> want = findGroupLocal(name);
				if (want.Present)
				{
					gbuilders.Remove(want.get());
				}
				return want.Present;
			}

			public virtual Optional<Group.Builder> findGroupLocal(string shortName)
			{
				foreach (Group.Builder g in this.gbuilders)
				{
					if (g.shortName.Equals(shortName))
					{
						return Optional<Group.Builder>.of(g);
					}
				}
				return Optional<Group.Builder>.empty("");
			}

			/// <summary>
			/// Find a subgroup of this Group, with the specified relative name.
			/// An embedded "/" separates group names.
			/// Can have a leading "/" only if this is the root group.
			/// </summary>
			/// <param name="relativeName"> eg "group/subgroup/wantGroup". </param>
			/// <returns> Group or empty if not found. </returns>
			public virtual Optional<Group.Builder> findGroupNested(string relativeName)
			{
				if (string.ReferenceEquals(relativeName, null) || relativeName.Length == 0)
				{
					return (this.getParentGroup() == null) ? Optional<Group.Builder>.of(this) : Optional<Group.Builder>.empty("");
				}

				Group.Builder g = this;
				StringTokenizer stoke = new StringTokenizer(relativeName, "/");
				while (stoke.hasMoreTokens())
				{
					string groupName = NetcdfFiles.makeNameUnescaped(stoke.nextToken());
					Optional<Group.Builder> sub = g.findGroupLocal(groupName);
					if (!sub.Present)
					{
						return Optional<Group.Builder>.empty("");
					}
					g = sub.get();
				}
				return Optional<Group.Builder>.of(g);
			}

			/// <summary>
			/// Is this group a parent of the other group ? </summary>
			public virtual bool isParent(Group.Builder other)
			{
				while ((other != this) && (other.parentGroup != null))
				{
					other = other.parentGroup;
				}
				return (other == this);
			}

			/// <summary>
			/// Find the common parent with the other group ? </summary>
			public virtual Group.Builder commonParent(Group.Builder other)
			{
				if (isParent(other))
				{
					return this;
				}
				if (other.isParent(this))
				{
					return other;
				}
				while (!other.isParent(this))
				{
					other = other.parentGroup;
				}
				return other;
			}

			/// <summary>
			/// Locate an enum type definition that is structurally
			/// similar to the template type def. The Enum names are ignored.
			/// </summary>
			/// <param name="template"> match this enum type def </param>
			/// <param name="searchup"> if true, then search this group and then parent groups. </param>
			public virtual Optional<EnumTypedef> findSimilarEnumTypedef(EnumTypedef template, bool searchup)
			{
				Debug.Assert((template != null));
				// search this group builders's EnumTypedefs but with constraint on name
				foreach (EnumTypedef e in this.enumTypedefs)
				{
					if (e.equalsMapOnly(template))
					{
						return Optional<EnumTypedef>.of(e);
					}
				}
				// Optionally search parents
				if (searchup)
				{
					Group.Builder gb = getParentGroup();
					if (gb != null)
					{
						Optional<EnumTypedef> ed = gb.findSimilarEnumTypedef(template, searchup);
						if (ed.Present)
						{
							return ed;
						}
					}
				}
				return Optional<EnumTypedef>.empty("");
			}

			/// <summary>
			/// Find a Enumeration in this Group Builder, using its short name. </summary>
			public virtual Optional<EnumTypedef> findEnumTypedef(string name)
			{
				// Keep the old behavior
				return findEnumTypedef(name, false);
			}

			/// <summary>
			/// Find a Enumeration in this or a parent Group Builder, using its short name.
			/// </summary>
			/// <param name="name"> for which to search </param>
			/// <param name="searchup"> if true, then search this group and then parent groups. </param>
			public virtual Optional<EnumTypedef> findEnumTypedef(string name, bool searchup)
			{
				if (string.ReferenceEquals(name, null))
				{
					return Optional<EnumTypedef>.empty("");
				}
				// search this group builders's EnumTypedefs
				foreach (EnumTypedef e in this.enumTypedefs)
				{
					if (e.ShortName.Equals(name))
					{
						return Optional<EnumTypedef>.of(e);
					}
				}
				// Optionally search parents
				if (searchup)
				{
					Group.Builder gb = getParentGroup();
					if (gb != null)
					{
						Optional<EnumTypedef> ed = gb.findEnumTypedef(name, searchup);
						if (ed.Present)
						{
							return ed;
						}
					}
				}
				return Optional<EnumTypedef>.empty("");
			}

			public virtual Builder addEnumTypedef(EnumTypedef typedef)
			{
				Preconditions.checkNotNull(typedef);
				enumTypedefs.Add(typedef);
				return this;
			}

			public virtual Builder addEnumTypedefs(IEnumerable<EnumTypedef> typedefs)
			{
				Preconditions.checkNotNull(typedefs);
				foreach (EnumTypedef t in typedefs)
				{
					enumTypedefs.Add(t);
				}
				return this;
			}

			/// <summary>
			/// Add a EnumTypedef if it does not already exist.
			/// Return new or existing.
			/// </summary>
			public virtual EnumTypedef findOrAddEnumTypedef(string name, IDictionary<int, string> map)
			{
				Optional<EnumTypedef> opt = findEnumTypedef(name); // Find in this group
				if (opt.Present)
				{
					return opt.get();
				}
				else
				{
					EnumTypedef enumTypedef = new EnumTypedef(name, map);
					addEnumTypedef(enumTypedef);
					return enumTypedef;
				}
			}

			/// <summary>
			/// Add a Variable, throw error if one of the same name if it exists. </summary>
			public virtual Builder addVariable(Variable.Builder variable)
			{
				Preconditions.checkNotNull(variable);
				Optional<Variable.Builder> existing = findVariableLocal(variable.shortName);
				if (existing.Present)
				{
					throw new ArgumentException("Variable '" + existing.get().shortName + "' already exists");
				}
				vbuilders.Add(variable);
				variable.setParentGroupBuilder(this);
				return this;
			}

			/// <summary>
			/// Add Variables, throw error if one of the same name if it exists. </summary>
			public virtual Builder addVariables(IEnumerable<Variable.Builder> vars)
			{
				foreach (Variable.Builder v in vars)
				{
					addVariable(v);
				}
				return this;
			}

			/// <summary>
			/// Replace variable of same name, if it exists, else just add it.
			/// </summary>
			/// <returns> true if there was an existing variable of that name </returns>
			public virtual bool replaceVariable(Variable.Builder vb)
			{
				Optional<Variable.Builder> want = findVariableLocal(vb.shortName);
				if (want.Present)
				{
					vbuilders.Remove(want.get());
				}
				addVariable(vb);
				return want.Present;
			}

			/// <summary>
			/// Remove variable, if it exists.
			/// </summary>
			/// <returns> true if there was an existing variable of that name </returns>
			public virtual bool removeVariable(string name)
			{
				Optional<Variable.Builder> want = findVariableLocal(name);
				if (want.Present)
				{
					vbuilders.Remove(want.get());
				}
				return want.Present;
			}

			public virtual Optional<Variable.Builder> findVariableLocal(string name)
			{
				foreach (Variable.Builder v in vbuilders)
				{
					if (v.shortName.Equals(name))
					{
						return Optional<Variable.Builder>.of(v);
					}
				}
				return Optional<Variable.Builder>.empty("");
			}

			/// <summary>
			/// Find a Variable, with the specified relative name. No structure members.
			/// </summary>
			/// <param name="relativeName"> eg "group/subgroup/varname". </param>
			public virtual Optional<Variable.Builder> findVariableNested(string relativeName)
			{
				if (string.ReferenceEquals(relativeName, null) || relativeName.Length == 0)
				{
					return Optional<Variable.Builder>.empty("");
				}

				// break into groupNames and varName
				Group.Builder group = this;
				string varName = relativeName;
				int pos = relativeName.LastIndexOf('/');
				if (pos >= 0)
				{
					string groupNames = relativeName.Substring(0, pos);
					varName = relativeName.Substring(pos + 1);
					Optional<Group.Builder> gopt = findGroupNested(groupNames);
					group = gopt.Present ? gopt.get() : null;
				}

				return group == null ? Optional<Variable.Builder>.empty("") : group.findVariableLocal(varName);
			}

			/// <summary>
			/// Find the Variable with the specified (short) name in this group or a parent group.
			/// </summary>
			/// <param name="varShortName"> short name of Variable. </param>
			/// <returns> the Variable or empty. </returns>
			public virtual Optional<Variable.Builder> findVariableOrInParent(string varShortName)
			{
				if (string.ReferenceEquals(varShortName, null))
				{
					return Optional<Variable.Builder>.empty("");
				}

				Optional<Variable.Builder> vopt = findVariableLocal(varShortName);

				Group.Builder parent = getParentGroup();
				if (!vopt.Present && (parent != null))
				{
					vopt = parent.findVariableOrInParent(varShortName);
				}
				return vopt;
			}

			// Generally ncfile is set in NetcdfFile.build()
			public virtual Builder setNcfile(NetcdfFile ncfile)
			{
				this.ncfile = ncfile;
				return this;
			}

			/// <summary>
			/// NetcdfFile that owns this group (property form for translated call sites). </summary>
			public virtual NetcdfFile Ncfile
			{
				get => ncfile;
				set => ncfile = value;
			}

			public virtual Builder setName(string shortName)
			{
				this.shortName = NetcdfFiles.makeValidCdmObjectName(shortName);
				return this;
			}

			/// @deprecated Use Group.builder() 
			[Obsolete("Use Group.builder()")]
			public virtual NetcdfFile getNcfile()
			{
				return this.ncfile;
			}

			/// <summary>
			/// Make list of dimensions by looking in this Group or parent groups </summary>
			public virtual ImmutableList<Dimension> makeDimensionsList(string dimString)
			{
				return NetcdfSharp.ucar.nc2.Dimensions.makeDimensionsList(name => this.findDimension(name).orElse(null), dimString);
			}

			private class DimensionFinder : Dimensions.FindDimensionByName
			{
				private readonly Group.Builder group;

				internal DimensionFinder(Group.Builder group)
				{
					this.group = group;
				}

				public Dimension? findByName(string dimName)
				{
					Optional<Dimension> dopt = group.findDimension(dimName);
					return dopt.Present ? dopt.get() : null;
				}
			}

			/// <summary>
			/// Make the full name of the this group.
			/// TODO In light of CF groups, we may have to start full names with '/'
			/// </summary>
			public virtual string makeFullName()
			{
				if (parentGroup == null)
				{
					return "";
				}
				StringBuilder sbuff = new StringBuilder();
				appendGroupName(sbuff, this);
				return sbuff.ToString();
			}

			private void appendGroupName(StringBuilder sbuff, Group.Builder g)
			{
				if ((g == null) || (g.getParentGroup() == null))
				{
					return;
				}
				appendGroupName(sbuff, g.getParentGroup());
				sbuff.Append(EscapeStrings.backslashEscape(g.shortName, NetcdfSharp.ucar.nc2.NetcdfFiles.reservedFullName));
				sbuff.Append("/");
			}

			/// <summary>
			/// Remove the given dimension from this group and any subgroups </summary>
			public virtual void removeDimensionFromAllGroups(Group.Builder group, Dimension remove)
			{
				for (int i = 0; i < group.dimensions.Count; i++)
				{
					if (group.dimensions[i].Equals(remove))
					{
						group.dimensions.RemoveAt(i);
						i--;
					}
				}
				foreach (Group.Builder g in group.gbuilders)
				{
					removeDimensionFromAllGroups(g, remove);
				}
			}

			/// <summary>
			/// Make a multimap of Dimensions and all the variables that reference them, in this group and its nested groups. </summary>
			public virtual void makeDimensionMap(Group.Builder parent, Multimap<Dimension, Variable.Builder> dimUsedMap)
			{
				foreach (Variable.Builder v in parent.vbuilders)
				{
					foreach (Dimension d in getDimensionsFor(parent, v))
					{
						if (d.Shared)
						{
							dimUsedMap.put(d, v);
						}
					}
				}
				foreach (Group.Builder g in parent.gbuilders)
				{
					makeDimensionMap(g, dimUsedMap);
				}
			}

			private IList<Dimension> getDimensionsFor(Group.Builder gb, Variable.Builder vb)
			{
				// TODO: In 6.0 remove group field in dimensions, just use equals() to match.
				IList<Dimension> dims = new List<Dimension>();
				foreach (Dimension dim in vb.getDimensions())
				{
					if (dim.Shared)
					{
						Optional<Dimension> sharedDimOpt = gb.findDimension(dim.ShortName);
						Dimension sharedDim = sharedDimOpt.Present ? sharedDimOpt.get() : null;
						if (sharedDim == null)
						{
							throw new System.InvalidOperationException(string.Format("Shared Dimension {0} does not exist in a parent group", dim));
						}
						else
						{
							dims.Add(sharedDim);
						}
					}
					else
					{
						dims.Add(dim);
					}
				}
				return dims;
			}

			/// <summary>
			/// Build the root group, with parent = null. </summary>
			public virtual Group build()
			{
				return build(null);
			}

			/// <summary>
			/// Normally this is called by NetcdfFile.build() </summary>
			internal virtual Group build(Group parent)
			{
				if (built)
				{
					throw new System.InvalidOperationException("Group was already built " + this.shortName);
				}
				built = true;
				return new Group(this, parent);
			}
		}
	}

}
