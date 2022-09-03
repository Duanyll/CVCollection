using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModels
{
    public enum ModelIOType
    {
        ImageProcessing,
        ImageClassification,
        TargetDetection
    }
    public class ModelInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ModelIOType ModelType { get; set; }
        public bool IsLocal { get; set; }
        public string Description { get; set; }
        public string SampleImage { get; set; } = null;
        public Type Type { get; set; } = null;
    }
    public static class ModelList
    {
        static Dictionary<string, ModelInfo> info = new()
        {
            {
                "Derain", 
                new ModelInfo() 
                {
                    Name = "Derain",
                    DisplayName = "Image Deraining",
                    ModelType = ModelIOType.ImageProcessing,
                    IsLocal = true,
                    Description = "Removes rain streaks from image",
                    SampleImage = "derain.jpg",
                    Type = typeof(Local.Derain)
                }
            },
            {
                "Dehaze",
                new ModelInfo()
                {
                    Name = "Dehaze",
                    DisplayName = "Image Dehazing",
                    ModelType = ModelIOType.ImageProcessing,
                    IsLocal = false,
                    Description = "Remove haze from image",
                    SampleImage = "msbdn_dff.png",
                    Type = typeof(Local.MsbdnDff)
                }
            },
            {
                "Restore",
                new ModelInfo()
                {
                    Name = "Restore",
                    DisplayName = "Image Restoration",
                    ModelType = ModelIOType.ImageProcessing,
                    IsLocal = false,
                    Description = "Lorem ipsum."
                }
            },
            {
                "Enhance",
                new ModelInfo()
                {
                    Name = "Enhance",
                    DisplayName = "Image Enhancement",
                    ModelType = ModelIOType.ImageProcessing,
                    IsLocal = false,
                    Description = "Lorem ipsum.",
                    SampleImage = "zero_dce.jpg",
                    Type = typeof(Local.ZeroDce)
                }
            },
            {
                "Demoire",
                new ModelInfo()
                {
                    Name = "Demoire",
                    DisplayName = "Image Demoireing",
                    ModelType = ModelIOType.ImageProcessing,
                    IsLocal = false,
                    Description = "Lorem ipsum.",
                    SampleImage = "demoire.jpg",
                    Type = typeof(Remote.Demoire)
                }
            },

            {
                "ResNet",
                new ModelInfo()
                {
                    Name = "ResNet",
                    DisplayName = "ImageNet Classification",
                    ModelType = ModelIOType.ImageClassification,
                    IsLocal = true,
                    Description = "Classification with ResNet"
                }
            },

            {
                "Ultraface",
                new ModelInfo()
                {
                    Name = "Ultraface",
                    DisplayName = "Face Recognition",
                    ModelType = ModelIOType.TargetDetection,
                    IsLocal = true,
                    Description = "Fast Face Recognition with Ultraface"
                }
            }
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
