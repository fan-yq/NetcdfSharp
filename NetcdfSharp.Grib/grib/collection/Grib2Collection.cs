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
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using GribCoverageDataset = NetcdfSharp.ucar.nc2.grib.coverage.GribCoverageDataset;
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using MFile = NetcdfSharp.thredds.inventory.MFile;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NetcdfDataset = NetcdfSharp.ucar.nc2.dataset.NetcdfDataset;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Grib2 specific subclass of GribCollection.
	/// 
	/// @author John
	/// @since 9/5/11
	/// </summary>
	public class Grib2Collection : GribCollectionImmutable
	{

		internal Grib2Collection(GribCollectionMutable gc) : base(gc)
		{
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override @Nullable public NetcdfSharp.ucar.nc2.dataset.NetcdfDataset getNetcdfDataset(Dataset ds, GroupGC group, String filename, NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig gribConfig, NetcdfSharp.java.util.Formatter errlog, NetcdfSharp.org.slf4j.Logger logger) throws NetcdfSharp.java.io.IOException
		public override NetcdfDataset getNetcdfDataset(Dataset ds, GroupGC group, string filename, FeatureCollectionConfig gribConfig, Formatter errlog, NetcdfSharp.org.slf4j.Logger logger)
		{

			if (string.ReferenceEquals(filename, null))
			{
				Grib2Iosp iosp = new Grib2Iosp(group, ds.Type);
				return buildNetcdfDataset(iosp, Location);

			}
			else
			{
				MFile wantFile = findMFileByName(filename);
				if (wantFile != null)
				{
					GribCollectionImmutable gc = GribCdmIndex.openGribCollectionFromDataFile(false, wantFile, CollectionUpdateType.nocheck, gribConfig, errlog, logger); // LOOK thread-safety : creating ncx
					if (gc == null)
					{
						return null;
					}

					Grib2Iosp iosp = new Grib2Iosp(gc);
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
				Grib2Iosp iosp = new Grib2Iosp(group, ds.Type);
				NetcdfDataset ncd = buildNetcdfDataset(iosp, Location + "#" + group.Id);
				return new NetcdfSharp.ucar.nc2.dt.grid.GridDataset(ncd); // LOOK - replace with custom GridDataset??

			}
			else
			{
				MFile wantFile = findMFileByName(filename);
				if (wantFile != null)
				{
					GribCollectionImmutable gc = GribCdmIndex.openGribCollectionFromDataFile(false, wantFile, CollectionUpdateType.nocheck, gribConfig, errlog, logger); // LOOK thread-safety : creating ncx
					if (gc == null)
					{
						return null;
					}

					Grib2Iosp iosp = new Grib2Iosp(gc);
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
					GribCollectionImmutable gc = GribCdmIndex.openGribCollectionFromDataFile(false, wantFile, CollectionUpdateType.nocheck, gribConfig, errlog, logger); // LOOK thread-safety : creating ncx
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
			string val = cust.getGeneratingProcessTypeName(GenProcessType);
			if (!string.ReferenceEquals(val, null))
			{
				result.addAttribute(new NcAttribute("Type_of_generating_process", val));
			}
			val = cust.getGeneratingProcessName(GenProcessId);
			if (!string.ReferenceEquals(val, null))
			{
				result.addAttribute(new NcAttribute("Analysis_or_forecast_generating_process_identifier_defined_by_originating_centre", val));
			}
			val = cust.getGeneratingProcessName(BackProcessId);
			if (!string.ReferenceEquals(val, null))
			{
				result.addAttribute(new NcAttribute("Background_generating_process_identifier_defined_by_originating_centre", val));
			}
			result.addAttribute(new NcAttribute(CDM.FILE_FORMAT, DataFormatType.GRIB2.Description));
		}

		/*
		 * http://www.ncl.ucar.edu/Document/Manuals/Ref_Manual/NclFormatSupport.shtml#GRIB
		 * GRIB2 data variable name encoding
		 * 
		 * (Note: examples show intermediate steps in the formation of the name)
		 * 
		 * if production status is TIGGE test or operational and matches entry in TIGGE table:
		 * <parameter_short_name> (ex: t)
		 * else if entry matching product discipline, parameter category, and parameter number is found:
		 * <parameter_short_name> (ex: TMP)
		 * else:
		 * VAR_<product_discipline_number>_<parameter_category_number>_<parameter_number> (ex: VAR_3_0_9)
		 * 
		 * _P<product_definition_template_number> (ex: TMP_P0)
		 * 
		 * if single level type:
		 * _L<level_type_number> (ex: TMP_P0_L103)
		 * else if two levels of the same type:
		 * _2L<level_type_number> (ex: TMP_P0_2L106)
		 * else if two levels of different types:
		 * _2L<_first_level_type_number>_<second_level_type_number> (ex: LCLD_P0_2L212_213)
		 * 
		 * if grid type is supported (fully or partially):
		 * _G<grid_abbreviation><grid_number> (ex: UGRD_P0_L108_GLC0)
		 * else:
		 * _G<grid_number> (ex: UGRD_P0_2L104_G0)
		 * 
		 * if not statistically processed variable and not duplicate name the name is complete at this point.
		 * 
		 * if statistically-processed variable and constant statistical processing duration:
		 * if statistical processing type is defined:
		 * _<statistical_processing_type_abbreviation><statistical_processing_duration><duration_units> (ex:
		 * APCP_P8_L1_GLL0_acc3h)
		 * else
		 * _<statistical_processing_duration><duration_units> (ex: TMAX_P8_L103_GCA0_6h)
		 * else if statistically-processed variable and variable-duration processing always begins at initial time:
		 * _<statistical_processing_type_abbreviation> (ex: ssr_P11_GCA0_acc)
		 * 
		 * if variable name is duplicate of existing variable name (this should not normally occur):
		 * _n (where n begins with 1 for first duplicate) (ex: TMAX_P8_L103_GCA0_6h_1)
		 * 
		 * VAR_%d-%d-%d[_error][_L%d][_layer][_I%s_S%d][_D%d][_Prob_%s]
		 * %d-%d-%d = discipline-category-paramNo
		 * L = level type
		 * S = stat type
		 * D = derived type
		 */
		public override string makeVariableId(GribCollectionImmutable.VariableIndex vindex)
		{
			return makeVariableId(vindex, this);
		}

		private static string makeVariableId(GribCollectionImmutable.VariableIndex vindex, GribCollectionImmutable gc)
		{
			using (Formatter f = new Formatter())
			{

				f.format("VAR_%d-%d-%d", vindex.Discipline, vindex.Category, vindex.Parameter);

				if (vindex.GenProcessType == 6 || vindex.GenProcessType == 7)
				{
					f.format("_error"); // its an "error" type variable - add to name
				}

				if (vindex.LevelType != GribNumbers.UNDEFINED)
				{ // satellite data doesnt have a level
					f.format("_L%d", vindex.LevelType); // code table 4.5
					if (vindex.Layer)
					{
						f.format("_layer");
					}
				}

				string intvName = vindex.IntvName;
				if (!string.ReferenceEquals(intvName, null) && intvName.Length > 0)
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

				if (vindex.IntvType >= 0)
				{
					f.format("_S%s", vindex.IntvType);
				}

				if (vindex.EnsDerivedType >= 0)
				{
					f.format("_D%d", vindex.EnsDerivedType);
				}
				else if (!string.ReferenceEquals(vindex.ProbabilityName, null) && vindex.ProbabilityName.Length > 0)
				{
					string s = StringUtil2.substitute(vindex.ProbabilityName, ".", "p");
					f.format("_Prob_%s", s);
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
			Grib2Tables cust2 = (Grib2Tables)gc.cust;

			v.addAttribute(new NcAttribute(Grib.VARIABLE_ID_ATTNAME, gc.makeVariableId(vindex)));
			int[] param = new int[] { vindex.Discipline, vindex.Category, vindex.Parameter };
			v.addAttribute(new NcAttribute("Grib2_Parameter", NcArray.makeFromJavaArray(param, false)));
			string disc = cust2.getCodeTableValue("0.0", vindex.Discipline);
			if (!string.ReferenceEquals(disc, null))
			{
				v.addAttribute(new NcAttribute("Grib2_Parameter_Discipline", disc));
			}
			string cat = cust2.getCategory(vindex.Discipline, vindex.Category);
			if (!string.ReferenceEquals(cat, null))
			{
				v.addAttribute(new NcAttribute("Grib2_Parameter_Category", cat));
			}
			GribTables.Parameter entry = cust2.getParameter(vindex);
			if (entry != null)
			{
				v.addAttribute(new NcAttribute("Grib2_Parameter_Name", entry.Name));
			}

			if (vindex.LevelType != GribNumbers.MISSING)
			{
				v.addAttribute(new NcAttribute("Grib2_Level_Type", vindex.LevelType));
			}
			string ldesc = cust2.getLevelName(vindex.LevelType);
			if (!string.ReferenceEquals(ldesc, null))
			{
				v.addAttribute(new NcAttribute("Grib2_Level_Desc", ldesc));
			}

			if (vindex.EnsDerivedType >= 0)
			{
				v.addAttribute(new NcAttribute("Grib2_Ensemble_Derived_Type", vindex.EnsDerivedType));
			}
			else if (!string.ReferenceEquals(vindex.ProbabilityName, null) && vindex.ProbabilityName.Length > 0)
			{
				v.addAttribute(new NcAttribute("Grib2_Probability_Type", vindex.ProbType));
				v.addAttribute(new NcAttribute("Grib2_Probability_Name", vindex.ProbabilityName));
			}

			if (vindex.GenProcessType >= 0)
			{
				string genProcessTypeName = cust2.getGeneratingProcessTypeName(vindex.GenProcessType);
				if (!string.ReferenceEquals(genProcessTypeName, null))
				{
					v.addAttribute(new NcAttribute("Grib2_Generating_Process_Type", genProcessTypeName));
				}
				else
				{
					v.addAttribute(new NcAttribute("Grib2_Generating_Process_Type", vindex.GenProcessType));
				}
			}

			string statType = cust2.getStatisticName(vindex.IntvType);
			if (!string.ReferenceEquals(statType, null))
			{
				v.addAttribute(new NcAttribute("Grib2_Statistical_Process_Type", statType));
			}

		}

	}

}
