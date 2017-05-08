using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;

namespace JAM.WebScraper.Android
{
    public class CustomListAdapterVideos : BaseAdapter<dto.Format>
    {
        Activity context;
        dto.VideosResult result;
        public CustomListAdapterVideos(Activity _context, dto.VideosResult _result)
            : base()
        {
            this.context = _context;
            this.result = _result;
        }

        public override int Count
        {
            get {
                if (result == null || result.info == null)
                    return 0;
                return result.info.formats.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override dto.Format this[int index]
        {
            get { return result.info.formats[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            // re-use an existing view, if one is available
            // otherwise create a new one
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.ListItemRowVideos, parent, false);

            var item = this[position];
            view.FindViewById<TextView>(Resource.Id.Type).Text = item.ext;
            view.FindViewById<TextView>(Resource.Id.Title).Text = result.info.title;
            view.FindViewById<TextView>(Resource.Id.Description).Text = item.format_id;


            //using (var imageView = view.FindViewById<ImageView>(Resource.Id.Thumbnail))
            //{
            //    string url = Android.Text.Html.FromHtml(item.thumbnail).ToString();

            //    //Download and display image
            //    Koush.UrlImageViewHelper.SetUrlDrawable(imageView,
            //        url, Resource.Drawable.Placeholder);
            //}
            return view;
        }
    }
}