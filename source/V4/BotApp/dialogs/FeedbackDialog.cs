using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotApp.Classes;
using BotApp.dialogs;
using BotApp.helpers;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace BotApp
{
  public class FeedbackDialog : ComponentDialog
  {
    #region Variables
    private BotState _userState;
    public const string dialogId = "FeedbackDialog";
    private BotAccessors accessors = null;
    #endregion
    public FeedbackDialog(BotAccessors accessors) : base(dialogId)
    {
      this.accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
      _userState = accessors.UserState;

      AddDialog(new WaterfallDialog(dialogId, new WaterfallStep[]
      {
                StartAsync,
                MessageReceivedAsync
      }));

    }

    #region Methods
   
    /// <summary>
    /// display feedback dialog card
    /// </summary>
    /// <param name="step"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DialogTurnResult> StartAsync(WaterfallStepContext step, CancellationToken cancellationToken = default(CancellationToken))
    {

      var telemetry = new TelemetryClient();
      try
      {
        if (!step.Context.Activity.Text.Contains(QNABotSettings.positivefeedback) && !step.Context.Activity.Text.Contains(QNABotSettings.negativefeedback))
        {
          var feedback = step.Context.Activity.CreateReply(QNABotSettings.didyoufind);
          var card = new HeroCard();
          feedback.AttachmentLayout = AttachmentLayoutTypes.Carousel;
          feedback.Attachments = GetCardsAttachments();
          await step.Context.SendActivityAsync(feedback);
        }
        return await step.NextAsync();
      }
      catch
      {
        throw;
      }
    }

    /// <summary>
    /// receive and handle input on click of feedback dialog
    /// </summary>
    /// <param name="step"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<DialogTurnResult> MessageReceivedAsync(WaterfallStepContext step, CancellationToken cancellationToken = default(CancellationToken))
    {
      var question = (string)step.Result;
      step.ActiveDialog.State["question"] = question;
      string searchedText = step.Context.Activity.Text;

      // for storing data into telemetry.
      string lastSearchedText = await accessors.LastSearchPreference.GetAsync(step.Context, () => { return string.Empty; });
      string lastAnswerText = await accessors.LastAnswerPreference.GetAsync(step.Context, () => { return string.Empty; });
      int negative_flag = 0;

      if (searchedText.Contains(QNABotSettings.positivefeedback) || searchedText.Contains(QNABotSettings.negativefeedback))
      {
        try
        {
          if (searchedText.Contains(QNABotSettings.positivefeedback))
          {
            TrackEvents.TrackEvent(lastSearchedText, lastAnswerText,"Yes");
            await step.Context.SendActivityAsync(QNABotSettings.positivemsg);
          }

          else if (searchedText.Contains(QNABotSettings.negativefeedback) && lastSearchedText != "")
          {
            TrackEvents.TrackEvent(lastSearchedText, lastAnswerText, "No");

            var feedback = step.Context.Activity.CreateReply(QNABotSettings.accurateanswer);
            step.Context.Activity.Text = lastSearchedText;
            var recognizerNegResult = await accessors.LuisServices[Settings.LuisName01].RecognizeAsync(step.Context, cancellationToken);
                        negative_flag=1;
            await CarouselDialog.CarouselAfterNegativeFeedback(feedback, recognizerNegResult, accessors, step, negative_flag, cancellationToken); // Carousel Dialog
            await _userState.DeleteAsync(step.Context, cancellationToken);
                        negative_flag = 0;
          }
          else
          {
            TrackEvents.TrackEvent(lastSearchedText, lastAnswerText, "No");
            await NoneOfAboveDialog.NoneofTheAbove(step);//No Answer Dialog
          }
        }

        catch
        {
          throw;
        }
      }
      return await step.EndDialogAsync();
    }

    #endregion
    #region Private Methods
    private static Attachment GetHeroCard(CardImage cardImage, CardAction cardAction)
    {
      var heroCard = new HeroCard
      {
        Images = new List<CardImage>() { cardImage },
        Buttons = new List<CardAction>() { cardAction },
      };

      return heroCard.ToAttachment();
    }

    /// <summary>
    /// get hero card details like email, title
    /// </summary>
    /// <returns></returns>
    private static IList<Attachment> GetCardsAttachments()
    {
      return new List<Attachment>()
            {
                GetHeroCard(
                    new CardImage(url: QNABotSettings.happyimage ),
                    new CardAction(ActionTypes.PostBack, QNABotSettings.satisfaction, value: QNABotSettings.positivefeedback)),
                GetHeroCard(
                   new CardImage(url: QNABotSettings.sadimage),
                   new CardAction(ActionTypes.PostBack, QNABotSettings.desatisfaction, value: QNABotSettings.negativefeedback)),
            };
    }

    #endregion
  }
}
