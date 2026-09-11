// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using GdsHorizCoordSys = NetcdfSharp.ucar.nc2.grib.GdsHorizCoordSys;

	/// <summary>
	/// Encapsolates the GdsHorizCoordSys; shared by the GroupHcs
	/// 
	/// @author caron
	/// @since 11/10/2014
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class GribHorizCoordSystem
	public class GribHorizCoordSystem
	{
	  private readonly GdsHorizCoordSys hcs;
	  private readonly byte[] rawGds; // raw gds: Grib1SectionGridDefinition or Grib2SectionGridDefinition
	  private readonly object gdsHash;
	  private readonly string id, description;
	  private readonly int predefinedGridDefinition; // grib1

	  internal GribHorizCoordSystem(GdsHorizCoordSys hcs, byte[] rawGds, object gdsHash, string id, string description, int predefinedGridDefinition)
	  {
		this.hcs = hcs;
		this.rawGds = rawGds;
		this.gdsHash = gdsHash;
		this.predefinedGridDefinition = predefinedGridDefinition;

		this.id = id;
		this.description = description;
	  }

	  public virtual GdsHorizCoordSys Hcs
	  {
		  get
		  {
			return hcs;
		  }
	  }

	  public virtual byte[] RawGds
	  {
		  get
		  {
			return rawGds;
		  }
	  }

	  // use this object for hashmaps
	  public virtual object GdsHash
	  {
		  get
		  {
			return gdsHash;
		  }
	  }

	  // unique name for Group
	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  // human readable
	  public virtual string Description
	  {
		  get
		  {
			return description;
		  }
	  }

	  public virtual int PredefinedGridDefinition
	  {
		  get
		  {
			return predefinedGridDefinition;
		  }
	  }

	}

}
