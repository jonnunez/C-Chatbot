using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;

namespace QnaBOT.Dialogs
{
    public class TextAnalysisHelper
    {
        public static double scores;

        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["TextAnalyticsKey"]);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public static async Task<string> obtainLanguage(string key, AzureRegions region, string text)
        {
            TextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials());
            client.AzureRegion = region;

            BatchInput input = new BatchInput(new List<Input>()
            {
                new Input("1", text)
            });
            var result = await client.DetectLanguageAsync(input);
            var languages = result.Documents.FirstOrDefault()?.DetectedLanguages;
            if (languages.Count > 0)
                return "Language: " + string.Join(", ", languages.Select(x => x.Name));
            else
                return "Unable to detect language!";
        }

        public static async Task<string> obtainKeyPhrase(string key, AzureRegions region, string text)
        {
            TextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials());
            client.AzureRegion = region;

            MultiLanguageBatchInput input = new MultiLanguageBatchInput(new List<MultiLanguageInput>()
            {
                new MultiLanguageInput("en", "1", text)
            });
            var result = await client.KeyPhrasesAsync(input);
            var keyphrases = result.Documents.FirstOrDefault().KeyPhrases;
            if (keyphrases.Count > 0)
                return "Keyphrases: " + string.Join(", ", keyphrases);
            else
                return "";
        }

        public static async Task<double?> obtainSentiment(string key, AzureRegions region, string text)
        {
            TextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials());
            client.AzureRegion = region;

            MultiLanguageBatchInput input = new MultiLanguageBatchInput(new List<MultiLanguageInput>()
            {
                new MultiLanguageInput("en", "1", text)
            });
            var result = await client.SentimentAsync(input);
            var score = result.Documents.FirstOrDefault().Score;
            //var tostring = Convert.ToString(score);
            return score;
        }
    }
}