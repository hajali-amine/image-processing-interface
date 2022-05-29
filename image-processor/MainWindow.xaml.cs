using image_processor.io;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace image_processor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private data.Image image = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select a picture"
            };
            if (op.ShowDialog() == true)
            {
                this.image = ImageIO.Read(op.FileName);
                this.ImageLoaded.Source = image.ToBitmap();
                this.Histogram.Source = HistogramIO.GetImage(this.image.Histogram);
                this.StatsText.Text = this.image.GetStatsText();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.image != null)
            {
                SaveFileDialog sv = new SaveFileDialog
                {
                    Title = "Save your picture",
                    Filter = "PGM image | *.pgm"
                };
                if (sv.ShowDialog() == true)
                {
                    _ = ImageIO.WriteAsync(sv.FileName, this.image);
                }
            }
        }
    }
}
