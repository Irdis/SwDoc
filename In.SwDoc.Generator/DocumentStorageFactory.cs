namespace In.SwDoc.Generator
{
    public static class DocumentStorageFactory
    {
        private static readonly object _lock = new object();
        private static volatile DocumentStorage _storage;

        public static DocumentStorage Get()
        {
            if (_storage != null)
                return _storage;
            lock (_lock)
            {
                if (_storage != null)
                    return _storage;
                return _storage = new DocumentStorage();
            }
        }

    }
}