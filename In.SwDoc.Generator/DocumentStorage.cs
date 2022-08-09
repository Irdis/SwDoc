using System;
using System.IO;
using System.Threading;

namespace In.SwDoc.Generator
{
    public class DocumentStorage : IDocumentStorage
    {
        private readonly string _docDirectory = @"..\swaggercli\docs";
        private Timer _timer;
        public DocumentStorage()
        {
            if (!Directory.Exists(_docDirectory))
            {
                Directory.CreateDirectory(_docDirectory);
            }
            _timer = new Timer(OnTimer, null, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(20));
        }

        private void OnTimer(object state)
        {
            var files = Directory.GetFiles(_docDirectory);
            foreach (var file in files)
            {
                var time = File.GetCreationTime(file);
                if (time.Add(TimeSpan.FromDays(1)) < DateTime.Now)
                {
                    File.Delete(file);
                }
            }
        }

        public Stream GetDocument(string id)
        {
            var fileStream = File.OpenRead(Path.Combine(_docDirectory, id));
            return fileStream;
        }

        public string SaveDocument(Stream document)
        {
            var docName = Guid.NewGuid().ToString("N");
            var docPath = Path.Combine(_docDirectory, docName);
            using (var file = File.Create(docPath))
            {
                document.CopyTo(file);
            }
            return docName;
        }
    }
}