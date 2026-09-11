// NetcdfSharp.java.io.FormatHelper: Java String.format / Formatter compat.
using System;
using System.Globalization;
using System.Text;

namespace NetcdfSharp.java.io
{
    /// <summary>
    /// Formatting helper backing NetcdfSharp.java.util.Formatter.
    /// Supports the Java conversion syntax used by the translated code (%d, %s, %f, %e, %g,
    /// %x, %o, %c, %b, %n, %%), including flags, width and precision, and falls back to
    /// .NET composite formatting ({0}) for hand-written shims.
    /// </summary>
    public static class FormatHelper
    {
        public static string Format(string format, params object[] args)
        {
            if (string.IsNullOrEmpty(format))
            {
                return format ?? "";
            }

            object[] a = args ?? new object[0];

            // .NET composite format (hand-written shims). Java %-formats with a literal '{'
            // will fail here and fall through to the Java parser below.
            if (format.IndexOf('{') >= 0)
            {
                try
                {
                    return string.Format(CultureInfo.InvariantCulture, format, a);
                }
                catch (FormatException)
                {
                    // fall through
                }
            }

            if (format.IndexOf('%') < 0)
            {
                return format;
            }

            return JavaFormat(format, a);
        }

        private const string FlagChars = "-+ 0,(#";

        private static string JavaFormat(string format, object[] args)
        {
            StringBuilder sb = new StringBuilder(format.Length + 16);
            int seq = 0;
            int i = 0;
            while (i < format.Length)
            {
                char ch = format[i++];
                if (ch != '%')
                {
                    sb.Append(ch);
                    continue;
                }
                if (i >= format.Length)
                {
                    sb.Append('%');
                    break;
                }
                if (format[i] == '%')
                {
                    sb.Append('%');
                    i++;
                    continue;
                }
                if (format[i] == 'n')
                {
                    sb.Append(Environment.NewLine);
                    i++;
                    continue;
                }

                int explicitIndex = -1;
                int j = i;
                while (j < format.Length && char.IsDigit(format[j]))
                {
                    j++;
                }
                if (j > i && j < format.Length && format[j] == '$')
                {
                    explicitIndex = int.Parse(format.Substring(i, j - i), CultureInfo.InvariantCulture) - 1;
                    i = j + 1;
                }

                string flags = "";
                while (i < format.Length && FlagChars.IndexOf(format[i]) >= 0)
                {
                    flags += format[i];
                    i++;
                }

                int width = -1;
                int ws = i;
                while (i < format.Length && char.IsDigit(format[i]))
                {
                    i++;
                }
                if (i > ws)
                {
                    width = int.Parse(format.Substring(ws, i - ws), CultureInfo.InvariantCulture);
                }

                int precision = -1;
                if (i < format.Length && format[i] == '.')
                {
                    i++;
                    int ps = i;
                    while (i < format.Length && char.IsDigit(format[i]))
                    {
                        i++;
                    }
                    precision = (i > ps) ? int.Parse(format.Substring(ps, i - ps), CultureInfo.InvariantCulture) : 0;
                }

                if (i >= format.Length)
                {
                    break; // malformed trailing specifier
                }

                char conv = format[i++];
                int argIndex = (explicitIndex >= 0) ? explicitIndex : seq++;
                object arg = (argIndex >= 0 && argIndex < args.Length) ? args[argIndex] : null;

                string body = FormatValue(conv, arg, flags, precision);
                sb.Append(ApplyWidth(body, width, flags, conv));
            }
            return sb.ToString();
        }

        private static string FormatValue(char conv, object arg, string flags, int precision)
        {
            switch (conv)
            {
                case 'd':
                {
                    long v = ToInt64(arg);
                    string s = v.ToString(CultureInfo.InvariantCulture);
                    if (flags.IndexOf(',') >= 0)
                    {
                        s = v.ToString("N0", CultureInfo.InvariantCulture);
                    }
                    return ApplySign(s, v, flags);
                }
                case 'f':
                case 'F':
                {
                    double d = ToDouble(arg);
                    int p = (precision >= 0) ? precision : 6;
                    string s = d.ToString("F" + p, CultureInfo.InvariantCulture);
                    return ApplySign(s, d, flags);
                }
                case 'e':
                    return ApplySign(ToDouble(arg).ToString("e" + ((precision >= 0) ? precision : 6), CultureInfo.InvariantCulture), ToDouble(arg), flags);
                case 'E':
                    return ApplySign(ToDouble(arg).ToString("E" + ((precision >= 0) ? precision : 6), CultureInfo.InvariantCulture), ToDouble(arg), flags);
                case 'g':
                case 'G':
                {
                    double d = ToDouble(arg);
                    int p = (precision >= 0) ? precision : 6;
                    string s = d.ToString("G" + p, CultureInfo.InvariantCulture);
                    return ApplySign(s, d, flags);
                }
                case 'x':
                    return ToInt64(arg).ToString("x", CultureInfo.InvariantCulture);
                case 'X':
                    return ToInt64(arg).ToString("X", CultureInfo.InvariantCulture);
                case 'o':
                    return Convert.ToString(ToInt64(arg), 8);
                case 'c':
                    return (arg is char) ? arg.ToString() : ((char)ToInt64(arg)).ToString();
                case 'b':
                    return (arg is bool) ? (((bool)arg) ? "true" : "false") : ((arg == null) ? "false" : "true");
                case 's':
                {
                    string s = (arg == null) ? "null" : arg.ToString();
                    if (precision >= 0 && s.Length > precision)
                    {
                        s = s.Substring(0, precision);
                    }
                    return s;
                }
                default:
                    return (arg == null) ? "null" : arg.ToString();
            }
        }

        private static string ApplySign(string s, double value, string flags)
        {
            if (value >= 0)
            {
                if (flags.IndexOf('+') >= 0)
                {
                    return "+" + s;
                }
                if (flags.IndexOf(' ') >= 0)
                {
                    return " " + s;
                }
            }
            else if (flags.IndexOf('(') >= 0)
            {
                return "(" + s.Substring(1) + ")";
            }
            return s;
        }

        private static string ApplyWidth(string body, int width, string flags, char conv)
        {
            if (width <= 0 || body.Length >= width)
            {
                return body;
            }
            if (flags.IndexOf('-') >= 0)
            {
                return body.PadRight(width);
            }
            if (flags.IndexOf('0') >= 0 && IsNumeric(conv))
            {
                bool neg = body.StartsWith("-", StringComparison.Ordinal) || body.StartsWith("+", StringComparison.Ordinal) || body.StartsWith(" ", StringComparison.Ordinal);
                string sign = neg ? body.Substring(0, 1) : "";
                string digits = neg ? body.Substring(1) : body;
                return sign + digits.PadLeft(width - sign.Length, '0');
            }
            return body.PadLeft(width);
        }

        private static bool IsNumeric(char conv)
        {
            return conv == 'd' || conv == 'f' || conv == 'F' || conv == 'e' || conv == 'E' || conv == 'g' || conv == 'G';
        }

        private static long ToInt64(object arg)
        {
            if (arg == null)
            {
                return 0;
            }
            if (arg is char)
            {
                return (long)(char)arg;
            }
            if (arg is bool)
            {
                return ((bool)arg) ? 1 : 0;
            }
            return Convert.ToInt64(arg, CultureInfo.InvariantCulture);
        }

        private static double ToDouble(object arg)
        {
            if (arg == null)
            {
                return 0.0;
            }
            return Convert.ToDouble(arg, CultureInfo.InvariantCulture);
        }
    }
}
