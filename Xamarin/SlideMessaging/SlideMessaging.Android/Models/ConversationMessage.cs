using Java.Util;
using SlideMessaging.Droid.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlideMessaging.Droid.Models
{
    public class ConversationMessage
    {

        public ConversationMessage(string threadId, string id, string messageText, Date rawDate, Boolean inbound)
        {
            this.ThreadId = threadId;
            this.Id = id;
            this.MessageText = messageText;
            this.RawDate = rawDate;
            this.Inbound = inbound;
            this.SenderAddress = "";
        }

        public ConversationMessage(string threadId, string id, string messageText, Date rawDate, string inOutType)
        {
            this.ThreadId = threadId;
            this.Id = id;
            this.MessageText = messageText;
            this.RawDate = rawDate;

            if (inOutType == null)
                inOutType = "1";

            this.Inbound = inOutType == "1";
            this.SenderAddress = String.Empty;
        }

        public static IEnumerable<ConversationMessage> MergeLists(IEnumerable<ConversationMessage> a, IEnumerable<ConversationMessage> b)
        {
            var list = a.ToList();
            list.AddRange(b);

            return list.OrderBy(s => s.Timestamp);
        }

        public string MessageText { get; set; }
        public string Id { get; set; }
        public string ThreadId { get; set; }
        public Date RawDate { get; set; }
        public string Timestamp
        {
            get
            {
                return DateTimeHelper.GetDateTimeBasedOnTodayString(this.RawDate);
            }
        }
        public bool Inbound { get; set; }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
    }
}