// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// Creates a StructureDataIterator by wrapping a section of a ArrayStructure.
	/// 
	/// @author caron
	/// @since Nov 16, 2009
	/// </summary>


	public class SequenceIterator : StructureDataIterator
	{
	  private int start, size, count;
	  private ArrayStructure abb;

	  public SequenceIterator(int start, int size, ArrayStructure abb)
	  {
		this.start = start;
		this.size = size;
		this.abb = abb;
		this.count = 0;
	  }

	  public virtual bool hasNext()
	  {
		return (count < size);
	  }

	  public virtual StructureData next()
	  {
		StructureData result = abb.getStructureData(start + count);
		count++;
		return result;
	  }

	  public virtual StructureDataIterator reset()
	  {
		count = 0;
		return this;
	  }

	  public virtual int CurrentRecno
	  {
		  get
		  {
			return count;
		  }
	  }

	  public virtual void Dispose() { }

	}

}
