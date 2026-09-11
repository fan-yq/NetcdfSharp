// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp
{

	/// <summary>
	/// A Layout that supplies the "source" ByteBuffer.
	/// This is used when the data must be massaged after being read, eg uncompresed or filtered.
	/// The modified data is placed in a ByteBuffer, which may change for different chunks, and
	/// so is supplied by each chunk.
	/// 
	/// <p/>
	/// Example for Integers:
	/// 
	/// <pre>
	/// 
	/// int[] read(LayoutBB index, int[] pa) {
	///   while (index.hasNext()) {
	///     LayoutBB.Chunk chunk = index.next();
	///     IntBuffer buff = chunk.getIntBuffer();
	///     buff.position(chunk.getSrcElem());
	///     int pos = (int) chunk.getDestElem();
	///     for (int i = 0; i < chunk.getNelems(); i++)
	///       pa[pos++] = buff.get();
	///   }
	///   return pa;
	/// }
	/// 
	/// </pre>
	/// 
	/// @author caron
	/// @since Jan 9, 2008
	/// </summary>

	public interface LayoutBB : Layout
	{
		/// <summary>
		/// A chunk of data that is contiguous in both the source and destination.
		/// Read nelems from ByteBuffer at filePos, store in destination at startElem.
		/// </summary>
		internal interface ChunkBB : Layout.Chunk
		{

			/// <summary>
			/// Get the position in source <Type>Buffer where to read or write: "file position"
			/// </summary>
			/// <returns> position as a element index into the <Type>Buffer </returns>
			int SrcElem { get; }

			ByteBuffer ByteBuffer { get; }

			ShortBuffer ShortBuffer { get; }

			IntBuffer IntBuffer { get; }

			FloatBuffer FloatBuffer { get; }

			DoubleBuffer DoubleBuffer { get; }

			LongBuffer LongBuffer { get; }
		}
	}

}
