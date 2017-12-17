using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Util;
using SlideMessaging.Droid.Models;
using System.Collections.Generic;
using System.Linq;

namespace SlideMessaging.Droid.Helpers
{
    public class ConversationHelper
    {
        public static IEnumerable<ConversationListItem> getFullConvoListFromCursor(Context context, ContentResolver cr, ICursor c)
        {
            int depth = c.Count;
            var listConvos = new List<ConversationListItem>();

            try
            {
                if (c.MoveToFirst())
                {
                    do
                    {
                        string rawPhone = c.GetString(c.GetColumnIndex("address"));
                        Contact contact = GetContactFromAddress(cr, rawPhone);
                        string strDate = c.GetString(c.GetColumnIndex("date"));
                        string id = c.GetString(c.GetColumnIndex("_id"));
                        string messageType = c.GetString(c.GetColumnIndex("ct_t"));

                        if (messageType != null && messageType.Contains("application/vnd.wap.multipart"))
                        {
                            listConvos.Add(MultimediaHelper.GetConversationListItemById(context, cr, id));
                        }
                        else
                        {
                            listConvos.Add(new ConversationListItem(contact,
                                    long.Parse(strDate), //date as long
                                    DateTimeHelper.GetDateFromString(strDate), //date
                                    c.GetString(c.GetColumnIndex("body")),
                                    c.GetString(c.GetColumnIndex("thread_id")),
                                    c.GetString(c.GetColumnIndex("read")),
                                    true));
                        }
                    }
                    while (c.MoveToNext());
                }
            }
            catch (System.Exception ex)
            {
                string DEBUG_TAG = "ConversationHelper";
                Log.Error(DEBUG_TAG, ex.Message);
            }
            
            // Descending?
            var orderedList = listConvos.OrderBy(x => x.LastTextSentOn);

            return orderedList;
        }

        public static Contact GetContactFromAddress(ContentResolver cr, string address)
        {
            string contactId;
            string contactName;
            string stringUri;
            string dbPhone;
            Android.Net.Uri picUri = null;
            string formattedPhone = PhoneHelper.GetCleansedPhoneNumber(address);
            Contact contact = new Contact();
            contact.Number = address;
            contact.DisplayName = address;

            var phoneCursor = cr.Query(
                    ContactsContract.CommonDataKinds.Phone.ContentUri,
                    new string[] { "data1", "contact_id", "display_name", "photo_uri", "data1" },
                    "REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(data1, ' ', ''), '+', ''), '(',''), ')',''), '-', '') LIKE '%" + formattedPhone + "'",
                    //"data1 LIKE '%8480'",
                    null, null);

            if (phoneCursor.MoveToFirst())
            {
                dbPhone = phoneCursor.GetString(phoneCursor.GetColumnIndex("data1"));
                contactId = phoneCursor.GetString(phoneCursor.GetColumnIndex("contact_id"));
                contactName = phoneCursor.GetString(phoneCursor.GetColumnIndex("display_name"));
                stringUri = phoneCursor.GetString(phoneCursor.GetColumnIndex("photo_uri"));

                if (PhoneHelper.GetCleansedPhoneNumber(dbPhone) == formattedPhone)
                {
                    if (stringUri != null)
                    {
                        picUri = Android.Net.Uri.Parse(stringUri);
                    }

                    contact.ID = contactId;
                    contact.DisplayName = contactName;
                    contact.PictureUri = picUri;
                }
            }

            return contact;
        }

        public static IEnumerable<ConversationMessage> GetConvoFromCursor(ContentResolver cr, ICursor c)
        {
            int depth = c.Count;
            var listMessages = new List<ConversationMessage>();

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

                        listMessages.Add(new ConversationMessage(
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
    }
}