// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;
	using Parameter = NetcdfSharp.ucar.nc2.grib.GribTables.Parameter;

	/// <summary>
	/// Superclass for local table implementations.
	/// A Local table overrides some of the methods in Grib2Tables.
	/// 
	/// @author John
	/// @since 6/22/11
	/// </summary>
	public abstract class LocalTables : Grib2Tables 
	{
	  protected internal IDictionary<int, Grib2Parameter> localParams = new Dictionary<int, Grib2Parameter>(); // subclass must set

	  internal LocalTables(Grib2TableConfig config) : base(config)
	  {
	  }

	  public override string getParamTablePathUsedFor(int discipline, int category, int number)
	  {
		return isLocal(discipline, category, number) ? base.getParamTablePathUsedFor(discipline, category, number) : config.Path;
	  }

	  public override ImmutableList<Parameter> Parameters
	  {
		  get
		  {
			return LocalParameters;
		  }
	  }

	  protected internal virtual ImmutableList<Parameter> LocalParameters
	  {
		  get
		  {
	// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
			return ImmutableList<GribTables.Parameter>.copyOf(localParams.Values.OrderBy(p => p, new ParameterSort()));
		  }
	  }

	  protected internal class ParameterSort : IComparer<Parameter>
	  {
		public virtual int Compare(Parameter p1, Parameter p2)
		{
		  int c = p1.Discipline - p2.Discipline;
		  if (c != 0)
		  {
			return c;
		  }
		  c = p1.Category - p2.Category;
		  if (c != 0)
		  {
			return c;
		  }
		  return p1.Number - p2.Number;
		}
	  }

	  public override string getVariableName(int discipline, int category, int number)
	  {
		if (isLocal(discipline, category, number))
		{
		  return base.getVariableName(discipline, category, number); // LOOK may have to change
		}

		GribTables.Parameter te = getParameter(discipline, category, number);
		if (te == null)
		{
		  return base.getVariableName(discipline, category, number);
		}
		else
		{
		  return te.Name;
		}
	  }

	  public override GribTables.Parameter getParameter(int discipline, int category, int number)
	  {
		Grib2Parameter plocal = localParams[makeParamId(discipline, category, number)];

		if (isLocal(discipline, category, number))
		{
		  GribTables.Parameter pwmo = WmoParamTable.getParameter(discipline, category, number);
		  if (plocal == null)
		  {
			return pwmo;
		  }
		  if (pwmo == null)
		  {
			return plocal;
		  }

		  // allow local table to override all but name, units
		  return new Grib2Parameter(plocal, pwmo.Name, pwmo.Unit);
		}

		return plocal;
	  }

	  public override GribTables.Parameter getParameterRaw(int discipline, int category, int number)
	  {
		return localParams[makeParamId(discipline, category, number)];
	  }

	}

}
