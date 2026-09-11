// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NetcdfSharp.java.util;
	using VariableEnhanced = NetcdfSharp.ucar.nc2.dataset.VariableDS;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

// $Id: TestCompare.java 51 2006-07-12 17:13:13Z caron $

namespace NetcdfSharp.ucar.nc2.util
{
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using NetcdfSharp.ucar.nc2.dataset;
	using NetcdfSharp.ucar.nc2;
	using NetcdfSharp.ucar.ma2;
	using Nc4 = NetcdfSharp.ucar.nc2.iosp.netcdf4.Nc4;

	/// <summary>
	/// Compare two NetcdfFile.
	/// Doesnt fail (eg doesnt use assert), places results in Formatter.
	/// 
	/// TODO will move to test classes in ver6.
	/// </summary>
	public class CompareNetcdf2
	{
	  public static readonly ObjFilter IDENTITY_FILTER = new ObjFilterAnonymousInnerClass();

	  private class ObjFilterAnonymousInnerClass : ObjFilter
	  {
		  private readonly CompareNetcdf2 outerInstance;

	  }
	  private const int MAX_SIZE_TO_READ = 10_000_000;

	  public interface ObjFilter
	  {
		// if true, compare attribute, else skip comparison
		bool attCheckOk(Variable v, Attribute att)
		{
		  return true;
		}

		// override att comparison if needed
		bool attsAreEqual(Attribute att1, Attribute att2)
		{
		  return att1.Equals(att2);
		}

		// override dimension comparison if needed
		bool enumsAreEqual(EnumTypedef enum1, EnumTypedef enum2)
		{
		  return enum1.Equals(enum2);
		}

		// if true, compare variable, else skip comparison
		bool varDataTypeCheckOk(Variable v)
		{
		  return true;
		}

		// if true, compare dimension, else skip comparison
		bool checkDimensionsForFile(string filename)
		{
		  return true;
		}

		// if true, compare dimension, else skip comparison
		bool compareCoordinateTransform(CoordinateTransform ct1, CoordinateTransform ct2)
		{
		  return ct1.Equals(ct2);
		}
	  }

	  public class Netcdf4ObjectFilter : ObjFilter
	  {

		public virtual bool attCheckOk(Variable v, Attribute att)
		{
		  // if (v != null && v.isMemberOfStructure()) return false;
		  string name = att.ShortName;

		  // added by cdm
		  if (name.Equals(CDM.CHUNK_SIZES))
		  {
			return false;
		  }
		  if (name.Equals(CDM.FILL_VALUE))
		  {
			return false;
		  }
		  if (name.Equals("_lastModified"))
		  {
			return false;
		  }

		  // hidden by nc4
		  if (name.Equals(Nc4.NETCDF4_DIMID))
		  {
			return false; // preserve the order of the dimensions
		  }
		  if (name.Equals(Nc4.NETCDF4_COORDINATES))
		  {
			return false; // ??
		  }
		  if (name.Equals(Nc4.NETCDF4_STRICT))
		  {
			return false;
		  }

		  return !name.StartsWith("_", StringComparison.Ordinal);

		  // not implemented yet
		  // if (att.getDataType().isEnum()) return false;

		}

		public virtual bool varDataTypeCheckOk(Variable v)
		{
		  if (v.DataType == DataType.CHAR)
		  {
			return false; // temp workaround
		  }
		  return v.DataType != DataType.STRING;
		}

		// override att comparision if needed
		public virtual bool attsAreEqual(Attribute att1, Attribute att2)
		{
		  if (att1.ShortName.Equals(CDM.UNITS, StringComparison.OrdinalIgnoreCase) && att2.ShortName.Equals(CDM.UNITS, StringComparison.OrdinalIgnoreCase))
		  {
			return att1.StringValue.Trim().Equals(att2.StringValue.Trim());
		  }
		  return att1.Equals(att2);
		}
	  }

	  public virtual bool enumsAreEqual(EnumTypedef enum1, EnumTypedef enum2)
	  {
		string name1 = enum1.ShortName;
		string name2 = enum2.ShortName;
		if (name1.EndsWith("_t", StringComparison.Ordinal))
		{
		  name1 = name1.Substring(0, name1.Length - 2);
		}
		if (name2.EndsWith("_t", StringComparison.Ordinal))
		{
		  name2 = name2.Substring(0, name2.Length - 2);
		}
		return object.Equals(name1, name2) && object.Equals(enum1.Map, enum2.Map) && enum1.BaseType == enum2.BaseType;
	  }

	  public static bool compareData(string name, NcArray data1, NcArray data2)
	  {
		return (new CompareNetcdf2()).compareData(name, data1, data2, false, true);
	  }

	  public static bool compareData(string name, NcArray data1, double[] data2)
	  {
		NcArray data2a = NcArray.factory(DataType.DOUBLE, new int[] {data2.Length}, data2);
		return compareData(name, data1, data2a);
	  }

	  public static bool compareFiles(NetcdfFile org, NetcdfFile copy, Formatter f)
	  {
		return compareFiles(org, copy, f, false, false, false);
	  }

	  public static bool compareFiles(NetcdfFile org, NetcdfFile copy, Formatter f, bool _compareData, bool _showCompare, bool _showEach)
	  {
		CompareNetcdf2 tc = new CompareNetcdf2(f, _showCompare, _showEach, _compareData);
		return tc.compare(org, copy);
	  }

	  public static bool compareLists(System.Collections.IList org, System.Collections.IList copy, Formatter f)
	  {
		bool ok1 = checkContains("first", org, copy, f);
		bool ok2 = checkContains("second", copy, org, f);
		return ok1 && ok2;
	  }

	  public static bool checkContains(string what, IList<object> container, IList<object> wantList, Formatter f)
	  {
		bool ok = true;

		foreach (object want1 in wantList)
		{
		  int index2 = container.IndexOf(want1);
		  if (index2 < 0)
		  {
			f.format("  ** %s missing in %s %n", want1, what);
			ok = false;
		  }
		}

		return ok;
	  }

	  /////////

	  private Formatter f;
	  private bool showCompare;
	  private bool showEach;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private bool compareData_Conflict;
	  private bool ignoreattrcase;

	  public CompareNetcdf2() : this(new Formatter())
	  {
	  }

	  public CompareNetcdf2(Formatter f) : this(f, false, false, JSystem.getProperty("allTests") != null)
	  {
	  }

	  public CompareNetcdf2(Formatter f, bool showCompare, bool showEach, bool compareData) : this(f, showCompare, showEach, compareData, true)
	  {
	  }

	  public CompareNetcdf2(Formatter f, bool showCompare, bool showEach, bool compareData, bool ignoreattrcase)
	  {
		this.f = f;
		this.compareData_Conflict = compareData;
		this.showCompare = showCompare;
		this.showEach = showEach;
		this.ignoreattrcase = ignoreattrcase;
	  }

	  public virtual bool compare(NetcdfFile org, NetcdfFile copy)
	  {
		return compare(org, copy, showCompare, showEach, compareData_Conflict);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: public boolean compare(NetcdfFile org, NetcdfFile copy, @Nullable ObjFilter filter)
	  public virtual bool compare(NetcdfFile org, NetcdfFile copy, ObjFilter filter)
	  {
		return compare(org, copy, filter, showCompare, showEach, compareData_Conflict);
	  }

	  /// @deprecated use constructor to set options, then compare(NetcdfFile org, NetcdfFile copy) 
	  [Obsolete("use constructor to set options, then compare(NetcdfFile org, NetcdfFile copy)")]
	  public virtual bool compare(NetcdfFile org, NetcdfFile copy, bool showCompare, bool showEach, bool compareData)
	  {
		return compare(org, copy, null, showCompare, showEach, compareData);
	  }

	  /// @deprecated use constructor to set options, then compare(NetcdfFile org, NetcdfFile copy, ObjFilter filter) 
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Deprecated("use constructor to set options, then compare(NetcdfFile org, NetcdfFile copy, ObjFilter filter)") public boolean compare(NetcdfFile org, NetcdfFile copy, @Nullable ObjFilter objFilter, boolean showCompare, boolean showEach, boolean compareData)
	  [Obsolete("use constructor to set options, then compare(NetcdfFile org, NetcdfFile copy, ObjFilter filter)")]
	  public virtual bool compare(NetcdfFile org, NetcdfFile copy, ObjFilter objFilter, bool showCompare, bool showEach, bool compareData)
	  {
		if (objFilter == null)
		{
		  objFilter = IDENTITY_FILTER;
		}
		this.compareData_Conflict = compareData;
		this.showCompare = showCompare;
		this.showEach = showEach;

		f.format(" First file = %s%n", org.Location);
		f.format(" Second file= %s%n", copy.Location);

		long start = DateTimeHelper.CurrentUnixTimeMillis();

		bool ok = compareGroups(org.RootGroup, copy.RootGroup, objFilter);
		f.format(" Files are the same = %s%n", ok);

		long took = DateTimeHelper.CurrentUnixTimeMillis() - start;
		f.format(" Time to compare = %d msecs%n", took);

		// coordinate systems
		if (org is NetcdfDataset && copy is NetcdfDataset)
		{
		  NetcdfDataset orgds = (NetcdfDataset) org;
		  NetcdfDataset copyds = (NetcdfDataset) copy;

		  // coordinate systems
		  foreach (CoordinateSystem cs1 in orgds.CoordinateSystems)
		  {
			CoordinateSystem cs2 = copyds.CoordinateSystems.Where(cs => cs.getName().Equals(cs1.Name)).First().orElse(null);
			if (cs2 == null)
			{
			  ok = false;
			  f.format("  ** Cant find CoordinateSystem '%s' in file2 %n", cs1.Name);
			}
			else
			{
			  ok &= compareCoordinateSystem(cs1, cs2, objFilter);
			}
		  }
		}

		return ok;
	  }

	  public virtual bool compareVariables(NetcdfFile org, NetcdfFile copy)
	  {
		f.format("Original = %s%n", org.Location);
		f.format("CompareTo= %s%n", copy.Location);
		bool ok = true;

		foreach (Variable orgV in org.Variables)
		{
		  // if (orgV.isCoordinateVariable()) continue;

		  Variable copyVar = copy.findVariable(orgV.ShortName);
		  if (copyVar == null)
		  {
			f.format(" MISSING '%s' in 2nd file%n", orgV.FullName);
			ok = false;
		  }
		  else
		  {
			ok &= compareVariables(orgV, copyVar, null, compareData_Conflict, true);
		  }
		}

		f.format("%n");
		foreach (Variable orgV in copy.Variables)
		{
		  // if (orgV.isCoordinateVariable()) continue;
		  Variable copyVar = org.findVariable(orgV.ShortName);
		  if (copyVar == null)
		  {
			f.format(" MISSING '%s' in 1st file%n", orgV.FullName);
			ok = false;
		  }
		}

		return ok;
	  }

	  private bool compareGroups(Group org, Group copy, ObjFilter filter)
	  {
		if (showCompare)
		{
		  f.format("compare Group '%s' to '%s' %n", org.ShortName, copy.ShortName);
		}
		bool ok = true;

		if (!NetcdfSharp.org.ShortName.Equals(copy.ShortName))
		{
		  f.format(" ** names are different %s != %s %n", org.ShortName, copy.ShortName);
		  ok = false;
		}

		// dimensions
		if (filter.checkDimensionsForFile(NetcdfSharp.org.NetcdfFile.Location))
		{
		  ok &= checkGroupDimensions(org, copy, "copy");
		  ok &= checkGroupDimensions(copy, org, "org");
		}

		// attributes
		ok &= checkAttributes(null, org.attributes(), copy.attributes(), filter);

		// enums
		ok &= checkEnums(org, copy, filter);

		// variables
		// cant use object equality, just match on short name
		foreach (Variable orgV in org.Variables)
		{
		  Variable copyVar = copy.findVariableLocal(orgV.ShortName);
		  if (copyVar == null)
		  {
			f.format(" ** cant find variable %s in 2nd file%n", orgV.FullName);
			ok = false;
		  }
		  else
		  {
			ok &= compareVariables(orgV, copyVar, filter, compareData_Conflict, true);
		  }
		}

		foreach (Variable copyV in copy.Variables)
		{
		  Variable orgV = org.findVariableLocal(copyV.ShortName);
		  if (orgV == null)
		  {
			f.format(" ** cant find variable %s in 1st file%n", copyV.FullName);
			ok = false;
		  }
		}

		// nested groups
		System.Collections.IList groups = new ArrayList();
		string name = org.Root ? "root group" : org.FullName;
		ok &= checkAll(name, org.Groups, copy.Groups, groups);
		for (int i = 0; i < groups.Count; i += 2)
		{
		  Group orgGroup = (Group) groups[i];
		  Group copyGroup = (Group) groups[i + 1];
		  ok &= compareGroups(orgGroup, copyGroup, filter);
		}

		return ok;
	  }


	  public virtual bool compareVariable(Variable org, Variable copy, ObjFilter filter)
	  {
		return compareVariables(org, copy, filter, compareData_Conflict, true);
	  }

	  private bool compareVariables(Variable org, Variable copy, ObjFilter filter, bool compareData, bool justOne)
	  {
		bool ok = true;

		if (showCompare)
		{
		  f.format("compare Variable %s to %s %n", org.FullName, copy.FullName);
		}
		if (!NetcdfSharp.org.FullName.Equals(copy.FullName))
		{
		  f.format(" ** names are different %s != %s %n", org.FullName, copy.FullName);
		  ok = false;
		}
		if (filter.varDataTypeCheckOk(org) && (org.DataType != copy.DataType))
		{
		  f.format(" ** %s dataTypes are different %s != %s %n", org.FullName, org.DataType, copy.DataType);
		  ok = false;
		}

		// dimensions
		ok &= checkDimensions(org.Dimensions, copy.Dimensions, copy.FullName + " copy");
		ok &= checkDimensions(copy.Dimensions, org.Dimensions, org.FullName + " org");

		// attributes
		ok &= checkAttributes(org, org.attributes(), copy.attributes(), filter);

		// data !!
		if (compareData)
		{
		  try
		  {
			ok &= compareVariableData(org, copy, showCompare, justOne);

		  }
		  catch (Exception e) when (e is IOException || e is InvalidRangeException)
		  {
			StringWriter sw = new StringWriter(5000);
			sw.Write(e.StackTrace);
			f.format("%s", sw.ToString());
			return false;
		  }
		}

		// nested variables
		if (org is Structure)
		{
		  if (!(copy is Structure))
		  {
			f.format("  ** %s not Structure%n", org);
			ok = false;

		  }
		  else
		  {
			Structure orgS = (Structure) org;
			Structure copyS = (Structure) copy;

			foreach (Variable orgV in orgS.Variables)
			{
			  Variable copyVar = copyS.findVariable(orgV.ShortName);
			  if (copyVar == null)
			  {
				f.format(" ** cant find variable %s in 2nd file%n", orgV.FullName);
				ok = false;
			  }
			  else
			  {
				bool compareStructData = compareData && !(orgV is Sequence);
				ok &= compareVariables(orgV, copyVar, filter, compareStructData, true);
			  }
			}

			/*
			 * List vars = new ArrayList();
			 * ok &= checkAll("struct " + orgS.getNameAndDimensions(), orgS.getVariables(), copyS.getVariables(), vars);
			 * for (int i = 0; i < vars.size(); i += 2) {
			 * Variable orgV = (Variable) vars.get(i);
			 * Variable ncmlV = (Variable) vars.get(i + 1);
			 * ok &= compareVariables(orgV, ncmlV, filter, false, true);
			 * }
			 */
		  }
		}

		// coordinate systems
		if (org is VariableEnhanced && copy is VariableEnhanced)
		{
		  VariableEnhanced orge = (VariableEnhanced) org;
		  VariableEnhanced copye = (VariableEnhanced) copy;

		  foreach (CoordinateSystem cs1 in orge.CoordinateSystems)
		  {
			CoordinateSystem cs2 = copye.CoordinateSystems.Where(cs => cs.getName().Equals(cs1.Name)).First().orElse(null);
			if (cs2 == null)
			{
			  ok = false;
			  f.format("  ** Cant find CoordinateSystem '%s' in file2 for var %s %n", cs1.Name, org.ShortName);
			}
			else
			{
			  ok &= compareCoordinateSystem(cs1, cs2, filter);
			}
		  }
		}

		// f.format(" Variable '%s' ok %s %n", org.getName(), ok);
		return ok;
	  }

	  private bool compareCoordinateSystem(CoordinateSystem cs1, CoordinateSystem cs2, ObjFilter filter)
	  {
		if (showCompare)
		{
		  f.format("compare CoordinateSystem '%s' to '%s' %n", cs1.Name, cs2.Name);
		}

		bool ok = true;
		foreach (CoordinateAxis ct1 in cs1.CoordinateAxes)
		{
		  CoordinateAxis ct2 = cs2.CoordinateAxes.Where(ct => ct.getFullName().Equals(ct1.FullName)).First().orElse(null);
		  if (ct2 == null)
		  {
			ok = false;
			f.format("  ** Cant find coordinateAxis %s in file2 %n", ct1.FullName);
		  }
		  else
		  {
			ok &= compareCoordinateAxis(ct1, ct2, filter);
		  }
		}

		foreach (CoordinateTransform ct1 in cs1.CoordinateTransforms)
		{
		  CoordinateTransform ct2 = cs2.CoordinateTransforms.Where(ct => filter.compareCoordinateTransform(ct1, ct)).First().orElse(null);
		  if (ct2 == null)
		  {
			ok = false;
			f.format("  ** Cant find transform %s in file2 %n", ct1.Name);
		  }
		  else
		  {
			bool ctOk = filter.compareCoordinateTransform(ct1, ct2);
			if (!ctOk)
			{
			  f.format("  ** compareCoordinateTransform failed on ct %s for cs %s %n", ct1.Name, cs1.Name);
			}
			ok = ok && ctOk;
		  }
		}

		return ok;
	  }

	  private bool compareCoordinateAxis(CoordinateAxis a1, CoordinateAxis a2, ObjFilter filter)
	  {
		if (showCompare)
		{
		  f.format("  compare CoordinateAxis '%s' to '%s' %n", a1.getShortName(), a2.getShortName());
		}

		compareVariable(a1, a2, filter);
		return true;
	  }


	  // make sure each object in wantList is contained in container, using equals().

	  // make sure each object in each list are in the other list, using equals().
	  // return an arrayList of paired objects.

	  private bool checkAttributes(Variable v, AttributeContainer list1, AttributeContainer list2, ObjFilter objFilter)
	  {
		bool ok = true;

		string name = v == null ? "global" : "variable " + v.getFullName();
		foreach (Attribute att1 in list1)
		{
		  if (objFilter.attCheckOk(v, att1))
		  {
			ok &= checkAtt(name, att1, "file1", list1, "file2", list2, objFilter);
		  }
		}

		foreach (Attribute att2 in list2)
		{
		  if (objFilter.attCheckOk(v, att2))
		  {
			ok &= checkAtt(name, att2, "file2", list2, "file1", list1, objFilter);
		  }
		}

		return ok;
	  }

	  // Theres a bug in old HDF4 (eg "MOD021KM.A2004328.1735.004.2004329164007.hdf) where dimensions
	  // are not properly moved up (eg dim BAND_250M is in both root and Data_Fields).
	  // So we are going to allow that to be ok (until proven otherwise) but we have to adjust
	  // dimension comparision. Currently Dimension.equals() checks the Group.
	  private bool checkDimensions(IList<Dimension> list1, IList<Dimension> list2, string where)
	  {
		bool ok = true;

		foreach (Dimension d1 in list1)
		{
		  if (d1.Shared)
		  {
			bool hasit = listContains(list2, d1);
			if (!hasit)
			{
			  f.format("  ** Missing Variable dim '%s' not in %s %n", d1, where);
			}
			ok &= hasit;
		  }
		}

		return ok;
	  }

	  // Check contains not using Group
	  private bool listContains(IList<Dimension> list, Dimension d2)
	  {
		foreach (Dimension d1 in list)
		{
		  if (equalInValue(d1, d2))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public Dimension findDimension(Group g, Dimension dim)
	  {
		if (dim == null)
		{
		  return null;
		}
		foreach (Dimension d in g.Dimensions)
		{
		  if (equalInValue(d, dim))
		  {
			return d;
		  }
		}
		Group parent = g.ParentGroup;
		if (parent != null)
		{
		  return findDimension(parent, dim);
		}
		return null;
	  }

	  public EnumTypedef findEnum(Group g, EnumTypedef typedef, ObjFilter filter)
	  {
		if (typedef == null)
		{
		  return null;
		}
		foreach (EnumTypedef other in g.EnumTypedefs)
		{
		  if (filter.enumsAreEqual(typedef, other))
		  {
			return other;
		  }
		}
		Group parent = g.ParentGroup;
		if (parent != null)
		{
		  return findEnum(parent, typedef, filter);
		}
		return null;
	  }

	  // values equal, not using Group
	  private bool equalInValue(Dimension d1, Dimension other)
	  {
		if ((d1.ShortName == null) && (other.ShortName != null))
		{
		  return false;
		}
		if ((d1.ShortName != null) && !d1.ShortName.Equals(other.ShortName))
		{
		  return false;
		}
		return (d1.Length == other.Length) && (d1.Unlimited == other.Unlimited) && (d1.VariableLength == other.VariableLength) && (d1.Shared == other.Shared);
	  }

	  private bool checkGroupDimensions(Group group1, Group group2, string where)
	  {
		bool ok = true;
		foreach (Dimension d1 in group1.Dimensions)
		{
		  if (d1.Shared)
		  {
			if (!group2.Dimensions.Contains(d1))
			{
			  // not in local, is it in a parent?
			  if (findDimension(group2, d1) != null)
			  {
				f.format("  ** Dimension '%s' found in parent group of %s %s%n", d1, where, group2.FullName);
			  }
			  else
			  {
				f.format("  ** Missing Group dim '%s' not in %s %s%n", d1, where, group2.FullName);
				ok = false;
			  }
			}
		  }
		}
		return ok;
	  }

	  // make sure each object in each list are in the other list, using equals().
	  // return an arrayList of paired objects.

	  private bool checkEnums(Group org, Group copy, ObjFilter filter)
	  {
		bool ok = true;

		foreach (EnumTypedef enum1 in org.EnumTypedefs)
		{
		  if (showCompare)
		  {
			f.format("compare Enum %s%n", enum1.ShortName);
		  }
		  EnumTypedef enum2 = findEnum(copy, enum1, filter);
		  if (enum2 == null)
		  {
			findEnum(org, enum1, filter);
			f.format("  ** Enum %s not in file2 %n", enum1.ShortName);
			ok = false;
			continue;
		  }
		}

		foreach (EnumTypedef enum2 in copy.EnumTypedefs)
		{
		  EnumTypedef enum1 = findEnum(org, enum2, filter);
		  if (enum1 == null)
		  {
			findEnum(org, enum2, filter);
			f.format("  ** Enum %s not in file1 %n", enum2.ShortName);
			ok = false;
		  }
		}
		return ok;
	  }

	  private bool checkAll(string what, System.Collections.IList list1, System.Collections.IList list2, System.Collections.IList result)
	  {
		bool ok = true;

		foreach (object aList1 in list1)
		{
		  ok &= checkEach(what, aList1, "file1", list1, "file2", list2, result);
		}

		foreach (object aList2 in list2)
		{
		  ok &= checkEach(what, aList2, "file2", list2, "file1", list1, null);
		}

		return ok;
	  }

	  // check that want is in both list1 and list2, using object.equals()
	  private bool checkEach(string what, object want1, string name1, System.Collections.IList list1, string name2, System.Collections.IList list2, System.Collections.IList result)
	  {
		bool ok = true;
		try
		{
		  int index2 = list2.IndexOf(want1);
		  if (index2 < 0)
		  {
			f.format("  ** %s: %s 0x%x (%s) not in %s %n", what, want1, want1.GetHashCode(), name1, name2);
			ok = false;
		  }
		  else
		  { // found it in second list
			object want2 = list2[index2];
			int index1 = list1.IndexOf(want2);
			if (index1 < 0)
			{ // can this happen ??
			  f.format("  ** %s: %s 0x%x (%s) not in %s %n", what, want2, want2.GetHashCode(), name2, name1);
			  ok = false;
			}
			else
			{ // found it in both lists
			  object want = list1[index1];
			  if (!want.Equals(want1))
			  {
				f.format("  ** %s: %s 0x%x (%s) not equal to %s 0x%x (%s) %n", what, want1, want1.GetHashCode(), name1, want2, want2.GetHashCode(), name2);
				ok = false;
			  }
			  else
			  {
				if (showEach)
				{
				  f.format("  OK <%s> equals <%s>%n", want1, want2);
				}
				if (result != null)
				{
				  result.Add(want1);
				  result.Add(want2);
				}
			  }
			}
		  }
		}
		catch (Exception t)
		{
		  Console.Write(t.StackTrace);
		  f.format(" *** Throwable= %s %n", t.Message);
		  ok = false;
		}

		return ok;
	  }

	  // check that want is in both list1 and list2, using object.equals()
	  private bool checkAtt(string what, Attribute want, string name1, AttributeContainer list1, string name2, AttributeContainer list2, ObjFilter objFilter)
	  {
		bool ok = true;
		Attribute found;
		if (this.ignoreattrcase)
		{
		  found = list2.findAttributeIgnoreCase(want.ShortName);
		}
		else
		{
		  found = list2.findAttribute(want.ShortName);
		}
		if (found == null)
		{
		  f.format("  ** %s: %s (%s) not in %s %n", what, want, name1, name2);
		  ok = false;
		}
		else
		{
		  if (!objFilter.attsAreEqual(want, found))
		  {
			f.format("  ** %s: %s 0x%x (%s) not equal to %s 0x%x (%s) %n", what, want, want.GetHashCode(), name1, found, found.GetHashCode(), name2);
			ok = false;
		  }
		  else if (showEach)
		  {
			f.format("  OK <%s> equals <%s>%n", want, found);
		  }
		}
		return ok;
	  }

	  private static int[] makeShapeSubset(int[] shape)
	  {
		// Arrays can be 1-7 dimensions so the largest one we could make this way is 10^7 elements
		const int maxLength = 10;
		int[] result = new int[shape.Length];
		for (int i = 0; i < shape.Length; i++)
		{
		  result[i] = Math.Min(maxLength, shape[i]);
		}
		return result;
	  }

	  private bool compareVariableData(Variable var1, Variable var2, bool showCompare, bool justOne)
	  {
		long size = var1.Size * var1.ElementSize;
		Section section = size > MAX_SIZE_TO_READ ? new Section(makeShapeSubset(var1.Shape)) : null;
		if (showCompare && section != null)
		{
		  f.format(" compareArrays %s too large so only comparing section %s%n", var1.getNameAndDimensions(), section);
		}
		NcArray data1 = var1.read(section);
		NcArray data2 = var2.read(section);

		if (showCompare)
		{
		  f.format(" compareArrays %s unlimited=%s size=%d%n", var1.getNameAndDimensions(), var1.Unlimited, data1.Size);
		}
		bool ok = compareData(var1.getFullName(), data1, data2, justOne);
		if (showCompare)
		{
		  f.format("   ok=%s%n", ok);
		}
		return ok;
	  }

	  public bool compareData(string name, double[] data1, double[] data2)
	  {
		NcArray data1a = NcArray.factory(DataType.DOUBLE, new int[] {data1.Length}, data1);
		NcArray data2a = NcArray.factory(DataType.DOUBLE, new int[] {data2.Length}, data2);
		return compareData(name, data1a, data2a, false, false);
	  }

	  public bool compareData(string name, NcArray data1, NcArray data2, bool justOne)
	  {
		return compareData(name, data1, data2, justOne, true);
	  }

	  private bool compareData(string name, NcArray data1, NcArray data2, bool justOne, bool testTypes)
	  {
		bool ok = true;
		if (data1.Size != data2.Size)
		{
		  f.format(" DIFF %s: data size %d !== %d%n", name, data1.Size, data2.Size);
		  ok = false;
		}

		if (testTypes && data1.ElementType != data2.ElementType)
		{
		  f.format(" DIFF %s: data element type %s !== %s%n", name, data1.ElementType, data2.ElementType);
		  ok = false;
		}

		if (testTypes && data1.DataType != data2.DataType)
		{
		  f.format(" DIFF %s: data type %s !== %s%n", name, data1.DataType, data2.DataType);
		  ok = false;
		}

		if (!Misc.compare(data1.Shape, data2.Shape, f))
		{
		  f.format(" DIFF %s: data shape %s !== %s%n", name, Misc.showInts(data1.Shape), Misc.showInts(data2.Shape));
		  ok = false;
		}

		if (!ok)
		{
		  return false;
		}

		DataType dt = data1.DataType;

		IndexIterator iter1 = data1.IndexIterator;
		IndexIterator iter2 = data2.IndexIterator;

		if (data1.isVlen())
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			object v1 = iter1.ObjectNext;
			object v2 = iter2.ObjectNext;
			if (v1.GetType() != v2.GetType())
			{
			  f.format(" DIFF %s: ArrayObject class %s != %s %n", name, v1.GetType().FullName, v2.GetType().FullName);
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
			else if (v1 is NcArray)
			{
			  ok &= compareData(name, (NcArray) v1, (NcArray) v2, justOne, testTypes);
			}
		  }
		}
		else if (dt == DataType.DOUBLE)
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			double v1 = iter1.DoubleNext;
			double v2 = iter2.DoubleNext;
			if (!Misc.nearlyEquals(v1, v2))
			{
			  f.format(createNumericDataDiffMessage(dt, name, v1, v2, iter1));
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else if (dt == DataType.FLOAT)
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			float v1 = iter1.FloatNext;
			float v2 = iter2.FloatNext;
			if (!Misc.nearlyEquals(v1, v2))
			{
			  f.format(createNumericDataDiffMessage(dt, name, v1, v2, iter1));
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else if (dt.getPrimitiveClassType() == typeof(int))
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			int v1 = iter1.IntNext;
			int v2 = iter2.IntNext;
			if (v1 != v2)
			{
			  f.format(createNumericDataDiffMessage(dt, name, v1, v2, iter1));
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else if (dt.getPrimitiveClassType() == typeof(short))
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			short v1 = iter1.ShortNext;
			short v2 = iter2.ShortNext;
			if (v1 != v2)
			{
			  f.format(createNumericDataDiffMessage(dt, name, v1, v2, iter1));
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else if (dt.getPrimitiveClassType() == typeof(byte))
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			byte v1 = iter1.ByteNext;
			byte v2 = iter2.ByteNext;
			if (v1 != v2)
			{
			  f.format(createNumericDataDiffMessage(dt, name, v1, v2, iter1));
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else if (dt.getPrimitiveClassType() == typeof(long))
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			long v1 = iter1.LongNext;
			long v2 = iter2.LongNext;
			if (v1 != v2)
			{
			  f.format(createNumericDataDiffMessage(dt, name, v1, v2, iter1));
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else if (dt.getPrimitiveClassType() == typeof(char))
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			char v1 = iter1.CharNext;
			char v2 = iter2.CharNext;
			if (v1 != v2)
			{
			  f.format(" DIFF char %s: %s != %s count=%s%n", name, v1, v2, iter1);
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else if (dt == DataType.STRING)
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			string v1 = (string) iter1.ObjectNext;
			string v2 = (string) iter2.ObjectNext;
			if (!v1.Equals(v2))
			{
			  f.format(" DIFF string %s: %s != %s count=%s%n", name, v1, v2, iter1);
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else if (dt == DataType.STRUCTURE)
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			compareStructureData((StructureData) iter1.next(), (StructureData) iter2.next(), justOne);
		  }
		}
		else if (dt == DataType.OPAQUE)
		{
		  while (iter1.hasNext() && iter2.hasNext())
		  {
			NetcdfSharp.java.nio.ByteBuffer obj1 = (NetcdfSharp.java.nio.ByteBuffer) iter1.next();
			NetcdfSharp.java.nio.ByteBuffer obj2 = (NetcdfSharp.java.nio.ByteBuffer) iter2.next();
			if (obj1.limit() != obj2.limit())
			{
			  f.format(" DIFF %s: opaque size %d != %d%n", name, obj1.limit(), obj2.limit());
			  ok = false;
			  if (justOne)
			  {
				break;
			  }
			}
		  }
		}
		else
		{
		  ok = false;
		  f.format(" %s: Unknown data type %s%n", name, data1.GetType().FullName);
		}

		return ok;
	  }

	  private string createNumericDataDiffMessage(DataType dt, string name, object v1, object v2, IndexIterator iter)
	  {
		return NetcdfSharp.java.io.FormatHelper.Format(" DIFF %s %s: %s != %s;  count = %s, absDiff = %s, relDiff = %s %n", dt, name, v1, v2, iter, Misc.absoluteDifference(Convert.ToDouble(v1), Convert.ToDouble(v2)), Misc.relativeDifference(Convert.ToDouble(v1), Convert.ToDouble(v2)));
	  }

	  private bool compareStructureData(StructureData sdata1, StructureData sdata2, bool justOne)
	  {
		bool ok = true;

		StructureMembers sm1 = sdata1.StructureMembers;
		StructureMembers sm2 = sdata2.StructureMembers;
		if (sm1.Members.size() != sm2.Members.size())
		{
		  f.format(" size %d !== %d%n", sm1.Members.size(), sm2.Members.size());
		  ok = false;
		}

		foreach (StructureMembers.Member m1 in sm1.Members)
		{
		  StructureMembers.Member m2 = sm2.findMember(m1.Name);
		  NcArray data1 = sdata1.getArray(m1);
		  NcArray data2 = sdata2.getArray(m2);
		  ok &= compareData(m1.Name, data1, data2, justOne, true);
		}

		return ok;
	  }

	}

}
