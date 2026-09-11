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
 * Copyright (c) 1998-2020 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;
	using Parameter = NetcdfSharp.ucar.nc2.grib.GribTables.Parameter;

	/// <summary>
	/// FSL/GSD (center 59)
	/// genProcessId 125 = HRRR
	/// 
	/// @author caron </summary>
	/// <seealso cref=""http://ruc.noaa.gov/hrrr/GRIB2Table.txt""
	/// @since 2/1/12/>
	public class FslHrrrLocalTables : NcepLocalTables
	{
	  public const int center_id = 59;

	  internal FslHrrrLocalTables(Grib2TableConfig config) : base(config) // default resource path
	  {
		initLocalTable(null);
	  }

	  public override string getParamTablePathUsedFor(int discipline, int category, int number)
	  {
		if ((category <= 191) && (number <= 191))
		{
		  return base.getParamTablePathUsedFor(discipline, category, number);
		}
		return config.Path;
	  }

	  public override ImmutableList<Parameter> Parameters
	  {
		  get
		  {
			return LocalParameters;
		  }
	  }

	  public override GribTables.Parameter getParameter(int discipline, int category, int number)
	  {
		return localParams[makeParamId(discipline, category, number)];
	  }

	  public override GribTables.Parameter getParameterRaw(int discipline, int category, int number)
	  {
		return localParams[makeParamId(discipline, category, number)];
	  }

	  // LOOK maybe combine grib1, grib2 and bufr ??
	  public override string getSubCenterName(int center, int subcenter)
	  {

		switch (subcenter)
		{
		  case 0:
			return null;
		  case 1:
			return "FSL/FRD Regional Analysis and Prediction Branch";
		  case 2:
			return "FSL/FRD Local Analysis and Prediction Branch";
		}
		return base.getSubCenterName(center, subcenter);
	  }

	  public override string getGeneratingProcessName(int genProcess)
	  {
		switch (genProcess)
		{
		  case 103:
			return "ExREF";
		  case 105:
			return "RAP/ RUC";
		  case 106:
			return "Developmental Testbed Center Winter Field Experiment, WRF-ARW";
		  case 112:
			return "Developmental Testbed Center Winter Field Experiment, WRF-NMM";
		  case 116:
			return "Flow-following Finite-volume Icosahedral Model (FIM)";
		  case 125:
			return "High-Resolution Rapid Refresh";
		  default:
			return null;
		}
	  }

	  public override string getLevelNameShort(int id)
	  {
		if (id == 200)
		{
		  return "Entire_atmosphere";
		}
		return base.getLevelNameShort(id);
	  }

	  public override string getLevelName(int id)
	  {
		if (id == 200)
		{
		  return "Entire atmosphere layer";
		}
		return base.getLevelName(id);
	  }

	  public override string getStatisticNameShort(int id)
	  {
		if (id == 255)
		{
		  return "Interval";
		}
		return base.getStatisticNameShort(id);
	  }

	  private void initLocalTable(Formatter f)
	  {
		localParams = readCsv(config.Path, f);
	  }

	  // debugging
	  public override void lookForProblems(Formatter f)
	  {
		initLocalTable(f);
	  }

	  private static Grib2Parameter parseVersion4(string[] flds, Formatter f)
	  {
		// V4:
		// Record, MasterTable, LocalTable, Discipline, Category, Parameter, WGrib2Name, NCLName,
		// FieldType, VerticalLevels, Units,
		string recordNumber = flds[0].Trim();
		int masterTableNumber = int.Parse(flds[1].Trim());
		int localTableNumber = int.Parse(flds[2].Trim());
		int disciplineNumber = int.Parse(flds[3].Trim());
		int categoryNumber = int.Parse(flds[4].Trim());
		int parameterNumber = int.Parse(flds[5].Trim());

		string WGrib2Name = flds[6].Trim();
		string NCLName = flds[7].Trim();
		string FieldType = flds[8].Trim(); // closest we have to a description in version 4 of the FSL table
		string VerticalLevels = flds[9].Trim();
		string Units = flds[10].Trim();
		if (f != null)
		{
		  f.format("%3s %3d %3d %3d %3d %3d %-10s %-25s %-30s %-100s %-20s%n", recordNumber, masterTableNumber, localTableNumber, disciplineNumber, categoryNumber, parameterNumber, WGrib2Name, NCLName, FieldType, VerticalLevels, Units);
		}

		string name = !NCLName.Equals("var") ? NCLName : FieldType;
		return new Grib2Parameter(disciplineNumber, categoryNumber, parameterNumber, name, Units, null, FieldType);
	  }

	  private static Grib2Parameter parsePre4(string[] flds, Formatter f)
	  {
		// v1, v2:
		// RecordNumber, TableNumber, DisciplineNumber, CategoryNumber, ParameterNumber, WGrib2Name, NCLName,
		// FieldType, Description, Units,
		string recordNumber = flds[0].Trim();
		int tableNumber = int.Parse(flds[1].Trim());
		int disciplineNumber = int.Parse(flds[2].Trim());
		int categoryNumber = int.Parse(flds[3].Trim());
		int parameterNumber = int.Parse(flds[4].Trim());

		string WGrib2Name = flds[5].Trim();
		string NCLName = flds[6].Trim();
		string FieldType = flds[7].Trim();
		string Description = flds[8].Trim();
		string Units = flds[9].Trim();
		if (f != null)
		{
		  f.format("%3s %3d %3d %3d %3d %-10s %-25s %-30s %-100s %-20s%n", recordNumber, tableNumber, disciplineNumber, categoryNumber, parameterNumber, WGrib2Name, NCLName, FieldType, Description, Units);
		}

		string name = !NCLName.Equals("var") ? NCLName : FieldType;
		return new Grib2Parameter(disciplineNumber, categoryNumber, parameterNumber, name, Units, null, Description);
	  }

	  private IDictionary<int, Grib2Parameter> readCsv(string resourcePath, Formatter f)
	  {
		bool header = true;
		int version = resourcePath.EndsWith("hrrr4.csv", StringComparison.Ordinal) ? 4 : -1;

		IDictionary<int, Grib2Parameter> result = new Dictionary<int, Grib2Parameter>(100);

		try
		{
				using (Stream @is = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStream(resourcePath))
				{
			  if (@is == null)
			  {
				throw new System.InvalidOperationException("Cant find " + resourcePath);
			  }
			  using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
			  {
				Dictionary<string, Grib2Parameter> names = new Dictionary<string, Grib2Parameter>(200);

				while (true)
				{
				  string line = br.ReadLine();
				  if (string.ReferenceEquals(line, null))
				  {
					break;
				  }
				  if (line.StartsWith("Record", StringComparison.Ordinal))
				  {
					break;
				  }
				}

				while (true)
				{
				  string line = br.ReadLine();
				  if (string.ReferenceEquals(line, null))
				  {
					break;
				  }
				  if ((line.Length == 0) || line.StartsWith("#", StringComparison.Ordinal))
				  {
					continue;
				  }

				  string[] flds = System.Text.RegularExpressions.Regex.Split(line, ",");
				  // starting with version 4, the HRRR parameter table includes multiple tables:
				  // HRRR 2-D Hourly
				  // HRRR 2-D Sub-hrly
				  // HRRR 3-D Native Level
				  // HRRR 3-D Isobaric Level
				  //
				  // To make updating simple, the tables are concatenated in a single file, which leads to
				  // the presence of multiple headers. This allows us to skip the internal headers of the csv "table".
				  // Also, some of the subtables include garbage, like the CONUS 3-D Native Level Fields table, which has
				  // things like:
				  // 1-14 repeats for remaining 49 native sigma levels...
				  // and this will allow us to skip those as well.
				  if ((version >= 4) && (flds.Length != 11) || line.StartsWith("Record", StringComparison.Ordinal))
				  {
					continue;
				  }

				  Grib2Parameter s = version == 4 ? parseVersion4(flds, f) : parsePre4(flds, f);

				  result[makeParamId(s.Discipline, s.Category, s.Number)] = s;
				  if (f != null)
				  {
					f.format(" %s%n", s);
				  }
				  if (s.Category > 191 || s.Number > 191)
				  {
					Grib2Parameter dup;
					names.TryGetValue(s.Name, out dup);
					if (dup != null && f != null)
					{
					  if (header)
					  {
						if (version >= 4)
						{
						  // Duplicate entries are only potentially an issue in version 4 and above.
						  // Need to examine them to know for sure.
						  f.format("Potential problem in table %s %n", resourcePath);
						}
						else
						{
						  f.format("Problems in table %s %n", resourcePath);
						}
					  }
					  header = false;
					  f.format(" DUPLICATE NAME %s and %s (%s)%n", s.Id, dup.Id, s.Name);
					}
				  }
				  names[s.Name] = s;
				}
			  }

				}
		}
		catch (IOException ioe)
		{
		  throw new Exception(ioe.Message, ioe);
		}

		return result;
	  }
	}

}
