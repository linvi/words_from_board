using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Boggler.Helpers;
using Boggler.Logic;
using Boggler.Models;

namespace Boggle
{
    class Program
    {
        static void Main()
        {
            var lines = new List<char[]>();

            Console.WriteLine("Enter 1st line");
            var l1 = Console.ReadLine();
            lines.Add(GetLineAsCharArray(l1));

            Console.WriteLine("Enter 2nd line");
            var l2 = Console.ReadLine();
            lines.Add(GetLineAsCharArray(l2));

            Console.WriteLine("Enter 3rd line");
            var l3 = Console.ReadLine();
            lines.Add(GetLineAsCharArray(l3));

            Console.WriteLine("Enter 4th line");
            var l4 = Console.ReadLine();
            lines.Add(GetLineAsCharArray(l4));

            var boardAsCharArray = lines.ToArray();
            var board = new ConsoleBoardRetriever(boardAsCharArray);
            var wordsDictionary = new DictionaryOfWordsRetrieverFromFile("dictionary.txt");

            var logger = new EmptyLogger();

            var boardAnalyzer = new DefaultBoardAnalyzer(wordsDictionary, board, logger);

            boardAnalyzer.Initialize().Wait();

            var timer = new Stopwatch();
            timer.Start();

            var result = boardAnalyzer.Analyze().Result;

            timer.Stop();

            var detectedWords = result.OrderByDescending(x => x.Key.Length).ToArray();

            Console.WriteLine($"Found {detectedWords.Length} words in {timer.ElapsedMilliseconds}ms");

            foreach (var detectedWord in detectedWords)
            {
                var coordinates = detectedWord.Value.Select(x => x.ToString());
                Console.WriteLine($"{detectedWord.Key} with path: {string.Join(" -> ", coordinates)}");

                PrintBoardDetectedWords(boardAsCharArray, detectedWord.Key, detectedWord.Value);

                Console.WriteLine("Click to get next solution");
                Console.ReadKey();
            }
        }

        private static void PrintBoardDetectedWords(char[][] board, string word, Coordinates[] path)
        {
            var pathAsList = path.ToList();
            var boardXSize = board.Length;
            var boardYSize = board[0].Length;

            Console.WriteLine($"Found word {word} starting at {path[0]}");

            for (var x = 0; x < boardXSize; ++x)
            {
                Console.WriteLine("-----------------------------------------");

                for (var y = 0; y < boardYSize; ++y)
                {
                    var text = $"|   {board[x][y]}    ";

                    var matchingCoordinate = pathAsList.FirstOrDefault(coordinate => coordinate.X == x && coordinate.Y == y);
                    if (matchingCoordinate != null)
                    {
                        var index = pathAsList.IndexOf(matchingCoordinate);

                        text = $"| {index}  {char.ToUpper(board[x][y])}   ";
                    }

                    Console.Write(text);
                }

                Console.WriteLine();
            }
        }

        private static char[] GetLineAsCharArray(string line)
        {
            return line.Split(" ").Select(x => x[0]).ToArray();
        }
    }
}
