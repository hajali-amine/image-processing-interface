using image_processor.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace image_processor.io
{
    class ImageIO
    {
        public static Image Read(string filepath)
        {
            string type = ImageIO.ReadType(filepath);
            Image image = null;

            if (type.Equals(string.Empty))
            {
                throw new Exception("unknown type");
            }
            else if (type.Equals("P2"))
            {
                image = ImageIO.ReadPgmAsciiType(filepath);
            }
            else if (type.Equals("P5"))
            {
                image = ImageIO.ReadPgmBinaryType(filepath);
            }

            return image;
        }

        public static string ReadType(string filepath)
        {
            using (StreamReader file = new StreamReader(filepath))
            {
                string type = file.ReadLine();
                if (!type.Contains("P2") && !type.Contains("P5"))
                {
                    type = string.Empty;
                }
                file.Close();
                return type.Length > 2 ? type.Substring(0, 2) : type;
            }
        }

        public static Image ReadPgmAsciiType(string filepath)
        {
            using (StreamReader file = new StreamReader(filepath))
            {
                _ = file.ReadLine();
                string potentialComment = file.ReadLine();
                while (potentialComment[0] == '#')
                {
                    potentialComment = file.ReadLine();
                }

                int width = Convert.ToInt32(potentialComment.Trim().Split(" ")[0]);
                int height = Convert.ToInt32(potentialComment.Trim().Split(" ")[1]);
                int grayscale = Convert.ToInt32(file.ReadLine().Trim()) + 1;

                Image image = new Image(height: height, width: width, grayscale: grayscale);
                
                List<int> intValues = new List<int>();
                foreach (string line in file.ReadToEnd().Split("\n"))
                {
                    if (line.Equals(string.Empty))
                    {
                        break;
                    }

                    string[] values = line.Trim().Split(" ");
                    foreach (string pixel in values)
                    {
                        intValues.Add(Convert.ToInt32(pixel));
                    }
                    image.Pixels.Add(new List<int>(intValues));
                    _ = intValues.RemoveAll(x => true);
                }

                file.Close();

                return image.IsValid ? image : new Image(0, 0, 0);
            }
        }
        
        public static Image ReadPgmBinaryType(string filepath)
        {
            using (StreamReader file = new StreamReader(filepath, encoding: Encoding.UTF8))
            {
                // We'll need this later to reposition the stream to the pixels data
                StringBuilder dataPriorToPixelsValue = new StringBuilder();

                dataPriorToPixelsValue.Append(file.ReadLine());
                string potentialComment = file.ReadLine();
                dataPriorToPixelsValue.Append(potentialComment);
                while (potentialComment[0] == '#')
                {
                    potentialComment = file.ReadLine();
                    dataPriorToPixelsValue.Append(potentialComment);
                }

                int width = Convert.ToInt32(potentialComment.Trim().Split(" ")[0]);
                int height = Convert.ToInt32(potentialComment.Trim().Split(" ")[1]);
                int grayscale = Convert.ToInt32(file.ReadLine().Trim()) + 1;

                dataPriorToPixelsValue.Append(grayscale.ToString() + "\n");

                Image image = new Image(height: height, width: width, grayscale: grayscale);

                Stream fileBaseStream = file.BaseStream;
                // Stream starts after metadata - This actually took me some time to understand the root of why the image doesn't start from the right position :'(
                fileBaseStream.Position = dataPriorToPixelsValue.ToString().Length;
                for (int i = 0; i < height; i++)
                {
                    image.Pixels.Add(new List<int>());
                    for (int j = 0; j < width; j++)
                    {
                        int value = fileBaseStream.ReadByte();
                        image.Pixels[i].Add(value >= 0 ? value : 0);
                    }
                }

                file.Close();

                return image.IsValid ? image : new Image(0, 0, 0);
            }
        }

        public static async Task WriteAsync(string filepath, Image image)
        {
            StringBuilder file = new StringBuilder();
            _ = file.Append("P2\n");
            _ = file.Append(string.Format("# Image modified using Amine Haj Ali and Chaima Akkari's tool on {0}\n", DateTime.Now.ToString(format: "dd-MM-yy hh:mm:ss")));
            _ = file.Append("# This tool is highly inspired by Melek Elloumi's tool - https://github.com/MelekElloumi/Image-Processing-Tool\n");
            _ = file.Append(string.Format("{0} {1}\n", image.Width, image.Height));
            _ = file.Append(string.Format("{0}\n", image.Grayscale - 1));
            foreach (List<int> row in image.Pixels)
            {
                foreach (int pixel in row)
                {
                    _ = file.Append(string.Format("{0} ", pixel));
                }

                _ = file.Append("\n");
            }

            await File.WriteAllTextAsync(filepath, file.ToString());
        }
    }
}
