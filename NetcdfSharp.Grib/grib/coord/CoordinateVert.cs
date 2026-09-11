// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;
using System.Linq;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using Grib1ParamLevel = NetcdfSharp.ucar.nc2.grib.grib1.Grib1ParamLevel;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib2Pds = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Pds;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2Utils = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Utils;
	using Counters = NetcdfSharp.ucar.nc2.util.Counters;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Vertical GRIB coordinates
	/// Effectively immutable; setName() can only be called once.
	/// 
	/// @author caron
	/// @since 11/27/13
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CoordinateVert implements Coordinate
	public class CoordinateVert : Coordinate
	{
	  private readonly IList<VertCoordValue> levelSorted;
	  private readonly int code; // Grib1 - code table 3; Grib2 - Code table 4.5
	  private string name;
	  private readonly VertCoordType vunit;
	  private readonly bool isLayer;

	  public CoordinateVert(int code, VertCoordType vunit, IList<VertCoordValue> levelSorted)
	  {
		this.levelSorted = levelSorted.AsReadOnly();
		this.code = code;
		this.vunit = vunit;
		this.isLayer = levelSorted[0].Layer;
	  }

	  public virtual IList<VertCoordValue> LevelSorted
	  {
		  get
		  {
			return levelSorted;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.java.util.List<?> getValues()
	  public virtual IList<object> Values
	  {
		  get
		  {
			return levelSorted.Cast<object>().ToList();
		  }
	  }

	  public virtual int getIndex(object val)
	  {
		return levelSorted.IndexOf((VertCoordValue) val);
	  }

	  public virtual object getValue(int idx)
	  {
		return levelSorted[idx];
	  }

	  public virtual int Size
	  {
		  get
		  {
			return levelSorted.Count;
		  }
	  }

	  public virtual int NCoords
	  {
		  get
		  {
			return Size;
		  }
	  }

	  public virtual Coordinate.Type getType()
	  {
		return NetcdfSharp.ucar.nc2.grib.coord.Coordinate.Type.vert;
	  }

	  public virtual int estMemorySize()
	  {
		return 160 + Size * (40 + Misc.referenceSize);
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @Nullable public String getUnit()
	  public virtual string Unit
	  {
		  get
		  {
			return vunit == null ? null : vunit.Units;
		  }
	  }

	  public virtual VertCoordType VertUnit
	  {
		  get
		  {
			return vunit;
		  }
	  }

	  public virtual bool Layer
	  {
		  get
		  {
			return isLayer;
		  }
	  }

	  public virtual bool PositiveUp
	  {
		  get
		  {
			return vunit.PositiveUp;
		  }
	  }

	  public virtual int Code
	  {
		  get
		  {
			return code;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			if (!string.ReferenceEquals(this.name, null))
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			this.name = value;
		  }
	  }


	  public virtual void showInfo(Formatter info, Indent indent)
	  {
		info.format("%s%s: ", indent, getType());
		foreach (VertCoordValue level in levelSorted)
		{
		  info.format(" %s", level);
		}
		info.format(" (%d)%n", levelSorted.Count);
	  }

	  public virtual void showCoords(Formatter info)
	  {
		info.format("Levels: (%s)%n", Unit);
		foreach (VertCoordValue level in levelSorted)
		{
		  info.format("   %s%n", level);
		}
	  }

	  public virtual Counters calcDistributions()
	  {
		Counters counters = new Counters();
		counters.add("resol");

		if (isLayer)
		{
		  counters.add("intv");
		  for (int i = 0; i < levelSorted.Count; i++)
		  {
			double intv = (levelSorted[i]).Value2 - (levelSorted[i]).Value1;
			counters.count("intv", intv);
			if (i < levelSorted.Count - 1)
			{
			  double resol = (levelSorted[i + 1]).Value1 - (levelSorted[i]).Value1;
			  counters.count("resol", resol);
			}
		  }

		}
		else
		{

		  for (int i = 0; i < levelSorted.Count - 1; i++)
		  {
			double diff = levelSorted[i + 1].Value1 - levelSorted[i].Value1;
			counters.count("resol", diff);
		  }
		}

		return counters;
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

		CoordinateVert that = (CoordinateVert) o;

		if (code != that.code)
		{
		  return false;
		}
// JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
// ORIGINAL LINE: return levelSorted.equals(that.levelSorted);
		return levelSorted.SequenceEqual(that.levelSorted);

	  }

	  public override int GetHashCode()
	  {
		int result = levelSorted.GetHashCode();
		result = 31 * result + code;
		return result;
	  }

	  //////////////////////////////////////////////////////////////

	  public class Builder2 : CoordinateBuilderImpl<Grib2Record>
	  {
		internal int code;
		internal VertCoordType vunit;

		public Builder2(int code, VertCoordType vunit)
		{
		  this.code = code;
		  this.vunit = vunit;
		}

		public override object extract(Grib2Record gr)
		{
		  Grib2Pds pds = gr.PDS;
		  if (Grib2Utils.isLayer(pds))
		  {
			return new VertCoordValue(pds.LevelValue1, pds.LevelValue2);
		  }
		  else
		  {
			return new VertCoordValue(pds.LevelValue1);
		  }
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<VertCoordValue> levelSorted = new List<VertCoordValue>(values.Count);
		  foreach (object val in values)
		  {
			levelSorted.Add((VertCoordValue) val);
		  }
		  levelSorted.Sort();
		  return new CoordinateVert(code, vunit, levelSorted);
		}
	  }

	  public class Builder1 : CoordinateBuilderImpl<Grib1Record>
	  {
		internal int code;
		internal Grib1Customizer cust;

		public Builder1(Grib1Customizer cust, int code)
		{
		  this.cust = cust;
		  this.code = code;
		}

		public override object extract(Grib1Record gr)
		{
		  Grib1SectionProductDefinition pds = gr.PDSsection;
		  bool isLayer = cust.isLayer(pds.LevelType);
		  Grib1ParamLevel plevel = cust.getParamLevel(pds);
		  if (isLayer)
		  {
			return new VertCoordValue(plevel.Value1, plevel.Value2);
		  }
		  else
		  {
			return new VertCoordValue(plevel.Value1);
		  }
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<VertCoordValue> levelSorted = new List<VertCoordValue>(values.Count);
		  foreach (object val in values)
		  {
			levelSorted.Add((VertCoordValue) val);
		  }
		  levelSorted.Sort();
		  return new CoordinateVert(code, cust.getVertUnit(code), levelSorted);
		}
	  }

	}

}
