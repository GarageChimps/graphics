using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageDiffer
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("Invalid number of parameters.");
        return;
      }
      if (!File.Exists(args[0]) || !File.Exists(args[1]))
      {
        Console.WriteLine("Image names are not valid filepaths.");
        return;
      }

      Bitmap image1, image2;
      try
      {
        image1 = (Bitmap)Bitmap.FromFile(args[0]);
        image2 = (Bitmap)Bitmap.FromFile(args[1]);
        
      }
      catch (Exception e)
      {
        Console.WriteLine("Error reading files as images: " + e.Message);
        return;
      }

      if (image1.Width != image2.Width || image1.Height != image2.Height)
      {
        Console.WriteLine("Incompatible sizes of images for comparison.");
        return;
      }

      var output = new Bitmap(image1.Width, image1.Height);
      var numberOfDifferentPixels = 0;
      for (int i = 0; i < output.Width; i++)
      {
        for (int j = 0; j < output.Height; j++)
        {
          var pix1 = image1.GetPixel(i, j);
          var pix2 = image2.GetPixel(i, j);
          var diffColor = Color.FromArgb(255, Math.Abs(pix1.R - pix2.R), Math.Abs(pix1.G - pix2.G),
            Math.Abs(pix1.B - pix2.B));
          if (!(diffColor.R == 0 && diffColor.G == 0 && diffColor.B == 0))
            numberOfDifferentPixels++;
          output.SetPixel(i,j, diffColor);
        }
      }

      var differencePercentage = (double)numberOfDifferentPixels/ (double)(output.Width*output.Height);

      output.Save("diff.png", ImageFormat.Png);
      Console.WriteLine("Number of different pixels: " + numberOfDifferentPixels);
      Console.WriteLine("Percentage of difference: " + (differencePercentage * 100) + "%");

      Console.WriteLine("<Press any key to exit>");
      Console.ReadLine();
    }
  }
}
