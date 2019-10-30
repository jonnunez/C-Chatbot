using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using QnaBOT.Classes;


namespace QnaBOT.Helpers
{
    public static class Extensions
    {
        public static void Extend(this QnAMakerResults qnaMakerResults)
        {

        }
    }
    public class QnaHelper
    {      

        //private static readonly string _Host = ConfigurationManager.AppSettings["QnAMakerEndPoint"];
        //private static readonly string _KBID = ConfigurationManager.AppSettings["QnAMakerKBId"];
        //private static readonly string _Authkey = ConfigurationManager.AppSettings["QnAMakerAuthKey"];
        //private static readonly string _OcpApimSubscriptionKey = ConfigurationManager.AppSettings["QnAMakerOcpApimSubscriptionKey"];
   

        public QnaHelper()
        {

        }

        public static QnaEntity QNAConnection(string question)
        {
            var lst = new QnaEntity();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true; //---> Remove later 

                var client = new RestClient(QNAMakerSettings.host);
                var request = new RestRequest($"/knowledgebases/{QNAMakerSettings.kbid}/generateAnswer", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Authorization", $"EndpointKey {QNAMakerSettings.authkey}");
                request.AddBody(new
                {
                    question = question,
                    top = 5
                });
                lst = client.Execute<QnaEntity>(request).Data;
          
            }
            catch (Exception ex)
            {
                //throw?
            }
            return lst;
        }
    

    }
}


//public static string Question1 { get; set; }
//public static string Question2 { get; set; }
//public static string Question3 { get; set; }


//private const string _Host = "https://testqnafm.azurewebsites.net/qnamaker";
//private const string _KBID = "609b81e3-f266-44df-95ee-d85d1ca8dda9";
//private const string _Authkey = "EndpointKey bb775fd1-40a3-4a79-a3cc-0487825f0a89";
//private const string _OcpApimSubscriptionKey = "b713b429f9e444bbb108b37d0079941a";