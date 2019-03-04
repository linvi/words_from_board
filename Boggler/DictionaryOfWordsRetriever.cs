using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boggler.Logic;
using Boggler.Models;

namespace Boggler
{
    public abstract class DictionaryOfWordsRetriever : IDictionaryOfWordsRetriever
    {
        public HashSet<string> AllWords { get; private set; }

        public Dictionary<char, LetterDictionaryInfo> WordsByFirstLetter { get; private set; }

        public abstract Task Populate();

        protected void _buildWordsMetadata(string[] words)
        {
            words = words.Select(x => x.ToLowerInvariant()).ToArray();

            AllWords = new HashSet<string>(words);
            WordsByFirstLetter = words.GroupBy(x => x[0]).ToDictionary(x => x.Key, x =>
            {
                return new LetterDictionaryInfo(new HashSet<string>(x));
            });
        }
    }
}
