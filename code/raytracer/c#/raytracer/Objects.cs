using System;
using System.Collections.Generic;

namespace raytracer
{
  interface IObject
  {
    List<float> Position { get; set; }
    List<string> Materials { get; set; }
    float Intersect(Ray ray);
  }

  class Sphere : IObject
  {
    public float Radius { get; set; }
    public List<float> Position { get; set; }
    public List<string> Materials { get; set; }
    Sphere(float radius, List<float> position, List<string> materials)
    {
      Radius = radius;
      Position = position;
      Materials = materials;
    }

    // Checks intersection between ray and specific sphere
    public float Intersect(Ray ray)
    {
      float a = LinearAlgebra.Dot(ray.Direction, ray.Direction);
      float b = 2 * LinearAlgebra.Dot(LinearAlgebra.Sub(ray.Position, Position), ray.Direction);
      float c = LinearAlgebra.Dot(LinearAlgebra.Sub(ray.Position, Position), LinearAlgebra.Sub(ray.Position, Position)) - Radius * Radius;

      var discr = b * b - 4 * a * c;
      if (discr < 0.0)
      {
        return float.PositiveInfinity;
      }

      discr = (float)Math.Sqrt(discr);
      var t0 = (-b - discr) / (2 * a);
      var t1 = (-b + discr) / (2 * a);

      var tMin = Math.Min(t0, t1);
      if (tMin < 0.0)
      {
        return float.PositiveInfinity;
      }

      return tMin;
    }
  }


}
