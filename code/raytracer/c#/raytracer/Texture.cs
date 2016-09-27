using System;
using System.Drawing;

namespace raytracer
{
  enum TextureFiltering
  {
    NearestNeighbor = 0,
    Bilinear
  }

  interface ITexture
  {
    string Name { get; set; }
    Vector GetColor(Vector coords, TextureFiltering filteringMode);
  }

  class ColorTexture : ITexture
  {
    public Bitmap Image { get; set; }
    public string Name { get; set; }

    public Vector GetColor(Vector coords, TextureFiltering filteringMode)
    {
      switch (filteringMode)
      {
        case TextureFiltering.NearestNeighbor:
          return GetNearestNeighborColor(coords);
        case TextureFiltering.Bilinear:
          return GetBilinearColor(coords);
        default:
          throw new ArgumentOutOfRangeException(nameof(filteringMode), filteringMode, null);
      }
    }

    public Vector GetNearestNeighborColor(Vector coords)
    {
      var x = (int)(coords.X*(Image.Width - 1) + 0.5);
      var y = (int)((1 - coords.Y)*(Image.Height- 1) + 0.5);
      return ColorToVector(Image.GetPixel(x, y));
    }

    public Vector GetBilinearColor(Vector coords)
    {
      var x = (coords.X * (Image.Width - 1));
      var y = ((1 - coords.Y) * (Image.Height - 1));
      var i = (int) x;
      var j = (int) y;
      var di = x - i;
      var dj = y - j;
      var colorIJ = ColorToVector(Image.GetPixel(i, j));
      var colorI1J = ColorToVector(Image.GetPixel(Math.Min(Image.Width - 1, i + 1), j));
      var colorIJ1 = ColorToVector(Image.GetPixel(i, Math.Min(Image.Height - 1, j + 1)));
      var colorI1J1 = ColorToVector(Image.GetPixel(Math.Min(Image.Width - 1, i + 1), Math.Min(Image.Height - 1, j + 1)));

      var colorA = (1.0f - di)*colorIJ + di*colorI1J;
      var colorB = (1.0f - di)*colorIJ1 + di*colorI1J1;
      return (1.0f - dj)*colorA + dj*colorB;
    }

    private static Vector ColorToVector(Color color)
    {
      return new Vector(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
    }
  }
}