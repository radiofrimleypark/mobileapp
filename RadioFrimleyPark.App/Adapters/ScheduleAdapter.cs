using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using RadioFrimleyPark.App.Fragments;
using RadioFrimleyPark.App.Models;

namespace RadioFrimleyPark.App.Adapters
{
    public class ScheduleAdapter : RecyclerView.Adapter
    {
        public readonly Schedule schedule;
        private Context context;

        public event EventHandler<int> ItemClick;

        public ScheduleAdapter(Context context, Schedule schedule)
        {
            this.schedule = schedule;
            this.context = context;
        }

        public override int ItemCount
        {
            get { return schedule.Count; }
        }
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ScheduleViewHolder vh = holder as ScheduleViewHolder;

            Programme programme = schedule[position];
            DateTime endTime = programme.StartTime.AddMinutes(programme.Duration);

            vh.startTime.Text = programme.StartTime.ToShortTimeString();
            vh.endTime.Text = endTime.ToShortTimeString();
            vh.programmeName.Text = programme.ProgrammeName;

            if (programme.DayOfWeek == DateTime.Today.DayOfWeek &&
                programme.StartTime < DateTime.Now &&
                endTime > DateTime.Now)
            {
                vh.ItemView.SetBackgroundColor(Color.ParseColor("#ff00478B"));
                vh.startTime.SetTextColor(Color.ParseColor("#ffffffff"));
                vh.endTime.SetTextColor(Color.ParseColor("#ffffffff"));
                vh.programmeName.SetTextColor(Color.ParseColor("#ffffffff"));
            }
            else
            {
                vh.ItemView.SetBackgroundColor(Color.ParseColor("#ffc0c0c0"));
                vh.startTime.SetTextColor(Color.ParseColor("#ff00478B"));
                vh.endTime.SetTextColor(Color.ParseColor("#ff00478B"));
                vh.programmeName.SetTextColor(Color.ParseColor("#ff00478B"));
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Programme, parent, false);
            ScheduleViewHolder vh = new ScheduleViewHolder(itemView, OnClick);
            return vh;
        }

        public class ScheduleViewHolder : RecyclerView.ViewHolder
        {
            public ImageView image { get; private set; }
            public string link { get; set; }
            public TextView startTime { get; set; }
            public TextView endTime { get; set; }
            public TextView programmeName { get; set; }
            public ScheduleViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                startTime = itemView.FindViewById<TextView>(Resource.Id.startTime);
                endTime = itemView.FindViewById<TextView>(Resource.Id.endTime);
                programmeName = itemView.FindViewById<TextView>(Resource.Id.programmeName);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}