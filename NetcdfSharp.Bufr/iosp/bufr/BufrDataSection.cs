// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{

	/// <summary>
	/// Represents Section 4 of a BUFR message.
	/// 
	/// @author caron
	/// @since May 10, 2008
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class BufrDataSection
	public class BufrDataSection
	{
	  private readonly long dataPos;
	  private readonly int dataLength;

	  public BufrDataSection(long dataPos, int dataLength)
	  {
		this.dataPos = dataPos;
		this.dataLength = dataLength;
	  }

	  public virtual long DataPos
	  {
		  get
		  {
			return dataPos;
		  }
	  }

	  public virtual int DataLength
	  {
		  get
		  {
			return dataLength;
		  }
	  }


	}

}
