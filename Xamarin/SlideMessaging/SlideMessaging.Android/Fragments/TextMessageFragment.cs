//using System;
//using Android.OS;
//using Android.Support.V7.Widget;
//using Android.Views;
//using Android.Widget;
//using Android.Support.V4.Widget;
//using Android.App;
//using Android.Content;
//using SlideMessaging.Helpers;
//using Android;

//namespace SlideMessaging.Droid
//{
//    public class TextMessageFragment : Android.Support.V4.App.Fragment, IFragmentVisible
//    {
//        public static TextMessageFragment NewInstance() =>
//            new TextMessageFragment { Arguments = new Bundle() };

//        TextMessageItemsAdapter adapter;
//        SwipeRefreshLayout refresher;

//        ProgressBar progress;
//        public static TextMessagesViewModel ViewModel { get; set; }

//        public override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);

//            // Create your fragment here
//        }

//        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            var threadId = savedInstanceState.GetString("threadId");
//            ViewModel = new TextMessagesViewModel(this.conten, threadId);

//            View view = inflater.Inflate(Resource.Layout.fragment_threads, container, false);
//            var recyclerView =
//                view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

//            recyclerView.HasFixedSize = true;
//            recyclerView.SetAdapter(adapter = new TextMessageItemsAdapter(Activity, ViewModel));

//            refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
//            refresher.SetColorSchemeColors(Resource.Color.accent);

//            progress = view.FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
//            progress.Visibility = ViewStates.Gone;

//            return view;
//        }

//        public override void OnStart()
//        {
//            base.OnStart();

//            refresher.Refresh += Refresher_Refresh;
//            adapter.ItemClick += Adapter_ItemClick;

//            if (ViewModel.Items.Count == 0)
//                ViewModel.LoadItemsCommand.Execute(null);
//        }

//        public override void OnStop()
//        {
//            base.OnStop();
//            refresher.Refresh -= Refresher_Refresh;
//            adapter.ItemClick -= Adapter_ItemClick;
//        }

//        void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
//        {
//            var item = ViewModel.Items[e.Position];
//            var intent = new Intent(Activity, typeof(BrowseItemDetailActivity));

//            intent.PutExtra("data", Newtonsoft.Json.JsonConvert.SerializeObject(item));
//            Activity.StartActivity(intent);
//        }

//        void Refresher_Refresh(object sender, EventArgs e)
//        {
//            ViewModel.LoadItemsCommand.Execute(null);
//            refresher.Refreshing = false;
//        }

//        public void BecameVisible()
//        {

//        }
//    }

//    class TextMessageItemsAdapter : BaseRecycleViewAdapter
//    {
//        TextMessagesViewModel viewModel;
//        Activity activity;

//        public TextMessageItemsAdapter(Activity activity, TextMessagesViewModel viewModel)
//        {
//            this.viewModel = viewModel;
//            this.activity = activity;

//            this.viewModel.Items.CollectionChanged += (sender, args) =>
//            {
//                this.activity.RunOnUiThread(NotifyDataSetChanged);
//            };
//        }

//        // Create new views (invoked by the layout manager)
//        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
//        {
//            //Setup your layout here
//            View itemView = null;
//            var id = Resource.Layout.item_thread;
//            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

//            var vh = new MyTextMessageViewHolder(itemView, OnClick, OnLongClick);
//            return vh;
//        }

//        // Replace the contents of a view (invoked by the layout manager)
//        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
//        {
//            var item = viewModel.Items[position];

//            // Replace the contents of the view with that element
//            var myHolder = holder as MyTextMessageViewHolder;
//            //myHolder.SenderName = item.SenderName;
//            myHolder.MessageText.Text = item.MessageText;
//            myHolder.Timestamp.Text = DateTimeHelper.GetDateStringBasedOnToday(item.RawDate);
//        }

//        public override int ItemCount => viewModel.Items.Count;
//    }

//    public class MyTextMessageViewHolder : RecyclerView.ViewHolder
//    {
//        //public TextView SenderName { get; set; }
//        public TextView MessageText { get; set; }
//        public TextView Timestamp { get; set; }
//        public bool IsRead { get; set; }

//        public MyTextMessageViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
//                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
//        {
//            //SenderName = itemView.FindViewById<TextView>(Resource.Id.sender_name);
//            MessageText = itemView.FindViewById<TextView>(Resource.Id.message_text);
//            Timestamp = itemView.FindViewById<TextView>(Resource.Id.timestamp);

//            itemView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
//            itemView.LongClick += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
//        }
//    }
//}
