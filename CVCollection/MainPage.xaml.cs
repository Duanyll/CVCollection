namespace CVCollection;

public partial class MainPage : ContentPage
{
	MainPageViewModel viewModel;
	public MainPage()
	{
		InitializeComponent();
		BindingContext = viewModel = new MainPageViewModel();
	}
}

