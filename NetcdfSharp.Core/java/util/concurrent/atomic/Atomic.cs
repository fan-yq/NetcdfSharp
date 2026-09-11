// NetcdfSharp.java.util.concurrent.atomic shims for netcdf-java translation.
using System;
using System.Threading;

namespace NetcdfSharp.java.util.concurrent.atomic
{
    /// <summary>A long value that may be updated atomically.</summary>
    public class AtomicLong
    {
        private long _value;
        public AtomicLong(long initialValue = 0) { _value = initialValue; }
        public long get() => Interlocked.Read(ref _value);
        public void set(long newValue) => Interlocked.Exchange(ref _value, newValue);
        public long getAndIncrement() => Interlocked.Increment(ref _value) - 1;
        public long getAndDecrement() => Interlocked.Decrement(ref _value) + 1;
        public long incrementAndGet() => Interlocked.Increment(ref _value);
        public long decrementAndGet() => Interlocked.Decrement(ref _value);
        public long getAndAdd(long delta) => Interlocked.Add(ref _value, delta) - delta;
        public long addAndGet(long delta) => Interlocked.Add(ref _value, delta);
        public bool compareAndSet(long expect, long update) => Interlocked.CompareExchange(ref _value, update, expect) == expect;
        public override string ToString() => get().ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>An int value that may be updated atomically.</summary>
    public class AtomicInteger
    {
        private int _value;
        public AtomicInteger(int initialValue = 0) { _value = initialValue; }
        public int get() => Volatile.Read(ref _value);
        public void set(int newValue) => Volatile.Write(ref _value, newValue);
        public int getAndIncrement() => Interlocked.Increment(ref _value) - 1;
        public int getAndDecrement() => Interlocked.Decrement(ref _value) + 1;
        public int incrementAndGet() => Interlocked.Increment(ref _value);
        public int decrementAndGet() => Interlocked.Decrement(ref _value);
        public int getAndAdd(int delta) => Interlocked.Add(ref _value, delta) - delta;
        public int addAndGet(int delta) => Interlocked.Add(ref _value, delta);
        public bool compareAndSet(int expect, int update) => Interlocked.CompareExchange(ref _value, update, expect) == expect;
        public override string ToString() => get().ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
