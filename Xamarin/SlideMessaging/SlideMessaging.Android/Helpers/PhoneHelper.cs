using Android.Content;
using Android.Provider;
using Android.Telephony;
using Java.Lang;
using SlideMessaging.Droid.Models;

namespace SlideMessaging.Droid.Helpers
{
    public class PhoneHelper
    {
        public static string GetCleansedPhoneNumber(string rawPhone)
        {
            var cleansedPhone = rawPhone;

            cleansedPhone = cleansedPhone
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("+", "")
                .Replace("-", "");

            if (cleansedPhone.StartsWith("1") && cleansedPhone.Length > 1)
            {
                cleansedPhone = cleansedPhone.Substring(1);
            }

            return cleansedPhone;
        }

        public static string FormatPhone(string number)
        {
            string r = GetCleansedPhoneNumber(number);

            if (r.Length == 7)
            {
                r = r.Substring(0, 3) + "-" + r.Substring(3, 7);
            }
            else if (r.Length == 10)
            {
                r = "(" + r.Substring(0, 3) + ") " + r.Substring(3, 3) + "-" + r.Substring(6, 4);
            }
            else if (r.Length == 11)
            {

            }

            return r;
        }

        public static Contact GetContactAndPhoneFromId(ContentResolver cr, string contactId)
        {
            var cRet = new Contact();

            var phoneCursor = cr.Query(
                    ContactsContract.CommonDataKinds.Phone.ContentUri, null,
                    "contact_id" + "=? AND " +
                    "type" + "=?",
                    new string[] { contactId, PhoneDataKind.Mobile.ToString() }, null);

            if (phoneCursor.Count > 0)
            {
                phoneCursor.MoveToNext();

                string number = phoneCursor.GetString(
                        phoneCursor.GetColumnIndex("data1"));

                cRet.Number = number;
            }

            var contactCursor = cr.Query(
                    ContactsContract.Contacts.ContentUri, null,
                    "_id" + "=?",
                    new string[] { contactId }, null);

            if (contactCursor.Count > 0)
            {
                contactCursor.MoveToNext();

                var id = contactCursor.GetString(
                        contactCursor.GetColumnIndex("_id"));
                var name = contactCursor.GetString(
                        contactCursor.GetColumnIndex("display_name"));

                cRet.ID = id;
                cRet.DisplayName = name;
            }

            return cRet;
        }

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            var clean = GetCleansedPhoneNumber(phoneNumber);

            if (clean.Length == 7 || clean.Length == 10 || clean.Length == 11)
            {
                try
                {
                    var l = Long.ParseLong(clean);
                    return true;
                }
                catch { }
            }

            return false;
        }

        public static string GetProperFormat(string phone)
        {
            var cleansed = GetCleansedPhoneNumber(phone);
            var p = cleansed;

            if (cleansed.Length == 10)
            {
                p = "+1" + cleansed;
            }
            else if (cleansed.Length == 11)
            {
                p = "+" + cleansed;
            }

            return p;
        }

        public static string GetMyPhoneNumber(Context context)
        {
            var tManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            return tManager.Line1Number;
        }
    }
}