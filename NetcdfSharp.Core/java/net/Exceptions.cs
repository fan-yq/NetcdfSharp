// java.text / java.net exception shims for netcdf-java translation.
using System;

namespace NetcdfSharp.java.text
{
    /// <summary>Signals that a string could not be parsed.</summary>
    public class ParseException : FormatException
    {
        public ParseException() { }
        public ParseException(string message) : base(message) { }
        public ParseException(string message, Exception inner) : base(message, inner) { }
        public ParseException(string message, int errorOffset) : base(message) { }
    }
}

namespace NetcdfSharp.java.net
{
    /// <summary>Signals that a connection could not be established.</summary>
    public class ConnectException : System.Net.Sockets.SocketException
    {
        public ConnectException() { }
        public ConnectException(string message) : base() { }
    }

    /// <summary>Signals that a host could not be resolved.</summary>
    public class UnknownHostException : System.Exception
    {
        public UnknownHostException() { }
        public UnknownHostException(string message) : base(message) { }
    }

    /// <summary>Signals that a URL is malformed.</summary>
    public class MalformedURLException : System.UriFormatException
    {
        public MalformedURLException() { }
        public MalformedURLException(string message) : base(message) { }
    }
}
