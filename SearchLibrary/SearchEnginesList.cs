using System;
using System.Collections.Generic;

namespace SearchLibrary
{
    [Serializable]
    public class SearchEnginesList
    {
        public IList<ISearchEngine> SearchEngines { get; set; }
    }
}
