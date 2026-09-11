// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.ma2
{
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using IospHelper = NetcdfSharp.ucar.nc2.iosp.IospHelper;

	/// <summary>
	/// Make a deep copy of an existing StructureData, so that all the data is contained in this object
	/// 
	/// @author caron
	/// @since 6/19/2014
	/// </summary>
	public class StructureDataDeep : StructureDataA
	{

	  /// <summary>
	  /// Make deep copy from sdata to another StructureData object whose data is self contained
	  /// </summary>
	  /// <param name="sdata"> original sdata </param>
	  /// <param name="members"> the StructureData members. a reference is kept to this object </param>
	  /// <returns> StructureData with all data self contained </returns>
	  public static StructureDataDeep copy(StructureData sdata, StructureMembers members)
	  {
		ArrayStructureBB abb = copyToArrayBB(sdata, members, ByteOrder.BIG_ENDIAN);
		return new StructureDataDeep(abb);
	  }

	  /// <summary>
	  /// Make deep copy from an ArrayStructure to a ArrayStructureBB whose data is contained in a ByteBuffer
	  /// </summary>
	  /// <param name="as"> original ArrayStructure </param>
	  /// <param name="bo"> what byte order to use ? (null for any) </param>
	  /// <param name="canonical"> packing must be canonical </param>
	  /// <returns> ArrayStructureBB with all data self contained </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static ArrayStructureBB copyToArrayBB(ArrayStructure as, NetcdfSharp.java.nio.ByteOrder bo, boolean canonical) throws NetcdfSharp.java.io.IOException
	  public static ArrayStructureBB copyToArrayBB(ArrayStructure @as, ByteOrder bo, bool canonical)
	  {
		if (!canonical && @as.GetType().Equals(typeof(ArrayStructureBB)))
		{ // no subclasses, LOOK detect already canonical
																		  // later
		  ArrayStructureBB abb2 = (ArrayStructureBB) @as;
		  ByteBuffer bb = abb2.ByteBuffer;
		  if (bo == null || bo.Equals(bb.order()))
		  {
			return abb2;
		  }
		}

		StructureMembers sm = @as.StructureMembers.toBuilder(false).build();
		ArrayStructureBB abb = new ArrayStructureBB(sm, @as.Shape);
		ArrayStructureBB.setOffsets(sm); // this makes the packing canonical
		if (bo != null)
		{
		  ByteBuffer bb = abb.ByteBuffer;
		  bb.order(bo);
		}

		using (StructureDataIterator iter = @as.StructureDataIterator)
		{
		  while (iter.hasNext())
		  {
			copyToArrayBB(iter.next(), abb);
		  }
		}
		return abb;
	  }

	  /// <summary>
	  /// Make deep copy to an ArrayStructureBB whose data is contained in a ByteBuffer.
	  /// Use the order of the members in the given Structure; skip copying any not in the Structure
	  /// </summary>
	  /// <param name="s"> list of structure members come from here; must be compatible with ArrayStructure's data </param>
	  /// <param name="as"> original ArrayStructure </param>
	  /// <param name="bo"> what byte order to use ? (null for default) </param>
	  /// <returns> ArrayStructureBB with all data self contained </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static ArrayStructureBB copyToArrayBB(NetcdfSharp.ucar.nc2.Structure s, ArrayStructure as, NetcdfSharp.java.nio.ByteOrder bo) throws NetcdfSharp.java.io.IOException
	  public static ArrayStructureBB copyToArrayBB(Structure s, ArrayStructure @as, ByteOrder bo)
	  {
		StructureMembers sm = s.makeStructureMembers();
		ArrayStructureBB abb = new ArrayStructureBB(sm, @as.Shape);
		ArrayStructureBB.setOffsets(sm);
		if (bo != null)
		{
		  ByteBuffer bb = abb.ByteBuffer;
		  bb.order(bo);
		}

		using (StructureDataIterator iter = @as.StructureDataIterator)
		{
		  while (iter.hasNext())
		  {
			copyToArrayBB(iter.next(), abb);
		  }
		}
		return abb;
	  }


	  /// <summary>
	  /// Make deep copy from a StructureData to a ArrayStructureBB whose data is contained in a ByteBuffer.
	  /// </summary>
	  /// <param name="sdata"> original ArrayStructure. </param>
	  /// <returns> ArrayStructureBB with all data self contained </returns>
	  public static ArrayStructureBB copyToArrayBB(StructureData sdata)
	  {
		return copyToArrayBB(sdata, sdata.StructureMembers.toBuilder(false).build(), ByteOrder.BIG_ENDIAN);
	  }

	  /// <summary>
	  /// Make deep copy from a StructureData to a ArrayStructureBB whose data is contained in a ByteBuffer
	  /// </summary>
	  /// <param name="sdata"> original ArrayStructure </param>
	  /// <param name="sm"> the StructureData members. a reference is kept to this object </param>
	  /// <param name="bo"> Byte Order of the ByteBuffer </param>
	  /// <returns> ArrayStructureBB with all data self contained </returns>
	  public static ArrayStructureBB copyToArrayBB(StructureData sdata, StructureMembers sm, ByteOrder bo)
	  {
		int size = sm.StructureSize;
		ByteBuffer bb = ByteBuffer.allocate(size); // default is big endian
		bb.order(bo);
		ArrayStructureBB abb = new ArrayStructureBB(sm, new int[] {1}, bb, 0);
		ArrayStructureBB.setOffsets(sm);
		copyToArrayBB(sdata, abb);
		return abb;
	  }

	  /// <summary>
	  /// Make deep copy from a StructureData into the given ArrayStructureBB
	  /// </summary>
	  /// <param name="sdata"> original data from here </param>
	  /// <param name="abb"> copy data into this ArrayStructureBB, starting from wherever the ByteBuffer current position is </param>
	  /// <returns> number of bytes copied </returns>
	  public static int copyToArrayBB(StructureData sdata, ArrayStructureBB abb)
	  {
		// StructureMembers sm = sdata.getStructureMembers();
		ByteBuffer bb = abb.ByteBuffer;
		int start = bb.limit();

		foreach (StructureMembers.Member wantMember in abb.Members)
		{
		  StructureMembers.Member m = sdata.findMember(wantMember.Name);
		  Debug.Assert(m != null);
		  Debug.Assert(m.DataType == wantMember.DataType);

		  DataType dtype = m.DataType;
		  if (m.Scalar)
		  {
			switch (dtype)
			{
			  case NetcdfSharp.ucar.ma2.DataType.STRING:
				bb.putInt(abb.addObjectToHeap(sdata.getScalarString(m)));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.FLOAT:
				bb.putFloat(sdata.getScalarFloat(m));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.DOUBLE:
				bb.putDouble(sdata.getScalarDouble(m));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.INT:
			  case NetcdfSharp.ucar.ma2.DataType.UINT:
			  case NetcdfSharp.ucar.ma2.DataType.ENUM4:
				bb.putInt(sdata.getScalarInt(m));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.SHORT:
			  case NetcdfSharp.ucar.ma2.DataType.USHORT:
			  case NetcdfSharp.ucar.ma2.DataType.ENUM2:
				bb.putShort(sdata.getScalarShort(m));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.BYTE:
			  case NetcdfSharp.ucar.ma2.DataType.UBYTE:
			  case NetcdfSharp.ucar.ma2.DataType.ENUM1:
				bb.put(sdata.getScalarByte(m));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.CHAR:
				bb.put((byte) sdata.getScalarChar(m));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.LONG:
			  case NetcdfSharp.ucar.ma2.DataType.ULONG:
				bb.putLong(sdata.getScalarLong(m));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.STRUCTURE:
				StructureData sd = sdata.getScalarStructure(m);
				ArrayStructureBB out_abb = new ArrayStructureBB(sd.StructureMembers, new int[] {1}, bb, 0);
				copyToArrayBB(sd, out_abb);
				break;
			  default:
				throw new System.InvalidOperationException("scalar " + dtype);
			  /*
			   * case BOOLEAN:
			   * break;
			   * case SEQUENCE:
			   * break;
			   * case OPAQUE:
			   * break;
			   */
			}
		  }
		  else
		  {
			int n = m.Size;
			switch (dtype)
			{
			  case NetcdfSharp.ucar.ma2.DataType.STRING:
				string[] ss = sdata.getJavaArrayString(m);
				bb.putInt(abb.addObjectToHeap(ss)); // stored as String[] on the heap
				break;
			  case NetcdfSharp.ucar.ma2.DataType.FLOAT:
				float[] fdata = sdata.getJavaArrayFloat(m);
				for (int i = 0; i < n; i++)
				{
				  bb.putFloat(fdata[i]);
				}
				break;
			  case NetcdfSharp.ucar.ma2.DataType.DOUBLE:
				double[] ddata = sdata.getJavaArrayDouble(m);
				for (int i = 0; i < n; i++)
				{
				  bb.putDouble(ddata[i]);
				}
				break;
			  case NetcdfSharp.ucar.ma2.DataType.INT:
			  case NetcdfSharp.ucar.ma2.DataType.ENUM4:
				int[] idata = sdata.getJavaArrayInt(m);
				for (int i = 0; i < n; i++)
				{
				  bb.putInt(idata[i]);
				}
				break;
			  case NetcdfSharp.ucar.ma2.DataType.SHORT:
			  case NetcdfSharp.ucar.ma2.DataType.ENUM2:
				short[] shdata = sdata.getJavaArrayShort(m);
				for (int i = 0; i < n; i++)
				{
				  bb.putShort(shdata[i]);
				}
				break;
			  case NetcdfSharp.ucar.ma2.DataType.BYTE:
			  case NetcdfSharp.ucar.ma2.DataType.ENUM1:
				byte[] bdata = sdata.getJavaArrayByte(m);
				for (int i = 0; i < n; i++)
				{
				  bb.put(bdata[i]);
				}
				break;
			  case NetcdfSharp.ucar.ma2.DataType.CHAR:
				char[] cdata = sdata.getJavaArrayChar(m);
				bb.put(IospHelper.convertCharToByte(cdata));
				break;
			  case NetcdfSharp.ucar.ma2.DataType.LONG:
				long[] ldata = sdata.getJavaArrayLong(m);
				for (int i = 0; i < n; i++)
				{
				  bb.putLong(ldata[i]);
				}
				break;
			  default:
				throw new System.InvalidOperationException("array " + dtype);
			  /*
			   * case BOOLEAN:
			   * break;
			   * case OPAQUE:
			   * break;
			   * case STRUCTURE:
			   * break; //
			   */
			  case NetcdfSharp.ucar.ma2.DataType.SEQUENCE:
				break; // skip
			}
		  }
		}
		return bb.limit() - start;
	  }

	  // private ArrayStructureBB abb;
	  private StructureDataDeep(ArrayStructureBB abb) : base(abb, 0)
	  {
		// this.abb = abb;
	  }

	  // public ByteBuffer getByteBuffer() { return abb.getByteBuffer(); }

	}

}
