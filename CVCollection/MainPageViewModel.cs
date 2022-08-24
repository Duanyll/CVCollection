using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVModels;

namespace CVCollection
{
    public class ModelGroup : List<ModelInfo>
    {
        public string Name { get; set; }
        public ModelGroup(string name, IEnumerable<ModelInfo> models) : base(models)
        {
            Name = name;
        }
    }
    public class MainPageViewModel : BaseViewModel
    {
        public IEnumerable<ModelGroup> Groups { get; }
        public Command<ModelInfo> OpenModelCommand { get; }
        public MainPageViewModel()
        {
            Title = "CVCollection";
            Groups = ModelList.GetAllModels().GroupBy(i => i.ModelType switch
            {
                ModelIOType.ImageProcessing => "Image Processing",
                ModelIOType.ImageClassification => "Image Classification",
                ModelIOType.TargetDetection => "Image Target Detection",
                _ => "Other"
            }).Select(i => new ModelGroup(i.Key, i));
            OpenModelCommand = new Command<ModelInfo>(async info => {
                try
                {
                    if (info?.Type != null)
                    {
                        var navigationParam = new Dictionary<string, object>() { { "modelName", info.Name } };
                        await Shell.Current.GoToAsync(info.ModelType switch
                        {
                            ModelIOType.ImageProcessing => "models/imageProcessing",
                            _ => throw new NotImplementedException()
                        }, navigationParam);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                } catch (Exception ex)
                {
                    App.AlertSvc.ShowAlert("Oops", ex.Message);
                }
            });
        }
    }
}
