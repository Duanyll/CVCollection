using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;

namespace CVModels.Local
{
    internal interface ILocalSession
    {
        void Initialize(IProgress<string> progress = null);
        InferenceSession Instance { get; }
    }
}
