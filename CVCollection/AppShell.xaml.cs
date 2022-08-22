namespace CVCollection;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		if (DeviceInfo.Idiom == DeviceIdiom.Phone)
		{
			Routing.RegisterRoute("models/imageProcessing", typeof(Models.ImageProcessingPagePortrait));
		} 
		else
		{
            Routing.RegisterRoute("models/imageProcessing", typeof(Models.ImageProcessingPageLandscape));
        }
    }
}
