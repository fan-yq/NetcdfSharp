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
	/// A StructureData implementation that has its data self-contained.
	/// This is often the easiest to construct, but not very efficient for large arrays of Structures.
	/// You should call setMemberData() for each member.
	/// 
	/// @author caron
	/// </summary>
	public class StructureDataW : StructureData
	{
		protected internal readonly IDictionary<StructureMembers.Member, NcArray> memberData;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="members"> StructureData is always contained in a StructureArray. </param>
		public StructureDataW(StructureMembers members) : base(members)
		{
			memberData = new Dictionary<StructureMembers.Member, NcArray>(2 * members.Members.size());
		}

		/// @deprecated use StructureDataW(StructureMembers members) 
		[Obsolete("use StructureDataW(StructureMembers members)")]
		public StructureDataW(StructureMembers members, int size) : base(members)
		{
			memberData = new Dictionary<StructureMembers.Member, NcArray>(2 * size);
		}

		public virtual void setMemberData(StructureMembers.Member m, NcArray data)
		{
			if (data == null)
			{
				throw new ArgumentException("data cant be null");
			}

			memberData[m] = data;
		}

		public virtual void setMemberData(string memberName, NcArray data)
		{
			StructureMembers.Member m = members.findMember(memberName);
			if (m == null)
			{
				throw new ArgumentException("illegal member name =" + memberName);
			}
			setMemberData(m, data);
		}

		/// <summary>
		/// Get member data array of any type as an Array.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. </param>
		/// <returns> Array values. </returns>
		public override NcArray getArray(StructureMembers.Member m)
		{
			if (m == null)
			{
				throw new ArgumentException("member is null");
			}
			return memberData[m];
		}

		public override float convertScalarFloat(StructureMembers.Member m)
		{
			return getScalarFloat(m);
		}

		public override double convertScalarDouble(StructureMembers.Member m)
		{
			return getScalarDouble(m);
		}

		public override int convertScalarInt(StructureMembers.Member m)
		{
			return getScalarInt(m);
		}

		public override long convertScalarLong(StructureMembers.Member m)
		{
			return getScalarLong(m);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Get member data of type double.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type double. </param>
		/// <returns> scalar double value </returns>
		public override double getScalarDouble(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return data.getDouble(NcIndex.scalarIndexImmutable);
		}


		/// <summary>
		/// Get java double array for a member of type double.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type double. </param>
		/// <returns> 1D java array of doubles </returns>
		public override double[] getJavaArrayDouble(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return (double[])data.Storage;
		}

		////////////////

		/// <summary>
		/// Get member data of type float.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
		/// <returns> scalar double value </returns>
		public override float getScalarFloat(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return data.getFloat(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get java float array for a member of type float.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type float. </param>
		/// <returns> 1D java array of floats </returns>
		public override float[] getJavaArrayFloat(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return (float[])data.Storage;
		}

		/////

		/// <summary>
		/// Get member data of type byte.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type byte. </param>
		/// <returns> scalar byte value </returns>
		public override byte getScalarByte(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return data.getByte(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get java byte array for a member of type byte.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type byte. </param>
		/// <returns> 1D java array of bytes </returns>
		public override byte[] getJavaArrayByte(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return (byte[])data.Storage;
		}

		/////

		/// <summary>
		/// Get member data of type int.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type int. </param>
		/// <returns> scalar int value </returns>
		public override int getScalarInt(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return data.getInt(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get java int array for a member of type int.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type int. </param>
		/// <returns> 1D java array of ints </returns>
		public override int[] getJavaArrayInt(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return (int[])data.Storage;
		}

		/////

		/// <summary>
		/// Get member data of type short.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type short. </param>
		/// <returns> scalar short value </returns>
		public override short getScalarShort(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return data.getShort(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get java short array for a member of type short.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type short. </param>
		/// <returns> 1D java array of shorts </returns>
		public override short[] getJavaArrayShort(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return (short[])data.Storage;
		}


		/////

		/// <summary>
		/// Get member data of type long.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type long. </param>
		/// <returns> scalar long value </returns>
		public override long getScalarLong(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return data.getLong(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get java long array for a member of type long.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type long. </param>
		/// <returns> 1D java array of longs </returns>
		public override long[] getJavaArrayLong(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return (long[])data.Storage;
		}


		/////

		/// <summary>
		/// Get member data of type char.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
		/// <returns> scalar char value </returns>
		public override char getScalarChar(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return data.getChar(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get java char array for a member of type char.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type char. </param>
		/// <returns> 1D java array of chars </returns>
		public override char[] getJavaArrayChar(StructureMembers.Member m)
		{
			NcArray data = getArray(m);
			return (char[])data.Storage;
		}

		/// <summary>
		/// Get String value, from rank 0 String or rank 1 char member array.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type char or String. </param>
		public override string getScalarString(StructureMembers.Member m)
		{
			if (m.DataType == DataType.STRING)
			{
				NcArray data = getArray(m);
				if (data == null)
				{
					data = getArray(m);
				}
				return (string)data.getObject(0);
			}
			else
			{
				char[] ba = getJavaArrayChar(m);
				int count = 0;
				while (count < ba.Length)
				{
					if ((char)0 == ba[count])
					{
						break;
					}
					count++;
				}
				return new string(ba, 0, count);
			}
		}

		public override string[] getJavaArrayString(StructureMembers.Member m)
		{
			if (m.DataType == DataType.STRING)
			{
				NcArray data = getArray(m);
				int n = m.Size;
				string[] result = new string[n];
				for (int i = 0; i < result.Length; i++)
				{
					result[i] = (string)data.getObject(i);
				}
				return result;

			}
			else if (m.DataType == DataType.CHAR)
			{
				ArrayChar data = (ArrayChar)getArray(m);
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

		/// <summary>
		/// Get member data of type Structure.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type Structure. </param>
		/// <returns> StructureData </returns>
		public override StructureData getScalarStructure(StructureMembers.Member m)
		{
			ArrayStructure data = (ArrayStructure)getArray(m);
			return data.getStructureData(0);
		}

		/// <summary>
		/// Get member data of type Structure.
		/// </summary>
		/// <param name="m"> get data from this StructureMembers.Member. Must be of type Structure. </param>
		/// <returns> StructureData </returns>
		public override ArrayStructure getArrayStructure(StructureMembers.Member m)
		{
			return (ArrayStructure)getArray(m);
		}

		public override ArraySequence getArraySequence(StructureMembers.Member m)
		{
			return (ArraySequence)getArray(m);
		}

		public override void showInternal(Formatter f, Indent indent)
		{
			base.showInternal(f, indent);
			foreach (StructureMembers.Member m in memberData.Keys)
			{
				NcArray data = memberData[m];
				f.format("%s %s = %s%n", indent, m, data);
			}

		}

	}

}
