// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2
{
	using ArrayStructure = NetcdfSharp.ucar.ma2.ArrayStructure;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcIndex = NetcdfSharp.ucar.ma2.NcIndex;
	using NcRange = NetcdfSharp.ucar.ma2.NcRange;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StructureData = NetcdfSharp.ucar.ma2.StructureData;
	using StructureDataIterator = NetcdfSharp.ucar.ma2.StructureDataIterator;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;

	/// <summary>
	/// A Structure is a type of Variable that contains other Variables, like a struct in C.
	/// A Structure can be scalar or multidimensional.
	/// <para>
	/// A call to structure.read() will read all of the data in a Structure,
	/// including nested structures, and returns an Array of StructureData, with all of the data in memory.
	/// If there is a nested sequence, the sequence data may be read into memory all at once, or it may be
	/// read in increments as the iteration proceeds.
	/// </para>
	/// <para>
	/// Generally, the programmer can assume that the data in one Structure are stored together,
	/// so that it is efficient to read an entire Structure, and then access the Variable data through the
	/// Arrays in the StructureData.
	/// 
	/// @author caron
	/// </para>
	/// </summary>

	public class Structure : Variable
	{
		private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Structure));
		private const int defaultBufferSize = 500 * 1000; // 500K bytes

		/// <summary>
		/// Create a Structure "from scratch". Also must call setDimensions().
		/// </summary>
		/// <param name="ncfile"> the containing NetcdfFile. </param>
		/// <param name="group"> the containing group; if null, use rootGroup </param>
		/// <param name="parent"> parent Structure, may be null </param>
		/// <param name="shortName"> variable shortName, must be unique within the Group </param>
		/// @deprecated Use Structure.builder() 
		[Obsolete("Use Structure.builder()")]
		public Structure(NetcdfFile ncfile, Group group, Structure parent, string shortName) : base(ncfile, group, parent, shortName)
		{
			DataType = DataType.STRUCTURE;
			this.elementSize = -1; // gotta wait before calculating
			members = new List<Variable>();
			memberHash = new Dictionary<string, Variable>();
		}

		/// <summary>
		/// Create a Structure "from scratch".
		/// </summary>
		/// <param name="ncfile"> the containing NetcdfFile. </param>
		/// <param name="group"> the containing group; if null, use rootGroup </param>
		/// <param name="parent"> parent Structure, may be null </param>
		/// <param name="shortName"> variable shortName, must be unique within the Group </param>
		/// <param name="dimList"> list of Dimensions </param>
		/// @deprecated Use Structure.builder() 
		[Obsolete("Use Structure.builder()")]
		public Structure(NetcdfFile ncfile, Group group, Structure parent, string shortName, IList<Dimension> dimList) : this(ncfile, group, parent, shortName)
		{
			setDimensions(dimList);
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from"> copy from this </param>
		/// @deprecated Use Structure.builder() 
		[Obsolete("Use Structure.builder()")]
		protected internal Structure(Structure from) : base(from)
		{ // , boolean reparent) {

			members = new List<Variable>(from.members);
			memberHash = new Dictionary<string, Variable>(from.memberHash);
			isSubset_Conflict = from.Subset;
		}

		/// <summary>
		/// Create a subset of the Structure consisting only of the given member variables
		/// </summary>
		/// <param name="memberNames"> list of Variable names, already a member </param>
		/// <returns> Structure containing just those members </returns>
		public virtual Structure select(IList<string> memberNames)
		{
			Structure result = copy();
			IList<Variable> members = new List<Variable>();
			foreach (string name in memberNames)
			{
				Variable m = findVariable(name);
				if (null != m)
				{
					members.Add(m);
				}
			}
			result.MemberVariables = members;
			result.isSubset_Conflict = true;
			return result;
		}

		/// <summary>
		/// Create a subset of the Structure consisting only of the one member variable
		/// </summary>
		/// <param name="varName"> name of member Variable </param>
		/// <returns> containing just that member </returns>
		public virtual Structure select(string varName)
		{
			IList<string> memberNames = new List<string>(1);
			memberNames.Add(varName);
			return select(memberNames);
		}

		/// <summary>
		/// Find if this was created from a subset() method.
		/// </summary>
		/// <returns> true if this is a subset </returns>
		public virtual bool Subset
		{
			get
			{
				return isSubset_Conflict;
			}
		}

		// for section and slice
		/// @deprecated Use <seealso cref="toBuilder()"/> 
		[Obsolete("Use <seealso cref=\"toBuilder()\"/>")]
		protected internal override Structure copy()
		{
			return new Structure(this);
		}

		protected internal virtual int calcStructureSize()
		{
			int structureSize = 0;
			foreach (Variable member in members)
			{
				structureSize += (int)(member.Size * member.ElementSize);
			}
			return structureSize;
		}

		/// <summary>
		/// Caching is not allowed </summary>
		public override bool Caching
		{
			get
			{
				return false;
			}
			set
			{
				this.cache.isCaching = false;
				this.cache.cachingSet = true;
			}
		}


		/// <summary>
		/// Add a member variable
		/// </summary>
		/// <param name="v"> add this variable as a member of this structure </param>
		/// <returns> the added variable </returns>
		/// @deprecated Use Structure.builder() 
		[Obsolete("Use Structure.builder()")]
		public virtual Variable addMemberVariable(Variable v)
		{
			if (Immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			members.Add(v);
			memberHash[v.ShortName] = v;
			v.ParentStructure = this;
			return v;
		}

		/// <summary>
		/// Set the list of member variables.
		/// </summary>
		/// <param name="vars"> this is the list of member variables </param>
		/// @deprecated Use Structure.builder() 
		[Obsolete("Use Structure.builder()")]
		public virtual IList<Variable> MemberVariables
		{
			set
			{
				if (Immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}
				members = new List<Variable>();
				memberHash = new Dictionary<string, Variable>(2 * value.Count);
				foreach (Variable v in value)
				{
					addMemberVariable(v);
				}
			}
		}

		/// <summary>
		/// Remove a Variable : uses the Variable name to find it.
		/// </summary>
		/// <param name="v"> remove this variable as a member of this structure </param>
		/// <returns> true if was found and removed </returns>
		/// @deprecated Use Structure.builder() 
		[Obsolete("Use Structure.builder()")]
		public virtual bool removeMemberVariable(Variable v)
		{
			if (Immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			if (v == null)
			{
				return false;
			}
			// smembers = null;
			for (int i = members.Count - 1; i >= 0; i--)
			{
				Variable mv = members[i];
				if (mv.ShortName.Equals(v.ShortName))
				{
					members.RemoveAt(i);
					memberHash.Remove(v.ShortName);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Set the parent group of this Structure, and all member variables.
		/// </summary>
		/// @deprecated Use Structure.builder() 
		[Obsolete("Use Structure.builder()")]
		public override Group ParentGroup
		{
			set
			{
				if (Immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}
				base.ParentGroup = value;
				if (members != null)
				{
					foreach (Variable v in members)
					{
						v.ParentGroup = value;
					}
				}
			}
		}

		/// @deprecated Use Structure.builder() 
		[Obsolete("Use Structure.builder()")]
		public override Variable setImmutable()
		{
			members = members.AsReadOnly();
			foreach (Variable m in members)
			{
				m.setImmutable();
			}

			base.setImmutable();
			return this;
		}

		/// <summary>
		/// Get the variables contained directly in this Structure. </summary>
		public virtual ImmutableList<Variable> Variables
		{
			get
			{
				return ImmutableList<Variable>.copyOf(members);
			}
		}

		/// <summary>
		/// Get the number of variables contained directly in this Structure. </summary>
		public virtual int NumberOfMemberVariables
		{
			get
			{
				return members.Count;
			}
		}

		/// <summary>
		/// Get the (short) names of the variables contained directly in this Structure. </summary>
		public virtual ImmutableList<string> VariableNames
		{
			get
			{
				// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
				return ImmutableList<string>.copyOf(members.Select(m => m.getShortName()));
			}
		}

		/// <summary>
		/// Find the Variable member with the specified (short) name.
		/// </summary>
		/// <param name="shortName"> name of the member variable. </param>
		/// <returns> the Variable member with the specified (short) name, or null if not found. </returns>
		public virtual Variable? findVariable(string shortName)
		{
			if (string.ReferenceEquals(shortName, null))
			{
				return null;
			}
			return memberHash[shortName];
		}

		/// <summary>
		/// Create a StructureMembers object that describes this Structure.
		/// CAUTION: Do not use for iterating over a StructureData or ArrayStructure - get the StructureMembers object
		/// directly from the StructureData or ArrayStructure.
		/// </summary>
		/// <returns> a StructureMembers object that describes this Structure. </returns>
		public virtual StructureMembers makeStructureMembers()
		{
			StructureMembers.Builder builder = StructureMembers.builder().setName(ShortName);
			foreach (Variable v2 in this.Variables)
			{
				StructureMembers.MemberBuilder m = builder.addMember(v2.ShortName, v2.Description, v2.UnitsString, v2.DataType, v2.Shape);
				if (v2 is Structure)
				{
					m.StructureMembers = ((Structure)v2).makeStructureMembers();
				}
			}
			return builder.build();
		}

		/// <summary>
		/// Get the size of one element of the Structure.
		/// </summary>
		/// <returns> size (in bytes) </returns>
		public override int ElementSize
		{
			get
			{
				if (elementSize <= 0)
				{
					calcElementSize();
				}
				return elementSize;
			}
		}

		/// <summary>
		/// Force recalculation of size of one element of this structure - equals the sum of sizes of its members.
		/// This is used only by low level classes like IOSPs.
		/// </summary>
		/// @deprecated will be private in ver6, where Structure will be immutable. 
		[Obsolete("will be private in ver6, where Structure will be immutable.")]
		public virtual void calcElementSize()
		{
			int total = 0;
			foreach (Variable v in members)
			{
				total += (int)(v.ElementSize * v.Size);
			}
			elementSize = total;
		}

		/// <summary>
		/// Use this when this is a scalar Structure. Its the same as read(), but it extracts the single
		/// StructureData out of the Array.
		/// </summary>
		/// <returns> StructureData for a scalar </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		/// @deprecated use readStructure(0) 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use readStructure(0)") public NetcdfSharp.ucar.ma2.StructureData readStructure() throws NetcdfSharp.java.io.IOException
		[Obsolete("use readStructure(0)")]
		public virtual StructureData readStructure()
		{
			if (Rank != 0)
			{
				throw new System.NotSupportedException("not a scalar structure");
			}
			NcArray dataArray = read();
			ArrayStructure data = (ArrayStructure)dataArray;
			return data.getStructureData(0);
		}

		/// <summary>
		/// Use this when this is a one dimensional array of Structures, or you are doing the index calculation yourself for
		/// a multidimension array. This will read only the ith structure, and return the data as a StructureData object.
		/// </summary>
		/// <param name="index"> index into 1D array </param>
		/// <returns> ith StructureData </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if index out of range </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.StructureData readStructure(int index) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		public virtual StructureData readStructure(int index)
		{
			Section.Builder sb = Section.builder();

			if (Rank == 1)
			{
				sb.appendRange(index, index);

			}
			else if (Rank > 1)
			{
				NcIndex ii = NcIndex.factory(shape); // convert to nD index
				ii.setCurrentCounter(index);
				int[] origin = ii.CurrentCounter;
				foreach (int anOrigin in origin)
				{
					sb.appendRange(anOrigin, anOrigin);
				}
			}

			NcArray dataArray = read(sb.build());
			ArrayStructure data = (ArrayStructure)dataArray;
			return data.getStructureData(0);
		}

		/// <summary>
		/// For rank 1 array of Structures, read count Structures and return the data as an ArrayStructure.
		/// Use only when this is a one dimensional array of Structures.
		/// </summary>
		/// <param name="start"> start at this index </param>
		/// <param name="count"> return this many StructureData </param>
		/// <returns> the StructureData recordsfrom start to start+count-1 </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if start, count out of range </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.ArrayStructure readStructure(int start, int count) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		public virtual ArrayStructure readStructure(int start, int count)
		{
			if (Rank != 1)
			{
				throw new System.NotSupportedException("not a vector structure");
			}
			int[] origin = new int[] { start };
			int[] shape = new int[] { count };
			if (NetcdfFile.debugStructureIterator)
			{
				Console.WriteLine("readStructure " + start + " " + count);
			}
			return (ArrayStructure)read(origin, shape);
		}

		/// <summary>
		/// Iterator over all the data in a Structure.
		/// 
		/// <pre>
		/// StructureDataIterator ii = structVariable.getStructureIterator();
		/// while (ii.hasNext()) {
		///   StructureData sdata = ii.next();
		/// }
		/// </pre>
		/// </summary>
		/// <returns> StructureDataIterator over type StructureData </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		/// <seealso cref=".getStructureIterator(int bufferSize)"/>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.StructureDataIterator getStructureIterator() throws NetcdfSharp.java.io.IOException
		public virtual StructureDataIterator StructureIterator
		{
			get
			{
				return getStructureIterator(defaultBufferSize);
			}
		}

		/// <summary>
		/// Get an efficient iterator over all the data in the Structure.
		/// 
		/// This is the efficient way to get all the data, it can be much faster than reading one record at a time,
		/// and is optimized for large datasets.
		/// This is accomplished by buffering bufferSize amount of data at once.
		/// 
		/// <pre>
		/// Example:
		/// 
		///  StructureDataIterator ii = structVariable.getStructureIterator(100 * 1000);
		///  while (ii.hasNext()) {
		///    StructureData sdata = ii.next();
		///  }
		/// </pre>
		/// </summary>
		/// <param name="bufferSize"> size in bytes to buffer, set < 0 to use default size </param>
		/// <returns> StructureDataIterator over type StructureData </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.StructureDataIterator getStructureIterator(int bufferSize) throws NetcdfSharp.java.io.IOException
		public virtual StructureDataIterator getStructureIterator(int bufferSize)
		{
			return (Rank < 2) ? new Structure.IteratorRank1(this, bufferSize) : new Structure.Iterator(this, bufferSize);
		}

		/// <summary>
		/// Iterator over type StructureData, rank 1 (common) case
		/// </summary>
		private class IteratorRank1 : StructureDataIterator
		{
			internal virtual void InitializeInstanceFields()
			{
				recnum = (int)outerInstance.Size;
			}

			private readonly Structure outerInstance;

			internal int count;
			internal int recnum;
			internal int readStart;
			internal int readCount;
			internal int readAtaTime;
			internal ArrayStructure @as;

			internal IteratorRank1(Structure outerInstance, int bufferSize)
			{
				this.outerInstance = outerInstance;

				InitializeInstanceFields();
				BufferSize = bufferSize;
			}

			public virtual bool hasNext()
			{
				return count < recnum;
			}

			public virtual StructureDataIterator reset()
			{
				count = 0;
				readStart = 0;
				readCount = 0;
				return this;
			}

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: @Override public NetcdfSharp.ucar.ma2.StructureData next() throws NetcdfSharp.java.io.IOException
			public virtual StructureData next()
			{
				if (count >= readStart)
				{
					readNext();
				}

				count++;
				return @as.getStructureData(readCount++);
			}

			public virtual int CurrentRecno
			{
				get
				{
					return count - 1;
				}
			}

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: private void readNext() throws NetcdfSharp.java.io.IOException
			internal virtual void readNext()
			{
				int left = Math.Min(recnum, readStart + readAtaTime); // dont go over recnum
				int need = left - readStart; // how many to read this time
				try
				{
					@as = outerInstance.readStructure(readStart, need);
					if (NetcdfFile.debugStructureIterator)
					{
						Console.WriteLine("readNext " + count + " " + readStart);
					}

				}
				catch (InvalidRangeException e)
				{
					log.error("Structure.IteratorRank1.readNext() ", e);
					throw new System.InvalidOperationException("Structure.Iterator.readNext() ", e);
				} // cant happen
				readStart += need;
				readCount = 0;
			}

			public virtual int BufferSize
			{
				set
				{
					if (count > 0)
					{
						return; // too late
					}
					int structureSize = outerInstance.calcStructureSize();
					if (structureSize <= 0)
					{
						structureSize = 1; // no members in the psuedo-structure LOOK is this ok?
					}
					if (value <= 0)
					{
						value = defaultBufferSize;
					}
					readAtaTime = Math.Max(10, value / structureSize);
					if (NetcdfFile.debugStructureIterator)
					{
						Console.WriteLine("Iterator structureSize= " + structureSize + " readAtaTime= " + readAtaTime);
					}
				}
			}
			public virtual void Dispose() { }
		}

		/// <summary>
		/// Iterator over type StructureData, general case
		/// </summary>
		private class Iterator : StructureDataIterator
		{
			private readonly Structure outerInstance;

			internal int count; // done so far
			internal int total; // total to do
			internal int readStart; // current buffer starts at
			internal int readCount; // count within the current buffer [0,readAtaTime)
			internal int outerCount; // over the outer Dimension
			internal ArrayStructure @as;

			internal Iterator(Structure outerInstance, int bufferSize)
			{
				this.outerInstance = outerInstance;
				reset();
			}

			public virtual bool hasNext()
			{
				return count < total;
			}

			public virtual StructureDataIterator reset()
			{
				count = 0;
				total = (int)outerInstance.Size;
				readStart = 0;
				readCount = 0;
				outerCount = 0;
				return this;
			}

			public virtual int CurrentRecno
			{
				get
				{
					return count - 1;
				}
			}

			public virtual StructureData next()
			{
				if (count >= readStart)
				{
					readNextGeneralRank();
				}

				count++;
				return @as.getStructureData(readCount++);
			}

			internal virtual void readNextGeneralRank()
			{
				try
				{
					Section.Builder sb = Section.builder().appendRanges(outerInstance.shape);
					sb.setRange(0, new NcRange(outerCount, outerCount));

					@as = (ArrayStructure)outerInstance.read(sb.build());

					if (NetcdfFile.debugStructureIterator)
					{
						Console.WriteLine("readNext inner=" + outerCount + " total=" + outerCount);
					}

					outerCount++;

				}
				catch (InvalidRangeException e)
				{
					log.error("Structure.Iterator.readNext() ", e);
					throw new System.InvalidOperationException("Structure.Iterator.readNext() ", e);
				} // cant happen

				readStart += (int)@as.Size;
				readCount = 0;
			}

			public virtual void Dispose() { }
		}

		////////////////////////////////////////////

		/// <summary>
		/// Get String with name and attributes. Used in short descriptions like tooltips.
		/// </summary>
		/// <returns> name and attributes String </returns>
		public virtual string getNameAndAttributes()
		{
			Formatter sbuff = new Formatter();
			sbuff.format("Structure ");
			getNameAndDimensions(sbuff, false, true);
			sbuff.format("%n");
			foreach (NcAttribute att in attributes())
			{
				sbuff.format("  %s:%s;%n", ShortName, att.ToString());
			}
			return sbuff.ToString();
		}

		protected internal override void writeCDL(Formatter buf, Indent indent, bool useFullName, bool strict)
		{
			buf.format("%n%s%s {%n", indent, dataType);

			indent.incr();
			foreach (Variable v in members)
			{
				v.writeCDL(buf, indent, useFullName, strict);
			}
			indent.decr();

			buf.format("%s} ", indent);
			getNameAndDimensions(buf, useFullName, strict);
			buf.format(";%s%n", extraInfo());

			foreach (NcAttribute att in attributes())
			{
				if (CDM.isspecial(att))
				{
					continue;
				}
				buf.format("%s", indent);
				att.writeCDL(buf, strict, ShortName);
				buf.format(";");
				if (!strict && (att.DataType != DataType.STRING))
				{
					buf.format(" // %s", att.DataType);
				}
				buf.format("%n");
			}
			buf.format("%n");
		}

		////////////////////////////////////////////////////////
		// TODO make private final and Immutable in release 6.
		protected internal IList<Variable> members;
		private IDictionary<string, Variable> memberHash;
		protected internal bool isSubset_Conflict;

		protected internal Structure(Variable.Builder builder, Group parentGroup) : base(builder, parentGroup)
		{
			BuilderStruct sb = (BuilderStruct)builder;
			foreach (Variable.Builder v in sb.vbuilders)
			{
				v.setParentStructure(this).setNcfile(sb.ncfile);
			}
			this.members = new List<Variable>();
			foreach (Variable.Builder vb in sb.vbuilders)
			{
				this.members.Add(vb.build(parentGroup));
			}
			memberHash = new Dictionary<string, Variable>();
			foreach (Variable m in this.members)
			{
				memberHash[m.ShortName] = m;
			}
			if (elementSize <= 0)
			{
				calcElementSize();
			}
		}

		/// <summary>
		/// Turn into a mutable Builder. Can use toBuilder().build() to copy. </summary>
		public override BuilderStruct toBuilder()
		{
			return addLocalFieldsToBuilder(builder1());
		}

		// Add local fields to the passed - in builder.
		protected internal virtual BuilderStruct addLocalFieldsToBuilder(BuilderStruct b)
		{
			foreach (Variable m in this.members)
			{
				b.addMemberVariable(m.toBuilder());
			}
			return (BuilderStruct)base.addLocalFieldsToBuilder(b);
		}
		// FIX for CS0108;CS0114
		/// <summary>
		/// Get Builder for this class that allows subclassing.
		/// </summary>
		public static BuilderStruct builder1()
		{
			return new BuilderStruct();
		}

		/// <summary>
		/// A builder of Structures. </summary>
		public class BuilderStruct : Variable.Builder
		{
			public IList<Variable.Builder> vbuilders = new List<Variable.Builder>();
			private bool built;
			// FIX for CS0108;CS0114
			public BuilderStruct setName1(string shortName)
			{
				base.setName(shortName);
				return this;
			}
			// FIX for CS0108;CS0114
			public BuilderStruct setParentGroupBuilder1(Group.Builder parent)
			{
				base.setParentGroupBuilder(parent);
				return this;
			}

			public BuilderStruct setParentStructureBuilder1(Structure.BuilderStruct structureBuilder)
			{
				base.setParentStructureBuilder(structureBuilder);
				return this;
			}

			public BuilderStruct setDataType1(DataType dataType)
			{
				base.setDataType(dataType);
				return this;
			}

			public virtual BuilderStruct addMemberVariable(Variable.Builder v)
			{
				vbuilders.Add(v);
				v.setParentStructureBuilder((Structure.BuilderStruct)this);
				return this;
			}

			public virtual BuilderStruct addMemberVariables(IList<Variable.Builder> vars)
			{
				vbuilders.AddRange(vars);
				return this;
			}

			/// <summary>
			/// Remove memeber variable, if present. Return whether it was present </summary>
			public virtual bool removeMemberVariable(string memberName)
			{
				Variable.Builder want = findMemberVariable(memberName);
				if (want != null)
				{
					vbuilders.Remove(want);
					return true;
				}
				return false;
			}

			/// <summary>
			/// Remove member variable, if present. Return whether it was present </summary>
			public virtual bool replaceMemberVariable(Variable.Builder replacement)
			{
				bool wasPresent = removeMemberVariable(replacement.shortName);
				addMemberVariable(replacement);
				return wasPresent;
			}

			public virtual Variable.Builder? findMemberVariable(string name)
			{
				foreach (Variable.Builder d in vbuilders)
				{
					if (d.shortName.Equals(name))
					{
						return d;
					}
				}
				return null;
			}

			/// <summary>
			/// Normally this is only called by Group.build() </summary>
			public override Structure build(Group parentGroup)
			{
				if (built)
				{
					throw new System.InvalidOperationException("already built");
				}
				built = true;
				this.setDataType1(DataType.STRUCTURE);
				return new Structure(this, parentGroup);
			}
		}
	}

}
