using System;

namespace GuideMe.BackEnd.Model
{
    public class Item
    {
        public string Description { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public double Duration { get { return (End - Start).TotalSeconds / 4; } }
    }
}