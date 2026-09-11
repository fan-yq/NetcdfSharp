// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{

	/// <summary>
	/// Provides support for a reference time unit whose origin is at a certain time.
	/// <p/>
	/// Instances of this class are immutable.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @Immutable public final class TimeScaleUnit extends UnitImpl
	[Serializable]
	public sealed class TimeScaleUnit : UnitImpl
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// The second unit.
		/// </summary>
		internal static readonly BaseUnit SECOND;

		static TimeScaleUnit()
		{
			try
			{
				SECOND = BaseUnit.getOrCreate(UnitName.newUnitName("second", null, "s"), BaseQuantity.TIME);
			}
			// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
			// ORIGINAL LINE: catch (final Exception e)
			catch (Exception e)
			{
				throw new System.InvalidOperationException("Exception in initializer", e);
			}
		}

		///////////////////////////////////////////////////////


		/// <summary>
		/// The reference time unit.
		/// 
		/// @serial
		/// </summary>
		private readonly Unit _unit;

		/// <summary>
		/// The time origin for this instance.
		/// 
		/// @serial
		/// </summary>
		private readonly DateTime _origin;


		/// <summary>
		/// Constructs from a reference unit and a time origin.
		/// </summary>
		/// <param name="unit"> The reference time unit. </param>
		/// <param name="origin"> The time origin. </param>
		/// <exception cref="BadUnitException"> <code>unit</code> is not a unit of time. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public TimeScaleUnit(final Unit unit, final NetcdfSharp.java.util.Date origin) throws BadUnitException, UnitSystemException
		public TimeScaleUnit(in Unit unit, in DateTime origin) : this(unit, origin, null)
		{
		}

		/// <summary>
		/// Constructs from a reference unit, a time origin, and an identifier.
		/// </summary>
		/// <param name="unit"> The reference time unit. </param>
		/// <param name="origin"> The time origin. </param>
		/// <param name="id"> The identifier. </param>
		/// <exception cref="BadUnitException"> <code>unit</code> is not a unit of time. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public TimeScaleUnit(final Unit unit, final NetcdfSharp.java.util.Date origin, final UnitName id) throws BadUnitException, UnitSystemException
		public TimeScaleUnit(in Unit unit, in DateTime origin, in UnitName id) : base(id)
		{
			if (!unit.isCompatible(UnitSystemManager.instance().getBaseUnit(BaseQuantity.TIME)))
			{
				throw new BadUnitException("\"" + unit + "\" is not a unit of time");
			}
			_unit = unit;
			_origin = origin;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: static Unit getInstance(final Unit unit, final NetcdfSharp.java.util.Date origin) throws ShiftException
		internal static Unit getInstance(in Unit unit, in DateTime origin)
		{
			try
			{
				return unit is TimeScaleUnit ? new TimeScaleUnit(((TimeScaleUnit)unit)._unit, origin) : new TimeScaleUnit(unit, origin);
			}
			// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
			// ORIGINAL LINE: catch (final Exception e)
			catch (Exception e)
			{
				throw new ShiftException(unit, origin);
			}
		}

		/// <summary>
		/// Returns the reference unit.
		/// </summary>
		/// <returns> The reference unit. </returns>
		public Unit Unit
		{
			get
			{
				return _unit;
			}
		}

		/// <summary>
		/// Returns the time origin.
		/// </summary>
		/// <returns> The time origin. </returns>
		public DateTime Origin
		{
			get
			{
				return _origin;
			}
		}

		/*
		 * From UnitImpl:
		 */

		/// <summary>
		/// Clones this unit, changing the identifier.
		/// </summary>
		/// <param name="id"> The new identifier. </param>
		/// <returns> This unit with the new identifier. </returns>
		public override Unit clone(UnitName id)
		{
			Unit clone;
			try
			{
				clone = new TimeScaleUnit(Unit, Origin, id);
			}
			// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
			// ORIGINAL LINE: catch (final UnitException e)
			catch (UnitException)
			{
				clone = null; // can't happen
			}
			return clone;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public Unit shiftTo(final double origin) throws ShiftException
		public override Unit shiftTo(double origin)
		{
			DateTime newOrigin;
			try
			{
				newOrigin = new DateTime(_origin.Ticks + (long)(_unit.convertTo(origin, SECOND) * 1000));
			}
			// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
			// ORIGINAL LINE: catch (final ConversionException e)
			catch (ConversionException e)
			{
				throw new ShiftException(this, origin);
			}
			try
			{
				return new TimeScaleUnit(_unit, newOrigin);
			}
			// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
			// ORIGINAL LINE: catch (final BadUnitException e)
			catch (BadUnitException)
			{
				throw new System.InvalidOperationException("assertion error");
			}
			// JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
			// ORIGINAL LINE: catch (final UnitSystemException e)
			catch (UnitSystemException e)
			{
				throw new ShiftException(this, origin);
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public Unit shiftTo(final NetcdfSharp.java.util.Date origin) throws ShiftException
		public override Unit shiftTo(DateTime origin)
		{
			return getInstance(_unit, origin);
		}

		/// <summary>
		/// Multiplies this unit by another unit. This operation is invalid.
		/// </summary>
		/// <param name="that"> The other unit. </param>
		/// <returns> The product of multiplying this unit by the other unit. </returns>
		/// <exception cref="MultiplyException"> Illegal operation. Always thrown. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override protected Unit myMultiplyBy(final Unit that) throws MultiplyException
		protected internal override Unit myMultiplyBy(Unit that)
		{
			throw new MultiplyException(this);
		}

		/// <summary>
		/// Divides this unit by another unit. This operation is invalid.
		/// </summary>
		/// <param name="that"> The other unit. </param>
		/// <returns> The quotient of dividing this unit by the other unit. </returns>
		/// <exception cref="DivideException"> Illegal operation. Always thrown. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override protected Unit myDivideBy(final Unit that) throws DivideException
		protected internal override Unit myDivideBy(Unit that)
		{
			throw new DivideException(this);
		}

		/// <summary>
		/// Divides this unit into another unit. This operation is invalid.
		/// </summary>
		/// <param name="that"> The other unit. </param>
		/// <returns> The quotient of dividing this unit into the other unit. </returns>
		/// <exception cref="DivideException"> Illegal operation. Always thrown. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override protected Unit myDivideInto(final Unit that) throws DivideException
		protected internal override Unit myDivideInto(Unit that)
		{
			throw new DivideException(that, this);
		}

		/// <summary>
		/// Raises this unit to a power. This operation is invalid.
		/// </summary>
		/// <param name="power"> The power. </param>
		/// <returns> The result of raising this unit to the power. </returns>
		/// <exception cref="RaiseException"> Illegal operation. Always thrown. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override protected Unit myRaiseTo(final int power) throws RaiseException
		protected internal override Unit myRaiseTo(int power)
		{
			throw new RaiseException(this);
		}

		/// <summary>
		/// Returns the derived unit underlying the reference time unit.
		/// </summary>
		/// <returns> The derived unit underlying the reference time unit. </returns>
		public override DerivedUnit DerivedUnit
		{
			get
			{
				return Unit.DerivedUnit;
			}
		}

		/// <summary>
		/// Provides support for Converter-s.
		/// </summary>
		protected internal sealed class MyConverter : ConverterImpl
		{
			internal readonly double offset;
			internal readonly IShimConverter converter;

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: protected MyConverter(final TimeScaleUnit fromUnit, final Unit toUnit) throws ConversionException
			protected internal MyConverter(in TimeScaleUnit fromUnit, in Unit toUnit) : base(fromUnit, toUnit)
			{
				converter = fromUnit.Unit.getConverterTo(((TimeScaleUnit)toUnit).Unit);
				offset = SI.SECOND.convertTo((fromUnit.Origin.Ticks - ((TimeScaleUnit)toUnit).Origin.Ticks) / 1000.0, ((TimeScaleUnit)toUnit).Unit);
			}

			public override double convert(double amount)
			{
				return converter.convert(amount) + offset;
			}

			public override float[] convert(float[] input, float[] output)
			{
				output = converter.convert(input, output);
				for (int i = input.Length; --i >= 0;)
				{
					output[i] += (float)offset;
				}
				return output;
			}

			public override double[] convert(double[] input, double[] output)
			{
				output = converter.convert(input, output);
				for (int i = input.Length; --i >= 0;)
				{
					output[i] += offset;
				}
				return output;
			}
		}

		/// <summary>
		/// Returns a Converter for converting numeric values from this unit to
		/// another unit.
		/// </summary>
		/// <param name="outputUnit"> The other unit. Shall be a TimeScaleUnit. </param>
		/// <returns> A Converter. </returns>
		/// <exception cref="ConversionException"> <code>outputUnit</code> is not a TimeScaleUnit. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public Converter getConverterTo(final Unit outputUnit) throws ConversionException
		public override IShimConverter getConverterTo(Unit outputUnit)
		{
			return new MyConverter(this, outputUnit);
		}

		/// <summary>
		/// Indicates if numeric values in this unit are convertible to another unit.
		/// </summary>
		/// <param name="that"> The other unit. </param>
		/// <returns> <code>true</code> if and only if numeric values in this unit are
		///         convertible to <code>
		/// that</code>. </returns>
		public override bool isCompatible(Unit that)
		{
			return that is TimeScaleUnit;
		}

		/// <summary>
		/// Indicates if this unit is semantically identical to an object.
		/// </summary>
		/// <param name="object"> The object. </param>
		/// <returns> <code>true</code> if and only if this unit is semantically
		///         identical to <code>object
		/// </code>. </returns>
		public override bool Equals(object @object)
		{
			if (this == @object)
			{
				return true;
			}
			if (!(@object is TimeScaleUnit))
			{
				return false;
			}
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final TimeScaleUnit that = (TimeScaleUnit) object;
			TimeScaleUnit that = (TimeScaleUnit)@object;
			return _origin.Equals(that._origin) && _unit.Equals(that._unit);
		}

		/// <summary>
		/// Returns the hash code of this instance.
		/// </summary>
		/// <returns> The hash code of this instance. </returns>
		public override int GetHashCode()
		{
			return Unit.GetHashCode() ^ Origin.GetHashCode();
		}

		/// <summary>
		/// Indicates if this unit is dimensionless. TimeScaleUnit-s are never
		/// dimensionless.
		/// </summary>
		/// <returns> <code>false</code>. </returns>
		public override bool Dimensionless
		{
			get
			{
				return false; // a TimeScaleUnit is never dimensionless by definition
			}
		}

		/// <summary>
		/// Returns the string representation of this unit.
		/// </summary>
		/// <returns> The string representation of this unit. </returns>
		public override string ToString()
		{
			// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			// ORIGINAL LINE: final String string = super.toString();
			string @string = base.ToString(); // get symbol or name
			return !string.ReferenceEquals(@string, null) ? @string : CanonicalString;
		}

		/// <summary>
		/// Returns the canonical string representation of the unit.
		/// </summary>
		/// <returns> The canonical string representation. </returns>
		public override string CanonicalString
		{
			get
			{
				/*
				 * change this, not thread-safe; require dependency of joda-time (!)
				 * dateFormat = (SimpleDateFormat) DateFormat.getDateInstance(DateFormat.SHORT, Locale.US);
				 * dateFormat.setTimeZone(TimeZone.getTimeZone("UTC"));
				 * dateFormat.applyPattern(" 'since' yyyy-MM-dd HH:mm:ss.SSS 'UTC'");
				 */
				return Unit.ToString() + " since " + Origin.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.SSS 'UTC'", System.Globalization.CultureInfo.InvariantCulture);
			}
		}

	}

}
