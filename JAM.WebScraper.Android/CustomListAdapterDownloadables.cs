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
    public class CustomListAdapterDownloadables : BaseAdapter<dto.DownloadResult>
    {
        Activity context;
        List<dto.DownloadResult> list;

        public CustomListAdapterDownloadables(Activity _context, List<dto.DownloadResult> _list)
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

        public override dto.DownloadResult this[int index]
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

            var item = this[position];
            view.FindViewById<TextView>(Resource.Id.Title).Text = item.Name;          
            return view;
        }
    }
}