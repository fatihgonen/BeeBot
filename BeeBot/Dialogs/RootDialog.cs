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
using Microsoft.Bot.Connector;

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
            //Coure_db data = new Coure_db();
            //data.Lecturer_info();





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
            string to_write;
            for (int i = 0; i< ring_times.Count; i++)
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                TimeSpan ring = TimeSpan.Parse(ring_times[i]);
               if(ring > now)
                {
                    to_write=" Soonest shuttle hours: ";
                    to_write=to_write+ring_times[i]+"|";
                    if (i+1<ring_times.Count)
                    {
                        to_write=to_write+ring_times[i+1]+"|";
                        if (i + 2< ring_times.Count)
                        {
                            to_write=to_write+ring_times[i + 2];
                        }
                    }
                    await context.PostAsync(to_write);
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
                    if (reader[0].ToString().ToLower().Contains(ToSearch))
                    {
                        await context.PostAsync($"Found");
                        await context.PostAsync(String.Format("{0} \t |Mail Adress:  {1} \t | Office No: {2}",
                        reader[0], reader[1], reader[2]));
                    }
                }
            }
            context.Wait(MessageReceived);
        }

       
        [LuisIntent("CheckLecture")]
        public async Task CheckLecture(IDialogContext context, LuisResult result)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Course_info", conn);

            string search_type;
            string search_key;
            search_key=result.Entities[0].Entity;
            search_type = result.Entities[0].Type;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                List<Fact> lesson = new List<Fact>();
                    if (search_type == "CRN")
                    {
                        while (reader.Read())
                        {
                            if (reader[0].ToString().ToLower().Contains(search_key))
                            {
                            //    await context.PostAsync($"Lesson with CRN {search_key}");
                            //    await context.PostAsync(String.Format("CRN: {0} \t\t |CourseCode:  {1} \t\t |" +
                            //    "CourseTitle: {2} \t |Instructor: {3} \t\t |Building: {4} \t\t |Day {5} \t |" +
                            //    "Time: {6} \t\t |Room: {7} \t\t |Capacity: {8} ",
                            //    reader[0], reader[1], reader[2], reader[3], reader[4],  reader [5], reader[6],reader[7],reader[8]));                        

                            var receipt = new ReceiptCard
                            {

                                Title = reader[2].ToString(),
                                Facts = new List<Fact> { new Fact("CRN", reader[0].ToString()),
                                new Fact("CourseCode",reader[1].ToString()), new Fact("Instructor",reader[3].ToString()),
                                new Fact("Building",reader[4].ToString()), new Fact("Day",reader[5].ToString()),
                                new Fact("Time",reader[6].ToString()), new Fact("Room",reader[7].ToString()),
                            },
                                Tax=null,
                                Total=null,
                            };
                            var message1 = context.MakeMessage();
                            message1.Attachments = new List<Attachment>();
                            message1.Attachments.Add(receipt.ToAttachment());
                            await context.PostAsync(message1);

                        }
                    }
                    }
                    else if (search_type == "LecturerName")
                    {
                        while (reader.Read())
                        {
                            if (reader[3].ToString().ToLower().Contains(search_key))
                            {
                                await context.PostAsync($"Lesson with Lecturer {search_key}");
                                await context.PostAsync(String.Format("CRN: {0} \t\t |CourseCode:  {1} \t\t |" +
                                "CourseTitle: {2} \t |Instructor: {3} \t\t |Building: {4} \t\t |Day {5} \t |" +
                                "Time: {6} \t\t |Room: {7} \t\t |Capacity: {8}",
                                reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6],
                                reader[7], reader[8]));
                            }
                        }
                    }   
                    else if (search_type == "CourseCode")
                    {
                        while (reader.Read())
                        {
                            if (reader[1].ToString().ToLower().Contains(search_key))
                            {
                                await context.PostAsync($"Lesson with Course Code {search_key}");
                                await context.PostAsync(String.Format("CRN: {0} \t\t |CourseCode:  {1} \t\t |" +
                                "CourseTitle: {2} \t |Instructor: {3} \t\t |Building: {4} \t\t |Day {5} \t |" +
                                "Time: {6} \t\t |Room: {7} \t\t |Capacity: {8}",
                                reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6],
                                reader[7], reader[8]));
                            }
                        }
                        await context.PostAsync("Do you know? You can find lecture notes for this course and other courses in Mert Kırtasiye located Main Campus");

                    }
            }

            var message = context.MakeMessage();

            var receiptCard = new ReceiptCard
            {
                Title = "John Doe",
                Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****") },
                Tax = "$ 7.50",
                Total = "$ 90.95",
            };

            message.Attachments = new List<Attachment>();
            message.Attachments.Add(receiptCard.ToAttachment());



            await context.PostAsync(message);




            conn.Close();
            context.Wait(MessageReceived);
        }

        [LuisIntent("SearchLocation")]
        public async Task SearchLocation(IDialogContext context, LuisResult result)
        {

            string location;
            location = result.Entities[0].Entity;

            if(location=="yemekhane")
            {
                await context.PostAsync($"https://goo.gl/maps/ygfp7KGjT7p");
            }
            else if(location=="mediko")
            {
                await context.PostAsync($"https://goo.gl/maps/KTENZLoRuNT2");
            }
            else if (location=="ogrenci isleri")
            {
                await context.PostAsync($"https://goo.gl/maps/oUfPT19Boe22");
            }
            else if (location=="med")
            {
                await context.PostAsync($"https://goo.gl/maps/DioWWKCFRaU2");
            }
            else if (location=="eeb")
            {
                await context.PostAsync($"https://goo.gl/maps/zfCAP9Zntz22");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("CheckMeal")]
        public async Task  CheckMeal(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($" The Meal is: BALIK PANE, ETSIZ BULGURLU ISPANAK" +
                $"SALCALI MAKARNA , ROKA SALATASI , OSMANLI TULUMBA TATLISI");
            await context.PostAsync($"Do you know ? Today  KOFTE IZGARA menu in FanFan Cafe is just 15 tl");

        }
    }
}