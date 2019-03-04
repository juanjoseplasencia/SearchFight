using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace SearchLibrary
{
    public class SearchManager
    {
        /// <summary>
        /// Performs the search for a search term (programming language name) over the set of search engines
        /// </summary>
        /// <param name="searchEngines">Id of analysis</param>
        /// <param name="searchTerm">Cancellation token</param>
        /// <returns>Results of search by search engine</returns>
        public Dictionary<string, long> Search(List<ISearchEngine> searchEngines, string searchTerm)
        {
            Dictionary<string, long> results = new Dictionary<string, long>();
            if (searchEngines.Count > 0 && searchTerm != null) {
                foreach (var searchEngine in searchEngines)
                {
                    long result = searchEngine.Search(searchTerm);
                    results.Add(searchEngine.Name, result);
                }
            }
            return results;
        }

        /// <summary>
        /// Builds the list of search engine names to be used. It uses as input a set of "search engine names" from a json file
        /// </summary>
        /// <returns>List of search engine names</returns>
        public List<string> GetSearchEngineNames()
        {
            List<string> searchEngineNames;
            using (StreamReader r = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/SearchEngines.json"))
            {
                string json = r.ReadToEnd();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                searchEngineNames = serializer.Deserialize<List<string>>(json);
            }
            return searchEngineNames;
        }

        /// <summary>
        /// Builds the list of search engine objects from which the search method will be invoked.
        /// </summary>
        /// <returns>List of search engines</returns>
        public List<ISearchEngine> BuildSearchEnginesList(List<string> searchEngineNames) {
            const string SearchEngineBaseName = "SearchEngine";
            List<ISearchEngine> searchEngines = new List<ISearchEngine>();
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory + "SearchLibrary.dll";
            string RemoveSuffix (string value) => value.Remove(value.IndexOf(SearchEngineBaseName));

            Assembly assembly;
            assembly = Assembly.LoadFrom(assemblyPath);
            var searchEnginesFromSearchLibrary = assembly.DefinedTypes.Where(T => T.ImplementedInterfaces.Any() 
            && T.BaseType.Name == SearchEngineBaseName && searchEngineNames.Contains(RemoveSuffix(T.Name)));

            foreach (var searchEngineItem in searchEnginesFromSearchLibrary)
            {
                ISearchEngine searchEngine;
                searchEngine = (ISearchEngine)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assembly.FullName,
                   searchEngineItem.FullName);
                if (searchEngine != null)
                    searchEngines.Add(searchEngine);
            }
            return searchEngines;
        }

        /// <summary>
        /// Builds the list of search results for all the programming languages and search engines.
        /// </summary>
        /// <param name="progLangs">List of programming languages to search for</param>
        /// <param name="searchEngines">List of search engines to search with</param>
        /// <returns>Results of search by programming language</returns>
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

        /// <summary>
        /// Organizes the results of search by search engine to rank which programming language has the highest count of results and show it
        /// </summary>
        /// <param name="fullResults">Search results by programming language</param>
        /// <param name="searchEngines">List of search engines to search with</param>
        /// <returns>Results of search grouped by search engine</returns>
        public Dictionary<string,string> BuildResultsBySearchEngine(Dictionary<string, Dictionary<string, long>> fullResults,
            List<string> searchEngineNames) {
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (var item in searchEngineNames)
            {
                var resultsBySearchEngine = fullResults.Where(R => R.Value.Keys.Contains(item)).ToDictionary(R => R.Key, R => R.Value[item]);
                if (resultsBySearchEngine.Any()) { 
                    long languageWinner = resultsBySearchEngine.Values.Max();
                    results.Add(item, resultsBySearchEngine.First(R => R.Value == languageWinner).Key);
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
