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
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using ArrayDouble = NetcdfSharp.ucar.ma2.ArrayDouble;
	using D1 = NetcdfSharp.ucar.ma2.ArrayDouble.D1;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Range = NetcdfSharp.ucar.ma2.Range;

	/// <summary>
	/// A subset of a vertical transform.
	/// 
	/// @author Unidata Development Team
	/// </summary>
	public class VerticalTransformSubset : VerticalTransformImpl
	{

	  private VerticalTransform original;
	  private Range t_range;
	  private IList<Range> subsetList = new List<Range>();

	  /// <summary>
	  /// Create a subset of an existing VerticalTransform
	  /// </summary>
	  /// <param name="original"> make a subset of this </param>
	  /// <param name="t_range"> subset the time dimension, or null if you want all of it </param>
	  /// <param name="z_range"> subset the vertical dimension, or null if you want all of it </param>
	  /// <param name="y_range"> subset the y dimension, or null if you want all of it </param>
	  /// <param name="x_range"> subset the x dimension, or null if you want all of it </param>
	  public VerticalTransformSubset(VerticalTransform original, Range t_range, Range z_range, Range y_range, Range x_range) : base(null) // timeDim not used in this class
	  {

		this.original = original;
		this.t_range = t_range;
		subsetList.Add(z_range);
		subsetList.Add(y_range);
		subsetList.Add(x_range);

		units = original.UnitString;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.ArrayDouble.D3 getCoordinateArray(int subsetIndex) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public override ArrayDouble.D3 getCoordinateArray(int subsetIndex)
	  {
		int orgIndex = subsetIndex;
		if (TimeDependent && (t_range != null))
		{
		  orgIndex = t_range.element(subsetIndex);
		}

		ArrayDouble.D3 data = original.getCoordinateArray(orgIndex);

		return (ArrayDouble.D3) data.sectionNoReduce(subsetList);
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


		ArrayDouble.D3 data = original.getCoordinateArray(timeIndex);

		int[] origin = new int[3];
		int[] shape = new int[3];

		shape[0] = subsetList[0].length();
		shape[1] = 1;
		shape[2] = 1;

		origin[0] = timeIndex;
		if (TimeDependent && (t_range != null))
		{
		  origin[0] = t_range.element(timeIndex);
		}

		origin[1] = yIndex;
		origin[2] = xIndex;

		Array section = data.section(origin, shape);

		return (ArrayDouble.D1) section.reduce();


	  }


	  public override bool TimeDependent
	  {
		  get
		  {
			return original.TimeDependent;
		  }
	  }
	}


}
