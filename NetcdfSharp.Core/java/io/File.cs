// NetcdfSharp.java.io.File shim for netcdf-java translation.
using System;
using System.IO;

namespace NetcdfSharp.java.io
{
    /// <summary>
    /// An abstract representation of file and directory pathnames.
    /// Minimal subset used by netcdf-java translated code.
    /// </summary>
    public class File : IComparable<File>, IComparable
    {
        public static readonly char separatorChar = Path.DirectorySeparatorChar;
        public static readonly string separator = Path.DirectorySeparatorChar.ToString();
        public static readonly char pathSeparatorChar = Path.PathSeparator;
        public static readonly string pathSeparator = Path.PathSeparator.ToString();

        private readonly string _path;

        public File(string pathname)
        {
            _path = pathname ?? "";
        }
        public File(string parent, string child)
        {
            if (string.IsNullOrEmpty(parent)) _path = child ?? "";
            else if (string.IsNullOrEmpty(child)) _path = parent;
            else _path = System.IO.Path.Combine(parent, child);
        }
        public File(File parent, string child) : this(parent?.GetPath(), child) { }

        public string GetPath() => _path;
        public string getPath() => _path;
        public string GetAbsolutePath() => System.IO.Path.GetFullPath(_path);
        public string getAbsolutePath() => GetAbsolutePath();
        public string GetName() => System.IO.Path.GetFileName(_path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        public string getName() => GetName();
        public string GetParent() => System.IO.Path.GetDirectoryName(_path);
        public string getParent() => GetParent();
        public string getCanonicalPath() => GetAbsolutePath();
        public bool IsAbsolute() => System.IO.Path.IsPathRooted(_path);
        public bool isAbsolute() => IsAbsolute();
        public bool Exists() => System.IO.File.Exists(_path) || System.IO.Directory.Exists(_path);
        public bool exists() => Exists();
        public bool IsFile() => System.IO.File.Exists(_path);
        public bool isFile() => IsFile();
        public bool IsDirectory() => System.IO.Directory.Exists(_path);
        public bool isDirectory() => IsDirectory();
        public bool CanRead() { try { return Exists(); } catch { return false; } }
        public bool canRead() => CanRead();
        public bool CanWrite() => Exists();
        public bool canWrite() => CanWrite();
        public long Length() => new System.IO.FileInfo(_path).Length;
        public long length() => Length();
        public long LastModified() => System.IO.File.Exists(_path) ? new System.IO.FileInfo(_path).LastWriteTimeUtc.Ticks / 10000 : 0;
        public long lastModified() => LastModified();
        public string[] List() => System.IO.Directory.Exists(_path) ? System.IO.Directory.GetFileSystemEntries(_path) : new string[0];
        public string[] list() => List();
        public File[] ListFiles() => System.IO.Directory.Exists(_path)
            ? System.Array.ConvertAll(System.IO.Directory.GetFileSystemEntries(_path), p => new File(p))
            : new File[0];
        public File[] listFiles() => ListFiles();
        public bool Mkdir() { try { return System.IO.Directory.CreateDirectory(_path) != null; } catch { return false; } }
        public bool mkdir() => Mkdir();
        public bool Mkdirs()
        {
            try { System.IO.Directory.CreateDirectory(_path); return true; }
            catch { return false; }
        }
        public bool mkdirs() => Mkdirs();
        public bool Delete() { try { if (System.IO.File.Exists(_path)) { System.IO.File.Delete(_path); return true; } if (System.IO.Directory.Exists(_path)) { System.IO.Directory.Delete(_path, false); return true; } return false; } catch { return false; } }
        public bool delete() => Delete();
        public bool CreateNewFile()
        {
            try { using (System.IO.File.Create(_path)) { } return true; }
            catch { return false; }
        }
        public bool createNewFile() => CreateNewFile();
        public bool RenameTo(File dest) { try { System.IO.File.Move(_path, dest.GetPath()); return true; } catch { return false; } }
        public bool renameTo(File dest) => RenameTo(dest);
        public string ToURI() => "file:" + _path.Replace('\\', '/');
        public string ToURL() => ToURI();
        public override string ToString() => _path;
        public override bool Equals(object obj) => obj is File f && f._path == _path;
        public override int GetHashCode() => _path?.GetHashCode() ?? 0;
        public int CompareTo(File other) => string.CompareOrdinal(_path, other?._path);
        int IComparable.CompareTo(object obj) => obj is File f ? CompareTo(f) : 1;
    }
}
