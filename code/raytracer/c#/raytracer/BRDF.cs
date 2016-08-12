﻿using System;
using System.Collections.Generic;
//using static raytracer.LinearAlgebra;

namespace raytracer
{
  class BRDF
  {
    public static Dictionary<string, Func<Vector, Vector, Vector, Dictionary<string, float>, float>> GetBRDFDictionary()
    {
      var BRDFDictionary =
        new Dictionary<string, Func<Vector, Vector, Vector, Dictionary<string, float>, float>>();
      BRDFDictionary.Add("constant", constant);
      BRDFDictionary.Add("lambert", lambert);
      BRDFDictionary.Add("blinnPhong", blinnPhong);
      return BRDFDictionary;
    }


    public static float constant(Vector n, Vector l, Vector v, Dictionary<string, float> parameters)
    {
      return 1.0f;
    }

    public static float lambert(Vector n, Vector l, Vector v, Dictionary<string, float> parameters)
    {
      return Math.Max(0.0f, n ^ l); 
        //return Math.Max(0.0f, LinearAlgebra.Dot(n, l)); 
    }

    public static float blinnPhong(Vector n, Vector l, Vector v, Dictionary<string, float> parameters)
    {
      Vector h = (v + l).Normalized; 
        //LinearAlgebra.Normalize(LinearAlgebra.Add(v, l));

      return (float)Math.Pow(Math.Max(0.0f, n ^ h), parameters["shininess"]);      
        //return (float)Math.Pow(Math.Max(0.0f, LinearAlgebra.Dot(n, h)), parameters["shininess"]);
    }
  }
}
