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
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Range = NetcdfSharp.ucar.ma2.Range;
	using NetcdfSharp.ucar.nc2;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;

	/// <summary>
	/// A transformation to a vertical reference coordinate system,
	/// such as height or pressure.
	/// 
	/// @author Unidata Development Team
	/// </summary>

	public abstract class VerticalTransformImpl : VerticalTransform
	{
	  protected internal string units;
	  private Dimension timeDim;

	  /// <summary>
	  /// Construct a VerticalCoordinate
	  /// </summary>
	  /// <param name="timeDim"> time dimension </param>
	  public VerticalTransformImpl(Dimension timeDim)
	  {
		this.timeDim = timeDim;
	  }

	  /// <summary>
	  /// Get the 3D vertical coordinate array for this time step.
	  /// </summary>
	  /// <param name="timeIndex"> the time index. Ignored if !isTimeDependent(). </param>
	  /// <returns> vertical coordinate array </returns>
	  /// <exception cref="InvalidRangeException"> _more_ </exception>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> problem reading the data </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public abstract NetcdfSharp.ucar.ma2.ArrayDouble.D3 getCoordinateArray(int timeIndex) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
	  public abstract NetcdfSharp.ucar.ma2.ArrayDouble.D3 getCoordinateArray(int timeIndex);

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
// ORIGINAL LINE: public abstract NetcdfSharp.ucar.ma2.ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
	  public abstract NetcdfSharp.ucar.ma2.ArrayDouble.D1 getCoordinateArray1D(int timeIndex, int xIndex, int yIndex);

	  /// <summary>
	  /// Get the unit string for the vertical coordinate.
	  /// </summary>
	  /// <returns> unit string </returns>
	  public virtual string UnitString
	  {
		  get
		  {
			return units;
		  }
	  }

	  /// <summary>
	  /// Get whether this coordinate is time dependent.
	  /// </summary>
	  /// <returns> true if time dependent </returns>
	  public virtual bool TimeDependent
	  {
		  get
		  {
			return (timeDim != null);
		  }
	  }

	  /// <summary>
	  /// Get the time Dimension
	  /// </summary>
	  /// <returns> time Dimension </returns>
	  protected internal virtual Dimension TimeDimension
	  {
		  get
		  {
			return timeDim;
		  }
	  }


	  /// <summary>
	  /// Read the data <seealso cref="NetcdfSharp.ucar.ma2.Array"/> from the variable, at the specified
	  /// time index if applicable. If the variable does not have a time
	  /// dimension, the data array will have the same rank as the Variable.
	  /// If the variable has a time dimension, the data array will have rank-1.
	  /// </summary>
	  /// <param name="v"> variable to read </param>
	  /// <param name="timeIndex"> time index, ignored if !isTimeDependent() </param>
	  /// <returns> Array from the variable at that time index
	  /// </returns>
	  /// <exception cref="IOException"> problem reading data </exception>
	  /// <exception cref="InvalidRangeException"> _more_ </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected NetcdfSharp.ucar.ma2.Array readArray(Variable v, int timeIndex) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  protected internal virtual Array readArray(Variable v, int timeIndex)
	  {
		int[] shape = v.Shape;
		int[] origin = new int[v.Rank];

		if (TimeDimension != null)
		{
		  int dimIndex = v.findDimensionIndex(TimeDimension.ShortName);
		  if (dimIndex >= 0)
		  {
			shape[dimIndex] = 1;
			origin[dimIndex] = timeIndex;
			return v.read(origin, shape).reduce(dimIndex);
		  }
		}

		return v.read(origin, shape);
	  }

	  /// <summary>
	  /// Create a subset of this VerticalTransform.
	  /// </summary>
	  /// <param name="t_range"> subset the time dimension, or null if you want all of it </param>
	  /// <param name="z_range"> subset the vertical dimension, or null if you want all of it </param>
	  /// <param name="y_range"> subset the y dimension, or null if you want all of it </param>
	  /// <param name="x_range"> subset the x dimension, or null if you want all of it
	  /// </param>
	  /// <returns> the subsetted VerticalTransform
	  ///  </returns>
	  public virtual VerticalTransform subset(Range t_range, Range z_range, Range y_range, Range x_range)
	  {
		return new VerticalTransformSubset(this, t_range, z_range, y_range, x_range);
	  }

	  protected internal virtual string getParameterStringValue(IList<Parameter> @params, string name)
	  {
		foreach (Parameter a in @params)
		{
		  if (name.Equals(a.Name, StringComparison.OrdinalIgnoreCase))
		  {
			return a.StringValue;
		  }
		}
		return null;
	  }

	  protected internal virtual bool getParameterBooleanValue(IList<Parameter> @params, string name)
	  {
		foreach (Parameter p in @params)
		{
		  if (name.Equals(p.Name, StringComparison.OrdinalIgnoreCase))
		  {
			return bool.Parse(p.StringValue);
		  }
		}
		return false;
	  }

	}


}
