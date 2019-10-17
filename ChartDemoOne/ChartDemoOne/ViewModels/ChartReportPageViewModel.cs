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
        private string chartType="line";
        public ChartDataModel ChartDataModel = null;
        private PatientChartDataService patientChartDataService;
        public Command LoadChartDataCommand { get; set; }
        public ChartReportPageViewModel(string chartType)
        {
            this.chartType = chartType;
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
                    BuildReportHtml(chartType);
                    //await Task.Yield();
                }
                else
                {
                    BuildReportHtml(chartType);
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
            ChartGenerator chartGenerator = new ChartGenerator(ChartDataModel, chartType);
            ReportHtml =  chartGenerator.GenerateChart();
        }
    }
}
