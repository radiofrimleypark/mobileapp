using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RadioFrimleyPark.Core.ViewModels.Main;
using RadioFrimleyPark.Droid.Views.Base;

namespace RadioFrimleyPark.Droid.Views.Main
{
    [Activity(
        Theme = "@style/AppTheme",
        WindowSoftInputMode = SoftInput.AdjustResize | SoftInput.StateHidden)]
    public class MainContainerActivity : BaseActivity<MainContainerViewModel>
    {
        protected override int ActivityLayoutId => Resource.Layout.activity_main_container;
    }

    public interface ICastAvailable
    {
        void SetChromecast(Uri uri);
    }

}
