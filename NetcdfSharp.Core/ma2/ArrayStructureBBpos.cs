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
	/// Variation of ArrayStructureBB, where the offsets of the records into the ByteBuffer are uneven and must be
	/// passed in by the user.
	/// 
	/// @author caron
	/// </summary>
	public class ArrayStructureBBpos : ArrayStructureBB
	{
	  protected internal int[] positions;

	  /// <summary>
	  /// Construct an ArrayStructureBB with the given ByteBuffer.
	  /// </summary>
	  /// <param name="members"> the list of structure members. </param>
	  /// <param name="shape"> the shape of the structure array </param>
	  /// <param name="bbuffer"> the data is stored in this ByteBuffer. bbuffer.order must already be set. </param>
	  /// <param name="positions"> offset from the start of the ByteBufffer to each record. must have length = with shape.getSize() </param>
	  public ArrayStructureBBpos(StructureMembers members, int[] shape, ByteBuffer bbuffer, int[] positions) : base(members, shape, bbuffer, 0)
	  {
		this.positions = positions;
	  }


	  protected internal override int calcOffsetSetOrder(int recnum, StructureMembers.Member m)
	  {
		if (null != m.DataObject)
		{
		  bbuffer.order((ByteOrder) m.DataObject);
		}
		return positions[recnum] + m.DataParam;
	  }
	}

}
