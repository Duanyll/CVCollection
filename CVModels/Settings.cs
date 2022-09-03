using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels
{
    public static class Settings
    {
        public static string ModelDownloadingHost { get; set; } = "http://localhost:8008";
        public static string RemoteProcessHost { get; set; } = "http://localhost:2003";
        public static string DownloadedModelFolder { get; set; }
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "models");
    }
}
