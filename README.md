IronmanTrack
============

This is a quick app I wrote to automatically scrape the [Ironman tracking page for my dad](http://tracking.ironmanlive.com/newathlete.php?rid=1143240022&race=lakeplacid&bib=2855&v=3.0) at Ironman Lake Placid on July 28, 2013 and livetweet the race on [@IronmanGrumps](https://twitter.com/ironmangrumps). I wrote it the night before, using remote desktop from a condo in Lake Placid, New York to my desktop in Dallas, Texas to code and test it. The day of the event it worked fairly well, with a few bugs caused by the tracking site. Unfortunately sometimes the tracking site would return "--:--" for every time causing a few preemptory finish Tweets. Also the tracking would sometimes lag behind the actual race and sometimes would be instant. With a few tweaks throughout the day though it worked well, and my dad "Ironman Grumps" finished in 16 hours, 50 minutes.

Uses TweetSharp and the HtmlAgilityPack, both great libraries which made it possible for me to make this so quickly.
