using Android.App;
using android = Android;
using Android.Widget;
using Android.OS;
using Android.Gms.Ads;

namespace JAM.WebScraper.Android
{
    [Activity(Label = "JAMTech.cl AIO", MainLauncher = true, ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
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
            Helpers.Ads.InitAds(FindViewById<AdView>(Resource.Id.adViewMain));
        }
    }
}

