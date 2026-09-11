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
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Generalization of Range (which is restricted to (start:stop:stride).
	/// RangeIterator is over an arbitrary set of integers from the set {0..fullSize-1}.
	/// 
	/// @author John
	/// @since 8/19/2015
	/// </summary>
	public interface RangeIterator : IEnumerable<int>
	{

	  IEnumerator<int> iterator();

	  /// <summary>
	  /// The number of index in this iterator. </summary>
	  int length();

	  /// <summary>
	  /// The name of this Range iterator. </summary>
	  string Name {get;}

	  /// @deprecated use copyWithName(). 
	  [Obsolete("use copyWithName().")]
	  RangeIterator setName(string name);

	  /// <summary>
	  /// Make a copy with a different name. </summary>
	  RangeIterator copyWithName(string name);
	}

}
