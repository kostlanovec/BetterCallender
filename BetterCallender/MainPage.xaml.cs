namespace BetterCallender;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}
    void OnButtonTapped(object sender, EventArgs args)
    {
        // zde můžete vložit kód, který se má provést, když je na tlačítko klepnuto
        Console.WriteLine("Klepnuto na tlačítko");
    }
}

