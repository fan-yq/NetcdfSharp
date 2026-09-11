// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using Signedness = NetcdfSharp.ucar.ma2.Signedness;

/*
 * Copyright (c) 2021 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.filter
{
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;


	// JAVA TO C# CONVERTER TASK: This Java 'import static' statement cannot be converted to C#:
	//	import static NetcdfSharp.ucar.ma2.DataType.*;

	/// <summary>
	/// Filter implementation of FixedScaleOffset as described by the
	/// <a href="https://numcodecs.readthedocs.io/en/stable/fixedscaleoffset.html">NumCodecs</a> project
	/// </summary>
	public class ScaleOffset : Filter, Enhancement
	{

		private const string name = "fixedscaleoffset";

		private const int id = 6;

		public class Keys
		{
			public const string OFFSET_KEY = "offset";
			public const string SCALE_KEY = "scale";
			public const string DTYPE_KEY = "dtype";
			public const string ASTYPE_KEY = "astype";
		}

		// maps numeric zarr datatypes to CDM datatypes
		private static IDictionary<string, DataType> dTypeMap;

		static ScaleOffset()
		{
			dTypeMap = new Dictionary<string, DataType>();
			dTypeMap["i1"] = DataType.BYTE;
			dTypeMap["u1"] = DataType.UBYTE;
			dTypeMap["i2"] = DataType.SHORT;
			dTypeMap["u2"] = DataType.USHORT;
			dTypeMap["i4"] = DataType.INT;
			dTypeMap["f4"] = DataType.FLOAT;
			dTypeMap["u4"] = DataType.UINT;
			dTypeMap["i8"] = DataType.LONG;
			dTypeMap["f8"] = DataType.DOUBLE;
			dTypeMap["u8"] = DataType.ULONG;
		}

		private readonly double offset;
		private readonly double scale;

		private const double DEFAULT_OFFSET = 0.0;
		private const double DEFAULT_SCALE = 1.0;

		// type information for original data type
		private readonly ByteOrder dtypeOrder;
		private readonly DataType dtype;

		// type information for storage type
		private readonly DataType astype;
		private readonly ByteOrder astypeOrder;

		public static ScaleOffset createFromVariable(VariableDS var)
		{

			DataType? scaleType = null, offsetType = null;
			double scale = DEFAULT_SCALE, offset = DEFAULT_OFFSET;

			DataType origDataType = var.DataType;
			Signedness signedness = var.Signedness;

			NcAttribute scaleAtt = var.findAttribute(CDM.SCALE_FACTOR);
			if (scaleAtt != null && !scaleAtt.String)
			{
				scaleType = FilterHelpers.getAttributeDataType(scaleAtt, signedness);
				scale = 1 / Convert.ToDouble(var.convertUnsigned(scaleAtt.NumericValue, scaleType.Value));
				var.remove(scaleAtt);
			}

			NcAttribute offsetAtt = var.findAttribute(CDM.ADD_OFFSET);
			if (offsetAtt != null && !offsetAtt.String)
			{
				offsetType = FilterHelpers.getAttributeDataType(offsetAtt, signedness);
				offset = Convert.ToDouble(var.convertUnsigned(offsetAtt.NumericValue, offsetType.Value));
				var.remove(offsetAtt);
			}

			if (scale != DEFAULT_SCALE || offset != DEFAULT_OFFSET)
			{
				DataType scaledOffsetType = FilterHelpers.largestOf(var.UnsignedConversionType, scaleType, offsetType).withSignedness(signedness);

				IDictionary<string, object> scaleOffsetProps = new Dictionary<string, object>();
				scaleOffsetProps[ScaleOffset.Keys.OFFSET_KEY] = offset;
				scaleOffsetProps[ScaleOffset.Keys.SCALE_KEY] = scale;
				scaleOffsetProps[ScaleOffset.Keys.DTYPE_KEY] = scaledOffsetType;
				scaleOffsetProps[ScaleOffset.Keys.ASTYPE_KEY] = origDataType;
				return new ScaleOffset(scaleOffsetProps);
			}
			return null;
		}

		public ScaleOffset(IDictionary<string, object> properties)
		{
			// get offset and scale parameters
			offset = Convert.ToDouble(properties.GetOrDefault(Keys.OFFSET_KEY, DEFAULT_OFFSET));
			scale = Convert.ToDouble(properties.GetOrDefault(Keys.SCALE_KEY, DEFAULT_SCALE));

			// input data type
			object typeProp = properties.TryGetValue(Keys.DTYPE_KEY, out object typeVal) ? typeVal : null;
			if (typeProp is string)
			{
				string type = (string)typeProp;
				DataType? parsed = parseDataType(type);
				if (parsed == null)
				{
					throw new Exception("ScaleOffset error: could not parse dtype");
				}
				dtype = parsed.Value;
				dtypeOrder = parseByteOrder(type, ByteOrder.LITTLE_ENDIAN);
			}
			else if (typeProp is DataType)
			{
				dtype = (DataType)typeProp;
				dtypeOrder = ByteOrder.LITTLE_ENDIAN;
			}
			else
			{
				throw new Exception("ScaleOffset error: could not parse dtype");
			}

			// get storage type, if exists, or default to dtype
			object aTypeProp = properties.GetOrDefault(Keys.ASTYPE_KEY, null);
			if (aTypeProp is string)
			{
				string aType = (string)aTypeProp;
				astype = parseDataType(aType) ?? dtype;
				astypeOrder = parseByteOrder(aType, dtypeOrder);
			}
			else if (aTypeProp is DataType)
			{
				astype = (DataType)aTypeProp;
				astypeOrder = ByteOrder.LITTLE_ENDIAN;
			}
			else
			{
				astype = dtype;
				astypeOrder = dtypeOrder;
			}
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		public override int Id
		{
			get
			{
				return id;
			}
		}

		public virtual double ScaleFactor
		{
			get
			{
				return this.scale;
			}
		}

		public virtual double Offset
		{
			get
			{
				return this.offset;
			}
		}

		public virtual DataType ScaledOffsetType
		{
			get
			{
				return this.dtype;
			}
		}

		public override byte[] encode(byte[] dataIn)
		{
			if (scale == DEFAULT_SCALE && offset == DEFAULT_OFFSET)
			{
				return dataIn;
			}
			NcArray @out = applyScaleOffset(FilterHelpers.bytesToArray(dataIn, dtype, dtypeOrder));
			return FilterHelpers.arrayToBytes(@out, astype, astypeOrder);
		}

		public override byte[] decode(byte[] dataIn)
		{
			if (scale == DEFAULT_SCALE && offset == DEFAULT_OFFSET)
			{
				return dataIn;
			}
			NcArray @out = convert(FilterHelpers.bytesToArray(dataIn, astype, astypeOrder));
			return FilterHelpers.arrayToBytes(@out, dtype, dtypeOrder);
		}

		public virtual NcArray applyScaleOffset(NcArray @in)
		{
			if (scale == DEFAULT_SCALE && offset == DEFAULT_OFFSET)
			{
				return @in;
			}
			// use wider datatype if unsigned
			DataType outType = astype;
			if (astype.getSignedness() == Signedness.UNSIGNED)
			{
				outType = FilterHelpers.nextLarger(astype).withSignedness(Signedness.UNSIGNED);
			}

			// create conversion array
			NcArray @out = NcArray.factory(outType, @in.Shape);
			IndexIterator iterIn = @in.IndexIterator;
			IndexIterator iterOut = @out.IndexIterator;

			// iterate and convert elements
			while (iterIn.hasNext())
			{
				object value = convertUnsigned(iterIn.ObjectNext, dtype.getSignedness());
				double newVal = applyScaleOffset(Convert.ToDouble(value));
				iterOut.ObjectNext = newVal;
			}

			return @out;
		}

		public virtual NcArray convert(NcArray @in)
		{
			if (scale == DEFAULT_SCALE && offset == DEFAULT_OFFSET)
			{
				return @in;
			}
			// use wider datatype if unsigned
			DataType outType = dtype;
			if (dtype.getSignedness() == Signedness.UNSIGNED)
			{
				outType = FilterHelpers.nextLarger(dtype).withSignedness(Signedness.UNSIGNED);
			}

			// create conversion array
			NcArray @out = NcArray.factory(outType, @in.Shape);
			IndexIterator iterIn = @in.IndexIterator;
			IndexIterator iterOut = @out.IndexIterator;

			// iterate and convert elements
			while (iterIn.hasNext())
			{
				object value = iterIn.ObjectNext;
				value = convertUnsigned(value, astype.getSignedness());
				value = convert(Convert.ToDouble(value));
				iterOut.ObjectNext = value;
			}

			return @out;
		}

		////////////////////////////////////////////////////
		// helpers
		////////////////////////////////////////////////////
		private static DataType? parseDataType(string dtype)
		{
			dtype = dtype.Replace(">", "");
			dtype = dtype.Replace("<", "");
			dtype = dtype.Replace("|", "");
			return dTypeMap.TryGetValue(dtype, out DataType dt) ? dt : (DataType?)null;
		}

		private static ByteOrder parseByteOrder(string dtype, ByteOrder defaultOrder)
		{
			if (dtype.StartsWith(">", StringComparison.Ordinal))
			{
				return ByteOrder.BIG_ENDIAN;
			}
			else if (dtype.StartsWith("<", StringComparison.Ordinal))
			{
				return ByteOrder.LITTLE_ENDIAN;
			}
			else if (dtype.StartsWith("|", StringComparison.Ordinal))
			{
				return ByteOrder.nativeOrder();
			}
			return defaultOrder;
		}

		public virtual double applyScaleOffset(double value)
		{
			if (astype.isIntegral())
			{
				return (long)Math.Floor((value - offset) * scale + 0.5);
			}
			return (value - offset) * scale;
		}

		public virtual double convert(double value)
		{
			if (dtype.isIntegral())
			{
				return (long)Math.Floor(value / scale + offset + 0.5);
			}
			return value / scale + offset;
		}

		private object convertUnsigned(object value, Signedness signedness)
		{
			if (signedness == Signedness.UNSIGNED)
			{
				// Handle integral types that should be treated as unsigned by widening them if necessary.
				return DataTypes.widenNumberIfNegative(value);
			}
			else
			{
				return value;
			}
		}


		public class Provider : FilterProvider
		{

			public virtual string Name
			{
				get
				{
					return name;
				}
			}

			public virtual int Id
			{
				get
				{
					return id;
				}
			}

			public virtual Filter create(IDictionary<string, object> properties)
			{
				return new ScaleOffset(properties);
			}

		}


	}

}
