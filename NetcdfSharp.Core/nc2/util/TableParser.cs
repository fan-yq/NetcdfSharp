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
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.util
{
#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Utility class to read and parse a fixed length table.
	/// Each line of the table becomes a "Record". Each Record has a set of Fields described by the format string.
	/// <p/>
	/// 
	/// <pre>
	/// List<TableParser.Record> recs = TableParser.readTable(is, "3,15,46,54,60d,67d,73d", 50000);
	/// for (TableParser.Record record : recs) {
	/// Station s = new Station();
	/// s.id = "K" + record.get(0);
	/// s.name = record.get(2) + " " + record.get(3);
	/// s.lat = (Double) record.get(4) * .01;
	/// s.lon = (Double) record.get(5) * .01;
	/// s.elev = (Double) record.get(6);
	/// <p/>
	/// stationTableHash.put(s.id, s);
	/// if (showStations) System.out.println(" station= " + s);
	/// }
	/// <p/>
	/// Example Table:
	/// TLX      000001 OKLAHOMA_CITY/Norman             OK US  3532  -9727   370  0 NWS
	/// AMA      000313 AMARILLO/Amarillo                TX US  3523 -10170  1093  0 NWS
	/// HGX      000378 HOUSTON/GALVESTON/Dickinson      TX US  2947  -9507     5  0 NWS
	/// MLB      000302 MELBOURNE/Melbourne              FL US  2810  -8065    11  0 NWS
	/// <p/>
	/// format:
	/// "3,15,54,60d,67d,73d"
	/// <p/>
	/// grammer:
	/// format = {field,}
	/// field = endPos type
	/// endPos = ending pos in the line, 0 based, exclusive, ie [start, end)
	/// type = i=integer, d=double, L=long else String
	/// field[0] goes from [0, endPos[0])
	/// field[i] goes from [endPos[i-1] to endPos[i])
	/// <p/>
	/// </pre>
	/// </summary>
	/// @deprecated move in ver6 
	[Obsolete("move in ver6")]
	public class TableParser
	{
	  private static Logger logger = LoggerFactory.getLogger(typeof(TableParser));
	  private const bool debug = false;

	  /// <summary>
	  /// Reads a URL or file in as a table.
	  /// </summary>
	  /// <param name="urlString"> starts with http, read URL contenets, else read file. </param>
	  /// <param name="format"> describe format of each line. </param>
	  /// <param name="maxLines"> maximum number of lines to parse, set to < 0 to read all </param>
	  /// <returns> List of TableParser.Record </returns>
	  /// <exception cref="IOException"> on read error </exception>
	  /// <exception cref="NumberFormatException"> on parse number error </exception>
	  /// <seealso cref=".readTable(InputStream ios, String format, int maxLines)"/>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static List<Record> readTable(String urlString, String format, int maxLines) throws IOException, NumberFormatException
	  public static IList<Record> readTable(string urlString, string format, int maxLines)
	  {

		Stream ios;
		if (urlString.StartsWith("http:", StringComparison.Ordinal))
		{
		  URL url = new URL(urlString);
		  ios = url.openStream();
		}
		else
		{
		  ios = new FileStream(urlString, FileMode.Open, FileAccess.Read);
		}

		return readTable(ios, format, maxLines);
	  }

	  /// <summary>
	  /// Reads an input stream, containing lines of ascii in fixed width format.
	  /// Breaks each line into a set of Fields (space or comma delimited) which may be String, integer or double.
	  /// </summary>
	  /// <param name="ios"> the input stream, will be closed </param>
	  /// <param name="format"> describe format of each line. </param>
	  /// <param name="maxLines"> maximum number of lines to parse, set to < 0 to read all </param>
	  /// <returns> List of TableParser.Record </returns>
	  /// <exception cref="IOException"> on read error </exception>
	  /// <exception cref="NumberFormatException"> on parse number error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static List<Record> readTable(InputStream ios, String format, int maxLines) throws IOException, NumberFormatException
	  public static IList<Record> readTable(Stream ios, string format, int maxLines)
	  {
		IList<Record> result;
		try
		{
		  TableParser parser = new TableParser(format);
		  result = parser.readAllRecords(ios, maxLines);
		}
		finally
		{
		  ios.Close();
		}
		return result;
	  }


	  /////////////////////////////////////////////////////////////////////////////////////////////////

	  private IList<Field> fields = new List<Field>();

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public TableParser(String format) throws NumberFormatException
	  public TableParser(string format)
	  {
		int start = 0;
		StringTokenizer stoker = new StringTokenizer(format, " ,");
		while (stoker.hasMoreTokens())
		{
		  string tok = stoker.nextToken();
		  // see what type
		  Type type = typeof(string);
		  char last = tok[tok.Length - 1];
		  if (last == 'i')
		  {
			type = typeof(int);
		  }
		  if (last == 'd')
		  {
			type = typeof(double);
		  }
		  if (last == 'L')
		  {
			type = typeof(long);
		  }
		  if (type != typeof(string))
		  {
			tok = tok.Substring(0, tok.Length - 1);
		  }

		  int end = int.Parse(tok);
		  fields.Add(new Field(start, end, type));
		  start = end;
		}
	  }

	  private string comment = "#";

	  public virtual string Comment
	  {
		  set
		  {
			this.comment = value;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public List<Record> readAllRecords(InputStream ios, int maxLines) throws IOException, NumberFormatException
	  public virtual IList<Record> readAllRecords(Stream ios, int maxLines)
	  {

		IList<Record> records = new List<Record>();

		StreamReader dataIS = new StreamReader(ios, Encoding.UTF8);
		int count = 0;
		while ((maxLines < 0) || (count < maxLines))
		{
		  string line = dataIS.ReadLine();
		  if (string.ReferenceEquals(line, null))
		  {
			break;
		  }
		  if (line.StartsWith(comment, StringComparison.Ordinal))
		  {
			continue;
		  }
		  if (line.Trim().Length == 0)
		  {
			continue;
		  }
		  if (debug)
		  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			Console.Write("{0}%n", line);
		  }
		  Record r = Record.make(line, (System.Collections.IList)fields);
		  if (r != null)
		  {
			records.Add(r);
		  }
		  count++;
		}

		return records;
	  }

	  public virtual Field getField(int fldno)
	  {
		return fields[fldno];
	  }

	  public virtual int NumberOfFields
	  {
		  get
		  {
			return fields.Count;
		  }
	  }

	  /// @deprecated will move in ver6. 
	  public class Field
	  {
		internal int start, end;
		internal Type type;

		internal bool hasScale;
		internal float scale;

		internal Field(int start, int end, Type type)
		{
		  this.start = start;
		  this.end = end;
		  this.type = type;
		}

		protected internal Field()
		{
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Object parse(String line) throws NumberFormatException
		public virtual object parse(string line)
		{
		  return parse(line, this.start, this.end);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Object parse(String line, int offset) throws NumberFormatException
		public virtual object parse(string line, int offset)
		{
		  return parse(line, this.start + offset, this.end + offset);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected Object parse(String line, int start, int end) throws NumberFormatException
		protected internal virtual object parse(string line, int start, int end)
		{
		  string svalue;
		  if (start >= line.Length)
		  {
			svalue = "";
		  }
		  else if (end >= line.Length)
		  {
			svalue = line.Substring(start);
		  }
		  else
		  {
			svalue = line.Substring(start, end - start);
		  }

		  if (type == typeof(string))
		  {
			return svalue;
		  }

		  try
		  {
			svalue = StringUtil2.remove(svalue, ' ');
			bool isBlank = (svalue.Trim().Length == 0);
			if (type == typeof(double))
			{
			  return isBlank ? 0.0 : Convert.ToDouble(svalue);
			}
			if (type == typeof(int))
			{
			  int? result = isBlank ? 0 : Convert.ToInt32(svalue);
			  if (hasScale)
			  {
				return result.Value * scale;
			  }
			  else
			  {
				return result.Value;
			  }
			}
			if (type == typeof(long))
			{
			  return isBlank ? 0L : Convert.ToInt64(svalue);
			}

		  }
		  catch (System.FormatException)
		  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			logger.warn("Bad line={} ", string.Format("  [{0:D},{1:D}) = <{2}> %n", start, end, svalue));
			throw;
		  }

		  return null;
		}

		public virtual float Scale
		{
			set
			{
			  this.scale = value;
			  hasScale = true;
			}
		}

	  }

	  public virtual DerivedField addDerivedField(Field from, Transform transform, Type type)
	  {
		DerivedField fld = new DerivedField(from, transform, type);
		fields.Add(fld);
		return fld;
	  }

	  /// @deprecated will move in ver6. 
	  public class DerivedField : Field
	  {
		internal Field from;
		internal Transform transform;

		internal DerivedField(Field from, Transform transform, Type type)
		{
		  this.from = from;
		  this.transform = transform;
		  this.type = type;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected Object parse(String line, int start, int end) throws NumberFormatException
		protected internal override object parse(string line, int start, int end)
		{
		  object org = from.parse(line);
		  return transform.derive(org);
		}
	  }

	  /// @deprecated will move in ver6. 
	  public interface Transform
	  {
		object derive(object org);
	  }

	  /// @deprecated will move in ver6. 
	  public class Record
	  {
		internal IList<object> values = new List<object>();

		internal static Record make(string line, System.Collections.IList fields)
		{
		  try
		  {
			Record r = new Record();
			foreach (object field in fields)
			{
			  Field f = (Field) field;
			  r.values.Add(f.parse(line));
			}
			return r;
		  }
		  catch (System.FormatException)
		  {
			logger.warn("Bad line={}", line);
			return null;
		  }
		}

		public virtual int nfields()
		{
		  return values.Count;
		}

		/// <summary>
		/// Get the kth value of this record. Will be a String, Double, or Integer.
		/// </summary>
		/// <param name="k"> which one </param>
		/// <returns> object </returns>
		public virtual object get(int k)
		{
		  return values[k];
		}

		public virtual void toString(Formatter f)
		{
		  foreach (object s in values)
		  {
			f.format(" %s,", s.ToString());
		  }
		  f.format("%n");
		}
	  }

#pragma warning restore CS0162

	}

}
