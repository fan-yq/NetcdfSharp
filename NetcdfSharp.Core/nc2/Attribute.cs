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

namespace NetcdfSharp.ucar.nc2
{
	using ArrayChar = NetcdfSharp.ucar.ma2.ArrayChar;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using ForbiddenConversionException = NetcdfSharp.ucar.ma2.ForbiddenConversionException;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcIndex = NetcdfSharp.ucar.ma2.NcIndex;
	using Parameter = NetcdfSharp.ucar.unidata.util.Parameter;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// An Attribute is a name and a value, used for associating arbitrary metadata with another object.
	/// The value can be a one dimensional array of Strings or numeric values.
	/// <p/>
	/// TODO Attributes will be immutable in 6.
	/// TODO Attribute will not extend CDMNode in 6.
	/// TODO Attribute will not know who it belongs to in 6 (Group or Variable).
	/// TODO Attribute.getFullName() will not exist in 6.
	/// 
	/// @author caron
	/// </summary>
	public class NcAttribute : CDMNode
	{
		/// @deprecated move to jni.Nc4Iosp 
		[Obsolete("move to jni.Nc4Iosp")]
		private const string SPECIALPREFIX = "_";

		/// @deprecated move to jni.Nc4Iosp 
		[Obsolete("move to jni.Nc4Iosp")]
		public static readonly string[] SPECIALS = new string[] { CDM.NCPROPERTIES, CDM.ISNETCDF4, CDM.SUPERBLOCKVERSION, CDM.DAP4_LITTLE_ENDIAN, CDM.EDU_UCAR_PREFIX };

		/// <summary>
		/// Turn a list into a map
		/// </summary>
		/// <param name="atts"> list of attributes </param>
		/// <returns> map of attributes by name </returns>
		/// @deprecated do not use 
		[Obsolete("do not use")]
		public static IDictionary<string, NcAttribute> makeMap(IList<NcAttribute> atts)
		{
			int size = (atts == null) ? 1 : atts.Count;
			IDictionary<string, NcAttribute> result = new Dictionary<string, NcAttribute>(size);
			if (atts == null)
			{
				return result;
			}
			foreach (NcAttribute att in atts)
			{
				result[att.ShortName] = att;
			}
			return result;
		}

		/// @deprecated move to jni.Nc4Iosp 
		[Obsolete("move to jni.Nc4Iosp")]
		public static bool isspecial(NcAttribute a)
		{
			string nm = a.ShortName;
			if (nm.StartsWith(SPECIALPREFIX, StringComparison.Ordinal))
			{
				/* Check for selected special attributes */
				foreach (string s in SPECIALS)
				{
					if (nm.StartsWith(s, StringComparison.Ordinal))
					{
						return true; // is special
					}
				}
			}
			return false; // is not special
		}

		///////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Get the Attribute name.
		/// Not deprecated in version 5 for Attribute.
		/// </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @SuppressWarnings("deprecated") public String getName()
		public override string Name
		{
			get
			{
				return shortName;
			}
		}

		public void setShortName(string name)
		{
			lock (this)
			{
				if (immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}
				ShortName = name;
			}

		}

		/// <summary>
		/// Get the data type of the Attribute value. </summary>
		public virtual DataType DataType
		{
			get
			{
				return dataType;
			}
			set
			{
				this.dataType = value;
			}
		}


		/// <summary>
		/// Get the EnumTypedef of the Attribute value, if DataType is an ENUM. </summary>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public EnumTypedef getEnumType()
		public virtual EnumTypedef EnumType
		{
			get
			{
				return this.enumtype;
			}
			set
			{
				this.enumtype = value;
			}
		}


		/// <summary>
		/// True if value is an array (getLength() > 1)
		/// </summary>
		/// <returns> if its an array. </returns>
		public virtual bool Array
		{
			get
			{
				return (Length > 1);
			}
		}

		/// <summary>
		/// Get the length of the array of values
		/// </summary>
		/// <returns> number of elements in the array. </returns>
		public virtual int Length
		{
			get
			{
				return nelems;
			}
		}

		/// <summary>
		/// Get the value as an Array.
		/// </summary>
		/// <returns> Array of values. </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public NetcdfSharp.ucar.ma2.Array getValues()
		public virtual NcArray Values
		{
			get
			{
				if (values == null && !string.ReferenceEquals(svalue, null))
				{
					values = NcArray.factory(DataType.STRING, new int[] { 1 });
					values.setObject(values.Index, svalue);
					return values;
				}

				return values == null ? null : values.copy();
			}
			set
			{
				if (immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}

				NcArray arr = value;
				if (arr == null)
				{
					dataType = DataType.STRING;
					return;
				}

				if (arr.ElementType == typeof(char)) // turn CHAR into STRING
				{
					ArrayChar carr = (ArrayChar)arr;
					if (carr.Rank == 1) // common case
					{
						svalue = carr.getString();
						this.nelems = 1;
						this.dataType = DataType.STRING;
						return;
					}
					// otherwise its an array of Strings
					arr = carr.make1DStringArray();
				}

				// this should be a utility somewhere
				if (arr.ElementType == typeof(ByteBuffer)) // turn OPAQUE into BYTE
				{
					int totalLen = 0;
					arr.resetLocalIterator();
					while (arr.hasNext())
					{
						ByteBuffer bb = (ByteBuffer)arr.next();
						totalLen += bb.limit();
					}
					byte[] ba = new byte[totalLen];
					int pos = 0;
					arr.resetLocalIterator();
					while (arr.hasNext())
					{
						ByteBuffer bb = (ByteBuffer)arr.next();
						System.Array.Copy(bb.array(), 0, ba, pos, bb.limit());
						pos += bb.limit();
					}
					arr = NcArray.factory(DataType.BYTE, new int[] { totalLen }, ba);
				}

				if (DataTypes.getType(arr) == DataType.OBJECT)
				{
					throw new ArgumentException("Cant set Attribute with type " + arr.ElementType);
				}

				if (arr.Rank > 1)
				{
					arr = arr.reshape(new int[] { (int)arr.Size }); // make sure 1D
				}

				this.values = arr;
				this.nelems = (int)arr.Size;
				this.dataType = DataTypes.getType(arr);
			}
		}

		/// <summary>
		/// Get the value as an Object.
		/// </summary>
		/// <param name="index"> which index </param>
		/// <returns> ith value as an Object. </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Object getValue(int index)
		public virtual object getObject(int index)
		{
			if (String)
			{
				return getStringValue(index);
			}
			return getNumericValue(index);
		}

		/// <summary>
		/// True if value is of type String and not null.
		/// </summary>
		/// <returns> if its a String and not null. </returns>
		public virtual bool String
		{
			get
			{
				return (dataType == DataType.STRING) && null != StringValue;
			}
		}

		/// <summary>
		/// Retrieve String value; only call if isString() is true.
		/// </summary>
		/// <returns> String if this is a String valued attribute, else null. </returns>
		/// <seealso cref="NcAttribute.isString"/>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public String getStringValue()
		public virtual string StringValue
		{
			get
			{
				if (dataType != DataType.STRING)
				{
					return null;
				}
				return (!string.ReferenceEquals(svalue, null)) ? svalue : _getStringValue(0);
			}
			set
			{
				if (string.ReferenceEquals(value, null))
				{
					throw new ArgumentException("Attribute value cannot be null");
				}

				// get rid of trailing nul characters
				int len = value.Length;
				while ((len > 0) && (value[len - 1] == (char)0))
				{
					len--;
				}
				if (len != value.Length)
				{
					value = value.Substring(0, len);
				}

				this.svalue = value;
				this.nelems = 1;
				this.dataType = DataType.STRING;
			}
		}

		public virtual string getStringValue() => StringValue;

		/// <summary>
		/// Retrieve ith String value; only call if isString() is true.
		/// </summary>
		/// <param name="index"> which index </param>
		/// <returns> ith String value (if this is a String valued attribute and index in range), else null. </returns>
		/// <seealso cref="NcAttribute.isString"/>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public String getStringValue(int index)
		public virtual string getStringValue(int index)
		{
			if (dataType != DataType.STRING)
			{
				return null;
			}
			if ((!string.ReferenceEquals(svalue, null)) && (index == 0))
			{
				return svalue;
			}
			return _getStringValue(index);
		}

		private string _getStringValue(int index)
		{
			if ((index < 0) || (index >= nelems))
			{
				return null;
			}
			return (string)values.getObject(index);
		}

		/// <summary>
		/// Retrieve numeric value. Equivalent to <code>getNumericValue(0)</code>
		/// </summary>
		/// <returns> the first element of the value array, or null if its a String that cant be converted. </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Number getNumericValue()
		public virtual object NumericValue
		{
			get
			{
				return getNumericValue(0);
			}
		}

		public virtual object getNumericValue() => NumericValue;

		/// these deal with array-valued attributes

		/// <summary>
		/// Retrieve a numeric value by index. If it's a String, it will try to parse it as a double.
		/// </summary>
		/// <param name="index"> the index into the value array. </param>
		/// <returns> Number <code>value[index]</code>, or null if its a non-parseable String or
		///         the index is out of range. </returns>
		// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
		// ORIGINAL LINE: @Nullable public Number getNumericValue(int index)
		public virtual object getNumericValue(int index)
		{
			if ((index < 0) || (index >= nelems))
			{
				return null;
			}

			// LOOK can attributes be enum valued? for now, no
			switch (dataType)
			{
				case DataType.STRING:
					try
					{
						return Convert.ToDouble(getStringValue(index));
					}
					catch (System.FormatException)
					{
						return null;
					}
				case DataType.BYTE:
				case DataType.UBYTE:
					return values.getByte(index);
				case DataType.SHORT:
				case DataType.USHORT:
					return values.getShort(index);
				case DataType.INT:
				case DataType.UINT:
					return values.getInt(index);
				case DataType.FLOAT:
					return values.getFloat(index);
				case DataType.DOUBLE:
					return values.getDouble(index);
				case DataType.LONG:
				case DataType.ULONG:
					return values.getLong(index);
			}
			return null;
		}

		/// <summary>
		/// Type-safe value access: get the first value as T, converting as needed.
		/// Equivalent to getNumericValue() with automatic conversion; strings are returned as-is.
		/// </summary>
		/// <typeparam name="T"> desired value type, e.g. double, int, string </typeparam>
		/// <returns> the first value, converted to T, or default(T) if not available </returns>
		public virtual T getValue<T>()
		{
			if (typeof(T) == typeof(string) && String)
			{
				return (T)(object)getStringValue(0);
			}
			object val = getNumericValue();
			return (val == null) ? default(T) : (T)Convert.ChangeType(val, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Type-safe value access by index: get the value at <paramref name="index"/> as T, converting as needed.
		/// </summary>
		/// <typeparam name="T"> desired value type, e.g. double, int, string </typeparam>
		/// <param name="index"> the index into the value array </param>
		/// <returns> the value, converted to T, or default(T) if not available </returns>
		public virtual T getValue<T>(int index)
		{
			if (typeof(T) == typeof(string) && String)
			{
				return (T)(object)getStringValue(index);
			}
			object val = getNumericValue(index);
			return (val == null) ? default(T) : (T)Convert.ChangeType(val, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
		}

		public override string ToString()
		{
			return toString(false);
		}

		/// <summary>
		/// CDL representation, may be strict.
		/// </summary>
		/// <param name="strict"> if true, create strict CDL, escaping names </param>
		/// <returns> CDL representation </returns>
		/// @deprecated use CDLWriter 
		[Obsolete("use CDLWriter")]
		public virtual string toString(bool strict)
		{
			Formatter f = new Formatter();
			writeCDL(f, strict, null);
			return f.ToString();
		}

		/// <summary>
		/// Write CDL representation into a Formatter.
		/// </summary>
		/// <param name="f"> write into this </param>
		/// <param name="strict"> if true, create strict CDL, escaping names </param>
		/// @deprecated use CDLWriter 
		[Obsolete("use CDLWriter")]
		protected internal virtual void writeCDL(Formatter f, bool strict, string parentname)
		{
			if (strict && (String || this.EnumType != null))
			{
				// Force type explicitly for string.
				f.format("string "); // note lower case and trailing blank
			}
			if (strict && !string.ReferenceEquals(parentname, null))
			{
				f.format(NetcdfFiles.makeValidCDLName(parentname));
			}
			f.format(":");
			f.format("%s", strict ? NetcdfFiles.makeValidCDLName(ShortName) : ShortName);
			if (String)
			{
				f.format(" = ");
				for (int i = 0; i < Length; i++)
				{
					if (i != 0)
					{
						f.format(", ");
					}
					string val = getStringValue(i);
					if (!string.ReferenceEquals(val, null))
					{
						f.format("\"%s\"", encodeString(val));
					}
				}
			}
			else if (EnumType != null)
			{
				f.format(" = ");
				for (int i = 0; i < Length; i++)
				{
					if (i != 0)
					{
						f.format(", ");
					}
					EnumTypedef en = EnumType;
					string econst = getStringValue(i);
					int? ecint = en.lookupEnumInt(econst);
					if (ecint == null)
					{
						throw new ForbiddenConversionException("Illegal enum constant: " + econst);
					}
					f.format("\"%s\"", encodeString(econst));
				}
			}
			else
			{
				f.format(" = ");
				for (int i = 0; i < Length; i++)
				{
					if (i != 0)
					{
						f.format(", ");
					}

					object number = getNumericValue(i);
					if (dataType.isUnsigned())
					{
						// 'number' is unsigned, but will be treated as signed when we print it below, because Java only has signed
						// types. If it is large enough ( >= 2^(BIT_WIDTH-1) ), its most-significant bit will be interpreted as the
						// sign bit, which will result in an invalid (negative) value being printed. To prevent that, we're going
						// to widen the number before printing it.
						number = DataTypes.widenNumber(number);
					}
					f.format("%s", number);

					if (dataType.isUnsigned())
					{
						f.format("U");
					}

					if (dataType == DataType.FLOAT)
					{
						f.format("f");
					}
					else if (dataType == DataType.SHORT || dataType == DataType.USHORT)
					{
						f.format("S");
					}
					else if (dataType == DataType.BYTE || dataType == DataType.UBYTE)
					{
						f.format("B");
					}
					else if (dataType == DataType.LONG || dataType == DataType.ULONG)
					{
						f.format("L");
					}
				}
			}
		}

		private static char[] org = new char[] { '\b', '\f', '\n', '\r', '\t', '\\', '\'', '\"' };
		private static string[] replace = new string[] { "\\b", "\\f", "\\n", "\\r", "\\t", "\\\\", "\\\'", "\\\"" };

		[Obsolete]
		private static string encodeString(string s)
		{
			return StringUtil2.replace(s, org, replace);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="name"> name of new Attribute </param>
		/// <param name="from"> copy value from here. </param>
		/// @deprecated Use Attribute.toBuilder().build(); 
		[Obsolete("Use Attribute.toBuilder().build();")]
		public NcAttribute(string name, NcAttribute from) : base(name)
		{
			if (string.ReferenceEquals(name, null))
			{
				throw new ArgumentException("Trying to set name to null on " + this);
			}
			DataType = from.dataType;
			EnumType = from.enumtype;
			this.nelems = from.nelems;
			this.svalue = from.svalue;
			this.values = from.values;
			setImmutable();
		}

		/// <summary>
		/// Create a String-valued Attribute.
		/// </summary>
		/// <param name="name"> name of Attribute </param>
		/// <param name="val"> value of Attribute </param>
		public NcAttribute(string name, string val) : base(name)
		{
			if (string.ReferenceEquals(name, null))
			{
				throw new ArgumentException("Trying to set name to null on " + this);
			}
			this.dataType = DataType.STRING;
			StringValue = val;
			setImmutable();
		}

		/// <summary>
		/// Create a scalar, signed, numeric-valued Attribute.
		/// </summary>
		/// <param name="name"> name of Attribute </param>
		/// <param name="val"> value of Attribute </param>
		public NcAttribute(string name, object val) : this(name, val, false)
		{
		}

		/// <summary>
		/// Create a scalar numeric-valued Attribute, possibly unsigned.
		/// </summary>
		/// <param name="name"> name of Attribute </param>
		/// <param name="val"> value of Attribute </param>
		/// <param name="isUnsigned"> if value is unsigned, used only for integer types. </param>
		/// @deprecated Use Attribute.builder() 
		[Obsolete("Use Attribute.builder()")]
		public NcAttribute(string name, object val, bool isUnsigned) : base(name)
		{
			if (string.ReferenceEquals(name, null))
			{
				throw new ArgumentException("Trying to set name to null on " + this);
			}

			int[] shape = new int[1];
			shape[0] = 1;
			DataType dt = DataTypes.getType(val.GetType(), isUnsigned);
			this.dataType = dt;
			NcArray vala = NcArray.factory(dt, shape);
			NcIndex ima = vala.Index;
			vala.setObject(ima.set0(0), val);
			Values = vala; // make private
			setImmutable();
		}

		/// <summary>
		/// Construct attribute with Array of values.
		/// </summary>
		/// <param name="name"> name of attribute </param>
		/// <param name="values"> array of values. </param>
		/// @deprecated Use Attribute.builder() 
		[Obsolete("Use Attribute.builder()")]
		public NcAttribute(string name, NcArray values) : this(name, values.DataType)
		{
			Values = values; // make private
			setImmutable();
		}

		/// <summary>
		/// Construct an empty attribute with no values
		/// </summary>
		/// @deprecated Use Attribute.builder() 
		[Obsolete("Use Attribute.builder()")]
		public NcAttribute(string name, DataType dataType) : this(name)
		{
			DataType = dataType;
		}

		/// @deprecated Use Attribute.builder() 
		[Obsolete("Use Attribute.builder()")]
		public NcAttribute(string name, System.Collections.IList values) : this(name, values, false)
		{
		}

		/// <summary>
		/// Construct attribute with list of String or Number values.
		/// The list determines the attribute type
		/// </summary>
		/// <param name="name"> name of attribute </param>
		/// <param name="values"> list of values. must be String or Number, must all be the same type, and have at least 1 member </param>
		/// <param name="isUnsigned"> if the data type is unsigned. </param>
		/// @deprecated Use Attribute.builder() 
		[Obsolete("Use Attribute.builder()")]
		public NcAttribute(string name, System.Collections.IList values, bool isUnsigned) : this(name)
		{
			if (values == null || values.Count == 0)
			{
				throw new ArgumentException("Cannot determine attribute's type");
			}
			Type c = values[0].GetType();
			this.dataType = DataTypes.getType(c, isUnsigned);
			setValues(values); // make private
			setImmutable();
		}

		/// <summary>
		/// A copy constructor using a NetcdfSharp.ucar.unidata.util.Parameter.
		/// Need to do this so NetcdfSharp.ucar.unidata.geoloc package doesnt depend on ucar.nc2 library
		/// </summary>
		/// <param name="param"> copy info from here. </param>
		/// @deprecated Use Attribute.builder() 
		[Obsolete("Use Attribute.builder()")]
		public NcAttribute(Parameter param) : this(param.Name)
		{

			if (param.String)
			{
				StringValue = param.StringValue;

			}
			else
			{
				double[] values = param.NumericValues;
				int n = values.Length;
				NcArray vala = NcArray.factory(DataType.DOUBLE, new int[] { n }, values);
				Values = vala; // make private
			}
			setImmutable();
		}


		//////////////////////////////////////////
		// the following make this mutable, but its restricted to subclasses, namely DODSAttribute


		/// <summary>
		/// Constructor. Must also set value
		/// </summary>
		/// <param name="name"> name of Attribute </param>
		/// @deprecated Use Attribute.builder() 


		protected NcAttribute(string name) : base(name)
		{
			if (string.ReferenceEquals(name, null))
			{
				throw new ArgumentException("Trying to set name to null on " + this);
			}
		}

		/// <summary>
		/// Set the values from a list
		/// </summary>
		/// @deprecated Use Attribute.builder() 
		[Obsolete("Use Attribute.builder()")]
		public virtual void setValues(System.Collections.IList values)
		{
			if (values == null || values.Count == 0)
			{
				throw new ArgumentException("Cannot determine attribute's type");
			}
			int n = values.Count;
			Type c = values[0].GetType();
			object pa;

			if (c == typeof(string))
			{
				string[] va = new string[n];
				pa = va;
				for (int i = 0; i < n; i++)
				{
					va[i] = (string)values[i];
				}
			}
			else if (c == typeof(int))
			{
				int[] va = new int[n];
				pa = va;
				for (int i = 0; i < n; i++)
				{
					va[i] = (int)values[i];
				}
			}
			else if (c == typeof(double))
			{
				double[] va = new double[n];
				pa = va;
				for (int i = 0; i < n; i++)
				{
					va[i] = (double)values[i];
				}
			}
			else if (c == typeof(float))
			{
				float[] va = new float[n];
				pa = va;
				for (int i = 0; i < n; i++)
				{
					va[i] = (float)values[i];
				}
			}
			else if (c == typeof(short))
			{
				short[] va = new short[n];
				pa = va;
				for (int i = 0; i < n; i++)
				{
					va[i] = (short)values[i];
				}
			}
			else if (c == typeof(byte))
			{
				byte[] va = new byte[n];
				pa = va;
				for (int i = 0; i < n; i++)
				{
					va[i] = (byte)values[i];
				}
			}
			else if (c == typeof(long))
			{
				long[] va = new long[n];
				pa = va;
				for (int i = 0; i < n; i++)
				{
					va[i] = (long)values[i];
				}
			}
			else
			{
				throw new ArgumentException("Unknown type for Attribute = " + c.Name);
			}
			Values = NcArray.factory(this.dataType, new int[] { n }, pa);
		}

		/// <summary>
		/// Instances which have same content are equal.
		/// </summary>
		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is NcAttribute))
			{
				return false;
			}

			NcAttribute att = (NcAttribute)o;

			string name = ShortName;
			if (!name.Equals(att.ShortName))
			{
				return false;
			}
			if (nelems != att.nelems)
			{
				return false;
			}
			if (dataType != att.dataType)
			{
				return false;
			}

			if (String)
			{
				return att.StringValue.Equals(StringValue);
			}

			if (values != null)
			{
				for (int i = 0; i < Length; i++)
				{
					int r1 = String ? getStringValue(i).GetHashCode() : getNumericValue(i).GetHashCode();
					int r2 = att.String ? att.getStringValue(i).GetHashCode() : att.getNumericValue(i).GetHashCode();
					if (r1 != r2)
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Override Object.hashCode() to implement equals.
		/// </summary>
		public override int GetHashCode()
		{
			int result = 17;
			result = 37 * result + ShortName.GetHashCode();
			result = 37 * result + nelems;
			result = 37 * result + DataType.GetHashCode();
			if (!string.ReferenceEquals(svalue, null))
			{
				result = 37 * result + svalue.GetHashCode();
			}
			else if (values != null)
			{
				for (int i = 0; i < Length; i++)
				{
					int h = String ? getStringValue(i).GetHashCode() : getNumericValue(i).GetHashCode();
					result = 37 * result + h;
				}
			}
			return result;
		}

		///////////////////////////////////////////////////////////////////////////////

		// TODO make these final in 6.
		private string svalue; // optimization for common case of single String valued attribute
		private DataType dataType;
		private EnumTypedef enumtype;
		private int nelems; // can be 0 or greater
		private NcArray values; // can this be made immutable?? Otherwise return a copy.

		private NcAttribute(Builder builder) : base(builder.name)
		{
			this.svalue = builder.svalue;
			this.dataType = builder.dataType;
			this.enumtype = builder.enumtype;
			this.nelems = builder.nelems;
			this.values = builder.values;
			this.nelems = (!string.ReferenceEquals(svalue, null)) ? 1 : (this.values != null) ? (int)this.values.Size : 0;
		}

		/// <summary>
		/// Turn into a mutable Builder. Can use toBuilder().build() to copy. </summary>
		public virtual Builder toBuilder()
		{
			Builder b = builder().setName(this.shortName).setDataType(this.dataType).setEnumType(this.enumtype);
			if (!string.ReferenceEquals(this.svalue, null))
			{
				b.setStringValue(this.svalue);
			}
			else if (this.values != null)
			{
				b.setValues(this.values);
			}
			return b;
		}

		/// <summary>
		/// Create an Attribute builder. </summary>
		public static Builder builder()
		{
			return new Builder();
		}

		/// <summary>
		/// Create an Attribute builder with the given Attribute name. </summary>
		public static Builder builder(string name)
		{
			return new Builder().setName(name);
		}

		/// <summary>
		/// A builder for Attributes
		/// </summary>
		public class Builder
		{
			internal string name;
			internal DataType dataType;
			internal string svalue; // optimization for common case of single String valued attribute
			internal NcArray values;
			internal int nelems;
			internal EnumTypedef enumtype;
			private bool built;

			internal Builder()
			{
			}

			public virtual Builder setName(string name)
			{
				this.name = StringUtil2.makeValidCdmObjectName(name);
				return this;
			}

			public virtual Builder setDataType(DataType dataType)
			{
				this.dataType = dataType;
				return this;
			}

			public virtual Builder setEnumType(EnumTypedef enumtype)
			{
				this.enumtype = enumtype;
				return this;
			}

			public virtual Builder setNumericValue(object val, bool isUnsigned)
			{
				int[] shape = new int[] { 1 };
				DataType dt = DataTypes.getType(val.GetType(), isUnsigned);
				setDataType(dt);
				NcArray vala = NcArray.factory(dt, shape);
				NcIndex ima = vala.Index;
				vala.setObject(ima.set0(0), val);
				setValues(vala);
				return this;
			}

			/// <summary>
			/// Set the value as a String, trimming trailing zeros
			/// </summary>
			/// <param name="svalue"> value of Attribute </param>
			public virtual Builder setStringValue(string svalue)
			{
				if (string.ReferenceEquals(svalue, null))
				{
					throw new ArgumentException("Attribute value cannot be null");
				}

				// get rid of trailing nul characters
				int len = svalue.Length;
				while ((len > 0) && (svalue[len - 1] == (char)0))
				{
					len--;
				}
				if (len != svalue.Length)
				{
					svalue = svalue.Substring(0, len);
				}

				this.svalue = svalue;
				this.nelems = 1;
				this.dataType = DataType.STRING;
				return this;
			}

			/// <summary>
			/// Set the values from a list of String or one of the primitives
			/// Integer, Float, Double, Short, Long, Integer, Byte.
			/// </summary>
			public virtual Builder setValues(IList<object> values, bool unsigned)
			{
				if (values == null || values.Count == 0)
				{
					throw new ArgumentException("Cannot determine attribute's type");
				}
				int n = values.Count;
				Type c = values[0].GetType();
				object pa;

				if (c == typeof(string))
				{
					this.dataType = DataType.STRING;
					string[] va = new string[n];
					pa = va;
					for (int i = 0; i < n; i++)
					{
						va[i] = (string)values[i];
					}
				}
				else if (c == typeof(int))
				{
					this.dataType = unsigned ? DataType.UINT : DataType.INT;
					int[] va = new int[n];
					pa = va;
					for (int i = 0; i < n; i++)
					{
						va[i] = (int)values[i];
					}
				}
				else if (c == typeof(double))
				{
					this.dataType = DataType.DOUBLE;
					double[] va = new double[n];
					pa = va;
					for (int i = 0; i < n; i++)
					{
						va[i] = (double)values[i];
					}
				}
				else if (c == typeof(float))
				{
					this.dataType = DataType.FLOAT;
					float[] va = new float[n];
					pa = va;
					for (int i = 0; i < n; i++)
					{
						va[i] = (float)values[i];
					}
				}
				else if (c == typeof(short))
				{
					this.dataType = unsigned ? DataType.USHORT : DataType.SHORT;
					short[] va = new short[n];
					pa = va;
					for (int i = 0; i < n; i++)
					{
						va[i] = (short)values[i];
					}
				}
				else if (c == typeof(byte))
				{
					this.dataType = unsigned ? DataType.UBYTE : DataType.BYTE;
					byte[] va = new byte[n];
					pa = va;
					for (int i = 0; i < n; i++)
					{
						va[i] = (byte)values[i];
					}
				}
				else if (c == typeof(long))
				{
					this.dataType = unsigned ? DataType.ULONG : DataType.LONG;
					long[] va = new long[n];
					pa = va;
					for (int i = 0; i < n; i++)
					{
						va[i] = (long)values[i];
					}
				}
				else
				{
					throw new ArgumentException("Unknown type for Attribute = " + c.Name);
				}

				return setValues(NcArray.factory(this.dataType, new int[] { n }, pa));
			}

			/// <summary>
			/// Set the values from an Array, and the DataType from values.getElementType().
			/// </summary>
			public virtual Builder setValues(NcArray arr)
			{
				if (arr == null)
				{
					dataType = DataType.STRING;
					return this;
				}

				if (arr.ElementType == typeof(char)) // turn CHAR into STRING
				{
					ArrayChar carr = (ArrayChar)arr;
					if (carr.Rank == 1) // common case
					{
						svalue = carr.getString();
						this.nelems = 1;
						this.dataType = DataType.STRING;
						return this;
					}
					// otherwise its an array of Strings
					arr = carr.make1DStringArray();
				}

				// this should be a utility somewhere
				if (arr.ElementType == typeof(ByteBuffer)) // turn OPAQUE into BYTE
				{
					int totalLen = 0;
					arr.resetLocalIterator();
					while (arr.hasNext())
					{
						ByteBuffer bb = (ByteBuffer)arr.next();
						totalLen += bb.limit();
					}
					byte[] ba = new byte[totalLen];
					int pos = 0;
					arr.resetLocalIterator();
					while (arr.hasNext())
					{
						ByteBuffer bb = (ByteBuffer)arr.next();
						System.Array.Copy(bb.array(), 0, ba, pos, bb.limit());
						pos += bb.limit();
					}
					arr = NcArray.factory(DataType.BYTE, new int[] { totalLen }, ba);
				}

				if (DataTypes.getType(arr) == DataType.OBJECT)
				{
					throw new ArgumentException("Cant set Attribute with type " + arr.ElementType);
				}

				if (arr.Rank > 1)
				{
					arr = arr.reshape(new int[] { (int)arr.Size }); // make sure 1D
				}

				this.values = arr;
				this.nelems = (int)arr.Size;
				this.dataType = DataTypes.getType(arr);
				return this;
			}

			public virtual NcAttribute build()
			{
				if (built)
				{
					throw new System.InvalidOperationException("already built");
				}
				built = true;
				return new NcAttribute(this);
			}
		}

	}

}
