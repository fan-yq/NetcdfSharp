// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;

	/// <summary>
	/// Used to group records into a CDM variable
	/// Herein lies the semantics of variable object identity.
	/// Read it and weep.
	/// 
	/// @author caron
	/// @since 12/28/2014
	/// </summary>
	public class Grib2Variable
	{

	  public static int cdmVariableHash(Grib2Tables cust, Grib2Record gr, int gdsHashOverride, bool intvMerge, bool useGenType)
	  {
		Grib2Variable gv = new Grib2Variable(cust, gr, gdsHashOverride, intvMerge, useGenType);
		return gv.GetHashCode();
	  }

	  ////////////////////////////////////////////////////////////////////////
	  private readonly Grib2Tables cust;
	  private readonly int discipline, center, subcenter;
	  private readonly int gdsHash;
	  private readonly Grib2Gds gds;
	  private readonly Grib2Pds pds;
	  private readonly bool intvMerge;
	  private readonly bool useGenType;

	  /// <summary>
	  /// Used when building from gbx9 (full records)
	  /// </summary>
	  /// <param name="cust"> customizer </param>
	  /// <param name="gr"> the Grib record </param>
	  /// <param name="gdsHashOverride"> can override the gdsHash, 0 for no override </param>
	  /// <param name="intvMerge"> should intervals be merged? default true </param>
	  /// <param name="useGenType"> should genProcessType be used in hash? default false </param>
	  public Grib2Variable(Grib2Tables cust, Grib2Record gr, int gdsHashOverride, bool intvMerge, bool useGenType)
	  {
		this.cust = cust;
		this.discipline = gr.Discipline;
		this.center = gr.Id.Center_id;
		this.subcenter = gr.Id.Subcenter_id;
		this.gds = gr.GDS;
		this.gdsHash = gdsHashOverride != 0 ? gdsHashOverride : gr.GDS.GetHashCode();
		this.pds = gr.PDS;
		this.intvMerge = intvMerge;
		this.useGenType = useGenType;
	  }

	  /// <summary>
	  /// Used when building from ncx (full records)
	  /// </summary>
	  public Grib2Variable(Grib2Tables cust, int discipline, int center, int subcenter, Grib2Gds gds, Grib2Pds pds, bool intvMerge, bool useGenType)
	  {
		this.cust = cust;
		this.discipline = discipline;
		this.center = center;
		this.subcenter = subcenter;
		this.gds = gds;
		this.gdsHash = gds.hashCode_Conflict; // ok because no overridden gds hashCodes have made it into the ncx
		this.pds = pds;
		this.intvMerge = intvMerge;
		this.useGenType = useGenType;
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

		Grib2Variable var2 = (Grib2Variable) o;
		if (gdsHash != var2.gdsHash)
		{
		  return false;
		}
		if (!gds.Equals(var2.gds))
		{
		  return false;
		}

		Grib2Pds pds2 = var2.pds;

		if (pds.ParameterNumber != pds2.ParameterNumber)
		{
		  return false;
		}
		if (pds.ParameterCategory != pds2.ParameterCategory)
		{
		  return false;
		}
		if (pds.TemplateNumber != pds2.TemplateNumber)
		{
		  return false;
		}
		if (discipline != var2.discipline)
		{
		  return false;
		}

		if (Grib2Utils.isLayer(pds) != Grib2Utils.isLayer(pds2))
		{
		  return false;
		}
		if (pds.LevelType1 != pds2.LevelType1)
		{
		  return false;
		}

		if (pds.isTimeInterval() != pds2.isTimeInterval())
		{
		  return false;
		}
		if (pds.isTimeInterval())
		{
		  if (!intvMerge)
		  {
			double size = cust.getForecastTimeIntervalSizeInHours(pds); // only used to decide on variable identity
			double size2 = cust.getForecastTimeIntervalSizeInHours(pds2);
			if (size != size2)
			{
			  return false;
			}
		  }
		  if (pds.StatisticalProcessType != pds2.StatisticalProcessType)
		  {
			return false;
		  }
		}

		if (pds.SpatialInterval != pds2.SpatialInterval)
		{
		  return false;
		}
		if (pds.SpatialInterval)
		{
		  Grib2Pds.PdsSpatialInterval pdsSpatial = (Grib2Pds.PdsSpatialInterval) pds;
		  if (pdsSpatial.SpatialStatisticalProcessType != pdsSpatial.SpatialStatisticalProcessType)
		  {
			return false;
		  }
		}

		int ensDerivedType = -1;
		if (pds.EnsembleDerived != pds2.EnsembleDerived)
		{
		  return false;
		}
		if (pds.EnsembleDerived)
		{
		  Grib2Pds.PdsEnsembleDerived pdsDerived = (Grib2Pds.PdsEnsembleDerived) pds;
		  Grib2Pds.PdsEnsembleDerived pdsDerived2 = (Grib2Pds.PdsEnsembleDerived) pds2;
		  if (pdsDerived.DerivedForecastType != pdsDerived2.DerivedForecastType)
		  {
			return false;
		  }
		  ensDerivedType = pdsDerived.DerivedForecastType; // derived type (table 4.7)

		}
		else
		{
		  if (pds.Ensemble != pds2.Ensemble)
		  {
			return false;
		  }
		}

		int probType = -1;
		if (pds.Probability != pds2.Probability)
		{
		  return false;
		}
		if (pds.Probability)
		{
		  Grib2Pds.PdsProbability pdsProb = (Grib2Pds.PdsProbability) pds;
		  Grib2Pds.PdsProbability pdsProb2 = (Grib2Pds.PdsProbability) pds2;
		  if (pdsProb.ProbabilityHashcode != pdsProb2.ProbabilityHashcode)
		  {
			return false;
		  }
		  probType = pdsProb.ProbabilityType;
		}

		if (pds.Percentile != pds2.Percentile)
		{
		  return false;
		}
		if (pds.Percentile)
		{
		  Grib2Pds.PdsPercentile pdsPctl = (Grib2Pds.PdsPercentile) pds;
		  Grib2Pds.PdsPercentile pdsPctl2 = (Grib2Pds.PdsPercentile) pds2;
		  if (pdsPctl.PercentileValue != pdsPctl2.PercentileValue)
		  {
			return false;
		  }
		}

		// if this uses any local tables, then we have to add the center id, and subcenter if present
		if ((pds2.ParameterCategory > 191) || (pds2.ParameterNumber > 191) || (pds2.LevelType1 > 191) || (pds2.isTimeInterval() && pds2.StatisticalProcessType > 191) || (ensDerivedType > 191) || (probType > 191))
		{

		  if (center != var2.center)
		  {
			return false;
		  }
		  if (subcenter != var2.subcenter)
		  {
			return false;
		  }
		}

		// always use the GenProcessType when "error" (6 or 7) 2/8/2012; added to equals 2/7/2016
		int genType = pds.GenProcessType;
		int genType2 = pds2.GenProcessType;
		bool error = (genType == 6 || genType == 7);
		bool error2 = (genType2 == 6 || genType2 == 7);
		if (error != error2)
		{
		  return false;
		}
		return !useGenType || (genType == genType2);
	  }


	  public override int GetHashCode()
	  {
		int result = 17;

		result += result * 31 + discipline;
		result += result * 31 + pds.LevelType1;
		if (Grib2Utils.isLayer(pds))
		{
		  result += result * 31 + 1;
		}

		result += result * 31 + this.gdsHash; // the horizontal grid

		result += result * 31 + pds.ParameterCategory;
		result += result * 31 + pds.TemplateNumber;

		if (pds.isTimeInterval())
		{
		  if (!intvMerge)
		  {
			double size = cust.getForecastTimeIntervalSizeInHours(pds); // LOOK using an Hour here, but will need to make
																		// this configurable
			result += result * (int)(31 + (1000 * size)); // create new variable for each interval size - default not
		  }
		  result += result * 31 + pds.StatisticalProcessType; // create new variable for each stat type
		}

		if (pds.SpatialInterval)
		{
		  Grib2Pds.PdsSpatialInterval pdsSpatial = (Grib2Pds.PdsSpatialInterval) pds;
		  result += result * 31 + pdsSpatial.SpatialStatisticalProcessType; // template 15
		}

		result += result * 31 + pds.ParameterNumber;

		int ensDerivedType = -1;
		if (pds.EnsembleDerived)
		{ // a derived ensemble must have a derivedForecastType
		  Grib2Pds.PdsEnsembleDerived pdsDerived = (Grib2Pds.PdsEnsembleDerived) pds;
		  ensDerivedType = pdsDerived.DerivedForecastType; // derived type (table 4.7)
		  result += result * 31 + ensDerivedType;

		}
		else if (pds.Ensemble)
		{
		  result += result * 31 + 1;
		}

		// each probability interval generates a separate variable; could be a dimension instead
		int probType = -1;
		if (pds.Probability)
		{
		  Grib2Pds.PdsProbability pdsProb = (Grib2Pds.PdsProbability) pds;
		  probType = pdsProb.ProbabilityType;
		  result += result * 31 + pdsProb.ProbabilityHashcode;
		}

		if (pds.Percentile)
		{
		  Grib2Pds.PdsPercentile pdsPerc = (Grib2Pds.PdsPercentile) pds;
		  result += result * 31 + pdsPerc.PercentileValue;
		}

		// if this uses any local tables, then we have to add the center id, and subcenter if present
		if ((pds.ParameterCategory > 191) || (pds.ParameterNumber > 191) || (pds.LevelType1 > 191) || (pds.isTimeInterval() && pds.StatisticalProcessType > 191) || (ensDerivedType > 191) || (probType > 191))
		{
		  result += result * 31 + center;
		  if (subcenter > 0)
		  {
			result += result * 31 + subcenter;
		  }
		}

		// always use the GenProcessType when "error" (6 or 7) 2/8/2012
		int genType = pds.GenProcessType;
		if (genType == 6 || genType == 7 || (useGenType && genType > 0))
		{
		  result += result * 31 + genType;
		}

		return result;
	  }

	}

}
