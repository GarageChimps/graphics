using System;
using System.Collections.Generic;

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
    public float LensSize { get; set; }
    public float Exposure { get; set; }
    public Vector Position { get; set; }
    public Vector Target { get; set; }
    public Vector Up { get; set; }
    public CameraBounds Bounds { get; set; }
    public List<Vector> CameraBasis { get; set; }

    public Camera(float fov, Vector position, Vector up, Vector target, float near=0.1f, float lensSize=0.0f, float exposure=0.0f)
    {
      FOV = fov;
      Near = near;
      LensSize = lensSize;
      Exposure = exposure;
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
    public Vector PixelToCameraCoords(float i, float j, int width, int height)
    {
      float u = Bounds.Left + (Bounds.Right - Bounds.Left) * i / width;
      float v = Bounds.Bottom + (Bounds.Top - Bounds.Bottom) * j / height;
      float w = -Near;
      return new Vector( u, v, w );
    }

    // Gets camera space coordinate basis
    public List<Vector> GetCameraCoordinatesBasis()
    {
      Vector w = (Position - Target).Normalized;
      Vector u = (Up % w).Normalized;
      Vector v = (w % u).Normalized;
      return new List<Vector> { u, v, w };
    }

    // Convert vector in camera space (cameraCoords) into world space for camera
    public Vector CameraToWorldCoords(Vector  cameraCoords)
    {
      var worldCoords = Position;
      for (int i = 0; i < 3; i++)
      {
        worldCoords = worldCoords + cameraCoords[i] * CameraBasis[i];
      }
      return worldCoords;
    }

    public Vector SampleCameraPosition(Random sampler)
    {
      var uRand = LensSize * ((float)sampler.NextDouble() - 1.0f);
      var vRand = LensSize * ((float)sampler.NextDouble() - 1.0f);
      return Position + uRand* CameraBasis[0] + vRand* CameraBasis[1];
    }
  }
}
