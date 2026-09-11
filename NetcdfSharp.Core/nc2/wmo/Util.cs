// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.wmo
{
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Uitlities common to WMO table parsing.
	/// </summary>
	/// @deprecated will be moved in ver6 
	[Obsolete("will be moved in ver6")]
	public class Util
	{

	  /// <summary>
	  /// Clean up strings to be used for unit string
	  /// </summary>
	  /// <param name="unit"> original </param>
	  /// <returns> cleaned up </returns>
	  public static string cleanUnit(string unit)
	  {
		if (string.ReferenceEquals(unit, null))
		{
		  return null;
		}
		// These specific words become dimensionless
		if (unit.Equals("Proportion", StringComparison.OrdinalIgnoreCase) || unit.Equals("Numeric", StringComparison.OrdinalIgnoreCase))
		{
		  unit = "";
		}
		// So does '-'
		else if (unit.Equals("-", StringComparison.OrdinalIgnoreCase))
		{
		  unit = "";
		  // Make sure degree(s) true gets concatenated with '_'
		}
		else if (unit.StartsWith("degree", StringComparison.Ordinal) && unit.EndsWith("true", StringComparison.Ordinal))
		{
		  unit = unit.Replace(' ', '_');
		  // And only do the rest of the conversion if it's not a "* table *" entry
		}
		else if (!unit.Contains(" table "))
		{
		  if (unit.StartsWith("/", StringComparison.Ordinal))
		  {
			unit = "1" + unit;
		  }
		  unit = unit.Trim();
		  unit = StringUtil2.remove(unit, "**");
		  StringBuilder sb = new StringBuilder(unit);
		  StringUtil2.removeAll(sb, "^[]");
		  StringUtil2.replace(sb, ' ', ".");
		  StringUtil2.replace(sb, '*', ".");
		  unit = sb.ToString();
		}
		return unit;
	  }

	  /// <summary>
	  /// Clean up strings to be used in Netcdf Object names
	  /// </summary>
	  /// <param name="name"> original name </param>
	  /// <returns> cleaned up name </returns>
	  public static string cleanName(string name)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  return null;
		}
		int pos = name.IndexOf("(see", StringComparison.Ordinal);
		if (pos < 0)
		{
		  pos = name.IndexOf("(See", StringComparison.Ordinal);
		}
		if (pos > 0)
		{
		  name = name.Substring(0, pos);
		}

		name = StringUtil2.replace(name, '/', "-");
		StringBuilder sb = new StringBuilder(name);
		StringUtil2.replace(sb, '+', "plus");
		StringUtil2.removeAll(sb, ".;,=[]()/*\"");
		return StringUtil2.collapseWhitespace(sb.ToString().Trim());
	  }

	  public static string cleanupDescription(string desc)
	  {
		if (string.ReferenceEquals(desc, null))
		{
		  return null;
		}
		int pos = desc.IndexOf("(see", StringComparison.Ordinal);
		if (pos > 0)
		{
		  desc = desc.Substring(0, pos);
		}

		StringBuilder sb = new StringBuilder(desc.Trim());
		StringUtil2.removeAll(sb, ".;,=[]()/*");
		return sb.ToString().Trim();
	  }

	  /// <summary>
	  /// Compare two names from tables, trying to ignore superfulous characters.
	  /// </summary>
	  /// <returns> true if these are equivilent </returns>
	  public static bool equivilantName(string name1, string name2)
	  {
		if (string.ReferenceEquals(name1, null) || string.ReferenceEquals(name2, null))
		{
		  return (string.ReferenceEquals(name1, name2));
		}
		string name1clean = cleanName(name1).ToLower();
		string name2clean = cleanName(name2).ToLower();
		if (name1.Equals(name2))
		{
		  return true;
		}

		StringBuilder sb1 = new StringBuilder(name1clean);
		StringUtil2.removeAll(sb1, " -’'");
		StringBuilder sb2 = new StringBuilder(name2clean);
		StringUtil2.removeAll(sb2, " -’'");
		return sb1.ToString().Equals(sb2.ToString());
	  }

	  /// <summary>
	  /// The given unit is "unitless". </summary>
	  public static bool isUnitless(string unit)
	  {
		if (string.ReferenceEquals(unit, null))
		{
		  return true;
		}
		string munge = unit.ToLower().Trim();
		munge = StringUtil2.remove(munge, '(');
		return munge.Length == 0 || munge.StartsWith("numeric", StringComparison.Ordinal) || munge.StartsWith("non-dim", StringComparison.Ordinal) || munge.StartsWith("see", StringComparison.Ordinal) || munge.StartsWith("proportion", StringComparison.Ordinal) || munge.StartsWith("code", StringComparison.Ordinal) || munge.StartsWith("0=", StringComparison.Ordinal) || munge.Equals("1");
	  }

	}

}
