using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace QnaBOT.Classes
{
    /// <summary>
    /// Setting used with all the LUIS intents used in the hr dialog
    /// </summary>
    public class QNABotSettings
    {

        #region Bot Configuration 
        static private List<string> Threshold1 = ConfigurationManager.AppSettings["Threshold1"].Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        static private List<string> Threshold2 = ConfigurationManager.AppSettings["Threshold2"].Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        static private List<string> Threshold3 = ConfigurationManager.AppSettings["Threshold3"].Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        static private string Prompt1 = "Me puede preguntar sobre los siguientes temas:\n\n* 401k: beneficio, pareo de Popular, renuncia, retiros al plan\n* Evidencia de enfermedad\n* Acceso a leave request\n* Licencia FMLA\n* Plan médico: costo, dependientes, descripción\n* Examen Preventivo Annual\n* Servicio militar\n* Incentivo annual: elegibilidad, taxes\n* Gimnasios: modalidades, elegibilidad, ligas deportivas\n* Fallecimiento de familiares\n* Proceso de jubilación\n* Personal time off: elegibilidad, cuota\n* Community time off\n* Licencia de maternidad: tiempo, solicitud, pago\n* Licencia de paternidad: solicitud\n* Beneficio para estudios\n* Renuncia\n* Bono de navidad\n* Horario reducido de trabajo\n* Incapacidad: Seguro, bono, SINOT\n* Vacaciones: acumulación, 10 días, tiempo parcial\n* Adiestramientos: errores tomando adiestramiento, cancelación";//If you are not sure what to write or how to write you may try some of the following:\n\n\n *Where do I access my 401k?*\n\n *Wow does PTO quota work?* \n\n *What are my vacation benefits?\n\n\n*Some topics I may know, however may include 401k, vacations, licenses, family death, PTO.
        static private string Prompt2 = "Me puedes hacer preguntas de la siguiente manera:\n\n - Cual es el pareo de popular a mi 401k?\n- Cuantos dias de PTO tiene un empleado?\n- Como tomo las licencias de maternidad?\n\n\nEres bienvenido a hacerme cualquier pregunta.";//You may ask another question if you'd like!
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
        static private string Scenario2Message = "Disculpa que no pude ofrecerte una pregunta apropiada, recuerda que aqun estoy en entrenamiento.";


        public static List<string> threshold1
        {
            get
            {
                return Threshold1;
            }
        }
        public static List<string> threshold2
        {
            get
            {
                return Threshold2;
            }
        }
        public static List<string> threshold3
        {
            get
            {
                return Threshold3;
            }
        }

        public static string prompt1
        {
            get
            {
                return Prompt1;
            }
        }

        public static string prompt2
        {
            get
            {
                return Prompt2;
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

        #endregion Bot Configuration 
    }

    public static class CarouselSettings
    {
        private static string CarouselTitle = "title";
        private static string CarouselSummary = "summary";

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

    public static class QNAMakerSettings
    {
        private static readonly string _Host = ConfigurationManager.AppSettings["QnAMakerEndPoint"];
        private static readonly string _KBID = ConfigurationManager.AppSettings["QnAMakerKBId"];
        private static readonly string _Authkey = ConfigurationManager.AppSettings["QnAMakerAuthKey"];
        private static readonly string _OcpApimSubscriptionKey = ConfigurationManager.AppSettings["QnAMakerOcpApimSubscriptionKey"];

        public static string host
        {
            get
            {
                return _Host;
            }
        }

        public static string kbid
        {
            get
            {
                return _KBID;
            }
        }

        public static string authkey
        {
            get
            {
                return _Authkey;
            }
        }

        public static string OcpApimSubscriptionKey
        {
            get
            {
                return _OcpApimSubscriptionKey;
            }
        }
    }
}