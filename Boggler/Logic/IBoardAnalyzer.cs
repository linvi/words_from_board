using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boggler.Models;

namespace Boggler.Logic
{
    public interface IBoardAnalyzer
    {
        event EventHandler<string> OnMatchDetected;

        Task Initialize();

        Task<IDictionary<string, Coordinates[]>> Analyze();
    }
}
