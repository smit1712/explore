using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExploreTheWorld.Classes;
using Xamarin.Essentials;

namespace ExploreTheWorld.Services
{
    //[Service(Exported = true, Name = "ExploreTheWorld.Location.Service" , Process = ":ExploreTheWorld_LocationService")]
    [Service][IntentFilter(new String[] { "ExploreTheWorld.Location.Service" })]
    public class LocationService : Service
    {
        private LocationManager LocationManager;
        public const int NOTIFICATION_ID = 9001;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 9002;
        PendingIntent pendingIntent;
        SaveData SD;
        mygps GPS;
        // Magical code that makes the service do wonderful things.
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            const int pendingIntentId = 0;
            Intent Mainintent = new Intent(this, typeof(MainActivity));
            pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, Mainintent, PendingIntentFlags.OneShot);

            //StartForegroundService();
            // This method executes on the main thread of the application.
            var notification = new Notification.Builder(this)
            .SetContentTitle(Resources.GetString(Resource.String.app_name))
            .SetContentText(Resources.GetString(Resource.String.notification_text))
            .SetSmallIcon(Resource.Drawable.ic_explore_black_18dp)
            .SetContentIntent(pendingIntent)
            .SetOngoing(true)
            //.AddAction(BuildRestartTimerAction())
            //.AddAction(BuildStopServiceAction())
            .Build();

            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);

            LocationManager = (LocationManager)GetSystemService(LocationService);
            SD = new SaveData();
            GPS = new mygps(LocationManager, this, SD);

            Timer Btimer = new System.Timers.Timer();
            Btimer.Interval = 30000;
            Btimer.Elapsed += Ontimedevent;
            Btimer.AutoReset = true;
            Btimer.Enabled = true;

           
            return StartCommandResult.Sticky;
        }

        public override bool StopService(Intent name)
        {
            return base.StopService(name);
        }


        private void Ontimedevent(object sender, ElapsedEventArgs e)
        {
            Xamarin.Essentials.NetworkAccess networkAccess = Connectivity.NetworkAccess;           

            if (!SD.Contains(GPS.GetAddress()) && networkAccess == Xamarin.Essentials.NetworkAccess.Internet && LocationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                Notification.Builder notificationBuilder = new Notification.Builder(this)
               .SetSmallIcon(Resource.Drawable.ic_explore_black_18dp)
               .SetContentIntent(pendingIntent)
               .SetContentTitle("Explore The World")
               .SetContentText("You have Found Something");

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());
            }                       
        }
    }
}