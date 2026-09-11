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
	using NetcdfSharp.ucar.ma2;

	/// <summary>
	/// Sequence is a one-dimensional Structure with indeterminate length.
	/// The only data access is through getStructureIterator().
	/// However, read() will read in the entire data and return an in-memory ArraySequence.
	/// 
	/// @author caron
	/// @since Feb 23, 2008
	/// </summary>
	public class Sequence : Structure
	{

		/// <summary>
		/// Sequence Constructor
		/// </summary>
		/// <param name="ncfile"> the containing NetcdfFile. </param>
		/// <param name="group"> the containing group; if null, use rootGroup </param>
		/// <param name="parent"> parent Structure, may be null </param>
		/// <param name="shortName"> variable shortName, must be unique within the Group </param>
		/// @deprecated use Builder. 
		[Obsolete("use Builder.")]
		public Sequence(NetcdfFile ncfile, Group group, Structure parent, string shortName) : base(ncfile, group, parent, shortName)
		{

			IList<Dimension> dims = new List<Dimension>();
			dims.Add(Dimension.VLEN);
			setDimensions(dims);
			DataType = DataType.SEQUENCE;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public StructureDataIterator getStructureIterator(int bufferSize) throws NetcdfSharp.java.io.IOException
		public override StructureDataIterator getStructureIterator(int bufferSize)
		{
			return ncfile.getStructureIterator(this, bufferSize);
		}

		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override NcArray read(int[] origin, int[] shape)
		{
			throw new System.NotSupportedException();
		}

		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override NcArray read(string sectionSpec)
		{
			throw new System.NotSupportedException();
		}

		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override NcArray read(IList<NcRange> ranges)
		{
			throw new System.NotSupportedException();
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public Array read(NetcdfSharp.ucar.ma2.Section section) throws NetcdfSharp.java.io.IOException
		public override NcArray read(NetcdfSharp.ucar.ma2.Section section)
		{
			return read();
		}

		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override StructureData readStructure()
		{
			throw new System.NotSupportedException();
		}

		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override StructureData readStructure(int index)
		{
			throw new System.NotSupportedException();
		}

		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override ArrayStructure readStructure(int start, int count)
		{
			throw new System.NotSupportedException();
		}

		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override Variable slice(int dim, int value)
		{
			throw new System.NotSupportedException();
		}

		/// <exception cref="UnsupportedOperationException"> always </exception>
		public override Variable section(Section subsection)
		{
			throw new System.NotSupportedException();
		}

		////////////////////////////////////////////////////////////////////////////////////////////

		// JAVA TO C# CONVERTER TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
		// ORIGINAL LINE: protected Sequence(Builder<?> builder, Group parentGroup)
		protected internal Sequence(Variable.Builder builder, Group parentGroup) : base(builder, parentGroup)
		{
		}

		/// <summary>
		/// Turn into a mutable Builder. Can use toBuilder().build() to copy. </summary>
		// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
		// ORIGINAL LINE: public Builder<?> toBuilder()
		public override BuilderSeq toBuilder()
		{
			return addLocalFieldsToBuilder(builderSeq());
		}

		// Add local fields to the passed - in builder.
		// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
		// ORIGINAL LINE: protected Builder<?> addLocalFieldsToBuilder(Builder<? extends Builder<?>> b)
		protected internal virtual BuilderSeq addLocalFieldsToBuilder(BuilderSeq b)
		{
			// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
			// ORIGINAL LINE: return (Builder<?>) super.addLocalFieldsToBuilder(b);
			return (BuilderSeq)base.addLocalFieldsToBuilder(b);
		}

		/// <summary>
		/// Get Builder for this class that allows subclassing.
		/// </summary>
		/// <seealso cref=""https://community.oracle.com/blogs/emcmanus/2010/10/24/using-builder-pattern-subclasses""/>
		// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
		// ORIGINAL LINE: public static Builder<?> builder()
		public static BuilderSeq builderSeq()
		{
			return new BuilderSeq();
		}

		/// <summary>
		/// A builder of Sequences </summary>
		public class BuilderSeq : Structure.BuilderStruct
		{
			internal bool built;

			public override Sequence build(Group parentGroup)
			{
				if (built)
				{
					throw new System.InvalidOperationException("already built");
				}
				built = true;
				this.setDataType1(DataType.SEQUENCE);
				return new Sequence(this, parentGroup);
			}
		}

	}

}
