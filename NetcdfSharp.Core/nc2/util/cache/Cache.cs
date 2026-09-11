// NetcdfSharp.ucar.nc2.util.cache minimal shims.
// File caching is an optional performance feature; these stubs keep the translated
// RandomAccessFile compiling while the global file cache is effectively disabled.
using System;
using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.util.cache
{
    /// <summary>Interface for objects that can be cached in a FileCache.</summary>
    public interface FileCacheable : IDisposable
    {
        string Location { get; }
    }

    /// <summary>Interface for the file cache.</summary>
    public interface FileCacheIF
    {
        FileCacheable acquire(NetcdfSharp.ucar.nc2.dataset.DatasetUrl durl, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask, object iospMessage);
        void release(FileCacheable file);
    }

    /// <summary>Factory that creates FileCacheable objects.</summary>
    public interface FileFactory
    {
        FileCacheable open(NetcdfSharp.ucar.nc2.dataset.DatasetUrl durl, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask, object iospMessage);
    }

    /// <summary>A (mostly disabled) file cache. Keeps objects open until process exit.</summary>
    public class FileCache : FileCacheIF
    {
        private readonly string _name;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, FileCacheable> _open = new();
        private readonly int _maxElements;
        private readonly int _periodsToKeepAlive;
        private readonly int _maxPeriods;

        public FileCache(string name, int maxElements, int periodsToKeepAlive, int maxPeriods, long keepAlivePeriod)
        {
            _name = name;
            _maxElements = maxElements;
            _periodsToKeepAlive = periodsToKeepAlive;
            _maxPeriods = maxPeriods;
        }

        public FileCacheable acquire(NetcdfSharp.ucar.nc2.dataset.DatasetUrl durl, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask, object iospMessage)
        {
            string key = durl?.location ?? "?";
            if (_open.TryGetValue(key, out var f) && f != null)
                return f;
            return null;
        }

        public void release(FileCacheable file)
        {
            // do nothing (keep open)
        }
    }
}
