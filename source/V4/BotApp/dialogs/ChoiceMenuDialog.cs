using BotApp.Classes;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotApp.dialogs
{
  public class ChoiceMenuDialog
  {
    public static async Task ChoiceMenu(string msg, WaterfallStepContext step)
    {
      var context = step.Context;
      if (msg.Contains(QNABotSettings.howtoask))
      {
        await context.SendCustomResponseAsync(QNABotSettings.howtoaskmsg1);
        await context.SendCustomResponseAsync(QNABotSettings.howtoaskmsg2);
      }
      else if (msg.Contains(QNABotSettings.howtocontacthr))
      {
        var replyMessage = context.Activity.CreateReply();
        replyMessage.Attachments = new List<Attachment> { CardActionDialog.ContactHRAttachment() };
        await context.SendActivityAsync(replyMessage);
      }
      else
      {
        var feedbacktext = msg;
        var message = context.Activity.CreateReply();
        message.Text = feedbacktext;
        await context.SendActivityAsync(message);
      }
    }

  }
}
