using System;
using System.Collections.Generic;
using static raytracer.LinearAlgebra;

namespace raytracer
{
  class CameraBounds
  {
    public float Top { get; set; }
    public float Bottom { get; set; }
    public float Right { get; set; }
    public float Left { get; set; }
  }

  class Camera
  {
    public float FOV { get; set; }
    public float Near { get; set; }
    public List<float> Position { get; set; }
    public List<float> Target { get; set; }
    public List<float> Up { get; set; }
    public CameraBounds Bounds { get; set; }
    public List<List<float>> CameraBasis { get; set; }

    public Camera(float fov, List<float>  position, List<float>  up, List<float>  target, float near=0.1f)
    {
      FOV = fov;
      Near = near;
      Position = position;
      Target = target;
      Up = up;
      CameraBasis = GetCameraCoordinatesBasis();

    }

    // Returns the left,right,top,bottom bounds of the near plane in camera space
    public void SetCameraBounds(int width, int height)
    {
      var t = (float) (Math.Abs(Near) * Math.Tan(((FOV / 2.0f) / 180.0f) * Math.PI));
      var b = -t;
      var r = t * width / height;
      var l = -r;
      Bounds = new CameraBounds {Top= t, Bottom= b, Right= r, Left= l};
    }

    // Transforms pixel coordinate from image space to camera space
    public List<float> PixelToCameraCoords(int i, int j, int width, int height)
    {
      var u = Bounds.Left + (Bounds.Right - Bounds.Left) * (i + 0.5f) / width;
      var v = Bounds.Bottom + (Bounds.Top - Bounds.Bottom) * (j + 0.5f) / height;
      var w = -Near;
      return new List<float> { u, v, w};
    }

    // Gets camera space coordinate basis
    public List<List<float>> GetCameraCoordinatesBasis()
    {
      var w = Normalize(Sub(Position, Target));
      var u = Normalize(Cross(Up, w));
      var v = Normalize(Cross(w, u));
      return new List<List<float>> { u, v, w};
    }

    // Convert vector in camera space (cameraCoords) into world space for camera
    public List<float> CameraToWorldCoords(List<float>  cameraCoords)
    {
      var worldCoords = Position;
      for (int i = 0; i < 3; i++)
      {
        worldCoords = Add(worldCoords, MultScalar(cameraCoords[i], CameraBasis[i]));
      }
      return worldCoords;
    }
  }
}
