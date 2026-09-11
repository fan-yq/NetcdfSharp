// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.geoloc.vertical
{
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using Dimension = NetcdfSharp.ucar.nc2.Dimension;
	using NetcdfSharp.ucar.ma2;
	using D1 = NetcdfSharp.ucar.ma2.ArrayDouble.D1;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;

	/// <summary>
	/// Implement CF vertical coordinate "atmosphere_ln_pressure_coordinate"
	/// pressure(z) = p0 * exp(-lev(k))" .
	/// 
	/// Theres a problem here, since its not 3D, we dont know what the 2D extent is.
	/// DO NOT USE: see CF1Convention.makeAtmLnCoordinate()
	/// 
	/// @author caron </summary>
	/// <seealso cref="<a href="http://cf-pcmdi.llnl.gov/">http://cf-pcmdi.llnl.gov/</a>"
	/// @since May 6, 2008/>
	public class AtmosLnPressure : VerticalTransformImpl
	{
	  public const string P0 = "ReferencePressureVariableName";
	  public const string LEV = "VerticalCoordinateVariableName";

	  private readonly NcArray pressure;

	  /// <summary>
	  /// Create a new vertical transform for Ocean S coordinates
	  /// </summary>
	  /// <param name="ds"> dataset </param>
	  /// <param name="timeDim"> time dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public AtmosLnPressure(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {

		string p0name = getParameterStringValue(@params, P0);
		Variable p0var = ds.findVariable(p0name);
		double p0;
		try
		{
		  p0 = p0var.readScalarDouble();
		}
		catch (IOException e)
		{
		  throw new ArgumentException("AtmosLnPressure failed to read " + p0name + " err= " + e.Message);
		}

		string levName = getParameterStringValue(@params, LEV);
		Variable levVar = ds.findVariable(levName);

		try
		{
		  NcArray lev = levVar.read();
		  Debug.Assert(lev.Rank == 1);
		  pressure = new ArrayDouble.D1((int) lev.Size);
		  IndexIterator ii = pressure.IndexIterator;
		  while (lev.hasNext())
		  {
			double result = p0 * Math.Exp(-lev.nextDouble());
			ii.DoubleNext = result;
		  }

		}
		catch (IOException e)
		{
		  throw new ArgumentException("AtmosLnPressure failed to read " + levName + " err= " + e.Message);
		}

		units = p0var.UnitsString;
	  }

	  /// <summary>
	  /// Get the 3D vertical coordinate array for this time step.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <returns> vertical coordinate array </returns>
	  public override ArrayDouble.D3 getCoordinateArray(int timeIndex)
	  {

		int nz = (int) pressure.Size;
		int[] shape2D = pressure.Shape;
		int ny = shape2D[0];
		int nx = shape2D[1];

		ArrayDouble.D3 result = new ArrayDouble.D3(nz, ny, nx);

		IndexIterator ii = pressure.IndexIterator;
		for (int z = 0; z < nz; z++)
		{
		  double p = ii.DoubleNext;
		  for (int y = 0; y < ny; y++)
		  {
			for (int x = 0; x < nx; x++)
			{
			  result.set(z, y, x, p);
			}
		  }
		}

		return result;
	  }

	  /// <summary>
	  /// Get the 1D vertical coordinate array for this time step and point
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <param name="xIndex"> the x index </param>
	  /// <param name="yIndex"> the y index </param>
	  /// <returns> vertical coordinate array </returns>
	  public override D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex)
	  {

		throw new System.NotSupportedException("1D subsetting is not implemented yet for this vertical tranformation");
	  }

	}



}
