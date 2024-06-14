using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RadioFrimleyPark.Appz.Models;

namespace RadioFrimleyPark.Core.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly string _url = @"http://www.radiofrimleypark.co.uk/api/gallery.php?year={0}";

        public GalleryService()
        { }
        public async Task<Gallery1> GetGalleryAsync()
        {
            using (var client = new System.Net.Http.HttpClient(new NativeMessageHandler()))
            {
                return JsonConvert.DeserializeObject<Gallery1>(await client.GetStringAsync(String.Format(_url, DateTime.Today.Year)), new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            };
        }
    }
}
