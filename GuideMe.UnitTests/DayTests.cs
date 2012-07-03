using System;
using GuideMe.BackEnd.Model;
using NUnit.Framework;

namespace GuideMe.UnitTests
{
    [TestFixture]
    public class DayTests
    {
        private Day day = new Day { Date = new DateTime(2012, 06, 28, 15, 43, 15) };

        [Test]
        public void DateRemovesTimeInfo()
        {
            Assert.AreEqual(new DateTime(2012, 06, 28), day.Date);
        }

        [Test]
        public void HasCorrectWeekNumber()
        {
            Assert.AreEqual(26, day.Week);
        }

        [Test]
        public void HasCorrectMonthNumber()
        {
            Assert.AreEqual(6, day.Month);
        }

        [Test]
        public void HasCorrectDayNumber()
        {
            Assert.AreEqual(DayOfWeek.Thursday, day.DayOfWeek);
        }

        [Test]
        public void HasCorrectYear()
        {
            Assert.AreEqual(2012, day.Year);
        }

        [Test]
        public void HasCorrectDay()
        {
            Assert.AreEqual(28, day.DayOfMonth);
        }
    }
}