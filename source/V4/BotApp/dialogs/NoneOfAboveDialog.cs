using BotApp.Classes;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;

namespace BotApp.dialogs
{
    public class NoneOfAboveDialog
    {
        public static async Task NoneofTheAbove(WaterfallStepContext step)
        {
            var context = step.Context;
            var defaultMessage = context.Activity.CreateReply(QNABotSettings.sorrynextrelease);
            defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialog.GetContactInfoCard() };

            await context.SendActivityAsync(defaultMessage);
        }
    }
}