using Android.Content;
using SlideMessaging.Helpers;
using SlideMessaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlideMessaging.Services
{
    public class TextMessageDataStore : IDataStore<TextMessage>
    {
        List<TextMessage> items = new List<TextMessage>();

        public TextMessageDataStore(ContentResolver contentResolver, string threadId)
        {
            var messages = ConversationHelper.GetMessagesByThreadId(contentResolver, threadId);
            
            items.Clear();
            items.AddRange(messages);
        }

        public async Task<bool> AddItemAsync(TextMessage item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var _item = items.FirstOrDefault(arg => arg.Id == id);
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<TextMessage> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<TextMessage>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<bool> UpdateItemAsync(TextMessage item)
        {
            var _item = items.FirstOrDefault(arg => arg.Id == item.Id);
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }
    }
}
