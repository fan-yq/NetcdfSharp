// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Builds one Coordinate of one Variable,
	/// by keeping the Set of Values found in the records.
	/// 
	/// @author caron
	/// @since 11/27/13
	/// </summary>
	public abstract class CoordinateBuilderImpl<T> : CoordinateBuilder<T>
	{
		public abstract Coordinate makeCoordinate(IList<object> values);
		public abstract object extract(T gr);
	  private ISet<object> valSet = new HashSet<object>(100);
	  private IDictionary<object, int> valMap; // map of values to index in Coordinate
	  protected internal Coordinate coord;

	  public virtual void addRecord(T gr)
	  {
		object val = extract(gr);
		valSet.Add(val);
	  }

	  public virtual void addAll(Coordinate coord)
	  {
		valSet.UnionWith(coord.Values);
	  }

	  public virtual void add(object val)
	  {
		valSet.Add(val);
	  }

	  public virtual void addAll(IList<object> coords)
	  {
		valSet.UnionWith(coords);
	  }

	  public virtual Coordinate finish()
	  {
		IList<object> valList = new List<object>(valSet.Count);
		((List<object>)valList).AddRange(valSet);
		coord = makeCoordinate(valList);
		valSet = null;

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: List<?> values = coord.getValues();
		IList<object> values = coord.Values;
		if (values != null)
		{
		  valMap = new Dictionary<object, int>(coord.Size * 2);
		  for (int i = 0; i < values.Count; i++)
		  {
			valMap[values[i]] = i;
		  }
		}
		return coord;
	  }

	  // Used by CoordinateND.makeSparseArray; not used by CoordinateTime2D
	  public virtual int getIndex(T gr)
	  {
		int? result = valMap[extract(gr)];
		return (result == null) ? 0 : result.Value;
	  }

	  public virtual Coordinate Coordinate
	  {
		  get
		  {
			return coord;
		  }
	  }

	}

}
