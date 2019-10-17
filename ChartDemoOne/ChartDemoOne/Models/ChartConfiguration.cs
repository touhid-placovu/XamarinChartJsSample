using ChartDemoOne.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChartDemoOne.Models
{
    public class ChartConfiguration
    {
        private ChartDataModel _chartData;
        private string _chartType;
        private string chartConfigScript;
        public ChartConfiguration(ChartDataModel chartData,string chartType)
        {
            _chartData = chartData;
            _chartType = chartType;
            ApplyConfiguration();
        }        
        public string GenerateChart()
        {
            var chartConfigJsScript = $"<script type='text/javascript'>{chartConfigScript}</script>";
            var localScript = "<script src='file:///android_asset/chart_bundle.js' type='text/javascript'></script>";
            var style = @"<style>*,*::before,*::after {box-sizing: border-box;} html{height:100%; width:100%;-webkit-text-size-adjust: 100%;-webkit-tap-highlight-color: rgba(0, 0, 0, 0);} body{height:100%; width:100%;overflow:hidden;line-height: 1.15;padding:0; margin:0} #mycanvas{height:100%; width:1000px;padding:0; margin:0;} .conteiner{height:100%;overflow-x:auto}</style>";
            var meta = @"<meta http-equiv=""Content-Type"" content=""text/html"" charset=""utf-8"" /><meta name=""viewport"" content=""width=device-width"" />";
            var canvas = @"<canvas id=""mycanvas""></canvas>";
            var document = $@"<html><head>{meta}{style}{localScript}</head><body><div class=""conteiner"">{canvas}</div>{chartConfigJsScript}</body></html>";
            return document;
        }        
        private void ApplyConfiguration()
        {
            var chartConfigWithData = GenerateChartData(_chartType);
            chartConfigScript = $@"var config = {chartConfigWithData};
                {GetPatientProgressGraphSettings()}
                window.onload = function() {{
                  var canvasContext = document.getElementById(""mycanvas"").getContext(""2d"");
                  new Chart(canvasContext, config);
                }};";            
        }
        private object GenerateChartData(string chartType)
        {
            var labels = _chartData.Labels.ToArray();
            var config = new
            {
                type = chartType,
                data = new
                {
                    labels = _chartData.Labels,
                    labelsValue = _chartData.LabelsValue,
                    datasets = GetSeriesDataModels(_chartData),
                    tooltipLabels = _chartData.TooltipLabels,
                    tooltipDataSet = _chartData.TooltipSeries
                },
                options = GenerateChartOptions(),
                plugins = GenerateChartPlugins()
            };            
            var jsonConfig = JsonConvert.SerializeObject(config);
            jsonConfig = RemoveExtraQuotation(jsonConfig);
            return jsonConfig;
        }
        private object GenerateChartOptions()
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
                backgroundRules = GetChartBackgroundRules(),
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
                    callbacks = GetToolTipCallBackFunction()
                },
                animation = GetChartAnimation(),
                hover = new
                {
                    animationDuration = 0
                }

            };
            return chartOptions;
        }
        private object GetChartBackgroundRules()
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
        private string RemoveExtraQuotation(string jsonConfig)
        {
            try
            {
                var data = jsonConfig.Replace("\"replaceQuoteStart", "");
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
              title: function(tooltipItem, data) {
                title =
                  data.datasets[tooltipItem[0].datasetIndex].label +
                  ' : ' +
                  tooltipItem[0].yLabel +
                  ' (' +
                  tooltipItem[0].xLabel +
                  ' )';
            return title;
        },
              label: function(tooltipItem, data)
        {
            var dataArray = [];
            if (
              data.tooltipLabels == undefined ||
              data.tooltipLabels == null
            )
                return dataArray;
            var tooltipLabels = data.tooltipLabels;
            var tooltipData =
              data.tooltipDataSet[tooltipItem.datasetIndex].DataList[
                tooltipItem.index
              ];
            var size =
              tooltipLabels != undefined ? tooltipLabels.length : 0;
            for (var i = 0; i < size; i++)
            {
                var item =
                  '➤ ' + tooltipLabels[i] + ' : ' + tooltipData[i] + ' ';
                dataArray.push(item);
            }
            return dataArray;
        }
    }
    replaceQuoteEnd");

            var data = builder.ToString().Replace("\r\n", "").Trim('"');
            return data;
        }
        private string GetChartAnimation()
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
        private string GenerateChartPlugins()
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
        private string GetPatientProgressGraphSettings()
        {
            StringBuilder builder = new StringBuilder();
            if (_chartData.IsPatientProgressGraph)
            {
                if (_chartData.SurveyQuestionMaxScore > 0)
                {
                    builder.Append($@"config.options.scales.yAxes[0].ticks.max = {_chartData.SurveyQuestionMaxScore};");
                }
                builder.Append($@"config.options.scales.xAxes[0].gridLines.display = false;");
            }
            return builder.ToString();

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
