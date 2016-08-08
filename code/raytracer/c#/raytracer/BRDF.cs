using System;
using System.Collections.Generic;
using static raytracer.LinearAlgebra;

namespace raytracer
{
  class BRDF
  {
    public static float constant(List<float> n, List<float> l, List<float> v, Dictionary<string, float> parameters)
    {
      return 1.0f;
    }

    public static float lambert(List<float> n, List<float> l, List<float> v, Dictionary<string, float> parameters)
    {
      return Math.Max(0.0f, Dot(n, l));
    }

    public static float blinnPhong(List<float> n, List<float> l, List<float> v, Dictionary<string, float> parameters)
    {
      var h = Normalize(Add(v, l));
      return (float)Math.Pow(Math.Max(0.0f, Dot(n,h)), parameters["shininess"]);
    }
  }
}
