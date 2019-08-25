using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace In.SwDoc.Generator.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ILoggerRepository repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            var generator = new DocGenerator();
            var data = File.ReadAllText("test.json");
            var output = generator.ConvertJsonToPdf(data);
            Console.WriteLine(output);
        }
    }
}
