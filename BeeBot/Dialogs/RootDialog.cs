using AngleSharp;
using HtmlAgilityPack;
using HtmlParser.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Database;
namespace Database.Dialogs
{
    [LuisModel("dea53d37-6228-4476-aea3-02ba7b60eed9", "d0ec767e58ca4f72a7198d4cc49d56da")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);

            return Task.CompletedTask;
        }
        [LuisIntent("None")]
        private async Task MessageReceivedAsync(IDialogContext context, LuisResult result)
        {         
            await context.PostAsync($"I could not understand, please try again");
            context.Wait(MessageReceived);
        }
        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hi i am a BeeBot and i am here to help ITU students. If you want further information about how to use please try writing help.");
            Coure_db data = new Coure_db();
            data.Lecturer_info();
            context.Wait(MessageReceived);

        }
        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You can ask me some questions about ring times, courses and lecturer information ");
            context.Wait(MessageReceived);
        }
        [LuisIntent("CheckTime")]
        public async Task CheckTime(IDialogContext context, LuisResult result)
        {
            string myentity = string.Empty;
            var entities = new List<EntityRecommendation>(result.Entities);

            if(result.Entities.Count>0)
            {
                myentity=result.Entities.FirstOrDefault(e => e.Type == "location").Entity;               
            }
            if(myentity=="mediko")
            {
                await context.PostAsync($"Working hours of '{myentity}'== 9-6 ");
            }
            else if(myentity=="yemekhane"|| myentity =="lunch"||myentity=="dinner")
            {
                if(myentity == "lunch")
                {
                    await context.PostAsync($"{myentity} starts at  11.30 and continues till the 14.30 ");
                }
                if(myentity == "dinner")
                {
                    await context.PostAsync($"{myentity} starts at  17.30 and continues till the 19.30 ");
                }
                if(myentity == "yemekhane")
                {
                    await context.PostAsync($"Lunch starts at  11.30 and continues till the 14.30 and dinner starts at  17.30 and continues till the 19.30  ");

                }
            }
            else
            {
                await context.PostAsync($"could not find this location {myentity} ");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("ring")]
        public async Task Ring(IDialogContext context, LuisResult result)
        {
                   
            List<string> ring_times = new List<string>(new string[] { "8:00", "8:30" ,"9:00","9:30","10:00",
                 "10:30","10:50","11:10","11:30","11:50","12:10","12:30","12:50","13:10","13:30","13:50",
            "14:10","14:30","14:50","15:10","15:30","15:50","16:10","16:30","16:50",
                "17:10","17:30","17:50","18:20","18:50","19:20"});

            for (int i = 0; i< ring_times.Count; i++)
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                TimeSpan ring = TimeSpan.Parse(ring_times[i]);
               if(ring > now)
                {
                    await context.PostAsync($" Soonest shuttle hours:");
                    await context.PostAsync($" {ring_times[i]} ");
                    if (i+1<ring_times.Count)
                    {
                        await context.PostAsync($" {ring_times[i+1]} ");
                        if (i + 2< ring_times.Count)
                        {
                            await context.PostAsync($" {ring_times[i + 2]} ");
                        }
                    }
                    break;
                }
            }

            context.Wait(MessageReceived);
        }
       
        [LuisIntent("CheckLecturerInfo")]
        public async Task SearchLecturer(IDialogContext context, LuisResult result)
        {
            string ToSearch="";
            foreach (var searchEntity in result.Entities)
            {
                if (searchEntity.Type=="LecturerName")
                {
                   ToSearch = searchEntity.Entity;
                }
            }

            string connStr = ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM HocaBilgi", conn);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if(reader[0].ToString().ToLower().Contains(ToSearch))
                    {
                        await context.PostAsync($"Found");
                        await context.PostAsync(String.Format("{0} \t |Mail Adress  {1} \t | Office No {2}",
                        reader[0], reader[1], reader[2]));
                    }
                }  
            }
        }

        [LuisIntent("SearchBook")]
        public async Task SearchBook(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Searching");

       
                string Url = "";
                HtmlWeb web = new HtmlWeb();

            HtmlDocument doc = web.Load(Url);
            await context.PostAsync($"Searching2");

            //string metascore = doc.DocumentNode.SelectNodes("//*[@id=\"main\"]/div[3]/div/div[2]/div[1]/div[1]/div/div/div[2]/a/span[1]")[0].InnerText;
            string SearchResult = doc.DocumentNode.SelectNodes("//*[@id=\"entry - list\"]/li[1]/div[1]")[0].InnerText;
            //var SearchResult = doc.DocumentNode.SelectNodes("//*[@id=\"FETCH - itu_catalog_b209553882\"]/div[1]/div[2]/div/h3/span")[0].InnerText;

            //*[@id="FETCH-itu_catalog_b209553882"]
            await context.PostAsync($"Searching3");

            //string summary = doc.DocumentNode.SelectNodes("//*[@id=\"main\"]/div[3]/div/div[2]/div[2]/div[1]/ul/li/span[2]/span/span[1]")[0].InnerText;
            await context.PostAsync(SearchResult);
            await context.PostAsync($"Searching4");

            //*[@id="FETCH-itu_catalog_b209553882"]/div[1]/div[2]/div

            context.Wait(MessageReceived);

            //*[@id="results"]/div/ul/li[2]
            //*[@id="results"]/div/ul/li[2]
        }

        [LuisIntent("CheckLecture")]
        public async Task CheckLecture(IDialogContext context, LuisResult result)
        {

        
            // Setup the configuration to support document loading
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            // Load the names of all The Big Bang Theory episodes from Wikipedia
      
            string b = "BIO";
            var address = "http://www.sis.itu.edu.tr/tr/ders_programlari/LSprogramlar/prg.php?fb=" + b;
            // Asynchronously get the document in a new context using the configuration
            var document = await BrowsingContext.New(config).OpenAsync(address);
            // This CSS selector gets the desired content

            var cellSelector1 = "tr td:nth-child(1)";//Crn
            var cellSelector2 = "tr td:nth-child(2)";//Course Code
            var cellSelector3 = "tr td:nth-child(3)";//Course Title
            var cellSelector4 = "tr td:nth-child(4)";//Instructor
            var cellSelector5 = "tr td:nth-child(5)";//Building
            var cellSelector6 = "tr td:nth-child(6)";//Day
            var cellSelector7 = "tr td:nth-child(7)";//Time
            var cellSelector8 = "tr td:nth-child(8)";//Capacity
            var cellSelector9 = "tr td:nth-child(9)";//Major Restriction

            var crn = document.QuerySelectorAll(cellSelector1).Skip(5).ToList();
            var CourseCode = document.QuerySelectorAll(cellSelector2).Skip(4).ToList();
            var CourseTitle = document.QuerySelectorAll(cellSelector3).Skip(3).ToList();
            var Instructor = document.QuerySelectorAll(cellSelector4).Skip(2).ToList();
            var Building = document.QuerySelectorAll(cellSelector5).Skip(2).ToList();
            var Day = document.QuerySelectorAll(cellSelector6).Skip(2).ToList();
            var Time = document.QuerySelectorAll(cellSelector7).Skip(2).ToList();
            var Capacity = document.QuerySelectorAll(cellSelector8).Skip(2).ToList();
            var Restriction = document.QuerySelectorAll(cellSelector9).Skip(2).ToList();

            var records = new List<Record>();
            for (int i = 0; i < CourseTitle.Count - 1; i++)
            {
                var rec = new Record()
                {
                    CourseTitle_rec = CourseTitle[i].TextContent,
                    Instructor_rec = Instructor[i].TextContent,
                    Crn_rec = crn[i].TextContent,
                    CourseCode_rec = CourseCode[i].TextContent,
                    Building_rec = Building[i].TextContent,
                    Day_rec = Day[i].TextContent,
                    Time_rec = Time[i].TextContent,
                    Capacity_rec = Capacity[i].TextContent,
                    Restriction_rec = Restriction[i].TextContent,
                };

                records.Add(rec);
            }
            List<Record> SearchResults;



            string search_key;
            foreach (var searchEntity in result.Entities)
            {
                await context.PostAsync(searchEntity.Entity);
                search_key = searchEntity.Entity;

                if (searchEntity.Type == "CRN")
                {
                    SearchResults = records.Where(a => a.Crn_rec.ToLower().Contains(search_key.ToLower())).ToList();

                    await context.PostAsync($"Lesson with CRN {searchEntity.Entity}");

                    foreach (var value in SearchResults)
                    {
                        await context.PostAsync($"Lecturer: {value.Instructor_rec}");
                        await context.PostAsync($"Building: {value.Building_rec}");
                        await context.PostAsync($"Course Title: {value.CourseTitle_rec}");
                        await context.PostAsync($"Day: {value.Day_rec}");
                        await context.PostAsync($"Time: {value.Time_rec}");
                    }
                }
                else if (searchEntity.Type == "LecturerName")
                {
                    SearchResults = records.Where(a => a.Instructor_rec.ToLower().Contains(search_key.ToLower())).ToList();

                    await context.PostAsync($"Lessons {searchEntity.Entity} give:");

                    foreach (var value in SearchResults)
                    {
                        await context.PostAsync($"Course Title: {value.CourseTitle_rec}");
                        await context.PostAsync($"Course Code: {value.CourseCode_rec}");
                        await context.PostAsync($"Building: {value.Building_rec}");
                        await context.PostAsync($"Day: {value.Day_rec}");
                        await context.PostAsync($"Time: {value.Time_rec}");
                    }

                }
                else if (searchEntity.Type == "CourseCode")
                {
                    SearchResults = records.Where(a => a.CourseCode_rec.ToLower().Contains(search_key.ToLower())).ToList();
                    await context.PostAsync($"Lessons with course code{searchEntity.Entity}");
                    foreach (var value in SearchResults)
                    {
                        await context.PostAsync($"Building: {value.Building_rec}");
                        await context.PostAsync($"Course Title: {value.CourseTitle_rec}");
                        await context.PostAsync($"Day: {value.Day_rec}");
                        await context.PostAsync($"Time: {value.Time_rec}");
                    }
                }
            }

            context.Wait(MessageReceived);
        }

    }
}