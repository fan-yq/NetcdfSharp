// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp
{
	using Section = NetcdfSharp.ucar.ma2.Section;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;

	/// <summary>
	/// Indexer into data that has a "regular" layout, like netcdf-3 and hdf5 compact and contiguous storage.
	/// The data is contiguous, with outer dimension varying fastest.
	/// Given a Section, this calculates the set of contiguous "chunks" of the wanted data into the stored data.
	/// The wanted section is always a subset of the data section (see RegularSectionLayout where thats not the case).
	/// 
	/// @author caron
	/// @since Jan 3, 2008
	/// </summary>
	public class LayoutRegular : Layout
	{
	  private IndexChunker chunker;
	  private long startPos; // starting position
	  private int elemSize; // size of each element

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="startPos"> starting address of the entire data array. </param>
	  /// <param name="elemSize"> size of an element in bytes. </param>
	  /// <param name="varShape"> shape of the entire data array. </param>
	  /// <param name="wantSection"> the wanted section of data, contains a List of Range objects. </param>
	  /// <exception cref="InvalidRangeException"> if ranges are misformed </exception>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public LayoutRegular(long startPos, int elemSize, int[] varShape, @Nullable Section wantSection) throws NetcdfSharp.ucar.ma2.InvalidRangeException
	  public LayoutRegular(long startPos, int elemSize, int[] varShape, Section wantSection)
	  {
		Debug.Assert(startPos >= 0);
		Debug.Assert(elemSize > 0);

		this.startPos = startPos;
		this.elemSize = elemSize;

		chunker = new IndexChunker(varShape, wantSection);
	  }

	  public virtual long TotalNelems
	  {
		  get
		  {
			return chunker.TotalNelems;
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
		return chunker.hasNext();
	  }

	  public virtual Chunk next()
	  {
		IndexChunker.Chunk chunk = chunker.next();
		chunk.SrcPos = startPos + chunk.SrcElem * elemSize;
		return chunk;
	  }
	}

}
