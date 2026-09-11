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
namespace NetcdfSharp.ucar.nc2.iosp
{
#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NcRange = NetcdfSharp.ucar.ma2.NcRange;
	using Section = NetcdfSharp.ucar.ma2.Section;

	/// <summary>
	/// Iterator to read/write subsets of a multidimensional array, finding the contiguous chunks.
	/// The iteration is monotonic in both src and dest positions.
	/// 
	/// <p/>
	/// Example for Integers:
	/// 
	/// <pre>
	/// int[] read(IndexChunker index, int[] src) {
	///   int[] dest = new int[index.getTotalNelems()];
	///   while (index.hasNext()) {
	///     Indexer2.Chunk chunk = index.next();
	///     System.arraycopy(src, chunk.getSrcElem(), dest, chunk.getDestElem(), chunk.getNelems());
	///   }
	///   return dest;
	/// }
	/// 
	/// int[] read(IndexChunker index, RandomAccessFile raf, long start_pos) {
	///   int[] dest = new int[index.getTotalNelems()];
	///   while (index.hasNext()) {
	///     Indexer2.Chunk chunk = index.next();
	///     raf.seek(start_pos + chunk.getSrcElem() * 4);
	///     raf.readInt(dest, chunk.getDestElem(), chunk.getNelems());
	///   }
	///   return dest;
	/// }
	/// 
	/// // note src and dest misnamed
	/// void write(IndexChunker index, int[] src, RandomAccessFile raf, long start_pos) {
	///   while (index.hasNext()) {
	///     Indexer2.Chunk chunk = index.next();
	///     raf.seek(start_pos + chunk.getSrcElem() * 4);
	///     raf.writeInt(src, chunk.getDestElem(), chunk.getNelems());
	///   }
	/// }
	/// </pre>
	/// 
	/// @author caron
	/// @since Jan 2, 2008
	/// </summary>
	public class IndexChunker
	{
		private const bool debug = false, debugMerge = false, debugNext = false;

		private IList<Dim> dimList = new List<Dim>();
		private IndexLong chunkIndex; // each element is one chunk; strides track position in source

		private Chunk chunk; // gets returned on next().
		private int nelems; // number of elements to read at one time
		private long start, total, done;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="srcShape"> the shape of the source, eg Variable.getShape() </param>
		/// <param name="wantSection"> the wanted section in srcShape, ie must be subset of srcShape. </param>
		/// <exception cref="InvalidRangeException"> if wantSection is incorrect </exception>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public IndexChunker(int[] srcShape, @Nullable Section wantSection) throws NetcdfSharp.ucar.ma2.InvalidRangeException
		public IndexChunker(int[] srcShape, Section wantSection)
		{

			wantSection = Section.fill(wantSection, srcShape); // will throw InvalidRangeException if illegal section

			// compute total size of wanted section
			this.total = wantSection.computeSize();
			this.done = 0;
			this.start = 0;

			// see if this is a "want all of it" single chunk
			if (wantSection.equivalent(srcShape))
			{
				this.nelems = (int)this.total;
				chunkIndex = new IndexLong();
				return;
			}

			// create the List<Dim> tracking each dimension
			int varRank = srcShape.Length;
			long stride = 1;
			for (int ii = varRank - 1; ii >= 0; ii--)
			{
				dimList.Add(new Dim(stride, srcShape[ii], wantSection.getRange(ii))); // note reversed : fastest first
				stride *= srcShape[ii];
			}

			// merge contiguous inner dimensions for efficiency
			if (debugMerge)
			{
				Console.WriteLine("merge= " + this);
			}

			// count how many merge dimensions
			int merge = 0;
			for (int i = 0; i < dimList.Count - 1; i++)
			{
				Dim elem = dimList[i];
				Dim elem2 = dimList[i + 1];
				if (elem.maxSize == elem.wantSize && (elem2.want.stride() == 1))
				{
					merge++;
				}
				else
				{
					break;
				}
			}

			// merge the dimensions
			for (int i = 0; i < merge; i++)
			{
				Dim elem = dimList[i];
				Dim elem2 = dimList[i + 1];
				elem2.maxSize *= elem.maxSize;
				elem2.wantSize *= elem.wantSize;
				if (elem2.wantSize < 0)
				{
					throw new ArgumentException("array size may not exceed 2^31");
				}
				if (debugMerge)
				{
					Console.WriteLine(" ----" + this);
				}
			}

			// delete merged
			if (merge > 0)
			{
				for (int i = 0; i < merge; i++)
				{
					dimList.RemoveAt(0);
				}
			}
			if (debug)
			{
				Console.WriteLine(" final= " + this);
			}

			// how many elements can we do at a time?
			if ((varRank == 0) || (dimList[0].want.stride() > 1))
			{
				this.nelems = 1;
			}
			else
			{
				Dim innerDim = dimList[0];
				this.nelems = innerDim.wantSize;
				innerDim.wantSize = 1; // inner dimension has one element of length nelems (we dont actually need this here)
			}

			start = 0; // first wanted value
			foreach (Dim dim in dimList)
			{
				start += dim.stride * dim.want.first(); // watch for overflow on large files
			}

			// we will use an Index object to keep track of the chunks, each index represents nelems
			int rank = dimList.Count;
			long[] wstride = new long[rank];
			int[] shape = new int[rank];
			for (int i = 0; i < rank; i++)
			{
				Dim dim = dimList[i];
				wstride[rank - i - 1] = dim.stride * dim.want.stride(); // reverse to slowest first
				shape[rank - i - 1] = dim.wantSize;
			}
			if (debug)
			{
				Console.WriteLine("  elemsPerChunk=" + nelems + "  nchunks=" + IndexLong.computeSize(shape));
				printa("  indexShape=", shape);
				printl("  indexStride=", wstride);
			}
			chunkIndex = new IndexLong(shape, wstride);

			// sanity check
			Debug.Assert(IndexLong.computeSize(shape) * nelems == total);

			if (debug)
			{
				Console.WriteLine("Index2= " + this);
				Console.WriteLine(" start= " + start + " varShape= " + printa(srcShape) + " wantSection= " + wantSection);
			}
		}

		private class Dim
		{
			internal long stride; // number of elements
			internal long maxSize; // number of elements - must be a long since we may merge
			internal NcRange want; // desired Range
			internal int wantSize; // keep separate from want so we can modify when merging

			internal Dim(long byteStride, int maxSize, NcRange want)
			{
				this.stride = byteStride;
				this.maxSize = maxSize;
				this.wantSize = want.length();
				this.want = want;
			}
		}

		/// <summary>
		/// Get total number of elements in wantSection
		/// </summary>
		/// <returns> total number of elements in wantSection </returns>
		public virtual long TotalNelems
		{
			get
			{
				return total;
			}
		}

		/// <summary>
		/// If there are more chunks to process
		/// </summary>
		/// <returns> true if there are more chunks to process </returns>
		public virtual bool hasNext()
		{
			return done < total;
		}

		/// <summary>
		/// Get the next chunk
		/// </summary>
		/// <returns> the next chunk </returns>
		public virtual Chunk next()
		{
			if (chunk == null)
			{
				chunk = new Chunk(start, nelems, 0);
			}
			else
			{
				chunkIndex.incr(); // increment one element, which represents one chunk = nelems * sizeElem
				chunk.incrDestElem(nelems); // always read nelems at a time
			}

			// Get the current element's index from the start of the file
			chunk.SrcElem = start + chunkIndex.currentElement();

			if (debugNext)
			{
				Console.WriteLine(" next chunk: " + chunk);
			}

			done += nelems;
			return chunk;
		}


		/// <summary>
		/// A chunk of data that is contiguous in both the source and destination.
		/// Everything is done in elements, not bytes.
		/// Read nelems from src at srcPos, store in destination at destPos.
		/// </summary>
		public class Chunk : Layout.Chunk
		{
			internal long srcElem; // start reading/writing here in the file
			internal int nelems; // read these many contiguous elements
			internal long destElem; // start writing/reading here in array
			internal long srcPos;

			public Chunk(long srcElem, int nelems, long destElem)
			{
				this.srcElem = srcElem;
				this.nelems = nelems;
				this.destElem = destElem;
			}

			/// <summary>
			/// Get the position in source where to read or write
			/// </summary>
			/// <returns> position as an element count </returns>
			public virtual long SrcElem
			{
				get
				{
					return srcElem;
				}
				set
				{
					this.srcElem = value;
				}
			}


			public virtual void incrSrcElem(int incr)
			{
				this.srcElem += incr;
			}

			/// <returns> number of elements to transfer contiguously (Note: elements, not bytes) </returns>
			public virtual int Nelems
			{
				get
				{
					return nelems;
				}
				set
				{
					this.nelems = value;
				}
			}


			/// <summary>
			/// Get the position in destination where to read or write
			/// </summary>
			/// <returns> starting element in the array: "starting array element" (Note: elements, not bytes) </returns>
			public virtual long DestElem
			{
				get
				{
					return destElem;
				}
				set
				{
					this.destElem = value;
				}
			}


			public virtual void incrDestElem(int incr)
			{
				this.destElem += incr;
			}

			public override string ToString()
			{
				return " srcPos=" + srcPos + " srcElem=" + srcElem + " nelems=" + nelems + " destElem=" + destElem;
			}

			// must be set by controlling Layout class - not used here
			public virtual long SrcPos
			{
				get
				{
					return srcPos;
				}
				set
				{
					this.srcPos = value;
				}
			}


			public virtual void incrSrcPos(int incr)
			{
				this.srcPos += incr;
			}
		}

		public override string ToString()
		{
			StringBuilder sbuff = new StringBuilder();
			sbuff.Append("wantSize=");
			for (int i = 0; i < dimList.Count; i++)
			{
				Dim elem = dimList[i];
				if (i > 0)
				{
					sbuff.Append(",");
				}
				sbuff.Append(elem.wantSize);
			}
			sbuff.Append(" maxSize=");
			for (int i = 0; i < dimList.Count; i++)
			{
				Dim elem = dimList[i];
				if (i > 0)
				{
					sbuff.Append(",");
				}
				sbuff.Append(elem.maxSize);
			}
			sbuff.Append(" wantStride=");
			for (int i = 0; i < dimList.Count; i++)
			{
				Dim elem = dimList[i];
				if (i > 0)
				{
					sbuff.Append(",");
				}
				sbuff.Append(elem.want.stride());
			}
			sbuff.Append(" stride=");
			for (int i = 0; i < dimList.Count; i++)
			{
				Dim elem = dimList[i];
				if (i > 0)
				{
					sbuff.Append(",");
				}
				sbuff.Append(elem.stride);
			}
			return sbuff.ToString();
		}

		// debugging
		protected internal static string printa(int[] a)
		{
			StringBuilder sbuff = new StringBuilder();
			foreach (int anA in a)
			{
				sbuff.Append(anA).Append(" ");
			}
			return sbuff.ToString();
		}

		protected internal static void printa(string name, int[] a)
		{
			Console.Write(name + "= ");
			foreach (int anA in a)
			{
				Console.Write(anA + " ");
			}
			Console.WriteLine();
		}

		protected internal static void printl(string name, long[] a)
		{
			Console.Write(name + "= ");
			foreach (long anA in a)
			{
				Console.Write(anA + " ");
			}
			Console.WriteLine();
		}

	}
#pragma warning restore CS0162

}
