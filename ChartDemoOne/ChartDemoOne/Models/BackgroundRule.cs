namespace ChartDemoOne.Models
{
    public class BackgroundRule
    {
		public BackgroundRule()
		{
			//ctr
		}
        public string BackgroundColor { get; set; }
        public double YaxisSegementStart { get; set; }
        public double YaxisSegementEnd { get; set; }
        public string LabelText { get; set; }
    }
}
