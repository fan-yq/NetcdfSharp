// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.units
{
	/// <summary>
	/// Standard formatter/parser for unit specifications.
	/// <p/>
	/// This is a hand-written replacement for the JavaCC-generated
	/// StandardUnitFormat of the Java original. It covers the unit specification
	/// syntax used by StandardUnitDB (numbers, identifiers with SI prefixes,
	/// '*', '/', '.', '^' and implicit exponents, parentheses, and time origins
	/// via "since"/"@").
	/// 
	/// @author Steven R. Emmerson
	/// </summary>
	[Serializable]
	public sealed class StandardUnitFormat : UnitFormatImpl
	{
		private const long serialVersionUID = 2L;

		/// <summary>
		/// The singleton instance of this class.
		/// 
		/// @serial
		/// </summary>
		private static StandardUnitFormat _instance;

		/// <summary>
		/// Constructs from nothing.
		/// </summary>
		private StandardUnitFormat()
		{
		}

		/// <summary>
		/// Returns an instance of this class.
		/// </summary>
		/// <returns> An instance of this class. </returns>
		public static StandardUnitFormat instance()
		{
			lock (typeof(StandardUnitFormat))
			{
				if (_instance == null)
				{
					_instance = new StandardUnitFormat();
				}
				return _instance;
			}
		}

		/// <summary>
		/// Formats a Factor (a base unit/exponent pair).
		/// </summary>
		/// <param name="factor"> The base unit/exponent pair. </param>
		/// <param name="buffer"> The buffer to append to. </param>
		/// <returns> The appended-to buffer. </returns>
		public override StringBuilder format(Factor factor, StringBuilder buffer)
		{
			return buffer.Append(factor.ToString());
		}

		/// <summary>
		/// Formats a unit.
		/// </summary>
		/// <param name="unit"> The unit to be formatted. </param>
		/// <param name="buffer"> The buffer to append to. </param>
		/// <returns> The appended-to buffer. </returns>
		public override StringBuilder format(Unit unit, StringBuilder buffer)
		{
			return buffer.Append(unit.ToString());
		}

		/// <summary>
		/// Formats a unit using a long form.
		/// </summary>
		/// <param name="unit"> The unit to be formatted. </param>
		/// <param name="buffer"> The buffer to append to. </param>
		/// <returns> The appended-to buffer. </returns>
		public override StringBuilder longFormat(Unit unit, StringBuilder buffer)
		{
			return buffer.Append(unit.ToString());
		}

		/// <summary>
		/// Decodes a unit specification. An unrecognized unit is made into an
		/// UnknownUnit.
		/// </summary>
		/// <param name="spec"> The unit specification to be decoded. </param>
		/// <param name="unitDB"> The unit database to use. </param>
		/// <returns> The unit corresponding to the specification. </returns>
		public override Unit parse(string spec, UnitDB unitDB)
		{
			try
			{
				Parser parser = new Parser(spec == null ? "" : spec.Trim(), unitDB);
				return parser.parse();
			}
			catch (Exception e)
			{
				throw new UnitParseException(spec, e);
			}
		}

		/// <summary>
		/// A small tokenizer/parser for the udunits unit specification syntax.
		/// </summary>
		private sealed class Parser
		{
			private readonly string s;
			private readonly UnitDB unitDB;
			private int pos;

			internal Parser(string spec, UnitDB unitDB)
			{
				this.s = spec;
				this.unitDB = unitDB;
			}

			// ------------------------------------------------------------
			// tokenizer helpers
			// ------------------------------------------------------------

			private void skipSpace()
			{
				while (pos < s.Length && char.IsWhiteSpace(s[pos]))
				{
					pos++;
				}
			}

			private bool atEnd()
			{
				skipSpace();
				return pos >= s.Length;
			}

			private char peek()
			{
				skipSpace();
				return pos < s.Length ? s[pos] : '\0';
			}

			private bool startsNumber()
			{
				skipSpace();
				if (pos >= s.Length)
				{
					return false;
				}
				char c = s[pos];
				if (char.IsDigit(c))
				{
					return true;
				}
				if ((c == '+' || c == '-') && pos + 1 < s.Length && char.IsDigit(s[pos + 1]))
				{
					return true;
				}
				return false;
			}

			private double readNumber()
			{
				skipSpace();
				int start = pos;
				if (pos < s.Length && (s[pos] == '+' || s[pos] == '-'))
				{
					pos++;
				}
				while (pos < s.Length && (char.IsDigit(s[pos]) || s[pos] == '.'))
				{
					pos++;
				}
				if (pos < s.Length && (s[pos] == 'e' || s[pos] == 'E'))
				{
					pos++;
					if (pos < s.Length && (s[pos] == '+' || s[pos] == '-'))
					{
						pos++;
					}
					while (pos < s.Length && char.IsDigit(s[pos]))
					{
						pos++;
					}
				}
				return double.Parse(s.Substring(start, pos - start), CultureInfo.InvariantCulture);
			}

			private string readIdentifier()
			{
				skipSpace();
				int start = pos;
				while (pos < s.Length && (char.IsLetterOrDigit(s[pos]) || s[pos] == '_'))
				{
					pos++;
				}
				string id = s.Substring(start, pos - start);
				// an identifier that ends in a digit-run (and is not all digits)
				// is really "identifier" followed by an implicit exponent, e.g.
				// "m2" == "m"^2, "s-2" == "s"^-2.
				if (id.Length > 1)
				{
					int i = id.Length;
					while (i > 0 && char.IsDigit(id[i - 1]))
					{
						i--;
					}
					if (i > 0 && i < id.Length && char.IsLetter(id[i - 1]))
					{
						pos -= (id.Length - i);
						id = id.Substring(0, i);
					}
				}
				return id;
			}

			// ------------------------------------------------------------
			// grammar: unitSpec -> productExpr ("since"|"@" time)?
			// ------------------------------------------------------------

			internal Unit parse()
			{
				Unit unit = productExpr();
				skipSpace();
				if (pos < s.Length && s[pos] == '@')
				{
					pos++;
					DateTime origin = readTimeOrigin();
					return new TimeScaleUnit(unit, origin);
				}
				if (pos < s.Length && s.IndexOf("since", pos, StringComparison.Ordinal) == pos)
				{
					pos += "since".Length;
					DateTime origin = readTimeOrigin();
					return new TimeScaleUnit(unit, origin);
				}
				return unit;
			}

			// productExpr -> powerExpr (('/'|'.'|'*'|space) powerExpr)*
			private Unit productExpr()
			{
				Unit unit = powerExpr();
				while (true)
				{
					skipSpace();
					if (pos >= s.Length)
					{
						break;
					}
					char c = s[pos];
					if (c == '/')
					{
						pos++;
						Unit other = powerExpr();
						unit = unit.divideBy(other);
					}
					else if (c == '.' || c == '*')
					{
						pos++;
						Unit other = powerExpr();
						unit = unit.multiplyBy(other);
					}
					else if (startsNumber() || char.IsLetter(s[pos]) || s[pos] == '(' || s[pos] == '_')
					{
						// implicit multiplication via whitespace
						Unit other = powerExpr();
						unit = unit.multiplyBy(other);
					}
					else
					{
						break;
					}
				}
				return unit;
			}

			// powerExpr -> basicExpr ('^' integer | integer)?
			private Unit powerExpr()
			{
				Unit unit = basicExpr();
				skipSpace();
				if (pos < s.Length && s[pos] == '^')
				{
					pos++;
					int exponent = (int) readNumber();
					unit = unit.raiseTo(exponent);
				}
				else if (startsNumber())
				{
					int exponent = (int) readNumber();
					unit = unit.raiseTo(exponent);
				}
				return unit;
			}

			// basicExpr -> number | unitIdentifier | '(' productExpr ')'
			private Unit basicExpr()
			{
				skipSpace();
				if (pos >= s.Length)
				{
					throw new UnitParseException(s, "unexpected end of unit specification");
				}
				char c = s[pos];
				if (c == '(')
				{
					pos++;
					Unit unit = productExpr();
					skipSpace();
					if (pos >= s.Length || s[pos] != ')')
					{
						throw new UnitParseException(s, "missing ')' in unit specification");
					}
					pos++;
					return unit;
				}
				if (startsNumber())
				{
					double number = readNumber();
					return DerivedUnitImpl.DIMENSIONLESS.multiplyBy(number);
				}
				if (char.IsLetter(c) || c == '_')
				{
					return unitIdentifier();
				}
				throw new UnitParseException(s, "unexpected character '" + c + "' in unit specification");
			}

			// unitIdentifier -> identifier with optional SI prefix stripping
			private Unit unitIdentifier()
			{
				string id = readIdentifier();
				double scale = 1;
				string remain = id;
				Unit unit = getUnit(unitDB, remain);
				while (unit == null)
				{
					Prefix prefix = null;
					try
					{
						PrefixDB prefixDB = PrefixDBManager.instance();
						prefix = prefixDB.getPrefixBySymbol(remain);
						if (prefix == null)
						{
							prefix = prefixDB.getPrefixByName(remain);
						}
					}
					catch (Exception e)
					{
						throw new UnitParseException(s, e);
					}
					if (prefix == null)
					{
						unit = UnknownUnit.create(remain);
						break;
					}
					scale *= prefix.Value;
					remain = remain.Substring(prefix.ID.Length);
					unit = getUnit(unitDB, remain);
				}
				return unit.multiplyBy(scale);
			}

			private static Unit getUnit(UnitDB unitDB, string id)
			{
				if (string.ReferenceEquals(id, null) || id.Length == 0)
				{
					return null;
				}
				try
				{
					Unit unit = unitDB.get(id);
					if (unit != null)
					{
						return unit;
					}
					return unitDB.getByName(id);
				}
				catch (Exception e)
				{
					return null;
				}
			}

			// readTimeOrigin -> an ISO 8601-ish date/time string
			private DateTime readTimeOrigin()
			{
				skipSpace();
				int start = pos;
				while (pos < s.Length && !char.IsWhiteSpace(s[pos]))
				{
					pos++;
				}
				string dateText = s.Substring(start, pos - start);
				DateTime origin;
				if (!DateTime.TryParse(dateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out origin))
				{
					throw new UnitParseException(s, "couldn't parse time origin \"" + dateText + "\"");
				}
				return origin;
			}
		}
	}
}
