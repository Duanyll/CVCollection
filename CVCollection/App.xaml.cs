using CVCollection.Service;

namespace CVCollection;

public partial class App : Application
{
    public static IServiceProvider Services;
    public static IAlertService AlertSvc;

    public App(IServiceProvider provider)
	{
		InitializeComponent();

        Services = provider;
        AlertSvc = Services.GetService<IAlertService>();

        MainPage = new AppShell();
	}
}
