using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Util;
using SlideMessaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static Android.Provider.ContactsContract;

namespace SlideMessaging.Helpers
{
    public class ConversationHelper
    {
        public static IEnumerable<Thread> GetDistinctThreads(Context context, Android.Net.Uri uri)
        {
            var threads = new List<Thread>();

            var cursor = context.ContentResolver.Query(uri,
                    new string[] { "distinct thread_id" }, //columns
                    null,
                    null,
                    null);

            try
            {
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        var threadId = cursor.GetString(cursor.GetColumnIndex("thread_id"));
                        var thread = new Thread(threadId, true);

                        threads.Add(thread);
                    }
                    while (cursor.MoveToNext());
                }
            }
            catch (Exception ex)
            {
                string DEBUG_TAG = "GetGroupedThreads";
                Log.Error(DEBUG_TAG, ex.ToString());
            }

            return threads;
        }

        public static IEnumerable<Thread> GetThreadDetails(Context context, Android.Net.Uri uri, IEnumerable<Thread> threads)
        {
            var output = new List<Thread>();

            try
            {
                foreach (var thread in threads)
                {
                    var cursor = context.ContentResolver.Query(uri,
                            new string[] { "address", "date", "body", /*"_id", "thread_id", */"read", "person" }, //columns
                            "thread_id = ?",
                            new string[] { thread.ThreadId },
                            "date desc limit 1");

                    int depth = cursor.Count;

                    if (cursor.MoveToFirst())
                    {
                        var rawPhone = cursor.GetString(cursor.GetColumnIndex("address"));
                        var contactId = cursor.GetString(cursor.GetColumnIndex("person"));
                        var contact = GetContactFromAddress(context.ContentResolver, rawPhone);
                        var strDate = cursor.GetString(cursor.GetColumnIndex("date"));
                        //var id = cursor.GetString(cursor.GetColumnIndex("_id"));
                        var dateAsLong = long.Parse(strDate);
                        var date = DateTimeHelper.GetDateFromString(strDate);

                        var updatedThread = new Thread(contact,
                                    dateAsLong, //date as long
                                    date, //date
                                    cursor.GetString(cursor.GetColumnIndex("body")),
                                    thread.ThreadId, //cursor.GetString(cursor.GetColumnIndex("thread_id")),
                                    cursor.GetString(cursor.GetColumnIndex("read")),
                                    true);

                        output.Add(updatedThread);
                    }
                }
            }
            catch (Exception ex)
            {
                string DEBUG_TAG = "GetThreadDetailsf";
                Log.Error(DEBUG_TAG, ex.ToString());
            }

            return output;
        }

        public static IEnumerable<Thread> GetFullConvoListFromCursor(Context context, ICursor c)
        {
            int depth = c.Count;
            var listConvos = new List<Thread>();
            var cr = context.ContentResolver;

            try
            {
                if (c.MoveToFirst())
                {
                    do
                    {
                        var rawPhone = c.GetString(c.GetColumnIndex("address"));
                        var contactId = c.GetString(c.GetColumnIndex("person"));
                        var contact = GetContactFromAddress(cr, rawPhone);
                        var strDate = c.GetString(c.GetColumnIndex("date"));
                        var id = c.GetString(c.GetColumnIndex("_id"));
                        var dateAsLong = long.Parse(strDate);
                        var date = DateTimeHelper.GetDateFromString(strDate);

                        var thread = new Thread(null/*contact*/,
                                    dateAsLong, //date as long
                                    date, //date
                                    c.GetString(c.GetColumnIndex("body")),
                                    c.GetString(c.GetColumnIndex("thread_id")),
                                    c.GetString(c.GetColumnIndex("read")),
                                    true);

                        listConvos.Add(thread);
                    }
                    while (c.MoveToNext());
                }
            }
            catch (Exception ex)
            {
                string DEBUG_TAG = "ConversationHelper";
                Log.Error(DEBUG_TAG, ex.Message);
            }

            // Descending?
            //var orderedList = listConvos.OrderBy(x => x.LastTextSentOn);

            return listConvos;
        }

        public static Contact GetContactFromAddress(ContentResolver cr, string address)
        {
            //string contactId;
            //string contactName;
            //string stringUri;
            //string dbPhone;
            Android.Net.Uri picUri = null;
            //string formattedPhone = PhoneHelper.GetCleansedPhoneNumber(address);
            Contact contact = new Contact();
            contact.Number = address;
            contact.DisplayName = address;

            try
            {
                var personUri = Android.Net.Uri.WithAppendedPath(
                        ContactsContract.PhoneLookup.ContentFilterUri, address);

                var contactCursor = cr.Query(personUri,
                        new String[] { PhoneLookup.InterfaceConsts.DisplayName, PhoneLookup.InterfaceConsts.ContactId, PhoneLookup.InterfaceConsts.PhotoUri },
                        null,
                        null,
                        null);

                if (contactCursor.MoveToFirst())
                {
                    contact.ID = contactCursor.GetString(contactCursor.GetColumnIndex(PhoneLookup.InterfaceConsts.ContactId));
                    contact.DisplayName = contactCursor.GetString(contactCursor.GetColumnIndex(PhoneLookup.InterfaceConsts.DisplayName));

                    var stringPhotoUri = contactCursor.GetString(contactCursor.GetColumnIndex(PhoneLookup.InterfaceConsts.PhotoUri));

                    if (stringPhotoUri != null)
                    {
                        picUri = Android.Net.Uri.Parse(stringPhotoUri);
                    }

                    contactCursor.Close();
                }

                //var phoneCursor = cr.Query(
                //        ContactsContract.CommonDataKinds.Phone.ContentUri,
                //        new string[] { "data1", "contact_id", "display_name", "photo_uri" },
                //        "REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(data1, ' ', ''), '+', ''), '(',''), ')',''), '-', '') LIKE '%" + formattedPhone + "'",
                //        //"data1 LIKE '%8480'",
                //        null, null);

                //if (phoneCursor.MoveToFirst())
                //{
                //    dbPhone = phoneCursor.GetString(phoneCursor.GetColumnIndex("data1"));
                //    contactId = phoneCursor.GetString(phoneCursor.GetColumnIndex("contact_id"));
                //    contactName = phoneCursor.GetString(phoneCursor.GetColumnIndex("display_name"));
                //    stringUri = phoneCursor.GetString(phoneCursor.GetColumnIndex("photo_uri"));

                //    if (PhoneHelper.GetCleansedPhoneNumber(dbPhone) == formattedPhone)
                //    {
                //        if (stringUri != null)
                //        {
                //            picUri = Android.Net.Uri.Parse(stringUri);
                //        }

                //        contact.ID = contactId;
                //        contact.DisplayName = contactName;
                //        contact.PictureUri = picUri;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log.Error("GetContactFromAddress", ex.Message);
            }

            return contact;
        }

        public static IEnumerable<TextMessage> GetConvoFromCursor(ContentResolver cr, ICursor c)
        {
            int depth = c.Count;
            var listMessages = new List<TextMessage>();

            if (c.MoveToFirst())
            {
                do
                {
                    string messageType = c.GetString(c.GetColumnIndex("ct_t"));

                    if (messageType != null && messageType.Contains("application/vnd.wap.multipart"))
                    {
                        string messageId = c.GetString(c.GetColumnIndex("_id"));
                        listMessages.Add(MultimediaHelper.GetMessageById(cr, c, messageId));
                    }
                    else
                    {
                        string strDate = c.GetString(c.GetColumnIndex("date"));

                        listMessages.Add(new TextMessage(
                                c.GetString(c.GetColumnIndex("thread_id")),
                                c.GetString(c.GetColumnIndex("_id")),
                                c.GetString(c.GetColumnIndex("body")), //message
                                DateTimeHelper.GetDateFromString(strDate), //date
                                c.GetString(c.GetColumnIndex("type")))); //whether the message is inbound or outbound
                    }
                }
                while (c.MoveToNext());
            }

            var orderedList = listMessages.OrderBy(x => x.Timestamp);

            return orderedList;
        }

        public static IEnumerable<TextMessage> GetMessagesByThreadId(ContentResolver cr, string threadId)
        {
            var messages = new List<TextMessage>();
            
            try
            {
                var inboxCursor = cr.Query(Telephony.Sms.Inbox.ContentUri,
                    new string[] { "thread_id", "_id", "date", "body" }, //columns
                    "thread_id = ?",
                    new string[] { threadId },
                    "date desc limit 200");

                if (inboxCursor.MoveToFirst())
                {
                    do
                    {
                        string strDate = inboxCursor.GetString(inboxCursor.GetColumnIndex("date"));

                        messages.Add(new TextMessage(
                                inboxCursor.GetString(inboxCursor.GetColumnIndex("thread_id")),
                                inboxCursor.GetString(inboxCursor.GetColumnIndex("_id")),
                                inboxCursor.GetString(inboxCursor.GetColumnIndex("body")), //message
                                DateTimeHelper.GetDateFromString(strDate), //date
                                "1")); //whether the message is inbound or outbound
                    }
                    while (inboxCursor.MoveToNext());
                }

                var sentCursor = cr.Query(Telephony.Sms.Sent.ContentUri,
                    new string[] { "thread_id", "_id", "date", "body" }, //columns
                    "thread_id = ?",
                    new string[] { threadId },
                    "date desc limit 200");

                if (sentCursor.MoveToFirst())
                {
                    do
                    {
                        string strDate = sentCursor.GetString(sentCursor.GetColumnIndex("date"));

                        messages.Add(new TextMessage(
                                sentCursor.GetString(sentCursor.GetColumnIndex("thread_id")),
                                sentCursor.GetString(sentCursor.GetColumnIndex("_id")),
                                sentCursor.GetString(sentCursor.GetColumnIndex("body")), //message
                                DateTimeHelper.GetDateFromString(strDate), //date
                                "0")); //whether the message is inbound or outbound
                    }
                    while (sentCursor.MoveToNext());
                }
            }
            catch (Exception ex)
            {
                string DEBUG_TAG = "GetMessagesByThreadId";
                Log.Error(DEBUG_TAG, ex.ToString());
            }

            // sort oldest to newest
            messages.Sort((x, y) => y.RawDate.CompareTo(x.RawDate));

            return messages;
        }
    }
}
