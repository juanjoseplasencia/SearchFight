using System;

namespace SearchLibrary
{
    [Serializable]
    public class SearchEngine :  ISearchEngine
    {
        public SearchEngine() { }
        public string Name { get; set; }
        public string Url { get; set; }
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
