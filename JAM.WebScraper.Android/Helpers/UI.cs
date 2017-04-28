using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using Android.Widget;

namespace JAM.WebScraper.Android.Helpers
{
    public static class UI
    {
        public static void ShowToast(Activity context, string text, ToastLength length)
        {
            Toast.MakeText(context, text, length);
        }
        public static void HideKeyboard(Activity context)
        {
            var inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(context.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }
    }
}