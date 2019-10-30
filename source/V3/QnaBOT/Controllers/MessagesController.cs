using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Services.Description;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
//using QnaBOT.Services;

namespace QnaBOT
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
          //string targetLang = "en";
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //var detect = "en";
            if (activity.Type == ActivityTypes.Message)
            {
                /////Typing Activity///                 
                //var connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));
                //Activity isTypingReply = activity.CreateReply();
                //isTypingReply.Type = ActivityTypes.Typing;
                //await connector.Conversations.ReplyToActivityAsync(isTypingReply);

                // Send "typing" information
                Activity reply = activity.CreateReply();
                string msg = activity.Text.ToLower().Trim();
                reply.Type = ActivityTypes.Typing;
                reply.Text = null;
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.ReplyToActivityAsync(reply);

                await Conversation.SendAsync(activity, () => new Dialogs.QnADialog());

                //if (msg == "start over" || msg == "exit" || msg == "quit" || msg == "done" || msg == "start again" || msg == "restart" || msg == "leave" || msg == "reset")
                //{
                //    //This is where the conversation gets reset!
                //    activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                //}
                //else
                //{
                //}
                //var input = activity.Text;
                //Task.Run(async () =>
                //{
                //    var accessToken = await TranslatorToken.GetAuthenticationToken();
                //    var translatorClient = new TranslatorAPIClient(accessToken);
                //    var output = await translatorClient.TranslateText(input, targetLang);
                //    detect = await translatorClient.Detect(input);
                //    activity.Text = output;
                //    //translatorClient.SetUserLanguageCode(activity, detect);
                //    //call to RootDialog
                //    await Conversation.SendAsync(activity, () => new Dialogs.QnADialog());
                //}).Wait();
            }
            else
            {
                HandleSystemMessageAsync(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

   

        private  Activity HandleSystemMessageAsync(Activity message)//causes issues in Azure Portal Bot
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {


                //Handle conversation state changes, like members being added and removed
                //Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                //Not available in all channels
                IConversationUpdateActivity iConversationUpdated = message as IConversationUpdateActivity;
                if (iConversationUpdated != null)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));

                    foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        // if the bot is added, then
                        if (member.Id == iConversationUpdated.Recipient.Id)
                        {
                            var reply = ((Activity)iConversationUpdated).CreateReply(
                                $"Saludos, soy el chatbot de Nuestra Gente. Puedo ayudarle contestando algunas de las preguntas mas frecuentes para Nuestra Gente. Por favor ten en cuenta que tan solo puedo contestar preguntas generales. Para saber de los temas que conozco, puede escribir *ayuda*." +
                                $"\n\n\n**AVISO:** Aun me encuentro en mi periodo de prueba, por lo cual mi conocimiento no esta completo y puedes experimentar dificultades al hablar conmigo. Por favor reporta cualquier incidente en el formulario que se le proveyó.");/*$"Hi! I am {member.Name} and I can help you answer any questions or concerns you may have! **Disclaimer!** Please be aware I can only help answer questions only. You may type *help* if you don't know " +
                                $"what you can do.*/
                            connector.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }

              
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
               
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {

            }
            else if (message.Type == ActivityTypes.Event)
            {
                // Send TokenResponse Events along to the Dialog stack
                if (message.IsTokenResponseEvent())
                {
                    Conversation.SendAsync(message, () => new Dialogs.QnADialog());
                }
            }
            else if (message.Type == ActivityTypes.Invoke)
            {
                // Send teams Invoke along to the Dialog stack
                if (message.IsTeamsVerificationInvoke())
                {
                    Conversation.SendAsync(message, () => new Dialogs.QnADialog());
                }
            }
            return null;
        }


    }
}


//if (message.MembersAdded.Any(o => o.Id == message.Recipient.Id))
//{
//    //// Activity halreply = message.CreateReply("Hello, I am HAL.");
//    // connectors.Conversations.ReplyToActivityAsync(halreply);

//    //carousel
//    ConnectorClient connectors = new ConnectorClient(new Uri(message.ServiceUrl));
//    var reply = message.CreateReply();
//    //var card = new HeroCard();
//    reply.Text ="This is Our People Bot Assisstant, Beta version [Full disclaimer message here]";


//    //reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;                    
//    //context.Call(CreateGetTokenDialog(), ListMe);

//    await connectors.Conversations.ReplyToActivityAsync(reply);
//}