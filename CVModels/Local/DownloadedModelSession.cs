using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net;

namespace CVModels.Local
{
    internal class DownloadedModelSession : ILocalSession
    {
        string fileName = string.Empty;
        string md5 = null;

        public DownloadedModelSession(string fileName, string md5 = null)
        {
            this.fileName = fileName;
            this.md5 = md5;
        }

        InferenceSession _session;
        public InferenceSession Instance
        {
            get
            {
                Initialize();
                return _session;
            }
        }

        bool ValidateFileHash(byte[] bytes)
        {
            if (md5 != null)
            {
                using var algo = MD5.Create();
                var result = BitConverter.ToString(algo.ComputeHash(bytes)).Replace("-", "").ToUpperInvariant();
                return result == md5;
            }
            else
            {
                return true;
            }
        }

        public void Initialize(IProgress<string> progress = null)
        {
            if (_session == null)
            {
                Directory.CreateDirectory(Settings.DownloadedModelFolder);
                var localFile = Path.Combine(Settings.DownloadedModelFolder, fileName);
                byte[] model = null;
                if (File.Exists(localFile))
                {
                    model = File.ReadAllBytes(localFile);
                    if (ValidateFileHash(model))
                    {
                        _session = new InferenceSession(model);
                        return;
                    }
                }
                progress.Report("Downloading");
#pragma warning disable SYSLIB0014 // 类型或成员已过时
                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) => progress.Report($"Downloading {e.ProgressPercentage}%");
                    client.DownloadFileTaskAsync(new Uri(new Uri(Settings.ModelDownloadingHost), fileName), localFile).Wait();
                }
#pragma warning restore SYSLIB0014 // 类型或成员已过时
                model = File.ReadAllBytes(localFile);
                if (ValidateFileHash(model))
                {
                    _session = new InferenceSession(model);
                } 
                else
                {
                    throw new Exception($"Downloaded model checksum error");
                }
            }
        }
    }
}
