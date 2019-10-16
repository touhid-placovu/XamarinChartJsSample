using ChartDemoOne.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace ChartDemoOne.Serrvice
{
    public class PatientChartDataService
    {
        private string PatientChartUrl = "https://ontrack-healthdemo.com/webapi/v3/api/Account/GetTempGraphReportData";
        private HttpClient InitializeHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;
            return httpClient;
        }
        public async Task<ChartDataModel> GetChartDataModel()
        {
            ChartDataModel chartData = null;
            try
            {
                
                using (var httpClient = InitializeHttpClient())
                {
                    var response = await httpClient.GetAsync(PatientChartUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        chartData = JsonConvert.DeserializeObject<ChartDataModel>(result);
                        if (chartData.PatientData == null)
                        {
                            chartData.PatientData = new PatientDataModel();
                        }
                    }
                }               
            }
            catch (Exception ex)
            {
                Log.Warning("Error: ", ex.ToString());
            }
            return chartData;
        }
    }
}
