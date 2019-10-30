using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QnaBOT.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public static string MyProperty { get; set; }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //var activity = await result as Activity;
            var msg = await result;
            if (msg.Text.Contains("yes"))
            {
                await context.PostAsync("ok");
            }
            else
            {
                await context.PostAsync("ohh...");
            }
            //await context.Forward(new QnADialog(), ResumeAfterDialog, msg, CancellationToken.None);
            //context.Call(new QnADialog(), ResumeAfterDialog);
        }
        #region Dialogs
        private async Task ResumeAfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            ////HRSupport (false)
            //if (_suppressmsg == true || (bool)(await result) != true)
            //{
            //    context.Done(true);//goes back to RootDialog    
            //}
            //else
            //{
            //var msg = new StringBuilder(HR.msg_NeedAnything).AppendLine().Append(HR.msg_YesOrNO);
            //await context.PostAsync("Was this what you were looking for?");
            ////await context.PostAsync(RootDialog.MyProperty);
            //var Message = context.MakeMessage();
            //await DisplayYesNoResponse(context);
            //context.Wait(EndofDialog);
            //}
        }
        private async Task EndofDialog(IDialogContext context, IAwaitable<object> result)
        {
            //var message = await result as IMessageActivity;
            //if (message.Text.ToLower().Contains("yes"))
            //{
            //    await context.PostAsync("Great! If there is anything else, just ask again!");

            //    context.Wait(ExpandDialog);

            //}
            //else if (message.Text.ToLower().Contains("no"))
            //{
            //    var msg = await result;
            //    //var youhoe = QnaHelper.QNAConnection(RootDialog.MyProperty);
            //    //await context.PostAsync(youhoe.FirstOrDefault().answers.FirstOrDefault().metadata.FirstOrDefault().name + ": " + youhoe.FirstOrDefault().answers.FirstOrDefault().metadata.FirstOrDefault().value);

            //    await context.PostAsync("Oh ok! you may ask again!");
            //    //await context.Forward(new QnADialog(), ResumeAfterDialog, msg, CancellationToken.None);
            //    context.Wait(ExpandDialog);

            //}

        }
        private async Task ExpandDialog(IDialogContext context, IAwaitable<object> result)
        {
            //var msg = await result;
            ////var youhoe = QnaHelper.QNAConnection(RootDialog.MyProperty);
            ////await context.PostAsync(youhoe.FirstOrDefault().answers.FirstOrDefault().metadata.FirstOrDefault().value + youhoe.FirstOrDefault().answers.FirstOrDefault().metadata.FirstOrDefault().value);

            //await context.Forward(new QnADialog(), ResumeAfterDialog, msg, CancellationToken.None);

        }
        #endregion

        #region HeroCards
        public static Attachment YesNoHeroCards()
        {
            var heroCard = new HeroCard
            {
                //Title = "BotFramework Hero Card",
                //Subtitle = "",
                //Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                //Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> { new CardAction (ActionTypes.ImBack,"Yes", value: "yes"),
                                                 new CardAction(ActionTypes.ImBack, "No", value: "no")}
            };
            return heroCard.ToAttachment();
        }

        public static Attachment OptionsCard()
        {
            var heroCard = new HeroCard
            {
                //Title = "BotFramework Hero Card",
                //Subtitle = "",
                //Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                //Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> { new CardAction (ActionTypes.ImBack,"Yes", value: "yes"),
                                                 new CardAction(ActionTypes.ImBack, "No", value: "no")}
            };
            return heroCard.ToAttachment();
        }
        #endregion

        #region Methods
        public async Task DisplayYesNoResponse(IDialogContext context) //
        {
            var replyMessage = context.MakeMessage();
            Attachment attachment = YesNoHeroCards();
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);

        }
        #endregion
    }
}
