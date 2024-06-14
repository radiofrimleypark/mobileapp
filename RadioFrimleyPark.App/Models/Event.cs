using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RadioFrimleyPark.App.Models
{
    public class Event
    {
        public string eventId { set; get; }
        public string eventTitle { set; get; }
        public DateTime eventDate { set; get; }
        public List<Video> videos { set; get; }
        public List<Photo> photos { set; get; }
    }
}