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
using System.IO;
using System.Web;

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
            var checkBoxAll = FindViewById<CheckBox>(Resource.Id.checkBoxAll);

            checkBoxAll.Enabled = buttonDownloadDownloadables.Enabled = false;
            listViewDownloadables.ItemLongClick += delegate(object sender, AdapterView.ItemLongClickEventArgs e)
            {
                if (results != null && e.Position >= 0 && results.Count > e.Position)
                {
                    var selected = results[e.Position];
                    Helpers.UI.ShowToast(this, "Start downloading " + selected.Name, ToastLength.Long);
                    Download(this, selected, e.Position);
                }
            };
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
                                checkBoxAll.Enabled = buttonDownloadDownloadables.Enabled = results.Any();
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
                        //Async download with progressBar
                        Download(this, item, i);
                        //var intent = new Intent(Intent.ActionView, android.Net.Uri.Parse(item.Url));
                        //StartActivity(intent);
                    }
                }
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
            

            //Ads
            Helpers.Ads.InitAds(FindViewById<AdView>(Resource.Id.adViewDownloadables));
        }

        private static DateTime lastProgressBarUpdateTime = DateTime.MinValue;
        static async void Download(Activity context, dto.DownloadResult item, int position)
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
                    var bytes = await webClient.DownloadDataTaskAsync(item.Url);
                    var downloadsPath = android.OS.Environment.ExternalStorageDirectory + "/Download"; // The direction is Download folder in Device store. A file will be save here.
                    var fileName = Path.GetFileNameWithoutExtension(System.Web.HttpUtility.UrlDecode(item.BaseUrl.Substring(item.BaseUrl.LastIndexOf("/") + 1)));
                    var localPath = Path.Combine(downloadsPath, fileName);

                    var path = Path.Combine(downloadsPath, fileName);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    //Save using writeAsync
                    var fs = new FileStream(Path.Combine(path, item.Name), FileMode.OpenOrCreate);
                    await fs.WriteAsync(bytes, 0, bytes.Length);// writes to Download folder 
                    fs.Close();

                    //File downloaded
                    item.DownloadProgress = 100;
                    item.Selected = false;
                    adapter.NotifyDataSetChanged();
                    Console.WriteLine("{0} bytes downloaded of file {1}", bytes.Length, item.Name);
                }
                catch (TaskCanceledException te)
                {
                    Helpers.UI.ShowToast(context, "Canceled: " + te.ToString(), ToastLength.Long);
                    return;
                }
                catch (Exception a)
                {
                    Helpers.UI.ShowToast(context, "ERROR: " + a.ToString(), ToastLength.Long);
                    return;
                }

            }
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

