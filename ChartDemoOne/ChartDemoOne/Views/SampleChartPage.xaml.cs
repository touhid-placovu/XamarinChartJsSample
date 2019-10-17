using ChartDemoOne.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChartDemoOne.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SampleChartPage : ContentPage
	{
        readonly ChartReportPageViewModel _viewModel;

        public SampleChartPage ()
		{
			InitializeComponent ();
            _viewModel = new ChartReportPageViewModel("line");
            this.BindingContext = _viewModel;           
           
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();


            _viewModel.LoadChartDataCommand.Execute(null);










            //var html = new HtmlWebViewSource
            //{
            //    Html = viewModel.ReportHtmlC
            //};
            //var hybridWebView = new WebView
            //{
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    VerticalOptions = LayoutOptions.FillAndExpand
            //};
            //hybridWebView.Source = html;
        }
    }
}