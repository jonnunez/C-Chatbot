using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BotApp
{
  public class MainDialog : ComponentDialog
  {
    public const string dialogId = "MainDialog";
    private BotAccessors accessors = null;

    /// <summary>
    /// Access point to start bot
    /// </summary>
    /// <param name="accessors"></param>
    public MainDialog(BotAccessors accessors) : base(dialogId)
    {
      this.accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

      AddDialog(new WaterfallDialog(dialogId, new WaterfallStep[]
      {
                StartDialog
      }));
      // added dialogs
      AddDialog(new LuisQnADialog(accessors));
      AddDialog(new FeedbackDialog(accessors));
    }

    private async Task<DialogTurnResult> StartDialog(WaterfallStepContext step, CancellationToken cancellationToken = default(CancellationToken))
    {
     
      await step.EndDialogAsync(step.ActiveDialog.State);

      await step.BeginDialogAsync(LuisQnADialog.dialogId, null, cancellationToken);

      return Dialog.EndOfTurn;
    }
  }
}
