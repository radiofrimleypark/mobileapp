using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RadioFrimleyPark.App.Adapters;
using RadioFrimleyPark.App.Models;

namespace RadioFrimleyPark.App.Fragments
{
    public class GalleryFragment : Android.Support.V4.App.Fragment
    {
        private readonly string url = @"http://www.radiofrimleypark.co.uk/api/gallery.php?year={0}";

        private RecyclerView recycler;

        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            using (var client = new System.Net.Http.HttpClient(new NativeMessageHandler()))
            {
                var gallery = JsonConvert.DeserializeObject<Gallery1>(await client.GetStringAsync(String.Format(url, DateTime.Today.Year)), new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                //ScheduleAdapter adapter = new ScheduleAdapter(this.Activity, schedule);

                GalleryAdapter adapter = new GalleryAdapter(this.Activity, gallery);
                //adapter.NotifyDataSetChanged();
                adapter.ItemClick += OnItemClick;
                recycler.SetAdapter(adapter);

            };

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View rootView = inflater.Inflate(Resource.Layout.Gallery, container, false);

            recycler = rootView.FindViewById<RecyclerView>(Resource.Id.gallery_recycler);

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(this.Activity);
            recycler.SetLayoutManager(layoutManager);

            return rootView;
        }
        private void OnItemClick(object o, int i)
        {
        }

    }
}