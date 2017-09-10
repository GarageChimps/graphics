namespace raytracer
{
  class Transform
  {
    public Transform(Vector translation, Vector scaling, Vector rotation)
    {
      Translation = translation;
      Scaling = scaling;
      Rotation = rotation;
      RotationMatrix = Matrix.RotationZ(Rotation[2]) * Matrix.RotationY(Rotation[1]) * Matrix.RotationX(Rotation[0]);
    }

    private Vector Translation { get; }
    private Vector Scaling { get; }
    private Vector Rotation { get; }

    private Matrix RotationMatrix { get; }

    public Vector TransformPosition(Vector v)
    {
      return RotationMatrix * (Scaling * v) + Translation;
    }

    public Vector TransformDirection(Vector v)
    {
      return RotationMatrix * v;
    }
  }
}
