// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.util
{
	/// <summary>
	/// Maintains indentation level for printing nested structures.
	/// </summary>
	public class Indent
	{
	  private int nspaces;

// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private int level_Conflict;
	  private readonly StringBuilder blanks = new StringBuilder();
	  private string indent = "";

	  // nspaces = how many spaces each level adds.
	  // max 100 levels
	  public Indent(int nspaces)
	  {
		this.nspaces = nspaces;
		makeBlanks(100);
	  }

	  public virtual Indent incr()
	  {
		level_Conflict++;
		IndentLevel = level_Conflict;
		return this;
	  }

	  public virtual Indent decr()
	  {
		level_Conflict--;
		IndentLevel = level_Conflict;
		return this;
	  }

	  public virtual int level()
	  {
		return level_Conflict;
	  }

	  public override string ToString()
	  {
		return indent;
	  }

	  public virtual int IndentLevel
	  {
		  set
		  {
			this.level_Conflict = value;
			if (value * nspaces >= blanks.Length)
			{
			  makeBlanks(100);
			}
			int end = Math.Min(value * nspaces, blanks.Length);
			indent = blanks.ToString().Substring(0, end);
		  }
	  }

	  private void makeBlanks(int len)
	  {
		for (int i = 0; i < len * nspaces; i++)
		{
		  blanks.Append(" ");
		}
	  }
	}

}
