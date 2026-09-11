// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2021 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.filter
{

	/// <summary>
	/// Filter implementation of Shuffle.
	/// </summary>
	public class Shuffle : Filter
	{

	  private const string name = "shuffle";

	  private const int id = 2;

	  private int elemSize;
	  private const int DEFAULT_SIZE = 4;

	  public Shuffle(IDictionary<string, object> properties)
	  {
		try
		{
		  elemSize = (int) properties[Filters.Keys.ELEM_SIZE];
		}
		catch (Exception)
		{
		  elemSize = DEFAULT_SIZE;
		}
	  }

	  public override string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public override int Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public override byte[] encode(byte[] dataIn)
	  {
		if (elemSize <= 1)
		{
		  return dataIn;
		}

		int nElems = dataIn.Length / elemSize;
		int[] start = new int[elemSize];
		for (int k = 0; k < elemSize; k++)
		{
		  start[k] = k * nElems;
		}

		byte[] result = new byte[dataIn.Length];
		for (int i = 0; i < nElems; i++)
		{
		  for (int j = 0; j < elemSize; j++)
		  {
			result[i + start[j]] = dataIn[(i * elemSize) + j];
		  }
		}

		int leftoverBytes = dataIn.Length % this.elemSize;
		System.Array.Copy(dataIn, dataIn.Length - leftoverBytes, result, result.Length - leftoverBytes, leftoverBytes);

		return result;
	  }

	  public override byte[] decode(byte[] dataIn)
	  {
		if (elemSize <= 1)
		{
		  return dataIn;
		}

		int nElems = dataIn.Length / this.elemSize;
		byte[] result = new byte[dataIn.Length];

		for (int j = 0; j < this.elemSize; ++j)
		{
		  int sourceIndex = j * nElems;
		  int destIndex = j;
		  for (int i = 0; i < nElems; ++i)
		  {
			result[destIndex] = dataIn[sourceIndex];
			sourceIndex++;
			destIndex += this.elemSize;
		  }
		}

		int leftoverBytes = dataIn.Length % this.elemSize;
		System.Array.Copy(dataIn, dataIn.Length - leftoverBytes, result, result.Length - leftoverBytes, leftoverBytes);

		return result;
	  }

	  public class Provider : FilterProvider
	  {

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual int Id
		{
			get
			{
			  return id;
			}
		}

		public virtual Filter create(IDictionary<string, object> properties)
		{
		  return new Shuffle(properties);
		}
	  }
	}

}
