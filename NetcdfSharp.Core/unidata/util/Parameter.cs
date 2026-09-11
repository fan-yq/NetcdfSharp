// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;
using System.Linq;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.util
{

	/// <summary>
	/// A parameter has a name and a value that is String, a double, or an array of doubles.
	/// A substitute for NetcdfSharp.ucar.nc2.Attribute, to prevent dependencies of the ucar.unidata packages on ucar.nc2.
	/// 
	/// LOOK will not implement Serializable in ver6; may be replaced by Attribute.
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Parameter implements NetcdfSharp.java.io.Serializable
	[Serializable]
	public class Parameter
	{
	  private readonly string name;
	  private readonly string valueS;
	  private readonly double[] valueD;
	  private readonly bool isString;

	  /// <summary>
	  /// Copy constructor, with new name.
	  /// </summary>
	  /// <param name="name"> name of new Parameter. </param>
	  /// <param name="from"> copy values from here. </param>
	  public Parameter(string name, Parameter from)
	  {
		this.name = name;
		this.valueS = from.valueS;
		this.valueD = from.valueD;
		this.isString = from.isString;
	  }

	  /// <summary>
	  /// Create a String-valued param.
	  /// </summary>
	  /// <param name="name"> name of new Parameter. </param>
	  /// <param name="val"> value of Parameter </param>
	  public Parameter(string name, string val)
	  {
		this.name = name;
		this.valueS = val;
		this.valueD = null;
		this.isString = true;
	  }

	  /// <summary>
	  /// Create a scalar double-valued param.
	  /// </summary>
	  /// <param name="name"> name of new Parameter. </param>
	  /// <param name="value"> value of Parameter </param>
	  public Parameter(string name, double value)
	  {
		this.name = name;
		valueD = new double[1];
		valueD[0] = value;
		this.isString = false;
		this.valueS = null;
	  }

	  /// <summary>
	  /// Create an array double-valued param.
	  /// </summary>
	  /// <param name="name"> name of new Parameter. </param>
	  /// <param name="value"> value of Parameter </param>
	  public Parameter(string name, double[] value)
	  {
		this.name = name;
		valueD = (double[])value.Clone();
		this.isString = false;
		this.valueS = null;
	  }

	  /// <summary>
	  /// Get the name of this Parameter.
	  /// </summary>
	  /// <returns> name </returns>
	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  /// <summary>
	  /// True if value is a String.
	  /// </summary>
	  /// <returns> if its String valued </returns>
	  public virtual bool String
	  {
		  get
		  {
			return isString;
		  }
	  }

	  /// <summary>
	  /// Retrieve String value.
	  /// </summary>
	  /// <returns> String value if this is a String valued attribute, else null. </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public String getStringValue()
	  public virtual string StringValue
	  {
		  get
		  {
			return valueS;
		  }
	  }

	  /// <summary>
	  /// Retrieve numeric value, use if isString() is false.
	  /// Equivalent to <code>getNumericValue(0)</code>
	  /// </summary>
	  /// <returns> the first element of the value array, or Exception if its a String. </returns>
	  public virtual double NumericValue
	  {
		  get
		  {
			return valueD[0];
		  }
	  }

	  /// <summary>
	  /// Get the ith numeric value.
	  /// </summary>
	  /// <param name="i"> index </param>
	  /// <returns> ith numeric value, or Exception if its a String. </returns>
	  public virtual double getNumericValue(int i)
	  {
		return valueD[i];
	  }

	  /// <summary>
	  /// Get the number of double values.
	  /// </summary>
	  /// <returns> the number of values, 0 is its a string. </returns>
	  public virtual int Length
	  {
		  get
		  {
			return isString ? 0 : valueD.Length;
		  }
	  }

	  /// <summary>
	  /// Get array of numeric values as doubles.
	  /// Do not modify unless you own this object!
	  /// </summary>
	  /// <returns> array of numeric values. LOOK: will return copy of array in ver 6. </returns>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public double[] getNumericValues()
	  public virtual double[] NumericValues
	  {
		  get
		  {
			return valueD;
		  }
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		Parameter parameter = (Parameter) o;

		if (isString != parameter.isString)
		{
		  return false;
		}
		if (!name.Equals(parameter.name))
		{
		  return false;
		}
		if (!Objects.equal(valueS, parameter.valueS))
		{
		  return false;
		}
		return valueD.SequenceEqual(parameter.valueD);
	  }

	  public override int GetHashCode()
	  {
		int result = name.GetHashCode();
		result = 31 * result + (!string.ReferenceEquals(valueS, null) ? valueS.GetHashCode() : 0);
		result = 31 * result + Arrays.hashCode(valueD);
		result = 31 * result + (isString ? 1 : 0);
		return result;
	  }

	  public override string ToString()
	  {
		StringBuilder buff = new StringBuilder();
		buff.Append(Name);
		if (String)
		{
		  buff.Append(" = ");
		  buff.Append(valueS);
		}
		else
		{
		  buff.Append(" = ");
		  for (int i = 0; i < Length; i++)
		  {
			if (i != 0)
			{
			  buff.Append(", ");
			}
			buff.Append(getNumericValue(i));
		  }
		}
		return buff.ToString();
	  }
	}

}
