using Microsoft.ApplicationInsights;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Rest;
using QnaBOT.Helpers;
using QnaBOT.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

//using QnaBOT.Services;

namespace QnaBOT.Dialogs
{
    //[LuisModel("8af85957-7fa3-4a34-9a10-d963c0f7c6aa", "b1a521a868fd4d7ca41786f4803d762e", domain: "eastus.api.cognitive.microsoft.com")]

    [Serializable]
    public class FeedbackCheck : IDialog<IMessageActivity>
    {
        private string qnaURL;
        private bool suppressmsg;
        private string userQuestion;
        private string qnaAnswer;
        public static bool newinstance = false;
        private static bool secondflag = false;
        public static string CarouselTitle = ConfigurationManager.AppSettings["CarouselTitle"];
        public static string CarouselSummary = ConfigurationManager.AppSettings["CarouselSummary"];
        public static string key = ConfigurationManager.AppSettings["TextAnalyticsKey"];
        public const AzureRegions region = AzureRegions.Eastus;

        public FeedbackCheck(string question) => userQuestion = question;
    
       

        public async Task StartAsync(IDialogContext context)
        {

            var telemetry = new TelemetryClient();
            try
            {
                var feedback = ((Activity)context.Activity).CreateReply(QnaHelper.DidYouFind  + "2");//Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts

                feedback.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetFeedbackCard() };

                await context.PostAsync(feedback);
                context.Wait(this.MessageReceivedAsync);
            }
            catch (Exception)
            {
                throw;
                //telemetry.TrackEvent("Feedback Dialog StartAsync Exception", new Dictionary<string, string> { { "Exception", SimpleJson.SimpleJson.SerializeObject(ex) } });
            }
        }


        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var userFeedback = await result;
                var feedbacktext = userFeedback.Text;
                string text = ((await result) as Activity).Text;
                 bool fcheck = false;
                context.UserData.TryGetValue("firstCheck", out fcheck);

                var feedbackResponse = await FeedbackLUISAPI.GetFeedbackIntent(text);

              
                TelemetryClient telemetry = new TelemetryClient();

                context.UserData.SetValue("firstCheck", false);
                 if (feedbackResponse?.topScoringIntent?.intent.ToUpper() == "POSITIVE")
                {
                    var properties = new Dictionary<string, string>
                        {
                            {"Question", userQuestion },
                            {"URL", qnaURL },
                            {"Vote", "Yes" },
                            {"QnaAnswer",qnaAnswer }
                            // add properties relevant to your bot 
                        };
                    telemetry.TrackEvent("Yes-Vote", properties);

                    await context.PostAsync("Great! Is there any other question, concern or doubt you would like to know about?");
                    context.Done<IMessageActivity>(userFeedback);//goes back to QNADialog
                }

                else if (feedbackResponse?.topScoringIntent?.intent.ToUpper() == "NEGATIVE" && !fcheck)
                //else if ((feedbacktext.Equals("no") || feedbacktext.Contains("nope") || feedbacktext.Equals("negative-feedback")) && !fcheck)
                {
                    context.UserData.SetValue("firstCheck", true);
                    var properties = new Dictionary<string, string>
                        {
                                {"Question", userQuestion },
                                {"URL", qnaURL },
                                {"Vote", "No" },
                                {"QnaAnswer",qnaAnswer }
                                // add properties relevant to your bot negative
                        };

                    telemetry.TrackEvent("No-Vote", properties);

                    var feedback = ((Activity)context.Activity).CreateReply(QnaHelper.SorryNextRelease);

                    var results = new QnaEntity();
                    var qnamakerlist = context.UserData.TryGetValue("questions", out results);

                    //Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts
                    var herocardlst = new List<HeroCard>();
                    if (results != null && results.answers != null && results.answers.Any())
                    {
                        var firstItem = results.answers[0];

                        results.answers.RemoveAt(0);

                        results.answers.Insert(results.answers.Count(), firstItem);
                        foreach (var item in results.answers.Where(i => i.metadata != null))// && i.metadata.Any()).OrderBy(i=> i.score))
                        {
                            herocardlst.Add(new HeroCard
                            {
                                Title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase((from b in item.metadata
                                                                                                              where b.name == CarouselTitle
                                                                                                              select b.value).FirstOrDefault()),
                                Subtitle = (from b in item.metadata
                                            where b.name == CarouselSummary
                                            select b.value).FirstOrDefault(),
                                Buttons = new List<CardAction>() { new CardAction(ActionTypes.PostBack, QnaHelper.SeeDetails, value: item.questions?.FirstOrDefault()) },
                            });
                        }
                        feedback.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetContactInfoCard() };
                    }

                    feedback.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    herocardlst.ForEach(i => feedback.Attachments.Add(i.ToAttachment()));
                    await context.PostAsync(feedback);
                    context.Done<IMessageActivity>(userFeedback);//goes back to QNADialog
                }

                //else if ((feedbacktext.Equals("no") || feedbacktext.Contains("nope") || feedbacktext.Equals("negative-feedback")) && fcheck)
                else if (feedbackResponse?.topScoringIntent?.intent.ToUpper() == "NEGATIVE" && fcheck)
                {
                    var properties = new Dictionary<string, string>
                        {
                            {"Question", userQuestion },
                            {"URL", qnaURL },
                            {"Vote", "No" },
                            {"QnaAnswer",qnaAnswer }
                         };
                    telemetry.TrackEvent("No", properties);

                    var defaultMessage = ((Activity)context.Activity).CreateReply(QnaHelper.SorryNextRelease);//Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts


                    defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetContactInfoCard() };


                    await context.PostAsync(defaultMessage);
                    context.UserData.SetValue("firstCheck", false);
                    context.Wait(ChoiceMenuLogic);

                }

                else
                {
                    await context.PostAsync(QnaHelper.ProvideFeedback  + "2");
                    context.Call(new FeedbackDialog((context.Activity as Activity).Text), ResumeAfterFeedback);
                }                
            }

            catch (Exception)
            {
                throw;
                //telemetry.TrackEvent("Feedback Dialog MessageReceivedAsync Exception", new Dictionary<string, string>{{"Exception", SimpleJson.SimpleJson.SerializeObject(ex)}});
            }
        }


        private async Task ChoiceMenuLogic(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var msg = await result;

            if (msg.Text.Contains(QnaHelper.HowtoAsk))
            {
                await context.PostAsync(QnaHelper.Prompt1);

                Task.Delay(2000);
                await context.PostAsync(QnaHelper.Prompt2);
            }
            else if (msg.Text.Contains(QnaHelper.HowtoContactHR))
            {
                var replyMessage = context.MakeMessage();
                Attachment attachment = CardActionDialogue.ContactHRAttachment();
                replyMessage.Attachments = new List<Attachment> { attachment };
                await context.PostAsync(replyMessage);
            }
            else
            {
                var userFeedback = await result;
                var feedbacktext = userFeedback.Text;
                var message = context.MakeMessage();
                message.Text = feedbacktext;
                await context.Forward(new QnADialog(), ResumeAfterFeedback, message, CancellationToken.None);
            }
            context.Done(1);
        }



        private async Task ResumeAfterFeedback(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(await result);
        }

    }  
}