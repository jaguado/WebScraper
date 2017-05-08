using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace JAM.WebScraper.Android
{
    public class CustomListAdapterTorrents : BaseAdapter<dto.TorrentResult>
    {
        Activity context;
        List<dto.TorrentResult> list;

        public CustomListAdapterTorrents(Activity _context, List<dto.TorrentResult> _list)
            : base()
        {
            this.context = _context;
            this.list = _list;
        }

        public override int Count
        {
            get { return list.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override dto.TorrentResult this[int index]
        {
            get { return list[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            // re-use an existing view, if one is available
            // otherwise create a new one
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.ListItemRow, parent, false);

            dto.TorrentResult item = this[position];
            view.FindViewById<TextView>(Resource.Id.Type).Text = item.SubType;
            view.FindViewById<TextView>(Resource.Id.Title).Text = item.Name;
            var description = string.Empty;
            if (item.Description != null && item.Description.Any())
                description = item.Description.First();
            view.FindViewById<TextView>(Resource.Id.Description).Text = description;
            view.FindViewById<TextView>(Resource.Id.Seeds).Text = "S: " + item.Seeds.ToString() + " / ";
            view.FindViewById<TextView>(Resource.Id.Leeds).Text = "L: " + item.Leeds.ToString();

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