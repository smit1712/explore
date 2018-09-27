using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Json;
using Xamarin.Essentials;

namespace ExploreTheWorld.Classes
{
    class PlacesAPI
    {
        public List<string> Getinfo(string address, string type)
        {
           
            //address = "52.379189, 4.899431";
            string importstring;
            //string type = "point_of_interest";
            List<string> result = new List<string>();
            string URL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
                "location=" + address + "" +
                "&radius=1500" +
                "&type=" + type + "" +
                "&key=AIzaSyB14uIG5whsFqi082hecZtzjPibj4r0KjU";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            WebResponse response = request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                importstring = reader.ReadToEnd();
                JSONObject places = new JSONObject(importstring);
                JSONArray results = places.GetJSONArray("results");
                string status = places.GetString("status");
                int test = places.Length();
                if (status == "OK")
                {
                    if (places.Length() == 3 )
                    {
                        JSONObject Name = results.GetJSONObject(0);
                        result.Add(Name.GetString("name").ToString() + "   ||   " + Name.GetString("vicinity").ToString());

                    }
                    else
                    {
                        for (int i = 0; i < places.Length(); i++)
                        {
                            JSONObject Name = results.GetJSONObject(i);
                            
                            result.Add(Name.GetString("name").ToString() + "   ||   " + Name.GetString("vicinity").ToString());
                        }
                    }
                }
            }
            return result;


        }
    }
}