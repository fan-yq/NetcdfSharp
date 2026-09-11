// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Define a superclass for all the CDM node classes: Group, Dimension, etc.
	/// Define the sort of the node <seealso cref="CDMSort"/> so that we can
	/// 1. do true switching on node type
	/// 2. avoid use of instanceof
	/// 3. Use container classes that have more than one kind of node
	/// <para>
	/// Also move various common fields and methods to here.
	/// 
	/// @author Heimbigner
	/// </para>
	/// </summary>
	/// @deprecated Will move to Dap4 module in version 6. 
	[Obsolete("Will move to Dap4 module in version 6.")]
	public abstract class CDMNode
	{

		protected internal CDMSort sort;
		protected internal Group group;
		protected internal Structure parentstruct;
		protected internal bool immutable;
		protected internal string shortName;

		protected internal IDictionary<object, object> annotations;

		// String fullName = null; // uses backslash escaping

		//////////////////////////////////////////////////
		// Constructors

		protected internal CDMNode()
		{
			// Use Instanceof to figure out the sort
			if (this is NcAttribute)
			{
				Sort = CDMSort.ATTRIBUTE;
			}
			else if (this is Dimension)
			{
				Sort = CDMSort.DIMENSION;
			}
			else if (this is EnumTypedef)
			{
				Sort = CDMSort.ENUMERATION;
			}
			else if (this is Sequence)
			{
				Sort = CDMSort.SEQUENCE;
			}
			else if (this is Structure)
			{
				Sort = CDMSort.STRUCTURE;
			}
			else if (this is Group)
			{
				Sort = CDMSort.GROUP;
			}
			else if (this is Variable) // Only case left is atomic var
			{
				Sort = CDMSort.VARIABLE;
			}
		}

		public CDMNode(string name) : this()
		{
			ShortName = name;
		}

		/// @deprecated do not use 
		[Obsolete("do not use")]
		public virtual CDMSort Sort
		{
			get
			{
				return this.sort;
			}
			set
			{
				if (!immutable)
				{
					this.sort = value;
				}
			}
		}


		/// <summary>
		/// Get the short name of this Variable. The name is unique within its parent group.
		/// </summary>
		public virtual string ShortName
		{
			get
			{
				return this.shortName;
			}
			set
			{
				if (!immutable)
				{
					this.shortName = NetcdfFile.makeValidCdmObjectName(value);
				}
			}
		}

		public virtual string getShortName() => ShortName;


		/// <summary>
		/// Get its parent Group, or null if its the root group.
		/// </summary>
		/// <returns> parent Group </returns>
		/// @deprecated Do not use for Dimension or Attribute 
		[Obsolete("Do not use for Dimension or Attribute")]
		public virtual Group ParentGroup
		{
			get
			{
				return this.group;
			}
			set
			{
				if (!immutable)
				{
					this.group = value;
				}
			}
		}

		/// <summary>
		/// Alias for getParentGroup
		/// </summary>
		/// <returns> parent Group </returns>
		/// @deprecated Do not use. 
		[Obsolete("Do not use.")]
		public virtual Group Group
		{
			get
			{
				return ParentGroup;
			}
		}


		/// <summary>
		/// Get its parent structure, or null if not in structure
		/// </summary>
		/// <returns> parent structure </returns>
		/// @deprecated Do not use. 
		[Obsolete("Do not use.")]
		public virtual Structure ParentStructure
		{
			get
			{
				return this.parentstruct;
			}
			set
			{
				if (!immutable)
				{
					this.parentstruct = value;
				}
			}
		}


		/// <summary>
		/// Test for presence of parent Structure
		/// </summary>
		/// <returns> true iff struct != null </returns>
		/// @deprecated Do not use. 
		[Obsolete("Do not use.")]
		public virtual bool MemberOfStructure
		{
			get
			{
				return this.parentstruct != null;
			}
		}

		/// <summary>
		/// Get immutable flag
		/// As a rule, subclasses will access directly
		/// </summary>
		/// <returns> Immutable flag </returns>
		/// @deprecated Do not use. 
		[Obsolete("Do not use.")]
		public virtual bool Immutable
		{
			get
			{
				return this.immutable;
			}
		}

		/// <summary>
		/// Set the immutable flag to true.
		/// Once set, cannot be unset
		/// </summary>
		/// @deprecated Do not use. 
		[Obsolete("Do not use.")]
		public virtual CDMNode setImmutable()
		{
			this.immutable = true;
			return this;
		}


		/// <summary>
		/// Get the Full name of this object. Certain characters are
		/// backslash escaped (see NetcdfFile)
		/// </summary>
		/// <returns> full name with backslash escapes </returns>
		/// @deprecated use NetcdfFiles.makeFullName(*) 
		[Obsolete("use NetcdfFiles.makeFullName(*)")]
		public virtual string FullName
		{
			get
			{
				return NetcdfFile.makeFullName(this);
			}
		}

		/// <summary>
		/// Alias for getFullName
		/// </summary>
		/// <returns> full name with backslash escapes </returns>
		/// @deprecated use getFullName() when it exists in ver6. 
		public virtual string FullNameEscaped
		{
			get
			{
				return FullName;
			}
		}


		/// <summary>
		/// getName is deprecated because, as the code below shows,
		/// it has no consistent meaning. Sometimes it returns
		/// the short name, sometimes it returns the full name.
		/// </summary>
		/// @deprecated Replaced by <seealso cref="getShortName()"/> and <seealso cref=".getFullName()"/> 
		[Obsolete("Replaced by <seealso cref=\"getShortName()\"/> and <seealso cref=\".getFullName()\"/>")]
		public virtual string Name
		{
			get
			{
				switch (sort)
				{
					case NetcdfSharp.ucar.nc2.CDMSort.ATTRIBUTE:
					case NetcdfSharp.ucar.nc2.CDMSort.DIMENSION:
					case NetcdfSharp.ucar.nc2.CDMSort.ENUMERATION:
						// for these cases, getName is getShortName
						return ShortName;

					case NetcdfSharp.ucar.nc2.CDMSort.VARIABLE: // Atomic
					case NetcdfSharp.ucar.nc2.CDMSort.SEQUENCE:
					case NetcdfSharp.ucar.nc2.CDMSort.STRUCTURE:
					case NetcdfSharp.ucar.nc2.CDMSort.GROUP:
						// for these cases, getName is getFullName
						return FullName;
					default:
						break;
				}
				return ShortName; // default
			}
		}

		// Override the node's hashCode for subclasses of CDMNode.

		/// @deprecated do not use 
		public virtual int localhash()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// NetcdfDataset can end up wrapping a variable
		/// in multiple wrapping classes (e.g. VariableDS).
		/// Goal of this procedure is to get down to the
		/// lowest level Variable instance
		/// </summary>
		/// <param name="node"> possibly wrapped ode </param>
		/// <returns> the lowest level node instance </returns>
		/// @deprecated do not use 
		[Obsolete("do not use")]
		public virtual object annotation(object id)
		{
			return (this.annotations == null ? null : this.annotations[id]);
		}

		/// @deprecated do not use 
		public virtual object annotate(object id, object value)
		{
			if (annotations == null)
			{
				annotations = new Dictionary<object, object>();
			}
			object old = annotations[id];
			annotations[id] = value;
			return old;
		}

	}

}
