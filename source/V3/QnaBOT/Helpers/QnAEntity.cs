using System.Collections.Generic;

namespace QnaBOT
{
    public class Metadata
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Answer
    {
        public List<string> questions { get; set; }
        public string answer { get; set; }
        public int score { get; set; }
        public int id { get; set; }
        public string source { get; set; }
      
        public List<Metadata> metadata { get; set; }
    }

    public class QnaEntity
    {
        public List<Answer> answers { get; set; }
    }

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class SentimentAnalysis
    {
        public string label { get; set; }
        public double score { get; set; }
    }

    public class FeedbackLUISEntity
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Intent> intents { get; set; }
        public List<object> entities { get; set; }
        public SentimentAnalysis sentimentAnalysis { get; set; }
        public double score { get; set; }

    }

}