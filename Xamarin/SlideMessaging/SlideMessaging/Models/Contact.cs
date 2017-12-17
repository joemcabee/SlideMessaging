using SlideMessaging.Droid.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlideMessaging.Models
{
    public class Contact
    {
        public Contact() { }

        public Contact(string id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public string Initials
        {
            get
            {
                var initials = "#";

                if (this.DisplayName.Length > 0)
                {
                    try
                    {
                        //If it parses, then we don't have the contact name
                        var cleansedDisplay = PhoneHelper.GetCleansedPhoneNumber(this.DisplayName);
                        long.Parse(cleansedDisplay);
                    }
                    catch
                    {
                        initials = this.DisplayName.Substring(0, 1);
                    }
                }

                return initials;
            }
        }

        private string _Phone = string.Empty;

        public string Number
        {
            get
            {
                return _Phone;
            }
            set
            {
                _Phone = PhoneHelper.FormatPhone(value);
            }
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Sender { get; set; }
        public Android.Net.Uri PictureUri { get; set; }
    }
}
