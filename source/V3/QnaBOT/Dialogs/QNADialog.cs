/*THIS IS AN EXPERIMENTAL VERSION!*/
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using QnaBOT.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QnaBOT.Classes;

namespace QnaBOT.Dialogs
{
    [Serializable]    
    public class QnADialog : QnAMakerDialog
    {       
        public bool showHRHeroCardFlag = true;
        public bool defaultmessageflag = false;
        

        //private static bool  suppressmsg = false;
        //public flag f;

        public flag f { get; set; }

        public QnADialog() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnAMakerAuthKey"], ConfigurationManager.AppSettings["QnAMakerKBId"], ConfigurationManager.AppSettings["QnAMakerDefaultMessage"], 0, 1, ConfigurationManager.AppSettings["QnAMakerEndPoint"])))
        {
            f = new flag();
        }        
        //public QnADialog(bool supressmsessage) => suppressmsg = supressmsessage;

        public QnADialog(flag fl) : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnAMakerAuthKey"], ConfigurationManager.AppSettings["QnAMakerKBId"], ConfigurationManager.AppSettings["QnAMakerDefaultMessage"], 0, 1, ConfigurationManager.AppSettings["QnAMakerEndPoint"])))
        {           
            if (this.f == null)
            {
                this.f = fl;
            }
            else
            {
                f = fl;
            }
        } 


        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var telemetry = new TelemetryClient();

            try
            {

                var qnaAnswer = result?.Answers?.First()?.Answer;//store qnamaker result text and store it in qnaAnswer variable
                var qnaQuestions = result?.Answers?.First()?.Questions;
                var questions = new QnaEntity();
                var userinput = new QnaHelper();
                var myquestion = result;
                Activity reply = ((Activity)context.Activity).CreateReply();

                if (qnaAnswer != null & qnaQuestions != null && qnaQuestions.Any())
                {
                    if (message.Text.ToString().ToLower().Contains("help") || message.Text.ToString().ToLower().Contains("ayuda"))
                    {
                        await context.PostAsync(QNABotSettings.prompt1);
                        Task.Delay(2000);
                        await context.PostAsync(QNABotSettings.prompt2);
                        return;
                    }

                    if (qnaQuestions.Any())
                    {
                        questions = QnaHelper.QNAConnection(qnaQuestions?.FirstOrDefault());
                        context.UserData.SetValue("questions", questions);
                    }
                    else
                        context.UserData.RemoveValue("questions");

                    var score = result.Answers.OrderByDescending(i => i.Score).FirstOrDefault()?.Score;

                    if (score != null)
                    {
                        score = score * 100;

                        //Threshold 1 Path! 100% - 80%
                        if (score <= (Convert.ToInt32(QNABotSettings.threshold1[0]) + 0.99) && score >= Convert.ToInt32(QNABotSettings.threshold1[1]))
                        {
                            f.Flags1++;
                            if (qnaAnswer.Contains("||"))
                            {
                                var msg = context.MakeMessage();
                                var answers = qnaAnswer.Split("||".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                                msg.Attachments.Add(await GetCardsAttachments(answers));
                                await context.PostAsync(msg);
                                if (qnaAnswer.Contains(ConfigurationManager.AppSettings["HRHeroTitle"]))
                                    showHRHeroCardFlag = false;
                                else
                                    showHRHeroCardFlag = true;
                            }
                            else
                            {
                                await context.PostAsync(qnaAnswer);
                            }

                        }

                        //Threshold 2 Path!79 % -40 %
                        else if (score <= (Convert.ToInt32(QNABotSettings.threshold2[0]) + 0.99) && score >= Convert.ToInt32(QNABotSettings.threshold2[1]))//79-40
                        {
                            f.Flags2++;
                            context.UserData.SetValue("firstCheck", "true");
                            showHRHeroCardFlag = true;
                            var msg = ((Activity)context.Activity).CreateReply(QNABotSettings.accurateanswer); //for carousel
                            var herocardlst = new List<HeroCard>();
                            if (questions != null && questions.answers != null && questions.answers.Any())
                            {

                                foreach (var item in questions.answers.Where(i => i.metadata != null))
                                {
                                    //string TitleCase = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase();
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
                            }
                            msg.SuggestedActions = new SuggestedActions
                            {
                                Actions = CardActionDialogue.GetNoneoftheAbove()
                            };

                            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            herocardlst.ForEach(i => msg.Attachments.Add(i.ToAttachment()));
                            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            await context.PostAsync(msg);
                            context.Wait(Noneoftheabovedialog);
                        }

                        //Threshold 3 Path!39 % -0 %
                        else if (score < (Convert.ToInt32(QNABotSettings.threshold3[0]) + 0.99) && score >= Convert.ToInt32(QNABotSettings.threshold3[1]))//39-0
                        {
                            var defaultMessage = ((Activity)context.Activity).CreateReply(QNABotSettings.sorrynextrelease);//Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts

                            defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetContactInfoCard() };

                            await context.PostAsync(defaultMessage);
                            context.Wait(ChoiceMenuLogic);
                        }
                    }
                }
                else
                {
                    var defaultMessage = ((Activity)context.Activity).CreateReply(QNABotSettings.sorrynextrelease);//Created a new Activity object which will prompt the user with a question – “Did you find what you need?” when the dialog starts

                    defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialogue.GetContactInfoCard() };
                    context.UserData.SetValue("firstCheck", "false");
                    await context.PostAsync(defaultMessage);
                    context.Wait(ChoiceMenuLogic);
                }
            }
            catch (Exception)
            {

                throw;
                //telemetry.TrackEvent("QnA Dialog RespondFromQnAMakerResultAsync Exception", new Dictionary<string, string> { { "Exception", SimpleJson.SimpleJson.SerializeObject(ex) } });
            }

        }

        /// <summary>
        /// This method reads the passed value of result and determines which option the user selected and then shows them an answer
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ChoiceMenuLogic(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var msg = await result;

            if (msg.Text.Contains(QNABotSettings.howtoask))
            {
                await context.PostAsync(QNABotSettings.prompt1);

                Task.Delay(2000);
                await context.PostAsync(QNABotSettings.prompt2);
                f.Flags1 = 0; //reset counter
                f.Flags2 = 0; //reset counter
                context.Done(1);
            }
            else if (msg.Text.Contains(QNABotSettings.howtocontacthr))
            {
                var replyMessage = context.MakeMessage();
                replyMessage.Attachments = new List<Attachment> { CardActionDialogue.ContactHRAttachment() };
                await context.PostAsync(replyMessage);
                f.Flags1 = 0; //reset counter
                f.Flags2 = 0; //reset counter
                context.Done(1);
            }
            else
            {
                var userFeedback = await result;
                var feedbacktext = userFeedback.Text;
                var message = context.MakeMessage();
                message.Text = feedbacktext;
                await context.Forward(new QnADialog(f), ResumeAfterFeedback, message, CancellationToken.None);
            }
        }

        /// <summary>
        /// This will disaplay a suggested action button menu with 2 options 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
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
                await context.Forward(new QnADialog(f), ResumeAfterFeedback, message, CancellationToken.None);
                //context.Done(1);//OJO
            }
        }

        private async Task<Attachment> GetCardsAttachments(List<string> answers)
        {
            var attachlst = new Attachment();

            if (answers.Any())

                attachlst = await CardActionDialogue.GetHeroCard(
                   (answers[0] == null ? string.Empty : answers[0]),
                   (answers[2] == null ? string.Empty : answers[2]),
                   (answers[3] == null ? string.Empty : answers[3]),
               (answers[4] == null ? string.Empty : answers[4]));

            return attachlst;
        }



        /// <summary>
        /// This method will ALWAYS run after QNA returns and answer with above methods. It's overriden.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var telemetry = new TelemetryClient();

            try
            {
                if (!result.Answers.Any())
                    context.UserData.RemoveValue("questions");
                var score = result.Answers.OrderByDescending(i => i.Score).FirstOrDefault()?.Score;

                if (score != null && showHRHeroCardFlag)
                {
                    score = score * 100;
                    if (score <= Convert.ToInt32(QNABotSettings.threshold1[0]) && score >= Convert.ToInt32(QNABotSettings.threshold1[1]))
                        context.Call(new FeedbackDialog((context.Activity as Activity).Text, f), ResumeAfterFeedback);
                }
            }
            catch (Exception)
            {
                throw;
                //telemetry.TrackEvent("QnA Dialog DefaultWaitNextMessageAsync Exception", new Dictionary<string, string> { { "Exception", SimpleJson.SimpleJson.SerializeObject(ex) } });
            }
        }

        private async Task ResumeAfterFeedback(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                //if (!suppressmsg)
                //    context.< IMessageActivity > (await result);
                //else
                //context.Done<IMessageActivity>(null);
                context.Done<object>(1);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task StartAsync(IDialogContext context)
        {
            throw new NotImplementedException();
        }
    }

    public static class CardActionDialogue
    {
        public static async Task<Attachment> GetHeroCard(string title, string text, string pic, string button)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Text = text,
                Images = new List<CardImage>() { new CardImage() { Url = pic } },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, QNABotSettings.learnmore, value: button) }
            };
            return heroCard.ToAttachment();
        }

        public static Attachment ContactHRAttachment()
        {
            var heroCard = new HeroCard
            {
                Title = QNABotSettings.contactourpeople,
                Subtitle = QNABotSettings.youmaycontact,
                Text = QNABotSettings.phonenumber,
                Images = new List<CardImage> { new CardImage("https://jonazurestorage123.blob.core.windows.net/myimages/OurPeopleDoneS.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, QNABotSettings.intranetportal, value: QNABotSettings.intranetportallink) }
            };

            return heroCard.ToAttachment();
        }

        public static List<CardAction> GetContactHRCard() =>

            new List<CardAction>()
                                    {
                                        new CardAction(){ Title = QNABotSettings.noneoftheabove, Type=ActionTypes.PostBack, Value= QNABotSettings.howtocontacthr, DisplayText = QNABotSettings.noneoftheabove },
                                    };

        public static List<CardAction> GetFeedbackCard() =>
        new List<CardAction>()
                          {
                            new CardAction() { Title = QNABotSettings.thumbsup, Type = ActionTypes.PostBack, Value = QNABotSettings.positivefeedback, DisplayText = QNABotSettings.thumbsup },
                            new CardAction() { Title = QNABotSettings.thumbsdown, Type = ActionTypes.PostBack, Value =  QNABotSettings.negativefeedback, DisplayText = QNABotSettings.thumbsdown }
      };

        public static List<CardAction> GetContactInfoCard() =>

             new List<CardAction>()
                                 {
                                new CardAction() { Title = QNABotSettings.learnhowtoask, Type = ActionTypes.PostBack, Value = QNABotSettings.howtoask, DisplayText = QNABotSettings.learnhowtoask },
                                new CardAction() { Title = QNABotSettings.contactourpeople, Type = ActionTypes.PostBack, Value = QNABotSettings.howtocontacthr,  DisplayText =  QNABotSettings.contactourpeople }
     };

        internal static IList<CardAction> GetNoneoftheAbove()=>
            new List<CardAction>()
                                {
                                    new CardAction(){ Title = QNABotSettings.noneoftheabove, Type=ActionTypes.PostBack, Value=  QNABotSettings.noneoftheabove, DisplayText = QNABotSettings.noneoftheabove },
                                };
    }
}

//Below method uses the V2 APIs : https://aka.ms/qnamaker-v2-apis. 
// To use V4 stack, you also need to add the Endpoint hostname to the parameters below : https://aka.ms/qnamaker-v4-apis
//[QnAMaker("182ffd0f-dfb4-4e2d-86e4-eed9662b8a70", "2d31530d-f1dc-4c9e-97dc-fe93a616124f", "https://qnaserv.azurewebsites.net/qnamaker")]