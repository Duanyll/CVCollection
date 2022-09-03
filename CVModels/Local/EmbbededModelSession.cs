using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CVModels.Local
{
    public enum ExecutionProviderOptions
    {
        CPU,
        Platform
    }

    internal class EmbbededModelSession : ILocalSession
    {
        byte[] _model;
        string _modelName;
        Task _prevAsyncTask;
        InferenceSession _session;
        ExecutionProviderOptions _curExecutionProvider;

        public EmbbededModelSession(string modelName)
        {
            _modelName = modelName;
        }

        public string ModelName => _modelName;
        public byte[] Model => _model;

        public InferenceSession Instance
        {
            get
            {
                AwaitLastTaskAsync().Wait();
                Initialize();
                return _session;
            }
        }

        public async Task UpdateExecutionProviderAsync(ExecutionProviderOptions executionProvider)
        {
            // make sure any existing async task completes before we change the session
            await AwaitLastTaskAsync();

            // creating the inference session can be expensive and should be done as a one-off.
            // additionally each session uses memory for the model and the infrastructure required to execute it,
            // and has its own threadpools.
            _prevAsyncTask = Task.Run(() =>
            {
                if (executionProvider == _curExecutionProvider)
                    return;

                if (executionProvider == ExecutionProviderOptions.CPU)
                {
                    // create session that uses the CPU execution provider
                    _session = new InferenceSession(_model);
                }
                else
                {
                    // create session that uses the NNAPI/CoreML. the CPU execution provider is also
                    // enabled by default to handle any parts of the model that NNAPI/CoreML cannot.
                    var options = SessionOptionsContainer.Create(nameof(ExecutionProviderOptions.Platform));
                    _session = new InferenceSession(_model, options);
                }
            });
        }

        public Task InitializeAsync(IProgress<int> progress = null)
        {
            _prevAsyncTask = Task.Run(() => Initialize());
            return _prevAsyncTask;
        }

        protected async Task AwaitLastTaskAsync()
        {
            if (_prevAsyncTask != null)
            {
                await _prevAsyncTask.ConfigureAwait(false);
                _prevAsyncTask = null;
            }
        }

        public void Initialize(IProgress<string> _ = null)
        {
            if (_session != null) return;
            _model = ResourceLoader.GetEmbeddedResource(ModelName);
            _session = new InferenceSession(_model);  // default to CPU 
            _curExecutionProvider = ExecutionProviderOptions.CPU;
        }
    }
}
