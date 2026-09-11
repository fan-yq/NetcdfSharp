// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc
{
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Points on the Projective geometry plane.
	/// 
	/// TODO will be an Immutable class in ver6
	/// </summary>
	public interface ProjectionPoint
	{

	  /// <summary>
	  /// Standard way to create a ProjectionPoint. </summary>
	  static ProjectionPoint create(double x, double y)
	  {
		return new ProjectionPointImpl(x, y);
	  }

	  /// <summary>
	  /// Standard way to create a "default" ProjectionPoint. </summary>
	  static ProjectionPoint create()
	  {
		return new ProjectionPointImpl();
	  }

	  /// <summary>
	  /// Get the X coordinate
	  /// </summary>
	  /// <returns> the X coordinate </returns>
	  double X {get;}

	  /// <summary>
	  /// Get the Y coordinate
	  /// </summary>
	  /// <returns> the Y coordinate </returns>
	  double Y {get;}

	  /// <summary>
	  /// Returns the result of <seealso cref="nearlyEquals(ProjectionPoint, double)"/>, with
	  /// <seealso cref="Misc.defaultMaxRelativeDiffDouble"/>.
	  /// </summary>
	  bool nearlyEquals(ProjectionPoint other)
	  {
		return nearlyEquals(other, Misc.defaultMaxRelativeDiffDouble);
	  }

	  /// <summary>
	  /// Returns {@code true} if this point is nearly equal to {@code other}. The "near equality" of points is determined
	  /// using <seealso cref="Misc.nearlyEquals(double, double, double)"/>, with the specified maxRelDiff.
	  /// </summary>
	  /// <param name="other"> the other point to check. </param>
	  /// <param name="maxRelDiff"> the maximum <seealso cref="Misc.relativeDifference relative difference"/> the two points may have. </param>
	  /// <returns> {@code true} if this point is nearly equal to {@code other}. </returns>
	  bool nearlyEquals(ProjectionPoint other, double maxRelDiff);
	}

}
