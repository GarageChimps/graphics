using System;
using System.Collections.Generic;

namespace raytracer
{
  struct Vector
  {
    /// <summary>
    /// Magnitude of the vector
    /// </summary>
    public float Size
    {
      get { return (float)Math.Sqrt(SizeSq); }
    }

    /// <summary>
    /// Size squared for fast magnitude comparison
    /// </summary>
    public float SizeSq
    {
      get { return this ^ this; }
    }

    /// <summary>
    /// A normalized version of this vector
    /// </summary>
    public Vector Normalized
    {
      get { return this / Size; }
    }

    public float R, G, B;

    public float X
    {
      get { return R; }
      set { R = value; }
    }
    public float Y
    {
      get { return G; }
      set { G = value; }
    }
    public float Z
    {
      get { return B; }
      set { B = value; }
    }

    Vector(float R, float G, float B)
    {
      this.R = R;
      this.G = G;
      this.B = B;
    }

    public static Vector operator +(Vector vector1, Vector vector2)
    {
      return new Vector(vector1.R + vector2.R, vector1.G + vector2.G, vector1.B + vector2.B);
    }

    public static Vector operator +(Vector vector1, float scalar)
    {
      return new Vector(vector1.R + scalar, vector1.G + scalar, vector1.B + scalar);
    }

    public static Vector operator -(Vector vector1, Vector vector2)
    {
      return new Vector(vector1.R - vector2.R, vector1.G - vector2.G, vector1.B - vector2.B);
    }

    public static Vector operator -(Vector vector1, float scalar)
    {
      return new Vector(vector1.R - scalar, vector1.G - scalar, vector1.B - scalar);
    }

    public static Vector operator *(Vector vector1, Vector vector2)
    {
      return new Vector(vector1.R * vector2.R, vector1.G * vector2.G, vector1.B * vector2.B);
    }

    public static Vector operator *(Vector vector1, float scalar)
    {
      return new Vector(vector1.R * scalar, vector1.G * scalar, vector1.B * scalar);
    }

    public static Vector operator /(Vector vector1, Vector vector2)
    {
      return new Vector(vector1.R / vector2.R, vector1.G / vector2.G, vector1.B / vector2.B);
    }
    
    public static Vector operator /(Vector vector1, float scalar)
    {
      return new Vector(vector1.R / scalar, vector1.G / scalar, vector1.B / scalar);
    }

    /// <summary>
    /// Dot product between 2 vectors
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static float operator ^(Vector vector1, Vector vector2)
    {
      return vector1.R * vector2.R + vector1.G * vector2.G + vector1.B * vector2.B;
    }

    /// <summary>
    /// Cross product between 2 vectors
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static Vector operator %(Vector vector1, Vector vector2)
    {
      float R = vector1.G * vector2.B - vector2.G * vector1.B;
      float G = vector2.R * vector1.B - vector1.R * vector2.B;
      float B = vector1.R * vector2.G - vector2.R * vector1.G;

      return new Vector(R, G, B);
    }

  }

  //Linear Algebra Lib
  static class LinearAlgebra
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
