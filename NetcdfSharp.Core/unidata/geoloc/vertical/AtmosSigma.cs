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
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using ArrayDouble = NetcdfSharp.ucar.ma2.ArrayDouble;
	using D1 = NetcdfSharp.ucar.ma2.ArrayDouble.D1;
	using Index = NetcdfSharp.ucar.ma2.Index;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Dimension = NetcdfSharp.ucar.nc2.Dimension;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using SimpleUnit = NetcdfSharp.ucar.nc2.units.SimpleUnit;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;


	/// <summary>
	/// Create a 3D height(z,y,x) array using the CF formula for
	/// "atmospheric sigma vertical coordinate".
	/// <para>
	/// <strong>pressure(x,y,z) = ptop + sigma(z)*surfacePressure(x,y)</strong>
	/// 
	/// @author caron
	/// </para>
	/// </summary>
	/// <seealso cref="<a href="http://cf-pcmdi.llnl.gov/">http://cf-pcmdi.llnl.gov/</a>"/>

	public class AtmosSigma : VerticalTransformImpl
	{

	  /// <summary>
	  /// P-naught identifier
	  /// </summary>
	  public const string PTOP = "PressureTop_variableName";

	  /// <summary>
	  /// Surface pressure name identifier
	  /// </summary>
	  public const string PS = "SurfacePressure_variableName";

	  /// <summary>
	  /// The "depth" variable name identifier
	  /// </summary>
	  public const string SIGMA = "Sigma_variableName";

	  /// <summary>
	  /// The ps, sigma variables
	  /// </summary>
	  private Variable psVar;

	  /// <summary>
	  /// The sigma array, function of z
	  /// </summary>
	  private double[] sigma;

	  /// <summary>
	  /// Top of the model
	  /// </summary>
	  private double ptop;

	  /// <summary>
	  /// Create a new vertical transform for Ocean S coordinates
	  /// </summary>
	  /// <param name="ds"> dataset </param>
	  /// <param name="timeDim"> time dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public AtmosSigma(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {

		string psName = getParameterStringValue(@params, PS);
		psVar = ds.findVariable(psName);

		string ptopName = getParameterStringValue(@params, PTOP);
		Variable ptopVar = ds.findVariable(ptopName);
		try
		{
		  this.ptop = ptopVar.readScalarDouble();
		}
		catch (IOException e)
		{
		  throw new ArgumentException("AtmosSigma failed to read " + ptopVar + " err= " + e.Message);
		}

		string sigmaName = getParameterStringValue(@params, SIGMA);
		Variable sigmaVar = ds.findVariable(sigmaName);

		try
		{
		  NcArray data = sigmaVar.read();
		  sigma = (double[]) data.get1DJavaArray(typeof(double));
		}
		catch (IOException e)
		{
		  throw new ArgumentException("AtmosSigma failed to read " + sigmaName + " err= " + e.Message);
		}

		units = psVar.findAttributeString(CDM.UNITS, "none");

		string ptopUnitStr = ptopVar.findAttributeString(CDM.UNITS, "none");
		if (!units.Equals(ptopUnitStr, StringComparison.OrdinalIgnoreCase))
		{
		  // Convert ptopVar to units of psVar
		  SimpleUnit psUnit = SimpleUnit.factory(units);
		  SimpleUnit ptopUnit = SimpleUnit.factory(ptopUnitStr);
		  double factor = ptopUnit.convertTo(1.0, psUnit);
		  this.ptop = this.ptop * factor;
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
// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.ArrayDouble.D3 getCoordinateArray(int timeIndex) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public override ArrayDouble.D3 getCoordinateArray(int timeIndex)
	  {
		NcArray ps = readArray(psVar, timeIndex);
		Index psIndex = ps.Index;

		int nz = sigma.Length;
		int[] shape2D = ps.Shape;
		int ny = shape2D[0];
		int nx = shape2D[1];

		ArrayDouble.D3 result = new ArrayDouble.D3(nz, ny, nx);

		for (int y = 0; y < ny; y++)
		{
		  for (int x = 0; x < nx; x++)
		  {
			double psVal = ps.getDouble(psIndex.set(y, x));
			for (int z = 0; z < nz; z++)
			{
			  result.set(z, y, x, ptop + sigma[z] * (psVal - ptop));
			}
		  }
		}

		return result;
	  }

	  /// <summary>
	  /// Get the 1D vertical coordinate array for this time step and point
	  /// 
	  /// (needds test!!!)
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <param name="xIndex"> the x index </param>
	  /// <param name="yIndex"> the y index </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> problem reading data </exception>
	  /// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> _more_ </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public override ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex)
	  {

		NcArray ps = readArray(psVar, timeIndex);
		Index psIndex = ps.Index;
		int nz = sigma.Length;
		ArrayDouble.D1 result = new ArrayDouble.D1(nz);

		double psVal = ps.getDouble(psIndex.set(yIndex, xIndex));
		for (int z = 0; z < nz; z++)
		{
		  result.set(z, ptop + sigma[z] * (psVal - ptop));
		}

		return result;

	  }

	}


}
