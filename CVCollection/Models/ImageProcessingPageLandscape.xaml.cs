namespace CVCollection.Models;

[QueryProperty(nameof(ModelName), "modelName")]
public partial class ImageProcessingPageLandscape : ContentPage
{
    ImageProcessingViewModel viewModel;
    public string ModelName
    {
        set => BindingContext = viewModel = new ImageProcessingViewModel(value);
    }

    public ImageProcessingPageLandscape()
    {
        InitializeComponent();
        BindingContext = null;
    }
}