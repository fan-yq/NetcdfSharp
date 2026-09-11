// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.util
{
	/// <summary>
	/// Default implementation of CancelTask, used in order to get status return.
	/// 
	/// @author caron
	/// @since 4/10/13 </summary>
	/// @deprecated will move or be private in ver6 
	[Obsolete("will move or be private in ver6")]
	public class CancelTaskImpl : CancelTask
	{
	  protected internal bool done;
	  protected internal bool success;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  protected internal bool cancel_Conflict;
	  protected internal string error;
	  protected internal string note;
	  protected internal int progress;

	  /// <summary>
	  /// Application calls to see if task is success.
	  /// </summary>
	  public virtual bool Success
	  {
		  get
		  {
			return success;
		  }
	  }

	  /// <summary>
	  /// Application call this to find out if there was an error.
	  /// </summary>
	  public virtual bool isError()
	  {
		return !string.ReferenceEquals(error, null);
	  }

	  /// <summary>
	  /// Called routine got an error, so it sets a message for calling program to show to user.
	  /// </summary>
	  public virtual string Error
	  {
		  set
		  {
			this.error = value;
		  }
	  }

	  /// <summary>
	  /// Application call this to get the error message, if any.
	  /// </summary>
	  public virtual string ErrorMessage
	  {
		  get
		  {
			return error;
		  }
	  }

	  /// <summary>
	  /// Application will call this when the user cancels.
	  /// </summary>
	  public virtual void cancel()
	  {
		cancel_Conflict = true;
	  }

	  /// <summary>
	  /// Application call this to get the progress message, if any.
	  /// </summary>
	  public virtual string ProgressMessage
	  {
		  get
		  {
			return note;
		  }
	  }

	  /// <summary>
	  /// Application call this to get the progress count, if any.
	  /// </summary>
	  public virtual int Progress
	  {
		  get
		  {
			return progress;
		  }
	  }

	  public virtual bool Done
	  {
		  get
		  {
			return done;
		  }
		  set
		  {
			this.done = value;
		  }
	  }


	  public virtual bool Cancel
	  {
		  get
		  {
			return cancel_Conflict;
		  }
	  }

	  public virtual void setSuccess()
	  {
		this.success = true;
		this.done = true;
	  }


	  public virtual void setProgress(string msg, int progress)
	  {
		this.note = msg;
		if (progress > 0)
		{
		  this.progress = progress;
		}
	  }

	  public override string ToString()
	  {
		if (cancel_Conflict)
		{
		  return "was canceled";
		}
		if (isError())
		{
		  return "error= " + error;
		}
		if (success)
		{
		  return "success";
		}
		return "finished=" + done;
	  }
	}

}
