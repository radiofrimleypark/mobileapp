using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using RadioFrimleyPark.Appz.Models;

namespace RadioFrimleyPark.Core.Services
{
    public class ScheduleService : IScheduleService
    {
        const string url = @"http://www.radiofrimleypark.co.uk/api/programmes.php";
        public async Task<Schedule> GetTodaysScheduleAsync()
        {
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                return JsonConvert.DeserializeObject<Schedule>(await client.GetStringAsync(url));
            }
        }
    }
}
