// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2
{
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// A Dimension is used to define the array shape of a Variable.
	/// A Variable can be thought of as a sampled function with Domain its Dimensions.
	/// A Dimension may be shared among Variables, which provides a simple yet powerful way of associating Variables.
	/// When a Dimension is shared, it has a unique name within its Group.
	/// It may have a coordinate Variable, which gives each index a coordinate value.
	/// A private Dimension cannot have a coordinate Variable, so use shared dimensions with coordinates when possible.
	/// The Dimension length must be > 0, except for an unlimited dimension which may have length = 0, and a vlen
	/// Dimension which has a length known only when the variable is read.
	/// <p/>
	/// <para>
	/// Immutable if setImmutable() was called, except for an Unlimited Dimension, whose size can change.
	/// </para>
	/// <para>
	/// Note: this class has a natural ordering that is inconsistent with equals.
	/// 
	/// TODO Dimension will be immutable in 6.
	/// TODO Dimension will not extend CDMNode in 6.
	/// TODO Dimension will not have a reference to its owning Group in 6.
	/// TODO Dimension.getFullName() will not exist in 6.
	/// 
	/// @author caron
	/// </para>
	/// </summary>
	public class Dimension : CDMNode, IComparable<Dimension>
	{
	  /// <summary>
	  /// A variable-length dimension: the length is not known until the data is read. </summary>
	  public static readonly Dimension VLEN = Dimension.builder().setName("*").setIsVariableLength(true).build().setImmutable();

	  /// <summary>
	  /// Make a space-delineated String from a list of Dimension names.
	  /// Inverse of makeDimensionsList().
	  /// </summary>
	  /// <returns> space-delineated String of Dimension names. </returns>
	  /// @deprecated use Dimensions.makeDimensionsString() 
	  [Obsolete("use Dimensions.makeDimensionsString()")]
	  public static string makeDimensionsString(IList<Dimension> dimensions)
	  {
		if (dimensions == null)
		{
		  return "";
		}

		Formatter buf = new Formatter();
		for (int i = 0; i < dimensions.Count; i++)
		{
		  Dimension myd = dimensions[i];
		  string dimName = myd.ShortName;

		  if (i != 0)
		  {
			buf.format(" ");
		  }

		  if (myd.VariableLength)
		  {
			buf.format("*");
		  }
		  else if (myd.Shared)
		  {
			buf.format("%s", dimName);
		  }
		  else
		  {
			// if (dimName != null) // LOOK losing anon dim name
			// buf.format("%s=", dimName);
			buf.format("%d", myd.Length);
		  }
		}
		return buf.ToString();
	  }

	  /// <summary>
	  /// Create a dimension list using the dimensions names. The dimension is searched for recursively in the parent groups,
	  /// so it must already have been added.
	  /// Inverse of makeDimensionsString().
	  /// </summary>
	  /// <param name="parentGroup"> containing group, may not be null </param>
	  /// <param name="dimString"> : whitespace separated list of dimension names, or '*' for Dimension.UNKNOWN, or number for anon
	  ///        dimension. null or empty String is a scalar. </param>
	  /// <returns> list of dimensions </returns>
	  /// <exception cref="IllegalArgumentException"> if cant find dimension or parse error. </exception>
	  /// @deprecated use Group.makeDimensionsList() 
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Deprecated("use Group.makeDimensionsList()") public static NetcdfSharp.java.util.List<Dimension> makeDimensionsList(Group parentGroup, String dimString) throws IllegalArgumentException
	  [Obsolete("use Group.makeDimensionsList()")]
	  public static IList<Dimension> makeDimensionsList(Group parentGroup, string dimString)
	  {
		IList<Dimension> newDimensions = new List<Dimension>();
		if (string.ReferenceEquals(dimString, null)) // scalar
		{
		  return newDimensions; // empty list
		}
		dimString = dimString.Trim();
		if (dimString.Length == 0) // scalar
		{
		  return newDimensions; // empty list
		}

		StringTokenizer stoke = new StringTokenizer(dimString);
		while (stoke.hasMoreTokens())
		{
		  string dimName = stoke.nextToken();
		  Dimension d;
		  if (dimName.Equals("*"))
		  {
			d = Dimension.VLEN;
		  }
		  else
		  {
			d = parentGroup.findDimension(dimName);
		  }
		  if (d == null)
		  {
			// if numeric - then its anonymous dimension
			try
			{
			  int len = int.Parse(dimName);
			  d = Dimension.builder().setLength(len).setIsShared(false).build();
			}
			catch (Exception)
			{
			  throw new ArgumentException("Dimension " + dimName + " does not exist");
			}
		  }
		  newDimensions.Add(d);
		}

		return newDimensions;
	  }

	  /// <summary>
	  /// Create a dimension list using the dimensions names.
	  /// Inverse of makeDimensionsString().
	  /// </summary>
	  /// <param name="dimensions"> list of possible dimensions, may not be null </param>
	  /// <param name="dimString"> : whitespace separated list of dimension names, or '*' for Dimension.UNKNOWN, or number for anon
	  ///        dimension. null or empty String is a scalar. </param>
	  /// <returns> list of dimensions </returns>
	  /// <exception cref="IllegalArgumentException"> if cant find dimension or parse error. </exception>
	  /// @deprecated use Dimensions.makeDimensionsList() 
	  [Obsolete("use Dimensions.makeDimensionsList()")]
	  public static IList<Dimension> makeDimensionsList(IList<Dimension> dimensions, string dimString)
	  {
		IList<Dimension> newDimensions = new List<Dimension>();
		if (string.ReferenceEquals(dimString, null)) // scalar
		{
		  return newDimensions; // empty list
		}
		dimString = dimString.Trim();
		if (dimString.Length == 0) // scalar
		{
		  return newDimensions; // empty list
		}

		StringTokenizer stoke = new StringTokenizer(dimString);
		while (stoke.hasMoreTokens())
		{
		  string dimName = stoke.nextToken();
		  Dimension dim;
		  if (dimName.Equals("*"))
		  {
			dim = Dimension.VLEN;
		  }
		  else
		  {
			dim = dimensions.FirstOrDefault(d => d.getShortName().Equals(dimName));
		  }
		  if (dim == null)
		  {
			// if numeric - then its anonymous dimension
			try
			{
			  int len = int.Parse(dimName);
			  dim = Dimension.builder().setLength(len).setIsShared(false).build();
			}
			catch (Exception)
			{
			  throw new ArgumentException("Dimension " + dimName + " does not exist");
			}
		  }
		  newDimensions.Add(dim);
		}
		return newDimensions;
	  }

	  /// @deprecated use Dimensions.makeDimensionsAnon() 
	  [Obsolete("use Dimensions.makeDimensionsAnon()")]
	  public static IList<Dimension> makeDimensionsAnon(int[] shape)
	  {
		IList<Dimension> newDimensions = new List<Dimension>();
		if ((shape == null) || (shape.Length == 0))
		{ // scalar
		  return newDimensions;
		}
		foreach (int len in shape)
		{
		  newDimensions.Add(Dimension.builder().setLength(len).setIsShared(false).build());
		}
		return newDimensions;
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  // TODO make these final in 6.
	  private bool isUnlimited;
	  private bool isVariableLength;
	  private bool isShared; // shared means its in a group dimension list.
	  private int length;

	  private Dimension(Builder builder) : base(builder.shortName)
	  {
		ParentGroup = builder.parent;
		this.isShared = builder.isShared;
		this.isVariableLength = builder.isVariableLength;
		this.isUnlimited = builder.isUnlimited;
		this.length = builder.length;
	  }

	  /// <summary>
	  /// Turn into a mutable Builder, use toBuilder().build() to make a copy. </summary>
	  public virtual Builder toBuilder()
	  {
		Builder b = builder();
		b.parent = this.Group;
		return b.setName(this.shortName).setIsUnlimited(this.isUnlimited).setIsVariableLength(this.isVariableLength).setIsShared(this.isShared).setLength(this.length);
	  }

	  /// <summary>
	  /// Get the length of the Dimension. </summary>
	  public virtual int Length
	  {
		  get
		  {
			return length;
		  }
		  set
		  {
			if (immutable && !isUnlimited)
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			if (isVariableLength)
			{
			  if (value != -1)
			  {
				throw new ArgumentException("VariableLength Dimension length =" + value + " must be -1");
			  }
			}
			else if (isUnlimited)
			{
			  if (value < 0)
			  {
				throw new ArgumentException("Unlimited Dimension length =" + value + " must >= 0");
			  }
			}
			this.length = value;
		  }
	  }

	  /// <summary>
	  /// Get the name of the Dimension. Same as getShortName. Not deprecated. </summary>
	  public override string Name
	  {
		  get
		  {
			return this.shortName;
		  }
	  }

	  /// <summary>
	  /// Get the name of the Dimension. </summary>
	  public override string ShortName
	  {
		  get
		  {
			return this.Name;
		  }
	  }

	  /// <summary>
	  /// If this is a NetCDF unlimited dimension. The length might increase between invocations,
	  /// but it remains fixed for the lifetime of the NetcdfFile.
	  /// If you modify the file in a separate process, you must close and reopen the file.
	  /// </summary>
	  /// <returns> if its an "unlimited" Dimension </returns>
	  public virtual bool Unlimited
	  {
		  get
		  {
			return isUnlimited;
		  }
		  set
		  {
			if (immutable)
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			this.isUnlimited = value;
			Length = this.length; // check legal
		  }
	  }

	  /// <summary>
	  /// If variable length, then the length is unknown until the data is read.
	  /// </summary>
	  /// <returns> if its a "variable length" Dimension. </returns>
	  public virtual bool VariableLength
	  {
		  get
		  {
			return isVariableLength;
		  }
		  set
		  {
			if (immutable)
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			this.isVariableLength = value;
			if (value)
			{
			  this.isShared = false;
			  this.isUnlimited = false;
			}
			Length = this.length; // check legal
		  }
	  }

	  /// <summary>
	  /// If this Dimension is shared, or is private to a Variable.
	  /// All Dimensions in NetcdfFile.getDimensions() or Group.getDimensions() are shared.
	  /// Dimensions in the Variable.getDimensions() may be shared or private.
	  /// </summary>
	  /// <returns> if its a "shared" Dimension. </returns>
	  public virtual bool Shared
	  {
		  get
		  {
			return isShared;
		  }
		  set
		  {
			if (immutable)
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			this.isShared = value;
		  }
	  }

	  /// <summary>
	  /// Get the Group that owns this Dimension.
	  /// </summary>
	  /// <returns> owning group or null if !isShared </returns>
	  /// @deprecated Do not use. 
	  [Obsolete("Do not use.")]
	  public override Group Group
	  {
		  get
		  {
			return ParentGroup;
		  }
	  }

	  public void setParentGroup(Group group)
		{
			if (immutable)
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			ParentGroup = group;
		}


	  /// @deprecated Do not use. 
	  [Obsolete("Do not use.")]
	  public virtual string makeFullName()
	  {
		return base.FullName;
	  }

	  public override bool Equals(object oo)
	  {
		if (this == oo)
		{
		  return true;
		}
		if (!(oo is Dimension))
		{
		  return false;
		}
		Dimension other = (Dimension) oo;
		Group g = Group;
		if ((g != null) && !g.Equals(other.Group)) // TODO remove group in 6
		{
		  return false;
		}
		if ((string.ReferenceEquals(ShortName, null)) && (!string.ReferenceEquals(other.ShortName, null)))
		{
		  return false;
		}
		if ((!string.ReferenceEquals(ShortName, null)) && !ShortName.Equals(other.ShortName))
		{
		  return false;
		}
		return (Length == other.Length) && (Unlimited == other.Unlimited) && (VariableLength == other.VariableLength) && (Shared == other.Shared);
	  }

	  /// <summary>
	  /// Override Object.hashCode() to implement equals.
	  /// </summary>
	  public override int GetHashCode()
	  {
		int result = 17;
		Group g = Group; // TODO remove group in 6
		if (g != null)
		{
		  result += 37 * result + g.GetHashCode();
		}
		if (null != ShortName)
		{
		  result += 37 * result + ShortName.GetHashCode();
		}
		result += 37 * result + Length;
		result += 37 * result + (Unlimited ? 0 : 1);
		result += 37 * result + (VariableLength ? 0 : 1);
		result += 37 * result + (Shared ? 0 : 1);
		return result;
	  }

	  public override string ToString()
	  {
		return writeCDL(false);
	  }

	  /// <summary>
	  /// Dimensions with the same name are equal. This method is inconsistent with equals()!
	  /// </summary>
	  /// <param name="odim"> compare to this Dimension </param>
	  /// <returns> 0, 1, or -1 </returns>
	  public virtual int CompareTo(Dimension odim)
	  {
		string name = ShortName;
		return string.CompareOrdinal(name, odim.ShortName);
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
		Formatter f = new Formatter();
		writeCDL(f, new Indent(2), strict);
		return f.ToString();
	  }

	  /// @deprecated use CDLWriter 
	  [Obsolete("use CDLWriter")]
	  internal virtual void writeCDL(Formatter @out, Indent indent, bool strict)
	  {
		string name = strict ? NetcdfFiles.makeValidCDLName(ShortName) : ShortName;
		@out.format("%s%s", indent, name);
		if (Unlimited)
		{
		  @out.format(" = UNLIMITED;   // (%d currently)", Length);
		}
		else if (VariableLength)
		{
		  @out.format(" = UNKNOWN;");
		}
		else
		{
		  @out.format(" = %d;", Length);
		}
	  }

	  /// <summary>
	  /// Constructor
	  /// </summary>
	  /// <param name="name"> name must be unique within group </param>
	  /// <param name="length"> length of Dimension </param>
	  public Dimension(string name, int length) : this(name, length, true, false, false)
	  {
	  }

	  /// <summary>
	  /// Constructor
	  /// </summary>
	  /// <param name="name"> name must be unique within group </param>
	  /// <param name="length"> length, or UNLIMITED.length or UNKNOWN.length </param>
	  /// <param name="isShared"> whether its shared or local to Variable. </param>
	  /// @deprecated Use Dimension.builder() 
	  [Obsolete("Use Dimension.builder()")]
	  public Dimension(string name, int length, bool isShared) : this(name, length, isShared, false, false)
	  {
	  }

	  /// <summary>
	  /// Constructor
	  /// </summary>
	  /// <param name="name"> name must be unique within group. Can be null only if not shared. </param>
	  /// <param name="length"> length, or UNLIMITED.length or UNKNOWN.length </param>
	  /// <param name="isShared"> whether its shared or local to Variable. </param>
	  /// <param name="isUnlimited"> whether the length can grow. </param>
	  /// <param name="isVariableLength"> whether the length is unknown until the data is read. </param>
	  public Dimension(string name, int length, bool isShared, bool isUnlimited, bool isVariableLength) : base(name)
	  {
		this.isShared = isShared;
		this.isUnlimited = isUnlimited;
		this.isVariableLength = isVariableLength;
		if (isVariableLength && (isUnlimited || isShared))
		{
		  throw new ArgumentException("variable length dimension cannot be shared or unlimited");
		}
		Length = length;
		Debug.Assert((!string.ReferenceEquals(name, null)) || !this.isShared);
	  }

	  /// <summary>
	  /// Copy Constructor. used to make global dimensions
	  /// </summary>
	  /// <param name="name"> name must be unique within group. Can be null only if not shared. </param>
	  /// <param name="from"> copy all other fields from here. </param>
	  /// @deprecated Use Dimension.builder() 
	  [Obsolete("Use Dimension.builder()")]
	  public Dimension(string name, Dimension from) : base(name)
	  {
		this.length = from.length;
		this.isUnlimited = from.isUnlimited;
		this.isVariableLength = from.isVariableLength;
		this.isShared = from.isShared;
	  }

	  ///////////////////////////////////////////////////////////
	  // the following make this mutable






	  /// <summary>
	  /// Set the name, converting to valid CDM object name if needed.
	  /// </summary>
	  /// <param name="name"> set to this value </param>
	  /// <returns> valid CDM object name </returns>
	  /// @deprecated Use Dimension.builder() 
	  [Obsolete("Use Dimension.builder()")]
	  public virtual string setName(string name)
	  {
		if (immutable)
		{
		  throw new System.InvalidOperationException("Cant modify");
		}
		if (name.Length == 0)
		{
		  name = null;
		}
		ShortName = name;
		return ShortName;
	  }



	  /// <summary>
	  /// Make this immutable.
	  /// </summary>
	  /// <returns> this </returns>
	  /// @deprecated Use Dimension.builder() 
	  [Obsolete("Use Dimension.builder()")]
	  public override Dimension setImmutable()
	  {
		base.setImmutable();
		return this;
	  }

	  //////////////////////////////////////////////////////////////

	  public static Builder builder()
	  {
		return new Builder();
	  }

	  /// <summary>
	  /// A builder with the Dimension name and length set </summary>
	  public static Builder builder(string name, int length)
	  {
		return (new Builder()).setName(name).setLength(length);
	  }

	  /// <summary>
	  /// A builder of Dimensions. </summary>
	  public class Builder
	  {
		internal Group parent;
		internal string shortName;
		internal bool isUnlimited;
		internal bool isVariableLength;
		internal bool isShared = true; // shared means its in a group dimension list.
		internal int length;
		internal bool built;

		internal Builder()
		{
		}

		/// <summary>
		/// Set is unlimited.
		/// Used by netcdf3 for variables whose outer dimension can change
		/// </summary>
		public virtual Builder setIsUnlimited(bool isUnlimited)
		{
		  this.isUnlimited = isUnlimited;
		  return this;
		}

		/// <summary>
		/// Set variable length is true.
		/// Implies that its not shared, nor unlimited.
		/// Used by sequences.
		/// </summary>
		public virtual Builder setIsVariableLength(bool isVariableLength)
		{
		  this.isVariableLength = isVariableLength;
		  if (isVariableLength)
		  {
			this.isShared = false;
			this.isUnlimited = false;
			this.length = -1;
		  }
		  return this;
		}

		/// <summary>
		/// Set whether this is shared.
		/// Default value is true.
		/// Implies it is owned by a Group.
		/// </summary>
		public virtual Builder setIsShared(bool isShared)
		{
		  this.isShared = isShared;
		  return this;
		}

		/// <summary>
		/// Set the Dimension length.
		/// </summary>
		/// <param name="n"> length of Dimension </param>
		public virtual Builder setLength(int n)
		{
		  if (isVariableLength)
		  {
			if (n != -1)
			{
			  throw new ArgumentException("VariableLength Dimension length =" + n + " must be -1");
			}
		  }
		  else if (isUnlimited)
		  {
			if (n < 0)
			{
			  throw new ArgumentException("Unlimited Dimension length =" + n + " must >= 0");
			}
		  }
		  else
		  {
			if (n < 1)
			{
			  throw new ArgumentException("Dimension length =" + n + " must be > 0");
			}
		  }
		  this.length = n;
		  return this;
		}

		public virtual Builder setName(string shortName)
		{
		  this.shortName = StringUtil2.makeValidCdmObjectName(shortName);
		  return this;
		}

		public virtual Dimension build()
		{
		  if (built)
		  {
			throw new System.InvalidOperationException("already built");
		  }
		  built = true;
		  return new Dimension(this);
		}
	  }

	}

}
