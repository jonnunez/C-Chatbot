using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace QnaBOT.Services
{
    public static class FeedbackLUISAPI
    {
        static string FeedbackLUISAPIEndPoint = ConfigurationManager.AppSettings["FeedbackLUISAPIEndPoint"];
        public static async Task<FeedbackLUISEntity> GetFeedbackIntent(string feedbackFromUser)
        {
            var result = new FeedbackLUISEntity();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var responseString = await client.GetStringAsync(string.Format(FeedbackLUISAPIEndPoint, feedbackFromUser));
                    result = JsonConvert.DeserializeObject<FeedbackLUISEntity>(responseString);
                }
                catch
                {
                    throw new Exception("Unable to deserialize QnA Maker response string.");
                }

            }
            return result;
        }
    }
}
