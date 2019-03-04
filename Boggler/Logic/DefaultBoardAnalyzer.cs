using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boggler.Models;

namespace Boggler.Logic
{
    public class DefaultBoardAnalyzer : IBoardAnalyzer
    {
        private readonly IDictionaryOfWordsRetriever _wordsRetriever;
        private readonly IBoardRetriever _boardRetriever;
        private readonly ILogger _logger;

        public event EventHandler<string> OnMatchDetected;

        public DefaultBoardAnalyzer(
            IDictionaryOfWordsRetriever wordsRetriever,
            IBoardRetriever boardRetriever,
            ILogger logger)
        {
            _wordsRetriever = wordsRetriever;
            _boardRetriever = boardRetriever;
            _logger = logger;
        }

        public async Task Initialize()
        {
            await _wordsRetriever.Populate();
        }

        public async Task<IDictionary<string, Coordinates[]>> Analyze()
        {
            var charactersBoard = await _boardRetriever.GetCharactersBoard();

            // TODO - Add logic th throw if all the lines do not have the same size
            // TODO - Add logic to throw if any of the cells are empty

            var uniqueFirstCharacters = charactersBoard.SelectMany(x => x).Distinct();
            var relevantDictionaries = _getDictionariesToUse(uniqueFirstCharacters);

            // TODO - Validation of no word containing only 1 char

            return await DetectWords(charactersBoard, relevantDictionaries);
        }

        private async Task<IDictionary<string, Coordinates[]>> DetectWords(char[][] board, Dictionary<char, LetterDictionaryInfo> wordsByFirstChar)
        {
            var xSize = board.Length;
            var ySize = board[0].Length;
            var detectedWords = new ConcurrentDictionary<string, Coordinates[]>();
            var tasks = new List<Task>();

            // For all starting element
            for (var x = 0; x < xSize; ++x)
            {
                for (var y = 0; y < ySize; ++y)
                {

                    var firstCharacter = board[x][y];
                    var wordsStartingWithCharacter = wordsByFirstChar[firstCharacter];

                    _logger.WriteLine($"Analyzing board[{x}][{y}] = {firstCharacter}...");

                    if (wordsStartingWithCharacter == null)
                    {
                        _logger.WriteLine($"The dictionary does not contain any word starting with character {firstCharacter}...");
                        continue;
                    }

                    var boardLetterInfo = new BoardAnalysisInfo
                    {
                        Board = board,
                        Dictionary = wordsStartingWithCharacter,
                        DetectedWords = detectedWords // TODO - This will become a bottleneck
                    };

                    var operationX = x;
                    var operationY = y;

                    var task = Task.Factory.StartNew(() =>
                    {
                        var coordinates = new List<Coordinates>();
                        AnalyzeRecursively(boardLetterInfo, new bool[xSize, ySize], "", new Coordinates(operationX, operationY), coordinates).Wait();
                    }, TaskCreationOptions.LongRunning);

                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks.ToArray());

            return detectedWords;
        }

        private async Task AnalyzeRecursively(BoardAnalysisInfo info, bool[,] visited, string currentWord, Coordinates currentCoordinates, List<Coordinates> path)
        {
            // Initialize information
            var coordinates = currentCoordinates;

            visited = (bool[,])visited.Clone();
            visited[coordinates.X, coordinates.Y] = true;

            path.Add(currentCoordinates);

            var cellCharacter = info.Board[currentCoordinates.X][currentCoordinates.Y];

            currentWord += cellCharacter;

            _logger.WriteLine($"Analyzing word {currentWord}");

            // Words Detection
            var hasCurrentWordAlreadyBeenMatched = info.DetectedWords.ContainsKey(currentWord);
            var isCurrentlyMatchingAWord = IsCurrentlyMatchingAWord(info, currentWord);

            if (isCurrentlyMatchingAWord && !hasCurrentWordAlreadyBeenMatched)
            {
                _logger.WriteLine($"Added word {currentWord}...");
                info.DetectedWords.TryAdd(currentWord, path.ToArray());
            }

            // Recursion
            await MoveTo(info, visited, currentWord, currentCoordinates, path);

            path.Remove(currentCoordinates);
        }

        private async Task MoveTo(BoardAnalysisInfo info, bool[,] visited, string currentWord, Coordinates currentCoordinates, List<Coordinates> path)
        {
            if (currentWord.Length >= info.Dictionary.MaxLength)
            {
                return;
            }

            var nextMoveCoordinates = GetNextCoordinates(info, visited, currentCoordinates);

            if (nextMoveCoordinates.Length == 0)
            {
                return;
            }

            foreach (var nextCoordinates in nextMoveCoordinates)
            {
                await AnalyzeRecursively(info, visited, currentWord, nextCoordinates, path);
            }
        }

        private Coordinates[] GetNextCoordinates(BoardAnalysisInfo info, bool[,] visited, Coordinates currentCoordinates)
        {
            var nextCoordinates = new List<Coordinates>();

            var startXPosition = Math.Max(0, currentCoordinates.X - 1);
            var maxXPosition = Math.Min(info.BoardDimensions.X - 1, currentCoordinates.X + 1); // BoardDimensions.X - 1 as array starts at 0

            var startYPosition = Math.Max(0, currentCoordinates.Y - 1);
            var maxYPosition = Math.Min(info.BoardDimensions.Y - 1, currentCoordinates.Y + 1); // BoardDimensions.Y + 1 as array starts at 0

            for (var x = startXPosition; x <= maxXPosition; ++x)
            {
                for (var y = startYPosition; y <= maxYPosition; ++y)
                {
                    var alreadyVisited = visited[x, y];

                    if (!alreadyVisited)
                    {
                        nextCoordinates.Add(new Coordinates(x, y));
                    }
                }
            }

            return nextCoordinates.ToArray();
        }

        private bool IsCurrentlyMatchingAWord(BoardAnalysisInfo info, string currentWord)
        {
            if (currentWord.Length < info.Dictionary.MinLength)
            {
                return false;
            }

            return info.Dictionary.Words.Contains(currentWord);
        }

        private Dictionary<char, LetterDictionaryInfo> _getDictionariesToUse(IEnumerable<char> firstCharacters)
        {
            return firstCharacters.ToDictionary(x => x, x =>
            {
                _wordsRetriever.WordsByFirstLetter.TryGetValue(x, out var dictionary);
                return dictionary;
            });
        }
    }
}
