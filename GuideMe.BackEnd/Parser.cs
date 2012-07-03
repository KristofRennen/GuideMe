using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GuideMe.BackEnd.Model;

namespace GuideMe.BackEnd
{
    public interface IParser
    {
        
    }

    public abstract class Parser : IParser
    {
        protected IEnumerable<string> InvalidValues = new[] { "\n", "\r", "\t", "<br>", "</br>", "&lt;/h4&gt;", "&lt;", "&gt;", "<h4>", "</h4>" };
        protected IDictionary<string, string> ReplaceValues = new Dictionary<string, string> { { "&nbsp;", " " }, { "&#039;", "'" }, { "&eacute;", "é" }, { "&quot;", "\"" } };

        public string RawContent { get; set; }
        public string RawStrippedContent { get; set; }
        public DateTime? ForDate { get; set; }

        protected Parser(string content)
        {
            RawContent = content;
            RawStrippedContent = Strip(content);
        }

        protected Parser(string content, DateTime date)
        {
            RawContent = content;
            RawStrippedContent = Strip(content);
            ForDate = date;
        }

        protected abstract string Strip(string content);

        public Day Parse()
        {
            var day = ParseDayInfo();
            day.Channels = ParseChannelInfo();

            for (var i = 0; i < day.Channels.Length; i++)
            {
                day.Channels[i].Items = ParseItemInfo(i, day.Channels[i]);
            }

            return day;
        }

        protected abstract Day ParseDayInfo();
        protected abstract Channel[] ParseChannelInfo();
        protected abstract Item[] ParseItemInfo(int channelPosition, Channel channel);

        protected string GetElementValue(XElement parentElement, string elementName)
        {
            string elementValue = null;

            var childElement = parentElement.Element(elementName);
            elementValue = childElement != null ? childElement.Value : null;

            if (elementValue == null)
            {
                var attribute = parentElement.Attribute(elementName);
                elementValue = attribute != null ? attribute.Value : null;
            }

            return elementValue;
        }
    }
}