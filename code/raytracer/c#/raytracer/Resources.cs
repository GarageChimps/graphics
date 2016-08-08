using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public static object LoadResources(string fileName)
    {
      var jsonString = File.ReadAllText(fileName);
      var jsonObject = Utils.Deserialize(jsonString);
      var materials = new List<IMaterial>();
      
      return jsonObject;
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
    public string Type { get; set; }
    public string Name { get; set; }
    public List<float> Color { get; set; }
    public Dictionary<string, float> BRDFParams { get; set; }
    public Func<List<float>, List<float>, List<float>, Dictionary<string, float>, float> BRDF { get; set; }
    public bool UseForAmbient { get; set; }

    BRDFMaterial(string type, string name, List<float> color, Dictionary<string, float> brdfParams,
      Func<List<float>, List<float>, List<float>, Dictionary<string, float>, float> brdf, bool use_for_ambient)
    {
      Type = type;
      Name = name;
      Color = color;
      BRDFParams = brdfParams;
      BRDF = brdf;
      UseForAmbient = use_for_ambient;
    }
  }


  class ReflectiveMaterial : IMaterial
  {
    public string Type { get; set; }
    public string Name { get; set; }
    public float Reflectivity { get; set; }

    ReflectiveMaterial(string type, string name, float reflectivity)
    {
      Type = type;
      Name = name;
      Reflectivity = reflectivity;
    }
  }
}
