using System;
using System.Linq;

namespace raytracer
{
  class Matrix
  {
    public float[,] Items { get; set; }

    public Matrix()
    {
      Items = new float[3, 3];
    }

    public float this[int i, int j]
    {
      get { return Items[i, j]; }
      set { Items[i, j] = value; }
    }

    public Vector GetRow(int i)
    {
      return new Vector(this[i, 0], this[i, 1], this[i, 2]);
    }

    public void SetRow(int i, Vector v)
    {
      this[i, 0] = v[0];
      this[i, 1] = v[1];
      this[i, 2] = v[2];
    }

    public Vector GetColumn(int j)
    {
      return new Vector(this[0, j], this[1, j], this[2, j]);
    }

    public void SetColumn(int j, Vector v)
    {
      this[0, j] = v[0];
      this[1, j] = v[1];
      this[2, j] = v[2];
    }

    public static Matrix I()
    {
      var result = new Matrix();
      Enumerable.Range(0, 3).ToList().ForEach(i => result[i,i] = 1);
      return result;
    }

    public static Matrix operator +(Matrix matrix1, Matrix matrix2)
    {
      var result = new Matrix();
      Enumerable.Range(0, 3).ToList().ForEach(i => Enumerable.Range(0, 3).ToList().ForEach(j => result[i,j] = matrix1[i,j] + matrix2[i, j]));
      return result;
    }

    public static Matrix operator -(Matrix matrix1, Matrix matrix2)
    {
      var result = new Matrix();
      Enumerable.Range(0, 3).ToList().ForEach(i => Enumerable.Range(0, 3).ToList().ForEach(j => result[i, j] = matrix1[i, j] - matrix2[i, j]));
      return result;
    }

    public static Vector operator *(Matrix matrix, Vector vector)
    {
      var result = new Vector(matrix.GetRow(0) ^vector, matrix.GetRow(1) ^ vector, matrix.GetRow(2) ^ vector);
      return result;
    }

    public static Matrix operator *(Matrix matrix1, Matrix matrix2)
    {
      var result = new Matrix();
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 3; j++)
        {
          result[i, j] = matrix1.GetRow(i) ^ matrix2.GetColumn(j);
        }
      }
      return result;
    }

    public static Matrix RotationX(float angle)
    {
      var matrix = I();
      matrix[1, 1] = (float)Math.Cos(angle);
      matrix[2, 2] = (float)Math.Cos(angle);
      matrix[1, 2] = -(float)Math.Sin(angle);
      matrix[2, 1] = (float)Math.Sin(angle);
      return matrix;
    }

    public static Matrix RotationY(float angle)
    {
      var matrix = I();
      matrix[0, 0] = (float)Math.Cos(angle);
      matrix[2, 2] = (float)Math.Cos(angle);
      matrix[0, 2] = (float)Math.Sin(angle);
      matrix[2, 0] = -(float)Math.Sin(angle);
      return matrix;
    }

    public static Matrix RotationZ(float angle)
    {
      var matrix = I();
      matrix[0, 0] = (float)Math.Cos(angle);
      matrix[1, 1] = (float)Math.Cos(angle);
      matrix[0, 1] = -(float)Math.Sin(angle);
      matrix[1, 0] = (float)Math.Sin(angle);
      return matrix;
    }

  }
}
