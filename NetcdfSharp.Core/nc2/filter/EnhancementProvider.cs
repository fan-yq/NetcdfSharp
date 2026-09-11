// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 2021 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.filter
{
	using NetcdfSharp.ucar.ma2;
	using NetcdfSharp.ucar.nc2;
	using Enhance = NetcdfSharp.ucar.nc2.dataset.NetcdfDataset.Enhance;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;


	/// <summary>
	/// A Service Provider of <seealso cref="Enhancement"/>.
	/// </summary>
	public interface EnhancementProvider
	{
	  string AttributeName {get;}

	  bool appliesTo(ISet<Enhance> enhance, DataType dt);

	  Enhancement create(VariableDS var);


	}



}
