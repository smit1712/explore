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
using Android.Locations;
using Android;
using Java.Util;
using Xamarin.Essentials;
using System.Globalization;
using System.Threading;

namespace ExploreTheWorld.Classes 
{
    class mygps : Activity, ILocationListener
    {
        private LocationManager locationManager;
        private Android.Locations.Location CurruntLocation;
        private SaveData SD;
        private Context CXT;
        public mygps(LocationManager LManager, Context context, SaveData SD)
        {
            this.SD = SD;
            CXT = context;
            locationManager = LManager;            
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 30000, 1, this);
            CurruntLocation = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
            var userSelectedCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = userSelectedCulture;
        }

        public bool HasSignal()
        {
            if (locationManager.IsProviderEnabled(LocationManager.GpsProvider) == true)
            {
               return true;
            }
            else
            {
               return false;
            }
        }
        public string GetLocation()
        {
            return CurruntLocation.ToString();
        }
        public string GetAddress()
        {            
            Geocoder Geo = new Geocoder(CXT, Java.Util.Locale.English);
            var NetworkAccess = Connectivity.NetworkAccess;
            CurruntLocation = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
           
            if (Geocoder.IsPresent == true && NetworkAccess == NetworkAccess.Internet && CurruntLocation != null)
            {
                IList<Address> Addr = Geo.GetFromLocation(CurruntLocation.Latitude, CurruntLocation.Longitude, 20);
                string tempaddr = Addr[0].Locality.ToString();               
                
                return tempaddr;
                //return "harlingen";
            }
            else
            {
                return "Can't Find location";
            }
        }
        public string GetCountry()
        {
            //Java.Util.Locale english =  new Java.Util.Locale.;
            
            Geocoder Geo = new Geocoder(CXT, Java.Util.Locale.English);
            var NetworkAccess = Connectivity.NetworkAccess;
           
            CurruntLocation = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);

            if (Geocoder.IsPresent == true && NetworkAccess == NetworkAccess.Internet && CurruntLocation != null)
            {
                
                IList<Address> Addr = Geo.GetFromLocation(CurruntLocation.Latitude, CurruntLocation.Longitude, 20);
                string tempcounty = Addr[0].CountryName.ToString();

                return tempcounty;
            }
            else
            {
                return "Can't Find Country";
            }
        }
        
        public string GetCoordinate()
        {
            if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                return null;
            }

            string La = Convert.ToString(CurruntLocation.Latitude);
            string Lo = Convert.ToString(CurruntLocation.Longitude);

            string RLa;
            string RLo;

            RLa = La.Replace(',', '.');
            RLo = Lo.Replace(',', '.');
          
            return RLa + "," + RLo;
        }

        public void OnLocationChanged(Android.Locations.Location location )
        {
            string tempstring = GetAddress();
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }

       
    }
}