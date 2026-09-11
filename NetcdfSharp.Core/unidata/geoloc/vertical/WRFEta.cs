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
	using Index = NetcdfSharp.ucar.ma2.Index;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using D1 = NetcdfSharp.ucar.ma2.ArrayDouble.D1;
	using Dimension = NetcdfSharp.ucar.nc2.Dimension;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;

	/// <summary>
	/// Models the vertical coordinate for the Weather Research and Forecast (WRF) model's vertical Eta coordinate </summary>
	public class WRFEta : VerticalTransformImpl
	{
	  public const string BasePressureVariable = "base_presure";
	  public const string PerturbationPressureVariable = "perturbation_presure";
	  public const string BaseGeopotentialVariable = "base_geopotential";
	  public const string PerturbationGeopotentialVariable = "perturbation_geopotential";
	  public const string IsStaggeredX = "staggered_x";
	  public const string IsStaggeredY = "staggered_y";
	  public const string IsStaggeredZ = "staggered_z";

	  private Variable pertVar;
	  private Variable baseVar;
	  private bool isXStag, isYStag, isZStag;

	  /// <summary>
	  /// Construct a vertical coordinate for the Weather Research and Forecast
	  /// (WRF) model's vertical Eta coordinate
	  /// </summary>
	  /// <param name="ds"> netCDF dataset </param>
	  /// <param name="timeDim"> time dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public WRFEta(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {

		isXStag = getParameterBooleanValue(@params, IsStaggeredX);
		isYStag = getParameterBooleanValue(@params, IsStaggeredY);
		isZStag = getParameterBooleanValue(@params, IsStaggeredZ);

		string pertVarName;
		string baseVarName;
		if (isZStag)
		{
		  // Geopotential is provided on the staggered z grid
		  // so we'll transform like grids to height levels
		  pertVarName = getParameterStringValue(@params, PerturbationGeopotentialVariable);
		  baseVarName = getParameterStringValue(@params, BaseGeopotentialVariable);
		  units = "m"; // PH and PHB are in m^2/s^2, so dividing by g=9.81 m/s^2 results in meters
		}
		else
		{
		  // Pressure is provided on the non-staggered z grid
		  // so we'll transform like grids to pressure levels
		  pertVarName = getParameterStringValue(@params, PerturbationPressureVariable);
		  baseVarName = getParameterStringValue(@params, BasePressureVariable);
		  units = "Pa"; // P and PB are in Pascals //ADD:safe assumption? grab unit attribute?
		}

		pertVar = ds.findVariable(pertVarName);
		baseVar = ds.findVariable(baseVarName);

		if (pertVar == null)
		{
		  throw new Exception("Cant find perturbation pressure variable= " + pertVarName + " in WRF file");
		}
		if (baseVar == null)
		{
		  throw new Exception("Cant find base state pressure variable=  " + baseVarName + " in WRF file");
		}
	  }

	  /// <summary>
	  /// Get the 3D vertical coordinate array for this time step.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="IOException"> problem reading data </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.ArrayDouble.D3 getCoordinateArray(int timeIndex) throws NetcdfSharp.java.io.IOException
	  public override ArrayDouble.D3 getCoordinateArray(int timeIndex)
	  {
		ArrayDouble.D3 array;

		NcArray pertArray = getTimeSlice(pertVar, timeIndex);
		NcArray baseArray = getTimeSlice(baseVar, timeIndex);

		// ADD: use MAMath?
		// ADD: use IndexIterator from getIndexIteratorFast?
		int[] shape = pertArray.Shape;
		// ADD: assert that rank = 3
		// ADD: assert that both arrays are same shape
		int ni = shape[0];
		int nj = shape[1];
		int nk = shape[2];

		array = new ArrayDouble.D3(ni, nj, nk);
		Index index = array.Index;

		for (int i = 0; i < ni; i++)
		{
		  for (int j = 0; j < nj; j++)
		  {
			for (int k = 0; k < nk; k++)
			{
			  index.set(i, j, k);
			  double d = pertArray.getDouble(index) + baseArray.getDouble(index);
			  if (isZStag)
			  {
				d = d / 9.81; // convert geopotential to height
			  }
			  array.setDouble(index, d);
			}
		  }
		}

		if (isXStag)
		{
		  array = addStagger(array, 2); // assuming x dim index is 2
		}
		if (isYStag)
		{
		  array = addStagger(array, 1); // assuming y dim index is 1
		}

		return array;
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
// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public override ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex)
	  {
		ArrayDouble.D3 data = getCoordinateArray(timeIndex);

		int[] origin = new int[3];
		int[] shape = new int[3];

		origin[0] = 0;
		origin[1] = yIndex;
		origin[2] = xIndex;

		shape[0] = data.Shape[0];
		shape[1] = 1;
		shape[2] = 1;

		NcArray tmp = data.section(origin, shape);
		return (ArrayDouble.D1) tmp.reduce();
	  }

	  /// <summary>
	  /// Add 1 to the size of the array for the given dimension.
	  /// Use linear average and interpolation to fill in the values.
	  /// </summary>
	  /// <param name="array"> use this array </param>
	  /// <param name="dimIndex"> use this dimension </param>
	  /// <returns> new array with stagger </returns>
	  private ArrayDouble.D3 addStagger(ArrayDouble.D3 array, int dimIndex)
	  {

		// ADD: assert 0<=dimIndex<=2

		int[] shape = array.Shape;
		int[] newShape = new int[3];
		System.Array.Copy(shape, 0, newShape, 0, 3);

		newShape[dimIndex]++;
		int ni = newShape[0];
		int nj = newShape[1];
		int nk = newShape[2];
		ArrayDouble.D3 newArray = new ArrayDouble.D3(ni, nj, nk);
		// Index newIndex = newArray.getIndex();

		// extract 1d array to be extended
		int n = shape[dimIndex]; // length of extracted array
		double[] d = new double[n]; // tmp array to hold extracted values
		int[] eshape = new int[3]; // shape of extracted array
		int[] neweshape = new int[3]; // shape of new array slice to write into
		for (int i = 0; i < 3; i++)
		{
		  eshape[i] = (i == dimIndex) ? n : 1;
		  neweshape[i] = (i == dimIndex) ? n + 1 : 1;
		}
		int[] origin = new int[3];

		try
		{
		  // loop through the other 2 dimensions and "extrapinterpolate" the other
		  for (int i = 0; i < ((dimIndex == 0) ? 1 : ni); i++)
		  {
			for (int j = 0; j < ((dimIndex == 1) ? 1 : nj); j++)
			{
			  for (int k = 0; k < ((dimIndex == 2) ? 1 : nk); k++)
			  {
				origin[0] = i;
				origin[1] = j;
				origin[2] = k;
				IndexIterator it = array.section(origin, eshape).getIndexIterator();
				for (int l = 0; l < n; l++)
				{
				  d[l] = it.DoubleNext; // get the original values
				}
				double[] d2 = extrapinterpolate(d); // compute new values
				// define slice of new array to write into
				IndexIterator newit = newArray.section(origin, neweshape).getIndexIterator();
				for (int l = 0; l < n + 1; l++)
				{
				  newit.DoubleNext = d2[l];
				}
			  }
			}
		  }
		}
		catch (InvalidRangeException)
		{
		  // ADD: report error?
		  return null;
		}

		return newArray;
	  }

	  /// <summary>
	  /// Add one element to the array by linear interpolation
	  /// and extrapolation at the ends.
	  /// </summary>
	  /// <param name="array"> input array </param>
	  /// <returns> extrapolated/interpolated array </returns>
	  private double[] extrapinterpolate(double[] array)
	  {
		int n = array.Length;
		double[] d = new double[n + 1];

		// end points from linear extrapolation
		// equations confirmed by Christopher Lindholm
		d[0] = 1.5 * array[0] - 0.5 * array[1];
		d[n] = 1.5 * array[n - 1] - 0.5 * array[n - 2];

		// inner points from simple average
		for (int i = 1; i < n; i++)
		{
		  d[i] = 0.5 * (array[i - 1] + array[i]);
		}

		return d;
	  }

	  /// <summary>
	  /// Extract an NcArray (with rank reduced by one) from the Variable
	  /// for the given time index.
	  /// </summary>
	  /// <param name="v"> variable to extract from </param>
	  /// <param name="timeIndex"> time index </param>
	  /// <returns> NcArray of data </returns>
	  /// <exception cref="IOException"> problem getting NcArray </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.Array getTimeSlice(NetcdfSharp.ucar.nc2.Variable v, int timeIndex) throws NetcdfSharp.java.io.IOException
	  private NcArray getTimeSlice(Variable v, int timeIndex)
	  {
		// ADD: this would make a good utility method
		// ADD: use Array.slice?
		int[] shape = v.Shape;
		int[] origin = new int[v.Rank];

		if (TimeDimension != null)
		{
		  int dimIndex = v.findDimensionIndex(TimeDimension.getShortName());
		  if (dimIndex >= 0)
		  {
			shape[dimIndex] = 1;
			origin[dimIndex] = timeIndex;
		  }
		}

		try
		{
		  return v.read(origin, shape).reduce();
		}
		catch (InvalidRangeException e)
		{
		  throw new IOException(e.Message, e);
		}
	  }

	}


}
