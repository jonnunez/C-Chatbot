//using Microsoft.ApplicationInsights;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Connector;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using System.Xml.Linq;

//namespace QnaBOT.Services
//{
//    public static class GetLanguage
//    {
//        public static string GetLocaleString(IDialogContext context)
//        {
//            var telemetry = new TelemetryClient();
//            string result = "EN";
//            try
//            {
               
//                context.UserData.TryGetValue("LanguageCode", out result);               
//            }
//            catch (Exception ex)
//            {
//                telemetry.TrackEvent("GetLocaleString", new Dictionary<string, string> { { "Exception", SimpleJson.SimpleJson.SerializeObject(ex) } });
//            }
//            return result;
//        }
//    }
//    public static class TranslatorToken
//    {
//        static string ApiKey = ConfigurationManager.AppSettings["TranslatorTextAPIKey"];

//        public static async Task<string> GetAuthenticationToken()
//        {
//            string endpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

//            using (var client = new HttpClient())
//            {
//                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
//                var response = await client.PostAsync(endpoint, null);
//                var token = await response.Content.ReadAsStringAsync();
//                return token;
//            }
//        }
//    }
//    public static class StringExtensions
//    {
//        public async static Task<string> ToUserLocale(this string text, string tolanguage)
//        {
//            var accessToken = await TranslatorToken.GetAuthenticationToken();
//            var translatorClient = new TranslatorAPIClient(accessToken);
//            text = await translatorClient.TranslateText(text, tolanguage);
//            return text;
//        }
//    }
//    public class TranslatorAPIClient
//    {
//        private readonly string _token;
//        TelemetryClient telemetry = new TelemetryClient();
//        public TranslatorAPIClient(string token)
//        {
//            _token = token;
//        }
//        public async Task<string> TranslateText(string inputText, string language)
//        {
//            string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate";
//            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={language}&contentType=text/plain";

//            using (var client = new HttpClient())
//            {
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
//                var response = await client.GetAsync(url + query);
//                var result = await response.Content.ReadAsStringAsync();

//                if (!response.IsSuccessStatusCode)
//                    return inputText;

//                var translatedText = XElement.Parse(result).Value;
//                return translatedText;
//            }
//        }
//        public async Task<string> Detect(string inputText)
//        {
//            string url = "http://api.microsofttranslator.com/v2/Http.svc/Detect";
//            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&contentType=text/plain";

//            using (var client = new HttpClient())
//            {
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
//                var response = await client.GetAsync(url + query);
//                var result = await response.Content.ReadAsStringAsync();

//                if (!response.IsSuccessStatusCode)
//                    return "Hata: " + result;

//                var translatedText = XElement.Parse(result).Value;
//                return translatedText;
//            }
//        }

//        public string GetUserLanguageCode(Activity activity)
//        {
           
//            string languageCode = string.Empty;

//            try
//            {
//                StateClient stateClient = activity.GetStateClient();
//                BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);
//                languageCode = userData.GetProperty<string>("LanguageCode");
//            }
//            catch (Exception ex)
//            {
//                telemetry.TrackEvent("GetUserLanguageCode", new Dictionary<string, string> { { "Exception", SimpleJson.SimpleJson.SerializeObject(ex) } });

//            }
//            return languageCode;
//        }
//        //public async void SetUserLanguageCode(Activity activity, string languageCode)
//        //{
//        //    try
//        //    {
//        //        StateClient stateClient = activity.GetStateClient();
//        //        BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);

//        //        var currentLanguageCode = userData.GetProperty<string>("LanguageCode");

//        //        if (currentLanguageCode != languageCode)
//        //        {
//        //            userData.SetProperty<string>("LanguageCode", languageCode);
//        //            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw;
//        //        //telemetry.TrackEvent("SetUserLanguageCode", new Dictionary<string, string> { { "Exception", SimpleJson.SimpleJson.SerializeObject(ex) } });
//        //    }

//        //}
//        public void SetUserLanguageCode(IDialogContext context, string languageCode)
//        {
//            try
//            {
//                context.UserData.SetValue("LanguageCode", languageCode);
//            }
//            catch (Exception ex)
//            {
//                telemetry.TrackEvent("SetUserLanguageCode", new Dictionary<string, string> { { "Exception", SimpleJson.SimpleJson.SerializeObject(ex) } });
//            }

//        }
//        public string GetUserLanguageCode(IDialogContext context)
//        {
//            string result = string.Empty;
//            try
//            {
                
//                context.UserData.TryGetValue("LanguageCode", out result);

                
//            }
//            catch (Exception ex)
//            {

//                telemetry.TrackEvent("SetUserLanguageCode", new Dictionary<string, string> { { "Exception", SimpleJson.SimpleJson.SerializeObject(ex) } });
//            }
//            return result;
//        }

//    }
//}