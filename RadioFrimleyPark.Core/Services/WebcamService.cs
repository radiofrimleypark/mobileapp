using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using ModernHttpClient;

namespace RadioFrimleyPark.Core.Services
{
    public class WebcamService : IWebcamService
    {
        private const string url = "http://www.radiofrimleypark.co.uk/webcam/webcam{0}.jpg";
        private Timer _timer;
        private int _index = 1;
        private EntityTagHeaderValue _etag;
        public event EventHandler<ImageEventArgs> ImageReady;

        public WebcamService()
        {
            _timer = new Timer(OnTimerAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }


        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        public void Start()
        {
            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }


        
        public async void OnTimerAsync(object state)
        {

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, string.Format(url, _index)),
                    HttpCompletionOption.ResponseHeadersRead);
                if (_etag == null || result.Headers.ETag.Tag != _etag.Tag)
                {
                    _etag = result.Headers.ETag;
                    var stream = await result.Content.ReadAsStreamAsync();
                    try
                    {
                        ImageReady?.Invoke(this, new ImageEventArgs(stream));
                    }
                    catch
                    { }
                    if (_etag != null)
                        Console.WriteLine("Updated: " + _etag.Tag);
                }
                else
                    Console.WriteLine("Not updated: " + _etag.Tag);
            }
        }
    }
    public class ImageEventArgs : EventArgs
    {
        public Stream Stream { set; get; }
        public ImageEventArgs(Stream stream)
        {
            Stream = stream;
        }
    }
}
