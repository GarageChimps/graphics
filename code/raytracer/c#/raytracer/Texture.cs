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
      var x = (int)(coords.X*(Image.Width - 1));
      var y = (int)((1 - coords.Y)*(Image.Height- 1));
      var color = Image.GetPixel(x, y);
      return new Vector(color.R/255.0f, color.G / 255.0f, color.B / 255.0f);
    }

    public Vector GetBilinearColor(Vector coords)
    {
      throw new System.NotImplementedException();
    }
  }
}