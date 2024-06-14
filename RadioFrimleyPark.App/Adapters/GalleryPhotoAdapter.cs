using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RadioFrimleyPark.App.Fragments;
using RadioFrimleyPark.App.Models;

namespace RadioFrimleyPark.App.Adapters
{
    public class GalleryPhotoAdapter: RecyclerView.Adapter
    {
        public readonly List<Photo> photos;
        private Context context;

        public event EventHandler<int> ItemClick;

        public GalleryPhotoAdapter(Context context, List<Photo> photos)
        {
            this.photos = photos;
            this.context = context;
        }

        public override int ItemCount
        {
            get { return photos.Count; }
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            GalleryPhotoViewHolder vh = holder as GalleryPhotoViewHolder;

            Photo photo = photos[position];
            vh.title.Text = photo.title;

            Task.Factory.StartNew(() => DisplayImages(vh.photo, new Uri("http://www.radiofrimleypark.co.uk/thumbs/" + photo.photo)));
        }

        private async void DisplayImages(ImageView image, Uri url)
        {
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                Bitmap bitmap = null;

                var result = await client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Get, url),
                    HttpCompletionOption.ResponseHeadersRead);
                var stream = await result.Content.ReadAsStreamAsync();
                bitmap = BitmapFactory.DecodeStream(stream);
                ((Activity)this.context).RunOnUiThread(() =>
                {
                    image.SetImageBitmap(bitmap);
                });
            }
        }
        private void OnItemClick(object o, int i)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Photo, parent, false);
            GalleryPhotoViewHolder vh = new GalleryPhotoViewHolder(itemView, OnClick);
            return vh;

        }

        public class GalleryPhotoViewHolder : RecyclerView.ViewHolder
        {
            //public ImageView image { get; private set; }
            public TextView title { get; set; }
            public ImageView photo { get; set; }

            public GalleryPhotoViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                //image = itemView.FindViewById<ImageView>(Resource.Id.image);
                title = itemView.FindViewById<TextView>(Resource.Id.title);
                photo = itemView.FindViewById<ImageView>(Resource.Id.photo);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}