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
	using NetcdfSharp.ucar.nc2;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;

	/// <summary>
	/// Create a 3D height(z,y,x) array using the CF formula for
	/// "ocean s vertical coordinate".
	/// <p/>
	/// Modified April 20, 2009 by sachin
	/// 
	/// @author caron
	/// @author skbhate@gmail.com </summary>
	/// <seealso cref=""https://www.myroms.org/wiki/index.php/Vertical_S-coordinate.Metadata_Considerations""/>

	public class OceanS : VerticalTransformImpl
	{

	  /// <summary>
	  /// The eta variable name identifier
	  /// </summary>
	  public const string ETA = "Eta_variableName";

	  /// <summary>
	  /// The "s" variable name identifier
	  /// </summary>
	  public const string S = "S_variableName";

	  /// <summary>
	  /// The "depth" variable name identifier
	  /// </summary>
	  public const string DEPTH = "Depth_variableName";

	  /// <summary>
	  /// The "depth c" variable name identifier
	  /// </summary>
	  public const string DEPTH_C = "Depth_c_variableName";

	  /// <summary>
	  /// The "a" variable name
	  /// </summary>
	  public const string A = "A_variableName";

	  /// <summary>
	  /// The "b" variable name
	  /// </summary>
	  public const string B = "B_variableName";

	  /// <summary>
	  /// the values of depth_c, a, and b
	  /// </summary>
	  private double depth_c;

	  /// <summary>
	  /// The eta, s and depth variables
	  /// </summary>
	  private Variable etaVar, sVar, depthVar, aVar, bVar, depthCVar;

	  /// <summary>
	  /// the c array
	  /// </summary>
	  private NcArray c;

	  /// <summary>
	  /// Create a new vertical transform for Ocean S coordinates
	  /// </summary>
	  /// <param name="ds"> dataset </param>
	  /// <param name="timeDim"> time dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public OceanS(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {

		string etaName = getParameterStringValue(@params, ETA);
		string sName = getParameterStringValue(@params, S);
		string depthName = getParameterStringValue(@params, DEPTH);
		string aName = getParameterStringValue(@params, A);
		string bName = getParameterStringValue(@params, B);
		string depthCName = getParameterStringValue(@params, DEPTH_C);

		etaVar = ds.findVariable(etaName);
		sVar = ds.findVariable(sName);
		depthVar = ds.findVariable(depthName);
		aVar = ds.findVariable(aName);
		bVar = ds.findVariable(bName);
		depthCVar = ds.findVariable(depthCName);

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
		NcArray etaArray = readArray(etaVar, timeIndex);
		NcArray sArray = readArray(sVar, timeIndex);
		NcArray depthArray = readArray(depthVar, timeIndex);

		if (null == c)
		{
		  double a = aVar.readScalarDouble();
		  double b = bVar.readScalarDouble();
		  depth_c = depthCVar.readScalarDouble();
		  c = makeC(sArray, a, b);
		}

		return makeHeight(etaArray, sArray, depthArray, c, depth_c);
	  }

	  /// <summary>
	  /// Get the 1D vertical coordinate array for this time step and
	  /// the specified X,Y index for Lat-Lon point.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <param name="xIndex"> the x index </param>
	  /// <param name="yIndex"> the y index </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="IOException"> problem reading data </exception>
	  /// <exception cref="InvalidRangeException"> _more_ </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex) throws IOException, InvalidRangeException
	  public override ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex)
	  {
		NcArray etaArray = readArray(etaVar, timeIndex);
		NcArray sArray = readArray(sVar, timeIndex);
		NcArray depthArray = readArray(depthVar, timeIndex);

		if (null == c)
		{
		  double a = aVar.readScalarDouble();
		  double b = bVar.readScalarDouble();
		  depth_c = depthCVar.readScalarDouble();
		  c = makeC(sArray, a, b);
		}


		return makeHeight1D(etaArray, sArray, depthArray, c, depth_c, xIndex, yIndex);
	  }

	  // C(z) = (1-b)*sinh(a*s(z))/sinh(a) + b*(tanh(a*(s(z)+0.5))/(2*tanh(0.5*a))-0.5)

	  /// <summary>
	  /// Make the C array
	  /// </summary>
	  /// <param name="s"> s Array </param>
	  /// <param name="a"> "a" value </param>
	  /// <param name="b"> "b" value </param>
	  /// <returns> the C array </returns>
	  private NcArray makeC(NcArray s, double a, double b)
	  {
		int nz = (int) s.Size;
		Index sIndex = s.Index;
		if (a == 0)
		{
		  return s; // per R. Signell, USGS
		}

		ArrayDouble.D1 c = new ArrayDouble.D1(nz);

		double fac1 = 1.0 - b;
		double denom1 = 1.0 / Math.Sinh(a);
		double denom2 = 1.0 / (2.0 * Math.Tanh(0.5 * a));

		for (int i = 0; i < nz; i++)
		{
		  double sz = s.getDouble(sIndex.set(i));
		  double term1 = fac1 * Math.Sinh(a * sz) * denom1;
		  double term2 = b * (Math.Tanh(a * (sz + 0.5)) * denom2 - 0.5);
		  c.set(i, term1 + term2);
		}

		return c;
	  }


	  /// <summary>
	  /// Make height from the given data. <br>
	  /// old equationn: height(x,y,z) = eta(x,y)*(1+s(z)) + depth_c*s(z) + (depth(x,y)-depth_c)*C(z)
	  /// <p/>
	  /// <p/>
	  /// /* -sachin 03/23/09
	  /// The new corrected equation according to Hernan Arango (Rutgers)
	  /// height(x,y,z) = S(x,y,z) + eta(x,y) * (1 + S(x,y,z) / depth(x,y) )
	  /// <p/>
	  /// where,
	  /// S(x,y,z) = depth_c*s(z) + (depth(x,y)-depth_c)*C(z)
	  /// /
	  /// </summary>
	  /// <param name="eta"> eta Array </param>
	  /// <param name="s"> s Array </param>
	  /// <param name="depth"> depth Array </param>
	  /// <param name="c"> c Array </param>
	  /// <param name="depth_c"> value of depth_c </param>
	  /// <returns> hieght data </returns>
	  private ArrayDouble.D3 makeHeight(NcArray eta, NcArray s, NcArray depth, NcArray c, double depth_c)
	  {
		int nz = (int) s.Size;
		Index sIndex = s.Index;
		Index cIndex = c.Index;

		int[] shape2D = eta.Shape;
		int ny = shape2D[0];
		int nx = shape2D[1];
		Index etaIndex = eta.Index;
		Index depthIndex = depth.Index;

		ArrayDouble.D3 height = new ArrayDouble.D3(nz, ny, nx);

		for (int z = 0; z < nz; z++)
		{
		  double sz = s.getDouble(sIndex.set(z));
		  double cz = c.getDouble(cIndex.set(z));

		  double term1 = depth_c * sz;

		  for (int y = 0; y < ny; y++)
		  {
			for (int x = 0; x < nx; x++)
			{
			  // -sachin 03/23/09 modifications according to corrected equation.
			  double fac1 = depth.getDouble(depthIndex.set(y, x));
			  double term2 = (fac1 - depth_c) * cz;

			  double Sterm = term1 + term2;

			  double term3 = eta.getDouble(etaIndex.set(y, x));
			  double term4 = 1 + Sterm / fac1;
			  double hterm = Sterm + term3 * term4;

			  height.set(z, y, x, hterm);

			}
		  }
		}

		return height;
	  }


	  // Modify method 'makeHeight' as new method for getting vertical coordinate array for single point.
	  // - sachin
	  private ArrayDouble.D1 makeHeight1D(NcArray eta, NcArray s, NcArray depth, NcArray c, double depth_c, int x_index, int y_index)
	  {
		int nz = (int) s.Size;
		Index sIndex = s.Index;
		Index cIndex = c.Index;


		Index etaIndex = eta.Index;
		Index depthIndex = depth.Index;

		ArrayDouble.D1 height = new ArrayDouble.D1(nz);

		for (int z = 0; z < nz; z++)
		{
		  double sz = s.getDouble(sIndex.set(z));
		  double cz = c.getDouble(cIndex.set(z));

		  double term1 = depth_c * sz;
		  // -sachin 03/06/09 modifications according to corrected equation.

		  double fac1 = depth.getDouble(depthIndex.set(y_index, x_index));
		  double term2 = (fac1 - depth_c) * cz;


		  double Sterm = term1 + term2;

		  double term3 = eta.getDouble(etaIndex.set(y_index, x_index));
		  double term4 = 1 + Sterm / fac1;
		  double hterm = Sterm + term3 * term4;

		  height.set(z, hterm);
		}

		return height;
	  }
	}


}
