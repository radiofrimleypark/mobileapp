using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvxRecyclerView = MvvmCross.DroidX.RecyclerView.MvxRecyclerView;
using MvxRecyclerAdapter = MvvmCross.DroidX.RecyclerView.MvxRecyclerAdapter;
using RadioFrimleyPark.Core.ViewModels;
using RadioFrimleyPark.Core.ViewModels.Main;
using RadioFrimleyPark.Droid.Views.Base;
using MvvmCross.DroidX;
using Google.Android.Material.AppBar;
using Android;

namespace RadioFrimleyPark.Droid.Views
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame)]
    public class GalleryFragment : BaseFragment<GalleryViewModel>
    {
        protected override int FragmentLayoutId => Resource.Layout.fragment_gallery;

        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = base.OnCreateView(inflater, container, savedInstanceState);

            MvxRecyclerView recycler = rootView.FindViewById<MvxRecyclerView>(Resource.Id.gallery_recycler);
            recycler.Adapter = new MvxRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
#if false
            recycler.Adapter.ItemClick = new MvxAsyncCommand<NewsItem>((news) => {  });
#endif
            var swipeToRefresh = rootView.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.gallery_refresher);
            var appBar = Activity.FindViewById<AppBarLayout>(Resource.Id.appbar);
            if (swipeToRefresh != null && appBar != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

#if false
            recycler = rootView.FindViewById<RecyclerView>(Resource.Id.gallery_recycler);

            //ScheduleAdapter adapter = new ScheduleAdapter(this.Activity, schedule);

            GalleryAdapter adapter = new GalleryAdapter(this.Activity, await ViewModel.GetGallery());
            //adapter.NotifyDataSetChanged();
            adapter.ItemClick += OnItemClick;
            recycler.SetAdapter(adapter);

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(this.Activity);
            recycler.SetLayoutManager(layoutManager);
#endif

            return rootView;
        }
        private void OnItemClick(object o, int i)
        {
        }

    }
}
