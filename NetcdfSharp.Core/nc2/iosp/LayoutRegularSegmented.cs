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
	/// LayoutRegularSegmented has data stored in segments that are regularly spaced.
	/// This is now Netcdf-3 "record variables" are laid out.
	/// 
	/// @author caron
	/// @since Dec 31, 2007
	/// </summary>
	public class LayoutRegularSegmented : Layout
	{
		private long total, done, innerNelems;
		private long startPos;
		private long recSize;
		private int elemSize;

		// outer chunk
		private IndexChunker chunker;
		private IndexChunker.Chunk chunkOuter;

		// inner chunk = deal with segmentation
		private IndexChunker.Chunk chunkInner = new IndexChunker.Chunk(0, 0, 0);

		private const bool debugNext = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="startPos"> starting address of the entire data array. </param>
		/// <param name="elemSize"> size of an element in bytes. </param>
		/// <param name="recSize"> size of outer stride in bytes </param>
		/// <param name="srcShape"> shape of the entire data array. must have rank > 0 </param>
		/// <param name="wantSection"> the wanted section of data </param>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if ranges are misformed </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public LayoutRegularSegmented(long startPos, int elemSize, long recSize, int[] srcShape, NetcdfSharp.ucar.ma2.Section wantSection) throws NetcdfSharp.ucar.ma2.InvalidRangeException
		public LayoutRegularSegmented(long startPos, int elemSize, long recSize, int[] srcShape, Section wantSection)
		{
			Debug.Assert(startPos > 0);
			Debug.Assert(elemSize > 0);
			Debug.Assert(recSize > 0);
			Debug.Assert(srcShape.Length > 0);

			this.startPos = startPos;
			this.elemSize = elemSize;
			this.recSize = recSize;

			chunker = new IndexChunker(srcShape, wantSection);
			this.total = chunker.TotalNelems;
			this.innerNelems = (srcShape[0] == 0) ? 0 : NcIndex.computeSize(srcShape) / srcShape[0];
			this.done = 0;
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
			long segno = elem / innerNelems;
			long offset = elem % innerNelems;
			return startPos + segno * recSize + offset * elemSize;
		}

		// how many more elements are in this segment ?
		private int getMaxElem(long startElem)
		{
			return (int)(innerNelems - startElem % innerNelems);
		}

		private int needInner;
		private int doneInner;

		public virtual Chunk next()
		{
			IndexChunker.Chunk result;

			if (needInner > 0)
			{
				result = nextInner(false, 0);

			}
			else
			{
				result = nextOuter();
				int nelems = getMaxElem(result.SrcElem);
				if (nelems < result.Nelems)
				{
					result = nextInner(true, nelems);
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

		private IndexChunker.Chunk nextInner(bool first, int nelems)
		{
			if (first)
			{
				chunkInner.Nelems = nelems;
				chunkInner.DestElem = chunkOuter.DestElem;
				needInner = chunkOuter.Nelems;
				doneInner = 0;

			}
			else
			{
				chunkInner.incrDestElem(chunkInner.Nelems); // increment using last chunks' value
				nelems = getMaxElem(chunkOuter.SrcElem + doneInner);
				nelems = Math.Min(nelems, needInner);
				chunkInner.Nelems = nelems; // set this chunk's value
			}

			chunkInner.SrcElem = chunkOuter.SrcElem + doneInner;
			chunkInner.SrcPos = getFilePos(chunkOuter.SrcElem + doneInner);
			return chunkInner;
		}

		public virtual IndexChunker.Chunk nextOuter()
		{
			chunkOuter = chunker.next();
			chunkOuter.SrcPos = getFilePos(chunkOuter.SrcElem);
			return chunkOuter;
		}

#pragma warning restore CS0162

}


}
