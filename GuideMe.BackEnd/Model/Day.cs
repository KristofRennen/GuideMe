using System;
using System.Globalization;

namespace GuideMe.BackEnd.Model
{
    public class Day
    {
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = new DateTime(value.Year, value.Month, value.Day); }
        }

        public int Year { get { return Date.Year; } }
        public int Month { get { return Date.Month; } }
        public int DayOfMonth { get { return Date.Day; } }
        public DayOfWeek DayOfWeek { get { return Date.DayOfWeek; } }
        public int Week { get { return DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(Date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday); } }

        public Channel[] Channels { get; set; }
    }
}