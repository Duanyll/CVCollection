namespace CVCollection;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("models/imageProcessing", typeof(Models.ImageProcessingPageLandscape));
	}
}
