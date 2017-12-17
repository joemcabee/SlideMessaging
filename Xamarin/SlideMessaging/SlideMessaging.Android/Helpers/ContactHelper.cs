using Android.Content;
using Android.Provider;
using Java.Lang;
using SlideMessaging.Droid.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlideMessaging.Droid.Helpers
{
    public class ContactHelper
    {
        public ContactHelper(ContentResolver cr)
        {
            this.ContactList = new List<Contact>();
            this.PhoneList = new List<PhoneNumber>();

            var phoneCursor = cr.Query(
                    ContactsContract.CommonDataKinds.Phone.ContentUri,
                    null, null, null, null);

            List<PhoneNumber> contactPhoneList = new List<PhoneNumber>();

            if (phoneCursor.Count > 0)
            {
                while (phoneCursor.MoveToNext())
                {
                    var phone = new PhoneNumber(phoneCursor.GetString(phoneCursor.GetColumnIndex("contact_id")),
                            phoneCursor.GetString(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number)));

                    contactPhoneList.Add(phone);
                }
            }

            // keep list of contact names and phone numbers
            var contactList = new List<Contact>();

            var contactCursor = cr.Query(ContactsContract.Contacts.ContentUri,
                    null, null, null, null);

            if (contactCursor.Count > 0)
            {
                while (contactCursor.MoveToNext())
                {
                    string id = contactCursor.GetString(
                            contactCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id));
                    string name = contactCursor.GetString(
                            contactCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));

                    contactList.Add(new Contact(id, name));
                }
            }

            this.PhoneList = contactPhoneList;
            this.ContactList = contactList;
        }

        public List<PhoneNumber> PhoneList { get; set; }

        public List<Contact> ContactList { get; set; }

        public Android.Net.Uri GetPhotoUriFromContactId(string contactId)
        {
            var person = ContentUris.WithAppendedId(ContactsContract.Contacts.ContentUri, Long.ParseLong(contactId));
            return Android.Net.Uri.WithAppendedPath(person, ContactsContract.Contacts.Photo.ContentDirectory);
        }

        public string GetContactNameFromNumber(string phoneNumber)
        {
            var contactName = phoneNumber;

            for (int x = 0; x < this.PhoneList.Count; x++)
            {
                var possibleContactPhone = this.PhoneList.ElementAt(x);
                var possibleContactCleansedPhone = PhoneHelper.GetCleansedPhoneNumber(possibleContactPhone.Number);
                var cleansedTextMessageNumber = PhoneHelper.GetCleansedPhoneNumber(phoneNumber);

                if (possibleContactCleansedPhone == cleansedTextMessageNumber && possibleContactCleansedPhone.Length > 0)
                {
                    var contactId = possibleContactPhone.ContactID;

                    for (int y = 0; y < this.ContactList.Count; y++)
                    {
                        var possibleContact = this.ContactList.ElementAt(y);

                        if (possibleContact.ID == contactId)
                        {
                            contactName = possibleContact.DisplayName;
                        }
                    }
                }
            }

            return contactName;
        }

        public static Uri GetContactPhotoUriByAddress(ContentResolver cr, string address)
        {
            var contactCursor = cr.Query(ContactsContract.CommonDataKinds.Phone.ContentUri,
                    null, null, null, null);

            string thumbnail;
            Uri result = null;

            if (contactCursor.Count > 0)
            {
                while (contactCursor.MoveToNext() && result == null)
                {
                    thumbnail = contactCursor.GetString(
                            contactCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.PhotoThumbnailUri));

                    Android.Net.Uri.Parse(thumbnail);
                }
            }

            return result;
        }
    }
}