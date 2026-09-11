// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using GribUtils = NetcdfSharp.ucar.nc2.grib.GribUtils;
	using Grib1ParamTableReader = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1ParamTableReader;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// A Grib-1 Parameter
	/// 
	/// @author John
	/// @since 9/8/11
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib1Parameter implements NetcdfSharp.ucar.nc2.grib.GribTables.Parameter
	public class Grib1Parameter : GribTables.Parameter
	{

	  private readonly Grib1ParamTableReader table; // which table did this come from ?
	  private readonly int number;
	  private readonly string name;
	  private readonly string description;
	  private readonly string unit;
	  private readonly string cfName; // CF standard name, if it exists

	  public Grib1Parameter(Grib1ParamTableReader table, int number, string name, string description, string unit)
	  {
		this.table = table;
		this.number = number;
		this.name = setName(name);
		this.description = setDescription(description);
		this.unit = unit; // cleanupUnit(unit);
		this.cfName = null;
	  }

	  public Grib1Parameter(Grib1ParamTableReader table, int number, string name, string description, string unit, string cf_name)
	  {
		this.table = table;
		this.number = number;
		this.name = setName(name);
		this.description = setDescription(description);
		this.unit = unit; // cleanupUnit(unit);
		this.cfName = cf_name;
	  }

	  public virtual Grib1ParamTableReader Table
	  {
		  get
		  {
			return table;
		  }
	  }

	  public virtual int Discipline
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual int Category
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual int Number
	  {
		  get
		  {
			return number;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual bool useName()
	  {
		return (!string.ReferenceEquals(name, null)) && table.useParamName();
	  }

	  public string Description
	  {
		  get
		  {
			return description;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return table.Center_id + "." + table.Subcenter_id + "." + number;
		  }
	  }

	  public string Unit
	  {
		  get
		  {
			return unit;
		  }
	  }

	  public virtual string Abbrev
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual float? Fill
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual float? Missing
	  {
		  get
		  {
			return float.NaN;
		  }
	  }

	  public virtual string OperationalStatus
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual string CFname
	  {
		  get
		  {
			return cfName;
		  }
	  }

	  private string setName(string name)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  name = "";
		}
		return StringUtil2.replace(name, ' ', "_"); // replace blanks
	  }

	  private string setDescription(string description)
	  {
		return GribUtils.cleanupDescription(description);
	  }

	  private string cleanupUnit(string unit)
	  {
		return GribUtils.cleanupUnits(unit);
	  }

	  public override string ToString()
	  {
		return "GridParameter{" + "number=" + number + ", name='" + name + '\'' + ", description='" + description + '\'' + ", unit='" + unit + '\'' + '}';
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

		Grib1Parameter that = (Grib1Parameter) o;

		if (number != that.number)
		{
		  return false;
		}
		if (!Objects.equals(cfName, that.cfName))
		{
		  return false;
		}
		if (!Objects.equals(description, that.description))
		{
		  return false;
		}
		if (!Objects.equals(name, that.name))
		{
		  return false;
		}
		return Objects.equals(unit, that.unit);

	  }

	  public override int GetHashCode()
	  {
		int result = number;
		result = 31 * result + (!string.ReferenceEquals(name, null) ? name.GetHashCode() : 0);
		result = 31 * result + (!string.ReferenceEquals(description, null) ? description.GetHashCode() : 0);
		result = 31 * result + (!string.ReferenceEquals(unit, null) ? unit.GetHashCode() : 0);
		result = 31 * result + (!string.ReferenceEquals(cfName, null) ? cfName.GetHashCode() : 0);
		return result;
	  }
	}


}
