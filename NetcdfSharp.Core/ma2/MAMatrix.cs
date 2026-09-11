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
	/// Abstraction for matrix operations.
	/// A matrix is a rank-2 Array: m[rows, cols].
	/// All operations done in double precision
	/// 
	/// @author @caron
	/// </summary>

	public class MAMatrix
	{
		private NcArray a;
		private int nrows, ncols;
		private NcIndex ima;

		/// <summary>
		/// Create an MAMatrix of the given shape.
		/// </summary>
		/// <param name="nrows"> number of rows </param>
		/// <param name="ncols"> number of cols </param>
		public MAMatrix(int nrows, int ncols)
		{
			this.a = new ArrayDouble.D2(nrows, ncols);
			this.nrows = nrows;
			this.ncols = ncols;
			ima = a.Index;
		}

		/// <summary>
		/// Create an MAMatrix using the given rank-2 array.
		/// </summary>
		/// <param name="a"> rank-2 array </param>
		/// <exception cref="IllegalArgumentException"> is a is not rank 2 </exception>
		public MAMatrix(NcArray a)
		{
			this.a = a;
			if (a.Rank != 2)
			{
				throw new ArgumentException("rank != 2, instead = " + a.Rank);
			}
			nrows = a.Shape[0];
			ncols = a.Shape[1];
			ima = a.Index;
		}

		public virtual int Nrows
		{
			get
			{
				return nrows;
			}
		}

		public virtual int Ncols
		{
			get
			{
				return ncols;
			}
		}

		public virtual double getDouble(int i, int j)
		{
			return a.getDouble(ima.set(i, j));
		}

		public virtual void setDouble(int i, int j, double val)
		{
			a.setDouble(ima.set(i, j), val);
		}

		/// <summary>
		/// Create a new MAMatrix that is the same as this one, with a copy of the backing store.
		/// </summary>
		public virtual MAMatrix copy()
		{
			return new MAMatrix(a.copy());
		}

		/// <summary>
		/// Create a MAMatrix that is the transpose of this one, with the same backing store.
		/// Use copy() to get a copy.
		/// </summary>
		public virtual MAMatrix transpose()
		{
			return new MAMatrix(a.transpose(0, 1));
		}

		/// <summary>
		/// Get the jth column, return as a MAVector: same backing store.
		/// </summary>
		public virtual MAVector column(int j)
		{
			return new MAVector(a.slice(1, j));
		}

		/// <summary>
		/// Get the ith row, return as a MAVector: same backing store.
		/// </summary>
		public virtual MAVector row(int i)
		{
			return new MAVector(a.slice(0, i));
		}

		/// <summary>
		/// Dot product of matrix and vector: return M dot v
		/// </summary>
		/// <param name="v"> dot product with this vector </param>
		/// <returns> MAVector result: new vector </returns>
		/// <exception cref="IllegalArgumentException"> if ncols != v.getSize(). </exception>
		public virtual MAVector dot(MAVector v)
		{

			if (ncols != v.Nelems)
			{
				throw new ArgumentException("MAMatrix.dot " + ncols + " != " + v.Nelems);
			}

			ArrayDouble.D1 result = new ArrayDouble.D1(nrows);
			NcIndex imr = result.Index;

			for (int i = 0; i < nrows; i++)
			{
				double sum = 0.0;
				for (int k = 0; k < ncols; k++)
				{
					sum += getDouble(i, k) * v.getDouble(k);
				}
				result.setDouble(imr.set(i), sum);
			}

			return new MAVector(result);
		}

		/// <summary>
		/// Matrix multiply: return m1 * m2.
		/// </summary>
		/// <param name="m1"> left matrix </param>
		/// <param name="m2"> right matrix </param>
		/// <returns> MAMatrix result: new matrix </returns>
		/// <exception cref="IllegalArgumentException"> if m1.getNcols() != m2.getNrows(). </exception>
		public static MAMatrix multiply(MAMatrix m1, MAMatrix m2)
		{

			if (m1.Ncols != m2.Nrows)
			{
				throw new ArgumentException("MAMatrix.multiply " + m1.Ncols + " != " + m2.Nrows);
			}
			int kdims = m1.Ncols;

			ArrayDouble.D2 result = new ArrayDouble.D2(m1.Nrows, m2.Ncols);
			NcIndex imr = result.Index;

			for (int i = 0; i < m1.Nrows; i++)
			{
				for (int j = 0; j < m2.Ncols; j++)
				{
					double sum = 0.0;
					for (int k = 0; k < kdims; k++)
					{
						sum += m1.getDouble(i, k) * m2.getDouble(k, j);
					}
					result.setDouble(imr.set(i, j), sum);
				}
			}

			return new MAMatrix(result);
		}

		/// <summary>
		/// Matrix multiply by a diagonal matrix, store result in this: this = this * diag
		/// </summary>
		/// <param name="diag"> diagonal matrix stored as a Vector </param>
		/// <exception cref="IllegalArgumentException"> if ncols != diag.getNelems(). </exception>
		public virtual void postMultiplyDiagonal(MAVector diag)
		{

			if (ncols != diag.Nelems)
			{
				throw new ArgumentException("MAMatrix.postMultiplyDiagonal " + ncols + " != " + diag.Nelems);
			}

			for (int i = 0; i < nrows; i++)
			{
				for (int j = 0; j < ncols; j++)
				{
					double val = a.getDouble(ima.set(i, j)) * diag.getDouble(j);
					a.setDouble(ima, val);
				}
			}
		}

		/// <summary>
		/// Matrix multiply by a diagonal matrix, store result in this: this = diag * this
		/// </summary>
		/// <param name="diag"> diagonal matrix stored as a Vector </param>
		/// <exception cref="IllegalArgumentException"> if nrows != diag.getNelems(). </exception>
		public virtual void preMultiplyDiagonal(MAVector diag)
		{

			if (nrows != diag.Nelems)
			{
				throw new ArgumentException("MAMatrix.preMultiplyDiagonal " + nrows + " != " + diag.Nelems);
			}

			for (int i = 0; i < nrows; i++)
			{
				for (int j = 0; j < ncols; j++)
				{
					double val = a.getDouble(ima.set(i, j)) * diag.getDouble(i);
					a.setDouble(ima, val);
				}
			}
		}

	}

}
