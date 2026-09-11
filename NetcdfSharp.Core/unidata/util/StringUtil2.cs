// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.util
{

	/// <summary>
	/// Static String and StringBuilder utilities
	/// 
	/// @author caron
	/// @since 7/29/11
	/// </summary>
	public class StringUtil2
	{

	  /// <summary>
	  /// Replace any char not alphanumeric or in allowChars by replaceChar.
	  /// </summary>
	  /// <param name="x"> operate on this string </param>
	  /// <param name="allowChars"> these are ok. </param>
	  /// <param name="replaceChar"> thar char to replace </param>
	  /// <returns> resulting string. </returns>
	  public static string allow(string x, string allowChars, char replaceChar)
	  {
		bool ok = true;
		for (int pos = 0; pos < x.Length; pos++)
		{
		  char c = x[pos];
		  if (!(char.IsLetterOrDigit(c) || (0 <= allowChars.IndexOf(c))))
		  {
			ok = false;
			break;
		  }
		}
		if (ok)
		{
		  return x;
		}

		// gotta do it
		StringBuilder sb = new StringBuilder(x);
		for (int pos = 0; pos < sb.Length; pos++)
		{
		  char c = sb[pos];
		  if (char.IsLetterOrDigit(c) || (0 <= allowChars.IndexOf(c)))
		  {
			continue;
		  }

		  sb[pos] = replaceChar;
		}

		return sb.ToString();
	  }

	  /// <summary>
	  /// Break the given text into lines, respecting word boundaries (blank space).
	  /// </summary>
	  /// <param name="text"> The text to convert </param>
	  /// <param name="insert"> break to insert </param>
	  /// <param name="lineSize"> line size to insert at </param>
	  /// <returns> The text with added br tags. </returns>
	  public static string breakTextAtWords(string text, string insert, int lineSize)
	  {
		StringBuilder buff = new StringBuilder();
		StringTokenizer stoker = new StringTokenizer(text);
		int lineCount = 0;
		while (stoker.hasMoreTokens())
		{
		  string tok = stoker.nextToken();
		  if (tok.Length + lineCount >= lineSize)
		  {
			buff.Append(insert);
			lineCount = 0;
		  }
		  buff.Append(tok);
		  buff.Append(" ");
		  lineCount += tok.Length + 1;
		}

		return buff.ToString();
	  }

	  /// <summary>
	  /// Delete any non-printable characters
	  /// </summary>
	  /// <param name="h"> byte array </param>
	  /// <returns> cleaned up string </returns>
	  public static string cleanup(byte[] h)
	  {
		byte[] bb = new byte[h.Length];
		int count = 0;
		foreach (byte b in h)
		{
		  if (b >= 32 && b < 127)
		  {
			bb[count++] = b;
		  }
		}
		return Encoding.UTF8.GetString(bb, 0, count);
	  }

	  public static string cleanup(string s)
	  {
		if (string.ReferenceEquals(s, null))
		{
		  return null;
		}
		return cleanup(Encoding.UTF8.GetBytes(s));
	  }


	  /// <summary>
	  /// Remove any char not alphanumeric or in okChars.
	  /// </summary>
	  /// <param name="x"> filter this string </param>
	  /// <param name="okChars"> these are ok. </param>
	  /// <returns> filtered string. </returns>
	  public static string filter(string x, string okChars)
	  {
		bool ok = true;
		for (int pos = 0; pos < x.Length; pos++)
		{
		  char c = x[pos];
		  if (!(char.IsLetterOrDigit(c) || (0 <= okChars.IndexOf(c))))
		  {
			ok = false;
			break;
		  }
		}
		if (ok)
		{
		  return x;
		}

		// gotta do it
		StringBuilder sb = new StringBuilder(x.Length);
		for (int pos = 0; pos < x.Length; pos++)
		{
		  char c = x[pos];
		  if (char.IsLetterOrDigit(c) || (0 <= okChars.IndexOf(c)))
		  {
			sb.Append(c);
		  }
		}

		return sb.ToString();
	  }

	  /// <summary>
	  /// Remove all but printable ascii
	  /// </summary>
	  /// <param name="s"> filter this string </param>
	  /// <returns> filtered string. </returns>
	  public static string filter7bits(string s)
	  {
		if (string.ReferenceEquals(s, null))
		{
		  return null;
		}
		char[] bo = new char[s.Length];
		int count = 0;
		for (int i = 0; i < s.Length; i++)
		{
		  char c = s[i];
		  if ((c < (char)128) && (c > (char)31) || ((c == '\n') || (c == '\t')))
		  {
			bo[count++] = c;
		  }
		}

		return new string(bo, 0, count);
	  }

	  // remove leading and trailing blanks
	  // remove control characters (< 0x20)
	  // transform "/" to "_"
	  // transform embedded space to "_"
	  public static string makeValidCdmObjectName(string name)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  return null;
		}
		name = name.Trim();
		// common case no change
		bool ok = true;
		for (int i = 0; i < name.Length; i++)
		{
		  int c = name[i];
		  if (c < 0x20)
		  {
			ok = false;
		  }
		  if (c == '/')
		  {
			ok = false;
		  }
		  if (c == ' ')
		  {
			ok = false;
		  }
		  if (!ok)
		  {
			break;
		  }
		}
		if (ok)
		{
		  return name;
		}

		StringBuilder sbuff = new StringBuilder(name.Length);
		for (int i = 0, len = name.Length; i < len; i++)
		{
		  int c = name[i];
		  if ((c == '/') || (c == ' '))
		  {
			sbuff.Append('_');
		  }
		  else if (c >= 0x20)
		  {
			sbuff.Append((char) c);
		  }
		}
		return sbuff.ToString();
	  }

	  /// <summary>
	  /// Count number of chars that match in two strings, starting from front.
	  /// </summary>
	  /// <param name="s1"> compare this string </param>
	  /// <param name="s2"> compare this string </param>
	  /// <returns> number of matching chars, starting from first char </returns>
	  public static int match(string s1, string s2)
	  {
		int i = 0;
		while ((i < s1.Length) && (i < s2.Length))
		{
		  if (s1[i] != s2[i])
		  {
			break;
		  }
		  i++;
		}
		return i;
	  }

	  /// <summary>
	  /// Left pad the given value with zeros up to the number of digits
	  /// </summary>
	  /// <param name="value"> The value. </param>
	  /// <param name="numDigits"> number of digits </param>
	  /// <returns> The String represenation of the value, padded with
	  ///         leading "0"-s if value &lt; 10E(numDigits-1) </returns>
	  public static string padZero(int value, int numDigits)
	  {
		return padLeft(value.ToString(), numDigits, "0");
	  }


	  /// <summary>
	  /// Pad the given string with spaces on the left up to the given length.
	  /// </summary>
	  /// <param name="s"> String to pad </param>
	  /// <param name="desiredLength"> ending length </param>
	  /// <returns> padded String </returns>
	  public static string padLeft(string s, int desiredLength)
	  {
		return padLeft(s, desiredLength, " ");
	  }


	  /// <summary>
	  /// Pad the given string with padString on the left up to the given length.
	  /// </summary>
	  /// <param name="s"> String to pad </param>
	  /// <param name="desiredLength"> ending length </param>
	  /// <param name="padString"> String to pad with (e.g, " ") </param>
	  /// <returns> padded String </returns>
	  public static string padLeft(string s, int desiredLength, string padString)
	  {
		while (s.Length < desiredLength)
		{
		  s = padString + s;
		}
		return s;
	  }


	  /// <summary>
	  /// Pad the given string with spaces on the right up to the given length.
	  /// </summary>
	  /// <param name="s"> String to pad </param>
	  /// <param name="desiredLength"> ending length </param>
	  /// <returns> padded String </returns>
	  public static string padRight(string s, int desiredLength)
	  {
		return padRight(s, desiredLength, " ");
	  }


	  /// <summary>
	  /// Pad the given string with padString on the right up to the given length.
	  /// </summary>
	  /// <param name="s"> String to pad </param>
	  /// <param name="desiredLength"> ending length </param>
	  /// <param name="padString"> String to pad with (e.g, " ") </param>
	  /// <returns> padded String </returns>
	  public static string padRight(string s, int desiredLength, string padString)
	  {
		StringBuilder ret = new StringBuilder(s);
		while (ret.Length < desiredLength)
		{
		  ret.Append(padString);
		}
		return ret.ToString();
	  }

	  /// <summary>
	  /// Remove all occurrences of the substring sub in the string s.
	  /// </summary>
	  /// <param name="s"> operate on this string </param>
	  /// <param name="sub"> remove all occurrences of this substring. </param>
	  /// <returns> result with substrings removed </returns>
	  public static string remove(string s, string sub)
	  {
		int len = sub.Length;
		int pos;
		while (0 <= (pos = s.IndexOf(sub, StringComparison.Ordinal)))
		{
		  s = s.Substring(0, pos) + s.Substring(pos + len);
		}
		return s;
	  }

	  /// <summary>
	  /// Remove all occurrences of the character c in the string s.
	  /// </summary>
	  /// <param name="s"> operate on this string </param>
	  /// <param name="c"> remove all occurrences of this character. </param>
	  /// <returns> result with any character c removed </returns>
	  public static string remove(string s, int c)
	  {
		if (0 > s.IndexOf((char) c))
		{ // none
		  return s;
		}

		StringBuilder buff = new StringBuilder(s);
		int i = 0;
		while (i < buff.Length)
		{
		  if (buff[i] == c)
		  {
			buff.Remove(i, 1);
		  }
		  else
		  {
			i++;
		  }
		}
		return buff.ToString();
	  }

	  /// <summary>
	  /// Remove all occurrences of the character c at the end of s.
	  /// </summary>
	  /// <param name="s"> operate on this string </param>
	  /// <param name="c"> remove all occurrences of this character that are at the end of the string. </param>
	  /// <returns> result with any character c removed </returns>
	  public static string removeFromEnd(string s, int c)
	  {
		if (0 > s.IndexOf((char) c)) // none
		{
		  return s;
		}

		int len = s.Length;
		while ((s[len - 1] == (char)c) && (len > 0))
		{
		  len--;
		}

		if (len == s.Length)
		{
		  return s;
		}
		return s.Substring(0, len);
	  }

	  public static string removeFromEnd(string s, string suffix)
	  {
		if (s.EndsWith(suffix, StringComparison.Ordinal))
		{
		  return s.Substring(0, s.Length - suffix.Length);
		}

		return s;
	  }


	  /// <summary>
	  /// Remove any whitespace (ie., Character.isWhitespace) from the input string.
	  /// </summary>
	  /// <param name="inputString"> The string to remove the whitespace. </param>
	  /// <returns> The whitespaceless result. </returns>
	  public static string removeWhitespace(string inputString)
	  {
		StringBuilder sb = new StringBuilder();
		char[] chars = inputString.ToCharArray();
		foreach (char c in chars)
		{
		  if (char.IsWhiteSpace(c))
		  {
			continue;
		  }
		  sb.Append(c);
		}
		return sb.ToString();
	  }

	  /// <summary>
	  /// Collapse continuous whitespace into one single " ".
	  /// </summary>
	  /// <param name="s"> operate on this string </param>
	  /// <returns> result with collapsed whitespace </returns>
	  public static string collapseWhitespace(string s)
	  {
		int len = s.Length;
		StringBuilder b = new StringBuilder(len);
		for (int i = 0; i < len; i++)
		{
		  char c = s[i];
		  if (!char.IsWhiteSpace(c))
		  {
			b.Append(c);
		  }
		  else
		  {
			b.Append(' ');
			while ((i + 1 < len) && char.IsWhiteSpace(s[i + 1]))
			{
			  i++; /// skip further whitespace
			}
		  }
		}
		return b.ToString();
	  }

	  /// <summary>
	  /// Replace any char "out" in s with "in".
	  /// </summary>
	  /// <param name="s"> string to replace </param>
	  /// <param name="out"> replace this character </param>
	  /// <param name="in"> with this string </param>
	  /// <returns> modified string if needed </returns>
	  public static string replace(string s, char @out, string @in)
	  {
		if (s.IndexOf(@out) < 0)
		{
		  return s;
		}

		// gotta do it
		StringBuilder sb = new StringBuilder(s);
		replace(sb, @out, @in);
		return sb.ToString();
	  }


	  /// <summary>
	  /// Replace all occurrences of any char in replaceChar with corresponding String in replaceWith
	  /// </summary>
	  /// <param name="x"> operate on this string </param>
	  /// <param name="replaceChar"> get rid of these </param>
	  /// <param name="replaceWith"> replace with these </param>
	  /// <returns> resulting string </returns>
	  public static string replace(string x, char[] replaceChar, string[] replaceWith)
	  {
		// common case no replacement
		bool ok = true;
		foreach (char aReplaceChar in replaceChar)
		{
		  int pos = x.IndexOf(aReplaceChar);
		  ok = (pos < 0);
		  if (!ok)
		  {
			break;
		  }
		}
		if (ok)
		{
		  return x;
		}

		// gotta do it
		StringBuilder sb = new StringBuilder(x);
		for (int i = 0; i < replaceChar.Length; i++)
		{
		  int pos = x.IndexOf(replaceChar[i]);
		  if (pos >= 0)
		  {
			replace(sb, replaceChar[i], replaceWith[i]);
		  }
		}

		return sb.ToString();
	  }

	  /// <summary>
	  /// Replaces all occurrences of "pattern" in "string" with "value"
	  /// </summary>
	  /// <param name="string"> string to munge </param>
	  /// <param name="pattern"> pattern to replace </param>
	  /// <param name="value"> replacement value </param>
	  /// <returns> munged string </returns>
	  public static string replace(string @string, string pattern, string value)
	  {
		if (pattern.Length == 0)
		{
		  return @string;
		}

		if (!@string.Contains(pattern))
		{
		  return @string;
		}

		// ok gotta do it
		StringBuilder returnValue = new StringBuilder();
		int patternLength = pattern.Length;
		while (true)
		{
		  int idx = @string.IndexOf(pattern, StringComparison.Ordinal);
		  if (idx < 0)
		  {
			break;
		  }

		  returnValue.Append(@string, 0, idx);
		  if (!string.ReferenceEquals(value, null))
		  {
			returnValue.Append(value);
		  }

		  @string = @string.Substring(idx + patternLength);
		}
		returnValue.Append(@string);
		return returnValue.ToString();
	  }


	  /// <summary>
	  /// Replace all occurrences of orgReplace with orgChar; inverse of replace().
	  /// </summary>
	  /// <param name="x"> operate on this string </param>
	  /// <param name="orgReplace"> get rid of these </param>
	  /// <param name="orgChar"> replace with these </param>
	  /// <returns> resulting string </returns>
	  public static string unreplace(string x, string[] orgReplace, char[] orgChar)
	  {
		// common case no replacement
		bool ok = true;
		foreach (string anOrgReplace in orgReplace)
		{
		  int pos = x.IndexOf(anOrgReplace, StringComparison.Ordinal);
		  ok = (pos < 0);
		  if (!ok)
		  {
			break;
		  }
		}
		if (ok)
		{
		  return x;
		}

		// gotta do it
		StringBuilder result = new StringBuilder(x);
		for (int i = 0; i < orgReplace.Length; i++)
		{
		  int pos = result.ToString().IndexOf(orgReplace[i]);
		  if (pos >= 0)
		  {
			unreplace(result, orgReplace[i], orgChar[i]);
		  }
		}

		return result.ToString();
	  }

	  /// <summary>
	  /// Find all occurrences of the "match" in original, and substitute the "subst" string.
	  /// </summary>
	  /// <param name="original"> starting string </param>
	  /// <param name="match"> string to match </param>
	  /// <param name="subst"> string to substitute </param>
	  /// <returns> a new string with substitutions </returns>
	  /// @deprecated use original.replace(match, subst) 
	  [Obsolete("use original.replace(match, subst)")]
	  public static string substitute(string original, string match, string subst)
	  {
		string s = original;
		int pos;
		while (0 <= (pos = s.IndexOf(match, StringComparison.Ordinal)))
		{
		  StringBuilder sb = new StringBuilder(s);
		  s = sb.Remove(pos, pos + match.Length - pos).Insert(pos, subst).ToString();
		}
		return s;
	  }

	  /// <summary>
	  /// Escape any char not alphanumeric or in okChars.
	  /// Escape by replacing char with %xx (hex).
	  /// </summary>
	  /// <param name="x"> escape this string </param>
	  /// <param name="okChars"> these are ok. </param>
	  /// <returns> equivilent escaped string. </returns>
	  public static string escape(string x, string okChars)
	  {
		StringBuilder newname = new StringBuilder();
		foreach (char c in x.ToCharArray())
		{
		  if (c == '%')
		  {
			newname.Append("%%");
		  }
		  else if (!char.IsLetterOrDigit(c) && okChars.IndexOf(c) < 0)
		  {
			newname.Append('%');
			newname.Append(Convert.ToString((0xFF & (int) c), 16));
		  }
		  else
		  {
			newname.Append(c);
		  }
		}
		return newname.ToString();
	  }

	  /// <summary>
	  /// This finds any '%xx' and converts to the equivalent char. Inverse of
	  /// escape().
	  /// </summary>
	  /// <param name="x"> operate on this String </param>
	  /// <returns> original String. </returns>
	  public static string unescape(string x)
	  {
		if (x.IndexOf('%') < 0)
		{
		  return x;
		}

		// gotta do it
		char[] b = new char[2];
		StringBuilder sb = new StringBuilder(x);
		for (int pos = 0; pos < sb.Length; pos++)
		{
		  char c = sb[pos];
		  if (c != '%')
		  {
			continue;
		  }
		  if (pos >= sb.Length - 2)
		  { // malformed - should be %xx
			return x;
		  }
		  b[0] = sb[pos + 1];
		  b[1] = sb[pos + 2];
		  int value;
		  try
		  {
			value = Convert.ToInt32(new string(b), 16);
		  }
		  catch (FormatException)
		  {
			continue; // not a hex number
		  }
		  c = (char) value;
		  sb[pos] = c;
		  sb.Remove(pos + 1, 2);
		}

		return sb.ToString();
	  }

	  /// <summary>
	  /// Split a string on one or more whitespace.
	  /// Cover for String.split, because who can remember regexp?
	  /// </summary>
	  /// <param name="source"> split this string </param>
	  /// <returns> tokens that were separated by whitespace </returns>
	  /// @deprecated use StringUtil2.split() 
	  [Obsolete("use StringUtil2.split()")]
	  public static string[] splitString(string source)
	  {
		return Regex.Split(source.Trim(), "\\s+"); // Separated by "whitespace"
	  }

	  public static IEnumerable<string> split(string source)
	  {
		return Splitter.on(CharMatcher.whitespace()).omitEmptyStrings().split(source);
	  }

	  /// <summary>
	  /// Find all occurences of match strings in original, and substitute the corresponding
	  /// subst string.
	  /// </summary>
	  /// <param name="original"> starting string </param>
	  /// <param name="match"> array of strings to match </param>
	  /// <param name="subst"> array of strings to substitute </param>
	  /// <returns> a new string with substitutions </returns>
	  public static string substitute(string original, string[] match, string[] subst)
	  {
		bool ok = true;
		foreach (string aMatch in match)
		{
		  if (original.Contains(aMatch))
		  {
			ok = false;
			break;
		  }
		}
		if (ok)
		{
		  return original;
		}

		// gotta do it;
		StringBuilder sb = new StringBuilder(original);
		for (int i = 0; i < match.Length; i++)
		{
		  substitute(sb, match[i], subst[i]);
		}
		return sb.ToString();
	  }

	  public static List<string> getTokens(string fullString, string sep)
	  {
		List<string> strs = new List<string>();
		if (sep != null)
		{
		  int sepLength = sep.Length;
		  switch (sepLength)
		  {
			case 0:
			  string[] tokens = splitString(fullString); // default to use white space if separator is ""
			  strs = new List<string>(tokens);
			  break;

			case 1:
			  StringTokenizer tokenizer = new StringTokenizer(fullString, sep); // maybe use StreamTokenizer?
			  while (tokenizer.hasMoreTokens())
			  {
				strs.Add(tokenizer.nextToken());
			  }
			  break;

			default:
			  string remainderString = fullString; // multicharacter separator
			  int location = remainderString.IndexOf(sep, StringComparison.Ordinal);
			  while (location != -1)
			  { // watch out for off-by-one errors on the string splitting indices!!!
				if (location == 0)
				{ // remainderString starts with the separator, cut it off
				  remainderString = remainderString.Substring(location + sepLength);
				  location = remainderString.IndexOf(sep, StringComparison.Ordinal);
				}
				else
				{
				  string token = remainderString.Substring(0, location); // pull the token off the front of the string
				  strs.Add(token); // add the token to our list
				  remainderString = remainderString.Substring(location + sepLength); // cut out both the token and the
																					 // separator
				  location = remainderString.IndexOf(sep, StringComparison.Ordinal);
				}
			  } // close while loop
			  if (remainderString.Length != 0)
			  {
				strs.Add(remainderString); // add the last token, post last separator
			  }
			  break;
		  } // close switch (sepLength)
		}
		else
		{ // default to white space separator if sep is null
		  string[] tokens = splitString(fullString);
		  strs = new List<string>(tokens);
		}
		if (strs.Count == 0)
		{
		  strs.Add(""); // maybe thrown an exception instead? return null?
		}
		return strs;
	  }

	  ////////////////////////////////////////////////////
	  // StringBuilder

	  /// <summary>
	  /// Remove any of the characters in out from sb
	  /// </summary>
	  /// <param name="sb"> the StringBuilder </param>
	  /// <param name="out"> get rid of any of these characters </param>
	  public static void removeAll(StringBuilder sb, string @out)
	  {
		int i = 0;
		while (i < sb.Length)
		{
		  int c = sb[i];
		  bool ok = true;
		  for (int j = 0; j < @out.Length; j++)
		  {
			if (@out[j] == c)
			{
			  sb.Remove(i, 1);
			  ok = false;
			  break;
			}
		  }
		  if (ok)
		  {
			i++;
		  }
		}
	  }

	  /// <summary>
	  /// Replace any char "out" in sb with String "in".
	  /// </summary>
	  /// <param name="sb"> StringBuilder to replace </param>
	  /// <param name="out"> repalce this character </param>
	  /// <param name="in"> with this string </param>
	  public static void replace(StringBuilder sb, char @out, string @in)
	  {
		for (int i = 0; i < sb.Length; i++)
		{
		  if (sb[i] == @out)
		  {
			sb.Remove(i, 1).Insert(i, @in);
			i += @in.Length - 1;
		  }
		}
	  }

	  /// <summary>
	  /// Replace any String "out" in sb with char "in".
	  /// </summary>
	  /// <param name="sb"> StringBuilder to replace </param>
	  /// <param name="out"> repalce this String </param>
	  /// <param name="in"> with this char </param>
	  public static void unreplace(StringBuilder sb, string @out, char @in)
	  {
		int pos;
		while (0 <= (pos = sb.ToString().IndexOf(@out, StringComparison.Ordinal)))
		{
		  sb[pos] = @in;
		  sb.Remove(pos + 1, @out.Length - 1);
		}
	  }

	  /// <summary>
	  /// Replace any of the characters from out with corresponding character from in
	  /// </summary>
	  /// <param name="sb"> the StringBuilder </param>
	  /// <param name="out"> get rid of any of these characters </param>
	  /// <param name="in"> replacing with the character at same index </param>
	  public static void replace(StringBuilder sb, string @out, string @in)
	  {
		for (int i = 0; i < sb.Length; i++)
		{
		  int c = sb[i];
		  for (int j = 0; j < @out.Length; j++)
		  {
			if (@out[j] == c)
			{
			  sb[i] = @in[j];
			}
		  }
		}
	  }

	  /// <summary>
	  /// Find all occurences of the "match" in original, and substitute the "subst" string,
	  /// directly into the original.
	  /// </summary>
	  /// <param name="sbuff"> starting string buffer </param>
	  /// <param name="match"> string to match </param>
	  /// <param name="subst"> string to substitute </param>
	  public static void substitute(StringBuilder sbuff, string match, string subst)
	  {
		int pos, fromIndex = 0;
		int substLen = subst.Length;
		int matchLen = match.Length;
		while (0 <= (pos = sbuff.ToString().IndexOf(match, fromIndex, StringComparison.Ordinal)))
		{
		  sbuff.Remove(pos, matchLen).Insert(pos, subst);
		  fromIndex = pos + substLen; // make sure dont get into an infinite loop
		}
	  }

	  /// <summary>
	  /// Remove bad char from beginning or end of string
	  /// </summary>
	  /// <param name="s"> operate on this </param>
	  /// <returns> trimmed string </returns>
	  public static string trim(string s, int bad)
	  {
		int len = s.Length;
		int st = 0;

		while ((st < len) && (s[st] == bad))
		{
		  st++;
		}
		while ((st < len) && (s[len - 1] == bad))
		{
		  len--;
		}
		return ((st > 0) || (len < s.Length)) ? s.Substring(st, len - st) : s;
	  }

	  //////////////////////////////////////////////////////////////////////

	  private static readonly char[] htmlIn = new char[] {'&', '"', '\'', '<', '>', '\n'};
	  private static readonly string[] htmlOut = new string[] {"&amp;", "&quot;", "&#39;", "&lt;", "&gt;", "\n<p>"};

	}

}
