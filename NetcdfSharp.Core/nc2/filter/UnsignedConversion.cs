// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using Signedness = NetcdfSharp.ucar.ma2.Signedness;

namespace NetcdfSharp.ucar.nc2.filter
{
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;

	public class UnsignedConversion : Enhancement
	{

	  private DataType outType;
	  private Signedness signedness;

	  public static UnsignedConversion createFromVar(VariableDS var)
	  {
		DataType origDataType = var.DataType;
		// won't need to convert non-integral types
		if (!origDataType.isIntegral())
		{
		  return new UnsignedConversion(origDataType, Signedness.SIGNED);
		}
		DataType unsignedConversionType = origDataType;
		// unsignedConversionType may start out as integral, but then be widened to non-integral (i.e. LONG -> DOUBLE)
		// so we cannot rely upon unsignedConversionType to store the signedness of the variable. We need a separate field.
		Signedness signedness = origDataType.getSignedness();

		// In the event of conflict, "unsigned" wins. Potential conflicts include:
		// 1. origDataType is unsigned, but variable has "_Unsigned == false" attribute.
		// 2. origDataType is signed, but variable has "_Unsigned == true" attribute.
		if (signedness == Signedness.SIGNED)
		{
		  string unsignedAtt = var.attributes().findAttributeString(CDM.UNSIGNED, null);
		  if (!string.ReferenceEquals(unsignedAtt, null) && unsignedAtt.Equals("true", StringComparison.OrdinalIgnoreCase))
		  {
			signedness = Signedness.UNSIGNED;
		  }
		}

		if (signedness == Signedness.UNSIGNED)
		{
		  // We may need a larger data type to hold the results of the unsigned conversion.
		  unsignedConversionType = FilterHelpers.nextLarger(origDataType).withSignedness(Signedness.UNSIGNED);
		}
		return new UnsignedConversion(unsignedConversionType, signedness);
	  }

	  public UnsignedConversion(DataType outType, Signedness signedness)
	  {
		this.outType = outType;
		this.signedness = signedness;
	  }

	  public virtual DataType OutType
	  {
		  get
		  {
			return this.outType;
		  }
	  }

	  public virtual Signedness Signedness
	  {
		  get
		  {
			return this.signedness;
		  }
	  }

	  public virtual double convert(double value)
	  {
		object val;
		switch (outType)
		{
		  case DataType.UBYTE:
			val = new byte?((byte) value);
			break;
		  case DataType.USHORT:
			val = new byte?((byte) value);
			break;
		  case DataType.UINT:
			val = new short?((short) value);
			break;
		  case DataType.ULONG:
			val = new int?((int) value);
			break;
		  default:
			val = new double?(value);
		break;
		}
		return this.signedness == Signedness.UNSIGNED ? Convert.ToDouble(DataTypes.widenNumberIfNegative(val)) : value;
	  }

	  public virtual NcArray convertUnsigned(NcArray @in)
	  {
		if (signedness == Signedness.SIGNED)
		{
		  return @in;
		}
		NcArray @out = NcArray.factory(outType, @in.Shape);
		IndexIterator iterIn = @in.IndexIterator;
		IndexIterator iterOut = @out.IndexIterator;

		try
		{
		  // iterate and convert elements
		  while (iterIn.hasNext())
		  {
			object value = iterIn.ObjectNext;
			value = DataTypes.widenNumberIfNegative(value);
			iterOut.ObjectNext = value;
		  }
		}
		catch (System.InvalidCastException)
		{
		  return @in;
		}

		return @out;
	  }

	  public static object convertUnsigned(object value, DataType dataType)
	  {
		return dataType.isUnsigned() ? DataTypes.widenNumberIfNegative(value) : value;
	  }
	}

}
