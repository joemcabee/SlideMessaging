using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using System.Linq;
using SlideMessaging.Helpers;
using SlideMessaging.Models;
using Android;

namespace SlideMessaging.Services
{
    public class ThreadDataStore : IDataStore<Thread>
    {
        List<Thread> items = new List<Thread>();

        public ThreadDataStore()
        {
        }

        public Context Context { get; set; }
        public ContentResolver ContentResolver { get; set; }

        public ThreadDataStore(Context context, ContentResolver contentResolver, List<string> favorites)
        {
            this.Context = context;
            this.ContentResolver = contentResolver;

            var allMessages = new List<Thread>();
            var smsInbox = TelephonyService.GetInboxSMS(context);
            var smsSent = TelephonyService.GetSentSMS(context);

            allMessages.AddRange(smsInbox);
            allMessages.AddRange(smsSent);

            var grouped = from s in allMessages
                          group s by s.ThreadId into m
                          select new { ThreadId = m.Key, MaxDate = m.Max(s => s.DateAsLong) };

            var output = from o in allMessages
                     join g in grouped on o.ThreadId equals g.ThreadId
                     where o.DateAsLong == g.MaxDate
                     orderby o.DateAsLong descending
                     select o;

            items.Clear();
            items.AddRange(output);
            
            //mark favorites            
            foreach (var fav in favorites)
            {
                var item = items.FirstOrDefault(i => i.ThreadId == fav);

                if (item != null)
                {
                    item.IsFavorite = true;
                }
            }
        }

        public async Task<bool> AddItemAsync(Thread item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var _item = items.FirstOrDefault(arg => arg.ThreadId == id);
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Thread> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.ThreadId == id));
        }

        public async Task<IEnumerable<Thread>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<bool> UpdateItemAsync(Thread item)
        {
            var _item = items.FirstOrDefault(arg => arg.ThreadId == item.ThreadId);
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }
    }
}
