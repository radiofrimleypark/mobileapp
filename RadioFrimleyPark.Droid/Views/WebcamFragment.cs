using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using RadioFrimleyPark.Droid.Views.Base;
using RadioFrimleyPark.Core.ViewModels;
using Android;

namespace RadioFrimleyPark.Droid.Views
{
    public class WebcamFragment : BaseFragment<WebcamViewModel>
    {
        protected int index;
        private ImageView _image;

        protected override int FragmentLayoutId => Resource.Layout.fragment_webcam;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ViewModel.ImageReady += (s, e) =>
            {
                var bitmap = BitmapFactory.DecodeStream(e.Stream);
                Activity.RunOnUiThread(() =>
                {
                    _image.SetImageBitmap(bitmap);
                });
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.index = 2;
            View rootView = base.OnCreateView(inflater, container, savedInstanceState);

            var snapshot = rootView.FindViewById<ImageView>(Resource.Id.webcam);
            _image = Activity.FindViewById<ImageView>(Resource.Id.webcam);

            snapshot.Click += (s, e) => TakeSnapshot();

            return rootView;
        }
        public override void OnPause()
        {
            ViewModel.Stop();
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            ViewModel.Start();
        }

        protected void TakeSnapshot()
        {
            ImageView image = this.Activity.FindViewById<ImageView>(Resource.Id.webcam);
            image.SetScaleType(ImageView.ScaleType.CenterCrop);
            Drawable clone = image.Drawable.GetConstantState().NewDrawable();
            image.SetImageDrawable(clone);
            if (clone is BitmapDrawable bitmapDrawable)
            {
                if (bitmapDrawable.Bitmap != null)
                {
                    string fileName = String.Format("RadioFrimleyPark-{0}-{1}.jpg", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()).Replace("/", "").Replace(":", "");
                    String filePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryPictures, fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        Toast.MakeText(this.Activity, "Image was already downloaded", ToastLength.Long).Show();
                        return;
                    }
                    try
                    {
                        using (FileStream fs = new FileInfo(filePath).Create())
                        {
                            bitmapDrawable.Bitmap.Compress(Bitmap.CompressFormat.Jpeg, 95, fs);
                            fs.Close();
                            Toast.MakeText(this.Activity, String.Format("Image {0} saved to gallery", fileName), ToastLength.Long).Show();
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Toast.MakeText(this.Activity, String.Format("Unable to save {0} to gallery", fileName), ToastLength.Long).Show();
                    }
                }
            }
        }
    }
}
