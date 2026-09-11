// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using Section = NetcdfSharp.ucar.ma2.Section;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// N-dimensional coordinates and a SparseArray that tracks if data is available.
	/// 
	/// @author caron
	/// @since 11/27/13
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class CoordinateND<T>
	public class CoordinateND<T>
	{

	  private readonly IList<Coordinate> coordinates; // result is orthogonal coordinates
	  private readonly SparseArray<T> sa; // indexes refer to coordinates

	  internal CoordinateND(IList<Coordinate> coordinates, SparseArray<T> sa)
	  {
		Debug.Assert(coordinates.Count == sa.Rank);
		this.coordinates = coordinates.AsReadOnly();
		this.sa = sa;
	  }

	  public virtual IList<Coordinate> Coordinates
	  {
		  get
		  {
			return coordinates;
		  }
	  }

	  private int NCoordinates
	  {
		  get
		  {
			return coordinates.Count;
		  }
	  }

	  public virtual SparseArray<T> SparseArray
	  {
		  get
		  {
			return sa;
		  }
	  }

	  public virtual void showInfo(Formatter info, GribRecordStats all)
	  {
		foreach (Coordinate coord in coordinates)
		{
		  coord.showInfo(info, new Indent(2));
		}

		sa.showInfo(info, all);
	  }

	  public override string ToString()
	  {
		using (Formatter f = new Formatter())
		{
		  f.format("CoordinateND[");
		  foreach (Coordinate c in coordinates)
		  {
			f.format("%s,", c.Name);
		  }
		  f.format("]");
		  return f.ToString();
		}
	  }

	  ////////////////////

	  public class Builder<T1>
	  {
		internal IList<CoordinateBuilder<T1>> builders;
		internal IList<Coordinate> coordb = new List<Coordinate>();

		public Builder()
		{
		  builders = new List<CoordinateBuilder<T1>>();
		}

		public virtual void addBuilder(CoordinateBuilder<T1> builder)
		{
		  builders.Add(builder);
		}

		public virtual void addRecord(T1 gr)
		{
		  foreach (CoordinateBuilder<T1> builder in builders)
		  {
			builder.addRecord(gr);
		  }
		}

		public virtual CoordinateND<T1> finish(IList<T1> records, Formatter info)
		{
		  foreach (CoordinateBuilder<T1> builder in builders)
		  {
			Coordinate coord = builder.finish();
			if (coord.getType() == Coordinate.Type.time2D)
			{
			  coordb.Add(((CoordinateTime2D) coord).RuntimeCoordinate);
			}
			coordb.Add(coord);
		  }

		  SparseArray<T1> sa = buildSparseArray(records, info);
		  return new CoordinateND<T1>(coordb, sa);
		}

		internal virtual SparseArray<T1> buildSparseArray(IList<T1> records, Formatter info)
		{
		  int[] sizeArray = new int[coordb.Count];
		  for (int i = 0; i < coordb.Count; i++)
		  {
			Coordinate coord = coordb[i];
			if (coord.getType() == Coordinate.Type.time2D)
			{
			  sizeArray[i] = ((CoordinateTime2D) coord).Ntimes;
			}
			else
			{
			  sizeArray[i] = coord.Size;
			}
		  }
		  SparseArray<T1>.Builder<T1> saBuilder = new SparseArray<T1>.Builder<T1>(sizeArray);

		  int[] index = new int[coordb.Count];
		  foreach (T1 gr in records)
		  {
			int count = 0;
			foreach (CoordinateBuilder<T1> builder in builders)
			{
			  if (builder is CoordinateBuilder<T1>.TwoD<T1>)
			  {
				CoordinateBuilder<T1>.TwoD<T1> builder2D = (CoordinateBuilder<T1>.TwoD<T1>) builder;
				int[] coordsIdx = builder2D.getCoordIndices(gr);
				index[count++] = coordsIdx[0];
				index[count++] = coordsIdx[1];

			  }
			  else
			  {
				index[count++] = builder.getIndex(gr);
			  }
			}

			saBuilder.add(gr, info, index);
		  }

		  return saBuilder.finish();
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Reindex the sparse array, based on the new Coordinates.
		/// Do this by running all the Records through the Coordinates, assigning each to a new spot in the new sparse array.
		/// </summary>
		/// <param name="newCoords"> must have same list of Coordinates as prev, with possibly additional values. </param>
		internal virtual CoordinateND<T1> reindex(IList<Coordinate> newCoords, CoordinateND<T1> prev)
		{
		  Debug.Assert(newCoords.Count == prev.NCoordinates);

		  bool has2Dcoord = false;
		  foreach (Coordinate coord in newCoords)
		  {
			if (coord.getType() == Coordinate.Type.time2D)
			{
			  has2Dcoord = true;
			}
		  }

		  return (has2Dcoord) ? reindex2D(newCoords, prev) : reindexOrth(newCoords, prev);
		}

		internal virtual CoordinateND<T1> reindexOrth(IList<Coordinate> newCoords, CoordinateND<T1> prev)
		{
		  SparseArray<T1> prevSA = prev.SparseArray;
		  IList<Coordinate> prevCoords = prev.Coordinates;

		  // make a working sparse array with new shape
		  int[] sizeArray = new int[newCoords.Count];
		  for (int i = 0; i < newCoords.Count; i++)
		  {
			Coordinate coord = newCoords[i];
			sizeArray[i] = (coord is CoordinateTime2D) ? ((CoordinateTime2D) coord).Ntimes : coord.Size;
		  }
		  SparseArray<T1>.Builder<T1> workingSAbuilder = new SparseArray<T1>.Builder<T1>(sizeArray);

		  // for each coordinate, calculate the map of oldIndex -> newIndex
		  IList<IndexMap> indexMaps = new List<IndexMap>();
		  int count = 0;
		  foreach (Coordinate coord in newCoords)
		  {
			indexMaps.Add(new IndexMap(coord, prevCoords[count++]));
		  }

		  int[] currIndex = new int[newCoords.Count];
		  int[] prevIndex = new int[newCoords.Count];
		  int[] track = new int[SparseArray<T1>.calcTotalSize(sizeArray)];

		  // iterate through the contents of the prev track array
		  Section section = new Section(prevSA.Shape);
		  Section.Iterator iter = section.getIterator(prevSA.Shape);
		  while (iter.hasNext())
		  {
			int oldTrackIdx = iter.next(prevIndex); // gets both the oldTrackIdx (1D) and prevIndex (nD)
			int oldTrackValue = prevSA.getTrack(oldTrackIdx);
			if (oldTrackValue == 0)
			{
			  continue; // skip missing values
			}

			// calculate position in the current track array, and store the value there
			int coordIdx = 0;
			foreach (IndexMap indexMap in indexMaps)
			{
			  currIndex[coordIdx] = indexMap.map(prevIndex[coordIdx]);
			  coordIdx++;
			}
			int trackIdx = workingSAbuilder.calcIndex(currIndex);
			track[trackIdx] = oldTrackValue;
		  }

		  // now that we have the track, make the real SA
		  SparseArray<T1> newSA = new SparseArray<T1>(sizeArray, track, prevSA.Content, prevSA.Ndups); // content
																											  // (list of
																											  // records) is
																											  // the same
		  return new CoordinateND<T1>(newCoords, newSA); // reindexed result
		}

		// every coord in prev must be in curr
		private class IndexMap
		{
		  internal bool identity;
		  internal int[] indexMap;

		  internal IndexMap(Coordinate curr, Coordinate prev)
		  {
			identity = curr.Equals(prev);
			if (identity)
			{
			  return;
			}

			Debug.Assert(curr.getType() == prev.getType(), curr.getType() + " != " + prev.getType());

			int count = 0;
			IDictionary<object, int> currValMap = new Dictionary<object, int>();
			if (curr.Values == null)
			{
			  throw new System.InvalidOperationException();
			}
			foreach (object val in curr.Values)
			{
			  currValMap[val] = count++;
			}

			count = 0;
			indexMap = new int[prev.Size];
			foreach (object val in prev.Values)
			{
			  indexMap[count++] = currValMap[val]; // where does this value fit in the curr coordinates?
			}
		  }

		  public virtual int map(int oldIndex)
		  {
			if (identity)
			{
			  return oldIndex;
			}
			return indexMap[oldIndex];
		  }
		}

		//////////////////////////////////////////////

		internal virtual CoordinateND<T1> reindex2D(IList<Coordinate> newCoords, CoordinateND<T1> prev)
		{
		  SparseArray<T1> prevSA = prev.SparseArray;
		  IList<Coordinate> prevCoords = prev.Coordinates;
		  CoordinateTime2D prev2Dcoord = null;

		  // make a working sparse array with new shape
		  int ncoords = newCoords.Count;
		  int[] sizeArray = new int[ncoords];
		  for (int i = 0; i < ncoords; i++)
		  {
			Coordinate coord = newCoords[i];
			sizeArray[i] = (coord is CoordinateTime2D) ? ((CoordinateTime2D) coord).Ntimes : coord.Size;
		  }
		  SparseArray<T1>.Builder<T1> workingSAbuilder = new SparseArray<T1>.Builder<T1>(sizeArray);

		  // for each coordinate, calculate the map of oldIndex -> newIndex
		  IndexMap[] indexMaps = new IndexMap[ncoords];
		  Time2DIndexMap timeIndexMap = null;
		  for (int i = 0; i < ncoords; i++)
		  {
			Coordinate coord = newCoords[i];
			if (coord.getType() == Coordinate.Type.time2D)
			{
			  prev2Dcoord = (CoordinateTime2D) prevCoords[i];
			  timeIndexMap = new Time2DIndexMap((CoordinateTime2D) coord, prev2Dcoord);
			}
			else
			{
			  indexMaps[i] = new IndexMap(coord, prevCoords[i]);
			}
		  }

		  int[] currIndex = new int[ncoords];
		  int[] prevIndex = new int[ncoords];
		  int[] track = new int[SparseArray<T1>.calcTotalSize(sizeArray)];

		  // iterate through the contents of the prev track array
		  Section section = new Section(prevSA.Shape);
		  Section.Iterator iter = section.getIterator(prevSA.Shape);
		  while (iter.hasNext())
		  {
			int oldTrackIdx = iter.next(prevIndex); // gets both the oldTrackIdx (1D) and prevIndex (nD)
			int oldTrackValue = prevSA.getTrack(oldTrackIdx);
			if (oldTrackValue == 0)
			{
			  continue; // skip missing values
			}

			for (int i = 0; i < ncoords; i++)
			{
			  Coordinate coord = newCoords[i];
			  if (coord.getType() == Coordinate.Type.time2D)
			  {
				CoordinateTime2D.Time2D prevValue = prev2Dcoord.getOrgValue(prevIndex[0], prevIndex[1]);
				currIndex[i] = timeIndexMap.map(prevValue);
			  }
			  else
			  {
				currIndex[i] = indexMaps[i].map(prevIndex[i]);
			  }
			}
			int trackIdx = workingSAbuilder.calcIndex(currIndex);
			track[trackIdx] = oldTrackValue;
		  }

		  // now that we have the track, make the real SA
		  SparseArray<T1> newSA = new SparseArray<T1>(sizeArray, track, prevSA.Content, prevSA.Ndups); // content
																											  // (list of
																											  // records) is
																											  // the same
		  return new CoordinateND<T1>(newCoords, newSA); // reindexed result
		}

		private class Time2DIndexMap
		{
		  internal IDictionary<object, int> currValMap;

		  // every coord in prev must be in curr
		  internal Time2DIndexMap(CoordinateTime2D curr, CoordinateTime2D prev)
		  {
			Debug.Assert(curr.getType() == prev.getType(), curr.getType() + " != " + prev.getType());
			currValMap = new Dictionary<object, int>(2 * curr.Values.Count);

			int[] index2D = new int[2];
			foreach (object val in prev.Values)
			{
			  if (!curr.getIndex((CoordinateTime2D.Time2D) val, index2D))
			  {
				throw new System.InvalidOperationException();
			  }
			  currValMap[val] = index2D[1]; // save the time index
			}
		  }

		  public virtual int map(CoordinateTime2D.Time2D prevCoord)
		  {
			int? val = currValMap[prevCoord];
			if (val == null)
			{
			  throw new System.InvalidOperationException("reindex does not have coordinate Time2D " + prevCoord);
			}
			return val.Value;
		  }

		}

	  }
	}


}
