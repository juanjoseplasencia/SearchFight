﻿using System;
using System.Net.Http;

namespace SearchLibrary
{
    [Serializable]
    public class GoogleSearchEngine : SearchEngine, ISearchEngine
    {
        public GoogleSearchEngine(string name, string url) : base(name, url)
        {
        }
        public GoogleSearchEngine() : this("Google", "http://www.google.com/search?q=")
        {
        }

        public override long Search(string searchTerm)
        {
            long resultsAsLong = 0;
            if (!string.IsNullOrEmpty(searchTerm)) {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(Url + searchTerm)
                };
                HttpResponseMessage response = client.GetAsync(Url + searchTerm).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    int keyPositionBefore = result.IndexOf("\"resultStats\">Cerca de ") == -1 ? result.IndexOf("\"resultStats\">About ") + 20 : result.IndexOf("\"resultStats\">Cerca de ") + 23;
                    string remainingPart = result.Substring(keyPositionBefore);
                    int keyPositionAfter = remainingPart.IndexOf(" resultados") == -1 ? remainingPart.IndexOf(" results") : remainingPart.IndexOf(" resultados");
                    string resultsCounter = remainingPart.Substring(0, keyPositionAfter);
                    resultsAsLong = Convert.ToInt64(resultsCounter
                        .Replace(",", string.Empty)
                        .Replace(".", string.Empty));
                }
            }
            return resultsAsLong;
        }
    }
}
