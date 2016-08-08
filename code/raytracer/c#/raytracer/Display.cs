﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace raytracer
{
  class Display
  {
    public static void GenerateImage(List<float>[,] imageData, int width, int height)
    {
      var image = new Bitmap(width, height);
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
          var color = imageData[i, j];
          image.SetPixel(i, j, Color.FromArgb(((int)color[0] * 255),
            ((int)color[2] * 255), ((int)color[2] * 255)));
        }
      }
      image.Save("image.png", ImageFormat.Png);
    }
  }
}
