// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// A Collection of members contained in a StructureData.
	/// TODO make immutable
	/// </summary>
	public sealed class StructureMembers
	{

		/// @deprecated use Builder 
		[Obsolete("use Builder")]
		public StructureMembers(string name)
		{
			this.name = name;
			members = new List<Member>();
		}

		/// @deprecated use toBuilder().build(false) to make a copy with no data 
		[Obsolete("use toBuilder().build(false) to make a copy with no data")]
		public StructureMembers(StructureMembers from)
		{
			this.name = from.name;
			members = new List<Member>(from.Members.size());
			foreach (Member m in from.members)
			{
				Member nm = new Member(m); // make copy - without the data info
				addMember(nm);
				if (m.members != null)
				{ // recurse
					nm.members = m.members.toBuilder(false).build();
				}
			}
		}

		/// <summary>
		/// Get the StructureMembers' name. </summary>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Add a member.
		/// </summary>
		/// @deprecated use Builder 
		[Obsolete("use Builder")]
		public void addMember(Member m)
		{
			members.Add(m);
		}

		/// <summary>
		/// Add a member at the given position.
		/// </summary>
		/// @deprecated use Builder 
		[Obsolete("use Builder")]
		public void addMember(int pos, Member m)
		{
			members.Insert(pos, m);
		}

		/// @deprecated use Builder 
		[Obsolete("use Builder")]
		public Member addMember(string name, string desc, string units, DataType dtype, int[] shape)
		{
			Member m = new Member(name, desc, units, dtype, shape);
			addMember(m);
			return m;
		}

		/// <summary>
		/// Remove the given member
		/// </summary>
		/// <param name="m"> member </param>
		/// <returns> position that it used to occupy, or -1 if not found </returns>
		/// @deprecated use Builder 
		[Obsolete("use Builder")]
		public int hideMember(Member m)
		{
			if (m == null)
			{
				return -1;
			}
			int index = members.IndexOf(m);
			members.Remove(m);
			return index;
		}

		/// <summary>
		/// Get the total size of the Structure in bytes.
		/// </summary>
		/// <returns> the total size of the Structure in bytes. </returns>
		public int StructureSize
		{
			get
			{
				if (structureSize < 0)
				{
					structureSize = calcStructureSize();
				}
				return structureSize;
			}
			set
			{
				this.structureSize = value;
			}
		}

		private int calcStructureSize()
		{
			int sum = 0;
			foreach (Member member in members)
			{
				sum += member.SizeBytes;
			}
			return sum;
		}


		/// <summary>
		/// Get the list of Member objects. </summary>
		public ImmutableList<Member> Members
		{
			get
			{
				return ImmutableList<Member>.copyOf(members);
			}
		}

		/// <summary>
		/// Get the names of the members. </summary>
		public ImmutableList<string> MemberNames
		{
			get
			{
				// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
				return ImmutableList<string>.copyOf(members.Select(m => m.name));
			}
		}

		/// <summary>
		/// Get the index-th member
		/// </summary>
		/// <param name="index"> of member </param>
		/// <returns> Member </returns>
		public Member getMember(int index)
		{
			return members[index];
		}

		/// <summary>
		/// Find the member by its name. </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Member findMember(String memberName)
		public Member findMember(string memberName)
		{
			if (string.ReferenceEquals(memberName, null))
			{
				return null;
			}

			return members.FirstOrDefault(m => m.name.Equals(memberName));
		}

		public override string ToString()
		{
			return MoreObjects.toStringHelper(this).add("name", name).add("members", members).add("structureSize", structureSize).ToString();
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is StructureMembers))
			{
				return false;
			}
			StructureMembers other = (StructureMembers)o;
			return Name.Equals(other.Name) && StructureSize == other.StructureSize && Members.size() == other.Members.size() && Members.All(m => m.Equals(other.findMember(m.name)));
		}

		public override int GetHashCode()
		{
			int result = 1;
			result = 31 * result + (Name != null ? Name.GetHashCode() : 0);
			result = 31 * result + StructureSize;
			result = 31 * result + Members.size();
			return result;
		}

		/// <summary>
		/// A member of a StructureData. </summary>
		public sealed class Member
		{
			// TODO make these final and immutable in 6.
			internal string name, desc, units;
			internal DataType dtype;
			internal int size = 1;
			internal int[] shape;
			internal StructureMembers members; // only if member is type Structure
			internal bool isVariableLength;

			// optional, use depends on ArrayStructure subclass
			internal NcArray dataArray;
			internal object dataObject;
			internal int dataParam;

			internal Member(MemberBuilder builder)
			{
				this.name = Preconditions.checkNotNull(builder.name);
				this.desc = builder.desc;
				this.units = builder.units;
				this.dtype = Preconditions.checkNotNull(builder.dtype);
				this.shape = builder.shape;
				this.members = builder.members;
				this.dataArray = builder.dataArray;
				this.dataObject = builder.dataObject;
				this.dataParam = builder.dataParam;

				this.size = (int)NcIndex.computeSize(shape);
				this.isVariableLength = (shape.Length > 0 && shape[shape.Length - 1] < 0);
			}

			/// <summary>
			/// Turn into a mutable Builder. Can use toBuilder().build() to copy. </summary>
			public MemberBuilder toBuilder(bool wantsData)
			{
				MemberBuilder b = (new MemberBuilder()).setName(this.name).setDesc(this.desc).setUnits(this.units).setDataType(this.dtype).setShape(this.shape);
				if (wantsData)
				{
					b.setDataArray(this.dataArray).setDataObject(this.dataObject).setDataParam(this.dataParam).StructureMembers = this.members;
				}
				else if (this.members != null)
				{
					b.StructureMembers = this.members.toBuilder(wantsData).build();
				}
				return b;
			}

			/// @deprecated use MemberBuilder 
			[Obsolete("use MemberBuilder")]
			public Member(string name, string desc, string units, DataType dtype, int[] shape)
			{
				this.name = Preconditions.checkNotNull(name);
				this.desc = desc;
				this.units = units;
				this.dtype = Preconditions.checkNotNull(dtype);
				Shape = shape;
			}

			/// @deprecated use MemberBuilder 
			[Obsolete("use MemberBuilder")]
			public Member(Member from)
			{
				this.name = from.name;
				this.desc = from.desc;
				this.units = from.units;
				this.dtype = from.dtype;
				StructureMembers = from.members;
				Shape = from.shape;
			}

			/// <summary>
			/// If member is type Structure, you must set its constituent members.
			/// </summary>
			/// <param name="members"> set to this value </param>
			/// <exception cref="IllegalArgumentException"> if {@code members} is this Member's enclosing class instance. </exception>
			/// @deprecated use MemberBuilder 
			[Obsolete("use MemberBuilder")]
			public StructureMembers StructureMembers
			{
				set
				{
					this.members = value;
				}
				get
				{
					return members;
				}
			}


			/// @deprecated use MemberBuilder 
			[Obsolete("use MemberBuilder")]
			public int[] Shape
			{
				set
				{
					this.shape = Preconditions.checkNotNull(value);
					this.size = (int)NcIndex.computeSize(value);
					this.isVariableLength = (value.Length > 0 && value[value.Length - 1] < 0);
				}
				get
				{
					return shape;
				}
			}

			/// <summary>
			/// Get the StructureMembers name. </summary>
			public string Name
			{
				get
				{
					return name;
				}
			}

			public string FullNameEscaped
			{
				get
				{
					return name;
				}
			}

			public string FullName
			{
				get
				{
					return name;
				}
			}

			/// <summary>
			/// Get the units string, if any.
			/// </summary>
			/// <returns> the units string, or null if none. </returns>
			public string UnitsString
			{
				get
				{
					return units;
				}
			}

			/// <summary>
			/// Get the description, if any.
			/// </summary>
			/// <returns> the description, or null if none. </returns>
			public string Description
			{
				get
				{
					return desc;
				}
			}

			/// <summary>
			/// Get the DataType.
			/// </summary>
			/// <returns> the DataType. </returns>
			public DataType DataType
			{
				get
				{
					return dtype;
				}
			}


			/// <summary>
			/// Get the total number of elements.
			/// This does not have to match the VariableSimpleIF.
			/// </summary>
			/// <returns> the total number of elements. </returns>
			public int Size
			{
				get
				{
					return size;
				}
			}

			public bool VariableLength
			{
				get
				{
					return isVariableLength;
				}
			}

			/// <summary>
			/// Get the total size in bytes. This does not have to match the VariableSimpleIF.
			/// 
			/// Note that this will not be correct when containing a member of type Sequence, or String, since those
			/// are variable length. In that case
			/// </summary>
			/// <returns> total size in bytes </returns>
			public int SizeBytes
			{
				get
				{
					if (DataType == NetcdfSharp.ucar.ma2.DataType.SEQUENCE)
					{
						return DataType.getSize();
					}
					else if (DataType == NetcdfSharp.ucar.ma2.DataType.STRING)
					{
						return DataType.getSize();
					}
					else if (DataType == NetcdfSharp.ucar.ma2.DataType.STRUCTURE)
					{
						return size * members.StructureSize;
					}
					// else if (this.isVariableLength())
					// return 0; // do not know
					else
					{
						return size * DataType.getSize();
					}
				}
			}

			/// <summary>
			/// Is this a scalar (size == 1).
			/// This does not have to match the VariableSimpleIF.
			/// </summary>
			/// <returns> if this is a scalar </returns>
			public bool Scalar
			{
				get
				{
					return size == 1;
				}
			}

			public override bool Equals(object o)
			{
				if (this == o)
				{
					return true;
				}
				if (!(o is Member))
				{
					return false;
				}
				Member other = (Member)o;
				return FullName.Equals(other.Name) && Objects.equal(Description, other.Description) && Objects.equal(UnitsString, other.UnitsString) && DataType == other.DataType && Size == other.Size && Shape.SequenceEqual(other.Shape) && Objects.equal(StructureMembers, other.StructureMembers);
			}

			public override int GetHashCode()
			{
				int result = 1;
				result = 31 * result + (FullName != null ? FullName.GetHashCode() : 0);
				result = 31 * result + (Description != null ? Description.GetHashCode() : 0);
				result = 31 * result + (UnitsString != null ? UnitsString.GetHashCode() : 0);
				result = 31 * result + DataType.GetHashCode();
				result = 31 * result + Size;
				result = 31 * result + (Shape != null ? Shape.Aggregate(1, (h, v) => 31 * h + v) : 0);
				result = 31 * result + (StructureMembers != null ? StructureMembers.GetHashCode() : 0);
				return result;
			}

			////////////////////////////////////////////////
			// these should not really be public

			/// <summary>
			/// Get the data parameter value, for use behind the scenes.
			/// </summary>
			/// <returns> data parameter value </returns>
			public int DataParam
			{
				get
				{
					return dataParam;
				}
				set
				{
					this.dataParam = value;
				}
			}


			/// <summary>
			/// Get the data array, if any. Used for implementation, DO NOT USE DIRECTLY!
			/// </summary>
			/// <returns> data object, may be null </returns>
			public NcArray DataArray
			{
				get
				{
					return dataArray;
				}
				set
				{
					this.dataArray = value;
				}
			}


			/// <summary>
			/// Get an opaque data object, for use behind the scenes. May be null
			/// </summary>
			/// <returns> data object, may be null </returns>
			public object DataObject
			{
				get
				{
					return dataObject;
				}
				set
				{
					this.dataObject = value;
				}
			}

			internal void showInternal(Formatter f, Indent indent)
			{
				f.format("%sname='%s' desc='%s' units='%s' dtype=%s size=%d dataObject=%s dataParam=%d", indent, name, desc, units, dtype, size, dataObject, dataParam);
				if (members != null)
				{
					indent.incr();
					f.format("%n%sNested members %s%n", indent, members.Name);
					foreach (StructureMembers.Member m in members.Members)
					{
						m.showInternal(f, indent);
					}
					indent.decr();
				}
				f.format("%n");
			}

			public override string ToString()
			{
				return name;
			}
		}

		public static MemberBuilder memberBuilder()
		{
			return new MemberBuilder();
		}

		public class MemberBuilder
		{
			internal string name, desc, units;
			internal DataType dtype;
			internal int[] shape;
			internal StructureMembers members; // only if member is type Structure

			// optional, use depends on ArrayStructure subclass
			internal NcArray dataArray;
			internal object dataObject;
			internal int dataParam;
			internal bool built;

			internal MemberBuilder()
			{
			}

			public virtual MemberBuilder setName(string name)
			{
				this.name = name;
				return this;
			}

			public virtual MemberBuilder setDesc(string desc)
			{
				this.desc = desc;
				return this;
			}

			public virtual MemberBuilder setUnits(string units)
			{
				this.units = units;
				return this;
			}

			public virtual MemberBuilder setDataType(DataType dtype)
			{
				this.dtype = dtype;
				return this;
			}

			public virtual MemberBuilder setDataArray(NcArray data)
			{
				this.dataArray = data;
				return this;
			}

			public virtual MemberBuilder setDataObject(object o)
			{
				this.dataObject = o;
				return this;
			}

			public virtual MemberBuilder setDataParam(int dataParam)
			{
				this.dataParam = dataParam;
				return this;
			}

			public virtual StructureMembers StructureMembers
			{
				set
				{
					this.members = value;
				}
				get
				{
					return members;
				}
			}

			public virtual MemberBuilder setShape(int[] shape)
			{
				this.shape = Preconditions.checkNotNull(shape);
				return this;
			}

			public virtual Member build()
			{
				if (built)
				{
					throw new System.InvalidOperationException("already built");
				}
				built = true;
				return new Member(this);
			}
		}

		///////////////////////////////////////////////////////////////////////////////

		// TODO make these final and immutable in 6.
		private string name;
		private List<Member> members;
		private int structureSize = -1;

		private StructureMembers(Builder builder)
		{
			this.name = builder.name == null ? "" : builder.name;
			this.members = builder.members.Select(mb => mb.build()).ToList();
			this.structureSize = builder.structureSize < 0 ? calcStructureSize() : builder.structureSize;
		}

		/// <summary>
		/// Turn into a mutable Builder. Can use toBuilder().build(wantsData) to copy.
		/// </summary>
		public Builder toBuilder(bool wantsData)
		{
			Builder b = builder().setName(this.name).setStructureSize(this.structureSize);
			this.members.ForEach(m => b.addMember(m.toBuilder(wantsData)));
			return b;
		}

		/// <summary>
		/// Create an StructureMembers builder.
		/// </summary>
		public static Builder builder()
		{
			return new Builder();
		}

		/// <summary>
		/// A builder for StructureMembers
		/// </summary>
		public class Builder
		{
			internal string name;
			internal List<MemberBuilder> members = new List<MemberBuilder>();
			internal int structureSize = -1;
			private bool built;

			internal Builder()
			{
			}

			public virtual Builder setName(string name)
			{
				this.name = name;
				return this;
			}

			public virtual Builder addMember(MemberBuilder m)
			{
				members.Add(m);
				return this;
			}

			public virtual Builder addMember(int pos, MemberBuilder m)
			{
				members.Insert(pos, m);
				return this;
			}

			public virtual MemberBuilder addMember(string name, string desc, string units, DataType dtype, int[] shape)
			{
				MemberBuilder m = new MemberBuilder().setName(name).setDesc(desc).setUnits(units).setDataType(dtype).setShape(shape);
				addMember(m);
				return m;
			}

			/// <summary>
			/// Add a Member, creating a scalar DataArray from the val. Use with StructureDataFromMember.
			/// </summary>
			public virtual MemberBuilder addMemberScalar(string name, string desc, string units, DataType dtype, object val)
			{
				MemberBuilder mb = addMember(name, desc, units, dtype, new int[0]);
				NcArray data = null;
				switch (dtype)
				{
					case NetcdfSharp.ucar.ma2.DataType.UBYTE:
					case NetcdfSharp.ucar.ma2.DataType.BYTE:
					case NetcdfSharp.ucar.ma2.DataType.ENUM1:
						data = new ArrayByte.D0(dtype.isUnsigned());
						data.setByte(0, Convert.ToByte(val));
						break;
					case NetcdfSharp.ucar.ma2.DataType.SHORT:
					case NetcdfSharp.ucar.ma2.DataType.USHORT:
					case NetcdfSharp.ucar.ma2.DataType.ENUM2:
						data = new ArrayShort.D0(dtype.isUnsigned());
						data.setShort(0, Convert.ToInt16(val));
						break;
					case NetcdfSharp.ucar.ma2.DataType.INT:
					case NetcdfSharp.ucar.ma2.DataType.UINT:
					case NetcdfSharp.ucar.ma2.DataType.ENUM4:
						data = new ArrayInt.D0(dtype.isUnsigned());
						data.setInt(0, Convert.ToInt32(val));
						break;
					case NetcdfSharp.ucar.ma2.DataType.LONG:
					case NetcdfSharp.ucar.ma2.DataType.ULONG:
						data = new ArrayLong.D0(dtype.isUnsigned());
						data.setDouble(0, Convert.ToInt64(val));
						break;
					case NetcdfSharp.ucar.ma2.DataType.FLOAT:
						data = new ArrayFloat.D0();
						data.setFloat(0, Convert.ToSingle(val));
						break;
					case NetcdfSharp.ucar.ma2.DataType.DOUBLE:
						data = new ArrayDouble.D0();
						data.setDouble(0, Convert.ToDouble(val));
						break;
				}
				mb.setDataArray(data);
				return mb;
			}

			/// <summary>
			/// Add a Member, creating a CHAR DataArray from the String val. Use with StructureDataFromMember.
			/// </summary>
			public virtual MemberBuilder addMemberString(string name, string desc, string units, string val, int max_len)
			{
				MemberBuilder mb = addMember(name, desc, units, DataType.CHAR, new int[] { max_len });
				NcArray data = ArrayChar.makeFromString(val, max_len);
				mb.setDataArray(data);
				return mb;
			}

			public virtual bool hasMember(string memberName)
			{
				return members.Any(m => m.name.Equals(memberName));
			}

			public virtual Builder setStructureSize(int structureSize)
			{
				this.structureSize = structureSize;
				return this;
			}

			public virtual StructureMembers build()
			{
				if (built)
				{
					throw new System.InvalidOperationException("already built");
				}
				built = true;
				return new StructureMembers(this);
			}
		}

	}
}
