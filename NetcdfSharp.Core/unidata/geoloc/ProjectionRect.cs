// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.geoloc
{
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Bounding box for ProjectionPoint's.
	/// Note that getX() getY() really means getMinX(), getMinY(), rather than
	/// "upper left point" of the rectangle.
	/// 
	/// LOOK will not implement Serializable in ver6
	/// LOOK may be immutable AutoValue in ver6
	/// </summary>
	[Serializable]
	public class ProjectionRect
	{
	  // TODO make final in ver6
	  private double x, y, width, height;

	  /// <summary>
	  /// default constructor, initialized to center (0,0) and width (10000, 10000)
	  /// </summary>
	  public ProjectionRect() : this(-5000, -5000, 5000, 5000)
	  {
	  }

	  /// <summary>
	  /// Construct a ProjectionRect from any two opposite corner points.
	  /// </summary>
	  /// <param name="corner1"> a corner. </param>
	  /// <param name="corner2"> the opposite corner. </param>
	  public ProjectionRect(ProjectionPoint corner1, ProjectionPoint corner2) : this(corner1.X, corner1.Y, corner2.X, corner2.Y)
	  {
	  }

	  /// <summary>
	  /// Construct a ProjectionRect from any two opposite corner points.
	  /// </summary>
	  /// <param name="minimum"> lower left corner, ie the minimum x and y </param>
	  /// <param name="width"> x width. </param>
	  /// <param name="height"> y height </param>
	  public ProjectionRect(ProjectionPoint minimum, double width, double height)
	  {
		setRect(minimum.X, minimum.Y, width, height);
	  }

	  /// <summary>
	  /// Copy Constructor
	  /// </summary>
	  /// <param name="r"> rectangle to copy </param>
	  public ProjectionRect(ProjectionRect r) : this(r.MinX, r.MinY, r.MaxX, r.MaxY)
	  {
	  }

	  /// <summary>
	  /// construct a MapArea from any two opposite corner points
	  /// </summary>
	  /// <param name="x1"> x coord of any corner of the bounding box </param>
	  /// <param name="y1"> y coord of the same corner as x1 </param>
	  /// <param name="x2"> x coord of opposite corner from x1,y1 </param>
	  /// <param name="y2"> y coord of same corner as x2 </param>
	  public ProjectionRect(double x1, double y1, double x2, double y2)
	  {
		double wx0 = 0.5 * (x1 + x2);
		double wy0 = 0.5 * (y1 + y2);
		double width = Math.Abs(x1 - x2);
		double height = Math.Abs(y1 - y2);
		setRect(wx0 - width / 2, wy0 - height / 2, width, height);
	  }

	  public virtual double X
	  {
		  get
		  {
			return x;
		  }
		  set
		  {
			setRect(value, Y, Width, Height);
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
			setRect(X, value, Width, Height);
		  }
	  }

	  public virtual double Width
	  {
		  get
		  {
			return width;
		  }
		  set
		  {
			setRect(X, Y, value, Height);
		  }
	  }

	  public virtual double Height
	  {
		  get
		  {
			return height;
		  }
		  set
		  {
			setRect(X, Y, Width, value);
		  }
	  }

	  ////////////////////////////////////////////////////
	  // taken from NetcdfSharp.java.awt.geom.Rectangle2D, removed because awt missing on android

	  /// <summary>
	  /// Returns the smallest X coordinate of the framing
	  /// rectangle of the <code>Shape</code> in <code>double</code>
	  /// precision.
	  /// </summary>
	  /// <returns> the smallest X coordinate of the framing
	  ///         rectangle of the <code>Shape</code>.
	  /// @since 1.2 </returns>
	  public virtual double MinX
	  {
		  get
		  {
			return X;
		  }
	  }

	  /// <summary>
	  /// Returns the smallest Y coordinate of the framing
	  /// rectangle of the <code>Shape</code> in <code>double</code>
	  /// precision.
	  /// </summary>
	  /// <returns> the smallest Y coordinate of the framing
	  ///         rectangle of the <code>Shape</code>.
	  /// @since 1.2 </returns>
	  public virtual double MinY
	  {
		  get
		  {
			return Y;
		  }
	  }

	  /// <summary>
	  /// Returns the largest X coordinate of the framing
	  /// rectangle of the <code>Shape</code> in <code>double</code>
	  /// precision.
	  /// </summary>
	  /// <returns> the largest X coordinate of the framing
	  ///         rectangle of the <code>Shape</code>.
	  /// @since 1.2 </returns>
	  public virtual double MaxX
	  {
		  get
		  {
			return X + Width;
		  }
	  }

	  /// <summary>
	  /// Returns the largest Y coordinate of the framing
	  /// rectangle of the <code>Shape</code> in <code>double</code>
	  /// precision.
	  /// </summary>
	  /// <returns> the largest Y coordinate of the framing
	  ///         rectangle of the <code>Shape</code>.
	  /// @since 1.2 </returns>
	  public virtual double MaxY
	  {
		  get
		  {
			return Y + Height;
		  }
	  }

	  /// <summary>
	  /// Returns the X coordinate of the center of the framing
	  /// rectangle of the <code>Shape</code> in <code>double</code>
	  /// precision.
	  /// </summary>
	  /// <returns> the X coordinate of the center of the framing rectangle
	  ///         of the <code>Shape</code>.
	  /// @since 1.2 </returns>
	  public virtual double CenterX
	  {
		  get
		  {
			return X + Width / 2.0;
		  }
	  }

	  /// <summary>
	  /// Returns the Y coordinate of the center of the framing
	  /// rectangle of the <code>Shape</code> in <code>double</code>
	  /// precision.
	  /// </summary>
	  /// <returns> the Y coordinate of the center of the framing rectangle
	  ///         of the <code>Shape</code>.
	  /// @since 1.2 </returns>
	  public virtual double CenterY
	  {
		  get
		  {
			return Y + Height / 2.0;
		  }
	  }

	  /// <summary>
	  /// Adds a <code>Rectangle2D</code> object to this
	  /// <code>Rectangle2D</code>. The resulting <code>Rectangle2D</code>
	  /// is the union of the two <code>Rectangle2D</code> objects.
	  /// </summary>
	  /// <param name="r"> the <code>Rectangle2D</code> to add to this
	  ///        <code>Rectangle2D</code>.
	  /// @since 1.2 </param>
	  /// @deprecated use builder 
	  [Obsolete("use builder")]
	  public virtual void add(ProjectionRect r)
	  {
		double x1 = Math.Min(MinX, r.MinX);
		double x2 = Math.Max(MaxX, r.MaxX);
		double y1 = Math.Min(MinY, r.MinY);
		double y2 = Math.Max(MaxY, r.MaxY);
		setRect(x1, y1, x2 - x1, y2 - y1);
	  }

	  /// <summary>
	  /// Adds a point, specified by the double precision arguments
	  /// <code>newx</code> and <code>newy</code>, to this
	  /// <code>Rectangle2D</code>. The resulting <code>Rectangle2D</code>
	  /// is the smallest <code>Rectangle2D</code> that
	  /// contains both the original <code>Rectangle2D</code> and the
	  /// specified point.
	  /// <para>
	  /// After adding a point, a call to <code>contains</code> with the
	  /// added point as an argument does not necessarily return
	  /// <code>true</code>. The <code>contains</code> method does not
	  /// return <code>true</code> for points on the right or bottom
	  /// edges of a rectangle. Therefore, if the added point falls on
	  /// the left or bottom edge of the enlarged rectangle,
	  /// <code>contains</code> returns <code>false</code> for that point.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="newx"> the X coordinate of the new point </param>
	  /// <param name="newy"> the Y coordinate of the new point
	  /// @since 1.2 </param>
	  /// @deprecated use builder 
	  [Obsolete("use builder")]
	  public virtual void add(double newx, double newy)
	  {
		double x1 = Math.Min(MinX, newx);
		double x2 = Math.Max(MaxX, newx);
		double y1 = Math.Min(MinY, newy);
		double y2 = Math.Max(MaxY, newy);
		setRect(x1, y1, x2 - x1, y2 - y1);
	  }

	  /// <summary>
	  /// Adds the <code>Point2D</code> object <code>pt</code> to this
	  /// <code>Rectangle2D</code>.
	  /// The resulting <code>Rectangle2D</code> is the smallest
	  /// <code>Rectangle2D</code> that contains both the original
	  /// <code>Rectangle2D</code> and the specified <code>Point2D</code>.
	  /// <para>
	  /// After adding a point, a call to <code>contains</code> with the
	  /// added point as an argument does not necessarily return
	  /// <code>true</code>. The <code>contains</code>
	  /// method does not return <code>true</code> for points on the right
	  /// or bottom edges of a rectangle. Therefore, if the added point falls
	  /// on the left or bottom edge of the enlarged rectangle,
	  /// <code>contains</code> returns <code>false</code> for that point.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="pt"> the new <code>Point2D</code> to add to this
	  ///        <code>Rectangle2D</code>.
	  /// @since 1.2 </param>
	  /// @deprecated use builder 
	  [Obsolete("use builder")]
	  public virtual void add(ProjectionPoint pt)
	  {
		add(pt.X, pt.Y);
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return (width <= 0.0) || (height <= 0.0);
		  }
	  }

	  public virtual bool intersects(ProjectionRect r)
	  {
		return intersects(r.X, r.Y, r.Width, r.Height);
	  }

	  public virtual bool intersects(double x, double y, double w, double h)
	  {
		if (Empty || w <= 0 || h <= 0)
		{
		  return false;
		}
		double x0 = X;
		double y0 = Y;
		return (x + w > x0 && y + h > y0 && x < x0 + Width && y < y0 + Height);
	  }

	  /// <summary>
	  /// Intersects the pair of specified source <code>Rectangle2D</code>
	  /// objects and puts the result into the specified destination
	  /// <code>Rectangle2D</code> object. One of the source rectangles
	  /// can also be the destination to avoid creating a third Rectangle2D
	  /// object, but in this case the original points of this source
	  /// rectangle will be overwritten by this method.
	  /// </summary>
	  /// <param name="src1"> the first of a pair of <code>Rectangle2D</code>
	  ///        objects to be intersected with each other </param>
	  /// <param name="src2"> the second of a pair of <code>Rectangle2D</code>
	  ///        objects to be intersected with each other </param>
	  /// <param name="dest"> the <code>Rectangle2D</code> that holds the
	  ///        results of the intersection of <code>src1</code> and
	  ///        <code>src2</code>
	  /// @since 1.2 </param>
	  public static void intersect(ProjectionRect src1, ProjectionRect src2, ProjectionRect dest)
	  {
		double x1 = Math.Max(src1.MinX, src2.MinX);
		double y1 = Math.Max(src1.MinY, src2.MinY);
		double x2 = Math.Min(src1.MaxX, src2.MaxX);
		double y2 = Math.Min(src1.MaxY, src2.MaxY);
		dest.setRect(x1, y1, x2 - x1, y2 - y1);
	  }

	  /// <summary>
	  /// Returns {@code true} if this bounding box contains {@code point}.
	  /// </summary>
	  /// <param name="point"> a point in projection coordinates. </param>
	  /// <returns> {@code true} if this bounding box contains {@code point}. </returns>
	  public virtual bool contains(ProjectionPoint point)
	  {
		return DoubleMath.fuzzyCompare(point.X, MinX, 1e-6) >= 0 && DoubleMath.fuzzyCompare(point.X, MaxX, 1e-6) <= 0 && DoubleMath.fuzzyCompare(point.Y, MinY, 1e-6) >= 0 && DoubleMath.fuzzyCompare(point.Y, MaxY, 1e-6) <= 0;
	  }

	  /// 
	  /// <summary>
	  /// Returns {@code true} if this bounding box contains {@code rect}.
	  /// </summary>
	  /// <param name="rect"> a bounding box in projection coordinates </param>
	  /// <returns> {@code true} if this bounding box contains {@code rect}. </returns>
	  public virtual bool contains(ProjectionRect rect)
	  {
		bool contained;
		// The old ProjectionRect class was based off of NetcdfSharp.java.awt.Rectangle.
		// If the rectangles were the same, .contains(rect) returned true.
		// This check makes sure the old behavior is preserved.
		if (this.Equals(rect))
		{
		  contained = true;
		}
		else
		{
		  // Just check to see if corners of rect contained within this
		  contained = (this.contains(rect.MinPoint) && this.contains(rect.MaxPoint));
		}
		return contained;
	  }

	  /////////////////////////////////////////////////////

	  /// <summary>
	  /// Get the Lower Right Point
	  /// </summary>
	  /// <returns> the Lower Right Point </returns>
	  public virtual ProjectionPoint LowerRightPoint
	  {
		  get
		  {
			return ProjectionPoint.create(MaxPoint.X, MinPoint.Y);
		  }
	  }

	  /// <summary>
	  /// Get the Upper Left Point (same as getMaxPoint)
	  /// </summary>
	  /// <returns> the Upper Left Point </returns>
	  public virtual ProjectionPoint UpperRightPoint
	  {
		  get
		  {
			return MaxPoint;
		  }
	  }

	  /// <summary>
	  /// Get the Lower Right Point (same as getMinPoint)
	  /// </summary>
	  /// <returns> the Lower Right Point </returns>
	  public virtual ProjectionPoint LowerLeftPoint
	  {
		  get
		  {
			return MinPoint;
		  }
	  }

	  /// <summary>
	  /// Get the Upper Left Point
	  /// </summary>
	  /// <returns> the Upper Left Point </returns>
	  public virtual ProjectionPoint UpperLeftPoint
	  {
		  get
		  {
			return ProjectionPoint.create(MinPoint.X, MaxPoint.Y);
		  }
	  }

	  /// <summary>
	  /// Get the minimum corner of the bounding box.
	  /// </summary>
	  /// <returns> minimum corner of the bounding box </returns>
	  public virtual ProjectionPoint MinPoint
	  {
		  get
		  {
			return ProjectionPoint.create(X, Y);
		  }
	  }

	  /// <summary>
	  /// Get the maximum corner of the bounding box.
	  /// </summary>
	  /// <returns> maximum corner of the bounding box </returns>
	  public virtual ProjectionPoint MaxPoint
	  {
		  get
		  {
			return ProjectionPoint.create(X + Width, Y + Height);
		  }
	  }

	  /// <summary>
	  /// Get a String representation of this object.
	  /// </summary>
	  /// <returns> a String representation of this object. </returns>
	  public override string ToString()
	  {
		return string.Format("min: {0:F3} {1:F3} size: {2:F3} {3:F3}", X, Y, Width, Height);
	  }

	  public virtual string toString2(int ndec)
	  {
		string f = " %." + ndec + "f";
		return string.Format("min:" + f + f + " max:" + f + f, X, Y, MaxX, MaxY);
	  }





	  /// @deprecated use builder 
	  [Obsolete("use builder")]
	  public virtual void setRect(ProjectionRect r)
	  {
		setRect(r.X, r.Y, r.Width, r.Height);
	  }

	  /// @deprecated use builder 
	  [Obsolete("use builder")]
	  public virtual void setRect(double x, double y, double w, double h)
	  {
		this.x = x;
		this.y = y;
		this.width = w;
		this.height = h;
	  }

	  // serialization

	  /// <summary>
	  /// Read the object from the input stream of the serialized object
	  /// </summary>
	  /// <param name="s"> stream to read </param>
	  /// <exception cref="IOException"> Problem reading from stream </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void readObject(ObjectInputStream s) throws IOException
	  private void readObject(ObjectInputStream s)
	  {
		double x = s.readDouble();
		double y = s.readDouble();
		double w = s.readDouble();
		double h = s.readDouble();
		setRect(x, y, w, h);
	  }

	  /// <summary>
	  /// Wrtie the object to the output stream
	  /// </summary>
	  /// <param name="s"> stream to write </param>
	  /// <exception cref="IOException"> Problem writing to stream </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void writeObject(ObjectOutputStream s) throws IOException
	  private void writeObject(ObjectOutputStream s)
	  {
		s.writeDouble(X);
		s.writeDouble(Y);
		s.writeDouble(Width);
		s.writeDouble(Height);
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
		ProjectionRect that = (ProjectionRect) o;

		if (that.height.CompareTo(height) != 0)
		{
		  return false;
		}
		if (that.width.CompareTo(width) != 0)
		{
		  return false;
		}
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
		temp = System.BitConverter.DoubleToInt64Bits(width);
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		temp = System.BitConverter.DoubleToInt64Bits(height);
		result = 31 * result + (int)(temp ^ (temp >>> 32));
		return result;
	  }

	  /// <summary>
	  /// Returns the result of <seealso cref="nearlyEquals(ProjectionRect, double)"/>, with <seealso cref="Misc.defaultMaxRelativeDiffFloat"/>.
	  /// </summary>
	  public virtual bool nearlyEquals(ProjectionRect other)
	  {
		return nearlyEquals(other, Misc.defaultMaxRelativeDiffFloat);
	  }

	  /// <summary>
	  /// Returns {@code true} if this rectangle is nearly equal to {@code other}. The "near equality" of corners is
	  /// determined using <seealso cref="ProjectionPoint.nearlyEquals(ProjectionPoint, double)"/>, with the specified maxRelDiff.
	  /// </summary>
	  /// <param name="other"> the other rectangle to check. </param>
	  /// <param name="maxRelDiff"> the maximum <seealso cref="Misc.relativeDifference relative difference"/> that two corners may have. </param>
	  /// <returns> {@code true} if this rectangle is nearly equal to {@code other}. </returns>
	  public virtual bool nearlyEquals(ProjectionRect other, double maxRelDiff)
	  {
		return this.LowerLeftPoint.nearlyEquals(other.LowerLeftPoint, maxRelDiff) && this.UpperRightPoint.nearlyEquals(other.UpperRightPoint, maxRelDiff);
	  }
	}

}
