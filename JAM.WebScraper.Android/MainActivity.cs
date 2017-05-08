using Android.App;
using android = Android;
using Android.Widget;
using Android.OS;
using Android.Gms.Ads;
using Android.Content;

namespace JAM.WebScraper.Android
{
    [Activity(Label = "JAMTech.cl AIO", MainLauncher = true, ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    [IntentFilter(new string[] { android.Content.Intent.ActionSend, android.Content.Intent.ActionSendto }, Categories = new string[] {android.Content.Intent.CategoryDefault, android.Content.Intent.CategoryBrowsable }, DataMimeType = "text/plain")]
    public class MainActivity : Activity
    {
        private AdView adView = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var url = Intent.GetStringExtra(android.Content.Intent.ExtraText);
        
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Ads
            adView = FindViewById<AdView>(Resource.Id.adViewMain);
            Helpers.Ads.InitBanner(ref adView);
            
            var buttonTorrent = FindViewById<Button>(Resource.Id.buttonTorrent);
            buttonTorrent.Click += delegate
            {
                RunOnUiThread(() =>
                {
                    var activity = new Intent(this, typeof(TorrentActivity));
                    activity.PutExtra("url", url);
                    StartActivity(activity);
                });
            };


            var buttonDownloadables = FindViewById<Button>(Resource.Id.buttonDownloadables);
            buttonDownloadables.Click += delegate
            {
                RunOnUiThread(() =>
                {
                    var activity = new Intent(this, typeof(DownloadablesActivity));
                    activity.PutExtra("url", url);
                    StartActivity(activity);
                });
            };

            var buttonVideos = FindViewById<Button>(Resource.Id.buttonVideos);
            buttonVideos.Click += delegate
            {
                RunOnUiThread(() =>
                {
                    var activity = new Intent(this, typeof(VideosActivity));
                    activity.PutExtra("url", url);
                    StartActivity(activity);
                });
            };

        }
    }
}

