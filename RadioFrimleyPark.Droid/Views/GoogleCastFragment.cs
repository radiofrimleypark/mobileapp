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
using Android.Util;
using Android.Views;
using Android.Widget;

using GoogleCast;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using RadioFrimleyPark.Core.ViewModels;
using RadioFrimleyPark.Droid.Views.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Commands;
using RadioFrimleyPark.Droid;
using Resource = RadioFrimleyPark.Droid.Resource;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX;

namespace RadioFrimleyPark.Droid.Views
{
    [MvxDialogFragmentPresentation]
    [Register(nameof(GoogleCastViewModel))]
    public class GoogleCastFragment : BaseDialogFragment<GoogleCastViewModel>
    {
        /*
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
        */
        protected override int FragmentLayoutId => Resource.Layout.fragment_cast_stream;

        public static readonly Sender Sender = new Sender();

        public DeviceLocator DeviceLocator { set; get; }
        public MvxRecyclerView _recycler;

        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            DeviceLocator = new DeviceLocator();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = base.OnCreateView(inflater, container, savedInstanceState);

            _recycler = rootView.FindViewById<MvxRecyclerView>(Resource.Id.cast_recycler);

            _recycler.Adapter.ItemClick = new MvxAsyncCommand<IReceiver>(async (receiver) =>
            {
                ((MainApplication)Activity.Application).Receiver = receiver;
                try
                {
                    await Sender.ConnectAsync(((MainApplication)Activity.Application).Receiver);
#if true
                        var toolbar = Activity.FindViewById<Toolbar>(Resource.Id.toolbar);
                    var cast = toolbar.FindViewById<ImageButton>(Resource.Id.cast_icon);
                    cast.SetImageResource(Resource.Drawable.ic_cast_connected_white_24dp);
                    cast.Click += (sender, e) =>
                    {

                    };
#endif

                        //                    var mediaChannel = Sender.GetChannel<IMediaChannel>();
                        //                    var receiverChannel = Sender.GetChannel<IReceiverChannel>();
                        //                    mediaChannel.StatusChanged += MediaChannel_StatusChanged;
                        //                    receiverChannel.StatusChanged += ReceiverChannel_StatusChanged;
                        //                    await Sender.LaunchAsync(mediaChannel);
                        this.Dismiss();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Activity, $"Exception: {ex.Message}", ToastLength.Long).Show();
                }
            });

            var swipeToRefresh = rootView.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.cast_stream_refresher);
            var appBar = Activity.FindViewById<AppBarLayout>(Resource.Id.appbar);
            if (appBar != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

            return rootView;
        }

        private void Cast_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            if (Dialog is AlertDialog alertDialog)
            {
                alertDialog.SetView(View);
            }
        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog dialog = new AlertDialog.Builder(Activity)
              //              .SetTitle(ViewModel.Name)
              .SetPositiveButton("OK", (_, __) => ViewModel.OkCommand.Execute(null))
              .Create();
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            return dialog;
        }

        public override void OnResume()
        {
            base.OnResume();
            ConnectivityManager connectivity = (ConnectivityManager)this.Activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivity.ActiveNetworkInfo;
            switch (networkInfo.Type)
            {
                case ConnectivityType.Wifi:
                    Task.Run(async () =>
                    {
                        //var receivers = await DeviceLocator.FindReceiversAsync();
                        //ChromecastsAdapter adapter = new ChromecastsAdapter(this.Activity, receivers.ToList());
                        //adapter.ItemClick += async (Sender, args) => { };
                        //recycler.SetAdapter(adapter);

                        //_recycler.Chromecasts = (await DeviceLocator.FindReceiversAsync()).ToList();
                        //_recycler.Adapter.NotifyDataSetChanged();

                        //adapter.Chromecasts.Add(new Receiver { FriendlyName = "Jim", IPEndPoint = new IPEndPoint(IPAddress.Parse("1.2.2.3"), 1234) });
                    }).Wait();
                    break;
            }
        }
    }
}
