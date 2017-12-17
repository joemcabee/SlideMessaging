using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SlideMessaging.Droid.Models;

namespace SlideMessaging.Droid.Helpers
{
    class MultimediaHelper
    {
        internal static ConversationMessage GetMessageById(ContentResolver cr, ICursor c, string messageId)
        {
            throw new NotImplementedException();
        }

        internal static ConversationListItem GetConversationListItemById(Context context, ContentResolver cr, string id)
        {
            throw new NotImplementedException();
        }
    }
}