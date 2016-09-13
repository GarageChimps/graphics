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
    float GetDistance(Vector p);
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
