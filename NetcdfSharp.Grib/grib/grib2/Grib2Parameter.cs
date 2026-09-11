// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using Util = NetcdfSharp.ucar.nc2.wmo.Util;

	/// <summary>
	/// A Grib-2 parameter
	/// 
	/// @author caron
	/// @since 1/9/12
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2Parameter implements NetcdfSharp.ucar.nc2.grib.GribTables.Parameter, Comparable<Grib2Parameter>
	public class Grib2Parameter : GribTables.Parameter, IComparable<Grib2Parameter>
	{
	  public readonly int discipline, category, number;
	  public readonly string name, unit, abbrev, desc;
	  public readonly float? fill, missing;

	  public Grib2Parameter(int discipline, int category, int number, string name, string unit, string abbrev, string desc, float fill, float missing)
	  {
		this.discipline = discipline;
		this.category = category;
		this.number = number;
		this.name = name.Trim();
		this.abbrev = abbrev;
		this.unit = Util.cleanUnit(unit);
		this.desc = desc;
		this.fill = fill;
		this.missing = missing;
	  }

	  public Grib2Parameter(int discipline, int category, int number, string name, string unit, string abbrev, string desc)
	  {
		this.discipline = discipline;
		this.category = category;
		this.number = number;
		this.name = name.Trim();
		this.abbrev = abbrev;
		this.unit = Util.cleanUnit(unit);
		this.desc = desc;
		this.fill = null;
		this.missing = float.NaN;
	  }

	  public Grib2Parameter(Grib2Parameter from, string name, string unit)
	  {
		this.discipline = from.discipline;
		this.category = from.category;
		this.number = from.number;
		this.desc = from.desc;
		this.abbrev = from.abbrev;

		this.name = name.Trim();
		this.unit = Util.cleanUnit(unit);
		this.fill = null;
		this.missing = float.NaN;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return discipline + "." + category + "." + number;
		  }
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: public int compareTo(@Nonnull Grib2Parameter o)
	  public virtual int CompareTo(Grib2Parameter o)
	  {
		int c = discipline - o.discipline;
		if (c != 0)
		{
		  return c;
		}
		c = category - o.category;
		if (c != 0)
		{
		  return c;
		}
		return number - o.number;
	  }

	  public virtual int Discipline
	  {
		  get
		  {
			return discipline;
		  }
	  }

	  public virtual int Category
	  {
		  get
		  {
			return category;
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

	  public virtual string Unit
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
			return abbrev;
		  }
	  }

	  public virtual string Description
	  {
		  get
		  {
			return desc;
		  }
	  }

	  public virtual float? Missing
	  {
		  get
		  {
			return missing;
		  }
	  }

	  public virtual string OperationalStatus
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
			return fill;
		  }
	  }

	  public override string ToString()
	  {
		return "Grib2Parameter{" + "discipline=" + discipline + ", category=" + category + ", number=" + number + ", name='" + name + '\'' + ", unit='" + unit + '\'' + ", abbrev='" + abbrev + '\'' + ", desc='" + desc + '\'' + ", fill='" + fill.Value + '\'' + ", missing='" + missing.Value + '\'' + '}';
	  }
	}


}
