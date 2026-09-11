// Hand-written replacement for the protobuf-generated Java class
// NetcdfSharp.ucar.nc2.ft.point.bufr.BufrCdmIndexProto (from bufrCdmIndex.proto).
//
// The Java original is generated at build time by protoc; we do not use
// Google.Protobuf. Only the enums + valueOf are needed by the compiled Bufr
// read path (the .ncx index persistence that would use the message builders
// / toByteArray is out of scope). Station/Field/BufrIndex getters are used
// only by BufrFeatureDatasetFactory.cs, which is <Compile Remove>'d.
//
// Java Enum values preserved for fidelity (matter only for the .ncx files we
// never read/write; names are what the code switches on).
//
// Enum names follow Java: camelCase constants; valueOf is case-sensitive and
// throws ArgumentException on unknown names (mirrors Java IllegalArgumentException).


using System;

namespace NetcdfSharp.ucar.nc2.ft.point.bufr
{
	/// <summary>
	/// Java-style outer class holding the enum types. In Java this is the generated
	/// protobuf outer class BufrCdmIndexProto; the message classes are not ported.
	/// </summary>
	public static class BufrCdmIndexProto
	{
		public enum FldType
		{
			def = 0,
			lat = 1,
			lon = 2,
			height = 3,
			heightAboveStation = 4,
			heightOfStation = 5,
			stationId = 10,
			stationDesc = 11,
			wmoId = 12,
			wmoBlock = 13,
			year = 15,
			month = 16,
			day = 17,
			hour = 18,
			minute = 19,
			sec = 20,
			doy = 21,
			timeIncr = 22,
			incrS = 23
		}

		public enum FldAction
		{
			defa = 0,
			none = 1,
			remove = 2,
			asMissing = 3,
			asArray = 4,
			concat = 5
		}

		/// <summary>
		/// Java Enum.valueOf(string) replacement: case-sensitive name lookup.
		/// Throws ArgumentException on unknown names, mirroring Java IllegalArgumentException.
		/// </summary>
		public static FldAction valueOfFldAction(string name)
		{
			return (FldAction)Enum.Parse(typeof(FldAction), name, false);
		}
	}
}
