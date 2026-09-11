// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr.tables
{
	/// <summary>
	/// BUFR Table C - Data operators
	/// 
	/// @author caron
	/// @since Oct 25, 2008
	/// </summary>
	public class TableC
	{
	  private static readonly string[] tableCdesc = new string[38];

	  static TableC()
	  {
		tableCdesc[1] = "change data width";
		tableCdesc[2] = "change scale";
		tableCdesc[3] = "change reference value";
		tableCdesc[4] = "add associated field";
		tableCdesc[5] = "signify character";
		tableCdesc[6] = "signify data width for next descriptor";
		tableCdesc[7] = "increase scale, reference value, and data width";
		tableCdesc[21] = "data not present";
		tableCdesc[22] = "quality information follows";
		tableCdesc[23] = "substituted values operator";
		tableCdesc[24] = "first order statistics";
		tableCdesc[25] = "difference statistics";
		tableCdesc[32] = "replaced/retained values";
		tableCdesc[35] = "cancel backward data reference";
		tableCdesc[36] = "define data present bit-map";
		tableCdesc[37] = "use/cancel data present bit-map";
	  }

	  public static string getOperatorName(int index)
	  {
		if ((index < 0) || (index >= tableCdesc.Length))
		{
		  return "unknown";
		}
		return (string.ReferenceEquals(tableCdesc[index], null)) ? "unknown" : tableCdesc[index];
	  }

	}

}
