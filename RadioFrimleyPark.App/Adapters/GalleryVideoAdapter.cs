using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using RadioFrimleyPark.App.Fragments;
using RadioFrimleyPark.App.Models;

namespace RadioFrimleyPark.App.Adapters
{
    public class GalleryVideoAdapter: RecyclerView.Adapter
    {
        public readonly List<Video> videos;
        private Context context;

        public event EventHandler<int> ItemClick;

        public GalleryVideoAdapter(Context context, List<Video> videos)
        {
            this.videos= videos;
            this.context = context;
        }

        public override int ItemCount
        {
            get { return videos.Count; }
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            GalleryVideoViewHolder vh = holder as GalleryVideoViewHolder;

            Video video = videos[position];
            vh.title.Text = video.title;
            //vh.video.SetVideoURI(Android.Net.Uri.Parse(video.video));

        }
        private void OnItemClick(object o, int i)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Video, parent, false);
            GalleryVideoViewHolder vh = new GalleryVideoViewHolder(itemView, OnClick);
            return vh;

        }

        public class GalleryVideoViewHolder : RecyclerView.ViewHolder
        {
            //public ImageView image { get; private set; }
            public TextView title { get; set; }
            public VideoView video { get; set; }

            public GalleryVideoViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                //image = itemView.FindViewById<ImageView>(Resource.Id.image);
                title = itemView.FindViewById<TextView>(Resource.Id.title);
                video = itemView.FindViewById<VideoView>(Resource.Id.video);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}