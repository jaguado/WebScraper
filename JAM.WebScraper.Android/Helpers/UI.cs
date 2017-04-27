using Android.App;
using Android.Widget;

namespace JAM.WebScraper.Android.Helpers
{
    public static class UI
    {
        public static void ShowToast(Activity context, string text, ToastLength length)
        {
            Toast.MakeText(context, text, length);
        }
    }
}