using System;
using System.Collections.Generic;
using System.Text;

namespace image_processor.data
{
    class Filters
    {
        public static List<List<double>> AverageFilter(int size)
        {
            if (size % 2 == 0)
            {
                size++;
            }

            List<List<double>> filter = new List<List<double>>();
            for (int i = 0; i < size; i++)
            {
                filter.Add(new List<double>());
                for (int j = 0; j < size; j++)
                {
                    filter[i].Add((double)1 / (size * size));
                }
            }
            return filter;
        }

        public static List<List<double>> GaussianFilter(int size)
        {
            if (size % 2 == 0)
            {
                size++;
            }

            int center = size / 2;
            double sum = 0;
            List<List<double>> filter = new List<List<double>>();
            for (int i = -center; i < center + 1; i++)
            {
                filter.Add(new List<double>());
                for (int j = -center; j < center + 1; j++)
                {
                    filter[center + i].Add(center * center - Math.Abs(i) - Math.Abs(j));
                    sum += filter[center + i][center + j];
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    filter[i][j] /= sum;
                }
            }

            return filter;
        }

        public static List<List<double>> LaplacianFilter()
        {
            List<List<double>> filter = new List<List<double>>();
            filter.Add(new List<double>(new double[] { 0, -1, 0 }));
            filter.Add(new List<double>(new double[] { -1, 9, -1 }));
            filter.Add(new List<double>(new double[] { 0, -1, 0 }));
            return filter;
        }

        public static data.Image MedianFilter(data.Image image, int size)
        {
            if (size % 2 == 0)
            {
                size++;
            }

            Image newImage = new Image(image.Height, image.Width, image.Grayscale);
            for (int i = 0; i < image.Height; i++)
            {
                newImage.Pixels.Add(new List<int>());
                for (int j = 0; j < image.Width; j++)
                {
                    List<int> median = new List<int>();
                    for (int py = -size / 2; py < size / 2 + 1; py++)
                    {
                        for (int px = -size / 2; px < size / 2 + 1; px++)
                        {
                            median.Add(image.Pixels[Math.Min(Math.Max(0, py + i), image.Height - 1)][Math.Min(Math.Max(0, px + j), image.Width - 1)]);
                        }
                    }
                    median.Sort();
                    newImage.Pixels[i].Add(median[size * size / 2]);
                }
            }

            return newImage;
        }

        public static List<List<double>> HorizontalFilter(int size)
        {
            if (size % 2 == 0)
            {
                size++;
            }

            List<List<double>> filter = new List<List<double>>();
            for (int i = 0; i < size; i++)
            {
                filter.Add(new List<double>());
                for (int j = -size / 2; j < size / 2 + 1; j++)
                {
                    filter[i].Add(j);
                }
            }

            return filter;
        }

        public static List<List<double>> VerticalFilter(int size)
        {
            if (size % 2 == 0)
            {
                size++;
            }

            List<List<double>> filter = new List<List<double>>();

            for (int j = -size / 2; j < size / 2 + 1; j++)
            {
                filter.Add(new List<double>());
                for (int i = 0; i < size; i++)
                {
                    filter[j + size / 2].Add(j);
                }
            }

            return filter;
        }

        public static Image PrewittFilter(data.Image image, int size)
        {
            if (size % 2 == 0)
            {
                size++;
            }

            Image newImage = new Image(image.Height, image.Width, image.Grayscale);

            Image horizontalPrewitt = image.Convolute(Filters.HorizontalFilter(size));
            Image verticalPrewitt = image.Convolute(Filters.VerticalFilter(size));

            for (int i = 0; i < image.Height; i++)
            {
                newImage.Pixels.Add(new List<int>());
                for (int j = 0; j < image.Width; j++)
                {
                    double module = Math.Sqrt(
                        horizontalPrewitt.Pixels[i][j] * horizontalPrewitt.Pixels[i][j] + verticalPrewitt.Pixels[i][j] * verticalPrewitt.Pixels[i][j]
                        );
                    newImage.Pixels[i].Add(Math.Max(0, Math.Min((int)module, image.Grayscale - 1)));
                }
            }

            return newImage;
        }
    }
}
