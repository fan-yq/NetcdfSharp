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
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib2Pds = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Pds;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Counters = NetcdfSharp.ucar.nc2.util.Counters;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Ensemble coordinates
	/// 
	/// @author caron
	/// @since 2/22/14
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CoordinateEns implements Coordinate
	public class CoordinateEns : Coordinate
	{
	  private readonly IList<EnsCoordValue> ensSorted;
	  private readonly int code;
	  private string name = "ens";

	  public CoordinateEns(int code, IList<EnsCoordValue> ensSorted)
	  {
		this.ensSorted = ensSorted.AsReadOnly();
		this.code = code;
	  }

	  public virtual IList<EnsCoordValue> EnsSorted
	  {
		  get
		  {
			return ensSorted;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.java.util.List<?> getValues()
	  public virtual IList<object> Values
	  {
		  get
		  {
			return ensSorted.Cast<object>().ToList();
		  }
	  }

	  public virtual int getIndex(object val)
	  {
		return ensSorted.IndexOf((EnsCoordValue) val);
	  }

	  public virtual object getValue(int idx)
	  {
		return ensSorted[idx];
	  }

	  public virtual int getIndexByMember(double need)
	  {
		for (int i = 0; i < ensSorted.Count; i++)
		{
		  EnsCoordValue coord = ensSorted[i];
		  if (Misc.nearlyEquals(need, coord.EnsMember))
		  {
			return i;
		  }
		}
		return -1;
	  }

	  public virtual int Size
	  {
		  get
		  {
			return ensSorted.Count;
		  }
	  }

	  public virtual int NCoords
	  {
		  get
		  {
			return Size;
		  }
	  }

	  public virtual int estMemorySize()
	  {
		return 160 + Size * (8 + Misc.referenceSize);
	  }

	  public virtual Coordinate.Type getType()
	  {
		return NetcdfSharp.ucar.nc2.grib.coord.Coordinate.Type.ens;
	  }

	  public virtual string Unit
	  {
		  get
		  {
			return ""; // LOOK
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
			if (!this.name.Equals("ens"))
			{
			  throw new System.InvalidOperationException("Cant modify");
			}
			this.name = value;
		  }
	  }


	  public virtual void showInfo(Formatter info, Indent indent)
	  {
		info.format("%s%s: ", indent, getType());
		foreach (EnsCoordValue level in ensSorted)
		{
		  info.format(" %s", level);
		}
		info.format(" (%d)%n", ensSorted.Count);
	  }

	  public virtual void showCoords(Formatter info)
	  {
		info.format("Ensemble coords: (%s)%n", Unit);
		foreach (EnsCoordValue level in ensSorted)
		{
		  info.format("   %s%n", level);
		}
	  }

	  public virtual Counters calcDistributions()
	  {
		Counters counters = new Counters();
		counters.add("resol");

		for (int i = 0; i < ensSorted.Count - 1; i++)
		{
		  int diff = ensSorted[i + 1].EnsMember - ensSorted[i].EnsMember;
		  counters.count("resol", diff);
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

		CoordinateEns that = (CoordinateEns) o;

		if (code != that.code)
		{
		  return false;
		}
// JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
// ORIGINAL LINE: return ensSorted.equals(that.ensSorted);
		return ensSorted.SequenceEqual(that.ensSorted);

	  }

	  public override int GetHashCode()
	  {
		int result = ensSorted.GetHashCode();
		result = 31 * result + code;
		return result;
	  }

	  //////////////////////////////////////////////////////////////

	  public class Builder2 : CoordinateBuilderImpl<Grib2Record>
	  {
		internal int code;

		public Builder2(int code)
		{
		  this.code = code;
		}

		public override object extract(Grib2Record gr)
		{
		  Grib2Pds pds = gr.PDS;
		  Grib2Pds.PdsEnsemble pdse = (Grib2Pds.PdsEnsemble) pds;
		  return new EnsCoordValue(pdse.PerturbationType, pdse.PerturbationNumber, pdse.NumberEnsembleForecasts);
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<EnsCoordValue> levelSorted = new List<EnsCoordValue>(values.Count);
		  foreach (object val in values)
		  {
			levelSorted.Add((EnsCoordValue) val);
		  }
		  levelSorted.Sort();
		  return new CoordinateEns(code, levelSorted);
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
		  return new EnsCoordValue(pds.PerturbationType, pds.PerturbationNumber, pds.Center);
		}

		public override Coordinate makeCoordinate(IList<object> values)
		{
		  IList<EnsCoordValue> levelSorted = values.Select(val => (EnsCoordValue) val).OrderBy(c => c).ToList();
		  return new CoordinateEns(code, levelSorted);
		}
	  }

	}

}
