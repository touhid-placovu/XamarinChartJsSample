using ChartDemoOne.Models;
using ChartDemoOne.Serrvice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                options = GenerateChartOptionsOne(chartDataModel),
                plugins = GenerateChartPlugins(chartDataModel)
            };
            var jsonConfig = JsonConvert.SerializeObject(config);
            jsonConfig = RemoveExtraQuotation(jsonConfig);
            return jsonConfig;
        }

        

        private object GenerateChartOptionsOne(ChartDataModel _chartData)
        {
            var chartOptions = new
            {
                responsive = false,
                maintainAspectRatio = false,
                scales = new
                {
                    xAxes = new[]
                    {
                        new
                        {
                            id = "x-axis-1",
                            ticks = new
                            {
                                fontSize =  15,
                                fontStyle =  "bold"
                            },
                            stacked = _chartData.IsStackedBarChart,
                            gridLines = new
                            {
                                zeroLineWidth= 1,
                                zeroLineColor= "#000",
                            },
                            scaleLabel = new
                            {
                                display= true,
                                labelString= _chartData.XLabel,
                                fontSize= 15,
                                fontStyle= "bold"
                            }

                        }
                    },
                    yAxes = new[]
                    {
                        new
                        {
                            id = "y-axis-1",
                            display= true,
                            ticks = new
                            {
                                fontSize =  15,
                                fontStyle =  "bold",
                                beginAtZero = true
                            },
                            stacked = _chartData.IsStackedBarChart,
                            gridLines = new
                            {
                                zeroLineWidth= 1,
                                zeroLineColor= "#000",
                                drawTicks= true,
                                tickMarkLength= 15,
                            },
                            scaleLabel = new
                            {
                                display= true,
                                labelString= _chartData.YLabel,
                                fontSize= 15,
                                fontStyle= "bold"
                            }
                        }
                    }
                },
                title = new
                {
                    display = true,
                    text = _chartData.ChartName,
                    fontSize = 20,
                    fontFamily = "Raleway",
                },
                backgroundRules = GetChartBackgroundRules(_chartData),
                legend = new
                {
                    labels = new
                    {
                        usePointStyle = true,
                        pointStyle = "round",
                        fontSize = 15,
                        padding = 40,
                        fontFamily = "Raleway",
                    }
                },
                tooltips = new
                {
                    backgroundColor = "#F7BB29",
                    bodyFontColor = "#000",
                    titleFontColor = "#000",
                    caretPadding = 10,
                    xPadding = 10,
                    yPadding = 10,
                    mode = "nearest",
                    position = "nearest",
                    titleFontSize = 15,
                    bodyFontSize = 15,
                    titleFontFamily = "Raleway",
                    bodyFontFamily = "Raleway",
                    titleSpacing = 8,
                    bodySpacing = 8,
                    displayColors = false,
                    callback = GetToolTipCallBackFunction()
                },
                animation = GetChartAnimation(_chartData),
                hover = new
                {
                    animationDuration = 0
                }

            };

            //if (_chartData.IsPatientProgressGraph)
            //{
            //    if (_chartData.SurveyQuestionMaxScore > 0)
            //    {
            //        chartOptions.scales.yAxes[0].ticks.max = _chartData.SurveyQuestionMaxScore;
            //    }
            //    chartOptions.scales.xAxes[0].gridLines.display = false;
            //}

            return chartOptions;
        }
        private object GetChartBackgroundRules(ChartDataModel _chartData)
        {
            var backgroundRules = new List<object>();
            if (_chartData.BackgroundRules != null)
            {
                foreach (var backgroundRule in _chartData.BackgroundRules)
                {
                    var ruleItem = new
                    {
                        BackgroundColor = $"{ backgroundRule.BackgroundColor}",
                        LabelText = $"{backgroundRule.LabelText}",
                        backgroundRule.YaxisSegementEnd,
                        backgroundRule.YaxisSegementStart
                    };
                    backgroundRules.Add(ruleItem);
                }
            }
            return backgroundRules;
        }
        private string RemoveExtraQuotation(string json)
        {
            try
            {
                var data = json.Replace("\"replaceQuoteStart", "");
                data = data.Replace("replaceQuoteEnd\"", "");
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string GetToolTipCallBackFunction()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"replaceQuoteStart{
                    title: function(tooltipItem, data)
                    {
                        title = data.datasets[tooltipItem[0].datasetIndex].label + ' : ' + tooltipItem[0].yLabel + ' (' + tooltipItem[0].xLabel + ' )';
                        return title;
                    },
                    label: function(tooltipItem, data)
                    {
                        var dataArray = [];
                        if (data.tooltipLabels == undefined || data.tooltipLabels == null)
                            return dataArray;
                        var tooltipLabels = data.tooltipLabels;
                        var tooltipData = data.tooltipDataSet[tooltipItem.datasetIndex].DataList[tooltipItem.index];
                        var size = tooltipLabels != undefined ? tooltipLabels.length : 0;
                        for (var i = 0; i < size; i++)
                        {
                            var item = '\u27A4 ' + tooltipLabels[i] + ' : ' + tooltipData[i] + ' ';
                            dataArray.push(item);
                        }
                        return dataArray;
                    },
                }replaceQuoteEnd");

            var data = builder.ToString().Replace("\r\n", "").Trim('"');
            return data;
        }

        private string GetChartAnimation(ChartDataModel _chartData)
        {
            string chartAnimation = $@"replaceQuoteStart{{
                duration: 1000,
                onComplete: function()
    {{
    if ({_chartData.IsPatientComparativeData.ToBooleanString()})
    {{
        var chartInstance = this.chart;
        var ctx = chartInstance.ctx;
        ctx.textAlign = 'left';
        ctx.font = '14px Open Sans';
        ctx.fillStyle = '#fff';
        Chart.helpers.each(this.data.datasets.forEach(function(dataset, i) {{
            var meta = chartInstance.controller.getDatasetMeta(i);
            Chart.helpers.each(meta.data.forEach(function(bar, index) {{
                if ({_chartData.IsPatientComparativeWithProfessionalData.ToBooleanString()} && {_chartData.HasPatientData.ToBooleanString()})
                {{
                    if (i === 0 && $.inArray(bar._model.label, {JsonConvert.SerializeObject(_chartData.PatientData.Labels)}) > -1) {{
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 10);
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 25);
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 40);
                    }}
                }}
                else
                {{
                    if ({_chartData.HasPatientData.ToBooleanString()} && bar._model.datasetLabel === {JsonConvert.SerializeObject(_chartData.PatientData.LabelNames)}[index] &&
                        bar._model.label === {JsonConvert.SerializeObject(_chartData.PatientData.Labels)}[index])
                    {{
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 10);
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 25);
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 40);
                    }}
                }}
            }}),
                                this);
        }}),
                            this);
    }}
}}
            }}replaceQuoteEnd";
            chartAnimation = chartAnimation.Replace("\r\n", "").Trim('"');
            return chartAnimation;
        }

        private string GenerateChartOptions(ChartDataModel _chartData)
        {
            var chartOptions = $@"{{
            responsive: false,
            maintainAspectRatio: false,
            scales: {{
                xAxes: [{{
                    id: 'x-axis-1',
                    ticks:
            {{
                        fontSize: 15,
                        fontStyle: 'bold'
                    }},
                    stacked: {_chartData.IsStackedBarChart.ToBooleanString()},
                    gridLines:
            {{
                zeroLineWidth: 1,
                        zeroLineColor: '#000',
                    }},
                    scaleLabel:
            {{
                        display: true,
                        labelString: '{_chartData.XLabel}',
                        fontSize: 15,
                        fontStyle: 'bold'
                    }}
        }}],
                yAxes: [{{
                    id: 'y-axis-1',
                    display: true,
                    ticks: {{
                        fontSize: 15,
                        fontStyle: 'bold',
                        beginAtZero: true                            
                    }},
                    stacked: {_chartData.IsStackedBarChart.ToBooleanString()},
                    gridLines: {{
                        zeroLineWidth: 1,
                        zeroLineColor: '#000',
                        drawTicks: true,
                        tickMarkLength: 15,
                    }},
                    scaleLabel: {{
                        display: true,
                        labelString: '{_chartData.YLabel}',
                        fontSize: 15,
                        fontStyle: 'bold'
                    }}
                }},
                {{
                    id: 'y-axis-2',
                    display: {_chartData.ShowRightYAxis.ToBooleanString()},
                    position: 'right',
                    ticks: {{
                        fontSize: 15,
                        fontStyle: 'bold',
                        beginAtZero: true
                    }},
                    gridLines: {{
                        zeroLineWidth: 1,
                        zeroLineColor: '#000',
                        drawTicks: true,
                        tickMarkLength: 15,
                        color: '#000',
                        display: false
                    }},
                    scaleLabel: {{
                        display: true,
                        labelString: '{_chartData.RightYLabel}',
                        fontSize: 15,
                        fontStyle: 'bold'
                    }}
                }}]
            }},
            title: {{
                display: true,
                text: '{_chartData.ChartName}',
                fontSize: 20,
                fontFamily: 'Raleway',
            }},
            backgroundRules: {JsonConvert.SerializeObject(_chartData.BackgroundRules)},
            legend: {{
                labels: {{
                    usePointStyle: true,
                    pointStyle: 'round',
                    fontSize: 15,
                    padding: 40,
                    fontFamily: 'Raleway',
                }}
            }},
            tooltips: {{
                backgroundColor: '#F7BB29',
                bodyFontColor: '#000',
                titleFontColor: '#000',
                caretPadding: 10,
                xPadding: 10,
                yPadding: 10,
                mode: 'nearest',
                position: 'nearest',
                titleFontSize: 15,
                bodyFontSize: 15,
                titleFontFamily: 'Raleway',
                bodyFontFamily: 'Raleway',
                titleSpacing: 8,
                bodySpacing: 8,
                displayColors: false,
                callbacks: {{
                    title: function(tooltipItem, data)
{{
    title = data.datasets[tooltipItem[0].datasetIndex].label + ' : ' + tooltipItem[0].yLabel + ' (' + tooltipItem[0].xLabel + ' )';
    return title;
}},
                    label: function(tooltipItem, data)
{{
    var dataArray = [];
    if (data.tooltipLabels == undefined || data.tooltipLabels == null)
        return dataArray;
    var tooltipLabels = data.tooltipLabels;
    var tooltipData = data.tooltipDataSet[tooltipItem.datasetIndex].DataList[tooltipItem.index];
    var size = tooltipLabels != undefined ? tooltipLabels.length : 0;
    for (var i = 0; i < size; i++)
    {{
        var item = '\u27A4 ' + tooltipLabels[i] + ' : ' + tooltipData[i] + ' ';
        dataArray.push(item);
    }}
    return dataArray;
}},
                }}
            }},
            animation: {{
                duration: 1000,
                onComplete: function()
{{
    if ({_chartData.IsPatientComparativeData.ToBooleanString()})
    {{
        var chartInstance = this.chart;
        var ctx = chartInstance.ctx;
        ctx.textAlign = 'left';
        ctx.font = '14px Open Sans';
        ctx.fillStyle = '#fff';
        Chart.helpers.each(this.data.datasets.forEach(function(dataset, i) {{
            var meta = chartInstance.controller.getDatasetMeta(i);
            Chart.helpers.each(meta.data.forEach(function(bar, index) {{
                if ({_chartData.IsPatientComparativeWithProfessionalData.ToBooleanString()} && {_chartData.HasPatientData.ToBooleanString()})
                {{
                    if (i === 0 && $.inArray(bar._model.label, {JsonConvert.SerializeObject(_chartData.PatientData.Labels)}) > -1) {{
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 10);
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 25);
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 40);
                    }}
                }}
                else
                {{
                    if ({_chartData.HasPatientData.ToBooleanString()} && bar._model.datasetLabel === {JsonConvert.SerializeObject(_chartData.PatientData.LabelNames)}[index] &&
                        bar._model.label === {JsonConvert.SerializeObject(_chartData.PatientData.Labels)}[index])
                    {{
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 10);
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 25);
                        ctx.fillText('\u25CF', bar._model.x - 5, bar._model.y + 40);
                    }}
                }}
            }}),
                                this);
        }}),
                            this);
    }}
}}
            }},
            hover: {{
                animationDuration: 0
            }}
        }};
        if ({_chartData.IsPatientProgressGraph.ToBooleanString()} != undefined && {_chartData.IsPatientProgressGraph.ToBooleanString()})
        {{
            if ({_chartData.SurveyQuestionMaxScore} != undefined && {_chartData.SurveyQuestionMaxScore} > 0)
            {{
                options.scales.yAxes[0].ticks.max = {_chartData.SurveyQuestionMaxScore};
            }}            
            options.scales.xAxes[0].gridLines.display = false;
        }}";

            return chartOptions;
        }

        private string GenerateChartPlugins(ChartDataModel _chartData)
        {
            string plugins = $@"replaceQuoteStart[{{
                beforeDraw: function (chart) {{
                    if ({_chartData.HasBackgroundRules.ToBooleanString()} != undefined && {_chartData.HasBackgroundRules.ToBooleanString()}) {{
                        var ctx = chart.chart.ctx;
                        var ruleIndex = 0;
                        var rules = chart.chart.options.backgroundRules;
                        var yaxis = chart.chart.scales['y-axis-1'];
                        var xaxis = chart.chart.scales['x-axis-1'];
            for (var i = 0; i < rules.length; i++)
            {{
                var yRangeBeginPixel = yaxis.getPixelForValue(rules[i].YaxisSegementStart);
                var yRangeEndPixel = yaxis.getPixelForValue(rules[i].YaxisSegementEnd);
                var width = chart.chart.width,
                height = chart.chart.height,
                ctx = chart.chart.ctx;
                ctx.restore();
                var fontSize = 16;
                ctx.font = 'bold ' + fontSize + 'px sans-serif';
                ctx.textBaseline = 'middle';
                ctx.beginPath();
                var xPos = xaxis.left;
                var yPos = Math.min(yRangeBeginPixel, yRangeEndPixel);
                var recWidth = xaxis.right - xaxis.left;
                var recHeight = Math.max(yRangeBeginPixel, yRangeEndPixel) - Math.min(yRangeBeginPixel, yRangeEndPixel);
                ctx.rect(xPos, yPos, recWidth, recHeight);
                ctx.fillStyle = rules[i].BackgroundColor;
                ctx.fill();
                ctx.fillStyle = '#585551';
                ctx.fillText(rules[i].LabelText, xPos + (recWidth / 2), yPos + (recHeight / 2));
                ctx.save();
            }}
        }}
    }}
}}]replaceQuoteEnd";
            plugins = plugins.Replace("\r\n", "");
            return plugins;
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

            foreach (var data in chartDataModel.Series)
            {
                models.Add(new SeriesDataModel
                {
                    data = data.DataList,
                    label = data.LabelName,
                    backgroundColor = data.GraphProperty.backgroundColor,
                    borderWidth = data.GraphProperty.pointBorderWidth,
                    fill = data.GraphProperty.fill,
                    pointRadius = data.GraphProperty.pointRadius,
                    pointHoverRadius = data.GraphProperty.pointRadius,
                    borderColor = data.GraphProperty.backgroundColor,
                });
            }
            return models;
        }
    }
}
