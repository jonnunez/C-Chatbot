using System;
using static BotApp.helpers.ConfidenceScoreEnums;

namespace BotApp.helpers
{
  public class ThresholdHandler
  {
    private static readonly double HighConfidenceThresholdValue1 = Convert.ToInt32(Settings.HighConfidenceThreshold[0]) + 0.99;
    private static readonly double HighConfidenceThresholdValue2 = Convert.ToInt32(Settings.HighConfidenceThreshold[1]);
    private static readonly double MedConfidenceThresholdValue1 = Convert.ToInt32(Settings.MediumConfidenceThreshold[0]) + 0.99;
    private static readonly double MedConfidenceThresholdValue2 = Convert.ToInt32(Settings.MediumConfidenceThreshold[1]);
    private static readonly double LowConfidenceThresholdValue1 = Convert.ToInt32(Settings.LowConfidenceThreshold[0]) + 0.99;
    private static readonly double LowConfidenceThresholdValue2 = Convert.ToInt32(Settings.LowConfidenceThreshold[1]);

    public static int ConfidenceScoreIdentification(double score)
    {
      if (score <= HighConfidenceThresholdValue1 && score >= HighConfidenceThresholdValue2)
      {
        return (int)ConfidenceScoreEnum.high;
      }
      else if (score <= MedConfidenceThresholdValue1 && score >= MedConfidenceThresholdValue2)
      {
        return (int)ConfidenceScoreEnum.mid;
      }
      else
      {
        return (int)ConfidenceScoreEnum.low;
      }

    }
  }
}
