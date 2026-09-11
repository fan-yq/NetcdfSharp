// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp
{
#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Section = NetcdfSharp.ucar.ma2.Section;

	/// <summary>
	/// For datasets where the data are stored in chunks, and must be processed, eg compressed or filtered.
	/// The data is read, processed, and placed in a ByteBuffer. Chunks have an offset into the ByteBuffer.
	/// "Tiled" means that all chunks are assumed to be equal size.
	/// Chunks do not necessarily cover the array, missing data is possible.
	/// 
	/// @author caron
	/// @since Jan 9, 2008
	/// </summary>
	public class LayoutBBTiled : LayoutBB
	{
		private Section want;
		private int[] chunkSize; // all chunks assumed to be the same size
		private int elemSize;

		private DataChunkIterator chunkIterator; // iterate across chunks
		private IndexChunkerTiled index; // iterate within a chunk

		// track the overall iteration
		private long totalNelems, totalNelemsDone; // total number of elemens

		private const bool debug = false, debugIntersection = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="chunkIterator"> iterator over all data chunks </param>
		/// <param name="chunkSize"> all chunks assumed to be the same size </param>
		/// <param name="elemSize"> size of an element in bytes. </param>
		/// <param name="wantSection"> the wanted section of data, contains a List of Range objects. Must be complete. </param>
		public LayoutBBTiled(DataChunkIterator chunkIterator, int[] chunkSize, int elemSize, Section wantSection)
		{
			this.chunkIterator = chunkIterator;
			this.chunkSize = chunkSize;
			this.elemSize = elemSize;
			this.want = wantSection;
			if (debug)
			{
				Console.WriteLine(" want section=" + this.want);
			}

			this.totalNelems = this.want.computeSize();
			this.totalNelemsDone = 0;
		}

		public virtual long TotalNelems
		{
			get
			{
				return totalNelems;
			}
		}

		public virtual int ElemSize
		{
			get
			{
				return elemSize;
			}
		}

		// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private LayoutBBTiled.Chunk next_Conflict;

		public virtual bool hasNext()
		{ // have to actually fetch the thing
			if (totalNelemsDone >= totalNelems)
			{
				return false;
			}

			if ((index == null) || !index.hasNext())
			{ // get new data node
				try
				{
					Section dataSection;
					DataChunk dataChunk;

					while (true)
					{ // look for intersecting sections
						if (!chunkIterator.hasNext())
						{
							next_Conflict = null;
							return false;
						}

						// get next dataChunk
						try
						{
							dataChunk = chunkIterator.next();
						}
						catch (IOException e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
							next_Conflict = null;
							return false;
						}

						// make the dataSection for this chunk
						dataSection = new Section(dataChunk.Offset, chunkSize);
						if (debugIntersection)
						{
							Console.WriteLine(" test intersecting: " + dataSection + " want: " + want);
						}
						if (dataSection.intersects(want)) // does it intersect ?
						{
							break;
						}
					}

					if (debug)
					{
						Console.WriteLine(" found intersecting dataSection: " + dataSection + " intersect= " + dataSection.intersect(want));
					}

					index = new IndexChunkerTiled(dataSection, want); // new indexer into this chunk
					next_Conflict = new Chunk(dataChunk.ByteBuffer); // this does the uncompression

				}
				catch (Exception e) when (e is InvalidRangeException || e is IOException)
				{
					throw new System.InvalidOperationException(e.Message);
				}
			}

			IndexChunker.Chunk chunk = index.next();
			totalNelemsDone += chunk.Nelems;
			next_Conflict.Delegate = chunk;

			return true;
		}

		public virtual Layout.Chunk next()
		{
			return next_Conflict;
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
			sbuff.Append("] totalNelems=").Append(totalNelems);
			sbuff.Append(" elemSize=").Append(elemSize);
			return sbuff.ToString();
		}

		/// <summary>
		/// An iterator over the data chunks.
		/// </summary>
		public interface DataChunkIterator
		{
			bool hasNext();

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: DataChunk next() throws NetcdfSharp.java.io.IOException;
			DataChunk next();
		}

		/// <summary>
		/// A data chunk
		/// </summary>
		public interface DataChunk
		{
			int[] Offset { get; }

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: ByteBuffer getByteBuffer() throws NetcdfSharp.java.io.IOException;
			ByteBuffer ByteBuffer { get; }
		}

		/// <summary>
		/// A chunk of data that is contiguous in both the source and destination.
		/// Everything is done in elements, not bytes.
		/// Read nelems from src at srcPos, store in destination at destPos.
		/// </summary>
		private class Chunk : LayoutBB.ChunkBB
		{
			internal IndexChunker.Chunk @delegate;

			internal ByteBuffer bb;
			internal ShortBuffer sb;
			internal IntBuffer ib;
			internal LongBuffer longb;
			internal FloatBuffer fb;
			internal DoubleBuffer db;

			internal Chunk(ByteBuffer bb)
			{
				this.bb = bb;
			}

			public virtual IndexChunker.Chunk Delegate
			{
				set
				{
					this.@delegate = value;
				}
			}

			public virtual int SrcElem
			{
				get
				{
					return (int)@delegate.SrcElem;
				}
			}

			public virtual int Nelems
			{
				get
				{
					return @delegate.Nelems;
				}
			}

			public virtual long DestElem
			{
				get
				{
					return @delegate.DestElem;
				}
			}

			public virtual ByteBuffer ByteBuffer
			{
				get
				{
					return bb;
				}
			}

			public virtual ShortBuffer ShortBuffer
			{
				get
				{
					if (sb == null)
					{
						sb = bb.asShortBuffer();
					}
					return sb;
				}
			}

			public virtual IntBuffer IntBuffer
			{
				get
				{
					if (ib == null)
					{
						ib = bb.asIntBuffer();
					}
					return ib;
				}
			}

			public virtual LongBuffer LongBuffer
			{
				get
				{
					if (longb == null)
					{
						longb = bb.asLongBuffer();
					}
					return longb;
				}
			}

			public virtual FloatBuffer FloatBuffer
			{
				get
				{
					if (fb == null)
					{
						fb = bb.asFloatBuffer();
					}
					return fb;
				}
			}

			public virtual DoubleBuffer DoubleBuffer
			{
				get
				{
					if (db == null)
					{
						db = bb.asDoubleBuffer();
					}
					return db;
				}
			}

			public override string ToString()
			{
				return @delegate.ToString();
			}

			// artifact of overriding Layout
			public virtual long SrcPos
			{
				get
				{
					throw new System.NotSupportedException();
				}
			}

		}

	}
#pragma warning restore CS0162

}
