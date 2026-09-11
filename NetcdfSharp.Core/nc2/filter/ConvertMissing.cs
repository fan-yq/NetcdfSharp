// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using Signedness = NetcdfSharp.ucar.ma2.Signedness;

namespace NetcdfSharp.ucar.nc2.filter
{
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using N3iospFillValues = NetcdfSharp.ucar.nc2.iosp.netcdf3.N3iospFillValues;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;

	public class ConvertMissing : Enhancement
	{

		private bool hasValidMin, hasValidMax;
		private double validMin, validMax;

		// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool hasFillValue_Conflict;
		private double fillValue; // LOOK: making it double not really correct. What about CHAR?

		// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private bool hasMissingValue_Conflict;
		private double[] missingValue; // LOOK: also wrong to make double, for the same reason.

		// defaults from NetcdfDataset modes
		private bool invalidDataIsMissing;
		private bool fillValueIsMissing;
		private bool missingDataIsMissing;

		public static ConvertMissing createFromVariable(VariableDS var)
		{
			// valid min and max
			Signedness signedness = var.Signedness;
			double validMin = -double.MaxValue, validMax = double.MaxValue;
			bool hasValidMin = false, hasValidMax = false;
			// assume here its in units of unpacked data. correct this below
			NcAttribute validRangeAtt = var.findAttribute(CDM.VALID_RANGE);
			DataType? validType = null;
			if (validRangeAtt != null && !validRangeAtt.String && validRangeAtt.Length > 1)
			{
				validType = FilterHelpers.getAttributeDataType(validRangeAtt, signedness);
				validMin = Convert.ToDouble(var.convertUnsigned(validRangeAtt.getNumericValue(0), validType.Value));
				validMax = Convert.ToDouble(var.convertUnsigned(validRangeAtt.getNumericValue(1), validType.Value));
				hasValidMin = true;
				hasValidMax = true;
			}

			NcAttribute validMinAtt = var.findAttribute(CDM.VALID_MIN);
			NcAttribute validMaxAtt = var.findAttribute(CDM.VALID_MAX);

			// Only process the valid_min and valid_max attributes if valid_range isn't present.
			if (!hasValidMin)
			{
				if (validMinAtt != null && !validMinAtt.String)
				{
					validType = FilterHelpers.getAttributeDataType(validMinAtt, signedness);
					validMin = Convert.ToDouble(var.convertUnsigned(validMinAtt.NumericValue, validType.Value));
					hasValidMin = true;
				}

				if (validMaxAtt != null && !validMaxAtt.String)
				{
					validType = FilterHelpers.largestOf(validType, FilterHelpers.getAttributeDataType(validMaxAtt, signedness));
					validMax = Convert.ToDouble(var.convertUnsigned(validMaxAtt.NumericValue, validType.Value));
					hasValidMax = true;
				}
			}

			if (validMin > validMax)
			{
				double temp = validMin;
				validMin = validMax;
				validMax = temp;
			}

			/// fill_value
			double fillValue = getFillValueOrDefault(var);
			bool hasFillValue = !double.IsNaN(fillValue);

			/// missing_value
			double[] missingValue = null;
			NcAttribute missingValueAtt = var.findAttribute(CDM.MISSING_VALUE);
			if (missingValueAtt != null)
			{
				if (missingValueAtt.String)
				{
					string svalue = missingValueAtt.StringValue;
					if (var.OriginalDataType == DataType.CHAR)
					{
						missingValue = new double[1];
						if (svalue.Length == 0)
						{
							missingValue[0] = 0;
						}
						else
						{
							missingValue[0] = svalue[0];
						}
					}
					else
					{ // not a CHAR - try to fix problem where they use a numeric value as a String attribute
						try
						{
							missingValue = new double[1];
							missingValue[0] = double.Parse(svalue);
						}
						catch (System.FormatException)
						{
							// TODO add logger
						}
					}
				}
				else
				{ // not a string
					missingValue = new double[missingValueAtt.Length];
					DataType missingType = FilterHelpers.getAttributeDataType(missingValueAtt, signedness);
					for (int i = 0; i < missingValue.Length; i++)
					{
						missingValue[i] = Convert.ToDouble(var.convertUnsigned(missingValueAtt.getNumericValue(i), missingType));
					}
				}
			}
			return new ConvertMissing(var.fillValueIsMissing(), var.invalidDataIsMissing(), var.missingDataIsMissing(), hasValidMin, hasValidMax, validMin, validMax, hasFillValue, fillValue, missingValue);
		}


		public ConvertMissing(bool fillValueIsMissing, bool invalidDataIsMissing, bool missingDataIsMissing, bool hasValidMin, bool hasValidMax, double validMin, double validMax, bool hasFillValue, double fillValue, double[] missingValue)
		{
			this.fillValueIsMissing = fillValueIsMissing;
			this.invalidDataIsMissing = invalidDataIsMissing;
			this.missingDataIsMissing = missingDataIsMissing;
			this.hasValidMin = hasValidMin;
			this.hasValidMax = hasValidMax;
			this.validMin = validMin;
			this.validMax = validMax;
			this.hasFillValue_Conflict = hasFillValue;
			this.fillValue = fillValue;
			this.missingValue = missingValue;
			this.hasMissingValue_Conflict = false;
			// clean up missing values: remove NaNs, fill values, and values outside valid range
			if (this.missingDataIsMissing && this.missingValue != null)
			{
				IList<double> missing = new List<double>();
				foreach (double mv in this.missingValue)
				{
					if (double.IsNaN(mv))
					{
						continue;
					}
					if (fillValueIsMissing && hasFillValue && mv == fillValue)
					{
						continue;
					}
					if (invalidDataIsMissing && hasValidMin && mv < validMin)
					{
						continue;
					}
					if (invalidDataIsMissing && hasValidMax && mv > validMax)
					{
						continue;
					}
					missing.Add(mv);
				}
				int nMissing = missing.Count;
				this.missingValue = new double[nMissing];
				for (int i = 0; i < nMissing; i++)
				{
					this.missingValue[i] = missing[i];
				}
				this.hasMissingValue_Conflict = this.missingValue.Length > 0;
			}
		}

		public virtual bool hasValidData()
		{
			return hasValidMin || hasValidMax;
		}

		public virtual double ValidMin
		{
			get
			{
				return validMin;
			}
		}

		public virtual double ValidMax
		{
			get
			{
				return validMax;
			}
		}

		public virtual bool isInvalidData(double val)
		{
			if (double.IsNaN(val))
			{
				return true;
			}
			if (val > validMax)
			{
				return true;
			}
			if (val < validMin)
			{
				return true;
			}
			return false;
		}

		public virtual bool hasFillValue()
		{
			return hasFillValue_Conflict;
		}

		public virtual bool isFillValue(double val)
		{
			return hasFillValue_Conflict && val == fillValue;
		}

		public virtual double FillValue
		{
			get
			{
				return fillValue;
			}
		}

		public virtual bool isMissingValue(double val)
		{
			foreach (double aMissingValue in missingValue)
			{
				if (val == aMissingValue)
				{
					return true;
				}
			}
			return false;
		}

		public virtual double[] MissingValues
		{
			get
			{
				return missingValue;
			}
		}

		public virtual bool hasMissingValue()
		{
			return hasMissingValue_Conflict;
		}

		public virtual bool hasMissing()
		{
			return (invalidDataIsMissing && hasValidData()) || (fillValueIsMissing && hasFillValue()) || (missingDataIsMissing && hasMissingValue());
		}

		public virtual bool isMissing(double val)
		{
			if (double.IsNaN(val))
			{
				return true;
			}
			return (missingDataIsMissing && hasMissingValue_Conflict && isMissingValue(val)) || (fillValueIsMissing && hasFillValue_Conflict && isFillValue(val)) || (invalidDataIsMissing && hasValidData() && isInvalidData(val));
		}

		public virtual double convert(double value)
		{
			return isMissing(value) ? double.NaN : value;
		}

		public virtual NcArray convertMissing(NcArray @in)
		{
			DataType type = @in.DataType;
			if (!type.isNumeric())
			{
				return @in;
			}
			if (!hasMissing())
			{
				return @in;
			}

			NcArray @out = NcArray.factory(type, @in.Shape);
			IndexIterator iterIn = @in.IndexIterator;
			IndexIterator iterOut = @out.IndexIterator;

			// iterate and convert elements
			while (iterIn.hasNext())
			{
				object value = iterIn.ObjectNext;
				value = convert(Convert.ToDouble(value));
				iterOut.ObjectNext = value;
			}

			return @out;
		}

		public static double getFillValueOrDefault(VariableDS var)
		{
			// need fill value info before convertMissing
			NcAttribute fillValueAtt = var.findAttribute(CDM.FILL_VALUE);
			if (fillValueAtt != null && !fillValueAtt.String)
			{
				DataType fillType = FilterHelpers.getAttributeDataType(fillValueAtt, var.Signedness);
				return Convert.ToDouble(var.convertUnsigned(fillValueAtt.NumericValue, fillType));
			}
			// No _FillValue attribute found. Instead, if file is NetCDF and variable is numeric, use the default fill value.
			string ncfileId = var.OriginalVariable == null ? null : var.OriginalVariable.getFileTypeId();
			if (DataFormatType.NETCDF.Description.Equals(ncfileId) || DataFormatType.NETCDF4.Description.Equals(ncfileId))
			{
				DataType fillType = var.DataType;
				if (fillType.isNumeric())
				{
					return Convert.ToDouble(var.convertUnsigned(N3iospFillValues.getFillValueDefault(fillType), fillType));
				}
			}
			return double.NaN;
		}
	}

}
