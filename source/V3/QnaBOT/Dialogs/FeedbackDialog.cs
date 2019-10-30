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
using QnaBOT.Classes;
//using QnaBOT.Services;

namespace QnaBOT.Dialogs
{
    //[LuisModel("8af85957-7fa3-4a34-9a10-d963c0f7c6aa", "b1a521a868fd4d7ca41786f4803d762e", domain: "eastus.api.cognitive.microsoft.com")]

    [Serializable]
    public class FeedbackDialog : IDialog<IMessageActivity>
    {
        private string qnaURL;
        private bool suppressmsg;
        private string userQuestion;
        private string qnaAnswer;


        //public static string key = ConfigurationManager.AppSettings["TextAnalyticsKey"];
        public const AzureRegions region = AzureRegions.Eastus;
        flag fl;

        public FeedbackDialog(string url, string question, string answer)
        {
            //keep track of data associated with feedback
            qnaURL = url;
            userQuestion = question;
            qnaAnswer = answer;
        }

        public FeedbackDialog(bool supressmsessage) => this.suppressmsg = supressmsessage;

        public FeedbackDialog(string question, flag f)
        {
            userQuestion = question;
            fl = f;
        }
        private static Attachment GetHeroCard(CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                //Title = title,
                //Subtitle = subtitle,
                //Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }
        private static IList<Attachment> GetCardsAttachments()
        {
            return new List<Attachment>()
            {//
                GetHeroCard(
                    //"Our People Support",
                    //"Our People Support Services",
                    //"I can help with anything related to HR Support",
                    new CardImage(url: "https://jonazurestorage123.blob.core.windows.net/myimages/happy.png"),
                    new CardAction(ActionTypes.PostBack, "Satisfecho", value: "positive-feedback")),
                GetHeroCard(
                    //"TMD Support",
                    //"Technology Management Division Support Services",
                    //"I can offer information or services that TMD offer!",
                   new CardImage(url: "https://jonazurestorage123.blob.core.windows.net/myimages/sad.png"),
                   new CardAction(ActionTypes.PostBack, "No Satisfecho", value: "negative-feedback")),                
            };
        }
        public async Task StartAsync(IDialogContext context)
        {
            //flag f = new flag();
            var telemetry = new TelemetryClient();
            try
            {
                var feedback = ((Activity)context.Activity).CreateReply(QNABotSettings.didyoufind);//Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts
                var card = new HeroCard();
                feedback.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                feedback.Attachments = GetCardsAttachments();
                //feedback.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetFeedbackCard() };



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

            var userFeedback = await result;
            var feedbacktext = userFeedback.Text;
            string text = ((await result) as Activity).Text;
            string resultfirstCheck;

            try
            {
                context.UserData.TryGetValue("firstCheck", out resultfirstCheck);
                var feedbackResponse = await FeedbackLUISAPI.GetFeedbackIntent(text);
             
                TelemetryClient telemetry = new TelemetryClient();
               
                if (feedbackResponse?.topScoringIntent?.intent.ToUpper() == "POSITIVE")
                {
                    context.UserData.SetValue("firstCheck", "false");
                    var properties = new Dictionary<string, string>
                        {
                            {"Question", userQuestion },
                            {"URL", qnaURL },
                            {"Vote", "Yes" },
                            {"QnaAnswer",qnaAnswer }
                            // add properties relevant to your bot 
                        };
                    telemetry.TrackEvent("Yes-Vote", properties);

                    await context.PostAsync(QNABotSettings.positivemsg);
                    context.Done<IMessageActivity>(userFeedback);//goes back to QNADialog
                    fl.Flags1 = 0;
                    fl.Flags2 = 0;
                    context.Done(1);
                }

                else if (feedbackResponse?.topScoringIntent?.intent.ToUpper() == "NEGATIVE" && (fl.Flags1 == 1 && fl.Flags2 == 0))
                //else if ((feedbacktext.Equals("no") || feedbacktext.Contains("nope") || feedbacktext.Equals("negative-feedback")) && !fcheck)
                {
                    context.UserData.SetValue("firstCheck", "true");
                    var properties = new Dictionary<string, string>
                        {
                                {"Question", userQuestion },
                                {"URL", qnaURL },
                                {"Vote", "No" },
                                {"QnaAnswer",qnaAnswer }
                                // add properties relevant to your bot negative
                        };

                    telemetry.TrackEvent("No-Vote", properties);

                    var feedback = ((Activity)context.Activity).CreateReply(QNABotSettings.accurateanswer);

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
                            //System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase() //this is causing issues with Spanish language
                            herocardlst.Add(new HeroCard
                            {
                                Title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase((from b in item.metadata
                                         where b.name == CarouselSettings.carouseltitle
                                         select b.value).FirstOrDefault()),
                                Subtitle = (from b in item.metadata
                                            where b.name == CarouselSettings.carouselsummary
                                            select b.value).FirstOrDefault(),
                                Buttons = new List<CardAction>() { new CardAction(ActionTypes.PostBack, QNABotSettings.seedetails, value: item.questions?.FirstOrDefault()) },
                            });
                        }

                        feedback.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetNoneoftheAbove() };

                        feedback.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        herocardlst.ForEach(i => feedback.Attachments.Add(i.ToAttachment()));
                        await context.PostAsync(feedback);


                        context.Wait(Noneoftheabovedialog);
                    }

                }

               
                else if (feedbackResponse?.topScoringIntent?.intent.ToUpper() == "NEGATIVE" && (fl.Flags1 == 2 || fl.Flags2 == 1))
                {
                    var properties = new Dictionary<string, string>
                        {
                            {"Question", userQuestion },
                            {"URL", qnaURL },
                            {"Vote", "No" },
                            {"QnaAnswer",qnaAnswer }
                         };
                    telemetry.TrackEvent("No", properties);

                    var defaultMessage = ((Activity)context.Activity).CreateReply(QNABotSettings.scneario2message);//Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts


                    defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetContactInfoCard() };


                    await context.PostAsync(defaultMessage);
                    context.UserData.SetValue("firstCheck", "false");
                    fl.Flags1 = 0;
                    fl.Flags2 = 0;
                    context.Wait<IMessageActivity>(ChoiceMenuLogic);
                }             

                else
                {
                    await context.PostAsync(QNABotSettings.providefeedback);
                    context.UserData.SetValue("firstCheck", "true");
                    context.Call(new FeedbackDialog((context.Activity as Activity).Text, fl), ResumeAfterFeedback);
                }
            }

            catch (Exception)
            {
                throw;
                //telemetry.TrackEvent("Feedback Dialog MessageReceivedAsync Exception", new Dictionary<string, string>{{"Exception", SimpleJson.SimpleJson.SerializeObject(ex)}});
            }
        }

        private async Task Noneoftheabovedialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var msg = await result;

            if (msg.Text.Contains(QNABotSettings.noneoftheabove) /*&& defaultmessageflag == false*/)
            {
                //defaultmessageflag = true;

                var defaultMessage = ((Activity)context.Activity).CreateReply(QNABotSettings.sorrynextrelease);//Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts


                defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetContactInfoCard() };

                await context.PostAsync(defaultMessage);
                context.Wait(ChoiceMenuLogic);//with the choice selected, move to ChoiceMenuLogic to determine the type of answer
            }
            else
            {
                var userFeedback = await result;
                var feedbacktext = userFeedback.Text;
                var message = context.MakeMessage();
                message.Text = feedbacktext;
                await context.Forward(new QnADialog(fl), ResumeAfterFeedback, message, CancellationToken.None);                
            }
        }

        private async Task ChoiceMenuLogic(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var msg = await result;
            var userFeedback = await result;
            var feedbacktext = userFeedback.Text;
            if (msg.Text.Contains(QNABotSettings.howtoask))
            {
                await context.PostAsync(QNABotSettings.prompt1);
                Task.Delay(2000);
                await context.PostAsync(QNABotSettings.prompt2);
                fl.Flags1 = 0; //reset counter
                fl.Flags2 = 0; //reset counter
                context.Done(1);
            }
            else if (msg.Text.Contains(QNABotSettings.howtocontacthr))
            {
                var replyMessage = context.MakeMessage();
                Attachment attachment = CardActionDialogue.ContactHRAttachment();
                replyMessage.Attachments = new List<Attachment> { attachment };
                await context.PostAsync(replyMessage);
                fl.Flags1 = 0; //reset counter
                fl.Flags2 = 0; //reset counter
                context.Done(1);
            }
            else
            {
                var message = context.MakeMessage();
                message.Text = feedbacktext;

                fl.Flags1 = 0; //reset counter
                fl.Flags2 = 0; //reset counter      
                await context.Forward(new QnADialog(fl), ResumeAfterFeedback, message, CancellationToken.None);
            }
        }

        private async Task ResumeAfterFeedback(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(1);
            //context.Done<object>(await result);
        }
    }

    class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Ocp-Apim-Subscription-Key", "d25306cc3109458e835312560eb5d2b6");
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}

//double? sentimentscore = await TextAnalysisHelper.obtainSentiment(key, region, text);
//string keypharses = await TextAnalysisHelper.obtainKeyPhrase(key, region, text);
//sentimentscore *= 100;

//if (feedbacktext.ToLower().Equals(QnaHelper.PositiveFeedback) || feedbacktext.ToLower().Equals("good") || feedbacktext.ToLower().Equals("excellent") || feedbacktext.ToLower().Equals("yes") || feedbacktext.ToLower().Equals("ok")
//        || feedbacktext.ToLower().Equals("yep") || feedbacktext.ToLower().Equals("yup") || feedbacktext.ToLower().Equals("fantastic") || feedbacktext.ToLower().Equals("bien") || feedbacktext.ToLower().Equals("si") || feedbacktext.Equals("correcto") || feedbacktext.ToLower().Equals("bad") || feedbacktext.Equals("terrible")
//        || feedbacktext.ToLower().Equals("horrible") || feedbacktext.ToLower().Contains("no") || feedbacktext.ToLower().Equals("nope")
//        || feedbacktext.ToLower().Equals("not sure") || feedbacktext.ToLower().Equals("mal") || feedbacktext.ToLower().Equals("negative-feedback") || feedbacktext.ToLower().Equals("negative-feedback2"))

//    // create telemetry client to post to Application Insights 

//if (feedbacktext.ToLower().Equals(QnaHelper.PositiveFeedback) || feedbacktext.ToLower().Equals("good") || feedbacktext.Equals("excellent") || feedbacktext.ToLower().Equals("yes") || feedbacktext.ToLower().Equals("ok")
//    || feedbacktext.ToLower().Equals("fantastic") || feedbacktext.ToLower().Equals("yep") || feedbacktext.ToLower().Equals("yup") || feedbacktext.ToLower().Equals("bien") || feedbacktext.ToLower().Equals("si") || feedbacktext.ToLower().Equals("correcto"))//positive feedback

//else if (feedbackResponse?.topScoringIntent?.intent.ToUpper() == "NEGATIVE" && (QnADialog.threshold1flag == 0 && QnADialog.threshold2flag == 0))
//{
//    var properties = new Dictionary<string, string>
//        {
//            {"Question", userQuestion },
//            {"URL", qnaURL },
//            {"Vote", "No" },
//            {"QnaAnswer",qnaAnswer }
//         };
//    telemetry.TrackEvent("No", properties);

//    var defaultMessage = ((Activity)context.Activity).CreateReply(QnaHelper.SorryNextRelease);//Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts


//    defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetContactInfoCard() };


//    await context.PostAsync(defaultMessage);
//    context.UserData.SetValue("firstCheck", "false");
//    QnADialog.threshold1flag = 0;
//    QnADialog.threshold2flag = 0;
//    context.Wait<IMessageActivity>(ChoiceMenuLogic);
//}
//qnadialog.threshold1flag = 0;
//qnadialog.threshold2flag = 0;

//else if ((feedbacktext.Equals("no") || feedbacktext.Contains("nope") || feedbacktext.Equals("negative-feedback")) && fcheck)

//QnADialog.threshold1flag = 0; //reset counter
//QnADialog.threshold2flag = 0; //reset counter
//context.Done(1);

//if (suppressmsg == true || (bool)(await result) != true)
//{
//    context.Done(false);
//}
//else
//{
//    //HRSupport (false)           
//    await context.PostAsync("Test");
//    var Message = context.MakeMessage();

//}