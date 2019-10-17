using System;
using System.Collections.Generic;
using System.Text;

namespace ChartDemoOne.Models
{
    public class ChartGenerator
    {
        private ChartConfiguration chartConfiguration;
        public ChartGenerator(ChartDataModel chartData,string chartType)
        {
            chartConfiguration = new ChartConfiguration(chartData, chartType);            
        }
        public string GenerateChart()
        {
            return chartConfiguration.GenerateChart();
        }
    }
}
