using System.Collections.Generic;
using System.Linq;

namespace raytracer
{
  class Scene
  {
    public Dictionary<string, object> Parameters { get; set; }
    public Camera Camera { get; set; }
    public List<IObject> Objects { get; set; }
    public List<ILight> Lights { get; set; }

    Scene(Dictionary<string, object> parameters, Camera camera, List<IObject> objects, List<ILight> lights)
    {
      Parameters = parameters;
      Camera = camera;
      Objects = objects;
      Lights = lights;
    }

    public List<float> GetBackgroundColor()
    {
      if (Parameters.ContainsKey("background_color"))
      {
        return (List<float>)Parameters["background_color"];
      }
      return new List<float> { 0, 0, 0 };
    }

    public object GetParam(string paramName)
    {
      if (Parameters.ContainsKey(paramName))
      {
        return Parameters[paramName];
      }
      return null;
    }

    public IEnumerable<AmbientLight> GetAmbientLights()
    {
      return Lights.Where(l => l is AmbientLight).Cast<AmbientLight>();
    }

    public IEnumerable<IShadingLight> GetShadingLights()
    {
      return Lights.Where(l => l is IShadingLight).Cast<IShadingLight>();
    }

    public static Scene LoadScene(string fileName)
    {
      return null;
    }
  }
}
