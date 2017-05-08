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
    [Activity(Label = "Search files", ScreenOrientation =android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/icon")]
    public class DownloadablesActivity : Activity
    {
        const string downloadablesApiUrl = "https://jamtech.herokuapp.com/api/Downloadables?urls={0}&extension={1}&levels={2}";
        private ProgressDialog dialog = null;
        private static string LastSearch = string.Empty;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Downloadables);

            var buttonSearchDownloadables = FindViewById<Button>(Resource.Id.buttonSearchDownloadables);
            var textUrlDownloadables = FindViewById<EditText>(Resource.Id.textSearchDownload);
            var textExtensionDownloadables = FindViewById<EditText>(Resource.Id.textExtensionDownload);
            var statusDownloadables = FindViewById<TextView>(Resource.Id.textStatusDownload);

            var url = Intent.GetStringExtra("url");
            if (!string.IsNullOrEmpty(url))
                textUrlDownloadables.Text = url;


            //Load subdirectory levels
            var spinner = FindViewById<Spinner>(Resource.Id.ddlDirLevels);
            var levelAdapter = new ArrayAdapter<string>(
                this, 17367048, new[] { "0", "1", "2", "3", "4", "5" });
            levelAdapter.SetDropDownViewResource(17367049);
            spinner.Adapter = levelAdapter;


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
                    LastSearch = new Uri(textUrlDownloadables.Text).ToString();
                    SearchDownloadables(textUrlDownloadables.Text, textExtensionDownloadables.Text, int.Parse(spinner.SelectedItem.ToString())).ContinueWith(files =>
                    {
                        if (!files.IsFaulted)
                        {
                            //TODO Show Results
                            var activityResults = new Intent(this, typeof(DownloadablesResultsActivity));
                            activityResults.PutExtra("lastSearch", LastSearch);
                            activityResults.PutExtra("results", files.Result.ToString());
                            StartActivityForResult(activityResults, 0);

                            RunOnUiThread(() =>
                            {
                                statusDownloadables.Text = string.Format("{0} files found by JAMTech.cl!!", files.Result.Count);
                                dialog.Dismiss();
                                //UI.HideKeyboard(this);
                            });
                        }
                        else
                        {
                            RunOnUiThread(() =>
                            {
                                statusDownloadables.Text = string.Format("ERROR !!");
                            });
                            throw files.Exception;
                        }
                    });
                }
                catch (Exception e)
                {
                    Helpers.UI.ShowToast(this, "Error: " + e.ToString(), ToastLength.Long);
                }
            };
                   
        }

        static async Task<JsonValue> SearchDownloadables(string url, string extension, int levels = 0)
        {
            return await Http.GetResponse(string.Format(downloadablesApiUrl, url, extension, levels));
        }

       
    }
}

