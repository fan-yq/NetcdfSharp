// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 2011-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using NetcdfSharp.ucar.nc2.grib;
	using NetcdfSharp.ucar.nc2.grib.grib2;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using HTTPRandomAccessFile = NetcdfSharp.ucar.unidata.io.http.HTTPRandomAccessFile;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Grib-2 Collection IOSP.
	/// Handles both collections and single GRIB files.
	/// 
	/// @author caron
	/// @since 4/6/11
	/// </summary>
	public class Grib2Iosp : GribIosp
	{
		internal static string makeVariableNameFromTable(Grib2Tables cust, GribCollectionImmutable gribCollection, GribCollectionImmutable.VariableIndex vindex, bool useGenType)
		{

			using (Formatter f = new Formatter())
			{
				GribTables.Parameter param = cust.getParameter(vindex);

				if (param == null)
				{
					f.format("VAR%d-%d-%d_FROM_%d-%d-%d", vindex.Discipline, vindex.Category, vindex.Parameter, gribCollection.Center, gribCollection.Subcenter, vindex.TableVersion);
				}
				else
				{
					f.format("%s", GribUtils.makeNameFromDescription(param.Name));
				}

				if (vindex.GenProcessType == 6 || vindex.GenProcessType == 7)
				{
					f.format("_error"); // its an "error" type variable - add to name

				}
				else if (useGenType && vindex.GenProcessType >= 0)
				{
					string genType = cust.getGeneratingProcessTypeName(vindex.GenProcessType);
					string s = genType.Replace(" ", "_");
					f.format("_%s", s);
				}

				if (vindex.LevelType != GribNumbers.UNDEFINED)
				{ // satellite data doesnt have a level
					f.format("_%s", cust.getLevelNameShort(vindex.LevelType)); // vindex.getLevelType()); // code table 4.5
					if (vindex.Layer)
					{
						f.format("_layer");
					}
				}

				string intvName = vindex.IntvName;
				if (!string.ReferenceEquals(intvName, null) && intvName.Length > 0)
				{
					f.format("_%s", intvName);
				}

				if (vindex.IntvType >= 0)
				{
					string statName = cust.getStatisticNameShort(vindex.IntvType);
					if (!string.ReferenceEquals(statName, null))
					{
						f.format("_%s", statName);
					}
				}

				if (vindex.SpatialStatisticalProcessType >= 0)
				{
					string statName = cust.getCodeTableValue("4.10", vindex.SpatialStatisticalProcessType);
					if (!string.ReferenceEquals(statName, null))
					{
						f.format("_%s", statName);
					}
				}

				if (vindex.EnsDerivedType >= 0)
				{
					f.format("_%s", cust.getProbabilityNameShort(vindex.EnsDerivedType));
				}
				else if (!string.ReferenceEquals(vindex.ProbabilityName, null) && vindex.ProbabilityName.Length > 0)
				{
					string s = vindex.ProbabilityName.Replace(".", "p");
					f.format("_probability_%s", s);
				}
				else if (vindex.Ensemble)
				{
					f.format("_ens");
				}

				if (vindex.PercentileValue >= 0)
				{
					f.format("_Percentile%2d", vindex.PercentileValue);
				}
				return f.ToString();
			}
		}

		internal static string makeVariableLongName(Grib2Tables cust, GribCollectionImmutable.VariableIndex vindex, bool useGenType)
		{

			using (Formatter f = new Formatter())
			{
				bool isProb = (!string.ReferenceEquals(vindex.ProbabilityName, null) && vindex.ProbabilityName.Length > 0);
				if (isProb)
				{
					f.format("Probability ");
				}

				GribTables.Parameter gp = cust.getParameter(vindex);
				if (gp == null)
				{
					f.format("Unknown Parameter %d-%d-%d", vindex.Discipline, vindex.Category, vindex.Parameter);
				}
				else
				{
					f.format("%s", gp.Name);
				}

				if (vindex.IntvType >= 0 && !string.ReferenceEquals(vindex.IntvName, null) && vindex.IntvName.Length > 0)
				{
					string intvName = cust.getStatisticNameShort(vindex.IntvType);
					if (string.ReferenceEquals(intvName, null) || intvName.Equals("Missing", StringComparison.OrdinalIgnoreCase))
					{
						intvName = cust.getStatisticNameShort(vindex.IntvType);
					}
					if (string.ReferenceEquals(intvName, null))
					{
						f.format(" (%s)", vindex.IntvName);
					}
					else
					{
						f.format(" (%s %s)", vindex.IntvName, intvName);
					}

				}
				else if (vindex.IntvType >= 0)
				{
					string intvName = cust.getStatisticNameShort(vindex.IntvType);
					f.format(" (%s)", intvName);
				}

				if (vindex.SpatialStatisticalProcessType >= 0)
				{
					string statName = cust.getCodeTableValue("4.10", vindex.SpatialStatisticalProcessType);
					if (!string.ReferenceEquals(statName, null))
					{
						f.format("_%s", statName);
					}
				}

				if (vindex.EnsDerivedType >= 0)
				{
					f.format(" (%s)", cust.getCodeTableValue("4.10", vindex.EnsDerivedType));
				}
				else if (isProb)
				{
					f.format(" %s %s", vindex.ProbabilityName, getVindexUnits(cust, vindex)); // add data units here
				}

				if (vindex.GenProcessType == 6 || vindex.GenProcessType == 7)
				{
					f.format(" error"); // its an "error" type variable - add to name

				}
				else if (useGenType && vindex.GenProcessType >= 0)
				{
					f.format(" %s", cust.getGeneratingProcessTypeName(vindex.GenProcessType));
				}

				if (vindex.PercentileValue >= 0)
				{
					f.format(" %d Percentile", vindex.PercentileValue);
				}

				if (vindex.LevelType != GribNumbers.UNDEFINED)
				{ // satellite data doesnt have a level
					f.format(" @ %s", cust.getCodeTableValue("4.5", vindex.LevelType));
					if (vindex.Layer)
					{
						f.format(" layer");
					}
				}
				return f.ToString();
			}
		}

		public override string makeVariableName(GribCollectionImmutable.VariableIndex vindex)
		{
			return makeVariableNameFromTable(cust, gribCollection, vindex, gribCollection.config.gribConfig.useGenType);
		}

		public override string makeVariableLongName(GribCollectionImmutable.VariableIndex vindex)
		{
			return makeVariableLongName(cust, vindex, gribCollection.config.gribConfig.useGenType);
		}

		protected internal override string makeVariableUnits(GribCollectionImmutable.VariableIndex vindex)
		{
			return makeVariableUnits(cust, vindex);
		}

		internal static string makeVariableUnits(Grib2Tables tables, GribCollectionImmutable.VariableIndex vindex)
		{
			if (!string.ReferenceEquals(vindex.ProbabilityName, null) && vindex.ProbabilityName.Length > 0)
			{
				return "%";
			}
			return getVindexUnits(tables, vindex);
		}

		private static string getVindexUnits(Grib2Tables tables, GribCollectionImmutable.VariableIndex vindex)
		{
			GribTables.Parameter gp = tables.getParameter(vindex);
			string val = (gp == null) ? "" : gp.Unit;
			return (string.ReferenceEquals(val, null)) ? "" : val;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private Grib2Tables cust;

		// accept grib2 or ncx files
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
		public override bool isValidFile(RandomAccessFile raf)
		{
			// Remote (HTTP) reading is out of scope: always the local branch (was `raf is HTTPRandomAccessFile`).
			{
				// wont accept remote index
				GribCdmIndex.GribCollectionType type = GribCdmIndex.getType(raf);
				if (type == GribCdmIndex.GribCollectionType.GRIB2)
				{
					return true;
				}
				if (type == GribCdmIndex.GribCollectionType.Partition2)
				{
					return true;
				}
			}

			// check for GRIB2 data file
			return Grib2RecordScanner.isValidFile(raf);
		}

		public override string FileTypeId
		{
			get
			{
				return DataFormatType.GRIB2.Description;
			}
		}

		public override string FileTypeDescription
		{
			get
			{
				return "GRIB2 Collection";
			}
		}

		// public no-arg constructor for reflection
		public Grib2Iosp() : base(false, NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2Iosp)))
		{
		}

		internal Grib2Iosp(GribCollectionImmutable.GroupGC gHcs, GribCollectionImmutable.GribType gtype) : base(false, NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2Iosp)))
		{
			this.gHcs = gHcs;
			this.owned = true;
			this.gtype = gtype;
		}

		// LOOK more likely we will set an individual dataset
		internal Grib2Iosp(GribCollectionImmutable gc) : base(false, NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2Iosp)))
		{
			this.gribCollection = gc;
			this.owned = true;
		}

		protected internal override NetcdfSharp.ucar.nc2.grib.GribTables createCustomizer()
		{
			cust = Grib2Tables.factory(gribCollection.Center, gribCollection.Subcenter, gribCollection.Master, gribCollection.Local, gribCollection.GenProcessId);
			return cust;
		}

		protected internal override string getVerticalCoordDesc(int vc_code)
		{
			return cust.getCodeTableValue("4.5", vc_code);
		}

		protected internal override GribTables.Parameter getParameter(GribCollectionImmutable.VariableIndex vindex)
		{
			return cust.getParameter(vindex);
		}

		public override object LastRecordRead
		{
			get
			{
				return Grib2Record.lastRecordRead;
			}
		}

		public override void clearLastRecordRead()
		{
			Grib2Record.lastRecordRead = null;
		}

		public override object GribCustomizer
		{
			get
			{
				return cust;
			}
		}
	}

}
