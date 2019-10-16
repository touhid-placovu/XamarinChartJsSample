namespace ChartDemoOne.Models
{
    public class GraphProperty
    {
		public GraphProperty()
		{
            //ctr
        }
        public string backgroundColor { get; set; }
        public bool fill { get; set; }
        public int pointRadius { get; set; }
        public string pointBorderColor { get; set; }
        public string pointBackgroundColor { get; set; }
        public int pointBorderWidth { get; set; }
        public int lineTension { get; set; }
        public string borderColor { get; set; }
        public int graphPropertyFor { get; set; }
    }
}
