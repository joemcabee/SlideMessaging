using Android.Content;
using SlideMessaging.Models;
using SlideMessaging.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SlideMessaging
{
    public class ThreadsViewModel : BaseViewModel
    {
        public new IDataStore<Thread> DataStore => ServiceLocator.Instance.Get<IDataStore<Thread>>() ??
            new ThreadDataStore(this.Context, this.ContentResolver, this.Favorites);

        public ObservableCollection<Thread> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command AddItemCommand { get; set; }
        public Context Context { get; set; }
        public ContentResolver ContentResolver { get; set; }
        public List<string> Favorites { get; set; }

        public ThreadsViewModel(Context context, ContentResolver contentResolver, List<string> favorites)
        {
            Title = "Threads";
            Items = new ObservableCollection<Thread>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command<Thread>(async (Thread item) => await AddItem(item));
            Context = context;
            ContentResolver = contentResolver;
            Favorites = favorites;
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

        async Task AddItem(Thread item)
        {
            Items.Add(item);
            await DataStore.AddItemAsync(item);
        }
    }
}
