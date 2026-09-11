// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using Grib1Iosp = NetcdfSharp.ucar.nc2.grib.collection.Grib1Iosp;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;

	/// <summary>
	/// Used to group records into a CDM variable
	/// Herein lies the semantics of a variable object identity.
	/// Read it and weep.
	/// 
	/// @author caron
	/// @since 12/28/2014
	/// </summary>
	public class Grib1Variable
	{

	  public static int cdmVariableHash(Grib1Customizer cust, Grib1Record gr, int gdsHashOverride, bool useTableVersion, bool intvMerge, bool useCenter)
	  {
		Grib1Variable gv = new Grib1Variable(cust, gr, gdsHashOverride, useTableVersion, intvMerge, useCenter);
		return gv.GetHashCode();
	  }

	  /////////////////////////////////////////

	  private readonly Grib1Customizer cust;
	  private readonly Grib1SectionProductDefinition pds;
	  private readonly Grib1Gds gds;
	  private readonly int gdsHash;
	  private readonly bool useTableVersion;
	  private readonly bool intvMerge;
	  private readonly bool useCenter;

	  /// <summary>
	  /// Used when processing the gbx9 files
	  /// </summary>
	  /// <param name="cust"> customizer </param>
	  /// <param name="gr"> grib record </param>
	  /// <param name="gdsHashOverride"> can override the gdsHash, 0 for no override </param>
	  /// <param name="useTableVersion"> use pdss.getTableVersion(), default is false </param>
	  /// <param name="intvMerge"> put all intervals together, default true </param>
	  /// <param name="useCenter"> use center id when param no > 127, default is false </param>
	  public Grib1Variable(Grib1Customizer cust, Grib1Record gr, int gdsHashOverride, bool useTableVersion, bool intvMerge, bool useCenter)
	  {
		this.cust = cust;
		this.pds = gr.PDSsection;
		this.gds = gr.GDS;
		this.gdsHash = gdsHashOverride != 0 ? gdsHashOverride : gr.GDS.GetHashCode();
		this.useTableVersion = useTableVersion;
		this.intvMerge = intvMerge;
		this.useCenter = useCenter;
	  }

	  /// <summary>
	  /// Used when processing the ncx files
	  /// </summary>
	  /// <param name="cust"> customizer </param>
	  /// <param name="pds"> pds section </param>
	  /// <param name="gds"> the group gds </param>
	  /// <param name="useTableVersion"> use pdss.getTableVersion(), default is false </param>
	  /// <param name="intvMerge"> put all intervals together, default true </param>
	  /// <param name="useCenter"> use center id when param no > 127, default is false </param>
	  public Grib1Variable(Grib1Customizer cust, Grib1SectionProductDefinition pds, Grib1Gds gds, bool useTableVersion, bool intvMerge, bool useCenter)
	  {
		this.cust = cust;
		this.pds = pds;
		this.gds = gds;
		this.gdsHash = gds.GetHashCode(); // LOOK this assumes no overridden gds hashCodes have made it into the ncx
		this.useTableVersion = useTableVersion;
		this.intvMerge = intvMerge;
		this.useCenter = useCenter;
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		Grib1Variable var2 = (Grib1Variable) o;
		if (gdsHash != var2.gdsHash)
		{
		  return false;
		}
		if (!gds.Equals(var2.gds))
		{
		  return false;
		}

		Grib1SectionProductDefinition pds2 = var2.pds;
		if (pds.ParameterNumber != pds2.ParameterNumber)
		{
		  return false;
		}
		if (pds.LevelType != pds2.LevelType)
		{
		  return false;
		}

		if (useTableVersion)
		{
		  if (pds.TableVersion != pds2.TableVersion)
		  {
			return false;
		  }
		}

		Grib1ParamTime ptime = cust.getParamTime(pds);
		Grib1ParamTime ptime2 = cust.getParamTime(pds2);
		if (ptime.IsInterval() != ptime2.IsInterval())
		{
		  return false;
		}
		if (ptime.IsInterval())
		{
		  if (!intvMerge)
		  {
			if (ptime.IntervalSize != ptime2.IntervalSize)
			{
			  return false;
			}
		  }
		  if (ptime.StatType != ptime2.StatType)
		  {
			return false;
		  }
		}

		if (useCenter && pds.ParameterNumber > 127)
		{
		  if (pds.Center != pds2.Center)
		  {
			return false;
		  }
		  return pds.SubCenter == pds2.SubCenter;
		}

		return true;
	  }

	  public override int GetHashCode()
	  { // could switch to using guava goodFastHash, if not storing (?)
		int result = 17; // Warning: a new random seed for these functions is chosen each time the Hashing class is loaded.
						 // Do not use this method if hash codes may escape the current process in any way, for example
						 // being sent over RPC, or saved to disk.

		result += result * 31 + pds.ParameterNumber;
		result += result * 31 + gdsHash;

		result += result * 31 + pds.LevelType;
		// if (cust.isLayer(pds.getLevelType())) result += result * 31 + 1; // LOOK not needed if same level type

		if (useTableVersion)
		{
		  result += result * 31 + pds.TableVersion;
		}

		Grib1ParamTime ptime = cust.getParamTime(pds);
		if (ptime.IsInterval())
		{
		  if (!intvMerge)
		  {
			result += result * 31 + ptime.IntervalSize; // create new variable for each interval size
		  }
		  if (ptime.StatType != null)
		  {
			result += result * 31 + ptime.StatType.ordinal(); // create new variable for each stat type
		  }
		}

		// if useCenter, and this uses any local tables, then we have to add the center id, and subcenter if present
		if (useCenter && pds.ParameterNumber > 127)
		{
		  result += result * 31 + pds.Center;
		  if (pds.SubCenter > 0)
		  {
			result += result * 31 + pds.SubCenter;
		  }
		}
		return result;
	  }

	  public virtual string makeVariableName(FeatureCollectionConfig.GribConfig gribConfig)
	  {
		return Grib1Iosp.makeVariableName(cust, gribConfig, pds);
	  }
	}

}
