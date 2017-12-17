using Android.Icu.Text;
using Android.Text.Format;
using Android.Util;
using Java.Util;
using System;

namespace SlideMessaging.Helpers
{
    public class DateTimeHelper
    {
        public static Date GetDateFromString(string strDate)
        {
            try
            {
                var l = long.Parse(strDate);
                var d = new Date(l);

                return d;
            }
            catch (Exception ex)
            {
                Log.Error("DateTimeHelper", ex.Message);
                return new Date();
            }
        }

        public static string GetShortDateTimeString(Date d)
        {
            return "";
        }

        public static string GetLongDateTimeString(Date d)
        {
            return "";
        }

        public static string GetDateStringBasedOnToday(string strDate)
        {
            return GetDateStringBasedOnToday(strDate, false);
        }

        public static string GetDateTimeBasedOnTodayString(string strDate)
        {
            return GetDateStringBasedOnToday(strDate, true);
        }

        public static string GetDateStringBasedOnToday(Date date)
        {
            return GetDateStringBasedOnToday(date, false);
        }

        public static string GetDateTimeBasedOnTodayString(Date date)
        {
            return GetDateStringBasedOnToday(date, true);
        }

        private static string GetDateStringBasedOnToday(string strDate, bool includeTimeWithDate)
        {
            Date date = GetDateFromString(strDate);

            return GetDateStringBasedOnToday(date, includeTimeWithDate);
        }

        public static string GetDateStringBasedOnToday(Date date, bool includeTimeWithDate)
        {
            string dateDisplay;
            Calendar cal = Calendar.Instance;
            Calendar GregCal = GregorianCalendar.Instance;

            //TODO: Implement
            //DateFormat.getDateInstance(DateFormat.LONG).format(date);

            string time;
            string minutes = "" + date.Minutes;

            if (minutes.Length == 1)
            {
                minutes = "0" + minutes;
            }

            if (date.Hours > 11)
            {
                var hour = date.Hours % 12;

                if (hour == 0)
                    hour = 12;

                time = (hour) + ":" + minutes + " PM";
            }
            else
            {
                time = date.Hours + ":" + minutes + " AM";
            }

            if (IsToday(date))
            {
                //date = DateFormat.getDateInstance(DateFormat.LONG).format(d);
                //dateDisplay = DateFormat.getDateInstance(DateFormat.AM_PM_FIELD).format(date);
                dateDisplay = time;
            }
            else
            {
                dateDisplay = Android.Icu.Text.DateFormat.GetDateInstance(DateFormatStyle.Short).Format(date);

                if (includeTimeWithDate)
                {
                    dateDisplay += " " + time;
                }
            }

            return dateDisplay;
        }

        public static string GetTimeString(Date d)
        {
            return "";
        }

        public static string GetDateString(Date d)
        {
            return "";
        }

        public static bool IsToday(long l)
        {
            return DateUtils.IsToday(l);
        }

        public static bool IsToday(Date d)
        {
            Calendar cal = GregorianCalendar.Instance;
            Date now = cal.Time;

            if (now.Year != d.Year || now.Month != d.Month || now.GetDate() != d.GetDate())
            {
                return false;
            }

            return true;
        }
    }
}
