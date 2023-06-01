using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls;


namespace BetterCallender
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        //Theme = "@style/Maui.MainTheme.NoActionBar",
        MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.SetStatusBarColor(Android.Graphics.Color.White);
            Window.SetNavigationBarColor(Android.Graphics.Color.WhiteSmoke);

            base.OnCreate(savedInstanceState);
        }
    }
}