using System.Threading.Tasks;
using Boggler.Logic;

namespace Boggle
{
    public class ConsoleBoardRetriever : IBoardRetriever
    {
        private readonly char[][] _boardCharacters;

        public ConsoleBoardRetriever(char[][] boardCharacters)
        {
            _boardCharacters = boardCharacters;
        }

        public Task<char[][]> GetCharactersBoard()
        {
            foreach (var row in _boardCharacters)
            {
                for (var x = 0; x < row.Length; ++x)
                {
                    var cell = row[x];
                    row[x] = char.ToLowerInvariant(cell);
                }
            }

            return Task.FromResult(_boardCharacters);
        }
    }
}
