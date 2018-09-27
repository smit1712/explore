using System;
using Android;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Content;
using Android.Views;
using Android.Locations;
using ExploreTheWorld.Classes;
using Android.Widget;

using System.Collections.Generic;
using System;
using System.Timers;
using ExploreTheWorld.Services;
using System.Globalization;
using System.Threading;

namespace ExploreTheWorld
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener 
    {
        public LocationManager LocationManager;        
        RelativeLayout MainLayout;
        LinearLayout placesLayout;
        LinearLayout VisitedLayout;
        List<string> PlacesResult = new List<string>();
        ListView placeslistview;
        ListView Visitedlistview;
        NavigationView navigationView;
        SaveData SD;
        string Country;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Intent LocationIntent = new Intent(this, typeof(LocationService));
            StartForegroundService(LocationIntent);

            SD = new SaveData();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);           
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
            fab.Visibility = ViewStates.Gone;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
            infotext = FindViewById<Android.Support.V7.Widget.AppCompatTextView>(Resource.Id.Addressinformation);

            MainLayout = FindViewById<RelativeLayout>(Resource.Id.Maincontent);
            placesLayout = FindViewById<LinearLayout>(Resource.Id.Places);
            placeslistview = FindViewById<ListView>(Resource.Id.PlacesListview);
            VisitedLayout = FindViewById<LinearLayout>(Resource.Id.VisitedPlaces);
            Visitedlistview = FindViewById<ListView>(Resource.Id.VisitedListview);


            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Interval = 60000;
            aTimer.Elapsed += Ontimedevent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            Updateinfotext();
        }


        private void Ontimedevent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Updateinfotext();
        }

        public Android.Support.V7.Widget.AppCompatTextView infotext;

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
           
            Snackbar.Make(view, "Visited Places Deleted", Snackbar.LengthLong).SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();            
            List<string> testlist1 = new List<String>();
            testlist1.Add("Harlingen");
            testlist1.Add("Franeker");
           //SD.SaveList(testlist1);
            SD.DeleteAll();
        }
        private void Updateinfotext()
        {
            
            WikipediaAPI WAPI = new WikipediaAPI();
            LocationManager = (LocationManager)GetSystemService(LocationService);
            mygps gps = new mygps(LocationManager, this,SD);
            infotext.Text = WAPI.Getinfo(gps.GetAddress(),gps.GetCountry());
            Country = gps.GetCountry();
            //infotext.Text = WAPI.Getinfo("Harlingen");
            infotext.TextSize = 20;           
        }
        private void Changeinfotext(string place,string country)
        {
            WikipediaAPI WAPI = new WikipediaAPI();
            infotext.Text = WAPI.Getinfo(place,country);
            MainLayout.Visibility = ViewStates.Visible;
            placesLayout.Visibility = ViewStates.Gone;
            VisitedLayout.Visibility = ViewStates.Gone;
            navigationView.SetCheckedItem(Resource.Id.About);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.About)
            {
                //show Mainlayout
                OpenMain();
            }
            else if (id == Resource.Id.Places)
            {                
                //show places
                OpenPlaces();
            }
            else if (id == Resource.Id.VisitedPlaces)
            {
                OpenVisitedPlaces();
            }          
           

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        public void OpenMain()
        {
            Updateinfotext();
            MainLayout.Visibility = ViewStates.Visible;
            placesLayout.Visibility = ViewStates.Gone;
            VisitedLayout.Visibility = ViewStates.Gone;
        }
        public void OpenPlaces()
        {
            MainLayout.Visibility = ViewStates.Gone;
            VisitedLayout.Visibility = ViewStates.Gone;
            placesLayout.Visibility = ViewStates.Visible;
            PlacesAPI places = new PlacesAPI();
            mygps gps = new mygps(LocationManager, this,SD);
            List<string> Points = new List<string>();
            Points = places.Getinfo(gps.GetCoordinate(), "museum");
            Points.AddRange(places.Getinfo(gps.GetCoordinate(), "cafe"));
            Points.AddRange(places.Getinfo(gps.GetCoordinate(), "restaurant"));
            if (Points.Count == 0)
            {
                string error = "Could Find any interesting places or no connection to google!";
                Points.Add(error);
            }
            ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Points);
            placeslistview.Adapter = arrayAdapter;

        }
        public void OpenVisitedPlaces()
        {
            MainLayout.Visibility = ViewStates.Gone;
            VisitedLayout.Visibility = ViewStates.Visible;
            placesLayout.Visibility = ViewStates.Gone;
            ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, SD.GetList());
            Visitedlistview.Adapter = arrayAdapter;
            Visitedlistview.ItemClick += Visitedlistview_ItemClick;
        }

        private void Visitedlistview_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = this.Visitedlistview.GetItemAtPosition(e.Position);
            string place = item.ToString();
            Changeinfotext(place, Country);
        }

    }
}

