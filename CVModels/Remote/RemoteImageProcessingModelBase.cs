using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonSubTypes;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace CVModels.Remote
{
    public abstract class RemoteImageProcessingModelBase : IImageProcessingModel
    {
        string BaseUrl => Settings.RemoteProcessHost;
        public string Name { get; }
        string RemoteModelName { get; }
        RestClient Client { get; }

        class BaseResponse
        {
            public virtual int Code { get; } = -1;
            public string Message { get; set; } = string.Empty;
        }

        class TokenData
        {
            public string Token { get; set; }
        }

        class SubmitResponse : BaseResponse
        {
            public override int Code => 0;
            public TokenData Data { get; set; }
        }

        class PositionData
        {
            public int Position { get; set;  }
        }

        [JsonConverter(typeof(JsonSubtypes), nameof(Code))]
        [JsonSubtypes.KnownSubType(typeof(Success), 0)]
        [JsonSubtypes.KnownSubType(typeof(Executing), 1)]
        [JsonSubtypes.KnownSubType(typeof(Waiting), 2)]
        [JsonSubtypes.KnownSubType(typeof(Error), 3)]
        class StatusResponse : BaseResponse
        {
            public class Success : StatusResponse
            {
                public override int Code => 0;
            }

            public class Executing : StatusResponse
            {
                public override int Code => 1;
            }

            public class Waiting : StatusResponse
            {
                public override int Code => 2;
                public PositionData Data { get; set;  }
            }

            public class Error : StatusResponse
            {
                public override int Code => 3;
                public string Data { get; set;  }
            }
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
            Client.UseNewtonsoftJson();
        }

        protected abstract Task<byte[]> OnPreprocessImage(byte[] image);

        public async Task<byte[]> PreprocessImageAsync(byte[] image)
        {
            return await OnPreprocessImage(image);
        }

        async Task<string> UploadImageAsync(byte[] image)
        {
            var request = new RestRequest("{model}/submit");
            request.AddUrlSegment("model", RemoteModelName);
            request.AddBody(SkiaSharpUtils.EncodeImageToJpeg(image), "image/jpeg");
            return (await Client.PostAsync<SubmitResponse>(request)).Data.Token;
        }

        async Task<StatusResponse> CheckStatusAsync(string token)
        {
            var request = new RestRequest("{model}/status");
            request.AddUrlSegment("model", RemoteModelName);
            request.AddJsonBody(new TokenData { Token = token });
            return (await Client.PostAsync<StatusResponse>(request));
        }

        async Task<byte[]> DownloadImageAsync(string token)
        {
            var request = new RestRequest("{model}/result");
            request.AddUrlSegment("model", RemoteModelName);
            request.AddJsonBody(new TokenData { Token = token });
            request.Method = Method.Post;
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
                    switch (status)
                    {
                        case StatusResponse.Success:
                            progress.Report("Downloading");
                            return await DownloadImageAsync(token);
                        case StatusResponse.Executing:
                            progress.Report("Processing");
                            await Task.Delay(1000);
                            break;
                        case StatusResponse.Waiting res:
                            progress.Report($"Waiting {res.Data.Position}");
                            await Task.Delay(3000);
                            break;
                        case StatusResponse.Error res:
                            throw new Exception(res.Message);
                        default:
                            throw new Exception("Unknown response code.");
                    }
                }
            } 
            catch (Exception ex)
            {
                throw new Exception($"Image remote processing failed, {ex.Message}", ex);
            }
        }
    }
}
