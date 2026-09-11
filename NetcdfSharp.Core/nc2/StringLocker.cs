// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Threading;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// A list of strings that only allows one thread to use any given value at the same time.
	/// 
	/// @author cmrose </summary>
	/// @deprecated will move to NetcdfSharp.ucar.nc2.util in ver 6. 
	[Obsolete("will move to NetcdfSharp.ucar.nc2.util in ver 6.")]
	public class StringLocker
	{

	  private readonly ISet<string> stringSet = new HashSet<string>();

	  public virtual void control(string item)
	  {
		  lock (this)
		  {
			// If the string is in use by another thread then wait() for the other thread
			while (stringSet.Contains(item))
			{
			  try
			  {
				Monitor.Wait(this);
			  }
			  catch (InterruptedException)
			  {
				Thread.CurrentThread.Interrupt();
			  }
			}
			// Finished waiting so the thread can have the string
			stringSet.Add(item);
		  }
	  }

	  public virtual void release(string item)
	  {
		  lock (this)
		  {
			// Tell StringLocker the thread is done with the string
			stringSet.Remove(item);
			Monitor.PulseAll(this);
		  }
	  }

	  public override string ToString()
	  {
		return stringSet.ToString();
	  }

	}

}
