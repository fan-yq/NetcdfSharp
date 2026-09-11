// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.util
{

	/// <summary>
	/// static formatting utilities. Replace with standard java library when possible.
	/// 
	/// @author John Caron
	/// </summary>
	public class Format
	{

  /// <summary>
  /// Blank fill sbuff with blanks, until position tabStop.
  /// </summary>
  /// <param name="sbuff"> StringBuilder to manipulate </param>
  /// <param name="tabStop"> pad out to here </param>
  /// <param name="alwaysOne"> true if you want to guarentee at least one space. </param>
  public static void tab(StringBuilder sbuff, int tabStop, bool alwaysOne)
  {
	int len = sbuff.Length;
	if (tabStop > len)
	{
	  sbuff.Length = tabStop;
	  for (int i = len; i < tabStop; i++)
	  {
		sbuff[i] = ' ';
	  }
	}
	else if (alwaysOne)
	{
	  sbuff.Length = len + 1;
	  sbuff[len] = ' ';
	}
  }

  /// <summary>
  /// Create a new string by padding the existing one with blanks to specified width.
  /// Do nothing if length is already greater or equal to width.
  /// </summary>
  /// <param name="s"> string to pad </param>
  /// <param name="width"> length of return string </param>
  /// <returns> padded string </returns>
	  public static string s(string s, int width)
	  {
		return pad(s, width, false);
	  }

	  /// <summary>
	  /// Create a new string by padding the existing one with blanks to specified width.
	  /// Do nothing if length is already greater or equal to width.
	  /// </summary>
	  /// <param name="s"> string to pad </param>
	  /// <param name="width"> length of return string </param>
	  /// <param name="rightJustify"> if true, right justify, else left justify </param>
	  /// <returns> padded string </returns>
	  public static string pad(string s, int width, bool rightJustify)
	  {
		if (s.Length >= width)
		{
		  return s;
		}
		StringBuilder sbuff = new StringBuilder(width);
		int need = width - s.Length;
		sbuff.Length = need;
		for (int i = 0; i < need; i++)
		{
		  sbuff[i] = ' ';
		}

		if (rightJustify)
		{
		  sbuff.Append(s);
		}
		else
		{
		  sbuff.Insert(0, s);
		}

		return sbuff.ToString();
	  }

	  /// <summary>
	  /// Format an integer value.
	  /// </summary>
	  /// <param name="v"> : value </param>
	  /// <param name="width"> pad to this width </param>
	  /// <returns> formatted string </returns>
	  public static string i(int v, int width)
	  {
		return pad(Convert.ToString(v), width, true);
	  }

	  /// <summary>
	  /// Format a long value.
	  /// </summary>
	  /// <param name="v"> : value </param>
	  /// <param name="width"> pad to this width </param>
	  /// <returns> formatted string </returns>
	  public static string l(long v, int width)
	  {
		return pad(Convert.ToString(v), width, true);
	  }

	  /// <summary>
	  /// Double value formatting with minimum number of significant figures in a minimum width.
	  /// This will try to do a reasonable job of getting
	  /// a representation that has min_sigfig significant figures in minimum width.
	  /// </summary>
	  /// <param name="d"> the number to format. </param>
	  /// <param name="min_sigfig"> minimum number of significant figures </param>
	  /// <returns> string representation </returns>
	  public static string d(double d, int min_sigfig)
	  {
		return formatDouble(d, min_sigfig, -1).Trim();
	  }

	  /// <summary>
	  /// Double value formatting with minimum number of significant figures in a specified width.
	  /// This will try to do a reasonable job of getting
	  /// a representation that has min_sigfig significant figures in the specified width.
	  /// Right now, all it does is call d( double d, int min_sigfig) and left pad out to width chars.
	  /// </summary>
	  /// <param name="d"> the number to format. </param>
	  /// <param name="min_sigfig"> minimum number of significant figures </param>
	  /// <param name="width"> width of the result </param>
	  /// <returns> string representation, right justified in field of specified width </returns>
	  public static string d(double d, int min_sigfig, int width)
	  {
		string s = Format.d(d, min_sigfig);
		return pad(s, width, true);
	  }

	  /// <summary>
	  /// Double value formatting with fixed number of digits to the right of the decimal point.
	  /// </summary>
	  /// <param name="d"> the number to format. </param>
	  /// <param name="fixed_decimals"> number of digits to the right of the decimal point </param>
	  /// <returns> string representation, with specified number of decimal places </returns>
	  public static string dfrac(double d, int fixed_decimals)
	  {
		return formatDouble(d, 100, fixed_decimals).Trim();
		// String s = JavaLangCompat.DoubleToString( d);
		// s = sigfigFix( s, 100, num_dec);
		// return s.trim();
	  }

	  /*
	   * This dorks with JavaLangCompat.DoubleToString():
	   *
	   * From JavaLangCompat.DoubleToString() (m = magnitude of the number):
	   *
	   * If m is greater than or equal to 10^-3 but less than 10^7, then it is represented as the
	   * integer part of m, in decimal form with no leading zeroes, followed by '.' (.),
	   * followed by one or more decimal digits representing the fractional part of m.
	   *
	   * If m is less than 10^-3 or greater than 10^7, then it is represented in scientific notation.
	   * Let n be the unique integer such that 10n<=m<10n+1; then let a be the mathematically exact
	   * quotient of m and 10n so that 1<=a<10. The magnitude is then represented as the integer part
	   * of a, as a single decimal digit, followed by '.' (.), followed by decimal digits representing
	   * the fractional part of a, followed by the letter 'E' (E), followed by a representation of n
	   * as a decimal integer, as produced by the method JavaLangCompat.IntToString(int).
	   *
	   * How many digits must be printed for the fractional part of m or a? There must be
	   * at least one digit to represent the fractional part, and beyond that as many,
	   * but only as many, more digits as are needed to uniquely distinguish the argument
	   * value from adjacent values of type double. That is, suppose that x is the exact
	   * mathematical value represented by the decimal representation produced by this method
	   * for a finite nonzero argument d. Then d must be the double value nearest to x; or if
	   * two double values are equally close to x, then d must be one of them and the least
	   * significant bit of the significand of d must be 0.
	   */

	  /// <summary>
	  /// Format a double value
	  /// </summary>
	  /// <param name="d"> value to format </param>
	  /// <param name="min_sigFigs"> minimum significant figures </param>
	  /// <param name="fixed_decimals"> number of fixed decimals </param>
	  /// <returns> double formatted as a string </returns>
	  public static string formatDouble(double d, int min_sigFigs, int fixed_decimals)
	  {

		string s = Convert.ToString(d);

		// extract the sign
		string sign;
		string unsigned;
		if (s.StartsWith("-", StringComparison.Ordinal) || s.StartsWith("+", StringComparison.Ordinal))
		{
		  sign = s.Substring(0, 1);
		  unsigned = s.Substring(1);
		}
		else
		{
		  sign = "";
		  unsigned = s;
		}

		// deal with exponential notation
		string mantissa;
		string exponent;
		int eInd = unsigned.IndexOf('E');
		if (eInd == -1)
		{
		  eInd = unsigned.IndexOf('e');
		}
		if (eInd == -1)
		{
		  mantissa = unsigned;
		  exponent = "";
		}
		else
		{
		  mantissa = unsigned.Substring(0, eInd);
		  exponent = unsigned.Substring(eInd);
		}

		// deal with decimal point
		StringBuilder number, fraction;
		int dotInd = mantissa.IndexOf('.');
		if (dotInd == -1)
		{
		  number = new StringBuilder(mantissa);
		  fraction = new StringBuilder();
		}
		else
		{
		  number = new StringBuilder(mantissa.Substring(0, dotInd));
		  fraction = new StringBuilder(mantissa.Substring(dotInd + 1));
		}

		// number of significant figures
		int numFigs = number.Length;
		int fracFigs = fraction.Length;

		// can do either fixed_decimals or min_sigFigs
		if (fixed_decimals != -1)
		{
		  if (fixed_decimals == 0)
		  {
			fraction.Length = 0;
		  }
		  else if (fixed_decimals > fracFigs)
		  {
			int want = fixed_decimals - fracFigs;
			for (int i = 0; i < want; i++)
			{
			  fraction.Append("0");
			}
		  }
		  else if (fixed_decimals < fracFigs)
		  {
			int chop = fracFigs - fixed_decimals; // LOOK should round !!
			fraction.Length = fraction.Length - chop;
		  }

		}
		else
		{
		  // Don't count leading zeros in the fraction, if no number
		  if (((numFigs == 0) || number.ToString().Equals("0")) && (fracFigs > 0))
		  {
			numFigs = 0;
			number = new StringBuilder();
			for (int i = 0; i < fraction.Length; ++i)
			{
			  if (fraction[i] != '0')
			  {
				break;
			  }
			  --fracFigs;
			}
		  }
		  // Don't count trailing zeroes in the number if no fraction
		  if ((fracFigs == 0) && (numFigs > 0))
		  {
			for (int i = number.Length - 1; i > 0; i--)
			{
			  if (number[i] != '0')
			  {
				break;
			  }
			  --numFigs;
			}
		  }
		  // deal with min sig figures
		  int sigFigs = numFigs + fracFigs;
		  if (sigFigs > min_sigFigs)
		  {
			// Want fewer figures in the fraction; chop (should round? )
			int chop = Math.Min(sigFigs - min_sigFigs, fracFigs);
			fraction.Length = fraction.Length - chop;
		  }
		}


		/*
		 * int sigFigs = numFigs + fracFigs;
		 * if (sigFigs > max_sigFigs) {
		 * 
		 * if (numFigs >= max_sigFigs) { // enough sig figs in just the number part
		 * fraction.setLength( 0 );
		 * for ( int i=max_sigFigs; i<numFigs; ++i )
		 * number.setCharAt( i, '0' ); // should round?
		 * } else {
		 * 
		 * // Want fewer figures in the fraction; chop (should round? )
		 * int chop = sigFigs - max_sigFigs;
		 * fraction.setLength( fraction.length() - chop );
		 * }
		 * }
		 * 
		 * 
		 * /* may want a fixed decimal place
		 * if (dec_places != -1) {
		 * 
		 * if (dec_places == 0) {
		 * fraction.setLength( 0 );
		 * fracFigs = 0;
		 * } else if (dec_places > fracFigs) {
		 * int want = dec_places - fracFigs;
		 * for (int i=0; i<want; i++)
		 * fraction.append("0");
		 * } else if (dec_places < fracFigs) {
		 * int chop = fracFigs - dec_places;
		 * fraction.setLength( fraction.length() - chop );
		 * fracFigs = dec_places;
		 * }
		 * 
		 * }
		 */

		if (fraction.Length == 0)
		{
		  return sign + number + exponent;
		}
		else
		{
		  return sign + number + "." + fraction + exponent;
		}
	  }


	  /// <summary>
	  /// Nicely formatted representation of bytes, eg turn 5.636E7 into '56.3 Mbytes'.
	  /// </summary>
	  /// <param name="size"> the size in bytes </param>
	  /// <returns> formatted string </returns>
	  public static string formatByteSize(double size)
	  {
		string unit;
		if (size > 1.0e15)
		{
		  unit = "Pbytes";
		  size *= 1.0e-15;
		}
		else if (size > 1.0e12)
		{
		  unit = "Tbytes";
		  size *= 1.0e-12;
		}
		else if (size > 1.0e9)
		{
		  unit = "Gbytes";
		  size *= 1.0e-9;
		}
		else if (size > 1.0e6)
		{
		  unit = "Mbytes";
		  size *= 1.0e-6;
		}
		else if (size > 1.0e3)
		{
		  unit = "Kbytes";
		  size *= 1.0e-3;
		}
		else
		{
		  unit = "bytes";
		}

		return Format.d(size, 4) + " " + unit;
	  }

	}

}
