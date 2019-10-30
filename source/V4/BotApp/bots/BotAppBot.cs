// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using BotApp.Classes;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotApp
{
  public class BotAppBot : ActivityHandler
  {
    #region Variables
    protected readonly Dialog _dialog;
    protected readonly BotState _conversationState;
    protected readonly BotState _userState;
    protected readonly ILogger _logger;
    protected BotAccessors _accessors = null;
    protected DialogSet dialogs = null;
    #endregion

    #region Constructor
    public BotAppBot(BotAccessors accessors, ConversationState conversationState, UserState userState, ILogger<BotAppBot> logger) 
    {
      _accessors = accessors;
      _userState = userState;
      _conversationState = conversationState;
      _logger = logger;

      this.dialogs = new DialogSet(accessors.ConversationDialogState);
      this.dialogs.Add(new MainDialog(accessors));
      this.dialogs.Add(new LuisQnADialog(accessors));
      this.dialogs.Add(new FeedbackDialog(accessors));
    }
    #endregion

    #region Methods
    private async Task LaunchWelcomeAsync(ITurnContext turnContext)
    {
      var message = string.Empty;

      message = QNABotSettings.welcomemessage;
      await turnContext.SendCustomResponseAsync(message);
    }

    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
      if (turnContext == null)
      {
        throw new ArgumentNullException(nameof(turnContext));
      }

      var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken: cancellationToken);

      switch (turnContext.Activity.Type)
      {
        case ActivityTypes.ConversationUpdate:
          if (turnContext.Activity.MembersAdded.FirstOrDefault()?.Id == turnContext.Activity.Recipient.Id)
          {
            await LaunchWelcomeAsync(turnContext);
            await dialogContext.BeginDialogAsync(MainDialog.dialogId, null, cancellationToken: cancellationToken);
          }
          break;

        case ActivityTypes.Message:
          var results = await dialogContext.ContinueDialogAsync(cancellationToken);

          var text = turnContext.Activity.Text;
          if (text == "/start")
          {
            await _accessors.ConversationDialogState.DeleteAsync(turnContext);
            await _accessors.IsAuthenticatedPreference.DeleteAsync(turnContext);
            await dialogContext.EndDialogAsync();
            await dialogContext.BeginDialogAsync(MainDialog.dialogId, null, cancellationToken);
          }
          else if (text == "negative-feedback" || text == "positive-feedback")
          {
            await dialogContext.BeginDialogAsync(FeedbackDialog.dialogId, null, cancellationToken);
          }
          else
          {
            if (!dialogContext.Context.Responded)
            {
              bool isAuthenticated = await _accessors.IsAuthenticatedPreference.GetAsync(turnContext, () => { return false; });
              await dialogContext.BeginDialogAsync(LuisQnADialog.dialogId, null, cancellationToken);
            }
          }
          break;
      }
      await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
      await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
    }
    #endregion
  }
}
