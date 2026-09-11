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
	/// Provides support for the International System of Units (SI). A good on-line
	/// reference for the SI can be found at <http://physics.nist.gov/cuu/Units/>.
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class SI : UnitSystemImpl
	{
	  private const long serialVersionUID = 1L;

	  /*
	   * The singleton instance.
	   *
	   * @serial
	   */
	  private static SI si;

	  /*
	   * Units corresponding to base quantities:
	   */

	  /// <summary>
	  /// The unit corresponding to the quantity of amount of substance.
	  /// </summary>
	  public static readonly Unit AMOUNT_OF_SUBSTANCE_UNIT;

	  /// <summary>
	  /// The unit corresponding to the quantity of electric current.
	  /// </summary>
	  public static readonly Unit ELECTRIC_CURRENT_UNIT;

	  /// <summary>
	  /// The unit corresponding to the quantity of length.
	  /// </summary>
	  public static readonly Unit LENGTH_UNIT;

	  /// <summary>
	  /// The unit corresponding to the quantity of luminous intensity.
	  /// </summary>
	  public static readonly Unit LUMINOUS_INTENSITY_UNIT;

	  /// <summary>
	  /// The unit corresponding to the quantity of mass.
	  /// </summary>
	  public static readonly Unit MASS_UNIT;

	  /// <summary>
	  /// The unit corresponding to the quantity of plane angle.
	  /// </summary>
	  public static readonly Unit PLANE_ANGLE_UNIT;

	  /// <summary>
	  /// The unit corresponding to the quantity of solid angle.
	  /// </summary>
	  public static readonly Unit SOLID_ANGLE_UNIT;

	  /// <summary>
	  /// The unit corresponding to the quantity of thermodynamic temperature.
	  /// </summary>
	  public static readonly Unit THERMODYNAMIC_TEMPERATURE_UNIT;

	  /// <summary>
	  /// The unit corresponding to the quantity of time.
	  /// </summary>
	  public static readonly Unit TIME_UNIT;

	  /*
	   * Base units:
	   */

	  /// <summary>
	  /// Base unit of electric current. The ampere is that constant current which,
	  /// if maintained in two straight parallel conductors of infinite length, of
	  /// negligible circular cross section, and placed 1 meter apart in vacuum,
	  /// would produce between these conductors a force equal to 2 x 10^-7 newton
	  /// per meter of length.
	  /// </summary>
	  public static readonly BaseUnit AMPERE;

	  /// <summary>
	  /// Base unit of luminous intensity. The candela is the luminous intensity,
	  /// in a given direction, of a source that emits monochromatic radiation of
	  /// frequency 540 x 10^12 hertz and that has a radiant intensity in that
	  /// direction of (1/683) watt per steradian.
	  /// </summary>
	  public static readonly BaseUnit CANDELA;

	  /// <summary>
	  /// Base unit of thermodynamic temperature. The kelvin, unit of thermodynamic
	  /// temperature, is the fraction 1/273.16 of the thermodynamic temperature of
	  /// the triple point of water.
	  /// </summary>
	  public static readonly BaseUnit KELVIN;

	  /// <summary>
	  /// Base unit of mass. The kilogram is the unit of mass; it is equal to the
	  /// mass of the international prototype of the kilogram.
	  /// </summary>
	  public static readonly BaseUnit KILOGRAM;

	  /// <summary>
	  /// Base unit of length. The meter is the length of the path travelled by
	  /// light in vacuum during a time interval of 1/299 792 458 of a second.
	  /// </summary>
	  public static readonly BaseUnit METER; // NIST

	  /// <summary>
	  /// Synonym for <code>METER</code>.
	  /// </summary>
	  public static readonly BaseUnit METRE; // ISO

	  /// <summary>
	  /// Base unit of amount of substance. The mole is the amount of substance of
	  /// a system which contains as many elementary entities as there are atoms in
	  /// 0.012 kilogram of carbon 12.
	  /// </summary>
	  public static readonly BaseUnit MOLE;

	  /// <summary>
	  /// Base unit of time. The second is the duration of 9 192 631 770 periods of
	  /// the radiation corresponding to the trasition between the two hyperfine
	  /// levels of the ground state of the cesium-133 atom.
	  /// </summary>
	  public static readonly BaseUnit SECOND;

	  /*
	   * Supplementary units: These units are more accurately referred to as
	   * "dimensionless derived units" -- for the purposes of this Java package,
	   * however, they are BaseUnit-s. Note, however, that their <code>
	   * isDimensionless()</code> method will return <code>true</code>.
	   */

	  /// <summary>
	  /// Base unit of angular measure. The radian is the plane angle between two
	  /// radii of a circle that cut off on the circumference an arc equal in
	  /// length to the radius.
	  /// </summary>
	  public static readonly BaseUnit RADIAN;

	  /// <summary>
	  /// Base unit of solid angle. The steradian is the solid angle that, having
	  /// its vertex in the center of a sphere, cuts off an area on the surface of
	  /// the sphere equal to that of a square with sides of length equal to the
	  /// radius of the sphere.
	  /// </summary>
	  public static readonly BaseUnit STERADIAN;

	  /*
	   * Derived units with special names and symbols:
	   */

	  /// <summary>
	  /// Special derived unit for the quantity of frequency.
	  /// </summary>
	  public static readonly Unit HERTZ;

	  /// <summary>
	  /// Special derived unit for the quantity of force.
	  /// </summary>
	  public static readonly Unit NEWTON;

	  /// <summary>
	  /// Special derived unit for the quantity of pressure or stress.
	  /// </summary>
	  public static readonly Unit PASCAL;

	  /// <summary>
	  /// Special derived unit for the quantity of energy, work, or quantity of
	  /// heat.
	  /// </summary>
	  public static readonly Unit JOULE;

	  /// <summary>
	  /// Special derived unit for the quantity of power or radiant flux.
	  /// </summary>
	  public static readonly Unit WATT;

	  /// <summary>
	  /// Special derived unit for the quantity of electric charge or quantity of
	  /// electricity.
	  /// </summary>
	  public static readonly Unit COULOMB;

	  /// <summary>
	  /// Special derived unit for the quantity of electric potential, potential
	  /// difference, or electromotive force.
	  /// </summary>
	  public static readonly Unit VOLT;

	  /// <summary>
	  /// Special derived unit for the quantity of electric capacitance.
	  /// </summary>
	  public static readonly Unit FARAD;

	  /// <summary>
	  /// Special derived unit for the quantity of electric resistance.
	  /// </summary>
	  public static readonly Unit OHM;

	  /// <summary>
	  /// Special derived unit for the quantity of electric conductance.
	  /// </summary>
	  public static readonly Unit SIEMENS;

	  /// <summary>
	  /// Special derived unit for the quantity of magnetic flux.
	  /// </summary>
	  public static readonly Unit WEBER;

	  /// <summary>
	  /// Special derived unit for the quantity of magnetic flux density.
	  /// </summary>
	  public static readonly Unit TESLA;

	  /// <summary>
	  /// Special derived unit for the quantity of inductance.
	  /// </summary>
	  public static readonly Unit HENRY;

	  /// <summary>
	  /// Special derived unit for the quantity of Celsius temperature.
	  /// </summary>
	  public static readonly Unit DEGREE_CELSIUS;

	  /// <summary>
	  /// Special derived unit for the quantity of luminous flux.
	  /// </summary>
	  public static readonly Unit LUMEN;

	  /// <summary>
	  /// Special derived unit for the quantity of illuminance.
	  /// </summary>
	  public static readonly Unit LUX;

	  /*
	   * Derived units with special names and symbols admitted for reasons of
	   * safegarding human health.
	   */

	  /// <summary>
	  /// Special derived unit for the quantity of activity of a radionuclide.
	  /// </summary>
	  public static readonly Unit BECQUEREL;

	  /// <summary>
	  /// Special derived unit for the quantity of absorbed dose, specific energy
	  /// (imparted), or kerms.
	  /// </summary>
	  public static readonly Unit GRAY;

	  /// <summary>
	  /// Special derived unit for the quantity of dose equivalent, ambient dose
	  /// equivalent, directional dose equivalent, personal dose equivalent, or
	  /// equivalent dose.
	  /// </summary>
	  public static readonly Unit SIEVERT;

	  /*
	   * Units outside the SI but accepted for use with the SI according to the
	   * USA National Institute of Standards and Technology (see
	   * <http://physics.nist.gov/cuu/Units/>):
	   */

	  /// <summary>
	  /// Special derived unit -- accepted for use with the SI -- for the quantity
	  /// of time.
	  /// </summary>
	  public static readonly Unit MINUTE;

	  /// <summary>
	  /// Special derived unit -- accepted for use with the SI -- for the quantity
	  /// of time.
	  /// </summary>
	  public static readonly Unit HOUR;

	  /// <summary>
	  /// Special derived unit -- accepted for use with the SI -- for the quantity
	  /// of time.
	  /// </summary>
	  public static readonly Unit DAY;

	  /// <summary>
	  /// Special derived unit -- accepted for use with the SI -- for the quantity
	  /// of plane angle.
	  /// </summary>
	  public static readonly Unit ARC_DEGREE;

	  /// <summary>
	  /// Special derived unit -- accepted for use with the SI -- for the quantity
	  /// of plane angle.
	  /// </summary>
	  public static readonly Unit ARC_MINUTE;

	  /// <summary>
	  /// Special derived unit -- accepted for use with the SI -- for the quantity
	  /// of plane angle.
	  /// </summary>
	  public static readonly Unit ARC_SECOND;

	  /// <summary>
	  /// Special derived unit -- accepted for use with the SI -- for the quantity
	  /// of volume.
	  /// </summary>
	  public static readonly Unit LITER; // NIST

	  /// <summary>
	  /// Synomym for <code>LITER</code>.
	  /// </summary>
	  public static readonly Unit LITRE; // ISO

	  /// <summary>
	  /// Special derived unit -- accepted for use with the SI -- for the quantity
	  /// of mass.
	  /// </summary>
	  public static readonly Unit METRIC_TON; // NIST

	  /// <summary>
	  /// Synomym for <code>METRIC_TON</code>.
	  /// </summary>
	  public static readonly Unit TONNE; // ISO

	  /*
	   * Units outside the SI but temporarily accepted for use with the SI
	   * according to the USA National Institute of Standards and Technology (see
	   * <http://physics.nist.gov/cuu/Units/>):
	   */

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI -- for
	  /// the quantity of length.
	  /// </summary>
	  public static readonly Unit NAUTICAL_MILE;

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI -- for
	  /// the quantity of speed.
	  /// </summary>
	  public static readonly Unit KNOT;

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI -- for
	  /// the quantity of length.
	  /// </summary>
	  public static readonly Unit ANGSTROM;

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI -- for
	  /// the quantity of area.
	  /// </summary>
	  public static readonly Unit ARE;

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI -- for
	  /// the quantity of area.
	  /// </summary>
	  public static readonly Unit HECTARE;

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI -- for
	  /// the quantity of area.
	  /// </summary>
	  public static readonly Unit BARN;

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI -- for
	  /// the quantity of pressure.
	  /// </summary>
	  public static readonly Unit BAR;

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI -- for
	  /// the quantity of acceleration.
	  /// </summary>
	  public static readonly Unit GAL;

	  /*
	   * Units outside the SI but temporarily accepted for use with the SI until
	   * the year 2000 according to the USA National Institute of Standards and
	   * Technology (see <http://physics.nist.gov/cuu/Units/>):
	   */

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI until
	  /// the year 2000 according to the USA National Institute of Standards and
	  /// Technology -- for the quantity of activity of a radionuclide.
	  /// </summary>
	  public static readonly Unit CURIE;

	  /// <summary>
	  /// Special derived unit -- temporarily accepted for use with the SI until
	  /// the year 2000 according to the USA National Institute of Standards and
	  /// Technology -- for the quantity of electric charge per mass.
	  /// </summary>
	  public static readonly Unit ROENTGEN;

	  /// <summary>
	  /// Special derived unit -- temporarilty accepted for use with the SI until
	  /// the year 2000 according to the USA National Institute of Standards and
	  /// Technology -- for the quantity of absorbed dose.
	  /// </summary>
	  public static readonly Unit RAD;

	  /// <summary>
	  /// Special derived unit -- temporarilty accepted for use with the SI until
	  /// the year 2000 according to the USA National Institute of Standards and
	  /// Technology -- for the quantity of equivalent absorbed dose.
	  /// </summary>
	  public static readonly Unit REM;

	  static SI()
	  {
		BaseUnit ampere = null;
		BaseUnit candela = null;
		BaseUnit kelvin = null;
		BaseUnit kilogram = null;
		BaseUnit meter = null;
		BaseUnit mole = null;
		BaseUnit second = null;
		BaseUnit radian = null;
		BaseUnit steradian = null;
		Unit hertz = null;
		Unit newton = null;
		Unit pascal = null;
		Unit joule = null;
		Unit watt = null;
		Unit coulomb = null;
		Unit volt = null;
		Unit farad = null;
		Unit ohm = null;
		Unit siemens = null;
		Unit weber = null;
		Unit tesla = null;
		Unit henry = null;
		Unit degree_celsius = null;
		Unit lumen = null;
		Unit lux = null;
		Unit becquerel = null;
		Unit gray = null;
		Unit sievert = null;
		Unit minute = null;
		Unit hour = null;
		Unit day = null;
		Unit arc_degree = null;
		Unit arc_minute = null;
		Unit arc_second = null;
		Unit liter = null;
		Unit metric_ton = null;
		Unit nautical_mile = null;
		Unit knot = null;
		Unit angstrom = null;
		Unit are = null;
		Unit hectare = null;
		Unit barn = null;
		Unit bar = null;
		Unit gal = null;
		Unit curie = null;
		Unit roentgen = null;
		Unit rad = null;
		Unit rem = null;

		try
		{
		  ampere = bu("ampere", "A", BaseQuantity.ELECTRIC_CURRENT);
		  candela = bu("candela", "cd", BaseQuantity.LUMINOUS_INTENSITY);
		  kelvin = bu("kelvin", "K", BaseQuantity.THERMODYNAMIC_TEMPERATURE);
		  kilogram = bu("kilogram", "kg", BaseQuantity.MASS);
		  meter = bu("meter", "m", BaseQuantity.LENGTH);
		  mole = bu("mole", "mol", BaseQuantity.AMOUNT_OF_SUBSTANCE);
		  second = bu("second", "s", BaseQuantity.TIME);
		  radian = bu("radian", "rad", BaseQuantity.PLANE_ANGLE);
		  steradian = bu("steradian", "sr", BaseQuantity.SOLID_ANGLE);

		  hertz = du("hertz", "Hz", second.raiseTo(-1));
		  newton = du("newton", "N", kilogram.multiplyBy(meter).divideBy(second.raiseTo(2)));
		  pascal = du("pascal", "Pa", newton.divideBy(meter.raiseTo(2)));
		  joule = du("joule", "J", newton.multiplyBy(meter));
		  watt = du("watt", "W", joule.divideBy(second));
		  coulomb = du("coulomb", "C", ampere.multiplyBy(second));
		  volt = du("volt", "V", watt.divideBy(ampere));
		  farad = du("farad", "F", coulomb.divideBy(volt));
		  ohm = du("ohm", "Ohm", volt.divideBy(ampere));
		  siemens = du("siemens", "S", ohm.raiseTo(-1));
		  weber = du("weber", "Wb", volt.multiplyBy(second));
		  tesla = du("tesla", "T", weber.divideBy(meter.raiseTo(2)));
		  henry = du("henry", "H", weber.divideBy(ampere));
		  degree_celsius = du("degree celsius", "Cel", new OffsetUnit(kelvin, 273.15));
		  lumen = du("lumen", "lm", candela.multiplyBy(steradian));
		  lux = du("lux", "lx", lumen.divideBy(meter.raiseTo(2)));

		  becquerel = du("becquerel", "Bq", hertz);
		  gray = du("gray", "Gy", joule.divideBy(kilogram));
		  sievert = du("sievert", "Sv", joule.divideBy(kilogram));

		  minute = du("minute", "min", new ScaledUnit(60, second));
		  hour = du("hour", "h", new ScaledUnit(60, minute));
		  day = du("day", "d", new ScaledUnit(24, hour));
		  arc_degree = du("arc degree", "deg", new ScaledUnit(Math.PI / 180, radian));
		  arc_minute = du("arc minute", "'", new ScaledUnit(1.0 / 60.0, arc_degree));
		  arc_second = du("arc second", "\"", new ScaledUnit(1.0 / 60.0, arc_minute));
		  liter = du("liter", "L", new ScaledUnit(1e-3, meter.raiseTo(3)));
		  // exact. However, from 1901 to 1964, 1 liter = 1.000028 dm3
		  metric_ton = du("metric ton", "t", new ScaledUnit(1e3, kilogram));

		  nautical_mile = du("nautical mile", "nmi", new ScaledUnit(1852, meter));
		  knot = du("knot", "kt", nautical_mile.divideBy(hour));
		  angstrom = du("angstrom", null, new ScaledUnit(1e-10, meter));
		  are = du("are", "are", (new ScaledUnit(10, meter)).raiseTo(2));
		  hectare = du("hectare", "ha", new ScaledUnit(100, are));
		  barn = du("barn", "b", new ScaledUnit(1e-28, meter.raiseTo(2)));
		  bar = du("bar", "bar", new ScaledUnit(1e5, pascal));
		  gal = du("gal", "Gal", (new ScaledUnit(1e-2, meter)).divideBy(second.raiseTo(2)));

		  curie = du("curie", "Ci", new ScaledUnit(3.7e10, becquerel));
		  roentgen = du("roentgen", "R", new ScaledUnit(2.58e-4, coulomb.divideBy(kilogram)));
		  rad = du("rad", "rd", new ScaledUnit(1e-2, gray));
		  rem = du("rem", "rem", new ScaledUnit(1e-2, sievert));

		}
		catch (UnitException e)
		{
		  string reason = e.Message;
		  System.Console.Error.WriteLine("Couldn't initialize class SI{0}", string.ReferenceEquals(reason, null) ? "" : (": " + reason));
		}

		AMOUNT_OF_SUBSTANCE_UNIT = mole;
		ELECTRIC_CURRENT_UNIT = ampere;
		LENGTH_UNIT = meter;
		LUMINOUS_INTENSITY_UNIT = candela;
		MASS_UNIT = kilogram;
		PLANE_ANGLE_UNIT = radian;
		SOLID_ANGLE_UNIT = steradian;
		THERMODYNAMIC_TEMPERATURE_UNIT = kelvin;
		TIME_UNIT = second;

		AMPERE = ampere;
		CANDELA = candela;
		KELVIN = kelvin;
		KILOGRAM = kilogram;
		METER = meter;
		METRE = METER;
		MOLE = mole;
		SECOND = second;
		RADIAN = radian;
		STERADIAN = steradian;
		HERTZ = hertz;
		NEWTON = newton;
		PASCAL = pascal;
		JOULE = joule;
		WATT = watt;
		COULOMB = coulomb;
		VOLT = volt;
		FARAD = farad;
		OHM = ohm;
		SIEMENS = siemens;
		WEBER = weber;
		TESLA = tesla;
		HENRY = henry;
		DEGREE_CELSIUS = degree_celsius;
		LUMEN = lumen;
		LUX = lux;
		BECQUEREL = becquerel;
		GRAY = gray;
		SIEVERT = sievert;
		MINUTE = minute;
		HOUR = hour;
		DAY = day;
		ARC_DEGREE = arc_degree;
		ARC_MINUTE = arc_minute;
		ARC_SECOND = arc_second;
		LITER = liter;
		LITRE = LITER;
		METRIC_TON = metric_ton;
		TONNE = METRIC_TON;
		NAUTICAL_MILE = nautical_mile;
		KNOT = knot;
		ANGSTROM = angstrom;
		ARE = are;
		HECTARE = hectare;
		BARN = barn;
		BAR = bar;
		GAL = gal;
		CURIE = curie;
		ROENTGEN = roentgen;
		RAD = rad;
		REM = rem;
	  }

	  /// <summary>
	  /// Factory method for constructing a base unit.
	  /// </summary>
	  /// <param name="name"> The name of the unit. </param>
	  /// <param name="symbol"> The symbol for the unit. </param>
	  /// <param name="quantity"> The base quantity of the unit. </param>
	  /// <returns> The base unit corresponding to the arguments. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static BaseUnit bu(final String name, final String symbol, final BaseQuantity quantity) throws NameException, UnitExistsException
	  private static BaseUnit bu(in string name, in string symbol, in BaseQuantity quantity)
	  {
		return BaseUnit.getOrCreate(UnitName.newUnitName(name, null, symbol), quantity);
	  }

	  /// <summary>
	  /// Factory method for constructing a derived unit.
	  /// </summary>
	  /// <param name="name"> The name of the unit. </param>
	  /// <param name="symbol"> The symbol for the unit. </param>
	  /// <param name="definition"> The definition of the unit. </param>
	  /// <returns> The derived unit corresponding to the arguments. </returns>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static Unit du(final String name, final String symbol, final Unit definition) throws NameException
	  private static Unit du(in string name, in string symbol, in Unit definition)
	  {
		return definition.clone(UnitName.newUnitName(name, null, symbol));
	  }

	  /// <summary>
	  /// Constructs an SI system of units from nothing.
	  /// </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private SI() throws UnitExistsException, NameException, PrefixDBException, NoSuchUnitException
	  private SI() : base(baseUnitDB(), derivedUnitDB())
	  {
	  }

	  /// <summary>
	  /// Returns the base unit database of the SI.
	  /// </summary>
	  /// <returns> The base unit database of the SI. </returns>
	  /// <exception cref="NameException"> Bad unit name. </exception>
	  /// <exception cref="UnitExistsException"> Attempt to redefine an existing unit. </exception>
	  /// <exception cref="NoSuchUnitException"> Necessary unit not found. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static UnitDBImpl baseUnitDB() throws NameException, UnitExistsException, NoSuchUnitException
	  private static UnitDBImpl baseUnitDB()
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final UnitDBImpl db = new UnitDBImpl(9, 9);
		UnitDBImpl db = new UnitDBImpl(9, 9);
		db.addUnit(AMPERE);
		db.addUnit(CANDELA);
		db.addUnit(KELVIN);
		db.addUnit(KILOGRAM);
		db.addUnit(METER);
		db.addUnit(MOLE);
		db.addUnit(SECOND);
		db.addUnit(RADIAN);
		db.addUnit(STERADIAN);
		db.addAlias("metre", "meter");
		return db;
	  }

	  /// <summary>
	  /// Returns the derived unit database of the SI.
	  /// </summary>
	  /// <returns> The derived unit database of the SI. </returns>
	  /// <exception cref="NameException"> Bad unit name. </exception>
	  /// <exception cref="UnitExistsException"> Attempt to redefine an existing unit. </exception>
	  /// <exception cref="NoSuchUnitException"> Necessary unit not found. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static UnitDBImpl derivedUnitDB() throws NameException, UnitExistsException, NoSuchUnitException
	  private static UnitDBImpl derivedUnitDB()
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final UnitDBImpl db = new UnitDBImpl(42, 43);
		UnitDBImpl db = new UnitDBImpl(42, 43);

		db.addUnit(HERTZ);
		db.addUnit(NEWTON);
		db.addUnit(PASCAL);
		db.addUnit(JOULE);
		db.addUnit(WATT);
		db.addUnit(COULOMB);
		db.addUnit(VOLT);
		db.addUnit(FARAD);
		db.addUnit(OHM);
		db.addUnit(SIEMENS);
		db.addUnit(WEBER);
		db.addUnit(TESLA);
		db.addUnit(HENRY);
		db.addUnit(DEGREE_CELSIUS);
		db.addUnit(LUMEN);
		db.addUnit(LUX);

		db.addUnit(BECQUEREL);
		db.addUnit(GRAY);
		db.addUnit(SIEVERT);

		db.addUnit(MINUTE);
		db.addUnit(HOUR);
		db.addUnit(DAY);
		db.addUnit(ARC_DEGREE);
		db.addUnit(ARC_MINUTE);
		db.addUnit(ARC_SECOND);
		db.addUnit(LITER);
		db.addUnit(METRIC_TON);

		db.addUnit(NAUTICAL_MILE);
		db.addUnit(KNOT);
		db.addUnit(ANGSTROM);
		db.addUnit(ARE);
		db.addUnit(HECTARE);
		db.addUnit(BARN);
		db.addUnit(BAR);
		db.addUnit(GAL);

		db.addUnit(CURIE);
		db.addUnit(ROENTGEN);
		db.addUnit(RAD);
		db.addUnit(REM);

		db.addAlias("litre", "liter", "l");
		db.addAlias("tonne", "metric ton");
		db.addSymbol("tne", "tonne");

		return db;
	  }

	  /// <summary>
	  /// Returns an instance of the SI system of units.
	  /// </summary>
	  /// <returns> An instance of the SI system of units. </returns>
	  /// <exception cref="UnitSystemException"> Couldn't create an instance of the SI system of units. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static synchronized SI instance() throws UnitSystemException
	  public static SI instance()
	  {
		  lock (typeof(SI))
		  {
			if (si == null)
			{
			  try
			  {
				si = new SI();
			  }
			  catch (UnitException e)
			  {
				throw new UnitSystemException("Couldn't initialize class SI", e);
			  }
			}
			return si;
		  }
	  }
	}

}
