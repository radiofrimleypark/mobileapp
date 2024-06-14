using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
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
using ModernHttpClient;

namespace RadioFrimleyPark.App.Fragments
{
    
    public class Webcam1Fragment : Android.Support.V4.App.Fragment
    {
        private const string url = "http://www.radiofrimleypark.co.uk/webcam/webcam{0}.jpg";
        private Timer timer;
        private EntityTagHeaderValue etag;
        protected int index;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            timer = new Timer(OnTimerAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.index = 1;
            View rootView = inflater.Inflate(Resource.Layout.Webcam1, container, false);

            var snapshot = rootView.FindViewById<ImageView>(Resource.Id.webcam1);

            snapshot.Click += (sender, args) => TakeSnapshot();

            return rootView;

        }
        public override void OnPause()
        {
            base.OnPause();
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public override void OnResume()
        {
            base.OnResume();
            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        protected void TakeSnapshot()
        {
            ImageView image = this.Activity.FindViewById<ImageView>(Resource.Id.webcam1);
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
        public async void OnTimerAsync(object state)
        {

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, String.Format(url,index)),
                    HttpCompletionOption.ResponseHeadersRead);
                if (etag == null || result.Headers.ETag.Tag != etag.Tag)
                {
                    etag = result.Headers.ETag;
                    var stream = await result.Content.ReadAsStreamAsync();
                    try
                    {
                        var bitmap = BitmapFactory.DecodeStream(stream);
                        this.Activity.RunOnUiThread(() =>
                        {
                            ImageView image = this.Activity.FindViewById<ImageView>(Resource.Id.webcam1);
                            image.SetImageBitmap(bitmap);
                        });
                    }
                    catch
                    { }
                    if (etag != null)
                        Console.WriteLine("Updated: " + etag.Tag);
                }
                else
                    Console.WriteLine("Not updated: " + etag.Tag);
            }
        }
    }


}