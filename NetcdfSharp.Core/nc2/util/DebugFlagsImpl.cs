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

namespace NetcdfSharp.ucar.nc2.util
{

	/// <summary>
	/// An implementation of DebugFlags
	/// 
	/// TODO will move in ver 6
	/// </summary>
	public class DebugFlagsImpl : DebugFlags
	{

	  private IDictionary<string, bool> map = new Dictionary<string, bool>();

	  /// <summary>
	  /// All flags are off
	  /// </summary>
	  public DebugFlagsImpl()
	  {
	  }

	  /// <summary>
	  /// Constructor.
	  /// </summary>
	  /// <param name="flagsOn"> space-separated list of flags to turn on. </param>
	  public DebugFlagsImpl(string flagsOn)
	  {
		StringTokenizer stoke = new StringTokenizer(flagsOn);
		while (stoke.hasMoreTokens())
		{
		  set(stoke.nextToken(), true);
		}
	  }

	  public virtual bool isSet(string flagName)
	  {
		bool? b = map[flagName];
		return (b != null) && b.Value;
	  }

	  public virtual void set(string flagName, bool value)
	  {
		map[flagName] = value;
	  }

	  public virtual string SetFlags
	  {
		  get
		  {
			return map.Keys.ToString();
		  }
	  }

	}

}
