using System;
using System.Collections.Generic;
using System.Linq;

namespace Design_Twitter
{
  class Program
  {
    static void Main(string[] args)
    {
      Twitter twitter = new Twitter();
      twitter.PostTweet(1, 5);
      var feeds = twitter.GetNewsFeed(1);
      Console.WriteLine(string.Join(",", feeds));
      twitter.Follow(1, 2);
      twitter.PostTweet(2, 6);
      feeds = twitter.GetNewsFeed(1);
      Console.WriteLine(string.Join(",", feeds));
      twitter.Unfollow(1, 2);
      feeds = twitter.GetNewsFeed(1);
      Console.WriteLine(string.Join(",", feeds));
      //twitter.PostTweet(1, 1);
      //var feeds = twitter.GetNewsFeed(1);
      //twitter.Follow(2, 1);
      //feeds = twitter.GetNewsFeed(2);
      //twitter.Unfollow(2, 1);
      //feeds = twitter.GetNewsFeed(2);
    }
  }

  public class Twitter
  {
    // This comparer will be taken care to put the newest tweet at the top of the sortedSet.
    public class MaxHeap : IComparer<Tweet>
    {
      public int Compare(Tweet a, Tweet b)
      {
        if (a.Created < b.Created) return 1;
        else if (a.Created == b.Created) return 0;
        else return -1;
      }
    }

    // Tweet class has a tweet Id and the time when tweet is created
    public class Tweet
    {
      public int TweetId;
      public DateTime Created;

      public Tweet(int tweetId, DateTime created)
      {
        TweetId = tweetId;
        Created = created;
      }
    }

    // This is our in memory table.
    // This will hold the user information, list of user tweets and follwing's list
    public Dictionary<int, (List<Tweet>, List<int>)> table;
    public Twitter()
    {
      table = new Dictionary<int, (List<Tweet>, List<int>)>();
    }

    public void PostTweet(int userId, int tweetId)
    {
      if (!table.ContainsKey(userId))
      {
        table[userId] = (new List<Tweet>(), new List<int>());
      }

      var (tweets, following) = table[userId];
      // new tweet always add to the top.
      tweets.Insert(0, new Tweet(tweetId, DateTime.Now));
      table[userId] = (tweets, following);
    }

    public IList<int> GetNewsFeed(int userId)
    {
      var feeds = new SortedSet<Tweet>(new MaxHeap());
      if (!table.ContainsKey(userId)) return new List<int>();
      // get user own tweets.
      var (userOwnTweets, following) = table[userId];
      // Add them into the feed. take only top 10
      foreach (var utweet in userOwnTweets.Take(10))
      {
        feeds.Add(utweet);
      }
      // get user following people tweets. 
      foreach (int celeb in following)
      {
        // get celeb tweets 
        if (table.ContainsKey(celeb))
        {
          var (celeTweets, _) = table[celeb];
          // add these tweets in user tweet list, take only top 10
          foreach (Tweet t in celeTweets.Take(10))
            feeds.Add(t);
        }
      }
      // get the top 10 tweet ids
      var tweetIds = feeds.Select(x => x.TweetId).Take(10).ToList();
      return tweetIds;
    }

    public void Follow(int followerId, int followeeId)
    {
      if (!table.ContainsKey(followerId))
      {
        table[followerId] = (new List<Tweet>(), new List<int>());
      }

      var (tweets, following) = table[followerId];
      following.Add(followeeId);
      table[followerId] = (tweets, following);
    }

    public void Unfollow(int followerId, int followeeId)
    {
      var (tweets, following) = table[followerId];
      following.Remove(followeeId);
      table[followerId] = (tweets, following);
    }
  }



  public class TwitterSimple
  {
    Dictionary<int, HashSet<(int tweet, int time)>> tweets;
    Dictionary<int, HashSet<int>> iAmFollowing;
    int time;
    public TwitterSimple()
    {
      tweets = new Dictionary<int, HashSet<(int tweet, int time)>>();
      iAmFollowing = new Dictionary<int, HashSet<int>>();
      time = 1;
    }

    public void PostTweet(int userId, int tweetId)
    {
      if (!tweets.ContainsKey(userId)) tweets.Add(userId, new HashSet<(int tweet, int time)>());
      tweets[userId].Add((tweetId, time++));
    }

    public IList<int> GetNewsFeed(int userId)
    {
      var result = new List<int>();
      // get people whom I am following
      var celebs = iAmFollowing.ContainsKey(userId) ? iAmFollowing[userId] : null;
      // get my tweets
      var myTweets = tweets.ContainsKey(userId) ? tweets[userId] : null;
      var allTweets = new List<(int tweet, int time)>();
      if (myTweets != null)
        allTweets.AddRange(myTweets.ToList());
      // get celebs tweets
      if (celebs != null)
      {
        foreach (var celeb in celebs)
        {
          var celebsTweets = tweets.ContainsKey(celeb) ? tweets[celeb] : null;
          if (celebsTweets != null)
            allTweets.AddRange(celebsTweets.ToList());
        }
      }
      //  Tweets must be ordered from most recent to least recent.
      result = allTweets.OrderByDescending(twt => twt.time).Take(10).Select(twt => twt.tweet).ToList();
      return result;

    }

    public void Follow(int followerId, int followeeId)
    {
      if (!iAmFollowing.ContainsKey(followerId)) iAmFollowing.Add(followerId, new HashSet<int>());
      iAmFollowing[followerId].Add(followeeId);
    }

    public void Unfollow(int followerId, int followeeId)
    {
      if (iAmFollowing.ContainsKey(followerId))
      {
        var peopleIAmFollowing = iAmFollowing[followerId];
        peopleIAmFollowing.Remove(followeeId);
      }
    }
  }

  /**
   * Your Twitter object will be instantiated and called as such:
   * Twitter obj = new Twitter();
   * obj.PostTweet(userId,tweetId);
   * IList<int> param_2 = obj.GetNewsFeed(userId);
   * obj.Follow(followerId,followeeId);
   * obj.Unfollow(followerId,followeeId);
   */
}
