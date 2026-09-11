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
	/// Read a maximum number of StructureData objects from a StructureDataIterator.
	/// 
	/// @author caron
	/// @since Apr 23, 2008
	/// </summary>
	public class StructureDataIteratorLimited : StructureDataIterator
	{

	  private StructureDataIterator org;
	  private int limit, count;

	  public StructureDataIteratorLimited(StructureDataIterator org, int limit)
	  {
		this.org = org;
		this.limit = limit;
		this.count = 0;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public StructureData next() throws NetcdfSharp.java.io.IOException
	  public virtual StructureData next()
	  {
		return org.next();
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean hasNext() throws NetcdfSharp.java.io.IOException
	  public virtual bool hasNext()
	  {
		return count < limit && org.hasNext();
	  }

	  public virtual StructureDataIterator reset()
	  {
		this.count = 0;
		org = org.reset();
		return (org == null) ? null : this;
	  }

	  public virtual int BufferSize
	  {
		  set
		  {
			org.BufferSize = value;
		  }
	  }

	  public virtual int CurrentRecno
	  {
		  get
		  {
			return org.CurrentRecno;
		  }
	  }

	  public virtual void Dispose()
	  {
		org.Dispose();
	  }

	}

}
