using BotApp.Classes;
using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace BotApp.dialogs
{
  public class CardActionDialog
  {
    #region Card Action Methods
    /// <summary>
    /// Display Contact Us Card 
    /// </summary>
    /// <returns></returns>
    public static Attachment ContactHRAttachment()
    {
      var heroCard = new HeroCard
      {
        Title = QNABotSettings.contactourpeople,
        Subtitle = QNABotSettings.youmaycontact,
        Text = QNABotSettings.phonenumber,
        Images = new List<CardImage> { new CardImage(QNABotSettings.contacthrimage) },
        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, QNABotSettings.intranetportal, value: QNABotSettings.intranetportallink) }
      };

      return heroCard.ToAttachment();
    }
    /// <summary>
    /// Display card of information 
    /// </summary>
    /// <returns></returns>
    public static List<CardAction> GetContactInfoCard() =>

         new List<CardAction>()
                             {
                                new CardAction() { Title = QNABotSettings.learnhowtoask, Type = ActionTypes.PostBack, Value = QNABotSettings.howtoask, DisplayText = QNABotSettings.learnhowtoask },
                                new CardAction() { Title = QNABotSettings.contactourpeople, Type = ActionTypes.PostBack, Value = QNABotSettings.howtocontacthr,  DisplayText =  QNABotSettings.contactourpeople }
 };
    /// <summary>
    /// display card of none of above
    /// </summary>
    /// <returns></returns>
    internal static IList<CardAction> GetNoneoftheAbove() =>
        new List<CardAction>()
                            {
                                    new CardAction(){ Title = QNABotSettings.noneoftheabove, Type=ActionTypes.PostBack, Value=  QNABotSettings.noneoftheabove, DisplayText = QNABotSettings.noneoftheabove },
                            };
    #endregion
  }
}
