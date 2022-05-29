using QuickChart;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;

namespace image_processor.io
{
    class HistogramIO
    {
        // In order for this to work, you have to install Matplotlib library globally
        // We use pythonw to avoid having the windows console showing
        private const string PATH_TO_PY_EXE = "C:\\Users\\Asus TUF\\AppData\\Local\\Programs\\Python\\Python39\\pythonw.exe";
        //private const string PATH_TO_PY_EXE = "C:\\Windows\\py.exe";
        private const string PYTHON_FILE_NAME = "histogram_generator.pyw";

        public static BitmapImage GetImage(List<int> histogram)
        {
            Chart qc = new Chart();

            qc.Width = 500;
            qc.Height = 300;
            string config = @"{{type:'line',data:{{labels:{0},datasets:[{{backgroundColor:'rgba(255,99,132,0.5)',borderColor:'rgb(255,99,132)',data:{1},label:'pixels',fill: true,}},],}},options:{{scales: {{xAxes: [{{ticks: {{autoSkip: false,maxRotation: 0,}},}},],}},}},}}";

            (string index, string values) = HistogramIO.GenerateHistogramData(histogram);
            qc.Config = string.Format(config, index, values);
            // Get the URL
            Console.WriteLine(qc.GetUrl());
            using (var ms = new System.IO.MemoryStream(qc.ToByteArray()))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        public static (string, string) GenerateHistogramData(List<int> histogram)
        {
            StringBuilder values = new StringBuilder("[");
            StringBuilder index = new StringBuilder("[");
            for (int i = 0; i < histogram.Count; i+=2)
            {

                values.Append(histogram[i].ToString());
                values.Append(",");
                index.Append("\' \'" +
                    "");
                index.Append(",");
            }
            // Remove last space
            values.Remove(values.Length - 1, 1);
            index.Remove(index.Length - 1, 1);
            values.Append("]");
            index.Append("]");

            return (index.ToString(), values.ToString());
        }
    }
}
