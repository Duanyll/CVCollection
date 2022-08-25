using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace CVModels.Remote
{
    public abstract class RemoteImageProcessingModelBase : IImageProcessingModel
    {
        const string BaseUrl = "http://localhost:8888";
        public string Name { get; }
        string RemoteModelName { get; }
        RestClient Client { get; }

        class BaseResponse<T> where T : class
        {
            public int Code { get; set; }
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = null;
            public BaseResponse<T> EnsureSuccess()
            {
                if (Code != 0)
                {
                    throw new Exception($"Code {Code}: {Message}");
                }
                return this;
            }
        }

        class SubmitResponse
        {
            public string Token { get; set; } = string.Empty;
        }
        
        class StatusRequest
        {
            public string Token { get; set; } = string.Empty;
        }

        class StatusResponse
        {
            public int Status { get; set; }
            public int Sequence { get; set; }
        }

        class ResultRequest
        {
            public string Token { get; set; } = string.Empty;
        }

        protected RemoteImageProcessingModelBase(string name, string remoteModelName)
        {
            Name = name;
            RemoteModelName = remoteModelName;
            Client = new RestClient(new RestClientOptions
            {
                BaseUrl = new Uri(BaseUrl),
                ThrowOnAnyError = true,
                MaxTimeout = 5000
            });
        }

        protected abstract Task<byte[]> OnPreprocessImage(byte[] image);

        public async Task<byte[]> PreprocessImageAsync(byte[] image)
        {
            return await OnPreprocessImage(image);
        }

        async Task<string> UploadImageAsync(byte[] image)
        {
            var request = new RestRequest("submit/{model}");
            request.AddUrlSegment("model", RemoteModelName);
            request.AddBody(image, "image/png");
            return (await Client.PostAsync<BaseResponse<SubmitResponse>>(request)).EnsureSuccess().Data.Token;
        }

        async Task<StatusResponse> CheckStatusAsync(string token)
        {
            var request = new RestRequest("status");
            request.AddJsonBody(new StatusRequest { Token = token });
            return (await Client.GetAsync<BaseResponse<StatusResponse>>(request)).EnsureSuccess().Data;
        }

        async Task<byte[]> DownloadImageAsync(string token)
        {
            var request = new RestRequest("result");
            request.AddJsonBody(new ResultRequest { Token = token });
            return await Client.DownloadDataAsync(request);
        }

        public async Task<byte[]> ProcessImageAsync(byte[] prepocessedImage, IProgress<string> progress = null)
        {
            try
            {
                progress.Report("Uploading");
                var token = await UploadImageAsync(prepocessedImage);
                await Task.Delay(1000);
                while (true)
                {
                    var status = await CheckStatusAsync(token);
                    if (status.Status != 0)
                    {
                        break;
                    }
                    if (status.Sequence > 0)
                    {
                        progress.Report($"Pending, {status.Sequence} ahead.");
                    }
                    else
                    {
                        progress.Report($"Processing");
                    }
                    await Task.Delay(3000);
                }

                progress.Report("Downloading");
                return await DownloadImageAsync(token);
            } 
            catch (Exception ex)
            {
                throw new Exception($"Image remote processing failed, {ex.Message}", ex);
            }
        }
    }
}
