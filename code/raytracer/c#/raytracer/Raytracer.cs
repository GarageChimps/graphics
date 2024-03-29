﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace raytracer
{
  class Ray
  {
    public Vector Position { get; set; }
    public Vector Direction { get; set; }
    public float Time { get; set; }
  }

  class Raytracer
  {
    private static Random _sampler = new Random();
    // Main raytracing function, recieves a scene and image size, and returns a rendered image
    public static Vector[,] RayTrace(Scene scene, Resources resources, int width, int height)
    {
      scene.Camera.SetCameraBounds(width, height);
      var image = CreateImage(scene, width, height);

      var currentTime = DateTime.Now.Ticks;

      Enumerable.Range(0, width).ToList().ForEach((i) => {
      //Enumerable.Range(0, width).ToList().AsParallel().ForAll((i) => {
        for (int j = 0; j < height; j++)
        {
          var samplesPerPixel = scene.GetSamplesPerPixel();
          var sqrtSamples = (int)(Math.Sqrt(samplesPerPixel));
          var deltaI = 1.0f / (sqrtSamples + 1);
          var deltaJ = 1.0f / (sqrtSamples + 1);
          var pixelColor = new Vector();
          for (float ii = i + deltaI, iSamples = 0; iSamples < sqrtSamples; ii+=deltaI, iSamples += 1)
          {
            for (float jj = j + deltaJ, jSamples = 0; jSamples < sqrtSamples; jj+=deltaJ, jSamples += 1)
            {
              var ray = GeneratePixelRay(scene.Camera, ii, jj, width, height);
              var sampleColor = IntersectAndShade(ray, scene, resources, 0);
              pixelColor += sampleColor;
            }
          }
          image[i, j] = pixelColor / samplesPerPixel;
        }
      });

      var totalTime = TimeSpan.FromTicks(DateTime.Now.Ticks - currentTime).TotalSeconds;
      Console.WriteLine("Raytrace time: " + totalTime + " s");
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
    private static Ray GeneratePixelRay(Camera camera, float i, float j, int width, int height)
    {
      var cameraCoords = camera.PixelToCameraCoords(i, j, width, height);
      var worldCoords = camera.CameraToWorldCoords(cameraCoords);
      var origin = camera.SampleCameraPosition(_sampler);
      var pixelDirection = (worldCoords - origin).Normalized;
      var time = camera.SampleTime(_sampler);
      return new Ray { Position = origin, Direction = pixelDirection, Time = time };
    }



    // For a given ray, tests objects intersection and calculate corresponding color
    private static Vector IntersectAndShade(Ray ray, Scene scene, Resources resources, int recursion)
    {
      if (recursion > scene.GetMaxNumberOfReflections())
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
      Vector n = obj.GetNormal(p, ray.Time);
      Vector t = obj.GetTextureCoords(p, ray.Time);
      return Shade(p, n, t, ray.Direction, obj.Materials, scene, resources, tIntersection, recursion, ray.Time);
    }


    // Performs the shading calculation for a point, based on material reflectances and lights illumination
    private static Vector Shade(Vector p, Vector n, Vector texCoords, Vector d, List<string> materials, Scene scene, Resources resources, float tIntersection, int recursion, float time)
    {
      var color = new Vector ( 0, 0, 0 );
      color += GetAmbientColor(texCoords, materials, scene, resources);
      color += GetShadingColor(p, n, texCoords,  d, materials, scene, resources, tIntersection, recursion, time);
      return color;
    }

    // Returns the ambient color generated by the ambient illumination of the scene for a material
    private static Vector GetAmbientColor(Vector texCoords, List<string> materials, Scene scene, Resources resources)
    {
      var color = new Vector ( 0, 0, 0 );
      var ambientMaterials = resources.GetAmbientMaterials(materials);

      foreach (var light in scene.GetAmbientLights())
      {
        foreach (var material in ambientMaterials)
        {
          var ambientColor = light.Color * material.GetColor(texCoords);
          color += ambientColor;
        }
      }
      return color;
    }


    // Returns the shading color generated by the shading illumination of the scene for a material
    private static Vector GetShadingColor(Vector p, Vector n, Vector texCoords, Vector d, List<string> materials, Scene scene, Resources resources, float tIntersection, int recursion, float time)
    {
      var color = new Vector(0, 0, 0);
      var brdfMaterials = resources.GetBrdfMaterials(materials);
      var reflectiveMaterials = resources.GetReflectiveMaterials(materials);
      var dielectricMaterials = resources.GetDielectricMaterials(materials);

      var v = (scene.Camera.Position - p).Normalized;
      color = GetDirectIlluminationColor(p, n, texCoords, scene, time, color, brdfMaterials, v);
      color = GetReflectiveRefractiveIlluminationColor(p, n, d, scene, resources, tIntersection, recursion, time, color, reflectiveMaterials, dielectricMaterials);
      if(scene.GetSamplesPerPixel() > 1)
        color = GetIndirectIlluminationColor(p, n, materials, scene, resources, color, recursion, time);
      return color;
    }

    private static Vector GetIndirectIlluminationColor(Vector p, Vector n, List<string> materials, Scene scene, Resources resources, Vector color, int recursion, float time)
    {
      var ambientMaterial = resources.GetAmbientMaterials(materials).First();
      var tuple = GetDiffuseReflectionRay(p, n);
      var diffuseReflectionRay = tuple.Item1;
      var cosTheta = tuple.Item2;
      diffuseReflectionRay.Time = time;
      var rayColor = IntersectAndShade(diffuseReflectionRay, scene, resources, recursion + 1);
      var normalizingFactor = ((float) scene.GetSamplesPerPixel()*(1.0f/(2.0f*(float) Math.PI)));
      var materialColor = cosTheta * ambientMaterial.Color * rayColor; //Divide by the number of samples per pixel
      color += materialColor;
      return color;
    }

    private static Tuple<Ray, float> GetDiffuseReflectionRay(Vector p, Vector n)
    {
      var tuple = GetDiffuseSampleDirection(n);
      var d = tuple.Item1;
      var cosTheta = tuple.Item2;
      var q = p + 0.001f * d;
      return new Tuple<Ray, float>(new  Ray { Position = q, Direction = d }, cosTheta);
    }

    private static Tuple<Vector, float> GetDiffuseSampleDirection(Vector n)
    {
      var y = (float)_sampler.NextDouble();
      var sinTheta = (float) Math.Sqrt(1 - y*y);
      var phi = (float)_sampler.NextDouble()*Math.PI*2;
      var sampleDirection = new Vector(sinTheta * (float)Math.Cos(phi), y, sinTheta * (float)Math.Sin(phi));
      return new Tuple<Vector, float>(TransformDirectionToNormalSpace(sampleDirection, n), y);
    }

    private static Vector TransformDirectionToNormalSpace(Vector sampleDirection, Vector n)
    {
      var nt = new Vector(n.Z, 0, -n.X).Normalized;
      var nb = (n % nt).Normalized;

      var matrix = new Matrix();
      matrix.SetColumn(0, nt);
      matrix.SetColumn(1, n);
      matrix.SetColumn(2, nb);

      return (matrix*sampleDirection).Normalized;
    }

    private static Vector GetReflectiveRefractiveIlluminationColor(Vector p, Vector n, Vector d, Scene scene, Resources resources, float tIntersection, int recursion, float time, Vector color, IEnumerable<ReflectiveMaterial> reflectiveMaterials, IEnumerable<DielectricMaterial> dielectricMaterials)
    {
      foreach (var material in reflectiveMaterials)
      {
        var reflectionRay = GetReflectionRay(p, n, d, material.GlossyFactor);
        reflectionRay.Time = time;
        var rayColor = IntersectAndShade(reflectionRay, scene, resources, recursion + 1);
        var materialColor = material.Color * rayColor;
        color += materialColor;
      }

      foreach (var material in dielectricMaterials)
      {
        var dielectricRays = GetDielectricRays(p, n, d, tIntersection, material, scene);
        foreach (var rayTuple in dielectricRays)
        {
          var rayAttenuation = rayTuple.Item1;
          var ray = rayTuple.Item2;
          ray.Time = time;
          var rayColor = IntersectAndShade(ray, scene, resources, recursion + 1);
          var materialColor = rayAttenuation * material.Color * rayColor;
          color += materialColor;
        }

      }

      return color;
    }

    private static Vector GetDirectIlluminationColor(Vector p, Vector n, Vector texCoords, Scene scene, float time, Vector color, IEnumerable<BRDFMaterial> brdfMaterials, Vector v)
    {
      foreach (var light in scene.GetShadingLights())
      {
        var sl = light.GetSampledDirection(p, _sampler);
        var l = light.GetDirection(p);
        var dl = light.GetDistance(p);
        // Direct illumination
        var shadowTest = !scene.GetBoolParam("enable_shadows") || !IsInShadow(p, sl, dl, scene, time);
        var reachTest = light.ReachesPoint(p);
        if (shadowTest && reachTest)
        {
          var lightColor = light.Color;
          foreach (var material in brdfMaterials)
          {
            var brdfVal = material.BRDF(n, l, v, material.BRDFParams);
            var materialColor = lightColor * brdfVal * material.GetColor(texCoords);
            color += materialColor;
          }
        }
      }

      return color;
    }


    //Generates a shadow ray for a given point p for light l
    private static Ray GenerateShadowRay(Vector p, Vector l)
    {
      var q = p + 0.001f * l;
      return new Ray { Position = q, Direction = l };
    }


    //Tests if a point p is in shadow for a given light l in the given scene
    private static bool IsInShadow(Vector p, Vector l, float distanteToLight, Scene scene, float time)
    {
      var ray = GenerateShadowRay(p, l);
      ray.Time = time;
      var intersectResult = IntersectAllObjects(ray, scene);
      var tIntersect = intersectResult.Item1;
      var inShadow = tIntersect < distanteToLight;
      return inShadow;
    }


    //Gets the reflection ray in a point p with normal n based on original viewing direction d
    private static Ray GetReflectionRay(Vector p, Vector n, Vector d, float glossyFactor=0.0f)
    {
      var perp1 = Vector.Perpendicular(n);
      var perp2 = (n % perp1).Normalized;
      var uRand = glossyFactor * ((float)_sampler.NextDouble() - 1.0f);
      var vRand = glossyFactor * ((float)_sampler.NextDouble() - 1.0f);
      var pertN = (n + uRand * perp1 + vRand * perp2).Normalized;
      var r = (d - (pertN * (d ^ pertN) * 2)).Normalized; 
      var q = p + (0.001f * r);
      return new Ray { Position = q, Direction = r };
    }

    //Gets the reflection ray in a point p with normal n based on original viewing direction d
    private static Ray GetTransmisionRay(Vector p, Vector t)
    {
      var q = p + (0.001f * t);
      return new Ray { Position = q, Direction = t };
    }

    private static Vector GetTransmisionDirection(Vector n, Vector d, float refIndexIncoming, float  refIndexOutgoing)
    {
      float dDotN = d ^ n;
      float a = 1 - (refIndexIncoming*refIndexIncoming/(refIndexOutgoing*refIndexOutgoing))*(1 - dDotN*dDotN);
      //Total internal reflection
      if (a < 0)
        return null;
      return ((refIndexIncoming/refIndexOutgoing)*(d - (n*(dDotN))) - (float)(Math.Sqrt(a))*n).Normalized;
    }

    private static List<Tuple<Vector, Ray>> GetDielectricRays(Vector p, Vector n, Vector d, float tIntersection, DielectricMaterial material, Scene scene)
    {
      var rays = new List<Tuple<Vector, Ray>>();
      float cosine = 0.0f;
      float r0 = 0.0f;
      Vector transmisionDirection = new Vector();
      Vector beerFactor = new Vector();
      bool totalInternalReflection = false;
      //The ray is entering the object
      if ((d ^ n) < 0)
      {
        cosine = -(d ^ n);
        beerFactor = new Vector(1, 1, 1);
        var t = GetTransmisionDirection(n, d, scene.GetRefractionIndex(), material.RefractionIndex);
        if (t == null)
          totalInternalReflection = true;
        else
        {
          transmisionDirection = t;
          
        }
        
      }
      else
      {
        var t = GetTransmisionDirection(-1.0f * n, d, material.RefractionIndex, scene.GetRefractionIndex());
        beerFactor = new Vector((float)Math.Exp(-material.Attenuation.R * tIntersection), (float)Math.Exp(-material.Attenuation.G * tIntersection), (float)Math.Exp(-material.Attenuation.B * tIntersection));
        if (t == null)
          totalInternalReflection = true;
        else
        {
          transmisionDirection = t;
          cosine = (transmisionDirection ^ n);
        }
      }
      if (totalInternalReflection)
      {
        rays.Add(new Tuple<Vector, Ray>(beerFactor, GetReflectionRay(p, n, d)));
      }
      else
      {
        r0 = (material.RefractionIndex - scene.GetRefractionIndex()) / 
          (material.RefractionIndex + scene.GetRefractionIndex());
        r0 = r0 * r0;
        float r = r0 + (1 - r0) * (float)Math.Pow(1 - cosine, 5);
        rays.Add(new Tuple<Vector, Ray>((1 - r) * beerFactor, GetTransmisionRay(p, transmisionDirection)));
        rays.Add(new Tuple<Vector, Ray>(r * beerFactor, GetReflectionRay(p, n, d)));
      }
      return rays;


    }
  }
}
