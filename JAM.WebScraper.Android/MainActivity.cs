using System;
using Android.App;
using Android.Content;
using android = Android;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Json;
using System.Linq;

namespace JAM.WebScraper.Android
{
    [Activity(Label = "JAMTech.cl AIO", MainLauncher = true, ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        const string torrentApiUrl = "http://www.jamtech.cl/api/Torrent?search=";
        private List<dto.TorrentResult> results = null;
        private ProgressDialog dialog = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button>(Resource.Id.MyButton);
            var text = FindViewById<EditText>(Resource.Id.textSearch);
            var status = FindViewById<TextView>(Resource.Id.textStatus);
            //Initializing listview
            var listView = FindViewById<ListView>(Resource.Id.ListView);
            listView.ItemClick += OnListItemClick;

            button.Click += delegate {
                try
                {
                    RunOnUiThread(() =>
                    {
                        //Loading dialog
                        dialog = ProgressDialog.Show(this, "", "Loading", true);
                        dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    });
                    //Search torrents and show results
                    SearchTorrents(text.Text).ContinueWith(moviesFound =>
                    {               
                        results = null;
                        if (!moviesFound.IsFaulted)
                        {
                            RunOnUiThread(() =>
                            {
                                status.Text = string.Format("{0} torrent found by JAMTech.cl!!", moviesFound.Result.Count);
                            });
                            //Initializing listview
                            if (listView != null)
                            {
                                results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dto.TorrentResult>>(moviesFound.Result.ToString());
                            }
                        }
                        else
                        {
                            button.Text = string.Format("ERROR !!");
                            throw moviesFound.Exception;
                        }    
                        }).ContinueWith(final=>
                        {
                            RunOnUiThread(() =>
                            {
                                listView.Adapter = new CustomListAdapterTorrents(this, results);
                                dialog.Dismiss();
                            });
                        });
                }
                catch(Exception e)
                {
                    ShowToast("Error: " + e.ToString(), ToastLength.Long);
                }
                finally
                {
                    
                }
            };
        }

        static async Task<JsonValue> SearchTorrents(string textToSearch)
        {
            return await GetResponse(torrentApiUrl + textToSearch);
        }

        // Gets weather data from the passed URL.
        private static async Task<JsonValue> GetResponse(string url, int timeout = 15000)
        {
            // Create an HTTP web request using the URL:
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Timeout = timeout;
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (var response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (var stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    var jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                    return jsonDoc;
                }
            }
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
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

        void ShowToast(string text, ToastLength length)
        {
            Toast.MakeText(this, text, length);
        }
    }
}

