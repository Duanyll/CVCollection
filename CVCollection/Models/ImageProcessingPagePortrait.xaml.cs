namespace CVCollection.Models;

[QueryProperty(nameof(ModelName), "modelName")]
public partial class ImageProcessingPagePortrait : ContentPage
{
    ImageProcessingViewModel viewModel;
    public string ModelName
    {
        set => BindingContext = viewModel = new ImageProcessingViewModel(value);
    }

    public ImageProcessingPagePortrait()
    {
        InitializeComponent();
        BindingContext = null;
    }
}