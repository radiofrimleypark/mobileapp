using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using RadioFrimleyPark.App.Fragments;
using GoogleCast;
using ModernHttpClient;

namespace RadioFrimleyPark.App.Adapters
{
    public class GoogleCastAdapter : RecyclerView.Adapter, INotifyCollectionChanged
    {
        private readonly Context _context;
        private ObservableCollection<IReceiver> _chromecasts;
        public event EventHandler<IReceiver> ItemClick;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public GoogleCastAdapter(Context context, List<IReceiver> chromecasts = null)
        {
            _context = context;
            _chromecasts = chromecasts?.ToObservableCollection<IReceiver>() ?? new ObservableCollection<IReceiver>();
            _chromecasts.CollectionChanged += _chromecasts_CollectionChanged;
        }

        private void _chromecasts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IReceiver newItem in e.NewItems)
                {
                    //ModifiedItems.Add(newItem);

                    //Add listener for each item on PropertyChanged event
                    //newItem.PropertyChanged += this.OnItemPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (IReceiver oldItem in e.OldItems)
                {
                    //ModifiedItems.Add(oldItem);

                    //oldItem.PropertyChanged -= this.OnItemPropertyChanged;
                }
            }
            CollectionChanged?.Invoke(sender, e);
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Item item = sender as Item;
            //if (item != null)
            //    ModifiedItems.Add(item);
        }
        public List<IReceiver> Chromecasts
        {
            set => _chromecasts = value.ToObservableCollection<IReceiver>();
            get { return _chromecasts.ToList(); }
        }

        public override int ItemCount
        {
            get { return _chromecasts.Count; }
        }

        private int _selectedPos = -1;
        void OnClick(int position)
        {
            var restorePos = _selectedPos;
            _selectedPos = position;
            NotifyItemChanged(restorePos); // reset original
            NotifyItemChanged(_selectedPos);
            ItemClick?.Invoke(this, _chromecasts[position]);
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            GoogleCastFragment.GoogleCastViewHolder vh = holder as GoogleCastFragment.GoogleCastViewHolder;
            vh.ItemView.Selected = _selectedPos == position;
            vh.ItemView.SetBackgroundColor(vh.ItemView.Selected ? Color.Red : Color.ParseColor("#ff268FCE"));   // Hmm...

            IReceiver receiver = _chromecasts[position];
            vh.name.Text = receiver.FriendlyName;
            Task.Factory.StartNew(() => vh.icon.DownloadIconsAsync(_context, new Uri($"http://{receiver.IPEndPoint.Address}:8008/setup/icon.png")));

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.z_Chromecast, parent, false);
            GoogleCastFragment.GoogleCastViewHolder vh = new GoogleCastFragment.GoogleCastViewHolder(itemView, OnClick);
            return vh;

        }


    }
    public static class LinqExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> _linqResult)
        {
            return new ObservableCollection<T>(_linqResult);
        }
    }

    public static class ImageViewExtensions
    {
        private static readonly Dictionary<string, Bitmap> _bitmaps = new Dictionary<string, Bitmap>();

        public static async Task DownloadIconsAsync(this ImageView image, Context context, Uri url)
        {
            Bitmap bitmap = null;
            if (_bitmaps.ContainsKey(url.ToString()))
                bitmap = _bitmaps[url.ToString()];
            else
            {
                using (var client = new HttpClient(new NativeMessageHandler()))
                {
                    var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url), HttpCompletionOption.ResponseHeadersRead);
                    var stream = await result.Content.ReadAsStreamAsync();
                    bitmap = BitmapFactory.DecodeStream(stream);
                    _bitmaps.Add(url.ToString(), bitmap);
                }
            }
            ((Activity)context).RunOnUiThread(() => { image.SetImageBitmap(bitmap); });
        }
    }
}