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

    /// <summary>
    /// Gives this vector, with components clamped inside the interval [0, 1]
    /// </summary>
    public Vector Clamped
    {
      get 
      {
        Vector clamped = this;

        if (clamped.R > 1.0f)
          clamped.R = 1.0f;
        else if (clamped.R < 0.0f)
          clamped.R = 0.0f;
        else if (float.IsNaN(clamped.R))
          clamped.R = 0.0f;

        if (clamped.G > 1.0f)
          clamped.G = 1.0f;
        else if (clamped.G < 0.0f)
          clamped.G = 0.0f;
        else if (float.IsNaN(clamped.G))
          clamped.G = 0.0f;

        if (clamped.B > 1.0f)
          clamped.B = 1.0f;
        else if (clamped.B < 0.0f)
          clamped.B = 0.0f;
        else if (float.IsNaN(clamped.B))
          clamped.B = 0.0f;

        return clamped;
      }
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

    public float this[int i]
    {
      get
      {
        if (i == 0)
          return R;
        else if (i == 1)
          return G;
        else if (i == 2)
          return B;
        else
          throw new Exception("Invalid Vector coordinate index");
      }
      set
      {
        if (i == 0)
          R = value;
        else if (i == 1)
          G = value;
        else if (i == 2)
          B = value;
        else
          throw new Exception("Invalid Vector coordinate index");
      }
    }

    public Vector(float R, float G, float B)
    {
      this.R = R;
      this.G = G;
      this.B = B;
    }

    public Vector(List<float> coords)
    {
      if (coords.Count != 3)
        throw new Exception("Vector: incorrect size of coords list");

      this.R = coords[0];
      this.G = coords[1];
      this.B = coords[2];
    }

    /// <summary>
    /// Gives the result of adding the two vectors component by component.
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static Vector operator +(Vector vector1, Vector vector2)
    {
      return new Vector(vector1.R + vector2.R, vector1.G + vector2.G, vector1.B + vector2.B);
    }

    /// <summary>
    /// Gives the result of adding a scalar to every component of the vector
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    /// <returns></returns>
    public static Vector operator +(Vector vector, float scalar)
    {
      return new Vector(vector.R + scalar, vector.G + scalar, vector.B + scalar);
    }

    /// <summary>
    /// Gives the result of adding a scalar to every component of the vector
    /// </summary>
    /// <param name="scalar"></param>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector operator +(float scalar, Vector vector)
    {
      return new Vector(vector.R + scalar, vector.G + scalar, vector.B + scalar);
    }

    /// <summary>
    /// Gives the result of subtracting the two vectors component by component.
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static Vector operator -(Vector vector1, Vector vector2)
    {
      return new Vector(vector1.R - vector2.R, vector1.G - vector2.G, vector1.B - vector2.B);
    }

    /// <summary>
    /// Gives the result of subtracting a scalar to every component of the vector
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    /// <returns></returns>
    public static Vector operator -(Vector vector, float scalar)
    {
      return new Vector(vector.R - scalar, vector.G - scalar, vector.B - scalar);
    }

    /// <summary>
    /// Gives the result of subtracting a one-valued vector by another vector
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    /// <returns></returns>
    public static Vector operator -(float scalar, Vector vector)
    {
      return new Vector(scalar - vector.R, scalar - vector.G, scalar - vector.B);
    }

    /// <summary>
    /// Gives the result of multiplying the two vectors component by component
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static Vector operator *(Vector vector1, Vector vector2)
    {
      return new Vector(vector1.R * vector2.R, vector1.G * vector2.G, vector1.B * vector2.B);
    }
    
    /// <summary>
    /// Gives the result of multiplying the vector by a scalar
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    /// <returns></returns>
    public static Vector operator *(Vector vector, float scalar)
    {
      return new Vector(vector.R * scalar, vector.G * scalar, vector.B * scalar);
    }

    /// <summary>
    /// Gives the result of multiplying the vector by a scalar
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    /// <returns></returns>
    public static Vector operator *(float scalar, Vector vector)
    {
      return vector * scalar;
    }

    /// <summary>
    /// Gives the result of dividing the two vectors component by component
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static Vector operator /(Vector vector1, Vector vector2)
    {
      return new Vector(vector1.R / vector2.R, vector1.G / vector2.G, vector1.B / vector2.B);
    }

    /// <summary>
    /// Gives the result of dividing a vector by a scalar
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="scalar"></param>
    /// <returns></returns>
    public static Vector operator /(Vector vector, float scalar)
    {
      return new Vector(vector.R / scalar, vector.G / scalar, vector.B / scalar);
    }

    /// <summary>
    /// Gives the result of dividing a one-valued vector by another vector
    /// </summary>
    /// <param name="scalar"></param>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector operator /(float scalar, Vector vector)
    {
      return new Vector(scalar / vector.R, scalar / vector.G, scalar / vector.B);
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

    public static Vector Perpendicular(Vector v)
    {
      var other = new Vector();
      if(Math.Abs(v.X) >= Math.Abs(v.Y) && Math.Abs(v.X) >= Math.Abs(v.Z))
        other = new Vector(0,1,0);
      else if (Math.Abs(v.Y) >= Math.Abs(v.X) && Math.Abs(v.Y) >= Math.Abs(v.Z))
        other = new Vector(0, 0, 1);
      else
        other = new Vector(1,0,0);
      return (v%other).Normalized;
    }

  }
}
