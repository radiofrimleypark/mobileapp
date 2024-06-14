using System;
using Android.App;
//using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Java.Lang;
using RadioFrimleyPark.App.Adapters;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using String = System.String;
using System.Collections.Generic;

namespace RadioFrimleyPark.App
{
    [Activity(Label = "@string/app_name", 
        Theme = "@style/Theme", 
        MainLauncher = true, 
        Icon = "@drawable/rfp_launcher")]
    public class MainActivity : AppCompatActivity
    {
        private DrawerLayout drawerLayout;
        private NavigationView navigationView;
        private ViewPager viewPager;
        private TabLayoutAdapter mAdapter;

        private List<string> tabs = new List<string> { "Schedule", "Webcam 1", "Webcam 2", "Listen", "Gallery" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            navigationView.NavigationItemSelected += (sender, e) => {
                e.MenuItem.SetChecked(true);
                //react to click here and swap fragments or navigate
                drawerLayout.CloseDrawers();
            };

            viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            viewPager.PageSelected += ViewPager_PageSelected;

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabs);
            tabLayout.TabGravity = TabLayout.GravityFill;
            tabLayout.SetupWithViewPager(viewPager);

            mAdapter = new TabLayoutAdapter(this.SupportFragmentManager, tabs);

            viewPager.Adapter = mAdapter;
        }

        private void ViewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            viewPager.SetCurrentItem(e.Position, true);
        }

#region Menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Toast.MakeText(this, "Action selected: " + item.TitleFormatted, ToastLength.Short).Show();
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;

                case Resource.Id.menu_preferences:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
#endregion

    }
}

