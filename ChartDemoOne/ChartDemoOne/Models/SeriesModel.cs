using System.Collections.Generic;

namespace ChartDemoOne.Models
{
    public class SeriesModel
    {
		public SeriesModel()
		{
            //ctr
        }
        public string LabelName { get; set; }
        public List<double> DataList { get; set; }
        public GraphProperty GraphProperty { get; set; }
        public object Type { get; set; }
        public object YAxisId { get; set; }
    }


    public class SeriesDataModel
    {
        public string label { get; set; }
        public List<double> data { get; set; }
        public string borderColor { get; set; }
        public string backgroundColor { get; set; }
        public bool fill { get; set; }
        public int pointRadius { get; set; }
        public int borderWidth { get; set; }
        public int pointHoverRadius { get; set; }
    }
}
