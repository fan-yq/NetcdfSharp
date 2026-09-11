// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

/* Copyright */
namespace NetcdfSharp.ucar.nc2.util
{
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Handle textual substitution for dataroots.
	/// This needs to be accessible to NcML, thredds catalogs, feature collecion config.
	/// Good candidate for dependency injection.
	/// </summary>
	public class AliasTranslator
	{

	  private static IDictionary<string, string> alias = new ConcurrentDictionary<string, string>();

	  public static void addAlias(string aliasKey, string actual)
	  {
		alias[aliasKey] = StringUtil2.substitute(actual, "\\", "/");
	  }

	  public static string translateAlias(string scanDir)
	  {
		foreach (KeyValuePair<string, string> entry in alias.SetOfKeyValuePairs())
		{
		  if (scanDir.StartsWith(entry.Key, StringComparison.Ordinal))
		  { // only at the front
			StringBuilder sb = new StringBuilder(scanDir);
			return sb.Remove(0, entry.Key.Length).Insert(0, entry.Value).ToString();
		  }
		}
		return scanDir;
	  }

	  public static int size()
	  {
		return alias.Count;
	  }

	}

}
