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

namespace NetcdfSharp.ucar.nc2.util
{

	/// <summary>
	/// An iterator that must be closed.
	/// 
	/// try (CloseableIterator iter = getIterator()) {
	/// // do stuff
	/// }
	/// 
	/// TODO will move to thredds.inventory in ver6
	/// </summary>
	public interface CloseableIterator<T> : IEnumerator<T>, IDisposable
	{
	}

}
