using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace SearchLibrary
{
    public class SearchManager
    {
        public Dictionary<string, long> Search(List<ISearchEngine> searchEngines, string searchTerm)
        {
            Dictionary<string, long> results = new Dictionary<string, long>();
            if (searchEngines.Count > 0) { 
                foreach (var searchEngine in searchEngines)
                {
                    long result = searchEngine.Search(searchTerm);
                    results.Add(searchEngine.Name, result);
                }
            }
            return results;
        }

        public List<ISearchEngine> BuildSearchEnginesList(List<string> searchEngineNames) {
            List<ISearchEngine> searchEngines = new List<ISearchEngine>();
            using (StreamReader r = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/SearchEngines.json"))
            {
                string json = r.ReadToEnd();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                SearchEnginesList searchEnginesList = serializer.Deserialize<SearchEnginesList>(json);

                foreach (var searchEngineItem in searchEnginesList.SearchEngines)
                {
                    ISearchEngine searchEngine = null;
                    switch (searchEngineItem.Name)
                    {
                        case "Google":
                            searchEngine = new GoogleSearchEngine(searchEngineItem.Name, searchEngineItem.Url);
                            break;
                        case "Bing":
                            searchEngine = new BingSearchEngine(searchEngineItem.Name, searchEngineItem.Url);
                            break;
                    }
                    if (searchEngine != null)
                        searchEngines.Add(searchEngine);
                }

            }
            return searchEngines;
        }

        public Dictionary<string, Dictionary<string, long>> BuildResults(List<string> progLangs, List<string> searchEngineNames) {
            Dictionary<string, Dictionary<string, long>> fullResults = new Dictionary<string, Dictionary<string, long>>();
            int total = progLangs.Count;
            var searchEngines = BuildSearchEnginesList(searchEngineNames);
            foreach (var progLang in progLangs)
            {
                var results = Search(searchEngines, progLang);
                fullResults.Add(progLang, results);
            }
            return fullResults;
        }

        public Dictionary<string,string> BuildResultsBySearchEngine(Dictionary<string, Dictionary<string, long>> fullResults,
            List<string> searchEngineNames)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (var name in searchEngineNames)
            {
                var resultsBySearchEngine = fullResults.Where(R => R.Value.Keys.Contains(name)).ToDictionary(R => R.Key, R => R.Value[name]);
                if (resultsBySearchEngine.Any()) { 
                    long languageWinner = resultsBySearchEngine.Values.Max();
                    results.Add(name, resultsBySearchEngine.First(R => R.Value == languageWinner).Key);
                }
            }
            return results;
        }

        public string BuildTotalWinner(Dictionary<string, Dictionary<string, long>> fullResults)
        {
            string winner = string.Empty;
            var resultsByProgLang = fullResults.ToDictionary(R => R.Key, R => R.Value.Values.Max<long>());
            if (resultsByProgLang.Any()) { 
                long totalWinner = resultsByProgLang.Values.Max();
                winner = resultsByProgLang.First(R => R.Value == totalWinner).Key;
            }
            return winner;
        }

    }
}
