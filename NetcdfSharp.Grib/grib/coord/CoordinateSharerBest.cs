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
	/// Create shared coordinate variables across the variables in the same group.
	/// Use Coordinate.equals(), hashCode() to find unique coordinate variables.
	/// 
	/// Used by GribPartitionBuilder.makeBest(), for all coordinates not CoordinateTime2D and not CoordinateRuntime.
	/// Best coordinates are translated into equivilent 2D coordinate, thus we dont need separate CoordinateND.
	/// 
	/// @author caron
	/// @since 12/10/13
	/// </summary>
	public class CoordinateSharerBest
	{
	  private IDictionary<Coordinate, Coordinate> runtimeBuilders = new Dictionary<Coordinate, Coordinate>();
	  private IDictionary<Coordinate, Coordinate> timeBuilders = new Dictionary<Coordinate, Coordinate>();
	  private IDictionary<Coordinate, Coordinate> timeIntvBuilders = new Dictionary<Coordinate, Coordinate>();
	  private IDictionary<Coordinate, Coordinate> vertBuilders = new Dictionary<Coordinate, Coordinate>();
	  private IDictionary<Coordinate, Coordinate> ensBuilders = new Dictionary<Coordinate, Coordinate>();
	  private IDictionary<Coordinate, Coordinate> time2DBuilders = new Dictionary<Coordinate, Coordinate>();
	  private IDictionary<Coordinate, Coordinate> swap = new Dictionary<Coordinate, Coordinate>();

	  private IDictionary<Coordinate, int> indexMap;

	  public virtual void addCoordinates(IList<Coordinate> coords)
	  {
		foreach (Coordinate coord in coords)
		{
		  addCoordinate(coord);
		}
	  }

  public virtual void addCoordinate(Coordinate coord)
  {
	Coordinate already;
	switch (coord.getType().innerEnumValue)
	{
	  case Coordinate.Type.InnerEnum.runtime:
		if (!runtimeBuilders.TryGetValue(coord, out already))
		{
		  runtimeBuilders[coord] = coord;
		}
		else
		{
		  swap[coord] = already; // keep track of substitutes
		}
		break;

	  case Coordinate.Type.InnerEnum.time:
		if (!timeBuilders.TryGetValue(coord, out already))
		{
		  timeBuilders[coord] = coord;
		}
		else
		{
		  swap[coord] = already;
		}
		break;

	  case Coordinate.Type.InnerEnum.timeIntv:
		if (!timeIntvBuilders.TryGetValue(coord, out already))
		{
		  timeIntvBuilders[coord] = coord;
		}
		else
		{
		  swap[coord] = already;
		}
		break;

	  case Coordinate.Type.InnerEnum.time2D:
		if (!time2DBuilders.TryGetValue(coord, out already))
		{
		  time2DBuilders[coord] = coord;
		}
		else
		{
		  swap[coord] = already;
		}
		break;

	  case Coordinate.Type.InnerEnum.vert:
		if (!vertBuilders.TryGetValue(coord, out already))
		{
		  vertBuilders[coord] = coord;
		}
		else
		{
		  swap[coord] = already;
		}
		break;

	  case Coordinate.Type.InnerEnum.ens:
		if (!ensBuilders.TryGetValue(coord, out already))
		{
		  ensBuilders[coord] = coord;
		}
		else
		{
		  swap[coord] = already;
		}
		break;
	}
  }

	  public virtual IList<Coordinate> finish()
	  {
		// results
		IList<Coordinate> unionCoords = new List<Coordinate>(20);
		((List<Coordinate>)unionCoords).AddRange(runtimeBuilders.Keys);
		((List<Coordinate>)unionCoords).AddRange(time2DBuilders.Keys);
		((List<Coordinate>)unionCoords).AddRange(timeBuilders.Keys);
		((List<Coordinate>)unionCoords).AddRange(timeIntvBuilders.Keys);
		((List<Coordinate>)unionCoords).AddRange(vertBuilders.Keys);
		((List<Coordinate>)unionCoords).AddRange(ensBuilders.Keys);

		indexMap = new Dictionary<Coordinate, int>();
		for (int i = 0; i < unionCoords.Count; i++)
		{
		  indexMap[unionCoords[i]] = i;
		}

		return unionCoords;
	  }

	  // redo the variables against the shared coordinates
	  public virtual IList<int> reindex(IList<Coordinate> coords)
	  {
		IList<int> result = new List<int>();
		foreach (Coordinate coord in coords)
		{
		  Coordinate sub;
		  Coordinate use = swap.TryGetValue(coord, out sub) ? sub : coord;
		  int idx;
		  if (!indexMap.TryGetValue(use, out idx)) // index into unionCoords
		  {
			throw new System.InvalidOperationException();
		  }
		  result.Add(idx);
		}
		return result;
	  }
	}

}
