using Android.Content;
using SlideMessaging.Models;
using SlideMessaging.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace SlideMessaging
{
    public class TextMessagesViewModel : BaseViewModel
    {
        public new IDataStore<TextMessage> DataStore => ServiceLocator.Instance.Get<IDataStore<TextMessage>>() ??
            new TextMessageDataStore(this.ContentResolver, this.ThreadId);

        public ObservableCollection<TextMessage> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command AddItemCommand { get; set; }
        public ContentResolver ContentResolver { get; set; }
        public string ThreadId { get; set; }

        public TextMessagesViewModel(ContentResolver contentResolver, string threadId)
        {
            Title = "Text Messages";
            Items = new ObservableCollection<TextMessage>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command<TextMessage>(async (TextMessage item) => await AddItem(item));
            ContentResolver = contentResolver;
            ThreadId = threadId;
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task AddItem(TextMessage item)
        {
            Items.Add(item);
            await DataStore.AddItemAsync(item);
        }
    }
}
