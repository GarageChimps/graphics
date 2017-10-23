using System;
using System.Collections.Generic;

namespace raytracer
{
  class Sphere : IObject
  {
    public float Radius { get; set; }
    public Vector Position { get; set; }
    public List<string> Materials { get; set; }
    public Vector Velocity { get; set; }

    public Sphere(float radius, Vector position, List<string> materials, Vector velocity)
    {
      Radius = radius;
      Position = position;
      Materials = materials;
      Velocity = velocity;
    }

    private Vector GetPosition(float t)
    {
      return Position + t*Velocity;
    }
    
    // Checks intersection between ray and specific sphere
    public Tuple<float, IObject> Intersect(Ray ray)
    {
      float a = 1.0f;//ray.Direction ^ ray.Direction;
      var subPos = (ray.Position - GetPosition(ray.Time));
      float b = 2 * ray.Direction ^ subPos; 
      float c = (subPos ^ subPos) - Radius * Radius;
      
      var discr = b * b - 4 * a * c;
      if (discr < 0.0)
      {
        return new Tuple<float, IObject> (float.PositiveInfinity, null);
      }

      discr = (float)Math.Sqrt(discr);
      var t0 = (-b - discr) / (2 * a);
      var t1 = (-b + discr) / (2 * a);

      var tMin = float.PositiveInfinity;
      if (t0 < 0.0 && t1 < 0.0)
      {
        return new Tuple<float, IObject>(float.PositiveInfinity, null);
      }
      else if (t0 < 0.0)
        tMin = t1;
      else if (t1 < 0.0)
        tMin = t0;
      else
        tMin = Math.Min(t0, t1);


      return new Tuple<float, IObject>(tMin, this);
    }

    public Vector GetNormal(Vector p, float time)
    {
      return (p - this.GetPosition(time)).Normalized;
    }

    public Vector GetTextureCoords(Vector p, float time)
    {
      var c = this.GetPosition(time);
      var theta = (float)Math.Acos((p.Y - c.Y)/Radius);
      var phi = (float)Math.Atan2(p.Z - c.Z, p.X - c.X);

      //var u = (float) (1.0 - (phi + Math.PI )/ (2.0*Math.PI));
      var u = (float) (0.5 - (phi/ (2.0*Math.PI)));
      //var v = (float) ((Math.PI - theta) / (Math.PI));
      var v = (float) (1.0 - (theta) / (Math.PI));
      return new Vector(u,v,0);
    }
  }
}