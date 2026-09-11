// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib
{
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;

	/// <summary>
	/// Abstraction of GribTable for Grib Collections.
	/// Allows Grin1 and Grib2 to be handles through common interface.
	/// 
	/// @author John
	/// @since 9/5/11
	/// </summary>
	public interface GribTables
	{

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable String getSubCenterName(int center, int subcenter);
	  string getSubCenterName(int center, int subcenter);

	  string getLevelNameShort(int code);

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable GribStatType getStatType(int intvType);
	  GribStatType getStatType(int intvType);

	  VertCoordType getVertUnit(int code);

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable String getGeneratingProcessName(int code);
	  string getGeneratingProcessName(int code);

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable String getGeneratingProcessTypeName(int code);
	  string getGeneratingProcessTypeName(int code);

	  public interface Parameter
	  {
		/// <summary>
		/// Unsigned byte </summary>
		int Discipline {get;}

		/// <summary>
		/// Unsigned byte </summary>
		int Category {get;}

		/// <summary>
		/// Unsigned byte </summary>
		int Number {get;}

		string Name {get;}

		string Unit {get;}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable String getAbbrev();
		string Abbrev {get;}

		string Description {get;}

		/// <summary>
		/// Unique across all Parameter tables </summary>
		string Id {get;}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable Float getFill();
		float? Fill {get;}

		float? Missing {get;}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable String getOperationalStatus();
		string OperationalStatus {get;}
	  }

	}

}
