using System;
using Android.App;
using Android.Content;
using android = Android;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Json;
using Android.Gms.Ads;
using JAM.WebScraper.Android.Helpers;

namespace JAM.WebScraper.Android
{
    [Activity(Label = "Videos Download", ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    public class VideosActivity : Activity
    {
        const string videosApiUrl = "https://jamtech.herokuapp.com/api/Videos?url=";
        private dto.VideosResult result = null;
        private ProgressDialog dialog = null;
        private InterstitialAd ad = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Videos);

            //Ads
            ad = Ads.ConstructFullPageAdd(this, "ca-app-pub-5755979550511763/6495925136");
            var intlistener = new Ads.adlistener();
            intlistener.AdLoaded += () => { if (ad.IsLoaded) ad.Show(); };
            ad.AdListener = intlistener;
            ad.CustomBuild();

          
            //Videos init
            var buttonVideos = FindViewById<Button>(Resource.Id.buttonSearchVideos);
            var textVideos = FindViewById<EditText>(Resource.Id.textSearchVideos);
            var statusVideos = FindViewById<TextView>(Resource.Id.textStatusVideos);
            var listViewVideos = FindViewById<ListView>(Resource.Id.ListViewVideos);

            var url = Intent.GetStringExtra("url");
            if (!string.IsNullOrEmpty(url))
                textVideos.Text = url;


            listViewVideos.ItemClick += ListViewVideos_ItemClick;
            buttonVideos.Click += delegate {
                try
                {
                    RunOnUiThread(() =>
                    {
                        //Loading dialog
                        dialog = ProgressDialog.Show(this, "", "Loading", true);
                        dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    });

                    //Search torrents and show results
                    SearchVideos(textVideos.Text).ContinueWith(videosFound =>
                    {               
                        result = null;
                        if (!videosFound.IsFaulted)
                        {
                            //Initializing listview
                            if (listViewVideos != null)
                            {
                                result = Newtonsoft.Json.JsonConvert.DeserializeObject<dto.VideosResult>(videosFound.Result.ToString());
                                if (result == null)
                                {
                                    result = new dto.VideosResult()
                                    {
                                        info = new dto.Info
                                        {
                                            formats = new List<dto.Format>()
                                        }
                                    };
                                }
                                else
                                {
                                    RunOnUiThread(() =>
                                    {
                                        statusVideos.Text = string.Format("{0} videos found by JAMTech.cl!!", result.info.formats.Count);
                                    });
                                }
                            }
                        }
                        else
                        {
                            statusVideos.Text = string.Format("ERROR !!");
                            throw videosFound.Exception;
                        }    
                        }).ContinueWith(final=>
                        {
                            RunOnUiThread(() =>
                            {
                                listViewVideos.Adapter = new CustomListAdapterVideos(this, result);
                                dialog.Dismiss();
                                UI.HideKeyboard(this);
                            });
                        });
                }
                catch(Exception e)
                {
                   UI.ShowToast(this, "Error: " + e.ToString(), ToastLength.Long);
                }
            };


        }

        private void ListViewVideos_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (result != null && e.Position >= 0)
            {
                var selected = result.info.formats[e.Position];
                var link = selected.url;
                if (!string.IsNullOrEmpty(link))
                {
                    var intent = new Intent(Intent.ActionView, android.Net.Uri.Parse(link));
                    StartActivity(Intent.CreateChooser(intent, "Select App"));
                }
            }
        }

        static async Task<JsonValue> SearchVideos(string textToSearch)
        {
            return await Http.GetResponse(videosApiUrl + textToSearch);
        }

    }
}

