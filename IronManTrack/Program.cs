using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        enum Section
        {
            Swim,
            Bike,
            Run
        }

        class Split
        {
            public Section Section { get; set; }
            public string Name { get; set; }
            public string Distance { get; set; }
            public string SplitTime { get; set; }
            public string RaceTime { get; set; }
            public string Pace { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                Split objSplit = obj as Split;
                if (objSplit == null)
                {
                    return false;
                }

                return (this.Section == objSplit.Section) && (this.Name == objSplit.Name) && (this.Distance == objSplit.Distance) && (this.SplitTime == this.SplitTime) && (this.RaceTime == objSplit.RaceTime) && (this.Pace == objSplit.Pace);
            }

            public override string ToString()
            {
                return Section.ToString() + " - " + Name + " - " + Distance + " - " + SplitTime + " - " + RaceTime + " - " + Pace;
            }
        }

        static void Main(string[] args)
        {
            var service = new TwitterService(Properties.Settings.Default.ConsumerKey, Properties.Settings.Default.ConsumerSecret);
            service.AuthenticateWith(Properties.Settings.Default.Token, Properties.Settings.Default.TokenSecret);

            var splits = new List<Split>();

            while (true)
            {
                HtmlDocument doc = new HtmlWeb().Load(Properties.Settings.Default.TrackingPage);

                var currentSection = Section.Swim; // caption was moved out of table, assume the page stays in order
                foreach (var table in doc.DocumentNode.SelectNodes("//table[@width='100%']"))
                {
                    try
                    {
                        var ths = table.SelectNodes("thead/tr/th").Select(t => t.InnerText.Trim()).ToArray();
                        if (ths.Count() == 0) // ignore the last transition table
                            continue;

                        foreach (var tr in table.SelectNodes("tbody/tr | tfoot/tr | tr")) // atm the bike split section is missing its tbody tag
                        {
                            var tds = tr.SelectNodes("td").Select(t => t.InnerText.Trim()).ToArray();

                            Split thisSplit = new Split
                            {
                                Section = currentSection,
                                Name = tds[0],
                                Distance = tds[1],
                                SplitTime = tds[2],
                                RaceTime = tds[3],
                                Pace = tds[4]
                            };

                            Console.WriteLine(thisSplit.ToString());

                            var existingSplit = splits.SingleOrDefault(t => t.Section == thisSplit.Section && t.Name == thisSplit.Name);

                            if (existingSplit == null)
                            {
                                Console.WriteLine("Adding new split");
                                splits.Add(thisSplit);
                            }
                            else
                            {
                                if (!existingSplit.Equals(thisSplit))
                                {
                                    if (thisSplit.ToString().Contains("--:--"))
                                    {
                                        Console.WriteLine("Tracking site bug? " + existingSplit.ToString() + " -> " + thisSplit.ToString());
                                    }
                                    else
                                    {
                                        splits.Remove(existingSplit);
                                        splits.Add(thisSplit);

                                        var tweet = new SendTweetOptions();

                                        tweet.Status = "OFFICIAL TIME: " + thisSplit.Section.ToString() + " " + thisSplit.Name + ", " + thisSplit.Distance + " in " + thisSplit.SplitTime + " (" + thisSplit.Pace + ") Race time " + thisSplit.RaceTime + " http://j.mp/1oT5plm #IMWI";
                                
                                        if (!tweet.Status.Contains("--:--"))
                                            service.SendTweet(tweet);
                                    }
                                }
                            }

                        }
                        Console.WriteLine("");

                        if (currentSection == Section.Swim)
                        {
                            currentSection = Section.Bike;
                        }
                        else if (currentSection == Section.Bike)
                        {
                            currentSection = Section.Run;
                        }
                    }
                    catch (ArgumentNullException) { }
#if !DEBUG
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Debug.Write(ex.ToString());
                    }
#endif
                }

                Thread.Sleep(5000);
            }
        }
    }
}

