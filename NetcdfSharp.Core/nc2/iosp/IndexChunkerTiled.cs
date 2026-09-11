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
	using NcIndex = NetcdfSharp.ucar.ma2.NcIndex;
	using NcRange = NetcdfSharp.ucar.ma2.NcRange;
	using Section = NetcdfSharp.ucar.ma2.Section;


	/// <summary>
	/// Assume that the data is stored divided into sections, described by dataSection. All the data within a dataSection is
	/// stored contiguously, in a regular layout. Assume dataSection strides must be = 1, that is, the stored data is not
	/// strided.
	/// <p/>
	/// The user asks for some section, wantSection (may have strides).
	/// For each dataSection that intersects wantSection, a IndexChunkerTiled is created, which
	/// figures out the optimal access pattern, based on reading contiguous runs of data. Each
	/// IndexChunkerTiled handles only one dataSection. Typically the cllaing program loops over
	/// all dataSections that intersect the wanted section.
	/// <p/>
	/// Both dataSection and wantSection refer to the variable's overall shape.
	/// 
	/// @author caron
	/// @since Jan 9, 2008
	/// </summary>
	public class IndexChunkerTiled
	{
		private IList<Dim> dimList = new List<Dim>();
		private IndexLong dataIndex; // Index into the data source section - used to calculate chunk.filePos
		private NcIndex resultIndex; // Index into the data result section - used to calculate chunk.startElem

		private IndexChunker.Chunk chunk; // gets returned on next().
		private int nelems; // number of elements to read at one time
		private long total, done;
		private int startDestElem; // the offset in the result Array of this piece of it
		private int startSrcElem; // the offset in the source Array of this piece of it

		private const bool debug = false, debugMerge = false, debugDetail = false, debugNext = false, debugStartingElems = false;

		/// <summary>
		/// Constructor.
		/// Assume varSection.intersects(wantSection).
		/// </summary>
		/// <param name="dataSection"> the section of data we actually have. must have all ranges with stride = 1. </param>
		/// <param name="wantSection"> the wanted section of data, it will be intersected with dataSection.
		///        dataSection.intersects(wantSection) must be true </param>
		/// <exception cref="InvalidRangeException"> if ranges are malformed </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public IndexChunkerTiled(NetcdfSharp.ucar.ma2.Section dataSection, NetcdfSharp.ucar.ma2.Section wantSection) throws NetcdfSharp.ucar.ma2.InvalidRangeException
		public IndexChunkerTiled(Section dataSection, Section wantSection)
		{
			this.done = 0;

			// LOOK - need test for "all" common case

			// The actual wanted data we can get from this section
			Section intersect = dataSection.intersect(wantSection);
			this.total = intersect.computeSize();
			if (total <= 0)
			{
				Console.WriteLine("IndexChunkerTiled HEY");
			}
			Debug.Assert(total > 0);
			int varRank = intersect.Rank;

			// create the List<Dim>
			// Section shifted = intersect.shiftOrigin(dataSection); // want relative to dataSection
			int wantStride = 1;
			int dataStride = 1;
			for (int ii = varRank - 1; ii >= 0; ii--)
			{
				NcRange dr = dataSection.getRange(ii);
				NcRange wr = wantSection.getRange(ii);
				NcRange ir = intersect.getRange(ii);
				dimList.Add(new Dim(dr, wr, ir, dataStride, wantStride)); // note reversed : fastest first
				dataStride *= dr.length();
				wantStride *= wr.length();
			}

			// the offset in the result Array of this piece of it
			startDestElem = wantSection.offset(intersect);
			startSrcElem = dataSection.offset(intersect);
			if (debugStartingElems)
			{
				Console.WriteLine(" startDestElem=" + startDestElem + " startSrcElem=" + startSrcElem);
			}

			// how many elements can we do at a time?
			if (varRank == 0)
			{
				this.nelems = 1;
			}
			else
			{
				Dim innerDim = dimList[0];
				this.nelems = innerDim.ncontigElements;
				if (innerDim.ncontigElements > 1)
				{
					innerDim.wantNelems = 1; // 1 wantIndex increment = nelems
					innerDim.wantStride = innerDim.ncontigElements;
				}
			}

			// we will use Index objects to keep track of the chunks
			int rank = dimList.Count;
			long[] dataStrides = new long[rank];
			int[] resultStrides = new int[rank];
			int[] shape = new int[rank];
			for (int i = 0; i < dimList.Count; i++)
			{ // reverse to slowest first
				Dim dim = dimList[i];
				dataStrides[rank - i - 1] = dim.dataStride * dim.want.stride();
				resultStrides[rank - i - 1] = dim.wantStride; // * dim.want.stride();
				shape[rank - i - 1] = dim.wantNelems;
			}
			if (debugDetail)
			{
				IndexChunker.printa(" indexShape=", shape);
				IndexChunker.printl(" dataStrides=", dataStrides);
				IndexChunker.printa(" wantStride=", resultStrides);
				Console.WriteLine(" indexChunks=" + NcIndex.computeSize(shape));
			}
			dataIndex = new IndexLong(shape, dataStrides);
			resultIndex = new NcIndex(shape, resultStrides);

			if (debugDetail)
			{
				Console.WriteLine(" dataIndex=" + dataIndex);
				Console.WriteLine(" resultIndex=" + resultIndex.toStringDebug());
			}

			// sanity checks
			long nchunks = NcIndex.computeSize(shape);
			Debug.Assert(nchunks * nelems == total);

			if (debug)
			{
				Console.WriteLine("RegularSectionLayout total = " + total + " nchunks= " + nchunks + " nelems= " + nelems + " dataSection= " + dataSection + " wantSection= " + wantSection + " intersect= " + intersect + this);
			}
		}

		private class Dim
		{
			internal NcRange data; // Range we got
			internal NcRange want; // Range we want
			internal NcRange intersect; // Range we want
			internal int dataStride; // stride in the data array
			internal int wantStride; // stride in the want array
			internal int wantNelems;
			internal int ncontigElements;

			internal Dim(NcRange data, NcRange want, NcRange intersect, int dataStride, int wantStride)
			{
				// assert want.length() <= maxSize : want.last() +" > "+ maxSize ;
				this.data = data;
				this.want = want;
				this.intersect = intersect;
				this.dataStride = dataStride;
				this.wantStride = wantStride;
				this.ncontigElements = intersect.stride() == 1 ? intersect.length() : 1;
				this.wantNelems = intersect.length();

				if (debugMerge)
				{
					Console.WriteLine("Dim=" + this);
				}
			}

			public override string ToString()
			{
				return "  data = " + data + " want = " + want + " intersect = " + intersect + " ncontigElements = " + ncontigElements;
			}
		} // Dim

		// Indexer methods
		public virtual long TotalNelems
		{
			get
			{
				return total;
			}
		}

		public virtual bool hasNext()
		{
			return done < total;
		}

		public virtual IndexChunker.Chunk next()
		{
			if (chunk == null)
			{
				chunk = new IndexChunker.Chunk(0, nelems, startDestElem);
			}
			else
			{
				dataIndex.incr();
				resultIndex.incr();
			}

			// Set the current element's index from the start of the data array
			chunk.SrcElem = startSrcElem + dataIndex.currentElement();

			// Set the current element's index from the start of the result array
			chunk.DestElem = startDestElem + resultIndex.currentElement();

			if (debugNext)
			{
				Console.WriteLine(" chunk: " + chunk);
			}
			if (debugDetail)
			{
				Console.WriteLine(" dataIndex: " + dataIndex);
				Console.WriteLine(" wantIndex: " + resultIndex);
			}

			done += nelems;
			return chunk;
		}

		////////////////////

		public override string ToString()
		{
			StringBuilder sbuff = new StringBuilder();
			foreach (Dim elem in dimList)
			{
				sbuff.Append("\n");
				sbuff.Append(elem);
			}
			return sbuff.ToString();
		}

	}
#pragma warning restore CS0162

}
