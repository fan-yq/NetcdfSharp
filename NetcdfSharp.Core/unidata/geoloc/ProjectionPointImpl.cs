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
namespace NetcdfSharp.ucar.unidata.geoloc
{
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;
	using Format = NetcdfSharp.ucar.unidata.util.Format;

	/// <summary>
	/// Implementation of ProjectionPoint
	/// </summary>
	/// @deprecated only use ProjectionPoint in 6. 
	[Obsolete("only use ProjectionPoint in 6."), Serializable]
	public class ProjectionPointImpl : ProjectionPoint
	{
	  private double x, y;

	  /// <summary>
	  /// Default constructor, initialized to 0,0
	  /// </summary>
	  /// @deprecated use ProjectionPoint.create 
	  [Obsolete("use ProjectionPoint.create")]
	  public ProjectionPointImpl() : this(0.0, 0.0)
	  {
	  }

	  /// <summary>
	  /// Constructor that copies ProjectionPoint values into this.
	  /// </summary>
	  /// @deprecated use ProjectionPoint.create 
	  [Obsolete("use ProjectionPoint.create")]
	  public ProjectionPointImpl(ProjectionPoint pt)
	  {
		this.x = pt.X;
		this.y = pt.Y;
	  }

	  /// <summary>
	  /// Constructor, initialized to x, y
	  /// </summary>
	  /// <param name="x"> x coordinate </param>
	  /// <param name="y"> y coordinate </param>
	  /// @deprecated use ProjectionPoint.create 
	  [Obsolete("use ProjectionPoint.create")]
	  public ProjectionPointImpl(double x, double y)
	  {
		this.x = x;
		this.y = y;
	  }

	  public virtual double X
	  {
		  get
		  {
			return x;
		  }
		  set
		  {
			this.x = value;
		  }
	  }

	  public virtual double Y
	  {
		  get
		  {
			return y;
		  }
		  set
		  {
			this.y = value;
		  }
	  }



	  public virtual bool nearlyEquals(ProjectionPoint other, double maxRelDiff)
	  {
		return Misc.nearlyEquals(x, other.X, maxRelDiff) && Misc.nearlyEquals(y, other.Y, maxRelDiff);
	  }

	  // Exact comparison is needed in order to be consistent with hashCode().
	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}
		ProjectionPointImpl that = (ProjectionPointImpl) o;
		if (that.x.CompareTo(x) != 0)
		{
		  return false;
		}
		return that.y.CompareTo(y) == 0;
	  }

	  public override int GetHashCode()
	  {
		int result;
		long temp;
		temp = System.BitConverter.DoubleToInt64Bits(x);
		result = (int)(temp ^ (temp >>> 32));
		temp = System.BitConverter.DoubleToInt64Bits(y);
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  /// <summary>
	  /// nicely format this point
	  /// </summary>
	  /// <returns> nicely formatted point </returns>
	  public override string ToString()
	  {
		return Format.d(X, 4) + " " + Format.d(Y, 4);
	  }

	  public virtual void toString(Formatter f)
	  {
		f.format("x=%f y=%f ", X, Y);
	  }

	  // convenience setting routines

	  /// <summary>
	  /// set x,y location from pt
	  /// </summary>
	  /// <param name="pt"> point to use for values </param>
	  public virtual void setLocation(ProjectionPoint pt)
	  {
		setLocation(pt.X, pt.Y);
	  }

	  public virtual void setLocation(double x, double y)
	  {
		this.x = x;
		this.y = y;
	  }

	  /// <summary>
	  /// See if either coordinate is +/- infinite. This happens sometimes
	  /// in projective geometry.
	  /// </summary>
	  /// <returns> true if either coordinate is +/- infinite. </returns>
	  public virtual bool Infinite
	  {
		  get
		  {
			return (x == double.PositiveInfinity) || (x == double.NegativeInfinity) || (y == double.PositiveInfinity) || (y == double.NegativeInfinity);
		  }
	  }

	  /// @deprecated use LatLonPoints.isInfinite 
	  [Obsolete("use LatLonPoints.isInfinite")]
	  public static bool isInfinite(ProjectionPoint pt)
	  {
		return (pt.X == double.PositiveInfinity) || (pt.X == double.NegativeInfinity) || (pt.Y == double.PositiveInfinity) || (pt.Y == double.NegativeInfinity);
	  }
	}

}
