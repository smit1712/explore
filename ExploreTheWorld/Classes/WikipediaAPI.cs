using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Generic;
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
    class WikipediaAPI
    {
        public string Getinfo(string address, string country)
        {
            Xamarin.Essentials.NetworkAccess networkAccess = Connectivity.NetworkAccess;

            if (networkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                return "No Internet Connection can be made";
            }
            if (address == "Can't Find location")
            {
                return "No GPS availible";
            }
            string[] importtext;            
            string URL = "https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro=&explaintext=&titles=" + address;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                
                string importstring = reader.ReadLine();

                JSONObject WIKI = new JSONObject(importstring);
                JSONObject query = WIKI.GetJSONObject("query");
                JSONObject pages = query.GetJSONObject("pages");
                string[] Psplit = pages.ToString().Split(':');
                string trimP = Psplit[0];
                trimP = trimP.Trim('"','{','\\');
                if (pages.ToString().Contains("extract") == true && !importstring.Contains("may mean") && !importstring.Contains("may refer"))
                {
                    JSONObject ID = pages.GetJSONObject(trimP);
                    return  ID.GetString("extract");
                }

                importtext = importstring.Split(':');
                importstring = "";
                importtext[0] = "";
                importtext[1] = "";
                importtext[2] = "";
                importtext[3] = "";
                importtext[4] = "";
                importtext[5] = "";
                importtext[6] = "";
                importtext[7] = "";

                foreach(string str in importtext)
                {
                    if (str.Contains("may mean") || str.Contains("may refer"))
                    {
                        WikipediaAPI wapi = new WikipediaAPI();
                        //string[] split = importtext[9].Split(',');
                        //string teststring = importtext[9];
                        return wapi.Getinfo(address + ",_" + country, country);
                    }
                }
               

                foreach (string str in importtext)
                {
                    importstring = importstring + str;
                }
                string finalstring = "";
                char oldchar = ' ';
                foreach (char cr in importstring)
                {
                    if (cr != '{' && cr != '}' && cr != '[' && cr != ']' && cr != '\\' && cr != ';')
                    {
                        if (oldchar != '\\')
                        {
                            finalstring = finalstring + cr;
                        }
                        else
                        {
                            finalstring = finalstring + ' ';
                        }
                    }
                    oldchar = cr;
                }
                return finalstring;
            }
        }
    }
}
