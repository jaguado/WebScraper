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
        public List<dto.DownloadResult> List { set; get; }
        
        public CustomListAdapterDownloadables(Activity _context, List<dto.DownloadResult> _list)
            : base()
        {
            this.context = _context;
            this.List = _list;
        }

        public override int Count
        {
            get { return List.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override dto.DownloadResult this[int index]
        {
            get { return List[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            // re-use an existing view, if one is available
            // otherwise create a new one
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.ListItemRowDownloadables, parent, false);

            var item = this[position];
            var checkBox = view.FindViewById<CheckBox>(Resource.Id.Title);
            checkBox.Tag = position;
            checkBox.Text = item.Name;           
            checkBox.SetOnCheckedChangeListener(new CheckedChangeListener(context, List));
            checkBox.Checked = item.Selected;

            return view;
        }

        private void CheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var pos = (int)(((CheckBox)sender).GetTag(Resource.Id.Title));
            if (this.Count > pos)
                this[pos].Selected = e.IsChecked;
        }

        private class CheckedChangeListener : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
        {
            private Activity activity;
            private List<dto.DownloadResult> _list;
            public CheckedChangeListener(Activity activity, List<dto.DownloadResult> list)
            {
                this.activity = activity;
                _list = list;
            }

            public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
            {
                var pos = (int)buttonView.Tag;
                if (_list.Count > pos)
                    _list[pos].Selected = isChecked;
            }
        }
    }
}