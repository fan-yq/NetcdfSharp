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
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// A container for a Structure's data.
	/// Is normally contained within an ArrayStructure, which is an Array of StructureData.
	/// This is the abstract supertype for all implementations.
	/// 
	/// <pre>
	///   for (Iterator iter = sdata.getMembers().iterator(); iter.hasNext(); ) {
	///      StructureMembers.Member m = (StructureMembers.Member) iter.next();
	///      Array sdataArray = sdata.getArray(m);
	///      ...
	///   }
	/// </pre>
	/// 
	/// General ways to access data in an StructureData are:
	/// 
	/// <pre>
	///  Array getArray(Member m)
	/// </pre>
	/// 
	/// <pre>
	///  Array getArray(String memberName)
	/// </pre>
	/// 
	/// The following will return an object of type Byte, Char, Double, Float, Int, Long, Short, String, or Structure,
	/// depending
	/// upon the member type:
	/// 
	/// <pre>
	///  Object getScalarObject( Member m)
	/// </pre>
	/// 
	/// A number of convenience routines may be able to avoid extra Object creation, and so are recommended for efficiency.
	/// These require that you know the data types of the member data, but they are the most efficent:
	/// 
	/// <pre>
	///  getScalarXXX(int recnum, Member m)
	///  getJavaArrayXXX(int recnum, Member m)
	/// </pre>
	/// 
	/// where XXX is Byte, Char, Double, Float, Int, Long, Short, or String. For members that are themselves Structures,
	/// the equivilent is:
	/// 
	/// <pre>
	///  StructureData getScalarStructure(int recnum, Member m)
	///  ArrayStructure getArrayStructure(int recnum, Member m)
	/// </pre>
	/// 
	/// These will return any compatible type as a double or float, but may have extra overhead when the types dont match:
	/// 
	/// <pre>
	///  convertScalarXXX(int recnum, Member m)
	///  convertJavaArrayXXX(int recnum, Member m)
	/// </pre>
	/// 
	/// where XXX is Double or Float
	/// 
	/// 
	/// @author caron </summary>
	/// <seealso cref="ArrayStructure"/>

	public abstract class StructureData
	{
	  public static readonly StructureData EMPTY = new StructureDataW(StructureMembers.builder().setName("empty").build());

	  protected internal StructureMembers members;

	  protected internal StructureData(StructureMembers members)
	  {
		this.members = members;
	  }

	  /// <returns> name of Structure </returns>
	  public virtual string Name
	  {
		  get
		  {
			return members.Name;
		  }
	  }

	  /// <returns> StructureMembers object </returns>
	  public virtual StructureMembers StructureMembers
	  {
		  get
		  {
			return members;
		  }
	  }

	  /// <returns> List of StructureMembers.Member </returns>
	  public virtual IList<StructureMembers.Member> Members
	  {
		  get
		  {
			return members.Members;
		  }
	  }

	  /// <summary>
	  /// Find a member by its name.
	  /// </summary>
	  /// <param name="memberName"> find member with this name </param>
	  /// <returns> StructureMembers.Member matching the name, or null if not found </returns>
	  public virtual StructureMembers.Member findMember(string memberName)
	  {
		return members.findMember(memberName);
	  }


	  //////////////////////////////////////////////////////////////////////////////////////////////////////


	  /// <summary>
	  /// Get member data array of any type as an Array.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. </param>
	  /// <returns> Array values. </returns>
	  public abstract NcArray getArray(StructureMembers.Member m);

	  /// <summary>
	  /// Get member data array of any type as an Array.
	  /// For more efficiency, use getScalarXXX(Member) or getJavaArrayXXX(Member) is possible.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. </param>
	  /// <returns> member data array of any type as an Array. </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual NcArray getArray(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getArray(m);
	  }

	  /// <summary>
	  /// Get member data array of any type as an Object, eg, Float, Double, String etc.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. </param>
	  /// <returns> value as Float, Double, etc.. </returns>
	  public virtual object getScalarObject(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getScalarObject(m);
	  }

	  /// <summary>
	  /// Get member data array of any type as an Object, eg, Float, Double, String etc.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. </param>
	  /// <returns> value as Float, Double, etc.. </returns>
	  public virtual object getScalarObject(StructureMembers.Member m)
	  {
		DataType dataType = m.DataType;
		// boolean isScalar = m.isScalar();

		if (dataType == DataType.DOUBLE)
		{
		  return getScalarDouble(m);

		}
		else if (dataType == DataType.FLOAT)
		{
		  return getScalarFloat(m);

		}
		else if (dataType.getPrimitiveClassType() == typeof(byte))
		{
		  return getScalarByte(m);

		}
		else if (dataType.getPrimitiveClassType() == typeof(short))
		{
		  return getScalarShort(m);

		}
		else if (dataType.getPrimitiveClassType() == typeof(int))
		{
		  return getScalarInt(m);

		}
		else if (dataType.getPrimitiveClassType() == typeof(long))
		{
		  return getScalarLong(m);

		}
		else if (dataType == DataType.CHAR)
		{
		  return getScalarString(m);

		}
		else if (dataType == DataType.STRING)
		{
		  return getScalarString(m);

		}
		else if (dataType == DataType.STRUCTURE)
		{
		  return getScalarStructure(m);

		}
		else if (dataType == DataType.SEQUENCE)
		{
		  return getArraySequence(m);
		}

		throw new Exception("Dont have implemenation for " + dataType);
	  }

	  /////////////////////////////////////////////////////////////////////////////////////////////


	  /// <summary>
	  /// Get scalar value as a float, with conversion as needed. Underlying type must be convertible to float.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be convertible to float. </param>
	  /// <returns> scalar value as a float </returns>
	  /// <exception cref="ForbiddenConversionException"> if not convertible to float. </exception>
	  public virtual float convertScalarFloat(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return convertScalarFloat(m);
	  }

	  /// <summary>
	  /// Get scalar value as a float, with conversion as needed. Underlying type must be convertible to float.
	  /// </summary>
	  /// <param name="m"> member Variable. </param>
	  /// <returns> scalar value as a float </returns>
	  /// <exception cref="ForbiddenConversionException"> if not convertible to float. </exception>
	  public abstract float convertScalarFloat(StructureMembers.Member m);

	  /// <summary>
	  /// Get scalar value as a double, with conversion as needed. Underlying type must be convertible to double.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be convertible to double. </param>
	  /// <returns> scalar value as a double </returns>
	  /// <exception cref="ForbiddenConversionException"> if not convertible to double. </exception>
	  public virtual double convertScalarDouble(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return convertScalarDouble(m);
	  }

	  /// <summary>
	  /// Get scalar value as a double, with conversion as needed. Underlying type must be convertible to double.
	  /// </summary>
	  /// <param name="m"> member Variable. </param>
	  /// <returns> scalar value as a double </returns>
	  /// <exception cref="ForbiddenConversionException"> if not convertible to double. </exception>
	  public abstract double convertScalarDouble(StructureMembers.Member m);

	  /// <summary>
	  /// Get scalar value as a int, with conversion as needed. Underlying type must be convertible to int.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be convertible to double. </param>
	  /// <returns> scalar value as a int </returns>
	  /// <exception cref="ForbiddenConversionException"> if not convertible to int. </exception>
	  public virtual int convertScalarInt(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return convertScalarInt(m);
	  }

	  /// <summary>
	  /// Get scalar value as a int, with conversion as needed. Underlying type must be convertible to int.
	  /// </summary>
	  /// <param name="m"> member Variable. </param>
	  /// <returns> scalar value as a int </returns>
	  /// <exception cref="ForbiddenConversionException"> if not convertible to int. </exception>
	  public abstract int convertScalarInt(StructureMembers.Member m);

	  /// <summary>
	  /// Get scalar value as a int, with conversion as needed. Underlying type must be convertible to int.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be convertible to double. </param>
	  /// <returns> scalar value as a int </returns>
	  /// <exception cref="ForbiddenConversionException"> if not convertible to int. </exception>
	  public virtual long convertScalarLong(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return convertScalarLong(m);
	  }

	  /// <summary>
	  /// Get scalar value as a int, with conversion as needed. Underlying type must be convertible to int.
	  /// </summary>
	  /// <param name="m"> member Variable. </param>
	  /// <returns> scalar value as a int </returns>
	  /// <exception cref="ForbiddenConversionException"> if not convertible to int. </exception>
	  public abstract long convertScalarLong(StructureMembers.Member m);

	  /////////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Get member data of type double.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type double. </param>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  /// <returns> scalar value as a double </returns>
	  public virtual double getScalarDouble(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getScalarDouble(m);
	  }

	  /// <summary>
	  /// Get member data of type double.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type double. </param>
	  /// <returns> scalar double value </returns>
	  public abstract double getScalarDouble(StructureMembers.Member m);

	  /// <summary>
	  /// Get java double array for a member of type double.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type double. </param>
	  /// <returns> 1D java array of doubles </returns>
	  public virtual double[] getJavaArrayDouble(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getJavaArrayDouble(m);
	  }

	  /// <summary>
	  /// Get java double array for a member of type double.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type double. </param>
	  /// <returns> 1D java array of doubles </returns>
	  public abstract double[] getJavaArrayDouble(StructureMembers.Member m);

	  ////////////////

	  /// <summary>
	  /// Get member data of type float.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type float. </param>
	  /// <returns> scalar float value </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual float getScalarFloat(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getScalarFloat(m);
	  }

	  /// <summary>
	  /// Get member data of type float.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
	  /// <returns> scalar double value </returns>
	  public abstract float getScalarFloat(StructureMembers.Member m);

	  /// <summary>
	  /// Get java float array for a member of type float.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type float. </param>
	  /// <returns> 1D java array of floats </returns>
	  public virtual float[] getJavaArrayFloat(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getJavaArrayFloat(m);
	  }

	  /// <summary>
	  /// Get java float array for a member of type float.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
	  /// <returns> 1D java array of floats </returns>
	  public abstract float[] getJavaArrayFloat(StructureMembers.Member m);

	  /////

	  /// <summary>
	  /// Get member data of type byte.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type byte. </param>
	  /// <returns> scalar byte value </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual byte getScalarByte(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getScalarByte(m);
	  }

	  /// <summary>
	  /// Get member data of type byte.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type byte. </param>
	  /// <returns> scalar byte value </returns>
	  public abstract byte getScalarByte(StructureMembers.Member m);

	  /// <summary>
	  /// Get java byte array for a member of type byte.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type byte. </param>
	  /// <returns> 1D java array of bytes </returns>
	  public virtual byte[] getJavaArrayByte(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getJavaArrayByte(m);
	  }

	  /// <summary>
	  /// Get java byte array for a member of type byte.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type byte. </param>
	  /// <returns> 1D java array of bytes </returns>
	  public abstract byte[] getJavaArrayByte(StructureMembers.Member m);

	  /////
	  /// <summary>
	  /// Get member data of type int.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type int. </param>
	  /// <returns> scalar int value </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual int getScalarInt(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getScalarInt(m);
	  }

	  /// <summary>
	  /// Get member data of type int.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type int. </param>
	  /// <returns> scalar int value </returns>
	  public abstract int getScalarInt(StructureMembers.Member m);

	  /// <summary>
	  /// Get java int array for a member of type int.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type int. </param>
	  /// <returns> 1D java array of ints </returns>
	  public virtual int[] getJavaArrayInt(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getJavaArrayInt(m);
	  }

	  /// <summary>
	  /// Get java int array for a member of type int.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type int. </param>
	  /// <returns> 1D java array of ints </returns>
	  public abstract int[] getJavaArrayInt(StructureMembers.Member m);

	  /////
	  /// <summary>
	  /// Get member data of type short.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type short. </param>
	  /// <returns> scalar short value </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual short getScalarShort(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getScalarShort(m);
	  }

	  /// <summary>
	  /// Get member data of type short.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type short. </param>
	  /// <returns> scalar short value </returns>
	  public abstract short getScalarShort(StructureMembers.Member m);

	  /// <summary>
	  /// Get java short array for a member of type short.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type short. </param>
	  /// <returns> 1D java array of shorts </returns>
	  public virtual short[] getJavaArrayShort(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getJavaArrayShort(m);
	  }

	  /// <summary>
	  /// Get java short array for a member of type short.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type short. </param>
	  /// <returns> 1D java array of shorts </returns>
	  public abstract short[] getJavaArrayShort(StructureMembers.Member m);

	  /////
	  /// <summary>
	  /// Get member data of type long.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type long. </param>
	  /// <returns> scalar long value </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual long getScalarLong(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getScalarLong(m);
	  }

	  /// <summary>
	  /// Get member data of type long.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type long. </param>
	  /// <returns> scalar long value </returns>
	  public abstract long getScalarLong(StructureMembers.Member m);

	  /// <summary>
	  /// Get java long array for a member of type long.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type long. </param>
	  /// <returns> 1D java array of longs </returns>
	  public virtual long[] getJavaArrayLong(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getJavaArrayLong(m);
	  }

	  /// <summary>
	  /// Get java long array for a member of type long.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type long. </param>
	  /// <returns> 1D java array of longs </returns>
	  public abstract long[] getJavaArrayLong(StructureMembers.Member m);

	  /////
	  /// <summary>
	  /// Get member data of type char.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type char. </param>
	  /// <returns> scalar char value </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual char getScalarChar(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getScalarChar(m);
	  }

	  /// <summary>
	  /// Get member data of type char.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
	  /// <returns> scalar char value </returns>
	  public abstract char getScalarChar(StructureMembers.Member m);

	  /// <summary>
	  /// Get java char array for a member of type char.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type char. </param>
	  /// <returns> 1D java array of chars </returns>
	  public virtual char[] getJavaArrayChar(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getJavaArrayChar(m);
	  }

	  /// <summary>
	  /// Get java char array for a member of type char.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
	  /// <returns> 1D java array of chars </returns>
	  public abstract char[] getJavaArrayChar(StructureMembers.Member m);

	  /////

	  /// <summary>
	  /// Get String value, from rank 0 String or rank 1 char member array.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. </param>
	  /// <returns> scalar String value </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual string getScalarString(string memberName)
	  {
		StructureMembers.Member m = findMember(memberName);
		if (null == m)
		{
		  throw new ArgumentException("Member not found= " + memberName);
		}
		return getScalarString(m);
	  }

	  /// <summary>
	  /// Get String value, from rank 0 String or rank 1 char member array.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type char or String. </param>
	  /// <returns> scalar String value </returns>
	  public abstract string getScalarString(StructureMembers.Member m);

	  /// <summary>
	  /// Get java String array for a member of type String.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type char or String. </param>
	  /// <returns> 1D java array of String </returns>
	  public virtual string[] getJavaArrayString(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getJavaArrayString(m);
	  }

	  /// <summary>
	  /// Get java array of Strings for a member of type char or String.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type char or String. </param>
	  /// <returns> 1D java array of String </returns>
	  public abstract string[] getJavaArrayString(StructureMembers.Member m);

	  ////

	  /// <summary>
	  /// Get member data of type Structure.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. </param>
	  /// <returns> scalar StructureData value </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual StructureData getScalarStructure(string memberName)
	  {
		StructureMembers.Member m = findMember(memberName);
		if (null == m)
		{
		  throw new ArgumentException("Member not found= " + memberName);
		}
		return getScalarStructure(m);
	  }

	  /// <summary>
	  /// Get member data of type Structure.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type Structure. </param>
	  /// <returns> StructureData </returns>
	  public abstract StructureData getScalarStructure(StructureMembers.Member m);

	  /// <summary>
	  /// Get member data of type Structure.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. </param>
	  /// <returns> array of StructureData </returns>
	  /// <exception cref="IllegalArgumentException"> if name is not legal member name. </exception>
	  public virtual ArrayStructure getArrayStructure(string memberName)
	  {
		StructureMembers.Member m = findMember(memberName);
		if (null == m)
		{
		  throw new ArgumentException("Member not found= " + memberName);
		}
		return getArrayStructure(m);
	  }

	  /// <summary>
	  /// Get ArrayStructure for a member of type Structure.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type Structure. </param>
	  /// <returns> ArrayStructure </returns>
	  public abstract ArrayStructure getArrayStructure(StructureMembers.Member m);

	  //////

	  /// <summary>
	  /// Get ArraySequence for a member of type Sequence.
	  /// </summary>
	  /// <param name="memberName"> name of member Variable. Must be of type Sequence. </param>
	  /// <returns> ArrayStructure </returns>
	  public virtual ArraySequence getArraySequence(string memberName)
	  {
		StructureMembers.Member m = members.findMember(memberName);
		if (m == null)
		{
		  throw new ArgumentException("illegal member name =" + memberName);
		}
		return getArraySequence(m);
	  }

	  /// <summary>
	  /// Get ArraySequence for a member of type Sequence.
	  /// </summary>
	  /// <param name="m"> get data from this StructureMembers.Member. Must be of type Sequence. </param>
	  /// <returns> ArrayStructure </returns>
	  public abstract ArraySequence getArraySequence(StructureMembers.Member m);

	  ////////////////////////////////////////////////////////////////////////////////////////////////
	  // debugging
	  public virtual void showInternal(Formatter f, Indent indent)
	  {
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		f.format("%sStructureData %s class=%s hash=0x%x%n", indent, members.Name, this.GetType().FullName, GetHashCode());
	  }

	  public virtual void showInternalMembers(Formatter f, Indent indent)
	  {
// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		f.format("%sStructureData %s class=%s hash=0x%x%n", indent, members.Name, this.GetType().FullName, GetHashCode());
		indent.incr();
		foreach (StructureMembers.Member m in Members)
		{
		  m.showInternal(f, indent);
		}
		indent.decr();
	  }

	  public override string ToString()
	  {
		return members.ToString();
	  }

	}

}
