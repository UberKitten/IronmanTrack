IronmanTrack
============

This is a quick app I wrote to automatically scrape the [Ironman tracking page for my dad](http://tracking.ironmanlive.com/newathlete.php?rid=1143240022&race=lakeplacid&bib=2855&v=3.0) at Ironman Lake Placid on July 28, 2013 and livetweet the race on [@IronmanGrumps](https://twitter.com/ironmangrumps). I wrote it the night before, using remote desktop from a condo in Lake Placid, New York to my desktop in Dallas, Texas to code and test it. The day of the event it worked fairly well, with a few bugs caused by the tracking site. Unfortunately sometimes the tracking site would return "--:--" for every time causing a few preemptory finish Tweets. Also the tracking would sometimes lag behind the actual race and sometimes would be instant. With a few tweaks throughout the day though it worked well, and my dad "Ironman Grumps" finished in 16 hours, 50 minutes.

Uses TweetSharp and the HtmlAgilityPack, both great libraries which made it possible for me to make this so quickly.


Reflections on Ironman Wisconsin 2014
------------
My dad completed his second Ironman, Ironman Wisconsin. I updated the code to incorporate some changes.

- Lake Placid's tracking site had problems with updating. Updates were extremely delayed, so I changed the voice of the Tweets to not say "I just finished my swin" to "Official time: Swim 2.2 miles". This made it look better. However the delays were almost nonexistent for Wisconsin. I'm 99% sure this has to do with the poor cell coverage at Lake Placid.
- The official tracking page forgot to open the tbody tag for the Run section. Huge pain figuring out the right XPath for that.
- The site was better behaved this time. One notable exception, when an entire section was completed, some of the splits in that section would change, not sure what changed and why. Notably, the last split would show the total, so for my dad's Bike the 112 split would show 7:51:24 as the split time. After a minute or two this would fix itself.
- I forgot to put the Html downloading in a try catch! Oops. During the Run there was a timeout that made Visual Studio pause execution, only happened once.
- Had Facebook sync with Twitter to make things easier. Ideally this would be in the app, because the syncing isn't instant.
- Dealing with site problems is tricky. For the split error mentioned above, it would be ideal if the program kept track of all messages and would delete Tweets and edit Facebook posts if there were updates. Also it would be ideal if the program would be able to sanity check times. This would require a lot of overhaul to the code though. If you do make these changes, feel free to send me a pull request.
- Family members were worried about overloading people during the event with messages. Reports from people friends with my dad said that the messages didn't show up much (unless they got a lot of interactions) which matches up with Facebook's news feed algorithm. Twitter friends of course got every message.

If and when my dad does another Ironman, I'll be incorporating some of the above changes