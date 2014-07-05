using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TweetSharp;

namespace IronManTrack
{
    class Program
    {

        static void Main(string[] args)
        {
            Dictionary<String, String> prevValues = new Dictionary<string, string>();

            var service = new TwitterService(Properties.Settings.Default.ConsumerKey, Properties.Settings.Default.ConsumerSecret);
            service.AuthenticateWith(Properties.Settings.Default.Token, Properties.Settings.Default.TokenSecret);

            while (true)
            {
                HtmlDocument doc = new HtmlWeb().Load(Properties.Settings.Default.TrackingPage);

                foreach (var table in doc.DocumentNode.SelectNodes("//table[@width='100%']"))
                {
                    try
                    {
                        var caption = table.SelectSingleNode("caption/strong").InnerText.Trim();
                        caption = caption.Substring(0, caption.IndexOf(' '));
                        Console.WriteLine(caption);

                        var ths = table.SelectNodes("thead/tr/th").Select(t => t.InnerText.Trim()).ToArray();
                        if (ths.Count() == 0)
                            continue;

                        foreach (var tr in table.SelectNodes("tbody/tr | tfoot/tr"))
                        {
                            var tds = tr.SelectNodes("td").Select(t => t.InnerText.Trim()).ToArray();

                            bool changedSplit = false;
                            for (int i = 0; i < 4; i++)
                            {
                                Console.WriteLine(ths[i] + ": " + tds[i]);
                                if (tds[i].Contains("--:--"))
                                    continue;

                                string key = caption + ths[i] + tds[0];
                                if (prevValues.ContainsKey(key))
                                {
                                    if (!prevValues[key].Equals(tds[i]))
                                    {
                                        changedSplit = true;
                                        Console.WriteLine("!!! Changed from " + prevValues[key]);
                                        prevValues[key] = tds[i];
                                    }
                                }
                                else
                                {
                                    // First add
                                    prevValues.Add(key, tds[i]);
                                }
                            }

                            if (changedSplit)
                            {
                                var tweet = new SendTweetOptions();
                                if (tds[0] == "Total")
                                {
                                    tweet.Status = "Finished with my {0}! {2} in {3}. Total race time: {4} {5} http://bit.ly/trackvic #teamvic #IMLP";
                                }
                                else
                                {
                                    tweet.Status = "{0} UPDATE: Just reached {1}! Total race time: {4} (this split: {2} in {3}) http://bit.ly/trackvic #teamvic #IMLP";
                                }

                                string nextEvent = "";
                                if (caption == "Swim")
                                    nextEvent = "Now I will bike 112 mi!";
                                else if (caption == "Bike")
                                    nextEvent = "Now I will run 26.2 mi!";
                                else if (caption == "Run")
                                    nextEvent = "I AM AN IRONMAN!";

                                tweet.Status = String.Format(tweet.Status, caption.ToUpper(), tds[0], tds[1], tds[2], tds[3], nextEvent);

                                if (!tweet.Status.Contains("--:--"))
                                    service.SendTweet(tweet);
                            }
                        }
                        Console.WriteLine("");
                    }
                    catch (ArgumentOutOfRangeException) { }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                Thread.Sleep(30000);
            }
        }
    }
}

