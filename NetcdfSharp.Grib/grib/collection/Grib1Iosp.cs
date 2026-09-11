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
	using NetcdfSharp.ucar.nc2.grib;
	using NetcdfSharp.ucar.nc2.grib.grib1;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib1ParamTables = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1ParamTables;
	using HTTPRandomAccessFile = NetcdfSharp.ucar.unidata.io.http.HTTPRandomAccessFile;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Grib-1 Collection IOSP.
	/// Handles both collections and single GRIB files.
	/// 
	/// @author caron
	/// @since 4/6/11
	/// </summary>
	public class Grib1Iosp : GribIosp
	{
		public override string makeVariableName(GribCollectionImmutable.VariableIndex v)
		{
			return makeVariableNameFromTables(gribCollection.Center, gribCollection.Subcenter, v.TableVersion, v.Parameter, v.LevelType, v.Layer, v.IntvType, v.IntvName);
		}

		public static string makeVariableName(Grib1Customizer cust, FeatureCollectionConfig.GribConfig gribConfig, Grib1SectionProductDefinition pds)
		{
			return makeVariableNameFromTables(cust, gribConfig, pds.Center, pds.SubCenter, pds.TableVersion, pds.ParameterNumber, pds.LevelType, cust.isLayer(pds.LevelType), pds.TimeRangeIndicator, null);
		}

		private string makeVariableNameFromTables(int center, int subcenter, int version, int paramNo, int levelType, bool isLayer, int intvType, string intvName)
		{
			return makeVariableNameFromTables(cust, config.gribConfig, center, subcenter, version, paramNo, levelType, isLayer, intvType, intvName);
		}

		private static string makeVariableNameFromTables(Grib1Customizer cust, FeatureCollectionConfig.GribConfig gribConfig, int center, int subcenter, int version, int paramNo, int levelType, bool isLayer, int timeRangeIndicator, string intvName)
		{
			using (Formatter f = new Formatter())
			{

				Grib1Parameter param = cust.getParameter(center, subcenter, version, paramNo); // code table 2
				if (param == null)
				{
					f.format("VAR%d-%d-%d-%d", center, subcenter, version, paramNo);
				}
				else
				{
					if (param.useName())
					{
						f.format("%s", param.Name);
					}
					else
					{
						f.format("%s", GribUtils.makeNameFromDescription(param.Description));
					}
				}

				if (gribConfig.useTableVersion)
				{
					f.format("_TableVersion%d", version);
				}

				if (gribConfig.useCenter)
				{
					f.format("_Center%d", center);
				}

				if (levelType != GribNumbers.UNDEFINED)
				{ // satellite data doesnt have a level
					f.format("_%s", cust.getLevelNameShort(levelType)); // code table 3
					if (isLayer)
					{
						f.format("_layer");
					}
				}

				if (timeRangeIndicator >= 0)
				{
					GribStatType stat = cust.getStatType(timeRangeIndicator);
					if (stat != null)
					{
						if (!string.ReferenceEquals(intvName, null))
						{
							f.format("_%s", intvName);
						}
						f.format("_%s", stat.ToString());
					}
					else
					{
						if (!string.ReferenceEquals(intvName, null))
						{
							f.format("_%s", intvName);
						}
						// f.format("_%d", timeRangeIndicator);
					}
				}

				return f.ToString();
			}
		}

		public override string makeVariableLongName(GribCollectionImmutable.VariableIndex v)
		{
			return makeVariableLongName(gribCollection.Center, gribCollection.Subcenter, v.TableVersion, v.Parameter, v.LevelType, v.Layer, v.IntvType, v.IntvName, v.ProbabilityName);
		}


		private string makeVariableLongName(int center, int subcenter, int version, int paramNo, int levelType, bool isLayer, int intvType, string intvName, string probabilityName)
		{
			return makeVariableLongName(cust, center, subcenter, version, paramNo, levelType, isLayer, intvType, intvName, probabilityName);
		}

		internal static string makeVariableLongName(Grib1Customizer cust, int center, int subcenter, int version, int paramNo, int levelType, bool isLayer, int intvType, string intvName, string probabilityName)
		{
			using (Formatter f = new Formatter())
			{

				bool isProb = (!string.ReferenceEquals(probabilityName, null) && probabilityName.Length > 0);
				if (isProb)
				{
					f.format("Probability ");
				}

				Grib1Parameter param = cust.getParameter(center, subcenter, version, paramNo);
				if (param == null)
				{
					f.format("Unknown Parameter %d-%d-%d-%d", center, subcenter, version, paramNo);
				}
				else
				{
					f.format("%s", param.Description);
				}

				if (intvType >= 0)
				{
					GribStatType stat = cust.getStatType(intvType);
					if (stat != null)
					{
						f.format(" (%s %s)", intvName, stat.ToString());
					}
					else if (!string.ReferenceEquals(intvName, null) && intvName.Length > 0)
					{
						f.format(" (%s)", intvName);
					}
				}

				if (levelType != GribNumbers.UNDEFINED)
				{ // satellite data doesnt have a level
					f.format(" @ %s", cust.getLevelDescription(levelType));
					if (isLayer)
					{
						f.format(" layer");
					}
				}

				return f.ToString();
			}
		}

		protected internal override string makeVariableUnits(GribCollectionImmutable.VariableIndex vindex)
		{
			return makeVariableUnits(gribCollection.Center, gribCollection.Subcenter, vindex.TableVersion, vindex.Parameter);
		}

		private string makeVariableUnits(int center, int subcenter, int version, int paramNo)
		{
			Grib1Parameter param = cust.getParameter(center, subcenter, version, paramNo);
			string val = (param == null) ? "" : param.Unit;
			return (string.ReferenceEquals(val, null)) ? "" : val;
		}

		internal static string makeVariableUnits(Grib1Customizer cust, GribCollectionImmutable gribCollection, GribCollectionImmutable.VariableIndex vindex)
		{
			Grib1Parameter param = cust.getParameter(gribCollection.Center, gribCollection.Subcenter, vindex.TableVersion, vindex.Parameter);
			string val = (param == null) ? "" : param.Unit;
			return (string.ReferenceEquals(val, null)) ? "" : val;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private Grib1Customizer cust;

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
		public override bool isValidFile(RandomAccessFile raf)
		{
			// Remote (HTTP) reading is out of scope: always the local branch (was `raf is HTTPRandomAccessFile`).
			{
				// wont accept remote index
				GribCdmIndex.GribCollectionType type = GribCdmIndex.getType(raf);
				if (type == GribCdmIndex.GribCollectionType.GRIB1)
				{
					return true;
				}
				if (type == GribCdmIndex.GribCollectionType.Partition1)
				{
					return true;
				}
			}

			// check for GRIB1 data file
			return Grib1RecordScanner.isValidFile(raf);
		}

		public override string FileTypeId
		{
			get
			{
				return DataFormatType.GRIB1.Description;
			}
		}

		public override string FileTypeDescription
		{
			get
			{
				return "GRIB1 Collection";
			}
		}

		// public no-arg constructor for reflection
		public Grib1Iosp() : base(true, NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2Iosp)))
		{
		}

		public Grib1Iosp(GribCollectionImmutable.GroupGC gHcs, GribCollectionImmutable.GribType gtype) : base(true, NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2Iosp)))
		{
			this.gHcs = gHcs;
			this.owned = true;
			this.gtype = gtype;
		}

		public Grib1Iosp(GribCollectionImmutable gc) : base(true, NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2Iosp)))
		{
			this.gribCollection = gc;
			this.owned = true;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override protected NetcdfSharp.ucar.nc2.grib.GribTables createCustomizer() throws NetcdfSharp.java.io.IOException
		protected internal override NetcdfSharp.ucar.nc2.grib.GribTables createCustomizer()
		{
			Grib1ParamTables tables = (config.gribConfig.paramTable != null) ? Grib1ParamTables.factory(config.gribConfig.paramTable) : Grib1ParamTables.factory(config.gribConfig.paramTablePath, config.gribConfig.lookupTablePath); // so an
																																																									   // iosp
																																																									   // message
																																																									   // must be
																																																									   // received
																																																									   // before
																																																									   // the
																																																									   // open()

			cust = Grib1Customizer.factory(gribCollection.Center, gribCollection.Subcenter, gribCollection.Master, tables);
			return cust;
		}

		protected internal override string getVerticalCoordDesc(int vc_code)
		{
			return cust.getLevelDescription(vc_code);
		}

		protected internal override GribTables.Parameter getParameter(GribCollectionImmutable.VariableIndex vindex)
		{
			return cust.getParameter(gribCollection.Center, gribCollection.Subcenter, gribCollection.Version, vindex.Parameter);
		}

		public override object LastRecordRead
		{
			get
			{
				return Grib1Record.lastRecordRead;
			}
		}

		public override void clearLastRecordRead()
		{
			Grib1Record.lastRecordRead = null;
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
