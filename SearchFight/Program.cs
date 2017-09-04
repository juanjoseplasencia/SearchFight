using System;
using System.Collections.Generic;
using System.Linq;
using SearchLibrary;

namespace SearchFight
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> theProgLangs = args.ToList();
            List<string> searchEngineNames = new List<string>() { "Google", "Bing" };
            Console.WriteLine();
            Console.Write("The results of programming languages search fight are: ");
            Console.WriteLine();
            SearchManager searchManager = new SearchManager();
            var fullResults = searchManager.BuildResults(theProgLangs, searchEngineNames);
            if (fullResults.Count > 0)
            {
                ShowResults(fullResults);
                Console.WriteLine();
                ShowResultsBySearchEngine(searchManager.BuildResultsBySearchEngine(fullResults, searchEngineNames));
                ShowTotalWinner(searchManager.BuildTotalWinner(fullResults));
            }
            else { 
                Console.WriteLine();
                Console.WriteLine("No programming languages were provided for searching.");
            }
            Console.ReadLine();
        }

        private static void ShowResults(Dictionary<string, Dictionary<string, long>> results) {
            int total = results.Count;
            foreach (var result in results)
            {
                Console.Write(results.Count - total + 1);
                Console.Write(" ");
                Console.Write(result.Key);
                Console.Write(":");
                foreach (var value in result.Value)
                {
                    Console.Write(string.Format(" {0}: {1}", value.Key, value.Value));
                }
                total--;
                Console.WriteLine();
            }
        }

        private static void ShowResultsBySearchEngine(Dictionary<string, string> results)
        {
            foreach (var result in results) {
                Console.WriteLine(string.Format("{0} winner : {1}", result.Key, result.Value));
            }
        }

        private static void ShowTotalWinner(string winner)
        {
            if (!string.IsNullOrEmpty(winner))
                Console.WriteLine(string.Format("Total winner: {0}", winner));
        }

    }

}
