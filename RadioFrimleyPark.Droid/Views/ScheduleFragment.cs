using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using RadioFrimleyPark.Droid.Views.Base;
using RadioFrimleyPark.Core.ViewModels;
using RadioFrimleyPark.Core.ViewModels.Main;
using MvvmCross.DroidX;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Resource = Android.Resource;
using MvvmCross.Commands;
using RadioFrimleyPark.Appz.Models;

namespace RadioFrimleyPark.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame)]
    public class ScheduleFragment : BaseFragment<ScheduleViewModel>
    {
        protected override int FragmentLayoutId => Resource.Layout.fragment_schedule;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = base.OnCreateView(inflater, container, savedInstanceState);

            MvxRecyclerView recycler = rootView.FindViewById<MvxRecyclerView>(Resource.Id.schedule_recycler);
            recycler.Adapter = new MvxRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
            recycler.Adapter.ItemClick = new MvxAsyncCommand<Programme>(async (site) =>
            {
                // set as current programme
            });

            var image = rootView.FindViewById<ImageView>(Resource.Id.radio_frimley_park);
            image.Click += (object sender, EventArgs e) => {
                var browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(ViewModel.Home.ToString()));
                StartActivity(browserIntent);
            };  

            var swipeToRefresh = rootView.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.schedule_refresher);
            var appBar = Activity.FindViewById<AppBarLayout>(Resource.Id.appbar);
            if (swipeToRefresh != null && appBar != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

            var toolbar = this.Activity.FindViewById<Toolbar>(Resource.Id.toolbar);
            var cast = toolbar.FindViewById<ImageButton>(Resource.Id.cast);
            cast.Click += (sender, args) =>
            {
                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new GoogleCastFragment();
                dialogFragment.Show(transaction, "google_cast_fragment");
            };

            return rootView;
        }

        private void OnItemClick(object o, int i)
        {
        }
    }
}
