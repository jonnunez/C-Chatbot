using BotApp.Classes;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using BotApp.helpers;

namespace BotApp.dialogs
{
    public class DirectAnswerDialog
    {
        public static async Task directAnswer(string lastEvent, BotAccessors accessors, string score, WaterfallStepContext step, CancellationToken cancellationToken = default(CancellationToken))
        {
            var recognizerResult = await accessors.LuisServices[Settings.LuisName01].RecognizeAsync(step.Context, cancellationToken);
            var topIntent = recognizerResult?.GetTopScoringIntent();

            string LastSearch = step.Context.Activity.Text;
            if (lastEvent.ToString().ToLower() != "postback")
            {
                await accessors.LastSearchPreference.SetAsync(step.Context, LastSearch);
                await accessors.UserState.SaveChangesAsync(step.Context, false, cancellationToken);
            }
            step.Context.Activity.Text = score;
            var response = await accessors.QnAServices[Settings.QnAName01].GetAnswersAsync(step.Context);

            if (response != null && response.Length > 0)
            {
                string responseType = "text";

                if (!string.IsNullOrEmpty(responseType))
                {
                    string LastAnswer = response[0].Answer;
                    if (lastEvent.ToString().ToLower() != "postback")
                    {
                        await accessors.LastAnswerPreference.SetAsync(step.Context, LastAnswer);
                        await accessors.UserState.SaveChangesAsync(step.Context, false, cancellationToken);
                    }
                    //if (true)
                    //{

                    //}
                    TrackEvents.TrackConversation(LastSearch, LastAnswer, "Direct Answer", topIntent.Value.score.ToString(), topIntent.Value.intent, from: step.Context.Activity.From);
                    await step.Context.SendCustomResponseAsync(response[0].Answer, responseType);
                    await step.BeginDialogAsync(FeedbackDialog.dialogId, null, cancellationToken);
                }
            }
            else
            {
                var message = QNABotSettings.sorrynextrelease;
                await step.Context.SendCustomResponseAsync(message);
            }
        }
        public static async Task directAnswerCarousel(string lastEvent, BotAccessors accessors, WaterfallStepContext step, CancellationToken cancellationToken = default(CancellationToken))
        {
            var recognizerResult = await accessors.LuisServices[Settings.LuisName01].RecognizeAsync(step.Context, cancellationToken);
            var topIntent = recognizerResult?.GetTopScoringIntent();

            string LastSearch = step.Context.Activity.Text;
            if (lastEvent.ToString().ToLower() != "postback")
            {
                await accessors.LastSearchPreference.SetAsync(step.Context, LastSearch);
                await accessors.UserState.SaveChangesAsync(step.Context, false, cancellationToken);
            }
            var response = await accessors.QnAServices[Settings.QnAName01].GetAnswersAsync(step.Context);

            if (response != null && response.Length > 0)
            {
                string responseType = "text";

                if (!string.IsNullOrEmpty(responseType))
                {
                    string LastAnswer = response[0].Answer;
                    if (lastEvent.ToString().ToLower() != "postback")
                    {
                        await accessors.LastAnswerPreference.SetAsync(step.Context, LastAnswer);
                        await accessors.UserState.SaveChangesAsync(step.Context, false, cancellationToken);
                    }
                    
                    TrackEvents.TrackConversation(LastSearch, LastAnswer, "Direct Answer from Carousel", topIntent.Value.score.ToString(), topIntent.Value.intent, from: step.Context.Activity.From);
                    await step.Context.SendCustomResponseAsync(response[0].Answer, responseType);
                    await step.BeginDialogAsync(FeedbackDialog.dialogId, null, cancellationToken);
                }
            }
            else
            {
                var message = QNABotSettings.sorrynextrelease;
                await step.Context.SendCustomResponseAsync(message);
            }
        }
    }
}