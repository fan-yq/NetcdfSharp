// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp.bufr.tables
{
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Ncep local table overrides
	/// 
	/// @author caron
	/// @since 8/22/13
	/// </summary>
	public class NcepTable
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(NcepTable));

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static void readNcepTable(String location) throws NetcdfSharp.java.io.IOException
	  private static void readNcepTable(string location)
	  {
		using (Stream ios = BufrTables.openStream(location))
		{
		  StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
		  int count = 0;
		  while (true)
		  {
			string line = dataIS.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  break;
			}
			if (line.StartsWith("#", StringComparison.Ordinal))
			{
			  continue;
			}
			count++;

			string[] flds = line.Split(';');
			if (flds.Length < 3)
			{
			  log.warn("{} BAD split == {}", count, line);
			  continue;
			}

			int fldidx = 0;
			try
			{
			  int cat = int.Parse(flds[fldidx++].Trim());
			  int subcat = int.Parse(flds[fldidx++].Trim());
			  string desc = StringUtil2.remove(flds[fldidx++], '"');
			  entries.Add(new TableEntry(cat, subcat, desc));
			}
			catch (Exception)
			{
			  log.warn("{} {} BAD line == {}", count, fldidx, line);
			}
		  }
		}
	  }

	  private static IList<TableEntry> entries;

	  private class TableEntry
	  {
		public int cat, subcat;
		public string value;

		public TableEntry(int cat, int subcat, string value)
		{
		  this.cat = cat;
		  this.subcat = subcat;
		  this.value = value.Trim();
		}
	  }

	  private static void init()
	  {
		entries = new List<TableEntry>(100);
		string location = "resource:/resources/bufrTables/local/ncep/DataSubCategories.csv";
		try
		{
		  readNcepTable(location);
		}
		catch (IOException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
	  }


	  public static string getDataSubcategory(int cat, int subcat)
	  {
		if (entries == null)
		{
		  init();
		}

		foreach (TableEntry p in entries)
		{
		  if ((p.cat == cat) && (p.subcat == subcat))
		  {
			return p.value;
		  }
		}
		return null;
	  }


	}

}
