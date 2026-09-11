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
	using NetcdfSharp.ucar.nc2;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;


	/// <summary>
	/// Create a 3D height(z,y,x) array using the CF formula for
	/// "ocean s vertical coordinate g2".
	/// standard name: ocean_s_coordinate_g2
	/// 
	/// @author Sachin (skbhate@ngi.msstate.edu) </summary>
	/// <seealso cref="<a href="http://cf-pcmdi.llnl.gov/">http://cf-pcmdi.llnl.gov/</a>"/>
	/// <seealso cref="<a href="
	///      "https://www.myroms.org/wiki/index.php/Vertical_S-coordinate#Metadata_Considerations">https://www.myroms.org/wiki/index.php/Vertical_S-coordinate#Metadata_Considerations</a>/>

	public class OceanSG2 : VerticalTransformImpl
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
	  /// The "C" variable name identifier
	  /// </summary>
	  public const string C = "c_variableName";
	  /// <summary>
	  /// the values of depth_c
	  /// </summary>
	  private double depth_c;

	  /// <summary>
	  /// The eta, s, C and depth variables
	  /// </summary>
	  private Variable etaVar, sVar, depthVar, cVar, depthCVar;


	  /// <summary>
	  /// Create a new vertical transform for Ocean_S_coordinate_g2
	  /// </summary>
	  /// <param name="ds"> dataset </param>
	  /// <param name="timeDim"> time dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public OceanSG2(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {

		string etaName = getParameterStringValue(@params, ETA);
		string sName = getParameterStringValue(@params, S);
		string depthName = getParameterStringValue(@params, DEPTH);
		string depthCName = getParameterStringValue(@params, DEPTH_C);
		string cName = getParameterStringValue(@params, C);

		etaVar = ds.findVariable(etaName);
		sVar = ds.findVariable(sName);
		depthVar = ds.findVariable(depthName);
		depthCVar = ds.findVariable(depthCName);
		cVar = ds.findVariable(cName);

		units = depthVar.findAttributeString(CDM.UNITS, "none");
	  }

	  /// <summary>
	  /// Get the 3D vertical coordinate array for this time step.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> problem reading data </exception>
	  /// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> _more_ </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public ArrayDouble.D3 getCoordinateArray(int timeIndex) throws IOException, InvalidRangeException
	  public override ArrayDouble.D3 getCoordinateArray(int timeIndex)
	  {
		NcArray etaArray = readArray(etaVar, timeIndex);
		NcArray sArray = readArray(sVar, timeIndex);
		NcArray depthArray = readArray(depthVar, timeIndex);
		NcArray cArray = readArray(cVar, timeIndex);

		depth_c = depthCVar.readScalarDouble();

		return makeHeight(etaArray, sArray, depthArray, cArray, depth_c);
	  }

	  /// <summary>
	  /// Get the 1D vertical coordinate array for this time step and
	  /// the specified X,Y index for Lat-Lon point.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <param name="xIndex"> the x index </param>
	  /// <param name="yIndex"> the y index </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> problem reading data </exception>
	  /// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> _more_ </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex) throws IOException, InvalidRangeException
	  public override ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex)
	  {
		NcArray etaArray = readArray(etaVar, timeIndex);
		NcArray sArray = readArray(sVar, timeIndex);
		NcArray depthArray = readArray(depthVar, timeIndex);
		NcArray cArray = readArray(cVar, timeIndex);

		depth_c = depthCVar.readScalarDouble();

		return makeHeight1D(etaArray, sArray, depthArray, cArray, depth_c, xIndex, yIndex);
	  }

	  /// <summary>
	  /// Make height from the given data. <br>
	  /// 
	  /// 
	  /// height(x,y,z) = eta(x,y) + ( eta(x,y) + depth([n],x,y) ) * S(x,y,z)
	  /// 
	  /// where,
	  /// S(x,y,z) = (depth_c*s(z) + (depth([n],x,y) * C(z)) / (depth_c + depth([n],x,y))
	  /// /
	  /// </summary>
	  /// <param name="eta"> eta Array </param>
	  /// <param name="s"> s Array </param>
	  /// <param name="depth"> depth Array </param>
	  /// <param name="c"> c Array </param>
	  /// <param name="depth_c"> value of depth_c </param>
	  /// <returns> height data </returns>
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

			  double fac1 = depth.getDouble(depthIndex.set(y, x));
			  double term2 = fac1 * cz;

			  double Sterm = (term1 + term2) / (depth_c + fac1);

			  double term3 = eta.getDouble(etaIndex.set(y, x));
			  double term4 = (term3 + fac1) * Sterm;
			  double hterm = term3 + term4;

			  height.set(z, y, x, hterm);

			}
		  }
		}

		return height;
	  }


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

		  double fac1 = depth.getDouble(depthIndex.set(y_index, x_index));
		  double term2 = fac1 * cz;

		  double Sterm = (term1 + term2) / (depth_c + fac1);

		  double term3 = eta.getDouble(etaIndex.set(y_index, x_index));
		  double term4 = (term3 + fac1) * Sterm;
		  double hterm = term3 + term4;

		  height.set(z, hterm);

		}

		return height;
	  }
	}

}
