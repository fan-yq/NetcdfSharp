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
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// A StructureData implementation delegates everything to the containing ArrayStructure.
	/// 
	/// @author caron
	/// </summary>

	public class StructureDataA : StructureData
	{
	  protected internal ArrayStructure sa;
	  protected internal int recno;

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="sa"> StructureData is always contained in a StructureArray. </param>
	  /// <param name="recno"> the recno in the StructureArray. </param>
	  public StructureDataA(ArrayStructure sa, int recno) : base(sa.StructureMembers)
	  {
		this.sa = sa;
		this.recno = recno;
	  }

	  public override NcArray getArray(StructureMembers.Member m)
	  {
		return sa.getArray(recno, m);
	  }

	  public override int convertScalarInt(StructureMembers.Member m)
	  {
		return sa.convertScalarInt(recno, m);
	  }

	  public override long convertScalarLong(StructureMembers.Member m)
	  {
		return sa.convertScalarLong(recno, m);
	  }

	  public override float convertScalarFloat(StructureMembers.Member m)
	  {
		return sa.convertScalarFloat(recno, m);
	  }

	  public override double convertScalarDouble(StructureMembers.Member m)
	  {
		return sa.convertScalarDouble(recno, m);
	  }

	  public override double getScalarDouble(StructureMembers.Member m)
	  {
		return sa.getScalarDouble(recno, m);
	  }

	  public override double[] getJavaArrayDouble(StructureMembers.Member m)
	  {
		return sa.getJavaArrayDouble(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type float.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
	  /// <returns> scalar float value </returns>
	  public override float getScalarFloat(StructureMembers.Member m)
	  {
		return sa.getScalarFloat(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type float array.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
	  /// <returns> 1D array of floats </returns>
	  public override float[] getJavaArrayFloat(StructureMembers.Member m)
	  {
		return sa.getJavaArrayFloat(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type byte.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type byte. </param>
	  /// <returns> scalar byte value </returns>
	  public override byte getScalarByte(StructureMembers.Member m)
	  {
		return sa.getScalarByte(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type byte array.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type byte. </param>
	  /// <returns> 1D array of bytes </returns>
	  public override byte[] getJavaArrayByte(StructureMembers.Member m)
	  {
		return sa.getJavaArrayByte(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type short.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type short. </param>
	  /// <returns> scalar short value </returns>
	  public override short getScalarShort(StructureMembers.Member m)
	  {
		return sa.getScalarShort(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type short array.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type short. </param>
	  /// <returns> 1D array of shorts </returns>
	  public override short[] getJavaArrayShort(StructureMembers.Member m)
	  {
		return sa.getJavaArrayShort(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type int.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type int. </param>
	  /// <returns> scalar int value </returns>
	  public override int getScalarInt(StructureMembers.Member m)
	  {
		return sa.getScalarInt(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type int array.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type int. </param>
	  /// <returns> 1D array of ints </returns>
	  public override int[] getJavaArrayInt(StructureMembers.Member m)
	  {
		return sa.getJavaArrayInt(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type long.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type long. </param>
	  /// <returns> scalar long value </returns>
	  public override long getScalarLong(StructureMembers.Member m)
	  {
		return sa.getScalarLong(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type long array.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type long. </param>
	  /// <returns> 1D array of longs </returns>
	  public override long[] getJavaArrayLong(StructureMembers.Member m)
	  {
		return sa.getJavaArrayLong(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type char.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
	  /// <returns> scalar char value </returns>
	  public override char getScalarChar(StructureMembers.Member m)
	  {
		return sa.getScalarChar(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type char array.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
	  /// <returns> 1D array of chars </returns>
	  public override char[] getJavaArrayChar(StructureMembers.Member m)
	  {
		return sa.getJavaArrayChar(recno, m);
	  }

	  /// <summary>
	  /// Get member data of type String or 1D char.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
	  /// <returns> scalar char value </returns>
	  public override string getScalarString(StructureMembers.Member m)
	  {
		return sa.getScalarString(recno, m);
	  }

	  /*
	   * Get String value, from rank 0 String or rank 1 char member array.
	   * For more efficiency, use getScalarString(StructureMembers.Member m) if possible.
	   *
	   * @param memberName name of member Variable.
	   * 
	   * @throws IllegalArgumentException if name is not legal member name.
	   *
	   * public String getScalarString(String memberName) {
	   * StructureMembers.Member m = findMember(memberName);
	   * if (null == m) throw new IllegalArgumentException("Member not found= " + memberName);
	   * if ((m.getDataType() == DataType.CHAR) || (m.getDataType() == DataType.STRING))
	   * return sa.getScalarString(recno, m);
	   * 
	   * Array a = getArray(m);
	   * if (a == null) throw new IllegalArgumentException("illegal member name =" + memberName);
	   * if (a instanceof ArrayChar.D1)
	   * return ((ArrayChar.D1) a).getString();
	   * 
	   * assert (a instanceof ArrayObject.D0);
	   * Object data = ((ArrayObject.D0) a).get();
	   * assert (data instanceof String) : data.getClass().getName();
	   * return (String) data;
	   * }
	   */

	  // LOOK can we optimize ??
	  public override string[] getJavaArrayString(StructureMembers.Member m)
	  {
		if (m.DataType == DataType.STRING)
		{
		  NcArray data = getArray(m);
		  int n = m.Size;
		  string[] result = new string[n];
		  for (int i = 0; i < result.Length; i++)
		  {
			result[i] = (string) data.getObject(i);
		  }
		  return result;

		}
		else if (m.DataType == DataType.CHAR)
		{
		  ArrayChar data = (ArrayChar) getArray(m);
		  ArrayChar.StringIterator iter = data.getStringIterator();
		  string[] result = new string[iter.getNumElems()];
		  int count = 0;
		  while (iter.MoveNext())
		  {
			result[count++] = iter.Current;
		  }
		  return result;
		}

		throw new ArgumentException("getJavaArrayString: not String DataType :" + m.DataType);
	  }

	  public override StructureData getScalarStructure(StructureMembers.Member m)
	  {
		return sa.getScalarStructure(recno, m);
	  }

	  public override ArrayStructure getArrayStructure(StructureMembers.Member m)
	  {
		return sa.getArrayStructure(recno, m);
	  }

	  public override ArraySequence getArraySequence(StructureMembers.Member m)
	  {
		return sa.getArraySequence(recno, m);
	  }

	  public override void showInternal(Formatter f, Indent indent)
	  {
		base.showInternal(f, indent);
		sa.showInternal(f, indent);
	  }

	}

}
