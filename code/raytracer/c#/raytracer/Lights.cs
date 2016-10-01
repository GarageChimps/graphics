using System;
using System.Collections.Generic;

namespace raytracer
{
  interface ILight
  {
    Vector Color { get; set; }
  }

  interface IShadingLight : ILight
  {
    Vector GetDirection(Vector p);
    Vector GetSampledDirection(Vector p, Random sampler);
    float GetDistance(Vector p);
  }

  class PointLight : IShadingLight
  {
    public Vector Position { get; set; }
    public Vector Color { get; set; }
    public float LightSize { get; set; }
    public Vector LightNormal { get; set; }
    public PointLight(Vector position, Vector color, float lightSize, Vector lightNormal)
    {
      Position = position;
      Color = color;
      LightSize = lightSize;
      LightNormal = lightNormal;
    }

    public Vector GetSampledDirection(Vector p, Random sampler)
    {
      var d = (Position - p).Normalized;
      if (LightNormal.Size == 0)
        return d;
      var perp = Vector.Perpendicular(LightNormal);
      var perp2 = (LightNormal % perp).Normalized;

      var uRand = LightSize * ((float)sampler.NextDouble() - 1.0f);
      var vRand = LightSize * ((float)sampler.NextDouble() - 1.0f);

      return (Position + uRand * perp + vRand * perp2 - p).Normalized;
    }

    public Vector GetDirection(Vector p)
    {
      return (Position - p).Normalized;
    }

    public float GetDistance(Vector p)
    {
      return (Position - p).Size;
    }
  }


  class DirectionalLight : IShadingLight
  {
    public Vector Direction { get; set; }
    public Vector Color { get; set; }
    public DirectionalLight(Vector direction, Vector color)
    {
      Direction = direction;
      Color = color;
    }

    public Vector GetSampledDirection(Vector p, Random sampler)
    {
      return Direction;
    }

    public Vector GetDirection(Vector p)
    {
      return Direction;
    }

    public float GetDistance(Vector p)
    {
      return float.PositiveInfinity;
    }
  }


  class AmbientLight : ILight
  {
    public Vector Color { get; set; }
    public AmbientLight(Vector color)
    {
      Color = color;
    }
  }
}
