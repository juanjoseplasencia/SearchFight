using System;

namespace SearchLibrary
{
    [Serializable]
    public class SearchEngine :  ISearchEngine
    {
        public SearchEngine() { }
        public string Name { get; }
        public string Url { get; }
        public SearchEngine(string engineName, string url)
        {
            Name = engineName;
            Url = url;
        }
        public virtual long Search(string searchTerm) {
            return 0;
        }
    }



}
