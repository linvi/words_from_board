using Boggler.Logic;

namespace Boggler.Helpers
{
    public class EmptyLogger : ILogger
    {
        public void WriteLine(string content)
        {
        }
    }
}
