// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// Specialization of Index for rank 0 arrays, ie scalars.
	/// </summary>
	/// <seealso cref="NcIndex"
	/// @author caron/>
	public class Index0D : NcIndex
	{

		internal Index0D() : base(0)
		{
			this.size = 1;
			this.offset = 0;
		}

		public Index0D(int[] shape) : base(shape)
		{
		}

		public override int currentElement()
		{
			return offset;
		}

		public override int incr()
		{
			return offset;
		}

		public override object clone()
		{
			return base.clone();
		}

		public virtual NcIndex set()
		{
			return this;
		}
	}

}
