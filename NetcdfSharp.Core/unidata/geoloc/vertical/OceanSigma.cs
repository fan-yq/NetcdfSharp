// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

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
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;

	/// <summary>
	/// Create a 3D height(z,y,x) array using the CF formula for
	/// "ocean sigma vertical coordinate".
	/// 
	/// @author caron </summary>
	/// <seealso cref="<a href="http://cf-pcmdi.llnl.gov/">http://cf-pcmdi.llnl.gov/</a>"/>

	public class OceanSigma : VerticalTransformImpl
	{

	  /// <summary>
	  /// The eta variable name identifier
	  /// </summary>
	  public const string ETA = "Eta_variableName";

	  /// <summary>
	  /// The "s" variable name identifier
	  /// </summary>
	  public const string SIGMA = "Sigma_variableName";

	  /// <summary>
	  /// The "depth" variable name identifier
	  /// </summary>
	  public const string DEPTH = "Depth_variableName";

	  /// <summary>
	  /// The eta, s and depth variables
	  /// </summary>
	  private Variable etaVar, sVar, depthVar;

	  /// <summary>
	  /// Create a new vertical transform for Ocean S coordinates
	  /// </summary>
	  /// <param name="ds"> dataset </param>
	  /// <param name="timeDim"> time dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public OceanSigma(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {

		string etaName = getParameterStringValue(@params, ETA);
		string sName = getParameterStringValue(@params, SIGMA);
		string depthName = getParameterStringValue(@params, DEPTH);

		etaVar = ds.findVariable(etaName);
		sVar = ds.findVariable(sName);
		depthVar = ds.findVariable(depthName);

		units = depthVar.findAttributeString(CDM.UNITS, "none");
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
		NcArray eta = readArray(etaVar, timeIndex);
		NcArray sigma = readArray(sVar, timeIndex);
		NcArray depth = readArray(depthVar, timeIndex);

		int nz = (int) sigma.Size;
		Index sIndex = sigma.Index;

		int[] shape2D = eta.Shape;
		int ny = shape2D[0];
		int nx = shape2D[1];
		Index etaIndex = eta.Index;
		Index depthIndex = depth.Index;

		ArrayDouble.D3 height = new ArrayDouble.D3(nz, ny, nx);

		for (int z = 0; z < nz; z++)
		{
		  double sigmaVal = sigma.getDouble(sIndex.set(z));
		  for (int y = 0; y < ny; y++)
		  {
			for (int x = 0; x < nx; x++)
			{
			  double etaVal = eta.getDouble(etaIndex.set(y, x));
			  double depthVal = depth.getDouble(depthIndex.set(y, x));

			  height.set(z, y, x, etaVal + sigmaVal * (depthVal + etaVal));
			}
		  }
		}

		return height;
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

		NcArray eta = readArray(etaVar, timeIndex);
		NcArray sigma = readArray(sVar, timeIndex);
		NcArray depth = readArray(depthVar, timeIndex);

		int nz = (int) sigma.Size;
		Index sIndex = sigma.Index;

		Index etaIndex = eta.Index;
		Index depthIndex = depth.Index;

		ArrayDouble.D1 height = new ArrayDouble.D1(nz);

		for (int z = 0; z < nz; z++)
		{
		  double sigmaVal = sigma.getDouble(sIndex.set(z));

		  double etaVal = eta.getDouble(etaIndex.set(yIndex, xIndex));
		  double depthVal = depth.getDouble(depthIndex.set(yIndex, xIndex));

		  height.set(z, etaVal + sigmaVal * (depthVal + etaVal));
		}

		return height;
	  }

	}


}
