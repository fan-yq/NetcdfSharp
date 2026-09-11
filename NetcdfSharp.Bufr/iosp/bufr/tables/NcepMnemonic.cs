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
	/*
	 * BufrRead mnemonic.java 1.0 05/09/2008
	 * 
	 * @author Robb Kambic
	 * 
	 * @version 1.0
	 */

	using Matcher = NetcdfSharp.com.google.re2j.Matcher;
	using Pattern = NetcdfSharp.com.google.re2j.Pattern;
	using NetcdfSharp.ucar.nc2.iosp.bufr;

	/// <summary>
	/// A class that reads a mnemonic table. It doesn't include X < 48 and Y < 192 type of
	/// descriptors because they are already stored in the latest WMO tables.
	/// </summary>

	public class NcepMnemonic
	{

	  // | HEADR | 362001 | TABLE D ENTRY - PROFILE COORDINATES | |
	  private static readonly Pattern fields3 = Pattern.compile("^\\|\\s+(.*)\\s+\\|\\s+(.*)\\s+\\|\\s+(.*)\\s*\\|");
	  private static readonly Pattern fields2 = Pattern.compile("^\\|\\s+(.*)\\s+\\|\\s+(.*)\\s+\\|");
	  private static readonly Pattern fields5 = Pattern.compile("^\\|\\s+(.*)\\s+\\|\\s+(.*)\\s+\\|\\s+(.*)\\s+\\|\\s+(.*)\\s+\\|\\s+(.*)\\s+\\|");

	  /// <summary>
	  /// Pattern to get 3 integers from beginning of line.
	  /// </summary>
	  private static readonly Pattern ints6 = Pattern.compile("^\\d{6}");

	  private const int XlocalCutoff = 48;
	  private const int YlocalCutoff = 192;

	  private const bool debugTable = false;

	  /// <summary>
	  /// Read NCEP mnemonic BUFR tables.
	  /// </summary>
	  /// <returns> true on success. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static boolean read(InputStream ios, BufrTables.Tables tables) throws IOException
	  public static bool read(Stream ios, BufrTables.Tables tables)
	  {
		if (ios == null)
		{
		  return false;
		}

		if (tables.b == null)
		{
		  tables.b = new TableB("fake", "fake");
		}
		if (tables.d == null)
		{
		  tables.d = new TableD("fake", "fake");
		}

		Dictionary<string, string> number = new Dictionary<string, string>(); // key = mnemonic value = fxy
		Dictionary<string, string> desc = new Dictionary<string, string>(); // key = mnemonic value = description
		Dictionary<string, string> mnseq = new Dictionary<string, string>();

		try
		{
		  StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);

		  // read mnemonic table
		  Matcher m;
		  // read header info and disregard
		  while (true)
		  {
			string line = dataIS.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  throw new Exception("Bad NCEP mnemonic BUFR table ");
			}
			if (line.Contains("MNEMONIC"))
			{
			  break;
			}
		  }
		  // read mnemonic, number, and description
		  // | HEADR | 362001 | TABLE D ENTRY - PROFILE COORDINATES |
		  while (true)
		  {
			string line = dataIS.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  break;
			}
			if (line.Contains("MNEMONIC"))
			{
			  break;
			}
			if (line.Contains("----"))
			{
			  continue;
			}
			if (line.StartsWith("*", StringComparison.Ordinal))
			{
			  continue;
			}
			if (line.StartsWith("|       ", StringComparison.Ordinal))
			{
			  continue;
			}
			m = fields3.matcher(line);
			if (m.find())
			{
			  string mnu = m.group(1).Trim();
			  string fxy = m.group(2).Trim();
			  if (fxy.StartsWith("3", StringComparison.Ordinal))
			  {
				number[mnu] = fxy;
				desc[mnu] = m.group(3).Replace("TABLE D ENTRY - ", "").Trim();
			  }
			  else if (fxy.StartsWith("0", StringComparison.Ordinal))
			  {
				number[mnu] = fxy;
				desc[mnu] = m.group(3).Replace("TABLE B ENTRY - ", "").Trim();
			  }
			  else if (fxy.StartsWith("A", StringComparison.Ordinal))
			  {
				number[mnu] = fxy;
				desc[mnu] = m.group(3).Replace("TABLE A ENTRY - ", "").Trim();
			  }
			}
			else if (debugTable)
			{
			  Console.WriteLine("bad mnemonic, number, and description: " + line);
			}
		  }
		  // read in sequences using mnemonics
		  // | ETACLS1 | HEADR {PROFILE} SURF FLUX HYDR D10M {SLYR} XTRA |
		  while (true)
		  {
			string line = dataIS.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  break;
			}
			if (line.Contains("MNEMONIC"))
			{
			  break;
			}
			if (line.Contains("----"))
			{
			  continue;
			}
			if (line.StartsWith("|       ", StringComparison.Ordinal))
			{
			  continue;
			}
			if (line.StartsWith("*", StringComparison.Ordinal))
			{
			  continue;
			}
			m = fields2.matcher(line);
			if (m.find())
			{
			  string mnu = m.group(1).Trim();
			  if (mnseq.ContainsKey(mnu))
			  { // concat lines with same mnu
				string value = mnseq[mnu];
				value = value + " " + m.group(2);
				mnseq[mnu] = value;
			  }
			  else
			  {
				mnseq[mnu] = m.group(2);
			  }
			}
			else if (debugTable)
			{
			  Console.WriteLine("bad sequence mnemonic: " + line);
			}
		  }

		  // create sequences, replacing mnemonics with numbers
		  foreach (KeyValuePair<string, string> ent in mnseq.SetOfKeyValuePairs())
		  {
			string seq = ent.Value;
			seq = seq.replaceAll("\\<", "1-1-0 0-31-0 ");
			seq = seq.replaceAll("\\>", "");
			seq = seq.replaceAll("\\{", "1-1-0 0-31-1 ");
			seq = seq.replaceAll("\\}", "");
			seq = seq.replaceAll("\\(", "1-1-0 0-31-2 ");
			seq = seq.replaceAll("\\)", "");

			StringTokenizer stoke = new StringTokenizer(seq, " ");
			IList<short> seqlist = new List<short>();
			while (stoke.hasMoreTokens())
			{
			  string mn = stoke.nextToken();
			  if (mn[1] == '-')
			  {
				seqlist.Add(Descriptor.getFxy(mn));
				continue;
			  }
			  // element descriptor needs hyphens
			  m = ints6.matcher(mn);
			  if (m.find())
			  {
				string F0 = mn.Substring(0, 1);
				string X0 = removeLeading0(mn.Substring(1, 2));
				string Y0 = removeLeading0(mn.Substring(3));
				seqlist.Add(Descriptor.getFxy(F0 + "-" + X0 + "-" + Y0));
				continue;
			  }
			  if (mn.StartsWith("\"", StringComparison.Ordinal))
			  {
				int idx = mn.LastIndexOf('"');
				string count = mn.Substring(idx + 1);
				seqlist.Add(Descriptor.getFxy("1-1-" + count));
				mn = mn.Substring(1, idx - 1);

			  }
			  if (mn.StartsWith(".", StringComparison.Ordinal))
			  {
				string des = mn.Substring(mn.Length - 4);
				mn = mn.Replace(des, "....");

			  }
			  string fxyInner = number[mn];
			  string F1 = fxyInner.Substring(0, 1);
			  string X1 = removeLeading0(fxyInner.Substring(1, 2));
			  string Y1 = removeLeading0(fxyInner.Substring(3));
			  seqlist.Add(Descriptor.getFxy(F1 + "-" + X1 + "-" + Y1));
			}

			string fxy = number[ent.Key];
			string X = removeLeading0(fxy.Substring(1, 2));
			string Y = removeLeading0(fxy.Substring(3));
			// these are in latest tables
			if (XlocalCutoff > int.Parse(X) && YlocalCutoff > int.Parse(Y))
			{
			  continue;
			}
			// key = F + "-" + X + "-" + Y;

			short seqX = short.Parse(X.Trim());
			short seqY = short.Parse(Y.Trim());

			tables.d.addDescriptor(seqX, seqY, ent.Key, seqlist);
			// short id = Descriptor.getFxy(key);
			// sequences.put(Short.valueOf(id), tableD);
		  }

		  // add some static repetition sequences
		  // LOOK why?
		  IList<short> list = new List<short>();
		  // 16 bit delayed repetition
		  list.Add(Descriptor.getFxy("1-1-0"));
		  list.Add(Descriptor.getFxy("0-31-2"));
		  tables.d.addDescriptor((short) 60, (short) 1, "", list);
		  // tableD = new DescriptorTableD("", "3-60-1", list, false);
		  // tableD.put( "3-60-1", d);
		  // short id = Descriptor.getFxy("3-60-1");
		  // sequences.put(Short.valueOf(id), tableD);

		  list = new List<short>();
		  // 8 bit delayed repetition
		  list.Add(Descriptor.getFxy("1-1-0"));
		  list.Add(Descriptor.getFxy("0-31-1"));
		  tables.d.addDescriptor((short) 60, (short) 2, "", list);
		  // tableD = new DescriptorTableD("", "3-60-2", list, false);
		  // tableD.put( "3-60-2", d);
		  // id = Descriptor.getFxy("3-60-2");
		  // sequences.put(Short.valueOf(id), tableD);

		  list = new List<short>();
		  // 8 bit delayed repetition
		  list.Add(Descriptor.getFxy("1-1-0"));
		  list.Add(Descriptor.getFxy("0-31-1"));
		  tables.d.addDescriptor((short) 60, (short) 3, "", list);
		  // tableD = new DescriptorTableD("", "3-60-3", list, false);
		  // tableD.put( "3-60-3", d);
		  // id = Descriptor.getFxy("3-60-3");
		  // sequences.put(Short.valueOf(id), tableD);

		  list = new List<short>();
		  // 1 bit delayed repetition
		  list.Add(Descriptor.getFxy("1-1-0"));
		  list.Add(Descriptor.getFxy("0-31-0"));
		  tables.d.addDescriptor((short) 60, (short) 4, "", list);
		  // tableD = new DescriptorTableD("", "3-60-4", list, false);
		  // tableD.put( "3-60-4", d);
		  // id = Descriptor.getFxy("3-60-4");
		  // sequences.put(Short.valueOf(id), tableD);

		  // add in element descriptors
		  // MNEMONIC | SCAL | REFERENCE | BIT | UNITS
		  // | FTIM | 0 | 0 | 24 | SECONDS |-------------|

		  // tableB = new TableB(tablename, tablename);

		  while (true)
		  {
			string line = dataIS.ReadLine();
			if (string.ReferenceEquals(line, null))
			{
			  break;
			}
			if (line.Contains("MNEMONIC"))
			{
			  break;
			}
			if (line.StartsWith("|       ", StringComparison.Ordinal))
			{
			  continue;
			}
			if (line.StartsWith("*", StringComparison.Ordinal))
			{
			  continue;
			}
			m = fields5.matcher(line);
			if (m.find())
			{
			  if (m.group(1).Equals(""))
			  {
				// do nothing

			  }
			  else if (number.ContainsKey(m.group(1).Trim()))
			  { // add descriptor to tableB
				string fxy = number[m.group(1).Trim()];
				string X = fxy.Substring(1, 2);
				string Y = fxy.Substring(3);
				string mnu = m.group(1).Trim();
				string descr = desc[mnu];

				short x = short.Parse(X.Trim());
				short y = short.Parse(Y.Trim());

				// these are in latest tables so skip LOOK WHY
				if (XlocalCutoff > x && YlocalCutoff > y)
				{
				  continue;
				}

				int scale = int.Parse(m.group(2).Trim());
				int refVal = int.Parse(m.group(3).Trim());
				int width = int.Parse(m.group(4).Trim());
				string units = m.group(5).Trim();

				tables.b.addDescriptor(x, y, scale, refVal, width, mnu, units, descr);

			  }
			  else if (debugTable)
			  {
				Console.WriteLine("bad element descriptors: " + line);
			  }
			}
		  }

		}
		finally
		{
		  ios.Close();
		}

		// LOOK why ?
		// default for NCEP
		// 0; 63; 0; 0; 0; 16; Numeric; Byte count
		tables.b.addDescriptor((short) 63, (short) 0, 0, 0, 16, "Byte count", "Numeric", null);

		return true;
	  }

	  private static string removeLeading0(string number)
	  {
		if (number.Length == 2 && number.StartsWith("0", StringComparison.Ordinal))
		{
		  number = number.Substring(1);
		}
		else if (number.Length == 3 && number.StartsWith("00", StringComparison.Ordinal))
		{
		  number = number.Substring(2);
		}
		else if (number.Length == 3 && number.StartsWith("0", StringComparison.Ordinal))
		{
		  number = number.Substring(1);
		}
		return number;
	  }
	}

}
