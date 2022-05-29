using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace image_processor.data
{
    class Image
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Grayscale { get; set; }
        public List<List<int>> Pixels { get; set; }

        public int Length { get { return this.Width * this.Height; } }
        public bool IsValid { get { return this.Pixels.Count != 0 && this.Height != 0 && this.Width != 0 && this.Pixels.Count == this.Height && this.Pixels[0].Count == this.Width; } }
        public double Avg
        {
            get { 
                int sum = 0;
                for (int i = 0; i < this.Height; i++)
                {
                    for (int j = 0; j < this.Width; j++)
                    {
                        sum += this.Pixels[i][j];
                    }
                }

                return (double) sum / this.Length;
            }
        }
        public double Std
        {
            get
            {
                double avg = this.Avg;
                double dev = 0;
                for (int i = 0; i < this.Height; i++)
                {
                    for (int j = 0; j < this.Width; j++)
                    {
                        dev += (this.Pixels[i][j] - avg) * (this.Pixels[i][j] - avg);
                    }
                }

                return Math.Sqrt(dev / this.Length);
            }
        }
        public List<int> Histogram
        {
            get
            {
                List<int> h = new List<int>(new int[this.Grayscale]);
                for (int i = 0; i < this.Height; i++)
                {
                    for (int j = 0; j < this.Width; j++)
                    {
                        h[this.Pixels[i][j]] += 1;
                    }
                }

                return h;
            }
        }
        public List<int> CumulatedHistogram
        {
            get
            {
                List<int> h = this.Histogram;
                for (int i = 0; i < this.Grayscale; i++)
                {
                    h[i] += h[i - 1];
                }

                return h;
            }
        }
        public double Entropy
        {
            get
            {
                List<int> h = this.Histogram;
                double ent = 0;
                for (int i = 0; i < this.Grayscale; i++)
                {
                    double proba = h[i] / this.Length;
                    if (proba != 0)
                    {
                        ent += proba * Math.Log2(1 / proba);
                    }
                }

                return ent;
            }
        }
        public (int, int) Dynamic
        {
            get
            {
                List<int> h = this.Histogram;
                (int dmin, int dmax) = (0, this.Grayscale - 1);
                for (int i = 0; i < this.Grayscale; i++)
                {
                    if (h[i] != 0)
                    {
                        dmin = i;
                        break;
                    }
                }
                for (int i = this.Grayscale - 1; i > 0; i--)
                {
                    if (h[i] != 0)
                    {
                        dmax = i;
                        break;
                    }
                }

                return (dmin, dmax);
            }
        }

        public Image(int height, int width, int grayscale)
        {
            this.Height = height;
            this.Width = width;
            this.Grayscale = grayscale;
            this.Pixels = new List<List<int>>();
        }

        public BitmapImage ToBitmap()
        {
            Bitmap bitmap = new Bitmap(this.Width, this.Height);
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    int a = this.Pixels[i][j];
                    int pixelIn8Bits = (256 / this.Grayscale) * this.Pixels[i][j];
                    Color newColor = Color.FromArgb(pixelIn8Bits, pixelIn8Bits, pixelIn8Bits);
                    bitmap.SetPixel(j, i, newColor);
                }
            }

            MemoryStream ms = new MemoryStream();  // no using here! BitmapImage will dispose the stream after loading
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            return bitmapImage;
        }
        public string GetStatsText()
        {
            return string.Format("width={0},height={1},grayscale={2},nbPixels={3},avg={4:0.##},std={5:0.##},dynamic=[{6},{7}]", this.Width, this.Height, this.Grayscale - 1, this.Length, this.Avg, this.Std, this.Dynamic.Item1, this.Dynamic.Item2);
        }

    }
}
