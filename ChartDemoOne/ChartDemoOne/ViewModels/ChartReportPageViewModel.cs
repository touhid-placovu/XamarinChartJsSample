using ChartDemoOne.Models;
using ChartDemoOne.Serrvice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ChartDemoOne.ViewModels
{
    public class ChartReportPageViewModel : BaseViewModel
    {
        public bool IsSuccess = false;
        public ChartDataModel ChartDataModel = null;
        private PatientChartDataService patientChartDataService;
        public Command LoadChartDataCommand { get; set; }
        public ChartReportPageViewModel()
        {
            patientChartDataService = new PatientChartDataService();
            LoadChartDataCommand = new Command(async () => await ExecuteLoadChartDataCommandAsync());            
        }

        public async Task ExecuteLoadChartDataCommandAsync()
        {
            if (IsBusy) { return; }
            if (ChartDataModel != null) { return; }
            try
            {
                IsBusy = true;
                var response = await patientChartDataService.GetChartDataModel();
                if (response != null)
                {
                    ChartDataModel = new ChartDataModel();                   
                    ChartDataModel = response;               
                    BuildReportHtml("line");
                    //await Task.Yield();
                }
                else
                {
                    BuildReportHtml("line");
                    await Task.Yield();
                }
            }
            catch (Exception)
            {
                IsSuccess = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        string reporthtml = string.Empty;

        public string ReportHtml
        {
            get { return reporthtml; }
            set { SetProperty(ref reporthtml, value); }
        }

        public void BuildReportHtml(string chartType)
        {
            var chartConfigScript = GetChartScript(chartType);
            var html = GetHtmlWithChartConfig(chartConfigScript);
            ReportHtml = html;
        }

        private string GetHtmlWithChartConfig(string chartConfigScript)
        {
            var chartConfigJsScript = $"<script type='text/javascript'>{chartConfigScript}</script>";
            var localScript = "<script src='file:///android_asset/chart_bundle.js' type='text/javascript'></script>";
            var style = @"<style>*,*::before,*::after {box-sizing: border-box;} html{height:100%; width:100%;-webkit-text-size-adjust: 100%;-webkit-tap-highlight-color: rgba(0, 0, 0, 0);} body{height:100%; width:100%;overflow:hidden;line-height: 1.15;padding:0; margin:0} #mycanvas{height:100%; width:1000px;padding:0; margin:0;} .conteiner{height:100%;overflow-x:auto}</style>";
            var meta = @"<meta charset=""utf-8"" /><meta name=""viewport"" content=""width=device-width"" />";
            var canvas = @"<canvas id=""mycanvas""></canvas>";
            var document = $@"<html><head>{meta}{style}{localScript}</head><body><div class=""conteiner"">{canvas}</div>{chartConfigJsScript}</body></html>";
            return document;
        }

        private string GetChartScript(string chartType)
        {
            var ddd = GenerateChartData(ChartDataModel, chartType);
            var chartConfig = GetSpendingChartConfig(chartType);
            var script = $@"var config = {ddd};
                window.onload = function() {{
                  var canvasContext = document.getElementById(""mycanvas"").getContext(""2d"");
                  new Chart(canvasContext, config);
                }};";
            return script;
        }

        private string GetSpendingChartConfig(string chartType)
        {
            var config = new
            {
                type = chartType,
                data = GetChartData(),
                options = new
                {
                    responsive = true,
                    maintainAspectRatio = false,
                    fill = false,                    
                    legend = new
                    {
                        position = "top"
                    },
                    animation = new
                    {
                        animateScale = false
                    },
                }
            };
            var jsonConfig = JsonConvert.SerializeObject(config);
            return jsonConfig;
        }

        private object GenerateChartData(ChartDataModel chartDataModel, string chartType)
        {
            var labels = chartDataModel.Labels.ToArray();
            var config = new
            {
                type = chartType,
                data = new
                {
                    labels = chartDataModel.Labels,
                    labelsValue = chartDataModel.LabelsValue,
                    datasets = GetSeriesDataModels(chartDataModel),
                    tooltipLabels = chartDataModel.TooltipLabels,
                    tooltipDataSet = chartDataModel.TooltipSeries
                },
                options = new
                {
                    responsive = true,
                    maintainAspectRatio = false,
                    fill = false,
                    legend = new
                    {
                        position = "top"
                    },
                    animation = new
                    {
                        animateScale = false
                    },
                }
            };
            var jsonConfig = JsonConvert.SerializeObject(config);
            return jsonConfig;
        }

        private object GetChartData()
        {
            var labels = new[] { "Groceries", "Car", "Flat", "Electronics", "Entertainment", "Insurance" };
            var randomGen = new Random();
            var dataPoints1 = Enumerable.Range(0, labels.Length)
                .Select(i => randomGen.Next(5, 25))
                .ToList();
            var dataPoints2 = Enumerable.Range(0, labels.Length)
                .Select(i => randomGen.Next(5, 25))
                .ToList();
            var data = new
            {
                datasets = new[]
                {
                    new
                    {
                        label = "Spending",
                        data = dataPoints1,
                        borderColor = "#FF0000",
                        backgroundColor = "#FF0000",
                        fill = false,
                        pointRadius = 7,
                        borderWidth = 3,
                        pointHoverRadius = 7,
                    },
                    new
                    {
                        label = "My Spending",
                        data = dataPoints2,
                        borderColor = "#0000FF",
                        backgroundColor = "#0000FF",
                        fill = false,
                        pointRadius = 7,
                        borderWidth = 3,
                        pointHoverRadius = 7,
                    }
                },
                labels
            };
            return data;
        }

        private List<SeriesDataModel> GetSeriesDataModels(ChartDataModel chartDataModel)
        {
            List<SeriesDataModel> models = new List<SeriesDataModel>();

            foreach ( var data in chartDataModel.Series)
            {
                models.Add(new SeriesDataModel {
                    data = data.DataList,
                    label = data.LabelName,
                    backgroundColor = data.GraphProperty.backgroundColor,
                    borderWidth = data.GraphProperty.pointBorderWidth,
                    fill = data.GraphProperty.fill.ToString().ToLower(),
                    pointRadius = data.GraphProperty.pointRadius,
                    pointHoverRadius = data.GraphProperty.pointRadius,
                    borderColor = data.GraphProperty.backgroundColor,
                });               
            }
            return models;
        }
    }
}
