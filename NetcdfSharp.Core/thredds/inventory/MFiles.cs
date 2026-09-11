// thredds.inventory MFile/MFiles minimal shims.
// MFile abstracts a file on disk (or remote); MFiles.create() is a factory.
// We implement the local-file subset used by the translated cdm code.
using System;
using System.IO;

namespace NetcdfSharp.thredds.inventory
{
    /// <summary>A file abstraction. Minimal local-disk subset.</summary>
    public class MFile
    {
        private readonly string _path;

        public MFile(string path)
        {
            _path = path ?? "";
        }

        public static MFile create(string location)
        {
            return new MFile(Resolve(location));
        }

        private static string Resolve(string location)
        {
            if (string.IsNullOrEmpty(location)) return location;
            var u = new Uri(location);
            if (u.IsFile) return u.LocalPath;
            return location;
        }

        public virtual string getPath() => _path;
        public virtual string getName() => System.IO.Path.GetFileName(_path);
        public virtual long getLastModified() => System.IO.File.Exists(_path) ? new System.IO.FileInfo(_path).LastWriteTimeUtc.Ticks / 10000 : 0;
        public virtual long LastModified => getLastModified();
        public virtual long getLength() => System.IO.File.Exists(_path) ? new System.IO.FileInfo(_path).Length : 0;
        public virtual bool isDirectory() => System.IO.Directory.Exists(_path);
        public virtual bool isFile() => System.IO.File.Exists(_path);
        public virtual bool exists() => System.IO.File.Exists(_path) || System.IO.Directory.Exists(_path);

        // property-style aliases used by the translated grib port
        public virtual string Path => _path;
        public virtual string Name => getName();
        public virtual long Length => getLength();
        public virtual MFile Parent => getParent();
        public virtual System.IO.Stream InputStream => getInputStream();
        public virtual MFile Directory => getDirectory();

        public virtual System.IO.Stream getInputStream()
        {
            return _path == null || _path.Length == 0 || !exists() ? null : System.IO.File.OpenRead(_path);
        }

        public virtual MFile getDirectory()
        {
            string dir = isDirectory() ? _path : System.IO.Path.GetDirectoryName(_path);
            return dir == null ? null : new MFile(dir);
        }

        public virtual MFile getParent()
        {
            string parent = System.IO.Path.GetDirectoryName(_path);
            return parent == null ? null : new MFile(parent);
        }

        public virtual MFile getChild(string child)
        {
            if (string.IsNullOrEmpty(child)) return null;
            return new MFile(System.IO.Path.Combine(_path, child));
        }

        public virtual MFile getPathOf() => this;

        public override string ToString() => _path;
    }

    /// <summary>Factory + helpers for MFile.</summary>
    public static class MFiles
    {
        public static MFile create(string location)
        {
            return MFile.create(location);
        }
    }
}
