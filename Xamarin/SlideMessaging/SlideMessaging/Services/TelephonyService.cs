using Android.Content;
using Android.Provider;
using SlideMessaging.Helpers;
using SlideMessaging.Models;
using System.Collections.Generic;
using System.Linq;

namespace SlideMessaging.Services
{
    public class TelephonyService
    {
        public static IEnumerable<Thread> GetInboxSMS(Context context)
        {
            var uri = Telephony.Sms.Inbox.ContentUri;
            var groupedThreads = ConversationHelper.GetDistinctThreads(context, uri);
            var populatedThreads = ConversationHelper.GetThreadDetails(context, uri, groupedThreads);

            return populatedThreads;
        }

        public static IEnumerable<Thread> GetDraftSMS(Context context)
        {
            var uri = Telephony.Sms.Draft.ContentUri;
            var groupedThreads = ConversationHelper.GetDistinctThreads(context, uri);
            var populatedThreads = ConversationHelper.GetThreadDetails(context, uri, groupedThreads);

            return populatedThreads;
        }

        public static IEnumerable<Thread> GetSentSMS(Context context)
        {
            var uri = Telephony.Sms.Sent.ContentUri;
            var groupedThreads = ConversationHelper.GetDistinctThreads(context, uri);
            var populatedThreads = ConversationHelper.GetThreadDetails(context, uri, groupedThreads);

            return populatedThreads;
        }

        public static IEnumerable<Thread> GetInboxMMS(Context context)
        {
            var cursor = context.ContentResolver.Query(Android.Provider.Telephony.Mms.Inbox.ContentUri,
                    new string[] { "address", "date", "body", "_id", "thread_id", "read" }, //columns
                    "thread_id", //GROUP BY
                    null,
                    null /*"DATE DESC"*/);

            var output = ConversationHelper.GetFullConvoListFromCursor(context, cursor);

            var grouped = from s in output
                          group s by s.ThreadId into m
                          select new { ThreadId = m.Key, MaxDate = m.Max(s => s.DateAsLong) };

            output = from o in output
                     join g in grouped on o.ThreadId equals g.ThreadId
                     where o.DateAsLong == g.MaxDate
                     select o;

            return output;
        }

        public static IEnumerable<Thread> GetDraftMMS(Context context)
        {
            var cursor = context.ContentResolver.Query(Android.Provider.Telephony.Mms.Draft.ContentUri,
                    new string[] { "address", "date", "body", "_id", "thread_id", "read" }, //columns
                    "thread_id", //GROUP BY
                    null,
                    null /*"DATE DESC"*/);

            var output = ConversationHelper.GetFullConvoListFromCursor(context, cursor);

            var grouped = from s in output
                          group s by s.ThreadId into m
                          select new { ThreadId = m.Key, MaxDate = m.Max(s => s.DateAsLong) };

            output = from o in output
                     join g in grouped on o.ThreadId equals g.ThreadId
                     where o.DateAsLong == g.MaxDate
                     select o;

            return output;
        }

        public static IEnumerable<Thread> GetSentMMS(Context context)
        {
            var cursor = context.ContentResolver.Query(Android.Provider.Telephony.Sms.Sent.ContentUri,
                    new string[] { "address", "date", "body", "_id", "thread_id", "read" }, //columns
                    "thread_id", //GROUP BY
                    null,
                    null /*"DATE DESC"*/);

            var output = ConversationHelper.GetFullConvoListFromCursor(context, cursor);

            var grouped = from s in output
                          group s by s.ThreadId into m
                          select new { ThreadId = m.Key, MaxDate = m.Max(s => s.DateAsLong) };

            output = from o in output
                     join g in grouped on o.ThreadId equals g.ThreadId
                     where o.DateAsLong == g.MaxDate
                     select o;

            return output;
        }
    }
}
