using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BotApp.Classes
{

  #region Variable Settings
  public class QNABotSettings
  {
    #region Bot Configuration 
    static private string HowToAskMsg1 = "Me puede preguntar sobre los siguientes temas:\n\n* 401k: beneficio, pareo de Popular, renuncia, retiros al plan\n* Evidencia de enfermedad\n* Acceso a leave request\n* Licencia FMLA\n* Plan médico: costo, dependientes, descripción\n* Examen Preventivo Annual\n* Servicio militar\n* Incentivo annual: elegibilidad, taxes\n* Gimnasios: modalidades, elegibilidad, ligas deportivas\n* Fallecimiento de familiares\n* Proceso de jubilación\n* Personal time off: elegibilidad, cuota\n* Community time off\n* Licencia de maternidad: tiempo, solicitud, pago\n* Licencia de paternidad: solicitud\n* Beneficio para estudios\n* Renuncia\n* Bono de navidad\n* Horario reducido de trabajo\n* Incapacidad: Seguro, bono, SINOT\n* Vacaciones: acumulación, 10 días, tiempo parcial\n* Adiestramientos: errores tomando adiestramiento, cancelación";//If you are not sure what to write or how to write you may try some of the following:\n\n\n *Where do I access my 401k?*\n\n *Wow does PTO quota work?* \n\n *What are my vacation benefits?\n\n\n*Some topics I may know, however may include 401k, vacations, licenses, family death, PTO.
    static private string HowToAskMsg2 = "Me puedes hacer preguntas de la siguiente manera:\n\n - Cual es el pareo de popular a mi 401k?\n- Cuantos dias de PTO tiene un empleado?\n- Como tomo las licencias de maternidad?\n\n\nEres bienvenido a hacerme cualquier pregunta.";//You may ask another question if you'd like!
    static private string AccurateAnswer = "Aqui te presento algunos temas relacionado a tu pegunta que encontre en mi base de conocimiento.";//I couldn’t find an accurate answer, but these topics might help you.
    static private string PositiveMsg = "Excelente! Si tiene alguna otra pregunta, estoy a sus disposicion.";
    static private string HowtoContactHR = "How do I contact human resources?";//How do I contact human resources?
    static private string HowtoAsk = "How to ask?";//How to ask?
    static private string SorryNextRelease = "Disculpa, no entendi completamente su pregunta, recuerde que aun estoy en entrenamiento.";//Sorry I couldn’t find a good answer to your query.Your question will be reviewed and added in next releases
    static private string DidYouFind = "Estas satisfecho con mi respuesta?";//Did you find what you need?
    static private string ProvideFeedback = "Antes de continuar con la proxima pregunta, necesito que me des tu opinion acerca de la respuesta que te ofreci para la pregunta anterior."; //Please provide feedback!! Favor proveer tu satisfaccion con la respuesta primero antes de hacer otra pregunta! Esta satisfecho con la respuesta?
    static private string SeeDetails = "Ver mas";//See Details
    static private string NoneoftheAbove = "Ninguna de las anteriores";//None of the Above
    static private string LearnMore = "Learn More";//Learn More
    static private string LearnHowToAsk = "Ver que puedes preguntarme";//Learn how to ask me and what I know
    static private string ContactOurPeople = "Contactar a Nuestra Gente";//Contact Our People
    static private string PositiveFeedback = "positive-feedback";
    static private string NegativeFeedback = "negative-feedback";
    static private string ThumbsUp = "👍";
    static private string ThumbsDown = "👎";
    static private string YouMayContact = "Puede contactar a un representate de Nuestra Gente para que lo ayude con su consulta.";//You may contact an Our People agent to help you sort your question out!
    static private string PhoneNumber = "Numeros de Telefonos\n\n\nInterno: 8+63 2774\n\nExterno: (787)756-2774\n\nToll Free: 1866-303-2774";//Phone Numbers\n\n\nInternal: 8+63 2774\n\nExternal: (787)756-2774\n\nToll Free: 1866-303-2774
    static private string IntranetPortal = "Portal de Servicio Nuestra Gente";//Intranet OUR PEOPLE SERVICE PORTAL
    static private string IntranetPortalLink = "http://apps.popularinc.com/HR_EM_CustomerPortal/Home.aspx";
    static private string Scenario2Message = "Disculpa que no pude ofrecerte una pregunta apropiada, recuerda que aqun estoy en entrenamiento."; //Sorry I could not offer you an appropriate question, remember that I'm still in training.
    static private string WelcomeMessage = "Saludos, soy el chatbot de Nuestra Gente. Puedo ayudarle contestando algunas de las preguntas mas frecuentes para Nuestra Gente. Por favor ten en cuenta que tan solo puedo contestar preguntas generales. Para saber de los temas que conozco, puede escribir *ayuda*.\n\n\n**AVISO:** Aun me encuentro en mi periodo de prueba, por lo cual mi conocimiento no esta completo y puedes experimentar dificultades al hablar conmigo. Por favor reporta cualquier incidente en el formulario que se le proveyó.";
    static private string Satisfaction = "Satisfecho";
    static private string DeSatisfaction = "No Satisfecho";
    static private string HappyImage = "https://jonazurestorage123.blob.core.windows.net/myimages/happy.png";
    static private string SadImage = "https://jonazurestorage123.blob.core.windows.net/myimages/sad.png";
    static private string ContactHRImage = "https://jonazurestorage123.blob.core.windows.net/myimages/OurPeopleDoneS.jpg";
    public static string howtoaskmsg1
    {
      get
      {
        return HowToAskMsg1;
      }
    }

    public static string howtoaskmsg2
    {
      get
      {
        return HowToAskMsg2;
      }
    }

    public static string accurateanswer
    {
      get
      {
        return AccurateAnswer;
      }
    }

    public static string positivemsg
    {
      get
      {
        return PositiveMsg;
      }
    }

    public static string howtocontacthr
    {
      get
      {
        return HowtoContactHR;
      }
    }

    public static string howtoask
    {
      get
      {
        return HowtoAsk;
      }
    }

    public static string didyoufind
    {
      get
      {
        return DidYouFind;
      }
    }

    public static string sorrynextrelease
    {
      get
      {
        return SorryNextRelease;
      }
    }

    public static string providefeedback
    {
      get
      {
        return ProvideFeedback;
      }
    }

    public static string seedetails
    {
      get
      {
        return SeeDetails;
      }
    }

    public static string noneoftheabove
    {
      get
      {
        return NoneoftheAbove;
      }
    }

    public static string learnmore
    {
      get
      {
        return LearnMore;
      }
    }

    public static string learnhowtoask
    {
      get
      {
        return LearnHowToAsk;
      }
    }

    public static string contactourpeople
    {
      get
      {
        return ContactOurPeople;
      }
    }

    public static string positivefeedback
    {
      get
      {
        return PositiveFeedback;
      }
    }

    public static string negativefeedback
    {
      get
      {
        return NegativeFeedback;
      }
    }

    public static string thumbsup
    {
      get
      {
        return ThumbsUp;
      }
    }

    public static string thumbsdown
    {
      get
      {
        return ThumbsDown;
      }
    }

    public static string youmaycontact
    {
      get
      {
        return YouMayContact;
      }
    }

    public static string phonenumber
    {
      get
      {
        return PhoneNumber;
      }
    }

    public static string intranetportal
    {
      get
      {
        return IntranetPortal;
      }
    }

    public static string intranetportallink
    {
      get
      {
        return IntranetPortalLink;
      }
    }

    public static string scneario2message
    {
      get
      {
        return Scenario2Message;
      }
    }
    public static string welcomemessage
    {
      get
      {
        return WelcomeMessage;
      }
    }
    public static string satisfaction
    {
      get
      {
        return Satisfaction;
      }
    }
    public static string desatisfaction
    {
      get
      {
        return DeSatisfaction;
      }
    }
    public static string happyimage
    {
      get
      {
        return HappyImage;
      }
    }
    public static string sadimage
    {
      get
      {
        return SadImage;
      }
    }
    public static string contacthrimage
    {
      get
      {
        return ContactHRImage;
      }
    }
    #endregion Bot Configuration 
  }

  public static class CarouselSettings
  {
    private static string CarouselTitle = "title";
    private static string CarouselSummary = "summary";
    private static string CarouselValueButton = "carouselbuttonvalue";

    public static string carouseltitle
    {
      get
      {
        return CarouselTitle;
      }
    }

    public static string carouselsummary
    {
      get
      {
        return CarouselSummary;
      }
    }
  }
   

    public class CardButton
  {
    public string text { get; set; }
    public string value { get; set; }
  }
  #endregion
}
