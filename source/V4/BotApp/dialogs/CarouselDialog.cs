using BotApp.Classes;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;

namespace BotApp.dialogs
{
    public class CarouselDialog
    {
        #region Variables
        private static readonly double MedConfidenceThresholdValue1 = Convert.ToInt32(Settings.MediumConfidenceThreshold[0]) + 0.99;
        private static readonly double MedConfidenceThresholdValue2 = Convert.ToInt32(Settings.MediumConfidenceThreshold[1]);

        private static readonly int Counter = Convert.ToInt32(Settings.TotalBestNextAnswers);
        #endregion

        #region Methods
        public static async Task Carousel(Activity msg, RecognizerResult recognizerResult, BotAccessors accessors, WaterfallStepContext step, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<HeroCard> herocardlst = new List<HeroCard>();
            var questions = new QnaEntity();
            questions.answers = new List<Answer>();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            int TotalNextBestAnswers = Convert.ToInt32(Settings.TotalBestNextAnswers);
            await accessors.UserState.SaveChangesAsync(step.Context, true, cancellationToken);
            int counter = 0;
            var intents = recognizerResult.Intents.OrderByDescending(i => i.Value.Score);

            foreach (var item in intents)
            {
                var luisscore = item.Value.Score * 100;
                step.Context.Activity.Text = item.Key;

                if (/*(luisscore <= MedConfidenceThresholdValue1 && luisscore >= MedConfidenceThresholdValue2)*/counter <= Counter)
                {
                    var qnaResponse = await accessors.QnAServices[Settings.QnAName01].GetAnswersAsync(step.Context);
                    counter++;
                    if (qnaResponse != null && qnaResponse.Length > 0)
                    {
                        Answer answer = new Answer
                        {
                            answer = qnaResponse[0].Answer,
                            metadata = qnaResponse[0].Metadata.ToList(),
                            questions = qnaResponse[0].Questions.ToList() //added this so that I can map the question to the variable
                        };
                        questions.answers.Add(answer);
                    }
                }
            }

            if (questions != null && questions.answers != null && questions.answers.Any() && questions.answers[0].answer != "No good match found in KB.")
            {
                foreach (var item in questions.answers.Where(i => i.metadata != null && i.metadata.Count > 0 && i.answer != "No good match found in KB."))
                {
                    var title = (from b in item.metadata where b.Name == CarouselSettings.carouseltitle select b.Value).FirstOrDefault();
                    var question = item.questions?.FirstOrDefault();
                    CardButton cButton = new CardButton();
                    cButton.value = question;// question;
                    herocardlst.Add(new HeroCard
                    {
                        Title = textInfo.ToTitleCase(title),
                        Subtitle = FirstCharToUpper((from b in item.metadata
                                                     where b.Name == CarouselSettings.carouselsummary
                                                     select b.Value).FirstOrDefault()),

                        Buttons = new List<CardAction>() { new CardAction(ActionTypes.PostBack, QNABotSettings.seedetails, value: JsonConvert.SerializeObject(cButton)) }, //qnaResponse.questions[0] JsonConvert.SerializeObject(cButton) changed instead of title, takes 
                    });
                }

                msg.SuggestedActions = new SuggestedActions
                {
                    Actions = CardActionDialog.GetNoneoftheAbove()
                };
                msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                herocardlst.ForEach(i => msg.Attachments.Add(i.ToAttachment()));
                await step.Context.SendActivityAsync(msg);
            }
            else
            {
                var defaultMessage = step.Context.Activity.CreateReply(QNABotSettings.sorrynextrelease);
                defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialog.GetContactInfoCard() };
                await step.Context.SendActivityAsync(defaultMessage);
            }
        }

        public static async Task CarouselAfterNegativeFeedback(Activity msg, RecognizerResult recognizerResult, BotAccessors accessors, WaterfallStepContext step, int negative_flag, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<HeroCard> herocardlst = new List<HeroCard>();
            var questions = new QnaEntity();
            questions.answers = new List<Answer>();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            int TotalNextBestAnswers = Convert.ToInt32(Settings.TotalBestNextAnswers);
            await accessors.UserState.SaveChangesAsync(step.Context, true, cancellationToken);
            int counter = 0;
            var intents = recognizerResult.Intents.OrderByDescending(i => i.Value.Score);


            if (negative_flag==1)
            {
                foreach (var item in intents.Skip(1))
                {
                    var luisscore = item.Value.Score * 100;
                    step.Context.Activity.Text = item.Key;

                    if (counter <= Counter)/*(luisscore <= MedConfidenceThresholdValue1 && luisscore >= MedConfidenceThresholdValue2)*/
                    {
                        var qnaResponse = await accessors.QnAServices[Settings.QnAName01].GetAnswersAsync(step.Context);
                        counter++;
                        if (qnaResponse != null && qnaResponse.Length > 0)
                        {
                            Answer answer = new Answer
                            {
                                answer = qnaResponse[0].Answer,
                                metadata = qnaResponse[0].Metadata.ToList(),
                                questions = qnaResponse[0].Questions.ToList() //added this so that I can map the question to the variable
                            };
                            questions.answers.Add(answer);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in intents)
                {
                    var luisscore = item.Value.Score * 100;
                    step.Context.Activity.Text = item.Key;

                    if (counter <= Counter)/*(luisscore <= MedConfidenceThresholdValue1 && luisscore >= MedConfidenceThresholdValue2)*/
                    {
                        var qnaResponse = await accessors.QnAServices[Settings.QnAName01].GetAnswersAsync(step.Context);
                        counter++;
                        if (qnaResponse != null && qnaResponse.Length > 0)
                        {
                            Answer answer = new Answer
                            {
                                answer = qnaResponse[0].Answer,
                                metadata = qnaResponse[0].Metadata.ToList(),
                                questions = qnaResponse[0].Questions.ToList() //added this so that I can map the question to the variable
                            };
                            questions.answers.Add(answer);
                        }
                    }
                }
            }

            if (questions != null && questions.answers != null && questions.answers.Any() && questions.answers[0].answer != "No good match found in KB.")
            {
                foreach (var item in questions.answers.Where(i => i.metadata != null && i.metadata.Count > 0 && i.answer != "No good match found in KB."))
                {
                    var title = (from b in item.metadata where b.Name == CarouselSettings.carouseltitle select b.Value).FirstOrDefault();
                    var question = item.questions?.FirstOrDefault();
                    CardButton cButton = new CardButton();
                    cButton.value = question;// question;
                    herocardlst.Add(new HeroCard
                    {
                        Title = textInfo.ToTitleCase(title),
                        Subtitle = FirstCharToUpper((from b in item.metadata
                                                     where b.Name == CarouselSettings.carouselsummary
                                                     select b.Value).FirstOrDefault()),

                        Buttons = new List<CardAction>() { new CardAction(ActionTypes.PostBack, QNABotSettings.seedetails, value: JsonConvert.SerializeObject(cButton)) }, //qnaResponse.questions[0] JsonConvert.SerializeObject(cButton) changed instead of title, takes 
                    });
                }

                msg.SuggestedActions = new SuggestedActions
                {
                    Actions = CardActionDialog.GetNoneoftheAbove()
                };
                msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                herocardlst.ForEach(i => msg.Attachments.Add(i.ToAttachment()));
                await step.Context.SendActivityAsync(msg);
            }
            else
            {
                var defaultMessage = step.Context.Activity.CreateReply(QNABotSettings.sorrynextrelease);
                defaultMessage.SuggestedActions = new SuggestedActions { Actions = CardActionDialog.GetContactInfoCard() };
                await step.Context.SendActivityAsync(defaultMessage);
            }
        }

        #endregion

        public static string FirstCharToUpper(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
