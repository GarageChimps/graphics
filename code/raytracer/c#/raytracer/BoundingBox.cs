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

    public bool TestIntersection(Ray ray)
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
        return false;

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
        return false;

      if (tzmin > tmin)
        tmin = tzmin;

      if (tzmax < tmax)
        tmax = tzmax;

      return true;
    }
  }
}
