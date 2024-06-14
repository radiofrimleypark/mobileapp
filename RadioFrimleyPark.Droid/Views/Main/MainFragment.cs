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
using RadioFrimleyPark.Core.ViewModels.Main;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using RadioFrimleyPark.Droid.Views.Base;
using MvvmCross.Platforms.Android.Views.ViewPager;
using Google.Android.Material.Tabs;
using AndroidX.DrawerLayout.Widget;
using AndroidX.ViewPager.Widget;
using MvvmCross.Platforms.Android.Views.AppCompat;
using MvvmCross.ViewModels;
using RadioFrimleyPark.Core.ViewModels;

namespace RadioFrimleyPark.Droid.Views.Main
{
    [MvxFragmentPresentation(typeof(MainContainerViewModel), Resource.Id.content_frame)]
    public class MainFragment : BaseFragment<MainViewModel>
    {
        protected override int FragmentLayoutId => Resource.Layout.fragment_main;

        public DrawerLayout DrawerLayout { get; private set; }
        private ViewPager viewPager;
        protected MvxActionBarDrawerToggle DrawerToggle { get; private set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = base.OnCreateView(inflater, container, savedInstanceState);

            var fragments = new List<MvxViewPagerFragmentInfo>
            {
                new MvxViewPagerFragmentInfo("Gallery","gallery", typeof(GalleryFragment), new MvxViewModelRequest<GalleryViewModel>()),
                new MvxViewPagerFragmentInfo("Webcam","webcam", typeof(WebcamFragment), new MvxViewModelRequest<WebcamViewModel>()),
                new MvxViewPagerFragmentInfo("Schedule","schedule", typeof(ScheduleFragment), new MvxViewModelRequest<ScheduleViewModel>()),
            };

            TabLayout tabLayout = rootView.FindViewById<TabLayout>(Resource.Id.tabs);
            tabLayout.TabGravity = TabLayout.GravityFill;

            viewPager = rootView.FindViewById<ViewPager>(Resource.Id.pager);
            if (viewPager != null)
            {
                tabLayout.SetupWithViewPager(viewPager);
                viewPager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) => {
                    Console.WriteLine(fragments[e.Position].Title);

                    if (fragments[e.Position].FragmentType == typeof(ScheduleFragment))
                        ((ICastAvailable)Activity).SetChromecast(ListenViewModel.StreamUri);
                    else
                        ((ICastAvailable)Activity).SetChromecast(null);
                };

                viewPager.Adapter = new MvxCachingFragmentStatePagerAdapter(Activity, ChildFragmentManager, fragments);
                viewPager.OffscreenPageLimit = fragments.Count;

                tabLayout.SetupWithViewPager(viewPager);
            }

            return rootView;
        }
    }
}
