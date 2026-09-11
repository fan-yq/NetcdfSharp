// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2
{
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcRange = NetcdfSharp.ucar.ma2.NcRange;
	using Section = NetcdfSharp.ucar.ma2.Section;

	/// <summary>
	/// Allow dimensions of length 1 to be removed
	/// 
	/// @author caron
	/// @since 12/16/13
	/// </summary>
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @Immutable class ReduceReader implements ProxyReader
	internal class ReduceReader : ProxyReader
	{
		private readonly Variable orgClient;
		private readonly IList<int> dims; // dimension index into original

		/// <summary>
		/// Reduce 1 or more dimension of length 1
		/// </summary>
		/// <param name="orgClient"> original variable </param>
		/// <param name="dims"> index(es) in original variable of the dimension to reduce; dimension must be length 1. </param>
		internal ReduceReader(Variable orgClient, IList<int> dims)
		{
			this.orgClient = orgClient;
			this.dims = new List<int>(dims);
			this.dims.Sort();
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public NetcdfSharp.ucar.ma2.Array reallyRead(Variable client, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		public virtual NcArray reallyRead(Variable client, CancelTask cancelTask)
		{
			NcArray data = orgClient._read();

			for (int i = dims.Count - 1; i >= 0; i--)
			{
				data = data.reduce(dims[i]); // highest first
			}
			return data;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override public NetcdfSharp.ucar.ma2.Array reallyRead(Variable client, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
		public virtual NcArray reallyRead(Variable client, Section section, CancelTask cancelTask)
		{
			Section.Builder orgSection = Section.builder().appendRanges(section.Ranges);
			foreach (int dim in dims)
			{
				orgSection.insertRange(dim, NcRange.ONE); // lowest first
			}

			NcArray data = orgClient._read(orgSection.build());
			for (int i = dims.Count - 1; i >= 0; i--)
			{
				data = data.reduce(dims[i]); // highest first
			}

			return data;
		}
	}

}
