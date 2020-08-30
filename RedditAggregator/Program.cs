using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reddit;
using Reddit.Controllers.Structures;

namespace RedditAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            var aggregatorRefreshToken = Properties.Settings.Default.aggregatorRefreshToken;
            var aggregatorId = Properties.Settings.Default.aggregatorAppID;
            var random = new Random();
            var redditQuestion = "";
            var redditAnswer = "";
            var redditCrazyNews = "";
            var redditNormalNews = "";

            try
            {
                var r = new RedditClient(aggregatorId, aggregatorRefreshToken);
                Console.WriteLine($"User: {r.Account.Me.Name}");
                var askReddit = r.Subreddit("AskReddit");
                var crazyNews = r.Subreddit("nottheonion");
                var normalNews = r.Subreddit("news");
                var crazyNewsNewPosts = crazyNews.Posts.New;
                var normalNewsNewPosts = normalNews.Posts.New;
                var topPost = askReddit.Posts.Top[0].Title;
                var hotPosts = askReddit.Posts.Hot;
                // rank of the comment you'd like to pull
                var commentRank = 0;
                Console.WriteLine($"About this thread: {askReddit.About().Title}\n");
                Console.WriteLine($"Top Post: {topPost}");

                while (redditQuestion == "")
                {
                    var randomQuestionRedditIndex = random.Next(hotPosts.Count);
                    var randomCrazyNewsRedditIndex = random.Next(hotPosts.Count);
                    var randomNormalNewsRedditIndex = random.Next(normalNewsNewPosts.Count);
                    var randomQuestionPost = hotPosts[randomQuestionRedditIndex];
                    var randomCrazyNewsPost = crazyNewsNewPosts[randomCrazyNewsRedditIndex];
                    var randomNormalNewsPost = crazyNewsNewPosts[randomNormalNewsRedditIndex];
                    if (!(randomQuestionPost.Title.Contains("Reddit") == false &
                          randomQuestionPost.Comments.Top[commentRank].Body.Contains("Reddit") == false)) continue;
                    redditQuestion = randomQuestionPost.Title;
                    redditAnswer = randomQuestionPost.Comments.Top[commentRank].Body;
                    redditCrazyNews = randomCrazyNewsPost.Title;
                    redditNormalNews = randomNormalNewsPost.Title;
                }

                Website.UpdateGetLuckyPage(redditQuestion, redditAnswer, redditCrazyNews, redditNormalNews);

                //for (int i = 0; i < 10; i++)
                //{
                //    var hotPost= hotPosts[i];
                //    if (hotPost.Title.Contains("Reddit") == false & hotPost.Comments.Top[0].Body.Contains("Reddit") == false)
                //    {
                //        Console.WriteLine($"--- RANDOM: {hotPosts[randomRedditIndex].Title} ---");
                //        Console.WriteLine($"Title: {i+1}. {hotPost.Title}");
                //        Console.WriteLine($"Upvote #/ Ratio: {hotPost.UpVotes} || {hotPost.UpvoteRatio}");
                //        Console.WriteLine($"Top Comment: {hotPost.Comments.Top[0].Body}\n");
                //    }
                //}

                //Console.WriteLine("Moderators: \n");
                //foreach (var moderator in askReddit.Moderators)
                //{
                //    Console.WriteLine(moderator.Name);
                //}
                //Console.ReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine($"Work smarter not harder mate. Error encountered while establishing connection: {e}");
                throw;
            }
        }
    }
}
