using Foundation;
using MvvmCross.Platforms.Ios.Core;
using RadioFrimleyPark.Core;

namespace RadioFrimleyPark.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : MvxApplicationDelegate<Setup, App>
    {
    }
}
