using System.Threading.Tasks;

namespace Boggler.Logic
{
    public interface IBoardRetriever
    {
        Task<char[][]> GetCharactersBoard();
    }
}
