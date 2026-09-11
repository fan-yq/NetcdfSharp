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
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// A named map from integers to Strings; a user-defined Enum used as a Variable's data type.
	/// For ENUM1, ENUM2, ENUM4 enumeration types.
	/// Immutable.
	/// 
	/// TODO EnumTypedef will not extend CDMNode in 6.
	/// TODO EnumTypedef will not have a reference to its owning Group in 6.
	/// TODO EnumTypedef.getFullName() will not exist in 6.
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class EnumTypedef extends CDMNode
	public class EnumTypedef : CDMNode
	{
	  // Constants for the unsigned max values for enum(1,2,4)
	  private const int UBYTE_MAX = 255;
	  private const int USHORT_MAX = 65535;

	  private readonly ImmutableMap<int, string> map;
	  private readonly ImmutableList<string> enumStrings;
	  private readonly DataType basetype;

	  /// <summary>
	  /// Make an EnumTypedef with base type ENUM4. </summary>
	  public EnumTypedef(string name, IDictionary<int, string> map) : this(name, map, DataType.ENUM4) // default basetype
	  {
	  }

	  /// <summary>
	  /// Make an EnumTypedef setting the base type (must be ENUM1, ENUM2, ENUM4). </summary>
	  public EnumTypedef(string name, IDictionary<int, string> map, DataType basetype) : base(name)
	  {
		Preconditions.checkArgument(validateMap(map, basetype));
		this.map = ImmutableMap<int, string>.copyOf(map);

		enumStrings = ImmutableList<string>.sortedCopyOf(map.Values);

		Debug.Assert(basetype == DataType.ENUM1 || basetype == DataType.ENUM2 || basetype == DataType.ENUM4);
		this.basetype = basetype;
	  }

	  /// @deprecated use getMap() 
	  [Obsolete("use getMap()")]
	  public virtual ImmutableList<string> EnumStrings
	  {
		  get
		  {
			return enumStrings;
		  }
	  }

	  public virtual ImmutableMap<int, string> Map
	  {
		  get
		  {
			return map;
		  }
	  }

	  /// <summary>
	  /// One of DataType.ENUM1, DataType.ENUM2, or DataType.ENUM4. </summary>
	  public virtual DataType BaseType
	  {
		  get
		  {
			return this.basetype;
		  }
	  }

	  private bool validateMap(IDictionary<int, string> map, DataType? basetype)
	  {
		if (map == null || basetype == null)
		{
		  return false;
		}
		foreach (int? i in map.Keys)
		{
		  // WARNING, we do not have signed/unsigned info available
		  switch (basetype)
		  {
			case DataType.ENUM1:
			  if (i < byte.MinValue || i.Value > UBYTE_MAX)
			  {
				return false;
			  }
			  break;
			case DataType.ENUM2:
			  if (i < short.MinValue || i.Value > USHORT_MAX)
			  {
				return false;
			  }
			  break;
			case DataType.ENUM4:
			  break; // enum4 is always ok
			default:
			  return false;
		  }
		}
		return true;
	  }

	  /// <summary>
	  /// Get the name corresponding to the enum value. </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String lookupEnumString(int e)
	  public virtual string lookupEnumString(int e)
	  {
		return map.get(e);
	  }

	  /// <summary>
	  /// Get the enum value corresponding to the name. </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public System.Nullable<int> lookupEnumInt(String name)
	  public virtual int? lookupEnumInt(string name)
	  {
		foreach (KeyValuePair<int, string> entry in map.entrySet())
		{
		  if (entry.Value.Equals(name, StringComparison.OrdinalIgnoreCase))
		  {
			return entry.Key;
		  }
		}
		return null;
	  }

	  /// <summary>
	  /// CDL string representation.
	  /// </summary>
	  /// <param name="strict"> if true, write in strict adherence to CDL definition. </param>
	  /// <returns> CDL representation. </returns>
	  /// @deprecated use CDLWriter 
	  [Obsolete("use CDLWriter")]
	  public virtual string writeCDL(bool strict)
	  {
		Formatter @out = new Formatter();
		writeCDL(@out, new Indent(2), strict);
		return @out.ToString();
	  }

	  /// @deprecated use CDLWriter 
	  [Obsolete("use CDLWriter")]
	  protected internal virtual void writeCDL(Formatter @out, Indent indent, bool strict)
	  {
		string name = strict ? NetcdfFile.makeValidCDLName(ShortName) : ShortName;
		string basetype = "";
		switch (this.basetype)
		{
		  case DataType.ENUM1:
			basetype = "byte ";
			break;
		  case DataType.ENUM2:
			basetype = "short ";
			break;
		  case DataType.ENUM4:
			basetype = "";
			break;
		  default:
			throw new System.InvalidOperationException("Unexpected basetype = " + basetype);
		}
		@out.format("%s%senum %s { ", indent, basetype, name);
		int count = 0;
		// List<Object> keyset = Arrays.asList(map.keySet().toArray());
		IList<int> keysetList = new List<int>(map.keySet());
		keysetList.Sort();
		foreach (int? key in keysetList)
		{
		  string s = map.get(key);
		  if (0 < count++)
		  {
			@out.format(", ");
		  }
		  if (strict)
		  {
			@out.format("%s = %s", NetcdfFile.makeValidCDLName(s), key);
		  }
		  else
		  {
			@out.format("'%s' = %s", s, key);
		  }
		}
		@out.format("};");
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		EnumTypedef that = (EnumTypedef) o;

		if (map == that.map)
		{
		  return true;
		}
		if (map == null)
		{
		  return false;
		}
		if (!map.Equals(that.map))
		{
		  return false;
		}
		string name = ShortName;
		string thatname = that.ShortName;
		return name == thatname;

	  }

	  // Needed for netCDF4 wierdness
	  public virtual bool equalsMapOnly(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}
		EnumTypedef that = (EnumTypedef) o;
		return NetcdfSharp.com.google.common.@base.Objects.equal(map, that.map);
	  }

	  public override int GetHashCode()
	  {
		string name = ShortName;
		int result = !string.ReferenceEquals(name, null) ? name.GetHashCode() : 0;
		result = 31 * result + (map != null ? map.GetHashCode() : 0);
		return result;
	  }

	  public override string ToString()
	  {
		Formatter f = new Formatter();
		f.format("EnumTypedef %s: ", ShortName);
		foreach (int key in map.keySet())
		{
		  f.format("%d=%s,", key, map.get(key));
		}
		return f.ToString();
	  }
	}

}
