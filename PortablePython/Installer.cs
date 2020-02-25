using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;


namespace PortablePython
{
    class Installer
    {
        private readonly Config _config;

        private readonly string _pythonZip;

        private readonly string _pythonDirectory;

        private readonly string _pipInstaller;

        private readonly string _pythonExe;

        private readonly string _requirementsFile;

        public Installer(Config config)
        {
            _config = config;
            _pythonDirectory = "Python";
            _pythonZip = "python.zip";
            _pipInstaller = Path.Combine(_pythonDirectory, "get-pip.py");
            _pythonExe = Path.Combine(_pythonDirectory, "python.exe");
            _requirementsFile = "requirements.txt";
        }

        public void Install()
        {
            Setup();
            DownloadPython();
            GetPip();
            InstallPackages();
            Cleanup();
        }

        private void Setup()
        {
            DeleteDirectoryIfExists(_pythonDirectory);
            DeleteFileIfExists(_pythonZip);
            DeleteFileIfExists(_requirementsFile);
            Directory.CreateDirectory(_pythonDirectory);
        }

        private void DownloadPython()
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(String.Format(_config.PythonDownloadUrl, _config.Version), _pythonZip);
            }
            using (ZipArchive archive = ZipFile.OpenRead(_pythonZip))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine(_pythonDirectory, entry.FullName), true);
                }
            }
        }

        private void GetPip()
        {
            string pthFile = FindFileByExtension(_pythonDirectory, "._pth");
            string content = File.ReadAllText(pthFile);
            content = content.Replace("#import site", "import site");
            File.WriteAllText(pthFile, content);
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(_config.PipDownloadUrl, _pipInstaller);
            }
            ExecutePython($"{_pipInstaller} --no-warn-script-location");
        }

        private void InstallPackages()
        {
            File.WriteAllText(_requirementsFile, String.Join(Environment.NewLine, _config.Packages));
            ExecutePython($"-m pip install -r {_requirementsFile} --no-warn-script-location");
        }

        private void Cleanup()
        {
            DeleteFileIfExists(_pythonZip);
            DeleteFileIfExists(_pipInstaller);
            DeleteFileIfExists(_requirementsFile);
        }

        private string FindFileByExtension(string directory, string extension)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                if (file.EndsWith(extension))
                {
                    return file;
                }
            }
            return "";
        }

        private void DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void DeleteDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private void ExecutePython(string arguments)
        {
            using (Process pythonInterpreter = new Process())
            {
                pythonInterpreter.StartInfo.FileName = _pythonExe;
                pythonInterpreter.StartInfo.Arguments = arguments;
                pythonInterpreter.StartInfo.UseShellExecute = false;
                pythonInterpreter.StartInfo.RedirectStandardOutput = true;
                pythonInterpreter.StartInfo.RedirectStandardError = true;
                pythonInterpreter.OutputDataReceived += (sender, e) => Trace.WriteLine(e.Data);
                pythonInterpreter.ErrorDataReceived += (sender, e) => Trace.WriteLine(e.Data);
                pythonInterpreter.Start();
                pythonInterpreter.BeginOutputReadLine();
                pythonInterpreter.BeginErrorReadLine();
                pythonInterpreter.WaitForExit();
            }
        }
    }
}
