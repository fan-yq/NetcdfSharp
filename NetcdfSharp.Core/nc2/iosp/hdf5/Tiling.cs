// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.hdf5
{

	/// <summary>
	/// A Tiling divides a multidimensional index into tiles.
	/// Abstraction of HDF5 chunking.
	/// <para>
	/// Index are points in the original multidimensional index.
	/// Tiles are points in the tiled space.
	/// </para>
	/// <para>
	/// Each tile has the same size, given by tileSize.
	/// 
	/// @author caron
	/// @since Jul 20, 2007
	/// </para>
	/// </summary>
	public class Tiling
	{

	  private readonly int rank;
	  private readonly int[] shape; // overall data shape - may be larger than actual variable shape
	  private readonly int[] chunk; // actual storage is in this shape
	  private readonly int[] stride; // for computing time index

	  /// <summary>
	  /// Create a Tiling
	  /// </summary>
	  /// <param name="shape"> overall shape of the dataset's index space </param>
	  /// <param name="chunk"> tile size. may be larger than the shape. </param>
	  public Tiling(int[] shape, int[] chunk)
	  {
		Debug.Assert(shape.Length <= chunk.Length); // convenient to allow tileSize to have (an) extra dimension at the end
											 // to accomodate hdf5 storage, which has the element size
		this.rank = shape.Length;
		this.chunk = chunk;
		this.shape = new int[rank];
		for (int i = 0; i < rank; i++)
		{
		  this.shape[i] = Math.Max(shape[i], chunk[i]);
		}
		// int[] tile = tile(this.shape);

		int[] tileSize = new int[rank];
		for (int i = 0; i < rank; i++)
		{
		  tileSize[i] = (this.shape[i] + chunk[i] - 1) / chunk[i];
		}

		this.stride = new int[rank];
		int strider = 1;
		for (int k = rank - 1; k >= 0; k--)
		{
		  stride[k] = strider;
		  strider *= tileSize[k];
		}
	  }

	  internal virtual string show(int[] a)
	  {
		Formatter f = new Formatter();
		foreach (int val in a)
		{
		  f.format("%3d,", val);
		}
		return f.ToString();
	  }

	  /// <summary>
	  /// Compute the tile
	  /// </summary>
	  /// <param name="pt"> index point </param>
	  /// <returns> corresponding tile </returns>
	  public virtual int[] tile(int[] pt)
	  {
		// assert pt.length == rank;
		int useRank = Math.Min(rank, pt.Length); // eg varlen (datatype 9) has mismatch
		int[] tile = new int[useRank];
		for (int i = 0; i < useRank; i++)
		{
		  // 7/30/2016 jcaron. Apparently in some cases, at the end of the array, the index can be greater than the shape.
		  // eg cdmUnitTest/formats/netcdf4/UpperDeschutes_t4p10_swemelt.nc
		  // Presumably to have even chunks. Could try to calculate the last even chunk.
		  // For now im removing this consistency check.
		  // assert shape[i] >= pt[i] : String.format("shape[%s]=(%s) should not be less than pt[%s]=(%s)", i, shape[i], i,
		  // pt[i]);
		  tile[i] = pt[i] / chunk[i]; // seems wrong, rounding down ??
		}
		return tile;
	  }

	  /// <summary>
	  /// Get order based on which tile the pt belongs to
	  /// </summary>
	  /// <param name="pt"> index point </param>
	  /// <returns> order number based on which tile the pt belongs to </returns>
	  public virtual int order(int[] pt)
	  {
		int[] tile = this.tile(pt);
		int order = 0;
		int useRank = Math.Min(rank, pt.Length); // eg varlen (datatype 9) has mismatch
		for (int i = 0; i < useRank; i++)
		{
		  order += stride[i] * tile[i];
		}
		return order;
	  }

	  /// <summary>
	  /// Create an ordering of index points based on which tile the point is in.
	  /// </summary>
	  /// <param name="p1"> index point 1 </param>
	  /// <param name="p2"> index point 2 </param>
	  /// <returns> order(p1) - order(p2) : negative if p1 < p2, positive if p1 > p2 , 0 if equal </returns>
	  public virtual int compare(int[] p1, int[] p2)
	  {
		return order(p1) - order(p2);
	  }
	}

}
