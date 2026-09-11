// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.units
{

	/// <summary>
	/// Date parsing and formatting. Always uses GMT.
	/// These are not thread-safe.
	/// These use NetcdfSharp.java.util.Date and NetcdfSharp.java.text.SimpleDateFormat.
	/// </summary>
	/// @deprecated use <seealso cref="NetcdfSharp.ucar.nc2.time.CalendarDateFormatter"/> 
	[Obsolete("use <seealso cref=\"NetcdfSharp.ucar.nc2.time.CalendarDateFormatter\"/>")]
	public class DateFormatter
	{

		// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private NetcdfSharp.java.text.SimpleDateFormat isoDateTimeFormat_Conflict, isoDateNoSecsFormat_Conflict, stdDateTimeFormat_Conflict, stdDateNoSecsFormat_Conflict, dateOnlyFormat_Conflict;

		private void isoDateTimeFormat()
		{
			if (isoDateTimeFormat_Conflict != null)
			{
				return;
			}
			isoDateTimeFormat_Conflict = new NetcdfSharp.java.text.SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
			isoDateTimeFormat_Conflict.setTimeZone(NetcdfSharp.java.util.JTimeZone.getTimeZone("GMT"));
		}

		private void isoDateNoSecsFormat()
		{
			if (isoDateNoSecsFormat_Conflict != null)
			{
				return;
			}
			isoDateNoSecsFormat_Conflict = new NetcdfSharp.java.text.SimpleDateFormat("yyyy-MM-dd'T'HH:mm");
			isoDateNoSecsFormat_Conflict.setTimeZone(NetcdfSharp.java.util.JTimeZone.getTimeZone("GMT"));
		}

		private void stdDateTimeFormat()
		{
			if (stdDateTimeFormat_Conflict != null)
			{
				return;
			}
			stdDateTimeFormat_Conflict = new NetcdfSharp.java.text.SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
			stdDateTimeFormat_Conflict.setTimeZone(NetcdfSharp.java.util.JTimeZone.getTimeZone("GMT"));
		}

		private void stdDateNoSecsFormat()
		{
			if (stdDateNoSecsFormat_Conflict != null)
			{
				return;
			}
			stdDateNoSecsFormat_Conflict = new NetcdfSharp.java.text.SimpleDateFormat("yyyy-MM-dd HH:mm");
			stdDateNoSecsFormat_Conflict.setTimeZone(NetcdfSharp.java.util.JTimeZone.getTimeZone("GMT")); // same as UTC
		}

		private void dateOnlyFormat()
		{
			if (dateOnlyFormat_Conflict != null)
			{
				return;
			}
			dateOnlyFormat_Conflict = new NetcdfSharp.java.text.SimpleDateFormat("yyyy-MM-dd");
			dateOnlyFormat_Conflict.setTimeZone(NetcdfSharp.java.util.JTimeZone.getTimeZone("GMT"));
		}

		////////////////////////////////////////////////////////////////////////////////


		/// <summary>
		/// Parse the text in W3C profile of ISO 8601 format.
		/// </summary>
		/// <param name="text"> parse this text </param>
		/// <returns> equivalent Date or null if failure </returns>
		/// <seealso cref="<a href="http://www.w3.org/TR/NOTE-datetime">W3C profile of ISO 8601</a>"
		////>
		/// @deprecated As of netCDF-JAVA 4.3.10. Use <seealso cref="NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.isoStringToDate(String) "/>
		///             instead
		///  
		[Obsolete("As of netCDF-JAVA 4.3.10. Use <seealso cref=\"NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.isoStringToDate(String) \"/>")]
		public virtual DateTime getISODate(string text)
		{
			DateTime result;

			// try "yyyy-MM-dd HH:mm:ss"
			try
			{
				result = stdDateTimeFormat(text);
				return result;
			}
			catch (NetcdfSharp.java.text.ParseException)
			{
			}

			// now try "yyyy-MM-dd'T'HH:mm:ss"
			try
			{
				result = isoDateTimeFormat(text);
				return result;
			}
			catch (NetcdfSharp.java.text.ParseException)
			{
			}

			// now try "yyyy-MM-dd'T'HH:mm"
			try
			{
				result = isoDateNoSecsFormat(text);
				return result;
			}
			catch (NetcdfSharp.java.text.ParseException)
			{
			}

			// now try "yyyy-MM-dd HH:mm"
			try
			{
				result = stdDateNoSecsFormat(text);
				return result;
			}
			catch (NetcdfSharp.java.text.ParseException)
			{
			}

			// now try "yyyy-MM-dd"
			try
			{
				result = dateOnlyFormat(text);
				return result;
			}
			catch (NetcdfSharp.java.text.ParseException)
			{
			}

			return default(DateTime);
		}

		/// <summary>
		/// Parse text in the format "yyyy-MM-dd HH:mm:ss"
		/// </summary>
		/// <param name="text"> parse this text </param>
		/// <returns> equivalent Date </returns>
		/// <exception cref="NetcdfSharp.java.text.ParseException"> if not formatted correctly </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private NetcdfSharp.java.util.Date stdDateTimeFormat(String text) throws NetcdfSharp.java.text.ParseException
		private DateTime stdDateTimeFormat(string text)
		{
			text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
			stdDateTimeFormat();
			return stdDateTimeFormat_Conflict.parse(text);
		}

		/// <summary>
		/// Parse text in the format "yyyy-MM-dd HH:mm"
		/// </summary>
		/// <param name="text"> parse this text </param>
		/// <returns> equivalent Date </returns>
		/// <exception cref="NetcdfSharp.java.text.ParseException"> if not formatted correctly </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private NetcdfSharp.java.util.Date stdDateNoSecsFormat(String text) throws NetcdfSharp.java.text.ParseException
		private DateTime stdDateNoSecsFormat(string text)
		{
			text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
			stdDateNoSecsFormat();
			return stdDateNoSecsFormat_Conflict.parse(text);
		}

		/// <summary>
		/// Parse text in the format "yyyy-MM-dd'T'HH:mm:ss"
		/// </summary>
		/// <param name="text"> parse this text </param>
		/// <returns> equivalent Date </returns>
		/// <exception cref="NetcdfSharp.java.text.ParseException"> if not formatted correctly </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private NetcdfSharp.java.util.Date isoDateTimeFormat(String text) throws NetcdfSharp.java.text.ParseException
		private DateTime isoDateTimeFormat(string text)
		{
			text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
			isoDateTimeFormat();
			return isoDateTimeFormat_Conflict.parse(text);
		}

		/// <summary>
		/// Parse text in the format "yyyy-MM-dd'T'HH:mm"
		/// </summary>
		/// <param name="text"> parse this text </param>
		/// <returns> equivalent Date </returns>
		/// <exception cref="NetcdfSharp.java.text.ParseException"> if not formatted correctly </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private NetcdfSharp.java.util.Date isoDateNoSecsFormat(String text) throws NetcdfSharp.java.text.ParseException
		private DateTime isoDateNoSecsFormat(string text)
		{
			text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
			isoDateNoSecsFormat();
			return isoDateNoSecsFormat_Conflict.parse(text);
		}

		/// <summary>
		/// Parse text in the format "yyyy-MM-dd"
		/// </summary>
		/// <param name="text"> parse this text </param>
		/// <returns> equivalent Date </returns>
		/// <exception cref="NetcdfSharp.java.text.ParseException"> if not formatted correctly </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private NetcdfSharp.java.util.Date dateOnlyFormat(String text) throws NetcdfSharp.java.text.ParseException
		private DateTime dateOnlyFormat(string text)
		{
			text = (string.ReferenceEquals(text, null)) ? "" : text.Trim();
			dateOnlyFormat();
			return dateOnlyFormat_Conflict.parse(text);
		}

		////////////


		/// <summary>
		/// Return standard GMT date format; show date only, not time. Format = "yyyy-MM-dd"
		/// </summary>
		/// @deprecated use toDateOnlyString 
		public virtual string getStandardDateOnlyString(DateTime date)
		{
			return toDateOnlyString(date);
		}

		/// <summary>
		/// date only format= yyyy-MM-dd
		/// </summary>
		/// @deprecated use <seealso cref="NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.toDateString(Date)"/> 
		[Obsolete("use <seealso cref=\"NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.toDateString(Date)\"/>")]
		public virtual string toDateString(DateTime date)
		{
			return toDateOnlyString(date);
		}

		/// <summary>
		/// date only format= yyyy-MM-dd
		/// </summary>
		/// <param name="date"> format this date </param>
		/// <returns> date formatted as date only </returns>
		/// @deprecated use <seealso cref="NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.toDateString(Date)"/> 
		[Obsolete("use <seealso cref=\"NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.toDateString(Date)\"/>")]
		public virtual string toDateOnlyString(DateTime date)
		{
			dateOnlyFormat();
			return dateOnlyFormat_Conflict.format(date);
		}

		/// <summary>
		/// "standard date format" = yyyy-MM-dd HH:mm:ssZ
		/// </summary>
		/// <param name="date"> format this date </param>
		/// <returns> date formatted as date/time </returns>
		/// @deprecated use <seealso cref="NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.toDateTimeString(Date)"/> 
		[Obsolete("use <seealso cref=\"NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.toDateTimeString(Date)\"/>")]
		public virtual string toDateTimeString(DateTime? date)
		{
			if (date == null)
			{
				return "Unknown";
			}
			stdDateTimeFormat();
			return stdDateTimeFormat_Conflict.format(date.Value) + "Z";
		}

		/// <summary>
		/// "ISO date format" = yyyy-MM-dd'T'HH:mm:ssZ
		/// </summary>
		/// <param name="date"> format this date </param>
		/// <returns> date formatted as ISO date string </returns>
		/// @deprecated use <seealso cref="NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.toDateTimeStringISO"/> 
		[Obsolete("use <seealso cref=\"NetcdfSharp.ucar.nc2.time.CalendarDateFormatter.toDateTimeStringISO\"/>")]
		public virtual string toDateTimeStringISO(DateTime date)
		{
			isoDateTimeFormat();
			return isoDateTimeFormat_Conflict.format(date) + "Z";
		}
	}

}
