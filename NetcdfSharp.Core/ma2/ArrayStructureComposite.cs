// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// An ArrayStructure compose of other ArrayStructures.
	/// Doesnt work because of read(StructureMembers.Member). this need to be withdrawn.
	/// 
	/// int total = 0;
	/// List<ArrayStructure> list = new ArrayList<ArrayStructure> (msgs.size());
	/// for (Message m : msgs) {
	/// ArrayStructure oneMess;
	/// if (!m.dds.isCompressed()) {
	/// MessageUncompressedDataReader reader = new MessageUncompressedDataReader();
	/// oneMess = reader.readEntireMessage(s, protoMessage, m, raf, null);
	/// } else {
	/// MessageCompressedDataReader reader = new MessageCompressedDataReader();
	/// oneMess = reader.readEntireMessage(s, protoMessage, m, raf, null);
	/// }
	/// list.add(oneMess);
	/// total += (int) oneMess.getSize();
	/// }
	/// 
	/// return (list.size() == 1) ? list.get(0) : new ArrayStructureComposite(sm, list, total);
	/// 
	/// 
	/// @author caron
	/// @since Nov 19, 2009
	/// </summary>
	public class ArrayStructureComposite : ArrayStructure
	{
	  private IList<ArrayStructure> compose;
	  private int[] start;

	  public ArrayStructureComposite(StructureMembers members, IList<ArrayStructure> c, int total) : base(members, new int[total])
	  {
		this.compose = c;

		start = new int[total];
		int count = 0;
		int i = 0;
		foreach (ArrayStructure @as in compose)
		{
		  start[i++] = count;
		  count += (int) @as.Size;
		}
	  }


	  protected internal override StructureData makeStructureData(ArrayStructure me, int recno)
	  {
		for (int i = 0; i < start.Length; i++)
		{
		  if (recno >= start[i])
		  {
			ArrayStructure @as = compose[i];
			return @as.makeStructureData(@as, recno - start[i]);
		  }
		}
		throw new ArgumentException();
	  }
	}

}
