﻿using System;
using System.Net.Http;

namespace SearchLibrary
{
    [Serializable]
    public class BingSearchEngine : SearchEngine, ISearchEngine
    {
        public BingSearchEngine(string name, string url) : base(name,url)
        {
        }

        public BingSearchEngine() : this("Bing", "http://www.bing.com/search?q=")
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
                    int keyPositionBefore = result.IndexOf("class=\"sb_count\">") + 17;
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
