namespace prototipoperguntasMaui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		// Register native route for MapPage so navigation is performed with MAUI Shell (avoids Blazor routing lifecycle issues)
		Routing.RegisterRoute(nameof(MauiPages.MapPage), typeof(MauiPages.MapPage));
	}
}
