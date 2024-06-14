using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using RadioFrimleyPark.App.Fragments;
using RadioFrimleyPark.App.Models;

namespace RadioFrimleyPark.App.Adapters
{

    public class GalleryAdapter : RecyclerView.Adapter
    {
        public readonly Gallery1 gallery;
        private Context context;

        public event EventHandler<int> ItemClick;

        public GalleryAdapter(Context context, Gallery1 gallery)
        {
            this.gallery = gallery;
            this.context = context;
        }

        public override int ItemCount
        {
            get { return gallery.Count; }
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            GalleryViewHolder vh = holder as GalleryViewHolder;

            Event _event = gallery[position];
            vh.title.Text = _event.eventTitle;
            vh.date.Text = _event.eventDate.ToShortDateString();

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager((Activity)this.context);
            vh.photos.SetLayoutManager(layoutManager);
            GalleryPhotoAdapter photoAdapter = new GalleryPhotoAdapter(this.context, _event.photos);
            vh.photos.SetAdapter(photoAdapter);

            RecyclerView.LayoutManager layoutManager1 = new LinearLayoutManager((Activity)this.context);
            vh.videos.SetLayoutManager(layoutManager1);
            GalleryVideoAdapter videoAdapter = new GalleryVideoAdapter(this.context, _event.videos);
            vh.videos.SetAdapter(videoAdapter);

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Event, parent, false);
            GalleryViewHolder vh = new GalleryViewHolder(itemView, OnClick);
            return vh;

        }

        public class GalleryViewHolder : RecyclerView.ViewHolder
        {
            //public ImageView image { get; private set; }
            public TextView title { get; set; }
            public TextView date { get; set; }
            public RecyclerView videos { get; set; }
            public RecyclerView photos { get; set; }

            public GalleryViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                //image = itemView.FindViewById<ImageView>(Resource.Id.image);
                title = itemView.FindViewById<TextView>(Resource.Id.title);
                date = itemView.FindViewById<TextView>(Resource.Id.date1);
                photos = itemView.FindViewById<RecyclerView>(Resource.Id.photos);
                videos = itemView.FindViewById<RecyclerView>(Resource.Id.videos);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}