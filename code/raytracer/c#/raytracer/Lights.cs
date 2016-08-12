using System.Collections.Generic;
//using static raytracer.LinearAlgebra;

namespace raytracer
{
  interface ILight
  {
    Vector Color { get; set; }
  }

  interface IShadingLight : ILight
  {
    Vector GetDirection(Vector p);
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
        //LinearAlgebra.Normalize(LinearAlgebra.Sub(Position, p));
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
