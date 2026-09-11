// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using NetcdfSharp.ucar.ma2;
	using BitReader = NetcdfSharp.ucar.nc2.iosp.BitReader;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Sequence = NetcdfSharp.ucar.nc2.Sequence;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Read data for uncompressed messages.
	/// 
	/// Within one message there are n obs (datasets) and s fields in each dataset.
	/// For uncompressed datasets, storage order is data(obs, fld) (fld varying fastest) :
	/// 
	/// R11, R12, R13, . . . R1s
	/// R21, R22, R23, . . . R2s
	/// ....
	/// Rn1, Rn2, Rn3, . . . Rns
	/// 
	/// where Rij is the jth value of the ith data subset.
	/// the datasets each occupy an identical number of bits, unless delayed replication is used,
	/// and are not necessarily aligned on octet boundaries.
	/// 
	/// A replicated field (structure) takes a group of fields and replicates them:
	/// 
	/// Ri1, (Ri2, Ri3)*r, . . . Ris
	/// 
	/// where r is set in the data descriptor, and is the same for all datasets.
	/// 
	/// A delayed replicated field (sequence) takes a group of fields and replicates them, and adds the number of
	/// replications
	/// in the data :
	/// 
	/// Ri1, dri, (Ri2, Ri3)*dri, . . . Ris
	/// 
	/// where the width (nbits) of dr is set in the data descriptor. This dr can be different for each dataset in the
	/// message.
	/// It can be 0. When it has a bit width of 1, it indicates an optional set of fields.
	/// 
	/// --------------------------
	/// 
	/// We use an ArrayStructureBB to hold the data, and fill it sequentially as we scan the message.
	/// Fixed length nested Structures are kept in the ArrayStructureBB.
	/// Variable length objects (Strings, Sequences) are added to the heap.
	/// </summary>

	public class MessageUncompressedDataReader
	{

		/// <summary>
		/// Read all datasets from a single message
		/// </summary>
		/// <param name="s"> outer variables </param>
		/// <param name="proto"> prototype message, has been processed </param>
		/// <param name="m"> read this message </param>
		/// <param name="raf"> from this file </param>
		/// <param name="f"> output bit count debugging info (may be null) </param>
		/// <returns> ArraySTructure with all the data from the message in it. </returns>
		/// <exception cref="IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: ArrayStructure readEntireMessage(NetcdfSharp.ucar.nc2.Structure s, Message proto, Message m, NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
		internal virtual ArrayStructure readEntireMessage(Structure s, Message proto, Message m, RandomAccessFile raf, Formatter f)
		{
			// transfer info from proto message
			DataDescriptor.transferInfo(proto.RootDataDescriptor.SubKeys, m.RootDataDescriptor.SubKeys);

			// allocate ArrayStructureBB for outer structure
			// This assumes that all of the fields and all of the datasets are being read
			StructureMembers members = s.makeStructureMembers();
			ArrayStructureBB.setOffsets(members);

			int n = m.NumberDatasets;
			ArrayStructureBB abb = new ArrayStructureBB(members, new int[] { n });
			ByteBuffer bb = abb.ByteBuffer;
			bb.order(ByteOrder.BIG_ENDIAN);

			bool addTime = false; // (s.findVariable(BufrIosp2.TIME_NAME) != null);
			readData(abb, m, raf, null, addTime, f);
			return abb;
		}

		/// <summary>
		/// Read some or all datasets from a single message
		/// </summary>
		/// <param name="abb"> place data into here in order (may be null) </param>
		/// <param name="m"> read this message </param>
		/// <param name="raf"> from this file </param>
		/// <param name="r"> which datasets, relative to this message. null == all. </param>
		/// <param name="addTime"> add the time coordinate </param>
		/// <param name="f"> output bit count debugging info (may be null) </param>
		/// <returns> number of datasets read </returns>
		/// <exception cref="IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public int readData(ArrayStructureBB abb, Message m, NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Range r, boolean addTime, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
		public virtual int readData(ArrayStructureBB abb, Message m, RandomAccessFile raf, NcRange r, bool addTime, Formatter f)
		{
			BitReader reader = new BitReader(raf, m.dataSection.DataPos + 4);
			DataDescriptor root = m.RootDataDescriptor;
			if (root.isBad)
			{
				return 0;
			}

			Request req = new Request(abb, r);

			int n = m.NumberDatasets;
			m.counterDatasets = new BitCounterUncompressed[n]; // one for each dataset
			m.msg_nbits = 0;

			// loop over the rows
			int count = 0;
			for (int i = 0; i < n; i++)
			{
				if (f != null)
				{
					f.format("Count bits in observation %d%n", i);
				}
				// the top table always has exactly one "row", since we are working with a single obs
				m.counterDatasets[i] = new BitCounterUncompressed(root, 1, 0);
				DebugOut @out = (f == null) ? null : new DebugOut(f);

				req.setRow(i);
				if (req.wantRow() && addTime)
				{
					req.bb.putInt(0); // placeholder for time assumes an int
					count++;
				}

				readData(@out, reader, m.counterDatasets[i], root.subKeys, 0, req);
				m.msg_nbits += m.counterDatasets[i].countBits(m.msg_nbits);
			}

			return count;
		}

		private class Request
		{
			internal ArrayStructureBB abb;
			internal ByteBuffer bb;
			internal NcRange r;
			internal int row;

			internal Request(ArrayStructureBB abb, NcRange r)
			{
				this.abb = abb;
				if (abb != null)
				{
					bb = abb.ByteBuffer;
				}
				this.r = r;
				this.row = 0;
			}

			internal virtual Request setRow(int row)
			{
				this.row = row;
				return this;
			}

			internal virtual bool wantRow()
			{
				if (abb == null)
				{
					return false;
				}
				if (r == null)
				{
					return true;
				}
				return r.contains(row);
			}

		}

		/// <summary>
		/// count/read the bits in one row of a "nested table", defined by List<DataDescriptor> dkeys.
		/// </summary>
		/// <param name="out"> optional debug output, may be null </param>
		/// <param name="reader"> read data with this </param>
		/// <param name="dkeys"> the fields of the table </param>
		/// <param name="table"> put the results here </param>
		/// <param name="nestedRow"> which row of the table </param>
		/// <param name="req"> read data into here, may be null </param>
		/// <exception cref="IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void readData(DebugOut out, NetcdfSharp.ucar.nc2.iosp.BitReader reader, BitCounterUncompressed table, NetcdfSharp.java.util.List<DataDescriptor> dkeys, int nestedRow, Request req) throws NetcdfSharp.java.io.IOException
		private void readData(DebugOut @out, BitReader reader, BitCounterUncompressed table, IList<DataDescriptor> dkeys, int nestedRow, Request req)
		{

			foreach (DataDescriptor dkey in dkeys)
			{
				if (!dkey.OkForVariable)
				{ // misc skip
					if (@out != null)
					{
						@out.f.format("%s %d %s (%s) %n", @out.indent(), @out.fldno++, dkey.name, dkey.FxyName);
					}
					continue;
				}

				// sequence
				if (dkey.replication == 0)
				{

					// find out how many objects in the sequence
					int count = (int)reader.bits2UInt(dkey.replicationCountSize);
					if (@out != null)
					{
						@out.f.format("%4d delayed replication count=%d %n", @out.fldno++, count);
					}
					if ((@out != null) && (count > 0))
					{
						@out.f.format("%4d %s read sequence %s count= %d bitSize=%d start at=0x%x %n", @out.fldno, @out.indent(), dkey.FxyName, count, dkey.replicationCountSize, reader.Pos);
					}

					// read the data
					BitCounterUncompressed bitCounterNested = table.makeNested(dkey, count, nestedRow, dkey.replicationCountSize);
					ArraySequence seq = makeArraySequenceUncompressed(@out, reader, bitCounterNested, dkey, req);

					if (req.wantRow())
					{
						int index = req.abb.addObjectToHeap(seq);
						req.bb.putInt(index); // an index into the Heap
					}
					continue;
				}

				// compound
				if (dkey.type == 3)
				{
					BitCounterUncompressed nested = table.makeNested(dkey, dkey.replication, nestedRow, 0);
					if (@out != null)
					{
						@out.f.format("%4d %s read structure %s count= %d%n", @out.fldno, @out.indent(), dkey.FxyName, dkey.replication);
					}

					for (int i = 0; i < dkey.replication; i++)
					{
						if (@out != null)
						{
							@out.f.format("%s read row %d (struct %s) %n", @out.indent(), i, dkey.FxyName);
							@out.indent_Conflict.incr();
							readData(@out, reader, nested, dkey.subKeys, i, req);
							@out.indent_Conflict.decr();
						}
						else
						{
							readData(null, reader, nested, dkey.subKeys, i, req);
						}
					}
					continue;
				}

				// char data
				if (dkey.type == 1)
				{
					byte[] vals = readCharData(dkey, reader, req);
					if (@out != null)
					{
						string s = StringHelper.NewString(vals, Encoding.UTF8);
						@out.f.format("%4d %s read char %s (%s) width=%d end at= 0x%x val=<%s>%n", @out.fldno++, @out.indent(), dkey.FxyName, dkey.Name, dkey.bitWidth, reader.Pos, s);
					}
					continue;
				}

				// otherwise read a number
				long val = readNumericData(dkey, reader, req);
				if (@out != null)
				{
					@out.f.format("%4d %s read %s (%s %s) bitWidth=%d end at= 0x%x raw=%d convert=%f%n", @out.fldno++, @out.indent(), dkey.FxyName, dkey.Name, dkey.Units, dkey.bitWidth, reader.Pos, val, dkey.convert(val));
				}
			}

		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private byte[] readCharData(DataDescriptor dkey, NetcdfSharp.ucar.nc2.iosp.BitReader reader, Request req) throws NetcdfSharp.java.io.IOException
		private byte[] readCharData(DataDescriptor dkey, BitReader reader, Request req)
		{
			int nchars = dkey.ByteWidthCDM;
			byte[] b = new byte[nchars];
			for (int i = 0; i < nchars; i++)
			{
				b[i] = (byte)reader.bits2UInt(8);
			}

			if (req.wantRow())
			{
				for (int i = 0; i < nchars; i++)
				{
					req.bb.put(b[i]);
				}
			}
			return b;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private long readNumericData(DataDescriptor dkey, NetcdfSharp.ucar.nc2.iosp.BitReader reader, Request req) throws NetcdfSharp.java.io.IOException
		private long readNumericData(DataDescriptor dkey, BitReader reader, Request req)
		{
			// numeric data
			long result = reader.bits2UInt(dkey.bitWidth);

			if (req.wantRow())
			{

				// place into byte buffer
				if (dkey.ByteWidthCDM == 1)
				{
					req.bb.put((byte)result);

				}
				else if (dkey.ByteWidthCDM == 2)
				{
					byte b1 = unchecked((byte)(result & 0xff));
					byte b2 = (byte)((result & 0xff00) >> 8);
					req.bb.put(b2);
					req.bb.put(b1);

				}
				else if (dkey.ByteWidthCDM == 4)
				{
					byte b1 = unchecked((byte)(result & 0xff));
					byte b2 = (byte)((result & 0xff00) >> 8);
					byte b3 = (byte)((result & 0xff0000) >> 16);
					byte b4 = (byte)((result & 0xff000000) >> 24);
					req.bb.put(b4);
					req.bb.put(b3);
					req.bb.put(b2);
					req.bb.put(b1);

				}
				else
				{
					byte b1 = unchecked((byte)(result & 0xff));
					byte b2 = (byte)((result & 0xff00) >> 8);
					byte b3 = (byte)((result & 0xff0000) >> 16);
					byte b4 = (byte)((result & 0xff000000) >> 24);
					byte b5 = (byte)((result & 0xff00000000L) >> 32);
					byte b6 = (byte)((result & 0xff0000000000L) >> 40);
					byte b7 = (byte)((result & 0xff000000000000L) >> 48);
					byte b8 = (byte)(((ulong)result & 0xff00000000000000UL) >> 56);
					req.bb.put(b8);
					req.bb.put(b7);
					req.bb.put(b6);
					req.bb.put(b5);
					req.bb.put(b4);
					req.bb.put(b3);
					req.bb.put(b2);
					req.bb.put(b1);
				}
			}

			return result;
		}

		// read in the data into an ArrayStructureBB, wrapped by an ArraySequence
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private ArraySequence makeArraySequenceUncompressed(DebugOut out, NetcdfSharp.ucar.nc2.iosp.BitReader reader, BitCounterUncompressed bitCounterNested, DataDescriptor seqdd, Request req) throws NetcdfSharp.java.io.IOException
		private ArraySequence makeArraySequenceUncompressed(DebugOut @out, BitReader reader, BitCounterUncompressed bitCounterNested, DataDescriptor seqdd, Request req)
		{

			int count = bitCounterNested.NumberRows; // the actual number of rows in this sequence
			ArrayStructureBB abb = null;
			StructureMembers members = null;

			if (req.wantRow())
			{
				Sequence seq = seqdd.refersTo;
				Debug.Assert(seq != null);

				// for the obs structure
				int[] shape = new int[] { count };

				// allocate ArrayStructureBB for outer structure
				// LOOK why is this different from ArrayStructureBB.setOffsets() ?
				int offset = 0;
				members = seq.makeStructureMembers();
				foreach (StructureMembers.Member m in members.Members)
				{
					m.DataParam = offset;

					Variable mv = seq.findVariable(m.Name);
					BufrConfig.FieldConverter fld = (BufrConfig.FieldConverter)mv.SPobject;
					DataDescriptor dk = fld.dds;
					if (dk.replication == 0) // LOOK
					{
						offset += 4;
					}
					else
					{
						offset += dk.ByteWidthCDM;
					}

					if (m.StructureMembers != null)
					{
						ArrayStructureBB.setOffsets(m.StructureMembers);
					}
				}

				abb = new ArrayStructureBB(members, shape);
				ByteBuffer bb = abb.ByteBuffer;
				bb.order(ByteOrder.BIG_ENDIAN);
			}

			Request nreq = new Request(abb, null);

			// loop through nested obs
			for (int i = 0; i < count; i++)
			{
				if (@out != null)
				{
					@out.f.format("%s read row %d (seq %s) %n", @out.indent(), i, seqdd.FxyName);
					@out.indent_Conflict.incr();
					readData(@out, reader, bitCounterNested, seqdd.SubKeys, i, nreq);
					@out.indent_Conflict.decr();

				}
				else
				{
					readData(null, reader, bitCounterNested, seqdd.SubKeys, i, nreq);
				}
			}

			return abb != null ? new ArraySequence(members, abb.StructureDataIterator, count) : null;
		}
	}

}
