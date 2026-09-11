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
	using Dimension = NetcdfSharp.ucar.nc2.Dimension;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;


	/// <summary>
	/// This implements a VerticalTransform using an existing 3D variable.
	/// This is a common case when the 3D pressure or height field is stored in the file.
	/// 
	/// @author john
	/// </summary>
	public class VTfromExistingData : VerticalTransformImpl
	{

	  /// <summary>
	  /// The name of the Parameter whose value is the variable that contains the 2D Height or Pressure field
	  /// </summary>
	  public const string existingDataField = "existingDataField";

	  /// <summary>
	  /// The variable that contains the 2D Height or Pressure field
	  /// </summary>
	  private Variable existingData;

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="ds"> containing Dataset </param>
	  /// <param name="timeDim"> time Dimension </param>
	  /// <param name="params"> list of transformation Parameters </param>
	  public VTfromExistingData(NetcdfFile ds, Dimension timeDim, IList<Parameter> @params) : base(timeDim)
	  {
		string vname = getParameterStringValue(@params, existingDataField);
		this.existingData = ds.findVariable(vname);
		units = existingData.UnitsString;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public ArrayDouble.D3 getCoordinateArray(int timeIndex) throws IOException, InvalidRangeException
	  public override ArrayDouble.D3 getCoordinateArray(int timeIndex)
	  {
		NcArray data = readArray(existingData, timeIndex);

		// copy for now - better to just return Array, with promise its rank 3
		int[] shape = data.Shape;
		ArrayDouble.D3 ddata = (ArrayDouble.D3) NcArray.factory(DataType.DOUBLE, shape);
		MAMath.copyDouble(ddata, data);
		return ddata;
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


		ArrayDouble.D3 ddata = getCoordinateArray(timeIndex);
		int[] origin = new int[] {0, yIndex, xIndex};
		int[] shape = new int[] {ddata.Shape[0], 1, 1};

		return (ArrayDouble.D1) ddata.section(origin, shape).reduce();
	  }

	}


}
