using System.Collections.Generic;
using System.Linq;

namespace Boggler.Models
{
    public class LetterDictionaryInfo
    {

        public LetterDictionaryInfo(HashSet<string> words)
        {
            Words = words;
            MinLength = words.Min(x => x.Length);
            MaxLength = words.Max(x => x.Length);
        }

        public HashSet<string> Words { get; }
        public int MaxLength { get; }
        public int MinLength { get; }
    }
}
