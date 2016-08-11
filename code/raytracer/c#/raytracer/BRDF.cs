using System;
using System.Collections.Generic;
//using static raytracer.LinearAlgebra;

namespace raytracer
{
  class BRDF
  {
    public static Dictionary<string, Func<List<float>, List<float>, List<float>, Dictionary<string, float>, float>> GetBRDFDictionary()
    {
      var BRDFDictionary =
        new Dictionary<string, Func<List<float>, List<float>, List<float>, Dictionary<string, float>, float>>();
      BRDFDictionary.Add("constant", constant);
      BRDFDictionary.Add("lambert", lambert);
      BRDFDictionary.Add("blinnPhong", blinnPhong);
      return BRDFDictionary;
    }


    public static float constant(List<float> n, List<float> l, List<float> v, Dictionary<string, float> parameters)
    {
      return 1.0f;
    }

    public static float lambert(List<float> n, List<float> l, List<float> v, Dictionary<string, float> parameters)
    {
      return Math.Max(0.0f, LinearAlgebra.Dot(n, l));
    }

    public static float blinnPhong(List<float> n, List<float> l, List<float> v, Dictionary<string, float> parameters)
    {
      var h = LinearAlgebra.Normalize(LinearAlgebra.Add(v, l));
      return (float)Math.Pow(Math.Max(0.0f, LinearAlgebra.Dot(n, h)), parameters["shininess"]);
    }
  }
}
