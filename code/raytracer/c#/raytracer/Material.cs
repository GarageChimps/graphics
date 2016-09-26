using System;
using System.Collections.Generic;

namespace raytracer
{

  interface IMaterial
  {
    string Name { get; set; }
  }

  interface IColorMaterial : IMaterial
  {
    Vector Color { get; set; }
    Vector GetColor(Vector coords);
  }

  class BRDFMaterial : IColorMaterial
  {
    public string Name { get; set; }
    public Vector Color { get; set; }
    public virtual Vector GetColor(Vector coords)
    {
      return Color;
    }

    public Dictionary<string, float> BRDFParams { get; set; }
    public Func<Vector, Vector, Vector, Dictionary<string, float>, float> BRDF { get; set; }
    public bool UseForAmbient { get; set; }

  }

  class BRDFColorTextureMaterial : BRDFMaterial
  {
    public ITexture Texture { get; set; }
    public string TextureName { get; set; }
    public TextureFiltering TextureFilterMode { get; set; }

    public override Vector GetColor(Vector coords)
    {
      return Texture.GetColor(coords, TextureFilterMode);
    }
  }


  class ReflectiveMaterial : IMaterial
  {
    public string Name { get; set; }
    public Vector Color { get; set; }
    public float GlossyFactor { get; set; }

  }

  class DielectricMaterial : IMaterial
  {
    public string Name { get; set; }
    public Vector Color { get; set; }
    public Vector Attenuation { get; set; }
    public float RefractionIndex { get; set; }

  }
}