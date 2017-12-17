using Java.Util;
using SlideMessaging.Droid.Helpers;

namespace SlideMessaging.Droid.Models
{
    public class ConversationListItem
    {

        public ConversationListItem(Contact contact, long dateAsLong, Date lastTextSentOn, string message, string threadId, string isRead, bool IsSMS)
        {
            this.PhoneContact = contact;
            this.DateAsLong = dateAsLong;
            this.LastTextSentOn = lastTextSentOn;
            this.ThreadId = threadId;
            this.IsSMS = IsSMS;
            this.IsRead = isRead == "0";

            string mesPrev;

            if (message.Length > 50)
                mesPrev = message.Substring(0, 49);
            else
                mesPrev = message;

            this.MessagePreview = mesPrev;
        }

        public string GetCleansedPhoneNumber()
        {
            var cleansed = PhoneHelper.GetCleansedPhoneNumber(this.PhoneContact.Number);
            return cleansed; ;
        }

        public string GetInitials()
        {
            string Initials = "#";
            string contactDisplayName = this.PhoneContact.DisplayName;

            try
            {
                //if It parses, then we don't have the contact name
                string cleansedDisplay = PhoneHelper.GetCleansedPhoneNumber(contactDisplayName);
                long.Parse(cleansedDisplay);
            }
            catch
            {
                Initials = contactDisplayName.Substring(0, 1);
            }

            return Initials;
        }

        public Contact PhoneContact { get; set; }
        public Date LastTextSentOn { get; set; }
        public string MessagePreview { get; set; }
        public long DateAsLong { get; set; }
        public string ThreadId { get; set; }
        public bool IsRead { get; set; }
        public bool IsSMS { get; set; }
        public int AddressCount { get; set; }
    }
}