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
using ExploreTheWorld.Services;
using Xamarin.Essentials;

namespace ExploreTheWorld.Classes
{
    class SaveData
    {
        public void SaveAddString(string str)
        {
            List<string> CurrentList = GetList();
            if (!CurrentList.Contains(str))
            {
                int count = Getsize();
                var LocalData = Application.Context.GetSharedPreferences("SavedLocations", FileCreationMode.Private);
                var DataEdit = LocalData.Edit();
                string DataName = "Location" + count.ToString();
                DataEdit.PutString(DataName, str);
                DataEdit.Commit();
            }
           
        }

        public void SaveList(List<string> Ls)
        {
            List<string> CurrentList = GetList();
            List<string> CheckedList = new List<string>();
            foreach(string str in Ls)
            {
                if (!CurrentList.Contains(str))
                {
                    CheckedList.Add(str);
                }
            }
            int count = Getsize();
            var LocalData = Application.Context.GetSharedPreferences("SavedLocations", FileCreationMode.Private);
            var DataEdit = LocalData.Edit();
            foreach (string str in CheckedList)
            {

                string DataName = "Location" + count.ToString();
                DataEdit.PutString(DataName, str);
                count++;
            }
            DataEdit.Commit();
        }
        public List<string> GetList()
        {
            int count = 0;
            List<string> Getlist = new List<string>();
            var LocalData = Application.Context.GetSharedPreferences("SavedLocations", FileCreationMode.Private);
            while (LocalData.GetString("Location" + count, null) != null)
            {
                Getlist.Add(LocalData.GetString("Location" + count, null));
                count++;
            }
            int DeleteCount = 0;
            foreach (string str in Getlist)
            {
                if (str == "Can't Find location")
                {
                    DeleteCount++;
                }
            }
            while (DeleteCount > 0)
            {
                Getlist.Remove("Can't Find location");
                DeleteCount--;
            }
            
            return Getlist;       

        }
        public int Getsize()
        {
            int count = 0;
            var LocalData = Application.Context.GetSharedPreferences("SavedLocations", FileCreationMode.Private);
            while (LocalData.GetString("Location" + count, null) != null)
            {                
                count++;
            }
            //count++;
            return count;
        }
        public bool Contains(string str)
        {
            List<string> CurrentList = GetList();
            if (CurrentList.Contains(str))
            {
                return true;
            }
            else
            {
                SaveAddString(str);
                return false;
            }
        }
        public void DeleteAll()
        {
            var LocalData = Application.Context.GetSharedPreferences("SavedLocations", FileCreationMode.Private);
            var EditData = LocalData.Edit();
            EditData.Clear();
            EditData.Commit();
        }

    }
}