using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SlideMessaging.Droid.Helpers;
using SlideMessaging.Models;
using System;
using System.Threading.Tasks;

namespace SlideMessaging.Droid
{
    [Activity(Label = "Conversation", ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".MainActivity")]
    public class TextMessageActivity : BaseActivity
    {
        /// <summary>
        /// Specify the layout to inflace
        /// </summary>
        protected override int LayoutResource => Resource.Layout.activity_text_messages;

        TextMessageItemsAdapter adapter;
        SwipeRefreshLayout refresher;

        ProgressBar progress;
        TextMessagesViewModel viewModel { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var data = Intent.GetStringExtra("data");

            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<Thread>(data);

            //
            viewModel = new TextMessagesViewModel(this.ContentResolver, item.ThreadId);

            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(adapter = new TextMessageItemsAdapter(this, viewModel));

            var layoutManager = (GridLayoutManager)recyclerView.GetLayoutManager();
            layoutManager.ReverseLayout = true;
            //layoutManager.StackFromEnd = true;

            recyclerView.SetLayoutManager(layoutManager);

            refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            refresher.SetColorSchemeColors(Resource.Color.accent);

            progress = FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
            progress.Visibility = ViewStates.Gone;
            //

            SupportActionBar.Title = item.PhoneContact?.DisplayName;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#2196F3")));
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        public override View OnCreateView(View parent, string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(parent, name, context, attrs);
        }

        protected override void OnStart()
        {
            base.OnStart();

            refresher.Refresh += Refresher_Refresh;
            adapter.ItemLongClick += Adapter_ItemLongClick;

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        protected override void OnStop()
        {
            base.OnStop();

            refresher.Refresh -= Refresher_Refresh;
            adapter.ItemLongClick += Adapter_ItemLongClick;
        }

        void Adapter_ItemLongClick(object sender, RecyclerClickEventArgs e)
        {
            //var item = viewModel.Items[e.Position];
            //var intent = new Intent(this, typeof(BrowseItemDetailActivity));

            //intent.PutExtra("data", Newtonsoft.Json.JsonConvert.SerializeObject(item));
            //StartActivity(intent);
        }

        void Refresher_Refresh(object sender, EventArgs e)
        {
            viewModel.LoadItemsCommand.Execute(null);
            refresher.Refreshing = false;
        }
    }

    class TextMessageItemsAdapter : BaseRecycleViewAdapter
    {
        TextMessagesViewModel viewModel;
        Activity activity;

        public TextMessageItemsAdapter(Activity activity, TextMessagesViewModel viewModel)
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
            var id = Resource.Layout.item_message;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new MyTextMessageViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = viewModel.Items[position];

            // Replace the contents of the view with that element
            var myHolder = holder as MyTextMessageViewHolder;

            //myHolder.SenderName = item.SenderName;
            myHolder.MessageText.Text = item.MessageText;
            myHolder.Timestamp.Text = DateTimeHelper.GetDateStringBasedOnToday(item.RawDate);
            
            var layoutParams = (GridLayoutManager.LayoutParams) myHolder.ItemView.LayoutParameters;

            if (item.Inbound)
            {
                // Pad on the left for inbound messages
                layoutParams.LeftMargin = 36;
                layoutParams.RightMargin = 200;

                var textColorInt = activity.GetColor(Resource.Color.window_background);
                var textColor = new Color(textColorInt);

                myHolder.MessageText.SetTextColor(textColor);
                myHolder.Timestamp.SetTextColor(textColor);

                myHolder.CardView.SetBackgroundResource(Resource.Color.primaryDark);
            }
            else
            {
                // Pad on the right for outbound messages
                layoutParams.LeftMargin = 200;
                layoutParams.RightMargin = 36;

                var textColorInt = activity.GetColor(Resource.Color.accent_foreground);
                var textColor = new Color(textColorInt);

                myHolder.MessageText.SetTextColor(textColor);
                myHolder.Timestamp.SetTextColor(textColor);

                myHolder.CardView.SetBackgroundResource(Resource.Color.accent);
            }

            if (position == viewModel.Items.Count - 1)
            {
                layoutParams.TopMargin = 36;
            }
            else
            {
                layoutParams.TopMargin = 0;
            }

            myHolder.ItemView.LayoutParameters = layoutParams;
        }

        public override int ItemCount => viewModel.Items.Count;
    }

    public class MyTextMessageViewHolder : RecyclerView.ViewHolder
    {
        //public TextView SenderName { get; set; }
        public TextView MessageText { get; set; }
        public TextView Timestamp { get; set; }
        public RelativeLayout MessageLayout { get; set; }
        public CardView CardView { get; set; }

        public MyTextMessageViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            //SenderName = itemView.FindViewById<TextView>(Resource.Id.sender_name);
            MessageText = itemView.FindViewById<TextView>(Resource.Id.message_text);
            Timestamp = itemView.FindViewById<TextView>(Resource.Id.timestamp);
            MessageLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.message_layout);
            CardView = itemView.FindViewById<CardView>(Resource.Id.message_card_view);
            
            //itemView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }
}
