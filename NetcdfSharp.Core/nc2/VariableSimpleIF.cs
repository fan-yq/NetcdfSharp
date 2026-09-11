// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2
{
	using DataType = NetcdfSharp.ucar.ma2.DataType;

	/// <summary>
	/// A lightweight abstraction of a Variable.
	/// </summary>
	public interface VariableSimpleIF : IComparable<VariableSimpleIF>
	{

		/// <summary>
		/// Name of the data Variable.
		/// Not that this is technically ambiguous v-a-v short or full name;
		/// however, since this is a Variable interface, one must assume
		/// that it is intended to be getFullName().
		/// </summary>
		/// <returns> name of the data Variable </returns>
		/// @deprecated use getFullName or getShortName 

		[Obsolete]
		string Name { get; }

		/// <summary>
		/// full, backslash escaped name of the data Variable
		/// </summary>
		/// <returns> full, backslash escaped name of the data Variable </returns>
		string FullName { get; }

		/// <summary>
		/// short name of the data Variable
		/// </summary>
		/// <returns> short name of the data Variable </returns>
		string ShortName { get; }

		/// <summary>
		/// description of the Variable
		/// </summary>
		/// <returns> description of the Variable, or null if none. </returns>
		string Description { get; }

		/// <summary>
		/// Units of the Variable. These should be udunits compatible if possible
		/// </summary>
		/// <returns> Units of the Variable, or null if none. </returns>
		string UnitsString { get; }

		/// <summary>
		/// Variable rank
		/// </summary>
		/// <returns> Variable rank </returns>
		int Rank { get; }

		/// <summary>
		/// Variable shape
		/// </summary>
		/// <returns> Variable shape </returns>
		int[] Shape { get; }

		/// <summary>
		/// Dimension List. empty for a scalar variable.
		/// </summary>
		/// <returns> List of NetcdfSharp.ucar.nc2.Dimension, ImmutableList in ver6 </returns>
		IList<Dimension> Dimensions { get; }

		/// <summary>
		/// Variable's data type
		/// </summary>
		/// <returns> Variable's data type </returns>
		DataType DataType { get; }

		/// <summary>
		/// Attributes for the variable. </summary>
		AttributeContainer attributes();

		/// <summary>
		/// Attributes for the variable.
		/// </summary>
		/// <returns> List of type NetcdfSharp.ucar.nc2.Attribute </returns>
		/// @deprecated Use attributes() 
		[Obsolete("Use attributes()")]
		IList<NcAttribute> Attributes { get; }

		/// <summary>
		/// find the attribute for the variable with the given name, ignoring case.
		/// </summary>
		/// <param name="name"> attribute name </param>
		/// <returns> the attribute for the variable with the given name, or null if not found. </returns>
		/// @deprecated Use attributes() 
		[Obsolete("Use attributes()")]
		NetcdfSharp.ucar.nc2.NcAttribute findAttributeIgnoreCase(string name);

	}

}
