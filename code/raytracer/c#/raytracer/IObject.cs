using System;
using System.Collections.Generic;

namespace raytracer
{
  interface IObject
  {
    List<string> Materials { get; set; }
    Tuple<float, IObject> Intersect(Ray ray);
    Vector GetNormal(Vector p, float time);
    Vector GetTextureCoords(Vector p, float time);
  }
}
