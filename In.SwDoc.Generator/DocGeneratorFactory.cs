namespace In.SwDoc.Generator
{
    public class DocGeneratorFactory
    {
        private static readonly object _lock = new object();
        private static volatile DocGenerator _generator;

        public static DocGenerator Get()
        {
            if (_generator != null)
            {
                return _generator;
            }

            lock (_lock)
            {
                if (_generator != null)
                {
                    return _generator;
                }
                return _generator = new DocGenerator();
            }
        }
    }
}