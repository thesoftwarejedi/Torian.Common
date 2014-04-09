using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Torian.Common.Logging;

namespace Torian.Common.Extensions
{

    public static class Tracing
    {

        public static void Trace(this Exception ex, string switchName, LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Torian.Common.Tracing.Trace.TraceInformation(switchName, "{0}: {1}", message, ex.ToString());
                    break;
                case LogLevel.Warn:
                    Torian.Common.Tracing.Trace.TraceWarning(switchName, "{0}: {1}", message, ex.ToString());
                    break;
                case LogLevel.Error:
                    Torian.Common.Tracing.Trace.TraceError(switchName, "{0}: {1}", message, ex.ToString());
                    break;
                default:
                    throw new Exception("not possible");
            }
        }

        public static void Trace(this Exception ex, string switchName, string message)
        {
            ex.Trace(switchName, LogLevel.Error, message);
        }

        public static void Trace(this Exception ex, string switchName, string message, params string[] parameters)
        {
            ex.Trace(switchName, string.Format(message, parameters));
        }

        public static void Trace(this Exception ex, string switchName, LogLevel level, string message, params string[] parameters)
        {
            ex.Trace(switchName, level, string.Format(message, parameters));
        }

    }

}
