// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/* Copyright Unidata */
namespace NetcdfSharp.ucar.nc2.util
{

	/// <summary>
	/// Optional with an error message when empty
	/// </summary>
	/// @deprecated All uses will move to NetcdfSharp.java.util.Optional in ver6 
	[Obsolete("All uses will move to NetcdfSharp.java.util.Optional in ver6")]
	public class Optional<T>
	{
	  private readonly T value;
	  private readonly string errMessage;

	  public static Optional<T> empty(string errMessage)
	  {
		return new Optional<T>(default(T), errMessage);
	  }

	  public static Optional<T> of(T value)
	  {
		value = Objects.requireNonNull(value);
		return new Optional<T>(value, null);
	  }

	  public static Optional<T> ofNullable(T value)
	  {
		return new Optional<T>(value, null);
	  }

	  private Optional(T value, string errMessage)
	  {
		this.value = value;
		this.errMessage = errMessage;
	  }

	  public virtual T get()
	  {
		if (value == null)
		{
		  throw new NoSuchElementException("No value present");
		}
		return value;
	  }

	  public virtual T orElse(T other)
	  {
		return value != null ? value : other;
	  }

	  public virtual T orElseThrow()
	  {
		if (value == null)
		{
		  throw new NoSuchElementException("No value present");
		}
		return value;
	  }

	  public virtual T orElseThrow(Func<Exception> exceptionSupplier)
	  {
		if (value == null)
		{
		  throw exceptionSupplier();
		}
		return value;
	  }

	  public virtual bool Present
	  {
		  get
		  {
			return value != null;
		  }
	  }

	  public virtual bool isPresent() => value != null;

	  public virtual void ifPresent(Action<T> consumer)
	  {
		if (value != null) consumer(value);
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errMessage;
		  }
	  }
	}


}
