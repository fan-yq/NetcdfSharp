// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{

	/// <summary>
	/// Provides support for classes that implement units.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @Immutable public abstract class UnitImpl implements Unit, NetcdfSharp.java.io.Serializable
	[Serializable]
	public abstract class UnitImpl : Unit
	{
		public abstract bool Dimensionless { get; }
		public override abstract bool Equals(object @object);
		public abstract Unit clone(UnitName id);
		public abstract DerivedUnit DerivedUnit { get; }
		public abstract string CanonicalString { get; }
		private const long serialVersionUID = 1L;
		/// <summary>
		/// The unit identifier.
		/// 
		/// @serial
		/// </summary>
		private readonly UnitName id;

		/// <summary>
		/// Constructs with no ID.
		/// </summary>
		protected internal UnitImpl() : this(null)
		{
		}

		/// <summary>
		/// Constructs with the given ID.
		/// </summary>
		/// <param name="id">
		///        The id of the unit (e.g. "foot"). May be null. </param>
		protected internal UnitImpl(in UnitName id)
		{
			this.id = id;
		}

		/// <summary>
		/// Gets the identifier of this unit.
		/// </summary>
		/// <returns> The ID of this unit. May be <code>null</code>. </returns>
		public UnitName UnitName
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Gets the name of the unit.
		/// </summary>
		/// <returns> The name of the unit. May be <code>null</code>. </returns>
		public string Name
		{
			get
			{
				return id == null ? null : id.Name;
			}
		}

		/// <summary>
		/// Gets the plural form of the name of the unit.
		/// </summary>
		/// <returns> The plural of the name of the unit. May be <code>null</code>. </returns>
		public string Plural
		{
			get
			{
				return id == null ? null : id.Plural;
			}
		}

		/// <summary>
		/// Gets the symbol for the unit.
		/// </summary>
		/// <returns> The symbol of the unit. May be <code>null</code>. </returns>
		public string Symbol
		{
			get
			{
				return id == null ? null : id.Symbol;
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Unit shiftTo(final double origin) throws ShiftException
		public virtual Unit shiftTo(double origin)
		{
			return OffsetUnit.getInstance(this, origin);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Unit shiftTo(final NetcdfSharp.java.util.Date origin) throws ShiftException
		public virtual Unit shiftTo(DateTime origin)
		{
			return TimeScaleUnit.getInstance(this, origin);
		}

		/// <summary>
		/// Multiplies this unit by another.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> The product of this unit multiplied by the other unit. </returns>
		/// <exception cref="MultiplyException">
		///         Can't multiply these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public final Unit multiplyBy(final Unit that) throws MultiplyException
		public Unit multiplyBy(Unit that)
		{
			return myMultiplyBy(that);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Unit multiplyBy(final double scale) throws MultiplyException
		public virtual Unit multiplyBy(double scale)
		{
			return ScaledUnit.getInstance(scale, this);
		}

		/// <summary>
		/// Multiplies this unit by another.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> The product of this unit multiplied by the other unit. </returns>
		/// <exception cref="MultiplyException">
		///         Can't multiply these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected abstract Unit myMultiplyBy(Unit that) throws MultiplyException;
		protected internal abstract Unit myMultiplyBy(Unit that);

		/// <summary>
		/// Divides this unit by another.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> The quotient of this unit divided by the other unit. </returns>
		/// <exception cref="OperationException">
		///         Can't divide these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public final Unit divideBy(final Unit that) throws OperationException
		public Unit divideBy(Unit that)
		{
			return myDivideBy(that);
		}

		/// <summary>
		/// Divides this unit by another.
		/// </summary>
		/// <param name="unit">
		///        The other unit. </param>
		/// <returns> The quotient of this unit divided by the other unit. </returns>
		/// <exception cref="OperationException">
		///         Can't divide these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected abstract Unit myDivideBy(Unit unit) throws OperationException;
		protected internal abstract Unit myDivideBy(Unit unit);

		/// <summary>
		/// Divides this unit into another.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> The quotient of this unit divided into the other unit. </returns>
		/// <exception cref="OperationException">
		///         Can't divide these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public final Unit divideInto(final Unit that) throws OperationException
		public Unit divideInto(Unit that)
		{
			return myDivideInto(that);
		}

		/// <summary>
		/// Divides this unit into another.
		/// </summary>
		/// <param name="unit">
		///        The other unit. </param>
		/// <returns> The quotient of this unit divided into the other unit. </returns>
		/// <exception cref="OperationException">
		///         Can't divide these units. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected abstract Unit myDivideInto(Unit unit) throws OperationException;
		protected internal abstract Unit myDivideInto(Unit unit);

		/// <summary>
		/// Raises this unit to a power.
		/// </summary>
		/// <param name="power">
		///        The power. </param>
		/// <returns> The result of raising this unit to the power. </returns>
		/// <exception cref="RaiseException">
		///         Can't raise this unit to a power. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public final Unit raiseTo(final int power) throws RaiseException
		public Unit raiseTo(int power)
		{
			return myRaiseTo(power);
		}

		/// <summary>
		/// Raises this unit to a power.
		/// </summary>
		/// <param name="power">
		///        The power. </param>
		/// <returns> The result of raising this unit to the power. </returns>
		/// <exception cref="RaiseException">
		///         Can't raise this unit to a power. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected abstract Unit myRaiseTo(int power) throws RaiseException;
		protected internal abstract Unit myRaiseTo(int power);

		/*
		 * Default implementation.
		 * 
		 * @see NetcdfSharp.ucar.units.Unit#log(double)
		 */
		public virtual Unit log(double @base)
		{
			return LogarithmicUnit.getInstance(this, @base);
		}

		/// <summary>
		/// Provides support for converting numeric values from this unit to another
		/// unit.
		/// </summary>
		protected internal class MyConverter : ConverterImpl
		{
			internal readonly DerivableUnit fromUnit;
			internal readonly DerivableUnit toUnit;

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: protected MyConverter(final Unit fromUnit, final Unit toUnit) throws ConversionException
			protected internal MyConverter(in Unit fromUnit, in Unit toUnit) : base(fromUnit, toUnit)
			{
				if (!(fromUnit is DerivableUnit) || !(toUnit is DerivableUnit))
				{
					throw new ConversionException(fromUnit, toUnit);
				}
				this.fromUnit = (DerivableUnit)fromUnit;
				this.toUnit = (DerivableUnit)toUnit;
			}

			public override double convert(double amount)
			{
				double output;
				try
				{
					output = toUnit.fromDerivedUnit(fromUnit.toDerivedUnit(amount));
				}
				// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
				// ORIGINAL LINE: catch (final ConversionException e)
				catch (ConversionException)
				{
					output = 0;
				} // can't happen because isCompatible() vetted
				return output;
			}

			public override float[] convert(float[] input, float[] output)
			{
				try
				{
					toUnit.fromDerivedUnit(fromUnit.toDerivedUnit(input, output), output);
				}
				// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
				// ORIGINAL LINE: catch (final ConversionException e)
				catch (ConversionException)
				{
				} // can't happen because isCompatible() vetted
				return output;
			}

			public override double[] convert(double[] input, double[] output)
			{
				try
				{
					toUnit.fromDerivedUnit(fromUnit.toDerivedUnit(input, output), output);
				}
				// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
				// ORIGINAL LINE: catch (final ConversionException e)
				catch (ConversionException)
				{
				} // can't happen because isCompatible() vetted
				return output;
			}
		}

		/// <summary>
		/// Gets a Converter for converting numeric values from this unit to another,
		/// compatible unit.
		/// </summary>
		/// <param name="outputUnit">
		///        The unit to which to convert the numeric values. </param>
		/// <returns> A converter of values from this unit to the other unit. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Converter getConverterTo(final Unit outputUnit) throws ConversionException
		public virtual IShimConverter getConverterTo(Unit outputUnit)
		{
			return new MyConverter(this, outputUnit);
		}

		/// <summary>
		/// Converts a numeric value from this unit to another unit.
		/// </summary>
		/// <param name="amount">
		///        The numeric value. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numeric value. </param>
		/// <returns> The numeric value in the output unit. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public float convertTo(final float amount, final Unit outputUnit) throws ConversionException
		public virtual float convertTo(float amount, Unit outputUnit)
		{
			return (float)convertTo((double)amount, outputUnit);
		}

		/// <summary>
		/// Converts a numeric value from this unit to another unit.
		/// </summary>
		/// <param name="amount">
		///        The numeric value. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numeric value. </param>
		/// <returns> The numeric value in the output unit. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public double convertTo(final double amount, final Unit outputUnit) throws ConversionException
		public virtual double convertTo(double amount, Unit outputUnit)
		{
			return getConverterTo(outputUnit).convert(amount);
		}

		/// <summary>
		/// Converts numeric values from this unit to another unit.
		/// </summary>
		/// <param name="amounts">
		///        The numeric values. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numeric values. </param>
		/// <returns> The numeric values in the output unit in allocated space. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public float[] convertTo(final float[] amounts, final Unit outputUnit) throws ConversionException
		public virtual float[] convertTo(float[] amounts, Unit outputUnit)
		{
			return convertTo(amounts, outputUnit, new float[amounts.Length]);
		}

		/// <summary>
		/// Converts numeric values from this unit to another unit.
		/// </summary>
		/// <param name="amounts">
		///        The numeric values. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numeric values. </param>
		/// <returns> The numeric values in the output unit in allocated space. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public double[] convertTo(final double[] amounts, final Unit outputUnit) throws ConversionException
		public virtual double[] convertTo(double[] amounts, Unit outputUnit)
		{
			return convertTo(amounts, outputUnit, new double[amounts.Length]);
		}

		/// <summary>
		/// Converts numeric values from this unit to another unit.
		/// </summary>
		/// <param name="input">
		///        The input numeric values. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numeric values. </param>
		/// <param name="output">
		///        The output numeric values. May be the same array as the input
		///        values. </param>
		/// <returns> The numeric values in the output unit. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public float[] convertTo(final float[] input, final Unit outputUnit, final float[] output) throws ConversionException
		public virtual float[] convertTo(float[] input, Unit outputUnit, float[] output)
		{
			return getConverterTo(outputUnit).convert(input, output);
		}

		/// <summary>
		/// Converts numeric values from this unit to another unit.
		/// </summary>
		/// <param name="input">
		///        The input numeric values. </param>
		/// <param name="outputUnit">
		///        The unit to which to convert the numeric values. </param>
		/// <param name="output">
		///        The output numeric values. May be the same array as the input
		///        values. </param>
		/// <returns> The numeric values in the output unit. </returns>
		/// <exception cref="ConversionException">
		///         The units aren't convertible. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public double[] convertTo(final double[] input, final Unit outputUnit, final double[] output) throws ConversionException
		public virtual double[] convertTo(double[] input, Unit outputUnit, double[] output)
		{
			return getConverterTo(outputUnit).convert(input, output);
		}

		/// <summary>
		/// Indicates if numeric values in this unit are convertible with another
		/// unit.
		/// </summary>
		/// <param name="that">
		///        The other unit. </param>
		/// <returns> <code>true</code> if and only if numeric values in this unit are
		///         convertible the other unit. </returns>
		public virtual bool isCompatible(Unit that)
		{
			// jeffm: for some reason just calling getDerivedUnit().equals(...)
			// with jikes 1.1.7 as the compiler causes the jvm to crash.
			// The Unit u1=... does not crash.
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final Unit u1 = getDerivedUnit();
			Unit u1 = DerivedUnit;
			return u1.Equals(that.DerivedUnit);
			// return getDerivedUnit().equals(that.getDerivedUnit());
		}

		/// <summary>
		/// Returns the hash code of this instance.
		/// </summary>
		/// <returns> The hash code of this instance. </returns>
		public override abstract int GetHashCode();

		/// <summary>
		/// Indicates if two string are equal.
		/// </summary>
		/// <param name="s1">
		///        One string. May be <code>null</code>. </param>
		/// <param name="s2">
		///        The other string. May be <code>null</code>. </param>
		/// <returns> <code>true</code> if an only if both strings are
		///         <code>null</code> or both strings are identical. </returns>
		private static bool equals(in string s1, in string s2)
		{
			return (string.ReferenceEquals(s1, null) && string.ReferenceEquals(s2, null)) || (!string.ReferenceEquals(s1, null) && s1.Equals(s2));
		}

		/// <summary>
		/// Indicates if two string are equal (ignoring case).
		/// </summary>
		/// <param name="s1">
		///        One string. May be <code>null</code>. </param>
		/// <param name="s2">
		///        The other string. May be <code>null</code>. </param>
		/// <returns> <code>true</code> if an only if both strings are
		///         <code>null</code> or both strings are identical (ignoring case). </returns>
		private static bool equalsIgnoreCase(in string s1, in string s2)
		{
			return (string.ReferenceEquals(s1, null) && string.ReferenceEquals(s2, null)) || (!string.ReferenceEquals(s1, null) && s1.Equals(s2, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Returns the string representation of this unit.
		/// </summary>
		/// <returns> The string representation of this unit. </returns>
		public override string ToString()
		{
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final String string = getSymbol();
			string @string = Symbol;
			return !string.ReferenceEquals(@string, null) ? @string : Name;
		}

		/// <summary>
		/// Returns a label for a quantity in this unit.
		/// </summary>
		/// <param name="quantityID">
		///        The identifier for the quantity (e.g. "altitude"). </param>
		/// <returns> The appropriate label (e.g. "altitude/m"). </returns>
		public virtual string makeLabel(string quantityID)
		{
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final StringBuilder buf = new StringBuilder(quantityID);
			StringBuilder buf = new StringBuilder(quantityID);
			if (quantityID.Contains(" "))
			{
				buf.Insert(0, '(').Append(')');
			}
			buf.Append('/');
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final int start = buf.length();
			int start = buf.Length;
			buf.Append(ToString());
			if (buf.ToString(start, buf.Length - start).IndexOf(' ') != -1)
			{
				buf.Insert(start, '(').Append(')');
			}
			return buf.ToString();
		}
	}

}
