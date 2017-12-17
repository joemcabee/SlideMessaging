using Android.Content;
using System;

namespace SlideMessaging.Droid.Helpers
{
    class MessageHelper
    {
        public void GetInbox(Context context)
        {
            var contentResolver = new ContextWrapper(context).ContentResolver;
            var cursor = contentResolver.Query(Android.Net.Uri.Parse("content://sms/inbox"), null, null, null, null);

            if (cursor.MoveToFirst())
            {
                do
                {
                    var msgData = "";

                    for (int idx = 0; idx < cursor.ColumnCount; idx++)
                    {
                        msgData += " " + cursor.GetColumnName(idx) + ":" + cursor.GetString(idx);
                    }
                }
                while (cursor.MoveToNext());
            }
        }

        public void GetSent(Context context)
        {
            var contentResolver = new ContextWrapper(context).ContentResolver;
            var cursor = contentResolver.Query(Android.Net.Uri.Parse("content://sms/sent"), null, null, null, null);

            if (cursor.MoveToFirst())
            {
                do
                {
                    var msgData = "";

                    for (int idx = 0; idx < cursor.ColumnCount; idx++)
                    {
                        msgData += " " + cursor.GetColumnName(idx) + ":" + cursor.GetString(idx);
                    }
                }
                while (cursor.MoveToNext());
            }
        }

        public void GetDraft(Context context)
        {
            var contentResolver = new ContextWrapper(context).ContentResolver;
            var cursor = contentResolver.Query(Android.Net.Uri.Parse("content://sms/draft"), null, null, null, null);

            if (cursor.MoveToFirst())
            {
                do
                {
                    var msgData = "";

                    for (int idx = 0; idx < cursor.ColumnCount; idx++)
                    {
                        msgData += " " + cursor.GetColumnName(idx) + ":" + cursor.GetString(idx);
                    }
                }
                while (cursor.MoveToNext());
            }
        }
    }
}