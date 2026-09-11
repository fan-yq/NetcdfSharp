// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
using System;
using System.Collections.Generic;
using System.Text;

/* Copyright Unidata */

namespace NetcdfSharp.ucar.nc2.util
{
	using Matcher = NetcdfSharp.com.google.re2j.Matcher;
	using Pattern = NetcdfSharp.com.google.re2j.Pattern;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;

	/*
	 * We want to refactor the escaping.
	 * 1. EscapeStrings (this class)
	 * 2. StringUtil2.escape() DONE
	 * 3. URLnaming DONE
	 * 4. Eliminate URLencode/decode in favor of guava.
	 * 
	 * https://texnoblog.wordpress.com/2014/06/11/urlencode-just-one-is-not-enough/
	 * "Starting from version 15.0, Google's excellent Guava libraries have UrlEscapers class to do just that!
	 * The documentation is pretty clear, use
	 * 1) urlPathSegmentEscaper to encode URL path segments (things that go between slashes),
	 * 2) urlFragmentEscaper if you already have a path/with/slashes
	 * 3) urlPathSegmentEscaper for the names and values of request parameters (things after the '?').
	 * 
	 * from Guava:
	 * 
	 * 1) HtmlEscapers
	 * HTML escaping is particularly tricky: For example, some elements' text contents must not be HTML escaped.
	 * As a result, it is impossible to escape an HTML document correctly without domain-specific knowledge beyond what
	 * HtmlEscapers provides.
	 * We strongly encourage the use of HTML templating systems.
	 * 
	 * 2) XMLEscaper
	 * Escaper instances suitable for strings to be included in XML attribute values and elements' text contents. When
	 * possible, avoid manual escaping
	 * by using templating systems and high-level APIs that provide autoescaping. For example, consider XOM or JDOM.
	 * 
	 * 3) PercentEscaper
	 * PercentEscaper(String safeChars, boolean plusForSpace)
	 * Constructs a percent escaper with the specified safe characters and optional handling of the space character.
	 * 
	 * TODO replace with standard escape libraries
	 */
	public class EscapeStrings
	{
	  protected internal const string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
	  protected internal const string numeric = "0123456789";
	  protected internal const string alphaNumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
	  protected internal const string _allowableInUrl = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$&'()*+,-./:;=?@_~";
	  protected internal const string _allowableInUrlQuery = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&'()*+,-./:;=?@_~";
	  protected internal const string _allowableInDAP = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_!~*'-\"./";
	  protected internal const string _allowableInOGC = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.!~*'()";
	  protected internal const char _URIEscape = '%';
	  protected internal const char _JavaEscape = '\\';
	  internal static readonly byte blank = ((byte) ' ');
	  internal static readonly byte plus = ((byte) '+');

	  /// <param name="in"> String to escape </param>
	  /// <param name="allowable"> allowedcharacters </param>
	  /// <param name="esc"> escape char prefix </param>
	  /// <param name="spaceplus"> true =>convert ' ' to '+' </param>
	  private static string xescapeString(string @in, string allowable, char esc, bool spaceplus)
	  {
		try
		{
		  StringBuilder @out = new StringBuilder();
		  if (string.ReferenceEquals(@in, null))
		  {
			return null;
		  }
		  byte[] utf8 = Encoding.UTF8.GetBytes(@in);
		  byte[] allow8 = Encoding.UTF8.GetBytes(allowable);
		  foreach (byte b in utf8)
		  {
			if (b == blank && spaceplus)
			{
			  @out.Append('+');
			}
			else
			{
			  // search allow8
			  bool found = false;
			  foreach (byte a in allow8)
			  {
				if (a == b)
				{
				  found = true;
				  break;
				}
			  }
			  if (found)
			  {
				@out.Append((char) b);
			  }
			  else
			  {
				string c = Convert.ToString(b, 16);
				@out.Append(esc);
				if (c.Length < 2)
				{
				  @out.Append('0');
				}
				@out.Append(c);
			  }
			}
		  }

		  return @out.ToString();

		}
		catch (Exception)
		{
		  return @in;
		}
	  }

	  private static string escapeString(string @in, string allowable)
	  {
		return xescapeString(@in, allowable, _URIEscape, false);
	  }

	  /// <summary>
	  /// Given a string that contains WWW escape sequences, translate those escape sequences back into
	  /// ASCII characters. Return the modified string.
	  /// </summary>
	  /// <param name="in"> The string to modify. </param>
	  /// <param name="escape"> The character used to signal the begining of an escape sequence. param except If
	  ///        there is some escape code that should not be removed by this call (e.g., you might not want to
	  ///        remove spaces, %20) use this parameter to specify that code. The function will then transform
	  ///        all escapes except that one. </param>
	  /// <param name="spaceplus"> True if spaces should be replaced by '+'. </param>
	  /// <returns> The modified string. </returns>

	  private static string xunescapeString(string @in, char escape, bool spaceplus)
	  {
		try
		{
		  if (string.ReferenceEquals(@in, null))
		  {
			return null;
		  }

		  byte[] utf8 = Encoding.UTF8.GetBytes(@in);
		  byte escape8 = (byte) escape;
		  byte[] @out = new byte[utf8.Length]; // Should be max we need

		  int index8 = 0;
		  for (int i = 0; i < utf8.Length;)
		  {
			byte b = utf8[i++];
			if (b == plus && spaceplus)
			{
			  @out[index8++] = blank;
			}
			else if (b == escape8)
			{
			  // check to see if there are enough characters left
			  if (i + 2 <= utf8.Length)
			  {
				b = (byte)(fromHex(utf8[i]) << 4 | fromHex(utf8[i + 1]));
				i += 2;
			  }
			}
			@out[index8++] = b;
		  }
		  return StringHelper.NewString(@out, 0, index8, Encoding.UTF8);
		}
		catch (Exception)
		{
		  return @in;
		}

	  }

	  private static string unescapeString(string @in)
	  {
		return xunescapeString(@in, _URIEscape, false);
	  }

	  private static readonly byte hexa = (byte) 'a';
	  private static readonly byte hexf = (byte) 'f';
	  private static readonly byte hexA = (byte) 'A';
	  private static readonly byte hexF = (byte) 'F';
	  private static readonly byte hex0 = (byte) '0';
	  private static readonly byte hex9 = (byte) '9';
	  private static readonly byte ten = (byte) 10;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static byte fromHex(byte b) throws NumberFormatException
	  private static byte fromHex(byte b)
	  {
		if (b >= hex0 && b <= hex9)
		{
		  return (byte)(b - hex0);
		}
		if (b >= hexa && b <= hexf)
		{
		  return (byte)(ten + (b - hexa));
		}
		if (b >= hexA && b <= hexF)
		{
		  return (byte)(ten + (b - hexA));
		}
		throw new System.FormatException("Illegal hex character: " + b);
	  }


	  /// <summary>
	  /// Define the DEFINITIVE opendap identifier unescape function.
	  /// </summary>
	  /// <param name="id"> The identifier to unescape. </param>
	  /// <returns> The unescaped identifier. </returns>
	  public static string unescapeDAPIdentifier(string id)
	  {
		string s;
		try
		{
		  s = unescapeString(id);
		}
		catch (Exception)
		{
		  s = null;
		}
		return s;
	  }

	  private static readonly Pattern p = Pattern.compile("([\\w]+)://([.\\w]+(:[\\d]+)?)([/][^?#])?([?][^#]*)?([#].*)?");


	  /// <summary>
	  /// Define the DEFINITIVE URL constraint expression escape function.
	  /// </summary>
	  /// <param name="ce"> The expression to modify. </param>
	  /// <returns> The escaped expression. </returns>
	  public static string escapeURLQuery(string ce)
	  {
		try
		{
		  ce = escapeString(ce, _allowableInUrlQuery);
		}
		catch (Exception)
		{
		  ce = null;
		}
		return ce;
	  }

	  /// <summary>
	  /// Define the DEFINITIVE URL constraint expression unescape function.
	  /// </summary>
	  /// <param name="ce"> The expression to unescape. </param>
	  /// <returns> The unescaped expression. </returns>
	  public static string unescapeURLQuery(string ce)
	  {
		try
		{
		  ce = unescapeString(ce);
		}
		catch (Exception)
		{
		  ce = null;
		}
		return ce;
	  }

	  /// <summary>
	  /// Define the DEFINITIVE URL escape function. Note that the whole string is escaped, so be careful
	  /// what you pass into this procedure.
	  /// </summary>
	  /// <param name="s"> The string to modify. </param>
	  /// <returns> The escaped expression. </returns>
	  private static string urlEncode(string s)
	  {
		// try {s = URLEncoder.encode(s,"UTF-8");} catch(Exception e) {s = null;}
		s = escapeString(s, _allowableInUrl);
		return s;
	  }

	  /// <summary>
	  /// Define the DEFINITIVE URL unescape function.
	  /// </summary>
	  /// <param name="s"> The string to unescape. </param>
	  /// <returns> The unescaped expression. </returns>
	  public static string urlDecode(string s)
	  {
		try
		{
		  // s = unescapeString(s, _URIEscape, "", false);
		  s = NetcdfSharp.java.net.URLDecoder.decode(s, CDM.UTF8);
		}
		catch (Exception)
		{
		  s = null;
		}
		return s;
	  }

	  /// <summary>
	  /// Decode all of the parts of the url including query and fragment
	  /// </summary>
	  /// <param name="url"> the url to encode </param>
	  public static string unescapeURL(string url)
	  {
		string newurl;
		newurl = urlDecode(url);
		return newurl;
	  }


	  /// <summary>
	  /// Define the DAP escape identifier function.
	  /// </summary>
	  /// <param name="s"> The string to encode. </param>
	  /// <returns> The escaped string. </returns>
	  public static string escapeDAPIdentifier(string s)
	  {
		return escapeString(s, _allowableInDAP);
	  }

	  /// <summary>
	  /// Define the OGC Web Services escape function.
	  /// </summary>
	  /// <param name="s"> The string to encode. </param>
	  /// <returns> The escaped string. </returns>
	  public static string escapeOGC(string s)
	  {
		return escapeString(s, _allowableInOGC);
	  }

	  public static void testOGC()
	  {
		foreach (char c in (alphaNumeric + " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~").ToCharArray())
		{
		  string encoded = EscapeStrings.escapeOGC("" + c);
		  JSystem.err.printf("|%c|=|%s|%n", c, encoded);
		}
	  }

	  ///////////////////////////////////////////////////////////////

	  /// <summary>
	  /// backslash escape a string
	  /// </summary>
	  /// <param name="x"> escape this; may be null </param>
	  /// <param name="reservedChars"> these chars get a backslash in front of them </param>
	  /// <returns> escaped string </returns>
	  public static string backslashEscape(string x, string reservedChars)
	  {
		if (string.ReferenceEquals(x, null))
		{
		  return null;
		}
		else if (string.ReferenceEquals(reservedChars, null))
		{
		  return x;
		}

		bool ok = true;
		for (int pos = 0; pos < x.Length; pos++)
		{
		  char c = x[pos];
		  if (reservedChars.IndexOf(c) >= 0)
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
		  if (reservedChars.IndexOf(c) < 0)
		  {
			continue;
		  }

		  sb[pos] = '\\';
		  pos++;
		  sb.Insert(pos, c);
		  pos++;
		}

		return sb.ToString();
	  }

	  /// <summary>
	  /// backslash unescape a string
	  /// </summary>
	  /// <param name="x"> unescape this </param>
	  /// <returns> string with \c -> c </returns>
	  public static string backslashUnescape(string x)
	  {
		if (!x.Contains("\\"))
		{
		  return x;
		}

		// gotta do it
		StringBuilder sb = new StringBuilder(x.Length);
		for (int pos = 0; pos < x.Length; pos++)
		{
		  char c = x[pos];
		  if (c == '\\')
		  {
			c = x[++pos]; // skip backslash, get next cha
		  }
		  sb.Append(c);
		}

		return sb.ToString();
	  }


	  /// <summary>
	  /// Tokenize an escaped name using "." as delimiter, skipping "\."
	  /// </summary>
	  /// <param name="escapedName"> an escaped name </param>
	  /// <returns> list of tokens </returns>
	  public static IList<string> tokenizeEscapedName(string escapedName)
	  {
		IList<string> result = new List<string>();
		int pos = 0;
		int start = 0;
		while (true)
		{
		  pos = escapedName.IndexOf(sep, pos + 1);
		  if (pos <= 0)
		  {
			break;
		  }
		  if (escapedName[pos - 1] != '\\')
		  {
			result.Add(escapedName.Substring(start, pos - start));
			start = pos + 1;
		  }
		}
		result.Add(escapedName.Substring(start)); // remaining
		return result;
	  }

	  private const char sep = '.';

	  /// <summary>
	  /// Find first occurence of char c in escapedName, excluding escaped c.
	  /// </summary>
	  /// <param name="escapedName"> search in this string </param>
	  /// <param name="c"> for this char but not \\cha </param>
	  /// <returns> pos in string, or -1 </returns>
	  public static int indexOf(string escapedName, char c)
	  {
		int pos = 0;
		while (true)
		{
		  pos = escapedName.IndexOf(c, pos + 1);
		  if (pos <= 0)
		  {
			return pos;
		  }
		  if (escapedName[pos - 1] != '\\')
		  {
			return pos;
		  }
		}
	  }

	  // Some handy definitons.
	  private const string xmlGT = "&gt;";
	  private const string xmlLT = "&lt;";
	  private const string xmlAmp = "&amp;";
	  private const string xmlApos = "&apos;";
	  private const string xmlQuote = "&quot;";


	  /// <summary>
	  /// Given a backslash escaped name, convert to a DAP escaped name
	  /// </summary>
	  /// <param name="bs"> the string to DAP encode; may have backslash escapes </param>
	  /// <returns> escaped string </returns>
	  public static string backslashToDAP(string bs)
	  {
		StringBuilder buf = new StringBuilder();
		int len = bs.Length;
		for (int i = 0; i < len; i++)
		{
		  char c = bs[i];
		  if (i < (len - 1) && c == '\\')
		  {
			c = bs[++i];
		  }
		  if (_allowableInDAP.IndexOf(c) < 0)
		  {
			buf.Append(_URIEscape);
			// convert the char to hex
			string ashex = Convert.ToString((int) c, 16);
			if (ashex.Length < 2)
			{
			  buf.Append('0');
			}
			buf.Append(ashex);
		  }
		  else
		  {
			buf.Append(c);
		  }
		}
		return buf.ToString();
	  }

	  /// <summary>
	  /// Given a DAP (attribute) string, insert backslashes before '"' and '/' characters. This code
	  /// also escapes control characters, although the spec does not call for it; make that code
	  /// conditional.
	  /// </summary>
	  public static string backslashEscapeDapString(string s)
	  {
		StringBuilder buf = new StringBuilder();
		for (int i = 0; i < s.Length; i++)
		{
		  int c = s[i];
		  if (true)
		  {
			if (c < ' ')
			{
			  switch (c)
			  {
				case '\n':
				case '\r':
				case '\t':
				case '\f':
				  buf.Append((char) c);
				  break;
				default:
				  buf.Append(string.Format("\\x{0:x2}", (c & 0xff)));
				  break;
			  }
			  continue;
			}
		  }
		  if (c == '"')
		  {
			buf.Append("\\\"");
		  }
		  else if (c == '\\')
		  {
			buf.Append("\\\\");
		  }
		  else
		  {
			buf.Append((char) c);
		  }
		}
		return buf.ToString();
	  }

	  /// <summary>
	  /// Given a CDM string, insert backslashes before <toescape> characters.
	  /// </summary>
	  public static string backslashEscapeCDMString(string s, string toescape)
	  {
		if (string.ReferenceEquals(toescape, null) || toescape.Length == 0)
		{
		  return s;
		}
		if (string.ReferenceEquals(s, null) || s.Length == 0)
		{
		  return s;
		}
		StringBuilder buf = new StringBuilder();
		for (int i = 0; i < s.Length; i++)
		{
		  int c = s[i];
		  if (toescape.IndexOf((char) c) >= 0)
		  {
			buf.Append('\\');
		  }
		  buf.Append((char) c);
		}
		return buf.ToString();
	  }

	}

}
