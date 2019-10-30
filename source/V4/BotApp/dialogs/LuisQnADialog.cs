using BotApp.Classes;
using BotApp.dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;
using BotApp.helpers;
using Microsoft.ApplicationInsights;
using System.Linq;
using System.Collections.Generic;
using static BotApp.helpers.ConfidenceScoreEnums;
using Newtonsoft.Json;

namespace BotApp
{
    public class LuisQnADialog : ComponentDialog
    {
        #region varibles
        public const string dialogId = "LuisQnADialog";
        private BotAccessors accessors = null;
        private BotState _userState;

        public LuisQnADialog(BotAccessors accessors) : base(dialogId)
        {
            this.accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
            _userState = accessors.UserState;
            AddDialog(new WaterfallDialog(dialogId, new WaterfallStep[]
            {
                ProcessQuestionDialog,
                EndDialog
            }));

            AddDialog(new FeedbackDialog(accessors));
        }
        #endregion

        #region Methods

        /// <summary>
        /// based on question returning answer from luis n qna.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> ProcessQuestionDialog(WaterfallStepContext step, CancellationToken cancellationToken = default(CancellationToken))
        {
            var question = (string)step.Result;
            step.ActiveDialog.State["question"] = question;
            string lastEvent = string.Empty;
            string searchedText = step.Context.Activity.Text;


            // for storing data into telemetry.
            string lastSearchedText = await accessors.LastSearchPreference.GetAsync(step.Context, () => { return string.Empty; });
            string lastAnswerText = await accessors.LastAnswerPreference.GetAsync(step.Context, () => { return string.Empty; });


            if (step.Context.Activity.Text != null && isValidJson(step.Context.Activity.Text))
            {
                var cButton = JsonConvert.DeserializeObject<CardButton>(step.Context.Activity.Text);
                lastEvent = "postback";
                step.Context.Activity.Text = cButton.value;
            }

            var recognizerResult = await accessors.LuisServices[Settings.LuisName01].RecognizeAsync(step.Context, cancellationToken);

            var topIntent = recognizerResult?.GetTopScoringIntent();

            var msg = step.Context.Activity.CreateReply(QNABotSettings.accurateanswer);

            if (lastEvent.ToString() == "postback")
            {
                //TrackEvents.TrackConversation(searchedText, lastAnswerText, "Carousel Direct Answer");
                await DirectAnswerDialog.directAnswerCarousel(lastEvent, accessors, step, cancellationToken); //Direct Answer Dialog
            }
            else if ((recognizerResult != null && recognizerResult.Text.Contains(QNABotSettings.noneoftheabove)) || (topIntent != null && topIntent.Value.intent == "None"))
            {

                TrackEvents.TrackConversation(lastSearchedText, lastAnswerText, "None Intent Prompt", topIntent.Value.score.ToString());
                await NoneOfAboveDialog.NoneofTheAbove(step);
            }

            else if (recognizerResult != null && (recognizerResult.Text.Contains(QNABotSettings.howtoask) || recognizerResult.Text.Contains(QNABotSettings.howtocontacthr)))
            {
                TrackEvents.TrackConversation(lastSearchedText, lastAnswerText, "How To Ask/How to Contact Prompt", topIntent.Value.score.ToString());
                await ChoiceMenuDialog.ChoiceMenu(recognizerResult.Text, step);
            }
            else
            {
                double score = 0.0;
                if (topIntent != null && topIntent.HasValue)
                {
                    score = topIntent.Value.score;
                    score = score * 100;

                    // Direct answer if its button click event of carousel dialog.
                    //if (lastEvent.ToString() == "postback")
                    //{
                    //    //    //TrackEvents.TrackConversation(searchedText, lastAnswerText, "Carousel Direct Answer");
                    //    //    await DirectAnswerDialog.directAnswer(lastEvent, accessors, topIntent.Value.intent, step, cancellationToken); //Direct Answer Dialog
                    //}

                    if(lastEvent.ToString() != "postback")
                    {
                        var confidenceScore = ThresholdHandler.ConfidenceScoreIdentification(score);

                        switch (confidenceScore)
                        {
                            case (int)ConfidenceScoreEnum.high:
                                if (topIntent.Value.intent != "None")
                                    await DirectAnswerDialog.directAnswer(lastEvent, accessors, topIntent.Value.intent, step, cancellationToken); //Direct Answer Dialog
                                break;

                            default:
                                    TrackEvents.TrackConversation(searchedText, lastAnswerText = "Carousel Options", "Carousel Prompt", confidenceScore.ToString(), topIntent.Value.intent);
                                    await CarouselDialog.Carousel(msg, recognizerResult, accessors, step, cancellationToken); //Carousel Dialog
                                break;


                                //case (int)ConfidenceScoreEnum.mid:
                                //    TrackEvents.TrackConversation(searchedText, lastAnswerText = "Carousel Options", "Carousel Prompt", confidenceScore.ToString(), topIntent.Value.intent);
                                //    await CarouselDialog.Carousel(msg, recognizerResult, accessors, step, cancellationToken); //Carousel Dialog
                                //    break;

                                //default:
                                //    TrackEvents.TrackConversation(searchedText, lastAnswerText, "None of the Above Answer", confidenceScore.ToString(), topIntent.Value.intent);
                                //    await NoneOfAboveDialog.NoneofTheAbove(step);//No Answer Dialog
                                //    break;
                        }
                    }
                }
            }
            return await step.NextAsync();
        }



        /// <summary>
        /// end dialog process
        /// </summary>
        /// <param name="step"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> EndDialog(WaterfallStepContext step, CancellationToken cancellationToken = default(CancellationToken))
        {
            return EndOfTurn;
        }

        private static bool isValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
              (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}