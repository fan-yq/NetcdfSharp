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
	/// Iterator to read/write subsets of a multidimensional array, finding the contiguous chunks.
	/// The iteration is monotonic in both src and dest positions.
	/// <p/>
	/// Example for Integers:
	/// 
	/// <pre>
	///  int[] read( Layout index, int[] src) {
	///    int[] dest = new int[index.getTotalNelems()];
	///    while (index.hasNext()) {
	///      Layout.Chunk chunk = index.next();
	///      System.arraycopy(src, chunk.getSrcElem(), dest, chunk.getDestElem(), chunk.getNelems());
	///    }
	///    return dest;
	///  }
	/// 
	///  int[] read( Layout index, RandomAccessFile raf) {
	///    int[] dest = new int[index.getTotalNelems()];
	///    while (index.hasNext()) {
	///      Layout.Chunk chunk = index.next();
	///      raf.seek( chunk.getSrcPos());
	///      raf.readInt(dest, chunk.getDestElem(), chunk.getNelems());
	///    }
	///    return dest;
	///  }
	/// 
	///   // note src and dest misnamed
	///    void write( Layout index, int[] src, RandomAccessFile raf) {
	///      while (index.hasNext()) {
	///        Layout.Chunk chunk = index.next();
	///        raf.seek ( chunk.getSrcPos());
	///        for (int k=0; k<chunk.getNelems(); k++)
	///          raf.writeInt(src, chunk.getDestElem(), chunk.getNelems());
	///          raf.writeInt( ii.getByteNext());
	///      }
	/// 
	/// </pre>
	/// 
	/// @author caron
	/// @since Jan 2, 2008
	/// </summary>

	public interface Layout
	{

	  /// <summary>
	  /// Get total number of elements in the wanted subset.
	  /// </summary>
	  /// <returns> total number of elements in the wanted subset. </returns>
	  long TotalNelems {get;}

	  /// <summary>
	  /// Get size of each element in bytes.
	  /// </summary>
	  /// <returns> size of each element in bytes. </returns>
	  int ElemSize {get;}

	  /// <summary>
	  /// Is there more to do
	  /// </summary>
	  /// <returns> true if theres more to do </returns>
	  bool hasNext();

	  /// <summary>
	  /// Get the next chunk
	  /// </summary>
	  /// <returns> next chunk, or null if !hasNext() </returns>
	  Chunk next();

	  /// <summary>
	  /// A chunk of data that is contiguous in both the source and destination.
	  /// Read nelems from src at filePos, store in destination at startElem.
	  /// (or) Write nelems to file at filePos, from array at startElem.
	  /// </summary>
	  public interface Chunk
	  {

		/// <summary>
		/// Get the position in source where to read or write: "file position"
		/// </summary>
		/// <returns> position as a byte count into the source, eg a file </returns>
		long SrcPos {get;}

		/// <summary>
		/// Get number of elements to transfer contiguously (Note: elements, not bytes)
		/// </summary>
		/// <returns> number of elements to transfer </returns>
		int Nelems {get;}

		/// <summary>
		/// Get starting element position as a 1D element index into the destination, eg the requested array with shape
		/// "wantSection".
		/// </summary>
		/// <returns> starting element in the array (Note: elements, not bytes) </returns>
		long DestElem {get;}
	  }
	}

}
