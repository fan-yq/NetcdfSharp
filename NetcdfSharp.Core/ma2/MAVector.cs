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

namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// Abstraction for vector operations.
	/// A vector is a rank-1 Array.
	/// All operations done in double precision
	/// 
	/// @author @caron
	/// </summary>

	public class MAVector
	{
		private NcArray a;
		private int nelems;
		private NcIndex ima;

		/// <summary>
		/// Create an MAVector out of a double array
		/// </summary>
		public MAVector(double[] values) : this(NcArray.makeFromJavaArray(values, false))
		{
		}

		/// <summary>
		/// Create an MAVector of the given length.
		/// </summary>
		public MAVector(int nelems)
		{
			this.a = new ArrayDouble.D1(nelems);
			this.nelems = nelems;
			ima = a.Index;
		}

		/// <summary>
		/// Create an MAVector using the given rank-1 array.
		/// </summary>
		/// <param name="a"> rank-1 array </param>
		/// <exception cref="IllegalArgumentException"> is a is not rank 1 </exception>
		public MAVector(NcArray a)
		{
			this.a = a;
			if (a.Rank != 1)
			{
				throw new ArgumentException("rank != 1, instead = " + a.Rank);
			}
			nelems = a.Shape[0];
			ima = a.Index;
		}

		public virtual int Nelems
		{
			get
			{
				return nelems;
			}
		}

		public virtual double getDouble(int i)
		{
			return a.getDouble(ima.set(i));
		}

		public virtual void setDouble(int i, double val)
		{
			a.setDouble(ima.set(i), val);
		}

		/// <summary>
		/// Create a new MAVector that is the same as this one, with a copy of the backing store.
		/// </summary>
		public virtual MAVector copy()
		{
			return new MAVector(a.copy());
		}

		/// <summary>
		/// Cos between two vectors = dot(v) / norm() * norm(v)
		/// </summary>
		/// <param name="v"> cosine with this vector </param>
		/// <returns> double result: cos between this and v </returns>
		/// <exception cref="IllegalArgumentException"> if nelems != v.getNelems(). </exception>
		public virtual double cos(MAVector v)
		{

			if (nelems != v.Nelems)
			{
				throw new ArgumentException("MAVector.cos " + nelems + " != " + v.Nelems);
			}

			double norm = this.norm();
			double normV = v.norm();
			if ((norm == 0.0) || (normV == 0.0))
			{
				return 0.0;
			}
			else
			{
				return dot(v) / (norm * normV);
			}
		}

		/// <summary>
		/// Dot product of 2 vectors
		/// </summary>
		/// <param name="v"> dot product with this vector </param>
		/// <returns> double result: dot product </returns>
		/// <exception cref="IllegalArgumentException"> if nelems != v.getNelems(). </exception>
		public virtual double dot(MAVector v)
		{

			if (nelems != v.Nelems)
			{
				throw new ArgumentException("MAVector.dot " + nelems + " != " + v.Nelems);
			}

			double sum = 0.0;
			for (int k = 0; k < nelems; k++)
			{
				sum += getDouble(k) * v.getDouble(k);
			}

			return sum;
		}

		/// <summary>
		/// Get the L2 norm of this vector.
		/// </summary>
		/// <returns> double norm </returns>
		public virtual double norm()
		{

			double sum = 0.0;
			for (int k = 0; k < nelems; k++)
			{
				double val = getDouble(k);
				sum += val * val;
			}

			return Math.Sqrt(sum);
		}

		/// <summary>
		/// Normalize this vector, so it has norm = 1.0.
		/// </summary>
		public virtual void normalize()
		{
			double norm = this.norm();
			if (norm <= 0.0)
			{
				return;
			}

			for (int k = 0; k < nelems; k++)
			{
				double val = getDouble(k);
				setDouble(k, val / norm);
			}
		}

	}


}
