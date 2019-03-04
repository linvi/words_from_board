using System.Threading.Tasks;
using Boggler;

namespace Tests.Samples
{
    public class SampleDictionaryOfWordsRetriever : DictionaryOfWordsRetriever
    {
        public override Task Populate()
        {
            var words = new[]
            {
                "geeks",
                "for",
                "quiz",
                "go"
            };

            _buildWordsMetadata(words);

            return Task.CompletedTask;
        }
    }
}
