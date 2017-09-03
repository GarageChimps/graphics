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
    bool ReachesPoint(Vector p);
  }
  
  class PointLight : IShadingLight
  {
    public Vector Position { get; set; }
    public Vector Color { get; set; }
    public PointLight(Vector position, Vector color)
    {
      Position = position;
      Color = color;
    }

    public virtual Vector GetSampledDirection(Vector p, Random sampler)
    {
      return GetDirection(p);
    }

    public Vector GetDirection(Vector p)
    {
      return (Position - p).Normalized;
    }

    public float GetDistance(Vector p)
    {
      return (Position - p).Size;
    }

    public virtual bool ReachesPoint(Vector p)
    {
      return true;
    }
  }

  class AreaLight : PointLight
  {
    public float SizeA { get; set; }
    public float SizeB { get; set; }
    public Vector DirA { get; set; }
    public Vector DirB { get; set; }

    public AreaLight(Vector position, Vector color, float sizeA, float sizeB, Vector dirA, Vector dirB) : 
      base(position, color)
    {
      SizeA = sizeA;
      SizeB = sizeB;
      DirA = dirA;
      DirB = dirB;
    }

    public override Vector GetSampledDirection(Vector p, Random sampler)
    {
      var aRand = SizeA * ((float)sampler.NextDouble() - 0.5f);
      var bRand = SizeB  * ((float)sampler.NextDouble() - 0.5f);

      return (Position + aRand * DirA + bRand * DirB - p).Normalized;
    }
  }

  class SpotLight : PointLight
  {
    public Vector Direction { get; set; }
    public float Angle { get; set; }

    public SpotLight(Vector position, Vector color, Vector direction, float angle) : base(position, color)
    {
      Direction = direction.Normalized;
      Angle = angle;
    }

    public override bool ReachesPoint(Vector p)
    {
      var l = (Position - p).Normalized;
      var lDotD = -1*l ^ Direction;
      return lDotD > Math.Cos(Angle/2.0f);
    }
  }


  class DirectionalLight : IShadingLight
  {
    public Vector Direction { get; set; }
    public Vector Color { get; set; }
    public DirectionalLight(Vector direction, Vector color)
    {
      Direction = direction.Normalized;
      Color = color;
    }

    public Vector GetSampledDirection(Vector p, Random sampler)
    {
      return -1f * Direction;
    }

    public Vector GetDirection(Vector p)
    {
      return -1f * Direction;
    }

    public float GetDistance(Vector p)
    {
      return float.PositiveInfinity;
    }

    public bool ReachesPoint(Vector p)
    {
      return true;
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
