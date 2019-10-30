using System.Collections.Generic;

namespace BotApp
{
  public class Settings
  {
    public static string MicrosoftAppId { get; set; }
    public static string MicrosoftAppPassword { get; set; }
    public static string BotConversationStorageConnectionString { get; set; }
    public static string BotConversationStorageKey { get; set; }
    public static string BotConversationStorageDatabaseId { get; set; }
    public static string BotConversationStorageUserCollection { get; set; }
    public static string BotConversationStorageConversationCollection { get; set; }
    public static string LuisAppId01 { get; set; }
    public static string LuisName01 { get; set; }
    public static string LuisAuthoringKey01 { get; set; }
    public static string LuisEndpoint01 { get; set; }
    public static string QnAKbId01 { get; set; }
    public static string QnAName01 { get; set; }
    public static string QnAEndpointKey01 { get; set; }
    public static string QnAHostname01 { get; set; }
    public static string MongoDBConnectionString { get; set; }
    public static string MongoDBDatabaseId { get; set; }
    public static string PersonCollection { get; set; }
    public static List<string> HighConfidenceThreshold { get; set; }
    public static List<string> MediumConfidenceThreshold { get; set; }
    public static List<string> LowConfidenceThreshold { get; set; }
    public static string TotalBestNextAnswers { get; internal set; }
    }
}
