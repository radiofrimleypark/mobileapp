using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using GoogleCast;
using GoogleCast.Channels;
using ModernHttpClient;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RadioFrimleyPark.Core.ViewModels;
using RadioFrimleyPark.Droid.Views.Base;

namespace RadioFrimleyPark.Droid.Views
{
    [MvxDialogFragmentPresentation]
    [Register(nameof(ListenViewModel))]
    public class ListenFragment : BaseDialogFragment<ListenViewModel>
    {
        /*
        private readonly string url = @"http://www.radiofrimleypark.co.uk/api/currentnext.php?format=json";
        protected override int FragmentLayoutId => Resource.Layout.fragment_cast_stream;

        bool isBound = false;

        private LinearLayout linearLayout1;

        private MediaPlayerServiceBinder binder;

        MediaPlayerServiceConnection mediaPlayerServiceConnection;

        private Intent mediaPlayerServiceIntent;

        public event StatusChangedEventHandler StatusChanged;

        public event CoverReloadedEventHandler CoverReloaded;

        public event PlayingEventHandler Playing;

        public event BufferingEventHandler Buffering;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            //return base.OnCreateView(inflater, container, savedInstanceState);
            View rootView = inflater.Inflate(Resource.Layout.Listen, container, false);

            var play = rootView.FindViewById<Button>(Resource.Id.play);
            var pause = rootView.FindViewById<Button>(Resource.Id.pause);
            var stop = rootView.FindViewById<Button>(Resource.Id.stop);

            linearLayout1 = rootView.FindViewById<LinearLayout>(Resource.Id.linearLayout1);

            /*
            var previous = FindViewById<ImageButton>(Resource.Id.btnPrevious);
            var playpause = FindViewById<Button>(Resource.Id.btnPlayPause);
            var next = FindViewById<ImageButton>(Resource.Id.btnNext);
            var position = FindViewById<TextView>(Resource.Id.textview_position);
            var duration = FindViewById<TextView>(Resource.Id.textview_duration);
            var seekbar = FindViewById<SeekBar>(Resource.Id.player_seekbar);
            var cover = FindViewById<ImageView>(Resource.Id.imageview_cover);
            var title = FindViewById<TextView>(Resource.Id.textview_title);
            var subtitle = FindViewById<TextView>(Resource.Id.textview_subtitle);
            * /

            if (mediaPlayerServiceConnection == null)
                InitializeMedia();

            / *
                        play.Click += (sender, args) => SendAudioCommand(StreamingBackgroundService.ActionPlay);
                        pause.Click += (sender, args) => SendAudioCommand(StreamingBackgroundService.ActionPause);
                        stop.Click += (sender, args) => SendAudioCommand(StreamingBackgroundService.ActionStop);
            */

            /*
            previous.Click += async (sender, args) => {
                if (binder.GetMediaPlayerService().mediaPlayer != null)
                    await binder.GetMediaPlayerService().PlayPrevious();
            };
            */

            /*
            next.Click += async (sender, args) => {
                if (binder.GetMediaPlayerService().mediaPlayer != null)
                    await binder.GetMediaPlayerService().PlayNext();
            };
            * /

            play.Click += async (sender, args) =>
            {
//                if (binder.GetMediaPlayerService().mediaPlayer != null &&
//                    binder.GetMediaPlayerService().MediaPlayerState != PlaybackStateCompat.StatePlaying)
                    await binder.GetMediaPlayerService().Play();

#if GOOGLECAST
                IReceiver receiver;
                try
                {
                    await Sender.ConnectAsync(receiver);
                    var mediaChannel = Sender.GetChannel<IMediaChannel>();
                    var receiverChannel = Sender.GetChannel<IReceiverChannel>();
                    mediaChannel.StatusChanged += MediaChannel_StatusChanged;
                    receiverChannel.StatusChanged += ReceiverChannel_StatusChanged;
                    await Sender.LaunchAsync(mediaChannel);
                    await mediaChannel.LoadAsync(channel.ToMedia(480));
                    mediaChannel.StatusChanged -= MediaChannel_StatusChanged;
                    receiverChannel.StatusChanged -= ReceiverChannel_StatusChanged;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this.Activity, $"Exception: {ex.Message}", ToastLength.Long).Show();
                }
#endif
            };
            pause.Click += async (sender, args) =>
            {
                if (binder.GetMediaPlayerService().mediaPlayer != null &&
                    binder.GetMediaPlayerService().MediaPlayerState != PlaybackStateCompat.StatePaused)
                    await binder.GetMediaPlayerService().Pause();
            };
            stop.Click += async (sender, args) =>
            {
                if (binder.GetMediaPlayerService().mediaPlayer != null &&
                    binder.GetMediaPlayerService().MediaPlayerState != PlaybackStateCompat.StateStopped)
                    await binder.GetMediaPlayerService().Stop();
            };

            Playing += (object sender, EventArgs e) => {
                //seekbar.Max = binder.GetMediaPlayerService().Duration;
                //seekbar.Progress = binder.GetMediaPlayerService().Position;

                //position.Text = GetFormattedTime(binder.GetMediaPlayerService().Position);
                //duration.Text = GetFormattedTime(binder.GetMediaPlayerService().Duration);
            };

            Buffering += (object sender, EventArgs e) => {
                //seekbar.SecondaryProgress = binder.GetMediaPlayerService().Buffered;
            };

            CoverReloaded += (object sender, EventArgs e) => {
                //cover.SetImageBitmap(binder.GetMediaPlayerService().Cover as Bitmap);
            };

            StatusChanged += (object sender, EventArgs e) => {
                var metadata = binder.GetMediaPlayerService().mediaControllerCompat.Metadata;
                if (metadata != null)
                {
                    this.Activity.RunOnUiThread(() => {
                        //title.Text = metadata.GetString(MediaMetadata.MetadataKeyTitle);
                        //subtitle.Text = metadata.GetString(MediaMetadata.MetadataKeyArtist);
                        //playpause.Selected = binder.GetMediaPlayerService().mediaControllerCompat.PlaybackState.State == PlaybackStateCompat.StatePlaying;
                    });
                }
            };

            Task.Factory.StartNew(() => DownloadBannerAsync());

            return rootView;
        }

        private void ReceiverChannel_StatusChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void MediaChannel_StatusChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void OnResume()
        {
            base.OnResume();
            ConnectivityManager connectivity = (ConnectivityManager)this.Activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivity.ActiveNetworkInfo;
            if (!networkInfo.IsConnected)
            {
                this.linearLayout1.Enabled = false;
                this.linearLayout1.SetBackgroundColor(Color.Red);
            }
            switch (networkInfo.Type)
            {
                case ConnectivityType.Wifi:
                    this.linearLayout1.SetBackgroundColor(Color.Green);
                    break;
                case ConnectivityType.Mobile:
                    this.linearLayout1.SetBackgroundColor(Color.Orange);
                    break;
            }
        }

        private void InitializeMedia()
        {
            mediaPlayerServiceIntent = new Intent(this.Activity.ApplicationContext, typeof(MediaPlayerService));
            mediaPlayerServiceConnection = new MediaPlayerServiceConnection(this);
            Activity.BindService(mediaPlayerServiceIntent, mediaPlayerServiceConnection, Bind.AutoCreate);
        }

        class MediaPlayerServiceConnection : Java.Lang.Object, IServiceConnection
        {
            readonly ListenFragment fragment;

            public MediaPlayerServiceConnection(ListenFragment fragment)
            {
                this.fragment = fragment;
            }

            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                var mediaPlayerServiceBinder = service as MediaPlayerServiceBinder;
                if (mediaPlayerServiceBinder != null)
                {
                    var binder = (MediaPlayerServiceBinder)service;
                    fragment.binder = binder;
                    fragment.isBound = true;

                    binder.GetMediaPlayerService().CoverReloaded += (object sender, EventArgs e) =>
                    {
                        fragment.CoverReloaded?.Invoke(sender, e);
                    };
                    binder.GetMediaPlayerService().StatusChanged += (object sender, EventArgs e) =>
                    {
                        fragment.StatusChanged?.Invoke(sender, e);
                    };
                    binder.GetMediaPlayerService().Playing += (object sender, EventArgs e) =>
                    {
                        fragment.Playing?.Invoke(sender, e);
                    };
                    binder.GetMediaPlayerService().Buffering += (object sender, EventArgs e) =>
                    {
                        fragment.Buffering?.Invoke(sender, e);
                    };
                }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                fragment.isBound = false;
            }
        }

        private string GetFormattedTime(int value)
        {
            var span = TimeSpan.FromMilliseconds(value);
            if (span.Hours > 0)
            {
                return String.Format("{0}:{1:00}:{2:00}", (int)span.TotalHours, span.Minutes, span.Seconds);
            }
            else
            {
                return String.Format("{0}:{1:00}", (int)span.Minutes, span.Seconds);
            }
        }
#if false
        private void SendAudioCommand(string action)
        {
            var intent = new Intent(this.Activity, typeof(StreamingBackgroundService));
            intent.SetAction(action);
            this.Activity.StartService(intent);
        }
#endif
        public async Task DownloadBannerAsync()
        {
            using (var client = new System.Net.Http.HttpClient(new NativeMessageHandler()))
            {
                Bitmap bitmap = null;

                var currentNext = JsonConvert.DeserializeObject<CurrentNext>(await client.GetStringAsync(url), new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" });

                this.Activity.RunOnUiThread(() =>
                {
                    TextView name = this.Activity.FindViewById<TextView>(Resource.Id.programme);
                    name.Text = currentNext.programmes.current.name;
                });

            }
        }
    */

        SimpleExoPlayer _player;

        protected override int FragmentLayoutId => Resource.Layout.fragment_play_stream;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = base.OnCreateView(inflater, container, savedInstanceState);

            return rootView;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            if (Dialog is AlertDialog alertDialog)
            {
                alertDialog.SetView(View);
            }
        }
        public override void OnResume()
        {
            var mediaUri = Android.Net.Uri.Parse(ListenViewModel.StreamUri.ToString());

            var context = Activity;
            var userAgent = Util.GetUserAgent(context, "ExoPlayerDemo");
            var defaultHttpDataSourceFactory = new DefaultHttpDataSourceFactory(userAgent);
            var defaultDataSourceFactory = new DefaultDataSourceFactory(context, null, defaultHttpDataSourceFactory);
            var extractorMediaSource = new ExtractorMediaSource(mediaUri, defaultDataSourceFactory, new DefaultExtractorsFactory(), null, null);
            var defaultBandwidthMeter = new DefaultBandwidthMeter();
            var adaptiveTrackSelectionFactory = new AdaptiveTrackSelection.Factory(defaultBandwidthMeter);
            var defaultTrackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);

            _player = ExoPlayerFactory.NewSimpleInstance(context, defaultTrackSelector);
            _player.Prepare(extractorMediaSource);
            _player.PlayWhenReady = true;

        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = new AlertDialog.Builder(Activity)
              // .SetTitle(ViewModel.Name)
              .SetPositiveButton("OK", (_, __) => ViewModel.OkCommand.Execute(null))
              .Create();
            dialog.RequestWindowFeature((int)Android.Views.WindowFeatures.NoTitle);
            return dialog;
        }
    }
}
