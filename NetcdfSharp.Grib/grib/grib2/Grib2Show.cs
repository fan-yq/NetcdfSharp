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
	using FeatureCollectionConfig = NetcdfSharp.thredds.featurecollection.FeatureCollectionConfig;
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using TimeCoordIntvDateValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvDateValue;
	using VertCoordType = NetcdfSharp.ucar.nc2.grib.coord.VertCoordType;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using WmoTemplateTables = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoTemplateTables;
	using TemplateTable = NetcdfSharp.ucar.nc2.grib.grib2.table.WmoTemplateTables.TemplateTable;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;
	using CommonCodeTable = NetcdfSharp.ucar.nc2.wmo.CommonCodeTable;

	/// <summary>
	/// Utilities to show Grib2 records.
	/// 
	/// @author caron
	/// @since 12/22/2015.
	/// </summary>
	public class Grib2Show
	{

	  public static void showBytes(Formatter f, byte[] buff, int max)
	  {
		int count = 0;
		foreach (byte b in buff)
		{
		  int ub = (b < 0) ? b + 256 : b;
		  if (b >= 32 && b < 127)
		  {
			f.format("%s", (char) ub);
		  }
		  else
		  {
			f.format("(%d)", ub);
		  }
		  if (max > 0 && count++ > max)
		  {
			break;
		  }
		}
	  }

	  public static void showCompleteGribRecord(Formatter f, string path, Grib2Record gr, Grib2Tables cust)
	  {
		f.format("File=%d %s offset=%d%n", gr.File, path, gr.Is.StartPos);
		f.format("Header=\"");
		showBytes(f, gr.Header, 100);
		f.format("\"%n");

		Grib2Variable gv = new Grib2Variable(cust, gr, 0, FeatureCollectionConfig.intvMergeDef, FeatureCollectionConfig.useGenTypeDef);
		f.format("cdmHash=%d%n", gv.GetHashCode());

		int d = gr.Discipline;
		f.format("Grib2IndicatorSection%n");
		f.format(" Discipline = (%d) %s%n", d, cust.getCodeTableValue("0.0", d));
		f.format(" Length     = %d%n", gr.Is.MessageLength);

		Grib2SectionIdentification id = gr.Id;
		f.format("%nGrib2IdentificationSection%n");
		f.format(" Center        = (%d) %s%n", id.Center_id, CommonCodeTable.getCenterName(id.Center_id, 2));
		f.format(" SubCenter     = (%d) %s%n", id.Subcenter_id, cust.getSubCenterName(id.Center_id, id.Subcenter_id));
		f.format(" Master Table  = %d%n", id.Master_table_version);
		f.format(" Local Table   = %d%n", id.Local_table_version);
		f.format(" RefTimeSignif = %d (%s)%n", id.SignificanceOfRT, cust.getCodeTableValue("1.2", id.SignificanceOfRT));
		f.format(" RefTime       = %s%n", id.ReferenceDate);
		f.format(" RefTime Fields = %d-%d-%d %d:%d:%d%n", id.Year, id.Month, id.Day, id.Hour, id.Minute, id.Second);
		f.format(" ProductionStatus      = %d (%s)%n", id.ProductionStatus, cust.getCodeTableValue("1.3", id.ProductionStatus));
		f.format(" TypeOfProcessedData   = %d (%s)%n", id.TypeOfProcessedData, cust.getCodeTableValue("1.4", id.TypeOfProcessedData));

		if (gr.hasLocalUseSection())
		{
		  byte[] lus = gr.LocalUseSection.RawBytes;
		  f.format("%nLocal Use Section (grib section 2)%n");
		  f.format("bytes (len=%d) =%s%n", lus.Length, "[" + string.Join(", ", lus) + "]");
		}

		Grib2SectionGridDefinition gds = gr.GDSsection;
		Grib2Gds ggds = gds.GDS;
		f.format("%nGrib2GridDefinitionSection hash=%d crc=%d%n", ggds.GetHashCode(), gds.calcCRC());
		f.format(" Length             = %d%n", gds.Length);
		f.format(" Source  (3.0)      = %d (%s) %n", gds.Source, cust.getCodeTableValue("3.0", gds.Source));
		f.format(" Npts               = %d%n", gds.NumberPoints);
		f.format(" Template (3.1)     = %d%n", gds.GDSTemplateNumber);
		showGdsTemplate(gds, f, cust);

		Grib2SectionProductDefinition pdss = gr.PDSsection;
		f.format("%nGrib2ProductDefinitionSection%n");
		Grib2Pds pds = gr.PDS;
		if (pds.isTimeInterval())
		{
		  TimeCoordIntvDateValue intv = cust.getForecastTimeInterval(gr);
		  if (intv != null)
		  {
			f.format(" Interval     = %s%n", intv);
		  }
		}
		showPdsTemplate(pdss, f, cust);
		if (pds.ExtraCoordinatesCount > 0)
		{
		  float[] coords = pds.ExtraCoordinates;
		  if (coords != null)
		  {
			f.format("Hybrid Coordinates (%d) %n  ", coords.Length);
			foreach (float fc in coords)
			{
			  f.format("%10.5f ", fc);
			}
		  }
		  f.format("%n%n");
		}

		Grib2SectionDataRepresentation drs = gr.DataRepresentationSection;
		f.format("%nGrib2SectionDataRepresentation%n");
		f.format("  Template           = %d (%s) %n", drs.DataTemplate, cust.getCodeTableValue("5.0", drs.DataTemplate));
		f.format("  NPoints            = %d%n", drs.DataPoints);

		Grib2SectionData ds = gr.DataSection;
		f.format("%nGrib2SectionData%n");
		f.format("  Starting Pos       = %d %n", ds.StartingPosition);
		f.format("  Data Length        = %d%n", ds.MsgLength);
	  }

	  public static void showGdsTemplate(Grib2SectionGridDefinition gds, Formatter f, Grib2Tables cust)
	  {
		int template = gds.GDSTemplateNumber;
		byte[] raw = gds.RawBytes;
		showRawWithTemplate("3." + template, raw, f, cust);
	  }

	  public static void showPdsTemplate(Grib2SectionProductDefinition pdss, Formatter f, Grib2Tables cust)
	  {
		int template = pdss.PDSTemplateNumber;
		byte[] raw = pdss.RawBytes;
		showRawWithTemplate("4." + template, raw, f, cust);
	  }

	  private static void showRawWithTemplate(string key, byte[] raw, Formatter f, Grib2Tables cust)
	  {
		WmoTemplateTables.TemplateTable template = WmoTemplateTables.Instance.getTemplateTable(key);
		if (template == null)
		{
		  f.format("Cant find template %s%n", key);
		}
		else
		{
		  template.showInfo(cust, raw, f);
		}
	  }

	  public static void showProcessedPds(Grib2Tables cust, Grib2Pds pds, int discipline, Formatter f)
	  {
		int template = pds.TemplateNumber;
		f.format(" Product Template %3d = %s%n", template, cust.getCodeTableValue("4.0", template));
		f.format(" Discipline %3d     = %s%n", discipline, cust.getCodeTableValue("0.0", discipline));
		f.format(" Category %3d       = %s%n", pds.ParameterCategory, cust.getCategory(discipline, pds.ParameterCategory));
		GribTables.Parameter entry = cust.getParameter(discipline, pds);
		if (entry != null)
		{
		  f.format(" Parameter Name     = %3d %s %n", pds.ParameterNumber, entry.Name);
		  f.format(" Parameter Units    = %s %n", entry.Unit);
		}
		else
		{
		  f.format(" Unknown Parameter  = %d-%d-%d %n", discipline, pds.ParameterCategory, pds.ParameterNumber);
		  cust.getParameter(discipline, pds); // debug
		}
		f.format(" Parameter Table  = %s%n", cust.getParamTablePathUsedFor(discipline, pds.ParameterCategory, pds.ParameterNumber));

		int tgp = pds.GenProcessType;
		f.format(" Generating Process Type = %3d %s %n", tgp, cust.getCodeTableValue("4.3", tgp));
		f.format(" Forecast Offset    = %3d %n", pds.ForecastTime);
		f.format(" First Surface Type = %3d %s %n", pds.LevelType1, cust.getLevelNameShort(pds.LevelType1));
		f.format(" First Surface value= %3f %n", pds.LevelValue1);
		f.format(" Second Surface Type= %3d %s %n", pds.LevelType2, cust.getLevelNameShort(pds.LevelType2));
		f.format(" Second Surface val = %3f %n", pds.LevelValue2);
		f.format("%n Level Name (from table 4.5) = %3s %n", cust.getCodeTableValue("4.5", pds.LevelType1));
		f.format(" Gen Process Ttype (from table 4.3) = %3s %n", cust.getCodeTableValue("4.3", pds.GenProcessType));
	  }

	  public static void showProcessedGridRecord(Grib2Tables cust, Grib2Record gr, Formatter f)
	  {
		GribTables.Parameter param = cust.getParameter(gr);
		if (param != null)
		{
		  f.format("  Parameter=%s (%s)%n", param.Name, param.Abbrev);
		}
		else
		{
		  f.format(" Unknown Parameter  = %d-%d-%d %n", gr.Discipline, gr.PDS.ParameterCategory, gr.PDS.ParameterNumber);
		}

		Grib2Pds pds = gr.PDS;
		VertCoordType levelUnit = cust.getVertUnit(pds.LevelType1);
		f.format("  Level=%f/%f %s; level name =  (%s)%n", pds.LevelValue1, pds.LevelValue1, levelUnit.Units, cust.getLevelNameShort(pds.LevelType1));

		string intvName = "none";
		if (pds is Grib2Pds.PdsInterval)
		{
		  Grib2Pds.PdsInterval pdsi = (Grib2Pds.PdsInterval) pds;
		  Grib2Pds.TimeInterval[] ti = pdsi.TimeIntervals;
		  int statType = ti[0].statProcessType;
		  intvName = cust.getStatisticNameShort(statType);
		}

		f.format("  Time Unit=%s; Stat=%s%n", Grib2Utils.getCalendarPeriod(pds.TimeUnit), intvName);
		f.format("  ReferenceDate=%s%n", gr.ReferenceDate);
		f.format("  ForecastDate=%s%n", cust.getForecastDate(gr));
		TimeCoordIntvDateValue intv = cust.getForecastTimeInterval(gr);
		if (intv != null)
		{
		  f.format("  TimeInterval=%s%n", intv);
		}
		f.format("%n");
		pds.show(f);

		// CFSR malarky
		cust.showSpecialPdsInfo(gr, f);
	  }
	}

}
