// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
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
	/// For datasets where the data are stored in chunks.
	/// "Tiled" means that all chunks are assumed to be equal size.
	/// Chunks have an offset into the complete array.
	/// Chunks do not necessarily cover the array, missing data is possible.
	/// 
	/// @author caron
	/// @since Jan 9, 2008
	/// </summary>
	public class LayoutTiled : Layout
	{
		private Section want;
		private int[] chunkSize; // all chunks assumed to be the same size
		private int elemSize;
		private long startSrcPos;

		private DataChunkIterator chunkIterator; // iterate across chunks
		private IndexChunkerTiled index; // iterate within a chunk

		// track the overall iteration
		private long totalNelems, totalNelemsDone; // total number of elemens

		private const bool debug = false, debugNext = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="chunkIterator"> iterator over all available data chunks </param>
		/// <param name="chunkSize"> all chunks assumed to be the same size </param>
		/// <param name="elemSize"> size of an element in bytes. </param>
		/// <param name="wantSection"> the wanted section of data, contains a List of Range objects. Must be complete </param>
		public LayoutTiled(DataChunkIterator chunkIterator, int[] chunkSize, int elemSize, Section wantSection)
		{
			this.chunkIterator = chunkIterator;
			this.chunkSize = chunkSize;
			this.elemSize = elemSize;
			this.want = wantSection;
			if (this.want.isVariableLength())
			{
				// remove the varlen
				IList<NcRange> newrange = new List<NcRange>(this.want.Ranges);
				newrange.RemoveAt(newrange.Count - 1);
				this.want = new Section(newrange);
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
		private Layout.Chunk next_Conflict;

		public virtual bool hasNext()
		{ // have to actually fetch the thing here
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
						dataSection = new Section(dataChunk.offset, chunkSize);
						if (debug)
						{
							// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
							Console.Write(" dataChunk: {0}%n", dataSection);
						}
						if (dataSection.intersects(want)) // does it intersect ?
						{
							break;
						}
					}

					if (debug)
					{
						Console.WriteLine(" found intersecting section: " + dataSection + " for filePos " + dataChunk.filePos);
					}
					index = new IndexChunkerTiled(dataSection, want);
					startSrcPos = dataChunk.filePos;

				}
				catch (InvalidRangeException e)
				{
					throw new System.InvalidOperationException(e.Message);
				}
			}

			IndexChunker.Chunk chunk = index.next();
			totalNelemsDone += chunk.Nelems;
			chunk.SrcPos = startSrcPos + chunk.SrcElem * elemSize;
			next_Conflict = chunk;
			return true;
		}

		public virtual Layout.Chunk next()
		{
			if (debugNext)
			{
				Console.WriteLine("  next=" + next_Conflict);
			}
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
		/// An iterator over DataChunk's </summary>
		public interface DataChunkIterator
		{
			bool hasNext();

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: DataChunk next() throws NetcdfSharp.java.io.IOException;
			DataChunk next();
		}

		/// <summary>
		/// The chunks of a tiled layout. </summary>
		public class DataChunk
		{
			public int[] offset; // offset index of this chunk, relative to entire array
			public long filePos; // filePos of a single raw data chunk

			public DataChunk(int[] offset, long filePos)
			{
				this.offset = offset;
				this.filePos = filePos;
			}
		}

#pragma warning restore CS0162

	}

}
