using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using log4net;
using log4net.Repository.Hierarchy;

namespace In.SwDoc.Generator
{
    public class DocGenerator
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(DocGenerator));
        private readonly string _swaggerCli = @"..\swaggercli\swagger2markup-cli-1.3.4-SNAPSHOT.jar";
        private readonly string _swaggerCli2 = @"..\swaggercli\swagger2markup-cli2-1.3.4-SNAPSHOT.jar";
        private readonly string _tempDirectory = @"..\swaggercli\temp";
        private readonly byte[] _newLine = Encoding.UTF8.GetBytes(Environment.NewLine);

        internal DocGenerator()
        {
            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }
        }

        public Stream ConvertJsonToAscii(string data, bool openApi)
        {
            _log.Info("Start converting to ascii doc");
            var jsonName = Guid.NewGuid().ToString("N");
            var asciiName = Guid.NewGuid().ToString("N");
            var jsonPath = Path.Combine(_tempDirectory, jsonName);
            var asciiPath = Path.Combine(_tempDirectory, asciiName);
            try
            {
                File.WriteAllText(jsonPath, data);

                ConverJsonToAscii(jsonPath, asciiPath, openApi);

                var memory = new MemoryStream();
                var files = ReorderFiles(Directory.GetFiles(asciiPath));
                foreach (var file in files)
                {
                    using (var stream = File.OpenRead(file))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Write(_newLine, 0, _newLine.Length);
                }
                Directory.Delete(asciiPath, true);
                File.Delete(jsonPath);
                memory.Position = 0;
                return memory;
            }
            catch (Exception e)
            {
                _log.Error("Unable to generate ascii document", e);
                throw new DocumentGenerationException("Unable to generate ascii document", e);
            }
        }

        private List<string> ReorderFiles(string[] files)
        {
            var result = new List<string>();
            var dict = files.ToDictionary(Path.GetFileName, s => s);
            ProcessFileItem("overview.adoc", dict, result);
            ProcessFileItem("security.adoc", dict, result);
            ProcessFileItem("paths.adoc", dict, result);
            ProcessFileItem("definitions.adoc", dict, result);
            result.AddRange(dict.Values);
            return result;
        }

        private static void ProcessFileItem(string fileName, Dictionary<string, string> dict, List<string> result)
        {
            string path;
            if (dict.TryGetValue(fileName, out path))
            {
                result.Add(path);
                dict.Remove(fileName);
            }
        }

        public Stream ConvertJsonToPdf(string data, bool openApi)
        {
            var adocName = Guid.NewGuid().ToString("N");
            var pdfName = Guid.NewGuid().ToString("N");
            var adocPath = Path.Combine(_tempDirectory, adocName);
            var pdfPath = Path.Combine(_tempDirectory, pdfName);
            try
            {
                using (var stream = ConvertJsonToAscii(data, openApi))
                using (var file = File.Create(adocPath))
                {
                    stream.CopyTo(file);
                }

                ConverAsciiToPdf(adocPath, pdfPath);

                File.Delete(adocPath);
                var memory = new MemoryStream();
                using (var pdf = File.OpenRead(pdfPath))
                {
                    pdf.CopyTo(memory);
                }

                File.Delete(pdfPath);
                memory.Position = 0;
                return memory;
            }
            catch (DocumentGenerationException)
            {
                throw;
            }
            catch (Exception e)
            {
                _log.Error("Unable to generate pdf document", e);
                throw new DocumentGenerationException("Unable to generate ascii document", e);
            }
        }

        public void ConverJsonToAscii(string jsonPath, string asciiPath, bool openApi)
        {
            string cmd;
            if (openApi)
            {
                cmd = $"/C java -jar \"{_swaggerCli}\" convert -i \"{jsonPath}\" -d \"{asciiPath}\" -v v3";
            }
            else
            {

                cmd = $"/C java -jar \"{_swaggerCli2}\" convert -i \"{jsonPath}\" -d \"{asciiPath}\"";
            }

            var process = new Process();
            var startInfo = new ProcessStartInfo();
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = cmd;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }


        public void ConverAsciiToPdf(string asciiPath, string pdfPath)
        {
            var cmd = $"/C asciidoctor-pdf -o \"{pdfPath}\" \"{asciiPath}\"";

            var process = new Process();
            var startInfo = new ProcessStartInfo();
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = cmd;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
