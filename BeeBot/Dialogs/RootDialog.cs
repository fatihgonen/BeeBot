using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;
using System.Linq;

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
    }
}