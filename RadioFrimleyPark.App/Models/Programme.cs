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
    public class Programme
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
    }


    public class Gallery1 : List<Event>
    { }


    public class Media
    {
        public string title { get; set; }
    }
    public class Photo : Media
    {
        public string photo { get; set; }
    }
    public class Video : Media
    {
        public string video { get; set; }
    }
}