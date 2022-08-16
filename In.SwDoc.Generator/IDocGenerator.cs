using System.IO;

namespace In.SwDoc.Generator
{
    public interface IDocGenerator
    {
        Stream ConvertJsonToPdf(string data, bool openApi, AsciiFont? font);
    }
}