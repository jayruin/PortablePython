using System;
using System.Collections.Generic;
using System.Text;


namespace PortablePython
{
    class Config
    {
        public string PythonDownloadUrl { get; set; }
        public string PipDownloadUrl { get; set; }
        public string Version { get; set; }
        public List<string> Packages { get; set; }
    }
}
