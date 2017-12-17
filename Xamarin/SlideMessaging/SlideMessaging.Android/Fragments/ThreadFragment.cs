using System;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using Android.App;
using Android.Content;
using SlideMessaging.Helpers;
using Android;
using System.Collections.Generic;
using System.Linq;
using SlideMessaging.Droid.Helpers;

namespace SlideMessaging.Droid
{
    public class ThreadFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        public static ThreadFragment NewInstance() =>
            new ThreadFragment { Arguments = new Bundle() };

        ThreadItemsAdapter adapter;
        SwipeRefreshLayout refresher;
        List<string> favoriteThreadIds = new List<string>();

        ProgressBar progress;
        public static ThreadsViewModel ViewModel { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var favorites = FavoritesHelper.GetFavorites(Context);

            ViewModel = new ThreadsViewModel(this.Context, this.Context.ContentResolver, favorites);

            View view = inflater.Inflate(Resource.Layout.fragment_threads, container, false);
            var recyclerView =
                view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(adapter = new ThreadItemsAdapter(Activity, ViewModel));

            refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            refresher.SetColorSchemeColors(Resource.Color.accent);

            progress = view.FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
            progress.Visibility = ViewStates.Gone;

            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            refresher.Refresh += Refresher_Refresh;
            adapter.ItemClick += Adapter_ItemClick;

            ViewModel.Favorites = FavoritesHelper.GetFavorites(Context);

            if (ViewModel.Items.Count == 0)
                ViewModel.LoadItemsCommand.Execute(null);
        }

        public override void OnStop()
        {
            base.OnStop();
            refresher.Refresh -= Refresher_Refresh;
            adapter.ItemClick -= Adapter_ItemClick;
        }

        void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var item = ViewModel.Items[e.Position];
            var intent = new Intent(Activity, typeof(TextMessageActivity));

            intent.PutExtra("data", Newtonsoft.Json.JsonConvert.SerializeObject(item));
            Activity.StartActivity(intent);
        }

        void Refresher_Refresh(object sender, EventArgs e)
        {
            ViewModel.Favorites = FavoritesHelper.GetFavorites(Context);
            ViewModel.LoadItemsCommand.Execute(null);
            refresher.Refreshing = false;
        }

        public void BecameVisible()
        {

        }
    }

    class ThreadItemsAdapter : BaseRecycleViewAdapter
    {
        ThreadsViewModel viewModel;
        Activity activity;

        public ThreadItemsAdapter(Activity activity, ThreadsViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.activity = activity;

            this.viewModel.Items.CollectionChanged += (sender, args) =>
            {
                this.activity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.item_thread;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new MyThreadViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = viewModel.Items[position];

            // Replace the contents of the view with that element
            var myHolder = holder as MyThreadViewHolder;
            myHolder.SenderInitials.Text = item.Initials;
            myHolder.SenderName.Text = item.PhoneContact.DisplayName;
            myHolder.MessagePreview.Text = item.MessagePreview;
            myHolder.Timestamp.Text = SlideMessaging.Helpers.DateTimeHelper.GetDateStringBasedOnToday(item.LastTextSentOn);
            myHolder.ThreadId.Text = item.ThreadId;
            myHolder.IsFavorite = item.IsFavorite;

            if (item.IsFavorite)
            {
                myHolder.Favorite.SetImageResource(Resource.Drawable.ic_favorite_black_18dp);
            }
            else
            {
                myHolder.Favorite.SetImageResource(Resource.Drawable.ic_favorite_border_black_18dp);
            }
        }

        public override int ItemCount => viewModel.Items.Count;
    }

    public class MyThreadViewHolder : RecyclerView.ViewHolder
    {
        public TextView SenderInitials { get; set; }
        public TextView SenderName { get; set; }
        public TextView MessagePreview { get; set; }
        public TextView Timestamp { get; set; }
        public ImageView Favorite { get; set; }
        public bool IsFavorite { get; set; }
        public TextView ThreadId { get; set; }
        public bool IsRead { get; set; }
        public Context Context { get; set; }

        public MyThreadViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            Context = itemView.Context;
            SenderInitials = itemView.FindViewById<TextView>(Resource.Id.sender_initials);
            SenderName = itemView.FindViewById<TextView>(Resource.Id.sender_name);
            MessagePreview = itemView.FindViewById<TextView>(Resource.Id.message_preview);
            Timestamp = itemView.FindViewById<TextView>(Resource.Id.timestamp);
            Favorite = itemView.FindViewById<ImageView>(Resource.Id.favorites_icon);
            ThreadId = itemView.FindViewById<TextView>(Resource.Id.thread_id);
            
            Favorite.Click += Favorite_Click;

            itemView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        private void Favorite_Click(object sender, EventArgs e)
        {
            var threadId = ThreadId.Text;

            if (IsFavorite)
            {
                Favorite.SetImageResource(Resource.Drawable.ic_favorite_border_black_18dp);
                IsFavorite = false;

                FavoritesHelper.RemoveFavorite(Context, threadId);
            }
            else
            {
                Favorite.SetImageResource(Resource.Drawable.ic_favorite_black_18dp);
                IsFavorite = true;

                FavoritesHelper.AddFavorite(Context, threadId);
            }
        }
    }
}
