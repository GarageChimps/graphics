using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

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
      if (!Parameters.ContainsKey("enable_shadows"))
        Parameters.Add("enable_shadows", true);
    }

    public Vector GetBackgroundColor()
    {
      List<float> background = GetFloatListParam("background_color");

      return new Vector(background);
    }

    public object GetParam(string paramName)
    {
      if (Parameters.ContainsKey(paramName))
      {
        return Parameters[paramName];
      }
      return null;
    }

    public List<float> GetFloatListParam(string paramName)
    {
      if (Parameters.ContainsKey("background_color"))
      {
        return ((List<object>)Parameters["background_color"]).Select(Convert.ToSingle).ToList();
      }
      return new List<float> { 0, 0, 0 };
    }


    public int GetIntParam(string paramName)
    {
      if (Parameters.ContainsKey(paramName))
      {
        return Convert.ToInt32(Parameters[paramName]);
      }
      return 0;
    }

    public bool GetBoolParam(string paramName)
    {
      if (Parameters.ContainsKey(paramName))
      {
        return Convert.ToBoolean(Parameters[paramName]);
      }
      return false;
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
      var jsonString = File.ReadAllText(fileName);
      var scene = ObjectHook(JToken.Parse(jsonString));
      return (Scene)scene;
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
            if (dic["__type__"].ToString() == "scene")
            {
              var camera = (Camera)dic["camera"];
              var objects = new List<IObject>();
              var objectsObj = (List<object>)dic["objects"];
              foreach (var obj in objectsObj)
              {
                objects.Add((IObject)obj);
              }
              var lights = new List<ILight>();
              var lightsObj = (List<object>)dic["lights"];
              foreach (var l in lightsObj)
              {
                lights.Add((ILight)l);
              }
              return new Scene((Dictionary<string, object>)dic["params"], camera, objects, lights);
            }
            else if (dic["__type__"].ToString() == "camera")
            {
              var fov = Convert.ToSingle(dic["fov"]);
              //var near = Convert.ToSingle(dic["near"]);
              var position = ((List<object>)dic["position"]).Select(Convert.ToSingle).ToList();
              var target = ((List<object>)dic["target"]).Select(Convert.ToSingle).ToList();
              var up = ((List<object>)dic["up"]).Select(Convert.ToSingle).ToList();
              return new Camera(
                fov, 
                new Vector(position), 
                new Vector(up), 
                new Vector(target));
            }
            else if (dic["__type__"].ToString() == "sphere")
            {
              var radius = Convert.ToSingle(dic["radius"]);
              var position = ((List<object>)dic["position"]).Select(Convert.ToSingle).ToList();
              var materials = ((List<object>)dic["materials"]).Select(c => c.ToString()).ToList();
              return new Sphere(
                radius, 
                new Vector(position), 
                materials);
            }
            else if (dic["__type__"].ToString() == "mesh")
            {
              var filePath = dic["file_path"].ToString();
              var materials = ((List<object>)dic["materials"]).Select(c => c.ToString()).ToList();
              var computeVertexNormals = false;
              if (dic.ContainsKey("compute_vertex_normals"))
                computeVertexNormals = Convert.ToBoolean(dic["compute_vertex_normals"]);
              var mesh = new Mesh(filePath, materials, computeVertexNormals);
              mesh.Init();
              return mesh;
            }
            else if (dic["__type__"].ToString() == "point_light")
            {
              var position = ((List<object>)dic["position"]).Select(Convert.ToSingle).ToList();
              var color = ((List<object>)dic["color"]).Select(Convert.ToSingle).ToList();
              return new PointLight(
                new Vector(position), 
                new Vector(color));
            }
            else if (dic["__type__"].ToString() == "directional_light")
            {
              var direction = ((List<object>)dic["direction"]).Select(Convert.ToSingle).ToList();
              var color = ((List<object>)dic["color"]).Select(Convert.ToSingle).ToList();
              return new DirectionalLight(
                new Vector(direction), 
                new Vector(color));
            }
            else if (dic["__type__"].ToString() == "ambient_light")
            {
              var color = ((List<object>)dic["color"]).Select(Convert.ToSingle).ToList();
              return new AmbientLight(
                new Vector(color));
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
}
