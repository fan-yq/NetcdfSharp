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
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using Type = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2TablesId.Type;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// This manages configuring Grib2Tables and their local overrides.
	/// Configured by resources/grib2/standardTableMap.txt.
	/// 
	/// @author caron
	/// @since 8/1/2014
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable class Grib2TableConfig
	public class Grib2TableConfig                   
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2TableConfig));
	  private const string tableMapPath = "resources/grib2/standardTableMap.txt";
	  private static ImmutableList<Grib2TableConfig> tables;
	  private static Grib2TableConfig standardTable;

	  private static ImmutableList<Grib2TableConfig> init()
	  {
		IList<Grib2TableConfig> result = new List<Grib2TableConfig>();
		try
		{
				using (Stream @is = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStream(tableMapPath))
				{
			  if (@is == null)
			  {
				throw new System.InvalidOperationException("Cant find " + tableMapPath);
			  }
			  using (StreamReader dataIS = new StreamReader(@is, Encoding.UTF8))
			  {
				int count = 0;
				while (true)
				{
				  string line = dataIS.ReadLine();
				  if (string.ReferenceEquals(line, null))
				  {
					break;
				  }
				  if (line.StartsWith("#", StringComparison.Ordinal))
				  {
					continue;
				  }
				  count++;

				  string[] flds = System.Text.RegularExpressions.Regex.Split(line, ";");
				  if (flds.Length < 7)
				  {
					logger.warn("{} BAD format == {}", count, line);
					continue;
				  }

				  int fldidx = 0;
				  try
				  {
					int center = int.Parse(flds[fldidx++].Trim());
					int subcenter = int.Parse(flds[fldidx++].Trim());
					int master = int.Parse(flds[fldidx++].Trim());
					int local = int.Parse(flds[fldidx++].Trim());
					int genProcess = int.Parse(flds[fldidx++].Trim());
					string typeName = StringUtil2.remove(flds[fldidx++].Trim(), '"');
					string name = StringUtil2.remove(flds[fldidx++].Trim(), '"');
					string resource = (flds.Length > 7) ? StringUtil2.remove(flds[fldidx++].Trim(), '"') : null;
					Type type = (Grib2TablesId.Type)Enum.Parse(typeof(Grib2TablesId.Type), typeName);
					Grib2TableConfig table = new Grib2TableConfig(name, center, subcenter, master, local, genProcess, resource, type);
					result.Add(table);

				  }
				  catch (Exception e)
				  {
					logger.warn("{} {} BAD line == {} : {}", count, fldidx, line, e.Message);
				  }
				}
			  }
				}
		}
		catch (IOException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}

		standardTable = new Grib2TableConfig("WMO", 0, -1, -1, -1, -1, WmoCodeFlagTables.standard.ResourceName, Type.wmo);
		result.Add(standardTable);
		return ImmutableList<Grib2TableConfig>.copyOf(result);
	  }

	  internal static Grib2TableConfig matchTable(Grib2TablesId id)
	  {
		if (tables == null)
		{
		  tables = init();
		}

		// first match wins
		foreach (Grib2TableConfig table in tables)
		{
		  if (table.id.match(id))
		  {
			return table;
		  }
		}

		// no match
		return standardTable;
	  }

	  internal static ImmutableList<Grib2TableConfig> Tables
	  {
		  get
		  {
			if (tables == null)
			{
			  tables = init();
			}

			return tables;
		  }
	  }

	  ///////////////////////////////////////////////////////////////////////////////////////
	  private readonly string name;
	  private readonly Type type;
	  private readonly Grib2TablesId id;
	  private string path;

	  private Grib2TableConfig(string name, int center, int subCenter, int masterVersion, int localVersion, int genProcessId, string path, Type type)
	  {
		this.name = name;
		this.path = path;
		this.type = type;
		this.id = new Grib2TablesId(center, subCenter, masterVersion, localVersion, genProcessId);
	  }

	  internal virtual string Path
	  {
		  get
		  {
			return path;
		  }
	  }

	  internal virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  internal virtual Type Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  // The id in the configuration, will match but maybe not equal the Grib2Record's id.
	  internal virtual Grib2TablesId ConfigId
	  {
		  get
		  {
			return id;
		  }
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

		Grib2TableConfig that = (Grib2TableConfig) o;

		if (!id.Equals(that.id))
		{
		  return false;
		}
		if (!name.Equals(that.name))
		{
		  return false;
		}
		if (!Objects.equals(path, that.path))
		{
		  return false;
		}
		return type == that.type;

	  }

	  public override int GetHashCode()
	  {
		int result = name.GetHashCode();
		result = 31 * result + type.GetHashCode();
		result = 31 * result + id.GetHashCode();
		result = 31 * result + (!string.ReferenceEquals(path, null) ? path.GetHashCode() : 0);
		return result;
	  }
	}

}
