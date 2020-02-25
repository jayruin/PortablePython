using System;
using System.Diagnostics;
using System.Text.Json;
using System.IO;

namespace PortablePython
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Clear();
            Trace.AutoFlush = true;
            Trace.Listeners.Add(new TextWriterTraceListener("output.txt"));
            Trace.Listeners.Add(new ConsoleTraceListener());
            Installer installer = new Installer(JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json")));
            installer.Install();
        }
    }
}
