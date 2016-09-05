﻿using System;
using System.Collections.Generic;

namespace raytracer
{
  class Ray
  {
    public Vector Position { get; set; }
    public Vector Direction { get; set; }
  }

  class Raytracer
  {
    // Main raytracing function, recieves a scene and image size, and returns a rendered image
    public static Vector[,] RayTrace(Scene scene, Resources resources, int width, int height)
    {
      scene.Camera.SetCameraBounds(width, height);
      var image = CreateImage(scene, width, height);
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
          var ray = GeneratePixelRay(scene.Camera, i, j, width, height);
          image[i, j] = IntersectAndShade(ray, scene, resources, 0);
        }
      }

      return image;
    }


    // Initialize a matrix to store the image to be rendered
    private static Vector[,] CreateImage(Scene scene, int width, int height)
    {
      var image = new Vector[width, height];
      for (int i = 0; i < width; i++)
      {
        for (int j = 0; j < height; j++)
        {
          image[i, j] = scene.GetBackgroundColor();
        }
      }
      return image;
    }


    // Generates a pixel view ray from the pixel coordinates
    private static Ray GeneratePixelRay(Camera camera, int i, int j, int width, int height)
    {
      var cameraCoords = camera.PixelToCameraCoords(i, j, width, height);
      var worldCoords = camera.CameraToWorldCoords(cameraCoords);
      var pixelDirection = (worldCoords - camera.Position).Normalized; 
      return new Ray { Position = camera.Position, Direction = pixelDirection };
    }



    // For a given ray, tests objects intersection and calculate corresponding color
    private static Vector IntersectAndShade(Ray ray, Scene scene, Resources resources, int recursion)
    {
      if (recursion > scene.GetIntParam("maxReflectionRecursions"))
      {
        return scene.GetBackgroundColor();
      }
      var intersectResult = IntersectAllObjects(ray, scene);
      var tIntersect = intersectResult.Item1;
      var objIntersected = intersectResult.Item2;
      if (tIntersect < float.PositiveInfinity)
      {
        return GetRayColor(ray, tIntersect, objIntersected, scene, resources, recursion);
      }
      return scene.GetBackgroundColor();
    }


    // Check intersection between ray and all objects of the scene
    private static Tuple<float, IObject> IntersectAllObjects(Ray ray, Scene scene)
    {
      var tMin = float.PositiveInfinity;
      IObject objIntersected = null;
      for (int index = 0; index < scene.Objects.Count; index++)
      {
        var obj = scene.Objects[index];
        var intersectResult = obj.Intersect(ray);
        var t = intersectResult.Item1;
        var o = intersectResult.Item2;
        if (t < tMin)
        {
          tMin = t;
          objIntersected = o;
        }
      }

      return new Tuple<float, IObject>(tMin, objIntersected);
    }

    // Calculates the color gathered by this ray after intersecting an object
    private static Vector GetRayColor(Ray ray, float tIntersection, IObject obj, Scene scene, Resources resources, int recursion)
    {
      Vector p = ray.Position + (tIntersection * ray.Direction);
      Vector n = obj.GetNormal(p);
      return Shade(p, n, ray.Direction, obj.Materials, scene, resources, recursion);
    }


    // Performs the shading calculation for a point, based on material reflectances and lights illumination
    private static Vector Shade(Vector p, Vector n, Vector d, List<string> materials, Scene scene, Resources resources, int recursion)
    {
      var color = new Vector ( 0, 0, 0 );
      color += GetAmbientColor(materials, scene, resources);
      color += GetShadingColor(p, n, d, materials, scene, resources, recursion);
      return color;
    }

    // Returns the ambient color generated by the ambient illumination of the scene for a material
    private static Vector GetAmbientColor(List<string> materials, Scene scene, Resources resources)
    {
      var color = new Vector ( 0, 0, 0 );
      var ambientMaterials = resources.GetAmbientMaterials(materials);

      foreach (var light in scene.GetAmbientLights())
      {
        foreach (var material in ambientMaterials)
        {
          var ambientColor = light.Color * material.Color;
          color += ambientColor;
        }
      }
      return color;
    }


    // Returns the shading color generated by the shading illumination of the scene for a material
    private static Vector GetShadingColor(Vector p, Vector n, Vector d, List<string> materials, Scene scene, Resources resources, int recursion)
    {
      var color = new Vector ( 0, 0, 0 );
      var brdfMaterials = resources.GetBrdfMaterials(materials);
      var reflectiveMaterials = resources.GetReflectiveMaterials(materials);

      var v = (scene.Camera.Position - p).Normalized;
      
      foreach (var light in scene.GetShadingLights())
      {
        var l = light.GetDirection(p);
        // Direct illumination
        if (!scene.GetBoolParam("enable_shadows") || !IsInShadow(p, l, scene))
        {
          var lightColor = light.Color;
          foreach (var material in brdfMaterials)
          {
            var brdfVal = material.BRDF(n, l, v, material.BRDFParams);
            var materialColor = lightColor * brdfVal * material.Color; 
            color += materialColor; 
          }
        }

        // Indirect illummination
        foreach (var material in reflectiveMaterials)
        {
          var reflectionRay = GetReflectionRay(p, n, d);
          var rayColor = IntersectAndShade(reflectionRay, scene, resources, recursion + 1);
          var materialColor = material.Reflectivity * rayColor; 
          color += materialColor; 
        }
      }

      return color;
    }


    //Generates a shadow ray for a given point p for light l
    private static Ray GenerateShadowRay(Vector p, Vector l)
    {
      var q = p + (0.001f * l);
      return new Ray { Position = q, Direction = l };
    }


    //Tests if a point p is in shadow for a given light l in the given scene
    private static bool IsInShadow(Vector p, Vector l, Scene scene)
    {
      var ray = GenerateShadowRay(p, l);
      var intersectResult = IntersectAllObjects(ray, scene);
      var tIntersect = intersectResult.Item1;
      return tIntersect < float.PositiveInfinity;
    }


    //Gets the reflection ray in a point p with normal n based on original viewing direction d
    private static Ray GetReflectionRay(Vector p, Vector n, Vector d)
    {
      var r = (d - (n * (d ^ n) * 2)).Normalized; 
      var q = p + (0.001f * r);
      return new Ray { Position = q, Direction = r };
    }
  }
}
