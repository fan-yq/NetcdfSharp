// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Diagnostics;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf5
{
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using Layout = NetcdfSharp.ucar.nc2.iosp.Layout;
	using LayoutTiled = NetcdfSharp.ucar.nc2.iosp.LayoutTiled;

	/// <summary>
	/// Iterator to read/write subsets of an array.
	/// This calculates byte offsets for HD5 chunked datasets.
	/// Assumes that the data is stored in chunks, indexed by a Btree.
	/// for unfiltered data only
	/// 
	/// @author caron
	/// </summary>
	public class H5tiledLayout : Layout
	{
	  private LayoutTiled @delegate;

	  private Section want;
	  private int[] chunkSize; // from the StorageLayout message (exclude the elemSize)
	  private int elemSize; // last dimension of the StorageLayout message

	  private bool debug;

	  /// <summary>
	  /// Constructor.
	  /// This is for HDF5 chunked data storage. The data is read by chunk, for efficency.
	  /// </summary>
	  /// <param name="vinfo"> the vinfo object for this variable </param>
	  /// <param name="dtype"> type of data. may be different from v2. </param>
	  /// <param name="wantSection"> the wanted section of data, contains a List of Range objects, must be complete </param>
	  /// <exception cref="IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public H5tiledLayout(H5headerNew.Vinfo vinfo, NetcdfSharp.ucar.ma2.DataType dtype, NetcdfSharp.ucar.ma2.Section wantSection) throws NetcdfSharp.java.io.IOException
	  public H5tiledLayout(H5headerNew.Vinfo vinfo, DataType dtype, Section wantSection)
	  {
		Debug.Assert(vinfo.isChunked);
		Debug.Assert(vinfo.btree != null);

		// we have to translate the want section into the same rank as the storageSize, in order to be able to call
		// Section.intersect(). It appears that storageSize (actually msl.chunkSize) may have an extra dimension, relative
		// to the Variable.
		if ((dtype == DataType.CHAR) && (wantSection.Rank < vinfo.storageSize.Length))
		{
		  this.want = Section.builder().appendRanges(wantSection.Ranges).appendRange(1).build();
		}
		else
		{
		  this.want = wantSection;
		}

		// one less chunk dimension, except in the case of char
		int nChunkDims = (dtype == DataType.CHAR) ? vinfo.storageSize.Length : vinfo.storageSize.Length - 1;
		this.chunkSize = new int[nChunkDims];
		System.Array.Copy(vinfo.storageSize, 0, chunkSize, 0, nChunkDims);

		this.elemSize = vinfo.storageSize[vinfo.storageSize.Length - 1]; // last one is always the elements size
		if (debug)
		{
		  Console.WriteLine(" H5tiledLayout: " + this);
		}

		// create the data chunk iterator
		LayoutTiled.DataChunkIterator iter = vinfo.btree.getDataChunkIteratorNoFilter(this.want, nChunkDims);
		@delegate = new LayoutTiled(iter, chunkSize, elemSize, this.want);
	  }

	  public virtual long TotalNelems
	  {
		  get
		  {
			return @delegate.TotalNelems;
		  }
	  }

	  public virtual int ElemSize
	  {
		  get
		  {
			return @delegate.ElemSize;
		  }
	  }

	  public virtual bool hasNext()
	  {
		return @delegate.hasNext();
	  }

	  public virtual Chunk next()
	  {
		return @delegate.next();
	  }

	  public override string ToString()
	  {
		StringBuilder sbuff = new StringBuilder();
		sbuff.Append("want=").Append(want).Append("; ");
		sbuff.Append("chunkSize=[");
		for (int i = 0; i < chunkSize.Length; i++)
		{
		  if (i > 0)
		  {
			sbuff.Append(",");
		  }
		  sbuff.Append(chunkSize[i]);
		}
		sbuff.Append("] totalNelems=").Append(TotalNelems);
		sbuff.Append(" elemSize=").Append(elemSize);
		return sbuff.ToString();
	  }


	}

}
