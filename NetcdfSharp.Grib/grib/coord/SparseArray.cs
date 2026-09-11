// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Store objects of type T in a sparse array.
	/// Conceptually a multidim array with shape[n] and totalsize.
	/// Stored as track[totalsize] = {0 = missing, else = index+1 into List<T> content}
	/// So we dont have to store missing Ts.
	/// 
	/// @author caron
	/// @since 11/24/13
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class SparseArray<T>
	public class SparseArray<T>
	{
	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(SparseArray<>));

	  private readonly int[] shape; // multidim sizes
	  private readonly int[] stride; // for index calculation
	  private readonly int totalSize; // product of sizes

	  private readonly int[] track; // index into content, size totalSize. LOOK could use byte, short to save memory ??
	  private readonly IList<T> content; // keep the things in a List.
	  private readonly int ndups;

	  public SparseArray(int[] shape, int[] track, IList<T> content, int ndups)
	  {
		this.shape = shape;
		this.totalSize = calcTotalSize(shape);
		this.stride = calcStrides(shape);

		this.track = track;
		this.content = content.AsReadOnly();
		this.ndups = ndups;

		if (track.Length != totalSize)
		{
		  throw new System.InvalidOperationException("track len " + track.Length + " != totalSize " + totalSize);
		}
	  }

	  internal static int calcTotalSize(int[] shape)
	  {
		int total = 1;
		foreach (int aSize in shape)
		{
		  total *= aSize;
		}
		return total;
	  }

	  private static int[] calcStrides(int[] shape)
	  {
		int[] strides = new int[shape.Length];
		int product = 1;
		for (int ii = shape.Length - 1; ii >= 0; ii--)
		{
		  int thisDim = shape[ii];
		  strides[ii] = product;
		  product *= thisDim;
		}
		return strides;
	  }

	  private int calcIndex(params int[] index)
	  {
		Debug.Assert(index.Length == shape.Length);
		int result = 0;
		for (int ii = 0; ii < index.Length; ii++)
		{
		  result += index[ii] * stride[ii];
		}
		return result;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public T getContent(int idx)
	  public virtual T getContent(int idx)
	  {
		if (idx >= track.Length || idx < 0)
		{
		  logger.error("BAD index get=" + idx + " max= " + track.Length, new Exception());
		}
		int contentIdx = track[idx] - 1;
		if (contentIdx < 0)
		{
		  return default(T); // missing
		}
		return content[contentIdx];
	  }

	  public virtual T getContent(int[] index)
	  {
		int where = calcIndex(index);
		return getContent(where);
	  }

	  public virtual int[] Shape
	  {
		  get
		  {
			return (int[])shape.Clone();
		  }
	  }

	  public virtual int Rank
	  {
		  get
		  {
			return shape.Length;
		  }
	  }

	  public virtual int TotalSize
	  {
		  get
		  {
			return totalSize;
		  }
	  }

	  public virtual int[] Track
	  {
		  get
		  {
			return track;
		  }
	  }

	  public virtual int getTrack(int idx)
	  {
		return track[idx];
	  }

	  public virtual IList<T> Content
	  {
		  get
		  {
			return content;
		  }
	  }

	  public virtual int countNotMissing()
	  { // LOOK could use content.size()
		int result = 0;
		foreach (int idx in track)
		{
		  if (idx > 0)
		  {
			result++;
		  }
		}
		return result;
	  }

	  public virtual int countMissing()
	  {
		int result = 0;
		foreach (int idx in track)
		{
		  if (idx == 0)
		  {
			result++;
		  }
		}
		return result;
	  }

	  public virtual float Density
	  {
		  get
		  {
			return (float) countNotMissing() / totalSize;
		  }
	  }

	  public virtual int Ndups
	  {
		  get
		  {
			return ndups;
		  }
	  }

	  public virtual int getNdups()
	  {
		return ndups;
	  }

	  public virtual int getTotalSize()
	  {
		return totalSize;
	  }

	  public virtual void showInfo(Formatter info, GribRecordStats all)
	  {
		info.format("SparseArray shape=%s, ", "[" + string.Join(", ", shape) + "]");
		info.format("ndups=%d, missing/total=%d/%d, density=%f%n", ndups, countMissing(), totalSize, Density);

		if (all != null)
		{
		  all.dups += ndups;
		  all.recordsUnique += countNotMissing();
		  all.recordsTotal += totalSize;
		  all.vars++;
		}

		info.format("%n");
		showMissing(info);
	  }

	  public virtual void showMissing(Formatter info)
	  {
		IList<int> sizes = new List<int>();
		foreach (int s in shape)
		{
		  if (s == 1)
		  {
			continue; // skip dimension len 1
		  }
		  sizes.Add(s);
		}
		showMissingRecurse(0, sizes, info);
	  }

	  private int showMissingRecurse(int offset, IList<int> sizes, Formatter f)
	  {
		if (sizes.Count == 0)
		{
		  return 0;
		}
		if (sizes.Count == 1)
		{
		  int len = sizes[0];
		  for (int i = 0; i < len; i++)
		  {
			bool hasRecord = track[offset + i] > 0;
			if (hasRecord)
			{
			  f.format("X");
			}
			else
			{
			  f.format("-");
			}
		  }
		  f.format("%n");
		  return len;

		}
		else
		{
		  int total = 0;
		  int len = sizes[0];
		  for (int i = 0; i < len; i++)
		  {
			int count = showMissingRecurse(offset, sizes.Skip(1).ToList(), f);
			offset += count;
			total += count;
		  }
		  f.format("%n");
		  return total;
		}
	  }

	  public virtual void showContent(Formatter f)
	  {
		int count = 0;
		f.format("Content%n");
		foreach (T record in content)
		{
		  f.format(" %d %s %n", count++, record);
		}
	  }

	  public virtual void showTracks(Formatter f)
	  {
		int count = 0;
		f.format("Track%n");
		foreach (int t in track)
		{
		  f.format(" %4d %5d %n", count++, t);
		}
	  }

	  ////////////////////////////////////////////////////////////////////////////////////

	  // separate out the mutable part
	  public class Builder<T1>
	  {
		internal int[] shape; // multidim sizes
		internal int[] stride; // for index calculation
		internal int totalSize; // product of sizes
		internal int ndups; // number of duplicates

		internal int[] track; // index into content, size totalSize. LOOK use byte, short to save memory ??
		internal IList<T1> content; // keep the things in a List.

		public Builder(params int[] shape)
		{
		  this.shape = shape;
		  this.totalSize = calcTotalSize(shape);
		  this.stride = calcStrides(shape);

		  track = new int[totalSize];
		  this.content = new List<T1>(totalSize); // LOOK could only allocate part of this
		}

		public virtual void add(T1 thing, Formatter info, params int[] index)
		{
		  content.Add(thing); // add the thing at end of list, idx = size-1
		  int where = calcIndex(index);
		  if (where < 0 || where >= track.Length)
		  {
			logger.error("BAD index add=" + "[" + string.Join(", ", index) + "]", new Exception());
		  }
		  if (track[where] > 0)
		  {
			ndups++;
			if (info != null)
			{
			  info.format(" duplicate %s%n     with %s%n%n", thing, content[track[where] - 1]);
			}
		  }
		  track[where] = content.Count; // 1-based so that 0 = missing, so content at where = content.get(track[where]-1)
		}

		public virtual void setTrack(int[] track)
		{
		  this.track = track;
		}

		public virtual void setContent(IList<T1> content)
		{
		  this.content = content;
		}

		internal virtual int calcIndex(params int[] index)
		{
		  Debug.Assert(index.Length == shape.Length);
		  int result = 0;
		  for (int ii = 0; ii < index.Length; ii++)
		  {
			result += index[ii] * stride[ii];
		  }
		  return result;
		}



		internal virtual int TotalSize
		{
			get
			{
			  return totalSize;
			}
		}

		internal virtual SparseArray<T1> finish()
		{
		  return new SparseArray<T1>(shape, track, content, ndups);
		}
	  }


	}

}
