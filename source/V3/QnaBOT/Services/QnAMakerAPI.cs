using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QnaBOT.Dialogs
{

    public class QnAMakerAPI
    {
        public QnAMakerAPI() { }

        private class QnAMakerResult
        {
            public IList<Response> Answers { get; set; }
        }

        private class Response
        {
            public string Answer { get; set; }
            public IList<string> Questions { get; set; }
            public double Score { get; set; }
        }

        private static string knowledgebaseId = "2d31530d-f1dc-4c9e-97dc-fe93a616124f"; // Use knowledge base id created.   
        private static string qnamakerSubscriptionKey = "48df3bff3039480ca6518ef8dfb2a234"; //Use subscription key assigned to you.   

        /// <summary>
        /// Try to query a question and get the answer > 50 points
        /// </summary>
        /// <param name="question">The question you want to answer</param>
        /// <param name="answer">The answer you might get</param>
        /// <returns>If get an answer which scores more than 50</returns>
        /// 
        public bool TryQuery(string question, out string answer)
        {
            string responseString = string.Empty;
            answer = string.Empty;
            var query = question; //User Query

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v4.0"); //  https://westus.api.cognitive.microsoft.com/qnamaker/v2.0
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;
                try
                {
                    //Add the subscription key header
                    client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                    client.Headers.Add("Content-Type", "application/json");
                    responseString = client.UploadString(builder.Uri, postBody);
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var resp = ex.Response as HttpWebResponse;
                        if (resp != null)
                        {
                            Debug.WriteLine("HTTP Status Code: " + (int)resp.StatusCode);
                        }
                        //else
                        //{

                        //}
                    }
                    else
                    {

                    }
                    return false;
                }
            }

            QnAMakerResult response;

            try
            {
                response = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
            }
            catch
            {
                return false;
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }

            try
            {
                Debug.WriteLine(response.Answers[0].Answer + response.Answers[0].Score.ToString());
                answer = response.Answers[0].Answer;

                return response.Answers[0].Score >= 38 ? true : false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }
    }
}