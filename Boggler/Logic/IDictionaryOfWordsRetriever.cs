using System.Collections.Generic;
using System.Threading.Tasks;
using Boggler.Models;

namespace Boggler.Logic
{
    public interface IDictionaryOfWordsRetriever
    {
        HashSet<string> AllWords { get; }
        Dictionary<char, LetterDictionaryInfo> WordsByFirstLetter { get; }
        Task Populate();
    }
}
