// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2
{
	using NetcdfSharp.ucar.ma2;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using CF = NetcdfSharp.ucar.nc2.constants.CF;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using IospHelper = NetcdfSharp.ucar.nc2.iosp.IospHelper;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// A Variable is a logical container for data. It has a dataType, a set of Dimensions that define its array shape,
	/// and optionally a set of Attributes.
	/// <p/>
	/// The data is a multidimensional array of primitive types, Strings, or Structures.
	/// Data access is done through the read() methods, which return a memory resident Array.
	/// <para>
	/// Immutable if setImmutable() was called.
	/// TODO Variable will be immutable in 6.
	/// TODO Variable will not implement AttributeContainer in 6, use Variable.attributes().
	/// TODO Variable will not extend CDMNode in 6.
	/// 
	/// @author caron
	/// </para>
	/// </summary>
	/// <seealso cref="NetcdfSharp.ucar.ma2.Array"/>
	/// <seealso cref="NetcdfSharp.ucar.ma2.DataType"/>
	public class Variable : CDMNode, VariableSimpleIF, ProxyReader, AttributeContainer
	{
		private void InitializeInstanceFields()
		{
			proxyReader = this;
		}

		/// <summary>
		/// Globally permit or prohibit caching. For use during testing and debugging.
		/// <para>
		/// A {@code true} value for this field does not indicate whether a Variable
		/// <seealso cref="isCaching() is caching"/>, only that it's <i>permitted</i> to cache.
		/// </para>
		/// </summary>
		public static bool permitCaching = true;

		public static int defaultSizeToCache = 4000; // bytes cache any variable whose size() < defaultSizeToCache
		public static int defaultCoordsSizeToCache = 40 * 1000; // bytes cache coordinate variable whose size() <
																// defaultSizeToCache
		protected internal static bool debugCaching;
		private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Variable));

		/// <summary>
		/// Get the data type of the Variable.
		/// </summary>
		public virtual DataType DataType
		{
			get
			{
				return dataType;
			}
			set
			{
				if (immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}
				this.dataType = value;
				this.elementSize = DataType.getSize();
			}
		}

		/// <summary>
		/// Get the shape: length of Variable in each dimension.
		/// A scalar (rank 0) will have an int[0] shape.
		/// </summary>
		/// <returns> int array whose length is the rank of this Variable
		///         and whose values equal the length of that Dimension. </returns>
		public virtual int[] Shape
		{
			get
			{
				int[] result = new int[shape.Length]; // optimization over clone()
				System.Array.Copy(shape, 0, result, 0, shape.Length);
				return result;
			}
		}

		/// <summary>
		/// Get the size of the ith dimension
		/// </summary>
		/// <param name="index"> which dimension </param>
		/// <returns> size of the ith dimension </returns>
		public virtual int getShape(int index)
		{
			return shape[index];
		}

		/// <summary>
		/// Get the total number of elements in the Variable.
		/// If this is an unlimited Variable, will use the current number of elements.
		/// If this is a Sequence, will return 1.
		/// If variable length, will skip vlen dimensions
		/// </summary>
		/// <returns> total number of elements in the Variable. </returns>
		public virtual long Size
		{
			get
			{
				long size = 1;
				foreach (int aShape in shape)
				{
					if (aShape >= 0)
					{
						size *= aShape;
					}
				}
				return size;
			}
		}

		/// <summary>
		/// Get the number of bytes for one element of this Variable.
		/// For Variables of primitive type, this is equal to getDataType().getSize().
		/// Variables of String type don't know their size, so what they return is undefined.
		/// Variables of Structure type return the total number of bytes for all the members of
		/// one Structure, plus possibly some extra padding, depending on the underlying format.
		/// Variables of Sequence type return the number of bytes of one element.
		/// </summary>
		/// <returns> total number of bytes for the Variable </returns>
		public virtual int ElementSize
		{
			get
			{
				return elementSize;
			}
			set
			{
				if (immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}
				this.elementSize = value;
			}
		}

		/// <summary>
		/// Get the number of dimensions of the Variable, aka the rank. </summary>
		public virtual int Rank
		{
			get
			{
				return shape.Length;
			}
		}

		/// <summary>
		/// Get the parent group, or if null, the root group.
		/// </summary>
		/// @deprecated Will go away in ver6, shouldn't be needed when builders are used. 
		[Obsolete("Will go away in ver6, shouldn't be needed when builders are used.")]
		public virtual Group ParentGroupOrRoot
		{
			get
			{
				Group g = this.ParentGroup;
				if (g == null)
				{
					g = ncfile.RootGroup;
					// super.setParentGroup(g); // TODO: WTF?
				}
				return g;
			}
		}

		/// <summary>
		/// Is this variable metadata?. True if its values need to be included explicitly in NcML output.
		/// </summary>
		/// <returns> true if Variable values need to be included in NcML </returns>
		public virtual bool Metadata
		{
			get
			{
				return cache != null && cache.isMetadata;
			}
		}

		/// <summary>
		/// Whether this is a scalar Variable (rank == 0).
		/// </summary>
		/// <returns> true if Variable has rank 0 </returns>
		public virtual bool Scalar
		{
			get
			{
				return Rank == 0;
			}
		}

		/// <summary>
		/// Does this variable have a variable length dimension?
		/// If so, it has as one of its dimensions Dimension.VLEN.
		/// </summary>
		/// <returns> true if Variable has a variable length dimension? </returns>
		public virtual bool VariableLength
		{
			get
			{
				return isVariableLength_Conflict;
			}
		}

		/// <summary>
		/// Can this variable's size grow?.
		/// This is equivalent to saying at least one of its dimensions is unlimited.
		/// </summary>
		/// <returns> boolean true iff this variable can grow </returns>
		public virtual bool Unlimited
		{
			get
			{
				foreach (Dimension d in dimensions)
				{
					if (d.Unlimited)
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Get the list of dimensions used by this variable.
		/// The most slowly varying (leftmost for Java and C programmers) dimension is first.
		/// For scalar variables, the list is empty.
		/// </summary>
		/// <returns> List<Dimension>, will be ImmutableList in ver 6. </returns>
		public virtual IList<Dimension> Dimensions
		{
			get
			{
				return ImmutableList<Dimension>.copyOf(dimensions);
			}
		}

		/// <summary>
		/// Get the ith dimension.
		/// </summary>
		/// <param name="i"> index of the dimension. </param>
		/// <returns> requested Dimension, or null if i is out of bounds. </returns>
		public virtual Dimension getDimension(int i)
		{
			if ((i < 0) || (i >= Rank))
			{
				return null;
			}
			return dimensions[i];
		}

		/// <summary>
		/// Get the list of Dimension names, space delineated.
		/// </summary>
		/// <returns> Dimension names, space delineated </returns>
		public virtual string DimensionsString
		{
			get
			{
				return NetcdfSharp.ucar.nc2.Dimensions.makeDimensionsString(dimensions);
			}
		}

		/// <summary>
		/// Find the index of the named Dimension in this Variable.
		/// </summary>
		/// <param name="name"> the name of the dimension </param>
		/// <returns> the index of the named Dimension, or -1 if not found. </returns>
		public virtual int findDimensionIndex(string name)
		{
			for (int i = 0; i < dimensions.Count; i++)
			{
				Dimension d = dimensions[i];
				if (name.Equals(d.ShortName))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Get the description of the Variable.
		/// Default is to use CDM.LONG_NAME attribute value. If not exist, look for "description", "title", or
		/// "standard_name" attribute value (in that order).
		/// </summary>
		/// <returns> description, or null if not found. </returns>
		public virtual string Description
		{
			get
			{
				string desc = null;
				NcAttribute att = findAttributeIgnoreCase(CDM.LONG_NAME);
				if ((att != null) && att.String)
				{
					desc = att.StringValue;
				}

				if (string.ReferenceEquals(desc, null))
				{
					att = findAttributeIgnoreCase("description");
					if ((att != null) && att.String)
					{
						desc = att.StringValue;
					}
				}

				if (string.ReferenceEquals(desc, null))
				{
					att = findAttributeIgnoreCase(CDM.TITLE);
					if ((att != null) && att.String)
					{
						desc = att.StringValue;
					}
				}

				if (string.ReferenceEquals(desc, null))
				{
					att = findAttributeIgnoreCase(CF.STANDARD_NAME);
					if ((att != null) && att.String)
					{
						desc = att.StringValue;
					}
				}

				return desc;
			}
		}

		/// <summary>
		/// Get the Unit String for the Variable.
		/// Looks for the CDM.UNITS attribute value
		/// </summary>
		/// <returns> unit string, or null if not found. </returns>
		public virtual string UnitsString
		{
			get
			{
				string units = null;
				NcAttribute att = attributes().findAttributeIgnoreCase(CDM.UNITS);
				if ((att != null) && att.String)
				{
					units = att.StringValue;
					if (!string.ReferenceEquals(units, null))
					{
						units = units.Trim();
					}
				}
				return units;
			}
		}

		/// <summary>
		/// Get shape as a List of Range objects.
		/// The List is immutable.
		/// </summary>
		/// <returns> List of Ranges, one for each Dimension. </returns>
		public virtual ImmutableList<NcRange> Ranges
		{
			get
			{
				// Ok to use Immutable as there are no nulls.
				return ImmutableList<NcRange>.copyOf(ShapeAsSection.Ranges);
			}
		}

		/// <summary>
		/// Get shape as a Section object.
		/// </summary>
		/// <returns> Section containing List<Range>, one for each Dimension. </returns>
		public virtual Section ShapeAsSection
		{
			get
			{
				if (shapeAsSection == null)
				{
					shapeAsSection = NetcdfSharp.ucar.nc2.Dimensions.makeSectionFromDimensions(this.dimensions).build();
				}
				return shapeAsSection;
			}
		}

		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual ProxyReader ProxyReader
		{
			get
			{
				return proxyReader;
			}
			set
			{
				this.proxyReader = value;
			}
		}


		/// <summary>
		/// Create a new Variable that is a logical subsection of this Variable.
		/// No data is read until a read method is called on it.
		/// </summary>
		/// <param name="ranges"> List of type NetcdfSharp.ucar.ma2.Range, with size equal to getRank().
		///        Each Range corresponds to a Dimension, and specifies the section of data to read in that Dimension.
		///        A Range object may be null, which means use the entire dimension. </param>
		/// <returns> a new Variable which is a logical section of this Variable. </returns>
		/// <exception cref="InvalidRangeException"> if shape and range list don't match </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Variable section(List<Range> ranges) throws InvalidRangeException
		public virtual Variable section(IList<NcRange> ranges)
		{
			return section((new Section(ranges, shape)).makeImmutable());
		}

		/// <summary>
		/// Create a new Variable that is a logical subsection of this Variable.
		/// No data is read until a read method is called on it.
		/// </summary>
		/// <param name="subsection"> Section of this variable.
		///        Each Range in the section corresponds to a Dimension, and specifies the section of data to read in that
		///        Dimension.
		///        A Range object may be null, which means use the entire dimension. </param>
		/// <returns> a new Variable which is a logical section of this Variable. </returns>
		/// <exception cref="InvalidRangeException"> if section not compatible with shape </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Variable section(Section subsection) throws InvalidRangeException
		public virtual Variable section(Section subsection)
		{

			subsection = Section.fill(subsection, this.shape);

			// create a copy of this variable with a proxy reader
			Variable sectionV = copy(); // subclasses must override
			sectionV.ProxyReader = new SectionReader(this, subsection);
			sectionV.shape = subsection.Shape;
			sectionV.createNewCache(); // dont share the cache
			sectionV.Caching = false; // dont cache

			// replace dimensions if needed !! LOOK not shared
			int[] shape = subsection.Shape;
			IList<Dimension> dimensions = new List<Dimension>();
			for (int i = 0; i < Rank; i++)
			{
				Dimension oldD = getDimension(i);
				Dimension newD = (oldD.Length == shape[i]) ? oldD : Dimension.builder().setName(oldD.ShortName).setIsUnlimited(oldD.Unlimited).setIsShared(false).setLength(shape[i]).build();
				dimensions.Add(newD);
			}
			sectionV.dimensions = dimensions;
			return sectionV;
		}

		/// <summary>
		/// Create a new Variable that is a logical slice of this Variable, by
		/// fixing the specified dimension at the specified index value. This reduces rank by 1.
		/// No data is read until a read method is called on it.
		/// </summary>
		/// <param name="dim"> which dimension to fix </param>
		/// <param name="value"> at what index value </param>
		/// <returns> a new Variable which is a logical slice of this Variable. </returns>
		/// <exception cref="InvalidRangeException"> if dimension or value is illegal </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Variable slice(int dim, int value) throws InvalidRangeException
		public virtual Variable slice(int dim, int value)
		{
			if ((dim < 0) || (dim >= shape.Length))
			{
				throw new InvalidRangeException("Slice dim invalid= " + dim);
			}

			// ok to make slice of record dimension with length 0
			bool recordSliceOk = false;
			if ((dim == 0) && (value == 0))
			{
				Dimension d = getDimension(0);
				recordSliceOk = d.Unlimited;
			}

			// otherwise check slice in range
			if (!recordSliceOk)
			{
				if ((value < 0) || (value >= shape[dim]))
				{
					throw new InvalidRangeException("Slice value invalid= " + value + " for dimension " + dim);
				}
			}

			// create a copy of this variable with a proxy reader
			Variable.Builder sliceV = this.toBuilder(); // subclasses override toBuilder()
			Section.Builder slice = Section.builder().appendRanges(Shape);
			slice.replaceRange(dim, new NcRange(value, value));
			sliceV.setProxyReader(new SliceReader(this, dim, slice.build()));
			sliceV.resetCache(); // dont share the cache
			sliceV.setCaching(false); // dont cache

			// remove that dimension - reduce rank
			sliceV.dimensions.RemoveAt(dim);
			return sliceV.build(ParentGroupOrRoot);
		}

		/// <summary>
		/// Create a new Variable that is a logical view of this Variable, by
		/// eliminating the specified dimension(s) of length 1.
		/// No data is read until a read method is called on it.
		/// </summary>
		/// <param name="dims"> list of dimensions of length 1 to reduce </param>
		/// <returns> a new Variable which is a logical slice of this Variable. </returns>
		public virtual Variable reduce(IList<Dimension> dims)
		{
			IList<int> dimIdx = new List<int>(dims.Count);
			foreach (Dimension d in dims)
			{
				Debug.Assert(dimensions.Contains(d));
				Debug.Assert(d.Length == 1);
				dimIdx.Add(dimensions.IndexOf(d));
			}

			// create a copy of this variable with a proxy reader
			Variable.Builder sliceV = this.toBuilder(); // subclasses override toBuilder()
			sliceV.setProxyReader(new ReduceReader(this, dimIdx));
			sliceV.resetCache(); // dont share the cache
			sliceV.setCaching(false); // dont cache

			// remove dimension(s) - reduce rank
			foreach (Dimension d in dims)
			{
				sliceV.dimensions.Remove(d);
			}
			return sliceV.build(ParentGroupOrRoot);
		}

		/// @deprecated Use <seealso cref="toBuilder()"/> 
		[Obsolete("Use <seealso cref=\"toBuilder()\"/>")]
		protected internal virtual Variable copy()
		{
			return new Variable(this);
		}

		/// <summary>
		/// Get the NetcdfFile that this variable is contained in. May be null. </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public NetcdfFile getNetcdfFile()
		public virtual NetcdfFile NetcdfFile
		{
			get
			{
				return ncfile;
			}
		}

		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public String getFileTypeId()
		public virtual string FileTypeId
		{
			get
			{
				return ncfile == null ? null : ncfile.FileTypeId;
			}
		}

		public virtual string getFileTypeId() => FileTypeId;

		//////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Lookup the enum string for this value.
		/// Can only be called on enum types, where dataType.isEnum() is true.
		/// </summary>
		/// <param name="val"> the integer value of this enum </param>
		/// <returns> the String value </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public String lookupEnumString(int val)
		public virtual string lookupEnumString(int val)
		{
			if (!dataType.isEnum())
			{
				throw new System.NotSupportedException("Can only call Variable.lookupEnumVal() on enum types");
			}
			return enumTypedef.lookupEnumString(val);
		}

		/// <summary>
		/// Public by accident.
		/// </summary>
		/// <param name="enumTypedef"> set the EnumTypedef, only use if getDataType.isEnum() </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual EnumTypedef EnumTypedef
		{
			set
			{
				if (immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}
				if (!dataType.isEnum())
				{
					throw new System.NotSupportedException("Can only call Variable.setEnumTypedef() on enum types");
				}
				this.enumTypedef = value;
			}
			get
			{
				return enumTypedef;
			}
		}


		//////////////////////////////////////////////////////////////////////////////
		// IO
		// implementation notes to subclassers
		// all other calls use them, so override only these:
		// _read()
		// _read(Section section)
		// _readNestedData(Section section, boolean flatten)

		/// <summary>
		/// Read a section of the data for this Variable and return a memory resident Array.
		/// The Array has the same element type as the Variable, and the requested shape.
		/// Note that this does not do rank reduction, so the returned Array has the same rank
		/// as the Variable. Use Array.reduce() for rank reduction.
		/// <p/>
		/// <code>assert(origin[ii] + shape[ii]*stride[ii] <= Variable.shape[ii]); </code>
		/// <p/>
		/// </summary>
		/// <param name="origin"> int array specifying the starting index. If null, assume all zeroes. </param>
		/// <param name="shape"> int array specifying the extents in each dimension.
		///        This becomes the shape of the returned Array. </param>
		/// <returns> the requested data in a memory-resident Array </returns>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Array read(int[] origin, int[] shape) throws IOException, InvalidRangeException
		public virtual NcArray read(int[] origin, int[] shape)
		{
			if ((origin == null) && (shape == null))
			{
				return read();
			}

			if (origin == null)
			{
				return read(new Section(shape));
			}

			if (shape == null)
			{
				return read(new Section(origin, this.shape));
			}

			return read(new Section(origin, shape));
		}

		/// <summary>
		/// Read data section specified by a "section selector", and return a memory resident Array. Uses
		/// Fortran 90 array section syntax.
		/// </summary>
		/// <param name="sectionSpec"> specification string, eg "1:2,10,:,1:100:10". May optionally have (). </param>
		/// <returns> the requested data in a memory-resident Array </returns>
		public virtual NcArray read(string sectionSpec)
		{
			return read(new Section(sectionSpec));
		}

		/// <summary>
		/// Read a section of the data for this Variable from the netcdf file and return a memory resident Array.
		/// </summary>
		/// <param name="ranges"> list of Range specifying the section of data to read. </param>
		/// <returns> the requested data in a memory-resident Array </returns>
		public virtual NcArray read(IList<NcRange> ranges)
		{
			if (null == ranges)
			{
				return _read();
			}
			return read(new Section(ranges));
		}

		/// <summary>
		/// Read a section of the data for this Variable from the netcdf file and return a memory resident Array.
		/// The Array has the same element type as the Variable, and the requested shape.
		/// Note that this does not do rank reduction, so the returned Array has the same rank
		/// as the Variable. Use Array.reduce() for rank reduction.
		/// </summary>
		/// <param name="section"> list of Range specifying the section of data to read.
		///        Must be null or same rank as variable.
		///        If list is null, assume all data. </param>
		/// <returns> the requested data in a memory-resident Array </returns>
		public virtual NcArray read(Section section)
		{
			return (section == null) ? _read() : _read(Section.fill(section, shape));
		}

		/// <summary>
		/// Read all the data for this Variable and return a memory resident Array.
		/// The Array has the same element type and shape as the Variable.
		/// </summary>
		/// <returns> the requested data in a memory-resident Array. </returns>
		public virtual NcArray read()
		{
			return _read();
		}

		///// scalar reading

		/// <summary>
		/// Get the value as a byte for a scalar Variable. May also be one-dimensional of length 1.
		/// </summary>
		/// <returns> value as a byte </returns>
		public virtual byte readScalarByte()
		{
			NcArray data = _readScalarData();
			return data.getByte(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get the value as a short for a scalar Variable. May also be one-dimensional of length 1.
		/// </summary>
		/// <returns> value as a short </returns>
		public virtual short readScalarShort()
		{
			NcArray data = _readScalarData();
			return data.getShort(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get the value as a int for a scalar Variable. May also be one-dimensional of length 1.
		/// </summary>
		/// <returns> value as a int </returns>
		public virtual int readScalarInt()
		{
			NcArray data = _readScalarData();
			return data.getInt(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get the value as a long for a scalar Variable. May also be one-dimensional of length 1.
		/// </summary>
		/// <returns> value as a long </returns>
		public virtual long readScalarLong()
		{
			NcArray data = _readScalarData();
			return data.getLong(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get the value as a float for a scalar Variable. May also be one-dimensional of length 1.
		/// </summary>
		/// <returns> value as a float </returns>
		public virtual float readScalarFloat()
		{
			NcArray data = _readScalarData();
			return data.getFloat(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get the value as a double for a scalar Variable. May also be one-dimensional of length 1.
		/// </summary>
		/// <returns> value as a double </returns>
		public virtual double readScalarDouble()
		{
			NcArray data = _readScalarData();
			return data.getDouble(NcIndex.scalarIndexImmutable);
		}

		/// <summary>
		/// Get the value as a String for a scalar Variable. May also be one-dimensional of length 1.
		/// May also be one-dimensional of type CHAR, which wil be turned into a scalar String.
		/// </summary>
		/// <returns> value as a String </returns>
		public virtual string readScalarString()
		{
			NcArray data = _readScalarData();
			if (dataType == DataType.STRING)
			{
				return (string)data.getObject(NcIndex.scalarIndexImmutable);
			}
			else if (dataType == DataType.CHAR)
			{
				ArrayChar dataC = (ArrayChar)data;
				return dataC.String;
			}
			else
			{
				throw new ArgumentException("readScalarString not STRING or CHAR " + getFullName());
			}
		}

		///////////////
		// internal reads: all other calls go through these.
		// subclasses must override, so that NetcdfDataset wrapping will work.

		// non-structure-member Variables.

		protected internal virtual NcArray _read()
		{
			// caching overrides the proxyReader
			// check if already cached
			if (cache.data != null)
			{
				if (debugCaching)
				{
					Console.WriteLine("got data from cache " + getFullName());
				}
				return cache.data.copy();
			}

			NcArray data = proxyReader.reallyRead(this, null);

			// optionally cache it
			if (Caching)
			{
				setCachedData(data);
				if (debugCaching)
				{
					Console.WriteLine("cache " + getFullName());
				}
				return cache.data.copy(); // dont let users get their nasty hands on cached data
			}
			else
			{
				return data;
			}
		}

		// section of non-structure-member Variable
		// assume filled, validated Section
		protected internal virtual NcArray _read(Section section)
		{
			// check if its really a full read
			if ((null == section) || section.computeSize() == Size)
			{
				return _read();
			}

			// full read was cached
			if (Caching)
			{
				if (cache.data == null)
				{
					setCachedData(_read()); // read and cache entire array
					if (debugCaching)
					{
						Console.WriteLine("cache " + getFullName());
					}
				}
				if (debugCaching)
				{
					Console.WriteLine("got data from cache " + getFullName());
				}
				return cache.data.sectionNoReduce(section.Ranges).copy(); // subset it, return copy
			}

			return proxyReader.reallyRead(this, section, null);
		}

		protected internal virtual NcArray _readScalarData()
		{
			NcArray scalarData = read();
			scalarData = scalarData.reduce();

			if ((scalarData.Rank == 0) || ((scalarData.Rank == 1) && dataType == DataType.CHAR))
			{
				return scalarData;
			}
			throw new System.NotSupportedException("not a scalar variable =" + this);
		}

		/// <summary>
		/// public by accident, do not call directly.
		/// </summary>
		/// <returns> Array </returns>
		public virtual NcArray reallyRead(Variable client, CancelTask cancelTask)
		{
			if (MemberOfStructure) // LOOK should be UnsupportedOperationException ??
			{
				IList<string> memList = new List<string>();
				memList.Add(this.ShortName);
				Structure s = ParentStructure.select(memList);

				// s.read() may return an ArrayObject with sequence data type when the client is sequence.
				// That's way we need the code to read the data array from it.
				NcArray a = s.read();
				if (a is ArrayStructure)
				{
					ArrayStructure @as = (ArrayStructure)a;
					return @as.extractMemberArray(@as.findMember(this.ShortName));
				}
				else
				{
					IndexIterator iter = a.IndexIterator;
					IList<NcArray> arrays = new List<NcArray>();
					int len = 0;
					DataType dataType1 = DataType.SEQUENCE;
					while (iter.hasNext())
					{
						ArrayStructure @as = (ArrayStructure)iter.ObjectNext;
						if (@as == null)
						{
							arrays.Add(null);
						}
						else
						{
							NcArray array = @as.extractMemberArray(@as.findMember(this.ShortName));
							if (len == 0)
							{
								dataType1 = array.DataType;
							}
							arrays.Add(array);
							len += (int)array.Size;
						}
					}

					if (arrays.Count == 1)
					{
						return arrays[0];
					}
					else
					{
						NcArray r;
						IndexIterator rIter;
						switch (dataType1)
						{
							case DataType.SEQUENCE:
							case DataType.STRUCTURE:
								len = arrays.Count;
								r = NcArray.factory(dataType1, new int[] { len });
								for (int i = 0; i < len; i++)
								{
									if (arrays[i].Size > 0)
									{
										r.setObject(i, arrays[i].getObject(0));
									}
								}
								break;
							default:
								r = NcArray.factory(dataType1, new int[] { len });
								rIter = r.IndexIterator;
								foreach (NcArray array in arrays)
								{
									if (array != null)
									{
										IndexIterator aIter = array.IndexIterator;
										while (aIter.hasNext())
										{
											if (rIter.hasNext())
											{
												rIter.ObjectNext = aIter.ObjectNext;
											}
										}
									}
								}
								break;
						}
						return r;
					}
				}
			}

			try
			{
				return ncfile.readData(this, ShapeAsSection);
			}
			catch (InvalidRangeException e)
			{
				Console.WriteLine(e.ToString());
				throw new IOException(e.Message); // cant happen haha
			}
		}

		/// <summary>
		/// public by accident, do not call directly.
		/// </summary>
		/// <returns> Array </returns>
		public virtual NcArray reallyRead(Variable client, Section section, CancelTask cancelTask)
		{
			if (MemberOfStructure)
			{
				throw new System.NotSupportedException("Cannot directly read section of Member Variable=" + getFullName());
			}
			// read just this section
			return ncfile.readData(this, section);
		}

		/// @deprecated do not use 
		[Obsolete("do not use")]
		public virtual long readToByteChannel(Section section, WritableByteChannel wbc)
		{
			if ((ncfile == null) || hasCachedData())
			{
				return IospHelper.copyToByteChannel(read(section), wbc);
			}
			return ncfile.readToByteChannel(this, section, wbc);
		}

		/// <summary>
		/// Read variable data to a stream. Support for NcStreamWriter. </summary>
		public virtual long readToStream(Section section, Stream @out)
		{
			if ((ncfile == null) || hasCachedData())
			{
				return IospHelper.copyToOutputStream(read(section), @out);
			}
			return ncfile.readToOutputStream(this, section, @out);
		}

		/// <summary>
		/// Get its containing Group.
		/// Not deprecated.
		/// LOOK if you relied on Group being set during construction, use getParentGroupOrRoot().
		/// </summary>
		public virtual Group getParentGroup()
		{
			return this.group;
		}

		/// <summary>
		/// Get the full name of this Variable.
		/// Certain characters are backslash escaped (see NetcdfFiles.getFullName(Variable))
		/// Not deprecated.
		/// </summary>
		/// <returns> full name with backslash escapes </returns>
		public virtual string getFullName()
		{
			return NetcdfFiles.makeFullName(this);
		}

		/// <summary>
		/// Get the display name plus the dimensions, eg 'float name(dim1, dim2)'
		/// </summary>
		/// <returns> display name plus the dimensions </returns>
		public virtual string getNameAndDimensions()
		{
			Formatter buf = new Formatter();
			getNameAndDimensions(buf, true, false);
			return buf.ToString();
		}

		/// <summary>
		/// Get the display name plus the dimensions, eg 'float name(dim1, dim2)'
		/// </summary>
		/// <param name="strict"> strictly comply with ncgen syntax, with name escaping. otherwise, get extra info, no escaping </param>
		/// <returns> display name plus the dimensions </returns>
		public virtual string getNameAndDimensions(bool strict)
		{
			Formatter buf = new Formatter();
			getNameAndDimensions(buf, false, strict);
			return buf.ToString();
		}

		/// <summary>
		/// Get the display name plus the dimensions, eg 'name(dim1, dim2)'
		/// </summary>
		/// <param name="buf"> add info to this StringBuilder </param>
		/// @deprecated use CDLWriter 
		[Obsolete("use CDLWriter")]
		public virtual void getNameAndDimensions(StringBuilder buf)
		{
			getNameAndDimensions(buf, true, false);
		}

		/// <summary>
		/// Add display name plus the dimensions to the StringBuffer
		/// </summary>
		/// <param name="buf"> add info to this </param>
		/// <param name="useFullName"> use full name else short name. strict = true implies short name </param>
		/// <param name="strict"> strictly comply with ncgen syntax, with name escaping. otherwise, get extra info, no escaping </param>
		/// @deprecated use CDLWriter 
		[Obsolete("use CDLWriter")]
		public virtual void getNameAndDimensions(StringBuilder buf, bool useFullName, bool strict)
		{
			Formatter proxy = new Formatter();
			getNameAndDimensions(proxy, useFullName, strict);
			buf.Append(proxy.ToString());
		}

		/// <summary>
		/// Add display name plus the dimensions to the Formatter
		/// </summary>
		/// <param name="buf"> add info to this </param>
		/// <param name="useFullName"> use full name else short name. strict = true implies short name </param>
		/// <param name="strict"> strictly comply with ncgen syntax, with name escaping. otherwise, get extra info, no escaping </param>
		public virtual void getNameAndDimensions(Formatter buf, bool useFullName, bool strict)
		{
			useFullName = useFullName && !strict;
			string name = useFullName ? getFullName() : ShortName;
			if (strict)
			{
				name = NetcdfFile.makeValidCDLName(ShortName);
			}
			buf.format("%s", name);

			if (shape != null)
			{
				if (Rank > 0)
				{
					buf.format("(");
				}
				for (int i = 0; i < dimensions.Count; i++)
				{
					Dimension myd = dimensions[i];
					string dimName = myd.ShortName;
					if ((!string.ReferenceEquals(dimName, null)) && strict)
					{
						dimName = NetcdfFile.makeValidCDLName(dimName);
					}
					if (i != 0)
					{
						buf.format(", ");
					}
					if (myd.VariableLength)
					{
						buf.format("*");
					}
					else if (myd.Shared)
					{
						if (!strict)
						{
							buf.format("%s=%d", dimName, myd.Length);
						}
						else
						{
							buf.format("%s", dimName);
						}
					}
					else
					{
						if (!string.ReferenceEquals(dimName, null))
						{
							buf.format("%s=", dimName);
						}
						buf.format("%d", myd.Length);
					}
				}
				if (Rank > 0)
				{
					buf.format(")");
				}
			}
		}

		public override string ToString()
		{
			return writeCDL(false, false);
		}

		/// <summary>
		/// CDL representation of a Variable.
		/// </summary>
		/// <param name="useFullName"> use full name, else use short name </param>
		/// <param name="strict"> strictly comply with ncgen syntax </param>
		/// <returns> CDL representation of the Variable. </returns>
		/// @deprecated use CDLWriter 
		[Obsolete("use CDLWriter")]
		public virtual string writeCDL(bool useFullName, bool strict)
		{
			Formatter buf = new Formatter();
			writeCDL(buf, new Indent(2), useFullName, strict);
			return buf.ToString();
		}

		/// @deprecated use CDLWriter 
		[Obsolete("use CDLWriter")]
		protected internal virtual void writeCDL(Formatter buf, Indent indent, bool useFullName, bool strict)
		{
			buf.format("%s", indent);
			if (dataType.isEnum())
			{
				if (enumTypedef == null)
				{
					buf.format("enum UNKNOWN");
				}
				else
				{
					buf.format("enum %s", NetcdfFile.makeValidCDLName(enumTypedef.ShortName));
				}
			}
			else
			{
				buf.format("%s", DataTypes.ToString(dataType));
			}

			// if (isVariableLength) buf.append("(*)"); // LOOK
			buf.format(" ");
			getNameAndDimensions(buf, useFullName, strict);
			buf.format(";");
			if (!strict)
			{
				buf.format(extraInfo());
			}
			buf.format("%n");

			indent.incr();
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
			indent.decr();
		}

		/// <summary>
		/// String representation of Variable and its attributes.
		/// </summary>
		public virtual string toStringDebug()
		{
			Formatter f = new Formatter();
			f.format("Variable %s", getFullName());
			if (ncfile != null)
			{
				f.format(" in file %s", getDatasetLocation());
				string extra = ncfile.toStringDebug(this);
				if (!string.ReferenceEquals(extra, null))
				{
					f.format(" %s", extra);
				}
			}
			return f.ToString();
		}

		private static bool showSize = false;

		protected internal virtual string extraInfo()
		{
			return showSize ? " // " + ElementSize + " " + Size : "";
		}

		/// <summary>
		/// The location of the dataset this belongs to. Labeling purposes only. </summary>
		public virtual string getDatasetLocation()
		{
			if (ncfile != null)
			{
				return ncfile.Location;
			}
			return "N/A";
		}

		/// <summary>
		/// Instances which have same content are equal.
		/// </summary>
		public override bool Equals(object oo)
		{
			if (this == oo)
			{
				return true;
			}
			if (!(oo is Variable))
			{
				return false;
			}
			Variable o = (Variable)oo;

			if (!ShortName.Equals(o.ShortName))
			{
				return false;
			}
			if (Scalar != o.Scalar)
			{
				return false;
			}
			if (DataType != o.DataType)
			{
				return false;
			}
			if (!getParentGroup().Equals(o.getParentGroup()))
			{
				return false;
			}
			if ((ParentStructure != null) && !ParentStructure.Equals(o.ParentStructure))
			{
				return false;
			}
			if (VariableLength != o.VariableLength)
			{
				return false;
			}
			if (dimensions.Count != o.dimensions.Count)
			{
				return false;
			}
			for (int i = 0; i < dimensions.Count; i++)
			{
				if (!getDimension(i).Equals(o.getDimension(i)))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Override Object.hashCode() to implement equals.
		/// </summary>
		public override int GetHashCode()
		{
			if (hashCode == 0)
			{
				int result = 17;
				result = 37 * result + ShortName.GetHashCode();
				if (Scalar)
				{
					result++;
				}
				result = 37 * result + DataType.GetHashCode();
				result = 37 * result + getParentGroup().GetHashCode();
				if (ParentStructure != null)
				{
					result = 37 * result + ParentStructure.GetHashCode();
				}
				if (isVariableLength_Conflict)
				{
					result++;
				}
				result = 37 * result + dimensions.GetHashCode();
				hashCode = result;
			}
			return hashCode;
		}

		protected int hashCode;

		/// <summary>
		/// Sort by name
		/// </summary>
		public virtual int CompareTo(VariableSimpleIF o)
		{
			return string.CompareOrdinal(ShortName, o.ShortName);
		}

		/////////////////////////////////////////////////////////////////////////////

		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		protected internal Variable()
		{
		}

		/// <summary>
		/// Create a Variable. Also must call setDataType() and setDimensions()
		/// </summary>
		/// <param name="ncfile"> the containing NetcdfFile. </param>
		/// <param name="group"> the containing group; if null, use rootGroup </param>
		/// <param name="parent"> parent Structure, may be null </param>
		/// <param name="shortName"> variable shortName, must be unique within the Group </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public Variable(NetcdfFile ncfile, Group group, Structure parent, string shortName) : base(shortName)
		{
			this.ncfile = ncfile;
			if (parent == null)
			{
				setParentGroup((group == null) ? ncfile.RootGroup : group);
			}
			else
			{
				ParentStructure = parent;
			}
			attributesField = new AttributeContainerMutable(shortName);
		}

		/// <summary>
		/// Create a Variable. Also must call setDataType() and setDimensions()
		/// </summary>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public Variable(NetcdfFile ncfile, Group group, Structure parent, string shortName, DataType dtype, string dims) : this(ncfile, group, parent, shortName, dtype, (IList<Dimension>)null)
		{
			if (group == null)
			{
				group = ncfile.RootGroup;
			}
			setDimensions(NetcdfSharp.ucar.nc2.Dimensions.makeDimensionsList(group.findDimension, dims));
		}

		/// <summary>
		/// Create a Variable. Also must call setDataType() and setDimensions()
		/// </summary>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public Variable(NetcdfFile ncfile, Group group, Structure parent, string shortName, DataType dtype, IList<Dimension> dims) : this(ncfile, group, parent, shortName)
		{
			DataType = dtype;
			setDimensions(dims);
		}

		/// <summary>
		/// Copy constructor.
		/// The returned Variable is mutable.
		/// It shares the cache object and the iosp Object, attributes and dimensions with the original.
		/// Does not share the proxyReader.
		/// Use for section, slice, "logical views" of original variable.
		/// </summary>
		/// <param name="from"> copy from this Variable. </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public Variable(Variable from) : base(from.ShortName)
		{
			this.attributesField = new AttributeContainerMutable(from.ShortName, from.attributes());
			this.cache = from.cache; // caller should do createNewCache() if dont want to share
			DataType = from.DataType;
			this.dimensions = new List<Dimension>(from.dimensions); // dimensions are shared
			this.elementSize = from.ElementSize;
			this.enumTypedef = from.enumTypedef;
			setParentGroup(from.group);
			ParentStructure = from.ParentStructure;
			this.isVariableLength_Conflict = from.isVariableLength_Conflict;
			this.ncfile = from.ncfile;
			this.shape = from.Shape;
			this.sizeToCache = from.sizeToCache;
			this.spiObject = from.spiObject;
		}

		/// <summary>
		/// Set the short name, converting to valid CDM object name if needed.
		/// </summary>
		/// <param name="shortName"> set to this value </param>
		/// <returns> valid CDM object name </returns>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual string setName(string shortName)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			ShortName = shortName;
			return ShortName;
		}

		/// <summary>
		/// Set the parent group.
		/// </summary>
		/// <param name="group"> set to this value </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setParentGroup(Group group)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			base.ParentGroup = group;
		}

		/// <summary>
		/// Set the element size. Usually elementSize is determined by the dataType,
		/// use this only for exceptional cases.
		/// </summary>
		/// <param name="elementSize"> set to this value </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setElementSize(int elementSize)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			this.elementSize = elementSize;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////
		// Attributes

		/// <summary>
		/// The attributes contained by this Variable. </summary>
		public virtual AttributeContainer attributes()
		{
			return attributesField;
		}

		public virtual IEnumerator<NcAttribute> GetEnumerator()
		{
			return attributesField.GetEnumerator();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return attributesField.GetEnumerator();
		}

		/// <summary>
		/// Find the attribute by name, return null if not exist </summary>
		public virtual NcAttribute? findAttribute(string name)
		{
			return attributesField.findAttribute(name);
		}

		/// <summary>
		/// Find a String-valued Attribute by name (ignore case), return the String value of the Attribute.
		/// </summary>
		/// <returns> the attribute value, or defaultValue if not found </returns>
		public virtual string findAttributeString(string attName, string defaultValue)
		{
			return attributesField.findAttributeString(attName, defaultValue);
		}

		/// @deprecated Use attributes() 
		public virtual bool Empty
		{
			get
			{
				return attributesField.Empty;
			}
		}

		/// @deprecated Use attributes() 
		[Obsolete("Use attributes()")]
		public virtual bool isEmpty()
		{
			return attributesField.Empty;
		}

		/// @deprecated Use attributes() 
		[Obsolete("Use attributes()")]
		public virtual IList<NcAttribute> Attributes
		{
			get
			{
				return attributesField.Attributes;
			}
		}

		/// @deprecated Use attributes() 
		[Obsolete("Use attributes()")]
		public virtual NcAttribute? findAttributeIgnoreCase(string name)
		{
			return attributesField.findAttributeIgnoreCase(name);
		}

		/// @deprecated Use attributes() 
		[Obsolete("Use attributes()")]
		public virtual double findAttributeDouble(string attName, double defaultValue)
		{
			return attributesField.findAttributeDouble(attName, defaultValue);
		}

		/// @deprecated Use attributes() 
		[Obsolete("Use attributes()")]
		public virtual int findAttributeInteger(string attName, int defaultValue)
		{
			return attributesField.findAttributeInteger(attName, defaultValue);
		}

		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual NcAttribute addAttribute(NcAttribute att)
		{
			return attributesField.addAttribute(att);
		}

		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void addAll(IEnumerable<NcAttribute> atts)
		{
			attributesField.addAll(atts);
		}

		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual bool remove(NcAttribute a)
		{
			return attributesField.remove(a);
		}

		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual bool removeAttribute(string attName)
		{
			return attributesField.removeAttribute(attName);
		}

		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual bool removeAttributeIgnoreCase(string attName)
		{
			return attributesField.removeAttributeIgnoreCase(attName);
		}

		////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Set the shape with a list of Dimensions. The Dimensions may be shared or not.
		/// Dimensions are in order, slowest varying first. Send a null for a scalar.
		/// </summary>
		/// <param name="dims"> list of type NetcdfSharp.ucar.nc2.Dimension </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setDimensions(IList<Dimension> dims)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			this.dimensions = (dims == null) ? new List<Dimension>() : new List<Dimension>(dims);
			resetShape();
		}

		/// <summary>
		/// Use when dimensions have changed, to recalculate the shape.
		/// </summary>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void resetShape()
		{
			// if (immutable) throw new IllegalStateException("Cant modify"); LOOK allow this for unlimited dimension updating
			this.shape = new int[dimensions.Count];
			for (int i = 0; i < dimensions.Count; i++)
			{
				Dimension dim = dimensions[i];
				shape[i] = dim.Length;
				if (dim.VariableLength)
				{
					isVariableLength_Conflict = true;
				}
			}
			this.shapeAsSection = null; // recalc next time its asked for
		}

		/// <summary>
		/// Set the dimensions using the dimensions names. The dimension is searched for recursively in the parent groups.
		/// </summary>
		/// <param name="dimString"> : whitespace separated list of dimension names, or '*' for Dimension.UNKNOWN, or number for anon
		///        dimension. null or empty String is a scalar. </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setDimensions(string dimString)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			try
			{
				setDimensions(NetcdfSharp.ucar.nc2.Dimensions.makeDimensionsList(ParentGroupOrRoot.findDimension, dimString));
				resetShape();
			}
			catch (InvalidOperationException e)
			{
				throw new ArgumentException("Variable " + getFullName() + " setDimensions = '" + dimString + "' FAILED: " + e.Message + " file = " + getDatasetLocation());
			}
		}

		/// <summary>
		/// Set the dimensions using all anonymous (unshared) dimensions
		/// </summary>
		/// <param name="shape"> defines the dimension lengths. must be > 0, or -1 for VLEN </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setDimensionsAnonymous(int[] shape)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			this.dimensions = new List<Dimension>();
			for (int i = 0; i < shape.Length; i++)
			{
				if ((shape[i] < 1) && (shape[i] != -1))
				{
					throw new InvalidRangeException("shape[" + i + "]=" + shape[i] + " must be > 0");
				}
				Dimension anon;
				if (shape[i] == -1)
				{
					anon = Dimension.VLEN;
					isVariableLength_Conflict = true;
				}
				else
				{
					anon = new Dimension(null, shape[i], false, false, false);
				}
				dimensions.Add(anon);
			}
			resetShape();
		}

		/// <summary>
		/// Set this Variable to be a scalar
		/// </summary>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setIsScalar()
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			this.dimensions = new List<Dimension>();
			resetShape();
		}

		/// <summary>
		/// Replace a dimension with an equivalent one.
		/// </summary>
		/// <param name="idx"> index into dimension array </param>
		/// <param name="dim"> to set </param>
		/// <summary>
		/// Make this immutable.
		/// </summary>
		/// <returns> this </returns>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public override Variable setImmutable()
		{
			base.setImmutable();
			dimensions = dimensions.AsReadOnly();
			attributesField.setImmutable();
			return this;
		}

		/// <summary>
		/// Get immutable service provider opaque object. </summary>
		public virtual object getSPobject()
		{
			return spiObject;
		}

		/// @deprecated Do not use. 
		[Obsolete("Do not use.")]
		public virtual void setSPobject(object spiObject)
		{
			this.spiObject = spiObject;
		}

		/// <summary>
		/// Immutable service provider opaque object (property form for translated call sites). </summary>
		public virtual object SPobject
		{
			get => spiObject;
			set => spiObject = value;
		}

		////////////////////////////////////////////////////////////////////////////////////
		// caching

		/// <summary>
		/// If total data size is less than SizeToCache in bytes, then cache.
		/// </summary>
		/// <returns> size at which caching happens </returns>
		public virtual int getSizeToCache()
		{
			if (sizeToCache >= 0)
			{
				return sizeToCache; // it was set
			}
			return CoordinateVariable ? defaultCoordsSizeToCache : defaultSizeToCache;
		}

		/// <summary>
		/// Set whether to cache or not. Implies that the entire array will be stored, once read.
		/// Normally this is set automatically based on size of data.
		/// </summary>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual bool Caching
		{
			get
			{
				if (!permitCaching)
				{
					return false;
				}
				if (!this.cache.cachingSet)
				{
					cache.isCaching = !isVariableLength_Conflict && (Size * ElementSize < getSizeToCache());
					if (debugCaching)
					{
						Console.WriteLine("  cache " + getFullName() + " " + cache.isCaching + " " + Size * ElementSize + " < " + getSizeToCache());
					}
					this.cache.cachingSet = true;
				}
				return cache.isCaching;
			}
			set
			{
				this.cache.isCaching = value;
				this.cache.cachingSet = true;
			}
		}

		/// <summary>
		/// Note that standalone Ncml caches data values set in the Ncml.
		/// So one cannot invalidate those caches.
		/// </summary>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void invalidateCache()
		{
			cache.data = null;
		}

		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setCachedData(NcArray cacheData)
		{
			setCachedData(cacheData, false);
		}

		/// <summary>
		/// Set the data cache
		/// </summary>
		/// <param name="cacheData"> cache this Array </param>
		/// <param name="isMetadata"> : synthesized data, set true if must be saved in NcML output (ie data not actually in the file). </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setCachedData(NcArray cacheData, bool isMetadata)
		{
			if ((cacheData != null) && (cacheData.ElementType != DataType.getPrimitiveClassType()))
			{
				throw new ArgumentException("setCachedData type=" + cacheData.ElementType + " incompatible with variable type=" + DataType);
			}
			this.cache.data = cacheData;
			this.cache.isMetadata = isMetadata;
			this.cache.cachingSet = true;
			this.cache.isCaching = true;
		}

		/// <summary>
		/// Create a new data cache, use this when you dont want to share the cache.
		/// </summary>
		public virtual void createNewCache()
		{
			this.cache = new Cache();
		}

		/// <summary>
		/// Gets or sets the cached data for this variable.
		/// </summary>
		public virtual NcArray CachedData
		{
			get
			{
				return cache.data;
			}
			set
			{
				setCachedData(value);
			}
		}

		/// <summary>
		/// Has data been read and cached.
		/// Use only on a Variable, not a subclass.
		/// </summary>
		/// <returns> true if data is read and cached </returns>
		public virtual bool hasCachedData()
		{
			return (null != cache.data);
		}

		// this indirection allows us to share the cache among the variable's sections and copies
		protected internal class Cache
		{
			internal NcArray data;
			internal bool isCaching;
			internal bool cachingSet;
			internal bool isMetadata;

			internal Cache()
			{
			}

			public virtual void reset()
			{
				this.data = null;
				this.isCaching = false;
				this.cachingSet = false;
				this.isMetadata = false;
			}
		}

		///////////////////////////////////////////////////////////////////////
		// setting variable data values

		/// <summary>
		/// Generate the list of values from a starting value and an increment.
		/// Will reshape to variable if needed.
		/// </summary>
		/// <param name="npts"> number of values, must = v.getSize() </param>
		/// <param name="start"> starting value </param>
		/// <param name="incr"> increment </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setValues(int npts, double start, double incr)
		{
			if (npts != Size)
			{
				throw new ArgumentException("bad npts = " + npts + " should be " + Size);
			}
			NcArray data = NcArray.makeArray(DataType, npts, start, incr);
			if (Rank != 1)
			{
				data = data.reshape(Shape);
			}
			setCachedData(data, true);
		}

		/// <summary>
		/// Set the data values from a list of Strings.
		/// </summary>
		/// <param name="values"> list of Strings </param>
		/// @deprecated Use Variable.builder() 
		[Obsolete("Use Variable.builder()")]
		public virtual void setValues(IList<string> values)
		{
			NcArray data = NcArray.makeArray(DataType, values);

			if (data.Size != Size)
			{
				throw new ArgumentException("Incorrect number of values specified for the Variable " + getFullName() + " needed= " + Size + " given=" + data.Size);
			}

			if (Rank != 1) // dont have to reshape for rank 1
			{
				data = data.reshape(Shape);
			}

			setCachedData(data, true);
		}

		////////////////////////////////////////////////////////////////////////
		// StructureMember - could be a subclass, but that has problems

		/// <summary>
		/// Get list of Dimensions, including parents if any.
		/// </summary>
		/// <returns> array of Dimension, rank of v plus all parents. </returns>
		/// @deprecated use Dimensions.makeDimensionsAll(Variable); 
		[Obsolete("use Dimensions.makeDimensionsAll(Variable);")]
		public virtual IList<Dimension> DimensionsAll
		{
			get
			{
				IList<Dimension> dimsAll = new List<Dimension>();
				addDimensionsAll(dimsAll, this);
				return dimsAll;
			}
		}

		private void addDimensionsAll(IList<Dimension> result, Variable v)
		{
			if (v.MemberOfStructure)
			{
				addDimensionsAll(result, v.ParentStructure);
			}
			for (int i = 0; i < v.Rank; i++)
			{
				result.Add(v.getDimension(i));
			}
		}

		/// @deprecated use Dimensions.makeDimensionsAll(Variable); 
		[Obsolete("use Dimensions.makeDimensionsAll(Variable);")]
		public virtual int[] getShapeAll()
		{
			if (ParentStructure == null)
			{
				return Shape;
			}
			IList<Dimension> dimAll = DimensionsAll;
			int[] shapeAll = new int[dimAll.Count];
			for (int i = 0; i < dimAll.Count; i++)
			{
				shapeAll[i] = dimAll[i].Length;
			}
			return shapeAll;
		}

		/// <summary>
		/// Calculate if this is a classic coordinate variable: has same name as its first dimension.
		/// If type char, must be 2D, else must be 1D.
		/// </summary>
		/// <returns> true if a coordinate variable. </returns>
		public virtual bool CoordinateVariable
		{
			get
			{
				if ((dataType == DataType.STRUCTURE) || MemberOfStructure) // Structures and StructureMembers cant be coordinate variables
				{
					return false;
				}

				int n = Rank;
				if ((n == 1) && (dimensions.Count == 1))
				{
					Dimension firstd = dimensions[0];
					if (ShortName.Equals(firstd.ShortName)) // : short names match
					{
						return true;
					}
				}
				if ((n == 2) && (dimensions.Count == 2)) // two dimensional
				{
					Dimension firstd = dimensions[0];
					// must be char valued (really a String)
					return shortName.Equals(firstd.ShortName) && // short names match
						(dataType == DataType.CHAR);
				}

				return false;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////
		// TODO make private final and Immutable in release 6.
		// Physical container for this Variable where the I/O happens. may be null if Variable is self contained.
		protected internal NetcdfFile ncfile;
		protected internal DataType dataType;
		private EnumTypedef enumTypedef;
		protected internal IList<Dimension> dimensions = new List<Dimension>(5); // TODO immutable in ver 6
		protected internal AttributeContainerMutable attributesField; // TODO immutable in ver 6
		private ProxyReader _proxyReader;
		protected internal ProxyReader proxyReader
		{
			get
			{
				return _proxyReader ?? this;
			}
			set
			{
				_proxyReader = value;
			}
		}
		protected internal object spiObject;

		// computed
		private Section shapeAsSection; // derived from the shape, immutable; used for every read, deferred creation
		protected internal int[] shape = new int[0];
		protected internal bool isVariableLength_Conflict;
		protected internal int elementSize;

		// TODO do we need these? breaks immutability
		// TODO maybe caching read data should be separate from "this is the source of the data".
		protected internal Cache cache = new Cache(); // cache cannot be null
		protected internal int sizeToCache = -1; // bytes

		protected internal Variable(Builder builder, Group parentGroup) : base(builder.shortName)
		{
			if (parentGroup == null)
			{
				throw new System.InvalidOperationException(string.Format("Parent Group must be set for Variable {0}", builder.shortName));
			}

			if (string.ReferenceEquals(builder.shortName, null) || builder.shortName.Length == 0)
			{
				throw new System.InvalidOperationException("Name must be set for Variable");
			}

			this.group = parentGroup;
			this.ncfile = builder.ncfile;
			this.parentstruct = builder.parentStruct;
			this.dataType = builder.dataType;
			this.attributesField = builder.attributes;
			this.proxyReader = builder.proxyReader == null ? this : builder.proxyReader;
			this.spiObject = builder.spiObject;
			this.cache = builder.cache;

			if (this.dataType.isEnum())
			{
				this.enumTypedef = this.group.findEnumeration(builder.enumTypeName, true);
				if (this.enumTypedef == null)
				{
					throw new System.InvalidOperationException(string.Format("EnumTypedef '{0}' does not exist in a parent Group", builder.enumTypeName));
				}
			}

			// Convert dimension to shared dimensions that live in a parent group.
			IList<Dimension> dims = new List<Dimension>();
			foreach (Dimension dim in builder.dimensions)
			{
				if (dim.Shared)
				{
					Dimension sharedDim = this.group.findDimension(dim.ShortName);
					if (sharedDim == null)
					{
						throw new System.InvalidOperationException(string.Format("Shared Dimension {0} does not exist in a parent group", dim));
					}
					else
					{
						dims.Add(sharedDim);
					}
				}
				else
				{
					dims.Add(dim);
				}
			}

			// possible slice of another variable
			if (builder.slicer != null)
			{
				int dim = builder.slicer.dim;
				int index = builder.slicer.index;
				Section slice = NetcdfSharp.ucar.nc2.Dimensions.makeSectionFromDimensions(dims).replaceRange(dim, NcRange.make(index, index)).build();
				Variable orgClient = parentGroup.findVariableLocal(builder.slicer.orgName);
				ProxyReader = new SliceReader(orgClient, dim, slice);
				Caching = false; // dont cache
								 // remove that dimension - reduce rank
				dims.RemoveAt(dim);
			}

			this.dimensions = dims;
			if (builder.autoGen != null)
			{
				this.cache.data = builder.autoGen.makeDataArray(this.dataType, this.dimensions);
				this.cache.isMetadata = true; // So it gets copied
			}

			// calculated fields
			this.elementSize = builder.elementSize > 0 ? builder.elementSize : this.dataType.getSize();
			this.isVariableLength_Conflict = false;
			foreach (Dimension d in dimensions)
			{
				if (d.VariableLength)
				{
					this.isVariableLength_Conflict = true;
					break;
				}
			}
			try
			{
				IList<NcRange> list = new List<NcRange>();
				foreach (Dimension d in dimensions)
				{
					int len = d.Length;
					if (len > 0)
					{
						list.Add(new NcRange(d.ShortName, 0, len - 1));
					}
					else if (len == 0)
					{
						list.Add(NcRange.EMPTY); // LOOK empty not named
					}
					else
					{
						Debug.Assert(d.VariableLength);
						list.Add(NcRange.VLEN); // LOOK vlen not named
					}
				}
				this.shapeAsSection = new Section(list).makeImmutable();
				this.shape = shapeAsSection.Shape;
			}
			catch (InvalidRangeException e)
			{
				log.error("Bad shape in variable " + getFullName(), e);
				throw new System.InvalidOperationException(e.Message);
			}
		}

		/// <summary>
		/// Turn into a mutable Builder. Can use toBuilder().build() to copy. </summary>
		public virtual Builder toBuilder()
		{
			return addLocalFieldsToBuilder(builder());
		}

		// Add local fields to the passed - in builder.
		// This makes an exact copy, including ncfile and parent and proxyReader.
		// build() replaces parent but respects ncfile and proxyReader.
		// Normally on a copy you want to set proxyReader to null;
		protected internal virtual Builder addLocalFieldsToBuilder(Builder builder)
		{
			builder.setName(this.shortName).setNcfile(this.ncfile).setParentStructure(ParentStructure)
				.setDataType(this.dataType).setEnumTypeName(enumTypedef != null ? enumTypedef.ShortName : null)
				.addDimensions(this.dimensions).addAttributes(this.attributesField).setProxyReader(this.proxyReader)
				.setSPobject(this.spiObject);

			if (this.cache.isMetadata)
			{
				builder.setCachedData(this.cache.data, true);
			}
			return builder;
		}

		/// <summary>
		/// Get Builder for this class that allows subclassing.
		/// </summary>
		public static Builder builder()
		{
			return new Builder();
		}

		/// <summary>
		/// A builder for Variables. </summary>
		public class Builder
		{
			public string shortName;
			public DataType dataType;
			internal int elementSize;

			public NetcdfFile ncfile; // set in Group build() if null
			internal Structure parentStruct; // set in Structure.build(), no not use otherwise

			protected internal Group.Builder parentBuilder;
			protected internal Structure.BuilderStruct parentStructureBuilder;
			internal IList<Dimension> dimensions = new List<Dimension>(); // The dimension's group is ignored; replaced when build()
			public object spiObject;
			public ProxyReader proxyReader;
			internal Cache cache = new Cache(); // cache cannot be null

			internal string enumTypeName;
			internal AutoGen autoGen;
			internal Slicer slicer;
			internal AttributeContainerMutable attributes = new AttributeContainerMutable("");
			private bool built;

			public Builder()
			{
			}

			// add / replace previous
			public virtual Builder addAttribute(NcAttribute att)
			{
				attributes.addAttribute(att);
				return this;
			}

			public virtual Builder addAttributes(IEnumerable<NcAttribute> atts)
			{
				attributes.addAll(atts);
				return this;
			}

			public virtual Builder setDesc(string desc)
			{
				attributes.addAttribute(new NcAttribute("description", desc));
				return this;
			}

			public virtual AttributeContainerMutable getAttributeContainer()
			{
				return attributes;
			}

			public virtual Builder addDimension(Dimension dim)
			{
				dimensions.Add(dim);
				return this;
			}

			public virtual Builder addDimensions(IEnumerable<Dimension> dims)
			{
				foreach (Dimension d in dims)
				{
					dimensions.Add(d);
				}
				return this;
			}

			public virtual Builder setDimensions(IList<Dimension> dims)
			{
				this.dimensions = new List<Dimension>(dims);
				return this;
			}

			/// <summary>
			/// Find the dimension with the same name as dim, and replace it with dim </summary>
			public virtual bool replaceDimensionByName(Dimension dim)
			{
				int idx = -1;
				for (int i = 0; i < dimensions.Count; i++)
				{
					if (dimensions[i].ShortName.Equals(dim.ShortName))
					{
						idx = i;
					}
				}
				if (idx >= 0)
				{
					dimensions[idx] = dim;
				}
				return (idx >= 0);
			}

			/// <summary>
			/// Set dimensions by name. The parent group builder must be set. </summary>
			public virtual Builder setDimensionsByName(string dimString)
			{
				Preconditions.checkNotNull(this.parentBuilder);
				this.dimensions = new List<Dimension>(this.parentBuilder.makeDimensionsList(dimString));
				return this;
			}

			public virtual string getFirstDimensionName()
			{
				return getDimensionName(0);
			}

			public virtual string getDimensionName(int index)
			{
				if (dimensions.Count > index)
				{
					return dimensions[index].ShortName;
				}
				return null;
			}

			public virtual IEnumerable<string> getDimensionNames()
			{
				IList<string> result = new List<string>();
				foreach (Dimension d in dimensions)
				{
					if (!string.ReferenceEquals(d.ShortName, null))
					{
						result.Add(d.ShortName);
					}
				}
				return result;
			}

			public virtual string makeDimensionsString()
			{
				return NetcdfSharp.ucar.nc2.Dimensions.makeDimensionsString(this.dimensions);
			}

			/// <summary>
			/// Set the dimensions using all anonymous (unshared) dimensions
			/// </summary>
			/// <param name="shape"> defines the dimension lengths. must be > 0, or -1 for VLEN </param>
			public virtual Builder setDimensionsAnonymous(int[] shape)
			{
				this.dimensions = new List<Dimension>(NetcdfSharp.ucar.nc2.Dimensions.makeDimensionsAnon(shape));
				return this;
			}

			public virtual ImmutableList<Dimension> getDimensions()
			{
				return ImmutableList<Dimension>.copyOf(this.dimensions);
			}

			// Get all dimension names, including parent structure
			public virtual ImmutableSet<string> getDimensionsAll()
			{
				ImmutableSet<string>.Builder dimsAll = ImmutableSet<string>.builder();
				addDimensionsAll(dimsAll, this);
				return dimsAll.build();
			}

			private void addDimensionsAll(ImmutableSet<string>.Builder result, Builder v)
			{
				if (v.parentStructureBuilder != null)
				{
					foreach (string s in v.parentStructureBuilder.getDimensionsAll())
					{
						result.add(s);
					}
				}
				foreach (string s in getDimensionNames())
				{
					result.add(s);
				}
			}

			public virtual Builder setIsScalar()
			{
				this.dimensions = new List<Dimension>();
				return this;
			}

			public virtual int getRank()
			{
				return dimensions.Count;
			}

			public virtual Builder setDataType(DataType dataType)
			{
				this.dataType = dataType;
				return this;
			}

			/// <summary>
			/// DataType (property form for translated call sites). </summary>
			public virtual DataType DataType
			{
				get => dataType;
				set => dataType = value;
			}

			/// <summary>
			/// Set the dimensions using all anonymous (unshared) dimensions (property form for translated call sites). </summary>
			public virtual int[] DimensionsAnonymous
			{
				set => setDimensionsAnonymous(value);
			}

			/// <summary>
			/// EnumTypedef type name (property form for translated call sites). </summary>
			public virtual string EnumTypeName
			{
				get => enumTypeName;
				set => enumTypeName = value;
			}

			public virtual string getEnumTypeName()
			{
				return this.enumTypeName;
			}

			public virtual int getElementSize()
			{
				return elementSize > 0 ? elementSize : dataType.getSize();
			}

			// In some case we need to override standard element size.
			public virtual Builder setElementSize(int elementSize)
			{
				this.elementSize = elementSize;
				return this;
			}

			public virtual Builder setEnumTypeName(string enumTypeName)
			{
				this.enumTypeName = enumTypeName;
				return this;
			}

			public virtual Builder setNcfile(NetcdfFile ncfile)
			{
				this.ncfile = ncfile;
				return this;
			}

			public virtual Builder setSPobject(object spiObject)
			{
				this.spiObject = spiObject;
				return this;
			}

			/// <summary>
			/// Service provider opaque object (property form for translated call sites). </summary>
			public virtual object SPobject
			{
				get => spiObject;
				set => spiObject = value;
			}

			public virtual Builder setName(string shortName)
			{
				this.shortName = StringUtil2.makeValidCdmObjectName(shortName);
				this.attributes.Name = shortName;
				return this;
			}

			public virtual string getFullName()
			{
				string full = "";
				Group.Builder group = parentStructureBuilder != null ? parentStructureBuilder.parentBuilder : parentBuilder;
				if (group != null)
				{
					full = group.makeFullName();
				}
				if (parentStructureBuilder != null)
				{
					full += parentStructureBuilder.shortName + ".";
				}
				return full + this.shortName;
			}

			public virtual Builder setParentGroupBuilder(Group.Builder parent)
			{
				this.parentBuilder = parent;
				return this;
			}

			public virtual Group.Builder getParentGroupBuilder()
			{
				return parentBuilder;
			}

			public virtual Builder setParentStructureBuilder(Structure.BuilderStruct structureBuilder)
			{
				this.parentStructureBuilder = structureBuilder;
				return this;
			}

			public virtual Structure.BuilderStruct getParentStructureBuilder()
			{
				return parentStructureBuilder;
			}

			// Only the parent Structure should call this.
			public virtual Builder setParentStructure(Structure parent)
			{
				this.parentStruct = parent;
				return this;
			}

			public virtual Builder setProxyReader(ProxyReader proxy)
			{
				this.proxyReader = proxy;
				return this;
			}

			public virtual Builder setCachedData(NcArray cacheData, bool isMetadata)
			{
				this.cache.data = cacheData;
				this.cache.isMetadata = isMetadata;
				this.cache.cachingSet = true;
				this.cache.isCaching = true;
				return this;
			}

			public virtual Builder setAutoGen(double start, double incr)
			{
				this.autoGen = new AutoGen(start, incr);
				return this;
			}

			public virtual Builder resetCache()
			{
				this.cache.data = null;
				return this;
			}

			public virtual Builder setCaching(bool caching)
			{
				this.cache.isCaching = caching;
				this.cache.cachingSet = true;
				return this;
			}

			/// <summary>
			/// Create a new Variable.Builder that is a logical slice of this one, by
			/// fixing the specified dimension at the specified index value.
			/// </summary>
			/// <param name="dim"> which dimension to fix </param>
			/// <param name="index"> at what index value </param>
			public virtual Builder makeSliceBuilder(int dim, int index)
			{
				Console.WriteLine(" slice of " + this.shortName);
				// create a copy of this builder with a slicer
				Builder sliced = this.copy();
				sliced.slicer = new Slicer(dim, index, this.shortName);
				return sliced;
			}

			public virtual Builder copy()
			{
				Builder result = new Builder();
				return result.copyFrom(this);
			}

			/// <summary>
			/// TODO Copy metadata from orgVar. </summary>
			public virtual Builder copyFrom(Variable orgVar)
			{
				setName(orgVar.ShortName);
				setDataType(orgVar.DataType);
				if (orgVar.EnumTypedef != null)
				{
					setEnumTypeName(orgVar.EnumTypedef.ShortName);
				}
				setSPobject(orgVar.getSPobject());
				addDimensions(orgVar.dimensions);
				addAttributes(orgVar.attributes()); // copy
				return this;
			}

			public virtual Builder copyFrom(Builder builder)
			{
				addAttributes(builder.attributes); // copy
				this.autoGen = builder.autoGen;
				this.cache = builder.cache;
				setDataType(builder.dataType);
				addDimensions(builder.dimensions);
				this.elementSize = builder.elementSize;
				setEnumTypeName(builder.getEnumTypeName());
				setNcfile(builder.ncfile);
				this.parentBuilder = builder.parentBuilder;
				setParentStructure(builder.parentStruct);
				setParentStructureBuilder(builder.parentStructureBuilder);
				setProxyReader(builder.proxyReader);
				setName(builder.shortName);
				setSPobject(builder.spiObject);
				return this;
			}

			public override string ToString()
			{
				return dataType + " " + shortName;
			}

			/// <summary>
			/// Normally this is called by Group.build() </summary>
			public virtual Variable build(Group parentGroup)
			{
				if (built)
				{
					throw new System.InvalidOperationException("already built");
				}
				built = true;
				return new Variable(this, parentGroup);
			}
		}

		internal class AutoGen
		{
			internal readonly double start;
			internal readonly double incr;

			internal AutoGen(double start, double incr)
			{
				this.start = start;
				this.incr = incr;
			}

			internal virtual NcArray makeDataArray(DataType dtype, IList<Dimension> dimensions)
			{
				Section section = NetcdfSharp.ucar.nc2.Dimensions.makeSectionFromDimensions(dimensions).build();
				return NcArray.makeArray(dtype, (int)section.Size, start, incr).reshape(section.Shape);
			}
		}

		internal class Slicer
		{
			internal readonly int dim;
			internal readonly int index;
			internal readonly string orgName;

			internal Slicer(int dim, int index, string orgName)
			{
				this.dim = dim;
				this.index = index;
				this.orgName = orgName;
			}
		}

		///////////////////////////////////////////////////////////////////////
		// deprecated

		/// <summary>
		/// @return isVariableLength()
		/// @deprecated use isVariableLength()
		/// </summary>
		public virtual bool isUnknownLength()
		{
			return isVariableLength_Conflict;
		}
	}

}
