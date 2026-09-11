// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// An iterator over StructureData.
	/// Make sure that you call close().
	/// Best to put in a try/finally block like:
	/// 
	/// <pre>
	/// try {
	///   while (iter.hasNext())
	///     process(iter.next());
	/// } finally {
	///   iter.finish();
	/// }
	/// </pre>
	/// 
	/// @author caron
	/// @since Feb 23, 2008
	/// </summary>
	public interface StructureDataIterator : IDisposable
	{

	  /// <summary>
	  /// See if theres more StructureData in the iteration.
	  /// You must always call this before calling next().
	  /// </summary>
	  /// <returns> true if more records are available </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: boolean hasNext() throws NetcdfSharp.java.io.IOException;
	  bool hasNext();

	  /// <summary>
	  /// Get the next StructureData in the iteration.
	  /// </summary>
	  /// <returns> next StructureData record. </returns>
	  /// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: StructureData next() throws NetcdfSharp.java.io.IOException;
	  StructureData next();

	  /// <summary>
	  /// Hint to use this much memory in buffering the iteration.
	  /// No guarentee that it will be used by the implementation.
	  /// </summary>
	  /// <param name="bytes"> amount of memory in bytes </param>
	  int BufferSize
	  {
		  set
		  {
			// doan do nuthin
		  }
	  }

	  /// <summary>
	  /// Start the iteration over again.
	  /// </summary>
	  /// <returns> a new or reset iterator. </returns>
	  StructureDataIterator reset();

	  int CurrentRecno {get;}

	  /// <summary>
	  /// Make sure that the iterator is complete, and recover resources.
	  /// Best to put in a try/finally block like:
	  /// 
	  /// <pre>
	  /// try (StructureDataIterator iter = obj.getStructureDataIterator()) {
	  ///   while (iter.hasNext())
	  ///     process(iter.next());
	  /// }
	  /// </pre>
	  /// </summary>
	  void close()
	  {
		// doan do nuthin
	  }

	  ////////////

	  /// @deprecated use close() or try-with-resource 
	  void finish()
	  {
		close();
	  }



	}

}
