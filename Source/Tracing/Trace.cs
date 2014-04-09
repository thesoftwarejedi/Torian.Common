using System;
using System.Collections.Generic;
using Torian.Common.Logging;

namespace Torian.Common.Tracing
{

    public static class Trace
    {

        private static readonly Dictionary<string, System.Diagnostics.TraceSwitch> Switches = new Dictionary<string, System.Diagnostics.TraceSwitch>();

        public static void TraceInformation(string switchName, string format, params object[] args)
        {
            TraceInformation(switchName, string.Format(format, args));
        }

        public static void TraceInformation(string switchName, string message)
        {
            message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + message;
            System.Diagnostics.TraceSwitch mySwitch = LookupSwitch(switchName);
            System.Diagnostics.Trace.WriteLineIf(mySwitch.TraceInfo, message, switchName);
        }

        public static void TraceWarning(string switchName, string format, params object[] args)
        {
            TraceWarning(switchName, string.Format(format, args));
        }

        private static void TraceWarning(string switchName, string message)
        {
            message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + message;
            System.Diagnostics.TraceSwitch mySwitch = LookupSwitch(switchName);
            System.Diagnostics.Trace.WriteLineIf(mySwitch.TraceWarning, message, switchName);
        }

        public static void TraceError(string switchName, string format, params object[] args)
        {
            TraceError(switchName, string.Format(format, args));
        }

        private static void TraceError(string switchName, string message)
        {
            string newMessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + message;
            System.Diagnostics.TraceSwitch mySwitch = LookupSwitch(switchName);
            System.Diagnostics.Trace.WriteLineIf(mySwitch.TraceError, newMessage, switchName);
            if (mySwitch.TraceError)
            {
                Log.LogMessage(switchName, LogLevel.Error, message);
            }
        }

        private static System.Diagnostics.TraceSwitch LookupSwitch(string switchName)
        {
            System.Diagnostics.TraceSwitch mySwitch;
            if (Switches.TryGetValue(switchName, out mySwitch)) return mySwitch;
            //late locking strategy for performance
            lock (Switches)
            {
                //have to look again since late lock used
                if (Switches.TryGetValue(switchName, out mySwitch)) return mySwitch;
                mySwitch = new System.Diagnostics.TraceSwitch(switchName, switchName);
                Switches[switchName] = mySwitch;
            }
            return mySwitch;
        }

    }

}
