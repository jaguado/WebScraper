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
    [Activity(Label = "Download files", ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    public class DownloadablesActivity : Activity
    {
        const string downloadablesApiUrl = "http://www.jamtech.cl/api/Downloadables?urls={0}&extension={1}";
        private List<dto.DownloadResult> results = null;
        private ProgressDialog dialog = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Downloadables);
          
            //Torrent init
            var buttonSearchDownloadables = FindViewById<Button>(Resource.Id.buttonSearchDownloadables);
            var buttonDownloadDownloadables = FindViewById<Button>(Resource.Id.buttonDownload);  
            var textUrlDownloadables = FindViewById<EditText>(Resource.Id.textSearchDownload);
            var textExtensionDownloadables = FindViewById<EditText>(Resource.Id.textExtensionDownload);
            var statusDownloadables = FindViewById<TextView>(Resource.Id.textStatusDownload);
            var listViewDownloadables = FindViewById<ListView>(Resource.Id.ListViewDownload);

            buttonDownloadDownloadables.Enabled = false;
            //listViewDownloadables.ItemClick += OnDownloadablesListItemClick;
            buttonSearchDownloadables.Click += delegate {
                try
                {
                    RunOnUiThread(() =>
                    {
                        //Loading dialog
                        dialog = ProgressDialog.Show(this, "", "Loading", true);
                        dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    });

                    //Search torrents and show results
                    SearchDownloadables(textUrlDownloadables.Text, textExtensionDownloadables.Text).ContinueWith(files =>
                    {               
                        results = null;
                        if (!files.IsFaulted)
                        {
                            RunOnUiThread(() =>
                            {
                               statusDownloadables.Text = string.Format("{0} files found by JAMTech.cl!!", files.Result.Count);
                            });
                            //Initializing listview
                            if (listViewDownloadables != null)
                            {
                                results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dto.DownloadResult>>(files.Result.ToString());
                            }
                        }
                        else
                        {
                            statusDownloadables.Text = string.Format("ERROR !!");
                            throw files.Exception;
                        }    
                        }).ContinueWith(final=>
                        {
                            RunOnUiThread(() =>
                            {
                                listViewDownloadables.Adapter = new CustomListAdapterDownloadables(this, results);
                                dialog.Dismiss();
                                Helpers.UI.HideKeyboard(this);
                                if (results.Any())
                                    buttonDownloadDownloadables.Enabled = true;
                            });
                        });
                }
                catch(Exception e)
                {
                   Helpers.UI.ShowToast(this, "Error: " + e.ToString(), ToastLength.Long);
                }
            };

            buttonDownloadDownloadables.Click += delegate
            {
                var adpTemp = (CustomListAdapterDownloadables) listViewDownloadables.Adapter;
                for(int i=0; i < adpTemp.Count; i++)
                {
                    var item = adpTemp.List[i];
                    if (item.Selected)
                    {
                        var intent = new Intent(Intent.ActionView, android.Net.Uri.Parse(item.Url));
                        StartActivity(intent);
                    }
                }
            };

            //Ads
            Helpers.Ads.InitAds(FindViewById<AdView>(Resource.Id.adViewDownloadables));
        }

        static async Task<JsonValue> SearchDownloadables(string url, string extension)
        {
            return await Helpers.Http.GetResponse(string.Format(downloadablesApiUrl, url, extension));
        }

        void OnDownloadablesListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (results != null && e.Position >= 0 && results.Count > e.Position)
            {
                var selected = results[e.Position];
                var link = selected.Url;
                if (!string.IsNullOrEmpty(link))
                {
                    var intent = new Intent(Intent.ActionView, android.Net.Uri.Parse(link));
                    StartActivity(Intent.CreateChooser(intent, "Select App"));
                }
            } 
        }
    }
}

