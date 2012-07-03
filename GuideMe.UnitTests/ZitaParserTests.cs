using System;
using System.IO;
using System.Reflection;
using GuideMe.BackEnd.Model;
using GuideMe.BackEnd.Parsers;
using NUnit.Framework;

namespace GuideMe.UnitTests
{
    [TestFixture]
    public class ZitaParserTests
    {
        private string contentToParse = "";

        [SetUp]
        public void SetUp()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GuideMe.UnitTests.SampleData.Zita.htm");
            contentToParse = new StreamReader(stream).ReadToEnd();
        }

        [Test]
        public void ContentIsNotNull()
        {
            Assert.IsNotNull(contentToParse);
        }

        [Test]
        public void ParserOnlyConsidersEpg()
        {
            var parser = new ZitaParser(contentToParse);

            Assert.AreEqual(parser.RawContent, contentToParse);
            Assert.IsNotNull(parser.RawStrippedContent);
            
            Assert.IsTrue(parser.RawStrippedContent.StartsWith(@"<div class=""epg-field simple box"">"));
            Assert.IsTrue(parser.RawStrippedContent.EndsWith("</div>"));
        }

        [Test]
        public void ParseReturnsCorrectDays()
        {
            var result = new ZitaParser(contentToParse).Parse();

            Assert.AreEqual(new DateTime(2012, 06, 28), result.Date);
        }

        [Test]
        public void ParseReturnsCorrectChannels()
        {
            var day = new ZitaParser(contentToParse).Parse();

            Assert.AreEqual(10, day.Channels.Length);

            Assert.AreEqual("VTM", day.Channels[0].Name);
            Assert.AreEqual("één", day.Channels[1].Name);
            Assert.AreEqual("Canvas", day.Channels[2].Name);
            Assert.AreEqual("2BE", day.Channels[3].Name);
            Assert.AreEqual("VijfTV", day.Channels[4].Name);
            Assert.AreEqual("VT4", day.Channels[5].Name);
            Assert.AreEqual("Vitaya", day.Channels[6].Name);
            Assert.AreEqual("JIM", day.Channels[7].Name);
            Assert.AreEqual("TMF", day.Channels[8].Name);
            Assert.AreEqual("Acht", day.Channels[9].Name);
        }

        [Test]
        public void ParseReturnsCorrectItems()
        {
            var day = new ZitaParser(contentToParse, new DateTime(2012, 06, 29)).Parse();
            
            foreach (var channel in day.Channels)
            {
                Assert.IsNotNull(channel.Items);
                Assert.AreNotEqual(0, channel.Items.Length);
            }

            Print(day);
        }

        private void Print(Day day)
        {
            Console.WriteLine(day.Date);

            foreach (var channel in day.Channels)
            {
                Console.WriteLine("\t -> " + channel.Name);

                foreach (var item in channel.Items)
                {
                    Console.WriteLine("\t\t - " + item.Start + " -> " + item.End + " : " + item.Description);
                }
            }
        }
    }
}