﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace raytracer
{
  class Resources
  {
    public  Dictionary< string, IMaterial> Materials { get; set; }
    Resources(List<IMaterial> materials)
    {
      Materials = new Dictionary<string, IMaterial>();
      foreach (var material in materials)
      {
        Materials[material.Name] = material;
      }
    }

    public IEnumerable<BRDFMaterial> GetBrdfMaterials(List<string> materialNames)
    {
      return Materials.Values.Where(m => materialNames.Contains(m.Name) && m is BRDFMaterial).Select(m => m).Cast<BRDFMaterial>();
    }

    public IEnumerable<ReflectiveMaterial> GetReflectiveMaterials(List<string> materialNames)
    {
      return Materials.Values.Where(m => materialNames.Contains(m.Name) && m is ReflectiveMaterial).Select(m => m).Cast<ReflectiveMaterial>();
    }

    public IEnumerable<IColorMaterial> GetAmbientMaterials(List<string> materialNames)
    {
      return Materials.Values.Where(m => materialNames.Contains(m.Name) && m is BRDFMaterial && ((BRDFMaterial)m).UseForAmbient).Select(m => m).Cast<IColorMaterial>();
    }

    public static Resources LoadResources(string fileName)
    {
      var jsonString = File.ReadAllText(fileName);
      var resources = ObjectHook(JToken.Parse(jsonString));
      return (Resources)resources;
    }

    private static object ObjectHook(JToken token)
    {
      switch (token.Type)
      {
        case JTokenType.Object:

          var children = token.Children<JProperty>();
          var dic = children
                      .ToDictionary(prop => prop.Name,
                                    prop => ObjectHook(prop.Value));
          if (dic.ContainsKey("__type__"))
          {
            if (dic["__type__"].ToString() == "resources")
            {
              var materials = new List<IMaterial>();
              var materialsObj = (List<object>)dic["materials"];
              foreach (var mat in materialsObj)
              {
                materials.Add((IMaterial)mat);
              }
              return new Resources(materials);
            }
            else if (dic["__type__"].ToString() == "brdf_material")
            {
              var name = dic["name"].ToString();
              var color = ((List < object >) dic["color"]).Select(Convert.ToSingle).ToList();
              var brdfParams = ((Dictionary<string, object>)dic["brdfParams"]).Select(kv => new KeyValuePair<string, float> (kv.Key, Convert.ToSingle(kv.Value))).ToDictionary(kv => kv.Key, kv => kv.Value);
              var brdf = BRDF.GetBRDFDictionary()[dic["brdf"].ToString()];
              var useForAmbient = false;
              if (dic.ContainsKey("use_for_ambient"))
              {
                useForAmbient = (bool)dic["use_for_ambient"];
              }
              return new BRDFMaterial {Name = name, Color=color, BRDF = brdf, BRDFParams = brdfParams, UseForAmbient = useForAmbient };
            }
            else if (dic["__type__"].ToString() == "reflective_material")
            {
              var name = dic["name"].ToString();
              var reflectivity = Convert.ToSingle(dic["reflectivity"]);
              return new ReflectiveMaterial { Name = name, Reflectivity= reflectivity };
            }
          }
          return dic;

        case JTokenType.Array:
          return token.Select(ObjectHook).ToList();

        default:
          return ((JValue)token).Value;
      }
    }
  }

  interface IMaterial
  {
    string Name { get; set; }
  }

  interface IColorMaterial : IMaterial
  {
    List<float> Color { get; set; }
  }

  class BRDFMaterial : IColorMaterial
  {
    public string Name { get; set; }
    public List<float> Color { get; set; }
    public Dictionary<string, float> BRDFParams { get; set; }
    public Func<List<float>, List<float>, List<float>, Dictionary<string, float>, float> BRDF { get; set; }
    public bool UseForAmbient { get; set; }
    
  }


  class ReflectiveMaterial : IMaterial
  {
    public string Name { get; set; }
    public float Reflectivity { get; set; }
    
  }
}