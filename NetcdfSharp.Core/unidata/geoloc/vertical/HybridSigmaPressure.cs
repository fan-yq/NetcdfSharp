// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc.vertical
{
	using NetcdfSharp.ucar.ma2;
	using D1 = NetcdfSharp.ucar.ma2.ArrayDouble.D1;
	using NetcdfSharp.ucar.nc2;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using SimpleUnit = NetcdfSharp.ucar.nc2.units.SimpleUnit;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;

	/// <summary>
	/// Create a 3D height(z,y,x) array using the netCDF CF convention formula for
	/// "Hybrid Sigma Pressure".
	/// <para>
	/// <strong>pressure(x,y,z) = a(z)*p0 + b(z)*surfacePressure(x,y)</strong>
	/// or
	/// </para>
	/// <para>
	/// <strong>pressure(x,y,z) = ap(z) + b(z)*surfacePressure(x,y)</strong>
	/// 
	/// @author caron
	/// </para>
	/// </summary>
	/// <seealso cref="<a href="http://cf-pcmdi.llnl.gov/">http://cf-pcmdi.llnl.gov/</a>"/>
	public class HybridSigmaPressure : VerticalTransformImpl
	{

	  /// <summary>
	  /// P-naught identifier </summary>
	  public const string P0 = "P0_variableName";

	  /// <summary>
	  /// Surface pressure name identifier </summary>
	  public const string PS = "SurfacePressure_variableName";

	  /// <summary>
	  /// The "a" variable name identifier </summary>
	  public const string A = "A_variableName";

	  /// <summary>
	  /// The "a" variable name identifier </summary>
	  public const string AP = "AP_variableName";

	  /// <summary>
	  /// The "b" variable name identifier </summary>
	  public const string B = "B_variableName";

	  /// <summary>
	  /// value of p-naught </summary>
	  private double p0;

	  /// <summary>
	  /// ps, a, and b variables </summary>
	  private Variable psVar, aVar, bVar, p0Var;

	  /// <summary>
	  /// If has AP, this is a pressure value and has to be also in the same units as PS </summary>
	  private string apUnits = "";

	  /// <summary>
	  /// a and b Arrays </summary>
	  private NcArray aArray, bArray;

	  /// <summary>
	  /// Construct a coordinate transform for sigma pressure
	  /// </summary>
	  /// <param name="ds"> netCDF dataset </param>
	  /// <param name="timeDim"> time dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public HybridSigmaPressure(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {
		string psName = getParameterStringValue(@params, PS);
		string aName = getParameterStringValue(@params, A);
		string bName = getParameterStringValue(@params, B);
		string p0Name = getParameterStringValue(@params, P0);
		string apName = getParameterStringValue(@params, AP);

		if (!string.ReferenceEquals(apName, null))
		{
		  aVar = ds.findVariable(apName);
		}
		else
		{
		  aVar = ds.findVariable(aName);
		}

		if (aVar != null)
		{
		  string unitValue = aVar.attributes().findAttributeString(CDM.UNITS, null);
		  if (!string.ReferenceEquals(unitValue, null))
		  {
			apUnits = unitValue;
		  }
		}

		psVar = ds.findVariable(psName);
		bVar = ds.findVariable(bName);
		units = psVar.findAttributeString(CDM.UNITS, "none");
		if (!string.ReferenceEquals(p0Name, null))
		{
		  p0Var = ds.findVariable(p0Name);
		  apUnits = units; // Won't need transformation for AP = A * P0 * 1 (in this case)
		}
	  }

	  /// <summary>
	  /// Get the 3D vertical coordinate array for this time step.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="IOException"> problem reading data </exception>
	  /// <exception cref="InvalidRangeException"> _more_ </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public ArrayDouble.D3 getCoordinateArray(int timeIndex) throws IOException, InvalidRangeException
	  public override ArrayDouble.D3 getCoordinateArray(int timeIndex)
	  {
		NcArray psArray = readArray(psVar, timeIndex);

		if (null == aArray)
		{
		  aArray = aVar.read();
		  bArray = bVar.read();
		  // p0 = (p0Var == null) ? 1.0 : p0Var.readScalarDouble();
		  p0 = computeP0();
		}

		int nz = (int) aArray.Size;
		Index aIndex = aArray.Index;
		Index bIndex = bArray.Index;

		// it's possible to have rank 3 because pressure can have a level, usually 1
		// Check if rank 3 and try to reduce
		if (psArray.Rank == 3)
		{
		  psArray = psArray.reduce(0);
		}

		int[] shape2D = psArray.Shape;
		int ny = shape2D[0];
		int nx = shape2D[1];

		Index psIndex = psArray.Index;

		ArrayDouble.D3 press = new ArrayDouble.D3(nz, ny, nx);

		double ps;
		for (int z = 0; z < nz; z++)
		{
		  double term1 = aArray.getDouble(aIndex.set(z)) * p0;

		  // AP might need unit conversion
		  if (!apUnits.Equals(units))
		  {
			term1 = convertPressureToPSUnits(apUnits, term1);
		  }

		  double bz = bArray.getDouble(bIndex.set(z));

		  for (int y = 0; y < ny; y++)
		  {
			for (int x = 0; x < nx; x++)
			{
			  ps = psArray.getDouble(psIndex.set(y, x));
			  press.set(z, y, x, term1 + bz * ps);
			}
		  }
		}

		return press;
	  }

	  /// <summary>
	  /// Get the 1D vertical coordinate array for this time step and point
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <param name="xIndex"> the x index </param>
	  /// <param name="yIndex"> the y index </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> problem reading data </exception>
	  /// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> _more_ </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex) throws IOException, InvalidRangeException
	  public override D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex)
	  {

		NcArray psArray = readArray(psVar, timeIndex);

		if (null == aArray)
		{
		  aArray = aVar.read();
		  bArray = bVar.read();
		  // p0 = (p0Var == null) ? 1.0 : p0Var.readScalarDouble();
		  p0 = computeP0();
		}

		int nz = (int) aArray.Size;
		Index aIndex = aArray.Index;
		Index bIndex = bArray.Index;

		// it's possible to have rank 3 because pressure can have a level, usually 1
		// Check if rank 3 and try to reduce
		if (psArray.Rank == 3)
		{
		  psArray = psArray.reduce(0);
		}

		Index psIndex = psArray.Index;

		ArrayDouble.D1 press = new ArrayDouble.D1(nz);

		double ps;
		for (int z = 0; z < nz; z++)
		{

		  double term1 = aArray.getDouble(aIndex.set(z)) * p0;
		  // AP might need unit conversion
		  if (!apUnits.Equals(units))
		  {
			term1 = convertPressureToPSUnits(apUnits, term1);
		  }

		  double bz = bArray.getDouble(bIndex.set(z));

		  ps = psArray.getDouble(psIndex.set(yIndex, xIndex));
		  press.set(z, term1 + bz * ps);

		}

		return press;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private double computeP0() throws NetcdfSharp.java.io.IOException
	  private double computeP0()
	  {
		if (p0Var == null)
		{
		  return 1.0; // Has AP variable
		}
		double p0 = p0Var.readScalarDouble();

		// Units check: P0 must have same units as PS
		string p0UnitStr = p0Var.attributes().findAttributeString(CDM.UNITS, null);
		if (string.ReferenceEquals(p0UnitStr, null))
		{
		  throw new System.InvalidOperationException();
		}
		if (!units.Equals(p0UnitStr, StringComparison.OrdinalIgnoreCase))
		{
		  p0 = convertPressureToPSUnits(p0UnitStr, p0);
		}

		return p0;
	  }

	  private double convertPressureToPSUnits(string unit, double val)
	  {
		SimpleUnit psUnit = SimpleUnit.factory(units);
		SimpleUnit ptopUnit = SimpleUnit.factory(unit);
		double factor = ptopUnit.convertTo(1.0, psUnit);
		return val * factor;
	  }

	}


}
