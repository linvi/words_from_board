using System.IO;
using System.Threading.Tasks;

namespace Boggler.Logic
{
    public class DictionaryOfWordsRetrieverFromFile : DictionaryOfWordsRetriever
    {
        private readonly string _filePath;

        public DictionaryOfWordsRetrieverFromFile(string filePath)
        {
            _filePath = filePath;
        }

        public override Task Populate()
        {
            var words = File.ReadAllLines(_filePath);

            _buildWordsMetadata(words);

            return Task.CompletedTask;
        }
    }
}
