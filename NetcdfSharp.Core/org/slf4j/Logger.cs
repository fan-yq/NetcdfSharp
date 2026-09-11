// slf4j-compatible Logger - backed by Microsoft.Extensions.Logging.
// Java's "{}" placeholders are substituted with the supplied arguments.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NetcdfSharp.org.slf4j
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.slf4j.Logger, backed by Microsoft.Extensions.Logging.ILogger.
    /// </summary>
    public class Logger
    {
        private readonly string _name;
        private readonly ILogger _logger;

        internal Logger(string name, ILogger logger)
        {
            _name = name;
            _logger = logger;
        }

        public string getName()
        {
            return _name;
        }

        public bool isDebugEnabled() => _logger.IsEnabled(LogLevel.Debug);

        public bool isInfoEnabled() => _logger.IsEnabled(LogLevel.Information);

        public bool isWarnEnabled() => _logger.IsEnabled(LogLevel.Warning);

        public bool isErrorEnabled() => _logger.IsEnabled(LogLevel.Error);

        public bool isTraceEnabled() => _logger.IsEnabled(LogLevel.Trace);

        public void debug(string msg)
        {
            _logger.LogDebug(msg);
        }

        public void debug(string format, params object[] args)
        {
            _logger.LogDebug(FormatMessage(format, args));
        }

        public void debug(string format, object arg1)
        {
            _logger.LogDebug(FormatMessage(format, new object[] { arg1 }));
        }

        public void debug(string format, object arg1, object arg2)
        {
            _logger.LogDebug(FormatMessage(format, new object[] { arg1, arg2 }));
        }

        public void info(string msg)
        {
            _logger.LogInformation(msg);
        }

        public void info(string format, params object[] args)
        {
            _logger.LogInformation(FormatMessage(format, args));
        }

        public void info(string format, object arg1)
        {
            _logger.LogInformation(FormatMessage(format, new object[] { arg1 }));
        }

        public void info(string format, object arg1, object arg2)
        {
            _logger.LogInformation(FormatMessage(format, new object[] { arg1, arg2 }));
        }

        public void warn(string msg)
        {
            _logger.LogWarning(msg);
        }

        public void warn(string format, params object[] args)
        {
            _logger.LogWarning(FormatMessage(format, args));
        }

        public void warn(string format, object arg1)
        {
            _logger.LogWarning(FormatMessage(format, new object[] { arg1 }));
        }

        public void warn(string format, object arg1, object arg2)
        {
            _logger.LogWarning(FormatMessage(format, new object[] { arg1, arg2 }));
        }

        public void error(string msg)
        {
            _logger.LogError(msg);
        }

        public void error(string format, params object[] args)
        {
            _logger.LogError(FormatMessage(format, args));
        }

        public void error(string format, object arg1)
        {
            _logger.LogError(FormatMessage(format, new object[] { arg1 }));
        }

        public void error(string format, object arg1, object arg2)
        {
            _logger.LogError(FormatMessage(format, new object[] { arg1, arg2 }));
        }

        public void trace(string msg)
        {
            _logger.LogTrace(msg);
        }

        public void trace(string format, params object[] args)
        {
            _logger.LogTrace(FormatMessage(format, args));
        }

        /// <summary>
        /// Replace Java log "{}" placeholders (in order) with the supplied arguments.
        /// If the last argument is an Exception, it is appended with its stack trace.
        /// </summary>
        private static string FormatMessage(string format, object[] args)
        {
            if (string.IsNullOrEmpty(format))
            {
                return "";
            }

            var sb = new StringBuilder(format.Length + 32);
            int argIndex = 0;
            int start = 0;
            int idx;
            while ((idx = format.IndexOf("{}", start, StringComparison.Ordinal)) >= 0)
            {
                sb.Append(format, start, idx - start);
                if (args != null && argIndex < args.Length)
                {
                    sb.Append(StringValue(args[argIndex]));
                    argIndex++;
                }
                else
                {
                    sb.Append("{}");
                }
                start = idx + 2;
            }
            sb.Append(format, start, format.Length - start);

            // append any leftover arguments (e.g. a trailing Exception not matched by a placeholder)
            if (args != null)
            {
                for (; argIndex < args.Length; argIndex++)
                {
                    sb.Append(' ').Append(StringValue(args[argIndex]));
                }
            }
            return sb.ToString();
        }

        private static string StringValue(object o)
        {
            if (o == null)
            {
                return "null";
            }
            if (o is Exception e)
            {
                return e.GetType().Name + ": " + e.Message + Environment.NewLine + e.StackTrace;
            }
            return o.ToString();
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.slf4j.LoggerFactory, backed by Microsoft.Extensions.Logging.
    /// Returns one shared ILogger per name, written to the console.
    /// </summary>
    public static class LoggerFactory
    {
        private static readonly object Lock = new object();
        private static readonly Dictionary<string, Logger> Loggers = new Dictionary<string, Logger>();
        private static Microsoft.Extensions.Logging.ILoggerFactory _factory;

        /// <summary>
        /// Set a custom ILoggerFactory (e.g. one configured with file/console/structured providers).
        /// If not set, a console logger factory is used.
        /// </summary>
        public static void SetLoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory factory)
        {
            lock (Lock)
            {
                _factory = factory;
                Loggers.Clear();
            }
        }

        private static Microsoft.Extensions.Logging.ILoggerFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
                        builder.AddConsole());
                }
                return _factory;
            }
        }

        public static Logger getLogger(Type type)
        {
            return getLogger(type != null ? type.FullName : "unknown");
        }

        public static Logger getLogger(string name)
        {
            if (name == null)
            {
                name = "unknown";
            }
            lock (Lock)
            {
                if (!Loggers.TryGetValue(name, out Logger logger))
                {
                    var ilogger = Factory.CreateLogger(name);
                    logger = new Logger(name, ilogger);
                    Loggers[name] = logger;
                }
                return logger;
            }
        }
    }
}
