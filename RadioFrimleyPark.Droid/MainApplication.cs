using System;

using Android.App;
using Android.Runtime;
using GoogleCast;
using MvvmCross.Platforms.Android.Views;
using RadioFrimleyPark.Core;

namespace RadioFrimleyPark.Droid
{
    #if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : MvxAndroidApplication<Setup, App>
    {
        public IReceiver Receiver { set; get; }

        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}
