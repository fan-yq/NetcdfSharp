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
namespace NetcdfSharp.ucar.nc2.units
{
	using Matcher = NetcdfSharp.com.google.re2j.Matcher;
	using Pattern = NetcdfSharp.com.google.re2j.Pattern;

	/// <summary>
	/// Convenience routines for parsing a String to produce a Date.
	/// 
	/// @author edavis
	/// @since Nov 29, 2005 4:53:46 PM
	/// </summary>
	public class DateFromString
	{
	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(DateFromString));

	  /// <summary>
	  /// Parse the given date string (starting at the first numeric character)
	  /// using the given date format string (as described in
	  /// NetcdfSharp.java.text.SimpleDateFormat) and return a Date.
	  /// </summary>
	  /// <param name="dateString"> the String to be parsed </param>
	  /// <param name="dateFormatString"> the date format String </param>
	  /// <returns> the Date that was parsed. </returns>
	  public static DateTime getDateUsingSimpleDateFormat(string dateString, string dateFormatString)
	  {
		// Determine first numeric character in dateString and drop proceeding characters.
		int smallestIndex = dateString.Length;
		if (smallestIndex == 0)
		{
		  return default(DateTime);
		}
		for (int i = 0; i < 10; i++)
		{
		  int curIndex = dateString.IndexOf(i.ToString(), StringComparison.Ordinal);
		  if (curIndex != -1 && smallestIndex > curIndex)
		  {
			smallestIndex = curIndex;
		  }
		}

		return getDateUsingCompleteDateFormatWithOffset(dateString, dateFormatString, smallestIndex);
	  }

	  /// <summary>
	  /// Parse the given date string, starting at a position given by the offset of the demark character in the
	  /// dateFormatString.
	  /// The rest of the dateFormatString is the date format string (as described in NetcdfSharp.java.text.SimpleDateFormat).
	  /// 
	  /// <pre>
	  ///  Example:
	  ///   dateString =        wrfout_d01_2006-07-06_080000.nc
	  ///   dateFormatString = wrfout_d01_#yyyy-MM-dd_HHmm
	  /// </pre>
	  /// 
	  /// This simply counts over "wrfout_d01_" number of chars in dateString, then applies the remaining dateFormatString.
	  /// </summary>
	  /// <param name="dateString"> the String to be parsed </param>
	  /// <param name="dateFormatString"> the date format String </param>
	  /// <param name="demark"> the demarkation character </param>
	  /// <returns> the Date that was parsed. </returns>
	  public static DateTime getDateUsingDemarkatedCount(string dateString, string dateFormatString, char demark)
	  {
		// the position of the demark char is where to start parsing the dateString
		int pos1 = dateFormatString.IndexOf(demark);

		// the rest of the dateFormatString is the SimpleDateFormat
		dateFormatString = dateFormatString.Substring(pos1 + 1);

		return getDateUsingCompleteDateFormatWithOffset(dateString, dateFormatString, pos1);
	  }

	  /// <summary>
	  /// Parse the given date string (between the demarcation characters)
	  /// using the given date format string (as described in
	  /// NetcdfSharp.java.text.SimpleDateFormat) and return a Date.
	  /// 
	  /// <pre>
	  /// Example:
	  ///  dateString =  /data/anything/2006070611/wrfout_d01_2006-07-06_080000.nc
	  ///  dateFormatString =                    #wrfout_d01_#yyyy-MM-dd_HHmm
	  ///  would extract the date 2006-07-06T08:00
	  /// 
	  ///  dateString =  /data/anything/2006070611/wrfout_d01_2006-07-06_080000.nc
	  ///  dateFormatString =          yyyyMMddHH#/wrfout_d01_#
	  ///  would extract the date 2006-07-06T11:00
	  /// </pre>
	  /// </summary>
	  /// <param name="dateString"> the String to be parsed </param>
	  /// <param name="dateFormatString"> the date format String </param>
	  /// <param name="demark"> the demarkation character </param>
	  /// <returns> the Date that was parsed. </returns>
	  public static DateTime getDateUsingDemarkatedMatch(string dateString, string dateFormatString, char demark)
	  {
		// extract the match string
		int pos1 = dateFormatString.IndexOf(demark);
		int pos2 = dateFormatString.IndexOf(demark, pos1 + 1);
		if ((pos1 < 0) || (pos2 < 0))
		{
		  logger.error("Must delineate Date between 2 '#' chars, dateFormatString = '" + dateFormatString + "'", new Exception());
		  return default(DateTime);
		}
		string match = dateFormatString.Substring(pos1 + 1, pos2 - (pos1 + 1));

		int pos3 = dateString.IndexOf(match, StringComparison.Ordinal);
		if (pos3 < 0)
		{
		  return default(DateTime);
		}

		if (pos1 > 0)
		{ // pos1 > 0, date is before the match: "yyyyMMddHH#/wrfout_d01_#"
		  dateFormatString = dateFormatString.Substring(0, pos1);
					var tempVar = pos3 - dateFormatString.Length;
		  dateString = dateString.Substring(tempVar, pos3 - (tempVar));

		}
		else
		{ // pos1 == 0, date is after the match: "#wrfout_d01_#yyyy-MM-dd_HHmm"
		  dateFormatString = dateFormatString.Substring(pos2 + 1);
		  dateString = dateString.Substring(pos3 + match.Length);
		}

		// any leading or trailing "." in the dateFormatString means trim
		int posDot1 = 0;
		while (dateFormatString[posDot1] == '.')
		{
		  posDot1++;
		}
		int posDot2 = dateFormatString.Length;
		while (dateFormatString[posDot2 - 1] == '.')
		{
		  posDot2--;
		}
		if (posDot1 != 0 || posDot2 != dateFormatString.Length)
		{
		  dateFormatString = dateFormatString.Substring(posDot1, posDot2 - posDot1);
		  dateString = dateString.Substring(posDot1, posDot2 - posDot1);
		}

		return getDateUsingCompleteDateFormatWithOffset(dateString, dateFormatString, 0);
	  }

	  /// <summary>
	  /// Parse the given date string using the given date format string (as
	  /// described in NetcdfSharp.java.text.SimpleDateFormat) and return a Date.
	  /// </summary>
	  /// <param name="dateString"> the String to be parsed </param>
	  /// <param name="dateFormatString"> the date format String </param>
	  /// <returns> the Date that was parsed. </returns>
	  public static DateTime getDateUsingCompleteDateFormat(string dateString, string dateFormatString)
	  {
		return getDateUsingCompleteDateFormatWithOffset(dateString, dateFormatString, 0);
	  }

	  /// <summary>
	  /// Parse the given date string (starting at the given startIndex) using the
	  /// given date format string (as described in NetcdfSharp.java.text.SimpleDateFormat) and
	  /// return a Date. Assumes TimeZone is GMT.
	  /// </summary>
	  /// <param name="dateString"> the String to be parsed </param>
	  /// <param name="dateFormatString"> the date format String </param>
	  /// <param name="startIndex"> the index at which to start parsing the date string </param>
	  /// <returns> the Date that was parsed. </returns>
	  public static DateTime getDateUsingCompleteDateFormatWithOffset(string dateString, string dateFormatString, int startIndex)
	  {
		try
		{
		  SimpleDateFormat dateFormat = new SimpleDateFormat(dateFormatString);
		  dateFormat.setTimeZone(NetcdfSharp.java.util.JTimeZone.getTimeZone("GMT"));

		  // We have to cut off the dateString, so that it doesnt grab extra characters.
		  // eg new SimpleDateFormat("yyyyMMdd_HH").parse("20061129_06") -> 2006-12-24T00:00:00Z (WRONG!)
		  string s;
		  if (startIndex + dateFormatString.Length <= dateString.Length)
		  {
			s = dateString.Substring(startIndex, dateFormatString.Length);
		  }
		  else
		  {
			s = dateString;
		  }

		  DateTime result = dateFormat.parse(s);
		  return result;

		}
		catch (ParseException e)
		{
		  throw new Exception("SimpleDateFormat = " + dateFormatString + " fails on " + dateString + " ParseException:" + e.Message);
		}
		catch (ArgumentException e)
		{
		  throw new Exception("SimpleDateFormat = " + dateFormatString + " fails on " + dateString + " IllegalArgumentException:" + e.Message);
		}
	  }

	  /// <summary>
	  /// Use regular expression capture group replacement to construct a date
	  /// string and return the Date that is obtained by parseing the constructed
	  /// date string using the date format string "yyyy-MM-dd'T'HH:mm".
	  /// 
	  /// The date string is constructed by matching the given dateString against
	  /// the given regular expression matchPattern and then using the capture
	  /// groups from the match to replace the capture group references, i.e., "$n", where n is an
	  /// integer.
	  /// </summary>
	  /// <param name="dateString"> the String to be parsed </param>
	  /// <param name="matchPattern"> the regular expression String on which to match. </param>
	  /// <param name="substitutionPattern"> the String to use in the capture group replacement. </param>
	  /// <returns> the calculated Date </returns>
	  public static DateTime getDateUsingRegExp(string dateString, string matchPattern, string substitutionPattern)
	  {
		string dateFormatString = "yyyy-MM-dd'T'HH:mm";
		return getDateUsingRegExpAndDateFormat(dateString, matchPattern, substitutionPattern, dateFormatString);
	  }

	  /// <summary>
	  /// The same as getDateUsingRegExp() except the date format string to be used
	  /// must be specified.
	  /// </summary>
	  /// <param name="dateString"> the String to be parsed </param>
	  /// <param name="matchPattern"> the regular expression String on which to match. </param>
	  /// <param name="substitutionPattern"> the String to use in the capture group replacement. </param>
	  /// <param name="dateFormatString"> the date format string to use in the parsing of the date string. </param>
	  /// <returns> the calculated Date </returns>
	  public static DateTime getDateUsingRegExpAndDateFormat(string dateString, string matchPattern, string substitutionPattern, string dateFormatString)
	  {
		// Match the given date string against the regular expression.
		Pattern pattern = Pattern.compile(matchPattern);
		Matcher matcher = pattern.matcher(dateString);
		if (!matcher.matches())
		{
		  return default(DateTime);
		}

		// Build date string to use with date format string by
		// substituting the capture groups into the substitution pattern.
		StringBuilder dateStringFormatted = new StringBuilder();
		matcher.appendReplacement(dateStringFormatted, substitutionPattern);
		if (dateStringFormatted.Length == 0)
		{
		  return default(DateTime);
		}

		return getDateUsingCompleteDateFormat(dateStringFormatted.ToString(), dateFormatString);
	  }


	}

}
