// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc.vertical
{
	using ArrayDouble = NetcdfSharp.ucar.ma2.ArrayDouble;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Range = NetcdfSharp.ucar.ma2.Range;

	/// <summary>
	/// A transformation to a vertical reference coordinate system,
	/// such as height or pressure.
	/// </summary>
	/// @deprecated will move to NetcdfSharp.ucar.unidata.geoloc in ver6 
	[Obsolete("will move to NetcdfSharp.ucar.unidata.geoloc in ver6")]
	public interface VerticalTransform
	{

	  /// <summary>
	  /// Get the 3D vertical coordinate array for this time step.
	  /// Must be in "canonical order" : z, y, x.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent().
	  /// </param>
	  /// <returns> vertical coordinate array
	  /// </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> problem reading the data </exception>
	  /// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> timeIndex out of bounds </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: NetcdfSharp.ucar.ma2.ArrayDouble.D3 getCoordinateArray(int timeIndex) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
	  ArrayDouble.D3 getCoordinateArray(int timeIndex);

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
// ORIGINAL LINE: NetcdfSharp.ucar.ma2.ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
	  ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex);


	  /// <summary>
	  /// Get the unit string for the vertical coordinate.
	  /// </summary>
	  /// <returns> unit string </returns>
	  string UnitString {get;}

	  /// <summary>
	  /// Get whether this coordinate is time dependent.
	  /// </summary>
	  /// <returns> true if time dependent </returns>
	  bool TimeDependent {get;}

	  /// <summary>
	  /// Create a VerticalTransform as a section of an existing VerticalTransform.
	  /// </summary>
	  /// <param name="t_range"> subset the time dimension, or null if you want all of it </param>
	  /// <param name="z_range"> subset the vertical dimension, or null if you want all of it </param>
	  /// <param name="y_range"> subset the y dimension, or null if you want all of it </param>
	  /// <param name="x_range"> subset the x dimension, or null if you want all of it </param>
	  /// <returns> a new VerticalTransform for the given subset </returns>
	  VerticalTransform subset(Range t_range, Range z_range, Range y_range, Range x_range);
	}


}
