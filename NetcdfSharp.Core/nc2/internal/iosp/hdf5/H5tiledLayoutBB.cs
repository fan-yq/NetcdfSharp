// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

/*
 * Copyright (c) 1998-2026 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf5
{

	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using Filter = NetcdfSharp.ucar.nc2.filter.Filter;
	using Filters = NetcdfSharp.ucar.nc2.filter.Filters;
	using UnknownFilterException = NetcdfSharp.ucar.nc2.filter.UnknownFilterException;
	using LayoutBB = NetcdfSharp.ucar.nc2.iosp.LayoutBB;
	using LayoutBBTiled = NetcdfSharp.ucar.nc2.iosp.LayoutBBTiled;
	using Layout = NetcdfSharp.ucar.nc2.iosp.Layout;
	using DataBTree = NetcdfSharp.ucar.nc2.iosp.hdf5.DataBTree;
	using IO = NetcdfSharp.ucar.nc2.util.IO;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Iterator to read/write subsets of an array.
	/// This calculates byte offsets for HD5 chunked datasets.
	/// Assumes that the data is stored in chunks, indexed by a Btree.
	/// Used for filtered data
	/// Had to split from old H5tiledLayoutBB because need to use H5headerNew.Vinfo.
	/// 
	/// @author caron
	/// </summary>
	public class H5tiledLayoutBB : LayoutBB
	{

	  internal const int DEFAULTZIPBUFFERSIZE = 512;
	  // System property name for -D flag
	  internal const string INFLATEBUFFERSIZE = "unidata.h5iosp.inflate.buffersize";

	  public static bool debugFilter;

	  private LayoutBBTiled @delegate;

	  private RandomAccessFile raf;
	  private Filter[] filters;
	  private ByteOrder byteOrder;

	  private Section want;
	  private int[] chunkSize; // from the StorageLayout message (exclude the elemSize)
	  private int elemSize; // last dimension of the StorageLayout message
	  private int nChunkDims;

	  private bool debug;

	  private int inflatebuffersize = DEFAULTZIPBUFFERSIZE;

	  /// <summary>
	  /// Constructor.
	  /// This is for HDF5 chunked data storage. The data is read by chunk, for efficiency.
	  /// </summary>
	  /// <param name="v2"> Variable to index over; assumes that vinfo is the data object </param>
	  /// <param name="wantSection"> the wanted section of data, contains a List of Range objects. must be complete </param>
	  /// <param name="raf"> the RandomAccessFile </param>
	  /// <param name="filterProps"> set of filter properties from which filter object will be created </param>
	  /// <exception cref="InvalidRangeException"> if section invalid for this variable </exception>
	  /// <exception cref="IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public H5tiledLayoutBB(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.ma2.Section wantSection, NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, H5objects.Filter[] filterProps, NetcdfSharp.java.nio.ByteOrder byteOrder) throws InvalidRangeException, NetcdfSharp.java.io.IOException
	  public H5tiledLayoutBB(Variable v2, Section wantSection, RandomAccessFile raf, H5objects.Filter[] filterProps, ByteOrder byteOrder) : this((H5headerNew.Vinfo) v2.SPobject, v2.Shape, v2.ElementSize, wantSection, raf, filterProps, byteOrder)
	  {
	  }

	  /// <summary>
	  /// This constructor can be used when the Variable is not yet built.
	  /// </summary>
	  /// <param name="vinfo"> the data object </param>
	  /// <param name="varShape"> the variable's shape </param>
	  /// <param name="elemSize"> the variable's element size in bytes </param>
	  /// <param name="wantSection"> the wanted section of data, contains a List of Range objects. must be complete </param>
	  /// <param name="raf"> the RandomAccessFile </param>
	  /// <param name="filterProps"> set of filter properties from which filter object will be created </param>
	  /// <exception cref="InvalidRangeException"> if section invalid for this variable </exception>
	  /// <exception cref="IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: H5tiledLayoutBB(H5headerNew.Vinfo vinfo, int[] varShape, int elemSize, NetcdfSharp.ucar.ma2.Section wantSection, NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, H5objects.Filter[] filterProps, NetcdfSharp.java.nio.ByteOrder byteOrder) throws InvalidRangeException, NetcdfSharp.java.io.IOException
	  internal H5tiledLayoutBB(H5headerNew.Vinfo vinfo, int[] varShape, int elemSize, Section wantSection, RandomAccessFile raf, H5objects.Filter[] filterProps, ByteOrder byteOrder)
	  {

		wantSection = Section.fill(wantSection, varShape);

		Debug.Assert(vinfo.isChunked);
		Debug.Assert(vinfo.btree != null);

		this.raf = raf;
		this.filters = new Filter[filterProps.Length];
		for (int i = 0; i < filterProps.Length; i++)
		{
		  // add var info to filter props
		  IDictionary<string, object> props = filterProps[i].Properties;
		  props[Filters.Keys.ELEM_SIZE] = elemSize;
		  // try to get filter by name or id, throw if not recognized filter
		  try
		  {
			filters[i] = Filters.getFilter(props);
		  }
		  catch (UnknownFilterException ex)
		  {
			throw new IOException(ex.Message, ex);
		  }
		}
		this.byteOrder = byteOrder;

		// we have to translate the want section into the same rank as the storageSize, in order to be able to call
		// Section.intersect(). It appears that storageSize (actually msl.chunkSize) may have an extra dimension, relative
		// to the Variable.
		DataType? dtype = vinfo.NCDataType;
		if ((dtype == DataType.CHAR) && (wantSection.Rank < vinfo.storageSize.Length))
		{
		  this.want = Section.builder().appendRanges(wantSection.Ranges).appendRange(1).build();
		}
		else
		{
		  this.want = wantSection;
		}

		// one less chunk dimension, except in the case of char
		nChunkDims = (dtype == DataType.CHAR) ? vinfo.storageSize.Length : vinfo.storageSize.Length - 1;
		this.chunkSize = new int[nChunkDims];
		System.Array.Copy(vinfo.storageSize, 0, chunkSize, 0, nChunkDims);
		this.elemSize = vinfo.storageSize[vinfo.storageSize.Length - 1]; // last one is always the elements size

		// create the data chunk iterator
		DataBTree.DataChunkIterator iter = vinfo.btree.getDataChunkIteratorFilter(this.want);
		DataChunkIterator dcIter = new DataChunkIterator(this, iter);
		@delegate = new LayoutBBTiled(dcIter, chunkSize, elemSize, this.want);

		if (JSystem.getProperty(INFLATEBUFFERSIZE) != null)
		{
		  try
		  {
			int size = int.Parse(JSystem.getProperty(INFLATEBUFFERSIZE));
			if (size <= 0)
			{
			  H5iospNew.log.warn(string.Format("-D{0} must be > 0", INFLATEBUFFERSIZE));
			}
			else
			{
			  this.inflatebuffersize = size;
			}
		  }
		  catch (System.FormatException)
		  {
			H5iospNew.log.warn(string.Format("-D{0} is not an integer", INFLATEBUFFERSIZE));
		  }
		}
		if (debugFilter)
		{
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
		  Console.Write("inflate buffer size -D{0} = {1:D}%n", INFLATEBUFFERSIZE, this.inflatebuffersize);
		}

		if (debug)
		{
		  Console.WriteLine(" H5tiledLayout: " + this);
		}
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

	  public virtual Layout.Chunk next()
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

	  private class DataChunkIterator : LayoutBBTiled.DataChunkIterator
	  {
		  private readonly H5tiledLayoutBB outerInstance;

		internal DataBTree.DataChunkIterator @delegate;

		internal DataChunkIterator(H5tiledLayoutBB outerInstance, DataBTree.DataChunkIterator @delegate)
		{
			this.outerInstance = outerInstance;
		  this.@delegate = @delegate;
		}

		public virtual bool hasNext()
		{
		  return @delegate.hasNext();
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.nc2.iosp.LayoutBBTiled.DataChunk next() throws NetcdfSharp.java.io.IOException
		public virtual LayoutBBTiled.DataChunk next()
		{
		  return new DataChunk(outerInstance, @delegate.next());
		}
	  }

	  private class DataChunk : LayoutBBTiled.DataChunk
	  {
		  private readonly H5tiledLayoutBB outerInstance;

		// Copied from ArrayList.
		internal static readonly int MAX_ARRAY_LEN = int.MaxValue - 8;

		internal DataBTree.DataChunk @delegate;

		internal DataChunk(H5tiledLayoutBB outerInstance, DataBTree.DataChunk @delegate)
		{
			this.outerInstance = outerInstance;
		  this.@delegate = @delegate;

		  // Check that the chunk length (delegate.size) isn't greater than the maximum array length that we can
		  // allocate (MAX_ARRAY_LEN). This condition manifests in two ways.
		  // 1) According to the HDF docs (https://www.hdfgroup.org/HDF5/doc/Advanced/Chunking/, "Chunk Maximum Limits"),
		  // max chunk length is 4GB (i.e. representable in an unsigned int). Java, however, only has signed ints.
		  // So, if we try to store a large unsigned int in a singed int, it'll overflow, and the signed int will come
		  // out negative. We're trusting here that the chunk size read from the HDF file is never negative.
		  // 2) In most JVM implementations MAX_ARRAY_LEN is actually less than Integer.MAX_VALUE (see note in ArrayList).
		  // So, we could have: "MAX_ARRAY_LEN < chunkSize <= Integer.MAX_VALUE".
		  if (@delegate.size < 0 || @delegate.size > MAX_ARRAY_LEN)
		  {
			// We want to report the size of the chunk, but we may be in an arithmetic overflow situation. So to get the
			// correct value, we're going to reinterpet the integer's bytes as long bytes.
			byte[] intBytes = Ints.toByteArray(@delegate.size);
			byte[] longBytes = new byte[8];
			System.Array.Copy(intBytes, 0, longBytes, 4, 4); // Copy int bytes to the lowest 4 positions.
			long chunkSize = Longs.fromByteArray(longBytes); // Method requires an array of length 8.

			throw new ArgumentException(string.Format("Filtered data chunk is {0} bytes and we must load it all " + "into memory. However the maximum length of a byte array in Java is {1}.", chunkSize, MAX_ARRAY_LEN));
		  }
		}

		public virtual int[] Offset
		{
			get
			{
			  int[] offset = @delegate.offset;
			  if (offset.Length > outerInstance.nChunkDims)
			  { // may have to eliminate last offset
				offset = new int[outerInstance.nChunkDims];
				System.Array.Copy(@delegate.offset, 0, offset, 0, outerInstance.nChunkDims);
			  }
			  return offset;
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public ByteBuffer getByteBuffer() throws NetcdfSharp.java.io.IOException
		public virtual ByteBuffer ByteBuffer
		{
			get
			{
			  try
			  {
				// read the data
				byte[] data = new byte[@delegate.size];
				outerInstance.raf.seek(@delegate.filePos);
				outerInstance.raf.readFully(data);

				// apply filters backwards
				for (int i = outerInstance.filters.Length - 1; i >= 0; i--)
				{
				  Filter f = outerInstance.filters[i];
				  if (isBitSet(@delegate.filterMask, i))
				  {
					if (outerInstance.debug)
					{
					  Console.WriteLine("skip for chunk " + @delegate);
					}
					continue;
				  }
				  data = f.decode(data);
				}

				ByteBuffer result = ByteBuffer.wrap(data);
				result.order(outerInstance.byteOrder);
				return result;
			  }
			  catch (System.OutOfMemoryException e)
			  {
				Exception oom = new System.OutOfMemoryException("Ran out of memory trying to read HDF5 filtered chunk. Either increase the " + "JVM's heap size (use the -Xmx switch) or reduce the size of the dataset's chunks (use nccopy -c).", e);
				throw oom;
			  }
			}
		}

		internal virtual bool isBitSet(int val, int bitno)
		{
		  return ((val >>> bitno) & 1) != 0;
		}
	  }
	}

}
