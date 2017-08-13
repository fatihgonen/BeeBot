using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace BeeBot.Dialogs
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
         
            // return our reply to the user
            await context.PostAsync($"I could not understand");

            context.Wait(MessageReceived);
        }
        [LuisIntent("CheckTime")]
        public async Task CheckTime(IDialogContext context, LuisResult result)
        {
            // var activity = await result as Activity;

            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            string tag = string.Empty;
            var entities = new List<EntityRecommendation>(result.Entities);

            if(result.Entities.Count>0)
            {
                tag=result.Entities.FirstOrDefault(e => e.Type == "location").Entity;
                
            }
            if(tag=="mediko")
            {
                await context.PostAsync($"Working hours of '{tag}'== 8 6 ");
            }
            else if(tag=="yemekhane")
            {
                await context.PostAsync($"{tag} starts at  9 5 ");
            }
            else
            {
                await context.PostAsync($"could not find this location {tag} ");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("ring")]
        public async Task Ring(IDialogContext context, LuisResult result)
        {
            List<string> ring_saat = new List<string>(new string[] { "8:00", "8:30" ,"9:00","9:30","10:00","10:30","10:50","11:10","11:30","11:50","12:10","12:30","12:50",
            "13:10","13:30","13:50","14:10","14:30","14:50","15:10","15:30","15:50","16:10","16:30","16:50","17:10","17:30",
            "17:50","18:20","18:50","19:20"});

            for (int i = 0; i< ring_saat.Count; i++)
            {
                TimeSpan now = DateTime.Now.TimeOfDay;
                TimeSpan ring = TimeSpan.Parse(ring_saat[i]);
               if(ring > now)
                {

                    await context.PostAsync($" Soonest shuttle hours:");
                    await context.PostAsync($" {ring_saat[i]} ");
                    if (i+1<ring_saat.Count)
                    {
                        await context.PostAsync($" {ring_saat[i+1]} ");
                        if (i + 2< ring_saat.Count)
                        {
                            await context.PostAsync($" {ring_saat[i + 2]} ");
                        }
                    }
                    break;
                }
            }
            context.Wait(MessageReceived);
        }
        [LuisIntent("HocaSorgu")]
        private async Task HocaAra(IDialogContext context, LuisResult result)
        {
            string name =   
                SqlConnection con = new SqlConnection("Server=tcp:bot-server.database.windows.net,1433;Initial Catalog=BeeBot_Db;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;         Initial Catalog=YourSqlServerDatabase;         Persist Security Info=False;         User ID=bot-admin;         Password=cookie-5843;         MultipleActiveResultSets=False;         Encrypt=True;         TrustServerCertificate=False;         Connection Timeout = 30;");
            await context.PostAsync($"Hoca");

            context.Wait(MessageReceived);
        }

    }
}