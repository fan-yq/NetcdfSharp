// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.util
{
	/// <summary>
	/// Allows long tasks to be cancelled. Used in potentially long method calls which the user may want the option to
	/// cancel.
	/// </summary>
	public interface CancelTask
	{
	  static CancelTask create()
	  {
		return new CancelTaskImpl();
	  }

	  /// <summary>
	  /// Calling routine may cancel, called routine checks this method, and if true, return asap.
	  /// </summary>
	  /// <returns> true if task was cancelled </returns>
	  bool Cancel {get;}

	  /// <summary>
	  /// Application calls to see if task is done. </summary>
	  bool Done {get;set;}


	  /// <summary>
	  /// Called routine sets whether operation successfully completed. </summary>
	  void setSuccess()
	  {
	  }

	  /// <summary>
	  /// Called routine got an error, so it sets a message for calling program to show to user.
	  /// </summary>
	  /// <param name="msg"> message to show user </param>
	  string Error {set;}

	  /// <summary>
	  /// Called routine may optionally show a progress message for calling program to show to user.
	  /// </summary>
	  /// <param name="msg"> message to show user </param>
	  /// <param name="progress"> count of progress </param>
	  void setProgress(string msg, int progress);
	}

}
