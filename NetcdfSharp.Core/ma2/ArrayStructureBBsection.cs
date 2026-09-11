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
	/// Make a section of an ArrayStructureBB </summary>
	public class ArrayStructureBBsection : ArrayStructureBB
	{
	  protected internal int[] orgRecnum;

	  /// <summary>
	  /// Make a section of an ArrayStructureBB
	  /// </summary>
	  /// <param name="org"> the original </param>
	  /// <param name="section"> of the whole </param>
	  /// <returns> original, if section is empty or the same saze as the original, else a section of the original </returns>
	  public static ArrayStructureBB factory(ArrayStructureBB org, Section section)
	  {
		if (section == null || section.computeSize() == org.Size)
		{
		  return org;
		}
		return new ArrayStructureBBsection(org.StructureMembers, org.Shape, org.ByteBuffer, section);
	  }

	  private ArrayStructureBBsection(StructureMembers members, int[] shape, ByteBuffer bbuffer, Section section) : base(members, shape, bbuffer, 0)
	  {
		int n = (int) section.computeSize();
		Section.Iterator iter = section.getIterator(shape);
		orgRecnum = new int[n];
		int count = 0;
		while (iter.hasNext())
		{
		  orgRecnum[count++] = iter.next(null);
		}
	  }


	  protected internal override int calcOffsetSetOrder(int recnum, StructureMembers.Member m)
	  {
		if (null != m.DataObject)
		{
		  bbuffer.order((ByteOrder) m.DataObject);
		}
		return bb_offset + orgRecnum[recnum] * StructureSize + m.DataParam;
	  }
	}

}
