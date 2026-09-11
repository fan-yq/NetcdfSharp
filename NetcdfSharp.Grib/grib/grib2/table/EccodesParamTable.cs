// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.IO;
using System.Text;

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;

	/// <summary>
	/// The results of EcmwfParamTableCompare indicate there are no significant differences of the parameter tables with WMO.
	/// So this class is not used currently in production.
	/// </summary>
	public class EccodesParamTable : Grib2ParamTableInterface
	{
	  private const bool debugOpen = false;
	  private const bool debug = false;
	  private const string PATH = "resources/grib2/ecmwf/tables";

#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks

	  private string title;
	  private string source;
	  private string tableName;

	  private readonly int version, discipline, category;
	  private readonly ImmutableMap<int, Grib2Parameter> paramMap;

	  public static EccodesParamTable factory(int version, int discipline, int category)
	  {
		try
		{
		  return new EccodesParamTable(version, discipline, category);
		}
		catch (Exception)
		{
		  return null;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private EccodesParamTable(int version, int discipline, int category) throws NetcdfSharp.java.io.IOException
	  private EccodesParamTable(int version, int discipline, int category)
	  {
		this.version = version;
		this.discipline = discipline;
		this.category = category;

		this.paramMap = this.readTable(TablePath);
	  }

	  public virtual string Name
	  {
		  get
		  {
			return string.Format("Ecmwf version {0:D} discipline {1:D} category {2:D} ({3})", version, discipline, category, TablePath);
		  }
	  }

	  public virtual string ShortName
	  {
		  get
		  {
			return string.Format("Ecmwf version {0:D} ({1:D}-{2:D})", version, discipline, category);
		  }
	  }

	  public virtual ImmutableList<GribTables.Parameter> Parameters
	  {
		  get
		  {
	// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
			return ImmutableList<GribTables.Parameter>.copyOf(paramMap.Values.OrderBy(c => c).Select(p => (GribTables.Parameter) p));
		  }
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public GribTables.Parameter getParameter(int number)
	  public virtual GribTables.Parameter getParameter(int code)
	  {
		return paramMap.get(code);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter getParameter(int discipline, int category, int number)
	  public virtual Grib2Parameter getParameter(int discipline, int category, int number)
	  {
		return paramMap.get(number);
	  }

	  private string TablePath
	  {
		  get
		  {
			return string.Format("{0}/{1:D}/4.2.{2:D}.{3:D}.table", PATH, version, discipline, category);
		  }
	  }

	  /*
	   * # Code table 4.2 - Parameter number by product discipline and parameter category
	   * 0 0 Estimated precipitation (kg m-2)
	   * 1 1 Instantaneous rain rate (kg m-2 s-1)
	   * 2 2 Cloud top height (m)
	   * 3 3 Cloud top height quality indicator (Code table 4.219)
	   * 4 4 Estimated u-component of wind (m/s)
	   * 5 5 Estimated v-component of wind (m/s)
	   * 6 6 Number of pixel used (Numeric)
	   * 7 7 Solar zenith angle (deg)
	   * 8 8 Relative azimuth angle (deg)
	   * 9 9 Reflectance in 0.6 micron channel (%)
	   * 10 10 Reflectance in 0.8 micron channel (%)
	   * 11 11 Reflectance in 1.6 micron channel (%)
	   * 12 12 Reflectance in 3.9 micron channel (%)
	   * 13 13 Atmospheric divergence (/s)
	   * 14 14 Cloudy brightness temperature (K)
	   * 15 15 Clear-sky brightness temperature (K)
	   * 16 16 Cloudy radiance (with respect to wave number) (W m-1 sr-1)
	   * 17 17 Clear-sky radiance (with respect to wave number) (W m-1 sr-1)
	   * 18 18 Reserved
	   * 19 19 Wind speed (m/s)
	   * 20 20 Aerosol optical thickness at 0.635 um
	   * 21 21 Aerosol optical thickness at 0.810 um
	   * 22 22 Aerosol optical thickness at 1.640 um
	   * 23 23 Angstrom coefficient
	   * # 24-26 Reserved
	   * 27 27 Bidirectional reflectance factor (numeric)
	   * 28 28 Brightness temperature (K)
	   * 29 29 Scaled radiance (numeric)
	   * # 30-191 Reserved
	   * # 192-254 Reserved for local use
	   * 255 255 Missing
	   */
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.com.google.common.collect.ImmutableMap<int, NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter> readTable(String path) throws NetcdfSharp.java.io.IOException
	  private ImmutableMap<int, Grib2Parameter> readTable(string path)
	  {
		ImmutableMap<int, Grib2Parameter>.Builder builder = ImmutableMap<int, Grib2Parameter>.builder();

		if (debugOpen)
		{
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
		  Console.Write("readEcmwfTable path= {0}%n", path);
		}

		using (Stream @is = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStream(path))
		{
		  if (@is == null)
		  {
			throw new System.InvalidOperationException("Cant find " + path);
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
			  if (line.StartsWith("#", StringComparison.Ordinal) || line.Trim().Length == 0)
			  {
				continue;
			  }
			  count++;

			  int posBlank1 = line.IndexOf(' ');
			  int posBlank2 = line.IndexOf(' ', posBlank1 + 1);
			  int lastParen = line.LastIndexOf('(');

			  string num1 = line.Substring(0, posBlank1).Trim();
			  string num2 = line.Substring(posBlank1 + 1, posBlank2 - (posBlank1 + 1));
			  string desc = (lastParen > 0) ? line.Substring(posBlank2 + 1, lastParen - (posBlank2 + 1)).Trim() : line.Substring(posBlank2 + 1).Trim();
			  string units = (lastParen > 0) ? line.Substring(lastParen).Trim() : "";
			  if (units.StartsWith("(", StringComparison.Ordinal) & units.EndsWith(")", StringComparison.Ordinal))
			  {
				units = units.Substring(1, (units.Length - 1) - 1);
			  }

			  if (!num1.Equals(num2))
			  {
				if (debug)
				{
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				  Console.Write("*****num1 != num2 for {0}%n", line);
				}
				continue;
			  }
			  int number = int.Parse(num1);

			  // public Grib2Parameter(int discipline, int category, int number, String name, String unit, String abbrev) {
			  Grib2Parameter parameter = new Grib2Parameter(discipline, category, number, desc, units, null, desc);
			  builder.put(parameter.Number, parameter);
			  if (debug)
			  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Console.Write(" {0}%n", parameter);
			  }
			}
		  }
		}
		return builder.build();
	  }
	}

#pragma warning restore CS0162

}
