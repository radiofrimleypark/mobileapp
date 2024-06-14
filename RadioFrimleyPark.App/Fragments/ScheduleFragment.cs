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
using RadioFrimleyPark.App.Adapters;
using ModernHttpClient;
using Newtonsoft.Json;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using RadioFrimleyPark.App.Models;

namespace RadioFrimleyPark.App.Fragments
{
    public class ScheduleFragment : Android.Support.V4.App.Fragment
    {
        const string url = @"http://www.radiofrimleypark.co.uk/api/programmes.php";

        private RecyclerView recycler;

        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            using (var client = new System.Net.Http.HttpClient(new NativeMessageHandler()))
            {
                Schedule schedule = JsonConvert.DeserializeObject<Schedule>(await client.GetStringAsync(url));

                ScheduleAdapter adapter = new ScheduleAdapter(this.Activity, schedule);
                adapter.ItemClick += OnItemClick;
                recycler.SetAdapter(adapter);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.Schedule, container, false);

            recycler = rootView.FindViewById<RecyclerView>(Resource.Id.schedule_recycler);

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(this.Activity);
            recycler.SetLayoutManager(layoutManager);

            var toolbar = this.Activity.FindViewById<Toolbar>(Resource.Id.toolbar);
            var cast = toolbar.FindViewById<ImageButton>(Resource.Id.cast);
            cast.Click += (sender, args) => ShowDialog();

            return rootView;
        }

        public void ShowDialog()
        {
            var transaction = FragmentManager.BeginTransaction();
            var dialogFragment = new GoogleCastFragment();
            dialogFragment.Show(transaction, "google_cast_fragment");
        }

        private void OnItemClick(object o, int i)
        {
        }

    }
}