using Android.Content;
using Android.Gms.Ads;

namespace JAM.WebScraper.Android.Helpers
{
    public static class Ads
    {
        public const bool toggleEnabled = true;
        public static AdRequest.Builder builder { set; get; }
        public static AdRequest adRequest { set; get; }

        public static void InitBanner(ref AdView ad)
        {
            if (builder == null)
            {
                builder = new AdRequest.Builder();
#if DEBUG
                builder.AddTestDevice("82CFB8E783635A810C2D3B383C3342CA");
#endif
            }
            if(toggleEnabled)
                ad.LoadAd(builder.Build());
        }
      
        public static InterstitialAd ConstructFullPageAdd(Context con, string UnitID)
        {
            var ad = new InterstitialAd(con)
            {
                AdUnitId = UnitID
            };
            return ad;
        }
        public static InterstitialAd CustomBuild(this InterstitialAd ad)
        {
            if (builder == null)
            {
                builder = new AdRequest.Builder();
#if DEBUG
                builder.AddTestDevice("82CFB8E783635A810C2D3B383C3342CA");
#endif
            }
            if(toggleEnabled)
                ad.LoadAd(builder.Build());
            return ad;
        }

        public class adlistener : AdListener
        {
            // Declare the delegate (if using non-generic pattern).
            public delegate void AdLoadedEvent();
            public delegate void AdClosedEvent();
            public delegate void AdOpenedEvent();
            // Declare the event.
            public event AdLoadedEvent AdLoaded;
            public event AdClosedEvent AdClosed;
            public event AdOpenedEvent AdOpened;
            public override void OnAdLoaded()
            {
                if (AdLoaded != null) this.AdLoaded();
                base.OnAdLoaded();
            }
            public override void OnAdClosed()
            {
                if (AdClosed != null) this.AdClosed();
                base.OnAdClosed();
            }
            public override void OnAdOpened()
            {
                if (AdOpened != null) this.AdOpened();
                base.OnAdOpened();
            }
        }
    }

  
}