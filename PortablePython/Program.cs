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
            Scribe.Provision(true, "output.txt");
            try
            {
                Installer installer = new Installer(JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json")));
                installer.Install();
            }
            catch (Exception e)
            {
                Scribe.WriteLine(e.Message);
            }
        }
    }
}
