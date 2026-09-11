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
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp.hdf5
{
#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks
	using Section = NetcdfSharp.ucar.ma2.Section;
	using LayoutTiled = NetcdfSharp.ucar.nc2.iosp.LayoutTiled;
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// This holds the chunked data storage.
	/// level 1A
	/// A B-tree, version 1, used for data (node type 1)
	/// 
	/// Version 1 B-trees in HDF5 files an implementation of the B-link tree, in which the sibling nodes at a particular
	/// level
	/// in the tree are stored in a doubly-linked list
	/// The B-link trees implemented by the file format contain one more key than the number of children.
	/// In other words, each child pointer out of a B-tree node has a left key and a right key.
	/// The pointers out of internal nodes point to sub-trees while the pointers out of leaf nodes point to symbol nodes and
	/// raw data chunks. Aside from that difference, internal nodes and leaf nodes are identical.
	/// </summary>
	/// <seealso cref=""http://www.hdfgroup.org/HDF5/doc/H5.format.html.Btrees""
	/// @author caron
	/// @since 6/27/12/>
	public class DataBTree
	{
	  private const bool debugDataBtree = false;
	  private const bool debugDataChunk = false;
	  private const bool debugChunkOrder = false;
	  private static NetcdfSharp.java.io.PrintStream debugOut = NetcdfSharp.java.io.PrintStream.Out;

	  private readonly H5headerIF h5;
	  private readonly MemTracker memTracker;

	  private readonly long rootNodeAddress;
	  private readonly Tiling tiling;
	  private readonly int ndimStorage, wantType;

	  private object owner;

	  public DataBTree(H5headerIF h5, long rootNodeAddress, int[] varShape, int[] storageSize, MemTracker memTracker)
	  {
		this.h5 = h5;
		this.rootNodeAddress = rootNodeAddress;
		this.tiling = new Tiling(varShape, storageSize);
		this.ndimStorage = storageSize.Length;
		this.memTracker = memTracker;

		wantType = 1;
	  }

	  internal virtual RandomAccessFile RandomAccessFile
	  {
		  get
		  {
			return h5.RandomAccessFile;
		  }
	  }

	  public virtual object Owner
	  {
		  set
		  {
			this.owner = value;
		  }
	  }

	  // used by H5tiledLayoutBB
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public DataChunkIterator getDataChunkIteratorFilter(NetcdfSharp.ucar.ma2.Section want) throws NetcdfSharp.java.io.IOException
	  public virtual DataChunkIterator getDataChunkIteratorFilter(Section want)
	  {
		return new DataChunkIterator(this, want);
	  }

	  // used by H5tiledLayout
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.nc2.iosp.LayoutTiled.DataChunkIterator getDataChunkIteratorNoFilter(NetcdfSharp.ucar.ma2.Section want, int nChunkDim) throws NetcdfSharp.java.io.IOException
	  public virtual LayoutTiled.DataChunkIterator getDataChunkIteratorNoFilter(Section want, int nChunkDim)
	  {
		return new DataChunkIteratorNoFilter(this, want, nChunkDim);
	  }

	  // An Iterator over the DataChunks in the btree.
	  // returns the actual data from the btree leaf (level 0) nodes.
	  // used by H5tiledLayout, when there are no filters
	  internal class DataChunkIteratorNoFilter : LayoutTiled.DataChunkIterator
	  {
		  private readonly DataBTree outerInstance;

		internal Node root;
		internal int nChunkDim;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="want"> skip any nodes that are before this section </param>
		/// <param name="nChunkDim"> number of chunk dimensions - may be less than the offset[] length </param>
		/// <exception cref="IOException"> on error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: DataChunkIteratorNoFilter(NetcdfSharp.ucar.ma2.Section want, int nChunkDim) throws NetcdfSharp.java.io.IOException
		internal DataChunkIteratorNoFilter(DataBTree outerInstance, Section want, int nChunkDim)
		{
			this.outerInstance = outerInstance;
		  this.nChunkDim = nChunkDim;
		  root = new Node(outerInstance, outerInstance.rootNodeAddress, -1); // should we cache the nodes ???
		  int[] wantOrigin = (want != null) ? want.Origin : null;
		  root.first(wantOrigin);
		}

		public virtual bool hasNext()
		{
		  return root.hasNext(); // && !node.greaterThan(wantOrigin);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.nc2.iosp.LayoutTiled.DataChunk next() throws NetcdfSharp.java.io.IOException
		public virtual LayoutTiled.DataChunk next()
		{
		  DataChunk dc = root.next();
		  int[] offset = dc.offset;
		  if (offset.Length > nChunkDim)
		  { // may have to eliminate last offset
			offset = new int[nChunkDim];
			System.Array.Copy(dc.offset, 0, offset, 0, nChunkDim);
		  }
		  if (debugChunkOrder)
		  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			Console.Write("LayoutTiled.DataChunk next order {0:D}%n", outerInstance.tiling.order(dc.offset));
		  }

		  return new LayoutTiled.DataChunk(offset, dc.filePos);
		}
	  }

	  // An Iterator over the DataChunks in the btree.
	  // returns the data chunck info from the btree leaf (level 0) nodes
	  // used by H5tiledLayoutBB, when there are filters
	  public class DataChunkIterator
	  {
		  private readonly DataBTree outerInstance;

		internal Node root;
		internal int[] wantOrigin;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="want"> skip any nodes that are before this section </param>
		/// <exception cref="IOException"> on error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: DataChunkIterator(NetcdfSharp.ucar.ma2.Section want) throws NetcdfSharp.java.io.IOException
		internal DataChunkIterator(DataBTree outerInstance, Section want)
		{
			this.outerInstance = outerInstance;
		  root = new Node(outerInstance, outerInstance.rootNodeAddress, -1); // should we cache the nodes ???
		  wantOrigin = (want != null) ? want.Origin : null;
		  root.first(wantOrigin);
		}

		public virtual bool hasNext()
		{
		  return root.hasNext(); // && !node.greaterThan(wantOrigin);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public DataChunk next() throws NetcdfSharp.java.io.IOException
		public virtual DataChunk next()
		{
		  return root.next();
		}
	  }

	  // Btree nodes
	  internal class Node
	  {
		  private readonly DataBTree outerInstance;

		internal long address;
		internal int level, nentries;
		internal Node currentNode;

		// level 0 only
		internal IList<DataChunk> myEntries;
		// level > 0 only
		internal int[][] offset; // int[nentries][ndim]; // other levels

		// "For raw data chunk nodes, the child pointer is the address of a single raw data chunk"
		internal long[] childPointer; // long[nentries];

		internal int currentEntry; // track iteration; LOOK this seems fishy - why not an iterator ??

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Node(long address, long parent) throws NetcdfSharp.java.io.IOException
		internal Node(DataBTree outerInstance, long address, long parent)
		{
			this.outerInstance = outerInstance;
		  if (debugDataBtree)
		  {
			debugOut.println("\n--> DataBTree read tree at address=" + address + " parent= " + parent + " owner= " + outerInstance.owner);
		  }

		  outerInstance.RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN); // header information is in le byte order
		  outerInstance.RandomAccessFile.seek(outerInstance.h5.getFileOffset(address));
		  this.address = address;

		  string magic = outerInstance.RandomAccessFile.readString(4);
		  if (!magic.Equals("TREE"))
		  {
			throw new System.InvalidOperationException("DataBTree doesnt start with TREE");
		  }

		  int type = outerInstance.RandomAccessFile.readByte();
		  level = outerInstance.RandomAccessFile.readByte();
		  nentries = outerInstance.RandomAccessFile.readShort();
		  if (type != outerInstance.wantType)
		  {
			throw new System.InvalidOperationException("DataBTree must be type " + outerInstance.wantType);
		  }

		  long size = 8 + 2 * outerInstance.h5.SizeOffsets + ((long) nentries) * (8 + outerInstance.h5.SizeOffsets + 8 + outerInstance.ndimStorage);
		  if (outerInstance.memTracker != null)
		  {
			outerInstance.memTracker.addByLen("Data BTree (" + outerInstance.owner + ")", address, size);
		  }
		  if (debugDataBtree)
		  {
			debugOut.println("    type=" + type + " level=" + level + " nentries=" + nentries + " size = " + size);
		  }

		  long leftAddress = outerInstance.h5.readOffset();
		  long rightAddress = outerInstance.h5.readOffset();
		  if (debugDataBtree)
		  {
			debugOut.println("    leftAddress=" + leftAddress + " =0x" + Convert.ToString(leftAddress, 16) + " rightAddress=" + rightAddress + " =0x" + Convert.ToString(rightAddress, 16));
		  }

		  if (level == 0)
		  {
			// read all entries as a DataChunk
			myEntries = new List<DataChunk>();
			for (int i = 0; i <= nentries; i++)
			{
			  DataChunk dc = new DataChunk(outerInstance, outerInstance.ndimStorage, (i == nentries));
			  myEntries.Add(dc);
			  if (debugDataChunk)
			  {
				debugOut.println(dc.ToString());
			  }
			}
		  }
		  else
		  { // just track the offsets and node addresses
// JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
// ORIGINAL LINE: offset = new int[nentries + 1][outerInstance.ndimStorage];
			offset = RectangularArrays.RectangularIntArray(nentries + 1, outerInstance.ndimStorage);
			childPointer = new long[nentries + 1];
			for (int i = 0; i <= nentries; i++)
			{
			  outerInstance.RandomAccessFile.skipBytes(8); // skip size, filterMask
			  for (int j = 0; j < outerInstance.ndimStorage; j++)
			  {
				long loffset = outerInstance.RandomAccessFile.readLong();
				Debug.Assert(loffset < int.MaxValue);
				offset[i][j] = (int) loffset;
			  }
			  this.childPointer[i] = (i == nentries) ? -1 : outerInstance.h5.readOffset();
			  if (debugDataBtree)
			  {
				debugOut.Write("    childPointer=" + childPointer[i] + " =0x" + Convert.ToString(childPointer[i], 16));
				foreach (long anOffset in offset[i])
				{
				  debugOut.Write(" " + anOffset);
				}
				debugOut.WriteLine();
			  }
			}
		  }
		}

		// this finds the first entry we dont want to skip.
		// entry i goes from [offset(i),offset(i+1))
		// we want to skip any entries we dont need, namely those where want >= offset(i+1)
		// so keep skipping until want < offset(i+1)
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void first(int[] wantOrigin) throws NetcdfSharp.java.io.IOException
		internal virtual void first(int[] wantOrigin)
		{
		  if (debugChunkOrder && wantOrigin != null)
		  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			Console.Write("Level {0:D}: Tile want {1:D}%n", level, outerInstance.tiling.order(wantOrigin));
		  }
		  if (level == 0)
		  {
			currentEntry = 0;
			// note nentries-1 - assume dont skip the last one
			for (currentEntry = 0; currentEntry < nentries - 1; currentEntry++)
			{
			  DataChunk entry = myEntries[currentEntry + 1]; // look at the next one
			  if (debugChunkOrder)
			  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Console.Write(" Entry={0:D}: Tile ending order= {1:D}%n", currentEntry, outerInstance.tiling.order(entry.offset));
			  }
			  if ((wantOrigin == null) || outerInstance.tiling.compare(wantOrigin, entry.offset) < 0)
			  {
				break;
			  }
			}
			if (debugChunkOrder)
			{
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			  Console.Write("Level {0:D} use entry= {1:D}%n", level, currentEntry);
			}

		  }
		  else
		  {
			currentNode = null;
			for (currentEntry = 0; currentEntry < nentries; currentEntry++)
			{
			  if (debugChunkOrder)
			  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Console.Write(" Entry={0,3:D} offset [{1,-15}]: Tile order {2:D}-{3:D}%n", currentEntry, Arrays.toString(offset[currentEntry]), outerInstance.tiling.order(offset[currentEntry]), outerInstance.tiling.order(offset[currentEntry + 1]));
			  }
			  if ((wantOrigin == null) || outerInstance.tiling.compare(wantOrigin, offset[currentEntry + 1]) < 0)
			  {
				currentNode = new Node(outerInstance, childPointer[currentEntry], this.address);
				if (debugChunkOrder)
				{
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				  Console.Write("Level {0:D} use entry= {1:D}%n", level, currentEntry);
				}
				currentNode.first(wantOrigin);
				break;
			  }
			}

			// heres the case where its the last entry we want; the tiling.compare() above may fail
			if (currentNode == null)
			{
			  currentEntry = nentries - 1;
			  currentNode = new Node(outerInstance, childPointer[currentEntry], this.address);
			  currentNode.first(wantOrigin);
			}
		  }

		  Debug.Assert((nentries == 0) || (currentEntry < nentries), currentEntry + " >= " + nentries);
		}

		// LOOK - wouldnt be a bad idea to terminate if possible instead of running through all subsequent entries
		internal virtual bool hasNext()
		{
		  if (level == 0)
		  {
			return currentEntry < nentries;

		  }
		  else
		  {
			if (currentNode.hasNext())
			{
			  return true;
			}
			return currentEntry < nentries - 1;
		  }
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: DataChunk next() throws NetcdfSharp.java.io.IOException
		internal virtual DataChunk next()
		{
		  if (level == 0)
		  {
			return myEntries[currentEntry++];

		  }
		  else
		  {
			if (currentNode.hasNext())
			{
			  return currentNode.next();
			}

			currentEntry++;
			currentNode = new Node(outerInstance, childPointer[currentEntry], this.address);
			currentNode.first(null);
			return currentNode.next();
		  }
		}
	  }

	  // these are part of the level 1A data structure, type 1
	  // see http://www.hdfgroup.org/HDF5/doc/H5.format.html#V1Btrees,
	  // see "Key" field (type 1) p 10
	  // this is only for leaf nodes (level 0)
	  public class DataChunk
	  {
		  private readonly DataBTree outerInstance;

		public readonly int size; // size of chunk in bytes; need storage layout dimensions to interpret
		public readonly int filterMask; // bitfield indicating which filters have been skipped for this chunk
		public readonly int[] offset; // offset index of this chunk, relative to entire array
		public readonly long filePos; // filePos of a single raw data chunk, already shifted by the offset if needed

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: DataChunk(int ndim, boolean last) throws NetcdfSharp.java.io.IOException
		internal DataChunk(DataBTree outerInstance, int ndim, bool last)
		{
			this.outerInstance = outerInstance;
		  this.size = outerInstance.RandomAccessFile.readInt();
		  this.filterMask = outerInstance.RandomAccessFile.readInt();
		  offset = new int[ndim];
		  for (int i = 0; i < ndim; i++)
		  {
			long loffset = outerInstance.RandomAccessFile.readLong();
			Debug.Assert(loffset < int.MaxValue);
			offset[i] = (int) loffset;
		  }
		  this.filePos = last ? -1 : outerInstance.h5.readAddress();
		  if (outerInstance.memTracker != null)
		  {
			outerInstance.memTracker.addByLen("Chunked Data (" + outerInstance.owner + ")", filePos, size);
		  }
		}

		public override string ToString()
		{
		  StringBuilder sbuff = new StringBuilder();
		  sbuff.Append("  ChunkedDataNode size=").Append(size).Append(" filterMask=").Append(filterMask).Append(" filePos=").Append(filePos).Append(" offsets= ");
		  foreach (long anOffset in offset)
		  {
			sbuff.Append(anOffset).Append(" ");
		  }
		  return sbuff.ToString();
		}
	  }

#pragma warning restore CS0162

	}

}
