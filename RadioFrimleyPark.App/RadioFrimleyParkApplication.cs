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
using GoogleCast;

namespace RadioFrimleyPark
{
    // https://stackoverflow.com/questions/21427981/how-to-register-my-own-application-subclass-in-xamarin-android
    [Application]
    public class RadioFrimleyParkApplication : Application
    {

        public IReceiver Receiver { set; get; }

        public RadioFrimleyParkApplication(IntPtr handle, JniHandleOwnership ownerShip)
            : base(handle, ownerShip)
        {
        }

        public override void OnCreate()
        {
            // If OnCreate is overridden, the overridden c'tor will also be called.
            base.OnCreate();
        }
    }
}