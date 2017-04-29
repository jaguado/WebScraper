using Android.Gms.Ads;

namespace JAM.WebScraper.Android.Helpers
{
    public static class Ads
    {
        private static AdRequest adRequest { set; get; }
        public static void InitAds(AdView adView)
        {   
            if(adRequest==null)
                adRequest = new AdRequest.Builder().Build();
            adView.LoadAd(adRequest);
        }
    }
}