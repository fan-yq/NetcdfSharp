// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf4
{
	using _Coordinate = NetcdfSharp.ucar.nc2.constants._Coordinate;
	using AttributeContainerMutable = NetcdfSharp.ucar.nc2.AttributeContainerMutable;
	using AxisType = NetcdfSharp.ucar.nc2.constants.AxisType;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using CoordSystemBuilder = NetcdfSharp.ucar.nc2.@internal.dataset.CoordSystemBuilder;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using Dimension = NetcdfSharp.ucar.nc2.Dimension;
	using Element = NetcdfSharp.org.jdom2.Element;
	using FeatureType = NetcdfSharp.ucar.nc2.constants.FeatureType;
	using Group = NetcdfSharp.ucar.nc2.Group;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using NetcdfFiles = NetcdfSharp.ucar.nc2.NetcdfFiles;
	using ODLparser = NetcdfSharp.ucar.nc2.iosp.hdf4.ODLparser;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Parse structural metadata from HDF-EOS.
	/// This allows us to use shared dimensions, identify Coordinate Axes, and the FeatureType.
	/// <para>
	/// </para>
	/// <para>
	/// from HDF-EOS.status.ppt:
	/// 
	/// <pre>
	/// HDF-EOS is format for EOS  Standard Products
	/// <ul>
	/// <li>Landsat 7 (ETM+)
	/// <li>Terra (CERES, MISR, MODIS, ASTER, MOPITT)
	/// <li>Meteor-3M (SAGE III)
	/// <li>Aqua (AIRS, AMSU-A, AMSR-E, CERES, MODIS)
	/// <li>Aura(MLS, TES, HIRDLS, OMI
	/// </ul>
	/// HDF is used by other EOS missions
	/// <ul>
	/// <li>OrbView 2 (SeaWIFS)
	/// <li>TRMM (CERES, VIRS, TMI, PR)
	/// <li>Quickscat (SeaWinds)
	/// <li>EO-1 (Hyperion, ALI)
	/// <li>ICESat (GLAS)
	/// <li>Calypso
	/// </ul>
	/// </pre>
	/// </para>
	/// 
	/// @author caron
	/// @since Jul 23, 2007
	/// </summary>
	public class HdfEos
	{
		public const string HDF5_GROUP = "HDFEOS_INFORMATION";
		public const string HDFEOS_CRS = "_HDFEOS_CRS";
		public const string HDFEOS_CRS_Projection = "Projection";
		public const string HDFEOS_CRS_UpperLeft = "UpperLeftPointMtrs";
		public const string HDFEOS_CRS_LowerRight = "LowerRightMtrs";
		public const string HDFEOS_CRS_ProjParams = "ProjParams";
		public const string HDFEOS_CRS_SphereCode = "SphereCode";

		private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(HdfEos));
		internal static bool showWork; // set in debug
		private const string GEOLOC_FIELDS = "Geolocation Fields";
		private const string GEOLOC_FIELDS2 = "Geolocation_Fields";
		private const string DATA_FIELDS = "Data Fields";
		private const string DATA_FIELDS2 = "Data_Fields";

		/// <summary>
		/// Amend the given NetcdfFile with metadata from HDF-EOS structMetadata.
		/// All Variables named StructMetadata.n, where n= 1, 2, 3 ... are read in and their contents concatenated
		/// to make the structMetadata String.
		/// </summary>
		/// <param name="eosGroup"> the group containing variables named StructMetadata.* </param>
		/// <returns> true if HDF-EOS info was found </returns>
		/// <exception cref="IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static boolean amendFromODL(String location, HdfHeaderIF header, NetcdfSharp.ucar.nc2.Group.Builder eosGroup) throws NetcdfSharp.java.io.IOException
		public static bool amendFromODL(string location, HdfHeaderIF header, Group.Builder eosGroup)
		{
			string smeta = getStructMetadata(header, eosGroup);
			if (string.ReferenceEquals(smeta, null))
			{
				return false;
			}

			HdfEos fixer = new HdfEos(location, header);
			fixer.fixAttributes(eosGroup);
			fixer.amendFromODL(header.RootGroup, smeta);
			return true;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static boolean getEosInfo(String location, HdfHeaderIF header, NetcdfSharp.ucar.nc2.Group.Builder eosGroup, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
		public static bool getEosInfo(string location, HdfHeaderIF header, Group.Builder eosGroup, Formatter f)
		{
			string smeta = getStructMetadata(header, eosGroup);
			if (string.ReferenceEquals(smeta, null))
			{
				f.format("No StructMetadata variables in group %s %n", eosGroup.shortName);
				return false;
			}
			f.format("raw = %n%s%n", smeta);
			ODLparser parser = new ODLparser();
			parser.parseFromString(smeta); // now we have the ODL as JDOM elements
			StringWriter sw = new StringWriter();
			parser.showDoc(new PrintWriter(sw));
			f.format("parsed = %n%s%n", sw.ToString());
			return true;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private static String getStructMetadata(HdfHeaderIF header, NetcdfSharp.ucar.nc2.Group.Builder eosGroup) throws NetcdfSharp.java.io.IOException
		private static string getStructMetadata(HdfHeaderIF header, Group.Builder eosGroup)
		{
			StringBuilder sbuff = null;
			string structMetadata = null;

			int n = 0;
			while (true)
			{
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: NetcdfSharp.ucar.nc2.Variable.Builder<?> structMetadataVar = eosGroup.findVariableLocal("StructMetadata." + n).orElse(null);
				Variable.Builder structMetadataVar = eosGroup.findVariableLocal("StructMetadata." + n).orElse(null);
				if (structMetadataVar == null)
				{
					break;
				}
				if (!string.ReferenceEquals(structMetadata, null) && sbuff == null)
				{ // more than 1 StructMetadata
					sbuff = new StringBuilder(64000);
					sbuff.Append(structMetadata);
				}

				// Since we dont have a Variable yet, we have to do something low-level
				structMetadata = header.readStructMetadata(structMetadataVar);

				if (sbuff != null)
				{
					sbuff.Append(structMetadata);
				}
				n++;
			}
			return (sbuff != null) ? sbuff.ToString() : structMetadata;
		}

		///////////////////////////////////////////
		private string location; // for debug messages
		private HdfHeaderIF header;

		private HdfEos(string location, HdfHeaderIF header)
		{
			this.location = location;
			this.header = header;
		}

		/// <summary>
		/// Amend the given NetcdfFile with metadata from HDF-EOS structMetadata
		/// </summary>
		/// <param name="rootg"> Amend this </param>
		/// <param name="structMetadata"> structMetadata as String </param>
		private void amendFromODL(Group.Builder rootg, string structMetadata)
		{
			ODLparser parser = new ODLparser();
			Element root = parser.parseFromString(structMetadata); // now we have the ODL in JDOM elements
			FeatureType featureType = null;

			// SWATH
			Element swathStructure = root.getChild("SwathStructure");
			if (swathStructure != null)
			{
				IList<Element> swaths = swathStructure.getChildren();
				foreach (Element elemSwath in swaths)
				{
					Element swathNameElem = elemSwath.getChild("SwathName");
					if (swathNameElem == null)
					{
						log.warn("No SwathName element in {} {} ", elemSwath.getName(), location);
						continue;
					}
					string swathName = NetcdfFiles.makeValidCdmObjectName(swathNameElem.getText().Trim());
					Group.Builder swathGroup = findGroupNested(rootg, swathName);
					// if (swathGroup == null)
					// swathGroup = findGroupNested(rootg, HdfHeaderIF.createValidObjectName(swathName));

					if (swathGroup != null)
					{
						featureType = amendSwath(elemSwath, swathGroup);
					}
					else
					{
						log.warn("Cant find swath group {} {}", swathName, location);
					}
				}
			}

			// GRID
			Element gridStructure = root.getChild("GridStructure");
			if (gridStructure != null)
			{
				IList<Element> grids = gridStructure.getChildren();
				foreach (Element elemGrid in grids)
				{
					Element gridNameElem = elemGrid.getChild("GridName");
					if (gridNameElem == null)
					{
						log.warn("No GridName element in {} {} ", elemGrid.getName(), location);
						continue;
					}
					string gridName = NetcdfFiles.makeValidCdmObjectName(gridNameElem.getText().Trim());
					Group.Builder gridGroup = findGroupNested(rootg, gridName);
					// if (gridGroup == null)
					// gridGroup = findGroupNested(rootg, HdfHeaderIF.createValidObjectName(gridName));
					if (gridGroup != null)
					{
						featureType = amendGrid(elemGrid, gridGroup, location);
					}
					else
					{
						log.warn("Cant find Grid group {} {}", gridName, location);
					}
				}
			}

			// POINT - NOT DONE YET
			Element pointStructure = root.getChild("PointStructure");
			if (pointStructure != null)
			{
				IList<Element> pts = pointStructure.getChildren();
				foreach (Element elem in pts)
				{
					Element nameElem = elem.getChild("PointName");
					if (nameElem == null)
					{
						log.warn("No PointName element in {} {}", elem.getName(), location);
						continue;
					}
					string name = nameElem.getText().Trim();
					Group.Builder ptGroup = findGroupNested(rootg, name);
					// if (ptGroup == null)
					// ptGroup = findGroupNested(rootg, HdfHeaderIF.createValidObjectName(name));
					if (ptGroup != null)
					{
						featureType = FeatureType.POINT;
					}
					else
					{
						log.warn("Cant find Point group {} {}", name, location);
					}
				}
			}

			if (featureType != null)
			{
				if (showWork)
				{
					log.debug("***EOS featureType= {}", featureType);
				}
				rootg.addAttribute(new NcAttribute(CF.FEATURE_TYPE, featureType.ToString()));
				// rootg.addAttribute(new Attribute(CDM.CONVENTIONS, "HDFEOS"));
			}

		}

		private FeatureType amendSwath(Element swathElem, Group.Builder parent)
		{
			FeatureType featureType = FeatureType.SWATH;
			IList<Dimension> unknownDims = new List<Dimension>();

			// Dimensions
			Element d = swathElem.getChild("Dimension");
			IList<Element> dims = d.getChildren();
			foreach (Element elem in dims)
			{
				string name = elem.getChild("DimensionName").getText().Trim();
				name = NetcdfFiles.makeValidCdmObjectName(name);

				if (name.Equals("scalar", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				string sizeS = elem.getChild("Size").getText().Trim();
				int length = int.Parse(sizeS);
				if (length > 0)
				{
					Dimension dim = parent.findDimensionLocal(name).orElse(null);
					if (dim != null)
					{ // already added - may be dimension scale ?
						if (dim.Length != length)
						{ // ok as long as it matches
							log.error("Conflicting Dimensions = {} != {} in location {}", dim, length, location);
							// Assume that the ODL is incorrect.
							// throw new IllegalStateException("Conflicting Dimensions = " + name);
						}
					}
					else
					{
						dim = new Dimension(name, length);
						if (parent.addDimensionIfNotExists(dim))
						{
							if (showWork)
							{
								log.debug(" Add dimension {}", dim);
							}
						}
					}
				}
				else
				{
					log.warn("Dimension {} has size {} {}", name, sizeS, location);
					Dimension udim = new Dimension(name, 1);
					unknownDims.Add(udim);
					if (showWork)
					{
						log.debug(" Add dimension {}", udim);
					}
				}
			}

			// Dimension Maps
			Element dmap = swathElem.getChild("DimensionMap");
			IList<Element> dimMaps = dmap.getChildren();
			foreach (Element elem in dimMaps)
			{
				string geoDimName = elem.getChild("GeoDimension").getText().Trim();
				geoDimName = NetcdfFiles.makeValidCdmObjectName(geoDimName);
				string dataDimName = elem.getChild("DataDimension").getText().Trim();
				dataDimName = NetcdfFiles.makeValidCdmObjectName(dataDimName);

				string offsetS = elem.getChild("Offset").getText().Trim();
				string incrS = elem.getChild("Increment").getText().Trim();
				int offset = int.Parse(offsetS);
				int incr = int.Parse(incrS);

				// make new variable for this dimension map
				Variable.Builder v = Variable.builder().setName(dataDimName);
				parent.addVariable(v);
				v.setDimensionsByName(geoDimName);
				v.setDataType(DataType.INT);
				v.setAutoGen(offset, incr);
				v.addAttribute(new NcAttribute("_DimensionMap", ""));
				header.makeVinfoForDimensionMapVariable(parent, v);

				if (showWork)
				{
					log.debug(" Add dimensionMap {}", v);
				}
			}

			// Geolocation Variables
			Group.Builder geoFieldsG = parent.findGroupLocal(GEOLOC_FIELDS).orElse(parent.findGroupLocal(GEOLOC_FIELDS2).orElse(null));
			if (geoFieldsG != null)
			{
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: NetcdfSharp.ucar.nc2.Variable.Builder<?> latAxis = null;
				Variable.Builder latAxis = null;
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: NetcdfSharp.ucar.nc2.Variable.Builder<?> lonAxis = null;
				Variable.Builder lonAxis = null;
				// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
				// ORIGINAL LINE: NetcdfSharp.ucar.nc2.Variable.Builder<?> timeAxis = null;
				Variable.Builder timeAxis = null;

				Element floc = swathElem.getChild("GeoField");
				foreach (Element elem in floc.getChildren())
				{
					string varname = elem.getChild("GeoFieldName").getText().Trim();
					// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
					// ORIGINAL LINE: NetcdfSharp.ucar.nc2.Variable.Builder<?> vb = geoFieldsG.findVariableLocal(varname).orElse(null);
					Variable.Builder vb = geoFieldsG.findVariableLocal(varname).orElse(null);
					if (vb != null)
					{
						AxisType axis = addAxisType(vb);
						if (axis == AxisType.Lat)
						{
							latAxis = vb;
						}
						if (axis == AxisType.Lon)
						{
							lonAxis = vb;
						}
						if (axis == AxisType.Time)
						{
							timeAxis = vb;
						}

						Element dimList = elem.getChild("DimList");
						IList<Element> values = dimList.getChildren("value");
						setSharedDimensions(geoFieldsG, vb, values, unknownDims, location);
						if (showWork)
						{
							log.debug(" set coordinate {}", vb);
						}
					}

					// Treat possibility that this is a discrete geometry featureType.
					// We check if lat and lon axes are 2D and if not (1) see if it looks like a
					// trajectory, or (2) otherwise tag it as a profile.
					// This could/should be expanded to consider other FTs.
					if ((latAxis != null) && (lonAxis != null))
					{
						log.debug("found lonAxis and latAxis -- testing XY domain");
						int xyDomainSize = CoordSystemBuilder.countDomainSize(latAxis, lonAxis);
						log.debug("xyDomain size {}", xyDomainSize);
						if (xyDomainSize < 2)
						{
							if (timeAxis != null)
							{
								log.debug("found timeAxis -- testing if trajectory");
								string dd1 = timeAxis.getFirstDimensionName();
								string dd2 = latAxis.getFirstDimensionName();
								string dd3 = lonAxis.getFirstDimensionName();

								if (dd1.Equals(dd2) && dd1.Equals(dd3))
								{
									featureType = FeatureType.TRAJECTORY;
								}
								else
								{
									featureType = FeatureType.PROFILE; // ??
								}
							}
							else
							{
								featureType = FeatureType.PROFILE; // ??
							}
						}
					}
				}
			}

			// Data Variables
			Group.Builder dataG = parent.findGroupLocal(DATA_FIELDS).orElse(parent.findGroupLocal(DATA_FIELDS2).orElse(null));
			if (dataG != null)
			{
				Element f = swathElem.getChild("DataField");
				IList<Element> vars = f.getChildren();
				foreach (Element elem in vars)
				{
					Element dataFieldNameElem = elem.getChild("DataFieldName");
					if (dataFieldNameElem == null)
					{
						continue;
					}
					string varname = NetcdfFile.makeValidCdmObjectName(dataFieldNameElem.getText().Trim());
					Variable.Builder v = dataG.findVariableLocal(varname).orElse(null);
					if (v == null)
					{
						log.error("Cant find variable {} {}", varname, location);
						continue;
					}
					Element dimList = elem.getChild("DimList");
					IList<Element> values = dimList.getChildren("value");
					setSharedDimensions(dataG, v, values, unknownDims, location);
				}
			}

			return featureType;
		}

		private AxisType addAxisType(Variable.Builder v)
		{
			string name = v.shortName;
			if (name.Equals("Latitude", StringComparison.OrdinalIgnoreCase) || name.Equals("GeodeticLatitude", StringComparison.OrdinalIgnoreCase))
			{
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Lat.ToString()));
				v.addAttribute(new NcAttribute(CDM.UNITS, CDM.LAT_UNITS));
				return AxisType.Lat;

			}
			else if (name.Equals("Longitude", StringComparison.OrdinalIgnoreCase))
			{
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Lon.ToString()));
				v.addAttribute(new NcAttribute(CDM.UNITS, CDM.LON_UNITS));
				return AxisType.Lon;

			}
			else if (name.Equals("Time", StringComparison.OrdinalIgnoreCase))
			{
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Time.ToString()));
				if (v.getAttributeContainer().findAttribute(CDM.UNITS) == null)
				{
					/*
					 * from http://newsroom.gsfc.nasa.gov/sdptoolkit/hdfeosfaq.html
					 * HDF-EOS uses the TAI93 (International Atomic Time) format. This means that time is stored as the number of
					 * elapsed seconds since January 1, 1993 (negative values represent times prior to this date).
					 * An 8 byte floating point number is used, producing microsecond accuracy from 1963 (when leap second records
					 * became available electronically) to 2100. The SDP Toolkit provides conversions from other date formats to and
					 * from TAI93. Other representations of time can be entered as ancillary data, if desired.
					 * For lists and descriptions of other supported time formats, consult the Toolkit documentation or write to
					 * landover_PGSTLKIT@raytheon.com.
					 */
					v.addAttribute(new NcAttribute(CDM.UNITS, "seconds since 1993-01-01T00:00:00Z"));
					v.addAttribute(new NcAttribute(CF.CALENDAR, "TAI"));
					/*
					 * String tit = ncfile.findAttValueIgnoreCase(v, "Title", null);
					 * if (tit != null && tit.contains("TAI93")) {
					 * // Time is given in the TAI-93 format, i.e. the number of seconds passed since 01-01-1993, 00:00 UTC.
					 * v.addAttribute(new Attribute(CDM.UNITS, "seconds since 1993-01-01T00:00:00Z"));
					 * v.addAttribute(new Attribute(CF.CALENDAR, "TAI"));
					 * } else { // who the hell knows ??
					 * v.addAttribute(new Attribute(CDM.UNITS, "seconds since 1970-01-01T00:00:00Z"));
					 * }
					 */
				}
				return AxisType.Time;

			}
			else if (name.Equals("Pressure", StringComparison.OrdinalIgnoreCase))
			{
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Pressure.ToString()));
				return AxisType.Pressure;

			}
			else if (name.Equals("Altitude", StringComparison.OrdinalIgnoreCase))
			{
				v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Height.ToString()));
				v.addAttribute(new NcAttribute(CF.POSITIVE, CF.POSITIVE_UP)); // probably
				return AxisType.Height;
			}

			return null;
		}


		private FeatureType amendGrid(Element gridElem, Group.Builder parent, string location)
		{
			IList<Dimension> unknownDims = new List<Dimension>();

			// always has x and y dimension
			string xdimSizeS = gridElem.getChild("XDim").getText().Trim();
			string ydimSizeS = gridElem.getChild("YDim").getText().Trim();
			int xdimSize = int.Parse(xdimSizeS);
			int ydimSize = int.Parse(ydimSizeS);
			parent.addDimensionIfNotExists(new Dimension("XDim", xdimSize));
			parent.addDimensionIfNotExists(new Dimension("YDim", ydimSize));

			/*
			 * see HdfEosModisConvention
			 * UpperLeftPointMtrs=(-20015109.354000,1111950.519667)
			 * LowerRightMtrs=(-18903158.834333,-0.000000)
			 * Projection=GCTP_SNSOID
			 * ProjParams=(6371007.181000,0,0,0,0,0,0,0,0,0,0,0,0)
			 * SphereCode=-1
			 */
			Element proj = gridElem.getChild("Projection");
			if (proj != null)
			{
				Variable.Builder crs = Variable.builder().setName(HDFEOS_CRS);
				crs.setDataType(DataType.SHORT);
				crs.setIsScalar();
				crs.setAutoGen(0, 0); // fake data
				parent.addVariable(crs);

				addAttributeIfExists(gridElem, HDFEOS_CRS_Projection, crs, false);
				addAttributeIfExists(gridElem, HDFEOS_CRS_UpperLeft, crs, true);
				addAttributeIfExists(gridElem, HDFEOS_CRS_LowerRight, crs, true);
				addAttributeIfExists(gridElem, HDFEOS_CRS_ProjParams, crs, true);
				addAttributeIfExists(gridElem, HDFEOS_CRS_SphereCode, crs, false);
			}

			// global Dimensions
			Element d = gridElem.getChild("Dimension");
			IList<Element> dims = d.getChildren();
			foreach (Element elem in dims)
			{
				string name = elem.getChild("DimensionName").getText().Trim();
				name = NetcdfFiles.makeValidCdmObjectName(name);
				if (name.Equals("scalar", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				string sizeS = elem.getChild("Size").getText().Trim();
				int length = int.Parse(sizeS);
				var oldOpt = parent.findDimension(name);
				Dimension old = oldOpt.Present ? oldOpt.get() : null;
				if ((old == null) || (old.Length != length))
				{
					if (length > 0)
					{
						Dimension dim = new Dimension(name, length);
						if (parent.addDimensionIfNotExists(dim) && showWork)
						{
							log.debug(" Add dimension {}", dim);
						}
					}
					else
					{
						log.warn("Dimension {} has size {} {} ", sizeS, name, location);
						Dimension udim = new Dimension(name, 1);
						unknownDims.Add(udim);
						if (showWork)
						{
							log.debug(" Add dimension {}", udim);
						}
					}
				}
			}

			// Geolocation Variables
			var geoFieldsGopt = parent.findGroupLocal(GEOLOC_FIELDS);
			if (!geoFieldsGopt.Present)
			{
				geoFieldsGopt = parent.findGroupLocal(GEOLOC_FIELDS2);
			}
			Group.Builder geoFieldsG = geoFieldsGopt.Present ? geoFieldsGopt.get() : null;
			if (geoFieldsG != null)
			{
				Element floc = gridElem.getChild("GeoField");
				IList<Element> varsLoc = floc.getChildren();
				foreach (Element elem in varsLoc)
				{
					string varname = elem.getChild("GeoFieldName").getText().Trim();
					var vbOpt = geoFieldsG.findVariableLocal(varname);
					if (vbOpt.Present)
					{
						Variable.Builder vb = vbOpt.get();
						Element dimList = elem.getChild("DimList");
						IList<Element> values = dimList.getChildren("value");
						setSharedDimensions(geoFieldsG, vb, values, unknownDims, location);
					}
				}
			}

			// Data Variables
			var dataGopt = parent.findGroupLocal(DATA_FIELDS);
			if (!dataGopt.Present)
			{
				dataGopt = parent.findGroupLocal(DATA_FIELDS2);
			}
			Group.Builder dataG = dataGopt.Present ? dataGopt.get() : null;
			if (dataG != null)
			{
				Element f = gridElem.getChild("DataField");
				IList<Element> vars = f.getChildren();
				foreach (Element elem in vars)
				{
					string varname = elem.getChild("DataFieldName").getText().Trim();
					varname = NetcdfFiles.makeValidCdmObjectName(varname);
					var vbOpt = dataG.findVariableLocal(varname);
					if (vbOpt.Present)
					{
						Variable.Builder vb = vbOpt.get();
						Element dimList = elem.getChild("DimList");
						IList<Element> values = dimList.getChildren("value");
						setSharedDimensions(dataG, vb, values, unknownDims, location);
					}
				}

				// get projection
				string projS = null;
				Element projElem = gridElem.getChild("Projection");
				if (projElem != null)
				{
					projS = projElem.getText().Trim();
				}
				bool isLatLon = "GCTP_GEO".Equals(projS);

				// look for XDim, YDim coordinate variables
				if (isLatLon)
				{
					foreach (Variable.Builder v in dataG.vbuilders)
					{
						if (CoordSystemBuilder.isCoordinateVariable(v))
						{
							if (v.shortName.Equals("YDim"))
							{
								v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Lat.ToString()));
								v.addAttribute(new NcAttribute(CDM.UNITS, CDM.LAT_UNITS));
							}
							if (v.shortName.Equals("XDim"))
							{
								v.addAttribute(new NcAttribute(_Coordinate.AxisType, AxisType.Lon.ToString()));
								v.addAttribute(new NcAttribute(CDM.UNITS, CDM.LON_UNITS));
							}
						}
					}
				}
			}
			return FeatureType.GRID;
		}

		private void addAttributeIfExists(Element elem, string name, Variable.Builder v, bool isDoubleArray)
		{
			Element child = elem.getChild(name);
			if (child == null)
			{
				return;
			}
			if (isDoubleArray)
			{
				IList<Element> vElems = child.getChildren();
				IList<object> values = new List<object>();
				foreach (Element ve in vElems)
				{
					string valueS = ve.getText().Trim();
					try
					{
						values.Add(double.Parse(valueS));
					}
					catch (FormatException)
					{
						log.warn("Cant parse double value " + valueS);
					}
				}
				NcAttribute att = NcAttribute.builder(name).setValues(values, false).build();
				v.addAttribute(att);
			}
			else
			{
				string value = child.getText().Trim();
				NcAttribute att = new NcAttribute(name, value);
				v.addAttribute(att);
			}
		}

		// convert to shared dimensions
		private void setSharedDimensions(Group.Builder group, Variable.Builder v, IList<Element> values, IList<Dimension> unknownDims, string location)
		{
			if (values.Count == 0)
			{
				return;
			}

			// remove the "scalar" dumbension
			for (int idx = values.Count - 1; idx >= 0; idx--)
			{
				Element value = values[idx];
				string dimName = value.getText().Trim();
				if (dimName.Equals("scalar", StringComparison.OrdinalIgnoreCase))
				{
					values.RemoveAt(idx);
				}
			}

			// gotta have same number of dimensions
			IList<Dimension> oldDims = v.getDimensions();
			if (oldDims.Count != values.Count)
			{
				log.error("Different number of dimensions for {} {}", v.shortName, location);
				return;
			}

			IList<Dimension> newDims = new List<Dimension>();

			for (int i = 0; i < values.Count; i++)
			{
				Element value = values[i];
				string dimName = value.getText().Trim();
				dimName = NetcdfFiles.makeValidCdmObjectName(dimName);

				var dimOpt = group.findDimension(dimName);
				Dimension dim = dimOpt.Present ? dimOpt.get() : null;
				Dimension oldDim = oldDims[i];
				if (dim == null)
				{
					dim = checkUnknownDims(dimName, unknownDims, oldDim, location);
				}

				if (dim == null)
				{
					log.error("Unknown Dimension= {} for variable = {} {} ", dimName, v.shortName, location);
					return;
				}
				if (dim.Length != oldDim.Length)
				{
					log.error("Shared dimension ({}) has different length than data dimension ({}) shared={} org={} for {} {}", dim.ShortName, oldDim.ShortName, dim.Length, oldDim.Length, v, location);
					return;
				}
				newDims.Add(dim);
			}
			v.setDimensions(newDims);
			if (showWork)
			{
				log.debug(" set shared dimensions for {}", v.shortName);
			}
		}

		// look if the wanted dimension is in the unknownDims list.
		// TODO this must be rewritten, stop using deprecated methods
		private Dimension checkUnknownDims(string wantDim, IList<Dimension> unknownDims, Dimension oldDim, string location)
		{
			foreach (Dimension dim in unknownDims)
			{
				if (dim.ShortName.Equals(wantDim))
				{
					int len = oldDim.Length;
					if (len == 0)
					{
						dim.Unlimited = true; // allow zero length dimension !!
					}
					dim.Length = len; // use existing (anon) dimension
					Group parent = dim.Group;
					parent.addDimensionIfNotExists(dim); // add to the parent
					unknownDims.Remove(dim); // remove from list LOOK is this ok?
					log.warn("unknownDim {} length set to {}{}", wantDim, oldDim.Length, location);
					return dim;
				}
			}
			return null;
		}

		// look for a group with the given name. recurse into subgroups if needed. breadth first
		private Group.Builder findGroupNested(Group.Builder parent, string name)
		{

			foreach (Group.Builder g in parent.gbuilders)
			{
				if (g.shortName.Equals(name))
				{
					return g;
				}
			}
			foreach (Group.Builder g in parent.gbuilders)
			{
				Group.Builder result = findGroupNested(g, name);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		private void fixAttributes(Group.Builder g)
		{
			foreach (Variable.Builder v in g.vbuilders)
			{
				AttributeContainerMutable attHelper = v.getAttributeContainer();
				IList<NcAttribute> atts = new List<NcAttribute>(attHelper);
				foreach (NcAttribute a in atts)
				{
					if (a.ShortName.Equals("UNIT", StringComparison.OrdinalIgnoreCase) || a.ShortName.Equals("UNITS", StringComparison.OrdinalIgnoreCase))
					{
						attHelper.replace(a, CDM.UNITS);
					}
					if (a.ShortName.Equals("SCALE_FACTOR", StringComparison.OrdinalIgnoreCase) || a.ShortName.Equals("FACTOR", StringComparison.OrdinalIgnoreCase))
					{
						attHelper.replace(a, CDM.SCALE_FACTOR);
					}
					if (a.ShortName.Equals("OFFSET", StringComparison.OrdinalIgnoreCase))
					{
						attHelper.replace(a, CDM.ADD_OFFSET);
					}
				}
			}

			foreach (Group.Builder ng in g.gbuilders)
			{
				fixAttributes(ng);
			}

		}

	}

}
