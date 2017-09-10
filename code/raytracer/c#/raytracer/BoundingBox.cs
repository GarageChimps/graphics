using System;
using System.Collections.Generic;
using System.Linq;

namespace raytracer
{
  class Box
  {
    public Box(Vector min, Vector max)
    {
      Min = min;
      Max = max;
    }

    public Vector Min { get; set; }
    public Vector Max { get; set; }

    public void Update(Vector p)
    {
      for (int i = 0; i < 3; i++)
      {
        if (p[i] < Min[i])
          Min[i] = p[i];
        if (p[i] > Max[i])
          Max[i] = p[i];
      }
      
    }

    public List<Box> OctPartition()
    {
      List<Vector> corners = GetCorners();

      var mid = corners.Aggregate((v, acc) => acc + v) / corners.Count;
      var v04 = (corners[0] + corners[4]) / 2;
      var v01 = (corners[0] + corners[1]) / 2;
      var v56 = (corners[5] + corners[6]) / 2;
      var v03 = (corners[0] + corners[3]) / 2;
      var v67 = (corners[6] + corners[7]) / 2;
      var v26 = (corners[2] + corners[6]) / 2;
      var v4567 = (corners[4] + corners[5] + corners[6] + corners[7]) / 4;
      var v1256 = (corners[1] + corners[2] + corners[5] + corners[6]) / 4;
      var v0145 = (corners[0] + corners[1] + corners[4] + corners[5]) / 4;
      var v2367 = (corners[2] + corners[3] + corners[6] + corners[7]) / 4;
      var v0347 = (corners[0] + corners[3] + corners[4] + corners[7]) / 4;
      var v0123 = (corners[0] + corners[1] + corners[2] + corners[3]) / 4;

      var boxes = new List<Box>();
      boxes.Add(new Box(corners[0], mid));
      boxes.Add(new Box(v04, v4567));
      boxes.Add(new Box(v01, v1256));
      boxes.Add(new Box(v0145, v56));

      boxes.Add(new Box(v03, v2367));
      boxes.Add(new Box(v0347, v67));
      boxes.Add(new Box(v0123, v26));
      boxes.Add(new Box(mid, corners[6]));

      return boxes;
    }

    public List<Vector> GetCorners()
    {
      var corners = new List<Vector>();
      corners.Add(new Vector(Min.X, Min.Y, Min.Z));
      corners.Add(new Vector(Min.X, Min.Y, Max.Z));
      corners.Add(new Vector(Min.X, Max.Y, Max.Z));
      corners.Add(new Vector(Min.X, Max.Y, Min.Z));

      corners.Add(new Vector(Max.X, Min.Y, Min.Z));
      corners.Add(new Vector(Max.X, Min.Y, Max.Z));
      corners.Add(new Vector(Max.X, Max.Y, Max.Z));
      corners.Add(new Vector(Max.X, Max.Y, Min.Z));
      return corners;
    }

    public bool HasPoint(Vector p)
    {
      if (p.X < Min.X) return false;
      if (p.X > Max.X) return false;
      if (p.Y < Min.Y) return false;
      if (p.Y > Max.Y) return false;
      if (p.Z < Min.Z) return false;
      if (p.Z > Max.Z) return false;
      return true;
    }

    public Tuple<float, bool> TestIntersection(Ray ray)
    {
      float tmin = (Min.X - ray.Position.X) / ray.Direction.X;
      float tmax = (Max.X - ray.Position.X) / ray.Direction.X;

      if (tmin > tmax)
      {
        var temp = tmin;
        tmin = tmax;
        tmax = temp;
      }

      float tymin = (Min.Y - ray.Position.Y) / ray.Direction.Y;
      float tymax = (Max.Y - ray.Position.Y) / ray.Direction.Y;

      if (tymin > tymax)
      {
        var temp = tymin;
        tymin = tymax;
        tymax = temp;
      }

      if ((tmin > tymax) || (tymin > tmax))
        return new Tuple<float, bool>(tmin, false);

      if (tymin > tmin)
        tmin = tymin;

      if (tymax < tmax)
        tmax = tymax;

      float tzmin = (Min.Z - ray.Position.Z) / ray.Direction.Z;
      float tzmax = (Max.Z - ray.Position.Z) / ray.Direction.Z;

      if (tzmin > tzmax)
      {
        var temp = tzmin;
        tzmin = tzmax;
        tzmax = temp;
      }

      if ((tmin > tzmax) || (tzmin > tmax))
        return new Tuple<float, bool>(tmin, false);

      if (tzmin > tmin)
        tmin = tzmin;

      if (tzmax < tmax)
        tmax = tzmax;

      return new Tuple<float, bool>(tmin, true);
    }

    public List<Vector> GetAxes()
    {
      return new List<Vector>() {new Vector(1, 0, 0), new Vector(0, 1, 0), new Vector(0, 0, 1)};
    } 
  }
}
