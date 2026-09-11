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
	using NetcdfSharp.ucar.nc2;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CollectionUpdateType = NetcdfSharp.thredds.inventory.CollectionUpdateType;
	using CoordinateTimeAbstract = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTimeAbstract;
	using CoverageCollection = NetcdfSharp.ucar.nc2.ft2.coverage.CoverageCollection;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib1Parameter = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Parameter;
	using GribCoverageDataset = NetcdfSharp.ucar.nc2.grib.coverage.GribCoverageDataset;
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using GribUtils = NetcdfSharp.ucar.nc2.grib.GribUtils;
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using NetcdfDataset = NetcdfSharp.ucar.nc2.dataset.NetcdfDataset;

	/// <summary>
	/// Grib1-specific subclass of GribCollection.
	/// 
	/// @author John
	/// @since 9/5/11
	/// </summary>
	public class Grib1Collection : GribCollectionImmutable
	{

		internal Grib1Collection(GribCollectionMutable gc) : base(gc)
		{
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override @Nullable public NetcdfSharp.ucar.nc2.dataset.NetcdfDataset getNetcdfDataset(Dataset ds, GroupGC group, String filename, NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig gribConfig, NetcdfSharp.java.util.Formatter errlog, NetcdfSharp.org.slf4j.Logger logger) throws NetcdfSharp.java.io.IOException
		public override NetcdfDataset getNetcdfDataset(Dataset ds, GroupGC group, string filename, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger)
		{
			if (string.ReferenceEquals(filename, null))
			{
				Grib1Iosp iosp = new Grib1Iosp(group, ds.Type);
				return buildNetcdfDataset(iosp, Location);

			}
			else
			{
				MFile wantFile = findMFileByName(filename);
				if (wantFile != null)
				{
					GribCollectionImmutable gc = GribCdmIndex.openGribCollectionFromDataFile(true, wantFile, CollectionUpdateType.nocheck, gribConfig, errlog, logger); // LOOK thread-safety : creating ncx
					if (gc == null)
					{
						return null;
					}

					Grib1Iosp iosp = new Grib1Iosp(gc);
					return buildNetcdfDataset(iosp, Location);
				}
				return null;
			}
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override @Nullable public NetcdfSharp.ucar.nc2.dt.grid.GridDataset getGridDataset(Dataset ds, GroupGC group, String filename, NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig gribConfig, NetcdfSharp.java.util.Formatter errlog, NetcdfSharp.org.slf4j.Logger logger) throws NetcdfSharp.java.io.IOException
		public override NetcdfSharp.ucar.nc2.dt.grid.GridDataset getGridDataset(Dataset ds, GroupGC group, string filename, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger)
		{
			if (string.ReferenceEquals(filename, null))
			{
				Grib1Iosp iosp = new Grib1Iosp(group, ds.Type);
				NetcdfDataset ncd = buildNetcdfDataset(iosp, Location + "#" + group.Id);
				return new NetcdfSharp.ucar.nc2.dt.grid.GridDataset(ncd); // LOOK - replace with custom GridDataset??

			}
			else
			{
				MFile wantFile = findMFileByName(filename);
				if (wantFile != null)
				{
					GribCollectionImmutable gc = GribCdmIndex.openGribCollectionFromDataFile(true, wantFile, CollectionUpdateType.nocheck, gribConfig, errlog, logger); // LOOK thread-safety : creating ncx
					if (gc == null)
					{
						return null;
					}

					Grib1Iosp iosp = new Grib1Iosp(gc);
					NetcdfDataset ncd = buildNetcdfDataset(iosp, Location);
					return new NetcdfSharp.ucar.nc2.dt.grid.GridDataset(ncd); // LOOK - replace with custom GridDataset??
				}
				return null;
			}
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override @Nullable public NetcdfSharp.ucar.nc2.ft2.coverage.CoverageCollection getGridCoverage(Dataset ds, GroupGC group, String filename, NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig gribConfig, NetcdfSharp.java.util.Formatter errlog, NetcdfSharp.org.slf4j.Logger logger) throws NetcdfSharp.java.io.IOException
		public override CoverageCollection getGridCoverage(Dataset ds, GroupGC group, string filename, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger)
		{
			if (string.ReferenceEquals(filename, null))
			{
				GribCoverageDataset gribCov = new GribCoverageDataset(this, ds, group);
				return gribCov.createCoverageCollection();

			}
			else
			{
				MFile wantFile = findMFileByName(filename);
				if (wantFile != null)
				{
					GribCollectionImmutable gc = GribCdmIndex.openGribCollectionFromDataFile(true, wantFile, CollectionUpdateType.nocheck, gribConfig, errlog, logger); // LOOK thread-safety : creating ncx
					if (gc == null)
					{
						return null;
					}
					GribCoverageDataset gribCov = new GribCoverageDataset(gc, null, null);
					return gribCov.createCoverageCollection();
				}
				return null;
			}
		}

		public override void addGlobalAttributes(AttributeContainer result)
		{
			string val = cust.getGeneratingProcessName(GenProcessId);
			if (!string.ReferenceEquals(val, null))
			{
				result.addAttribute(new NcAttribute(GribUtils.GEN_PROCESS, val));
			}
			result.addAttribute(new NcAttribute(CDM.FILE_FORMAT, DataFormatType.GRIB1.Description));
		}

		public override string makeVariableId(GribCollectionImmutable.VariableIndex v)
		{
			return makeVariableId(Center, Subcenter, v.TableVersion, v.Parameter, v.LevelType, v.Layer, v.IntvType, v.IntvName);
		}

		internal static string makeVariableId(int center, int subcenter, int tableVersion, int paramNo, int levelType, bool isLayer, int intvType, string intvName)
		{
			using (Formatter f = new Formatter())
			{

				f.format("VAR_%d-%d-%d-%d", center, subcenter, tableVersion, paramNo); // "VAR_7-15--1-20_L1";

				if (levelType != GribNumbers.UNDEFINED)
				{ // satellite data doesnt have a level
					f.format("_L%d", levelType); // code table 4.5
					if (isLayer)
					{
						f.format("_layer");
					}
				}

				if (intvType >= 0)
				{
					if (!string.ReferenceEquals(intvName, null))
					{
						if (intvName.Equals(CoordinateTimeAbstract.MIXED_INTERVALS))
						{
							f.format("_Imixed");
						}
						else
						{
							f.format("_I%s", intvName);
						}
					}
					f.format("_S%s", intvType);
				}

				return f.ToString();
			}
		}

		public override void addVariableAttributes(AttributeContainer v, GribCollectionImmutable.VariableIndex vindex)
		{
			addVariableAttributes(v, vindex, this);
		}

		internal static void addVariableAttributes(AttributeContainer v, GribCollectionImmutable.VariableIndex vindex, GribCollectionImmutable gc)
		{
			Grib1Customizer cust1 = (Grib1Customizer)gc.cust;

			// Grib attributes
			v.addAttribute(new NcAttribute(Grib.VARIABLE_ID_ATTNAME, gc.makeVariableId(vindex)));
			v.addAttribute(new NcAttribute("Grib1_Center", gc.Center));
			v.addAttribute(new NcAttribute("Grib1_Subcenter", gc.Subcenter));
			v.addAttribute(new NcAttribute("Grib1_TableVersion", vindex.TableVersion));
			v.addAttribute(new NcAttribute("Grib1_Parameter", vindex.Parameter));
			Grib1Parameter param = cust1.getParameter(gc.Center, gc.Subcenter, vindex.TableVersion, vindex.Parameter);
			if (param != null && !string.ReferenceEquals(param.Name, null))
			{
				v.addAttribute(new NcAttribute("Grib1_Parameter_Name", param.Name));
			}

			if (vindex.LevelType != GribNumbers.MISSING)
			{
				v.addAttribute(new NcAttribute("Grib1_Level_Type", vindex.LevelType));
			}
			string ldesc = cust1.getLevelDescription(vindex.LevelType);
			if (!string.ReferenceEquals(ldesc, null))
			{
				v.addAttribute(new NcAttribute("Grib1_Level_Desc", ldesc));
			}


			string timeTypeName = cust1.getTimeTypeName(vindex.IntvType);
			if (!string.ReferenceEquals(timeTypeName, null) && timeTypeName.Length > 0)
			{
				v.addAttribute(new NcAttribute("Grib1_Interval_Type", vindex.IntvType));
				v.addAttribute(new NcAttribute("Grib1_Interval_Name", timeTypeName));
			}

			if (vindex.EnsDerivedType >= 0)
			{
				v.addAttribute(new NcAttribute("Grib1_Ensemble_Derived_Type", vindex.EnsDerivedType));
			}
			else if (!string.ReferenceEquals(vindex.ProbabilityName, null) && vindex.ProbabilityName.Length > 0)
			{
				v.addAttribute(new NcAttribute("Grib1_Probability_Type", vindex.ProbabilityName));
			}
		}

	}

}
