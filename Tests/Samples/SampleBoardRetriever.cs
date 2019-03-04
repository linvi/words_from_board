using System.Threading.Tasks;
using Boggler.Logic;

namespace Tests.Samples
{
    public class SampleBoardRetriever : IBoardRetriever
    {
        public Task<char[][]> GetCharactersBoard()
        {
            var board = new[]
            {
                new [] { 'T', 'I', 'C', 'A' },
                new [] { 'Q', 'G', 'E', 'B' },
                new [] { 'U', 'S', 'E', 'I' },
                new [] { 'E', 'T', 'E', 'T' }
            };

            foreach (var row in board)
            {
                for (var x = 0; x < row.Length; ++x)
                {
                    var cell = row[x];
                    row[x] = char.ToLowerInvariant(cell);
                }
            }

            return Task.FromResult(board);
        }
    }
}
