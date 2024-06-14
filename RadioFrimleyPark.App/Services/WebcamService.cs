using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Newtonsoft.Json;

namespace RadioFrimleyPark.Services
{

    [Service]
    public class WebcamService : Service
    {
        static readonly string TAG = "X:" + typeof(WebcamService).Name;
        static readonly int TimerWait = 4000;
        Timer timer;
        DateTime startTime;
        bool isStarted = false;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(TAG, $"OnStartCommand called at {startTime}, flags={flags}, startid={startId}");
            if (isStarted)
            {
                TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
                Log.Debug(TAG, $"This service was already started, it's been running for {runtime:c}.");
            }
            else
            {
                startTime = DateTime.UtcNow;
                Log.Debug(TAG, $"Starting the service, at {startTime}.");
                timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
                isStarted = true;
            }
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }


        public override void OnDestroy()
        {
            timer.Dispose();
            timer = null;
            isStarted = false;

            TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(TAG, $"Simple Service destroyed at {DateTime.UtcNow} after running for {runtime:c}.");
            base.OnDestroy();
        }

        async void HandleTimerCallback(object state)
        {
            string url = "https://www.radiofrimleypark.co.uk//webcam/webcam1.jpg";

            TimeSpan runTime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(TAG, $"This service has been running for {runTime:c} (since ${state}).");


            string etag = null;
            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.GetAsync(url, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);
                if (response.Headers.ETag.ToString() != etag)
                {
                    etag = response.Headers.ETag.ToString();
                    var result = await response.Content.ReadAsStringAsync();
                    var webcam = JsonConvert.DeserializeObject<Schedule>(result);
                }
            };


        }
    }
}