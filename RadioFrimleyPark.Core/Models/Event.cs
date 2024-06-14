using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadioFrimleyPark.Appz.Models
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
