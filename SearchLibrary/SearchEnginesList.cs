using System;
using System.Collections.Generic;

namespace SearchLibrary
{
    [Serializable]
    public class SearchEnginesList
    {
        public IList<SearchEngine> SearchEngines { get; set; }
    }
}
