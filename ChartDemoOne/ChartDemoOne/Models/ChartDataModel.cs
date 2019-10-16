using System.Collections.Generic;

namespace ChartDemoOne.Models
{
    public class ChartDataModel
    {
        public ChartDataModel()
        {
            //ctr
        }
        public List<string> Labels { get; set; }
        public List<double> LabelsValue { get; set; }
        public List<SeriesModel> Series { get; set; }
        public List<string> TooltipLabels { get; set; }
        public object TooltipQuestionIds { get; set; }
        public string AggregateGraphUrl { get; set; }
        public object GraphUrl { get; set; }
        public object IndividualQuestionGraphUrl { get; set; }
        public List<TooltipSeries> TooltipSeries { get; set; }
        public string ChartType { get; set; }
        public int GraphType { get; set; }
        public string ChartName { get; set; }
        public object ReportIdentifier { get; set; }
        public string YLabel { get; set; }
        public string XLabel { get; set; }
        public bool IsAgeGroupGraph { get; set; }
        public bool IsScoreGraph { get; set; }
        public bool IsSingleQuestion { get; set; }
        public int SurveyQuestionId { get; set; }
        public List<int> SurveyQuestionIds { get; set; }
        public List<string> SurveyQuestionShortTexts { get; set; }
        public object SurveyQuestionDetailsIds { get; set; }
        public object SurveyQuestionDetailShortTexts { get; set; }
        public int SurveyQuestionSetMaxScore { get; set; }
        public int SurveyQuestionMaxScore { get; set; }
        public object GraphTypes { get; set; }
        public bool IsStackedBarChart { get; set; }
        public bool IsAlsoPieChart { get; set; }
        public bool IsDisable { get; set; }
        public bool HasAdditionalReport { get; set; }
        public object AdditionalReportUrl { get; set; }
        public int additionalReportNumber { get; set; }
        public int additionalReportType { get; set; }
        public bool IsComparativeData { get; set; }
        public object ComparativeDatahUrl { get; set; }
        public object RoboticAcutePainGraphs { get; set; }
        public object RoboticBowelRecoveryGraphs { get; set; }
        public PatientDataModel PatientData { get; set; }
        public bool IsPatientComparativeData { get; set; }
        public object PatientProfileName { get; set; }
        public object PracticeName { get; set; }
        public bool IsPatientComparativeWithProfessionalData { get; set; }
        public object LabelNameColors { get; set; }
        public bool IsCombinedBarAndLineChart { get; set; }
        public object RightYLabel { get; set; }
        public bool ShowRightYAxis { get; set; }
        public bool IsPatientProgressGraph { get; set; }
        public bool HideYAxisGrid { get; set; }
        public bool HideXAxisGrid { get; set; }
        public bool HasBackgroundRules { get; set; }
        public List<BackgroundRule> BackgroundRules { get; set; }
        public bool HasPatientData
        {
            get
            {
                return PatientData != null;
            }
        }
    }
}
