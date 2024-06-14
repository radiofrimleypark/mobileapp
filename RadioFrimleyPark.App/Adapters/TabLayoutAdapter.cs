using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using String = System.String;
using RadioFrimleyPark.App.Fragments;

namespace RadioFrimleyPark.App.Adapters
{
    public class TabLayoutAdapter : FragmentStatePagerAdapter
    {
        private List<string> _tabs;
        public TabLayoutAdapter(FragmentManager fm, List<string> tabs)
            : base(fm)
        {
            this._tabs = tabs;
        }

        public override Fragment GetItem(int index)
        {

            switch (index)
            {
                case 0:
                    return new ScheduleFragment();
                case 1:
                    return new Webcam1Fragment();
                case 2:
                    return new Webcam2Fragment();
                case 3:
                    return new ListenFragment();
                case 4:
                    return new GalleryFragment();
            }

            return null;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(_tabs[position]);
        }

        public override int Count
        {
            // get item count - equal to number of tabs
            get { return this._tabs.Count; }
        }
    }
}