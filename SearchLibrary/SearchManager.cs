using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace SearchLibrary
{
    public class SearchManager
    {
        /// <summary>
        /// Performs the search for a search term (programming language) over the set of search Engines
        /// </summary>
        /// <param name="searchEngines">Id of analysis</param>
        /// <param name="searchTerm">Cancellation token</param>
        /// <returns>Results of search by search engine</returns>
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

        /// <summary>
        /// Builds the list of searchEngine objects from which the search operation will be invoked. It uses as input a set of "search engine definitions" from a json file
        /// </summary>
        /// <returns>List of search engines</returns>
        public List<ISearchEngine> BuildSearchEnginesList() {
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

        /// <summary>
        /// Builds the list of search results for all the programming languages and search engines.
        /// </summary>
        /// <param name="progLangs">List of programming languages to search for</param>
        /// <param name="searchEngines">List of search engines to search with</param>
        /// <returns>Results of search by programming language</returns>
        public Dictionary<string, Dictionary<string, long>> BuildResults(List<string> progLangs, List<ISearchEngine> searchEngines) {
            Dictionary<string, Dictionary<string, long>> fullResults = new Dictionary<string, Dictionary<string, long>>();
            int total = progLangs.Count;
            foreach (var progLang in progLangs)
            {
                var results = Search(searchEngines, progLang);
                fullResults.Add(progLang, results);
            }
            return fullResults;
        }

        /// <summary>
        /// Organizes the results of search by search engine to rank which programming language has more results and show it
        /// </summary>
        /// <param name="fullResults">Search results by programming language</param>
        /// <param name="searchEngines">List of search engines to search with</param>
        /// <returns>Results of search grouped by search engine</returns>
        public Dictionary<string,string> BuildResultsBySearchEngine(Dictionary<string, Dictionary<string, long>> fullResults,
            List<ISearchEngine> searchEngines) {
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (var item in searchEngines)
            {
                var resultsBySearchEngine = fullResults.Where(R => R.Value.Keys.Contains(item.Name)).ToDictionary(R => R.Key, R => R.Value[item.Name]);
                if (resultsBySearchEngine.Any()) { 
                    long languageWinner = resultsBySearchEngine.Values.Max();
                    results.Add(item.Name, resultsBySearchEngine.First(R => R.Value == languageWinner).Key);
                }
            }
            return results;
        }

        /// <summary>
        /// Gets the programming language on the top of count of search results
        /// </summary>
        /// <param name="fullResults">Search results by programming language</param>
        /// <returns>Results of search by search engine</returns>
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
