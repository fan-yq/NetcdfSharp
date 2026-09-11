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
	/// Create a 3D height(z,y,x) array using the netCDF CF convention formula for
	/// "Atmospheric Hybrid Height".
	/// <para>
	/// <strong>height(x,y,z) = a(z) + b(z)*orog(x,y)</strong>
	/// 
	/// @author murray
	/// </para>
	/// </summary>
	/// <seealso cref="<a href="http://cf-pcmdi.llnl.gov/">http://cf-pcmdi.llnl.gov/</a>"/>
	public class HybridHeight : VerticalTransformImpl
	{

	  /// <summary>
	  /// Surface pressure name identifier
	  /// </summary>
	  public const string OROG = "Orography_variableName";

	  /// <summary>
	  /// The "a" variable name identifier
	  /// </summary>
	  public const string A = "A_variableName";

	  /// <summary>
	  /// The "b" variable name identifier
	  /// </summary>
	  public const string B = "B_variableName";

	  /// <summary>
	  /// ps, a, and b variables
	  /// </summary>
	  private Variable aVar, bVar, orogVar;

	  /// <summary>
	  /// a and b Arrays
	  /// </summary>
	  private NcArray aArray, bArray;

	  /// <summary>
	  /// Construct a coordinate transform for hybrid height
	  /// </summary>
	  /// <param name="ds"> netCDF dataset </param>
	  /// <param name="timeDim"> time dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public HybridHeight(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {

		string aName = getParameterStringValue(@params, A);
		string bName = getParameterStringValue(@params, B);
		string orogName = getParameterStringValue(@params, OROG);

		aVar = ds.findVariable(aName);
		bVar = ds.findVariable(bName);
		orogVar = ds.findVariable(orogName);
		units = orogVar.findAttributeString(CDM.UNITS, "none");

	  }

	  /// <summary>
	  /// Get the 3D vertical coordinate array for this time step.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="IOException"> problem reading data </exception>
	  /// <exception cref="InvalidRangeException"> not a valid time range </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public ArrayDouble.D3 getCoordinateArray(int timeIndex) throws IOException, InvalidRangeException
	  public override ArrayDouble.D3 getCoordinateArray(int timeIndex)
	  {
		NcArray orogArray = readArray(orogVar, timeIndex);

		if (null == aArray)
		{
		  aArray = aVar.read();
		  bArray = bVar.read();
		}

		int nz = (int) aArray.Size;
		Index aIndex = aArray.Index;
		Index bIndex = bArray.Index;

		int[] shape2D = orogArray.Shape;
		int ny = shape2D[0];
		int nx = shape2D[1];
		Index orogIndex = orogArray.Index;

		ArrayDouble.D3 height = new ArrayDouble.D3(nz, ny, nx);

		for (int z = 0; z < nz; z++)
		{
		  double az = aArray.getDouble(aIndex.set(z));
		  double bz = bArray.getDouble(bIndex.set(z));

		  for (int y = 0; y < ny; y++)
		  {
			for (int x = 0; x < nx; x++)
			{
			  double orog = orogArray.getDouble(orogIndex.set(y, x));
			  height.set(z, y, x, az + bz * orog);
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
		NcArray orogArray = readArray(orogVar, timeIndex);
		if (null == aArray)
		{
		  aArray = aVar.read();
		  bArray = bVar.read();
		}

		int nz = (int) aArray.Size;
		Index aIndex = aArray.Index;
		Index bIndex = bArray.Index;

		Index orogIndex = orogArray.Index;
		ArrayDouble.D1 height = new ArrayDouble.D1(nz);

		for (int z = 0; z < nz; z++)
		{
		  double az = aArray.getDouble(aIndex.set(z));
		  double bz = bArray.getDouble(bIndex.set(z));

		  double orog = orogArray.getDouble(orogIndex.set(yIndex, xIndex));
		  height.set(z, az + bz * orog);
		}
		return height;
	  }

	}


}
