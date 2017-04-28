using System;
using Android.App;
using Android.Content;
using android = Android;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Json;
using System.Linq;
using Android.Gms.Ads;

namespace JAM.WebScraper.Android
{
    [Activity(Label = "JAMTech.cl AIO", MainLauncher = true, ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected AdView adView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            
            var buttonTorrent = FindViewById<Button>(Resource.Id.buttonTorrent);
            buttonTorrent.Click += delegate
            {
                RunOnUiThread(() =>
                {
                    StartActivity(typeof(TorrentActivity));
                });
            };


            var buttonDownloadables = FindViewById<Button>(Resource.Id.buttonDownloadables);
            buttonDownloadables.Click += delegate
            {
                RunOnUiThread(() =>
                {
                    StartActivity(typeof(DownloadablesActivity));
                });
            };

            //Ads
            adView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            adView.LoadAd(adRequest);
        }
    }
}

