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
namespace NetcdfSharp.ucar.nc2.iosp
{
#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NcIndex = NetcdfSharp.ucar.ma2.NcIndex;
	using Section = NetcdfSharp.ucar.ma2.Section;

	/// <summary>
	/// LayoutSegmented has data stored in segments.
	/// Assume that each segment size is a multiple of elemSize.
	/// 
	/// @author caron
	/// @since Dec 31, 2007
	/// </summary>
	public class LayoutSegmented : Layout
	{
		private long total, done;
		private int elemSize; // size of each element

		private long[] segPos; // bytes
		private long[] segMax, segMin; // bytes

		// outer chunk
		private IndexChunker chunker;
		private IndexChunker.Chunk chunkOuter;

		// inner chunk = deal with segmentation
		private IndexChunker.Chunk chunkInner = new IndexChunker.Chunk(0, 0, 0);

		private const bool debugNext = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="segPos"> starting address of each segment. </param>
		/// <param name="segSize"> number of bytes in each segment. Assume multiple of elemSize </param>
		/// <param name="elemSize"> size of an element in bytes. </param>
		/// <param name="srcShape"> shape of the entire data array. </param>
		/// <param name="wantSection"> the wanted section of data </param>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if ranges are misformed </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public LayoutSegmented(long[] segPos, int[] segSize, int elemSize, int[] srcShape, NetcdfSharp.ucar.ma2.Section wantSection) throws NetcdfSharp.ucar.ma2.InvalidRangeException
		public LayoutSegmented(long[] segPos, int[] segSize, int elemSize, int[] srcShape, Section wantSection)
		{
			Debug.Assert(segPos.Length == segSize.Length);
			this.segPos = segPos;

			int nsegs = segPos.Length;
			segMin = new long[nsegs];
			segMax = new long[nsegs];
			long totalElems = 0;
			for (int i = 0; i < nsegs; i++)
			{
				Debug.Assert(segPos[i] >= 0);
				Debug.Assert(segSize[i] > 0);
				Debug.Assert((segSize[i] % elemSize) == 0);

				segMin[i] = totalElems;
				totalElems += segSize[i];
				segMax[i] = totalElems;
			}
			Debug.Assert(totalElems >= NcIndex.computeSize(srcShape) * elemSize);

			chunker = new IndexChunker(srcShape, wantSection);
			this.total = chunker.TotalNelems;
			this.done = 0;
			this.elemSize = elemSize;
		}

		public virtual long TotalNelems
		{
			get
			{
				return total;
			}
		}

		public virtual int ElemSize
		{
			get
			{
				return elemSize;
			}
		}

		public virtual bool hasNext()
		{
			return done < total;
		}

		///////////////////

		private long getFilePos(long elem)
		{
			int segno = 0;
			while (elem >= segMax[segno])
			{
				segno++;
			}
			return segPos[segno] + elem - segMin[segno];
		}

		// how many more bytes are in this segment ?
		private int getMaxBytes(long start)
		{
			int segno = 0;
			while (start >= segMax[segno])
			{
				segno++;
			}
			return (int)(segMax[segno] - start);
		}

		private int needInner;
		private int doneInner;

		public virtual Chunk next()
		{
			Chunk result;

			if (needInner > 0)
			{
				result = nextInner(false, 0);

			}
			else
			{
				result = nextOuter();
				int nbytes = getMaxBytes(chunkOuter.SrcElem * elemSize);
				if (nbytes < result.Nelems * elemSize)
				{
					result = nextInner(true, nbytes);
				}
			}

			done += result.Nelems;
			doneInner += result.Nelems;
			needInner -= result.Nelems;

			if (debugNext)
			{
				Console.WriteLine(" next chunk: " + result);
			}

			return result;
		}

		private Chunk nextInner(bool first, int nbytes)
		{
			if (first)
			{
				chunkInner.Nelems = nbytes / elemSize;
				chunkInner.DestElem = chunkOuter.DestElem;
				needInner = chunkOuter.Nelems;
				doneInner = 0;

			}
			else
			{
				chunkInner.incrDestElem(chunkInner.Nelems); // increment using last chunks' value
				nbytes = getMaxBytes((chunkOuter.SrcElem + doneInner) * elemSize);
				nbytes = Math.Min(nbytes, needInner * elemSize);
				chunkInner.Nelems = nbytes / elemSize; // set this chunk's value
			}

			chunkInner.SrcPos = getFilePos((chunkOuter.SrcElem + doneInner) * elemSize);
			return chunkInner;
		}

		public virtual Chunk nextOuter()
		{
			chunkOuter = chunker.next();
			long srcPos = getFilePos(chunkOuter.SrcElem * elemSize);
			chunkOuter.SrcPos = srcPos;
			return chunkOuter;
	}

#pragma warning restore CS0162

}


}
