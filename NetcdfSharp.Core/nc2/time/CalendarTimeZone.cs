// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.time
{
	using DateTimeZone = NetcdfSharp.org.joda.time.DateTimeZone;

	/// <summary>
	/// Encapsulate TimeZone functionality.
	/// 
	/// @author caron
	/// @since 10/28/12
	/// </summary>
	public class CalendarTimeZone
	{

		public static readonly CalendarTimeZone UTC = new CalendarTimeZone("UTC");

		public static ISet<string> AvailableIDs
		{
			get
			{
				return new HashSet<string>(DateTimeZone.getAvailableIDs());
			}
		}

		private readonly DateTimeZone jodaTimeZone;

		/// <summary>
		/// Gets a time zone instance for the specified time zone id.
		/// <para>
		/// The time zone id may be one of those returned by getAvailableIDs.
		/// Short ids, as accepted by <seealso cref="NetcdfSharp.java.util.JTimeZone"/>, are not accepted.
		/// All IDs must be specified in the long format.
		/// The exception is UTC, which is an acceptable id.
		/// </para>
		/// <para>
		/// Alternatively a locale independent, fixed offset, datetime zone can
		/// be specified. The form <code>[+-]hh:mm</code> can be used.
		/// 
		/// </para>
		/// </summary>
		/// <param name="id"> the ID of the datetime zone, null means default </param>
		/// <exception cref="IllegalArgumentException"> if the ID is not recognised </exception>
		public CalendarTimeZone(string id)
		{
			jodaTimeZone = DateTimeZone.forID(id);
		}

		public CalendarTimeZone(NetcdfSharp.java.util.JTimeZone zone)
		{
			jodaTimeZone = DateTimeZone.forTimeZone(zone);
		}

		// package private
		internal virtual DateTimeZone JodaTimeZone
		{
			get
			{
				return jodaTimeZone;
			}
		}
	}

}
