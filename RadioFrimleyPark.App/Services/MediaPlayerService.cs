﻿using Android.App;
using System;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Graphics;
using Android.Support.V4.Media.Session;
using Android.Support.V4.Media;
using Android.Support.V4.App;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using RadioFrimleyPark.App.Receivers;
using ModernHttpClient;

namespace RadioFrimleyPark.App.Services
{
    // https://github.com/jamesmontemagno/AndroidStreamingAudio
    // https://blog.xamarin.com/background-audio-streaming-with-xamarin-android/
    // https://blog.xamarin.com/lock-screen-music-controls-in-xamarin-android/


    [Service]
    [IntentFilter (new[] { ActionPlay, ActionPause, ActionStop, ActionTogglePlayback, ActionNext, ActionPrevious })]
    public class MediaPlayerService : Service, AudioManager.IOnAudioFocusChangeListener, 
    MediaPlayer.IOnBufferingUpdateListener, 
    MediaPlayer.IOnCompletionListener, 
    MediaPlayer.IOnErrorListener, 
    MediaPlayer.IOnPreparedListener, 
    MediaPlayer.IOnSeekCompleteListener
    {
        //private const string StreamUrl = @"http://fire.wavestreamer.com:7409";
        private const string StreamUrl = @"http://stream.radiofrimleypark.co.uk:8946";

        //Actions
        public const string ActionPrefix = "com.xamarin.action";
        public const string ActionPlay = ActionPrefix +".PLAY";
        public const string ActionPause = ActionPrefix + ".PAUSE";
        public const string ActionStop = ActionPrefix + ".STOP";
		public const string ActionTogglePlayback = ActionPrefix + ".TOGGLEPLAYBACK";
		public const string ActionNext = ActionPrefix + ".NEXT";
		public const string ActionPrevious = ActionPrefix + ".PREVIOUS";

        private const string audioUrl = @"http://www.montemagno.com/sample.mp3";

        public MediaPlayer mediaPlayer;
        private AudioManager audioManager;

        private MediaSessionCompat mediaSessionCompat;
        public MediaControllerCompat mediaControllerCompat;

        //private Channel channel;

        public int MediaPlayerState
		{
			get{
				return (mediaControllerCompat.PlaybackState != null ? 
					mediaControllerCompat.PlaybackState.State: 
					PlaybackStateCompat.StateNone);
			}
		}


        private WifiManager wifiManager;
        private WifiManager.WifiLock wifiLock;
        private ComponentName remoteComponentName;

        private const int NotificationId = 1;

        public event StatusChangedEventHandler StatusChanged;

        public event CoverReloadedEventHandler CoverReloaded;

        public event PlayingEventHandler Playing;

        public event BufferingEventHandler Buffering;

        private Handler PlayingHandler;
        private Java.Lang.Runnable PlayingHandlerRunnable;

        public MediaPlayerService ()
        {
            // Create an instance for a runnable-handler
            PlayingHandler = new Handler ();

            // Create a runnable, restarting itself if the status still is "playing"
            PlayingHandlerRunnable = new Java.Lang.Runnable (() => {
                OnPlaying (EventArgs.Empty);

                if (MediaPlayerState == PlaybackStateCompat.StatePlaying) {
                    PlayingHandler.PostDelayed (PlayingHandlerRunnable, 250);
                }
            });

            // On Status changed to PLAYING, start raising the Playing event
            StatusChanged += (object sender, EventArgs e) => {
                if(MediaPlayerState == PlaybackStateCompat.StatePlaying){
                    PlayingHandler.PostDelayed (PlayingHandlerRunnable, 0);
                }
            };
        }

        protected virtual void OnStatusChanged (EventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged (this, e);
        }

        protected virtual void OnCoverReloaded (EventArgs e)
        {
            if (CoverReloaded != null) {
                CoverReloaded (this, e);
                StartNotification ();
//                UpdateMediaMetadataCompat ();
            }
        }

        protected virtual void OnPlaying (EventArgs e)
        {
            if (Playing != null)
                Playing (this, e);
        }

        protected virtual void OnBuffering (EventArgs e)
        {
            if (Buffering != null)
                Buffering (this, e);
        }

        /// <summary>
        /// On create simply detect some of our managers
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate ();
            //Find our audio and notificaton managers
            audioManager = (AudioManager)GetSystemService (AudioService);
            wifiManager = (WifiManager)GetSystemService (WifiService);

            //remoteComponentName = new ComponentName (PackageName, new RemoteControlBroadcastReceiver ().ComponentName);
        }

        /// <summary>
        /// Will register for the remote control client commands in audio manager
        /// </summary>
        private void InitMediaSession()
        {
            try {
                if (mediaSessionCompat == null) {
                    Intent nIntent = new Intent(ApplicationContext, typeof(MainActivity));
                    PendingIntent pIntent = PendingIntent.GetActivity(ApplicationContext, 0, nIntent, 0);

                    //remoteComponentName = new ComponentName (PackageName, new RemoteControlBroadcastReceiver ().ComponentName);

                    mediaSessionCompat = new MediaSessionCompat (ApplicationContext, "XamarinStreamingAudio", remoteComponentName, pIntent);
                    mediaControllerCompat = new MediaControllerCompat(ApplicationContext, mediaSessionCompat.SessionToken);
                }

                mediaSessionCompat.Active = true;
                mediaSessionCompat.SetCallback (new MediaSessionCallback((MediaPlayerServiceBinder)binder));

                mediaSessionCompat.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);
            } 
            catch (Exception ex) {
                Console.WriteLine (ex);
            }
        }

        /// <summary>
        /// Intializes the player.
        /// </summary>
        private void InitializePlayer ()
        {
            mediaPlayer = new MediaPlayer ();

            //Tell our player to sream music
            mediaPlayer.SetAudioStreamType (Stream.Music);

            //Wake mode will be partial to keep the CPU still running under lock screen
            mediaPlayer.SetWakeMode (ApplicationContext, WakeLockFlags.Partial);

            mediaPlayer.SetOnBufferingUpdateListener (this);
            mediaPlayer.SetOnCompletionListener (this);
            mediaPlayer.SetOnErrorListener (this);
            mediaPlayer.SetOnPreparedListener (this);
        }


        public void OnBufferingUpdate (MediaPlayer mp, int percent)
        {
            int duration = 0;
            if (MediaPlayerState == PlaybackStateCompat.StatePlaying || MediaPlayerState == PlaybackStateCompat.StatePaused)
                duration = mp.Duration;

            int newBufferedTime = duration * percent / 100;
            if (newBufferedTime != Buffered) {
                Buffered = newBufferedTime;
            }
        }

        public async void OnCompletion (MediaPlayer mp)
        {
            // await PlayNext ();
        }

        public bool OnError (MediaPlayer mp, MediaError what, int extra)
        {
            
            UpdatePlaybackState(PlaybackStateCompat.StateError);
			Stop ();
            return true;
        }

        public void OnSeekComplete (MediaPlayer mp)
        {
            //TODO: Implement buffering on seeking
        }

        public void OnPrepared (MediaPlayer mp)
        {
            //Mediaplayer is prepared start track playback
            mp.Start ();
            UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
        }

        public int Position {
            get {
                if (mediaPlayer == null 
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying 
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return -1;
                else
                    return mediaPlayer.CurrentPosition;
            }
        }

        public int Duration {
            get {
                if (mediaPlayer == null 
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying 
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return 0;
                else
                    return mediaPlayer.Duration;
            }
        }

        private int buffered = 0;

        public int Buffered {
            get {
                if (mediaPlayer == null)
                    return 0;
                else
                    return buffered;
            }
            private set {
                buffered = value;
                OnBuffering (EventArgs.Empty);
            }
        }

        private Bitmap cover;

#if false
        public object Cover {
            get {
                if(cover == null)
                    cover = BitmapFactory.DecodeResource(Resources, Resource.Drawable.wordpress);
                return cover;
            }
            private set {
                cover = value as Bitmap;
                OnCoverReloaded (EventArgs.Empty);
            }
        }
#endif
		/// <summary>
		/// Intializes the player.
		/// </summary>
        public async Task Play ()
		{
            if (mediaPlayer != null && MediaPlayerState == PlaybackStateCompat.StatePaused) {
                //We are simply paused so just start again
                mediaPlayer.Start ();
                UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
                StartNotification ();

                //Update the metadata now that we are playing
//                UpdateMediaMetadataCompat ();
                return;
            }

            if (mediaPlayer == null)
                InitializePlayer ();

            if(mediaSessionCompat == null)
                InitMediaSession ();

            if (mediaPlayer.IsPlaying) {
                UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
                return;
            }

            try {

                await mediaPlayer.SetDataSourceAsync (ApplicationContext, Android.Net.Uri.Parse (StreamUrl));

                var focusResult = audioManager.RequestAudioFocus (this, Stream.Music, AudioFocus.Gain);
                if (focusResult != AudioFocusRequest.Granted) {
                    //could not get audio focus
                    Console.WriteLine("Could not get audio focus");
                }

                UpdatePlaybackState(PlaybackStateCompat.StateBuffering);
                mediaPlayer.PrepareAsync ();

                AquireWifiLock ();
                StartNotification ();

            } catch (Exception ex) {
                UpdatePlaybackState(PlaybackStateCompat.StateStopped);

                mediaPlayer.Reset();
                mediaPlayer.Release();
                mediaPlayer = null;

                //unable to start playback log error
                Console.WriteLine(ex);
            }
        }
#if false
        public async Task Seek (int position)
        {
            await Task.Run (() => {
                if (mediaPlayer != null)
                    mediaPlayer.SeekTo (position);
            });
        }

        public async Task PlayNext ()
        {
            if (mediaPlayer != null) {
                mediaPlayer.Reset ();
                mediaPlayer.Release ();
                mediaPlayer = null;
            }

            UpdatePlaybackState(PlaybackStateCompat.StateSkippingToNext);

            await Play(null);
        }

        public async Task PlayPrevious ()
        {
            // Start current track from beginning if it's the first track or the track has played more than 3sec and you hit "playPrevious".
            if (Position > 3000)
            {
                await Seek(0);
            }
            else
            {
                if (mediaPlayer != null) {
                    mediaPlayer.Reset ();
                    mediaPlayer.Release ();
                    mediaPlayer = null;
                }

                UpdatePlaybackState(PlaybackStateCompat.StateSkippingToPrevious);

                await Play(null);
            }
        }

        public async Task PlayPause()
        {
            if (mediaPlayer == null || (mediaPlayer != null && MediaPlayerState == PlaybackStateCompat.StatePaused))
            {
                await Play(null);
            }
            else
            {
                await Pause();
            }
        }
#endif

        public async Task Pause ()
        {
            await Task.Run (() => {
                if (mediaPlayer == null)
                    return;

                if (mediaPlayer.IsPlaying)
                    mediaPlayer.Pause ();

                UpdatePlaybackState(PlaybackStateCompat.StatePaused);
            });
        }

        public async Task Stop ()
        {
            await Task.Run (() => {
                if (mediaPlayer == null)
                    return;

                if (mediaPlayer.IsPlaying) {
                    mediaPlayer.Stop ();
                }

                UpdatePlaybackState(PlaybackStateCompat.StateStopped);
                mediaPlayer.Reset ();
                StopNotification();
                StopForeground (true);
                ReleaseWifiLock ();
                UnregisterMediaSessionCompat ();
            });
        }

        private void UpdatePlaybackState(int state)
        {
            if (mediaSessionCompat == null || mediaPlayer == null)
                return;

            try
            {
                PlaybackStateCompat.Builder stateBuilder = new PlaybackStateCompat.Builder()
                    .SetActions(
                        PlaybackStateCompat.ActionPause |
                        PlaybackStateCompat.ActionPlay |
                        PlaybackStateCompat.ActionPlayPause |
                        PlaybackStateCompat.ActionSkipToNext |
                        PlaybackStateCompat.ActionSkipToPrevious |
                        PlaybackStateCompat.ActionStop
                    )
                    .SetState(state, Position, 1.0f, SystemClock.ElapsedRealtime());

                mediaSessionCompat.SetPlaybackState(stateBuilder.Build());

                //Used for backwards compatibility
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    if (mediaSessionCompat.RemoteControlClient != null && mediaSessionCompat.RemoteControlClient.Equals(typeof(RemoteControlClient)))
                    {
                        RemoteControlClient remoteControlClient = (RemoteControlClient)mediaSessionCompat.RemoteControlClient;

                        RemoteControlFlags flags = RemoteControlFlags.Play
                            | RemoteControlFlags.Pause
                            | RemoteControlFlags.PlayPause
                            | RemoteControlFlags.Previous
                            | RemoteControlFlags.Next
                            | RemoteControlFlags.Stop;

                        remoteControlClient.SetTransportControlFlags(flags);
                    }
                }

                OnStatusChanged(EventArgs.Empty);

                if (state == PlaybackStateCompat.StatePlaying || state == PlaybackStateCompat.StatePaused)
                {
                    StartNotification();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex);
            }
        }

        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        private void StartNotification ()
        {
            if (mediaSessionCompat == null)
                return;

            var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0, new Intent(ApplicationContext, typeof(MainActivity)), PendingIntentFlags.UpdateCurrent);
            MediaMetadataCompat currentTrack = mediaControllerCompat.Metadata;

            Android.Support.V7.App.NotificationCompat.MediaStyle style = new Android.Support.V7.App.NotificationCompat.MediaStyle();
            style.SetMediaSession(mediaSessionCompat.SessionToken);

            Intent intent = new Intent(ApplicationContext, typeof(MediaPlayerService));
            intent.SetAction(ActionStop);
            PendingIntent pendingCancelIntent = PendingIntent.GetService(ApplicationContext, 1, intent, PendingIntentFlags.CancelCurrent);

            style.SetShowCancelButton(true);
            style.SetCancelButtonIntent(pendingCancelIntent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(ApplicationContext)
                .SetStyle(style)
//                .SetContentTitle(MainActivity.AppName)
//                .SetContentText(String.Format("Playing {0}", channel.name))
                .SetSmallIcon(Resource.Drawable.ic_stat_av_play_over_video)
//                .SetContentTitle (currentTrack.GetString(MediaMetadata.MetadataKeyTitle))
//                .SetContentText (currentTrack.GetString(MediaMetadata.MetadataKeyArtist))
//                .SetContentInfo (currentTrack.GetString(MediaMetadata.MetadataKeyAlbum))
//                .SetSmallIcon (Resource.Drawable.di_logo)
//                .SetLargeIcon (Cover as Bitmap)
                .SetContentIntent(pendingIntent)
                .SetShowWhen(false)
                .SetOngoing(MediaPlayerState == PlaybackStateCompat.StatePlaying)
                .SetVisibility(NotificationCompat.VisibilityPublic);

            builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPrevious, "Previous", ActionPrevious));
            AddPlayPauseActionCompat(builder);
            builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaNext, "Next", ActionNext));
            style.SetShowActionsInCompactView(0, 1, 2);

            NotificationManagerCompat.From(ApplicationContext).Notify(NotificationId, builder.Build());
        }

        private NotificationCompat.Action GenerateActionCompat(int icon, String title, String intentAction)
        {
            Intent intent = new Intent(ApplicationContext, typeof(MediaPlayerService));
            intent.SetAction(intentAction);

            PendingIntentFlags flags = PendingIntentFlags.UpdateCurrent;
            if (intentAction.Equals(ActionStop))
                flags = PendingIntentFlags.CancelCurrent;

            PendingIntent pendingIntent = PendingIntent.GetService(ApplicationContext, 1, intent, flags);

            return new NotificationCompat.Action.Builder(icon, title, pendingIntent).Build();
        }

        private void AddPlayPauseActionCompat(NotificationCompat.Builder builder)
        {
            if (MediaPlayerState == PlaybackStateCompat.StatePlaying)
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPause, "Pause", ActionPause));
            else
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPlay, "Play", ActionPlay));
        }

        public void StopNotification()
        {
            NotificationManagerCompat nm = NotificationManagerCompat.From(ApplicationContext);
            nm.CancelAll();
        }
#if false
        /// <summary>
        /// Updates the metadata on the lock screen
        /// </summary>
        private void UpdateMediaMetadataCompat (MediaMetadataRetriever metaRetriever = null)
        {
            if (mediaSessionCompat == null)
                return;

            MediaMetadataCompat.Builder builder = new MediaMetadataCompat.Builder ();

            if (metaRetriever != null) {
                builder
                .PutString (MediaMetadata.MetadataKeyAlbum, metaRetriever.ExtractMetadata (MetadataKey.Album))
                .PutString (MediaMetadata.MetadataKeyArtist, metaRetriever.ExtractMetadata (MetadataKey.Artist))
                .PutString (MediaMetadata.MetadataKeyTitle, metaRetriever.ExtractMetadata (MetadataKey.Title));
            } else {
                builder
                    .PutString (MediaMetadata.MetadataKeyAlbum, mediaSessionCompat.Controller.Metadata.GetString (MediaMetadata.MetadataKeyAlbum))
                    .PutString (MediaMetadata.MetadataKeyArtist, mediaSessionCompat.Controller.Metadata.GetString (MediaMetadata.MetadataKeyArtist))
                    .PutString (MediaMetadata.MetadataKeyTitle, mediaSessionCompat.Controller.Metadata.GetString (MediaMetadata.MetadataKeyTitle));
            }
            builder.PutBitmap (MediaMetadata.MetadataKeyAlbumArt, Cover as Bitmap);

            mediaSessionCompat.SetMetadata(builder.Build());
        }
        [Obsolete ("deprecated")]
        public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
        {
            HandleIntent( intent );
            return base.OnStartCommand(intent, flags, startId);
        }
#endif
        private void HandleIntent( Intent intent ) {
            if( intent == null || intent.Action == null )
                return;

            String action = intent.Action;

            if( action.Equals( ActionPlay ) ) {
                mediaControllerCompat.GetTransportControls().Play();
            } else if( action.Equals( ActionPause ) ) {
                mediaControllerCompat.GetTransportControls().Pause();
            } else if( action.Equals( ActionPrevious ) ) {
                mediaControllerCompat.GetTransportControls().SkipToPrevious();
            } else if( action.Equals( ActionNext ) ) {
                mediaControllerCompat.GetTransportControls().SkipToNext();
            } else if( action.Equals( ActionStop ) ) {
                mediaControllerCompat.GetTransportControls().Stop();
            }
        }

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        private void AquireWifiLock()
        {
            if (wifiLock == null){
                wifiLock = wifiManager.CreateWifiLock(WifiMode.Full, "xamarin_wifi_lock");
            } 
            wifiLock.Acquire();
        }

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        private void ReleaseWifiLock()
        {
            if (wifiLock == null)
                return;

            wifiLock.Release();
            wifiLock = null;
        }

        private void UnregisterMediaSessionCompat ()
        {
            try
            {
                if(mediaSessionCompat != null)
                {
                    mediaSessionCompat.Dispose();
                    mediaSessionCompat = null;
                }
            } catch (Exception ex) {
                Console.WriteLine (ex);
            }
        }

        IBinder binder;

        public override IBinder OnBind (Intent intent)
        {
            binder = new MediaPlayerServiceBinder (this);
            return binder;
        }

        public override bool OnUnbind (Intent intent)
        {
            StopNotification();
            return base.OnUnbind (intent);
        }

        /// <summary>
        /// Properly cleanup of your player by releasing resources
        /// </summary>
        public override void OnDestroy ()
        {
            base.OnDestroy ();
            if (mediaPlayer != null) {
                mediaPlayer.Release ();
                mediaPlayer = null;

                StopNotification ();
                StopForeground (true);
                ReleaseWifiLock ();
                UnregisterMediaSessionCompat ();
            }
        }

        /// <summary>
        /// For a good user experience we should account for when audio focus has changed.
        /// There is only 1 audio output there may be several media services trying to use it so
        /// we should act correctly based on this.  "duck" to be quiet and when we gain go full.
        /// All applications are encouraged to follow this, but are not enforced.
        /// </summary>
        /// <param name="focusChange"></param>
        public void OnAudioFocusChange (AudioFocus focusChange)
        {
            switch (focusChange) {
            case AudioFocus.Gain:
                if (mediaPlayer == null)
                    InitializePlayer ();

                if (!mediaPlayer.IsPlaying) {
                    mediaPlayer.Start ();
                }

                mediaPlayer.SetVolume (1.0f, 1.0f);//Turn it up!
                break;
            case AudioFocus.Loss:
                //We have lost focus stop!
                Stop ();
                break;
            case AudioFocus.LossTransient:
                //We have lost focus for a short time, but likely to resume so pause
                Pause ();
                break;
            case AudioFocus.LossTransientCanDuck:
                //We have lost focus but should till play at a muted 10% volume
                if (mediaPlayer.IsPlaying)
                    mediaPlayer.SetVolume (.1f, .1f);//turn it down!
                break;

            }
        }

        public class MediaSessionCallback : MediaSessionCompat.Callback {

            private MediaPlayerServiceBinder mediaPlayerService;
            public MediaSessionCallback (MediaPlayerServiceBinder service)
            {
                mediaPlayerService = service;
            }

            public override void OnPause ()
            {
                mediaPlayerService.GetMediaPlayerService().Pause();
                base.OnPause ();
            }

            public override async void OnPlay ()
            {
                await mediaPlayerService.GetMediaPlayerService().Play();
                base.OnPlay ();
            }

            public override void OnSkipToNext()
            {
                //mediaPlayerService.GetMediaPlayerService().PlayNext();
                base.OnSkipToNext();
            }

            public override void OnSkipToPrevious()
            {
                //mediaPlayerService.GetMediaPlayerService().PlayPrevious();
                base.OnSkipToPrevious();
            }

            public override async void OnStop()
            {
                await mediaPlayerService.GetMediaPlayerService().Stop();
                base.OnStop();
            }
        }
    }

    public class MediaPlayerServiceBinder : Binder
    {
        private MediaPlayerService service;

        public MediaPlayerServiceBinder (MediaPlayerService service)
        {
            this.service = service;
        }

        public MediaPlayerService GetMediaPlayerService ()
        {
            return service;
        }
    }
}
