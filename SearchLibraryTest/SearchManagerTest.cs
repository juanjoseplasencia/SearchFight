using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SearchLibrary;

namespace SearchLibraryTest
{
    [TestClass]
    public class SearchManagerTest : BaseTest
    {
        [TestMethod]
        public void Search_HappyPath()
        {
            SearchTerm = "java";
            SearchManager searchManager = new SearchManager();
            var result = searchManager.Search(SearchEnginesTestList, SearchTerm);

            Assert.IsTrue(result.Values.Count > 0, "There were no results from search.");
        }

        [TestMethod]
        public void Search_SearchEngineNames_Empty()
        {
            SearchEnginesTestList = new List<ISearchEngine>();
            SearchTerm = "java";
            SearchManager searchManager = new SearchManager();
            var result = searchManager.Search(SearchEnginesTestList, SearchTerm);
            
            Assert.IsFalse(result.Values.Count > 0, "There were results from search");
        }

        [TestMethod]
        public void Search_SearchTerm_Empty()
        {
            SearchTerm = string.Empty;
            SearchManager searchManager = new SearchManager();
            var result = searchManager.Search(SearchEnginesTestList, SearchTerm);

            Assert.IsFalse(result.Values.Any(V => V > 0), "There were results from search");
        }

        [TestMethod]
        public void Search_SearchTerm_Null()
        {
            SearchManager searchManager = new SearchManager();
            var result = searchManager.Search(SearchEnginesTestList, null);

            Assert.IsFalse(result.Values.Any(V => V > 0), "There were results from search");
        }

    }
}
