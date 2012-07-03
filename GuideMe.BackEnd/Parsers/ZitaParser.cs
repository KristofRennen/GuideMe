using System;
using System.Linq;
using System.Xml.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using GuideMe.BackEnd.Model;
using HtmlAgilityPack;

namespace GuideMe.BackEnd.Parsers
{
    public class ZitaParser : Parser
    {
        public ZitaParser(string content) : base(content) { }
        public ZitaParser(string content, DateTime date) : base(content, date) {}

        protected override string Strip(string content)
        {
            var startTag = @"<div class=""epg box"">";
            var endTag = @"<script language=""javascript"" type=""text/javascript"">";

            var start = content.IndexOf(startTag) + startTag.Length;
            var end = content.IndexOf(endTag, start);

            var selection = content.Substring(start, end - start);

            foreach (var invalidValue in InvalidValues)
            {
                selection = selection.Replace(invalidValue, "");
            }

            foreach (var replaceValue in ReplaceValues)
            {
                selection = selection.Replace(replaceValue.Key, replaceValue.Value);
            }

            return selection.Trim();
        }

        /*
            <div class="h3header">
			    <ul>
				    <li id="today" class="active"><a href="/entertainment/tv-gids/28-06-2012/">vandaag</a></li>
				    <li id="tomorrow"><a href="/entertainment/tv-gids/29-06-2012/">morgen</a></li>
				    <li><a href="/entertainment/tv-gids/30-06-2012/">za 30/6</a></li>
				    <li><a href="/entertainment/tv-gids/01-07-2012/">zo 1/7</a></li>
				    <li><a href="/entertainment/tv-gids/02-07-2012/">ma 2/7</a></li>
				    <li><a href="/entertainment/tv-gids/03-07-2012/">di 3/7</a></li>
				    <li><a href="/entertainment/tv-gids/04-07-2012/">wo 4/7</a></li>
			    </ul>
		    </div>
        */
        protected override Day ParseDayInfo()
        {
            var startTag = @"<div class=""h3header"">";
            var endTag = @"</div>";

            var start = RawStrippedContent.IndexOf(startTag) + startTag.Length;
            var end = RawStrippedContent.IndexOf(endTag, start);

            var dayInfo = RawStrippedContent.Substring(start, end - start).Trim();

            var html = new HtmlDocument();
            html.LoadHtml(dayInfo);

            var days = html
                .DocumentNode
                .SelectNodes("//a[@href]")
                .Select(n => new Day {Date = ParseDateFromEntertainmentUri(n.GetAttributeValue("href", "true"))})
                .ToArray();

            if (!ForDate.HasValue)
            {
                return days.First();
            } 
            else
            {
                return days.FirstOrDefault(d => d.Date == ForDate);
            }
        }

        /*
            <div id="epg-inner-left" class="epg-channels zenders">
                <div class="epg-channel-head"></div>
                <div>
                    <a class="id_1" href="/entertainment/tv-gids/zender/vtm/" title="VTM">
                        <span>&nbsp;</span><strong>VTM</strong>
                    </a> 
                </div>
                <div>
                    <a class="id_2" href="/entertainment/tv-gids/zender/een/" title="één">
                        <span>&nbsp;</span><strong>één</strong>
                    </a> 
                </div>
                <div>
                    <a class="id_681" href="/entertainment/tv-gids/zender/canvas/" title="Canvas">
                        <span>&nbsp;</span><strong>Canvas</strong>
                    </a>
                </div>
                <div>
                    <a class="id_5" href="/entertainment/tv-gids/zender/2be/" title="2BE">
                        <span>&nbsp;</span><strong>2BE</strong>
                    </a>
                </div>
                <div>
                    <a class="id_6" href="/entertainment/tv-gids/zender/vijftv/" title="VijfTV">
                        <span>&nbsp;</span><strong>VijfTV</strong>
                    </a>
                </div>
                <div>
                    <a class="id_3" href="/entertainment/tv-gids/zender/vt4/" title="VT4">
                        <span>&nbsp;</span><strong>VT4</strong>
                    </a>
                </div>
                <div>
                    <a class="id_7" href="/entertainment/tv-gids/zender/vitaya/" title="Vitaya">
                        <span>&nbsp;</span><strong>Vitaya</strong>
                    </a>
                </div>
                <div>
                    <a class="id_26" href="/entertainment/tv-gids/zender/jim/" title="JIM">
                        <span>&nbsp;</span><strong>JIM</strong>
                    </a> 
                </div>
                <div>
                    <a class="id_28" href="/entertainment/tv-gids/zender/tmf/" title="TMF">
                        <span>&nbsp;</span><strong>TMF</strong>
                    </a>
                </div>
                <div>
                    <a class="id_295" href="/entertainment/tv-gids/zender/acht/" title="Acht">
                        <span>&nbsp;</span><strong>Acht</strong>
                    </a> 
                </div>
            </div>
 
        */
        protected override Channel[] ParseChannelInfo()
        {
            var startTag = @"<div id=""epg-inner-left"" class=""epg-channels zenders"">";
            var endTag = @"</div><br /><br /><div class=""alignright"">";

            var start = RawStrippedContent.IndexOf(startTag);
            var end = RawStrippedContent.IndexOf(endTag, start);

            var channelInfo = RawStrippedContent.Substring(start, end - start).Trim();
            channelInfo = channelInfo.Replace("\"title=\"", "\" title=\"").Replace("\"class=\"", "\" class=\"");

            var html = new HtmlDocument();
            html.LoadHtml(channelInfo);

            return html
                .DocumentNode
                .SelectNodes("//a[@title]")
                .Select(n => new Channel { Name = n.GetAttributeValue("title", "true")})
                .ToArray();
        }

        /*
            <div class="epg-row epg-items">
			    <div id="epg-item-56802" class="item" rel="&lt;h4&gt;Het Weer&lt;/h4&gt; 23:55 - 00:00&lt;br /&gt;Weerbericht." style="width: 0px;">
				    <span>23:55-00:00</span><br />
				    <a href="/entertainment/tv-gids/vtm/56802_het-weer.html">Het Weer</a>
                </div>
			    <div id="epg-item-56803" class="item" rel="&lt;h4&gt;Rush&lt;/h4&gt; 00:00 - 00:45&lt;br /&gt;Australische politiereeks." style="width: 270px;">
				    <span>00:00-00:45</span><br />
				    <a href="/entertainment/tv-gids/vtm/56803_rush.html">Rush</a>
                </div>
			    <div id="epg-item-56804" class="item" rel="&lt;h4&gt;Nachtelijke loop&lt;/h4&gt; 00:45 - 07:20&lt;br /&gt;Nachtelijke loop." style="width: 2370px;">
				    <span>00:45-07:20</span><br />
				    <a href="/entertainment/tv-gids/vtm/56804_nachtelijke-loop.html">Nachtelijke loop</a>
                </div>
            </div>
        */
        protected override Item[] ParseItemInfo(int channelPosition, Channel channel)
        {
            var startTag = @"<div class=""epg-row epg-items"">";
            var endTag = @"</div></div></div>";

            var start = RawStrippedContent.IndexOf(startTag);
            var end = RawStrippedContent.IndexOf(endTag, start);

            var itemInfo = RawStrippedContent.Substring(start, end - start).Trim();

            var html = new HtmlDocument();
            html.LoadHtml(itemInfo);

            return html
                .DocumentNode
                .SelectNodes("//div[@class=\"epg-row epg-items\"]")
                .ElementAt(channelPosition)
                .SelectNodes("div[@class=\"item\"]")
                .Select(n => new Item
                                {
                                    Description = n.SelectSingleNode("a").InnerText,
                                    Start = TimeSpan.Parse(n.SelectSingleNode("span").InnerText.Split('-').First()),
                                    End = TimeSpan.Parse(n.SelectSingleNode("span").InnerText.Split('-').Last())
                                })
                .ToArray();
        }

        // /entertainment/tv-gids/30-06-2012/
        private DateTime ParseDateFromEntertainmentUri(string uri)
        {
            return DateTime.Parse(uri.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries)[2]);
        }
    }
}