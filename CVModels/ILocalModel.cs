using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels
{
    public interface ILocalModel
    {
        string Name { get; }
        string ModelName { get; }
        Task UpdateExecutionProviderAsync(ExecutionProviderOptions executionProvider);
    }
}
