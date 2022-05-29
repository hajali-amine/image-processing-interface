using image_processor.data;
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
        private data.Image currentImage = null;
        public MainWindow()
        {
            InitializeComponent();
            this.InitializeSizeBox();
        }

        private void InitializeSizeBox()
        {
            this.FilterSize.Text = "Size?";
            this.FilterSize.Foreground = Brushes.DarkGray;
            this.FilterSize.GotKeyboardFocus += OnFilterSizeGotFocused;
            this.FilterSize.LostKeyboardFocus += OnFilterSizeLostFocused;

        }

        private void OnFilterChecked(object sender, RoutedEventArgs e)
        {
            if (image != null)
            {
                if (sender == this.Average)
                {
                    this.SetLoadedImage(this.image.Convolute(Filters.AverageFilter(this.GetSize())));
                }

                if (sender == this.Median)
                {
                    this.SetLoadedImage(Filters.MedianFilter(this.image, this.GetSize()));
                }

                if (sender == this.Gaussian)
                {
                    this.SetLoadedImage(this.image.Convolute(Filters.GaussianFilter(this.GetSize())));
                }

                if (sender == this.Prewitt)
                {
                    this.SetLoadedImage(Filters.PrewittFilter(this.image, this.GetSize()));
                }
                
                if (sender == this.Laplacian)
                {
                    this.SetLoadedImage(this.image.Convolute(Filters.LaplacianFilter()));
                }

                if (sender == this.None)
                {
                    this.SetLoadedImage(this.image);
                }
            }
        }

        private int GetSize()
        {
            string size = this.FilterSize.Text;
            if (size.Equals("Size?"))
            {
                return 3;
            }
            else
            {
                return Convert.ToInt32(size);
            }
        }

        private void OnFilterSizeGotFocused(object sender, RoutedEventArgs e)
        {
            if (this.FilterSize.Text == "Size?" && this.FilterSize.Foreground == Brushes.DarkGray)
            {
                this.FilterSize.FontStyle = FontStyles.Normal;
                this.FilterSize.Text = string.Empty;
                this.FilterSize.Foreground = Brushes.Black;
            }
        }

        private void OnFilterSizeLostFocused(object sender, RoutedEventArgs e)
        {
            if (this.FilterSize.Text.Trim().Equals(string.Empty))
            {
                this.FilterSize.FontStyle = FontStyles.Italic;
                this.FilterSize.Text = "Size?";
                this.FilterSize.Foreground = Brushes.DarkGray;
            }
        }

        private void OnLutClicked(object sender, RoutedEventArgs e)
        {
            this.SetLoadedImage(this.image.Lut(Convert.ToDouble(Lut_a.Text), Convert.ToDouble(Lut_b.Text)));
        }

        private void SetLoadedImage(data.Image image)
        {
            this.currentImage = image;
            this.ImageLoaded.Source = image.ToBitmap();
            this.Histogram.Source = HistogramIO.GetImage(image.Histogram);
            this.StatsText.Text = image.GetStatsText();
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
                this.SetLoadedImage(image);
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
                    _ = ImageIO.WriteAsync(sv.FileName, this.currentImage);
                }
            }
        }
    }
}
