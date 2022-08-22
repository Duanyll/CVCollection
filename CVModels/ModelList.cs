using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels
{
    public enum ModelType
    {
        ImageProcessing
    }
    public class ModelInfo
    {
        public string Name { get; set; }
        public ModelType ModelType { get; set; }
        public bool IsLocal { get; set; }
        public string Description { get; set; }
        public string SampleImage { get; set; }
        public Type Type { get; set; }
    }
    public static class Models
    {
        static Dictionary<string, ModelInfo> info = new()
        {
            {"Derain", new ModelInfo() {
                Name = "Derain",
                ModelType = ModelType.ImageProcessing,
                IsLocal = true,
                Description = "Single Image Deraining via CNN",
                SampleImage = "derain.jpg",
                Type = typeof(LocalModels.Derain)
            }}
        };

        public static IEnumerable<ModelInfo> GetAllModels()
        {
            return info.Values;
        }

        public static ModelInfo GetModelInfo(string name)
        {
            return info[name];
        }
    }
}
