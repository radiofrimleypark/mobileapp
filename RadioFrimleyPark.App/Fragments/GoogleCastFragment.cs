#define GOOGLECAST
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using RadioFrimleyPark.App.Adapters;
using Toolbar = Android.Support.V7.Widget.Toolbar;

using GoogleCast;
using GoogleCast.Channels;
using GoogleCast.Models.Media;

namespace RadioFrimleyPark.App.Fragments
{
    public class GoogleCastFragment : Android.Support.V4.App.DialogFragment
    {
        private GoogleCastAdapter adapter;

        private RecyclerView recycler;

#if GOOGLECAST
        public static readonly Sender Sender = new Sender();

        public DeviceLocator DeviceLocator;
#endif

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.z_Chromecasts, container, false);
#if GOOGLECAST
            DeviceLocator = new DeviceLocator();
            //Task.Factory.StartNew(() => DeviceLocator.FindReceiversAsync());

            //Sender = new Sender();

#endif
            recycler = rootView.FindViewById<RecyclerView>(Resource.Id.recycler1);

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(this.Activity);
            recycler.SetLayoutManager(layoutManager);

            adapter = new GoogleCastAdapter(this.Activity);
            //adapter.CollectionChanged += Adapter_CollectionChanged;
            adapter.ItemClick += async (sender, args) =>
            {
                ((RadioFrimleyParkApplication)Activity.Application).Receiver = args;
                try
                {
                    await Sender.ConnectAsync(((RadioFrimleyParkApplication)Activity.Application).Receiver);
                    var toolbar = this.Activity.FindViewById<Toolbar>(Resource.Id.toolbar);
                    var cast = toolbar.FindViewById<ImageButton>(Resource.Id.cast);
                    cast.SetImageResource(Resource.Drawable.ic_cast_connected_white_24dp);
                    //                    var mediaChannel = Sender.GetChannel<IMediaChannel>();
                    //                    var receiverChannel = Sender.GetChannel<IReceiverChannel>();
                    //                    mediaChannel.StatusChanged += MediaChannel_StatusChanged;
                    //                    receiverChannel.StatusChanged += ReceiverChannel_StatusChanged;
                    //                    await Sender.LaunchAsync(mediaChannel);
                    this.Dismiss();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this.Activity, $"Exception: {ex.Message}", ToastLength.Long).Show();
                }
            };
            recycler.SetAdapter(adapter);

            return rootView;

        }

        public override async void OnResume()
        {
            base.OnResume();
            ConnectivityManager connectivity = (ConnectivityManager)this.Activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivity.ActiveNetworkInfo;
            switch (networkInfo.Type)
            {
                case ConnectivityType.Wifi:
#if GOOGLECAST
                    //var receivers = await DeviceLocator.FindReceiversAsync();
                    //ChromecastsAdapter adapter = new ChromecastsAdapter(this.Activity, receivers.ToList());
                    //adapter.ItemClick += async (Sender, args) => { };
                    //recycler.SetAdapter(adapter);

                    adapter.Chromecasts = (await DeviceLocator.FindReceiversAsync()).ToList();
                    adapter.NotifyDataSetChanged();
                    //adapter.Chromecasts.Add(new Receiver { FriendlyName = "Jim", IPEndPoint = new IPEndPoint(IPAddress.Parse("1.2.2.3"), 1234) });
#endif
                    break;
            }
        }
#if false
        private void Adapter_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
            adapter.NotifyDataSetChanged();
        }
#endif

        //private IReceiver receiver = null;

        public class GoogleCastViewHolder : RecyclerView.ViewHolder
        {
            public ImageView banner { get; private set; }
            public ImageView icon { get; private set; }
            public string link { get; set; }
            public TextView name { get; set; }

            public GoogleCastViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                icon = itemView.FindViewById<ImageView>(Resource.Id.icon1);
                //link = itemView.FindViewById<TextView>(Resource.Id.link);
                name = itemView.FindViewById<TextView>(Resource.Id.name1);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}