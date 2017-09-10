using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace raytracer
{
  class Display
  {
    public static void GenerateImage(Vector[,] imageData, int width, int height, string imageFile, int bitResolution = 8, bool dither = false)
    {
      var image = new Bitmap(width, height);
      var multiplier = (int)Math.Pow(2, bitResolution) - 1;

      for (int j = 0; j < height; j++)
      {
        for (int i = 0; i < width; i++)
        {
          var color = imageData[i, height - j - 1];
          color = color.Clamped;
          int[] rgb = dither ? Dither(color, multiplier, i, height - j - 1, imageData) : Quantize(color, multiplier);
            
          image.SetPixel(i, j, Color.FromArgb(rgb[0], rgb[1], rgb[2]));
        }
      }
      image.Save(imageFile, ImageFormat.Png);
    }

    public static int[] Quantize(Vector color, int multiplier)
    {
      var rgb = new int[3];
      for (int i = 0; i < 3; i++)
      {
        rgb[i] = (int)Math.Round(color[i] * multiplier);
        rgb[i] = (255 * rgb[i]) / multiplier;
      }
      return rgb;
    }

    public static int[] Dither(Vector color, int multiplier, int x, int y, Vector[,] imageData)
    {
      var rgb = new int[3];
      var quantError = new float[3];
      for (int i = 0; i < 3; i++)
      {
        rgb[i] = (int)Math.Round(color[i] * multiplier);
        rgb[i] = (255 * rgb[i]) / multiplier;
        quantError[i] = ((float)(rgb[i])) / 255.0f - color[i];
        if(x < imageData.GetLength(0) - 1)
          imageData[x + 1, y][i] = imageData[x + 1, y][i] + quantError[i] * 7.0f / 16.0f;
        if (x > 0 && y > 0)
          imageData[x - 1, y - 1][i] = imageData[x - 1, y - 1][i] + quantError[i] * 3.0f / 16.0f;
        if (y > 0)
          imageData[x, y - 1][i] = imageData[x, y - 1][i] + quantError[i] * 5.0f / 16.0f;
        if (x < imageData.GetLength(0) - 1 && y > 0)
          imageData[x + 1, y - 1][i] = imageData[x + 1, y - 1][i] + quantError[i] * 1.0f / 16.0f;
      }
      return rgb;
    }
  }
}
