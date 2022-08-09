using System.IO;

namespace In.SwDoc.Generator
{
    public interface IDocumentStorage
    {
        Stream GetDocument(string id);
        string SaveDocument(Stream document);
    }
}