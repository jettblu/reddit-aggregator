using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Renci.SshNet;

namespace RedditAggregator
{
    class Website
    {
        private static String PathGetLucky = "C:\\Users\\AirBu\\Documents\\blu web\\lucky.html";
        private static string PathRandomFile = "";
        private static string PathRandomFileDirectory = "C:\\Users\\AirBu\\Documents\\blu web\\thoughts";
        private static List<string> FilesToUpload = new List<string>();
        private static List<string> ContentList = new List<string>();

        private static void BeginningHtml(List<string> contentList, string title)
        {
            contentList.Add("<!doctype html> ");
            contentList.Add("<html>");
            contentList.Add("<head>");
            contentList.Add(@"<meta charset=""utf - 8"" name=""viewport"" content=""width=device-width,initial-scale=1.0""> ");
            contentList.Add($"<title> Lucky </title> ");
            contentList.Add(@"<link rel = ""stylesheet"" href=""styles.css"">");
            contentList.Add(@"<link rel = ""shortcut icon"" href = ""icon.ico"">");
            contentList.Add("</head>");
            contentList.Add(@"<body> <div id =""mainContainer"" >");
            contentList.Add("<div>");
            contentList.Add(@"<div class=""dropdown"">");
            contentList.Add(@"<button class=""dropbtn"">Explore</button>");
            contentList.Add(@"<div class=""dropdown-content"">");
            contentList.Add(@"<a href = ""writing.html""> Text </a>");
            contentList.Add(@"<a href=""video.html""> Images </a>");
            contentList.Add(@"<a href = ""thoughts/books.html""> Books </a>");
            contentList.Add("</div>");
            contentList.Add("</div>");
            contentList.Add(@"<a href=""index.html"">");
            contentList.Add(@"<div id = ""logo""><img src=""head.jpg"" alt=""print(Jett)"" class=""center""></div>");
            contentList.Add("</a>");
            contentList.Add("</div>");
            contentList.Add($@"<h1 class=""post"">{title}</h1>");
        }

        private static void ChooseRandomFile(string directory)
        {
            var rand = new Random();
            var files = Directory.GetFiles(directory, "*.html");
            PathRandomFile = files[rand.Next(files.Length)];
        }
        private static void CreateNewFile(string redditQuestion, string redditAnswer, string redditCrazyNews, string redditNews, string title)
        {
            try
            {
                BeginningHtml(ContentList, title);
                ChooseRandomFile(PathRandomFileDirectory);
                var randomThoughtTitle = Path.GetFileNameWithoutExtension(PathRandomFile);
                ContentList.Add($@"<p><span style=""color: #21ABFF"">Random Question:</span> {redditQuestion} </p>");
                ContentList.Add($@"<p><span style=""color: #21ABFF"">Possible Answer:</span> {redditAnswer} </p>");
                ContentList.Add("<br>");
                ContentList.Add($@"<p><span style=""color: #21ABFF"">(Wish was fake) News:</span> {redditCrazyNews} </p>");
                ContentList.Add($@"<p><span style=""color: #21ABFF"">(Real) News:</span> {redditNews} </p>");
                ContentList.Add("<br>");
                ContentList.Add($@"<p><span style=""color: #21ABFF"">Random Thought:</span> <a href = ""thoughts/{randomThoughtTitle}.html"" class=""thought"">{randomThoughtTitle}</a></p>");
                ContentList.Add("</div></body>");
                ContentList.Add("</html>");

                using (FileStream fileStream = new FileStream(PathGetLucky, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        foreach (var htmlLine in ContentList)
                        {
                            fileWriter.WriteLine(htmlLine);
                        }
                    }
                }
                FilesToUpload.Add(PathGetLucky);
                Console.WriteLine($"NOICE! File entitled {title} created!");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error encountered while creating file. Error: {exception}");
                return;
            }
        }

        private static void FileUploadSFTP(bool isConnectionTest, List<string> filesList)
        {
            var host = Properties.Settings.Default.Host;
            var port = Properties.Settings.Default.Port;
            var username = Properties.Settings.Default.Username;
            var password = Properties.Settings.Default.Password;
            var remoteDirectory = "/home/jettsozq/public_html";

            using (var client = new SftpClient(host, port, username, password))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    // returns connection status if IsConnected equals true
                    if (isConnectionTest)
                    {
                        Console.WriteLine("Connected baby");
                        return;
                    }
                    // uploads each file in FilesToUpload list
                    foreach (var file in FilesToUpload)
                    {
                        // switches directory client writes to based on files in upload list. If files are home pages...sent to public_html
                        // if individual thoughts or books, sent to respective directories within public_html
                        client.ChangeDirectory(remoteDirectory);
                        using (var fileStream = new FileStream(file, FileMode.Open))
                        {

                            client.BufferSize = 4 * 1024; // bypass Payload error large files
                            try
                            {
                                client.UploadFile(fileStream, Path.GetFileName(file));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Error while uploading file. Check path and current status of relevant files. {e}");
                                return;
                            }
                        }
                    }
                    Console.WriteLine("Thanks for sharing with the world. Mission complete captain.");
                }
                else
                {
                    Console.WriteLine("Awh. Failed to connect.");
                }
            }
        }

        public static void UpdateGetLuckyPage(string questionFromReddit, string answerFromReddit, string crazyNewsFromReddit, string newsFromReddit, string title = "The Daily Random", bool isConnectTest = false)
        {
            CreateNewFile(questionFromReddit, answerFromReddit, crazyNewsFromReddit, newsFromReddit, title);
            FileUploadSFTP(isConnectTest, FilesToUpload);
        }
    }
}
