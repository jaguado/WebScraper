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
using JAM.WebScraper.Android.Helpers;

namespace JAM.WebScraper.Android
{
    [Activity(Label = "Torrent Search", ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    public class TorrentActivity : Activity
    {
        const string torrentApiUrl = "https://jamtech.herokuapp.com/api/Torrent?search=";
        private List<dto.TorrentResult> results = null;
        private ProgressDialog dialog = null;
        private InterstitialAd ad = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Torrent);

            //Ads
            ad = Ads.ConstructFullPageAdd(this, "ca-app-pub-5755979550511763/1796857138");
            var intlistener = new Ads.adlistener();
            intlistener.AdLoaded += () => { if (ad.IsLoaded) ad.Show(); };
            ad.AdListener = intlistener;
            ad.CustomBuild();
            

            //Torrent init
            var buttonTorrent = FindViewById<Button>(Resource.Id.buttonSearchTorrent);
            var textTorrent = FindViewById<EditText>(Resource.Id.textSearchTorrent);
            var statusTorrent = FindViewById<TextView>(Resource.Id.textStatusTorrent);
            var listViewTorrent = FindViewById<ListView>(Resource.Id.ListViewTorrent);

            listViewTorrent.ItemClick += OnTorrentListItemClick;
            buttonTorrent.Click += delegate {
                try
                {
                    RunOnUiThread(() =>
                    {
                        //Loading dialog
                        dialog = ProgressDialog.Show(this, "", "Loading", true);
                        dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    });
                    
                    //Search torrents and show results
                    SearchTorrents(textTorrent.Text).ContinueWith(moviesFound =>
                    {               
                        results = null;
                        if (!moviesFound.IsFaulted)
                        {
                            RunOnUiThread(() =>
                            {
                                statusTorrent.Text = string.Format("{0} torrent found by JAMTech.cl!!", moviesFound.Result.Count);
                            });
                            //Initializing listview
                            if (listViewTorrent != null)
                            {
                                results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dto.TorrentResult>>(moviesFound.Result.ToString());
                            }
                        }
                        else
                        {
                            statusTorrent.Text = string.Format("ERROR !!");
                            throw moviesFound.Exception;
                        }    
                        }).ContinueWith(final=>
                        {
                            RunOnUiThread(() =>
                            {
                                listViewTorrent.Adapter = new CustomListAdapterTorrents(this, results);
                                dialog.Dismiss();
                                Helpers.UI.HideKeyboard(this);
                            });
                        });
                }
                catch(Exception e)
                {
                   Helpers.UI.ShowToast(this, "Error: " + e.ToString(), ToastLength.Long);
                }
            };


        }

        static async Task<JsonValue> SearchTorrents(string textToSearch)
        {
            return await Helpers.Http.GetResponse(torrentApiUrl + textToSearch);
        }

        void OnTorrentListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (results != null && e.Position >= 0 && results.Count > e.Position)
            {
                var selected = results[e.Position];
                var link = selected.Links.FirstOrDefault(x => x.Item2.StartsWith("magnet:")).Item2;
                if (!string.IsNullOrEmpty(link))
                {
                    var intent = new Intent(Intent.ActionView, android.Net.Uri.Parse(link));
                    StartActivity(Intent.CreateChooser(intent, "Select App"));
                }
            } 
        }
    }
}

