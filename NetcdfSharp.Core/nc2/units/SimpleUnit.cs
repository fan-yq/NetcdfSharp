// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.units
{
	using NetcdfSharp.ucar.units;

	/// <summary>
	/// Convenience routines on top of ucar.units package.
	/// <p/>
	/// The ucar.units package handles
	/// <ol>
	/// <li>scientific units, which are factors of the fundamental
	/// dimensions such as length, time, mass, etc
	/// <li>dates, represented as "n units of time since reference date" eg
	/// "1203 days since 1970-01-01 00:00:00"
	/// </ol>
	/// 
	/// @author caron
	/// </summary>

	public class SimpleUnit
	{
		public static readonly SimpleUnit kmUnit;
		public static readonly SimpleUnit meterUnit;
		public static readonly SimpleUnit pressureUnit;

		// static protected ShimUnitFormat format;
		protected internal static ShimUnit secsUnit, dateReferenceUnit;
		protected internal static bool debugParse;

		static SimpleUnit()
		{
			try
			{
				ShimUnitFormat udunit = ShimUnitFormatManager.instance();
				secsUnit = udunit.parse("sec");
				dateReferenceUnit = udunit.parse("ms since 1970-01-01");

				kmUnit = SimpleUnit.factoryWithExceptions("km");
				meterUnit = SimpleUnit.factoryWithExceptions("m");
				pressureUnit = SimpleUnit.factoryWithExceptions("Pa");

			}
			catch (Exception e)
			{
				throw new Exception("SimpleUnit initialization failed " + e);
			}
		}

		/// <summary>
		/// Create a SimpleUnit from the given name, catch Exceptions.
		/// </summary>
		/// <param name="name"> parse this name to create a unit. </param>
		/// <returns> SimpleUnit, DateUnit, TimeUnit, or null if failed </returns>
		/// <seealso cref="NetcdfSharp.ucar.units.ShimUnitFormat.parse"/>
		public static SimpleUnit factory(string name)
		{
			try
			{
				return factoryWithExceptions(name);
			}
			catch (Exception e)
			{
				if (debugParse)
				{
					Console.WriteLine("Parse " + name + " got Exception " + e);
				}
				return null;
			}
		}

		/// <summary>
		/// Create a SimpleUnit from the given name, allow Exceptions.
		/// </summary>
		/// <param name="name"> parse this name to create a unit. </param>
		/// <returns> SimpleUnit, DateUnit, or TimeUnit </returns>
		/// <exception cref="UnitException"> when date parser fails </exception>
		/// <seealso cref="NetcdfSharp.ucar.units.ShimUnitFormat.parse"/>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static SimpleUnit factoryWithExceptions(String name) throws UnitException
		public static SimpleUnit factoryWithExceptions(string name)
		{
			ShimUnitFormat format = ShimUnitFormatManager.instance();
			ShimUnit uu = format.parse(name);
			// if (isDateUnit(uu)) return new DateUnit(name);
			if (isTimeUnit(uu))
			{
				return new TimeUnit(name);
			}
			return new SimpleUnit(uu);
		}

		// need subclass access
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected static Unit makeUnit(String name) throws UnitException
		protected internal static ShimUnit makeUnit(string name)
		{
			ShimUnitFormat format = ShimUnitFormatManager.instance();
			return format.parse(name);
		}

		/// <summary>
		/// Return true if unitString1 is compatible to unitString2, meaning one can be converted to the other.
		/// If either unit string is illegal, return false.
		/// </summary>
		/// <param name="unitString1"> compare this unit </param>
		/// <param name="unitString2"> compare this unit </param>
		/// <returns> true if the 2 units are compatible </returns>
		public static bool isCompatible(string unitString1, string unitString2)
		{
			ShimUnit uu1, uu2;
			try
			{
				ShimUnitFormat format = ShimUnitFormatManager.instance();
				uu1 = format.parse(unitString1);
			}
			catch (Exception e)
			{
				if (debugParse)
				{
					Console.WriteLine("Parse " + unitString1 + " got Exception1 " + e);
				}
				return false;
			}

			try
			{
				ShimUnitFormat format = ShimUnitFormatManager.instance();
				uu2 = format.parse(unitString2);
			}
			catch (Exception e)
			{
				if (debugParse)
				{
					Console.WriteLine("Parse " + unitString2 + " got Exception2 " + e);
				}
				return false;
			}

			return uu1.isCompatible(uu2);
		}

		/// <summary>
		/// Return true if unitString1 is convertible to unitString2
		/// </summary>
		/// <param name="unitString1"> compare this unit </param>
		/// <param name="unitString2"> compare this unit </param>
		/// <returns> true if the 2 units are compatible </returns>
		/// <exception cref="UnitException"> if units parsing fails </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static boolean isCompatibleWithExceptions(String unitString1, String unitString2) throws UnitException
		public static bool isCompatibleWithExceptions(string unitString1, string unitString2)
		{
			ShimUnitFormat format = ShimUnitFormatManager.instance();
			ShimUnit uu1 = format.parse(unitString1);
			ShimUnit uu2 = format.parse(unitString2);
			return uu1.isCompatible(uu2);
		}

		/// <summary>
		/// Return true if this NetcdfSharp.ucar.units.Unit is a Date.
		/// </summary>
		/// <param name="uu"> check this Unit </param>
		/// <returns> true if its a Date </returns>
		public static bool isDateUnit(NetcdfSharp.ucar.units.ShimUnit uu)
		{
			bool ok = uu.isCompatible(dateReferenceUnit);
			if (!ok)
			{
				return false;
			}
			try
			{
				uu.getConverterTo(dateReferenceUnit);
				return true;
			}
			catch (ShimConversionException)
			{
				return false;
			}
		}

		/// <summary>
		/// Return true if this NetcdfSharp.ucar.units.Unit is convertible to secs.
		/// </summary>
		/// <param name="uu"> check this Unit </param>
		/// <returns> true if its a Time </returns>
		public static bool isTimeUnit(NetcdfSharp.ucar.units.ShimUnit uu)
		{
			return uu.isCompatible(secsUnit);
		}

		/// <summary>
		/// Return true if the given unit is convertible to a date Unit.
		/// allowed format is something like:
		/// 
		/// <pre>
		/// [-]Y[Y[Y[Y]]]-MM-DD[(T| )hh[:mm[:ss[.sss*]]][ [+|-]hh[[:]mm]]]
		/// </pre>
		/// </summary>
		/// <param name="unitString"> check this unit string </param>
		/// <returns> true if its a Date </returns>
		public static bool isDateUnit(string unitString)
		{
			// A date unit has the form "units since <date>". The shim's factory parses the leading
			// time unit; check the "since" marker and that the unit part is time-compatible.
			if (string.ReferenceEquals(unitString, null) || unitString.IndexOf(" since ", StringComparison.Ordinal) < 0)
			{
				return false;
			}
			try
			{
				ShimUnit uu = makeUnit(unitString);
				return isDateUnit(uu);
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Return true if the given unit is a time Unit, eg "seconds".
		/// </summary>
		/// <param name="unitString"> check this unit string </param>
		/// <returns> true if its a Time </returns>
		public static bool isTimeUnit(string unitString)
		{
			SimpleUnit su = factory(unitString);
			return su != null && isTimeUnit(su.Unit);
		}

		/// <summary>
		/// Get the conversion factor to convert inputUnit to outputUnit.
		/// </summary>
		/// <param name="inputUnitString"> inputUnit in string form </param>
		/// <param name="outputUnitString"> outputUnit in string form </param>
		/// <returns> conversion factor </returns>
		/// <exception cref="IllegalArgumentException"> if not convertible </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static double getConversionFactor(String inputUnitString, String outputUnitString) throws IllegalArgumentException
		public static double getConversionFactor(string inputUnitString, string outputUnitString)
		{
			SimpleUnit inputUnit = SimpleUnit.factory(inputUnitString);
			SimpleUnit outputUnit = SimpleUnit.factory(outputUnitString);
			return inputUnit.convertTo(1.0, outputUnit);
		}

		////////////////////////////////////////////////
		protected internal NetcdfSharp.ucar.units.ShimUnit uu;

		/// <summary>
		/// for subclasses.
		/// </summary>
		protected internal SimpleUnit()
		{
		}

		/// <summary>
		/// Wrap a NetcdfSharp.ucar.units.Unit in a SimpleUnit. Use factory().
		/// </summary>
		/// <param name="uu"> wrap this Unit </param>
		internal SimpleUnit(NetcdfSharp.ucar.units.ShimUnit uu)
		{
			this.uu = uu;
		}

		/// <summary>
		/// Unit string representation.
		/// </summary>
		public override string ToString()
		{
			return uu.ToString();
		}

		/// <summary>
		/// Get underlying NetcdfSharp.ucar.units.Unit.
		/// </summary>
		/// <returns> underlying NetcdfSharp.ucar.units.Unit. </returns>
		public virtual ShimUnit Unit
		{
			get
			{
				return uu;
			}
		}

		/// <summary>
		/// Convert given value of this unit to the new unit.
		/// </summary>
		/// <param name="value"> value in this unit </param>
		/// <param name="outputUnit"> convert to this unit </param>
		/// <returns> value in outputUnit </returns>
		/// <exception cref="IllegalArgumentException"> if outputUnit not convertible from this unit </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public double convertTo(double value, SimpleUnit outputUnit) throws IllegalArgumentException
		public virtual double convertTo(double value, SimpleUnit outputUnit)
		{
			try
			{
				return uu.convertTo(value, outputUnit.Unit);
			}
			catch (ShimConversionException e)
			{
				throw new ArgumentException(e.Message);
			}
		}

		/// <summary>
		/// Return true if unitString1 is compatible to unitString2,
		/// meaning one can be converted to the other.
		/// If either unit string is illegal, return false.
		/// </summary>
		/// <param name="unitString"> check if this is compatible with unitString </param>
		/// <returns> true if compatible </returns>
		public virtual bool isCompatible(string unitString)
		{
			ShimUnit uuWant;
			try
			{
				ShimUnitFormat format = ShimUnitFormatManager.instance();
				uuWant = format.parse(unitString);
			}
			catch (Exception e)
			{
				if (debugParse)
				{
					Console.WriteLine("Parse " + unitString + " got Exception1 " + e);
				}
				return false;
			}

			return uu.isCompatible(uuWant);
		}


		/// <summary>
		/// Is this an instance of an UnknownUnit?
		/// </summary>
		/// <returns> true if an instance of an UnknownUnit </returns>
		public virtual bool UnknownUnit
		{
			get
			{
				NetcdfSharp.ucar.units.ShimUnit uu = Unit;
				if (uu is NetcdfSharp.ucar.units.ShimUnknownUnit)
				{
					return true;
				}
				if (uu is NetcdfSharp.ucar.units.ShimDerivedUnit)
				{
					return isUnknownUnit((NetcdfSharp.ucar.units.ShimDerivedUnit)uu);
				}
				if (uu is NetcdfSharp.ucar.units.ShimScaledUnit)
				{
					NetcdfSharp.ucar.units.ShimScaledUnit scu = (NetcdfSharp.ucar.units.ShimScaledUnit)uu;
					ShimUnit u = scu.Unit;
					if (u is NetcdfSharp.ucar.units.ShimUnknownUnit)
					{
						return true;
					}

					if (u is NetcdfSharp.ucar.units.ShimDerivedUnit)
					{
						return isUnknownUnit((NetcdfSharp.ucar.units.ShimDerivedUnit)u);
					}
				}
				return false;
			}
		}

		private bool isUnknownUnit(NetcdfSharp.ucar.units.ShimDerivedUnit du)
		{
			ShimUnitDimension dim = du.Dimension;
			foreach (ShimFactor f in dim.Factors)
			{
				if (f.Base is NetcdfSharp.ucar.units.ShimUnknownUnit)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Extract the value, can only be called for ShimScaledUnit.
		/// </summary>
		/// <returns> value of this unit if ShimScaledUnit, else NaN </returns>
		public virtual double Value
		{
			get
			{
				if (!(uu is ShimScaledUnit))
				{
					return double.NaN;
				}
				ShimScaledUnit offset = (ShimScaledUnit)uu;
				return offset.Scale;
			}
		}

		/// <summary>
		/// Extract the simple unit string (no number), eg "s" or "m".
		/// </summary>
		/// <returns> unit string with no value </returns>
		public virtual string UnitString
		{
			get
			{
				if (uu is NetcdfSharp.ucar.units.ShimScaledUnit scu)
				{
					return scu.DerivedSymbol;
				}
				return uu.ShimDerivedUnit.ToString();
			}
		}

		public virtual string CanonicalString
		{
			get
			{
				return uu.CanonicalString;
			}
		}

		public virtual string ImplementingClass
		{
			get
			{
				// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return uu.GetType().FullName;
			}
		}

	}

}
