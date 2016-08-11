using System.Collections.Generic;
//using static raytracer.LinearAlgebra;

namespace raytracer
{
  interface ILight
  {
    List<float> Color { get; set; }
  }

  interface IShadingLight : ILight
  {
    List<float> GetDirection(List<float> p);
  }

  class PointLight : IShadingLight
  {
    public List<float> Position { get; set; }
    public List<float> Color { get; set; }
    public PointLight(List<float> position, List<float> color)
    {
      Position = position;
      Color = color;
    }

    public List<float> GetDirection(List<float> p)
    {
      return LinearAlgebra.Normalize(LinearAlgebra.Sub(Position, p));
    }
  }


  class DirectionalLight : IShadingLight
  {
    public List<float> Direction { get; set; }
    public List<float> Color { get; set; }
    public DirectionalLight(List<float> direction, List<float> color)
    {
      Direction = direction;
      Color = color;
    }

    public List<float> GetDirection(List<float> p)
    {
      return Direction;
    }
  }


  class AmbientLight : ILight
  {
    public List<float> Color { get; set; }
    public AmbientLight(List<float> color)
    {
      Color = color;
    }
  }
}
