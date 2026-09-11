// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
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

	/// <summary>
	/// Reads through the data of a message.
	/// Can count bits / transfer all or some data to an Array.
	/// 
	/// @author caron
	/// @since Nov 15, 2009
	/// </summary>

	/*
	 * Within one message there are n obs (datasets) and s fields in each dataset.
	 * For compressed datasets, storage order is data(fld, obs) (obs varying fastest) :
	 * 
	 * Ro1, NBINC1, I11, I12, . . . I1n
	 * Ro2, NBINC2, I21, I22, . . . I2n
	 * ...
	 * Ros, NBINCs, Is1, Is2, . . . Isn
	 * 
	 * where Ro1, Ro2, . . . Ros are local reference values (number of bits as Table B) for field i.
	 * NBINC1 . . . NBINCs contain, as 6-bit quantities, the number of bits occupied by the increments that follow.
	 * If NBINC1 = 0, all values of element I are equal to Ro1; in such cases, the increments shall be omitted.
	 * For character data, NBINC shall contain the number of octets occupied by the character element.
	 * However, if the character data in all subsets are identical NBINC=0.
	 * Iij is the increment for the ith field and the jth obs.
	 * 
	 * A replicated field (structure) takes a group of fields and replicates them.
	 * Let C be the entire compressed block for the ith field, as above.
	 * 
	 * Ci = Roi, NBINCi, Ii1, Ii2, . . . Iin
	 * 
	 * data:
	 * 
	 * C1, (C2, C3)*r, ... Cs
	 * 
	 * where r is set in the data descriptor, and is the same for all datasets.
	 * 
	 * A delayed replicated field (sequence) takes a group of fields and replicates them, with the number of replications
	 * in the data :
	 * 
	 * C1, dr, 6bits, (C2, C3)*dr, ... Cs
	 * 
	 * where the width (nbits) of dr is set in the data descriptor. This dr must be the same for each dataset in the
	 * message.
	 * For some reason there is an extra 6 bits after the dr. My guess its a programming mistake that is now needed.
	 * There is no description of this case in the spec or the guide.
	 * 
	 * 
	 * --------------------------
	 * 
	 * We use an ArrayStructureMA to hold the data, and fill it sequentially as we scan the message.
	 * Each field is held in an Array stored in the member.getDataArray().
	 * An iterator is stored in member.getDataObject() which keeps track of where we are.
	 * For fixed length nested Structures, we need fld(dataset, inner) but we have fld(inner, dataset) se we transpose the
	 * dimensions
	 * before we set the iterator.
	 * For Sequences, inner.length is the same for all datasets in the message. However, it may vary across messages.
	 * However, we
	 * only iterate over the inner sequence, never across all messages. So the implementation can be specific to the
	 * meassage.
	 * 
	 */

	public class MessageCompressedDataReader
	{

		/// <summary>
		/// Read all datasets from a single message
		/// </summary>
		/// <param name="s"> outer variables </param>
		/// <param name="proto"> prototype message, has been processed </param>
		/// <param name="m"> read this message </param>
		/// <param name="raf"> from this file </param>
		/// <param name="f"> output bit count debugging info (may be null) </param>
		/// <returns> ArrayStructure with all the data from the message in it. </returns>
		/// <exception cref="IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public ArrayStructure readEntireMessage(NetcdfSharp.ucar.nc2.Structure s, Message proto, Message m, NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
		public virtual ArrayStructure readEntireMessage(Structure s, Message proto, Message m, RandomAccessFile raf, Formatter f)
		{
			// transfer info (refersTo, name) from the proto message
			DataDescriptor.transferInfo(proto.RootDataDescriptor.SubKeys, m.RootDataDescriptor.SubKeys);

			// allocate ArrayStructureMA for outer structure
			int n = m.NumberDatasets;
			ArrayStructureMA ama = ArrayStructureMA.factoryMA(s, new int[] { n });
			Iterators = ama;

			// map dkey to Member recursively
			Dictionary<DataDescriptor, StructureMembers.Member> map = new Dictionary<DataDescriptor, StructureMembers.Member>(100);
			associateMessage2Members(ama.StructureMembers, m.RootDataDescriptor, map);

			readData(m, raf, f, new Request(ama, map, null));

			return ama;
		}

		/// <summary>
		/// Read some or all datasets from a single message
		/// </summary>
		/// <param name="ama"> place data into here in order (may be null). iterators must be already set. </param>
		/// <param name="m"> read this message </param>
		/// <param name="raf"> from this file </param>
		/// <param name="r"> which datasets, relative to this message. null == all. </param>
		/// <param name="f"> output bit count debugging info (may be null) </param>
		/// <exception cref="IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public void readData(ArrayStructureMA ama, Message m, NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Range r, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
		public virtual void readData(ArrayStructureMA ama, Message m, RandomAccessFile raf, NcRange r, Formatter f)
		{
			// map dkey to Member recursively
			Dictionary<DataDescriptor, StructureMembers.Member> map = null;
			if (ama != null)
			{
				map = new Dictionary<DataDescriptor, StructureMembers.Member>(2 * ama.Members.Count);
				associateMessage2Members(ama.StructureMembers, m.RootDataDescriptor, map);
			}

			readData(m, raf, f, new Request(ama, map, r));
		}

		// manage the request
		private class Request
		{
			internal ArrayStructureMA ama; // data goes here, may be null
			internal Dictionary<DataDescriptor, StructureMembers.Member> map; // map of DataDescriptor to members of ama, may be null
			internal NcRange r; // requested range
			internal DpiTracker dpiTracker; // may be null
			internal int outerRow; // if inner process needs to know what row its on

			internal Request(ArrayStructureMA ama, Dictionary<DataDescriptor, StructureMembers.Member> map, NcRange r)
			{
				this.ama = ama;
				this.map = map;
				this.r = r;
			}

			internal virtual bool wantRow(int row)
			{
				if (ama == null)
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

		// An iterator is stored in member.getDataObject() which keeps track of where we are.
		// For fixed length nested Structures, we need fld(dataset, inner1, inner2, ...) but we have fld(inner1, inner2, ... ,
		// dataset)
		// so we permute the dimensions
		// before we set the iterator.
		public static ArrayStructureMA Iterators
		{
			set
			{
				StructureMembers sms = value.StructureMembers;
				foreach (StructureMembers.Member sm in sms.Members)
				{
					NcArray data = sm.DataArray;
					if (data is ArrayStructureMA)
					{
						Iterators = (ArrayStructureMA)data;

					}
					else
					{
						int[] shape = data.Shape;
						if ((shape.Length > 1) && (sm.DataType != DataType.CHAR))
						{
							NcArray datap;
							if (shape.Length == 2)
							{
								datap = data.transpose(0, 1);
							}
							else
							{
								int[] pdims = new int[shape.Length]; // (0,1,2,3...) -> (1,2,3...,0)
								for (int i = 0; i < shape.Length - 1; i++)
								{
									pdims[i] = i + 1;
								}
								datap = data.permute(pdims);
							}
							sm.DataObject = datap.IndexIterator;
						}
						else
						{
							sm.DataObject = data.IndexIterator;
						}
					}
				}
			}
		}

		private void associateMessage2Members(StructureMembers members, DataDescriptor parent, Dictionary<DataDescriptor, StructureMembers.Member> map)
		{
			foreach (DataDescriptor dkey in parent.SubKeys)
			{
				if (string.ReferenceEquals(dkey.name, null))
				{
					if (dkey.SubKeys != null)
					{
						associateMessage2Members(members, dkey, map);
					}
					continue;
				}
				StructureMembers.Member m = members.findMember(dkey.name);
				if (m != null)
				{
					map[dkey] = m;

					if (m.DataType == DataType.STRUCTURE)
					{
						ArrayStructure nested = (ArrayStructure)m.DataArray;
						if (dkey.SubKeys != null)
						{
							associateMessage2Members(nested.StructureMembers, dkey, map);
						}
					}

				}
				else
				{
					if (dkey.SubKeys != null)
					{
						associateMessage2Members(members, dkey, map);
					}
				}
			}
		}

		// read / count the bits in a compressed message
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private int readData(Message m, NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.java.util.Formatter f, Request req) throws NetcdfSharp.java.io.IOException
		private int readData(Message m, RandomAccessFile raf, Formatter f, Request req)
		{

			BitReader reader = new BitReader(raf, m.dataSection.DataPos + 4);
			DataDescriptor root = m.RootDataDescriptor;
			if (root.isBad)
			{
				return 0;
			}

			DebugOut @out = (f == null) ? null : new DebugOut(f);
			BitCounterCompressed[] counterFlds = new BitCounterCompressed[root.subKeys.Count]; // one for each field LOOK why
																							   // not m.counterFlds ?
			readData(@out, reader, counterFlds, root, 0, m.NumberDatasets, req);

			m.msg_nbits = 0;
			foreach (BitCounterCompressed counter in counterFlds)
			{
				if (counter != null)
				{
					m.msg_nbits += counter.TotalBits;
				}
			}
			return m.msg_nbits;
		}

		/// 
		/// <param name="out"> debug info; may be null </param>
		/// <param name="reader"> raf wrapper for bit reading </param>
		/// <param name="fldCounters"> one for each field </param>
		/// <param name="parent"> parent.subkeys() holds the fields </param>
		/// <param name="bitOffset"> bit offset from beginning of data </param>
		/// <param name="ndatasets"> number of compressed datasets </param>
		/// <param name="req"> for writing into the ArrayStructure; </param>
		/// <returns> bitOffset </returns>
		/// <exception cref="IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private int readData(DebugOut out, NetcdfSharp.ucar.nc2.iosp.BitReader reader, BitCounterCompressed[] fldCounters, DataDescriptor parent, int bitOffset, int ndatasets, Request req) throws NetcdfSharp.java.io.IOException
		private int readData(DebugOut @out, BitReader reader, BitCounterCompressed[] fldCounters, DataDescriptor parent, int bitOffset, int ndatasets, Request req)
		{

			IList<DataDescriptor> flds = parent.SubKeys;
			for (int fldidx = 0; fldidx < flds.Count; fldidx++)
			{
				DataDescriptor dkey = flds[fldidx];
				if (!dkey.OkForVariable)
				{ // dds with no data to read

					// the dpi nightmare
					if ((dkey.f == 2) && (dkey.x == 36))
					{
						req.dpiTracker = new DpiTracker(dkey.dpi, dkey.dpi.Nfields);
					}

					if (@out != null)
					{
						@out.f.format("%s %d %s (%s) %n", @out.indent(), @out.fldno++, dkey.name, dkey.FxyName);
					}
					continue;
				}

				BitCounterCompressed counter = new BitCounterCompressed(dkey, ndatasets, bitOffset);
				fldCounters[fldidx] = counter;

				// sequence
				if (dkey.replication == 0)
				{
					reader.BitOffset = bitOffset;
					int count = (int)reader.bits2UInt(dkey.replicationCountSize);
					bitOffset += dkey.replicationCountSize;

					reader.bits2UInt(6);
					if (null != @out)
					{
						@out.f.format("%s--sequence %s bitOffset=%d replication=%s %n", @out.indent(), dkey.FxyName, bitOffset, count);
					}
					bitOffset += 6; // LOOK seems to be an extra 6 bits.

					counter.addNestedCounters(count);

					// make an ArrayObject of ArraySequence, place it into the data array
					bitOffset = makeArraySequenceCompressed(@out, reader, counter, dkey, bitOffset, ndatasets, count, req);
					// if (null != out) out.f.format("--back %s %d %n", dkey.getFxyName(), bitOffset);
					continue;
				}

				// structure
				if (dkey.type == 3)
				{
					if (null != @out)
					{
						@out.f.format("%s--structure %s bitOffset=%d replication=%s %n", @out.indent(), dkey.FxyName, bitOffset, dkey.replication);
					}

					// p 11 of "standard", doesnt really describe the case of replication AND compression
					counter.addNestedCounters(dkey.replication);
					for (int i = 0; i < dkey.replication; i++)
					{
						BitCounterCompressed[] nested = counter.getNestedCounters(i);
						req.outerRow = i;
						if (null != @out)
						{
							@out.f.format("%n");
							@out.indent_Conflict.incr();
							bitOffset = readData(@out, reader, nested, dkey, bitOffset, ndatasets, req);
							@out.indent_Conflict.decr();
						}
						else
						{
							bitOffset = readData(null, reader, nested, dkey, bitOffset, ndatasets, req);
						}
					}
					// if (null != out) out.f.format("--back %s %d %n", dkey.getFxyName(), bitOffset);

					continue;
				}

				// all other fields

				StructureMembers.Member member;
				IndexIterator iter = null;
				ArrayStructure dataDpi = null; // if iter is missing - for the dpi case
				if (req.map != null)
				{
					member = req.map[dkey];
					iter = (IndexIterator)member.DataObject;
					if (iter == null)
					{
						dataDpi = (ArrayStructure)member.DataArray;
					}
				}

				reader.BitOffset = bitOffset; // ?? needed ??

				// char data special case
				if (dkey.type == 1)
				{
					int nc = dkey.bitWidth / 8;
					byte[] minValue = new byte[nc];
					for (int i = 0; i < nc; i++)
					{
						minValue[i] = (byte)reader.bits2UInt(8);
					}
					int dataWidthC = (int)reader.bits2UInt(6); // incremental data width in bytes
					counter.DataWidth = 8 * dataWidthC;
					int totalWidthC = dkey.bitWidth + 6 + 8 * dataWidthC * ndatasets; // total width in bits for this compressed set
																					// of values
					bitOffset += totalWidthC; // bitOffset now points to the next field

					if (null != @out)
					{
						@out.f.format("%s read %d %s (%s) bitWidth=%d defValue=%s dataWidth=%d n=%d bitOffset=%d %n", @out.indent(), @out.fldno++, dkey.name, dkey.FxyName, dkey.bitWidth, StringHelper.NewString(minValue, Encoding.UTF8), dataWidthC, ndatasets, bitOffset);
					}

					if (iter != null)
					{
						for (int dataset = 0; dataset < ndatasets; dataset++)
						{
							if (dataWidthC == 0)
							{ // use the min value
								if (req.wantRow(dataset))
								{
									for (int i = 0; i < nc; i++)
									{
										iter.CharNext = (char)minValue[i]; // ??
									}
								}

							}
							else
							{ // read the incremental value
								int nt = Math.Min(nc, dataWidthC);
								byte[] incValue = new byte[nc];
								for (int i = 0; i < nt; i++)
								{
									incValue[i] = (byte)reader.bits2UInt(8);
								}
								for (int i = nt; i < nc; i++) // can dataWidth < n ?
								{
									incValue[i] = 0;
								}
								if (req.wantRow(dataset))
								{
									for (int i = 0; i < nc; i++)
									{
										int cval = incValue[i];
										if (cval < 32 || cval > 126)
										{
											cval = 0; // printable ascii KLUDGE!
										}
										iter.CharNext = (char)cval; // ??
									}
								}
								if (@out != null)
								{
									@out.f.format(" %s,", StringHelper.NewString(incValue, Encoding.UTF8));
								}
							}
						}
					}
					if (@out != null)
					{
						@out.f.format("%n");
					}
					continue;
				}

				// numeric fields
				int useBitWidth = dkey.bitWidth;

				// a dpi Field needs to be substituted
				bool isDpi = ((dkey.f == 0) && (dkey.x == 31) && (dkey.y == 31));
				bool isDpiField = false;
				if ((dkey.f == 2) && (dkey.x == 24) && (dkey.y == 255))
				{
					isDpiField = true;
					DataDescriptor dpiDD = req.dpiTracker.getDpiDD(req.outerRow);
					useBitWidth = dpiDD.bitWidth;
				}

				long dataMin = reader.bits2UInt(useBitWidth);
				int dataWidth = (int)reader.bits2UInt(6); // increment data width - always in 6 bits, so max is 2^6 = 64
				if (dataWidth > useBitWidth && (null != @out))
				{
					@out.f.format(" BAD WIDTH ");
				}
				if (dkey.type == 1)
				{
					dataWidth *= 8; // char data count is in bytes
				}
				counter.DataWidth = dataWidth;

				int totalWidth = useBitWidth + 6 + dataWidth * ndatasets; // total width in bits for this compressed set of values
				bitOffset += totalWidth; // bitOffset now points to the next field

				if (null != @out)
				{
					@out.f.format("%s read %d, %s (%s) bitWidth=%d dataMin=%d (%f) dataWidth=%d n=%d bitOffset=%d %n", @out.indent(), @out.fldno++, dkey.name, dkey.FxyName, useBitWidth, dataMin, dkey.convert(dataMin), dataWidth, ndatasets, bitOffset);
				}

				// numeric fields

				// if dataWidth == 0, just use min value, otherwise read the compressed value here
				for (int dataset = 0; dataset < ndatasets; dataset++)
				{
					long value = dataMin;

					if (dataWidth > 0)
					{
						long cv = reader.bits2UInt(dataWidth);
						if (BufrNumbers.isMissing(cv, dataWidth))
						{
							value = BufrNumbers.missingValue(useBitWidth); // set to missing value
						}
						else // add to minimum
						{
							value += cv;
						}
					}

					// workaround for malformed messages
					if (dataWidth > useBitWidth)
					{
						long missingVal = BufrNumbers.missingValue(useBitWidth);
						if ((value & missingVal) != value) // overflow
						{
							value = missingVal; // replace with missing value
						}
					}

					if (req.wantRow(dataset))
					{
						if (isDpiField)
						{
							if (dataDpi != null)
							{
								DataDescriptor dpiDD = req.dpiTracker.getDpiDD(req.outerRow);
								StructureMembers sms = dataDpi.StructureMembers;
								StructureMembers.Member m0 = sms.getMember(0);
								IndexIterator iter2 = (IndexIterator)m0.DataObject;
								iter2.ObjectNext = dpiDD.Name;

								StructureMembers.Member m1 = sms.getMember(1);
								iter2 = (IndexIterator)m1.DataObject;
								iter2.FloatNext = dpiDD.convert(value);
							}
						}
						else if (iter != null)
						{
							iter.LongNext = value;
						}
					}
					// since dpi must be the same for all datasets, just keep the first one
					if (isDpi && (dataset == 0))
					{
						req.dpiTracker.setDpiValue(req.outerRow, value); // keep track of dpi values in the tracker - perhaps not
					}
					// expose

					if ((@out != null) && (dataWidth > 0))
					{
						@out.f.format(" %d (%f)", value, dkey.convert(value));
					}
				}
				if (@out != null)
				{
					@out.f.format("%n");
				}
			}

			return bitOffset;
		}

		// read in the data into an ArrayStructureMA, holding an ArrayObject() of ArraySequence
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private int makeArraySequenceCompressed(DebugOut out, NetcdfSharp.ucar.nc2.iosp.BitReader reader, BitCounterCompressed bitCounterNested, DataDescriptor seqdd, int bitOffset, int ndatasets, int count, Request req) throws NetcdfSharp.java.io.IOException
		private int makeArraySequenceCompressed(DebugOut @out, BitReader reader, BitCounterCompressed bitCounterNested, DataDescriptor seqdd, int bitOffset, int ndatasets, int count, Request req)
		{

			// construct ArrayStructureMA and associated map
			ArrayStructureMA ama = null;
			StructureMembers members = null;
			Dictionary<DataDescriptor, StructureMembers.Member> nmap = null;
			if (req.map != null)
			{
				Sequence seq = (Sequence)seqdd.refersTo;
				int[] shape = new int[] { ndatasets, count }; // seems unlikely this can handle recursion
				ama = ArrayStructureMA.factoryMA(seq, shape);
				Iterators = ama;

				members = ama.StructureMembers;
				nmap = new Dictionary<DataDescriptor, StructureMembers.Member>(2 * members.Members.size());
				associateMessage2Members(members, seqdd, nmap);
			}
			Request nreq = new Request(ama, nmap, req.r);

			// iterate over the number of replications, reading ndataset compressed values at each iteration
			if (@out != null)
			{
				@out.indent_Conflict.incr();
			}
			for (int i = 0; i < count; i++)
			{
				BitCounterCompressed[] nested = bitCounterNested.getNestedCounters(i);
				nreq.outerRow = i;
				bitOffset = readData(@out, reader, nested, seqdd, bitOffset, ndatasets, nreq);
			}
			if (@out != null)
			{
				@out.indent_Conflict.decr();
			}

			// add ArraySequence to the ArrayObject in the outer structure
			if (req.map != null)
			{
				StructureMembers.Member m = req.map[seqdd];
				ArrayObject arrObj = (ArrayObject)m.DataArray;

				// we need to break ama into separate sequences, one for each dataset
				int start = 0;
				for (int i = 0; i < ndatasets; i++)
				{
					ArraySequence arrSeq = new ArraySequence(members, new SequenceIterator(start, count, ama), count);
					arrObj.setObject(i, arrSeq);
					start += count;
				}
			}

			return bitOffset;
		}

		private class DpiTracker
		{
			internal DataDescriptorTreeConstructor.DataPresentIndicator dpi;
			internal bool[] isPresent;
			internal IList<DataDescriptor> dpiDD;

			internal DpiTracker(DataDescriptorTreeConstructor.DataPresentIndicator dpi, int nPresentFlags)
			{
				this.dpi = dpi;
				isPresent = new bool[nPresentFlags];
			}

			internal virtual void setDpiValue(int fldidx, long value)
			{
				isPresent[fldidx] = (value == 0); // present if the value is zero
			}

			internal virtual DataDescriptor getDpiDD(int fldPresentIndex)
			{
				if (dpiDD == null)
				{
					dpiDD = new List<DataDescriptor>();
					for (int i = 0; i < isPresent.Length; i++)
					{
						if (isPresent[i])
						{
							dpiDD.Add(dpi.linear[i]);
						}
					}
				}
				return dpiDD[fldPresentIndex];
			}

			internal virtual bool isDpiDDs(DataDescriptor dkey)
			{
				return (dkey.f == 2) && (dkey.x == 24) && (dkey.y == 255);
			}

			internal virtual bool isDpiField(DataDescriptor dkey)
			{
				return (dkey.f == 2) && (dkey.x == 24) && (dkey.y == 255);
			}
		}
	}

}
