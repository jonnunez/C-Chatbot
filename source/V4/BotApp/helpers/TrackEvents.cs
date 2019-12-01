using Microsoft.ApplicationInsights;
using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace BotApp.helpers
{
    public class TrackEvents
    {
        static TelemetryClient telemetry = new TelemetryClient();
        static TelemetryClient telemetryConversation = new TelemetryClient();

        public static void TrackEvent(string lastSearchedText, string lastAnswerText, string vote)
        {
            var properties = new Dictionary<string, string>
                        {
                              {"Question", lastSearchedText },
                              {"Answer",lastAnswerText  },
                              {"Vote", vote },
                        };
            telemetry.TrackEvent(vote + "-Vote", properties);
        }

        public static void TrackConversation(string lastSearchedText = "N/A", string lastAnswerText = "N/A", string Conversationtype="N/A", string score = "N/A", string intent = "N/A", ChannelAccount from = null)
        {
            var properties = new Dictionary<string, string>
                        {
                              {"Question", lastSearchedText },
                              {"Answer",lastAnswerText  },
                              {"Conversation Type", Conversationtype},
                              {"Confidence Score", score},
                              {"Intent", intent},
                        };

            if (from != null)
            {
                properties.Add("Username", from.Id);
                properties.Add("Email", from.Name);
            }

            telemetryConversation.TrackEvent(Conversationtype + "- Conversation Type", properties);
        }
    }
}