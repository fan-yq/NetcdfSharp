// Minimal ucar.units (udunits) shim for netcdf-java translation.
// Provides just enough for NetcdfSharp.ucar.nc2.units and time handling. Not a full udunits port.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NetcdfSharp.ucar.units
{
    /// <summary>Thrown when a unit conversion fails.</summary>
    public class ShimConversionException : Exception
    {
        public ShimConversionException(string message) : base(message) { }
    }

    /// <summary>Physical dimension of a unit; compatibility requires equal dimensions.</summary>
    public enum UnitDim
    {
        Time, Length, Pressure, Temperature, Angle, Unitless, Unknown
    }

    /// <summary>A unit of measure. Minimal subset of udunits Unit.</summary>
    public class ShimUnit
    {
        internal string _symbol;
        internal double _scale;    // multiplier to canonical base
        internal double _offset;   // additive offset (for temp like C/F)
        internal UnitDim _dim = UnitDim.Unknown;

        public ShimUnit(string symbol, double scale = 1.0, double offset = 0.0)
        {
            _symbol = symbol;
            _scale = scale;
            _offset = offset;
        }

        public virtual UnitDim Dim => _dim;

        public virtual bool isConvertibleTo(ShimUnit outputUnit)
        {
            // Unknown units are treated leniently (can be converted); otherwise dimensions must match.
            if (_dim == UnitDim.Unknown || outputUnit._dim == UnitDim.Unknown)
            {
                return true;
            }
            return _dim == outputUnit._dim;
        }

        public virtual bool isCompatible(ShimUnit outputUnit)
        {
            if (_dim == UnitDim.Unknown || outputUnit._dim == UnitDim.Unknown)
            {
                return true;
            }
            return _dim == outputUnit._dim;
        }

        /// <summary>Returns a converter from this unit to the output unit (minimal shim).</summary>
        public virtual ShimConverter getConverterTo(ShimUnit outputUnit)
        {
            return new ShimConverter(this, outputUnit);
        }

        /// <summary>Minimal clone: a Unit with the given name/symbol.</summary>
        public virtual ShimUnit clone(ShimUnitName id) => new ShimUnit(id.Symbol, _scale, _offset);

        /// <summary>The derived unit underlying this unit (minimal: self).</summary>
        public virtual ShimDerivedUnit ShimDerivedUnit => this as ShimDerivedUnit ?? new ShimDerivedUnit(_symbol);

        /// <summary>Canonical string form.</summary>
        public virtual string CanonicalString => ToString();

        public virtual double convertTo(double amount, ShimUnit outputUnit)
        {
            // amount in this -> base -> output
            double baseValue = amount * _scale + _offset;
            return (baseValue - outputUnit._offset) / outputUnit._scale;
        }

        public virtual float convertTo(float amount, ShimUnit outputUnit) => (float)convertTo((double)amount, outputUnit);
        public virtual double[] convertTo(double[] amounts, ShimUnit outputUnit)
        {
            double[] r = new double[amounts.Length];
            for (int i = 0; i < amounts.Length; i++) r[i] = convertTo(amounts[i], outputUnit);
            return r;
        }
        public virtual float[] convertTo(float[] amounts, ShimUnit outputUnit)
        {
            float[] r = new float[amounts.Length];
            for (int i = 0; i < amounts.Length; i++) r[i] = (float)convertTo((double)amounts[i], outputUnit);
            return r;
        }

        public override string ToString() => _symbol;
    }

    /// <summary>A unit that is unknown/undefined (e.g. "X" or unitless).</summary>
    public class ShimUnknownUnit : ShimUnit
    {
        public ShimUnknownUnit(string symbol) : base(symbol) { }
    }

    /// <summary>A derived unit (product/quotient of base units).</summary>
    public class ShimDerivedUnit : ShimUnit
    {
        public ShimDerivedUnit(string symbol) : base(symbol) { }
        public virtual ShimUnitDimension Dimension => new ShimUnitDimension(this);
    }

    /// <summary>A scaled unit (base * scale).</summary>
    public class ShimScaledUnit : ShimUnit
    {
        // canonical (derived) symbol, e.g. "Pa" for "hPa", "m" for "km"
        internal string _derivedSymbol;
        public ShimScaledUnit(ShimUnit parent, double scale, string symbol) : base(symbol, scale)
        {
            _dim = parent._dim;
            _derivedSymbol = parent._symbol;
        }
        public ShimScaledUnit(UnitDim dim, double scale, string symbol) : base(symbol, scale)
        {
            _dim = dim;
            _derivedSymbol = symbol;
        }
        public virtual ShimUnit MyUnit => this;
        public virtual ShimUnit Unit => new ShimUnit(_symbol, _scale, _offset);
        public virtual double Scale => _scale;
        public virtual string DerivedSymbol => _derivedSymbol ?? _symbol;
    }

    /// <summary>A unit name/symbol pair.</summary>
    public class ShimUnitName
    {
        public string Symbol { get; }
        public ShimUnitName(string symbol) { Symbol = symbol; }
        public static ShimUnitName newUnitName(string symbol) => new ShimUnitName(symbol);
    }

    /// <summary>Dimension of a derived unit (minimal: the unit itself as a base factor).</summary>
    public class ShimUnitDimension
    {
        private readonly ShimUnit _base;
        public ShimUnitDimension(ShimUnit b) { _base = b; }
        public System.Collections.Generic.IList<ShimFactor> Factors
        {
            get { return new List<ShimFactor> { new ShimFactor(_base) }; }
        }
    }

    /// <summary>A factor of a unit dimension (base unit + exponent).</summary>
    public class ShimFactor
    {
        public ShimUnit Base { get; }
        public ShimFactor(ShimUnit b) { Base = b; }
    }

    /// <summary>Converts values between units (minimal shim, delegates to Unit.convertTo).</summary>
    public class ShimConverter
    {
        private readonly ShimUnit _from;
        private readonly ShimUnit _to;
        public ShimConverter(ShimUnit from, ShimUnit to) { _from = from; _to = to; }
        public virtual double convert(double amount) => _from.convertTo(amount, _to);
        public virtual double[] convert(double[] input, double[] output)
        {
            if (output == null) output = new double[input.Length];
            for (int i = 0; i < input.Length; i++) output[i] = convert(input[i]);
            return output;
        }
        public virtual float[] convert(float[] input, float[] output)
        {
            if (output == null) output = new float[input.Length];
            for (int i = 0; i < input.Length; i++) output[i] = (float)convert(input[i]);
            return output;
        }
    }

    /// <summary>Simple udunits parser producing Unit objects.</summary>
    public class ShimUnitFormat
    {
        public virtual ShimUnit parse(string unitString)
        {
            return UnitParser.Parse(unitString);
        }

        public virtual string format(ShimUnit unit) => unit.ToString();
    }

    /// <summary>Singleton accessor for ShimUnitFormat.</summary>
    public class ShimUnitFormatManager
    {
        private static readonly ShimUnitFormat _instance = new ShimUnitFormat();
        public static ShimUnitFormat instance() => _instance;
    }

    /// <summary>Parses unit strings into Unit objects, tracking physical dimensions.</summary>
    internal static class UnitParser
    {
        // (scale to canonical base, physical dimension)
        private static readonly Dictionary<string, (double scale, UnitDim dim)> BaseUnits =
            new Dictionary<string, (double, UnitDim)>(StringComparer.OrdinalIgnoreCase)
        {
            // length (base: meter)
            ["meter"] = (1, UnitDim.Length), ["metre"] = (1, UnitDim.Length), ["m"] = (1, UnitDim.Length),
            // time (base: second)
            ["second"] = (1, UnitDim.Time), ["seconds"] = (1, UnitDim.Time),
            ["s"] = (1, UnitDim.Time), ["sec"] = (1, UnitDim.Time), ["secs"] = (1, UnitDim.Time),
            ["minute"] = (60, UnitDim.Time), ["minutes"] = (60, UnitDim.Time), ["min"] = (60, UnitDim.Time),
            ["hour"] = (3600, UnitDim.Time), ["hours"] = (3600, UnitDim.Time),
            ["hr"] = (3600, UnitDim.Time), ["hrs"] = (3600, UnitDim.Time), ["h"] = (3600, UnitDim.Time),
            ["day"] = (86400, UnitDim.Time), ["days"] = (86400, UnitDim.Time), ["d"] = (86400, UnitDim.Time),
            ["week"] = (604800, UnitDim.Time), ["weeks"] = (604800, UnitDim.Time), ["wk"] = (604800, UnitDim.Time),
            ["month"] = (2629746, UnitDim.Time), ["months"] = (2629746, UnitDim.Time),
            ["year"] = (31556952, UnitDim.Time), ["years"] = (31556952, UnitDim.Time),
            ["yr"] = (31556952, UnitDim.Time), ["yrs"] = (31556952, UnitDim.Time),
            ["millisecond"] = (0.001, UnitDim.Time), ["milliseconds"] = (0.001, UnitDim.Time),
            ["ms"] = (0.001, UnitDim.Time), ["msec"] = (0.001, UnitDim.Time), ["msecs"] = (0.001, UnitDim.Time),
            ["microsecond"] = (1e-6, UnitDim.Time), ["microseconds"] = (1e-6, UnitDim.Time),
            ["us"] = (1e-6, UnitDim.Time), ["usecond"] = (1e-6, UnitDim.Time), ["useconds"] = (1e-6, UnitDim.Time),
            // pressure (base: pascal)
            ["pascal"] = (1, UnitDim.Pressure), ["Pa"] = (1, UnitDim.Pressure),
            ["hectopascal"] = (100, UnitDim.Pressure), ["hPa"] = (100, UnitDim.Pressure),
            ["millibar"] = (100, UnitDim.Pressure), ["mbar"] = (100, UnitDim.Pressure),
            ["bar"] = (100000, UnitDim.Pressure),
            // temperature (base: kelvin)
            ["kelvin"] = (1, UnitDim.Temperature), ["K"] = (1, UnitDim.Temperature),
            ["celsius"] = (1, UnitDim.Temperature), ["degC"] = (1, UnitDim.Temperature),
            ["degree_C"] = (1, UnitDim.Temperature),
            // angle
            ["degree"] = (1, UnitDim.Angle), ["deg"] = (1, UnitDim.Angle),
            ["radian"] = (1, UnitDim.Angle), ["rad"] = (1, UnitDim.Angle),
            // unitless
            ["percent"] = (0.01, UnitDim.Unitless), ["%"] = (0.01, UnitDim.Unitless), ["1"] = (1, UnitDim.Unitless),
        };

        // SI prefixes applied to a base unit of any dimension.
        private static readonly (char c, double f)[] Prefixes =
        {
            ('k', 1e3), ('M', 1e6), ('G', 1e9), ('m', 1e-3), ('u', 1e-6), ('n', 1e-9), ('p', 1e-12),
            ('h', 1e2), ('d', 1e-1), ('c', 1e-2)
        };

        // derived unit -> canonical base symbol, used for UnitString
        private static readonly Dictionary<string, string> DerivedSymbols =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["hPa"] = "Pa", ["mbar"] = "Pa", ["millibar"] = "Pa", ["bar"] = "Pa", ["hectopascal"] = "Pa",
            ["km"] = "m", ["meter"] = "m", ["metre"] = "m",
        };

        private static string CanonicalSymbol(string s)
        {
            return DerivedSymbols.TryGetValue(s, out var sym) ? sym : s;
        }

        public static ShimUnit Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return new ShimUnknownUnit("?");
            s = s.Trim().Trim(' ', '\t');
            // strip "since ..." part (DateUnit handles date part)
            int since = s.IndexOf(" since ", StringComparison.Ordinal);
            if (since > 0) s = s.Substring(0, since).Trim();

            // leading numeric coefficient, e.g. "11 hPa", "3 days", "0.5 m"
            double coefficient = 1.0;
            int sp = s.IndexOf(' ');
            if (sp > 0 && char.IsDigit(s[0]))
            {
                if (double.TryParse(s.Substring(0, sp), NumberStyles.Float, CultureInfo.InvariantCulture, out double coeff))
                {
                    coefficient = coeff;
                }
                s = s.Substring(sp + 1).Trim();
            }

            s = s.Trim().Replace(" ", "");

            // exact match first (so "hPa" is not taken as prefix "h" + "Pa")
            if (BaseUnits.TryGetValue(s, out var exact))
            {
                var eu = new ShimScaledUnit(exact.dim, coefficient * exact.scale, s);
                eu._dim = exact.dim;
                eu._derivedSymbol = CanonicalSymbol(s);
                return eu;
            }

            // handle common SI prefixes
            double prefix = 1.0;
            UnitDim dim = UnitDim.Unknown;
            if (s.Length >= 2)
            {
                foreach (var (c, f) in Prefixes)
                {
                    if (s[0] == c)
                    {
                        string rest = s.Substring(1);
                        if (BaseUnits.TryGetValue(rest, out var bu))
                        {
                            prefix = f;
                            dim = bu.dim;
                            s = rest;
                            break;
                        }
                    }
                }
            }

            if (s.Contains('/'))
            {
                var parts = s.Split('/');
                if (parts.Length == 2 && BaseUnits.TryGetValue(parts[0], out var n) && BaseUnits.TryGetValue(parts[1], out var d))
                {
                    var du = new ShimDerivedUnit(s) { _scale = coefficient * prefix * n.scale / d.scale };
                    du._dim = n.dim == d.dim ? n.dim : UnitDim.Unknown;
                    return du;
                }
            }

            if (BaseUnits.TryGetValue(s, out var baseUnit))
            {
                var u = new ShimScaledUnit(baseUnit.dim, coefficient * prefix * baseUnit.scale, s);
                u._dim = baseUnit.dim;
                u._derivedSymbol = CanonicalSymbol(s);
                return u;
            }

            // temperature with offset
            if (s == "C" || s == "celsius" || s == "degC")
            {
                var tu = new ShimScaledUnit(UnitDim.Temperature, coefficient * 1.0, s) { _offset = 273.15 };
                tu._dim = UnitDim.Temperature;
                return tu;
            }

            return new ShimUnknownUnit(s);
        }
    }
}
