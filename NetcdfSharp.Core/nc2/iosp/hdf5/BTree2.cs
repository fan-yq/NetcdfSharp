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

namespace NetcdfSharp.ucar.nc2.iosp.hdf5
{
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// // Level 1A2
	/// 
	/// These are used for symbols, not data i think. Version 1 is H5header.GroupBTree.
	/// 
	/// Version 2 B-trees are "traditional" B-trees, with one major difference. Instead of just using a simple pointer
	/// (or address in the file) to a child of an internal node, the pointer to the child node contains two additional
	/// pieces of information: the number of records in the child node itself, and the total number of records in the child
	/// node and all its descendents. Storing this additional information allows fast array-like indexing to locate the n'th
	/// record in the B-tree.
	/// 
	/// The entry into a version 2 B-tree is a header which contains global information about the structure of the B-tree.
	/// The root node address field in the header points to the B-tree root node, which is either an internal or leaf node,
	/// depending on the value in the header's depth field. An internal node consists of records plus pointers to further
	/// leaf
	/// or internal nodes in the tree. A leaf node consists of solely of records. The format of the records depends on the
	/// B-tree type (stored in the header).
	/// 
	/// 
	/// @author caron
	/// @since 6/27/12
	/// </summary>
	public class BTree2
	{
	  private bool debugBtree2, debugPos;
	  private NetcdfSharp.java.io.PrintStream debugOut = NetcdfSharp.java.io.PrintStream.Out;

	  public readonly byte btreeType;
	  private int nodeSize; // size in bytes of btree nodes
	  private short recordSize; // size in bytes of btree records

	  private string owner;
	  private H5headerIF h5;
	  private RandomAccessFile raf;

	  public IList<Entry2> entryList = new List<Entry2>();

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public BTree2(H5headerIF h5, String owner, long address) throws NetcdfSharp.java.io.IOException
	  public BTree2(H5headerIF h5, string owner, long address)
	  {
		this.h5 = h5;
		this.raf = h5.RandomAccessFile;
		this.owner = owner;

		raf.seek(h5.getFileOffset(address));

		// header
		byte[] heapname = new byte[4];
		raf.readFully(heapname);
		string magic = StringHelper.NewString(heapname, Encoding.UTF8);
		if (!magic.Equals("BTHD"))
		{
		  throw new System.InvalidOperationException(magic + " should equal BTHD");
		}

		byte version = raf.readByte();
		btreeType = raf.readByte();
		nodeSize = raf.readInt();
		recordSize = raf.readShort();
		short treeDepth = raf.readShort();
		byte split = raf.readByte();
		byte merge = raf.readByte();
		long rootNodeAddress = h5.readOffset();
		short numRecordsRootNode = raf.readShort();
		long totalRecords = h5.readLength(); // total in entire btree
		int checksum = raf.readInt();

		if (debugBtree2)
		{
		  debugOut.printf("BTree2 (%s) version=%d type=%d treeDepth=%d nodeSize=%d recordSize=%d numRecordsRootNode=%d totalRecords=%d rootNodeAddress=%d%n", owner, version, btreeType, treeDepth, nodeSize, recordSize, numRecordsRootNode, totalRecords, rootNodeAddress);
		}

		if (treeDepth > 0)
		{
		  InternalNode node = new InternalNode(this, rootNodeAddress, numRecordsRootNode, recordSize, treeDepth);
		  node.recurse();
		}
		else
		{
		  LeafNode leaf = new LeafNode(this, rootNodeAddress, numRecordsRootNode);
		  leaf.addEntries(entryList);
		}
	  }

	  internal virtual BTree2.Record1 getEntry1(int hugeObjectID)
	  {
		foreach (Entry2 entry in entryList)
		{
		  BTree2.Record1 record1 = (BTree2.Record1) entry.record;
		  if (record1.hugeObjectID == hugeObjectID)
		  {
			return record1;
		  }
		}
		return null;
	  }

	  // these are part of the level 1A data structure, type = 0
	  public class Entry2
	  {
		internal long childAddress, nrecords, totNrecords;
		public object record;
	  }

	  internal class InternalNode
	  {
		  private readonly BTree2 outerInstance;

		internal Entry2[] entries;
		internal int depth;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: InternalNode(long address, short nrecords, short recordSize, int depth) throws NetcdfSharp.java.io.IOException
		internal InternalNode(BTree2 outerInstance, long address, short nrecords, short recordSize, int depth)
		{
			this.outerInstance = outerInstance;
		  this.depth = depth;
		  outerInstance.raf.seek(outerInstance.h5.getFileOffset(address));

		  if (outerInstance.debugPos)
		  {
			outerInstance.debugOut.println("--Btree2 InternalNode position=" + outerInstance.raf.FilePointer);
		  }

		  // header
		  byte[] sig = new byte[4];
		  outerInstance.raf.readFully(sig);
		  string magic = StringHelper.NewString(sig, Encoding.UTF8);
		  if (!magic.Equals("BTIN"))
		  {
			throw new System.InvalidOperationException(magic + " should equal BTIN");
		  }

		  byte version = outerInstance.raf.readByte();
		  byte nodeType = outerInstance.raf.readByte();
		  if (nodeType != outerInstance.btreeType)
		  {
			throw new System.InvalidOperationException();
		  }

		  if (outerInstance.debugBtree2)
		  {
			outerInstance.debugOut.println("   BTree2 InternalNode version=" + version + " type=" + nodeType + " nrecords=" + nrecords);
		  }

		  entries = new Entry2[nrecords + 1]; // did i mention theres actually n+1 children?
		  for (int i = 0; i < nrecords; i++)
		  {
			entries[i] = new Entry2();
			entries[i].record = outerInstance.readRecord(outerInstance.btreeType);
		  }
		  entries[nrecords] = new Entry2();

		  int maxNumRecords = outerInstance.nodeSize / recordSize; // LOOK ?? guessing
		  int maxNumRecordsPlusDesc = outerInstance.nodeSize / recordSize; // LOOK ?? guessing
		  for (int i = 0; i < nrecords + 1; i++)
		  {
			Entry2 e = entries[i];
			e.childAddress = outerInstance.h5.readOffset();
			e.nrecords = outerInstance.h5.readVariableSizeUnsigned(1); // readVariableSizeMax(maxNumRecords);
			if (depth > 1)
			{
			  e.totNrecords = outerInstance.h5.readVariableSizeUnsigned(2); // readVariableSizeMax(maxNumRecordsPlusDesc);
			}

			if (outerInstance.debugBtree2)
			{
			  outerInstance.debugOut.println(" BTree2 entry childAddress=" + e.childAddress + " nrecords=" + e.nrecords + " totNrecords=" + e.totNrecords);
			}
		  }

		  // skip
		  outerInstance.raf.readInt();
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: void recurse() throws NetcdfSharp.java.io.IOException
		internal virtual void recurse()
		{
		  foreach (Entry2 e in entries)
		  {
			if (depth > 1)
			{
			  InternalNode node = new InternalNode(outerInstance, e.childAddress, (short) e.nrecords, outerInstance.recordSize, depth - 1);
			  node.recurse();
			}
			else
			{
			  long nrecs = e.nrecords;
			  LeafNode leaf = new LeafNode(outerInstance, e.childAddress, (short) nrecs);
			  leaf.addEntries(outerInstance.entryList);
			}
			if (e.record != null) // last one is null
			{
			  outerInstance.entryList.Add(e);
			}
		  }
		}
	  }

	  internal class LeafNode
	  {
		  private readonly BTree2 outerInstance;

		internal Entry2[] entries;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: LeafNode(long address, short nrecords) throws NetcdfSharp.java.io.IOException
		internal LeafNode(BTree2 outerInstance, long address, short nrecords)
		{
			this.outerInstance = outerInstance;
		  outerInstance.raf.seek(outerInstance.h5.getFileOffset(address));

		  if (outerInstance.debugPos)
		  {
			outerInstance.debugOut.println("--Btree2 InternalNode position=" + outerInstance.raf.FilePointer);
		  }

		  // header
		  byte[] sig = new byte[4];
		  outerInstance.raf.readFully(sig);
		  string magic = StringHelper.NewString(sig, Encoding.UTF8);
		  if (!magic.Equals("BTLF"))
		  {
			throw new System.InvalidOperationException(magic + " should equal BTLF");
		  }

		  byte version = outerInstance.raf.readByte();
		  byte nodeType = outerInstance.raf.readByte();
		  if (nodeType != outerInstance.btreeType)
		  {
			throw new System.InvalidOperationException();
		  }

		  if (outerInstance.debugBtree2)
		  {
			outerInstance.debugOut.println("   BTree2 LeafNode version=" + version + " type=" + nodeType + " nrecords=" + nrecords);
		  }

		  entries = new Entry2[nrecords];
		  for (int i = 0; i < nrecords; i++)
		  {
			entries[i] = new Entry2();
			entries[i].record = outerInstance.readRecord(outerInstance.btreeType);
		  }

		  // skip
		  outerInstance.raf.readInt();
		}

		internal virtual void addEntries(IList<Entry2> list)
		{
		  Collections.addAll(list, entries);
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Object readRecord(int type) throws NetcdfSharp.java.io.IOException
	  internal virtual object readRecord(int type)
	  {
		switch (type)
		{
		  case 1:
			return new Record1(this);
		  case 2:
			return new Record2(this);
		  case 3:
			return new Record3(this);
		  case 4:
			return new Record4(this);
		  case 5:
			return new Record5(this);
		  case 6:
			return new Record6(this);
		  case 7:
		  {
			return new Record70(this); // LOOK wrong
		  }
		  case 8:
			return new Record8(this);
		  case 9:
			return new Record9(this);
		  default:
			throw new System.InvalidOperationException();
		}
	  }

	  internal class Record1
	  {
		  private readonly BTree2 outerInstance;

		internal long hugeObjectAddress, hugeObjectLength, hugeObjectID;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record1() throws NetcdfSharp.java.io.IOException
		internal Record1(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  hugeObjectAddress = outerInstance.h5.readOffset();
		  hugeObjectLength = outerInstance.h5.readLength();
		  hugeObjectID = outerInstance.h5.readLength();
		}
	  }

	  internal class Record2
	  {
		  private readonly BTree2 outerInstance;

		internal long hugeObjectAddress, hugeObjectLength, hugeObjectID, hugeObjectSize;
		internal int filterMask;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record2() throws NetcdfSharp.java.io.IOException
		internal Record2(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  hugeObjectAddress = outerInstance.h5.readOffset();
		  hugeObjectLength = outerInstance.h5.readLength();
		  filterMask = outerInstance.raf.readInt();
		  hugeObjectSize = outerInstance.h5.readLength();
		  hugeObjectID = outerInstance.h5.readLength();
		}
	  }

	  internal class Record3
	  {
		  private readonly BTree2 outerInstance;

		internal long hugeObjectAddress, hugeObjectLength;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record3() throws NetcdfSharp.java.io.IOException
		internal Record3(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  hugeObjectAddress = outerInstance.h5.readOffset();
		  hugeObjectLength = outerInstance.h5.readLength();
		}
	  }

	  internal class Record4
	  {
		  private readonly BTree2 outerInstance;

		internal long hugeObjectAddress, hugeObjectLength, hugeObjectID, hugeObjectSize;
		internal int filterMask;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record4() throws NetcdfSharp.java.io.IOException
		internal Record4(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  hugeObjectAddress = outerInstance.h5.readOffset();
		  hugeObjectLength = outerInstance.h5.readLength();
		  filterMask = outerInstance.raf.readInt();
		  hugeObjectSize = outerInstance.h5.readLength();
		}
	  }

	  public class Record5
	  {
		  private readonly BTree2 outerInstance;

		internal int nameHash;
		internal byte[] heapId = new byte[7];

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record5() throws NetcdfSharp.java.io.IOException
		internal Record5(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  nameHash = outerInstance.raf.readInt();
		  outerInstance.raf.readFully(heapId);

		  if (outerInstance.debugBtree2)
		  {
			outerInstance.debugOut.println("  record5 nameHash=" + nameHash + " heapId=" + "[" + string.Join(", ", heapId) + "]");
		  }
		}

		public virtual byte[] HeapId
		{
			get
			{
			  return heapId;
			}
		}
	  }

	  public class Record6
	  {
		  private readonly BTree2 outerInstance;

		internal long creationOrder;
		internal byte[] heapId = new byte[7];

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record6() throws NetcdfSharp.java.io.IOException
		internal Record6(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  creationOrder = outerInstance.raf.readLong();
		  outerInstance.raf.readFully(heapId);
		  if (outerInstance.debugBtree2)
		  {
			outerInstance.debugOut.println("  record6 creationOrder=" + creationOrder + " heapId=" + "[" + string.Join(", ", heapId) + "]");
		  }
		}

		public virtual byte[] HeapId
		{
			get
			{
			  return heapId;
			}
		}
	  }

	  internal class Record70
	  {
		  private readonly BTree2 outerInstance;

		internal byte location;
		internal int refCount;
		internal byte[] id = new byte[8];

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record70() throws NetcdfSharp.java.io.IOException
		internal Record70(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  location = outerInstance.raf.readByte();
		  refCount = outerInstance.raf.readInt();
		  outerInstance.raf.readFully(id);
		}
	  }

	  internal class Record71
	  {
		  private readonly BTree2 outerInstance;

		internal byte location, messtype;
		internal short index;
		internal long address;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record71() throws NetcdfSharp.java.io.IOException
		internal Record71(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  location = outerInstance.raf.readByte();
		  outerInstance.raf.readByte(); // skip a byte
		  messtype = outerInstance.raf.readByte();
		  index = outerInstance.raf.readShort();
		  address = outerInstance.h5.readOffset();
		}
	  }

	  public class Record8
	  {
		  private readonly BTree2 outerInstance;

		internal byte flags;
		internal int creationOrder, nameHash;
		internal byte[] heapId = new byte[8];

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record8() throws NetcdfSharp.java.io.IOException
		internal Record8(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  outerInstance.raf.readFully(heapId);
		  flags = outerInstance.raf.readByte();
		  creationOrder = outerInstance.raf.readInt();
		  nameHash = outerInstance.raf.readInt();
		  if (outerInstance.debugBtree2)
		  {
			outerInstance.debugOut.println("  record8 creationOrder=" + creationOrder + " heapId=" + "[" + string.Join(", ", heapId) + "]");
		  }
		}

		public virtual byte[] HeapId
		{
			get
			{
			  return heapId;
			}
		}
	  }

	  public class Record9
	  {
		  private readonly BTree2 outerInstance;

		internal byte flags;
		internal int creationOrder;
		internal byte[] heapId = new byte[8];

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: Record9() throws NetcdfSharp.java.io.IOException
		internal Record9(BTree2 outerInstance)
		{
			this.outerInstance = outerInstance;
		  outerInstance.raf.readFully(heapId);
		  flags = outerInstance.raf.readByte();
		  creationOrder = outerInstance.raf.readInt();
		}

		public virtual byte[] HeapId
		{
			get
			{
			  return heapId;
			}
		}
	  }

	}

}
