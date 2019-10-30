using System.Collections.Generic;
using Microsoft.Bot.Builder.AI.QnA;
namespace BotApp
{

  public class Answer
  {
        public List<string> questions { get; set; }
        public string answer { get; set; }
    public List<Metadata> metadata { get; set; }
 

    }

    public class QnaEntity
  {
    public List<Answer> answers { get; set; }


    }
}
