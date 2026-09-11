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
	/// @since 7/9/2014
	/// </summary>
	public class StructureDataIteratorMediated : StructureDataIterator
	{

	  private StructureDataIterator org;
	  private StructureDataMediator mod;

	  public StructureDataIteratorMediated(StructureDataIterator org, StructureDataMediator mod)
	  {
		this.org = org;
		this.mod = mod;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public StructureData next() throws NetcdfSharp.java.io.IOException
	  public virtual StructureData next()
	  {
		return mod.modify(org.next());
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean hasNext() throws NetcdfSharp.java.io.IOException
	  public virtual bool hasNext()
	  {
		return org.hasNext();
	  }

	  public virtual StructureDataIterator reset()
	  {
		org.reset();
		return this;
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
