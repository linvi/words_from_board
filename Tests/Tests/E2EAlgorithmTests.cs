using System;
using System.Diagnostics;
using System.Linq;
using Boggler.Helpers;
using Boggler.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Samples;

namespace Tests.Tests
{
    [TestClass]
    public class E2EAlgorithmTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var charactersBoardRetriever = new SampleBoardRetriever();
            var wordsDictionary = new SampleDictionaryOfWordsRetriever();
            var logger = new EmptyLogger();

            var boardAnalyzer = new DefaultBoardAnalyzer(wordsDictionary, charactersBoardRetriever, logger);

            boardAnalyzer.Initialize().Wait();

            var timer = new Stopwatch();

            timer.Start();

            var result = boardAnalyzer.Analyze().Result;

            timer.Stop();

            Debug.WriteLine($"Duration : {timer.ElapsedMilliseconds}ms");

            Assert.IsTrue(result.ContainsKey("geeks"));
            Assert.IsTrue(result.ContainsKey("quiz"));
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public void FromFileDataSet()
        {
            var charactersBoardRetriever = new SampleBoardRetriever();
            var wordsDictionary = new DictionaryOfWordsRetrieverFromFile("sample-dictionary-french.txt");
            var logger = new EmptyLogger();

            var boardAnalyzer = new DefaultBoardAnalyzer(wordsDictionary, charactersBoardRetriever, logger);

            boardAnalyzer.Initialize().Wait();

            var timer = new Stopwatch();

            timer.Start();

            var result = boardAnalyzer.Analyze().Result;

            timer.Stop();

            var detectedWords = result.OrderBy(x => x.Key).ToArray();

            Console.WriteLine($"Found {detectedWords.Length} words in {timer.ElapsedMilliseconds}ms");

            foreach (var detectedWord in detectedWords)
            {
                var coordinates = detectedWord.Value.Select(x => x.ToString());
                Console.WriteLine($"{detectedWord.Key} with path: {string.Join(" -> ", coordinates)}");
            }

            Assert.IsTrue(timer.ElapsedMilliseconds < 10000);
        }
    }
}
