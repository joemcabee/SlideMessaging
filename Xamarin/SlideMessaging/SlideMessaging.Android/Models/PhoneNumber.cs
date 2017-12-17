namespace SlideMessaging.Droid.Models
{
    public class PhoneNumber
    {
        public PhoneNumber(string contactID, string number)
        {
            this.ContactID = contactID;
            this.Number = number;
        }

        public string ContactID { get; set; }
        public string Number { get; set; }
    }
}