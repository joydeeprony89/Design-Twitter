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
      //twitter.PostTweet(1, 5);
      //var feeds = twitter.GetNewsFeed(1);
      //Console.WriteLine(string.Join(",", feeds));
      //twitter.Follow(1, 2);
      //twitter.PostTweet(2, 6);
      //feeds = twitter.GetNewsFeed(1);
      //Console.WriteLine(string.Join(",", feeds));
      //twitter.Unfollow(1, 2);
      //feeds = twitter.GetNewsFeed(1);
      //Console.WriteLine(string.Join(",", feeds));
      twitter.PostTweet(1, 1);
      var feeds = twitter.GetNewsFeed(1);
      twitter.Follow(2, 1);
      feeds = twitter.GetNewsFeed(2);
      twitter.Unfollow(2, 1);
      feeds = twitter.GetNewsFeed(2);
    }
  }

  public class Twitter
  {
    public class MaxHeap : IComparer<Tweet>
    {
      public int Compare(Tweet a, Tweet b)
      {
        if (a.Created < b.Created) return 1;
        else if (a.Created == b.Created) return 0;
        else return -1;
      }
    }

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

    public Dictionary<int, (SortedSet<Tweet>, List<int>)> table;
    public Twitter()
    {
      table = new Dictionary<int, (SortedSet<Tweet>, List<int>)>();
    }

    public void PostTweet(int userId, int tweetId)
    {
      if (!table.ContainsKey(userId))
      {
        table[userId] = (new SortedSet<Tweet>(new MaxHeap()), new List<int>());
      }

      var (tweets, following) = table[userId];
      tweets.Add(new Tweet(tweetId, DateTime.Now));
      table[userId] = (tweets, following);
    }

    public IList<int> GetNewsFeed(int userId)
    {
      var feeds = new SortedSet<Tweet>(new MaxHeap());
      // own tweets
      if (!table.ContainsKey(userId)) return new List<int>();
      var (userOwnTweets, following) = table[userId];
      foreach (var utweet in userOwnTweets)
      {
        feeds.Add(utweet);
      }
      // get following people tweets
      foreach (int celeb in following)
      {
        // get celeb tweets
        if (table.ContainsKey(celeb))
        {
          var (celeTweets, _) = table[celeb];
          // add these tweets in user tweet list
          foreach (Tweet t in celeTweets)
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
        table[followerId] = (new SortedSet<Tweet>(new MaxHeap()), new List<int>());
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

  /**
   * Your Twitter object will be instantiated and called as such:
   * Twitter obj = new Twitter();
   * obj.PostTweet(userId,tweetId);
   * IList<int> param_2 = obj.GetNewsFeed(userId);
   * obj.Follow(followerId,followeeId);
   * obj.Unfollow(followerId,followeeId);
   */
}
