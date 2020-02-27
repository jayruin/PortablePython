using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PortablePython
{
    public static class Scribe
    {
        public static void Provision(bool console, params string[] parchments)
        {
            Trace.Listeners.Clear();
            Trace.AutoFlush = true;
            if (console)
            {
                Trace.Listeners.Add(new ConsoleTraceListener());
            }
            foreach (string parchment in parchments)
            {
                Trace.Listeners.Add(new TextWriterTraceListener(parchment));
            }
        }

        public static void Write(string message)
        {
            Trace.Write(message);
        }

        public static void WriteLine(string message)
        {
            Trace.WriteLine(message);
        }
    }
}