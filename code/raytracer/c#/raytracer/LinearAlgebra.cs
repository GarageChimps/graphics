using System;
using System.Collections.Generic;

namespace raytracer
{
  //Linear Algebra Lib
  class LinearAlgebra
  {
    public static List<float> Add(List<float>  vector1, List<float>  vector2)
    {
      return new List <float>{ vector1[0] + vector2[0], vector1[1] + vector2[1], vector1[2] + vector2[2]};
    }


    public static List<float> Mult(List<float> vector1, List<float> vector2)
    {
      return new List<float> { vector1[0] * vector2[0], vector1[1] * vector2[1], vector1[2] * vector2[2]};
    }


    public static List<float> MultScalar(float scalar, List<float> vector)
    {
      return new List<float> { scalar * vector[0], scalar * vector[1], scalar * vector[2]};
    }


    public static List<float> Sub(List<float> vector1, List<float> vector2)
    {
      return new List<float> { vector1[0] - vector2[0], vector1[1] - vector2[1], vector1[2] - vector2[2]};
    }


    public static float Dot(List<float> vector1, List<float> vector2)
    {
      return vector1[0] * vector2[0] + vector1[1] * vector2[1] + vector1[2] * vector2[2];
    }


    public static float Magnitude(List<float> vector1)
    {
      return (float)Math.Sqrt(LinearAlgebra.Dot(vector1, vector1));
    }


    public static List<float> Normalize(List<float> vector1)
    {
      var mag = LinearAlgebra.Magnitude(vector1);
      if (mag == 0)
      {
        return new List<float> {0, 0, 0};
      }
      return new List<float> { vector1[0] / mag, vector1[1] / mag, vector1[2] / mag};
    }


    public static List<float> Cross(List<float> vector1, List<float> vector2)
    {
      var crossVec = new List<float> { 0, 0, 0};
      crossVec[0] = vector1[1] * vector2[2] - vector2[1] * vector1[2];
      crossVec[1] = vector2[0] * vector1[2] - vector1[0] * vector2[2];
      crossVec[2] = vector1[0] * vector2[1] - vector2[0] * vector1[1];
      return crossVec;
    }
  }
}
