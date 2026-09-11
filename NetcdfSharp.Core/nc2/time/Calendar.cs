// ====================================================================================================
// NetcdfSharp.Core - NetcdfSharp.ucar.nc2.time.Calendar
// Rewritten from Java rich-enum (netcdf-java) to C# class-based enum using the self-written
// NetcdfSharp.org.joda.time compatible Chronology layer.
// ====================================================================================================

using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.time
{
	using Chronology = NetcdfSharp.org.joda.time.Chronology;
	using ISOChronology = NetcdfSharp.org.joda.time.chrono.ISOChronology;
	using JulianChronology = NetcdfSharp.org.joda.time.chrono.JulianChronology;
	using GJChronology = NetcdfSharp.org.joda.time.chrono.GJChronology;
	using AllLeapChronology = NetcdfSharp.org.joda.time.chrono.AllLeapChronology;
	using NoLeapChronology = NetcdfSharp.org.joda.time.chrono.NoLeapChronology;
	using ThreeSixtyDayChronology = NetcdfSharp.org.joda.time.chrono.ThreeSixtyDayChronology;

	/// <summary>
	/// Implements the CF calendar attribute. Uses the self-written Chronology layer (a
	/// joda-time compatible subset) which is not exposed to callers.
	///
	/// @author caron
	/// @since 3/22/11
	/// </summary>
	public sealed class Calendar
	{
		// do not change order, used in protobuf
		public static readonly Calendar gregorian = new Calendar("gregorian");
		public static readonly Calendar proleptic_gregorian = new Calendar("proleptic_gregorian");
		public static readonly Calendar noleap = new Calendar("noleap");
		public static readonly Calendar all_leap = new Calendar("all_leap");
		public static readonly Calendar uniform30day = new Calendar("uniform30day");
		public static readonly Calendar julian = new Calendar("julian");
		public static readonly Calendar none = new Calendar("none");

		private static readonly IList<Calendar> valueList = new List<Calendar>
		{
			gregorian, proleptic_gregorian, noleap, all_leap, uniform30day, julian, none
		};

		private readonly string _name;

		private Calendar(string name)
		{
			_name = name;
		}

		public override string ToString() => _name;

		public static Calendar get(string s)
		{
			if (s == null) return null;
			if (s.Equals("gregorian") || s.Equals("standard")) return Calendar.gregorian; // CF default, unfortunately
			if (s.Equals("proleptic_gregorian") || s.Equals("ISO8601")) return Calendar.proleptic_gregorian;
			if (s.Equals("noleap") || s.Equals("365_day")) return Calendar.noleap;
			if (s.Equals("all_leap") || s.Equals("366_day")) return Calendar.all_leap;
			if (s.Equals("uniform30day") || s.Equals("360_day")) return Calendar.uniform30day;
			if (s.Equals("julian")) return Calendar.julian;
			if (s.Equals("none")) return Calendar.none;
			return null;
		}

        public static Calendar getDefault() => proleptic_gregorian;
        public static Calendar Default => getDefault();

		public static bool isDefaultChronology(Calendar cal)
		{
			return cal == null || cal == getDefault() || cal == Calendar.none;
		}

		// Map of CF calendars to chronologies
		private static readonly IDictionary<Calendar, Chronology> CHRONOLOGIES = new Dictionary<Calendar, Chronology>();
		private static readonly IDictionary<Chronology, Calendar> CALENDARS = new Dictionary<Chronology, Calendar>();

		static Calendar()
		{
			// GJ = mixed Gregorian/Julian, the CF default "gregorian"/"standard"
			associate(Calendar.gregorian, GJChronology.getInstanceUTC());
			// ISO = proleptic Gregorian
			associate(Calendar.none, ISOChronology.getInstanceUTC());
			associate(Calendar.proleptic_gregorian, ISOChronology.getInstanceUTC());
			// Julian
			associate(Calendar.julian, JulianChronology.getInstanceUTC());
			// fixed-year calendars
			associate(Calendar.all_leap, AllLeapChronology.getInstanceUTC());
			associate(Calendar.noleap, NoLeapChronology.getInstanceUTC());
			associate(Calendar.uniform30day, ThreeSixtyDayChronology.getInstanceUTC());
		}

		private static void associate(Calendar cal, Chronology cron)
		{
			CHRONOLOGIES[cal] = cron;
			CALENDARS[cron] = cal;
		}

		/// <summary>
		/// Return the Chronology for the given Calendar (UTC time zone).
		/// </summary>
		public static Chronology getChronology(Calendar cal)
		{
			if (cal == null) cal = getDefault();
			return CHRONOLOGIES[cal];
		}

		/// <summary>
		/// Return the Calendar corresponding to the given Chronology.
		/// </summary>
		public static Calendar of(Chronology cron)
		{
			if (cron == null) return getDefault();
			if (CALENDARS.TryGetValue(cron, out Calendar result)) return result;
			return getDefault();
		}
	}
}
