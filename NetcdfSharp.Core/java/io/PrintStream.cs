// NetcdfSharp.java.io.PrintStream shim for netcdf-java translation.
using System;
using System.IO;

namespace NetcdfSharp.java.io
{
    /// <summary>
    /// A PrintStream adds functionality to another output stream, namely the
    /// ability to print representations of various data values conveniently.
    /// </summary>
    public class PrintStream : IDisposable
    {
        public static readonly PrintStream Out = new PrintStream(Console.Out);
        public static readonly PrintStream Err = new PrintStream(Console.Error);

        private readonly TextWriter _w;
        private bool _closed;

        public PrintStream(Stream file, bool autoFlush = false) : this(new StreamWriter(file), autoFlush) { }
        public PrintStream(string fileName) : this(new StreamWriter(fileName), false) { }
        public PrintStream(TextWriter @out, bool autoFlush = false) { _w = @out; }

        // PascalCase aliases used by converter-generated call sites (Java print/println).
        public void Write(object obj) => print(obj);
        public void Write(int i) => print(i);
        public void Write(string s) => print(s);
        public void Write(char c) => print(c);
        public void Write(bool b) => print(b);
        public void Write(long l) => print(l);
        public void Write(double d) => print(d);
        public void Write(float f) => print(f);
        public void WriteLine() => println();
        public void WriteLine(object obj) { print(obj); println(); }
        public void WriteLine(string s) { print(s); println(); }
        public void WriteLine(int i) { print(i); println(); }

        public void print(bool b) => _w.Write(b ? "true" : "false");
        public void print(char c) => _w.Write(c);
        public void print(int i) => _w.Write(i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public void print(long l) => _w.Write(l.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public void print(float f) => _w.Write(f.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public void print(double d) => _w.Write(d.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public void print(string s) => _w.Write(s ?? "null");
        public void print(object obj) => _w.Write(obj == null ? "null" : obj.ToString());
        public void println() => _w.WriteLine();
        public void println(bool b) { print(b); println(); }
        public void println(char c) { print(c); println(); }
        public void println(int i) { print(i); println(); }
        public void println(long l) { print(l); println(); }
        public void println(float f) { print(f); println(); }
        public void println(double d) { print(d); println(); }
        public void println(string s) { print(s); println(); }
        public void println(object obj) { print(obj); println(); }
        public void printf(string format, params object[] args)
        {
            _w.Write(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
        }
        public void format(string format, params object[] args) => printf(format, args);
        public void flush() => _w.Flush();
        public void close() { if (_closed) return; _closed = true; _w.Flush(); _w.Dispose(); }
        public void Dispose() => close();
    }
}
