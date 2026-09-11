// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using TableB = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableB;
	using TableC = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableC;
	using TableD = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableD;

	/// <summary>
	/// Static methods to manipulate the f-x-y descriptors
	/// 
	/// @author caron
	/// @since Oct 25, 2008
	/// </summary>
	public class Descriptor
	{

	  public static string makeString(short fxy)
	  {
		int f = (fxy & 0xC000) >> 14;
		int x = (fxy & 0x3F00) >> 8;
		int y = fxy & 0xFF;
		return makeString(f, x, y);
	  }

	  public static string makeString(int f, int x, int y)
	  {
		return string.Format("{0:D}-{1:D}-{2:D}", f, x, y);
	  }

	  public static bool isWmoRange(short fxy)
	  {
		int x = (fxy & 0x3F00) >> 8;
		int y = fxy & 0xFF;
		return (x < 48 && y < 192);
	  }

	  public static short getFxy(string name)
	  {
		string[] tok = name.Split('-');
		int f = (tok.Length > 0) ? int.Parse(tok[0]) : 0;
		int x = (tok.Length > 1) ? int.Parse(tok[1]) : 0;
		int y = (tok.Length > 2) ? int.Parse(tok[2]) : 0;
		return (short)((f << 14) + (x << 8) + (y));
	  }

	  public static short getFxy2(string fxxyyy)
	  {
		int fxy = int.Parse(fxxyyy.Trim());
		int y = fxy % 1000;
		fxy /= 1000;
		int x = fxy % 100;
		int f1 = fxy / 100;
		return (short)((f1 << 14) + (x << 8) + (y));
	  }

	  // contains a BUFR table entry
	  public static bool isBufrTable(short fxy)
	  {
		int f = (fxy & 0xC000) >> 14;
		int x = (fxy & 0x3F00) >> 8;
		int y = (fxy & 0xFF);
		return (f == 0) && (x == 0) && (y < 13);
	  }

	  public static short getFxy(short f, short x, short y)
	  {
		return (short)((f << 14) + (x << 8) + (y));
	  }

	  private static readonly string[] descType = new string[] {"tableB", "replication", "tableC-operators", "tableD"};

	  public static void show(Formatter @out, short fxy, BufrTableLookup lookup)
	  {
		int f = (fxy & 0xC000) >> 14;

		if (f == 0)
		{
		  TableB.Descriptor b = lookup.getDescriptorTableB(fxy);
		  if (b == null)
		  {
			@out.format("%-8s: NOT FOUND!!", makeString(fxy));
		  }
		  else
		  {
			@out.format("%-8s: %s", b.Fxy, b.Name);
		  }

		}
		else if (f == 1)
		{
		  @out.format("%-8s: %s", makeString(fxy), descType[1]);

		}
		else if (f == 2)
		{
		  int x = (fxy & 0x3F00) >> 8;
		  @out.format("%-8s: Operator= %s", makeString(fxy), TableC.getOperatorName(x));

		}
		else if (f == 3)
		{
		  TableD.Descriptor d = lookup.getDescriptorTableD(fxy);
		  if (d == null)
		  {
			@out.format("%-8s: NOT FOUND!!", makeString(fxy));
		  }
		  else
		  {
			@out.format("%-8s: %s", d.Fxy, d.Name);
		  }
		}
	  }

	  public static string getName(short fxy, BufrTableLookup lookup)
	  {
		int f = (fxy & 0xC000) >> 14;

		if (f == 0)
		{
		  TableB.Descriptor b = lookup.getDescriptorTableB(fxy);
		  if (b == null)
		  {
			return ("**NOT FOUND!!");
		  }
		  else
		  {
			return b.Name;
		  }

		}
		else if (f == 1)
		{
		  return descType[1];

		}
		else if (f == 2)
		{
		  int x = (fxy & 0x3F00) >> 8;
		  return TableC.getOperatorName(x);

		}
		else if (f == 3)
		{
		  TableD.Descriptor d = lookup.getDescriptorTableD(fxy);
		  if (d == null)
		  {
			return "**NOT FOUND!!";
		  }
		  else
		  {
			return d.Name;
		  }
		}

		return "illegal F=" + f;
	  }
	}

}
