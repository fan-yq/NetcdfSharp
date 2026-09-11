// NetcdfSharp.java.io.PrintWriter shim for netcdf-java translation.
using System;
using System.IO;

namespace NetcdfSharp.java.io
{
    /// <summary>
    /// Prints formatted representations of objects to a text-output stream.
    /// Mirrors the subset of NetcdfSharp.java.io.PrintWriter used by netcdf-java.
    /// </summary>
    public class PrintWriter : Writer
    {
        private readonly TextWriter _w;
        private bool _closed;

        public PrintWriter(Stream file, bool autoFlush = false) : this(new StreamWriter(file), autoFlush) { }
        public PrintWriter(string fileName) : this(new StreamWriter(fileName), false) { }
        public PrintWriter(PrintStream ps) : this(new PrintStreamAdapter(ps), false) { }
        public PrintWriter(TextWriter @out, bool autoFlush = false)
        {
            _w = @out;
        }

        /// <summary>Adapts a NetcdfSharp.java.io.PrintStream to a System.IO.TextWriter.</summary>
        private sealed class PrintStreamAdapter : TextWriter
        {
            private readonly PrintStream _ps;
            public PrintStreamAdapter(PrintStream ps) { _ps = ps; }
            public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
            public override void Write(char value) => _ps.print(value);
            public override void Write(string value) => _ps.print(value);
            public override void WriteLine() => _ps.println();
        }

        public PrintWriter append(object obj) => appendstr(obj == null ? "null" : obj.ToString());
        // FIX for CS0108;CS0114
        public PrintWriter appendstr(string csq) { write(csq); return this; }
        // FIX for CS0108;CS0114
        public PrintWriter appendchar(char c) { write(c); return this; }
        public virtual void print(bool b) => write(b ? "true" : "false");
        public virtual void print(char c) => write(c);
        public virtual void print(int i) => write(i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public virtual void print(long l) => write(l.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public virtual void print(float f) => write(f.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public virtual void print(double d) => write(d.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public virtual void print(string s) => write(s ?? "null");
        public virtual void print(object obj) => write(obj == null ? "null" : obj.ToString());
        public virtual void println() { write("\n"); }
        public virtual void println(bool b) { print(b); println(); }
        public virtual void println(char c) { print(c); println(); }
        public virtual void println(int i) { print(i); println(); }
        public virtual void println(long l) { print(l); println(); }
        public virtual void println(float f) { print(f); println(); }
        public virtual void println(double d) { print(d); println(); }
        public virtual void println(string s) { print(s); println(); }
        public virtual void println(object obj) { print(obj); println(); }
        public virtual void printf(string format, params object[] args)
        {
            write(string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args));
        }
        public virtual void format(string format, params object[] args) => printf(format, args);

        // PascalCase aliases used by converter-generated call sites (Java print/println).
        public virtual void Write(object obj) => write(obj == null ? "null" : obj.ToString());
        public virtual void Write(int i) => write(i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public virtual void Write(string s) => write(s);
        public virtual void Write(char c) => write(c);
        public virtual void Write(bool b) => write(b ? "true" : "false");
        public virtual void Write(long l) => write(l.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public virtual void Write(double d) => write(d.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public virtual void Write(float f) => write(f.ToString(System.Globalization.CultureInfo.InvariantCulture));
        public virtual void WriteLine() => write("\n");
        public virtual void WriteLine(object obj) { Write(obj); WriteLine(); }
        public virtual void WriteLine(string s) { Write(s); WriteLine(); }
        public virtual void WriteLine(int i) { Write(i); WriteLine(); }
        public bool checkError() => false;

        public override void write(int c) => _w.Write((char)c);
        public override void write(string str)
        {
            if (str == null) return;
            _w.Write(str);
        }
        public override void write(char[] cbuf, int off, int len)
        {
            _w.Write(cbuf, off, len);
        }
        public override void flush() { _w.Flush(); }
        public override void close()
        {
            if (_closed) return;
            _closed = true;
            _w.Flush();
            _w.Dispose();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) close();
            base.Dispose(disposing);
        }
    }
}
