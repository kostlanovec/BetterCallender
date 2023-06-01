using BetterCallender.View;

namespace BetterCallender;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var calllendarList = new CalllendarList();

        MainPage = new NavigationPage(calllendarList)
        {
            BarBackgroundColor = Colors.White,
            BarTextColor = Colors.Red
        };
    }

}