using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SearchLibrary;

namespace SearchLibraryTest 
{
    public class BaseTest
    {
        public TestContext TestContext { get; set; }
        public  List<ISearchEngine> SearchEnginesTestList { get; set; }
        public string SearchTerm { get; set; }

        public  void BuildSearchEnginesTestList() {
            SearchEnginesTestList = new List<ISearchEngine>();
            SearchEnginesTestList.Add(new GoogleSearchEngine("Google", @"http://www.google.com/search?q="));
            SearchEnginesTestList.Add(new BingSearchEngine("Bing", @"http://www.bing.com/search?q="));
        }

        [TestInitialize]
        public void TestInitialize() {
            BuildSearchEnginesTestList();
        }
    }
}
