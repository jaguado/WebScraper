using System;
using Android;
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
using System.IO;
using System.Web;
using System.Threading;
using JAM.WebScraper.Android.Helpers;

namespace JAM.WebScraper.Android
{
    [Activity(Label = "Download files", ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    public class DownloadablesResultsActivity : Activity
    {
        const string downloadablesApiUrl = "https://jamtech.herokuapp.com/api/Downloadables?urls={0}&extension={1}&levels={2}";
        public List<dto.DownloadResult> results = null;
        private InterstitialAd ad = null;
        private static string LastSearch = string.Empty;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.DownloadablesResults);

            //Ads
            ad = Ads.ConstructFullPageAdd(this, "ca-app-pub-5755979550511763/6227056731");
            var intlistener = new Ads.adlistener();
            intlistener.AdLoaded += () => { if (ad.IsLoaded) ad.Show(); };
            ad.AdListener = intlistener;
            ad.CustomBuild();

            //Torrent init
            var buttonDownloadDownloadables = FindViewById<Button>(Resource.Id.buttonDownload);  
            var listViewDownloadables = FindViewById<ListView>(Resource.Id.ListViewDownload);
            var checkBoxAll = FindViewById<CheckBox>(Resource.Id.checkBoxAll);

            //Get results
            LastSearch = Intent.GetStringExtra("lastSearch");
            var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dto.DownloadResult>>(Intent.GetStringExtra("results")).Where(x=>!x.IsDirectory).ToList();
            if (results != null)
            {
                listViewDownloadables.Adapter = new CustomListAdapterDownloadables(this, results);
                checkBoxAll.Enabled = buttonDownloadDownloadables.Enabled = results.Any();
            }

            buttonDownloadDownloadables.Click += delegate
            {
                RunOnUiThread(() => { buttonDownloadDownloadables.Enabled = false; });
                var adpTemp = (CustomListAdapterDownloadables) listViewDownloadables.Adapter;
                //var count = DownloadAll(this, adpTemp.List.Where(x => x.Selected));
                for (int i = 0; i < adpTemp.Count; i++)
                {
                    var item = adpTemp.List[i];
                    if (item.Selected)
                    {
                        //Async download with progressBar
                        Download(this, item);
                    }
                }
                //Wait until all donwload finished
                Task.Run(() =>
                {
                    while (adpTemp.List.Any(x => x.Selected && x.DownloadProgress < 100))
                        Thread.Sleep(10);

                    RunOnUiThread(() =>
                    {
                        UI.ShowAlertOk(this, "Download Finished", string.Format("{0} files downloaded.", adpTemp.List.Count(x=>x.DownloadProgress==100)), (senderAlert, args) =>
                            {
                                if (ad != null)
                                {
                                    if(!ad.IsLoaded)
                                        ad.CustomBuild();
                                    else
                                        ad.Show();
                                }
                            });
                    });
                }).ContinueWith(x=>
                {
                    RunOnUiThread(() => {
                        buttonDownloadDownloadables.Enabled = true;
                        checkBoxAll.Checked = false;
                    });
                    
                });
            };

            checkBoxAll.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                var adapter = (CustomListAdapterDownloadables)listViewDownloadables.Adapter;
                if (adapter != null && adapter.List != null && adapter.List.Any())
                {
                    adapter.List.ForEach(x => x.Selected = e.IsChecked);
                    adapter.NotifyDataSetChanged();
                }
            };
           
        }

        private static DateTime lastProgressBarUpdateTime = DateTime.MinValue;
        private static string DownloadsPath = android.OS.Environment.ExternalStorageDirectory + "/Download"; // The direction is Download folder in Device store. A file will be save here.
        async static Task<bool> Download(Activity context, dto.DownloadResult item)
        {
            using(var webClient = new System.Net.WebClient())
            {
                var view = context.FindViewById<ListView>(Resource.Id.ListViewDownload);
                var adapter = (CustomListAdapterDownloadables)view.Adapter;
                webClient.DownloadProgressChanged += delegate(object sender, System.Net.DownloadProgressChangedEventArgs e)
                {
                    item.DownloadProgress = e.ProgressPercentage;
                    //Update progress bar value on the view
                    if (lastProgressBarUpdateTime < DateTime.Now.AddSeconds(-5))
                    {
                        adapter.NotifyDataSetChanged();
                        lastProgressBarUpdateTime = DateTime.Now;
                    }
                };
          
                try
                {
                    //TODO Fixing baseUrl on client side
                    var baseUrl = LastSearch;
                    while (baseUrl.EndsWith("/"))
                    {
                        baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                    }
                    item.BaseUrl = baseUrl;

                    var fileName = Path.GetFileNameWithoutExtension(HttpUtility.UrlDecode(baseUrl.Substring(baseUrl.LastIndexOf("/") + 1)));

                    var path = Path.Combine(DownloadsPath, fileName);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    //Save using writeAsync
                    int bytesCount = 0;
                    var finalDirectory = string.Concat(path, "/", item.RelativePath);
                    var finalPath = string.Concat(finalDirectory, "/", item.Name);
                    Console.WriteLine("RelativePath: " + item.RelativePath);
                    Console.WriteLine("finalPath: " + finalPath);

                    if (!Directory.Exists(finalDirectory))
                        Directory.CreateDirectory(finalDirectory);

                    if (!File.Exists(finalPath))
                    {
                        Console.WriteLine(finalPath + " don't exist, downloading....");
                        var bytes = await webClient.DownloadDataTaskAsync(item.Url);
                        var fs = new FileStream(finalPath, FileMode.OpenOrCreate);
                        await fs.WriteAsync(bytes, 0, bytes.Length);// writes to Download folder 
                        fs.Close();
                        bytesCount = bytes.Length;
                        return true;
                    }

                    //File downloaded
                    item.DownloadProgress = 100;
                    item.Selected = false;
                    adapter.NotifyDataSetChanged();
                    Console.WriteLine("{0} bytes downloaded of file {1}", bytesCount, item.Name);
                }
                catch (TaskCanceledException te)
                {
                    UI.ShowToast(context, "Canceled: " + te.ToString(), ToastLength.Long);
                }
                catch (Exception a)
                {
                    UI.ShowToast(context, "ERROR: " + a.ToString(), ToastLength.Long);
                }
            }
            return false;
        }
        static int DownloadAll(Activity context, IEnumerable<dto.DownloadResult> items)
        {
            var TaskList = new List<Task>();
            items.ToList().ForEach(item => 
            {
                var task = new Task(() =>
                {
                    Download(context, item);
                });
                task.Start();
                TaskList.Add(task);
            });
            Task.WhenAll(TaskList.ToArray());
            return TaskList.Count();
        }

        static async Task<JsonValue> SearchDownloadables(string url, string extension, int levels = 0)
        {
            return await Http.GetResponse(string.Format(downloadablesApiUrl, url, extension, levels));
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

