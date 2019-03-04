using System.Collections.Concurrent;

namespace Boggler.Models
{
    public class BoardAnalysisInfo
    {
        private char[][] _board;

        public char[][] Board
        {
            get { return _board; }
            set
            {
                if (value != null)
                {
                    var xSize = value.Length;
                    var ySize = value[0].Length;

                    BoardDimensions = new Dimensions(xSize, ySize);
                }
                else
                {
                    BoardDimensions = null;
                }

                _board = value;
            }
        }

        public Dimensions BoardDimensions { get; private set; }
        public LetterDictionaryInfo Dictionary { get; set; }
        public ConcurrentDictionary<string, Coordinates[]> DetectedWords { get; set; }
    }
}
