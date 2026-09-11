// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2
{
#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using EscapeStrings = NetcdfSharp.ucar.nc2.util.EscapeStrings;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NcRange = NetcdfSharp.ucar.ma2.NcRange;
	using Section = NetcdfSharp.ucar.ma2.Section;

	/// <summary>
	/// A String expression for denoting a section of a Variable to be read.
	/// Allows nesting for subsetting of Structure member variables.
	/// 
	/// @author caron
	/// @since May 8, 2008 </summary>
	/// <seealso cref="<a href="
	///      "https://www.unidata.ucar.edu/software/netcdf-java/reference/SectionSpecification.html">SectionSpecification</a>/>
	public class ParsedSectionSpec
	{
		private const bool debugSelector = false;

		/// <summary>
		/// Parse a section specification String. These have the form:
		/// 
		/// <pre>
		///  section specification := selector | selector '.' selector
		///  selector := varName ['(' dims ')']
		///  varName := ESCAPED_STRING
		/// <p/>
		///   dims := dim | dim, dims
		///   dim := ':' | slice | start ':' end | start ':' end ':' stride
		///   slice := INTEGER
		///   start := INTEGER
		///   stride := INTEGER
		///   end := INTEGER
		///   ESCAPED_STRING : must escape characters = ".("
		/// </pre>
		/// <p/>
		/// Nonterminals are in lower case, terminals are in upper case, literals are in single quotes.
		/// Optional components are enclosed between square braces '[' and ']'.
		/// </summary>
		/// <param name="ncfile"> look for variable in here </param>
		/// <param name="variableSection"> the string to parse, eg "record(12).wind(1:20,:,3)" </param>
		/// <returns> return ParsedSectionSpec, parsed representation of the variableSection String </returns>
		/// <exception cref="IllegalArgumentException"> when token is misformed, or variable name doesnt exist in ncfile </exception>
		/// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if section does not match variable shape </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static ParsedSectionSpec parseVariableSection(NetcdfFile ncfile, String variableSection) throws NetcdfSharp.ucar.ma2.InvalidRangeException
		public static ParsedSectionSpec parseVariableSection(NetcdfFile ncfile, string variableSection)
		{
			IList<string> tokes = EscapeStrings.tokenizeEscapedName(variableSection);
			if (tokes.Count == 0)
			{
				throw new ArgumentException("empty sectionSpec = " + variableSection);
			}

			string selector = tokes[0];
			ParsedSectionSpec outerV = parseVariableSelector(ncfile, selector);

			// parse each selector, find the inner variable
			ParsedSectionSpec current = outerV;
			for (int i = 1; i < tokes.Count; i++)
			{
				selector = tokes[i];
				current.child = parseVariableSelector(current.v, selector);
				current = current.child;
			}

			return outerV;
		}

		// selector := varFullNameEsc(indexSelect) or memberNameEsc(indexSelect)
		// parse variable name and index selector out of the selector String. variable name must be escaped
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private static ParsedSectionSpec parseVariableSelector(Object parent, String selector) throws NetcdfSharp.ucar.ma2.InvalidRangeException
		private static ParsedSectionSpec parseVariableSelector(object parent, string selector)
		{
			string varNameEsc, indexSelect = null;

			int pos1 = EscapeStrings.indexOf(selector, '(');
			if (pos1 < 0)
			{ // no index
				varNameEsc = selector;
			}
			else
			{
				varNameEsc = selector.Substring(0, pos1);
				int pos2 = selector.IndexOf(')', pos1 + 1);
				indexSelect = selector.Substring(pos1, pos2 - pos1);
			}
			if (debugSelector)
			{
				Console.WriteLine(" parseVariableSection <" + selector + "> = <" + varNameEsc + ">, <" + indexSelect + ">");
			}

			Variable v = null;
			if (parent is NetcdfFile)
			{ // then varNameEsc = varFullNameEsc (i.e. includes groups)
				NetcdfFile ncfile = (NetcdfFile)parent;
				v = ncfile.findVariable(varNameEsc);

			}
			else if (parent is Structure)
			{ // then varNameEsc = memberNameEsc (i.e. includes groups)
				Structure s = (Structure)parent;
				v = s.findVariable(NetcdfFiles.makeNameUnescaped(varNameEsc)); // s.findVariable wants unescaped version
			}
			if (v == null)
			{
				throw new ArgumentException(" cant find variable: " + varNameEsc + " in selector=" + selector);
			}

			if (v.DataType == DataType.SEQUENCE)
			{
				indexSelect = null; // ignore whatever was sent
			}

			// get the selected Ranges, or all, and add to the list
			Section section;
			if (!string.ReferenceEquals(indexSelect, null))
			{
				section = new Section(indexSelect);
				section = Section.fill(section, v.Shape); // Check section has no nulls, set from shape array.
			}
			else
			{
				section = v.ShapeAsSection; // all
			}

			return new ParsedSectionSpec(v, section);
		}

		/// <summary>
		/// Make section specification String from a range list for a Variable.
		/// </summary>
		/// <param name="v"> for this Variable. </param>
		/// <param name="ranges"> list of Range. Must includes all parent structures. The list be null, meaning use all.
		///        Individual ranges may be null, meaning all for that dimension. </param>
		/// <returns> section specification String. </returns>
		public static string makeSectionSpecString(Variable v, IList<NcRange> ranges)
		{
			StringBuilder sb = new StringBuilder();
			makeSpec(sb, v, ranges);
			return sb.ToString();
		}

		private static IList<NcRange> makeSpec(StringBuilder sb, Variable v, IList<NcRange> orgRanges)
		{
			if (v.MemberOfStructure)
			{
				orgRanges = makeSpec(sb, v.ParentStructure, orgRanges);
				sb.Append('.');
			}

			IList<NcRange> ranges = (orgRanges == null) ? v.Ranges : orgRanges;

			sb.Append(v.MemberOfStructure ? NetcdfFiles.makeValidSectionSpecName(v.ShortName) : NetcdfFiles.makeFullNameSectionSpec(v));

			if (!v.VariableLength && !v.Scalar)
			{ // sequences cant be sectioned
				sb.Append('(');
				for (int count = 0; count < v.Rank; count++)
				{
					NcRange r = ranges[count];
					if (r == null)
					{
						r = new NcRange(v.getDimension(count).Length);
					}
					if (count > 0)
					{
						sb.Append(", ");
					}
					sb.Append(r);
				}
				sb.Append(')');
			}

			return (orgRanges == null) ? null : new List<NcRange>(ranges.Skip(v.Rank));
		}

		///////////////////////////////////////////////////////////////////////////
		// Modify to allow setting after creation
		public Variable v; // the variable
		public Section section; // section for this variable, filled in from variable if needed
		public ParsedSectionSpec child; // if not null, v is a Structure, and this is one of its members

		public ParsedSectionSpec(Variable v, Section section)
		{
			this.v = v;
			this.section = section;
		}

		/// <summary>
		/// Public simple constructor
		/// </summary>
		public ParsedSectionSpec()
		{
			this.v = null;
			this.section = null;
			this.child = null;
		}

		public virtual Variable Variable
		{
			get
			{
				return v;
			}
		}

		public virtual Section ArraySection
		{
			get
			{
				return section;
			}
		}

		public override string ToString()
		{
			return "ParsedSectionSpec{" + "v=" + v.FullName + ", section=" + section + ", child=" + child + '}';
		}

		public virtual string makeSectionSpecString()
		{
			return ParsedSectionSpec.makeSectionSpecString(this.v, this.section.Ranges);
		}
	}

#pragma warning restore CS0162

}
