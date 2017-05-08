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
            context.RunOnUiThread(() =>
            {
                Toast.MakeText(context, text, length).Show();
            });
            
        }
        public static void ShowAlertOk(Activity context, string title, string text, System.EventHandler<DialogClickEventArgs> handler=null)
        {
            context.RunOnUiThread(() =>
            {
                var dialog = new AlertDialog.Builder(context);
                var alert = dialog.Create();
                alert.SetTitle(title);
                alert.SetMessage(text);
                alert.SetButton("OK", handler);
                alert.Show();
            });
        }
        public static void HideKeyboard(Activity context)
        {
            var inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(context.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }
    }
}